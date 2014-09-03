using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoLib;

namespace SGIS
{
    public partial class SGIS : Form
    {
        public BindingList<Layer> layers = new BindingList<Layer>();
        GeoLib.CGeoDraw render = new GeoLib.CGeoDraw();
        public GeoLib.CScreenManager screen = new GeoLib.CScreenManager();
        MouseTactic mouse = new StandardMouseTactic();

        public SGIS()
        {
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(SGIS_MouseWheel);

            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            screen.WindowsRect = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
            screen.RealRect.Set(0, 0, 100, 100);
            screen.Calculate();

            layers.ListChanged += this.PopulateLayerControls;
            LayerControl.sgis = this;
            MouseTactic.sgis = this;
        }

        private void PopulateLayerControls(object sender, ListChangedEventArgs args)
        {
            var controls = tableLayoutPanel1.Controls;
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
            render.Scale = screen.Scale;
            render.Offset = screen.Offset;

            foreach (Layer l in layers.Reverse())
            {
                if (!l.visible)
                    continue;
                foreach (GeoLib.ShapeInterface s in l.shapes.Values)
                    s.Draw(render, e.Graphics, l.color);
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
            screen.WindowsRect = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
            screen.Calculate();
            this.Refresh();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void shapefileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "shp files|*.shp";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ShapeReader sr = new ShapeReader();
                Layer l = sr.read(openFileDialog1.FileName);
                layers.Insert(0, l);
                screen.RealRect.Set(l.boundingbox);
                screen.Calculate();
                this.Refresh();
            }
        }

        public void redraw()
        {
            pictureBox1.Refresh();
        }

        public void setStatusText(string s)
        {
            toolStripStatusLabel1.Text = s;
        }

        public System.Drawing.Point getMousePos()
        {
            var mouse = new System.Drawing.Point(Cursor.Position.X, Cursor.Position.Y);
            mouse = pictureBox1.PointToClient(mouse);
            return mouse;
        }
    }
}
