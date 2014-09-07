using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
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
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Color...", colorImg, (o, i) => { l.color = chooseColor(l.color); redraw(); }));
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
            }

            mouse.render(e.Graphics);
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
            toolPanel.Controls.Clear();

            Label lineLabel = new Label();
            lineLabel.Dock = DockStyle.Top;
            lineLabel.Height = 1;
            lineLabel.BorderStyle = BorderStyle.FixedSingle;

            Label label = new Label();
            label.Text = "Select by property";
            label.Anchor = AnchorStyles.None;

            TextBox textbox = new TextBox();
            textbox.Anchor = AnchorStyles.None;

            ComboBox comboBox = new ComboBox();
            comboBox.Items.Add("Column name");
            comboBox.SelectedIndex = 0;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Anchor = AnchorStyles.None;

            Label errorLabel = new Label();
            errorLabel.ForeColor = Color.Red;
            errorLabel.Anchor = AnchorStyles.None;

            Button selectButton = new Button();
            selectButton.Text = "Select";
            selectButton.Anchor = AnchorStyles.None;

            toolPanel.Controls.Add(label);
            toolPanel.Controls.Add(lineLabel);
            toolPanel.Controls.Add(textbox);
            toolPanel.Controls.Add(comboBox);
            toolPanel.Controls.Add(errorLabel);
            toolPanel.Controls.Add(selectButton);

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

            selectButton.Click += (o, ev) =>
            {
                Layer l = (Layer)layerList.SelectedItem;
                if (l == null || l.dataTable == null)
                    return;
                try
                {
                    DataRow[] rows = l.dataTable.Select(textbox.Text);
                    errorLabel.Text = "";
                    l.clearSelected();

                    foreach (DataRow dr in rows)
                    {
                        int id = (int)dr[0];
                        Feature f = l.features[id];
                        f.selected = true;
                        l.selected.Add(f);
                    }
                    SGIS.app.redraw();
                }
                catch (Exception ex)
                {
                    errorLabel.Text = "Error: " + ex.Message;
                    errorLabel.Width = TextRenderer.MeasureText(errorLabel.Text, errorLabel.Font).Width;
                }
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
    }
}
