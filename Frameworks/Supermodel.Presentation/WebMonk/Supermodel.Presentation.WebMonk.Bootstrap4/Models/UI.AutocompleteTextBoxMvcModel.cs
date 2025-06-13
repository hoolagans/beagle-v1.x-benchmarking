using System.Threading.Tasks;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.DataAnnotations.Misc;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.Entities;
using Supermodel.Presentation.WebMonk.Controllers.Api;
using WebMonk.Extensions;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public class AutocompleteTextBoxMvcModel <TEntity, TAutocompleteControllerType, TDataContext> : TextBoxMvcModel
        where TEntity : class, IEntity, new()
        where TAutocompleteControllerType : AutocompleteApiController<TEntity, TDataContext>, new()
        where TDataContext : class, IDataContext, new()
    {
        #region Constructors
        public AutocompleteTextBoxMvcModel()
        {
            AutocompleteControllerName = typeof(TAutocompleteControllerType).GetApiControllerName();
        }
        #endregion

        #region Overrides
        public override IGenerateHtml EditorTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            if (AutocompleteControllerName == null) throw new SupermodelException("AutocompleteControllerType == null");

            var tmpHtmlAttributesAsDict = new AttributesDict(HtmlAttributesAsDict);
                
            HtmlAttributesAsDict["data-autocomplete-source"] = Render.Helper.UrlForApiAction(AutocompleteControllerName, "");
            var result = base.EditorTemplate(screenOrderFrom, screenOrderTo, attributes);
                
            HtmlAttributesAsDict = tmpHtmlAttributesAsDict;
            return result;
        }
        public override TextBoxMvcModel InitFor<T>()
        {
            return base.InitFor<string>(); //autocomplete is always a string text box
        }
        #nullable disable
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
            if (string.IsNullOrEmpty(Value)) return (T)(object)null;

            var controller = new TAutocompleteControllerType();
            var entity = await controller.GetEntityFromNameAsync(Value);
            if (entity == null) throw new ValidationResultException($"'{Value}' does not exist");
            other = (T)(object)entity;
                
            return other;
        }
        #nullable enable
        #endregion

        #region Properies
        public string AutocompleteControllerName { get; } 
        #endregion
    }
}