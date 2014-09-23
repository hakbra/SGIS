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

        public delegate void SelectionChangedHandler();
        public event SelectionChangedHandler SelectionChanged;

        private ContextMenuStrip layerListContextMenu = new ContextMenuStrip();

        private void layerListContextMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            Layer l = (Layer)layerList.SelectedItem;
            Bitmap colorImg = new Bitmap(20, 20);
            var graphics = Graphics.FromImage(colorImg);
            graphics.FillRectangle(l.Style.brush, new Rectangle(0, 0, 20, 20));
            graphics.DrawRectangle(l.Style.pen, new Rectangle(0, 0, 20, 20));

            layerListContextMenu.Items.Clear();
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Up", null, (o, i) =>
            {
                upButton_Click(o, i);
            }));
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Down", null, (o, i) =>
            {
                downButton_Click(o, i);
            }));
            layerListContextMenu.Items.Add("-");
            layerListContextMenu.Items.Add(new ToolStripMenuItem(l.Visible ? "Hide" : "Show", null, (o, i) => { l.Visible = !l.Visible; redraw(); }));
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Zoom", null, (o, i) => { ScreenManager.RealRect.Set(l.Boundingbox); ScreenManager.Calculate(); redraw(); }));
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Style", colorImg, (o, i) =>
            {
                toolBuilder.addHeader("Style layer");

                Label fcolorLabel = toolBuilder.addLabel("Fill color:");
                Button fcolor = toolBuilder.addButton("");
                fcolor.Click += (o2, e2) =>
                {
                    ColorDialog cd = new ColorDialog();
                    if (cd.ShowDialog() == DialogResult.OK)
                    {
                        Layer cl = (Layer)layerList.SelectedItem;
                        fcolor.BackColor = cd.Color;
                        if (cl != null)
                        {
                            cl.Style.brush = new SolidBrush(cd.Color);
                            redraw();
                        }
                    }
                };

                Label lcolorLabel = toolBuilder.addLabel("Line color:");
                Button lcolor = toolBuilder.addButton("");
                lcolor.Click += (o2, e2) =>
                {
                    ColorDialog cd = new ColorDialog();
                    if (cd.ShowDialog() == DialogResult.OK)
                    {
                        Layer cl = (Layer)layerList.SelectedItem;
                        lcolor.BackColor = cd.Color;
                        if (cl != null)
                        {
                            cl.Style.pen.Color = cd.Color;
                            redraw();
                        }
                    }
                };

                TextBox alphaText = toolBuilder.addTextbox("Opacity 1-255:");
                alphaText.TextChanged += (o2, e2) =>
                {
                    Layer cl = (Layer)layerList.SelectedItem;
                    int alpha;
                    if (cl != null && int.TryParse(alphaText.Text, out alpha))
                    {
                        if (alpha >= 0 && alpha <= 255)
                        {
                            Color c = cl.Style.brush.Color;
                            c = Color.FromArgb(alpha, c);
                            cl.Style.brush = new SolidBrush(c);
                            redraw();
                        }
                    }
                };

                toolBuilder.resetAction = (Layer il) =>
                {
                    if (l != null)
                    {
                        fcolor.BackColor = il.Style.brush.Color;
                        lcolor.BackColor = il.Style.pen.Color;
                        alphaText.Text = il.Style.brush.Color.A.ToString();
                    }

                };
                toolBuilder.reset();
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
                delButton_Click(o, i);
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
                var oldIndex = layerList.SelectedIndex;
                var index = layerList.IndexFromPoint(e.Location);
                if (index != -1 && oldIndex != index)
                    layerList.SelectedIndex = index;
                if (layerList.SelectedIndex != -1)
                    layerListContextMenu.Show(layerList.PointToScreen(e.Location));
            }
        }

        private void layerList_SelectedIndexChanged(object sender, EventArgs e)
        {
           redraw();
        }

        public void fireSelectionChanged()
        {
            SelectionChanged();
        }

        private void selectionChangedHandler()
        {
            Layer l = (Layer)(layerList.SelectedItem);
            if (l == null || l.Selected.Count == 0)
                SGIS.App.StatusText = ("");
            else
                SGIS.App.StatusText = (l.Selected.Count + " objects");
        }
    }
}
