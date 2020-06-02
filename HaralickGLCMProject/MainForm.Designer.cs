namespace HaralickGLCMProject
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.haralickGLCMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bulkExtractGLCMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.haralickGLCMToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(383, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // haralickGLCMToolStripMenuItem
            // 
            this.haralickGLCMToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bulkExtractGLCMToolStripMenuItem});
            this.haralickGLCMToolStripMenuItem.Name = "haralickGLCMToolStripMenuItem";
            this.haralickGLCMToolStripMenuItem.Size = new System.Drawing.Size(117, 24);
            this.haralickGLCMToolStripMenuItem.Text = "Haralick GLCM";
            // 
            // bulkExtractGLCMToolStripMenuItem
            // 
            this.bulkExtractGLCMToolStripMenuItem.Name = "bulkExtractGLCMToolStripMenuItem";
            this.bulkExtractGLCMToolStripMenuItem.Size = new System.Drawing.Size(206, 24);
            this.bulkExtractGLCMToolStripMenuItem.Text = "Bulk Extract GLCM...";
            this.bulkExtractGLCMToolStripMenuItem.Click += new System.EventHandler(this.bulkExtractGLCMToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(383, 167);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Haralick GLCM";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem haralickGLCMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bulkExtractGLCMToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog fbd;
    }
}

