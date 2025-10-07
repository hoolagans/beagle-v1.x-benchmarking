using BeagleLib.Engine;
using BeagleLib.Engine.FitFunc;
using Run.MLSetups;

namespace Run;

public class Program
{
    static void Main()
    {
        #region Custom settings per machine (not used)
        //LG Gram
        if (Environment.MachineName == "LG-GRAM")
        {
            //This is supported only on Windows and Linux, so we are ok since we do not run on Mac
            #pragma warning disable CA1416
            //Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1111_1111_1111); //MAX Performance
            #pragma warning restore CA1416
        }

        //Alienware
        if (Environment.MachineName == "NB20527-M29598") 
        {
            //This is supported only on Windows and Linux, so we are ok since we do not run on Mac
            #pragma warning disable CA1416
            //Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1111_1111_1111); //MAX Performance
            #pragma warning restore CA1416
        }

        //Workstation
        if (Environment.MachineName == "NB12466-M29598")
        {
            //This is supported only on Windows and Linux, so we are ok since we do not run on Mac
            #pragma warning disable CA1416
            //Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1111_1111_1111_1111); //MAX Performance
            #pragma warning restore CA1416
        }

        //Server
        if (Environment.MachineName == "KILIMANJARO")
        {
            //This is supported only on Windows and Linux, so we are ok since we do not run on Mac
            #pragma warning disable CA1416
            //Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1111_1111_1111_1111); //MAX Performance
            #pragma warning restore CA1416
        }

        //H100 Server
        if (Environment.MachineName == "blackcomb")
        {
            //This is supported only on Windows and Linux, so we are ok since we do not run on Mac
            #pragma warning disable CA1416
            //Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(0b0000_0000_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111); //MAX Performance
            #pragma warning restore CA1416
        }
        #endregion

        #region Benchmarking
        //using var mlBenchmarkEngine = new MLEngine<Benchmark, StdFitFunc>(forceCPUAccelerator: false);
        //mlBenchmarkEngine.Train(benchmarkRun: true);
        //return;
        #endregion

        //new CsvGen<RydbergFormula>().CreateAndSaveCsvFile(5000); return;

        //using var mlEngine = new MLEngine<DemoForMSU, StdFitFunc>(forceCPUAccelerator: false);
        using var mlEngine = new MLEngine<DemoForMSU2, StdCubeFitFunc>(forceCPUAccelerator: false);
        //using var mlEngine = new MLEngine<AreaOfCircle, StdFitFunc>(forceCPUAccelerator: false);
        //using var mlEngine = new MLEngine<AvgOf2, StdFitFunc>(forceCPUAccelerator: false);
        //using var mlEngine = new MLEngine<QuadraticEqNormalized, StdFitFunc>(forceCPUAccelerator: false);
        //using var mlEngine = new MLEngine<QuadraticEq, StdFitFunc>(forceCPUAccelerator: false);
        //using var mlEngine = new MLEngine<DepressedCubicEq, StdFitFunc>(forceCPUAccelerator: false);
        //using var mlEngine = new MLEngine<SinApproximation, StdCubeFitFunc>(forceCPUAccelerator: false);
        //using var mlEngine = new MLEngine<DemoForJdAndTheBoys, StdFitFunc>(forceCPUAccelerator: false);
        //using var mlEngine = new MLEngine<RydbergFormula, StdFitFunc>(forceCPUAccelerator: false);
        //using var mlEngine = new MLEngine<ThrustData, StdFitFunc>(forceCPUAccelerator: false);

        mlEngine.Train();
    }
}