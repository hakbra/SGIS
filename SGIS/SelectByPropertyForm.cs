using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SGIS
{
    public partial class SelectByPropertyForm : Form
    {
        Layer l;
        public SelectByPropertyForm()
        {
            InitializeComponent();

            int index = SGIS.app.getLayerList().SelectedIndex;
            layerList.DataSource = SGIS.app.layers;
            layerList.SelectedIndex = index;
            populateColumns();
            queryBox.Select();
        }

        private void populateColumns()
        {
            columnBox.Items.Clear();
            columnBox.Items.Add("Column name");
            foreach (var column in l.dataTable.Columns)
            {
                string colName = column.ToString();
                columnBox.Items.Add(colName);
            }
            columnBox.SelectedIndex = 0;
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow[] rows = l.dataTable.Select(queryBox.Text);
                l.clearSelected();

                foreach (DataRow dr in rows) {
                    int id = (int) dr[0];
                    Feature f = l.features[id];
                    f.selected = true;
                    l.selected.Add(f);
                }
                SGIS.app.redraw();
            }
            catch (EvaluateException ex)
            {
                errorLabel.Text = ex.Message;
            }
        }

        private void columnBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (columnBox.SelectedIndex == 0)
                return;

            queryBox.Text += "["+columnBox.SelectedItem.ToString() + "] ";
            columnBox.SelectedIndex = 0;
            queryBox.Focus();
            queryBox.Select(queryBox.Text.Length, 0);
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void layerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            l = (Layer) layerList.SelectedItem;
            SGIS.app.getLayerList().SelectedItem = l;
            populateColumns();
            SGIS.app.redraw();
        }
    }
}
