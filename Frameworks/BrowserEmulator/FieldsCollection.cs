using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace BrowserEmulator;

public class FieldsCollection
{
    public FieldsCollection()
    {
        mArr.Clear();
    }

    public FieldsCollection(FieldsCollection original)
    {
        for (int i = 0; i < original.Count; i++)
        {
            HttpField field = new HttpField(original[i]);
            Add(field);
        }
    }

    public HttpField this[int key]
    {
        get { return (HttpField)mArr[key]; }
        set { mArr[key] = value; }
    }

    public HttpField this[string name]
    {
        get { return (HttpField)mArr[GetKey(name)]; }
        set { mArr[GetKey(name)] = value; }
    }

    // ReSharper disable once InconsistentNaming
    public int GetKey(string p_httpName)
    {
        int disabledRadioIdx = -1;
        for (int i = 0; i < this.Count; i++) {
            if (this[i].HttpName.ToLower() == p_httpName.ToLower()) {
                if (this[i].FieldType == HttpField.Type.Radio) {
                    if (this[i].Submit) return i;
                    else disabledRadioIdx = i;
                } else return i;
            }
        }
        if (disabledRadioIdx != -1) return disabledRadioIdx;
        else throw new FieldDoesNotExistException("FeildsCollection: Attempt to read non-existent string property parameter: " + p_httpName);
    }

    // ReSharper disable InconsistentNaming
    public int GetKey(string p_httpName, HttpField.Type p_type) 
        // ReSharper restore InconsistentNaming
    {
        return GetKey(p_httpName, p_type, true);
        /*
        int disabledRadioIdx = -1;
        for(int i = 0; i<this.count; i++)
        {
            if ((this[i].httpName.ToLower() == p_httpName.ToLower()) && (this[i].type == p_type))
            {
                if (this[i].type == HttpField.Type.Radio)
                {
                    if (this[i].submit == true) return i;
                    else disabledRadioIdx = i;
                }
                else return i;
            }
        }
        if (disabledRadioIdx != -1) return disabledRadioIdx;
        else throw new FieldDoesNotExistEx("FeildsCollection: Attempt to read non-existent string property parameter: " + p_httpName);
        */
    }

    // ReSharper disable InconsistentNaming
    public int GetKey(string p_httpName, HttpField.Type p_type, bool submit)
        // ReSharper restore InconsistentNaming
    {
        int inversedSubmitRadioIdx = -1;
        for (int i = 0; i < this.Count; i++) {
            if ((this[i].HttpName.ToLower() == p_httpName.ToLower()) && (this[i].FieldType == p_type)) {
                if (this[i].FieldType == HttpField.Type.Radio) {
                    if (this[i].Submit == submit) return i;
                    else inversedSubmitRadioIdx = i;
                } else return i;
            }
        }
        if (inversedSubmitRadioIdx != -1) return inversedSubmitRadioIdx;
        else throw new FieldDoesNotExistException("FeildsCollection: Attempt to read non-existent string property parameter: " + p_httpName);
    }

    public int Add(HttpField httpField)
    {
        return mArr.Add(httpField);
    }


    public int AddTextBox(string httpName, string httpValue, string screenName)
    {
        HttpField httpField = new HttpField(httpName, httpValue, screenName, httpValue, HttpField.Type.TextBox);
        return Add(httpField);
    }
    public int AddTextBox(string httpName, string httpValue, string screenName, bool visible, bool submit)
    {
        HttpField httpField = new HttpField(httpName, httpValue, screenName, httpValue, HttpField.Type.TextBox);
        httpField.Visible = visible;
        httpField.Submit = submit;
        return Add(httpField);
    }

    public int AddTextBox(string httpName, int intHttpValue, string screenName)
    {
        HttpField httpField = new HttpField(httpName, Common.Print(intHttpValue), screenName, Common.Print(intHttpValue), HttpField.Type.TextBox);
        return Add(httpField);
    }
    public int AddTextBox(string httpName, int intHttpValue, string screenName, bool visible, bool submit)
    {
        HttpField httpField = new HttpField(httpName, Common.Print(intHttpValue), screenName, Common.Print(intHttpValue), HttpField.Type.TextBox);
        httpField.Visible = visible;
        httpField.Submit = submit;
        return Add(httpField);
    }

    public int AddTextBox(string httpName, float floatHttpValue, string screenName)
    {
        HttpField httpField = new HttpField(httpName, Common.PrintF2(floatHttpValue), screenName, Common.PrintF2(floatHttpValue), HttpField.Type.TextBox);
        return Add(httpField);
    }
    public int AddTextBox(string httpName, float floatHttpValue, string screenName, bool visible, bool submit)
    {
        HttpField httpField = new HttpField(httpName, Common.PrintF2(floatHttpValue), screenName, Common.PrintF2(floatHttpValue), HttpField.Type.TextBox);
        httpField.Visible = visible;
        httpField.Submit = submit;
        return Add(httpField);
    }

    public int AddTextArea(string httpName, string httpValue, string screenName)
    {
        HttpField httpField = new HttpField(httpName, httpValue, screenName, httpValue, HttpField.Type.TextArea);
        return Add(httpField);
    }
    public int AddTextArea(string httpName, string httpValue, string screenName, bool visible, bool submit)
    {
        HttpField httpField = new HttpField(httpName, httpValue, screenName, httpValue, HttpField.Type.TextArea);
        httpField.Visible = visible;
        httpField.Submit = submit;
        return Add(httpField);
    }

    public int AddLabel(string name, string value)
    {
        HttpField httpField = new HttpField("No submit", "No Submit", name, value, HttpField.Type.Label);
        return Add(httpField);
    }

    public int AddCheckBox(string httpName, bool boolHttpValue, string screenName)
    {
        string strHttpValue;
        string screenValue;
        if (boolHttpValue)
        {
            strHttpValue = "on";
            screenValue = "Checked";
        }
        else
        {
            strHttpValue = "";
            screenValue = "Unchecked";
        }

        HttpField httpField = new HttpField(httpName, strHttpValue, screenName, screenValue, HttpField.Type.CheckBox);
        return Add(httpField);
    }
    public int AddCheckBox(string httpName, bool boolHttpValue, string screenName, bool visible, bool submit)
    {
        string strHttpValue;
        string screenValue;
        if (boolHttpValue)
        {
            strHttpValue = "on";
            screenValue = "Checked";
        }
        else
        {
            strHttpValue = "";
            screenValue = "Unchecked";
        }

        HttpField httpField = new HttpField(httpName, strHttpValue, screenName, screenValue, HttpField.Type.CheckBox);
        httpField.Visible = visible;
        httpField.Submit = submit;
        return Add(httpField);
    }

    public int AddCheckBox(string httpName, bool boolHttpValue, string screenName, string val)
    {
        string strHttpValue;
        string screenValue;
        if (boolHttpValue)
        {
            strHttpValue = val;
            screenValue = "Checked";
        }
        else
        {
            strHttpValue = "";
            screenValue = "Unchecked";
        }

        HttpField httpField = new HttpField(httpName, strHttpValue, screenName, screenValue, HttpField.Type.CheckBox);
        return Add(httpField);
    }
    public int AddCheckBox(string httpName, bool boolHttpValue, string screenName, string val, bool visible, bool submit)
    {
        string strHttpValue;
        string screenValue;
        if (boolHttpValue)
        {
            strHttpValue = val;
            screenValue = "Checked";
        }
        else
        {
            strHttpValue = "";
            screenValue = "Unchecked";
        }

        HttpField httpField = new HttpField(httpName, strHttpValue, screenName, screenValue, HttpField.Type.CheckBox);
        httpField.Visible = visible;
        httpField.Submit = submit;
        return Add(httpField);
    }


    public int AddRadioNoneSelected(string httpName, string screenName, int nOfOptions)
    {
        return AddRadioNoneSelected(httpName, screenName, nOfOptions, true);
    }
    public int AddRadioNoneSelected(string httpName, string screenName, int nOfOptions, bool visible)
    {
        HttpField httpField = new HttpField(httpName, "No Value", screenName, "None Selected", HttpField.Type.Radio);
        httpField.Visible = visible;
        httpField.Submit = false;
        int result = Add(httpField);
        for (int i = 0; i < nOfOptions - 1; i++)
        {
            int index = AddRadio(httpName, screenName, new SelectOption("Inactive Radio Value", "Inactive Radio Value"), 1);
            this[index].Submit = false;
            this[index].Visible = false;
        }
        return result;
    }

    public int AddRadio(string httpName, string screenName, SelectOption option, int nOfOptions)
    {
        if (option == null) return AddRadioNoneSelected(httpName, screenName, nOfOptions);

        HttpField httpField = new HttpField(httpName, option.HttpValue, screenName, option.ScreenValue, HttpField.Type.Radio);
        int result = Add(httpField);
        for (int i = 0; i < nOfOptions - 1; i++)
        {
            int index = AddRadio(httpName, screenName, new SelectOption("Inactive Radio Value", "Inactive Radio Value"), 1);
            this[index].Submit = false;
            this[index].Visible = false;
        }
        return result;
    }
    public int AddRadio(string httpName, string screenName, SelectOption option, int nOfOptions, bool visible, bool submit)
    {
        if (option == null) return AddRadioNoneSelected(httpName, screenName, nOfOptions);

        HttpField httpField = new HttpField(httpName, option.HttpValue, screenName, option.ScreenValue, HttpField.Type.Radio);
        httpField.Visible = visible;
        httpField.Submit = submit;
        int result = Add(httpField);
        for (int i = 0; i < nOfOptions - 1; i++)
        {
            int index = AddRadio(httpName, screenName, new SelectOption("Inactive Radio Value", "Inactive Radio Value"), 1);
            this[index].Submit = false;
            this[index].Visible = false;
        }
        return result;
    }

    public int AddSelect(string httpName, string screenName, SelectOption option)
    {
        HttpField httpField = new HttpField(httpName, option.HttpValue, screenName, option.ScreenValue, HttpField.Type.Select);
        return Add(httpField);
    }
    public int AddSelect(string httpName, string screenName, SelectOption option, bool visible, bool submit)
    {
        HttpField httpField = new HttpField(httpName, option.HttpValue, screenName, option.ScreenValue, HttpField.Type.Select);
        httpField.Visible = visible;
        httpField.Submit = submit;
        return Add(httpField);
    }

    public int AddRuntimeSelect(string httpName, string screenName, SelectOption option)
    {
        HttpField httpField = new HttpField(httpName, option.HttpValue, screenName, option.ScreenValue, HttpField.Type.RuntimeSelect);
        return Add(httpField);
    }
    public int AddRuntimeSelect(string httpName, string screenName, SelectOption option, bool visible, bool submit)
    {
        HttpField httpField = new HttpField(httpName, option.HttpValue, screenName, option.ScreenValue, HttpField.Type.RuntimeSelect);
        httpField.Visible = visible;
        httpField.Submit = submit;
        return Add(httpField);
    }

    public int AddEmptySelect(string httpName, string screenName, SelectOption option)
    {
        HttpField httpField = new HttpField(httpName, option.HttpValue, screenName, option.ScreenValue, HttpField.Type.EmptySelect);
        return Add(httpField);
    }
    public int AddEmptySelect(string httpName, string screenName, SelectOption option, bool visible, bool submit)
    {
        HttpField httpField = new HttpField(httpName, option.HttpValue, screenName, option.ScreenValue, HttpField.Type.EmptySelect);
        httpField.Visible = visible;
        httpField.Submit = submit;
        return Add(httpField);
    }

    public int AddSimpleSelect(string httpName, string screenName, string val)
    {
        HttpField httpField = new HttpField(httpName, val, screenName, val, HttpField.Type.Select);
        return Add(httpField);
    }
    public int AddSimpleSelect(string httpName, string screenName, string val, bool visible, bool submit)
    {
        HttpField httpField = new HttpField(httpName, val, screenName, val, HttpField.Type.Select);
        httpField.Visible = visible;
        httpField.Submit = submit;
        return Add(httpField);
    }

    public int AddHidden(string httpName, string httpValue)
    {
        HttpField httpField = new HttpField(httpName, httpValue, httpName, httpValue, HttpField.Type.Hidden);
        return Add(httpField);
    }
    public int AddHidden(string httpName, string httpValue, bool visible, bool submit)
    {
        HttpField httpField = new HttpField(httpName, httpValue, httpName, httpValue, HttpField.Type.Hidden);
        httpField.Visible = visible;
        httpField.Submit = submit;
        return Add(httpField);
    }

    public int AddHiddenConstPageContent(string fieldName, string page, string formName, string formAction)
    {
        return AddHiddenConst(fieldName, FieldsCollection.GetHiddenField(fieldName, page, formName, formAction));
    }
    public int AddHiddenConstPageContent(string fieldName, string page, string formName, string formAction, bool visible, bool submit)
    {
        return AddHiddenConst(fieldName, FieldsCollection.GetHiddenField(fieldName, page, formName, formAction), visible, submit);
    }

    public int AddHiddenConst(string httpName, string httpValue)
    {
        HttpField httpField = new HttpField(httpName, httpValue, httpName, httpValue, HttpField.Type.HiddenConst);
        return Add(httpField);
    }
    public int AddHiddenConst(string httpName, string httpValue, bool visible, bool submit)
    {
        HttpField httpField = new HttpField(httpName, httpValue, httpName, httpValue, HttpField.Type.HiddenConst);
        httpField.Visible = visible;
        httpField.Submit = submit;
        return Add(httpField);
    }

    public int AddSubmit(string httpName, string httpValue)
    {
        HttpField httpField = new HttpField(httpName, httpValue, httpName, httpValue, HttpField.Type.Submit);
        return Add(httpField);
    }
    public int AddSubmit(string httpName, string httpValue, bool visible, bool submit)
    {
        HttpField httpField = new HttpField(httpName, httpValue, httpName, httpValue, HttpField.Type.Submit);
        httpField.Visible = visible;
        httpField.Submit = submit;
        return Add(httpField);
    }

    public int AddReset(string httpName, string httpValue)
    {
        HttpField httpField = new HttpField(httpName, httpValue, httpName, httpValue, HttpField.Type.Reset);
        return Add(httpField);
    }
    public int AddReset(string httpName, string httpValue, bool visible, bool submit)
    {
        HttpField httpField = new HttpField(httpName, httpValue, httpName, httpValue, HttpField.Type.Reset);
        httpField.Visible = visible;
        httpField.Submit = submit;
        return Add(httpField);
    }

    public int AddImage(string httpName)
    {
        HttpField httpField = new HttpField(httpName, ".x=1&.y=1", "Image", "x=1, y=1", HttpField.Type.Image);
        return Add(httpField);
    }
    public int AddImage(string httpName, bool visible, bool submit)
    {
        HttpField httpField = new HttpField(httpName, ".x=1&.y=1", "Image", "x=1, y=1", HttpField.Type.Image);
        httpField.Visible = visible;
        httpField.Submit = submit;
        return Add(httpField);
    }

    public int AddButton(string httpName, string httpValue)
    {
        HttpField httpField = new HttpField(httpName, httpValue, httpName, httpValue, HttpField.Type.Button);
        int index = Add(httpField);
        this[index].Submit = false;
        this[index].Visible = false;
        return index;
    }

    public void AddAllHiddenFieldsFromPage(string page, string formName, string formAction)
    {
        BrowserEmulatorParser parser = new BrowserEmulatorParser(page);
        //find the correct form
        while (true) 
        {
            var tag = parser.FindTag("form");

            string name;
            if (tag["name"] == null) name = "";
            else name = tag["name"].Value;

            string action;
            if (tag["action"] == null) action = "";
            else action = tag["action"].Value;

            if ((name.ToLower() == formName.ToLower()) && action.ToLower().StartsWith(formAction.ToLower())) break;
        }

        while (true)
        {
            AttributeList tag = parser.ProcessTag();
            if (tag.Name.ToLower() == "/form") break;
            if (tag.Name.ToLower() != "input") continue;
            if (tag["type"] == null) continue;
            if (tag["name"] == null) continue;
            if (tag["value"] == null) continue;
            if (tag["type"].Value.ToLower() == "hidden") this.AddHiddenConst(tag["name"].Value, tag["value"].Value);
        }
    }

    public void Merge(FieldsCollection col)
    {
        for (int i = 0; i < col.Count; i++)
        {
            this.Add(col[i]);
        }
    }
    public void Remove(int key)
    {
        mArr.RemoveAt(key);
    }

    public void RemoveFieldWithEx(string feildName, HttpField.Type fieldType)
    {
        int key = this.GetKey(feildName, fieldType);
        this.Remove(key);
    }

    public void RemoveFieldNoEx(string feildName, HttpField.Type fieldType)
    {
        try
        {
            int key = this.GetKey(feildName, fieldType);
            this.Remove(key);
        }
        catch (FieldDoesNotExistException) { }
    }

    public void Clear()
    {
        mArr.Clear();
    }

    public int Count
    {
        get { return mArr.Count; }
    }

    /*
    private string PrintHiddenInput(string name, string val)
    {
        return "<input type=hidden name='" + name + "' value='" + HttpUtility.HtmlEncode(val) + "'>";
    }
    public string GetHiddenValuesScript()
    {
        StringBuilder str = new StringBuilder();
        for(int i=0; i<count; i++)
        {
            if (this[i].submit != true) continue;
            switch (this[i].type)
            {
                case HttpField.Type.Image:
                    str.Append(PrintHiddenInput(this[i].httpName + ".x", "1"));
                    str.Append(PrintHiddenInput(this[i].httpName + ".y", "1"));
                    break;
                case HttpField.Type.CheckBox:
                    if (this[i].httpValue != "") str.Append(PrintHiddenInput(this[i].httpName, this[i].httpValue));
                    break;
                default:
                    str.Append(PrintHiddenInput(this[i].httpName, this[i].httpValue));
                    break;
            }
            str.Append("\n");
        }
        return str.ToString();
    }
    */
    public string GetHTTPKeysAndValuesStringWithBR()
    {
        return GetHTTPKeysAndValuesString().Replace("&", "&<BR>");
    }
    public string GetHTTPKeysAndValuesString()
    {
        StringBuilder str = new StringBuilder();
        for (int i = 0; i < Count; i++)
        {
            if (this[i].HttpName.Trim() == "") continue;
            if (this[i].Submit != true) continue;
            switch (this[i].FieldType)
            {
                case HttpField.Type.Label:
                    //do nothing for label
                    break;
                case HttpField.Type.Image:
                    if (str.Length != 0) str.Append("&");
                    str.Append(SafeUrlEncode(this[i].HttpName) + ".x=1&" + SafeUrlEncode(this[i].HttpName) + ".y=1");
                    break;
                case HttpField.Type.CheckBox:
                    if (this[i].HttpValue != "") {
                        if (str.Length != 0) str.Append("&");
                        str.Append(SafeUrlEncode(this[i].HttpName) + "=" + SafeUrlEncode(this[i].HttpValue));
                    }
                    break;
                default:
                    if (str.Length != 0) str.Append("&");
                    str.Append(SafeUrlEncode(this[i].HttpName) + "=" + SafeUrlEncode(this[i].HttpValue));
                    break;
            }
        }
        return str.ToString();
    }

    public string GetScreenKeysAndValuesString()
    {
        StringBuilder str = new StringBuilder();
        for (int i = 0; i < Count; i++)
        {
            if (str.Length != 0) { str.Append("\n"); }
            str.Append(this[i].ScreenName + " (" + this[i].HttpName + ") " + ": " + this[i].ScreenValue + " (" + this[i].HttpValue + ") ");
        }
        return str.ToString();
    }

    public void DoNotSubmitSubmit() // this is used for ASP.NET PostBacks
    {
        for (int i = 0; i < this.Count; i++)
        {
            if (this[i].FieldType == HttpField.Type.Submit) this[i].Submit = false;
        }
    }

    public static string GetHiddenField(string fieldName, string page, string formName, string formAction)
    {
        BrowserEmulatorParser parser = new BrowserEmulatorParser(page);
        try
        {
            parser.FindTag("form", new Dictionary<string, string> { {"name", formName}, {"action", formAction} } );
            while (true)
            {
                AttributeList tag = parser.ProcessTag();
                if (tag.Name.ToLower() != "input") continue;
                if (tag["type"] == null) continue;
                if (tag["name"] == null) continue;
                if (tag["value"] == null) continue;
                if ((tag["type"].Value.ToLower() == "hidden") && (tag["name"].Value.ToLower() == fieldName.ToLower())) return tag["value"].Value;
            }
        }
        catch (Exception ex)
        {
            throw new BrowserEmulatorException("Unable to get hidden field (" + fieldName + "): " + ex.Message);
        }
    }

    public static string GetVIEWSTATE(string page, string formName, string formAction)
    {
        return GetHiddenField("__VIEWSTATE", page, formName, formAction);
    }


    public static string GenerateHTMLFields(string page, string formName, string formAction)
    {
        BrowserEmulatorParser parser = new BrowserEmulatorParser(page);
        StringBuilder results = new StringBuilder();

        try
        {
            //find correct <form>
            while (true)
            {
                AttributeList tag = parser.FindTag("form");

                string name;
                if (tag["name"] == null) name = "";
                else name = tag["name"].Value;

                string action;
                if (tag["action"] == null) action = "";
                else action = tag["action"].Value;

                if ((name.ToLower() == formName.ToLower()) && action.ToLower().StartsWith(formAction.ToLower())) {
                    results.Append(PrintTag(tag) + "<br><br>");
                    break;
                }
            }

            //find </form>, <input>, <textares> or <select> and button(?)
            while (true)
            {
                AttributeList tag = parser.FindTags("input", "textarea", "select", "button", "/form");
                results.Append(PrintTag(tag) + "<br><br>");
                if (tag.Name.ToLower() == "/form") break;
            }

            return results.ToString();
        }
        catch (BrowserEmulatorException ex)
        {
            throw new BrowserEmulatorException("ValidateAgainstHTMLPage(): " + ex.Message);
        }
    }

    public void MergeReplace(FieldsCollection input) //if the field to replace does not exist, error
    {
        for (int i = 0; i < input.Count; i++)
        {
            int key;
            //we do it that way to make sure when we are merging active and inactive radio values do not mix.
            try
            {
                if (input[i].FieldType == HttpField.Type.Radio) key = this.GetKey(input[i].HttpName, input[i].FieldType, input[i].Submit);
                else key = this.GetKey(input[i].HttpName, input[i].FieldType);
            }
            catch (FieldDoesNotExistException ex)
            {
                throw new BrowserEmulatorException("MergeReplace: Unable to find a field to replace: " + ex.Message);
            }

            this[key] = new HttpField(input[i]);
        }
    }
    public static string AutogenerateCodeForDefaultFields(string var, string page, string formName, string formAction) {
        StringBuilder code = new StringBuilder();
        BrowserEmulatorParser parser = new BrowserEmulatorParser(page);

        try
        {
            //find correct <form>
            parser.FindTag("form", new Dictionary<string, string> { { "name", formName }, { "action", formAction } });

            //find </form>, <input>, <textares> or <select>
            while (true)
            {
                AttributeList tag = parser.FindTags("input", "textarea", "select", "/form", "script");
                parser.UndoParseTag();
                string lowercaseTagName = tag.Name.ToLower();
                if (lowercaseTagName == "script")
                {
                    parser.FindTag("/script");
                    continue;
                }
                if (lowercaseTagName == "input")
                {
                    code.Append(var + "." + ParseAndCodeInputField(parser).Replace("'", "\"") + "<br>");
                    continue;
                }
                if (lowercaseTagName == "textarea")
                {
                    code.Append(var + "." + ParseAndCodeTextAreaField(parser).Replace("'", "\"") + "<br>");
                    continue;
                }
                if (lowercaseTagName == "select")
                {
                    code.Append(var + "." + ParseAndCodeSelectField(parser).Replace("'", "\"") + "<br>");
                    continue;
                }
                if (lowercaseTagName == "/form") break;
                throw new BrowserEmulatorException("Unknown tag: " + tag.Name);
            }
        }
        catch (BrowserEmulatorException ex)
        {
            throw new BrowserEmulatorException("AutogenerateCodeForDefaultFields(): " + ex.Message);
        }
        return code.ToString();
    }

    public static string AutogenerateCodeForParsing(string var, string page)
    {
        BrowserEmulatorParser parser = new BrowserEmulatorParser(page.ToLower());
        StringBuilder code = new StringBuilder();
        while (true)
        {
            AttributeList tag;
            try
            {
                tag = parser.ProcessTag();
            }
            catch (EOFException)
            {
                break;
            }
            code.Append(var + ".ProcessTag(\"" + tag.Name + "\");\n");
        }
        return code.ToString();
    }
    //public void AutogenerateDotNetFields(string page, string 

    public void AddDotNetVariables(string page, string formName, string formAction, string eventTarget, string eventArgument)
    {
        AddHidden("__EVENTTARGET", eventTarget);
        AddHidden("__EVENTARGUMENT", eventArgument);
        AddHiddenConstPageContent("__VIEWSTATE", page, formName, formAction);
    }

    public void AddDotNet20Variables(string page, string formName, string formAction, string eventTarget, string eventArgument)
    {
        AddDotNetVariables(page, formName, formAction, eventTarget, eventArgument);
        AddHiddenConstPageContent("__LASTFOCUS", page, formName, formAction);
        AddHiddenConstPageContent("__EVENTVALIDATION", page, formName, formAction);
    }

    public void AutogenerateDefaultFields(string page, string formName, string formAction)
    {
        BrowserEmulatorParser parser = new BrowserEmulatorParser(page);

        try
        {
            //find correct <form>
            parser.FindTag("form", new Dictionary<string, string> { { "name", formName }, { "action", formAction } });

            //find </form>, <input>, <textares> or <select>
            while (true)
            {
                AttributeList tag = parser.FindTags("input", "textarea", "select", "/form", "script");

                parser.UndoParseTag();
                string lowercaseTagName = tag.Name.ToLower();
                if (lowercaseTagName == "script")
                {
                    parser.FindTag("/script");
                    continue;
                }

                if (lowercaseTagName == "input")
                {
                    ParseAndAddInputField(parser);
                    continue;
                }
                if (lowercaseTagName == "textarea")
                {
                    ParseAndAddTextAreaField(parser);
                    continue;
                }
                if (lowercaseTagName == "select")
                {
                    ParseAndAddSelectField(parser);
                    continue;
                }
                if (lowercaseTagName == "/form") break;
                throw new BrowserEmulatorException("Unknown tag" + tag.Name);
            }
        }
        catch (BrowserEmulatorException ex)
        {
            throw new BrowserEmulatorException("AutogenerateDefaultFields(): " + ex.Message);
        }
    }

    public void ValidateAgainstHTMLPage(string page, string formName, string formAction)
    {
        //Note that we are not handling <button> field!!!
        ArrayList radioButtons = new ArrayList();

        BrowserEmulatorParser parser = new BrowserEmulatorParser(page.ToLower());
        int fieldsValidated = 0;

        try
        {
            //find correct <form>
            if (!string.IsNullOrEmpty(formName) && !string.IsNullOrEmpty(formAction))
            {
                parser.FindTag("form", new Dictionary<string, string> { { "name", formName }, { "action", formAction } });
            }
            else if (!string.IsNullOrEmpty(formName) && string.IsNullOrEmpty(formAction))
            {
                parser.FindTag("form", new Dictionary<string, string> { { "name", formName } });
            }
            else if (string.IsNullOrEmpty(formName) && !string.IsNullOrEmpty(formAction))
            {
                parser.FindTag("form", new Dictionary<string, string> { { "action", formAction } });
            }
            else
            {
                parser.FindTag("form");
            }

            //find </form>, <input>, <textares> or <select>
            while (true)
            {
                AttributeList tag = parser.FindTags("input", "textarea", "select", "/form", "script");
                parser.UndoParseTag();
                if (tag.Name == "script")
                {
                    parser.FindTag("/script");
                    continue;
                }
                if (tag.Name == "input")
                {
                    ValidateInputField(parser, ref fieldsValidated, radioButtons);
                    continue;
                }
                if (tag.Name == "textarea")
                {
                    ValidateTextAreaField(parser, ref fieldsValidated);
                    continue;
                }
                if (tag.Name == "select")
                {
                    ValidateSelectField(parser, ref fieldsValidated);
                    continue;
                }
                if (tag.Name == "/form") break;
                throw new BrowserEmulatorException("Unknown tag" + tag.Name);
            }

            //find </form>
            parser.ProcessTag("/form");

            int pageFields = 0;
            for (int i = 0; i < this.Count; i++)
            {
                if ((this[i].FieldType != HttpField.Type.Label) && (this[i].Dynamic == false)) pageFields++;
            }

            if (pageFields != fieldsValidated) throw new BrowserEmulatorException("Number of fields on HTML page and in input do not match. HTML page has " + fieldsValidated + " while FieldsCollection has " + pageFields + ". Form name = " + formName);
            ValidateRadioButtonValues(radioButtons);

        }
        catch (BrowserEmulatorException ex)
        {
            throw new BrowserEmulatorException("ValidateAgainstHTMLPage(): " + ex.Message);
        }
    }

    //----------------------------------------------------------------------------------------------------------------
    private void ParseAndAddSelectField(BrowserEmulatorParser parser)
    {
        AttributeList selectTag = parser.ProcessTag("select");
        if (selectTag["name"] == null) return;
        string name = selectTag["name"].Value;
        string scrName = "Autogenerated";

        string selectedVal = null;
        string selectedName = null;
        bool first = true;
        while (true)
        {
            AttributeList currTag = parser.FindTags("option", "/select", "/form");
            parser.UndoParseTag();
            string lowercaseCurrTagName = currTag.Name.ToLower();
            if (lowercaseCurrTagName == "option")
            {
                parser.ProcessTag("option");
                string optionVal = null;
                if (currTag["value"] != null) optionVal = currTag["value"].Value;

                string optionName = "";
                AttributeList nextTag;
                while (true)
                {
                    optionName = optionName + parser.GetTextToNextTag(out nextTag);
                    if (nextTag.Name.StartsWith("!--") != true) break;
                }
                optionName = optionName.Trim();

                //we are expecting </option> or <option>
                string lowercaseNextTagName = nextTag.Name.ToLower();
                if ((lowercaseNextTagName != "/option") && (lowercaseNextTagName != "option") && (lowercaseNextTagName != "/select")) throw new BrowserEmulatorException("ParseAndAddSelectField: <" + nextTag.Name + "> where <option> or </option> or </select> is expected");

                //if this is not an ending for this option, let's return it, so it can be processed appropriately
                if (lowercaseNextTagName != "/option") parser.UndoParseTag();

                //if this is a valueless option make value = name
                if (optionVal == null) optionVal = optionName;

                if ((first) || (currTag["selected"] != null)) {
                    selectedVal = optionVal;
                    selectedName = optionName;
                    first = false;
                }
            }
            if (lowercaseCurrTagName == "/select") break;
            if (lowercaseCurrTagName == "/form") break;
        }

        parser.ProcessTag("/select");
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (selectedName != null && selectedVal != null)
        {
            AddSelect(name, scrName, new SelectOption(selectedVal, selectedName));
        }
        else
        {
            //if this is an empty select, we show it but do not submit
            //AddRuntimeSelect(name, scrName, new SelectOption("", ""), true, false);

            //REALLY, if empty or runtime select, we do not do anything
        }
    }
    private static string ParseAndCodeSelectField(BrowserEmulatorParser parser)
    {
        AttributeList selectTag = parser.ProcessTag("select");
        if (selectTag["name"] == null) return "";
        string name = selectTag["name"].Value;
        string scrName = "Autogenerated";

        string selectedVal = null;
        string selectedName = null;
        bool first = true;
        while (true)
        {
            AttributeList currTag = parser.FindTags("option", "/select", "/form");
            parser.UndoParseTag();
            string lowercaseCurrTagName = currTag.Name.ToLower();
            if (lowercaseCurrTagName == "option")
            {
                parser.ProcessTag("option");
                string optionVal = null;
                if (currTag["value"] != null) optionVal = currTag["value"].Value;

                string optionName = "";
                AttributeList nextTag;
                while (true)
                {
                    optionName = optionName + parser.GetTextToNextTag(out nextTag);
                    if (nextTag.Name.StartsWith("!--") != true) break;
                }
                optionName = optionName.Trim();

                //we are expecting </option> or <option>
                string lowercaseNextTagName = nextTag.Name.ToLower();
                if ((lowercaseNextTagName != "/option") && (lowercaseNextTagName != "option") && (lowercaseNextTagName != "/select")) throw new BrowserEmulatorException("ParseAndCodeSelectField: <" + nextTag.Name + "> where <option> or </option> or </select> is expected");

                //if this is not an ending for this option, let's return it, so it can be processed appropriately
                if (lowercaseNextTagName != "/option") parser.UndoParseTag();

                //if this is a valueless option make value = name
                if (optionVal == null) optionVal = optionName;

                if ((first) || (currTag["selected"] != null))
                {
                    selectedVal = optionVal;
                    selectedName = optionName;
                    first = false;
                }
            }
            if (lowercaseCurrTagName == "/select") break;
            if (lowercaseCurrTagName == "/form") break;
        }

        parser.ProcessTag("/select");
        string lineResult;
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (selectedName != null && selectedVal != null)
        {
            lineResult = "AddSelect('" + name + "', '" + scrName + "', new SelectOption('" + selectedVal + "', '" + selectedName + "'));";
        }
        else
        {
            //if this is an empty select, we show it but do not submit-- and we comment out the code
            lineResult = "AddRuntimeSelect('" + name + "', '" + scrName + "', new SelectOption('', ''), true, false); //Handle Runtime or Empty Select*****************";
        }
        return lineResult;
    }

    private void ParseAndAddTextAreaField(BrowserEmulatorParser parser)
    {
        AttributeList tag = parser.ProcessTag("textarea");

        if (tag["name"] == null) return;
        string name = tag["name"].Value;
        //string val = parser.GetTextToTag("/textarea");
        string val = parser.GetTextAndTagsToTag("/textarea");
        string scrName = "Autogenerated";
        AddTextArea(name, val, scrName);
    }
    private static string ParseAndCodeTextAreaField(BrowserEmulatorParser parser)
    {
        AttributeList tag = parser.ProcessTag("textarea");

        if (tag["name"] == null) return "";
        string name = tag["name"].Value;
        //string val = parser.GetTextToTag("/textarea");
        string val = parser.GetTextAndTagsToTag("/textarea");
        string scrName = "Autogenerated";
        string lineResult = "AddTextArea('" + name + "', '" + val + "', '" + scrName + "');";
        return lineResult;
    }
    private void ParseAndAddInputField(BrowserEmulatorParser parser)
    {
        AttributeList tag = parser.ProcessTag("input");

        if (tag["name"] == null) return;
        string name = tag["name"].Value;

        string val = "";
        if (tag["value"] != null) val = tag["value"].Value;

        string type;
        if ((tag["type"] == null) || (tag["type"].Value == "")) type = "text";
        else type = tag["type"].Value;

        bool boolVal = false;
        if (tag["checked"] != null) boolVal = true;

        string scrName = "Autogenerated";
        string valName = "Autogenerated";

        switch (type.ToLower())
        {
            case "text":
            case "password":
                AddTextBox(name, val, scrName);
                break;
            case "checkbox":
                if (val != "") AddCheckBox(name, boolVal, scrName, val);
                else AddCheckBox(name, boolVal, scrName);
                break;
            case "radio":
                AddRadio(name, scrName, new SelectOption(val, valName), 1, true, boolVal);
                break;
            case "hidden":
                AddHidden(name, val);
                break;
            case "submit":
                AddSubmit(name, val);
                break;
            case "image":
                AddImage(name);
                break;
            case "reset":
                AddReset(name, val);
                break;
            case "button":
                AddButton(name, val);
                break;
            default:
                throw new BrowserEmulatorException("Invalid <input> type = " + type);
        }
    }
    private static string ParseAndCodeInputField(BrowserEmulatorParser parser)
    {
        AttributeList tag = parser.ProcessTag("input");

        if (tag["name"] == null) return "";
        string name = tag["name"].Value;

        string val = "";
        if (tag["value"] != null) val = tag["value"].Value;

        string type;
        if ((tag["type"] == null) || (tag["type"].Value == "")) type = "text";
        else type = tag["type"].Value;

        bool boolVal = false;
        if (tag["checked"] != null) boolVal = true;

        string scrName = "Autogenerated";
        string valName = "Autogenerated";

        string lineResult;
        switch (type.ToLower())
        {
            case "text":
            case "password":
                lineResult = "AddTextBox('" + name + "', '" + val + "', '" + scrName + "');".Replace("'", "\"");
                break;
            case "checkbox":
                if (val != "") lineResult = "AddCheckBox('" + name + "', " + boolVal.ToString().ToLower() + ", '" + scrName + "', '" + val + "');";
                else lineResult = "AddCheckBox('" + name + "', " + boolVal.ToString().ToLower() + ", '" + scrName + "');";
                break;
            case "radio":
                lineResult = "AddRadio('" + name + "', '" + scrName + "', new SelectOption('" + val + "', '" + valName + "'), 1, true, " + boolVal.ToString().ToLower() + ");";
                break;
            case "hidden":
                lineResult = "AddHidden('" + name + "', '" + val + "');";
                break;
            case "submit":
                lineResult = "AddSubmit('" + name + "', '" + val + "');";
                break;
            case "image":
                lineResult = "AddImage('" + name + "');";
                break;
            case "reset":
                lineResult = "AddReset('" + name + "', '" + val + "');";
                break;
            case "button":
                lineResult = "AddButton('" + name + "', '" + val + "');";
                break;
            default:
                throw new BrowserEmulatorException("Invalid <input> type = " + type);
        }
        return lineResult;
    }

    //------------------------
    private static string PrintTag(AttributeList tag)
    {
        StringBuilder strTag = new StringBuilder();
        strTag.Append("&lt;" + tag.Name + " ");
        for (int i = 0; i < tag.Count; i++) {
            strTag.Append(tag[i].Name + "='" + tag[i].Value + "' ");
        }
        strTag.Append("&gt;");
        return strTag.ToString();
    }

    //------------------------
    private void ValidateRadioButtonValues(ArrayList radioButtons)
    {
        for (int i = 0; i < this.Count; i++)
        {
            if ((this[i].FieldType == HttpField.Type.Radio) && (this[i].Submit) && (!this[i].Dynamic != true))
            {
                FindRadioValue(radioButtons, this[i].HttpName, this[i].HttpValue);
            }
        }
    }

    private void FindRadioValue(ArrayList radioButtons, string name, string val)
    {
        for (int i = 0; i < radioButtons.Count; i++)
        {
            AttributeList tag = (AttributeList)radioButtons[i];
            if (tag.Name == "input")
            {
                if (tag["type"] == null) continue;
                if (tag["name"] == null) continue;
                if (tag["value"] == null) continue;
                if ((tag["type"].Value == "radio") && (tag["name"].Value == name.ToLower()) && (tag["value"].Value == val.ToLower())) return;
            }

        }
        throw new BrowserEmulatorException("Attempting to submit a radio button (name=" + name + ", value=" + val + ") that does not exist in HTML file");
    }

    private void ValidateInputField(BrowserEmulatorParser parser, ref int fieldsValidated, ArrayList radioButtons)
    {
        AttributeList tag = parser.ProcessTag("input");
        if (tag["name"] == null) return;

        string type;
        if ((tag["type"] == null) || (tag["type"].Value == "")) type = "text";
        else type = tag["type"].Value;

        switch (type.ToLower())
        {
            case "text":
            case "password":
                try
                {
                    GetKey(tag["name"].Value, HttpField.Type.TextBox);
                }
                catch
                {
                    throw new BrowserEmulatorException("Fields Validation: field '" + tag["name"].Value + "' exists in HTML file but does not exist in input or is a diffrent type.");
                }
                break;
            case "checkbox":
                try
                {
                    GetKey(tag["name"].Value, HttpField.Type.CheckBox);
                }
                catch
                {
                    throw new BrowserEmulatorException("Fields Validation: field '" + tag["name"].Value + "' exists in HTML file but does not exist in input or is a diffrent type.");
                }
                break;
            case "radio":
                try
                {
                    GetKey(tag["name"].Value, HttpField.Type.Radio);
                    radioButtons.Add(tag);
                }
                catch
                {
                    throw new BrowserEmulatorException("Fields Validation: field '" + tag["name"].Value + "' exists in HTML file but does not exist in input or is a diffrent type.");
                }
                break;
            case "hidden":
                int index;

                //firt try for Hidden Const
                try { index = GetKey(tag["name"].Value, HttpField.Type.HiddenConst); }
                catch
                {
                    //if it does not exist try for Hidden
                    try
                    {
                        GetKey(tag["name"].Value, HttpField.Type.Hidden);
                        break;
                    }
                    catch
                    {
                        //if we can't find either throw exception.
                        throw new BrowserEmulatorException("Fields Validation: field '" + tag["name"].Value + "' exists in HTML file but does not exist in input or is a diffrent type.");
                    }
                }
                //if we found a Hidden Const make sure the values match
                string val;
                if (tag["value"] != null) val = tag["value"].Value;
                else val = "";

                if (val != this[index].HttpValue.ToLower()) throw new BrowserEmulatorException("Fields Validation: field '" + tag["name"].Value + "' is a Hidden Const. Value in HTML file is '" + val + "' while value in FieldsCollection is '" + this[index].HttpValue + "'");
                break;
            case "submit":
                try
                {
                    GetKey(tag["name"].Value, HttpField.Type.Submit);
                }
                catch
                {
                    throw new BrowserEmulatorException("Fields Validation: field '" + tag["name"].Value + "' exists in HTML file but does not exist in input or is a diffrent type.");
                }
                break;
            case "image":
                try
                {
                    GetKey(tag["name"].Value, HttpField.Type.Image);
                }
                catch
                {
                    throw new BrowserEmulatorException("Fields Validation: field '" + tag["name"].Value + "' exists in HTML file but does not exist in input or is a diffrent type.");
                }
                break;
            case "reset":
                try
                {
                    GetKey(tag["name"].Value, HttpField.Type.Reset);
                }
                catch
                {
                    throw new BrowserEmulatorException("Fields Validation: field '" + tag["name"].Value + "' exists in HTML file but does not exist in input or is a diffrent type.");
                }
                break;
            case "button":
                try
                {
                    GetKey(tag["name"].Value, HttpField.Type.Button);
                }
                catch
                {
                    throw new BrowserEmulatorException("Fields Validation: field '" + tag["name"].Value + "' exists in HTML file but does not exist in input or is a diffrent type.");
                }
                break;
            default:
                throw new BrowserEmulatorException("Fields Validation: unable to find a matching type for input nameed " + tag["name"].Value + ". It must be a " + type + " type");
        }
        fieldsValidated++;
    }

    private void ValidateTextAreaField(BrowserEmulatorParser parser, ref int fieldsValidated)
    {
        AttributeList tag = parser.ProcessTag("textarea");
        if (tag["name"] == null) return;

        try { GetKey(tag["name"].Value, HttpField.Type.TextArea); }
        catch (BrowserEmulatorException) { throw new BrowserEmulatorException("Fields Validation: textearea field '" + tag["name"].Value + "' exists in HTML file but does not exist in input."); }

        parser.FindTag("/textarea");
        fieldsValidated++;
    }

    private void ValidateSelectField(BrowserEmulatorParser parser, ref int fieldsValidated)
    {
        AttributeList selectTag = parser.ProcessTag("select");
        if (selectTag["name"] == null) return;

        int key;

        //handle if we defined this select as empty or runtime
        bool emptySelect, runtimeSelect;
        try
        {
            // ReSharper disable once RedundantAssignment
            key = GetKey(selectTag["name"].Value, HttpField.Type.EmptySelect);
            emptySelect = true;
        }
        catch (BrowserEmulatorException)
        {
            emptySelect = false;
        }

        try
        {
            // ReSharper disable once RedundantAssignment
            key = GetKey(selectTag["name"].Value, HttpField.Type.RuntimeSelect);
            runtimeSelect = true;
        }
        catch (BrowserEmulatorException)
        {
            runtimeSelect = false;
        }


        if (emptySelect)
        {
            AttributeList currTag = parser.FindTags("option", "/select", "/form");
            parser.UndoParseTag();
            if (currTag.Name != "/select") throw new BrowserEmulatorException("Fields Validation: </select> is expected (EmptySelect). Insted getting <" + currTag.Name + ">");
            fieldsValidated++;
            return;
        }

        if (runtimeSelect)
        {
            AttributeList currTag = parser.FindTags("/select", "/form");
            parser.UndoParseTag();
            if (currTag.Name != "/select") throw new BrowserEmulatorException("Fields Validation: </select> is expected (RuntimeSelect). Insted getting <" + currTag.Name + ">");
            fieldsValidated++;
            return;
        }

        //now handle normal select
        try { key = GetKey(selectTag["name"].Value, HttpField.Type.Select); }
        catch (BrowserEmulatorException) { throw new BrowserEmulatorException("Fields Validation: select field '" + selectTag["name"].Value + "' exists in HTML file but does not exist in input."); }

        bool optionFound = false;
        while (true)
        {
            AttributeList currTag = parser.FindTags("option", "/select", "/form");
            parser.UndoParseTag();
            if (currTag.Name == "option")
            {
                if (IsSelectedOption(parser, key))
                {
                    optionFound = true;
                    parser.FindTags("/select", "/form");
                    parser.UndoParseTag();
                    break;
                }
            }
            if (currTag.Name == "/select") break;
            if (currTag.Name == "/form") break;
        }

        parser.ProcessTag("/select");
        if (optionFound != true) throw new BrowserEmulatorException("Fields Validation: select field '" + selectTag["name"].Value + "': option selected in input (screenValue=" + this[key].ScreenValue + ", httpValue=" + this[key].HttpValue + ") does not exists in HTML file.");
        fieldsValidated++;
    }

    private bool IsSelectedOption(BrowserEmulatorParser parser, int key)
    {
        AttributeList tag = parser.ProcessTag("option");

        string val;
        if (tag["value"] != null) val = tag["value"].Value;
        else val = null;

        //handle comments properly
        string name = "";
        while (true)
        {
            name = name + parser.GetTextToNextTag(out tag);
            if (tag.Name.StartsWith("!--") != true) break;
        }
        name = name.Trim();

        //we are expecting </option> or <option>
        if ((tag.Name != "/option") && (tag.Name != "option") && (tag.Name != "/select")) throw new BrowserEmulatorException("Fields Validation: <" + tag.Name + "> where <option> or </option> or </select> is expected");

        //if this is not an ending for this option, let's return it, so it can be processed appropriately
        if (tag.Name != "/option") parser.UndoParseTag();

        //if this is a valueless option make value = name
        if (val == null) val = name.Trim();

        if ((val.Replace("<", "") == this[key].HttpValue.ToLower().Replace("<", "")) && (name.Trim().Replace("<", "") == this[key].ScreenValue.ToLower().Replace("<", ""))) return true;
        else return false;
    }


    private string SafeUrlEncode(string str)
    {
        string lowercaseEncode = HttpUtility.UrlEncode(str);
        StringBuilder uppercaseEncode = new StringBuilder();
        int upNChars = 0;
        for (int i = 0; i < lowercaseEncode.Length; i++)
        {
            if (upNChars <= 0)
            {
                uppercaseEncode.Append(lowercaseEncode[i]);
            }
            else
            {
                uppercaseEncode.Append(lowercaseEncode.ToUpper()[i]);
                upNChars--;
            }
            if (lowercaseEncode[i] == '%') upNChars = 2;
        }
        //return uppercaseEncode.ToString();
        return uppercaseEncode.ToString().Replace("!", "%21").Replace(")", "%29").Replace("(", "%28");
    }

    /// <summary>
    /// Returns an array of select options for the specified select name in the specified form.
    /// </summary>
    /// <param name="page">The page to parse for the selects.</param>
    /// <param name="formName">The form name.</param>
    /// <param name="formAction">The form action.</param>
    /// <param name="selectName">The select name.</param>
    /// <returns>Returns an array list of select option objects.</returns>
    public static ArrayList GetSelectOptions(string page, string formName, string formAction, string selectName)
    {
        BrowserEmulatorParser parser = new BrowserEmulatorParser(page);
        ArrayList arrayListOptions = new ArrayList();

        // Find the correct form.
        while (true) {
            AttributeList tag = parser.FindTag("form");
            string name = "";
            string action = "";

            if (tag["name"] != null)
            {
                name = tag["name"].Value;
            }

            if (tag["action"] != null)
            {
                action = tag["action"].Value;
            }

            if ((name.ToLower() == formName.ToLower()) && (action.ToLower().StartsWith(formAction.ToLower())))
            {
                // Found it, break out of the loop.
                break;
            }
        }

        // Find the correct select.
        while (true)
        {
            AttributeList tag = parser.FindTag("select");
            string name = "";

            if (tag["name"] != null)
            {
                name = tag["name"].Value;
            }

            if (name.ToLower() == selectName.ToLower())
            {
                // Found it, break out of the loop.
                break;
            }
        }

        // Now cycle through all the option values in this select.
        while (true)
        {
            AttributeList tag = parser.ProcessTag();

            if (tag.Name == "/select")
            {
                // No more options to add.
                break;
            }

            if (tag.Name != "option")
            {
                continue;
            }

            if (tag["value"] == null)
            {
                throw new BrowserEmulatorException("Error: <option> without value parameter");
            }

            arrayListOptions.Add(new SelectOption(tag["value"].Value, parser.GetTextToTag("/option")));
        }

        return arrayListOptions;
    }

    // ReSharper disable once InconsistentNaming
    private readonly ArrayList mArr = new();
}