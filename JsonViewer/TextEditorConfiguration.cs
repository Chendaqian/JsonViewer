using System.Configuration;

namespace Json.Viewer
{
    public class TextEditorConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("add")]
        public KeyValueConfigurationCollection Plugins => (KeyValueConfigurationCollection)base["add"];
    }
}