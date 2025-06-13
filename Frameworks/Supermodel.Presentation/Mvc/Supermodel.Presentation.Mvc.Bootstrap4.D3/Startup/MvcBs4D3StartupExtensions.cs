using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Supermodel.DataAnnotations;
using Supermodel.Persistence.DataContext;
using Supermodel.Presentation.Mvc.Auth;
using Supermodel.Presentation.Mvc.Bootstrap4.Startup;

namespace Supermodel.Presentation.Mvc.Bootstrap4.D3.Startup;

public static class MvcBs4D3StartupExtensions
{
    #region Constructors
    static MvcBs4D3StartupExtensions()
    {
        var assembly = typeof(MvcBs4D3StartupExtensions).Assembly;
        var names = EmbeddedResource.GetAllResourceNamesInFolder(assembly, "StaticWebFiles").Where(x => x.EndsWith(".css") || x.EndsWith(".js")).ToArray();
        foreach (var name in names) Files[name] = EmbeddedResource.ReadBinaryFileWithFileName(assembly, $"StaticWebFiles.{name}");
    }
    #endregion

    #region Methods
    public static IMvcBuilder AddSupermodelMvcBs4D3Services(this IServiceCollection services, string? accessDeniedPath = null, string loginPath="/Auth/Login")
    {
        return services.AddSupermodelMvcBs4Services(accessDeniedPath, loginPath);
    }
    public static IMvcBuilder AddSupermodelMvcBs4D3Services<TApiAuthenticationHandler>(this IServiceCollection services, string? accessDeniedPath = null, string loginPath="/Auth/Login")
        where TApiAuthenticationHandler : SupermodelApiAuthenticationHandlerBase
    {
        return services.AddSupermodelMvcBs4Services<TApiAuthenticationHandler>(accessDeniedPath, loginPath);
    }
    public static IApplicationBuilder UseSupermodelMvcBs4D3Middleware<TDataContext>(this IApplicationBuilder builder, IWebHostEnvironment env, string? errorPage = null)
        where TDataContext : class, IDataContext, new()
    {
        SetUpMiddlewareWithoutEndpoints<TDataContext>(builder, env, errorPage);
        builder.UseEndpoints(SetUpEndpoints);  
        return builder;
    }

    public static void SetUpMiddlewareWithoutEndpoints<TDataContext>(IApplicationBuilder builder, IWebHostEnvironment env, string? errorPage)
        where TDataContext : class, IDataContext, new()
    {
        MvcBs4StartupExtensions.SetUpMiddlewareWithoutEndpoints<TDataContext>(builder, env, errorPage);
    }
    public static void SetUpEndpoints(IEndpointRouteBuilder endpoints)
    {
        MvcBs4StartupExtensions.SetUpEndpoints(endpoints);

        foreach (var fileName in Files.Keys)
        {
            endpoints.MapGet($"static_web_files/{fileName}", async context => { await context.Response.Body.WriteAsync(Files[fileName], 0, Files[fileName].Length); });
        }

    }
    #endregion

    #region Properties
    public static Dictionary<string, byte[]> Files { get; } = new();
    #endregion
}