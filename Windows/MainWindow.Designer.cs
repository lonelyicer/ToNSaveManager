namespace ToNSaveManager
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            listBoxKeys = new System.Windows.Forms.ListBox();
            listBoxEntries = new System.Windows.Forms.ListBox();
            ctxMenuEntries = new System.Windows.Forms.ContextMenuStrip(components);
            ctxMenuEntriesCopyTo = new System.Windows.Forms.ToolStripMenuItem();
            ctxMenuEntriesNew = new System.Windows.Forms.ToolStripMenuItem();
            ctxMenuEntriesNote = new System.Windows.Forms.ToolStripMenuItem();
            ctxMenuEntriesBackup = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            ctxMenuEntriesDelete = new System.Windows.Forms.ToolStripMenuItem();
            ctxMenuKeys = new System.Windows.Forms.ContextMenuStrip(components);
            importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            btnSettings = new System.Windows.Forms.Button();
            btnObjectives = new System.Windows.Forms.Button();
            linkWiki = new System.Windows.Forms.Button();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            btnStats = new System.Windows.Forms.Button();
            linkSupport = new System.Windows.Forms.Button();
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            ctxMenuEntries.SuspendLayout();
            ctxMenuKeys.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView).BeginInit();
            SuspendLayout();
            // 
            // listBoxKeys
            // 
            listBoxKeys.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            listBoxKeys.BackColor = System.Drawing.Color.FromArgb(((int)((byte)59)), ((int)((byte)66)), ((int)((byte)82)));
            listBoxKeys.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            listBoxKeys.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            listBoxKeys.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)236)), ((int)((byte)239)), ((int)((byte)244)));
            listBoxKeys.FormattingEnabled = true;
            listBoxKeys.IntegralHeight = false;
            listBoxKeys.ItemHeight = 15;
            listBoxKeys.Location = new System.Drawing.Point(0, 0);
            listBoxKeys.Name = "listBoxKeys";
            listBoxKeys.Size = new System.Drawing.Size(267, 507);
            listBoxKeys.TabIndex = 0;
            listBoxKeys.TabStop = false;
            listBoxKeys.DrawItem += listBoxEntries_DrawItem;
            listBoxKeys.KeyDown += listBoxKeys_KeyDown;
            listBoxKeys.MouseDown += listBoxKeys_MouseDown;
            listBoxKeys.MouseUp += listBoxKeys_MouseUp;
            // 
            // listBoxEntries
            // 
            listBoxEntries.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            listBoxEntries.BackColor = System.Drawing.Color.FromArgb(((int)((byte)59)), ((int)((byte)66)), ((int)((byte)82)));
            listBoxEntries.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            listBoxEntries.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            listBoxEntries.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)236)), ((int)((byte)239)), ((int)((byte)244)));
            listBoxEntries.FormattingEnabled = true;
            listBoxEntries.IntegralHeight = false;
            listBoxEntries.ItemHeight = 15;
            listBoxEntries.Location = new System.Drawing.Point(0, 0);
            listBoxEntries.Name = "listBoxEntries";
            listBoxEntries.Size = new System.Drawing.Size(489, 507);
            listBoxEntries.TabIndex = 1;
            listBoxEntries.TabStop = false;
            listBoxEntries.DrawItem += listBoxEntries_DrawItem;
            listBoxEntries.MouseDown += listBoxEntries_MouseDown;
            listBoxEntries.MouseLeave += listBoxEntries_MouseLeave;
            listBoxEntries.MouseMove += listBoxEntries_MouseMove;
            listBoxEntries.MouseUp += listBoxEntries_MouseUp;
            listBoxEntries.Resize += listBoxEntries_Resize;
            // 
            // ctxMenuEntries
            // 
            ctxMenuEntries.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { ctxMenuEntriesCopyTo, ctxMenuEntriesNote, ctxMenuEntriesBackup, toolStripMenuItem2, ctxMenuEntriesDelete });
            ctxMenuEntries.Name = "ctxMenuEntries";
            ctxMenuEntries.Size = new System.Drawing.Size(132, 98);
            ctxMenuEntries.Closed += ctxMenuEntries_Closed;
            ctxMenuEntries.Opened += ctxMenuEntries_Opened;
            // 
            // ctxMenuEntriesCopyTo
            // 
            ctxMenuEntriesCopyTo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { ctxMenuEntriesNew });
            ctxMenuEntriesCopyTo.Name = "ctxMenuEntriesCopyTo";
            ctxMenuEntriesCopyTo.Size = new System.Drawing.Size(131, 22);
            ctxMenuEntriesCopyTo.Text = "Add to";
            // 
            // ctxMenuEntriesNew
            // 
            ctxMenuEntriesNew.Name = "ctxMenuEntriesNew";
            ctxMenuEntriesNew.Size = new System.Drawing.Size(163, 22);
            ctxMenuEntriesNew.Text = "New Collection";
            ctxMenuEntriesNew.ToolTipText = "Add this entry to a new collection.";
            ctxMenuEntriesNew.Click += ctxMenuEntriesNew_Click;
            // 
            // ctxMenuEntriesNote
            // 
            ctxMenuEntriesNote.Name = "ctxMenuEntriesNote";
            ctxMenuEntriesNote.Size = new System.Drawing.Size(131, 22);
            ctxMenuEntriesNote.Text = "Edit Note";
            ctxMenuEntriesNote.Click += ctxMenuEntriesNote_Click;
            // 
            // ctxMenuEntriesBackup
            // 
            ctxMenuEntriesBackup.Enabled = false;
            ctxMenuEntriesBackup.Name = "ctxMenuEntriesBackup";
            ctxMenuEntriesBackup.Size = new System.Drawing.Size(131, 22);
            ctxMenuEntriesBackup.Text = "Backup";
            ctxMenuEntriesBackup.ToolTipText = ("Force upload a backup of this code to Discord as a file, requires Auto Discord Ba" + "ckup to be enabled in settings.");
            ctxMenuEntriesBackup.Click += ctxMenuEntriesBackup_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(128, 6);
            // 
            // ctxMenuEntriesDelete
            // 
            ctxMenuEntriesDelete.Name = "ctxMenuEntriesDelete";
            ctxMenuEntriesDelete.Size = new System.Drawing.Size(131, 22);
            ctxMenuEntriesDelete.Text = "Delete";
            ctxMenuEntriesDelete.Click += ctxMenuEntriesDelete_Click;
            // 
            // ctxMenuKeys
            // 
            ctxMenuKeys.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { importToolStripMenuItem, renameToolStripMenuItem, toolStripMenuItem1, deleteToolStripMenuItem });
            ctxMenuKeys.Name = "ctxMenuKeys";
            ctxMenuKeys.Size = new System.Drawing.Size(124, 76);
            // 
            // importToolStripMenuItem
            // 
            importToolStripMenuItem.Name = "importToolStripMenuItem";
            importToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            importToolStripMenuItem.Text = "Import";
            importToolStripMenuItem.Click += ctxMenuKeysImport_Click;
            // 
            // renameToolStripMenuItem
            // 
            renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            renameToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            renameToolStripMenuItem.Text = "Rename";
            renameToolStripMenuItem.Click += ctxMenuKeysRename_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(120, 6);
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            deleteToolStripMenuItem.Text = "Delete";
            deleteToolStripMenuItem.Click += ctxMenuKeysDelete_Click;
            // 
            // btnSettings
            // 
            btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            btnSettings.BackColor = System.Drawing.Color.FromArgb(((int)((byte)46)), ((int)((byte)52)), ((int)((byte)64)));
            btnSettings.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)((byte)122)), ((int)((byte)122)), ((int)((byte)122)));
            btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnSettings.ForeColor = System.Drawing.Color.White;
            btnSettings.Location = new System.Drawing.Point(0, 513);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new System.Drawing.Size(267, 24);
            btnSettings.TabIndex = 0;
            btnSettings.TabStop = false;
            btnSettings.Text = "Settings";
            btnSettings.UseVisualStyleBackColor = false;
            btnSettings.Click += btnSettings_Click;
            // 
            // btnObjectives
            // 
            btnObjectives.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            btnObjectives.BackColor = System.Drawing.Color.FromArgb(((int)((byte)46)), ((int)((byte)52)), ((int)((byte)64)));
            btnObjectives.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)((byte)122)), ((int)((byte)122)), ((int)((byte)122)));
            btnObjectives.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnObjectives.ForeColor = System.Drawing.Color.White;
            btnObjectives.Location = new System.Drawing.Point(0, 513);
            btnObjectives.Name = "btnObjectives";
            btnObjectives.Size = new System.Drawing.Size(331, 24);
            btnObjectives.TabIndex = 0;
            btnObjectives.TabStop = false;
            btnObjectives.Text = "Objectives";
            btnObjectives.UseVisualStyleBackColor = false;
            btnObjectives.Click += btnObjectives_Click;
            // 
            // linkWiki
            // 
            linkWiki.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            linkWiki.BackColor = System.Drawing.Color.FromArgb(((int)((byte)46)), ((int)((byte)52)), ((int)((byte)64)));
            linkWiki.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)((byte)122)), ((int)((byte)122)), ((int)((byte)122)));
            linkWiki.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            linkWiki.ForeColor = System.Drawing.Color.White;
            linkWiki.Location = new System.Drawing.Point(401, 513);
            linkWiki.Name = "linkWiki";
            linkWiki.Size = new System.Drawing.Size(58, 24);
            linkWiki.TabIndex = 3;
            linkWiki.TabStop = false;
            linkWiki.Text = "Wiki";
            linkWiki.UseVisualStyleBackColor = false;
            linkWiki.Click += linkWiki_Clicked;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            splitContainer1.Location = new System.Drawing.Point(12, 12);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(btnSettings);
            splitContainer1.Panel1.Controls.Add(listBoxKeys);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(btnStats);
            splitContainer1.Panel2.Controls.Add(linkSupport);
            splitContainer1.Panel2.Controls.Add(listBoxEntries);
            splitContainer1.Panel2.Controls.Add(btnObjectives);
            splitContainer1.Panel2.Controls.Add(linkWiki);
            splitContainer1.Size = new System.Drawing.Size(760, 537);
            splitContainer1.SplitterDistance = 267;
            splitContainer1.TabIndex = 0;
            splitContainer1.TabStop = false;
            splitContainer1.SplitterMoved += splitContainer1_SplitterMoved;
            // 
            // btnStats
            // 
            btnStats.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            btnStats.BackColor = System.Drawing.Color.FromArgb(((int)((byte)46)), ((int)((byte)52)), ((int)((byte)64)));
            btnStats.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)((byte)122)), ((int)((byte)122)), ((int)((byte)122)));
            btnStats.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnStats.ForeColor = System.Drawing.Color.White;
            btnStats.Location = new System.Drawing.Point(337, 513);
            btnStats.Name = "btnStats";
            btnStats.Size = new System.Drawing.Size(58, 24);
            btnStats.TabIndex = 5;
            btnStats.TabStop = false;
            btnStats.Text = "Stats";
            btnStats.UseVisualStyleBackColor = false;
            btnStats.Click += btnStats_Click;
            // 
            // linkSupport
            // 
            linkSupport.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            linkSupport.BackColor = System.Drawing.Color.FromArgb(((int)((byte)46)), ((int)((byte)52)), ((int)((byte)64)));
            linkSupport.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)((byte)122)), ((int)((byte)122)), ((int)((byte)122)));
            linkSupport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            linkSupport.ForeColor = System.Drawing.Color.White;
            linkSupport.Image = ((System.Drawing.Image)resources.GetObject("linkSupport.Image"));
            linkSupport.Location = new System.Drawing.Point(465, 513);
            linkSupport.Name = "linkSupport";
            linkSupport.Size = new System.Drawing.Size(24, 24);
            linkSupport.TabIndex = 4;
            linkSupport.TabStop = false;
            linkSupport.UseVisualStyleBackColor = false;
            linkSupport.Click += linkSupport_Click;
            // 
            // webView
            // 
            webView.AllowExternalDrop = false;
            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = System.Drawing.Color.White;
            webView.Dock = System.Windows.Forms.DockStyle.Fill;
            webView.Location = new System.Drawing.Point(0, 0);
            webView.Margin = new System.Windows.Forms.Padding(0);
            webView.Name = "webView";
            webView.Size = new System.Drawing.Size(784, 561);
            webView.TabIndex = 1;
            webView.ZoomFactor = 1D;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.FromArgb(((int)((byte)46)), ((int)((byte)52)), ((int)((byte)64)));
            ClientSize = new System.Drawing.Size(784, 561);
            Controls.Add(webView);
            Controls.Add(splitContainer1);
            Icon = ((System.Drawing.Icon)resources.GetObject("$this.Icon"));
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimumSize = new System.Drawing.Size(800, 600);
            Text = "ToN Save Manager";
            Activated += MainWindow_Activated;
            FormClosing += MainWindow_FormClosing;
            Load += mainWindow_Loaded;
            Shown += mainWindow_Shown;
            ctxMenuEntries.ResumeLayout(false);
            ctxMenuKeys.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)webView).EndInit();
            ResumeLayout(false);
        }

        private Microsoft.Web.WebView2.WinForms.WebView2 webView;

        #endregion

        private System.Windows.Forms.ListBox listBoxKeys;
        private System.Windows.Forms.ListBox listBoxEntries;
        private ContextMenuStrip ctxMenuEntries;
        private ToolStripMenuItem ctxMenuEntriesCopyTo;
        private ToolStripMenuItem ctxMenuEntriesNew;
        private ToolStripMenuItem ctxMenuEntriesNote;
        private ContextMenuStrip ctxMenuKeys;
        private ToolStripMenuItem renameToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem ctxMenuEntriesDelete;
        private ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnObjectives;
        private System.Windows.Forms.Button linkWiki;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ToolStripMenuItem ctxMenuEntriesBackup;
        private System.Windows.Forms.Button linkSupport;
        private System.Windows.Forms.Button btnStats;
    }
}