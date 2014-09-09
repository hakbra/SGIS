using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Union;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SGIS
{
    public partial class SGIS
    {

        private void bufferButton_Click(object sender, EventArgs e)
        {
            Layer cl = (Layer)layerList.SelectedItem;
            ToolBuilder tb = new ToolBuilder(toolPanel, "Buffer");

            TextBox distBox = tb.addTextbox("Distance:");
            TextBox nameBox = tb.addTextbox("Layer name:", (cl != null) ? cl.Name + "_buffer": "");
            Label errorLabel = tb.addErrorLabel();
            Button selectButton = tb.addButton("Buffer", (Layer l) =>
            {
                double dist = 0;
                if (!double.TryParse(distBox.Text, out dist))
                {
                    tb.setError("Not a number");
                    return;
                }
                if (nameBox.Text.Length == 0)
                {
                    tb.setError("Provide a name");
                    return;
                }

                Layer newl = new Layer(nameBox.Text);
                newl.dataTable = l.dataTable;
                newl.boundingbox = l.boundingbox;
                newl.createQuadTree();

                List<Feature> flist = l.features.Values.ToList();

                progressLabel.Text = "Buffer";
                progressLabel.Invalidate();
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
        }

        private void unionButton_Click(object sender, EventArgs e)
        {
            Layer cl = (Layer)layerList.SelectedItem;
            ToolBuilder tb = new ToolBuilder(toolPanel, "Union");
            ComboBox layerSelect = tb.addLayerSelect("Union with:");
            TextBox textbox = tb.addTextbox("New layername:", (cl != null) ? cl.Name + "_union" : "");
            Label errorLabel = tb.addErrorLabel();
            Button button = tb.addButton("Union", (Layer l) =>
            {
                if (textbox.Text.Length == 0)
                {
                    tb.setError("Provide name");
                    return;
                }
                Layer unionLayer = (Layer) layerSelect.SelectedItem;
                if (l.shapetype != unionLayer.shapetype)
                {
                    tb.setError("Incompatible types");
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
        }

        private void mergeButton_Click(object sender, EventArgs e)
        {
            Layer cl = (Layer)layerList.SelectedItem;
            ToolBuilder tb = new ToolBuilder(toolPanel, "Merge");
            TextBox textbox = tb.addTextbox("New layername:", (cl != null) ? cl.Name + "_merge" : "");
            Label errorLabel = tb.addErrorLabel();
            Button button = tb.addButton("Merge", (Layer l) =>
            {
                if (textbox.Text.Length == 0)
                {
                    tb.setError("Provide name");
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
                            f = new Feature(f.geometry.Union(intersect.geometry));
                        }
                    }
                    newLayer.addFeature(f);
                }

                layers.Insert(0, newLayer);
                redraw();
            });
        }

        private void diffButton_Click(object sender, EventArgs e)
        {
            Layer cl = (Layer)layerList.SelectedItem;
            ToolBuilder tb = new ToolBuilder(toolPanel, "Difference");
            ComboBox layerSelect = tb.addLayerSelect("Subtract:");
            TextBox textbox = tb.addTextbox("New layername:", (cl != null) ? cl.Name + "_diff" : "");
            Label errorLabel = tb.addErrorLabel();
            Button button = tb.addButton("Subtract", (Layer l) =>
            {
                if (textbox.Text.Length == 0)
                {
                    tb.setError("Provide name");
                    return;
                }
                Layer unionLayer = (Layer)layerSelect.SelectedItem;
                if (l.shapetype != unionLayer.shapetype)
                {
                    tb.setError("Incompatible types");
                    return;
                }
                Layer newLayer = new Layer(textbox.Text);
                newLayer.boundingbox = new Envelope(l.boundingbox);
                newLayer.createQuadTree();

                foreach (Feature f in l.features.Values)
                {
                    Feature newf = new Feature((IGeometry)f.geometry.Clone(), f.id);
                    var intersects = unionLayer.getWithin(f.geometry);
                    foreach (Feature intersect in intersects)
                        newf.geometry = newf.geometry.Difference(intersect.geometry);
                    
                    newLayer.addFeature(newf);
                }

                layers.Insert(0, newLayer);
                redraw();
            });
        }

        private void intersectButton_Click(object sender, EventArgs e)
        {
            Layer cl = (Layer)layerList.SelectedItem;
            ToolBuilder tb = new ToolBuilder(toolPanel, "Intersect");
            ComboBox layerSelect = tb.addLayerSelect("Intersect with:");
            TextBox textbox = tb.addTextbox("New layername:", (cl != null) ? cl.Name + "_intersect" : "");
            Label errorLabel = tb.addErrorLabel();
            Button button = tb.addButton("Intersect", (Layer l) =>
            {
                if (textbox.Text.Length == 0)
                {
                    tb.setError("Provide name");
                    return;
                }
                Layer intersectLayer = (Layer)layerSelect.SelectedItem;
                Layer newLayer = new Layer(textbox.Text);
                newLayer.boundingbox = new Envelope(l.boundingbox);
                newLayer.createQuadTree();

                foreach (Feature f in l.features.Values)
                {
                    var intersections = intersectLayer.getWithin(f.geometry);
                    foreach (Feature intersect in intersections)
                        newLayer.addFeature(new Feature(f.geometry.Intersection(intersect.geometry)));
                }

                layers.Insert(0, newLayer);
                redraw();
            });
        }
    }
}
