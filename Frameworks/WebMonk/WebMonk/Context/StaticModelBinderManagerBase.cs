using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supermodel.DataAnnotations.Validations;
using WebMonk.Exceptions;
using WebMonk.ModeBinding;
using WebMonk.ValueProviders;

namespace WebMonk.Context;

public abstract class StaticModelBinderManagerBase
{
    #region Methods
    public abstract IStaticModelBinder GetStaticModelBinder();
        
    public virtual async Task<bool> TryUpdateMvcModelAsync<TModel>(TModel model, string additionalPrefix, List<IValueProvider>? valueProviders = null, bool ignoreRootObjectIModelBinder = false) where TModel : class
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        valueProviders ??= await HttpContext.Current.ValueProviderManager.GetValueProvidersListAsync().ConfigureAwait(false);

        if (string.IsNullOrEmpty(additionalPrefix)) return await TryUpdateMvcModelAsync(model, valueProviders, ignoreRootObjectIModelBinder).ConfigureAwait(false);

        using(HttpContext.Current.PrefixManager.NewPrefix(additionalPrefix, model))
        {
            return await TryUpdateMvcModelAsync(model, valueProviders, ignoreRootObjectIModelBinder).ConfigureAwait(false);
        }
    }
    public virtual async Task<bool> TryUpdateMvcModelAsync<TModel>(TModel model, List<IValueProvider>? valueProviders = null, bool ignoreRootObjectISelfModelBinder = false) where TModel : class
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        valueProviders ??= await HttpContext.Current.ValueProviderManager.GetValueProvidersListAsync().ConfigureAwait(false);

        var modelBinder = HttpContext.Current.StaticModelBinderManager.GetStaticModelBinder();
        var modelType = model.GetType();
        await modelBinder.BindExistingModelAsync(modelType, modelType, model, valueProviders, ignoreRootObjectISelfModelBinder).ConfigureAwait(false);

        var vrl = new ValidationResultList();
        if (!await AsyncValidator.TryValidateObjectAsync(model, new ValidationContext(model), vrl).ConfigureAwait(false))
        {
            var prefix = HttpContext.Current.PrefixManager.CurrentPrefix;
            HttpContext.Current.ValidationResultList.AddValidationResultList(vrl, prefix);
        }

        return HttpContext.Current.ValidationResultList.IsValid;
    }        
        
    public virtual async Task<bool> TryUpdateApiModelAsync(object model, List<IValueProvider>? valueProviders = null)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
            
        valueProviders ??= await HttpContext.Current.ValueProviderManager.GetValueProvidersListAsync().ConfigureAwait(false);

        var messageBodyValueProvider = valueProviders.GetFirstOrDefaultValueProviderOfType<MessageBodyValueProvider>() ?? throw new WebMonkException("Unable to find MessageBodyValueProvider");
        var bodyResult = messageBodyValueProvider.GetValueOrDefault(""); //get the entire body
        if (bodyResult.ValueMissing || bodyResult.Value == null) throw new WebMonkException("Message body is missing");
        var body = bodyResult.GetCastValue<string>();

        try
        {
            JsonConvert.PopulateObject(body, model);
            var vrl = new ValidationResultList();
            if (!await AsyncValidator.TryValidateObjectAsync(model, new ValidationContext(model), vrl).ConfigureAwait(false))
            {
                HttpContext.Current.ValidationResultList.AddValidationResultList(vrl);
            }
        }
        catch (Exception)
        {
            HttpContext.Current.ValidationResultList.AddValidationResult(new ValidationResult("Unable to deserialize the body of the message"));
        }

        return HttpContext.Current.ValidationResultList.IsValid;
    }
    #endregion
}