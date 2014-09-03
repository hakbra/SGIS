using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoAPI.Geometries;

namespace SGIS
{
    public partial class LayerControl : UserControl
    {
        public Layer layer;
        public static LayerControl current;
        public static SGIS sgis;

        public LayerControl()
        {
            InitializeComponent();
        }
        public void setLayer(Layer l)
        {
            layer = l;

            int labellength = 50;
            string label = l.ToString();
            if (label.Length < labellength+3)
                label1.Text = label;
            else
            {
                label1.Text = label.Substring(0, labellength) + "...";

                ToolTip yourToolTip = new ToolTip();
                yourToolTip.ShowAlways = true;
                yourToolTip.SetToolTip(label1, label);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult res = colorDialog1.ShowDialog();
            if (res == DialogResult.OK)
                layer.color = colorDialog1.Color;
            sgis.Refresh();
        }

        private void LayerControl_Click(object sender, EventArgs e)
        {
            current.BackColor = Color.White;
            this.BackColor = Color.LightBlue;
            current = this;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.LayerControl_Click(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            layer.visible = !layer.visible;
            sgis.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var bb = new Envelope(layer.boundingbox);
            sgis.screenManager.RealRect.Set(bb);
            sgis.screenManager.Calculate();
            sgis.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            sgis.layers.Remove(layer);
            sgis.Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var layers = sgis.layers;
            int index = layers.IndexOf(layer);
            index = Math.Max(0, index - 1);
            layers.Remove(layer);
            layers.Insert(index, layer);
            sgis.Refresh();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var layers = sgis.layers;
            int index = layers.IndexOf(layer);
            if (index == layers.Count-1)
                return;
            index += 1;
            layers.Remove(layer);
            layers.Insert(index, layer);
            sgis.Refresh();
        }
    }
}
