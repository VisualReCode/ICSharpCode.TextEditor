using System.Windows.Forms;

namespace ICSharpCode.TextEditor.Sample
{
    public sealed partial class SampleForm : Form
    {
        public SampleForm()
        {
            InitializeComponent();

            textEditor.Document.TextEditorProperties.EnableFolding = false;
        }
    }
}
