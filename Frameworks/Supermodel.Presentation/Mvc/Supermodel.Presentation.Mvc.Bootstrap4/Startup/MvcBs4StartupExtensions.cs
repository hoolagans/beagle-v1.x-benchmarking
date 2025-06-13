using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Supermodel.DataAnnotations;
using Supermodel.Persistence.DataContext;
using Supermodel.Presentation.Mvc.Auth;
using Supermodel.Presentation.Mvc.Bootstrap4.Models;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;
using Supermodel.Presentation.Mvc.Context;
using Supermodel.Presentation.Mvc.Startup;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Startup;

public static class MvcBs4StartupExtensions
{
    #region Constructors
    static MvcBs4StartupExtensions()
    {
        var assembly = typeof(MvcBs4StartupExtensions).Assembly;
        var names = EmbeddedResource.GetAllResourceNamesInFolder(assembly, "StaticWebFiles").Where(x => !x.EndsWith("Message.html")).ToArray();
        foreach (var name in names)
        {
            Files[name] = EmbeddedResource.ReadBinaryFileWithFileName(assembly, $"StaticWebFiles.{name}");
        }

        MessageHtml = EmbeddedResource.ReadTextFileWithFileName(typeof(MvcBs4StartupExtensions).Assembly, "StaticWebFiles.Message.html");
    }
    #endregion

    #region Methods
    public static IMvcBuilder AddSupermodelMvcBs4Services(this IServiceCollection services, string? accessDeniedPath = null, string loginPath="/Auth/Login")
    {
        accessDeniedPath ??= Bs4.Message.RegisterMultiReadMessageAndGetUrl("403 Forbidden: Access Denied");
        return services.AddSupermodelMvcServices(accessDeniedPath, loginPath);
    }
    public static IMvcBuilder AddSupermodelMvcBs4Services<TApiAuthenticationHandler>(this IServiceCollection services, string? accessDeniedPath = null, string loginPath="/Auth/Login")
        where TApiAuthenticationHandler : SupermodelApiAuthenticationHandlerBase
    {
        accessDeniedPath ??= Bs4.Message.RegisterMultiReadMessageAndGetUrl("403 Forbidden: Access Denied");
        return services.AddSupermodelMvcServices<TApiAuthenticationHandler>(accessDeniedPath, loginPath);
    }
    public static IApplicationBuilder UseSupermodelMvcBs4Middleware<TDataContext>(this IApplicationBuilder builder, IWebHostEnvironment env, string? errorPage = null)
        where TDataContext : class, IDataContext, new()
    {
        SetUpMiddlewareWithoutEndpoints<TDataContext>(builder, env, errorPage);
        builder.UseEndpoints(SetUpEndpoints);  
        return builder;
    }

    public static void SetUpMiddlewareWithoutEndpoints<TDataContext>(IApplicationBuilder builder, IWebHostEnvironment env, string? errorPage)
        where TDataContext : class, IDataContext, new()
    {
        errorPage ??= Bs4.Message.RegisterMultiReadMessageAndGetUrl("500 Internal Server Error");

        if (env.IsDevelopment())
        {
            builder.UseDeveloperExceptionPage();
        }
        else
        {
            builder.UseExceptionHandler(errorPage);
            builder.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        }

        builder.UseStaticFiles();

        builder.UseSupermodelMvcMiddleware<TDataContext>();

        //builder.UseHttpsRedirection();
        builder.UseCookiePolicy();
        builder.UseSession();
    }
    public static void SetUpEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapControllerRoute("DefaultMvc", "{controller=Home}/{action=Index}/{id:long?}");
        endpoints.MapRazorPages();

        foreach (var fileName in Files.Keys)
        {
            if (fileName.Contains("open_iconic.font.css."))
            {
                endpoints.MapGet($"static_web_files/open_iconic/font/css/{fileName.Replace("open_iconic.font.css.", "")}", async context => { await context.Response.Body.WriteAsync(Files[fileName], 0, Files[fileName].Length); });
            }
            else if (fileName.Contains("open_iconic.font.fonts."))
            {
                endpoints.MapGet($"static_web_files/open_iconic/font/fonts/{fileName.Replace("open_iconic.font.fonts.", "")}", async context => { await context.Response.Body.WriteAsync(Files[fileName], 0, Files[fileName].Length); });
            }
            else
            {
                endpoints.MapGet($"static_web_files/{fileName}", async context => { await context.Response.Body.WriteAsync(Files[fileName], 0, Files[fileName].Length); });
            }
        }
            
        endpoints.MapGet("static_web_files/Message.html", async context =>
        {
            var urlHelper = RequestHttpContext.GetUrlHelperWithEmptyViewContext();

            var messageHtml = MessageHtml
                .Replace("[%HeaderSnippet%]", SuperBs4HeadTagHelper.GetSupermodelSnippetStatic(urlHelper))
                .Replace("[%BodySnippet%]", SuperBs4BodyTagHelper.GetSupermodelSnippetStatic(urlHelper))
                .Replace("[%MessageSnippet%]", Bs4.Message.ReadMessageText(context.Request.Query["msgGuid"]!))
                .Replace("[%HomePageSnippet%]", urlHelper.Content("~/"));

            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(messageHtml);
        });
    }
    #endregion

    #region Properties
    public static Dictionary<string, byte[]> Files{ get; } =new();
    public static string MessageHtml { get; }
    #endregion
}