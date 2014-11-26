﻿using GeoAPI.Geometries;
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
    public partial class SGIS : Form
    {
        public static SGIS App { get; private set; }
        public BindingList<Layer> Layers {get;private set;}
        public ScreenManager ScreenManager {get;private set;}
        public IProjection SRS { get; private set; }
        private Bitmap map;
        private Envelope mapRect;
        private bool mapDirty;
        private List<Photo> photos;
        private BackgroundWorker bw = new BackgroundWorker();

        public SGIS()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            photos = new List<Photo>();
            Layers = new BindingList<Layer>();
            ScreenManager = new ScreenManager();
            SRS = Proj4CSharp.Proj4CSharp.ProjectionFactoryFromName("EPSG:32633"); // UTM 32N
            SelectionChanged += selectionChangedHandler;
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

        private void SGIS_Paint(object sender, PaintEventArgs e)
        {
            if (mapDirty)
            {
                mapDirty = false;
                Bitmap mapTemp = new Bitmap(mapWindow.Width, mapWindow.Height);
                var mapRectTemp = ScreenManager.MapScreenToReal(ScreenManager.WindowsRect);
                Layer selectedLayer = (Layer)layerList.SelectedItem;
                OgcCompliantGeometryFactory fact = new OgcCompliantGeometryFactory();
                var boundingGeometry = fact.ToGeometry(mapRectTemp);
                Render render = new Render(ScreenManager.Scale, ScreenManager.Offset);

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
                    var mapGraphics = Graphics.FromImage(mapTemp);

                    foreach(Photo p in photos)
                    {
                        if (p.Geometry.Intersects(boundingGeometry))
                            p.Draw(mapGraphics);
                    }

                    foreach (Layer l in Layers.Reverse())
                    {
                        if (!l.Visible)
                            continue;
                        var visibleFeatures = l.getWithin(boundingGeometry);
                        lock (l)
                        {
                            foreach (Feature s in visibleFeatures)
                            {
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
                            //if (l.QuadTree != null)
                            //    l.QuadTree.render(e.Graphics);
                        }
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

            try
            {
                var screenRect = ScreenManager.MapRealToScreen(mapRect);
                e.Graphics.DrawImage(map, screenRect);
            }
            catch (Exception ex)
            {
                // Will happen after changing SRS
            }


            if (!SRS.IsLatLong)
                renderScale(e.Graphics);
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
            System.Drawing.Point start = new System.Drawing.Point(20, this.Height-120);
            System.Drawing.Point end = new System.Drawing.Point(120, this.Height-120);
            System.Drawing.Point text = new System.Drawing.Point(30, this.Height-110);
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
