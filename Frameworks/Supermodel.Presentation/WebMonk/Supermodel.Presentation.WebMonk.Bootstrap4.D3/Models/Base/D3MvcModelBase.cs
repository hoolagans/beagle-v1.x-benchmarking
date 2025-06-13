using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.DataAnnotations.Misc;
using WebMonk.Context;
using WebMonk.Extensions;
using WebMonk.ModeBinding;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Templates;
using WebMonk.ValueProviders;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.D3.Models.Base;

public abstract class D3MvcModelBase : IEditorTemplate, IDisplayTemplate, IHiddenTemplate, ISelfModelBinder
{
    #region Methods
    public abstract bool ContainsData();
    public abstract IGenerateHtml GenerateD3Script(string containerId);
    public virtual IGenerateHtml GenerateContainerTag(string containerId, AttributesDict attributes)
    {
        return new Span(attributes);
    }
    #endregion

    #region ISupermodelEditorTemplate implementation
    public virtual IGenerateHtml EditorTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
    {
        return DisplayTemplate(screenOrderFrom, screenOrderTo, attributes);
    }
    #endregion

    #region ISupermodelDisplayTemplate implementation
    public virtual IGenerateHtml DisplayTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
    {
        if (!ContainsData()) return new Tags();

        //get the id for our property 
        var svgId = HttpContext.Current.PrefixManager.CurrentPrefix.ToHtmlId();

        //make a local, case-insensitive version of the dict
        var svgTagAttributesDict = new AttributesDict(DivTagAttributesAsDict);

        //If id is not already there, we use one from our property name
        if (svgTagAttributesDict.TryGetValue("id", out var value)) svgId = value!;
        else svgTagAttributesDict["id"] = svgId;

        return new Tags 
        {
            GenerateContainerTag(svgId, svgTagAttributesDict),
            GenerateD3Script(svgId)
        };
    }
    #endregion

    #region ISupermodelHiddenTemplate implementation
    public virtual IGenerateHtml HiddenTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
    {
        throw new SupermodelException("D3MvcModel cannot be used as hidden.");
    }
    #endregion

    #region ISuperModelBinder implementation
    public Task<object?> BindMeAsync(Type rootType, List<IValueProvider> valueProviders)
    {
        return Task.FromResult((object?)this); //Do nothing
    }
    #endregion

    #region Properties
    public object DivTagAttributesAsObj { set => DivTagAttributesAsDict = AttributesDict.FromAnonymousObject(value); }
    public AttributesDict DivTagAttributesAsDict { get; set; } = new();
    #endregion
}