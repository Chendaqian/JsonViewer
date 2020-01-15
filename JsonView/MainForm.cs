using Json.Viewer;

using System;
using System.IO;
using System.Windows.Forms;

namespace Json.JsonView
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 1; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg.Equals("/c", StringComparison.OrdinalIgnoreCase))
                {
                    LoadFromClipboard();
                }
                else if (File.Exists(arg))
                {
                    LoadFromFile(arg);
                }
            }
        }

        private void LoadFromFile(string fileName)
        {
            //string json = File.ReadAllText(fileName);
            JsonViewer.ShowTab(Tabs.Viewer);
            //JsonViewer.Json = json;

            Control c;
            c = this.JsonViewer.Controls.Find("txtJson", true)[0];
            ((ICSharpCode.TextEditor.TextEditorControlEx)c).LoadFile(fileName);
        }

        private void LoadFromClipboard()
        {
            string json = Clipboard.GetText();
            if (!String.IsNullOrEmpty(json))
            {
                JsonViewer.ShowTab(Tabs.Viewer);
                JsonViewer.Json = json;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            JsonViewer.ShowTab(Tabs.Text);
        }

        /// <summary>
        /// Closes the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item File > Exit</remarks>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private string fileName = null;

        /// <summary>
        /// Open File Dialog  for Yahoo! Pipe files or JSON files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item File > Open</remarks>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter =
               "Json files (*.json)|*.json|All files (*.*)|*.*";
            dialog.InitialDirectory = Application.StartupPath;
            dialog.Title = "Select a JSON file";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                fileName = dialog.FileName;
                this.LoadFromFile(dialog.FileName);
            }
        }

        /// <summary>
        /// Selects all text in the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Select All</remarks>
        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("txtJson", true)[0];
            ICSharpCode.TextEditor.TextEditorControl text = ((ICSharpCode.TextEditor.TextEditorControl)c);

            text.ActiveTextAreaControl.TextArea.AutoClearSelection = false;
            ICSharpCode.TextEditor.TextLocation startPoint = new ICSharpCode.TextEditor.TextLocation(0, 0);
            ICSharpCode.TextEditor.TextLocation endPoint = text.ActiveTextAreaControl.TextArea.Document.OffsetToPosition(
                text.ActiveTextAreaControl.TextArea.Document.TextLength);
            if (text.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected)
            {
                if (text.ActiveTextAreaControl.TextArea.SelectionManager.SelectionCollection[0].StartPosition == startPoint &&
                    text.ActiveTextAreaControl.TextArea.SelectionManager.SelectionCollection[0].EndPosition == endPoint)
                {
                    return;
                }
            }
            text.ActiveTextAreaControl.TextArea.Caret.Position = text.ActiveTextAreaControl.TextArea.SelectionManager.NextValidPosition(endPoint.Y);
            text.ActiveTextAreaControl.TextArea.SelectionManager.ExtendSelection(startPoint, endPoint);
            // after a SelectWholeDocument selection, the caret is placed correctly,
            // but it is not positioned internally.  The effect is when the cursor
            // is moved up or down a line, the caret will take on the column that
            // it was in before the SelectWholeDocument
            text.ActiveTextAreaControl.TextArea.SetDesiredColumn();
        }

        /// <summary>
        /// Deletes selected text in the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Delete</remarks>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("txtJson", true)[0];
            ICSharpCode.TextEditor.TextEditorControl text = ((ICSharpCode.TextEditor.TextEditorControl)c);
            text.ActiveTextAreaControl.TextArea.AutoClearSelection = false;
            text.ActiveTextAreaControl.TextArea.ClipboardHandler.Delete(sender, e);
        }

        /// <summary>
        /// Pastes text in the clipboard into the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Paste</remarks>
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("txtJson", true)[0];

            ICSharpCode.TextEditor.TextEditorControl text = ((ICSharpCode.TextEditor.TextEditorControl)c);

            text.ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(sender, e);
        }

        /// <summary>
        /// Copies text in the textbox into the clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Copy</remarks>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("txtJson", true)[0];
            ICSharpCode.TextEditor.TextEditorControl text = ((ICSharpCode.TextEditor.TextEditorControl)c);
            text.ActiveTextAreaControl.TextArea.AutoClearSelection = false;
            text.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(sender, e);
        }

        /// <summary>
        /// Cuts selected text from the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Cut</remarks>
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("txtJson", true)[0];
            ICSharpCode.TextEditor.TextEditorControl text = ((ICSharpCode.TextEditor.TextEditorControl)c);
            text.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(sender, e);
        }

        /// <summary>
        /// Undo the last action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Undo</remarks>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("txtJson", true)[0];
            ((ICSharpCode.TextEditor.TextEditorControlEx)c).Undo();
        }

        /// <summary>
        /// Displays the find prompt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Viewer > Find</remarks>
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("pnlFind", true)[0];
            ((Panel)c).Visible = true;
            Control t;
            t = this.JsonViewer.Controls.Find("txtFind", true)[0];
            ((TextBox)t).Focus();
        }

        /// <summary>
        /// Expands all the nodes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Viewer > Expand All</remarks>
        /// <!---->
        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("tvJson", true)[0];
            ((TreeView)c).BeginUpdate();
            try
            {
                if (((TreeView)c).SelectedNode != null)
                {
                    TreeNode topNode = ((TreeView)c).TopNode;
                    ((TreeView)c).SelectedNode.ExpandAll();
                    ((TreeView)c).TopNode = topNode;
                }
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
            Control c;
            c = this.JsonViewer.Controls.Find("tvJson", true)[0];
            TreeNode node = ((TreeView)c).SelectedNode;
            if (node != null)
            {
                Clipboard.SetText(node.Text);
            }
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
            Control c;
            c = this.JsonViewer.Controls.Find("tvJson", true)[0];
            JsonViewerTreeNode node = (JsonViewerTreeNode)((TreeView)c).SelectedNode;
            if (node != null && node.JsonObject.Value != null)
            {
                Clipboard.SetText(node.JsonObject.Value.ToString());
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("txtJson", true)[0];
            ((ICSharpCode.TextEditor.TextEditorControlEx)c).Redo();
        }

        private void JsonViewer_Load(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Json files (*.json)|*.json|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = saveFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }

            Control c;
            c = this.JsonViewer.Controls.Find("txtJson", true)[0];
            ((ICSharpCode.TextEditor.TextEditorControlEx)c).SaveFile(fileName);
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData) //¼¤»î»Ø³µ¼ü
        {
            int WM_KEYDOWN = 256;
            int WM_SYSKEYDOWN = 260;
            if (msg.Msg == WM_KEYDOWN | msg.Msg == WM_SYSKEYDOWN)
            {
                switch (keyData)
                {
                    case Keys.Escape:
                        this.Close();
                        break;
                }
            }
            return false;
        }
    }
}