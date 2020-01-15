using System.Windows.Forms;

namespace JsonVisualizerVSIX
{
    public partial class JsonVisualizerForm : Form
    {
        public JsonVisualizerForm()
        {
            InitializeComponent();
        }

        private void JsonVisualizerForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();

        }
    }
}