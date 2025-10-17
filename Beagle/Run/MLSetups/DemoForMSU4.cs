using BeagleLib.Engine;
using BeagleLib.Util;
using BeagleLib.VM;

namespace Run.MLSetups;

public class DemoForMSU4 : MLSetup
{
    #region Overrides
    public override (float[], float) GetNextInputsAndCorrectOutput(float[] inputs)
    {
        var m = 1 + Rnd.Random.NextSingle() * 4;
	var r = 1 + Rnd.Random.NextSingle() * 4;
	var v = 1 + Rnd.Random.NextSingle() * 4;
	var t = 1 + Rnd.Random.NextSingle() * 4;

        inputs[0] = m;
	inputs[1] = r;
	inputs[2] = v;
	inputs[3] = t;

        //var result = MathF.Pow(MathF.E, -m*m/2)/MathF.Sin(2 * MathF.PI);
	var result = m*r*v*MathF.Sin(t);
        return (inputs, result);
    }
    public override string[] GetInputLabels()
    {
        return ["M","R","V","T"];
    }

    public override int TargetColonySize(int generation)
    {
        if (generation % 1000 < 50) return 50_000_000;
        return 1_000_000;
    }

    public override long TotalBirthsToResetColonyIfNoProgress => 1_500_000_000;

    public override uint ExperimentsPerGeneration => 1024;

    public override double SolutionFoundASRThreshold => 1.0;
    public override bool KeepOptimizingAfterSolutionFound => true;

    //public override OpEnum[] GetAllowedOperations() => base.GetAllowedOperations().Where(x => x != OpEnum.Sin &&
    //                                                                                          x != OpEnum.Add &&
    //                                                                                          x != OpEnum.Sub &&
    //                                                                                          x != OpEnum.Cbrt &&
    //                                                                                          x != OpEnum.Cube &&
    //                                                                                          x != OpEnum.Ln).ToArray();

    #endregion
}
