using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Attributes;
using Supermodel.ReflectionMapper;
using WebMonk.Context;
using WebMonk.Exceptions;
using WebMonk.Extensions;
using WebMonk.ModeBinding;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Templates;
using WebMonk.Rendering.Views;
using WebMonk.ValueProviders;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models.Base;

public abstract class UIComponentBase : IRMapperCustom, IEditorTemplate, IDisplayTemplate, IHiddenTemplate, ISelfModelBinder, IUIComponentWithValue, IComparable 
{
    #region IRMapperCustom implemtation
    public abstract Task MapFromCustomAsync<T>(T other);
    public abstract Task<T> MapToCustomAsync<T>(T other);
    #endregion
        
    #region IEditorTemplate implemtation
    public abstract IGenerateHtml EditorTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null);
    #endregion

    #region IDisplayTemplate implementation
    public virtual IGenerateHtml DisplayTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
    {
        return new Txt(ComponentValue);
    }
    #endregion

    #region IHiddenTemplate implemtation
    public virtual IGenerateHtml HiddenTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
    {
        var inputTag = Render.HiddenForModel(ComponentValue, attributes);
        //inputTag.AddOrUpdateAttr(attributes);

        return inputTag;
    }
    #endregion

    #region ISelfModelBinder implementation
    public virtual Task<object?> BindMeAsync(Type rootType, List<IValueProvider> valueProviders)
    {
        var prefix = HttpContext.Current.PrefixManager.CurrentPrefix;

        if (string.IsNullOrEmpty(prefix)) throw new WebMonkException("prefix is not set");
        var name = prefix.ToHtmlName();

        ComponentValue = valueProviders.GetValueOrDefault<string>(name).UpdateInternal(ComponentValue);
                
        return Task.FromResult((object?)this);
    }
    #endregion

    #region IUIComponentWithValue implemtation 
    public abstract string ComponentValue { get; set; }
    #endregion

    #region IComparable implementation
    public virtual int CompareTo(object? obj)
    {
        if (obj == null) return 1;
                
        var valueToCompareWith = ((IUIComponentWithValue)obj).ComponentValue;
        return string.Compare(ComponentValue, valueToCompareWith, StringComparison.InvariantCulture);
    }
    #endregion

    #region ToString override
    public override string ToString()
    {
        return ComponentValue;
    }
    #endregion
}