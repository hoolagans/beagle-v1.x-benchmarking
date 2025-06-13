using Run.MLSetups;

namespace MLSetup2File;

public class Program
{
    public static void Main()
    {
        SyntheticDataHelper.Create<RydbergFormula>(50);
        Console.WriteLine("All Done!");
    }
}