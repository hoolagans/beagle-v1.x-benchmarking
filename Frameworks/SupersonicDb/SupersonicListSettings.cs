namespace Supersonic;

public static class SupersonicListSettings
{
    public enum SortMethod { OrderBy, ParallelOrderBy, Sort }
    public enum ExecutionOrder { Sequential, Parallel }

    public static ExecutionOrder RebuildIndexeslExecutionOrder { get; set; } = ExecutionOrder.Parallel;
    public static SortMethod SortingMethod { get; set; } = SortMethod.ParallelOrderBy;
}