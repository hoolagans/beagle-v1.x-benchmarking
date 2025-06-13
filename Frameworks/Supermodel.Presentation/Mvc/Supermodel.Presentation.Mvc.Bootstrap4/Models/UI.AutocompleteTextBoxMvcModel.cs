using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Supermodel.DataAnnotations.Misc;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.Entities;
using Supermodel.Presentation.Mvc.Controllers.Api;
using Supermodel.Presentation.Mvc.Extensions;
using Supermodel.Presentation.Mvc.HtmlHelpers;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models;

public static partial class Bs4
{
    public class AutocompleteTextBoxMvcModel<TEntity, TAutocompleteControllerType, TDataContext> : TextBoxMvcModel
        where TEntity : class, IEntity, new()
        where TAutocompleteControllerType : AutocompleteApiController<TEntity, TDataContext>, new()
        where TDataContext : class, IDataContext, new()
    {
        #region Constructors
        public AutocompleteTextBoxMvcModel()
        {
            AutocompleteControllerName = typeof(TAutocompleteControllerType).Name.RemoveControllerSuffix();
        }
        #endregion

        #region Overrides
        public override IHtmlContent EditorTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            var tmpHtmlAttributesAsDict = new AttributesDict(HtmlAttributesAsDict);
                
            HtmlAttributesAsDict["data-autocomplete-source"] = html.Super().GenerateUrl("", AutocompleteControllerName);
            var result = base.EditorTemplate(html, screenOrderFrom, screenOrderTo, markerAttribute);
                
            HtmlAttributesAsDict = tmpHtmlAttributesAsDict;
            return result;
        }
        public override TextBoxMvcModel InitFor<T>()
        {
            return base.InitFor<string>(); //autocomplete is always a string text box
        }
        public override Task MapFromCustomAsync<T>(T other)
        {
            if (other == null)
            {
                Value = "";
                return Task.CompletedTask;
            }
                
            var controller = new TAutocompleteControllerType();
            Value = controller.GetStringFromEntity((TEntity)(object)other);

            return Task.CompletedTask;
        }
        // ReSharper disable once RedundantAssignment
        public override async Task<T> MapToCustomAsync<T>(T other)
        {
            if (string.IsNullOrEmpty(Value)) throw new ValidationResultException($"Cannot parse blank string into {typeof(T).GetTypeFriendlyDescription()}");

            var controller = new TAutocompleteControllerType();
            var entity = await controller.GetEntityFromNameAsync(Value);
            if (entity == null) throw new ValidationResultException($"'{Value}' does not exist");
            other = (T)(object)entity;

            return other;
        }
        #endregion

        #region Properies
        public string AutocompleteControllerName { get; } 
        #endregion
    }
}