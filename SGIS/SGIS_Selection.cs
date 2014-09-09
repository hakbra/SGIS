using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SGIS
{
    public partial class SGIS
    {
        private void selectInvertButton_Click(object sender, EventArgs e)
        {
            Layer l = (Layer)layerList.SelectedItem;
            if (l == null)
                return;
            List<Feature> newSelected = new List<Feature>();
            foreach (Feature f in l.features.Values)
            {
                f.selected = !f.selected;
                if (f.selected)
                    newSelected.Add(f);
            }
            l.selected = newSelected;
            setStatusText(newSelected.Count + " objects");
            redraw();
        }

        private void selectAllItem_Click(object sender, EventArgs e)
        {
            Layer l = (Layer)layerList.SelectedItem;
            if (l == null)
                return;
            l.clearSelected();
            foreach (Feature f in l.features.Values)
            {
                l.selected.Add(f);
                f.selected = true;
            }
            setStatusText(l.features.Count + " objects");
            redraw();
        }

        private void selectNoneItem_Click(object sender, EventArgs e)
        {
            Layer l = (Layer)layerList.SelectedItem;
            if (l == null)
                return;
            l.clearSelected();
            setStatusText("");
            redraw();
        }

        private void selectByPropertyItem_Click(object sender, EventArgs e)
        {
            ToolBuilder tb = new ToolBuilder(toolPanel, "Select by property");
            TextBox textbox = tb.addTextbox("Expression:");

            ComboBox comboBox = new ComboBox();
            comboBox.Items.Add("Column name");
            comboBox.SelectedIndex = 0;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Anchor = AnchorStyles.None;
            tb.addControl(comboBox);
            tb.autoClose = false;

            Label errorLabel = tb.addErrorLabel();
            Button selectButton = tb.addButton("Select", (l) =>
            {
                if (l.dataTable == null)
                    return;
                try
                {
                    DataRow[] rows = l.dataTable.Select(textbox.Text);
                    l.clearSelected();

                    foreach (DataRow dr in rows)
                    {
                        int id = (int)dr[0];
                        Feature f;
                        if (l.features.TryGetValue(id, out f))
                        {
                            f.selected = true;
                            l.selected.Add(f);
                        }
                    }
                    SGIS.app.redraw();
                }
                catch (Exception ex)
                {
                    tb.setError(ex.Message);
                }
            });

            comboBox.DropDown += (o, ev) =>
            {
                Layer l = (Layer)layerList.SelectedItem;
                if (l == null)
                    return;
                comboBox.Items.Clear();
                comboBox.Items.Add("Column name");
                foreach (var column in l.dataTable.Columns)
                {
                    string colName = column.ToString();
                    comboBox.Items.Add(colName);
                }
                comboBox.SelectedIndex = 0;
            };
            comboBox.SelectedIndexChanged += (o, ev) =>
            {
                if (comboBox.SelectedIndex == 0)
                    return;

                textbox.Text += "[" + comboBox.SelectedItem.ToString() + "] ";
                comboBox.SelectedIndex = 0;
                textbox.Focus();
                textbox.Select(textbox.Text.Length, 0);
            };
        }

        private void toLayerButton_Click(object sender, EventArgs e)
        {
            ToolBuilder tb = new ToolBuilder(toolPanel, "Export selection");
            TextBox textbox = tb.addTextbox("Layer name:");

            Layer cl = (Layer)layerList.SelectedItem;
            if (cl != null)
                textbox.Text = cl.Name + "_copy";
            textbox.Focus();
            textbox.Select(textbox.Text.Length, 0);

            Label errorLabel = tb.addErrorLabel();
            Button selectButton = tb.addButton("Copy selection", (Layer l) =>
            {
                if (textbox.Text.Length == 0)
                {
                    errorLabel.Text = "Provide a name";
                    return;
                }

                Layer newl = new Layer(textbox.Text);
                newl.dataTable = l.dataTable;
                newl.boundingbox = l.boundingbox;
                newl.createQuadTree();
                foreach (Feature f in l.selected)
                    newl.addFeature(new Feature((Geometry)f.geometry.Clone(), f.id));
                layers.Insert(0, newl);
                layerList.SelectedItem = newl;
            });
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            ToolBuilder tb = new ToolBuilder(toolPanel, "Delete selection");
            tb.addLabel("Are you sure?");
            tb.addButton("Yes", (Layer l) =>
            {
                foreach (Feature f in l.selected)
                    l.features.Remove(f.id);
                l.clearSelected();
                l.createQuadTree();
                setStatusText("0 objects");
                redraw();
            });
            tb.addButton("No", (l) => { });
        }
    }
}
