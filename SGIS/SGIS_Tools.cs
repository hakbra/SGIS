using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Union;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SGIS
{
    public partial class SGIS
    {
        ToolBuilder toolBuilder;

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

            TextBox distBox = toolBuilder.addTextbox("Distance:");
            TextBox nameBox = toolBuilder.addTextbox("Layer name:");
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

        private void unionButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Union");
            ComboBox layerSelect = toolBuilder.addLayerSelect("Union with:");
            TextBox textbox = toolBuilder.addTextbox("New layername:");
            Label errorLabel = toolBuilder.addErrorLabel();
            Button button = toolBuilder.addButton("Union", (Layer l) =>
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
                textbox.Text = (l == null) ? "" : l.Name + "_union";
            };
            toolBuilder.reset();
        }

        private void mergeButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Merge");
            TextBox textbox = toolBuilder.addTextbox("New layername:");
            Label errorLabel = toolBuilder.addErrorLabel();
            Button button = toolBuilder.addButton("Merge", (Layer l) =>
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
                progressLabel.Text = "Merging";
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
                        progressLabel.Text = "Merging - Second pass";
                    }
                    else
                        progressBar.Value += we.ProgressPercentage;
                };
                bw.RunWorkerAsync();
            });
            toolBuilder.resetAction = (Layer l) => {
                textbox.Text = (l == null) ? "" : l.Name + "_merge";
            };
            toolBuilder.reset();
        }

        private void diffButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Difference");
            ComboBox layerSelect = toolBuilder.addLayerSelect("Subtract:");
            TextBox textbox = toolBuilder.addTextbox("New layername:");
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
            TextBox textbox = toolBuilder.addTextbox("New layername:");
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
    }
}
