using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Union;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SGIS
{
    public partial class SGIS
    {
        ToolBuilder toolBuilder;

        private void addButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "shp files|*.shp";
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                progressBar.Minimum = 0;
                progressBar.Maximum = openFileDialog1.FileNames.Count();
                progressBar.Value = 0;

                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;

                List<Layer> readLayers = new List<Layer>();

                bw.DoWork += (object wsender, DoWorkEventArgs we) =>
                {
                    foreach (String file in openFileDialog1.FileNames)
                    {
                        bw.ReportProgress(0, file.Split('\\').Last());
                        Layer l = ShpReader.read(file);
                        bw.ReportProgress(1);
                        readLayers.Insert(0, l);
                    }
                };
                bw.RunWorkerCompleted += (object wsender, RunWorkerCompletedEventArgs we) =>
                {
                    progressBar.Value = 0;
                    progressLabel.Text = "";

                    foreach (Layer l in readLayers)
                    {
                        Layers.Insert(0, l);
                        ScreenManager.RealRect.Set(l.Boundingbox);
                    }

                    layerList.SelectedIndex = 0;
                    ScreenManager.Calculate();
                    redraw();
                };
                bw.ProgressChanged += (object wsender, ProgressChangedEventArgs we) =>
                {
                    progressBar.Value += we.ProgressPercentage;
                    if (we.ProgressPercentage == 0)
                        progressLabel.Text = "Reading " + (string)we.UserState;
                };
                bw.RunWorkerAsync();
            }
        }

        private void delButton_Click(object sender, EventArgs e)
        {
            if (layerList.Items.Count == 0)
                return;
            toolBuilder.addHeader("Delete layer");
            toolBuilder.addLabel("Are you sure?");
            toolBuilder.addButton("Yes", (Layer il) =>
            {
                SGIS.App.Layers.Remove(il);
                redraw();
            });
            toolBuilder.addButton("No", (il) => { });
            toolBuilder.resetAction = (Layer il) =>
            {
                toolBuilder.clear();
            };
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            Layer selected = (Layer)layerList.SelectedItem;
            if (selected == null)
                return;
            int index = Layers.IndexOf(selected);
            if (index == 0)
                return;
            Layers.Remove(selected);
            Layers.Insert(index - 1, selected);
            layerList.SelectedItem = selected;
            redraw();
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            Layer selected = (Layer)layerList.SelectedItem;
            int index = Layers.IndexOf(selected);
            if (index == Layers.Count - 1)
                return;
            Layers.Remove(selected);
            Layers.Insert(index + 1, selected);
            layerList.SelectedItem = selected;
            redraw();
        }

        private void bufferButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Buffer");

            TextBox distBox = toolBuilder.addTextboxWithCaption("Distance:");
            TextBox nameBox = toolBuilder.addTextboxWithCaption("Layer name:");
            Label errorLabel = toolBuilder.addErrorLabel();

            Button selectButton = toolBuilder.addButton("Buffer", (Layer l) =>
            {
                double dist = 0;
                if (!double.TryParse(distBox.Text, out dist))
                {
                    toolBuilder.setError("Not a number");
                    return;
                }
                if (nameBox.Text.Length == 0)
                {
                    toolBuilder.setError("Provide a name");
                    return;
                }

                Layer newl = new Layer(nameBox.Text);
                newl.DataTable = l.DataTable;
                newl.Boundingbox = l.Boundingbox;
                newl.createQuadTree();

                List<Feature> flist = l.Features.Values.ToList();

                progressLabel.Text = "Buffering";
                progressBar.Minimum = 0;
                progressBar.Maximum = flist.Count;
                ConcurrentBag<Feature> newFeatures = new ConcurrentBag<Feature>();

                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.DoWork += (object wsender, DoWorkEventArgs we) =>
                {
                    using (var finished = new CountdownEvent(1))
                    {
                        for (int i = 0; i < flist.Count; i++)
                        {
                            finished.AddCount();
                            Feature capture = flist[i];
                            ThreadPool.QueueUserWorkItem((state) =>
                            {
                                Feature f = capture;
                                newFeatures.Add(new Feature(f.Geometry.Buffer(dist), f.ID));
                                bw.ReportProgress(i);
                                finished.Signal();
                            }, null);
                        }
                        finished.Signal();
                        finished.Wait();
                    }
                    bw.ReportProgress(-1);
                    foreach(Feature f in newFeatures)
                    {
                        newl.addFeature(f);
                        bw.ReportProgress(1);
                    }
                };
                bw.RunWorkerCompleted += (object wsender, RunWorkerCompletedEventArgs we) =>
                {
                    progressBar.Value = 0;
                    progressLabel.Text = "";

                    Layers.Insert(0, newl);
                    layerList.SelectedItem = newl;
                };
                bw.ProgressChanged += (object wsender, ProgressChangedEventArgs we) =>
                {
                    if (we.ProgressPercentage == -1){
                        progressBar.Value = 0;
                        progressLabel.Text = "Creating spatial index";
                    }
                    else
                        progressBar.Value += 1;
                };
                bw.RunWorkerAsync();
            });

            toolBuilder.resetAction = (Layer l) =>
            {
                nameBox.Text = (l == null) ? "" : l.Name + "_buffer";
            };

            toolBuilder.reset();
        }

        private void mergeButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Union");
            ComboBox layerSelect = toolBuilder.addLayerSelect("Merge with:");
            TextBox textbox = toolBuilder.addTextboxWithCaption("New layername:");
            Label errorLabel = toolBuilder.addErrorLabel();
            Button button = toolBuilder.addButton("Merge", (Layer l) =>
            {
                if (textbox.Text.Length == 0)
                {
                    toolBuilder.setError("Provide name");
                    return;
                }
                Layer unionLayer = (Layer) layerSelect.SelectedItem;
                if (l.shapetype != unionLayer.shapetype)
                {
                    toolBuilder.setError("Incompatible types");
                    return;
                }
                Layer newLayer = new Layer(textbox.Text);
                newLayer.Boundingbox = new Envelope(l.Boundingbox);
                newLayer.Boundingbox.ExpandToInclude(unionLayer.Boundingbox);
                newLayer.createQuadTree();

                foreach (Feature f in l.Features.Values)
                    newLayer.addFeature(new Feature((IGeometry)f.Geometry.Clone()));
                foreach (Feature f in unionLayer.Features.Values)
                    newLayer.addFeature(new Feature((IGeometry)f.Geometry.Clone()));

                Layers.Insert(0, newLayer);
                redraw();
            });
            toolBuilder.resetAction = (Layer l) =>
            {
                textbox.Text = (l == null) ? "" : l.Name + "_merge";
            };
            toolBuilder.reset();
        }

        private void unionButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Union");
            TextBox textbox = toolBuilder.addTextboxWithCaption("New layername:");
            Label errorLabel = toolBuilder.addErrorLabel();
            Button button = toolBuilder.addButton("Union", (Layer l) =>
            {
                if (textbox.Text.Length == 0)
                {
                    toolBuilder.setError("Provide name");
                    return;
                }
                Layer copyLayer = new Layer(l.Name);
                copyLayer.Boundingbox = new Envelope(l.Boundingbox);
                copyLayer.createQuadTree();

                foreach (Feature f in l.Features.Values)
                    copyLayer.addFeature(new Feature((IGeometry)f.Geometry.Clone(), f.ID));

                Layer newLayer = new Layer(textbox.Text);
                newLayer.Boundingbox = new Envelope(l.Boundingbox);
                newLayer.createQuadTree();

                int numFeatures = copyLayer.Features.Values.Count;
                progressLabel.Text = "Performing union";
                progressBar.Minimum = 0;
                progressBar.Maximum = numFeatures;
                
                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.DoWork += (object wsender, DoWorkEventArgs we) =>
                {
                    ConcurrentBag<Feature> newFeatures = new ConcurrentBag<Feature>();
                    var finished = new CountdownEvent(1);
                    Object _lock = new object();
                    var merge = new WaitCallback((state) =>
                    {
                        Random rnd = new Random();
                        while (true)
                        {
                            Feature f;
                            lock (_lock)
                            {
                                if (copyLayer.Features.Count == 0)
                                    break;
                                int index = rnd.Next(copyLayer.Features.Count);
                                f = copyLayer.Features[copyLayer.Features.Keys.ToList()[index]];
                                copyLayer.delFeature(f);
                            }
                            f.ID = -1;
                            while (true)
                            {
                                List<Feature> intersects;
                                lock (_lock)
                                {
                                    intersects = copyLayer.getWithin(f.Geometry);
                                    foreach (Feature intersect in intersects)
                                        copyLayer.delFeature(intersect);
                                }
                                if (intersects.Count == 0)
                                    break;
                                foreach (Feature intersect in intersects)
                                {
                                    f = new Feature(f.Geometry.Union(intersect.Geometry));
                                    bw.ReportProgress(1);
                                }
                            }
                            newFeatures.Add(f);
                        }
                        finished.Signal();
                    });

                    for (int i = 0; i < 8; i++)
                    {
                        finished.AddCount();
                        ThreadPool.QueueUserWorkItem(merge);
                    }
                    finished.Signal();
                    finished.Wait();

                    bw.ReportProgress(-newFeatures.Count);
                    foreach (Feature f in newFeatures)
                        copyLayer.addFeature(f);
                    newFeatures = new ConcurrentBag<Feature>();
                    finished = new CountdownEvent(1);
                    merge(false);

                    foreach (Feature f in newFeatures)
                    {
                        newLayer.addFeature(f);
                    }
                };
                bw.RunWorkerCompleted += (object wsender, RunWorkerCompletedEventArgs we) =>
                {
                    progressBar.Value = 0;
                    progressLabel.Text = "";

                    Layers.Insert(0, newLayer);
                    redraw();
                };
                bw.ProgressChanged += (object wsender, ProgressChangedEventArgs we) =>
                {
                    if (we.ProgressPercentage < 0)
                    {
                        progressBar.Value = 0;
                        progressBar.Maximum = -we.ProgressPercentage;
                        progressLabel.Text = "Union - Second pass";
                    }
                    else
                        progressBar.Value += we.ProgressPercentage;
                };
                bw.RunWorkerAsync();
            });
            toolBuilder.resetAction = (Layer l) => {
                textbox.Text = (l == null) ? "" : l.Name + "_union";
            };
            toolBuilder.reset();
        }

        private void diffButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Difference");
            ComboBox layerSelect = toolBuilder.addLayerSelect("Subtract:");
            TextBox textbox = toolBuilder.addTextboxWithCaption("New layername:");
            Label errorLabel = toolBuilder.addErrorLabel();
            Button button = toolBuilder.addButton("Subtract", (Layer l) =>
            {
                if (textbox.Text.Length == 0)
                {
                    toolBuilder.setError("Provide name");
                    return;
                }
                Layer unionLayer = (Layer)layerSelect.SelectedItem;
                if (l.shapetype != unionLayer.shapetype)
                {
                    toolBuilder.setError("Incompatible types");
                    return;
                }
                Layer newLayer = new Layer(textbox.Text);
                newLayer.DataTable = l.DataTable;
                newLayer.Boundingbox = new Envelope(l.Boundingbox);
                newLayer.createQuadTree();
                
                progressLabel.Text = "Subtracting";
                progressBar.Minimum = 0;
                progressBar.Maximum = l.Features.Values.Count;

                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.DoWork += (object wsender, DoWorkEventArgs we) =>
                {
                    ConcurrentBag<Feature> newFeatures = new ConcurrentBag<Feature>();
                    using (var finished = new CountdownEvent(1))
                    {
                        foreach (Feature f in l.Features.Values)
                        {
                            finished.AddCount();
                            Feature capt = f;
                            ThreadPool.QueueUserWorkItem((state) =>
                            {
                                Feature newf = new Feature((IGeometry)capt.Geometry.Clone(), capt.ID);
                                var intersects = unionLayer.getWithin(capt.Geometry);
                                foreach (Feature intersect in intersects)
                                    newf.Geometry = newf.Geometry.Difference(intersect.Geometry);

                                if (!newf.Geometry.IsEmpty)
                                    newFeatures.Add(newf);
                                bw.ReportProgress(1);
                                finished.Signal();
                            }, null);
                        }
                        finished.Signal();
                        finished.Wait();
                    }
                    bw.ReportProgress(-newFeatures.Count);
                    foreach (Feature f in newFeatures)
                    {
                        newLayer.addFeature(f);
                        bw.ReportProgress(1);
                    }
                };
                bw.RunWorkerCompleted += (object wsender, RunWorkerCompletedEventArgs we) =>
                {
                    progressBar.Value = 0;
                    progressLabel.Text = "";

                    Layers.Insert(0, newLayer);
                    redraw();
                };
                bw.ProgressChanged += (object wsender, ProgressChangedEventArgs we) =>
                {
                    if (we.ProgressPercentage < 0)
                    {
                        progressBar.Value = 0;
                        progressBar.Maximum = -we.ProgressPercentage;
                        progressLabel.Text = "Creating spatial index";
                    }
                    else
                        progressBar.Value += we.ProgressPercentage;
                };
                bw.RunWorkerAsync();
            });
            toolBuilder.resetAction += (Layer l) =>
            {
                textbox.Text = (l == null) ? "" : l.Name + "_diff";
            };
            toolBuilder.reset();
        }

        private void intersectButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Intersect");
            ComboBox layerSelect = toolBuilder.addLayerSelect("Intersect with:");
            TextBox textbox = toolBuilder.addTextboxWithCaption("New layername:");
            Label errorLabel = toolBuilder.addErrorLabel();
            Button button = toolBuilder.addButton("Intersect", (Layer l) =>
            {
                if (textbox.Text.Length == 0)
                {
                    toolBuilder.setError("Provide name");
                    return;
                }
                Layer intersectLayer = (Layer)layerSelect.SelectedItem;
                Layer newLayer = new Layer(textbox.Text);
                newLayer.Boundingbox = new Envelope(l.Boundingbox);
                newLayer.createQuadTree();

                if (l.DataTable != null && intersectLayer.DataTable != null)
                {
                    DataTable a = l.DataTable.Clone();
                    newLayer.DataTable = intersectLayer.DataTable.Clone();
                    newLayer.DataTable.Merge(a, true, MissingSchemaAction.Add);
                } 
                else if (l.DataTable != null && intersectLayer.DataTable == null)
                {
                    newLayer.DataTable = l.DataTable.Clone();
                }
                else if (l.DataTable == null && intersectLayer.DataTable != null)
                {
                    newLayer.DataTable = intersectLayer.DataTable.Clone();
                }

                progressLabel.Text = "Intersection";
                progressBar.Minimum = 0;
                progressBar.Maximum = l.Features.Values.Count;

                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.DoWork += (object wsender, DoWorkEventArgs we) =>
                {
                    for (int i = 0; i < l.Features.Count; i++ )
                    {
                        Feature f = l.Features.Values.ElementAt(i);
                        bw.ReportProgress(i);
                        var intersections = intersectLayer.getWithin(f.Geometry);
                        foreach (Feature intersect in intersections)
                        {
                            DataRow arow = l.getRow(f);
                            DataRow brow = intersectLayer.getRow(intersect);

                            Feature result = new Feature(f.Geometry.Intersection(intersect.Geometry));
                            int id = newLayer.addFeature(result);

                            if (newLayer.DataTable != null)
                            {
                                DataRow dr = newLayer.DataTable.NewRow();
                                if (arow != null)
                                    foreach (DataColumn dc in arow.Table.Columns)
                                        dr[dc.ColumnName] = arow[dc.ColumnName];
                                if (brow != null)
                                    foreach (DataColumn dc in brow.Table.Columns)
                                        dr[dc.ColumnName] = brow[dc.ColumnName];
                                dr["sgis_id"] = id;
                                newLayer.DataTable.Rows.Add(dr);
                            }
                        }
                    }
                };
                bw.RunWorkerCompleted += (object wsender, RunWorkerCompletedEventArgs we) =>
                {
                    progressBar.Value = 0;
                    progressLabel.Text = "";

                    newLayer.calculateBoundingBox();
                    newLayer.createQuadTree();
                    Layers.Insert(0, newLayer);
                    redraw();
                };
                bw.ProgressChanged += (object wsender, ProgressChangedEventArgs we) =>
                {
                    progressBar.Value = we.ProgressPercentage;
                };
                bw.RunWorkerAsync();
            });
            toolBuilder.resetAction += (Layer l) =>
            {
                textbox.Text = (l == null) ? "" : l.Name + "_intersect";
            };
            toolBuilder.reset();
        }

        private void measureButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Measurements");
            Label selectedLabel = toolBuilder.addLabel("Selected:");
            selectedLabel.Font = new Font(selectedLabel.Font, FontStyle.Underline);
            Label selCountLabel = toolBuilder.addLabel("");
            Label selLengthLabel = toolBuilder.addLabel("");
            Label selAreaLabel = toolBuilder.addLabel("");
            Label totalLabel = toolBuilder.addLabel("Total:");
            totalLabel.Font = new Font(selectedLabel.Font, FontStyle.Underline);
            Label countLabel = toolBuilder.addLabel("");
            Label lengthLabel = toolBuilder.addLabel("");
            Label areaLabel = toolBuilder.addLabel("");

            selCountLabel.Height = selLengthLabel.Height = selAreaLabel.Height = (int)(selAreaLabel.Height * 0.7);
            selAreaLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            countLabel.Height = lengthLabel.Height = areaLabel.Height = (int)(areaLabel.Height * 0.7);

            toolBuilder.resetAction += (Layer l) =>
            {
                {   // Selected measurements
                    string plural = "";
                    if (l != null && l.Selected.Count > 0)
                        plural = "s";

                    if (l == null || l.shapetype == ShapeType.EMPTY)
                    {
                        selCountLabel.Text = "No feature" + plural;
                        selLengthLabel.Text = "Length: N/A";
                        selAreaLabel.Text = "Area: N/A";
                    }
                    else if (l.shapetype == ShapeType.POINT)
                    {
                        selCountLabel.Text = l.Selected.Count + " points";
                        selLengthLabel.Text = "Length: N/A";
                        selAreaLabel.Text = "Area: N/A";
                    }
                    else if (l.shapetype == ShapeType.LINE)
                    {
                        selCountLabel.Text = l.Selected.Count + " feature" + plural;
                        double length = 0;
                        foreach (Feature f in l.Selected)
                            length += f.Geometry.Length;
                        selLengthLabel.Text = "Length: " + formatLength(length);
                        selAreaLabel.Text = "Area: N/A";
                    }
                    else if (l.shapetype == ShapeType.POLYGON)
                    {
                        selCountLabel.Text = l.Selected.Count + " feature" + plural;
                        double circum = 0;
                        double area = 0;
                        foreach (Feature f in l.Selected)
                        {
                            circum += f.Geometry.Length;
                            area += f.Geometry.Area;
                        }
                        selLengthLabel.Text = "Circ: " + formatLength(circum);
                        selAreaLabel.Text = "Area: " + formatArea(area);
                    }
                }
                {   // Total measurements
                    string plural = "";
                    if (l != null && l.Features.Count > 0)
                        plural = "s";

                    if (l == null || l.shapetype == ShapeType.EMPTY)
                    {
                        countLabel.Text = "No feature" + plural;
                        lengthLabel.Text = "Length: N/A";
                        areaLabel.Text = "Area: N/A";
                    }
                    else if (l.shapetype == ShapeType.POINT)
                    {
                        countLabel.Text = l.Features.Count + " points";
                        lengthLabel.Text = "Length: N/A";
                        areaLabel.Text = "Area: N/A";
                    }
                    else if (l.shapetype == ShapeType.LINE)
                    {
                        countLabel.Text = l.Features.Count + " feature" + plural;
                        double length = 0;
                        foreach (Feature f in l.Features.Values)
                            length += f.Geometry.Length;
                        lengthLabel.Text = "Length: " + formatLength(length);
                        areaLabel.Text = "Area: N/A";
                    }
                    else if (l.shapetype == ShapeType.POLYGON)
                    {
                        countLabel.Text = l.Features.Count + " feature" + plural;
                        double circum = 0;
                        double area = 0;
                        foreach (Feature f in l.Features.Values)
                        {
                            circum += f.Geometry.Length;
                            area += f.Geometry.Area;
                        }
                        lengthLabel.Text = "Circ: " + formatLength(circum);
                        areaLabel.Text = "Area: " + formatArea(area);
                    }
                }
            };
            toolBuilder.reset();
        }

        private string formatLength(double meters)
        {
            if (meters < 1000)
                return meters.ToString("N1") + " m";
            meters /= 1000;
            return meters.ToString("N1") + " km";
        }
        private string formatArea(double sqmeters)
        {
            if (sqmeters < 100000)
                return sqmeters.ToString("N1") + " m^2";
            sqmeters /= 1000000;
            return sqmeters.ToString("N1") + " km^2";
        }

        private void renderButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Export render");
            var zoom = toolBuilder.addTextbox("Zoom factor:", "1");
            var file = toolBuilder.addTextbox("Filename:", "");
            var browsebutton = toolBuilder.addButton("Browse...");
            var error = toolBuilder.addErrorLabel();

            browsebutton.Click += (o, w) =>
            {
                 SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                 saveFileDialog1.Filter = "bmp files (*.bmp)|*.bmp"  ;
                 if(saveFileDialog1.ShowDialog() == DialogResult.OK)
                     file.Text = saveFileDialog1.FileName;
            };

            Button button = toolBuilder.addButton("Export", (Layer selectedLayer) =>
            {
                if (file.Text == "")
                {
                    toolBuilder.setError("Please provide filename");
                    return;
                }
                double zoomfactor = 1;
                if (!double.TryParse(zoom.Text, out zoomfactor))
                {
                    toolBuilder.setError("Zoom factor not a number");
                    return;
                }

                var oldWindowRect = ScreenManager.WindowsRect.Clone();
                ScreenManager.WindowsRect = new ScreenManager.SGISEnvelope(0, oldWindowRect.MaxX * zoomfactor, 0, oldWindowRect.MaxY * zoomfactor);
                ScreenManager.Calculate();
                Bitmap mapTemp = new Bitmap((int)ScreenManager.WindowsRect.Width,
                                            (int)ScreenManager.WindowsRect.Height);
                var mapRectTemp = ScreenManager.MapScreenToReal(ScreenManager.WindowsRect);
                OgcCompliantGeometryFactory fact = new OgcCompliantGeometryFactory();
                var boundingGeometry = fact.ToGeometry(mapRectTemp);
                Render render = new Render(ScreenManager.Scale, ScreenManager.Offset);
                ScreenManager.WindowsRect.Set(oldWindowRect);
                ScreenManager.Calculate();
                
                BackgroundWorker bwRender = new BackgroundWorker();
                bwRender.DoWork += (obj, args) =>
                {
                    var mapGraphics = Graphics.FromImage(mapTemp);

                    foreach (Photo p in photos)
                    {
                        if (p.Geometry.Intersects(boundingGeometry))
                            render.Draw(p, mapGraphics);
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
                                render.Draw(s.Geometry, mapGraphics, l.Style);
                            }
                        }
                    }
                };
                bwRender.RunWorkerCompleted += (obj, args) =>
                {
                    if (file.Text.EndsWith(".bmp"))
                        file.Text = file.Text.Substring(0, file.Text.Length-4);
                    mapTemp.Save(file.Text+".bmp");

                    ///////////////////////////////
                   OSGeo.GDAL.Gdal.AllRegister();
                   OSGeo.GDAL.Driver srcDrv = OSGeo.GDAL.Gdal.GetDriverByName("GTiff");
                   OSGeo.GDAL.Dataset srcDs = OSGeo.GDAL.Gdal.Open(file.Text+".bmp", OSGeo.GDAL.Access.GA_ReadOnly);
                   OSGeo.GDAL.Dataset dstDs = srcDrv.CreateCopy(file.Text+".tiff", srcDs, 0, null, null, null);

                   //Set the map projection
                    {
                        OSGeo.OSR.SpatialReference oSRS = new OSGeo.OSR.SpatialReference("");
                        oSRS.ImportFromProj4( SRS.ToString() );
                        string wkt;
                        oSRS.ExportToWkt(out wkt);
                        dstDs.SetProjection(wkt);
                   }

                   //Set the map georeferencing
                   double mapWidth = mapRectTemp.Width;
                   double mapHeight = mapRectTemp.Height;
                   double[] geoTransfo = new double[] { mapRectTemp.MinX, mapWidth / mapTemp.Width, 0, mapRectTemp.MaxY, 0, -mapHeight / mapTemp.Height };
                   dstDs.SetGeoTransform(geoTransfo);

                   dstDs.FlushCache();
                   dstDs.Dispose();
                   srcDs.Dispose();
                   srcDrv.Dispose();

                    /////////////////////////
                };
                bwRender.RunWorkerAsync();
            });
        }

        private void photoButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Photo");

            TextBox server = toolBuilder.addTextbox("WMS Server", "http://wms.geonorge.no/skwms1/wms.kartdata2");
            Button connectButton = toolBuilder.addButton("Get layers");
            toolBuilder.addLabel("");
            toolBuilder.addLabel("Layers");

            ComboBox wmsLayers = new ComboBox();
            toolBuilder.addControl(wmsLayers);
            wmsLayers.DropDownStyle = ComboBoxStyle.DropDownList;

            toolBuilder.addErrorLabel();
            Button loadButton = toolBuilder.addButton("Load");
            Button clearButton = toolBuilder.addButton("Clear");

            connectButton.Click += (o, e2) =>
            {
                var par = "REQUEST=GetCapabilities&VERSION=1.1.1&service=WMS&format=text/xml";
                var url = server.Text + "?" + par;
                Console.WriteLine(url);
                var capab = "";

                try
                {
                    var request = System.Net.WebRequest.Create(url);
                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    {
                        capab = new StreamReader(stream).ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    toolBuilder.setError("Server error");
                    return;
                }
                toolBuilder.setError("");

                XmlDataDocument xmldoc = new XmlDataDocument();
                xmldoc.LoadXml(capab);
                XmlNodeList xmlnodes = xmldoc.GetElementsByTagName("Name");
                wmsLayers.Items.Clear();
                foreach (XmlNode n in xmlnodes)
                    if (n.ParentNode.Name == "Layer")
                        wmsLayers.Items.Add(n.InnerText);
                if (wmsLayers.Items.Count > 0)
                    wmsLayers.SelectedIndex = 0;
            };

            loadButton.Click += (o, e2) =>
            {
                var d = CultureInfo.InvariantCulture;
                var b = ScreenManager.RealWindowsRect;
                var sb = ScreenManager.MapRealToScreen(b);
                var bbox = "BBOX=" + b.MinX.ToString(d) + "," + b.MinY.ToString(d) + "," + b.MaxX.ToString(d) + "," + b.MaxY.ToString(d) + "&WIDTH=" + sb.Width.ToString(d) + "&HEIGHT=" + sb.Height.ToString(d)+"&";
                var srs = "SRS=EPSG:32633&";
                var par = "STYLES=&FORMAT=image/png&SERVICE=WMS&VERSION=1.1.1&REQUEST=GetMap&";
                if (wmsLayers.SelectedIndex == -1)
                {
                    toolBuilder.setError("Select layer");
                    return;
                }
                var layerString = "LAYERS=" + wmsLayers.SelectedItem + "&";

                var url = server.Text + "?" + layerString + par + srs + bbox;
                Console.WriteLine(url);
                Photo p;

                try
                {
                    var request = System.Net.WebRequest.Create(url);
                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    {
                        p = new Photo(Bitmap.FromStream(stream), b);
                    }
                } catch (Exception ex)
                {
                    toolBuilder.setError("Server error");
                    Console.WriteLine(ex.Message);
                    return;
                }
                toolBuilder.setError("");
                photos.Add(p);
                redraw();
            };

            clearButton.Click += (o, e2) => {
                photos.Clear();
                redraw();
            };

            toolBuilder.reset();
        }
    }
}
