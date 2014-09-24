using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using Proj4CSharp;
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
        public static SGIS App { get; private set; }
        public BindingList<Layer> Layers {get;private set;}
        public ScreenManager ScreenManager {get;private set;}
        public IProjection SRS { get; private set; }
        private Bitmap map;
        private Envelope mapRect;
        private bool mapDirty;
        BackgroundWorker bw = new BackgroundWorker();

        public SGIS()
        {
            InitializeComponent();

            Layers = new BindingList<Layer>();
            ScreenManager = new ScreenManager();
            SRS = Proj4CSharp.Proj4CSharp.ProjectionFactory("+proj = utm + zone = 33 + datum = WGS84 + units = m + no_defs");
            SelectionChanged += selectionChangedHandler;
        }

        private void SGIS_Load(object sender, EventArgs e)
        {
            App = this;

            this.MouseWheel += new MouseEventHandler(SGIS_MouseWheel);

            // Screenmanager for converting between screenspace and real world
            ScreenManager.WindowsRect = new ScreenManager.SGISEnvelope(0, mapWindow.Width, 0, mapWindow.Height);
            ScreenManager.RealRect = new ScreenManager.SGISEnvelope(0, 100, 0, 100);
            ScreenManager.Calculate();
            
            map = new Bitmap(mapWindow.Width, mapWindow.Height);
            mapRect = ScreenManager.RealRect.Clone();
            mapDirty = true;

            // List of layers
            layerList.DataSource = Layers;

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
            if (mapDirty)
            {
                mapDirty = false;
                Bitmap mapTemp = new Bitmap(mapWindow.Width, mapWindow.Height);
                var mapRectTemp = ScreenManager.MapScreenToReal(ScreenManager.WindowsRect);

                if (bw.IsBusy)
                {
                    bw.CancelAsync();
                    while(bw.IsBusy)
                        Application.DoEvents();
                }
                bw = new BackgroundWorker();
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += (obj, args) =>
                {
                    OgcCompliantGeometryFactory fact = new OgcCompliantGeometryFactory();
                    var boundingGeometry = fact.ToGeometry(mapRectTemp);
                    var mapGraphics = Graphics.FromImage(mapTemp);

                    foreach (Layer l in Layers.Reverse())
                    {
                        if (!l.Visible)
                            continue;
                        var visibleFeatures = l.getWithin(boundingGeometry);
                        Render render = new Render(ScreenManager.Scale, ScreenManager.Offset);
                        foreach (Feature s in visibleFeatures)
                        {
                            if (bw.CancellationPending){
                                args.Cancel = true;
                                return;
                            }
                            if (!s.Selected || l != layerList.SelectedItem)
                                render.Draw(s.Geometry, mapGraphics, l.Style);
                            else if (l == layerList.SelectedItem)
                                render.Draw(s.Geometry, mapGraphics, Style.Selected);
                        }
                        //if (l.QuadTree != null)
                        //    l.QuadTree.render(e.Graphics);
                    }
                };
                bw.RunWorkerCompleted += (obj, args) =>
                {
                    if (!args.Cancelled)
                    {
                        map = mapTemp;
                        mapRect = mapRectTemp;
                        redrawDirty();
                    }
                };
                bw.RunWorkerAsync();
            }

            var screenRect = ScreenManager.MapRealToScreen(mapRect);

            e.Graphics.DrawImage(map, screenRect);
            renderScale(e.Graphics);
            mouse.render(e.Graphics);
        }

        private void renderScale(Graphics graphics)
        {
            double mpp = 1/ScreenManager.Scale.X;
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
            ScreenManager.WindowsRect = new ScreenManager.SGISEnvelope(0, mapWindow.Width, 0, mapWindow.Height);
            ScreenManager.Calculate();
            redraw();
        }

        // Getters and convenience functions

        public string StatusText{ set { statusLabel.Text = value; } }
        public string CoordText { set { coordLabel.Text = value;  } }

        public new System.Drawing.Point MousePosition
        {
            get
            {
                var mouse = new System.Drawing.Point(Cursor.Position.X, Cursor.Position.Y);
                mouse = mapWindow.PointToClient(mouse);
                return mouse;
            }
        }
        public PictureBox MapWindow { get { return mapWindow; } }
        public ContextMenuStrip InfoMenu { get { return infoContextMenu; } }

        private Color chooseColor(Color c)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
                return cd.Color;
            return c;
        }

        public void redraw()
        {
            mapDirty = true;
            mapWindow.Refresh();
        }

        public void redrawDirty()
        {
            mapWindow.Refresh();
        }
    }
}
