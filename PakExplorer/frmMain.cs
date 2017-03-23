using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuitarHero.PakExplorer
{
    public partial class frmMain : Form
    {
        private PakArchive openArchive;

        public frmMain() {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialogResult = this.openFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                var oldArchive = this.openArchive;
                try {
                    this.openArchive = PakFile.Open(this.openFileDialog.FileName);
                }
                catch (UnauthorizedAccessException ex) {
                    MessageBox.Show(@"Please run as administrator!");
                }
                oldArchive?.Dispose();
                populateListView();
            }
        }

        private void populateListView()
        {
            this.SuspendLayout();
            this.pakEntryListView.Items.Clear();

            foreach (var entry in openArchive.Entries)
            {
                var filename = entry.FileShortNameKey.Checksum.ToString("X8");
                var offset = entry.FileOffset.ToString("X8");
                var length = entry.FileLength.ToString();
                var filetype = entry.FileType.Checksum.ToString("X8");
                var path = entry.FileFullNameKey.Checksum.ToString("X8");

                var item = new ListViewItem(new[] { filename, offset, length, filetype, path });

                this.pakEntryListView.Items.Add(item);
            }

            this.ResumeLayout();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                components?.Dispose();
                this.openArchive?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
