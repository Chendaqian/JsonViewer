using System.Windows.Forms;

namespace Json.Viewer
{
    public partial class JsonObjectVisualizer : UserControl, IJsonVisualizer
    {
        public JsonObjectVisualizer()
        {
            InitializeComponent();
        }

        string IJsonViewerPlugin.DisplayName => "Property Grid";

        Control IJsonVisualizer.GetControl(JsonObject jsonObject)
        {
            return this;
        }

        void IJsonVisualizer.Visualize(JsonObject jsonObject)
        {
            pgJsonObject.SelectedObject = new JsonTreeObjectTypeDescriptor(jsonObject);
        }

        bool IJsonViewerPlugin.CanVisualize(JsonObject jsonObject)
        {
            return true;
        }
    }
}