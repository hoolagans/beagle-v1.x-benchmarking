using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebMonk.RazorSharp.Html2RazorSharp;

namespace HTML2RazorSharpWinForms;

public partial class Form : System.Windows.Forms.Form
{
    public Form()
    {
        InitializeComponent();
    }

    //private void OutputTextBoxTextChanged(object sender, EventArgs e) { }

    private async void InputTextBoxTextChanged(object? sender, EventArgs? e)
    {
        if (CurrentCancellationTokenSource == null)
        {
            try
            {
                CurrentCancellationTokenSource = new CancellationTokenSource();
                await Task.Delay(1000, CurrentCancellationTokenSource.Token);
                RefreshResult();
                CurrentCancellationTokenSource = null;
            }
            catch (Exception)
            {
                CurrentCancellationTokenSource = null;
                InputTextBoxTextChanged(sender, e);
            }
        }
        else
        {
            CurrentCancellationTokenSource.Cancel(true);
        }
    }

    private void CheckBoxCheckedChanged(object? sender, EventArgs? e)
    {
        InputTextBox.WordWrap = OutputTextBox.WordWrap = WordWrapCheckBox.Checked;
        RefreshResult();
    }

    protected void RefreshResult()
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

    protected void InputTextBoxChangeLineNumbers(object? sender = null, EventArgs? e = null)
    {
        //from https://stackoverflow.com/questions/41523658/c-sharp-richtextbox-line-numbers-column
        InputTextBox.Select();
        Point pt = new Point(0, 0);
        int firstIndex = InputTextBox.GetCharIndexFromPosition(pt);
        int firstLine = InputTextBox.GetLineFromCharIndex(firstIndex);
        pt.X = ClientRectangle.Width;
        pt.Y = ClientRectangle.Height;
        int lastIndex = InputTextBox.GetCharIndexFromPosition(pt);
        int lastLine = InputTextBox.GetLineFromCharIndex(lastIndex);
        InputLineNumberTextBox.Text = "";
        for (int i = firstLine; i <= lastLine + 1; i++)
        {
            InputLineNumberTextBox.SelectionAlignment = HorizontalAlignment.Center;
            // ReSharper disable once LocalizableElement
            InputLineNumberTextBox.Text += $"{i + 1}\n";
        }
    }

    protected void OutputTextBoxChangeLineNumbers(object? sender = null, EventArgs? e = null)
    {
        //from https://stackoverflow.com/questions/41523658/c-sharp-richtextbox-line-numbers-column
        //OutputTextBox.Select();
        Point pt = new Point(0, 0);
        int firstIndex = OutputTextBox.GetCharIndexFromPosition(pt);
        int firstLine = OutputTextBox.GetLineFromCharIndex(firstIndex);
        pt.X = ClientRectangle.Width;
        pt.Y = ClientRectangle.Height;
        int lastIndex = OutputTextBox.GetCharIndexFromPosition(pt);
        int lastLine = OutputTextBox.GetLineFromCharIndex(lastIndex);
        OutputLineNumberTextBox.Text = "";
        for (int i = firstLine; i <= lastLine + 1; i++)
        {
            OutputLineNumberTextBox.SelectionAlignment = HorizontalAlignment.Center;
            // ReSharper disable once LocalizableElement
            OutputLineNumberTextBox.Text += $"{i + 1}\n";
        }
    }

    protected CancellationTokenSource? CurrentCancellationTokenSource { get; set; }
}