using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SGIS
{
    public partial class SGIS
    {
        MouseTactic mouse = new MoveMouseTactic();
        private ContextMenuStrip infoContextMenu = new ContextMenuStrip();

        private void SGIS_MouseWheel(object sender, MouseEventArgs e)
        {
            mouse.MouseWheel(e);
        }

        private void SGIS_MouseDown(object sender, MouseEventArgs e)
        {
            mouse.MouseDown(e);
        }

        private void SGIS_MouseUp(object sender, MouseEventArgs e)
        {
            mouse.MouseUp(e);
        }

        private void SGIS_MouseMove(object sender, MouseEventArgs e)
        {
            mouse.MouseMove(e);
        }

        // select move mouse, disable button, enable the two others
        private void mouseMoveItem_MouseDown(object sender, EventArgs e)
        {
            mouse = new MoveMouseTactic();
            mouseMoveButton.Enabled = false;
            mouseInfoButton.Enabled = true;
            mouseSelectButton.Enabled = true;
            SGIS.App.MapWindow.Cursor = Cursors.Hand;
        }

        // select select mouse, disable button, enable the two others
        private void mouseSelectItem_Click(object sender, EventArgs e)
        {
            mouse = new SelectMouseTactic();
            mouseMoveButton.Enabled = true;
            mouseInfoButton.Enabled = true;
            mouseSelectButton.Enabled = false;
            SGIS.App.MapWindow.Cursor = Cursors.Cross;
        }

        // select info mouse, disable button, enable the two others
        private void mouseInfoItem_Click(object sender, EventArgs e)
        {
            mouse = new InfoMouseTactic();
            mouseMoveButton.Enabled = true;
            mouseSelectButton.Enabled = true;
            mouseInfoButton.Enabled = false;
            SGIS.App.MapWindow.Cursor = Cursors.Help;
        }

        // function for populating menu when clicking on feature with info mouse
        private void infoContextMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            Layer layer = (Layer)layerList.SelectedItem;
            if (layer == null || layer.Selected.Count != 1)
                return;

            // get first and only selected feature
            Feature f = layer.Selected.First();
            DataRow dr = layer.getRow(f);

            infoContextMenu.Items.Clear();
            // if no attributes exists in layer
            if (dr == null)
            {
                infoContextMenu.Items.Add("No data");
                return;
            }
            // loop through columns in attribute data set
            foreach (DataColumn column in layer.DataTable.Columns)
            {
                string colName = column.ToString();
                // skip internal id column
                if (colName == "sgis_id")
                    continue;
                //  add column name, value and onclick-function
                infoContextMenu.Items.Add(colName + ": " + dr[colName].ToString().Trim(), null, (o, e2) =>
                {
                    // select all rows with same attribute value
                    var rows = layer.DataTable.Select("[" + colName + "] = '" + dr[colName] + "'");
                    layer.clearSelected();
                    // loop through all rows
                    foreach(var row in rows){
                        Feature selectedFeature;
                        if (layer.Features.TryGetValue((int)row["sgis_id"], out selectedFeature))
                        {
                            // add feature to selected set
                            selectedFeature.Selected = true;
                            layer.Selected.Add(selectedFeature);
                        }
                        SGIS.App.fireSelectionChanged();
                    }
                    redraw();
                });
            }
        }
    }
}
