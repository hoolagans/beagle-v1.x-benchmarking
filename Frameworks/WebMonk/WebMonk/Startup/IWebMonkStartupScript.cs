using System.Threading.Tasks;

namespace WebMonk.Startup;

public interface IWebMonkStartupScript
{
    int Priority { get; }
    Task ExecuteStartupTaskAsync();
}