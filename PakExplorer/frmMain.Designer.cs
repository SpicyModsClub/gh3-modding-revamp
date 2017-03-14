namespace GuitarHero.PakExplorer
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.ColumnHeader columnType;
            System.Windows.Forms.MenuStrip menuStrip;
            System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
            System.Windows.Forms.ColumnHeader columnName;
            System.Windows.Forms.ColumnHeader columnPath;
            System.Windows.Forms.ColumnHeader columnOffset;
            System.Windows.Forms.ColumnHeader columnLength;
            this.pakEntryListView = new System.Windows.Forms.ListView();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            columnType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            menuStrip = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnOffset = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnLength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // columnType
            // 
            columnType.Text = "File type";
            columnType.Width = 95;
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            fileToolStripMenuItem});
            menuStrip.Location = new System.Drawing.Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new System.Drawing.Size(798, 24);
            menuStrip.TabIndex = 1;
            menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            openToolStripMenuItem,
            saveToolStripMenuItem,
            toolStripSeparator1,
            exitToolStripMenuItem});
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            openToolStripMenuItem.Text = "&Open";
            openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            saveToolStripMenuItem.Text = "&Save";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(100, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // columnName
            // 
            columnName.Text = "Filename";
            columnName.Width = 144;
            // 
            // columnPath
            // 
            columnPath.Text = "Path";
            columnPath.Width = 383;
            // 
            // columnOffset
            // 
            columnOffset.Text = "Offset";
            columnOffset.Width = 76;
            // 
            // columnLength
            // 
            columnLength.Text = "Length";
            columnLength.Width = 76;
            // 
            // pakEntryListView
            // 
            this.pakEntryListView.AllowColumnReorder = true;
            this.pakEntryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnName,
            columnOffset,
            columnLength,
            columnType,
            columnPath});
            this.pakEntryListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pakEntryListView.FullRowSelect = true;
            this.pakEntryListView.Location = new System.Drawing.Point(0, 24);
            this.pakEntryListView.Name = "pakEntryListView";
            this.pakEntryListView.Size = new System.Drawing.Size(798, 478);
            this.pakEntryListView.TabIndex = 0;
            this.pakEntryListView.UseCompatibleStateImageBehavior = false;
            this.pakEntryListView.View = System.Windows.Forms.View.Details;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "PAK Archive (*.pak.xen)|*.pak.xen|All Files (*.*)|*.*";
            this.openFileDialog.InitialDirectory = "C:\\Program Files (x86)\\Aspyr\\Guitar Hero III\\DATA\\PAK";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(798, 502);
            this.Controls.Add(this.pakEntryListView);
            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;
            this.Name = "frmMain";
            this.Text = "PAK Explorer";
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView pakEntryListView;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}

