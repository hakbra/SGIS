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
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SGIS
{
    // partial class containing functions relating to tools
    public partial class SGIS
    {
        // instance of toolBuilder used to create tool interfaces
        ToolBuilder toolBuilder;

        // function for adding new layers
        private void addButton_Click(object sender, EventArgs e)
        {
            // file dialog for selecting one or more files
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "shp files|*.shp";
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // initialises values for the progress bar
                progressBar.Minimum = 0;
                progressBar.Maximum = openFileDialog1.FileNames.Count();
                progressBar.Value = 0;

                // background worker for reading new layers in another thread
                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;

                // list of new layers
                List<Layer> readLayers = new List<Layer>();

                bw.DoWork += (object wsender, DoWorkEventArgs we) =>
                {
                    // read each layer in turn
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
                    // reset progress bar
                    progressBar.Value = 0;
                    progressLabel.Text = "";

                    // add each layer and focus screen on the new layer
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
                    // update progress barvel, 
                    progressBar.Value += we.ProgressPercentage;
                    if (we.ProgressPercentage == 0)
                        progressLabel.Text = "Reading " + (string)we.UserState;
                };
                bw.RunWorkerAsync();
            }
        }

        // delete selected layer
        private void delButton_Click(object sender, EventArgs e)
        {
            if (layerList.Items.Count == 0)
                return;
            toolBuilder.addHeader("Delete layer");
            toolBuilder.addLabel("Are you sure?");
            // button for clicking i am sure
            Button yesButton = toolBuilder.addButton("Yes", (Layer il) =>
            {
                SGIS.App.Layers.Remove(il);
                redraw();
            });
            // button for i am not sure
            Button noButton = toolBuilder.addButton("No", (il) => { });
            // function called when another layer is selected
            AcceptButton = yesButton;
            toolBuilder.resetAction = (Layer il) =>
            {
                toolBuilder.clear();
            };
        }

        // move layer up in layerlist
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

        // move layer down in list
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

        // buffer layer
        private void bufferButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Buffer");

            // textbox for buffer distance
            TextBox distBox = toolBuilder.addTextboxWithCaption("Distance (m):");
            // textbox for new layername
            TextBox nameBox = toolBuilder.addTextboxWithCaption("New layername:");
            // label for errors
            Label errorLabel = toolBuilder.addErrorLabel();

            // button for performing buffer
            Button selectButton = toolBuilder.addButton("Buffer", (Layer l) =>
            {
                double dist = 0;
                // buffer does not work on lat-long projections
                if (SRS.IsLatLong)
                {
                    toolBuilder.setError("Incompatible SRS");
                    return;
                }
                // distance must be a number
                if (!double.TryParse(distBox.Text, out dist))
                {
                    toolBuilder.setError("Not a number");
                    return;
                }
                // user must give new layer name
                if (nameBox.Text.Length == 0)
                {
                    toolBuilder.setError("Provide a name");
                    return;
                }

                // create new layer
                Layer newl = new Layer(nameBox.Text);
                newl.DataTable = l.DataTable;

                List<Feature> flist = l.Features.Values.ToList();

                // initialise progress bar
                progressLabel.Text = "Buffering";
                progressBar.Minimum = 0;
                progressBar.Maximum = flist.Count;

                // threadsafe list of new features
                ConcurrentBag<Feature> newFeatures = new ConcurrentBag<Feature>();

                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;

                // perform buffering in other thread
                bw.DoWork += (object wsender, DoWorkEventArgs we) =>
                {
                    using (var finished = new CountdownEvent(1))
                    {
                        // for each feature
                        for (int i = 0; i < flist.Count; i++)
                        {
                            // add the task to buffer a feature to a thread pool
                            finished.AddCount();
                            Feature capture = flist[i];
                            ThreadPool.QueueUserWorkItem((state) =>
                            {
                                // get feature
                                Feature f = capture;
                                // add buffered feature
                                newFeatures.Add(new Feature(f.Geometry.Buffer(dist), f.ID));
                                bw.ReportProgress(i);
                                finished.Signal();
                            }, null);
                        }
                        finished.Signal();
                        finished.Wait();
                    }
                    bw.ReportProgress(-1);
                    // add all buffered features to layer
                    foreach(Feature f in newFeatures)
                    {
                        newl.addFeature(f);
                        bw.ReportProgress(1);
                    }
                    newl.calculateBoundingBox();
                    newl.createQuadTree();
                };
                bw.RunWorkerCompleted += (object wsender, RunWorkerCompletedEventArgs we) =>
                {
                    // reset progress bar
                    progressBar.Value = 0;
                    progressLabel.Text = "";

                    // add and zoom newly made layer
                    Layers.Insert(0, newl);
                    layerList.SelectedItem = newl;
                };
                bw.ProgressChanged += (object wsender, ProgressChangedEventArgs we) =>
                {
                    // update progress bar and progress label
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
                if (SRS.IsLatLong)
                    toolBuilder.setError("Incompatible SRS");
                nameBox.Text = (l == null) ? "" : l.Name + "_buffer";
            };

            toolBuilder.reset();
        }

        // merge two layers
        private void mergeButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Merge");
            // dropdown for selecting second layer
            ComboBox layerSelect = toolBuilder.addLayerSelect("Merge with:");
            // textbox for new layername
            TextBox textbox = toolBuilder.addTextboxWithCaption("New layername:");
            Label errorLabel = toolBuilder.addErrorLabel();
            // button for performing merge
            Button button = toolBuilder.addButton("Merge", (Layer l) =>
            {
                // no new layer name given
                if (textbox.Text.Length == 0)
                {
                    toolBuilder.setError("Provide name");
                    return;
                }
                Layer unionLayer = (Layer) layerSelect.SelectedItem;
                // layers must have same shapetype
                if (l.shapetype != unionLayer.shapetype)
                {
                    toolBuilder.setError("Incompatible types");
                    return;
                }
                // create new layer
                Layer newLayer = new Layer(textbox.Text);
                // create boundingbox as combination of two bb's
                newLayer.Boundingbox = new Envelope(l.Boundingbox);
                newLayer.Boundingbox.ExpandToInclude(unionLayer.Boundingbox);
                newLayer.createQuadTree();

                // add all features from one layer
                foreach (Feature f in l.Features.Values)
                    newLayer.addFeature(new Feature((IGeometry)f.Geometry.Clone()));
                // add all features from other layer
                foreach (Feature f in unionLayer.Features.Values)
                    newLayer.addFeature(new Feature((IGeometry)f.Geometry.Clone()));

                // insert newly made layer
                Layers.Insert(0, newLayer);
                // redraw map
                redraw();
            });
            // change default new layer name when selected layer changes
            toolBuilder.resetAction = (Layer l) =>
            {
                textbox.Text = (l == null) ? "" : l.Name + "_merge";
            };
            toolBuilder.reset();
        }

        // perform union on currently selected layer
        private void unionButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Union");
            // textbox for new layername
            TextBox textbox = toolBuilder.addTextboxWithCaption("New layername:");
            Label errorLabel = toolBuilder.addErrorLabel();
            Button button = toolBuilder.addButton("Union", (Layer l) =>
            {
                // user has not set new layername
                if (textbox.Text.Length == 0)
                {
                    toolBuilder.setError("Provide name");
                    return;
                }
                // create temporary layer
                Layer copyLayer = new Layer(l.Name);
                copyLayer.Boundingbox = new Envelope(l.Boundingbox);
                copyLayer.createQuadTree();

                // copy all features to temp layer
                foreach (Feature f in l.Features.Values)
                    copyLayer.addFeature(new Feature((IGeometry)f.Geometry.Clone(), f.ID));

                // create new layer with same boundingbox
                Layer newLayer = new Layer(textbox.Text);
                newLayer.Boundingbox = new Envelope(l.Boundingbox);
                newLayer.createQuadTree();

                // init progress bar
                int numFeatures = copyLayer.Features.Values.Count;
                progressLabel.Text = "Performing union";
                progressBar.Minimum = 0;
                progressBar.Maximum = numFeatures;
                
                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                // perform merge in another thread
                bw.DoWork += (object wsender, DoWorkEventArgs we) =>
                {
                    // threadsafe list of merged features
                    ConcurrentBag<Feature> newFeatures = new ConcurrentBag<Feature>();
                    var finished = new CountdownEvent(1);
                    Object _lock = new object();
                    // create thread function
                    var merge = new WaitCallback((state) =>
                    {
                        Random rnd = new Random();
                        while (true)
                        {
                            Feature f;
                            lock (_lock)
                            {
                                // break if no more features
                                if (copyLayer.Features.Count == 0)
                                    break;
                                // get random index
                                int index = rnd.Next(copyLayer.Features.Count);
                                // get corresponding random feature
                                f = copyLayer.Features[copyLayer.Features.Keys.ToList()[index]];
                                // remove feature from layer
                                copyLayer.delFeature(f);
                            }
                            f.ID = -1;
                            while (true)
                            {
                                List<Feature> intersects;
                                // aquire lock to avoid race conditions
                                lock (_lock)
                                {
                                    // get all features intersecting feature
                                    intersects = copyLayer.getWithin(f.Geometry);
                                    // remove features from layer
                                    foreach (Feature intersect in intersects)
                                        copyLayer.delFeature(intersect);
                                }
                                // if no intersects, no merging is necessary
                                if (intersects.Count == 0)
                                    break;
                                // merge all features
                                foreach (Feature intersect in intersects)
                                {
                                    f = new Feature(f.Geometry.Union(intersect.Geometry));
                                    bw.ReportProgress(1);
                                }
                            }
                            // add feature to list of new features
                            newFeatures.Add(f);
                        }
                        finished.Signal();
                    });
                    // spawn eight threads, this is not always optimal but a good approximation
                    for (int i = 0; i < 8; i++)
                    {
                        finished.AddCount();
                        ThreadPool.QueueUserWorkItem(merge);
                    }
                    finished.Signal();
                    finished.Wait();

                    bw.ReportProgress(-newFeatures.Count);
                    // add all merged features back to temp layer
                    foreach (Feature f in newFeatures)
                        copyLayer.addFeature(f);
                    newFeatures = new ConcurrentBag<Feature>();
                    finished = new CountdownEvent(1);
                    // perform a final single threaded merge
                    merge(false);

                    // add all final merged features to new layer
                    foreach (Feature f in newFeatures)
                        newLayer.addFeature(f);
                };
                bw.RunWorkerCompleted += (object wsender, RunWorkerCompletedEventArgs we) =>
                {
                    // reset progress bar
                    progressBar.Value = 0;
                    progressLabel.Text = "";

                    // insert new layer and redraw map
                    Layers.Insert(0, newLayer);
                    redraw();
                };
                bw.ProgressChanged += (object wsender, ProgressChangedEventArgs we) =>
                {
                    //  update progress bar
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
            // reset default new layer name when selected layer is changed
            toolBuilder.resetAction = (Layer l) => {
                textbox.Text = (l == null) ? "" : l.Name + "_union";
            };
            toolBuilder.reset();
        }

        private void diffButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Difference");
            // dropdown for selecting other layer
            ComboBox layerSelect = toolBuilder.addLayerSelect("Layer to subtract:");
            // textbox for new layer name
            TextBox textbox = toolBuilder.addTextboxWithCaption("New layername:");
            // label for errors
            Label errorLabel = toolBuilder.addErrorLabel();
            // button for performing subtraction
            Button button = toolBuilder.addButton("Subtract", (Layer l) =>
            {
                // no new layer name is given
                if (textbox.Text.Length == 0)
                {
                    toolBuilder.setError("Provide name");
                    return;
                }
                // layers must have the same type of shapes
                Layer unionLayer = (Layer)layerSelect.SelectedItem;
                if (l.shapetype != unionLayer.shapetype)
                {
                    toolBuilder.setError("Incompatible types");
                    return;
                }
                Layer newLayer = new Layer(textbox.Text);
                newLayer.DataTable = l.DataTable;
                
                // init progress bar
                progressLabel.Text = "Subtracting";
                progressBar.Minimum = 0;
                progressBar.Maximum = l.Features.Values.Count;

                // background worker for running operation in another thread
                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.DoWork += (object wsender, DoWorkEventArgs we) =>
                {
                    // threadsafe list for storing new features
                    ConcurrentBag<Feature> newFeatures = new ConcurrentBag<Feature>();
                    using (var finished = new CountdownEvent(1))
                    {
                        foreach (Feature f in l.Features.Values)
                        {
                            finished.AddCount();
                            Feature capt = f;
                            // in each thread
                            ThreadPool.QueueUserWorkItem((state) =>
                            {
                                // clone feature
                                Feature newf = new Feature((IGeometry)capt.Geometry.Clone(), capt.ID);
                                // get intersecting features
                                var intersects = unionLayer.getWithin(capt.Geometry);
                                // subtract intersecting features
                                foreach (Feature intersect in intersects)
                                    newf.Geometry = newf.Geometry.Difference(intersect.Geometry);

                                // if there is something left of the original feature
                                if (!newf.Geometry.IsEmpty) // add it to new feature list
                                    newFeatures.Add(newf);
                                bw.ReportProgress(1);
                                finished.Signal();
                            }, null);
                        }
                        finished.Signal();
                        finished.Wait();
                    }
                    bw.ReportProgress(-newFeatures.Count);
                    // add all processed features to new layer
                    foreach (Feature f in newFeatures)
                    {
                        newLayer.addFeature(f);
                        bw.ReportProgress(1);
                    }
                };
                bw.RunWorkerCompleted += (object wsender, RunWorkerCompletedEventArgs we) =>
                {
                    // reset progressbar
                    progressBar.Value = 0;
                    progressLabel.Text = "";

                    // create quad tree and insert new layer
                    newLayer.calculateBoundingBox();
                    newLayer.createQuadTree();
                    Layers.Insert(0, newLayer);

                    redraw();
                };
                bw.ProgressChanged += (object wsender, ProgressChangedEventArgs we) =>
                {
                    // update progress bar
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
            // reset new layer name when selected layer changes
            toolBuilder.resetAction += (Layer l) =>
            {
                textbox.Text = (l == null) ? "" : l.Name + "_diff";
            };
            toolBuilder.reset();
        }

        // calculate intersection of layers
        private void intersectButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Intersect");
            // dropdown for selecting other layer
            ComboBox layerSelect = toolBuilder.addLayerSelect("Intersect with:");
            // textbox for new layer name
            TextBox textbox = toolBuilder.addTextboxWithCaption("New layername:");
            // laber for errors
            Label errorLabel = toolBuilder.addErrorLabel();
            //button for performing intersection
            Button button = toolBuilder.addButton("Intersect", (Layer l) =>
            {
                // new layer name not given
                if (textbox.Text.Length == 0)
                {
                    toolBuilder.setError("Provide name");
                    return;
                }
                Layer intersectLayer = (Layer)layerSelect.SelectedItem;
                Layer newLayer = new Layer(textbox.Text);

                // if both layers have attributes
                if (l.DataTable != null && intersectLayer.DataTable != null)
                {
                    // merge attributes, columnName collisions may be overwritten
                    DataTable a = l.DataTable.Clone();
                    newLayer.DataTable = intersectLayer.DataTable.Clone();
                    newLayer.DataTable.Merge(a, true, MissingSchemaAction.Add);
                } 
                // if only one layer has attributes 
                else if (l.DataTable != null && intersectLayer.DataTable == null)
                {
                    newLayer.DataTable = l.DataTable.Clone();
                }
                // if only the other layer has attributes 
                else if (l.DataTable == null && intersectLayer.DataTable != null)
                {
                    newLayer.DataTable = intersectLayer.DataTable.Clone();
                }

                // init progress bar
                progressLabel.Text = "Intersection";
                progressBar.Minimum = 0;
                progressBar.Maximum = l.Features.Values.Count;

                // background worker for running in another thread
                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.DoWork += (object wsender, DoWorkEventArgs we) =>
                {
                    // loop through all features
                    for (int i = 0; i < l.Features.Count; i++ )
                    {
                        Feature f = l.Features.Values.ElementAt(i);
                        bw.ReportProgress(i);
                        // get intersecting features
                        var intersections = intersectLayer.getWithin(f.Geometry);
                        foreach (Feature intersect in intersections)
                        {
                            DataRow arow = l.getRow(f);
                            DataRow brow = intersectLayer.getRow(intersect);

                            // calculate intersection
                            Feature result = new Feature(f.Geometry.Intersection(intersect.Geometry));
                            int id = newLayer.addFeature(result);

                            // merge attributes
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
                    // reset progress bar
                    progressBar.Value = 0;
                    progressLabel.Text = "";

                    // finalise new layer
                    newLayer.calculateBoundingBox();
                    newLayer.createQuadTree();
                    Layers.Insert(0, newLayer);
                    redraw();
                };
                bw.ProgressChanged += (object wsender, ProgressChangedEventArgs we) =>
                {
                    // update progress bar
                    progressBar.Value = we.ProgressPercentage;
                };
                bw.RunWorkerAsync();
            });
            // reset new layer name
            toolBuilder.resetAction += (Layer l) =>
            {
                textbox.Text = (l == null) ? "" : l.Name + "_intersect";
            };
            toolBuilder.reset();
        }

        // measure features, length and area for selected layer
        private void measureButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Measurements");

            // labels for showing info
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

            // reduce height of labels for more compact view
            selCountLabel.Height = selLengthLabel.Height = selAreaLabel.Height = (int)(selAreaLabel.Height * 0.7);
            selAreaLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            countLabel.Height = lengthLabel.Height = areaLabel.Height = (int)(areaLabel.Height * 0.7);

            // recalculate measurements when new layer is selected
            toolBuilder.resetAction += (Layer l) =>
            {
                {
                    string plural = "";
                    if (l != null && l.Selected.Count > 0)
                        plural = "s";

                    if (l == null || l.shapetype == ShapeType.EMPTY)
                    {
                        selCountLabel.Text = "No features";
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
                        countLabel.Text = "No features";
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
            // if less than 1km, show meters with 10cm accuracy
            if (meters < 1000)
                return meters.ToString("N1") + " m";
            // else, show kilometers with 100m accuracy
            meters /= 1000;
            return meters.ToString("N1") + " km";
        }
        private string formatArea(double sqmeters)
        {
            // if less than 1km^2, show square meters with 0.1m^2 accuracy
            if (sqmeters < 100000)
                return sqmeters.ToString("N1") + " m^2";
            // else, show square kilometers with 0.1km^2 accuracy
            sqmeters /= 1000000;
            return sqmeters.ToString("N1") + " km^2";
        }

        // export map as raster-image
        private void renderButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Export to GeoTiff", false);
            // textbox for increase in resolution
            var zoom = toolBuilder.addTextboxWithCaption("Zoom factor:", "1");
            // textbox and button for new file name input
            var file = toolBuilder.addTextboxWithCaption("Filename:", "");
            var browsebutton = toolBuilder.addButton("Browse...");
            browsebutton.Click += (o, w) =>
            {
                 SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                 saveFileDialog1.Filter = "bmp files (*.bmp)|*.bmp"  ;
                 if(saveFileDialog1.ShowDialog() == DialogResult.OK)
                     file.Text = saveFileDialog1.FileName;
            };
            // label for errors
            var error = toolBuilder.addErrorLabel();

            // button for performing rasterization
            Button button = toolBuilder.addButton("Export", (Layer selectedLayer) =>
            {
                // no filename is given
                if (file.Text == "")
                {
                    toolBuilder.setError("Please provide filename");
                    return;
                }
                // æøå not accepted in filename, not supported by GDAL
                if (file.Text.ToLower().IndexOfAny("æøå".ToCharArray()) > -1)
                {
                    toolBuilder.setError("No æøå in filename");
                    return;
                }
                double zoomfactor = 1;
                if (!double.TryParse(zoom.Text, out zoomfactor))
                {
                    // zoom factor must be a number
                    toolBuilder.setError("Zoom factor not a number");
                    return;
                }

                // calulate image resolution and new real world coordinates
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
                
                // background worker for performing rasterisation in another thread
                BackgroundWorker bwRender = new BackgroundWorker();
                bwRender.DoWork += (obj, args) =>
                {
                    var mapGraphics = Graphics.FromImage(mapTemp);

                    // draw background maps
                    foreach (Photo p in photos)
                    {
                        // only if visible
                        if (p.Geometry.Intersects(boundingGeometry))
                            render.Draw(p, mapGraphics);
                    }
                    // draw layers
                    foreach (Layer l in Layers.Reverse())
                    {
                        // only if visible
                        if (!l.Visible)
                            continue;
                        // draw only visible features
                        var visibleFeatures = l.getWithin(boundingGeometry);
                        lock (l) // lock layer to prevent multithreaded access to style when drawing
                        {
                            // render feature
                            foreach (Feature s in visibleFeatures)
                            {
                                render.Draw(s.Geometry, mapGraphics, l.Style);
                            }
                        }
                    }
                };
                // georeference drawn bitmap
                bwRender.RunWorkerCompleted += (obj, args) =>
                {
                    // remove .bmp ending if present
                    if (file.Text.EndsWith(".bmp"))
                        file.Text = file.Text.Substring(0, file.Text.Length-4);
                    // save render
                    mapTemp.Save(file.Text+".bmp");

                   // init GDAL and copy image
                   OSGeo.GDAL.Gdal.AllRegister();
                   OSGeo.GDAL.Driver srcDrv = OSGeo.GDAL.Gdal.GetDriverByName("GTiff");
                   OSGeo.GDAL.Dataset srcDs = OSGeo.GDAL.Gdal.Open(file.Text+".bmp", OSGeo.GDAL.Access.GA_ReadOnly);
                   OSGeo.GDAL.Dataset dstDs = srcDrv.CreateCopy(file.Text+".tiff", srcDs, 0, null, null, null);

                   //Set the map projection
                    {
                        OSGeo.OSR.SpatialReference oSRS = new OSGeo.OSR.SpatialReference("");
                        oSRS.ImportFromProj4( SRS.ToString() );
                        string wkt;
                        // convert projection to wkt
                        oSRS.ExportToWkt(out wkt);
                        dstDs.SetProjection(wkt);
                   }

                   //Set the map coordinates
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

        // load background map from wms
        private void photoButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Background WMS", false);
            // textbox for wms-server
            TextBox server = toolBuilder.addTextboxWithCaption("WMS Server", "http://wms.geonorge.no/skwms1/wms.kartdata2");
            // button for fetching available layers
            Button connectButton = toolBuilder.addButton("Get layers");
            toolBuilder.addLabel("Layers");

            // dropdown for available layers
            ComboBox wmsLayers = new ComboBox();
            toolBuilder.addControl(wmsLayers);
            wmsLayers.DropDownStyle = ComboBoxStyle.DropDownList;

            // label for errors
            toolBuilder.addErrorLabel();
            // label for loading selected layer from wms
            Button loadButton = toolBuilder.addButton("Load");
            // button for clearing all loaded wms maps
            Button clearButton = toolBuilder.addButton("Clear");

            connectButton.Click += (o, e2) =>
            {
                // init progress bar
                progressLabel.Text = "Loading layers";
                progressBar.Style = ProgressBarStyle.Marquee;

                XmlNodeList xmlnodes = null;

                // perform connection in worker thread
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += (object wsender, DoWorkEventArgs we) =>
                {
                    // create url
                    var par = "REQUEST=GetCapabilities&VERSION=1.1.1&service=WMS&format=text/xml";
                    if (!server.Text.EndsWith("?"))
                        server.Text += "?";
                    var url = server.Text + par;
                    var capab = "";

                    try
                    {
                        // get response from url
                        var request = System.Net.WebRequest.Create(url);
                        using (var response = request.GetResponse())
                        using (var stream = response.GetResponseStream())
                        {
                            capab = new StreamReader(stream).ReadToEnd();
                        }
                    }
                    // handle errors
                    catch (WebException ex)
                    {
                        if (ex.Status == WebExceptionStatus.ProtocolError)
                        {
                            var response = ex.Response as HttpWebResponse;
                            if (response != null)
                            {
                                int errorNum = (int)response.StatusCode;
                                if (errorNum == 504)
                                {
                                    we.Result = "Server Timeout";
                                    return;
                                }

                                we.Result = "Http Error " + errorNum;
                            }
                        }
                        we.Result = "Server Error";
                        return;
                    }
                    catch (Exception ex)
                    {
                        we.Result = "Server Error";
                        return;
                    }

                    // parse response as xml document
                    XmlDataDocument xmldoc = new XmlDataDocument();
                    xmldoc.LoadXml(capab);
                    xmlnodes = xmldoc.GetElementsByTagName("Name");
                };
                bw.RunWorkerCompleted += (object wsender, RunWorkerCompletedEventArgs we) =>
                {
                    // add available layers to dropdown
                    wmsLayers.Items.Clear();
                    if (xmlnodes != null)
                    {
                        foreach (XmlNode n in xmlnodes)
                            if (n.ParentNode.Name == "Layer")
                                wmsLayers.Items.Add(n.InnerText);
                        if (wmsLayers.Items.Count > 0)
                            wmsLayers.SelectedIndex = 0;
                        toolBuilder.setError("");
                    }
                    else
                        toolBuilder.setError((string)we.Result);

                    // reset progress bar
                    progressLabel.Text = "";
                    progressBar.Style = ProgressBarStyle.Continuous;
                };
                bw.RunWorkerAsync();
            };

            loadButton.Click += (o, e2) =>
            {
                // create url request
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

                if (!server.Text.EndsWith("?"))
                    server.Text += "?";
                var url = server.Text + layerString + par + srs + bbox;
                Photo p = null;
                
                // init progress bar
                progressLabel.Text = "Loading map";
                progressBar.Style = ProgressBarStyle.Marquee;
                
                // peform loading in worker thread
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += (object wsender, DoWorkEventArgs we) =>
                {
                    // try loading bitmap from url response
                    try
                    {
                        var request = System.Net.WebRequest.Create(url);
                        using (var response = request.GetResponse())
                        {
                            using (var stream = response.GetResponseStream())
                            {
                                p = new Photo(Bitmap.FromStream(stream), b);
                            }
                        }
                    }
                    // handle errors
                    catch (WebException ex)
                    {
                        p = null;
                        if (ex.Status == WebExceptionStatus.ProtocolError)
                        {
                            var response = ex.Response as HttpWebResponse;
                            if (response != null)
                            {
                                int errorNum = (int)response.StatusCode;
                                if(errorNum == 504)
                                {
                                    we.Result = "Server Timeout";
                                    return;
                                }

                                we.Result = "Http Error " + errorNum;
                            }
                        }
                        we.Result = "Server Error";
                        return;
                    }
                    catch (Exception ex)
                    {
                        p = null;
                        we.Result = "Server Error";
                        return;
                    }
                };
                bw.RunWorkerCompleted += (object wsender, RunWorkerCompletedEventArgs we) =>
                {
                    // update map if loading went ok
                    if (p != null)
                    {
                        photos.Add(p);
                        redraw();
                        toolBuilder.setError("");
                    }
                    else
                        toolBuilder.setError((string) we.Result);

                    // reset progress bar
                    progressLabel.Text = "";
                    progressBar.Style = ProgressBarStyle.Continuous;
                };
                bw.RunWorkerAsync();
            };

            // clear loaded maps
            clearButton.Click += (o, e2) => {
                photos.Clear();
                redraw();
            };

            toolBuilder.reset();
        }
    }
}
