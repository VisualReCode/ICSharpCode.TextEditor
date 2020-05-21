using System.ComponentModel;

namespace ICSharpCode.TextEditor.Sample
{
    partial class SampleForm
    {
        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.textEditor = new ICSharpCode.TextEditor.TextEditorControl();
            this.SuspendLayout();
            //
            // textEditor
            //
            this.textEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textEditor.IsReadOnly = false;
            this.textEditor.Location = new System.Drawing.Point(0, 0);
            this.textEditor.Name = "textEditor";
            this.textEditor.Size = new System.Drawing.Size(989, 628);
            this.textEditor.TabIndex = 0;
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(989, 628);
            this.Controls.Add(this.textEditor);
            this.Name = "SampleForm";
            this.Text = "ICSharpCode.TextEditor Sample";
            this.ResumeLayout(false);

        }

        #endregion

        private TextEditorControl textEditor;
    }
}
