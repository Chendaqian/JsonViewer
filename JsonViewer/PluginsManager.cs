using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Json.Viewer
{
    internal class PluginsManager
    {
        private readonly List<IJsonViewerPlugin> plugins = new List<IJsonViewerPlugin>();

        private readonly List<ICustomTextProvider> textVisualizers = new List<ICustomTextProvider>();
        private readonly List<IJsonVisualizer> visualizers = new List<IJsonVisualizer>();
        private IJsonVisualizer _defaultVisualizer;

        public PluginsManager()
        {
        }

        public void Initialize()
        {
            try
            {
                string myDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                //AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;

                Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
                //if (config == null)
                InitDefaults();
                //ViewerConfiguration viewerConfig = (ViewerConfiguration)config.GetSection("jsonViewer");
                //InternalConfig(viewerConfig);
            }
            catch
            {
                InitDefaults();
            }
        }

        private void InitDefaults()
        {
            if (_defaultVisualizer == null)
            {
                AddPlugin(new JsonObjectVisualizer());
                AddPlugin(new AjaxNetDateTime());
                AddPlugin(new CustomDate());
            }
        }

        private void InternalConfig(ViewerConfiguration viewerConfig)
        {
            if (viewerConfig != null)
            {
                foreach (KeyValueConfigurationElement keyValue in viewerConfig.Plugins)
                {
                    string type = keyValue.Value;
                    Type pluginType = Type.GetType(type, false);
                    if (pluginType != null && typeof(IJsonViewerPlugin).IsAssignableFrom(pluginType))
                    {
                        try
                        {
                            IJsonViewerPlugin plugin = (IJsonViewerPlugin)Activator.CreateInstance(pluginType);
                            AddPlugin(plugin);
                        }
                        catch
                        {
                            //Silently ignore any errors in plugin creation
                        }
                    }
                }
            }
        }

        private void AddPlugin(IJsonViewerPlugin plugin)
        {
            plugins.Add(plugin);
            if (plugin is ICustomTextProvider)
                textVisualizers.Add((ICustomTextProvider)plugin);
            if (plugin is IJsonVisualizer)
            {
                if (_defaultVisualizer == null)
                    _defaultVisualizer = (IJsonVisualizer)plugin;
                visualizers.Add((IJsonVisualizer)plugin);
            }
        }

        public IEnumerable<ICustomTextProvider> TextVisualizers => textVisualizers;

        public IEnumerable<IJsonVisualizer> Visualizers => visualizers;

        public IJsonVisualizer DefaultVisualizer => _defaultVisualizer;
    }
}