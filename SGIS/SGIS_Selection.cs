using GeoAPI.Geometries;
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
    // partial class conatining functions related to selecting features
    public partial class SGIS
    {
        // inverts slection
        private void selectInvertButton_Click(object sender, EventArgs e)
        {
            Layer l = (Layer)layerList.SelectedItem;
            if (l == null)
                return;
            List<Feature> newSelected = new List<Feature>();
            foreach (Feature f in l.Features.Values)
            {
                f.Selected = !f.Selected;
                if (f.Selected)
                    newSelected.Add(f);
            }
            l.Selected = newSelected;
            fireSelectionChanged();
            redraw();
        }

        // selects all features
        private void selectAllItem_Click(object sender, EventArgs e)
        {
            Layer l = (Layer)layerList.SelectedItem;
            if (l == null)
                return;
            l.clearSelected();
            foreach (Feature f in l.Features.Values)
            {
                l.Selected.Add(f);
                f.Selected = true;
            }
            StatusText = (l.Features.Count + " objects");
            redraw();
        }

        // clears all selected features
        private void selectNoneItem_Click(object sender, EventArgs e)
        {
            Layer l = (Layer)layerList.SelectedItem;
            if (l == null)
                return;
            l.clearSelected();
            StatusText = ("");
            redraw();
        }

        // selects features based on attribute expression
        private void selectByPropertyItem_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Select by property");

            // textbox for expression
            TextBox textbox = toolBuilder.addTextboxWithCaption("Expression:");

            // dropdown list with available column names
            ComboBox comboBox = new ComboBox();
            comboBox.Items.Add("Column name");
            comboBox.SelectedIndex = 0;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            toolBuilder.addControl(comboBox);
            toolBuilder.autoClose = false;

            Label errorLabel = toolBuilder.addErrorLabel();

            // button for performing selection
            Button selectButton = toolBuilder.addButton("Select", (l) =>
            {
                if (l.DataTable == null)
                    return;
                try
                {
                       // get rows satisfying the expression
                    DataRow[] rows = l.DataTable.Select(textbox.Text);
                    l.clearSelected();

                    foreach (DataRow dr in rows)
                    {
                        int id = (int)dr["sgis_id"];
                        Feature f;
                        if (l.Features.TryGetValue(id, out f))
                        {
                            f.Selected = true;
                            l.Selected.Add(f);
                        }
                    }
                    SGIS.App.fireSelectionChanged();
                    SGIS.App.redraw();
                }
                catch (Exception ex)
                {
                    toolBuilder.setError(ex.Message);
                }
            });

            // populate column name dropdown
            comboBox.DropDown += (o, ev) =>
            {
                Layer l = (Layer)layerList.SelectedItem;
                if (l == null)
                    return;
                comboBox.Items.Clear();
                comboBox.Items.Add("Column name");
                foreach (var column in l.DataTable.Columns)
                {
                    string colName = column.ToString();
                    if (colName != "sgis_id")
                        comboBox.Items.Add(colName);
                }
                comboBox.SelectedIndex = 0;
            };

            // on column name selected, add [columnName] to expression textBox and reset dropDown
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

        // copy selected feature to new layer
        private void toLayerButton_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Export selection");
            // textbox for new layer name
            TextBox textbox = toolBuilder.addTextboxWithCaption("Layer name:");
            Label errorLabel = toolBuilder.addErrorLabel();
            // button for performing copy
            Button selectButton = toolBuilder.addButton("Copy selection", (Layer l) =>
            {
                if (textbox.Text.Length == 0)
                {
                    errorLabel.Text = "Provide a name";
                    return;
                }

                Layer newl = new Layer(textbox.Text);
                // copy attributes
                newl.DataTable = l.DataTable;
                // copy features
                foreach (Feature f in l.Selected)
                    newl.addFeature(new Feature((Geometry)f.Geometry.Clone(), f.ID));
                newl.calculateBoundingBox();
                newl.createQuadTree();
                Layers.Insert(0, newl);
                // select newly made layer
                layerList.SelectedItem = newl;
            });
            // change default new layer name when new layer is selected
            toolBuilder.resetAction = (Layer l) =>
            {
                textbox.Text = (l == null) ? "" : l.Name + "_copy";
            };
            toolBuilder.reset();
        }

        // zoom to selected features
        private void zoomButton_Click(object sender, EventArgs e)
        {
            Layer l = (Layer)layerList.SelectedItem;
            if (l == null || l.Selected.Count == 0)
                return;
            // create new boundingbox
            Envelope boundingbox = null;
            foreach (Feature f in l.Selected)
            {
                // expand to include feature
                if (boundingbox == null)
                    boundingbox = f.Geometry.EnvelopeInternal;
                else
                    boundingbox.ExpandToInclude(f.Geometry.EnvelopeInternal);
            }
            // calculate new window coords
            SGIS.App.ScreenManager.RealRect.Set(boundingbox);
            SGIS.App.ScreenManager.Calculate();
            SGIS.App.redraw();
        }
    }
}
