using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebMonk.Misc;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;
using WebMonk.ValueProviders;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public class ImageMvcModel : BinaryFileMvcModel
    {
        #region IRMCustomMapper implementation
        public override Task<T> MapToCustomAsync<T>(T other)
        {
            return Task.FromResult(other); //do nothing
        }
        #endregion

        #region IEditorTemplate implementation
        public override IGenerateHtml EditorTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            return DisplayTemplate(screenOrderFrom, screenOrderTo, attributes);
        }
        #endregion

        #region IDisplayTemplate implementation
        public override IGenerateHtml DisplayTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            if (IsEmpty) return new Tags();

            var (id, parentId, controller, pn) = GetIdParentIdControllerPropertyName();
            var qs = new QueryStringDict { { "parentId", parentId.ToString() }, {"pn", pn } };
            var imageLink = Render.Helper.UrlForMvcAction(controller, "BinaryFile", id.ToString(), qs);
            return new Div
            {
                new Img(HtmlAttributesAsDict).AddOrUpdateAttr(new { src= imageLink }).AddOrUpdateAttr(attributes)
            };
        }
        #endregion

        #region ISelfModelBinder implementation
        public override Task<object?> BindMeAsync(Type rootType, List<IValueProvider> valueProviders)
        {
            //Do nothing
            return Task.FromResult((object?)this);
        }
        #endregion

    }
}