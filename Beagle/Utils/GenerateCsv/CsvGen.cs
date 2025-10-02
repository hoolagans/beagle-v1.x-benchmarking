using System.Text;
using BeagleLib.Engine;

namespace GenerateCsv;

public class CsvGen<TMLSetup> where TMLSetup : MLSetup, new()
{
    #region Methods
    public string CreateAndSaveCsvFile(int rowCount)
    {
        var mlSetup = new TMLSetup();
        var csv = CreateCsvFile(rowCount);
        File.WriteAllText($"{mlSetup.Name}-{rowCount}.csv", csv);
        return csv;
    }
    public string CreateCsvFile(int rowCount)
    {
        var csv = new StringBuilder();
        var mlSetup = new TMLSetup();
        
        //First row
        var inputLabels = mlSetup.GetInputLabels();
        for (var i = 0; i < inputLabels.Length; i++)
        {
            if (i == 0) csv.Append($"{inputLabels[i]}");
            else csv.Append($", {inputLabels[i]}");
        }
        csv.AppendLine(", result");

        //Subsequent rows
        var inputsToFill = new float[inputLabels.Length];
        for (var row = 0; row < rowCount; row++)
        {
            var (rowInputs, rowOutput) = mlSetup.GetNextInputsAndCorrectOutput(inputsToFill);
            for (var i = 0; i < rowInputs.Length; i++)
            {
                if (i == 0) csv.Append($"{rowInputs[i]}");
                else csv.Append($", {rowInputs[i]}");
            }
            csv.AppendLine($", {rowOutput}");
        }
        return csv.ToString();
    }
    #endregion
}
