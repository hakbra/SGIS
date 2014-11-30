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

        // populates drop down when right click in layer list occures
        private void layerListContextMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            Layer l = (Layer)layerList.SelectedItem;

            // creates colored quare representing the layers style
            Bitmap colorImg = new Bitmap(20, 20);
            var graphics = Graphics.FromImage(colorImg);
            lock (l)
            {
                graphics.FillRectangle(l.Style.brush, new Rectangle(0, 0, 20, 20));
                graphics.DrawRectangle(l.Style.pen, new Rectangle(0, 0, 20, 20));
            }

            layerListContextMenu.Items.Clear();
            // move layer up
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Up", null, (o, i) =>
            {
                upButton_Click(o, i);
            }));
            // move layer down
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Down", null, (o, i) =>
            {
                downButton_Click(o, i);
            }));
            layerListContextMenu.Items.Add("-");
            // hide layer
            layerListContextMenu.Items.Add(new ToolStripMenuItem(l.Visible ? "Hide" : "Show", null, (o, i) => { l.Visible = !l.Visible; redraw(); }));
            // zoom to layer extent
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Zoom", null, (o, i) => { ScreenManager.RealRect.Set(l.Boundingbox); ScreenManager.Calculate(); redraw(); }));
            // change layer style
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Style", colorImg, (o, i) =>
            {
                toolBuilder.addHeader("Style layer");

                // button for selecting fill color
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
                            Color c = Color.FromArgb(cl.Style.brush.Color.A, cd.Color);
                            cl.Style.brush = new SolidBrush(c);
                            redraw();
                        }
                    }
                };

                // button for selecting line color
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

                // textbox for changing layer opacity
                TextBox alphaText = toolBuilder.addTextboxWithCaption("Opacity 1-255:");
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

                // textbox for changing linewidth or pointsize
                Label widthLabel = toolBuilder.addLabel("");
                TextBox widthText = toolBuilder.addTextbox("");
                widthText.TextChanged += (o2, e2) =>
                {
                    Layer cl = (Layer)layerList.SelectedItem;
                    float width;
                    if (cl != null &&
                        float.TryParse(widthText.Text, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture,out width))
                    {
                        if (width >= 0 && width <= 20)
                        {
                            Color c = cl.Style.pen.Color;
                            cl.Style.pen = new Pen(c, width);
                            Color sc = Style.Selected.pen.Color;
                            Style.Selected.pen = new Pen(sc, width);
                            redraw();
                        }
                    }
                };

                // function called when selected layer is changed
                toolBuilder.resetAction = (Layer il) =>
                {
                    if (l != null)
                    {
                        while (true)
                        {
                            try
                            {
                                fcolor.BackColor = il.Style.brush.Color;
                                lcolor.BackColor = il.Style.pen.Color;
                                alphaText.Text = il.Style.brush.Color.A.ToString();
                                widthText.Text = il.Style.pen.Width.ToString();
                                break;
                            } catch (Exception ex)
                            {

                            }
                        }
                    }
                    if (il.shapetype == ShapeType.POINT)
                        widthLabel.Text = "Point size:";
                    else
                        widthLabel.Text = "Line width:";
                };
                toolBuilder.reset();
            }));
            // rename layer
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Rename", null, (o, i) =>
            {
                toolBuilder.addHeader("Rename layer");
                TextBox name = toolBuilder.addTextboxWithCaption("New name:");
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
            // delete layer
            layerListContextMenu.Items.Add(new ToolStripMenuItem("Delete", null, (o, i) =>
            {
                delButton_Click(o, i);
            }));
        }

        internal ListBox getLayerList()
        {
            return layerList;
        }

        // function called on mouse click in layerlist
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

        // function for setting correct amount of selected objects in statusbar
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
