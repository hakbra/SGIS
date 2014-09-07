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
            this.components = new System.ComponentModel.Container();
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.mapWindow = new System.Windows.Forms.PictureBox();
            this.toolPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button8 = new System.Windows.Forms.Button();
            this.selectPropButton = new System.Windows.Forms.Button();
            this.selectInvertButton = new System.Windows.Forms.Button();
            this.selectNoneButton = new System.Windows.Forms.Button();
            this.selectAllButton = new System.Windows.Forms.Button();
            this.mouseMoveButton = new System.Windows.Forms.Button();
            this.layerLabel = new System.Windows.Forms.Label();
            this.layerList = new System.Windows.Forms.ListBox();
            this.mouseSelectButton = new System.Windows.Forms.Button();
            this.mouseInfoButton = new System.Windows.Forms.Button();
            this.selectLabel = new System.Windows.Forms.Label();
            this.toolLabel = new System.Windows.Forms.Label();
            this.pointerLabel = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.statusStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapWindow)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
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
            this.statusStrip.Location = new System.Drawing.Point(0, 707);
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
            this.fileMenu});
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
            this.splitContainer1.Panel2.Controls.Add(this.toolPanel);
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Panel2MinSize = 170;
            this.splitContainer1.Size = new System.Drawing.Size(721, 683);
            this.splitContainer1.SplitterDistance = 536;
            this.splitContainer1.TabIndex = 2;
            // 
            // mapWindow
            // 
            this.mapWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapWindow.Location = new System.Drawing.Point(0, 0);
            this.mapWindow.Name = "mapWindow";
            this.mapWindow.Size = new System.Drawing.Size(534, 681);
            this.mapWindow.TabIndex = 0;
            this.mapWindow.TabStop = false;
            this.mapWindow.Paint += new System.Windows.Forms.PaintEventHandler(this.SGIS_Paint);
            this.mapWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SGIS_MouseDown);
            this.mapWindow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SGIS_MouseMove);
            this.mapWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SGIS_MouseUp);
            this.mapWindow.Resize += new System.EventHandler(this.SGIS_Resize);
            // 
            // toolPanel
            // 
            this.toolPanel.AutoSize = true;
            this.toolPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.toolPanel.BackColor = System.Drawing.SystemColors.Control;
            this.toolPanel.ColumnCount = 1;
            this.toolPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.toolPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.toolPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolPanel.Location = new System.Drawing.Point(0, 681);
            this.toolPanel.Name = "toolPanel";
            this.toolPanel.RowCount = 1;
            this.toolPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.toolPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.toolPanel.Size = new System.Drawing.Size(179, 0);
            this.toolPanel.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.button8, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.selectPropButton, 3, 5);
            this.tableLayoutPanel1.Controls.Add(this.selectInvertButton, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.selectNoneButton, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.selectAllButton, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.mouseMoveButton, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.layerLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.layerList, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.mouseSelectButton, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.mouseInfoButton, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.selectLabel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.toolLabel, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.pointerLabel, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(179, 347);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // button8
            // 
            this.button8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button8.Location = new System.Drawing.Point(0, 324);
            this.button8.Margin = new System.Windows.Forms.Padding(0);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(46, 23);
            this.button8.TabIndex = 19;
            this.button8.Text = "button8";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // selectPropButton
            // 
            this.selectPropButton.AutoSize = true;
            this.selectPropButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.selectPropButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectPropButton.Enabled = false;
            this.selectPropButton.Location = new System.Drawing.Point(128, 278);
            this.selectPropButton.Margin = new System.Windows.Forms.Padding(0);
            this.selectPropButton.Name = "selectPropButton";
            this.selectPropButton.Size = new System.Drawing.Size(51, 23);
            this.selectPropButton.TabIndex = 17;
            this.selectPropButton.Text = "Prop.";
            this.selectPropButton.UseVisualStyleBackColor = true;
            this.selectPropButton.Click += new System.EventHandler(this.selectByPropertyItem_Click);
            // 
            // selectInvertButton
            // 
            this.selectInvertButton.AutoSize = true;
            this.selectInvertButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.selectInvertButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectInvertButton.Enabled = false;
            this.selectInvertButton.Location = new System.Drawing.Point(93, 278);
            this.selectInvertButton.Margin = new System.Windows.Forms.Padding(0);
            this.selectInvertButton.Name = "selectInvertButton";
            this.selectInvertButton.Size = new System.Drawing.Size(35, 23);
            this.selectInvertButton.TabIndex = 16;
            this.selectInvertButton.Text = "Inv.";
            this.selectInvertButton.UseVisualStyleBackColor = true;
            this.selectInvertButton.Click += new System.EventHandler(this.selectInvertButton_Click);
            // 
            // selectNoneButton
            // 
            this.selectNoneButton.AutoSize = true;
            this.selectNoneButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.selectNoneButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectNoneButton.Enabled = false;
            this.selectNoneButton.Location = new System.Drawing.Point(46, 278);
            this.selectNoneButton.Margin = new System.Windows.Forms.Padding(0);
            this.selectNoneButton.Name = "selectNoneButton";
            this.selectNoneButton.Size = new System.Drawing.Size(47, 23);
            this.selectNoneButton.TabIndex = 15;
            this.selectNoneButton.Text = "None";
            this.selectNoneButton.UseVisualStyleBackColor = true;
            this.selectNoneButton.Click += new System.EventHandler(this.selectNoneItem_Click);
            // 
            // selectAllButton
            // 
            this.selectAllButton.AutoSize = true;
            this.selectAllButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.selectAllButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectAllButton.Enabled = false;
            this.selectAllButton.Location = new System.Drawing.Point(0, 278);
            this.selectAllButton.Margin = new System.Windows.Forms.Padding(0);
            this.selectAllButton.Name = "selectAllButton";
            this.selectAllButton.Size = new System.Drawing.Size(46, 23);
            this.selectAllButton.TabIndex = 14;
            this.selectAllButton.Text = "All";
            this.selectAllButton.UseVisualStyleBackColor = true;
            this.selectAllButton.Click += new System.EventHandler(this.selectAllItem_Click);
            // 
            // mouseMoveButton
            // 
            this.mouseMoveButton.AutoSize = true;
            this.mouseMoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mouseMoveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mouseMoveButton.Enabled = false;
            this.mouseMoveButton.Location = new System.Drawing.Point(0, 232);
            this.mouseMoveButton.Margin = new System.Windows.Forms.Padding(0);
            this.mouseMoveButton.Name = "mouseMoveButton";
            this.mouseMoveButton.Size = new System.Drawing.Size(46, 23);
            this.mouseMoveButton.TabIndex = 10;
            this.mouseMoveButton.Text = "Move";
            this.mouseMoveButton.UseVisualStyleBackColor = true;
            this.mouseMoveButton.Click += new System.EventHandler(this.mouseMoveItem_MouseDown);
            // 
            // layerLabel
            // 
            this.layerLabel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.layerLabel, 4);
            this.layerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.layerLabel.Location = new System.Drawing.Point(3, 10);
            this.layerLabel.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.layerLabel.Name = "layerLabel";
            this.layerLabel.Size = new System.Drawing.Size(173, 13);
            this.layerLabel.TabIndex = 4;
            this.layerLabel.Text = "Layers";
            this.layerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // layerList
            // 
            this.layerList.AllowDrop = true;
            this.tableLayoutPanel1.SetColumnSpan(this.layerList, 4);
            this.layerList.Dock = System.Windows.Forms.DockStyle.Top;
            this.layerList.FormattingEnabled = true;
            this.layerList.Location = new System.Drawing.Point(0, 23);
            this.layerList.Margin = new System.Windows.Forms.Padding(0);
            this.layerList.Name = "layerList";
            this.layerList.Size = new System.Drawing.Size(179, 186);
            this.layerList.TabIndex = 7;
            this.layerList.SelectedIndexChanged += new System.EventHandler(this.layerList_SelectedIndexChanged);
            this.layerList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.layerList_MouseDown);
            // 
            // mouseSelectButton
            // 
            this.mouseSelectButton.AutoSize = true;
            this.mouseSelectButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mouseSelectButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mouseSelectButton.Location = new System.Drawing.Point(46, 232);
            this.mouseSelectButton.Margin = new System.Windows.Forms.Padding(0);
            this.mouseSelectButton.Name = "mouseSelectButton";
            this.mouseSelectButton.Size = new System.Drawing.Size(47, 23);
            this.mouseSelectButton.TabIndex = 11;
            this.mouseSelectButton.Text = "Select";
            this.mouseSelectButton.UseVisualStyleBackColor = true;
            this.mouseSelectButton.Click += new System.EventHandler(this.mouseSelectItem_Click);
            // 
            // mouseInfoButton
            // 
            this.mouseInfoButton.AutoSize = true;
            this.mouseInfoButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mouseInfoButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mouseInfoButton.Location = new System.Drawing.Point(93, 232);
            this.mouseInfoButton.Margin = new System.Windows.Forms.Padding(0);
            this.mouseInfoButton.Name = "mouseInfoButton";
            this.mouseInfoButton.Size = new System.Drawing.Size(35, 23);
            this.mouseInfoButton.TabIndex = 12;
            this.mouseInfoButton.Text = "Info";
            this.mouseInfoButton.UseVisualStyleBackColor = true;
            this.mouseInfoButton.Click += new System.EventHandler(this.mouseInfoItem_Click);
            // 
            // selectLabel
            // 
            this.selectLabel.AutoSize = true;
            this.selectLabel.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.SetColumnSpan(this.selectLabel, 4);
            this.selectLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.selectLabel.Location = new System.Drawing.Point(3, 265);
            this.selectLabel.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.selectLabel.Name = "selectLabel";
            this.selectLabel.Size = new System.Drawing.Size(173, 13);
            this.selectLabel.TabIndex = 13;
            this.selectLabel.Text = "Select";
            this.selectLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolLabel
            // 
            this.toolLabel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.toolLabel, 4);
            this.toolLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolLabel.Location = new System.Drawing.Point(3, 311);
            this.toolLabel.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.toolLabel.Name = "toolLabel";
            this.toolLabel.Size = new System.Drawing.Size(173, 13);
            this.toolLabel.TabIndex = 18;
            this.toolLabel.Text = "Tools";
            this.toolLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pointerLabel
            // 
            this.pointerLabel.AutoSize = true;
            this.pointerLabel.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.SetColumnSpan(this.pointerLabel, 4);
            this.pointerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pointerLabel.Location = new System.Drawing.Point(3, 219);
            this.pointerLabel.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.pointerLabel.Name = "pointerLabel";
            this.pointerLabel.Size = new System.Drawing.Size(173, 13);
            this.pointerLabel.TabIndex = 20;
            this.pointerLabel.Text = "Pointer";
            this.pointerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SGIS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 729);
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
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mapWindow)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
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
        private System.Windows.Forms.ToolStripStatusLabel progressLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel coordLabel;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label layerLabel;
        private System.Windows.Forms.Button mouseMoveButton;
        private System.Windows.Forms.ListBox layerList;
        private System.Windows.Forms.Button mouseSelectButton;
        private System.Windows.Forms.Button mouseInfoButton;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button selectPropButton;
        private System.Windows.Forms.Button selectInvertButton;
        private System.Windows.Forms.Button selectNoneButton;
        private System.Windows.Forms.Button selectAllButton;
        private System.Windows.Forms.Label selectLabel;
        private System.Windows.Forms.Label toolLabel;
        private System.Windows.Forms.Label pointerLabel;
        private System.Windows.Forms.TableLayoutPanel toolPanel;
    }
}

