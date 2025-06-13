using System;
using System.Threading.Tasks;
using Supermodel.Presentation.WebMonk.Controllers.Api;
using WebMonk.Filters;
using WebMonk.RazorSharp.Html2RazorSharp;

namespace HTML2RazorSharpWM.API.TranslatorApi;

[AllowDangerousValues]
public class TranslatorApiController : CommandApiController<TranslatorInput, TranslatorOutput>
{
    #region Methods
    protected override Task<TranslatorOutput> ExecuteAsync(TranslatorInput input)
    {
        var output = new TranslatorOutput();
        try
        {
            if (input.Html != string.Empty)
            {
                var translator = TranslatorBase.CreateTextual(input.Html, input.SortAttributes, input.GenerateInvalidTags);
                output.RazorSharp = translator.ToRazorSharp();
                
                //This is to test. Results should be identical
                //var t2 = TranslatorBase.CreateMnemonic(input.Html, input.SortAttributes, input.GenerateInvalidTags);
                //var x = t2.ToRazorSharp();
                //var html = x.ToHtml().ToString();
                //output.RazorSharp = html;
            }
            else
            {
                output.RazorSharp = string.Empty;
            }
        }
        catch (Exception exception)
        {
            output.RazorSharp = exception.Message;
            output.Error = true;
        }

        return Task.FromResult(output);
    }
    #endregion
}

    /*protected void RefreshResult()
    {
        InputTextBoxChangeLineNumbers();
        try
        {
            if (InputTextBox.Text != string.Empty)
            {
                var translator = TranslatorBase.CreateTextual(InputTextBox.Text, SortAttributesCheckBox.Checked, GenerateInvalidTagsCheckBox.Checked);
                OutputTextBox.Text = translator.ToRazorSharp();
            }
            else
            {
                OutputTextBox.Text = string.Empty;
                InputTextBoxChangeLineNumbers();
            }
            OutputTextBox.BackColor = Color.BlanchedAlmond;
        }
        catch (Exception exception)
        {
            OutputTextBox.Text = exception.Message;
            OutputTextBox.BackColor = Color.LightSalmon;
        }
        OutputTextBoxChangeLineNumbers();
        OutputLineNumberTextBox.BackColor = OutputTextBox.BackColor;
    }
    */