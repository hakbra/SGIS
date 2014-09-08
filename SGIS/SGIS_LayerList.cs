using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SGIS
{
    public partial class SGIS
    {
        private ContextMenuStrip layerListContextMenu = new ContextMenuStrip();

        private void layerListContextMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            Layer l = (Layer)layerList.SelectedItem;
            Bitmap colorImg = new Bitmap(20, 20);
            for (int x = 0; x < 20; x++)
                for (int y = 0; y < 20; y++)
                    colorImg.SetPixel(x, y, l.color);

            layerListContextMenu.Items.Clear();
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Up", null, (o, i) =>
            {
                Layer selected = (Layer)layerList.SelectedItem;
                int index = layers.IndexOf(l);
                if (index == 0)
                    return;
                layers.Remove(l);
                layers.Insert(index - 1, l);
                layerList.SelectedItem = selected;
                redraw();
            }));
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Down", null, (o, i) =>
            {
                Layer selected = (Layer)layerList.SelectedItem;
                int index = layers.IndexOf(l);
                if (index == layers.Count - 1)
                    return;
                layers.Remove(l);
                layers.Insert(index + 1, l);
                layerList.SelectedItem = selected;
                redraw();
            }));
            layerListContextMenu.Items.Add("-");
            layerListContextMenu.Items.Add(new ToolStripMenuItem(l.visible ? "Hide" : "Show", null, (o, i) => { l.visible = !l.visible; redraw(); }));
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Zoom", null, (o, i) => { screenManager.RealRect.Set(l.boundingbox); screenManager.Calculate(); redraw(); }));
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Color...", colorImg, (o, i) =>
            {
                l.color = chooseColor(l.color); redraw();
            }));
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Rename", null, (o, i) =>
            {
                ToolBuilder tb = new ToolBuilder(toolPanel, "Rename layer");
                TextBox name = tb.addTextbox("New name:");
                Label error = tb.addErrorLabel();
                Button button = tb.addButton("Rename", (Layer currentLayer) =>
                {
                    if (name.Text.Length == 0)
                    {
                        tb.setError("Provide name");
                        return;
                    }
                    currentLayer.Name = name.Text;
                });
            }));
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Remove", null, (o, i) => { layers.Remove(l); redraw(); }));
        }

        internal ListBox getLayerList()
        {
            return layerList;
        }

        private void layerList_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                var index = layerList.IndexFromPoint(e.Location);
                if (index != -1)
                    layerList.SelectedIndex = index;
                if (layerList.SelectedIndex != -1)
                    layerListContextMenu.Show(layerList.PointToScreen(e.Location));
            }
            Layer l = (Layer)(layerList.SelectedItem);
            if (l == null || l.selected.Count == 0)
                SGIS.app.setStatusText("");
            else
                SGIS.app.setStatusText(l.selected.Count + " objects");
            redraw();
        }

        private void layerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Layer l = (Layer)layerList.SelectedItem;
            if (l == null)
            {
                selectAllButton.Enabled = false;
                selectNoneButton.Enabled = false;
                selectPropButton.Enabled = false;
                selectInvertButton.Enabled = false;
            }
            else
            {
                selectAllButton.Enabled = true;
                selectNoneButton.Enabled = true;
                selectInvertButton.Enabled = true;
                selectPropButton.Enabled = l.dataTable != null;
            }
            redraw();
        }
    }
}
