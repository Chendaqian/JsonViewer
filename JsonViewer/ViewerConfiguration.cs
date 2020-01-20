using System.Configuration;

namespace Json.Viewer
{
    public class ViewerConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("plugins")]
        public KeyValueConfigurationCollection Plugins => (KeyValueConfigurationCollection)base["plugins"];

        [ConfigurationProperty("textEditor")]
        public KeyValueConfigurationCollection TextEditor => (KeyValueConfigurationCollection)base["textEditor"];
    }
}