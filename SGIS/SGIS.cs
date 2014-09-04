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
        MouseTactic mouse = new StandardMouseTactic();

        public SGIS()
        {
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(SGIS_MouseWheel);

            layerList.RowStyles.Clear();
            layerList.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            app = this;
        }

        private void SGIS_Load(object sender, EventArgs e)
        {
            screenManager.WindowsRect = new ScreenManager.SGISEnvelope(0, mapWindow.Width, 0, mapWindow.Height);
            screenManager.RealRect = new ScreenManager.SGISEnvelope(0, 100, 0, 100);
            screenManager.Calculate();

            layers.ListChanged += this.PopulateLayerControls;
        }

        private void PopulateLayerControls(object sender, ListChangedEventArgs args)
        {
            var controls = layerList.Controls;
            controls.Clear();
            foreach (Layer l in layers)
            {
                LayerControl lc = new LayerControl();
                lc.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                lc.setLayer(l);
                if (LayerControl.current == null || LayerControl.current.layer == l)
                    LayerControl.current = lc;
                controls.Add(lc);
            }
            LayerControl.current.BackColor = Color.LightBlue;
        }

        private void SGIS_Paint(object sender, PaintEventArgs e)
        {

            foreach (Layer l in layers.Reverse())
            {
                if (!l.visible)
                    continue;
                foreach (Feature s in l.shapes.Values)
                {
                    if (!s.selected || l != LayerControl.current.layer)
                        Render.Draw(s.geometry, e.Graphics, l.color);
                    else if(l == LayerControl.current.layer)
                        Render.Draw(s.geometry, e.Graphics, Color.DarkCyan);
                }
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
    }
}
