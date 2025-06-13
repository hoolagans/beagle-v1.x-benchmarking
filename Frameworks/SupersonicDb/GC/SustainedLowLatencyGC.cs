using System;
using System.Runtime;

namespace Supersonic.GC;

public class SustainedLowLatencyGC : IDisposable
{
    public SustainedLowLatencyGC()
    {
        //save current latency mode
        GCLatencyMode = GCSettings.LatencyMode; 
        GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
    }

    public void Dispose()
    {
        //restore previous latency mode
        GCSettings.LatencyMode = GCLatencyMode; 
    }

    private GCLatencyMode GCLatencyMode { get; }
}