using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Presentation.WebMonk.Models.Api;
using Supermodel.ReflectionMapper;
using WebMonk.Context;
using WebMonk.Results;

namespace Supermodel.Presentation.WebMonk.Controllers.Api;

public abstract class CommandApiController<TInput, TOutput> : ApiControllerBase
    where TInput : class, new()
    where TOutput : class, new()
{
    #region Action Methods
    public virtual async Task<ActionResult> PostAsync(TInput input)
    {
        try
        {
            //Validate input
            if (input is IAsyncValidatableObject validatableInput)
            {
                //Validation: we only run ValidateAsync() here because attribute-based validation is already picked up by the framework
                var vrl = await validatableInput.ValidateAsync(new ValidationContext(validatableInput)).ConfigureAwait(false);
                HttpContext.Current.ValidationResultList.AddValidationResultList(vrl);
            }
            if (!HttpContext.Current.ValidationResultList.IsValid) throw new ModelStateInvalidException(input);
                
            var output = await ExecuteAsync(input).ConfigureAwait(false);

            return new JsonApiResult(output);
        }
        catch (ModelStateInvalidException)
        {
            return new JsonApiResult(await new ValidationErrorsApiModel().MapFromAsync(HttpContext.Current.ValidationResultList).ConfigureAwait(false), HttpStatusCode.ExpectationFailed);
        }
        catch (Exception ex)
        {
            return new JsonApiResult(ex.Message, HttpStatusCode.InternalServerError);
        }
    }
    #endregion

    #region Abstracts
    protected abstract Task<TOutput> ExecuteAsync(TInput input);
    #endregion
}