using System;
using System.Diagnostics;

namespace Supersonic.Timer;

public class ConsoleTimer : IDisposable
{
    public ConsoleTimer(string name)
    {
        Name = name;
        Console.WriteLine($"Starting {name} at {DateTime.Now}...");
        Watch = Stopwatch.StartNew();
    }

    public void Dispose()
    {
        Watch.Stop();
        Console.Write($"{Name} completed in ");

        var defaultColor = Console.ForegroundColor;
        if (Environment.OSVersion.Platform == PlatformID.Unix) defaultColor = ConsoleColor.DarkBlue;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{Watch.Elapsed:c}.");

        Console.ForegroundColor = defaultColor;
    }

    private string Name { get; set; }
    public Stopwatch Watch { get; set; }
}