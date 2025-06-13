using BeagleLib.Agent;

namespace BeagleLib.Engine;

public interface IMLEngineNotificationsHandler : IDisposable
{
    void HandleMostAccurateEverOrganismUpdated(Organism newMostAccurateEverOrganismDeepCopy, uint currentGeneration);
    void HandleGenerationCounter(int generations);
    public bool Quit { get; set; }
}