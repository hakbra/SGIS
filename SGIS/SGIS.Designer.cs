namespace SGIS
{
    partial class SGIS
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
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.progressLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.coordLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shapefileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mouseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mouseMoveItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mouseSelectItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mouseInfoItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectNoneItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectByPropertyItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.mapWindow = new System.Windows.Forms.PictureBox();
            this.layerList = new System.Windows.Forms.ListBox();
            this.statusStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapWindow)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.SystemColors.ControlLight;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressLabel,
            this.progressBar,
            this.toolStripStatusLabel2,
            this.statusLabel,
            this.coordLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 531);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(721, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // progressLabel
            // 
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(39, 17);
            this.progressLabel.Text = "Status";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(565, 17);
            this.toolStripStatusLabel2.Spring = true;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // coordLabel
            // 
            this.coordLabel.Name = "coordLabel";
            this.coordLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.mouseToolStripMenuItem,
            this.selectToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(721, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exitMenuItem});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(37, 20);
            this.fileMenu.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.shapefileMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.openToolStripMenuItem.Text = "Open...";
            // 
            // shapefileMenuItem
            // 
            this.shapefileMenuItem.Name = "shapefileMenuItem";
            this.shapefileMenuItem.Size = new System.Drawing.Size(122, 22);
            this.shapefileMenuItem.Text = "Shapefile";
            this.shapefileMenuItem.Click += new System.EventHandler(this.shapefileMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(112, 22);
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // mouseToolStripMenuItem
            // 
            this.mouseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mouseMoveItem,
            this.mouseSelectItem,
            this.mouseInfoItem});
            this.mouseToolStripMenuItem.Name = "mouseToolStripMenuItem";
            this.mouseToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.mouseToolStripMenuItem.Text = "Pointer";
            // 
            // mouseMoveItem
            // 
            this.mouseMoveItem.Enabled = false;
            this.mouseMoveItem.Name = "mouseMoveItem";
            this.mouseMoveItem.Size = new System.Drawing.Size(105, 22);
            this.mouseMoveItem.Text = "Move";
            this.mouseMoveItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseMoveItem_MouseDown);
            // 
            // mouseSelectItem
            // 
            this.mouseSelectItem.Name = "mouseSelectItem";
            this.mouseSelectItem.Size = new System.Drawing.Size(105, 22);
            this.mouseSelectItem.Text = "Select";
            this.mouseSelectItem.Click += new System.EventHandler(this.mouseSelectItem_Click);
            // 
            // mouseInfoItem
            // 
            this.mouseInfoItem.Name = "mouseInfoItem";
            this.mouseInfoItem.Size = new System.Drawing.Size(105, 22);
            this.mouseInfoItem.Text = "Info";
            this.mouseInfoItem.Click += new System.EventHandler(this.mouseInfoItem_Click);
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllItem,
            this.selectNoneItem,
            this.selectByPropertyItem});
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            this.selectToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.selectToolStripMenuItem.Text = "Select";
            // 
            // selectAllItem
            // 
            this.selectAllItem.Enabled = false;
            this.selectAllItem.Name = "selectAllItem";
            this.selectAllItem.Size = new System.Drawing.Size(152, 22);
            this.selectAllItem.Text = "All";
            this.selectAllItem.Click += new System.EventHandler(this.selectAllItem_Click);
            // 
            // selectNoneItem
            // 
            this.selectNoneItem.Enabled = false;
            this.selectNoneItem.Name = "selectNoneItem";
            this.selectNoneItem.Size = new System.Drawing.Size(152, 22);
            this.selectNoneItem.Text = "None";
            this.selectNoneItem.Click += new System.EventHandler(this.selectNoneItem_Click);
            // 
            // selectByPropertyItem
            // 
            this.selectByPropertyItem.Enabled = false;
            this.selectByPropertyItem.Name = "selectByPropertyItem";
            this.selectByPropertyItem.Size = new System.Drawing.Size(152, 22);
            this.selectByPropertyItem.Text = "By property...";
            this.selectByPropertyItem.Click += new System.EventHandler(this.selectByPropertyItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.splitContainer1.Panel1.Controls.Add(this.mapWindow);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel2.Controls.Add(this.layerList);
            this.splitContainer1.Panel2MinSize = 170;
            this.splitContainer1.Size = new System.Drawing.Size(721, 507);
            this.splitContainer1.SplitterDistance = 536;
            this.splitContainer1.TabIndex = 2;
            // 
            // mapWindow
            // 
            this.mapWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapWindow.Location = new System.Drawing.Point(0, 0);
            this.mapWindow.Name = "mapWindow";
            this.mapWindow.Size = new System.Drawing.Size(534, 505);
            this.mapWindow.TabIndex = 0;
            this.mapWindow.TabStop = false;
            this.mapWindow.Paint += new System.Windows.Forms.PaintEventHandler(this.SGIS_Paint);
            this.mapWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SGIS_MouseDown);
            this.mapWindow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SGIS_MouseMove);
            this.mapWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SGIS_MouseUp);
            this.mapWindow.Resize += new System.EventHandler(this.SGIS_Resize);
            // 
            // layerList
            // 
            this.layerList.AllowDrop = true;
            this.layerList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.layerList.FormattingEnabled = true;
            this.layerList.Location = new System.Drawing.Point(3, 3);
            this.layerList.Name = "layerList";
            this.layerList.Size = new System.Drawing.Size(173, 173);
            this.layerList.TabIndex = 0;
            this.layerList.SelectedIndexChanged += new System.EventHandler(this.layerList_SelectedIndexChanged);
            this.layerList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.layerList_MouseDown);
            // 
            // SGIS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 553);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Name = "SGIS";
            this.Text = "SGIS";
            this.Load += new System.EventHandler(this.SGIS_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SGIS_Paint);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mapWindow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox mapWindow;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shapefileMenuItem;
        private System.Windows.Forms.ListBox layerList;
        private System.Windows.Forms.ToolStripMenuItem mouseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mouseMoveItem;
        private System.Windows.Forms.ToolStripMenuItem mouseSelectItem;
        private System.Windows.Forms.ToolStripMenuItem mouseInfoItem;
        private System.Windows.Forms.ToolStripStatusLabel progressLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel coordLabel;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllItem;
        private System.Windows.Forms.ToolStripMenuItem selectNoneItem;
        private System.Windows.Forms.ToolStripMenuItem selectByPropertyItem;
    }
}

