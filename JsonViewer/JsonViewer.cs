using Json.Viewer.Properties;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Json.Viewer
{
    public partial class JsonViewer : UserControl
    {
        private string _json;
        private ErrorDetails _errorDetails;
        private readonly PluginsManager _pluginsManager = new PluginsManager();
        private readonly TextEditorManager _textEditorManager = new TextEditorManager();
        private bool _updating;
        private Control _lastVisualizerControl;

        public JsonViewer()
        {
            InitializeComponent();
            try
            {
                _pluginsManager.Initialize();
                _textEditorManager.Initialize(this.txtJson);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format(Resources.ConfigMessage, e.Message), "Json Viewer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string Json
        {
            get => _json;
            set
            {
                if (_json != value)
                {
                    _json = value.Trim();
                    //txtJson.Text = _json;
                    Redraw();
                }
            }
        }

        [DefaultValue(25)]
        public int MaxErrorCount { get; set; } = 25;

        private void Redraw()
        {
            try
            {
                tvJson.BeginUpdate();
                try
                {
                    Reset();
                    if (string.IsNullOrEmpty(_json))
                        return;

                    JsonObjectTree tree = JsonObjectTree.Parse(_json);
                    VisualizeJsonTree(tree);
                }
                finally
                {
                    tvJson.EndUpdate();
                }
            }
            catch (JsonParseError e)
            {
                GetParseErrorDetails(e);
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }

        private void Reset()
        {
            tvJson.Nodes.Clear();
            pnlVisualizer.Controls.Clear();
            _lastVisualizerControl = null;
            cbVisualizers.Items.Clear();
        }

        private void GetParseErrorDetails(Exception parserError)
        {
            UnbufferedStringReader strReader = new UnbufferedStringReader(_json);
            using (JsonReader reader = new JsonTextReader(strReader))
            {
                try
                {
                    while (reader.Read()) { };
                }
                catch (Exception e)
                {
                    _errorDetails._err = e.Message;
                    _errorDetails._pos = strReader.Position;
                }
            }
            if (_errorDetails.Error == null)
                _errorDetails._err = parserError.Message;
            if (_errorDetails.Position == 0)
                _errorDetails._pos = _json.Length;
        }

        private void VisualizeJsonTree(JsonObjectTree tree)
        {
            AddNode(tvJson.Nodes, tree.Root);
            JsonViewerTreeNode node = GetRootNode();
            InitVisualizers(node);
            node.Expand();
            tvJson.SelectedNode = node;
        }

        private void AddNode(TreeNodeCollection nodes, JsonObject jsonObject)
        {
            JsonViewerTreeNode newNode = new JsonViewerTreeNode(jsonObject);
            nodes.Add(newNode);
            newNode.Text = jsonObject.Text;
            newNode.Tag = jsonObject;
            newNode.ImageIndex = (int)jsonObject.JsonType;
            newNode.SelectedImageIndex = newNode.ImageIndex;

            foreach (JsonObject field in jsonObject.Fields)
                AddNode(newNode.Nodes, field);
        }

        public ErrorDetails ErrorDetails => _errorDetails;

        public void Clear()
        {
            Json = string.Empty;
        }

        public bool HasErrors => _errorDetails._err != null;

        private void txtJson_TextChanged(object sender, EventArgs e)
        {
            // https://stackoverflow.com/questions/3476014/icsharpcode-texteditor-vertical-scrolling
            //bool isVisible = this.txtJson.ActiveTextAreaControl.TextArea.Document.TotalNumberOfLines >
            //    this.txtJson.ActiveTextAreaControl.TextArea.TextView.VisibleLineCount;
            //this.txtJson.ActiveTextAreaControl.ShowScrollBars(Orientation.Vertical, isVisible);

            Json = txtJson.Text;
        }

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            txtFind.BackColor = SystemColors.Window;
            FindNext(true, true);
        }

        public bool FindNext(bool includeSelected)
        {
            return FindNext(txtFind.Text, includeSelected);
        }

        public void FindNext(bool includeSelected, bool fromUI)
        {
            if (!FindNext(includeSelected) && fromUI)
                txtFind.BackColor = Color.LightCoral;
        }

        public bool FindNext(string text, bool includeSelected)
        {
            TreeNode startNode = tvJson.SelectedNode;
            if (startNode == null && HasNodes())
                startNode = GetRootNode();
            if (startNode != null)
            {
                startNode = FindNext(startNode, text, includeSelected);
                if (startNode != null)
                {
                    tvJson.SelectedNode = startNode;
                    return true;
                }
            }
            return false;
        }

        public TreeNode FindNext(TreeNode startNode, string text, bool includeSelected)
        {
            if (text == string.Empty)
                return startNode;

            if (includeSelected && IsMatchingNode(startNode, text))
                return startNode;

            TreeNode originalStartNode = startNode;
            startNode = GetNextNode(startNode);
            text = text.ToLower();
            while (startNode != originalStartNode)
            {
                if (IsMatchingNode(startNode, text))
                    return startNode;
                startNode = GetNextNode(startNode);
            }

            return null;
        }

        private TreeNode GetNextNode(TreeNode startNode)
        {
            TreeNode next = startNode.FirstNode ?? startNode.NextNode;
            if (next == null)
            {
                while (startNode != null && next == null)
                {
                    startNode = startNode.Parent;
                    if (startNode != null)
                        next = startNode.NextNode;
                }
                if (next == null)
                {
                    next = GetRootNode();
                    FlashControl(txtFind, Color.Cyan);
                }
            }
            return next;
        }

        private bool IsMatchingNode(TreeNode startNode, string text)
        {
            return startNode.Text.ToLower().Contains(text);
        }

        private JsonViewerTreeNode GetRootNode()
        {
            if (tvJson.Nodes.Count > 0)
                return (JsonViewerTreeNode)tvJson.Nodes[0];
            return null;
        }

        private bool HasNodes()
        {
            return tvJson.Nodes.Count > 0;
        }

        private void txtFind_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    FindNext(false, true);
                    break;

                case Keys.Escape:
                    HideFind();
                    break;
            }
        }

        private void FlashControl(Control control, Color color)
        {
            Color prevColor = control.BackColor;
            try
            {
                control.BackColor = color;
                control.Refresh();
                Thread.Sleep(25);
            }
            finally
            {
                control.BackColor = prevColor;
                control.Refresh();
            }
        }

        public void ShowTab(Tabs tab)
        {
            tabControl.SelectedIndex = (int)tab;
        }

        private void btnFormat_Click(object sender, EventArgs e)
        {
            try
            {
                string json = txtJson.Text;
                JsonSerializer s = new JsonSerializer();
                JsonReader reader = new JsonTextReader(new StringReader(json));
                object jsonObject = s.Deserialize(reader);
                if (jsonObject != null)
                {
                    StringWriter sWriter = new StringWriter();
                    JsonTextWriter writer = new JsonTextWriter(sWriter);
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 4;
                    writer.IndentChar = ' ';
                    s.Serialize(writer, jsonObject);
                    txtJson.Text = sWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        private void ShowException(Exception e)
        {
            MessageBox.Show(this, e.Message, "Json Viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnStripToSqr_Click(object sender, EventArgs e)
        {
            StripTextTo('[', ']');
        }

        private void btnStripToCurly_Click(object sender, EventArgs e)
        {
            StripTextTo('{', '}');
        }

        private void StripTextTo(char sChr, char eChr)
        {
            string text = txtJson.Text;
            int start = text.IndexOf(sChr);
            int end = text.LastIndexOf(eChr);
            int newLen = end - start + 1;
            if (newLen > 1)
            {
                txtJson.Text = text.Substring(start, newLen);
            }
        }

        private void tvJson_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (_pluginsManager.DefaultVisualizer == null)
                return;

            cbVisualizers.BeginUpdate();
            _updating = true;
            try
            {
                JsonViewerTreeNode node = (JsonViewerTreeNode)e.Node;
                IJsonVisualizer lastActive = node.LastVisualizer;
                if (lastActive == null)
                    lastActive = (IJsonVisualizer)cbVisualizers.SelectedItem;
                if (lastActive == null)
                    lastActive = _pluginsManager.DefaultVisualizer;

                cbVisualizers.Items.Clear();
                cbVisualizers.Items.AddRange(node.Visualizers.ToArray());
                int index = cbVisualizers.Items.IndexOf(lastActive);
                if (index != -1)
                {
                    cbVisualizers.SelectedIndex = index;
                }
                else
                {
                    cbVisualizers.SelectedIndex = cbVisualizers.Items.IndexOf(_pluginsManager.DefaultVisualizer);
                }
            }
            finally
            {
                cbVisualizers.EndUpdate();
                _updating = false;
            }
            ActivateVisualizer();
        }

        private void ActivateVisualizer()
        {
            IJsonVisualizer visualizer = (IJsonVisualizer)cbVisualizers.SelectedItem;
            if (visualizer != null)
            {
                JsonObject jsonObject = GetSelectedTreeNode().JsonObject;
                Control visualizerCtrl = visualizer.GetControl(jsonObject);
                if (_lastVisualizerControl != visualizerCtrl)
                {
                    pnlVisualizer.Controls.Remove(_lastVisualizerControl);
                    pnlVisualizer.Controls.Add(visualizerCtrl);
                    visualizerCtrl.Dock = DockStyle.Fill;
                    _lastVisualizerControl = visualizerCtrl;
                }
                visualizer.Visualize(jsonObject);
            }
        }

        private void cbVisualizers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating && GetSelectedTreeNode() != null)
            {
                ActivateVisualizer();
                GetSelectedTreeNode().LastVisualizer = (IJsonVisualizer)cbVisualizers.SelectedItem;
            }
        }

        private JsonViewerTreeNode GetSelectedTreeNode()
        {
            return (JsonViewerTreeNode)tvJson.SelectedNode;
        }

        private void tvJson_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            foreach (JsonViewerTreeNode node in e.Node.Nodes)
            {
                InitVisualizers(node);
            }
        }

        private void InitVisualizers(JsonViewerTreeNode node)
        {
            if (!node.Initialized)
            {
                node.Initialized = true;
                JsonObject jsonObject = node.JsonObject;
                foreach (ICustomTextProvider textVis in _pluginsManager.TextVisualizers)
                {
                    if (textVis.CanVisualize(jsonObject))
                        node.TextVisualizers.Add(textVis);
                }

                node.RefreshText();

                foreach (IJsonVisualizer visualizer in _pluginsManager.Visualizers)
                {
                    if (visualizer.CanVisualize(jsonObject))
                        node.Visualizers.Add(visualizer);
                }
            }
        }

        private void btnCloseFind_Click(object sender, EventArgs e)
        {
            HideFind();
        }

        private void JsonViewer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F && e.Control)
            {
                ShowFind();
            }
        }

        private void HideFind()
        {
            pnlFind.Visible = false;
            tvJson.Focus();
        }

        private void ShowFind()
        {
            pnlFind.Visible = true;
            txtFind.Focus();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFind();
        }

        private void expandallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvJson.BeginUpdate();
            try
            {
                if (tvJson.SelectedNode != null)
                {
                    TreeNode topNode = tvJson.TopNode;
                    tvJson.SelectedNode.ExpandAll();
                    tvJson.TopNode = topNode;
                }
            }
            finally
            {
                tvJson.EndUpdate();
            }
        }

        private void tvJson_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode node = tvJson.GetNodeAt(e.Location);
                if (node != null)
                {
                    tvJson.SelectedNode = node;
                }
            }
        }

        private void rightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender == mnuShowOnBottom)
            {
                spcViewer.Orientation = Orientation.Horizontal;
                mnuShowOnRight.Checked = false;
            }
            else
            {
                spcViewer.Orientation = Orientation.Vertical;
                mnuShowOnBottom.Checked = false;
            }
        }

        private void cbVisualizers_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = ((IJsonViewerPlugin)e.ListItem).DisplayName;
        }

        private void mnuTree_Opening(object sender, CancelEventArgs e)
        {
            mnuFind.Enabled = GetRootNode() != null;
            mnuExpandAll.Enabled = GetSelectedTreeNode() != null;

            mnuCopy.Enabled = mnuExpandAll.Enabled;
            mnuCopyValue.Enabled = mnuExpandAll.Enabled;
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            string text = Clipboard.GetText();
            if (!string.IsNullOrEmpty(text))
                txtJson.Text = text;
        }

        private void mnuCopy_Click(object sender, EventArgs e)
        {
            JsonViewerTreeNode node = GetSelectedTreeNode();
            if (node != null)
            {
                Clipboard.SetText(node.Text);
            }
        }

        private void mnuCopyValue_Click(object sender, EventArgs e)
        {
            JsonViewerTreeNode node = GetSelectedTreeNode();
            if (node != null && node.JsonObject.Value != null)
            {
                Clipboard.SetText(node.JsonObject.Value.ToString());
            }
        }

        private void removeNewLineMenuItem_Click(object sender, EventArgs e)
        {
            StripFromText('\n', '\r');
        }

        private void removeSpecialCharsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = txtJson.Text;
            text = text.Replace(@"\""", @"""");
            txtJson.Text = text;
        }

        private void StripFromText(params char[] chars)
        {
            string text = txtJson.Text;
            foreach (char ch in chars)
            {
                text = text.Replace(ch.ToString(), "");
            }
            txtJson.Text = text;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtJson.Text))
            {
                Clipboard.SetText(txtJson.Text);
            }
        }

        private void escapeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtJson.Text = txtJson.Text.Replace("\"", "\\\"");
        }

        private void undoEscapeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtJson.Text = txtJson.Text.Replace("\\", string.Empty);
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {
            string json = txtJson.Text;
            json = json.Replace(Environment.NewLine, string.Empty).Replace(" ", string.Empty);
            // json = JsonConvert.SerializeObject(json,Formatting.Indented);
            txtJson.Text = json;
            txtJson.ActiveTextAreaControl.TextArea.Refresh();
        }
    }

    public struct ErrorDetails
    {
        internal string _err;
        internal int _pos;

        public string Error => _err;

        public int Position => _pos;

        public void Clear()
        {
            _err = null;
            _pos = 0;
        }
    }

    public class JsonViewerTreeNode : TreeNode
    {
        private readonly JsonObject _jsonObject;
        private readonly List<ICustomTextProvider> _textVisualizers = new List<ICustomTextProvider>();
        private readonly List<IJsonVisualizer> _visualizers = new List<IJsonVisualizer>();
        private bool _init;
        private IJsonVisualizer _lastVisualizer;

        public JsonViewerTreeNode(JsonObject jsonObject)
        {
            _jsonObject = jsonObject;
        }

        public List<ICustomTextProvider> TextVisualizers => _textVisualizers;

        public List<IJsonVisualizer> Visualizers => _visualizers;

        public JsonObject JsonObject => _jsonObject;

        internal bool Initialized
        {
            get => _init;
            set => _init = value;
        }

        internal void RefreshText()
        {
            StringBuilder sb = new StringBuilder(_jsonObject.Text);
            foreach (ICustomTextProvider textVisualizer in _textVisualizers)
            {
                try
                {
                    string customText = textVisualizer.GetText(_jsonObject);
                    sb.Append(" (" + customText + ")");
                }
                catch
                {
                    //silently ignore
                }
            }
            string text = sb.ToString();
            if (text != Text)
                Text = text;
        }

        public IJsonVisualizer LastVisualizer
        {
            get => _lastVisualizer;
            set => _lastVisualizer = value;
        }
    }

    public enum Tabs { Viewer, Text };
}