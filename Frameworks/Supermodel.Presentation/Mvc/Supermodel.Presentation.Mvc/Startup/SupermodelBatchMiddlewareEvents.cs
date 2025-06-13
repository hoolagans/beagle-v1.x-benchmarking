using System.Threading;
using System.Threading.Tasks;
using HttpBatchHandler.Events;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.UnitOfWork;

namespace Supermodel.Presentation.Mvc.Startup;

public class SupermodelBatchMiddlewareEvents<TDataContext> : BatchMiddlewareEvents where TDataContext : class, IDataContext, new()
{
    public override Task BatchStartAsync(BatchStartContext context, CancellationToken cancellationToken = default)
    {
        Rollback = false;
        UnitOfWork = new UnitOfWork<TDataContext>();
        Transaction = UnitOfWorkContext<TDataContext>.CurrentDataContext.BeginTransaction();
        return base.BatchStartAsync(context, cancellationToken);
    }

    public override Task BatchRequestExecutingAsync(BatchRequestExecutingContext context, CancellationToken cancellationToken = default)
    {
        var scopeFactory = context.Request.HttpContext.RequestServices.GetService<IServiceScopeFactory>();
        context.Request.HttpContext.Features.Set<IServiceProvidersFeature>(new RequestServicesFeature(context.Request.HttpContext, scopeFactory));
        return base.BatchRequestExecutingAsync(context, cancellationToken);
    }

    public override async Task BatchRequestExecutedAsync(BatchRequestExecutedContext context, CancellationToken cancellationToken = default)
    {
        var requestServicesFeature = context.Request.HttpContext.Features.Get<IServiceProvidersFeature>() as RequestServicesFeature;
        if (requestServicesFeature != null) await requestServicesFeature.DisposeAsync();

        if (context.Response.StatusCode < 200 || context.Response.StatusCode > 299)
        {
            UnitOfWork!.Context.CommitOnDispose = false;
            Rollback = true;
        }

        await base.BatchRequestExecutedAsync(context, cancellationToken);
    }

    public override async Task BatchEndAsync(BatchEndContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (context.Exception != null) UnitOfWork!.Context.CommitOnDispose = false;
                
            if (UnitOfWork!.Context.CommitOnDispose && !Rollback)
            {
                await UnitOfWork!.Context.FinalSaveChangesAsync(cancellationToken);
                Transaction!.Commit();
            }
            else
            {
                Transaction!.Rollback();
            }                
        }
        finally
        {
            Transaction!.Dispose();
            await UnitOfWork!.DisposeAsync();
        }
        await base.BatchEndAsync(context, cancellationToken);
    }

    protected bool Rollback { get; set; }
    protected IDataContextTransaction? Transaction { get; set; }
    protected UnitOfWork<TDataContext>? UnitOfWork { get; set; }
}