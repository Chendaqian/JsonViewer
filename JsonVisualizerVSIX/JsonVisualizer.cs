using JsonVisualizerVSIX;

using Microsoft.VisualStudio.DebuggerVisualizers;

using Newtonsoft.Json;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;

using static JsonVisualizerVSIX.JsonVisualizer;

#region assembly

//Collections classes
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(ArrayList), Description = "ArrayList Visualizer")]
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(BitArray), Description = "BitArray Visualizer")]
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(Hashtable), Description = "Hashtable Visualizer")]
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(Queue), Description = "Queue Visualizer")]
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(SortedList), Description = "SortedList Visualizer")]
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(Stack), Description = "Stack Visualizer")]
//Collections.Specialized classes
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(HybridDictionary), Description = "HybridDictionary Visualizer")]
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(ListDictionary), Description = "ListDictionary Visualizer")]
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(NameObjectCollectionBase), Description = "NameObjectCollectionBase Visualizer")]
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(OrderedDictionary), Description = "OrderedDictionary Visualizer")]
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(StringCollection), Description = "StringCollection Visualizer")]
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(StringDictionary), Description = "StringDictionary Visualizer")]
//Collections.Generic classes
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(Dictionary<,>), Description = "Dictionary Visualizer")]
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(List<>), Description = "List Visualizer")]
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(LinkedList<>), Description = "LinkedList Visualizer")]
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(Queue<>), Description = "Queue Visualizer")]
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(SortedDictionary<,>), Description = "SortedDictionary Visualizer")]
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(SortedList<,>), Description = "SortedList Visualizer")]
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(Stack<>), Description = "Stack Visualizer")]
//Other classes
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), typeof(ObjectSource), Target = typeof(CollectionBase), Description = "Collection Visualizer")]
[assembly: DebuggerVisualizer(typeof(JsonVisualizer), Target = typeof(string), Description = "String Visualizer")]

#endregion assembly

namespace JsonVisualizerVSIX
{
    public class JsonVisualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            using (JsonVisualizerForm form = new JsonVisualizerForm())
            {
                form.jsonViewer.txtJson.Text = objectProvider.GetObject().ToString();
                //form.Viewer.Json = objectProvider.GetObject().ToString();
                windowService.ShowDialog(form);
            }
        }

        public class ObjectSource : VisualizerObjectSource
        {
            public override void GetData(object target, Stream outgoingData)
            {
                string json = JsonConvert.SerializeObject(target);
                VisualizerObjectSource.Serialize(outgoingData, json);
            }
        }
    }
}