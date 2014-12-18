using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using Proj4CSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SGIS
{
    // main class of SGIS application. Singleton with instance in App member.
    public partial class SGIS : Form
    {
        // Static instance of application
        public static SGIS App { get; private set; }

        // List of layers
        public BindingList<Layer> Layers {get;private set;}

        // Screen manager for calculating real-world and screen coordinates
        public ScreenManager ScreenManager = new ScreenManager();

        // current projection in use
        public IProjection SRS { get; private set; }

        // temporary storage of drawn map
        private Bitmap map;

        // bounding box of stored map
        private Envelope mapRect;

        // value deciding if map is to be redrawn, or stored map simply moved and scaled
        private bool mapDirty;

        // list of loaded wms-maps
        private List<Photo> photos;

        // background color of map
        private Color mapBgColor;

        // backgroundWorker for rendering refreshed map
        private BackgroundWorker bw = new BackgroundWorker();

        public SGIS()
        {
            InitializeComponent();
            // maximise window on startup
            ScreenManager = new ScreenManager();
            photos = new List<Photo>();
            Layers = new BindingList<Layer>();
            // initialise projectiond to UTM 33N
            SRS = Proj4CSharp.Proj4CSharp.ProjectionFactoryFromName("EPSG:32633"); // UTM 33N
            SelectionChanged += selectionChangedHandler;
            mapBgColor = Color.White;
            AcceptButton = null;
            CancelButton = null;
            this.WindowState = FormWindowState.Maximized;
        }

        private void SGIS_Load(object sender, EventArgs e)
        {
            App = this;

            this.MouseWheel += new MouseEventHandler(SGIS_MouseWheel);

            // Screenmanager for converting between screenspace and real world
            ScreenManager.WindowsRect = new ScreenManager.SGISEnvelope(0, mapWindow.Width, 0, mapWindow.Height);
            ScreenManager.RealRect = new ScreenManager.SGISEnvelope(0, 10000, 0, 10000);
            ScreenManager.ZoomTo(new NetTopologySuite.Geometries.Point(293405, 7039854));
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

        // draws map
        private void SGIS_Paint(object sender, PaintEventArgs e)
        {
            // if map is to be cmpletely redrawn
            if (mapDirty)
            {
                mapDirty = false;
                // create temporary map and bounding box
                Bitmap mapTemp = new Bitmap(mapWindow.Width, mapWindow.Height);
                var mapRectTemp = ScreenManager.MapScreenToReal(ScreenManager.WindowsRect);
                Layer selectedLayer = (Layer)layerList.SelectedItem;
                OgcCompliantGeometryFactory fact = new OgcCompliantGeometryFactory();
                var boundingGeometry = fact.ToGeometry(mapRectTemp);
                // create new render instance
                Render render = new Render(ScreenManager.Scale, ScreenManager.Offset);

                // if we are already rendering a new map
                if (bw.IsBusy)
                {
                    // cancel previous rendering
                    bw.CancelAsync();
                    while(bw.IsBusy)
                        Application.DoEvents();
                }
                bw = new BackgroundWorker();
                bw.WorkerSupportsCancellation = true;
                // start new rendering in other thread
                // this avoids making the application unresponsive
                bw.DoWork += (obj, args) =>
                {
                    var mapGraphics = Graphics.FromImage(mapTemp);

                    // draw wms-maps
                    foreach(Photo p in photos)
                    {
                        if (p.Geometry.Intersects(boundingGeometry))
                            p.Draw(mapGraphics);
                    }

                    // draw layers
                    foreach (Layer l in Layers.Reverse())
                    {
                        // skip layer if not visible
                        if (!l.Visible)
                            continue;
                        // get all visible features in layer
                        var visibleFeatures = l.getWithin(boundingGeometry);
                        lock (l)
                        {
                            // draw feature
                            foreach (Feature s in visibleFeatures)
                            {
                                // abort if this rendering has been cancelled
                                if (bw.CancellationPending)
                                {
                                    args.Cancel = true;
                                    return;
                                }
                                if (!s.Selected || l != selectedLayer)
                                    render.Draw(s.Geometry, mapGraphics, l.Style);
                                else if (l == selectedLayer)
                                    render.Draw(s.Geometry, mapGraphics, Style.Selected);
                            }
                            // render quad tree, only for debug purposes
                            //if (l.QuadTree != null)
                            //    l.QuadTree.render(e.Graphics);
                        }
                    }
                };
                bw.RunWorkerCompleted += (obj, args) =>
                {
                    // if rendering was not cancelled
                    if (!args.Cancelled)
                    {
                        // copy temporary map to stored map
                        map = mapTemp;
                        mapRect = mapRectTemp;
                        // draw new map
                        redrawDirty();
                    }
                };
                bw.RunWorkerAsync();
            }

            try
            {
                // draw map stored in member map
                var screenRect = ScreenManager.MapRealToScreen(mapRect);

                // draw background color
                if (mapBgColor != Color.White)
                    e.Graphics.Clear(mapBgColor);

                e.Graphics.DrawImage(map, screenRect);
            }
            catch (Exception ex)
            {
                // Will happen after changing SRS
            }

            // if current srs is projected, draw scale in lower left corner
            if (!SRS.IsLatLong)
                renderScale(e.Graphics);

            // calls mouse render function, will only draw select-rectangle when select is being used
            mouse.render(e.Graphics);
        }

        private void renderScale(Graphics graphics)
        {
            double mpp = 1/ScreenManager.Scale.X;
            double meters = mpp * 100;
            string meterstr = meters.ToString("N0")+"m";
            if (meters < 10)
                meterstr = meters.ToString("N1")+"m";
            if (meters > 1000)
            {
                meters /= 1000;
                meterstr = meters.ToString("N1") + "km";
            }

            Pen p = new Pen(Color.Black);
            System.Drawing.Point start = new System.Drawing.Point(20, this.Height-150);
            System.Drawing.Point end = new System.Drawing.Point(120, this.Height-150);
            System.Drawing.Point text = new System.Drawing.Point(30, this.Height-140);
            graphics.DrawLine(p, start, end);

            Font f = new Font(FontFamily.GenericMonospace, 10);
            graphics.DrawString(meterstr, f, new SolidBrush(Color.Black), text);
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

        // returns wkt-representation of current srs
        public string getSrsName()
        {
            OSGeo.OSR.SpatialReference oSRS = new OSGeo.OSR.SpatialReference("");
            oSRS.ImportFromProj4(SRS.ToString());

            if (oSRS.IsProjected() == 1)
                return oSRS.GetAttrValue("projcs", 0);
            return oSRS.GetAttrValue("geogcs", 0);
        }
    }
}
