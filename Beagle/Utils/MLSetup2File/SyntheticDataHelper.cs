using System.Text;
using BeagleLib.Engine;

namespace MLSetup2File;

public static class SyntheticDataHelper
{
    public static void Create<TMLSetup>(int numOfRecords, FileOutputType fileOutputType = FileOutputType.Csv)
        where TMLSetup : MLSetup, new()
    {
        MLSetup mlSetup = new TMLSetup();
        var output = "";
        var extension = "";

        switch (fileOutputType)
        {
            case FileOutputType.Csv:
            {
                output = GenerateCsv(mlSetup, numOfRecords);
                extension = ".csv";
                break;
            }
            case FileOutputType.Json:
            {
                output = GenerateJson(mlSetup, numOfRecords);
                extension = ".json";
                break;
            }
        }

        const string dirName = "SyntheticData";
        var fileName = Path.Combine(dirName, mlSetup.GetType().Name + extension);

        if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);
        if (File.Exists(fileName)) File.Delete(fileName);
        var file = File.Open(fileName, FileMode.Create);

        using StreamWriter writer = new StreamWriter(file);
        writer.Write(output);
    }

    static string GenerateCsv(MLSetup mlSetup, int numOfValues)
    {
        var buffer = new StringBuilder();

        for (int i = 0; i < mlSetup.GetInputLabels().Length; i++)
        {
            buffer.Append($"{mlSetup.GetInputLabels()[i]},");
        }

        buffer.Append("output\n");

        for (int i = 0; i < numOfValues; i++)
        {
            float[] inputsToFill = new float[mlSetup.GetInputLabels().Length];
            float output;
            var output2 = mlSetup.GetNextInputsAndCorrectOutput(inputsToFill);
            output = output2.Item2;

            foreach (float input in inputsToFill)
            {
                buffer.Append($"{input},");
            }
            buffer.Append($"{output}\n");
        }

        return buffer.ToString();
    }
    static string GenerateJson(MLSetup mlSetup, int numOfValues)
    {
        var buffer = new StringBuilder();

        buffer.Append("[\n");
        for (int i = 0; i < numOfValues; i++)
        {
            buffer.Append("{");
            float[] inputsToFill = new float[mlSetup.GetInputLabels().Length];
            var output2 = mlSetup.GetNextInputsAndCorrectOutput(inputsToFill);
            var output = output2.Item2;
            for (int i2 = 0; i2 < mlSetup.GetInputLabels().Length; i2++)
            {

                buffer.Append($"\"{mlSetup.GetInputLabels()[i2]}\": {inputsToFill[i2]},");
            }
            buffer.Append($"\"output\": {output}");
            buffer.Append("}\n");

            if (i + 1 < numOfValues)
            {
                buffer.Append(",");
            }
        }

        buffer.Append("]\n");

        return buffer.ToString();
    }
}