using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WebMonk.Startup;

namespace Supermodel.Presentation.WebMonk.Startup;

public class JsonNetConfigStartupScript : WebMonkStartupScript
{
    public override Task ExecuteStartupTaskAsync()
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Converters = { new StringEnumConverter() },
            MissingMemberHandling = MissingMemberHandling.Error,
        };            
        return Task.CompletedTask;
    }
}