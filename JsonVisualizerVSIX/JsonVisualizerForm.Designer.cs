namespace JsonVisualizerVSIX
{
    partial class JsonVisualizerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JsonVisualizerForm));
            this.jsonViewer = new Json.Viewer.JsonViewer();
            this.SuspendLayout();
            // 
            // jsonViewer
            // 
            this.jsonViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.jsonViewer.Json = null;
            this.jsonViewer.Location = new System.Drawing.Point(0, 0);
            this.jsonViewer.Name = "jsonViewer";
            this.jsonViewer.Size = new System.Drawing.Size(875, 570);
            this.jsonViewer.TabIndex = 0;
            // 
            // JsonVisualizerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 570);
            this.Controls.Add(this.jsonViewer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "JsonVisualizerForm";
            this.Text = "JsonVisualizerForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JsonVisualizerForm_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        public global::Json.Viewer.JsonViewer jsonViewer;
    }
}