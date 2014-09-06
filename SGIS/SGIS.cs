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
            //layerList.ContextMenuStrip = layerListContextMenu;
            //layerList.SelectedIndexChanged += (o, i) => { this.redraw(); };
            //layers.ListChanged += (o, i) => { this.redraw(); };
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
            redraw();
        }

        private void mouseMoveItem_MouseDown(object sender, MouseEventArgs e)
        {
            mouse = new MoveMouseTactic();
            mouseInfoItem.Enabled = true;
            mouseSelectItem.Enabled = true;
        }

        private void mouseSelectItem_Click(object sender, EventArgs e)
        {
            mouse = new SelectMouseTactic();
            mouseMoveItem.Enabled = true;
            mouseInfoItem.Enabled = true;
        }

        private void mouseInfoItem_Click(object sender, EventArgs e)
        {
            mouse = new InfoMouseTactic();
            mouseMoveItem.Enabled = true;
            mouseSelectItem.Enabled = true;
        }
    }
}
