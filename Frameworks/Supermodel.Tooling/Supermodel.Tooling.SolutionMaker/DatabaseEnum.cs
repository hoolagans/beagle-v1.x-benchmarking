using System.ComponentModel;

namespace Supermodel.Tooling.SolutionMaker;

public enum DatabaseEnum 
{ 
    Sqlite, 
    [Description("Sql Server")] SqlServer 
}