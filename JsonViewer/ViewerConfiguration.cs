using System.Configuration;

namespace Json.Viewer
{
    public class ViewerConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("plugins")]
        public KeyValueConfigurationCollection Plugins
        {
            get
            {
                return (KeyValueConfigurationCollection)base["plugins"];
            }
        }
    }
}