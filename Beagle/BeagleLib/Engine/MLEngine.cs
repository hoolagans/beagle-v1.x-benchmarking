﻿using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using BeagleLib.Agent;
using BeagleLib.Engine.FitFunc;
using BeagleLib.MathStackLib;
using BeagleLib.Util;
using BeagleLib.VM;
using ILGPU;
using ILGPU.Runtime;
using Newtonsoft.Json;
using WebMonk;

namespace BeagleLib.Engine;

public abstract class MLEngineCore : IDisposable
{
    public abstract void Train(bool benchmarkRun = false);
    public abstract void Dispose();
}

public class MLEngine<TMLSetup, TFitFunc> : MLEngineCore
    where TMLSetup : MLSetup, new()
    where TFitFunc : struct, IFitFunc
{
    #region Constructors & Dispose
    public MLEngine(bool forceCPUAccelerator = false, bool useSingleAccelerator = false, IMLEngineNotificationsHandler? mlEngineNotificationsHandler = null)
    {
        //This is to not show ^[ on Linux when escape is pressed early
        //if (Console.KeyAvailable) Console.ReadKey(true);

        checked
        {
            #region IMLEngineNotificationsHandler setup
            _mlEngineNotificationsHandler = mlEngineNotificationsHandler;
            #endregion

            #region Save ML Setup
            // ReSharper disable once ObjectCreationAsStatement
            new TMLSetup(); //this will automatically save into MLSetup.Current
            #endregion

            #region Set up Console 
            Console.ResetColor();
            Console.CursorVisible = false;
            Console.Clear();
            Console.Title = $"Beagle 1.6: {MLSetup.Current.Name}-{typeof(TFitFunc).Name}";
            #endregion

            #region Set up Json settings
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new CommandConverter());
                return settings;
            };
            #endregion

            #region Create Input and Output arrays
            _inputLabels = MLSetup.Current.GetInputLabels();
            _inputsArray = new float[MLSetup.Current.ExperimentsPerGeneration][];
            Parallel.For(0, _inputsArray.Length, i =>
            {
                _inputsArray[i] = new float[_inputLabels.Length];
            });
            _allInputs = new float[_inputLabels.Length * MLSetup.Current.ExperimentsPerGeneration];
            _correctOutputs = new float[MLSetup.Current.ExperimentsPerGeneration];
            #endregion

            #region Create AppOutput directory & Set up output file
            var fullDirPath = Path.Combine(Directory.GetCurrentDirectory(), "AppOutput");
            if (!Directory.Exists(fullDirPath)) Directory.CreateDirectory(fullDirPath);
            Directory.SetCurrentDirectory(fullDirPath);

            var now = DateTime.Now;
            Output.FileName = $"{MLSetup.Current.Name}-{typeof(TFitFunc).Name}_{now.Year}-{now.Month:D2}-{now.Day:D2}-{now.Hour:D2}-{now.Minute:D2}-{now.Second:D2}.txt";
            #endregion

            #region Set up read-only properties
            _allowedOperations = MLSetup.Current.GetAllowedOperations();
            _allowedAdjunctOperationsCount = MLSetup.Current.GetAllowedAdjunctOperationsCount();

            _generationWatch = new Stopwatch();
            _generationAcceleratorWatch = new Stopwatch();
            _totalTimeWatch = new Stopwatch();
            #endregion

            #region Construct GPU-related stuff
            //Lib Device is only available on CUDA devices
            if (Environment.GetEnvironmentVariable("CUDA_PATH") != null && !forceCPUAccelerator)
            {
                //https://github.com/m4rs-mt/ILGPU/pull/707
                _context = Context.Create(builder => builder.Default().LibDevice().EnableAlgorithms()); 
            }
            else
            {
                _context = Context.Create(builder => builder.Default().EnableAlgorithms());
            }

            //Get CUDA devices. If CUDA device does not exist, OpenCL devices, otherwise CPU device
            var firstDevice =
                _context.Devices.FirstOrDefault(x => x.AcceleratorType == AcceleratorType.Cuda) ??
                _context.Devices.FirstOrDefault(x => x.AcceleratorType == AcceleratorType.OpenCL) ??
                _context.Devices.Single(x => x.AcceleratorType == AcceleratorType.CPU);
            if (forceCPUAccelerator) firstDevice = _context.Devices.Single(x => x.AcceleratorType == AcceleratorType.CPU);
            var devices = _context.Devices.Where(x => x.AcceleratorType == firstDevice.AcceleratorType).ToArray();

            var acceleratorCount = useSingleAccelerator ? 1 : devices.Length; //this is for benchmarking One vs Multiple GPUs
            if (firstDevice.AcceleratorType == AcceleratorType.CPU) Output.WriteLine($"Running {MLSetup.Current.Name}-{typeof(TFitFunc).Name} exclusively on a CPU using a GPU emulator");
            else Output.WriteLine($"{Environment.MachineName}: Running {MLSetup.Current.Name}-{typeof(TFitFunc).Name} on {Environment.ProcessorCount} CPU(s) & {acceleratorCount} GPU(s) @ {MLSetup.Current.SolutionFoundASRThreshold:0.00##} target ASR");

            _accelerators = new AcceleratorInfo<TFitFunc>[acceleratorCount];
            for (var i = 0; i < _accelerators.Length; i++)
            {
                _accelerators[i] = new AcceleratorInfo<TFitFunc>();

                if (i == 0)
                {
                    if (devices[0].AcceleratorType == AcceleratorType.CPU) _accelerators[0].Accelerator = _context.GetPreferredDevice(true).CreateAccelerator(_context);
                    else _accelerators[i].Accelerator = devices[i].CreateAccelerator(_context);
                }
                else
                {
                    _accelerators[i].Accelerator = devices[i].CreateAccelerator(_context);
                }

                _accelerators[i].GroupSize = (uint)Math.Min(_accelerators[i].Accelerator.MaxNumThreadsPerGroup, MLSetup.Current.ExperimentsPerGeneration);

                //for CPU we cap memory at 1 Gb
                long memorySize;
                if (devices[i].AcceleratorType == AcceleratorType.CPU) memorySize = Math.Min(_accelerators[i].Accelerator.MemorySize, 1024L * 1024L * 1024L);
                else memorySize = _accelerators[i].Accelerator.MemorySize;

                _accelerators[i].MaxCommandBufferSize = Math.Min(memorySize / _accelerators[i].GroupSize * 75, 0X7FEFFFFF); //first we divide, then we multiply to reduce the change of overflow, we cap by .net max array size (0X7FEFFFFF)
                _accelerators[i].AllCommands = new Command[_accelerators[i].MaxCommandBufferSize];
                _accelerators[i].ScriptStarts = new int[(int)Math.Ceiling((double)MLSetup.Current.OrganismsArraySize / _accelerators.Length)];

                _accelerators[i].AllInputs = _accelerators[i].Accelerator.Allocate1D<float>(_allInputs.Length);
                _accelerators[i].CorrectOutputs = _accelerators[i].Accelerator.Allocate1D<float>(MLSetup.Current.ExperimentsPerGeneration);

                //_accelerators[i].Kernel = _accelerators[i].Accelerator.LoadStreamKernel<byte, uint, ArrayView<int>, ArrayView<Command>, uint, ArrayView<float>, uint, ArrayView<float>, ArrayView<int>, TFitFunc>(MainKernel.Kernel);
                _accelerators[i].Kernel = _accelerators[i].Accelerator.LoadKernel<byte, uint, ArrayView<int>, ArrayView<Command>, uint, ArrayView<float>, uint, ArrayView<float>, ArrayView<int>, TFitFunc>(MainKernel.Kernel);
            }
            #endregion

            #region Create Initial Colony
            //create all needed arrays
            _organisms = new Organism[MLSetup.Current.OrganismsArraySize];
            _newbornOrganisms = new Organism[MLSetup.Current.OrganismsArraySize];
            _scores = new int[MLSetup.Current.OrganismsArraySize];
            _taxedScorePercentiles = new int[100];

            using (new ConsoleTimer($"create initial colony of {MLSetup.Current.TargetColonySize(0):N0} organisms", true, ConsoleColor.Blue))
            {
                //Create new organisms in multi-threaded fashion
                _organismsCount = MLSetup.Current.TargetColonySize(0);
                Parallel.For(0, _organismsCount, i =>
                {
                    var newOrganism = Organism.CreateByRandomLoadOrConstCommandThenMutate((byte)_inputLabels.Length, _allowedOperations, _allowedAdjunctOperationsCount);
                    _organisms[i] = newOrganism;
                });
            }
            Output.WriteLine();
            #endregion

            #region Save and set GC latency mode
            _gcLatencyMode = GCSettings.LatencyMode;
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            #endregion
        }
    }
    public override void Dispose()
    {
        _mlEngineNotificationsHandler?.Dispose();
        GCSettings.LatencyMode = _gcLatencyMode;
        foreach (var accelerator in _accelerators) accelerator.Dispose();
        _context.Dispose();
        Console.CursorVisible = true;
    }
    #endregion

    #region Methods
    public override void Train(bool benchmarkRun = false)
    {
        try
        {
            _showProfilingInfo = benchmarkRun;

            _currentGeneration = 0;
            _generationAtLastColonyReset = 0;
            _totalTimeWatch.Start();
            _totalBirths = _organismsCount;

            _shortestEverSatisfactoryOrganism = null;
            _mostAccurateEverOrganism = null;
            _mostAccurateOrganismsSinceLastColonyReset = new Organism?[BConfig.TopMostAccurateOrganismsToKeep];
            for (var i = 0; i < _mostAccurateOrganismsSinceLastColonyReset.Length; i++) _mostAccurateOrganismsSinceLastColonyReset[i] = null;
            _mostAccurateEverOrganismTotalTime = TimeSpan.Zero;
            _totalBirthAtLastMostAccurateOrganismSinceLastColonyResetUpdate = 0;

            while (true)
            {
                _currentGeneration++;
                var done = TrainingLoopBody();

                while (false)//(Console.KeyAvailable & Environment.UserInteractive)//(Console.KeyAvailable)
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        _totalTimeWatch.Stop();

                        //We only handle the first Escape, ignore all the others
                        while (Console.KeyAvailable) Console.ReadKey(true);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Output.WriteLine("Evolution paused...\n");
                        while (true)
                        {
                            Output.WriteLine("[Q]uit");
                            Output.WriteLine("Resu[M]e");
                            Output.WriteLine("[S]ave colony");
                            Output.WriteLine("Load & [R]eplace colony");
                            Output.WriteLine("Load & [C]ombine colonies");
                            Output.WriteLine("[I]nject organism[s] as string");
                            Output.WriteLine("[D]isplay best organism as LaTeX formula");
                            Output.Write("Please choose: ");

                            var input = Output.ReadLine();
                            Output.WriteLine();

                            input = input.Trim().ToLower();
                            if (input == "q") { done = true; break; }
                            else if (input == "m") { Output.WriteLine("Resuming evolution...\n"); break; }
                            else if (input == "s") SaveColony();
                            else if (input == "r") LoadColony();
                            else if (input == "c") LoadColony(combine: true);
                            else if (input == "i") InjectOrganisms();
                            else if (input == "d") DisplayAsLatex();
                            else Output.WriteLine("Invalid input. Please choose again.\n");
                        }
                        Console.ResetColor();

                        _totalTimeWatch.Start();
                    }
                }

                if (done || benchmarkRun) break;
            }
            Console.WriteLine("Training Stopped...");
        }
        catch(Exception ex)
        {
            Notifications.SendSystemMessageSMTP(BConfig.ToEmail, $"Beagle Run Error on {Environment.MachineName}", $"Beagle 1.6: Error occurred on {Environment.MachineName} while running {MLSetup.Current.Name}\n\n{ex}");
            Output.WriteLine(ex.ToString());
            throw;
        }
        finally
        {
            Output.Dispose();
        }
    }
    protected bool TrainingLoopBody()
    {
        #region reset stuff for new generation, set up experiments (inputs/output), convert inputs to one long array
        using (new ConsoleTimer("reset stuff for new generation, set up experiments (inputs/output), convert inputs to one long array", _showProfilingInfo))
        {
            //reset stuff for new generation
            _generationWatch.Restart();

            //set up experiments (inputs and output)
            Parallel.For(0, MLSetup.Current.ExperimentsPerGeneration, i =>
            {
                (_inputsArray[i], _correctOutputs[i]) = MLSetup.Current.GetNextInputsAndCorrectOutput(_inputsArray[i]);
            });

            //convert inputs to one long array
            Parallel.For(0, MLSetup.Current.ExperimentsPerGeneration, i =>
            {
                var inputs = _inputsArray[i];
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var j = 0; j < inputs.Length; j++)
                {
                    _allInputs[i * inputs.Length + j] = inputs[j];
                }
            });
        }
        #endregion

        #region run parallel loop on multiple accelerators (if available)
        //Console.ForegroundColor = ConsoleColor.DarkYellow;
        using (new ConsoleTimer($"run parallel loop on {_accelerators.Length} accelerator(s)", _showProfilingInfo))
        {
            _generationAcceleratorWatch.Restart();
            var organismsPerAcceleratorCount = (int)Math.Ceiling((float)_organismsCount / _accelerators.Length);
            Parallel.For(0, _accelerators.Length, i =>
            {
                var startIdx = i * organismsPerAcceleratorCount;
                var endIdx = Math.Min(startIdx + organismsPerAcceleratorCount, _organismsCount);

                if (endIdx > startIdx)
                {
                    var length = endIdx - startIdx;
                    var acceleratorGrossScores = new Span<int>(_scores, startIdx, length);
                    var acceleratorNewbornOrganisms = new Span<Organism?>(_organisms, startIdx, length);
                    ScoreNewbornOrganismsOnSingleAccelerator(_accelerators[i], acceleratorNewbornOrganisms, acceleratorGrossScores, i == 0);
                }
            });
            _acceleratorGenerationTime = _generationAcceleratorWatch.Elapsed;
        }
        //Console.ResetColor();
        #endregion

        #region set scores on _organisms array, apply taxes, inject _mostAccurateOrganismSinceLastColonyReset with probability
        using (new ConsoleTimer("set scores on _organisms array, apply taxes, inject _mostAccurateOrganismSinceLastColonyReset with probability", _showProfilingInfo))
        {
            Parallel.For(0, _organismsCount, i =>
            {
                _organisms[i]!.Score = _scores[i];
                _organisms[i]!.TaxedScore = _scores[i] - MLSetup.Current.CalcScriptLengthTax(_organisms[i]!.Commands.Length);
            });
        }
        #endregion

        #region update most accurate & shortest satisfactory organisms
        var oldShortestEverSatisfactoryOrganism = _shortestEverSatisfactoryOrganism;
        using (new ConsoleTimer("update most accurate & shortest satisfactory organisms", _showProfilingInfo))
        {
            var oldMostAccurateEverOrganism = _mostAccurateEverOrganism;
            for (var i = 0; i < _organismsCount; i++)
            {
                var organism = _organisms[i]!;

                //update most accurate organisms
                UpdateMostAccurateOrganismsWith(organism);

                //update the shortest satisfactory organism
                if (_shortestEverSatisfactoryOrganism.IsSatisfactoryAccuracyAndShorterOrSameLengthButMoreAccurateThanMe(organism, MLSetup.Current.SolutionFoundASRThreshold))   
                {
                    _shortestEverSatisfactoryOrganism = organism;
                }
            }
            if (!ReferenceEquals(oldMostAccurateEverOrganism, _mostAccurateEverOrganism)) _mlEngineNotificationsHandler?.HandleMostAccurateEverOrganismUpdated(_mostAccurateEverOrganism!.CloneForExport(), (uint)_currentGeneration);
            if (_mlEngineNotificationsHandler?.Quit == true) return true;

            //we add the most accurate ones again with probability of 5%. It means every 20 generations
            if (Rnd.RandomBoolWithChance(0.05))
            {
                for (var i = 0; i < _mostAccurateOrganismsSinceLastColonyReset.Length; i++)
                {
                    if (_mostAccurateOrganismsSinceLastColonyReset[i] != null) _organisms[_organismsCount++] = _mostAccurateOrganismsSinceLastColonyReset[i];
                }
            }
        }
        #endregion

        #region set up and sort the percentiles array
        using (new ConsoleTimer("set up and sort the percentiles array", _showProfilingInfo))
        {
            Parallel.For(0, 100, i => { _taxedScorePercentiles[i] = _organisms[Rnd.Random.Next(_organismsCount)]!.TaxedScore; });
            Array.Sort(_taxedScorePercentiles);
        }
        #endregion

        #region calculate offset percentiles based on colony size vis-a-vis target colony size
        sbyte taxedScorePercentilesIdxOffset; //positive offset makes organisms die more
        using (new ConsoleTimer("calculate offset percentiles based on colony size vis-a-vis target colony size", _showProfilingInfo))
        {
            var ratio = (double)MLSetup.Current.TargetColonySize(_currentGeneration - _generationAtLastColonyReset) / _organismsCount;
            if (ratio > 1) taxedScorePercentilesIdxOffset = (sbyte)Math.Round(-Math.Min(10.0, (ratio - 1.0) * 10.0));
            else taxedScorePercentilesIdxOffset = (sbyte)Math.Round(Math.Min(10.0, (1.0 / ratio - 1.0) * 10.0));
        }
        #endregion

        #region births and deaths loop, reset colony if needed, swap _organisms and _newbornOrganisms arrays
        using (new ConsoleTimer("births and deaths loop, reset colony if needed, swap _organisms and _newbornOrganisms arrays", _showProfilingInfo))
        {
            if (MLSetup.Current.TotalBirthsToResetColonyIfNoProgress > 0 && _totalBirths - _totalBirthAtLastMostAccurateOrganismSinceLastColonyResetUpdate >= MLSetup.Current.TotalBirthsToResetColonyIfNoProgress)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Output.WriteLine("Resetting the colony fresh...");
                Console.ResetColor();

                //set up the new "begging of times"
                _generationAtLastColonyReset = _currentGeneration;

                //save _mostAccurateOrganismSinceLastColonyReset if needed
                for (var i = 0; i < _mostAccurateOrganismsSinceLastColonyReset.Length; i++)
                {
                    if (_mostAccurateOrganismsSinceLastColonyReset[i] != null &&
                        !ReferenceEquals(_mostAccurateOrganismsSinceLastColonyReset[i], _mostAccurateEverOrganism) &&
                        !ReferenceEquals(_mostAccurateOrganismsSinceLastColonyReset[i], _shortestEverSatisfactoryOrganism))
                    {
                        Organism.SaveOrganismToDeadPool(_mostAccurateOrganismsSinceLastColonyReset[i]!);
                    }
                    _mostAccurateOrganismsSinceLastColonyReset[i] = null;
                }

                //kill all current organisms
                Parallel.For(0, _organismsCount, i =>
                {
                    var organism = _organisms[i]!;

                    //death (100% chance)
                    if (!ReferenceEquals(organism, _mostAccurateEverOrganism) && 
                        !ReferenceEquals(organism, _shortestEverSatisfactoryOrganism) &&
                        !IsOrganismInMostAccurateOrganismsSinceLastColonyReset(organism))
                    {
                        Organism.SaveOrganismToDeadPool(organism);
                        _organisms[i] = null;
                    }
                });

                //Create new organisms in multithreaded fashion. we always begin with the initial number of organisms - TargetColonySize(0)
                _newbornOrganismsCount = MLSetup.Current.TargetColonySize(_currentGeneration - _generationAtLastColonyReset);
                Parallel.For(0, _newbornOrganismsCount, i =>
                {
                    _newbornOrganisms[i] = Organism.CreateByRandomLoadOrConstCommandThenMutate((byte)_inputLabels.Length, _allowedOperations, _allowedAdjunctOperationsCount);
                });
            }
            else
            {
                _newbornOrganismsCount = -1;
                Parallel.For(0, _organismsCount, i =>
                {
                    var organism = _organisms[i]!;

                    var step = 100 / _pctProbs.Length; Debug.Assert(100 % _pctProbs.Length == 0);
                    for (var pctProbsIdx = 0; pctProbsIdx < _pctProbs.Length; pctProbsIdx++)
                    {
                        var taxedScorePercentilesIdx = step * (pctProbsIdx + 1);
                        var offsetTaxedScorePercentilesIdx = FixTaxedScorePercentilesIdx(taxedScorePercentilesIdx + taxedScorePercentilesIdxOffset);
                        var taxedScorePercentile = _taxedScorePercentiles[offsetTaxedScorePercentilesIdx];

                        //if last or less than taxedScorePercentile
                        var isStrictInequality = !Rnd.RandomBoolWithChance(1.0/pctProbsIdx);
                        if (pctProbsIdx == _pctProbs.Length - 1 ||
                            isStrictInequality && organism.TaxedScore < taxedScorePercentile ||
                            !isStrictInequality && organism.TaxedScore <= taxedScorePercentile)
                        {
                            //decide how many children (if any) based on percentile probabilities
                            var pctProb = _pctProbs[pctProbsIdx];

                            do
                            {
                                if (pctProb >= 1 || Rnd.Random.NextDouble() < pctProb)
                                {
                                    var idx = Interlocked.Increment(ref _newbornOrganismsCount);
                                    _newbornOrganisms[idx] = organism.ProduceMutatedChild((byte)_inputLabels.Length, _allowedOperations, _allowedAdjunctOperationsCount);
                                }

                                pctProb--;
                            }
                            while (pctProb > 0);

                            break; //if we found the right percentile, we are done!
                        }
                    }

                    //death (100% chance)
                    if (!ReferenceEquals(organism, _mostAccurateEverOrganism) && 
                        !ReferenceEquals(organism, _shortestEverSatisfactoryOrganism) &&
                        !IsOrganismInMostAccurateOrganismsSinceLastColonyReset(organism))
                    {
                        Organism.SaveOrganismToDeadPool(organism);
                        _organisms[i] = null;
                    }
                });
                _newbornOrganismsCount++;
            }

            _totalBirths += _newbornOrganismsCount;

            //swap arrays and their counters
            (_organisms, _newbornOrganisms) = (_newbornOrganisms, _organisms);
            (_organismsCount, _newbornOrganismsCount) = (_newbornOrganismsCount, _organismsCount);
        }
        #endregion

        #region print generation results
        _generationTime = _generationWatch.Elapsed;
        Console.ForegroundColor = ConsoleColor.White;
        Output.WriteLine($"{_totalTimeWatch.Elapsed:c}: Generation {_currentGeneration:N0} took {_acceleratorGenerationTime:c}/{_generationTime:c}");
        Output.WriteLine($"Colony size: {_organismsCount:N0}: (target: {MLSetup.Current.TargetColonySize(_currentGeneration - _generationAtLastColonyReset):N0}). Total births: {_totalBirths:N0}");

        Output.Write("Most accurate ever genome: ASR = ");
        Console.ForegroundColor = ConsoleColor.Red;
        Output.Write($"{_mostAccurateEverOrganism!.ASR:0.00##}");
        Console.ForegroundColor = ConsoleColor.White;
        
        Output.Write(" (");
        Console.ForegroundColor = ConsoleColor.Red;
        Output.Write($"{_mostAccurateEverOrganism.Score:N0}");
        Console.ForegroundColor = ConsoleColor.White;
        Output.Write("/");
        Console.ForegroundColor = ConsoleColor.Red;
        Output.Write($"{BConfig.MaxScore * MLSetup.Current.ExperimentsPerGeneration:N0}");
        Console.ForegroundColor = ConsoleColor.White;

        Output.Write("), length = ");
        Console.ForegroundColor = ConsoleColor.Red;
        Output.Write($"{_mostAccurateEverOrganism.Commands.Length}");
        Console.ForegroundColor = ConsoleColor.White;
        
        Output.Write(" from ");
        Console.ForegroundColor = ConsoleColor.Red;
        Output.Write($"{_mostAccurateEverOrganismTotalTime:c}");
        Console.ForegroundColor = ConsoleColor.White;
        
        Output.Write(" at ");
        Console.ForegroundColor = ConsoleColor.Red;
        Output.Write($"{_totalBirthAtLastMostAccurateEverOrganism:N0}");
        Console.ForegroundColor = ConsoleColor.White;
        Output.WriteLine(" total births");
        Console.ForegroundColor = ConsoleColor.White;
        
        _mostAccurateEverOrganism.PrintCommandsInLine(_inputLabels);
        //Output.WriteLine(_mostAccurateEverOrganism.CommandsToJson());

        if (_mostAccurateOrganismsSinceLastColonyReset[0] != null && !ReferenceEquals(_mostAccurateOrganismsSinceLastColonyReset[0], _mostAccurateEverOrganism))
        {
            Output.Write("Last colony reset at ");
            Console.ForegroundColor = ConsoleColor.Red;
            Output.Write($"{_generationAtLastColonyReset:N0}th");
            Console.ForegroundColor = ConsoleColor.White;
            Output.WriteLine(" generation");

            Console.ForegroundColor = ConsoleColor.White;
            Output.Write("Most accurate genome since last colony reset: ASR = ");
            Console.ForegroundColor = ConsoleColor.Red;
            Output.Write($"{_mostAccurateOrganismsSinceLastColonyReset[0]!.ASR:0.00##}");
            Console.ForegroundColor = ConsoleColor.White;
            Output.Write(", length = ");
            Console.ForegroundColor = ConsoleColor.Red;
            Output.Write($"{_mostAccurateOrganismsSinceLastColonyReset[0]!.Commands.Length}");
            Console.ForegroundColor = ConsoleColor.White;
            Output.Write(" at ");
            Console.ForegroundColor = ConsoleColor.Red;
            Output.Write($"{_totalBirthAtLastMostAccurateOrganismSinceLastColonyResetUpdate:N0}");
            Console.ForegroundColor = ConsoleColor.White;
            Output.WriteLine(" total births");
            _mostAccurateOrganismsSinceLastColonyReset[0]!.PrintCommandsInLine(_inputLabels);
        }
        //Output.WriteLine($"Total births since last colony reset / reset after: {_totalBirthSinceLastColonyReset:N0}/{MLSetup.Current.TotalBirthsToResetColonyIfNoProgress:N0}");
        //Output.WriteLine(_mostAccurateOrganismSinceLastColonyReset!.CommandsToJson());
        Console.ResetColor();
        #endregion

        #region send generation results to notification handler
        _mlEngineNotificationsHandler?.HandleGenerationCounter(this._currentGeneration);
        #endregion

        #region determine if we have a new shortest satisfactory organism and print message
        if (!ReferenceEquals(oldShortestEverSatisfactoryOrganism, _shortestEverSatisfactoryOrganism))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Output.WriteLine($"Satisfactory (ASR >= {MLSetup.Current.SolutionFoundASRThreshold:0.00##}) Solution found. ASR = {_shortestEverSatisfactoryOrganism!.ASR:0.00##} ({_shortestEverSatisfactoryOrganism.Score:N0}/{BConfig.MaxScore * MLSetup.Current.ExperimentsPerGeneration:N0})");

            //Output.WriteLine(_shortestEverSatisfactoryOrganism.CommandsToJson());
            _shortestEverSatisfactoryOrganism.PrintCommands(_inputLabels);
            Console.ResetColor();

            #if DEBUG
            int score = 0;
            var fitFunc = new TFitFunc();
            Parallel.For(0, MLSetup.Current.ExperimentsPerGeneration, experiment =>
            {
                var codeMachineCPU = new CodeMachine();
                var output = codeMachineCPU.RunCommands(_inputsArray[experiment], _shortestEverSatisfactoryOrganism.Commands);
                var correctOutput = _correctOutputs[experiment];

                //fit function plus script length adjustment
                var isOutputValid = !float.IsNaN(output) && !float.IsInfinity(output) && !float.IsNegativeInfinity(output);
                var isCorrectOutputValid = !float.IsNaN(correctOutput) && !float.IsInfinity(correctOutput) && !float.IsNegativeInfinity(correctOutput);

                if (isOutputValid && isCorrectOutputValid) Interlocked.Add(ref score, fitFunc.FitFunction(output, correctOutput));
                else Interlocked.Add(ref score, fitFunc.FitFunctionIfInvalid(isOutputValid, isCorrectOutputValid)); 
            });

            if (_shortestEverSatisfactoryOrganism.Score != score)
            {
                Notifications.SendSystemMessageSMTP(BConfig.ToEmail, $"Beagle 1.6: Invalid shortest satisfactory organism score on {Environment.MachineName}!", "", System.Net.Mail.MailPriority.High);
                Debugger.Break();
            }
            #endif

            Notifications.SendSystemMessageSMTP(BConfig.ToEmail, $"Beagle Found Satisfactory Solution on {Environment.MachineName}", $"Beagle 1.6: {MLSetup.Current.Name} completed in {_totalTimeWatch.Elapsed:c} on {Environment.MachineName}\n\n{_shortestEverSatisfactoryOrganism.ToString(_inputLabels)}");
            if (!MLSetup.Current.KeepOptimizingAfterSolutionFound)
            {
                _totalTimeWatch.Stop();
                Console.ForegroundColor = ConsoleColor.Red;
                Output.Write("Would you like to continue the evolution for further optimization (y/n)? ");
                var response = Output.ReadLine();
                Console.ResetColor();
                if (response.Trim().ToLower() == "n") return true;
                _totalTimeWatch.Start();
            }
        }
        else
        {
            if (_shortestEverSatisfactoryOrganism != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Output.WriteLine($"Shortest satisfactory (ASR >= {MLSetup.Current.SolutionFoundASRThreshold:0.00##}) solution found so far. ASR = {_shortestEverSatisfactoryOrganism.ASR:0.00##} ({_shortestEverSatisfactoryOrganism.Score:N0}/{BConfig.MaxScore * MLSetup.Current.ExperimentsPerGeneration:N0}), Length = {_shortestEverSatisfactoryOrganism.Commands.Length}");
                _shortestEverSatisfactoryOrganism.PrintCommandsInLine(_inputLabels);
                Console.ResetColor();
            }
        }
        #endregion

        #region print new line if needed
        if (!_showProfilingInfo) Output.WriteLine();
        #endregion

        return false;
    }
    protected void ScoreNewbornOrganismsOnSingleAccelerator(AcceleratorInfo<TFitFunc> accelerator, Span<Organism?> organisms, Span<int> grossRewardsThisAccelerator, bool flashFileStream)
    {
        #region Declare scores logical length and figure out if we use LibDevice, create stream
        var useLibDevice = accelerator.Accelerator.AcceleratorType == AcceleratorType.Cuda ? (byte)1 : (byte)0;
        var scoresLogicalLength = 0;
        #endregion

        using (var stream = accelerator.Accelerator.CreateStream())
        {
            #region Copy stuff that does not change between batches to GPU
            accelerator.AllInputs.CopyFromCPU(stream, _allInputs);
            accelerator.CorrectOutputs.CopyFromCPU(stream, _correctOutputs);
            #endregion

            #region Run kernel in batches if needed
            var currentOrganismBatchStartIdx = 0;
            var currentOrganismBatchEndIdx = 0;
            //var first = true;

            while (currentOrganismBatchStartIdx < organisms.Length)
            {
                #region Set up batchScriptStarts and batchAllCommands
                var allCommandsIndex = 0;
                for (var i = currentOrganismBatchStartIdx; i < organisms.Length; i++)
                {
                    currentOrganismBatchEndIdx = i;
                    if (allCommandsIndex + organisms[i]!.Commands.Length > accelerator.MaxCommandBufferSize)
                    {
                        currentOrganismBatchEndIdx--; //roll back
                        break;
                    }
                    accelerator.ScriptStarts[i - currentOrganismBatchStartIdx] = allCommandsIndex;
                    for (var j = 0; j < organisms[i]!.Commands.Length; j++)
                    {
                        accelerator.AllCommands[allCommandsIndex++] = organisms[i]!.Commands[j];
                    }
                }

                var batchOrganismCount = currentOrganismBatchEndIdx + 1 - currentOrganismBatchStartIdx;
                Debug.Assert(batchOrganismCount > 0);
                var batchScriptStarts = new Span<int>(accelerator.ScriptStarts, 0, batchOrganismCount); //scriptStarts[..batchOrganismCount];
                var batchAllCommands = new Span<Command>(accelerator.AllCommands, 0, allCommandsIndex); //_allCommands[..allCommandsIndex];

                currentOrganismBatchStartIdx = currentOrganismBatchEndIdx + 1;
                #endregion

                //if (first)
                //{
                //    first = false;
                //    Output.WriteLineUnlessAtLineStart();
                //    Output.WriteLine($"-executing batch: {batchOrganismCount:N0} x {accelerator.GroupSize:N0}");
                //}
                //else
                //{
                //    Output.WriteLineUnlessAtLineStart();
                //    Output.WriteLine($"-executing additional batch: {batchOrganismCount:N0} x {accelerator.GroupSize:N0}");
                //}

                using (var acceleratorScriptStarts = accelerator.Accelerator.Allocate1D<int>(batchScriptStarts.Length))
                {
                    using (var acceleratorAllCommands = accelerator.Accelerator.Allocate1D<Command>(batchAllCommands.Length))
                    {
                        using (var acceleratorGrossRewards = accelerator.Accelerator.Allocate1D<int>(batchOrganismCount))
                        {
                            #region Copy stuff that changes to GPU
                            acceleratorScriptStarts.View.CopyFromCPU(stream, batchScriptStarts);
                            acceleratorAllCommands.View.CopyFromCPU(stream, batchAllCommands);
                            acceleratorGrossRewards.View.MemSetToZero(stream);
                            #endregion

                            #region Execute Kernel
                            var groupStart = (uint)0;
                            do
                            {
                                var currentGroupSize = Math.Min(accelerator.GroupSize, MLSetup.Current.ExperimentsPerGeneration - groupStart);
                                var launchDimension = new KernelConfig(new Index1D(batchScriptStarts.Length), new Index1D((int)currentGroupSize));

                                accelerator.Kernel(stream, launchDimension, useLibDevice, currentGroupSize, acceleratorScriptStarts.View, acceleratorAllCommands.View, groupStart, accelerator.AllInputs.View, (uint)_inputLabels.Length, accelerator.CorrectOutputs.View, acceleratorGrossRewards.View, FitFunc);
                                if (flashFileStream) Output.FlushFileStream();
                                stream.Synchronize();

                                groupStart += currentGroupSize;
                            }
                            while (groupStart < MLSetup.Current.ExperimentsPerGeneration);
                            #endregion

                            #region Get and return the results
                            var grossRewardsThisAcceleratorThisBatch = grossRewardsThisAccelerator.Slice(scoresLogicalLength, batchOrganismCount);
                            acceleratorGrossRewards.View.CopyToCPU(stream, grossRewardsThisAcceleratorThisBatch);
                            scoresLogicalLength += batchOrganismCount;
                            #endregion
                        }
                    }
                }
            }
            #endregion
        }
    }

    protected void SaveColony()
    {
        var now = DateTime.Now;
        var fileName = $"{Output.FileName.Replace(".txt", "")}-at-{now.Month:D2}-{now.Day:D2}-{now.Hour:D2}-{now.Minute:D2}-{now.Second:D2}.bin";
        using var fileStream = File.Open(fileName, FileMode.Create);
        using var binaryWriter = new BinaryWriter(fileStream);

        binaryWriter.Write('B');
        binaryWriter.Write('G');
        binaryWriter.Write('L');

        binaryWriter.Write(_organismsCount + GetMostAccurateOrganismsSinceLastColonyResetCount());

        for (var i = 0; i < _mostAccurateOrganismsSinceLastColonyReset.Length; i++)
        {
            if (_mostAccurateOrganismsSinceLastColonyReset[i] == null) break;
            binaryWriter.Write(_mostAccurateOrganismsSinceLastColonyReset[i]!.Commands);
        }

        Output.Write("Saving: ");
        var step = _organismsCount / 80;
        if (step == 0) step = 1;
        
        for (var i = 0; i < _organismsCount; i++)
        {
            binaryWriter.Write(_organisms[i]!.Commands);
            if (i % step == 0) Output.Write("*");
        }
        Output.WriteLine();

        Output.WriteLine($"Colony saved to {fileName}!");
        Output.WriteLine();
    }
    protected void LoadColony(bool combine = false)
    {
        #region Get file name from the user
        var binFiles = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.bin", SearchOption.TopDirectoryOnly).ToArray();
        Array.Sort(binFiles);

        if (binFiles.Length == 0)
        {
            Output.WriteLine("Error: No .bin files found in the directory!");
            Output.WriteLine();
            return;
        }

        for (var i = 0; i < binFiles.Length; i++)
        {
            Output.WriteLine($"[{i+1}] - {Path.GetFileName(binFiles[i])}");
        }

        int fileIdx;
        Output.Write("Please select file to load: ");
        while (true)
        {
            var inputStr = Output.ReadLine();
            try
            {
                fileIdx = int.Parse(inputStr) - 1;
                if (fileIdx >= 0 && fileIdx < binFiles.Length) break;
            }
            catch (Exception)
            {
                // ignored
            }
            Output.Write("Invalid input. Please re-enter: ");
        }
        #endregion

        #region Load the file
        try
        {
            var fileName = binFiles[fileIdx];
            using var fileStream = File.OpenRead(fileName);
            using var binaryReader = new BinaryReader(fileStream);

            if (binaryReader.ReadChar() != 'B') throw new Exception("binaryReader.ReadChar() != 'B'");
            if (binaryReader.ReadChar() != 'G') throw new Exception("binaryReader.ReadChar() != 'G'");
            if (binaryReader.ReadChar() != 'L') throw new Exception("binaryReader.ReadChar() != 'L'");

            var organismsInFileCount = binaryReader.ReadInt32();

            int requiredArraySize;
            if (combine)
            {
                requiredArraySize = _organismsCount + organismsInFileCount;
                if (requiredArraySize > _organisms.Length)
                {
                    _newbornOrganisms = null!;
                    _scores = null!;
                    GC.Collect();

                    for (var i = 0; i < _accelerators.Length; i++)
                    {
                        _accelerators[i].ScriptStarts = new int[(int)Math.Ceiling((double)requiredArraySize / _accelerators.Length)];
                    }
                    GC.Collect();

                    //we put it in a separate scope to release old organisms when done
                    var oldOrganisms = _organisms;
                    _organisms = new Organism[requiredArraySize];
                    // ReSharper disable once AccessToModifiedClosure
                    Parallel.For(0, oldOrganisms.Length, i => { _organisms[i] = oldOrganisms[i]; });

                    oldOrganisms = null;
                    GC.Collect();

                    _newbornOrganisms = new Organism[requiredArraySize];
                    _scores = new int[requiredArraySize];
                }
            }
            else //replace
            {
                requiredArraySize = organismsInFileCount;
                if (requiredArraySize > _organisms.Length)
                {
                    _organisms = null!;
                    _newbornOrganisms = null!;
                    _scores = null!;
                    GC.Collect();

                    for (var i = 0; i < _accelerators.Length; i++)
                    {
                        _accelerators[i].ScriptStarts = new int[(int)Math.Ceiling((double)requiredArraySize / _accelerators.Length)];
                    }
                    GC.Collect();

                    _organisms = new Organism[requiredArraySize];
                    _newbornOrganisms = new Organism[requiredArraySize];
                    _scores = new int[requiredArraySize];
                }
                _organismsCount = 0;
            }

            Span<Command> commandsBuffer = stackalloc Command[BConfig.MaxScriptLength];
            Output.Write("Loading: ");
            var step = organismsInFileCount / 80;
            if (step == 0) step = 1;
            var count = 0;
            while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
            {
                var orgCommands = binaryReader.ReadCommands(commandsBuffer);
                var organism = Organism.CreateByCopyingCommandsFromPartOfSpan(orgCommands, orgCommands.Length);
                _organisms[_organismsCount++] = organism;
                if (++count % step == 0) Output.Write("*");
            }
            Output.WriteLine();

            if (_organismsCount != requiredArraySize) throw new Exception("_organismsCount != requiredArraySize");
            if (count != organismsInFileCount) throw new Exception("count != organismsInFileCount");

            if (combine) Output.WriteLine($"Colony loaded & combined from {fileName}!\nNew colony size: {_organismsCount:N0}");
            else Output.WriteLine($"Colony loaded & replaced from {fileName}!\nNew colony size: {_organismsCount:N0}");

            Output.WriteLine();
        }
        catch (Exception)
        {
            Output.WriteLine("Error loading file!");
            Output.WriteLine();
            throw;
        }
        #endregion
    }
    protected void InjectOrganisms()
    {
        while (true)
        {
            InjectOrganism();
            Output.Write("Do you want to inject more organisms (y/n)? ");
            var input = Output.ReadLine();
            if (input != "y") break;
        }
        Output.WriteLine();
    }
    protected void InjectOrganism()
    {
        while (true)
        {
            Output.Write("Organism's string: ");
            try
            {
                var str = Output.ReadLine();
                var organism = Organism.CreateFromAnyString(str);
                _organisms[_organismsCount++] = organism;
                return;
            }
            catch (Exception)
            {
                Output.Write("Invalid format! Please retry: ");
            }
        }
    }
    protected void DisplayAsLatex()
    {
        var expr = MathExpr.FromCommands(_mostAccurateEverOrganism!.Commands, MLSetup.Current.GetInputLabels());
        var url = $"https://arachnoid.com/latex/?equ={expr.AsLatexString()}";
        WebServer.OpenInBrowser(url);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int FixTaxedScorePercentilesIdx(int idx)
    {
        var max = _taxedScorePercentiles.Length - 1;

        if (idx < 0) return 0;
        if (idx > max) return max;
        return idx;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int GetMostAccurateOrganismsSinceLastColonyResetCount()
    {
        for (var i = 0; i < _mostAccurateOrganismsSinceLastColonyReset.Length; i++)
        {
            if (_mostAccurateOrganismsSinceLastColonyReset[i] == null) return i;
        }
        return _mostAccurateOrganismsSinceLastColonyReset.Length;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool IsOrganismInMostAccurateOrganismsSinceLastColonyReset(Organism organism)
    {
        for (var i = 0; i < _mostAccurateOrganismsSinceLastColonyReset.Length; i++)
        {
            if (ReferenceEquals(organism, _mostAccurateOrganismsSinceLastColonyReset[i])) return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void UpdateMostAccurateOrganismsWith(Organism organism)
    {
        for (var i = _mostAccurateOrganismsSinceLastColonyReset.Length - 1; i >= 0; i--)
        {
            if (_mostAccurateOrganismsSinceLastColonyReset[i] == null) continue;

            if (!_mostAccurateOrganismsSinceLastColonyReset[i].IsMoreAccurateOrSameAccuracyButShorterThanMe(organism))
            {
                if (i != _mostAccurateOrganismsSinceLastColonyReset.Length - 1) InsertIntoMostAccurateOrganismsSinceLastColonyResetAt(i + 1, organism);
                return;
            }
        }

        //we only get here if organism is better than anything we have seen since last colony reset
        InsertIntoMostAccurateOrganismsSinceLastColonyResetAt(0, organism);
        _totalBirthAtLastMostAccurateOrganismSinceLastColonyResetUpdate = _totalBirths;

        //we only need to check for most accurate organism if most accurate organism since last colony reset was updated
        if (_mostAccurateEverOrganism.IsMoreAccurateOrSameAccuracyButShorterThanMe(_mostAccurateOrganismsSinceLastColonyReset[0]!))
        {
            _mostAccurateEverOrganism = _mostAccurateOrganismsSinceLastColonyReset[0]!;
            _mostAccurateEverOrganismTotalTime = _totalTimeWatch.Elapsed;
            _totalBirthAtLastMostAccurateEverOrganism = _totalBirths;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void InsertIntoMostAccurateOrganismsSinceLastColonyResetAt(int idx, Organism organism)
    {
        for (var i = _mostAccurateOrganismsSinceLastColonyReset.Length - 1; i > idx; i--)
        {
            _mostAccurateOrganismsSinceLastColonyReset[i] = _mostAccurateOrganismsSinceLastColonyReset[i - 1];
        }
        _mostAccurateOrganismsSinceLastColonyReset[idx] = organism;
    }
    #endregion


    #region Readonly Fields Set in Constructor
    //percentile probabilities
    private readonly double[] _pctProbs = [0.01, 0.03, 0.06, 0.15, 0.25, 0.5, 1, 1.5, 2.5, 4]; //min 80%, max 160%

    protected bool _showProfilingInfo;
    
    protected readonly string[] _inputLabels;
    protected readonly float[][] _inputsArray;
    protected readonly float[] _correctOutputs;
    protected readonly float[] _allInputs;

    protected readonly int[] _taxedScorePercentiles;

    protected readonly Context _context;
    protected readonly AcceleratorInfo<TFitFunc>[] _accelerators;

    protected readonly OpEnum[] _allowedOperations;
    protected readonly int _allowedAdjunctOperationsCount;

    protected readonly Stopwatch _totalTimeWatch;
    protected readonly Stopwatch _generationWatch;
    protected readonly Stopwatch _generationAcceleratorWatch;

    protected readonly IMLEngineNotificationsHandler? _mlEngineNotificationsHandler;
    #endregion

    #region Organism Arrays
    protected Organism?[] _organisms;
    protected int _organismsCount;

    protected Organism?[] _newbornOrganisms;
    protected int _newbornOrganismsCount;

    protected int[] _scores;
    #endregion

    #region Total & Generation Counters
    protected int _currentGeneration;
    protected long _totalBirths;
    #endregion

    #region TimeSpan Fields
    protected TimeSpan _generationTime;
    protected TimeSpan _acceleratorGenerationTime;
    #endregion

    #region Colony Reset Management
    protected int _generationAtLastColonyReset;
    protected long _totalBirthAtLastMostAccurateOrganismSinceLastColonyResetUpdate;
    protected Organism?[] _mostAccurateOrganismsSinceLastColonyReset = null!;
    #endregion

    #region Fields to Keep Track of most accurate and shortest satisfactory organisms
    protected Organism? _mostAccurateEverOrganism;
    protected TimeSpan _mostAccurateEverOrganismTotalTime;
    protected long _totalBirthAtLastMostAccurateEverOrganism;

    protected Organism? _shortestEverSatisfactoryOrganism;
    #endregion

    //#region External Thread-Safe Interface
    //public Organism? GetMostAccurateEverOrganismForExport()
    //{
    //    return _mostAccurateEverOrganismForExport;
    //}
    //protected Organism? _mostAccurateEverOrganismForExport;
    //#endregion

    #region GC-related settings
    protected readonly GCLatencyMode _gcLatencyMode;
    #endregion

    #region Cached Fit Function
    public TFitFunc FitFunc
    {
        get
        {
            _fitFunc ??= new TFitFunc();
            return _fitFunc.Value;
        }
    }
    protected TFitFunc? _fitFunc;
    #endregion
}
