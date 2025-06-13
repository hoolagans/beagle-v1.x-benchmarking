using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Supermodel.DataAnnotations;

public static class EmbeddedResource
{
    public static string[] GetAllResourceNamesInFolder(Assembly assembly, string folderName)
    {
        var fullFolderName = GetFullResourceName(assembly, folderName);
        return assembly.GetManifestResourceNames().Where(r => r.StartsWith(fullFolderName)).Select(r => r.Substring(fullFolderName.Length + 1)).ToArray();
    }

    public static byte[] ReadBinaryFileWithFileName(Assembly assembly, string fileName)
    {
        var fullFileName = GetFullResourceName(assembly, fileName);
        return ReadBinaryFile(assembly, fullFileName);
    }
    private static byte[] ReadBinaryFile(Assembly assembly, string resourceName)
    {
        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null) throw new ArgumentException(resourceName + " is not found.");
            return ReadBytesToEnd(stream);
        }
    }

    public static string ReadTextFileWithFileName(Assembly assembly, string fileName)
    {
        var fullFileName = GetFullResourceName(assembly, fileName);
        return ReadTextFile(assembly, fullFileName);
    }
    private static string ReadTextFile(Assembly assembly, string resourceName)
    {
        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null) throw new ArgumentException(resourceName + " is not found.");
            using (var reader = new StreamReader(stream)) { return reader.ReadToEnd(); }
        }
    }

    public static string GetFullResourceName(Assembly assembly, string name)
    {
        return $"{assembly.GetName().Name}.{name}";
    }
        
    private static byte[] ReadBytesToEnd(Stream input)
    {
        var buffer = new byte[16*1024];
        using (var ms = new MemoryStream())
        {
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0) ms.Write(buffer, 0, read);
            return ms.ToArray();
        }
    } 
}