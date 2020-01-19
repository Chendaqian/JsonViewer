using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.TextEditor;

using Json.Viewer;

namespace Json.JsonView
{
    public partial class MainForm : Form
    {
        private string _fileName;

        public MainForm()
        {
            InitializeComponent();
        }

        #region Methods

        private void LoadFromFile(string fileName)
        {
            //string json = File.ReadAllText(fileName);
            JsonViewer.ShowTab(Tabs.Viewer);
            //JsonViewer.Json = json;

            Control c = JsonViewer.Controls.Find("txtJson", true)[0];
            ((TextEditorControlEx)c).LoadFile(fileName);
        }

        private void LoadFromClipboard()
        {
            string json = Clipboard.GetText();
            if (string.IsNullOrEmpty(json))
                return;

            JsonViewer.ShowTab(Tabs.Viewer);
            JsonViewer.Json = json;
        }

        #endregion Methods

        #region EventHandlers

        #region Form

        private void MainForm_Load(object sender, EventArgs e)
        {
            JsonViewer.ShowTab(Tabs.Text);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 1; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg.Equals("/c", StringComparison.OrdinalIgnoreCase))
                    LoadFromClipboard();
                else if (File.Exists(arg))
                    LoadFromFile(arg);
            }
        }

        #endregion Form

        #region MenuItem

        /// <summary>
        /// Open File Dialog  for Yahoo! Pipe files or JSON files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item File > Open</remarks>
        private void tsmiOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = @"Json files (*.json)|*.json|All files (*.*)|*.*",
                InitialDirectory = Application.StartupPath,
                Title = @"Select a JSON file"
            };

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            _fileName = dialog.FileName;
            LoadFromFile(_fileName);
        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = @"Json files (*.json)|*.json|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    _fileName = saveFileDialog.FileName;
                else
                    return;
            }

            Control c = JsonViewer.Controls.Find("txtJson", true)[0];
            ((TextEditorControlEx)c).SaveFile(_fileName);
        }

        private void tsmiRedo_Click(object sender, EventArgs e)
        {
            Control c = JsonViewer.Controls.Find("txtJson", true)[0];
            ((TextEditorControlEx)c).Redo();
        }

        /// <summary>
        /// Undo the last action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Undo</remarks>
        private void tsmiUndo_Click(object sender, EventArgs e)
        {
            Control c = JsonViewer.Controls.Find("txtJson", true)[0];
            ((TextEditorControlEx)c).Undo();
        }

        /// <summary>
        /// Cuts selected text from the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Cut</remarks>
        private void tsmiCut_Click(object sender, EventArgs e)
        {
            Control c = JsonViewer.Controls.Find("txtJson", true)[0];
            TextEditorControl text = (TextEditorControl)c;
            text.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(sender, e);
        }

        /// <summary>
        /// Copies text in the textbox into the clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Copy</remarks>
        private void tsmiCopy_Click(object sender, EventArgs e)
        {
            Control c = JsonViewer.Controls.Find("txtJson", true)[0];
            TextEditorControl text = (TextEditorControl)c;
            text.ActiveTextAreaControl.TextArea.AutoClearSelection = false;
            text.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(sender, e);
        }

        /// <summary>
        /// Pastes text in the clipboard into the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Paste</remarks>
        private void tsmiPaste_Click(object sender, EventArgs e)
        {
            Control c = JsonViewer.Controls.Find("txtJson", true)[0];
            TextEditorControl text = (TextEditorControl)c;
            text.ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(sender, e);
        }

        /// <summary>
        /// Deletes selected text in the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Delete</remarks>
        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            Control c = JsonViewer.Controls.Find("txtJson", true)[0];
            TextEditorControl text = (TextEditorControl)c;
            text.ActiveTextAreaControl.TextArea.AutoClearSelection = false;
            text.ActiveTextAreaControl.TextArea.ClipboardHandler.Delete(sender, e);
        }
        
        /// <summary>
        /// Selects all text in the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Select All</remarks>
        private void tsmiSelectAll_Click(object sender, EventArgs e)
        {
            Control c = JsonViewer.Controls.Find("txtJson", true)[0];
            TextEditorControl text = (TextEditorControl)c;

            text.ActiveTextAreaControl.TextArea.AutoClearSelection = false;
            TextLocation startPoint = new TextLocation(0, 0);
            TextLocation endPoint = text.ActiveTextAreaControl.TextArea.Document.OffsetToPosition(
                text.ActiveTextAreaControl.TextArea.Document.TextLength);

            if (text.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected
                && text.ActiveTextAreaControl.TextArea.SelectionManager.SelectionCollection[0].StartPosition == startPoint
                && text.ActiveTextAreaControl.TextArea.SelectionManager.SelectionCollection[0].EndPosition == endPoint)
                return;

            text.ActiveTextAreaControl.TextArea.Caret.Position = text.ActiveTextAreaControl.TextArea.SelectionManager.NextValidPosition(endPoint.Y);
            text.ActiveTextAreaControl.TextArea.SelectionManager.ExtendSelection(startPoint, endPoint);

            // after a SelectWholeDocument selection, the caret is placed correctly,
            // but it is not positioned internally.  The effect is when the cursor
            // is moved up or down a line, the caret will take on the column that
            // it was in before the SelectWholeDocument
            text.ActiveTextAreaControl.TextArea.SetDesiredColumn();
        }

        /// <summary>
        /// Displays the find prompt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Viewer > Find</remarks>
        private void tsmiFind_Click(object sender, EventArgs e)
        {
            Control c = JsonViewer.Controls.Find("pnlFind", true)[0];
            ((Panel)c).Visible = true;
            Control t = JsonViewer.Controls.Find("txtFind", true)[0];
            ((TextBox)t).Focus();
        }
        
        /// <summary>
        /// Expands all the nodes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Viewer > Expand All</remarks>
        /// <!---->
        private void tsmiExpandAll_Click(object sender, EventArgs e)
        {
            Control c = JsonViewer.Controls.Find("tvJson", true)[0];
            ((TreeView)c).BeginUpdate();

            try
            {
                if (((TreeView) c).SelectedNode == null)
                    return;

                TreeNode topNode = ((TreeView)c).TopNode;
                ((TreeView)c).SelectedNode.ExpandAll();
                ((TreeView)c).TopNode = topNode;
            }
            finally
            {
                ((TreeView)c).EndUpdate();
            }
        }

        /// <summary>
        /// Copies a node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Viewer > Copy</remarks>
        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Control c = JsonViewer.Controls.Find("tvJson", true)[0];
            TreeNode node = ((TreeView)c).SelectedNode;
            if (node != null)
                Clipboard.SetText(node.Text);
        }

        /// <summary>
        /// Copies just the node's value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Viewer > Copy Value</remarks>
        /// <!-- JsonViewerTreeNode had to be made public to be accessible here -->
        private void copyValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c = JsonViewer.Controls.Find("tvJson", true)[0];
            JsonViewerTreeNode node = (JsonViewerTreeNode)((TreeView)c).SelectedNode;
            if (node?.JsonObject.Value != null)
                Clipboard.SetText(node.JsonObject.Value.ToString());
        }

        private void tsmiAbout_Click(object sender, EventArgs e)
        {

        }

        #endregion MenuItem


        private void JsonViewer_Load(object sender, EventArgs e)
        {
        }

        #endregion EventHandlers
    }
}