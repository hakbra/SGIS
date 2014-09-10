﻿using System;
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

        private void mouseMoveItem_MouseDown(object sender, EventArgs e)
        {
            mouse = new MoveMouseTactic();
            mouseMoveButton.Enabled = false;
            mouseInfoButton.Enabled = true;
            mouseSelectButton.Enabled = true;
            SGIS.app.getMapWindow().Cursor = Cursors.Hand;
        }

        private void mouseSelectItem_Click(object sender, EventArgs e)
        {
            mouse = new SelectMouseTactic();
            mouseMoveButton.Enabled = true;
            mouseInfoButton.Enabled = true;
            mouseSelectButton.Enabled = false;
            SGIS.app.getMapWindow().Cursor = Cursors.Cross;
        }

        private void mouseInfoItem_Click(object sender, EventArgs e)
        {
            mouse = new InfoMouseTactic();
            mouseMoveButton.Enabled = true;
            mouseSelectButton.Enabled = true;
            mouseInfoButton.Enabled = false;
            SGIS.app.getMapWindow().Cursor = Cursors.Help;
        }

        private void infoContextMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            Layer layer = (Layer)layerList.SelectedItem;
            if (layer == null || layer.selected.Count != 1)
                return;

            Feature f = layer.selected.First();
            DataRow dr = layer.dataTable.Rows[f.id-1];

            infoContextMenu.Items.Clear();
            foreach (DataColumn column in layer.dataTable.Columns)
            {
                string colName = column.ToString();
                if (colName == "sgis_id")
                    continue;
                infoContextMenu.Items.Add(colName + ": " + dr[colName], null, (o, e2) =>
                {
                    var rows = layer.dataTable.Select("[" + colName + "] = '" + dr[colName] + "'");
                    layer.clearSelected();
                    foreach(var row in rows){
                        Feature selectedFeature;
                        if (layer.features.TryGetValue((int)row["sgis_id"], out selectedFeature))
                        {
                            selectedFeature.selected = true;
                            layer.selected.Add(selectedFeature);
                        }
                    }
                    redraw();
                });
            }
        }
    }
}
