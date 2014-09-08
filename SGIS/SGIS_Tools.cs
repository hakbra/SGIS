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
            TextBox nameBox = tb.addTextbox("Layer name:", (cl != null) ? cl.Name : "");
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

                Layer newl = new Layer(nameBox.Text, ShapeType.POLYGON);
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
    }
}
