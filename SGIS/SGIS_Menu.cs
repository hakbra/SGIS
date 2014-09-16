using System;
using System.Collections.Concurrent;
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
        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void shapefileMenuItem_Click(object sender, EventArgs e)
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

                    ScreenManager.Calculate();
                    layerList.SelectedIndex = 0;
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
    }
}
