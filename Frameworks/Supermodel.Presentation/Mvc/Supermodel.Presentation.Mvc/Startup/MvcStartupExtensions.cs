using System;
using System.Threading.Tasks;
using HttpBatchHandler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Supermodel.Persistence.DataContext;
using Supermodel.Presentation.Mvc.Auth;
using Supermodel.Presentation.Mvc.Context;
using Supermodel.Presentation.Mvc.ModelBinding;

namespace Supermodel.Presentation.Mvc.Startup;

public static class MvcStartupExtensions
{
    public static IMvcBuilder AddSupermodelMvcServices(this IServiceCollection services, string accessDeniedPath, string loginPath="/Auth/Login")
    {
        //services.Configure<CookiePolicyOptions>(options =>
        //{
        //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        //    options.CheckConsentNeeded = context => true;
        //});

        services.AddSession();

        services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddControllersWithViews();
        services.AddRazorPages();

        var schemeWeb = CookieAuthenticationDefaults.AuthenticationScheme;

        services.AddAuthentication(schemeWeb)
            .AddCookie(schemeWeb, options =>
            {
                options.LoginPath = new PathString(loginPath);
                options.AccessDeniedPath = new PathString(accessDeniedPath);
                options.Events.OnRedirectToLogin = context =>
                {
                    var controllerName = context.Request.RouteValues["controller"]?.ToString();

                    if (controllerName != null && !controllerName.ToLower().EndsWith("api")) context.Response.Redirect(context.RedirectUri);
                    else context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    var controllerName = context.Request.RouteValues["controller"]!.ToString()!;

                    //This is to correct MVC craziness, they add ReturnUrl page to the 403 error page
                    context.RedirectUri = context.RedirectUri.Substring(0, context.RedirectUri.IndexOf("?", StringComparison.Ordinal)).Replace("%3F", "?");

                    if (!controllerName.ToLower().EndsWith("api")) context.Response.Redirect(context.RedirectUri);
                    else context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        
                    return Task.CompletedTask;
                };
            });


        services.AddAuthorization(options =>
        {
            var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(schemeWeb);
            defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
            options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
        });

        return services
            .AddMvc(config => config.ModelBinderProviders.Insert(0, new SuperModelBinderProvider()))
            .AddNewtonsoftJson(options => 
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
            });
    }        

    public static IMvcBuilder AddSupermodelMvcServices<TApiAuthenticationHandler>(this IServiceCollection services, string accessDeniedPath, string loginPath="/Auth/Login")
        where TApiAuthenticationHandler : SupermodelApiAuthenticationHandlerBase
    {
        //services.Configure<CookiePolicyOptions>(options =>
        //{
        //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        //    options.CheckConsentNeeded = context => true;
        //});

        services.AddSession();

        services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddControllersWithViews();
        services.AddRazorPages();

        var schemeApi = SupermodelApiAuthenticationHandlerBase.AuthenticationScheme;
        var schemeWeb = CookieAuthenticationDefaults.AuthenticationScheme;

        services.AddAuthentication(schemeWeb)
            .AddScheme<AuthenticationSchemeOptions, TApiAuthenticationHandler>(schemeApi, null)
            .AddCookie(schemeWeb, options =>
            {
                options.LoginPath = new PathString(loginPath);
                options.AccessDeniedPath = new PathString(accessDeniedPath);
                options.Events.OnRedirectToLogin = context =>
                {
                    var controllerName = context.Request.RouteValues["controller"]?.ToString();

                    if (controllerName != null && !controllerName.ToLower().EndsWith("api")) context.Response.Redirect(context.RedirectUri);
                    else context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    var controllerName = context.Request.RouteValues["controller"]!.ToString()!;

                    //This is to correct MVC craziness, they add ReturnUrl page to the 403 error page
                    context.RedirectUri = context.RedirectUri.Substring(0, context.RedirectUri.IndexOf("?", StringComparison.Ordinal)).Replace("%3F", "?");

                    if (!controllerName.ToLower().EndsWith("api")) context.Response.Redirect(context.RedirectUri);
                    else context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        
                    return Task.CompletedTask;
                };
            });


        services.AddAuthorization(options =>
        {
            var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(schemeApi, schemeWeb);
            defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
            options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
        });
            
        return services
            .AddMvc(config => config.ModelBinderProviders.Insert(0, new SuperModelBinderProvider()))
            .AddNewtonsoftJson(options => 
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
            });
    }
        
    public static IApplicationBuilder UseSupermodelMvcMiddleware<TDataContext>(this IApplicationBuilder builder) where TDataContext : class, IDataContext, new()
    {
        RequestHttpContext.Configure(builder.ApplicationServices.GetRequiredService<IHttpContextAccessor>());            
        builder.UseHttpMethodOverride(new HttpMethodOverrideOptions { FormFieldName = "X-HTTP-Method-Override" }); 
        builder.UseBatchMiddleware(options => 
        { 
            options.Match = "/$batch";
            options.Events = new SupermodelBatchMiddlewareEvents<TDataContext>();
        });
        builder.UseRouting();
        builder.UseAuthentication(); 
        return builder.UseAuthorization();
    } 
}