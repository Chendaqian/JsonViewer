using ICSharpCode.TextEditor;

using System.Configuration;
using System.Reflection;

namespace Json.Viewer
{
    public class TextEditorManager
    {
        public void Initialize(TextEditorControlEx ctrol)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            if (config == null)
                InitDefaults(ctrol);

            InitControl(ctrol, config);
        }

        private void InitDefaults(TextEditorControlEx ctrol)
        {
            ctrol.ShowHRuler = true;
            ctrol.ShowInvalidLines = true;
            ctrol.SyntaxHighlighting = "JavaScript";
        }

        private void InitControl(TextEditorControlEx ctrol, Configuration config)
        {
            ViewerConfiguration viewerConfig = (ViewerConfiguration)config.GetSection("jsonViewer");
            foreach (KeyValueConfigurationElement item in viewerConfig.TextEditor)
            {
                switch (item.Key)
                {
                    case "SyntaxHighlighting":
                        ctrol.SyntaxHighlighting = item.Value;
                        break;

                    case "ShowInvalidLines":
                        ctrol.ShowInvalidLines = bool.TryParse(item.Value, out bool lines) ? lines : true;
                        break;

                    case "ShowHRuler":
                        ctrol.ShowHRuler = bool.TryParse(item.Value, out bool ruler) ? ruler : true;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}