using Json.JsonVisualizerVSIX;


using Microsoft.VisualBasic;
using Microsoft.VisualStudio.DebuggerVisualizers;

using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using static Json.JsonVisualizerVSIX.JsonVisualizer;


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
//[assembly: DebuggerVisualizer(typeof(JsonVisualizer), Target = typeof(string), Description = "String Visualizer")]

#endregion assembly

namespace Json.JsonVisualizerVSIX
{
    public class JsonVisualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            using (JsonVisualizerForm form = new JsonVisualizerForm())
            {
      
                form.Viewer.txtJson.Text = objectProvider.GetObject().ToString();
                //form.Viewer.Json = objectProvider.GetObject().ToString();
                windowService.ShowDialog(form);
            }
        }

        public class ObjectSource : VisualizerObjectSource
        {
            public override void GetData(object target, Stream outgoingData)
            {
                DataTable dtData = null;
                Type type = target.GetType();

                switch (target)
                {
                    case NameValueCollection _:
                        dtData = PopulateCollectionInfo((NameValueCollection)target);
                        break;

                    case System.Web.HttpCookieCollection _:
                        dtData = PopulateCollectionInfo((System.Web.HttpCookieCollection)target);
                        break;

                    case NameObjectCollectionBase _:
                        dtData = PopulateCollectionInfo((NameObjectCollectionBase)target);
                        break;

                    case BitArray _:
                        dtData = PopulateCollectionInfo((BitArray)target);
                        break;

                    case Queue _:
                        dtData = PopulateCollectionInfo((Queue)target);
                        break;

                    case Stack _:
                        dtData = PopulateCollectionInfo((Stack)target);
                        break;

                    case IDictionary _:
                        dtData = PopulateCollectionInfo((IDictionary)target);
                        break;

                    case IList _:
                        dtData = PopulateCollectionInfo((IList)target);
                        break;

                    case ICollection _:
                        dtData = PopulateCollectionInfo((ICollection)target);
                        break;

                    case IEnumerable _:
                        dtData = PopulateCollectionInfo((IEnumerable)target);
                        break;

                    default:
                        MessageBox.Show("throw");
                        throw new ApplicationException("Unknown object type: " + (target));
                }

                dtData.TableName = Information.TypeName(target);

                string json = JsonConvert.SerializeObject(target);
                System.IO.File.WriteAllText("c:\\json.log", json.ToString());

                dtData.Rows.Add(dtData.NewRow()["Value"] = json);
                dtData.AcceptChanges();
                System.IO.File.WriteAllText("c:\\dtData.log", JsonConvert.SerializeObject(dtData));

                VisualizerObjectSource.Serialize(outgoingData, json);
            }

            private DataTable PopulateCollectionInfo(NameValueCollection coll)
            {
                DataTable dtData = InitDataTable(2);
                DataRow dr = null;
                int i = 0;

                for (i = 0; i <= coll.Count - 1; i++)
                {
                    dr = dtData.NewRow();
                    dtData.Rows.Add(dr);
                    dr["Name"] = coll.Keys[i];
                    dr["Value"] = coll[i];
                }

                return dtData;
            }

            private DataTable PopulateCollectionInfo(System.Web.HttpCookieCollection coll)
            {
                DataTable dtData = InitDataTable(3);
                DataRow dr = null;
                int i = 0;
                System.Web.HttpCookie cookie = null;

                for (i = 0; i <= coll.Count - 1; i++)
                {
                    dr = dtData.NewRow();
                    dtData.Rows.Add(dr);
                    cookie = coll[i];
                    dr["Name"] = cookie.Name;
                    dr["Value"] = cookie.Value;
                    if (cookie.Expires > DateTime.MinValue)
                        dr["Expires"] = cookie.Expires.ToString("yyyy-MM-dd HH:mm:ss");
                    else
                        dr["Expires"] = "";
                }

                return dtData;
            }

            private DataTable PopulateCollectionInfo(NameObjectCollectionBase coll)
            {
                DataTable dtData = InitDataTable(2);
                DataRow dr = null;
                int i = 0;
                System.Reflection.MemberInfo[] members = null;
                Array keys = null;
                object[] keysArray = null;
                Array values = null;
                object[] valuesArray = null;
                object valueItem = null;
                string thisValue = null;

                members = coll.GetType().FindMembers(System.Reflection.MemberTypes.Method, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, Type.FilterName, "BaseGetAllValues");
                if (members.Length == 0)
                    throw new ApplicationException("Unable to retrieve values from NameObjectCollectionBase");

                System.Reflection.MethodInfo mi = (System.Reflection.MethodInfo)members[0];
                values = (Array)mi.Invoke(coll, null);
                valuesArray = (object[])values;

                members = coll.GetType().FindMembers(System.Reflection.MemberTypes.Method, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, Type.FilterName, "BaseGetAllKeys");
                if (members.Length == 0)
                    throw new ApplicationException("Unable to retrieve keys from NameObjectCollectionBase");

                mi = (System.Reflection.MethodInfo)members[0];
                keys = (Array)mi.Invoke(coll, null);
                keysArray = (object[])keys;

                for (i = 0; i <= Information.UBound(keysArray); i++)
                {
                    dr = dtData.NewRow();
                    dtData.Rows.Add(dr);
                    dr["Name"] = GetObjectValue(keysArray[i]);

                    if (valuesArray[i] is ArrayList)
                    {
                        foreach (object valueItem_loopVariable in (ArrayList)valuesArray[i])
                        {
                            valueItem = valueItem_loopVariable;
                            if (Strings.Len(thisValue) > 0)
                                thisValue += ",";
                            thisValue += GetObjectValue(valueItem);
                        }
                    }
                    else if (valuesArray[i] is System.Web.HttpCookie)
                        thisValue = ((System.Web.HttpCookie)valuesArray[i]).Value;
                    else
                        thisValue = GetObjectValue(valuesArray[i]);

                    dr["Value"] = thisValue;
                }

                return dtData;
            }

            private DataTable PopulateCollectionInfo(BitArray coll)
            {
                DataTable dtData = InitDataTable(2);
                DataRow dr = null;
                int i = 0;

                dtData.Columns["Name"].Caption = "Index";

                for (i = 0; i <= coll.Count - 1; i++)
                {
                    dr = dtData.NewRow();
                    dtData.Rows.Add(dr);
                    dr["Name"] = i;
                    dr["Value"] = GetObjectValue(coll[i]);
                }

                return dtData;
            }

            private DataTable PopulateCollectionInfo(Queue coll)
            {
                DataTable dtData = InitDataTable(2);
                DataRow dr = null;
                int i = 0;
                object[] values = null;

                dtData.Columns["Name"].Caption = "Position";

                values = coll.ToArray();

                for (i = 0; i <= coll.Count - 1; i++)
                {
                    dr = dtData.NewRow();
                    dtData.Rows.Add(dr);
                    dr["Name"] = i;
                    dr["Value"] = GetObjectValue(values[i]);
                }

                return dtData;
            }

            private DataTable PopulateCollectionInfo(Stack coll)
            {
                DataTable dtData = InitDataTable(2);
                DataRow dr = null;
                int i = 0;
                object[] values = null;

                dtData.Columns["Name"].Caption = "Position";

                values = coll.ToArray();

                for (i = 0; i <= coll.Count - 1; i++)
                {
                    dr = dtData.NewRow();
                    dtData.Rows.Add(dr);
                    dr["Name"] = i;
                    dr["Value"] = GetObjectValue(values[i]);
                }

                return dtData;
            }

            private DataTable PopulateCollectionInfo(IDictionary coll)
            {
                DataTable dtData = InitDataTable(2);
                DataRow dr = null;
                int i = 0;
                object[] keys = new object[coll.Count];
                object[] values = new object[coll.Count];

                coll.Keys.CopyTo(keys, 0);
                coll.Values.CopyTo(values, 0);

                for (i = 0; i <= coll.Count - 1; i++)
                {
                    dr = dtData.NewRow();
                    dtData.Rows.Add(dr);
                    dr["Name"] = keys[i].ToString();
                    dr["Value"] = values[i].ToString();
                }

                return dtData;
            }

            private DataTable PopulateCollectionInfo(IList coll)
            {
                DataTable dtData = InitDataTable(1);
                DataRow dr = null;
                int i = 0;

                for (i = 0; i <= coll.Count - 1; i++)
                {
                    dr = dtData.NewRow();
                    dtData.Rows.Add(dr);
                    dr["Value"] = GetObjectValue(coll[i]);
                }

                return dtData;
            }

            private DataTable PopulateCollectionInfo(ICollection coll)
            {
                DataTable dtData = InitDataTable(1);
                DataRow dr = null;
                int i = 0;
                object[] values = new object[coll.Count];

                coll.CopyTo(values, 0);

                for (i = 0; i <= coll.Count - 1; i++)
                {
                    dr = dtData.NewRow();
                    dtData.Rows.Add(dr);
                    dr["Value"] = values[i].ToString();
                }

                return dtData;
            }

            private DataTable PopulateCollectionInfo(IEnumerable coll)
            {
                DataTable dtData = InitDataTable(2);
                DataRow dr = null;
                DictionaryEntry d = default(DictionaryEntry);

                foreach (DictionaryEntry d_loopVariable in coll)
                {
                    d = d_loopVariable;
                    dr = dtData.NewRow();
                    dtData.Rows.Add(dr);
                    dr["Name"] = GetObjectValue(d.Key);
                    dr["Value"] = GetObjectValue(d.Value);
                }

                return dtData;
            }

            private string GetObjectValue(object Obj)
            {
                if (Obj == null)
                    return null;

                return Obj.ToString();
            }

            private DataTable InitDataTable(int Columns)
            {
                DataTable dtData = new DataTable();

                var _with1 = dtData;
                if (Columns >= 2)
                    _with1.Columns.Add("Name");

                _with1.Columns.Add("Value");

                if (Columns >= 3)
                    _with1.Columns.Add("Expires");

                return dtData;
            }
        }
    }
}