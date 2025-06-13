using System.Drawing;
using System.Windows.Forms;

namespace HTML2RazorSharpWinForms
{
    partial class Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.OptionsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.OutputTextBox = new System.Windows.Forms.RichTextBox();
            this.InputTextBox = new System.Windows.Forms.RichTextBox();
            this.InputLineNumberTextBox = new System.Windows.Forms.RichTextBox();
            this.OutputLineNumberTextBox = new System.Windows.Forms.RichTextBox();
            this.SortAttributesCheckBox = new System.Windows.Forms.CheckBox();
            this.GenerateInvalidTagsCheckBox = new System.Windows.Forms.CheckBox();
            this.WordWrapCheckBox = new System.Windows.Forms.CheckBox();
            this.TableLayoutPanel.SuspendLayout();
            this.SuspendLayout();

            // 
            // OptionsTableLayoutPanel
            // 
            this.OptionsTableLayoutPanel.AutoSize = true;
            this.OptionsTableLayoutPanel.ColumnCount = 1;
            this.OptionsTableLayoutPanel.RowCount = 3;
            this.OptionsTableLayoutPanel.Size = new System.Drawing.Size(280, 1246);
            this.OptionsTableLayoutPanel.Controls.Add(this.SortAttributesCheckBox, 0, 0);
            this.OptionsTableLayoutPanel.Controls.Add(this.GenerateInvalidTagsCheckBox, 0, 1);
            this.OptionsTableLayoutPanel.Controls.Add(this.WordWrapCheckBox, 0, 2);

            // 
            // tableLayoutPanel
            // 
            this.TableLayoutPanel.AutoSize = true;
            this.TableLayoutPanel.ColumnCount = 3;
            this.TableLayoutPanel.Size = new System.Drawing.Size(1902, 1246);
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 280));
            this.TableLayoutPanel.Controls.Add(this.OutputTextBox, 1, 1);
            this.TableLayoutPanel.Controls.Add(this.InputTextBox, 1, 0);
            this.TableLayoutPanel.Controls.Add(this.InputLineNumberTextBox, 0, 0);
            this.TableLayoutPanel.Controls.Add(this.OutputLineNumberTextBox, 0, 1);
            this.TableLayoutPanel.Controls.Add(this.OptionsTableLayoutPanel);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 2;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.67897F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.32103F));
            this.TableLayoutPanel.TabIndex = 0;

            var tabArr = new int[15];
            for (var i = 1; i <= tabArr.Length; i++) tabArr[i - 1] = 30 * i;
            // 
            // textBox1
            // 
            this.OutputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputTextBox.Location = new System.Drawing.Point(3, 622);
            this.OutputTextBox.Multiline = true;
            this.OutputTextBox.Name = "OutputTextBox";
            this.OutputTextBox.ReadOnly = true;
            //this.OutputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.OutputTextBox.Size = new System.Drawing.Size(1679, 621);
            this.OutputTextBox.TabIndex = 0;
            //this.OutputTextBox.TextChanged += new System.EventHandler(this.OutputTextBoxTextChanged);
            this.OutputTextBox.Font = new Font(FontFamily.GenericMonospace, 12);
            this.OutputTextBox.SelectionTabs = tabArr;
            this.OutputTextBox.BackColor = Color.BlanchedAlmond;
            this.OutputTextBox.VScroll += this.OutputTextBoxChangeLineNumbers;
            this.OutputTextBox.AutoWordSelection = false;
            this.OutputTextBox.WordWrap = false;
            this.OutputTextBox.ScrollBars = RichTextBoxScrollBars.Both;

            // 
            // textBox2
            // 
            this.InputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InputTextBox.Location = new System.Drawing.Point(3, 3);
            this.InputTextBox.Multiline = true;
            this.InputTextBox.Name = "InputTextBox";
            this.InputTextBox.Size = new System.Drawing.Size(1679, 613);
            this.InputTextBox.TabIndex = 1;
            this.InputTextBox.TextChanged += new System.EventHandler(this.InputTextBoxTextChanged);
            this.InputTextBox.Font = new Font(FontFamily.GenericMonospace, 12);
            this.InputTextBox.SelectionTabs = tabArr;
            this.InputTextBox.BackColor = Color.AliceBlue;
            this.InputTextBox.AcceptsTab = true;
            this.InputTextBox.VScroll += this.InputTextBoxChangeLineNumbers;
            this.InputTextBox.AutoWordSelection = false;
            this.InputTextBox.WordWrap = false;
            this.InputTextBox.ScrollBars = RichTextBoxScrollBars.Both;

            this.InputLineNumberTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InputLineNumberTextBox.Location = new System.Drawing.Point(3, 622);
            this.InputLineNumberTextBox.Multiline = true;
            this.InputLineNumberTextBox.Name = "LineNumberTextBox";
            this.InputLineNumberTextBox.ReadOnly = true;
            this.InputLineNumberTextBox.Size = new System.Drawing.Size(1679, 621);
            this.InputLineNumberTextBox.TabIndex = 0;
            this.InputLineNumberTextBox.Font = new Font(FontFamily.GenericMonospace, 12);
            this.InputLineNumberTextBox.BackColor = Color.AliceBlue;
            this.InputLineNumberTextBox.ScrollBars = RichTextBoxScrollBars.None;
            this.InputLineNumberTextBox.AutoWordSelection = false;

            this.OutputLineNumberTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputLineNumberTextBox.Location = new System.Drawing.Point(3, 622);
            this.OutputLineNumberTextBox.Multiline = true;
            this.OutputLineNumberTextBox.Name = "LineNumberTextBox";
            this.OutputLineNumberTextBox.ReadOnly = true;
            this.OutputLineNumberTextBox.Size = new System.Drawing.Size(1679, 621);
            this.OutputLineNumberTextBox.TabIndex = 0;
            this.OutputLineNumberTextBox.Font = new Font(FontFamily.GenericMonospace, 12);
            this.OutputLineNumberTextBox.BackColor = Color.BlanchedAlmond;
            this.OutputLineNumberTextBox.ScrollBars = RichTextBoxScrollBars.None;
            this.OutputLineNumberTextBox.AutoWordSelection = false;
            // 
            // radioButton1
            // 
            this.SortAttributesCheckBox.AutoSize = true;
            this.SortAttributesCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.SortAttributesCheckBox.Location = new System.Drawing.Point(1688, 622);
            this.SortAttributesCheckBox.Name = "CheckBox";
            this.SortAttributesCheckBox.Size = new System.Drawing.Size(211, 36);
            this.SortAttributesCheckBox.TabIndex = 2;
            this.SortAttributesCheckBox.TabStop = true;
            this.SortAttributesCheckBox.Text = "Sort Attributes";
            this.SortAttributesCheckBox.UseVisualStyleBackColor = true;
            this.SortAttributesCheckBox.CheckedChanged += new System.EventHandler(this.CheckBoxCheckedChanged);

            this.GenerateInvalidTagsCheckBox.AutoSize = true;
            this.GenerateInvalidTagsCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.GenerateInvalidTagsCheckBox.Location = new System.Drawing.Point(1688, 622);
            this.GenerateInvalidTagsCheckBox.Name = "CheckBox";
            this.GenerateInvalidTagsCheckBox.Size = new System.Drawing.Size(211, 36);
            this.GenerateInvalidTagsCheckBox.TabIndex = 3;
            this.GenerateInvalidTagsCheckBox.TabStop = true;
            this.GenerateInvalidTagsCheckBox.Text = "Generate Invalid Tags";
            this.GenerateInvalidTagsCheckBox.UseVisualStyleBackColor = true;
            this.GenerateInvalidTagsCheckBox.CheckedChanged += new System.EventHandler(this.CheckBoxCheckedChanged);

            this.WordWrapCheckBox.AutoSize = true;
            this.WordWrapCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.WordWrapCheckBox.Location = new System.Drawing.Point(1688, 622);
            this.WordWrapCheckBox.Name = "CheckBox";
            this.WordWrapCheckBox.Size = new System.Drawing.Size(211, 36);
            this.WordWrapCheckBox.TabIndex = 3;
            this.WordWrapCheckBox.TabStop = true;
            this.WordWrapCheckBox.Text = "Word Wrap";
            this.WordWrapCheckBox.UseVisualStyleBackColor = true;
            this.WordWrapCheckBox.CheckedChanged += new System.EventHandler(this.CheckBoxCheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1902, 1246);
            this.Controls.Add(this.TableLayoutPanel);
            this.Name = "Form";
            this.Text = "HTML2RazorSharp";
            this.TableLayoutPanel.ResumeLayout(false);
            this.TableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel { get; set; }
        private System.Windows.Forms.TableLayoutPanel OptionsTableLayoutPanel { get; set; }
        private System.Windows.Forms.RichTextBox OutputTextBox { get; set; }
        private RichTextBox InputTextBox { get; set; }
        public RichTextBox InputLineNumberTextBox { get; set; }
        public RichTextBox OutputLineNumberTextBox { get; set; }
        private CheckBox SortAttributesCheckBox { get; set; }
        private CheckBox GenerateInvalidTagsCheckBox { get; set; }
        private CheckBox WordWrapCheckBox { get; set; }
    }
}

