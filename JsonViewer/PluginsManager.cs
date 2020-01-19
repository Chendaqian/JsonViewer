using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Json.Viewer
{
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    internal class PluginsManager
    {
        private readonly List<IJsonViewerPlugin> plugins = new List<IJsonViewerPlugin>();

        private readonly List<ICustomTextProvider> textVisualizers = new List<ICustomTextProvider>();
        private readonly List<IJsonVisualizer> visualizers = new List<IJsonVisualizer>();

        public IEnumerable<ICustomTextProvider> TextVisualizers => textVisualizers;

        public IEnumerable<IJsonVisualizer> Visualizers => visualizers;

        public IJsonVisualizer DefaultVisualizer { get; private set; }

        public PluginsManager()
        {
        }

        public void Initialize()
        {
            try
            {
                //string myDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                //AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;

                Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
                if (config == null)
                    InitDefaults();
                ViewerConfiguration viewerConfig = (ViewerConfiguration)config.GetSection("jsonViewer");
                InternalConfig(viewerConfig);
            }
            catch
            {
                InitDefaults();
            }
        }

        private void InitDefaults()
        {
            if (DefaultVisualizer == null)
            {
                AddPlugin(new JsonObjectVisualizer());
                AddPlugin(new AjaxNetDateTime());
                AddPlugin(new CustomDate());
            }
        }

        private void InternalConfig(ViewerConfiguration viewerConfig)
        {
            if (viewerConfig == null)
                return;

            foreach (KeyValueConfigurationElement keyValue in viewerConfig.Plugins)
            {
                string type = keyValue.Value;
                Type pluginType = Type.GetType(type, false);
                if (pluginType != null && typeof(IJsonViewerPlugin).IsAssignableFrom(pluginType))
                    continue;
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

        private void AddPlugin(IJsonViewerPlugin plugin)
        {
            plugins.Add(plugin);
            if (plugin is ICustomTextProvider)
                textVisualizers.Add((ICustomTextProvider)plugin);
            if (plugin is IJsonVisualizer)
            {
                if (DefaultVisualizer == null)
                    DefaultVisualizer = (IJsonVisualizer)plugin;
                visualizers.Add((IJsonVisualizer)plugin);
            }
        }
    }
}