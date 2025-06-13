using Supermodel.Encryptor;
using System.Threading.Tasks;
using Supermodel.Mobile.Runtime.Common.Models;
using Supermodel.Mobile.Runtime.Common.DataContext.WebApi;

namespace Supermodel.Mobile.Runtime.Common.DataContext.Core;

public interface IWebApiAuthorizationContext
{
    AuthHeader AuthHeader { get; set; }
    Task<LoginResult> ValidateLoginAsync<TModel>() where TModel : class, IModel;
}