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
            var graphics = Graphics.FromImage(colorImg);
            graphics.FillRectangle(l.style.brush, new Rectangle(0, 0, 20, 20));
            graphics.DrawRectangle(l.style.pen, new Rectangle(0, 0, 20, 20));

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
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Style", colorImg, (o, i) =>
            {
                l.style.pen = new Pen(chooseColor(l.style.pen.Color)); redraw();
            }));
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Rename", null, (o, i) =>
            {
                toolBuilder.addHeader("Rename layer");
                TextBox name = toolBuilder.addTextbox("New name:");
                Label error = toolBuilder.addErrorLabel();
                Button button = toolBuilder.addButton("Rename", (Layer currentLayer) =>
                {
                    if (name.Text.Length == 0)
                    {
                        toolBuilder.setError("Provide name");
                        return;
                    }
                    currentLayer.Name = name.Text;
                });
                toolBuilder.reset();
            }));
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Remove", null, (o, i) =>
            {
                toolBuilder.addHeader("Delete layer");
                toolBuilder.addLabel("Are you sure?");
                toolBuilder.addButton("Yes", (Layer il) =>
                {
                    SGIS.app.layers.Remove(il);
                    redraw();
                });
                toolBuilder.addButton("No", (il) => { });
                toolBuilder.resetAction = (Layer il) =>
                {
                    toolBuilder.clear();
                };
            }));
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
