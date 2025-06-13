using System.Collections.Generic;
using System.Linq;
using Supermodel.DataAnnotations.Misc;
using Supermodel.Presentation.WebMonk.Extensions;
using WebMonk.Context;
using WebMonk.Extensions;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models.Base;

public abstract class SingleSelectMvcModel : UIComponentBase
{
    #region Option nested class
    public class Option(string value, string label, bool isDisabled = false)
    {
        public string Value { get; } = value;
        public string Label { get; } = label;
        public bool IsDisabled { get; } = isDisabled;
    }
    public List<Option> Options { get; protected set; } = new();
    #endregion

    #region Static Dropdown and Radio helpers
    public virtual IGenerateHtml CommonDropdownEditorTemplate(SingleSelectMvcModel singleSelect, AttributesDict? attributesDict)
    {
        attributesDict ??= new AttributesDict();
        attributesDict.AddOrAppendCssClass("form-control");

        var selectListItemList = new List<SelectListItem> { SelectListItem.Empty };
        foreach (var option in singleSelect.Options)
        {
            var isSelectedOption = singleSelect.SelectedValue != null && string.CompareOrdinal(singleSelect.SelectedValue, option.Value) == 0;
            if (isSelectedOption || !option.IsDisabled)
            {
                var item = new SelectListItem(option.Value, !option.IsDisabled ? option.Label : option.Label + DisabledSuffix);
                selectListItemList.Add(item);
            }
        }
        var result = Render.DropdownListForModel(singleSelect.SelectedValue, selectListItemList, attributesDict);
        return result;
    }
    public virtual IGenerateHtml CommonRadioEditorTemplate(SingleSelectMvcModel singleSelect, AttributesDict? divAttributesDict, AttributesDict? inputAttributesDict, AttributesDict? labelAttributesDict)
    {
        var prefix = HttpContext.Current.PrefixManager.CurrentPrefix;
        var id = prefix.ToHtmlId();
        var name = prefix.ToHtmlName();

        var result = new HtmlStack();

        //Set up class attributes
        divAttributesDict ??= new AttributesDict();
        divAttributesDict.AddOrAppendCssClass("form-check");

        inputAttributesDict ??= new AttributesDict();
        inputAttributesDict.AddOrAppendCssClass("form-check-input");

        labelAttributesDict ??= new AttributesDict();
        labelAttributesDict.AddOrAppendCssClass("form-check-label");

        foreach (var option in singleSelect.Options)
        {
            result.AppendAndPush(new Div(divAttributesDict));
            var isSelectedOption = singleSelect.SelectedValue != null && string.CompareOrdinal(singleSelect.SelectedValue, option.Value) == 0;
            if (isSelectedOption || !option.IsDisabled)
            {
                result.Append(Render.RadioButtonForModel(singleSelect.SelectedValue, option.Value, inputAttributesDict));
                result.Append(new Label(new { @for = name }) { new Txt(!option.IsDisabled ? option.Label : option.Label + DisabledSuffix) }).AddOrUpdateAttr(labelAttributesDict);
            }
            result.Pop<Div>();
        }
        result.Append(new Input(new { id, name, type="hidden", value="" }));

        return result;
    }
    #endregion

    #region IUIComponentWithValue implementation
    public override string ComponentValue 
    {
        get => SelectedValue ?? "";
        set => SelectedValue = value;
    }
    #endregion

    #region IDisplayTemplate implementation
    public override IGenerateHtml DisplayTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
    {
        var displayStr = SelectedLabel ?? "";

        var isDisabled = Options.FirstOrDefault(x => x.Value == SelectedValue)?.IsDisabled ?? false;
        if (isDisabled) displayStr += DisabledSuffix;

        return new Txt(displayStr);
    }
    #endregion

    #region ToString override
    public override string ToString()
    {
        return SelectedValue ?? "";
    }
    #endregion

    #region Properties
    public string? SelectedValue { get; set; }
    public string? SelectedLabel
    {
        get
        {
            var selectedOption = Options.FirstOrDefault(x => x.Value == SelectedValue);
            return selectedOption?.Label;
        }
    }
    public bool IsEmpty => string.IsNullOrEmpty(SelectedValue);
    public string DisabledSuffix { get; set; } = " [DISABLED]";
    #endregion
}