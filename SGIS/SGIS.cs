using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SGIS
{
    public partial class SGIS : Form
    {
        public static SGIS app;
        public BindingList<Layer> layers = new BindingList<Layer>();
        public ScreenManager screenManager = new ScreenManager();
        MouseTactic mouse = new MoveMouseTactic();

        private ContextMenuStrip layerListContextMenu = new ContextMenuStrip();
        private ContextMenuStrip infoContextMenu = new ContextMenuStrip();

        public SGIS()
        {
            InitializeComponent();
        }

        private void SGIS_Load(object sender, EventArgs e)
        {
            app = this;

            this.MouseWheel += new MouseEventHandler(SGIS_MouseWheel);

            screenManager.WindowsRect = new ScreenManager.SGISEnvelope(0, mapWindow.Width, 0, mapWindow.Height);
            screenManager.RealRect = new ScreenManager.SGISEnvelope(0, 100, 0, 100);
            screenManager.Calculate();

            layerList.DataSource = layers;

            layerListContextMenu.Opening += new CancelEventHandler(layerListContextMenu_Opening);
            infoContextMenu.Opening += new CancelEventHandler(infoContextMenu_Opening);

            toolPanel.RowStyles.Clear();
            toolPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        private void infoContextMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            Layer layer = (Layer) layerList.SelectedItem;
            if (layer == null || layer.selected.Count != 1)
                return;

            Feature f = layer.selected.First();
            DataRow dr = layer.dataTable.Rows[f.id];

            infoContextMenu.Items.Clear();
            foreach (var column in layer.dataTable.Columns)
            {
                string colName = column.ToString();
                infoContextMenu.Items.Add(colName + ": " + dr[colName]);
            }
        }

        private void layerListContextMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            Layer l = (Layer) layerList.SelectedItem;
            Bitmap colorImg = new Bitmap(20, 20);
            for (int x = 0; x < 20; x++)
                for (int y = 0; y < 20; y++)
                colorImg.SetPixel(x, y, l.color);

            layerListContextMenu.Items.Clear();
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Up", null, (o, i) => {
                Layer selected = (Layer)layerList.SelectedItem;
                int index = layers.IndexOf(l);
                if (index == 0)
                    return;
                layers.Remove(l);
                layers.Insert(index-1, l);
                layerList.SelectedItem = selected;
                redraw();
            }));
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Down", null, (o, i) =>
            {
                Layer selected = (Layer)layerList.SelectedItem;
                int index = layers.IndexOf(l);
                if (index == layers.Count - 1)
                    return;
                layers.Remove(l);
                layers.Insert(index+1, l);
                layerList.SelectedItem = selected;
                redraw();
            }));
            layerListContextMenu.Items.Add("-");
            layerListContextMenu.Items.Add(new ToolStripMenuItem(l.visible ? "Hide" : "Show", null, (o, i) => { l.visible = !l.visible; redraw(); }));
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Zoom", null, (o, i) => { screenManager.RealRect.Set(l.boundingbox); screenManager.Calculate(); redraw(); }));
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Color...", colorImg, (o, i) => {
                l.color = chooseColor(l.color); redraw();
            }));
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Rename", null, (o, i) => {
                ToolBuilder tb = new ToolBuilder(toolPanel, "Rename layer");
                TextBox name = tb.addTextbox("New name:");
                Label error = tb.addErrorLabel();
                Button button = tb.addButton("Rename", (Layer currentLayer) =>
                {
                    if (name.Text.Length == 0)
                    {
                        tb.setError("Provide name");
                        return;
                    }
                    currentLayer.Name = name.Text;
                });
            }));
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Remove", null, (o, i) => { layers.Remove(l); redraw(); }));
        }

        private Color chooseColor(Color c)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
                return cd.Color;
            return c;
        }

        private void SGIS_Paint(object sender, PaintEventArgs e)
        {
            var boundary = screenManager.MapScreenToReal(screenManager.WindowsRect);
            foreach (Layer l in layers.Reverse())
            {
                if (!l.visible)
                    continue;
                var visibleFeatures = l.getWithin(boundary);
                foreach (Feature s in visibleFeatures)
                {
                    if (!s.selected || l != layerList.SelectedItem)
                        Render.Draw(s.geometry, e.Graphics, l.color);
                    else if (l == layerList.SelectedItem)
                        Render.Draw(s.geometry, e.Graphics, Color.DarkCyan);
                }
                //if (l.quadTree != null)
                //    l.quadTree.render(e.Graphics);
                renderScale(e.Graphics);
            }

            mouse.render(e.Graphics);
        }

        private void renderScale(Graphics graphics)
        {
            double mpp = screenManager.RealRect.Width / screenManager.WindowsRect.Width;

            Pen p = new Pen(Color.Black);
            System.Drawing.Point start = new System.Drawing.Point(50, this.Height-150);
            System.Drawing.Point end = new System.Drawing.Point(150, this.Height-150);
            System.Drawing.Point text = new System.Drawing.Point(170, this.Height-150);
            graphics.DrawLine(p, start, end);

            Font f = new Font(FontFamily.GenericMonospace, 12);
            graphics.DrawString(mpp * 100 + " meters", f, new SolidBrush(Color.Black), text);
        }

        private void SGIS_MouseWheel(object sender, MouseEventArgs e)
        {
            mouse.MouseWheel(e);
        }

        private void SGIS_MouseDown(object sender, MouseEventArgs e)
        {
            mouse.MouseDown(e);
        }

        private void SGIS_MouseUp(object sender, MouseEventArgs e)
        {
            mouse.MouseUp(e);
        }

        private void SGIS_MouseMove(object sender, MouseEventArgs e)
        {
            mouse.MouseMove(e);
        }

        private void SGIS_Resize(object sender, EventArgs e)
        {
            screenManager.WindowsRect = new ScreenManager.SGISEnvelope(0, mapWindow.Width, 0, mapWindow.Height);
            screenManager.Calculate();
            this.Refresh();
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void shapefileMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "shp files|*.shp";
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (String file in openFileDialog1.FileNames)
                {
                    ShpReader sr = new ShpReader();
                    Layer l = sr.read(file);
                    layers.Insert(0, l);
                    layerList.SelectedIndex = 0;
                    this.layerList_SelectedIndexChanged(null, null);
                    screenManager.RealRect.Set(l.boundingbox);
                    screenManager.Calculate();
                    this.Refresh();
                }
            }
        }

        public void redraw()
        {
            mapWindow.Refresh();
        }

        public void setStatusText(string s)
        {
            statusLabel.Text = s;
        }

        public void setCoordText(string s)
        {
            coordLabel.Text = s;
        }

        public System.Drawing.Point getMousePos()
        {
            var mouse = new System.Drawing.Point(Cursor.Position.X, Cursor.Position.Y);
            mouse = mapWindow.PointToClient(mouse);
            return mouse;
        }

        internal PictureBox getMapWindow()
        {
            return mapWindow;
        }

        internal ContextMenuStrip getInfoMenu()
        {
            return infoContextMenu;
        }

        internal ListBox getLayerList()
        {
            return layerList;
        }

        private void layerList_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                var index = layerList.IndexFromPoint(e.Location);
                if (index != -1)
                    layerList.SelectedIndex = index;
                if (layerList.SelectedIndex != -1)
                    layerListContextMenu.Show(layerList.PointToScreen(e.Location));
            }
            Layer l = (Layer)(layerList.SelectedItem);
            if (l == null || l.selected.Count == 0)
                SGIS.app.setStatusText("");
            else
                SGIS.app.setStatusText(l.selected.Count + " objects");
            redraw();
        }

        private void mouseMoveItem_MouseDown(object sender, EventArgs e)
        {
            mouse = new MoveMouseTactic();
            mouseMoveButton.Enabled = false;
            mouseInfoButton.Enabled = true;
            mouseSelectButton.Enabled = true;
        }

        private void mouseSelectItem_Click(object sender, EventArgs e)
        {
            mouse = new SelectMouseTactic();
            mouseMoveButton.Enabled = true;
            mouseInfoButton.Enabled = true;
            mouseSelectButton.Enabled = false;
        }

        private void mouseInfoItem_Click(object sender, EventArgs e)
        {
            mouse = new InfoMouseTactic();
            mouseMoveButton.Enabled = true;
            mouseSelectButton.Enabled = true;
            mouseInfoButton.Enabled = false;
        }

        private void selectAllItem_Click(object sender, EventArgs e)
        {
            Layer l = (Layer)layerList.SelectedItem;
            if (l == null)
                return;
            l.clearSelected();
            foreach (Feature f in l.features.Values) {
                l.selected.Add(f);
                f.selected = true;
            }
            setStatusText(l.features.Count + " objects");
            redraw();
        }

        private void selectNoneItem_Click(object sender, EventArgs e)
        {
            Layer l = (Layer)layerList.SelectedItem;
            if (l == null)
                return;
            l.clearSelected();
            setStatusText("");
            redraw();
        }

        private void selectByPropertyItem_Click(object sender, EventArgs e)
        {
            ToolBuilder tb = new ToolBuilder(toolPanel, "Select by property");
            TextBox textbox = tb.addTextbox("Expression:");

            ComboBox comboBox = new ComboBox();
            comboBox.Items.Add("Column name");
            comboBox.SelectedIndex = 0;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Anchor = AnchorStyles.None;
            tb.addControl(comboBox);

            Label errorLabel = tb.addErrorLabel();
            Button selectButton = tb.addButton("Select", (l) =>
            {
                if (l.dataTable == null)
                    return;
                try
                {
                    DataRow[] rows = l.dataTable.Select(textbox.Text);
                    tb.setError("");
                    l.clearSelected();

                    foreach (DataRow dr in rows)
                    {
                        int id = (int)dr[0];
                        Feature f;
                        if (l.features.TryGetValue(id, out f))
                        {
                            f.selected = true;
                            l.selected.Add(f);
                        }
                    }
                    SGIS.app.redraw();
                }
                catch (Exception ex)
                {
                    tb.setError(ex.Message);
                }
            });

            comboBox.DropDown += (o, ev) =>
            {
                Layer l = (Layer)layerList.SelectedItem;
                if (l == null)
                    return;
                comboBox.Items.Clear();
                comboBox.Items.Add("Column name");
                foreach (var column in l.dataTable.Columns)
                {
                    string colName = column.ToString();
                    comboBox.Items.Add(colName);
                }
                comboBox.SelectedIndex = 0;
            };
            comboBox.SelectedIndexChanged += (o, ev) =>
            {
                if (comboBox.SelectedIndex == 0)
                    return;

                textbox.Text += "[" + comboBox.SelectedItem.ToString() + "] ";
                comboBox.SelectedIndex = 0;
                textbox.Focus();
                textbox.Select(textbox.Text.Length, 0);
            };
        }

        private void layerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Layer l = (Layer)layerList.SelectedItem;
            if (l == null)
            {
                selectAllButton.Enabled = false;
                selectNoneButton.Enabled = false;
                selectPropButton.Enabled = false;
                selectInvertButton.Enabled = false;
            }
            else
            {
                selectAllButton.Enabled = true;
                selectNoneButton.Enabled = true;
                selectInvertButton.Enabled = true;
                selectPropButton.Enabled = l.dataTable != null;
            }
            redraw();
        }

        private void selectInvertButton_Click(object sender, EventArgs e)
        {
            Layer l = (Layer)layerList.SelectedItem;
            if (l == null)
                return;
            List<Feature> newSelected = new List<Feature>();
            foreach (Feature f in l.features.Values)
            {
                f.selected = !f.selected;
                if (f.selected)
                    newSelected.Add(f);
            }
            l.selected = newSelected;
            setStatusText(newSelected.Count + " objects");
            redraw();
        }

        private void toLayerButton_Click(object sender, EventArgs e)
        {
            ToolBuilder tb = new ToolBuilder(toolPanel, "Export selection");
            TextBox textbox = tb.addTextbox("Layer name:");

            Layer cl = (Layer)layerList.SelectedItem;
            if (cl != null)
                textbox.Text = cl.Name + "_copy";
            textbox.Focus();
            textbox.Select(textbox.Text.Length, 0);

            Label errorLabel = tb.addErrorLabel();
            Button selectButton = tb.addButton("Copy selection", (Layer l) =>
            {
                if (textbox.Text.Length == 0)
                {
                    errorLabel.Text = "Provide a name";
                    return;
                }

                Layer newl = new Layer(textbox.Text, l.shapetype);
                newl.dataTable = l.dataTable;
                newl.boundingbox = l.boundingbox;
                newl.createQuadTree();
                foreach (Feature f in l.selected)
                    newl.addFeature(new Feature((Geometry)f.geometry.Clone(), f.id));
                layers.Insert(0, newl);
                layerList.SelectedItem = newl;
            });
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            ToolBuilder tb = new ToolBuilder(toolPanel, "Delete selection");
            tb.addLabel("Are you sure?");
            tb.addButton("Yes", (Layer l) =>
            {
                foreach (Feature f in l.selected)
                    l.features.Remove(f.id);
                l.clearSelected();
                l.createQuadTree();
                setStatusText("0 objects");
                redraw();
            });
            tb.addButton("No", (l) => { });
        }

        private void bufferButton_Click(object sender, EventArgs e)
        {
            Layer cl = (Layer)layerList.SelectedItem;
            ToolBuilder tb = new ToolBuilder(toolPanel, "Buffer");

            TextBox distBox = tb.addTextbox("Distance:");
            TextBox nameBox = tb.addTextbox("Layer name:", (cl != null) ? cl.Name : "");
            Label errorLabel = tb.addErrorLabel();
            Button selectButton = tb.addButton("Buffer",(Layer l) =>
            {
                double dist = 0;
                if (!double.TryParse(distBox.Text, out dist))
                {
                    tb.setError("Not a number");
                    return;
                }
                if (nameBox.Text.Length == 0)
                {
                    tb.setError("Provide a name");
                    return;
                }

                Layer newl = new Layer(nameBox.Text, ShapeType.POLYGON);
                newl.dataTable = l.dataTable;
                newl.boundingbox = l.boundingbox;
                newl.createQuadTree();

                List<Feature> flist = l.features.Values.ToList();

                progressLabel.Text = "Buffer";
                progressLabel.Invalidate();
                progressBar.Minimum = 0;
                progressBar.Maximum = flist.Count;

                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.DoWork += (object wsender, DoWorkEventArgs we) =>
                {
                    for (int i = 0; i < flist.Count; i++)
                    {
                        bw.ReportProgress(i);
                        Feature f = flist[i];
                        newl.addFeature(new Feature(f.geometry.Buffer(dist), f.id));
                    }
                };
                bw.RunWorkerCompleted += (object wsender, RunWorkerCompletedEventArgs we) =>
                {
                    progressBar.Value = 0;
                    progressLabel.Text = "";

                    layers.Insert(0, newl);
                    layerList.SelectedItem = newl;
                };
                bw.ProgressChanged += (object wsender, ProgressChangedEventArgs we) =>{
                    
                        progressBar.Value = we.ProgressPercentage;
                };
                bw.RunWorkerAsync();
            });
        }
    }
}
