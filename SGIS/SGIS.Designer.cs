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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SGIS));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.progressLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.coordLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spatialReferenceSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.mapWindow = new System.Windows.Forms.PictureBox();
            this.toolPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.layerLabel = new System.Windows.Forms.Label();
            this.layerList = new System.Windows.Forms.ListBox();
            this.layerButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.innerTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pointerLabel = new System.Windows.Forms.Label();
            this.selectLabel = new System.Windows.Forms.Label();
            this.toolLabel = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.upButton = new NoSelectButton();
            this.downButton = new NoSelectButton();
            this.addButton = new NoSelectButton();
            this.delButton = new NoSelectButton();
            this.mouseMoveButton = new NoSelectButton();
            this.mouseSelectButton = new NoSelectButton();
            this.mouseInfoButton = new NoSelectButton();
            this.selectAllButton = new NoSelectButton();
            this.selectNoneButton = new NoSelectButton();
            this.selectInvertButton = new NoSelectButton();
            this.selectPropButton = new NoSelectButton();
            this.toLayerButton = new NoSelectButton();
            this.zoomButton = new NoSelectButton();
            this.measureButton = new NoSelectButton();
            this.bufferButton = new NoSelectButton();
            this.unionButton = new NoSelectButton();
            this.mergeButton = new NoSelectButton();
            this.subtractButton = new NoSelectButton();
            this.intersectButton = new NoSelectButton();
            this.renderButton = new NoSelectButton();
            this.photoButton = new NoSelectButton();
            this.statusStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapWindow)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.layerButtonPanel.SuspendLayout();
            this.innerTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.SystemColors.ControlLight;
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.progressLabel,
            this.toolStripStatusLabel2,
            this.statusLabel,
            this.coordLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 871);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip.Size = new System.Drawing.Size(961, 26);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(133, 20);
            // 
            // progressLabel
            // 
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(0, 21);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(806, 21);
            this.toolStripStatusLabel2.Spring = true;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 21);
            // 
            // coordLabel
            // 
            this.coordLabel.Name = "coordLabel";
            this.coordLabel.Size = new System.Drawing.Size(0, 21);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(961, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.exitMenuItem});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(44, 24);
            this.fileMenu.Text = "File";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spatialReferenceSystemToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(130, 24);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // spatialReferenceSystemToolStripMenuItem
            // 
            this.spatialReferenceSystemToolStripMenuItem.Name = "spatialReferenceSystemToolStripMenuItem";
            this.spatialReferenceSystemToolStripMenuItem.Size = new System.Drawing.Size(245, 24);
            this.spatialReferenceSystemToolStripMenuItem.Text = "Spatial Reference System";
            this.spatialReferenceSystemToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(130, 24);
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.splitContainer1.Size = new System.Drawing.Size(961, 843);
            this.splitContainer1.SplitterDistance = 775;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 2;
            // 
            // mapWindow
            // 
            this.mapWindow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mapWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapWindow.Location = new System.Drawing.Point(0, 0);
            this.mapWindow.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mapWindow.Name = "mapWindow";
            this.mapWindow.Size = new System.Drawing.Size(773, 841);
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
            this.toolPanel.Location = new System.Drawing.Point(0, 841);
            this.toolPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this.layerLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.layerList, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.layerButtonPanel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.pointerLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.mouseMoveButton, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.mouseSelectButton, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.mouseInfoButton, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.selectLabel, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.selectAllButton, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.selectNoneButton, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.selectInvertButton, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.selectPropButton, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.toLayerButton, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.zoomButton, 2, 7);
            this.tableLayoutPanel1.Controls.Add(this.toolLabel, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.measureButton, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.bufferButton, 1, 9);
            this.tableLayoutPanel1.Controls.Add(this.unionButton, 2, 9);
            this.tableLayoutPanel1.Controls.Add(this.mergeButton, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.subtractButton, 1, 10);
            this.tableLayoutPanel1.Controls.Add(this.intersectButton, 2, 10);
            this.tableLayoutPanel1.Controls.Add(this.renderButton, 0, 11);
            this.tableLayoutPanel1.Controls.Add(this.photoButton, 0, 11);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 12;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(179, 529);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // layerLabel
            // 
            this.layerLabel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.layerLabel, 3);
            this.layerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.layerLabel.Location = new System.Drawing.Point(4, 12);
            this.layerLabel.Margin = new System.Windows.Forms.Padding(4, 12, 4, 0);
            this.layerLabel.Name = "layerLabel";
            this.layerLabel.Size = new System.Drawing.Size(171, 17);
            this.layerLabel.TabIndex = 4;
            this.layerLabel.Text = "Layers";
            this.layerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // layerList
            // 
            this.layerList.AllowDrop = true;
            this.tableLayoutPanel1.SetColumnSpan(this.layerList, 3);
            this.layerList.Dock = System.Windows.Forms.DockStyle.Top;
            this.layerList.FormattingEnabled = true;
            this.layerList.ItemHeight = 16;
            this.layerList.Location = new System.Drawing.Point(0, 29);
            this.layerList.Margin = new System.Windows.Forms.Padding(0);
            this.layerList.Name = "layerList";
            this.layerList.Size = new System.Drawing.Size(179, 228);
            this.layerList.TabIndex = 7;
            this.layerList.SelectedIndexChanged += new System.EventHandler(this.layerList_SelectedIndexChanged);
            this.layerList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.layerList_MouseDown);
            // 
            // layerButtonPanel
            // 
            this.layerButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.layerButtonPanel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tableLayoutPanel1.SetColumnSpan(this.layerButtonPanel, 3);
            this.layerButtonPanel.Controls.Add(this.innerTableLayoutPanel);
            this.layerButtonPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.layerButtonPanel.Location = new System.Drawing.Point(0, 257);
            this.layerButtonPanel.Margin = new System.Windows.Forms.Padding(0);
            this.layerButtonPanel.Name = "layerButtonPanel";
            this.layerButtonPanel.Size = new System.Drawing.Size(179, 31);
            this.layerButtonPanel.TabIndex = 3;
            // 
            // innerTableLayoutPanel
            // 
            this.innerTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.innerTableLayoutPanel.AutoSize = true;
            this.innerTableLayoutPanel.BackColor = System.Drawing.SystemColors.Control;
            this.innerTableLayoutPanel.ColumnCount = 4;
            this.innerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.innerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.innerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.innerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.innerTableLayoutPanel.Controls.Add(this.upButton, 3, 0);
            this.innerTableLayoutPanel.Controls.Add(this.downButton, 2, 0);
            this.innerTableLayoutPanel.Controls.Add(this.addButton, 0, 0);
            this.innerTableLayoutPanel.Controls.Add(this.delButton, 1, 0);
            this.innerTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.innerTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.innerTableLayoutPanel.Name = "innerTableLayoutPanel";
            this.innerTableLayoutPanel.RowCount = 1;
            this.innerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.innerTableLayoutPanel.Size = new System.Drawing.Size(179, 31);
            this.innerTableLayoutPanel.TabIndex = 0;
            // 
            // pointerLabel
            // 
            this.pointerLabel.AutoSize = true;
            this.pointerLabel.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.SetColumnSpan(this.pointerLabel, 3);
            this.pointerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pointerLabel.Location = new System.Drawing.Point(4, 300);
            this.pointerLabel.Margin = new System.Windows.Forms.Padding(4, 12, 4, 0);
            this.pointerLabel.Name = "pointerLabel";
            this.pointerLabel.Size = new System.Drawing.Size(171, 17);
            this.pointerLabel.TabIndex = 20;
            this.pointerLabel.Text = "Pointer";
            this.pointerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // selectLabel
            // 
            this.selectLabel.AutoSize = true;
            this.selectLabel.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.SetColumnSpan(this.selectLabel, 3);
            this.selectLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.selectLabel.Location = new System.Drawing.Point(4, 360);
            this.selectLabel.Margin = new System.Windows.Forms.Padding(4, 12, 4, 0);
            this.selectLabel.Name = "selectLabel";
            this.selectLabel.Size = new System.Drawing.Size(171, 17);
            this.selectLabel.TabIndex = 13;
            this.selectLabel.Text = "Select";
            this.selectLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolLabel
            // 
            this.toolLabel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.toolLabel, 3);
            this.toolLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolLabel.Location = new System.Drawing.Point(4, 437);
            this.toolLabel.Margin = new System.Windows.Forms.Padding(4, 12, 4, 0);
            this.toolLabel.Name = "toolLabel";
            this.toolLabel.Size = new System.Drawing.Size(171, 17);
            this.toolLabel.TabIndex = 18;
            this.toolLabel.Text = "Tools";
            this.toolLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // upButton
            // 
            this.upButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.upButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("upButton.BackgroundImage")));
            this.upButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.upButton.Location = new System.Drawing.Point(132, 0);
            this.upButton.Margin = new System.Windows.Forms.Padding(0);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(47, 31);
            this.upButton.TabIndex = 4;
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            // 
            // downButton
            // 
            this.downButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("downButton.BackgroundImage")));
            this.downButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.downButton.Location = new System.Drawing.Point(88, 0);
            this.downButton.Margin = new System.Windows.Forms.Padding(0);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(44, 31);
            this.downButton.TabIndex = 5;
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.Click += new System.EventHandler(this.downButton_Click);
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("addButton.BackgroundImage")));
            this.addButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.addButton.Location = new System.Drawing.Point(0, 0);
            this.addButton.Margin = new System.Windows.Forms.Padding(0);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(44, 31);
            this.addButton.TabIndex = 6;
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // delButton
            // 
            this.delButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.delButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("delButton.BackgroundImage")));
            this.delButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.delButton.Location = new System.Drawing.Point(44, 0);
            this.delButton.Margin = new System.Windows.Forms.Padding(0);
            this.delButton.Name = "delButton";
            this.delButton.Size = new System.Drawing.Size(44, 31);
            this.delButton.TabIndex = 7;
            this.delButton.UseVisualStyleBackColor = true;
            this.delButton.Click += new System.EventHandler(this.delButton_Click);
            // 
            // mouseMoveButton
            // 
            this.mouseMoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mouseMoveButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mouseMoveButton.BackgroundImage")));
            this.mouseMoveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mouseMoveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mouseMoveButton.Enabled = false;
            this.mouseMoveButton.Location = new System.Drawing.Point(0, 317);
            this.mouseMoveButton.Margin = new System.Windows.Forms.Padding(0);
            this.mouseMoveButton.Name = "mouseMoveButton";
            this.mouseMoveButton.Size = new System.Drawing.Size(59, 31);
            this.mouseMoveButton.TabIndex = 10;
            this.mouseMoveButton.UseVisualStyleBackColor = true;
            this.mouseMoveButton.Click += new System.EventHandler(this.mouseMoveItem_MouseDown);
            // 
            // mouseSelectButton
            // 
            this.mouseSelectButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mouseSelectButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mouseSelectButton.BackgroundImage")));
            this.mouseSelectButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mouseSelectButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mouseSelectButton.Location = new System.Drawing.Point(59, 317);
            this.mouseSelectButton.Margin = new System.Windows.Forms.Padding(0);
            this.mouseSelectButton.Name = "mouseSelectButton";
            this.mouseSelectButton.Size = new System.Drawing.Size(59, 31);
            this.mouseSelectButton.TabIndex = 11;
            this.mouseSelectButton.UseVisualStyleBackColor = true;
            this.mouseSelectButton.Click += new System.EventHandler(this.mouseSelectItem_Click);
            // 
            // mouseInfoButton
            // 
            this.mouseInfoButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mouseInfoButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mouseInfoButton.BackgroundImage")));
            this.mouseInfoButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mouseInfoButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mouseInfoButton.Location = new System.Drawing.Point(118, 317);
            this.mouseInfoButton.Margin = new System.Windows.Forms.Padding(0);
            this.mouseInfoButton.Name = "mouseInfoButton";
            this.mouseInfoButton.Size = new System.Drawing.Size(61, 31);
            this.mouseInfoButton.TabIndex = 12;
            this.mouseInfoButton.UseVisualStyleBackColor = true;
            this.mouseInfoButton.Click += new System.EventHandler(this.mouseInfoItem_Click);
            // 
            // selectAllButton
            // 
            this.selectAllButton.AutoSize = true;
            this.selectAllButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.selectAllButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectAllButton.Font = new System.Drawing.Font("Lucida Sans", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectAllButton.Location = new System.Drawing.Point(0, 377);
            this.selectAllButton.Margin = new System.Windows.Forms.Padding(0);
            this.selectAllButton.Name = "selectAllButton";
            this.selectAllButton.Size = new System.Drawing.Size(59, 24);
            this.selectAllButton.TabIndex = 14;
            this.selectAllButton.Text = "All";
            this.selectAllButton.UseVisualStyleBackColor = true;
            this.selectAllButton.Click += new System.EventHandler(this.selectAllItem_Click);
            // 
            // selectNoneButton
            // 
            this.selectNoneButton.AutoSize = true;
            this.selectNoneButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.selectNoneButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectNoneButton.Font = new System.Drawing.Font("Lucida Sans", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectNoneButton.Location = new System.Drawing.Point(59, 377);
            this.selectNoneButton.Margin = new System.Windows.Forms.Padding(0);
            this.selectNoneButton.Name = "selectNoneButton";
            this.selectNoneButton.Size = new System.Drawing.Size(59, 24);
            this.selectNoneButton.TabIndex = 15;
            this.selectNoneButton.Text = "None";
            this.selectNoneButton.UseVisualStyleBackColor = true;
            this.selectNoneButton.Click += new System.EventHandler(this.selectNoneItem_Click);
            // 
            // selectInvertButton
            // 
            this.selectInvertButton.AutoSize = true;
            this.selectInvertButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.selectInvertButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectInvertButton.Font = new System.Drawing.Font("Lucida Sans", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectInvertButton.Location = new System.Drawing.Point(118, 377);
            this.selectInvertButton.Margin = new System.Windows.Forms.Padding(0);
            this.selectInvertButton.Name = "selectInvertButton";
            this.selectInvertButton.Size = new System.Drawing.Size(61, 24);
            this.selectInvertButton.TabIndex = 16;
            this.selectInvertButton.Text = "Invert";
            this.selectInvertButton.UseVisualStyleBackColor = true;
            this.selectInvertButton.Click += new System.EventHandler(this.selectInvertButton_Click);
            // 
            // selectPropButton
            // 
            this.selectPropButton.AutoSize = true;
            this.selectPropButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.selectPropButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectPropButton.Font = new System.Drawing.Font("Lucida Sans", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectPropButton.Location = new System.Drawing.Point(0, 401);
            this.selectPropButton.Margin = new System.Windows.Forms.Padding(0);
            this.selectPropButton.Name = "selectPropButton";
            this.selectPropButton.Size = new System.Drawing.Size(59, 24);
            this.selectPropButton.TabIndex = 17;
            this.selectPropButton.Text = "Attribute";
            this.selectPropButton.UseVisualStyleBackColor = true;
            this.selectPropButton.Click += new System.EventHandler(this.selectByPropertyItem_Click);
            // 
            // toLayerButton
            // 
            this.toLayerButton.AutoSize = true;
            this.toLayerButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.toLayerButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toLayerButton.Font = new System.Drawing.Font("Lucida Sans", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toLayerButton.Location = new System.Drawing.Point(59, 401);
            this.toLayerButton.Margin = new System.Windows.Forms.Padding(0);
            this.toLayerButton.Name = "toLayerButton";
            this.toLayerButton.Size = new System.Drawing.Size(59, 24);
            this.toLayerButton.TabIndex = 19;
            this.toLayerButton.Text = "Copy";
            this.toLayerButton.UseVisualStyleBackColor = true;
            this.toLayerButton.Click += new System.EventHandler(this.toLayerButton_Click);
            // 
            // zoomButton
            // 
            this.zoomButton.AutoSize = true;
            this.zoomButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.zoomButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zoomButton.Font = new System.Drawing.Font("Lucida Sans", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.zoomButton.Location = new System.Drawing.Point(118, 401);
            this.zoomButton.Margin = new System.Windows.Forms.Padding(0);
            this.zoomButton.Name = "zoomButton";
            this.zoomButton.Size = new System.Drawing.Size(61, 24);
            this.zoomButton.TabIndex = 21;
            this.zoomButton.Text = "Zoom";
            this.zoomButton.UseVisualStyleBackColor = true;
            this.zoomButton.Click += new System.EventHandler(this.zoomButton_Click);
            // 
            // measureButton
            // 
            this.measureButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.measureButton.Font = new System.Drawing.Font("Lucida Sans", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.measureButton.Location = new System.Drawing.Point(0, 454);
            this.measureButton.Margin = new System.Windows.Forms.Padding(0);
            this.measureButton.Name = "measureButton";
            this.measureButton.Size = new System.Drawing.Size(59, 25);
            this.measureButton.TabIndex = 27;
            this.measureButton.Text = "Measure";
            this.measureButton.UseVisualStyleBackColor = true;
            this.measureButton.Click += new System.EventHandler(this.measureButton_Click);
            // 
            // bufferButton
            // 
            this.bufferButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bufferButton.Font = new System.Drawing.Font("Lucida Sans", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bufferButton.Location = new System.Drawing.Point(59, 454);
            this.bufferButton.Margin = new System.Windows.Forms.Padding(0);
            this.bufferButton.Name = "bufferButton";
            this.bufferButton.Size = new System.Drawing.Size(59, 25);
            this.bufferButton.TabIndex = 23;
            this.bufferButton.Text = "Buffer";
            this.bufferButton.UseVisualStyleBackColor = true;
            this.bufferButton.Click += new System.EventHandler(this.bufferButton_Click);
            // 
            // unionButton
            // 
            this.unionButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.unionButton.Font = new System.Drawing.Font("Lucida Sans", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.unionButton.Location = new System.Drawing.Point(118, 454);
            this.unionButton.Margin = new System.Windows.Forms.Padding(0);
            this.unionButton.Name = "unionButton";
            this.unionButton.Size = new System.Drawing.Size(61, 25);
            this.unionButton.TabIndex = 22;
            this.unionButton.Text = "Union";
            this.unionButton.UseVisualStyleBackColor = true;
            this.unionButton.Click += new System.EventHandler(this.unionButton_Click);
            // 
            // mergeButton
            // 
            this.mergeButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mergeButton.Font = new System.Drawing.Font("Lucida Sans", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mergeButton.Location = new System.Drawing.Point(0, 479);
            this.mergeButton.Margin = new System.Windows.Forms.Padding(0);
            this.mergeButton.Name = "mergeButton";
            this.mergeButton.Size = new System.Drawing.Size(59, 25);
            this.mergeButton.TabIndex = 24;
            this.mergeButton.Text = "Merge";
            this.mergeButton.UseVisualStyleBackColor = true;
            this.mergeButton.Click += new System.EventHandler(this.mergeButton_Click);
            // 
            // subtractButton
            // 
            this.subtractButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subtractButton.Font = new System.Drawing.Font("Lucida Sans", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.subtractButton.Location = new System.Drawing.Point(59, 479);
            this.subtractButton.Margin = new System.Windows.Forms.Padding(0);
            this.subtractButton.Name = "subtractButton";
            this.subtractButton.Size = new System.Drawing.Size(59, 25);
            this.subtractButton.TabIndex = 26;
            this.subtractButton.Text = "Subtract";
            this.subtractButton.UseVisualStyleBackColor = true;
            this.subtractButton.Click += new System.EventHandler(this.diffButton_Click);
            // 
            // intersectButton
            // 
            this.intersectButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.intersectButton.Font = new System.Drawing.Font("Lucida Sans", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.intersectButton.Location = new System.Drawing.Point(118, 479);
            this.intersectButton.Margin = new System.Windows.Forms.Padding(0);
            this.intersectButton.Name = "intersectButton";
            this.intersectButton.Size = new System.Drawing.Size(61, 25);
            this.intersectButton.TabIndex = 25;
            this.intersectButton.Text = "Intersect";
            this.intersectButton.UseVisualStyleBackColor = true;
            this.intersectButton.Click += new System.EventHandler(this.intersectButton_Click);
            // 
            // renderButton
            // 
            this.renderButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderButton.Font = new System.Drawing.Font("Lucida Sans", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.renderButton.Location = new System.Drawing.Point(59, 504);
            this.renderButton.Margin = new System.Windows.Forms.Padding(0);
            this.renderButton.Name = "renderButton";
            this.renderButton.Size = new System.Drawing.Size(59, 25);
            this.renderButton.TabIndex = 28;
            this.renderButton.Text = "GeoTiff";
            this.renderButton.UseVisualStyleBackColor = true;
            this.renderButton.Click += new System.EventHandler(this.renderButton_Click);
            // 
            // photoButton
            // 
            this.photoButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.photoButton.Font = new System.Drawing.Font("Lucida Sans", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.photoButton.Location = new System.Drawing.Point(0, 504);
            this.photoButton.Margin = new System.Windows.Forms.Padding(0);
            this.photoButton.Name = "photoButton";
            this.photoButton.Size = new System.Drawing.Size(59, 25);
            this.photoButton.TabIndex = 29;
            this.photoButton.Text = "WMS";
            this.photoButton.UseVisualStyleBackColor = true;
            this.photoButton.Click += new System.EventHandler(this.photoButton_Click);
            // 
            // SGIS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(961, 897);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.layerButtonPanel.ResumeLayout(false);
            this.layerButtonPanel.PerformLayout();
            this.innerTableLayoutPanel.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripStatusLabel progressLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel coordLabel;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label layerLabel;
        private NoSelectButton mouseMoveButton;
        private System.Windows.Forms.ListBox layerList;
        private NoSelectButton mouseSelectButton;
        private NoSelectButton mouseInfoButton;
        private NoSelectButton toLayerButton;
        private NoSelectButton selectPropButton;
        private NoSelectButton selectInvertButton;
        private NoSelectButton selectNoneButton;
        private NoSelectButton selectAllButton;
        private System.Windows.Forms.Label selectLabel;
        private System.Windows.Forms.Label toolLabel;
        private System.Windows.Forms.Label pointerLabel;
        private System.Windows.Forms.TableLayoutPanel toolPanel;
        private NoSelectButton zoomButton;
        private NoSelectButton unionButton;
        private NoSelectButton bufferButton;
        private NoSelectButton mergeButton;
        private NoSelectButton intersectButton;
        private NoSelectButton subtractButton;
        private NoSelectButton measureButton;
        private System.Windows.Forms.FlowLayoutPanel layerButtonPanel;
        private System.Windows.Forms.TableLayoutPanel innerTableLayoutPanel;
        private NoSelectButton upButton;
        private NoSelectButton downButton;
        private NoSelectButton addButton;
        private NoSelectButton delButton;
        private NoSelectButton renderButton;
        private NoSelectButton photoButton;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spatialReferenceSystemToolStripMenuItem;
    }
}

