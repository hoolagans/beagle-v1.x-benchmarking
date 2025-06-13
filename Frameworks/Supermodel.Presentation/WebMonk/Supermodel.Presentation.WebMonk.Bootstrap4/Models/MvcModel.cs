using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Supermodel.DataAnnotations;
using Supermodel.DataAnnotations.Attributes;
using Supermodel.Presentation.WebMonk.Extensions;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using Supermodel.ReflectionMapper;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public class MvcModel : IMvcModel, IAsyncInit
    {
        #region EmbeddedTypes
        public enum NumberOfColumnsEnum
        {
            One = 1, 
            Two = 2, 
            Three = 3,
            Four = 4,
            Six = 6,
            Twelve = 12
        }
        #endregion
            
        #region IAsyncInit implementation
        [ScaffoldColumn(false), NotRMapped] public virtual bool AsyncInitialized { get; protected set; }
        public virtual async Task InitAsync()
        {
            //If already initialized, do nothing
            if (AsyncInitialized) return;

            //Run init async for all properties that we will show
            foreach (var propertyInfo in GetType().GetProperties())
            {
                var typedModel = this.PropertyGet(propertyInfo.Name);
                if (typedModel is IAsyncInit iAsyncInitModel && !iAsyncInitModel.AsyncInitialized) 
                {
                    await iAsyncInitModel.InitAsync().ConfigureAwait(false);
                }
            }

            //Mark as initialized
            AsyncInitialized = true;
        }
        #endregion
            
        #region Methods
        public virtual IGenerateHtml EditorTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            if (NumberOfColumns != NumberOfColumnsEnum.One) return EditorTemplateForMultipleColumnsInternal(screenOrderFrom, screenOrderTo, attributes, NumberOfColumns);

            var result = new HtmlStack();
                
            //var selectedId = ParseNullableLong(HttpContext.Current.HttpListenerContext.Request.QueryString["selectedId"]);
            //var showValidationSummary = !HttpContext.Current.ValidationResultList.IsValid && selectedId == null;
            //var validationSummaryPlaceholder = new HtmlStack();
            //result.Append(validationSummaryPlaceholder);

            foreach (var propertyInfo in GetDetailPropertyInfosInOrder(screenOrderFrom, screenOrderTo))
            {
                //skip if this property is not for edit
                if (propertyInfo.HasAttribute<SkipForEditAttribute>()) continue;

                //get html attribute
                var htmlAttrAttribute = propertyInfo.GetAttribute<HtmlAttrAttribute>();

                //if we want a hidden field
                if (propertyInfo.HasAttribute<HiddenOnlyAttribute>())
                {
                    var hiddenTags = Render.Hidden(this, propertyInfo.Name);
                    foreach (var tag in hiddenTags.GetTagsInOrder().Where(x => x.TagType == "Input" && x.Attributes.KeyExistsAndEqualsTo("type", "hidden")))
                    {
                        tag.AddOrUpdateAttr(htmlAttrAttribute?.Attributes);
                        tag.AddOrUpdateAttr(attributes);
                    }
                    result.Append(hiddenTags);
                    continue;
                }

                //Div 1
                result.AppendAndPush(new Div(new { @class="form-group row" }))
                    .AddOrUpdateAttr(htmlAttrAttribute?.Attributes)
                    .AddOrUpdateAttr(attributes);


                //Label
                var hideLabelAttribute = propertyInfo.GetAttribute<HideLabelAttribute>();
                if (hideLabelAttribute == null)
                {
                    var tooltipAttribute = propertyInfo.GetAttribute<TooltipAttribute>();
                    if (tooltipAttribute != null)
                    {
                        result.AppendAndPush(Render.Label(this, propertyInfo.Name, null, new { @class = ScaffoldingSettings.EditorLabelCssClass, data_toggle = "tooltip", title=tooltipAttribute.Tooltip }));
                        result.Append(new Span(new {@class = "text-primary" }){ new Txt(" \u24d8") });
                    }
                    else
                    {
                        result.AppendAndPush(Render.Label(this, propertyInfo.Name, null, new { @class = ScaffoldingSettings.EditorLabelCssClass }));
                    }

                    if (!propertyInfo.HasAttribute<NoRequiredLabelAttribute>() &&
                        (propertyInfo.HasAttribute<RequiredAttribute>() || propertyInfo.HasAttribute<ForceRequiredLabelAttribute>()))
                    {
                        result.Append(new Tags
                        { 
                            new Sup(null, true)
                            {
                                new Em(new { @class=$"text-danger font-weight-bold {ScaffoldingSettings.RequiredAsteriskCssClass}" }, true){ new Txt("*", true)}
                            }
                        });
                    }
                    result.Pop<Label>();
                }
                else
                {
                    if (hideLabelAttribute.KeepLabelSpace) result.Append(new Div(new { @class=ScaffoldingSettings.EditorLabelCssClass}));
                }
                    
                //Div 2
                if (hideLabelAttribute == null || hideLabelAttribute.KeepLabelSpace) result.AppendAndPush(new Div( new { @class="col-sm-10" } ));
                else result.AppendAndPush(new Div( new { @class="col-sm-12" } ));

                //Value
                if (!propertyInfo.HasAttribute<DisplayOnlyAttribute>())
                {
                    result.Append(Render.Editor(this, propertyInfo.Name, new { @class="form-control" } ));
                    var msg = Render.ValidationMessage(this, propertyInfo.Name, new { @class=ScaffoldingSettings.ValidationErrorCssClass }, true);
                    //if (!(msg is Tags tags && tags.Count == 0)) validationSummaryVisible = false;
                    result.Append(msg);
                }
                else
                {
                    result.Append(new Tags 
                    { 
                        new Span(new { @class=ScaffoldingSettings.DisplayCssClass })
                        {
                            Render.Display(this, propertyInfo.Name)
                        }
                    });
                }

                result.Pop<Div>(); //close Div 2
                result.Pop<Div>(); //close Div 1
            }

            //if (showValidationSummary)
            //{
            //    validationSummaryPlaceholder.AppendAndPush(new Div(new { @class=$"col-sm-12 {ScaffoldingSettings.ValidationSummaryCssClass}" }));
            //    validationSummaryPlaceholder.Append(Render.ValidationSummary());
            //    validationSummaryPlaceholder.Pop<Div>();
            //}
            return result; 
        }
        public virtual IGenerateHtml DisplayTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            if (NumberOfColumns != NumberOfColumnsEnum.One) return DisplayTemplateForMultipleColumnsInternal(screenOrderFrom, screenOrderTo, attributes, NumberOfColumns);
                
            var result = new HtmlStack();
            foreach (var propertyInfo in GetDetailPropertyInfosInOrder(screenOrderFrom, screenOrderTo))
            {
                //skip if this property is not for display
                if (propertyInfo.HasAttribute<SkipForDisplayAttribute>()) continue;

                //get html attribute
                var htmlAttrAttribute = propertyInfo.GetAttribute<HtmlAttrAttribute>();

                //if we want a hidden field
                if (propertyInfo.HasAttribute<HiddenOnlyAttribute>())
                {
                    var hiddenTags = Render.Hidden(this, propertyInfo.Name);
                    foreach (var tag in hiddenTags.GetTagsInOrder().Where(x => x.TagType == "Input" && x.Attributes.KeyExistsAndEqualsTo("type", "hidden")))
                    {
                        tag.AddOrUpdateAttr(htmlAttrAttribute?.Attributes);
                        tag.AddOrUpdateAttr(attributes);
                    }
                    result.Append(hiddenTags);
                    continue;
                }

                //Div 1
                result.AppendAndPush(new Div(new { @class="form-group row" }))
                    .AddOrUpdateAttr(htmlAttrAttribute?.Attributes)
                    .AddOrUpdateAttr(attributes);

                //Label
                var hideLabelAttribute = propertyInfo.GetAttribute<HideLabelAttribute>();
                if (hideLabelAttribute == null)
                {
                    var tooltipAttribute = propertyInfo.GetAttribute<TooltipAttribute>();
                    if (tooltipAttribute != null)
                    {
                        result.AppendAndPush(Render.Label(this, propertyInfo.Name, null, new { @class = ScaffoldingSettings.DisplayLabelCssClass, data_toggle = "tooltip", title = tooltipAttribute.Tooltip }));
                        result.Append(new Span(new { @class = "text-primary" }) { new Txt(" \u24d8") });
                    }
                    else
                    {
                        result.AppendAndPush(Render.Label(this, propertyInfo.Name, null, new { @class = ScaffoldingSettings.DisplayLabelCssClass }));
                    }

                    if (!propertyInfo.HasAttribute<NoRequiredLabelAttribute>())
                    {
                        if (propertyInfo.HasAttribute<RequiredAttribute>() || propertyInfo.HasAttribute<ForceRequiredLabelAttribute>()) 
                        {
                            result.Append(new Tags
                            { 
                                new Sup(null, true)
                                {
                                    new Em(new { @class=$"text-danger font-weight-bold {ScaffoldingSettings.RequiredAsteriskCssClass}" }, true){ new Txt("*", true)}
                                }
                            });
                        }
                    }
                    result.Pop<Label>();
                }
                else
                {
                    if (hideLabelAttribute.KeepLabelSpace) result.Append(new Div(new { @class=ScaffoldingSettings.EditorLabelCssClass}));
                }
                    
                //Div 2
                if (hideLabelAttribute == null || hideLabelAttribute.KeepLabelSpace) result.AppendAndPush(new Div( new { @class="col-sm-10" } ));
                else result.AppendAndPush(new Div( new { @class="col-sm-12" } ));

                //Value
                result.Append(new Tags 
                { 
                    new Span(new { @class=ScaffoldingSettings.DisplayCssClass })
                    {
                        Render.Display(this, propertyInfo.Name)
                    }
                });
                    
                result.Pop<Div>(); //close Div 2
                result.Pop<Div>(); //close Div 1
            }
            return result;   
        }
        public virtual IGenerateHtml HiddenTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            var tags = new Tags();
            foreach (var propertyInfo in GetDetailPropertyInfosInOrder(screenOrderFrom, screenOrderTo))
            {
                //skip if this property is not for display
                if (propertyInfo.HasAttribute<SkipForHiddenAttribute>()) continue;

                var hiddenTags = Render.Hidden(this, propertyInfo.Name);
                foreach (var tag in hiddenTags.GetTagsInOrder().Where(x => x.TagType == "Input" && x.Attributes.KeyExistsAndEqualsTo("type", "hidden")))
                {
                    var htmlAttrAttribute = propertyInfo.GetAttribute<HtmlAttrAttribute>();
                    tag.AddOrUpdateAttr(htmlAttrAttribute?.Attributes);
                    tag.AddOrUpdateAttr(attributes);
                }
                tags.Add(hiddenTags);
            }
            return tags; 
        }
            
        protected virtual IGenerateHtml EditorTemplateForMultipleColumnsInternal(int screenOrderFrom, int screenOrderTo, object? attributes, NumberOfColumnsEnum numberOfColumns)
        {
            var maxColumns = (int)numberOfColumns;
            var currentColumn = 1;
                
            var result = new HtmlStack();
                
            //var selectedId = ParseNullableLong(HttpContext.Current.HttpListenerContext.Request.QueryString["selectedId"]);
            //var showValidationSummary = !HttpContext.Current.ValidationResultList.IsValid && selectedId == null;
            //var validationSummaryPlaceholder = new HtmlStack();
            //result.Append(validationSummaryPlaceholder);

            foreach (var propertyInfo in GetDetailPropertyInfosInOrder(screenOrderFrom, screenOrderTo))
            {
                //skip if this property is not for edit
                if (propertyInfo.HasAttribute<SkipForEditAttribute>()) continue;
                    
                //If this is a beginning of a row
                if (currentColumn == 1) result.AppendAndPush(new Div(new { @class="form-row"}));

                //Div
                var htmlAttrAttribute = propertyInfo.GetAttribute<HtmlAttrAttribute>();
                result.AppendAndPush(new Div(new { @class=$"form-group col-md-{12/maxColumns}" } ))
                    .AddOrUpdateAttr(htmlAttrAttribute?.Attributes)
                    .AddOrUpdateAttr(attributes);

                //Label
                var hideLabelAttribute = propertyInfo.GetAttribute<HideLabelAttribute>();
                if (hideLabelAttribute == null)
                {
                    var tooltipAttribute = propertyInfo.GetAttribute<TooltipAttribute>();
                    if (tooltipAttribute != null)
                    {
                        result.AppendAndPush(Render.Label(this, propertyInfo.Name, null, new { @class = ScaffoldingSettings.EditorMultiColumnLabelCssClass, data_toggle = "tooltip", title = tooltipAttribute.Tooltip }));
                        result.Append(new Span(new { @class = "text-primary" }) { new Txt(" \u24d8") });
                    }
                    else
                    {
                        result.AppendAndPush(Render.Label(this, propertyInfo.Name, null, new { @class = ScaffoldingSettings.EditorMultiColumnLabelCssClass }));
                    }

                    if (!propertyInfo.HasAttribute<NoRequiredLabelAttribute>())
                    {
                        if (propertyInfo.HasAttribute<RequiredAttribute>() || propertyInfo.HasAttribute<ForceRequiredLabelAttribute>()) 
                        {
                            result.Append(new Tags
                            { 
                                new Sup(null, true)
                                {
                                    new Em(new { @class=$"text-danger font-weight-bold {ScaffoldingSettings.RequiredAsteriskCssClass}" }, true){ new Txt("*", true)}
                                }
                            });
                        }
                    }
                    result.Pop<Label>();
                }
                else
                {
                    if (hideLabelAttribute.KeepLabelSpace) result.Append(new Div(new { @class=ScaffoldingSettings.EditorMultiColumnLabelCssClass}));
                }                    
                    
                //Value
                if (!propertyInfo.HasAttribute<DisplayOnlyAttribute>())
                {
                    result.Append(Render.Editor(this, propertyInfo.Name));
                    var msg = Render.ValidationMessage(this, propertyInfo.Name, new { @class=ScaffoldingSettings.ValidationErrorCssClass }, true);
                    //if (!(msg is Tags tags && tags.Count == 0)) showValidationSummary = false;
                    result.Append(msg);
                }
                else
                {
                    result.Append(new Tags 
                    { 
                        new Span(new { @class=ScaffoldingSettings.MultiColumnDisplayCssClass })
                        {
                            Render.Display(this, propertyInfo.Name)
                        }
                    });
                }
                    
                result.Pop<Div>(); //close Div

                //if this is an ending of a row
                if (currentColumn == maxColumns)
                {
                    currentColumn = 1;
                    result.Pop<Div>();
                }
                else
                {
                    currentColumn++;
                }
            }
            if (currentColumn != 1) result.Pop<Div>();

            //if (showValidationSummary)
            //{
            //    validationSummaryPlaceholder.AppendAndPush(new Div(new { @class=$"col-sm-12 {ScaffoldingSettings.ValidationSummaryCssClass}" }));
            //    validationSummaryPlaceholder.Append(Render.ValidationSummary());
            //    validationSummaryPlaceholder.Pop<Div>();
            //}
            return result;                 
        }
        protected virtual IGenerateHtml DisplayTemplateForMultipleColumnsInternal(int screenOrderFrom, int screenOrderTo, object? attributes, NumberOfColumnsEnum numberOfColumns)
        {
            var maxColumns = (int)numberOfColumns;
            var currentColumn = 1;
                
            var result = new HtmlStack();
            foreach (var propertyInfo in GetDetailPropertyInfosInOrder(screenOrderFrom, screenOrderTo))
            {
                //skip if this property is not for display
                if (propertyInfo.HasAttribute<SkipForDisplayAttribute>()) continue;

                //If this is a beginning of a row
                if (currentColumn == 1) result.AppendAndPush(new Div(new { @class="form-row"}));

                //Div
                var htmlAttrAttribute = propertyInfo.GetAttribute<HtmlAttrAttribute>();
                result.AppendAndPush(new Div(new { @class=$"form-group col-md-{12/maxColumns}" } ))
                    .AddOrUpdateAttr(htmlAttrAttribute?.Attributes)
                    .AddOrUpdateAttr(attributes);

                //Label
                var hideLabelAttribute = propertyInfo.GetAttribute<HideLabelAttribute>();
                if (hideLabelAttribute == null)
                {
                    var tooltipAttribute = propertyInfo.GetAttribute<TooltipAttribute>();
                    if (tooltipAttribute != null)
                    {
                        result.AppendAndPush(Render.Label(this, propertyInfo.Name, null, new { @class = ScaffoldingSettings.DisplayMultiColumnLabelCssClass, data_toggle = "tooltip", title = tooltipAttribute.Tooltip }));
                        result.Append(new Span(new { @class = "text-primary" }) { new Txt(" \u24d8") });
                    }
                    else
                    {
                        result.AppendAndPush(Render.Label(this, propertyInfo.Name, null, new { @class = ScaffoldingSettings.DisplayMultiColumnLabelCssClass }));
                    }

                    if (!propertyInfo.HasAttribute<NoRequiredLabelAttribute>())
                    {
                        if (propertyInfo.HasAttribute<RequiredAttribute>() || propertyInfo.HasAttribute<ForceRequiredLabelAttribute>()) 
                        {
                            result.Append(new Tags
                            { 
                                new Sup(null, true)
                                {
                                    new Em(new { @class=$"text-danger font-weight-bold {ScaffoldingSettings.RequiredAsteriskCssClass}" }, true){ new Txt("*", true)}
                                }
                            });
                        }
                    }
                    result.Pop<Label>();
                }
                else
                {
                    if (hideLabelAttribute.KeepLabelSpace) result.Append(new Div(new { @class=ScaffoldingSettings.DisplayMultiColumnLabelCssClass}));
                }   

                //Value
                result.Append(new Tags 
                { 
                    new Span(new { @class=ScaffoldingSettings.MultiColumnDisplayCssClass })
                    {
                        Render.Display(this, propertyInfo.Name)
                    }
                });
                    
                result.Pop<Div>(); //close Div

                //if this is an ending of a row
                if (currentColumn == maxColumns)
                {
                    currentColumn = 1;
                    result.Pop<Div>();
                }
                else
                {
                    currentColumn++;
                }
            }
            if (currentColumn != 1) result.Pop<Div>();

            return result;                   
        }
        protected virtual IEnumerable<PropertyInfo> GetDetailPropertyInfosInOrder(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue)
        {
            return GetType().GetDetailPropertyInfosInOrder(screenOrderFrom, screenOrderTo);
        }
        #endregion

        #region Properties
        [ScaffoldColumn(false), NotRMapped] public virtual NumberOfColumnsEnum NumberOfColumns { get; set; } = NumberOfColumnsEnum.One;
        #endregion
    }
}