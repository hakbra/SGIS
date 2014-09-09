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

        public SGIS()
        {
            InitializeComponent();
        }

        private void SGIS_Load(object sender, EventArgs e)
        {
            app = this;

            this.MouseWheel += new MouseEventHandler(SGIS_MouseWheel);

            // Screenmanager for converting between screenspace and real world
            screenManager.WindowsRect = new ScreenManager.SGISEnvelope(0, mapWindow.Width, 0, mapWindow.Height);
            screenManager.RealRect = new ScreenManager.SGISEnvelope(0, 100, 0, 100);
            screenManager.Calculate();

            // List of layers
            layerList.DataSource = layers;

            // Context menu for layer list
            layerListContextMenu.Opening += new CancelEventHandler(layerListContextMenu_Opening);

            // Context menu for info pointer
            infoContextMenu.Opening += new CancelEventHandler(infoContextMenu_Opening);

            // Table for tools
            toolPanel.RowStyles.Clear();
            toolPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            toolBuilder = new ToolBuilder(toolPanel);
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
                        Render.Draw(s.geometry, e.Graphics, l.style);
                    else if (l == layerList.SelectedItem)
                        Render.Draw(s.geometry, e.Graphics, Style.Selected);
                }
                //if (l.quadTree != null)
                //    l.quadTree.render(e.Graphics);
                renderScale(e.Graphics);
            }

            mouse.render(e.Graphics);
        }

        private void renderScale(Graphics graphics)
        {
            double mpp = 1/screenManager.Scale.X;
            double meters = mpp * 100;
            string meterstr = meters.ToString("N0");
            if (meters < 10)
                meterstr = meters.ToString("N1");

            Pen p = new Pen(Color.Black);
            System.Drawing.Point start = new System.Drawing.Point(20, this.Height-120);
            System.Drawing.Point end = new System.Drawing.Point(120, this.Height-120);
            System.Drawing.Point text = new System.Drawing.Point(30, this.Height-110);
            graphics.DrawLine(p, start, end);

            Font f = new Font(FontFamily.GenericMonospace, 10);
            graphics.DrawString(meterstr + "m", f, new SolidBrush(Color.Black), text);
        }

        private void SGIS_Resize(object sender, EventArgs e)
        {
            screenManager.WindowsRect = new ScreenManager.SGISEnvelope(0, mapWindow.Width, 0, mapWindow.Height);
            screenManager.Calculate();
            this.Refresh();
        }

        // Getters and convenience functions
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

        private Color chooseColor(Color c)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
                return cd.Color;
            return c;
        }
    }
}
