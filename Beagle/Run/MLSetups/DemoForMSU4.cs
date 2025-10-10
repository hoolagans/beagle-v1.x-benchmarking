using BeagleLib.Engine;
using BeagleLib.Util;

namespace Run.MLSetups;

public class DemoForMSU4 : MLSetup
{
    #region Overrides
    public override (float[], float) GetNextInputsAndCorrectOutput(float[] inputs)
    {
        var t = Rnd.Random.NextSingle() * 5;

        inputs[0] = t;

        var result = MathF.Pow(MathF.E, -t*t/2)/MathF.Sqrt(2 * MathF.PI);
        return (inputs, result);
    }
    public override string[] GetInputLabels()
    {
        return ["t"];
    }

    public override int TargetColonySize(int generation)
    {
        if (generation % 1000 < 25) return 15_000_000;
        return 1_000_000;
    }

    public override long TotalBirthsToResetColonyIfNoProgress => 750_000_000;

    public override double SolutionFoundASRThreshold => 1.0;
    public override bool KeepOptimizingAfterSolutionFound => true;
    #endregion
}