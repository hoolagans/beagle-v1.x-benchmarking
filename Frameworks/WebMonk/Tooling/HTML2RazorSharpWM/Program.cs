using System.Threading.Tasks;
using HTML2RazorSharpWM.Mvc.Layouts;
using WebMonk;

namespace HTML2RazorSharpWM;

public class Program
{
    public static async Task Main()
    {
        var port = 51413; //WebServer.FindFreeTcpPort();
        var server = new WebServer(port, $"http://localhost:{port}")
        {
            ShowErrorDetails = true,
            DefaultLayout = new MasterMvcLayout()
        };
        await server.RunAsync("/");
    }
}