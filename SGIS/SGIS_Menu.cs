using System;
using System.Collections.Generic;
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
                foreach (String file in openFileDialog1.FileNames)
                {
                    Layer l = ShpReader.read(file);
                    Layers.Insert(0, l);
                    layerList.SelectedIndex = 0;
                    this.layerList_SelectedIndexChanged(null, null);
                    ScreenManager.RealRect.Set(l.Boundingbox);
                    ScreenManager.Calculate();
                    this.Refresh();
                }
            }
        }
    }
}
