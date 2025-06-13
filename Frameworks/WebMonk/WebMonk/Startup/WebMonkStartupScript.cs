using System.Threading.Tasks;

namespace WebMonk.Startup;

public abstract class WebMonkStartupScript : IWebMonkStartupScript
{
    public abstract Task ExecuteStartupTaskAsync();
    public virtual int Priority => 100;
}