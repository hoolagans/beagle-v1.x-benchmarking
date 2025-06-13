using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Presentation.Mvc.Models.Api;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.Controllers.Api;

public abstract class CommandApiController<TInput, TOutput> : ApiControllerBase
    where TInput : class, new()
    where TOutput : class, new()
{
    #region Action Methods
    public virtual async Task<IActionResult> Post(TInput input)
    {
        try
        {
            //Validate input
            if (input is IAsyncValidatableObject validatableInput)
            {
                //Validation: we only run ValidateAsync() here because attribute-based validation is already picked up by the framework
                var vrl = await validatableInput.ValidateAsync(new ValidationContext(validatableInput));
                ModelState.AddValidationResultList(vrl);
            }
            if (!ModelState.IsValid) throw new ModelStateInvalidException(input);
                
            var output = await ExecuteAsync(input);

            return Ok(output);
        }
        catch (ModelStateInvalidException)
        {
            return StatusCode((int)HttpStatusCode.ExpectationFailed, await new ValidationErrorsApiModel().MapFromAsync(ModelState));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }
    #endregion

    #region Abstracts
    protected abstract Task<TOutput> ExecuteAsync(TInput input);
    #endregion
}