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
            toolBuilder.addHeader("Select by property");
            TextBox textbox = toolBuilder.addTextbox("Expression:");

            ComboBox comboBox = new ComboBox();
            comboBox.Items.Add("Column name");
            comboBox.SelectedIndex = 0;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            toolBuilder.addControl(comboBox);
            toolBuilder.autoClose = false;

            Label errorLabel = toolBuilder.addErrorLabel();
            Button selectButton = toolBuilder.addButton("Select", (l) =>
            {
                if (l.dataTable == null)
                    return;
                try
                {
                    DataRow[] rows = l.dataTable.Select(textbox.Text);
                    l.clearSelected();

                    foreach (DataRow dr in rows)
                    {
                        int id = (int)dr["sgis_id"];
                        Feature f;
                        if (l.features.TryGetValue(id, out f))
                        {
                            f.selected = true;
                            l.selected.Add(f);
                        }
                    }
                    SGIS.app.setStatusText(l.selected.Count + " objects");
                    SGIS.app.redraw();
                }
                catch (Exception ex)
                {
                    toolBuilder.setError(ex.Message);
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
            toolBuilder.reset();
        }

        private void toLayerButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Export selection");
            TextBox textbox = toolBuilder.addTextbox("Layer name:");
            Label errorLabel = toolBuilder.addErrorLabel();
            Button selectButton = toolBuilder.addButton("Copy selection", (Layer l) =>
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
            toolBuilder.resetAction = (Layer l) =>
            {
                textbox.Text = (l == null) ? "" : l.Name + "_copy";
            };
            toolBuilder.reset();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Delete selection");
            toolBuilder.addLabel("Are you sure?");
            toolBuilder.addButton("Yes", (Layer l) =>
            {
                foreach (Feature f in l.selected)
                    l.features.Remove(f.id);
                l.clearSelected();
                l.createQuadTree();
                setStatusText("0 objects");
                redraw();
            });
            toolBuilder.addButton("No", (l) => { });
            toolBuilder.resetAction += (Layer l) =>
            {
                toolBuilder.clear();
            };
        }
    }
}
