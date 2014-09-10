using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Union;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SGIS
{
    public partial class SGIS
    {
        ToolBuilder toolBuilder;

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
                newl.dataTable = l.dataTable;
                newl.boundingbox = l.boundingbox;
                newl.createQuadTree();

                List<Feature> flist = l.features.Values.ToList();

                progressLabel.Text = "Buffer";
                progressBar.Minimum = 0;
                progressBar.Maximum = flist.Count;

                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.DoWork += (object wsender, DoWorkEventArgs we) =>
                {
                    for (int i = 0; i < flist.Count; i++)
                    {
                        bw.ReportProgress(i);
                        Feature f = flist[i];
                        newl.addFeature(new Feature(f.geometry.Buffer(dist), f.id));
                    }
                };
                bw.RunWorkerCompleted += (object wsender, RunWorkerCompletedEventArgs we) =>
                {
                    progressBar.Value = 0;
                    progressLabel.Text = "";

                    layers.Insert(0, newl);
                    layerList.SelectedItem = newl;
                };
                bw.ProgressChanged += (object wsender, ProgressChangedEventArgs we) =>
                {

                    progressBar.Value = we.ProgressPercentage;
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
                newLayer.boundingbox = new Envelope(l.boundingbox);
                newLayer.boundingbox.ExpandToInclude(unionLayer.boundingbox);
                newLayer.createQuadTree();

                foreach (Feature f in l.features.Values)
                    newLayer.addFeature(new Feature((IGeometry)f.geometry.Clone()));
                foreach (Feature f in unionLayer.features.Values)
                    newLayer.addFeature(new Feature((IGeometry)f.geometry.Clone()));

                layers.Insert(0, newLayer);
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
                copyLayer.boundingbox = new Envelope(l.boundingbox);
                copyLayer.createQuadTree();

                foreach (Feature f in l.features.Values)
                    copyLayer.addFeature(new Feature((IGeometry)f.geometry.Clone(), f.id));

                Layer newLayer = new Layer(textbox.Text);
                newLayer.boundingbox = new Envelope(l.boundingbox);
                newLayer.createQuadTree();

                int numFeatures = copyLayer.features.Values.Count;
                progressLabel.Text = "Merging";
                progressBar.Minimum = 0;
                progressBar.Maximum = numFeatures;
                
                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.DoWork += (object wsender, DoWorkEventArgs we) =>
                {
                    while (copyLayer.features.Values.Count > 0)
                    {
                        Feature f = copyLayer.features.Values.First();
                        copyLayer.delFeature(f);
                        f.id = -1;
                        while (true)
                        {
                            var intersects = copyLayer.getWithin(f.geometry);
                            if (intersects.Count == 0)
                                break;
                            foreach (Feature intersect in intersects)
                            {
                                copyLayer.delFeature(intersect);
                                bw.ReportProgress(numFeatures - copyLayer.features.Values.Count);
                                f = new Feature(f.geometry.Union(intersect.geometry));
                            }
                        }
                        newLayer.addFeature(f);
                    }
                };
                bw.RunWorkerCompleted += (object wsender, RunWorkerCompletedEventArgs we) =>
                {
                    progressBar.Value = 0;
                    progressLabel.Text = "";

                    layers.Insert(0, newLayer);
                    redraw();
                };
                bw.ProgressChanged += (object wsender, ProgressChangedEventArgs we) =>
                {

                    progressBar.Value = we.ProgressPercentage;
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
                newLayer.boundingbox = new Envelope(l.boundingbox);
                newLayer.createQuadTree();
                
                progressLabel.Text = "Subtracting";
                progressBar.Minimum = 0;
                progressBar.Maximum = l.features.Values.Count;

                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.DoWork += (object wsender, DoWorkEventArgs we) =>
                {
                    for (int i = 0; i < l.features.Count; i++ )
                    {
                        Feature f = l.features.Values.ElementAt(i);
                        bw.ReportProgress(i);
                        Feature newf = new Feature((IGeometry)f.geometry.Clone(), f.id);
                        var intersects = unionLayer.getWithin(f.geometry);
                        foreach (Feature intersect in intersects)
                            newf.geometry = newf.geometry.Difference(intersect.geometry);

                        newLayer.addFeature(newf);
                    }
                };
                bw.RunWorkerCompleted += (object wsender, RunWorkerCompletedEventArgs we) =>
                {
                    progressBar.Value = 0;
                    progressLabel.Text = "";

                    layers.Insert(0, newLayer);
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
                newLayer.boundingbox = new Envelope(l.boundingbox);
                newLayer.createQuadTree();

                DataTable a = l.dataTable.Clone();
                newLayer.dataTable = intersectLayer.dataTable.Clone();
                newLayer.dataTable.Merge(a, true, MissingSchemaAction.Add);

                progressLabel.Text = "Intersection";
                progressBar.Minimum = 0;
                progressBar.Maximum = l.features.Values.Count;

                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.DoWork += (object wsender, DoWorkEventArgs we) =>
                {
                    for (int i = 0; i < l.features.Count; i++ )
                    {
                        Feature f = l.features.Values.ElementAt(i);
                        bw.ReportProgress(i);
                        var intersections = intersectLayer.getWithin(f.geometry);
                        foreach (Feature intersect in intersections)
                        {
                            DataRow arow = l.getRow(f);
                            DataRow brow = intersectLayer.getRow(intersect);

                            Feature result = new Feature(f.geometry.Intersection(intersect.geometry));
                            int id = newLayer.addFeature(result);

                            DataRow dr = newLayer.dataTable.NewRow();
                            foreach (DataColumn dc in arow.Table.Columns)
                                dr[dc.ColumnName] = arow[dc.ColumnName];
                            foreach (DataColumn dc in brow.Table.Columns)
                                dr[dc.ColumnName] = brow[dc.ColumnName];
                            dr["sgis_id"] = id;
                            newLayer.dataTable.Rows.Add(dr);
                        }
                    }
                };
                bw.RunWorkerCompleted += (object wsender, RunWorkerCompletedEventArgs we) =>
                {
                    progressBar.Value = 0;
                    progressLabel.Text = "";

                    layers.Insert(0, newLayer);
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
            Label countLabel = toolBuilder.addLabel("");
            Label lengthLabel = toolBuilder.addLabel("");
            Label areaLabel = toolBuilder.addLabel("");

            toolBuilder.resetAction += (Layer l) =>
            {
                string plural = "";
                if (l != null && l.features.Count > 0)
                    plural = "s";

                if (l == null || l.shapetype == ShapeType.EMPTY)
                {
                    countLabel.Text = "No feature"+plural;
                    lengthLabel.Text = "Length: N/A";
                    areaLabel.Text = "Area: N/A";
                }
                else if (l.shapetype == ShapeType.POINT)
                {
                    countLabel.Text = l.features.Count + " points";
                    lengthLabel.Text = "Length: N/A";
                    areaLabel.Text = "Area: N/A";
                }
                else if (l.shapetype == ShapeType.LINE)
                {
                    countLabel.Text = l.features.Count + " feature" + plural;
                    double length = 0;
                    foreach (Feature f in l.features.Values)
                        length += f.geometry.Length;
                    lengthLabel.Text = "Length: "+(int)length+"m";
                    areaLabel.Text = "Area: N/A";
                }
                else if (l.shapetype == ShapeType.POLYGON)
                {
                    countLabel.Text = l.features.Count + " feature" + plural;
                    double circum = 0;
                    double area = 0;
                    foreach (Feature f in l.features.Values)
                    {
                        circum += f.geometry.Length;
                        area += f.geometry.Area;
                    }
                    lengthLabel.Text = "Circ: " + (int)circum+"m";
                    areaLabel.Text = "Area: " + Math.Round(area)+"m^2";
                }

            };
            toolBuilder.reset();
        }
    }
}
