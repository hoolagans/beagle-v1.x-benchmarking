using Xamarin.Forms;    
using Supermodel.Mobile.Runtime.Common.XForms.UIComponents.CustomControls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.Generic;
using Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base;
using Supermodel.Mobile.Runtime.Common.XForms.Pages.CRUDDetail;
using System;
using Supermodel.Mobile.Runtime.Common.Services;

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents;

public class DropdownXFModel : SingleCellWritableUIComponentWithoutBackingXFModel
{
    #region Option nested class
    public class Option
    {
        public Option(string value, string label, bool isDisabled = false)
        {
            Value = value;
            Label = label;
            IsDisabled = isDisabled;
        }
        public string Value { get; }
        public string Label { get; }
        public bool IsDisabled { get; }
    }
    public ObservableCollection<Option> Options { get; } = new ObservableCollection<Option>();
    public List<Option> DisplayedOptions { get; protected set; } = new List<Option>();
    #endregion
			        
    #region Constructors
    public DropdownXFModel()
    {
        Picker = new ExtPicker
        {
            HorizontalOptions = LayoutOptions.FillAndExpand, 
            VerticalOptions = LayoutOptions.FillAndExpand, 
            Border = false, 
            TextAlignment = TextAlignment.End,
            FontSize = XFormsSettings.LabelFontSize,
            TextColor = XFormsSettings.ValueTextColor
        };
        Picker.PropertyChanged += (_, _) =>
        {
            if (_currentValue != Picker.SelectedIndex)
            {
                _currentValue = Picker.SelectedIndex;
                OnChanged?.Invoke((IBasicCRUDDetailPage)ParentPage);
            }
        };

        StackLayoutView.Children.Add(Picker);
        Tapped += (_, _) => Picker.Focus();
        Options.CollectionChanged += OptionsChangedHandler;
        SelectedValue = "";

        Picker.SelectedIndexChanged += (_, _) => { UpdateTextColor(); };
    }
    #endregion

    #region Event Handlers
    protected virtual void OptionsChangedHandler(object sender, NotifyCollectionChangedEventArgs args)
    {
        ResetList(SelectedValue, true);
    }
    protected virtual void ResetList(string selectedValue, bool setValue)
    {
        DisplayedOptions = new List<Option> { new Option("", BlankOptionLabel) };
        foreach (var option in Options)
        {
            if (option.IsDisabled)
            {
                if (option.Value == selectedValue) DisplayedOptions.Add(option);
            }
            else
            {
                DisplayedOptions.Add(option);
            }
        }
			            
        Picker.Items.Clear();
        foreach (var option in DisplayedOptions)
        {
            if (option.IsDisabled) Picker.Items.Add(option.Label + " [DISABLED]");
            else Picker.Items.Add(option.Label);
        }
        if (setValue) SelectedValue = selectedValue;
    }

    public Action<IBasicCRUDDetailPage> OnChanged { get; set; }
    #endregion

    #region Properties
    public string BlankOptionLabel { get; set; } = " ";
    public string SelectedValue
    {
        get
        {
            if (SelectedIndex == null) return "";
            var selectedOption = DisplayedOptions[SelectedIndex.Value];
            return selectedOption.Value;
        } 
        set
        {
            if (value == null)
            {
                SelectedIndex = null;
            }
            else
            {
                var originalItemDisabled = false;
                if (SelectedIndex != null) originalItemDisabled = DisplayedOptions[SelectedIndex.Value].IsDisabled;
			                    
                var selectedOption = DisplayedOptions.FirstOrDefault(x => x.Value == value);
                if (selectedOption == null)
                {
                    SelectedIndex = null;
                }
                else
                {
                    //SelectedIndex = Options.Where(x => !x.IsDisabled || x.Value == value).ToList().IndexOf(selectedOption);
                    if (selectedOption.IsDisabled || originalItemDisabled) ResetList(selectedOption.Value, false);
                    SelectedIndex = DisplayedOptions.IndexOf(selectedOption);
                }
            }
        }
    }

    public string SelectedLabel
    {
        get
        {
            if (SelectedIndex == null) return "";
            var selectedOption = Options[SelectedIndex.Value];
            return selectedOption.Label;
        }
    }
    public bool IsEmpty => string.IsNullOrEmpty(SelectedValue);

    protected int? SelectedIndex
    {
        get => Picker.SelectedIndex == -1 ? null : Picker.SelectedIndex;
        set
        {
            _currentValue = value;
            if (value == null) Picker.SelectedIndex = -1;
            else Picker.SelectedIndex = value.Value;

            UpdateTextColor();
        }
    }
    private int? _currentValue;

    public void UpdateTextColor()
    {
        if (SelectedIndex == 0) Picker.TextColor = Color.Gray;
        //else Picker.TextColor = Pick.ForPlatform(Color.Black, Color.White, Color.Black);
        else Picker.TextColor = Pick.ForPlatform(Color.Black, Color.White);
    }

    public ExtPicker Picker { get; }
			
    public override object WrappedValue => SelectedValue;

    public override TextAlignment TextAlignmentIfApplies
    {
        get => Picker.TextAlignment;
        set => Picker.TextAlignment = value;
    }

    public bool Active
    {
        get => _active;
        set
        {
            if (_active == value) return;
            _active = IsEnabled = value;
            DisplayNameLabel.TextColor = value ? XFormsSettings.LabelTextColor : XFormsSettings.DisabledTextColor;
            Picker.TextColor = value ? XFormsSettings.ValueTextColor : XFormsSettings.DisabledTextColor;
            RequiredFieldIndicator.TextColor = value ? XFormsSettings.RequiredAsteriskColor : XFormsSettings.DisabledTextColor;
        }
    }
    private bool _active = true;
    #endregion
}