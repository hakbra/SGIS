using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SGIS
{
    public partial class SGIS
    {
        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolBuilder.addHeader("Spatial Ref. Sys.", false);
            Label current = toolBuilder.addLabel("Current SRS:");
            Label currentSrs = toolBuilder.addLabel("");
            Label setNew = toolBuilder.addLabel("Set new SRS:");
            setNew.Height = current.Height = (int)(current.Height * 0.7);

            ComboBox type = new ComboBox();
            type.Items.Add("EPSG");
            type.SelectedIndex = 0;
            type.DropDownStyle = ComboBoxStyle.DropDownList;
            toolBuilder.addControl(type);

            TextBox value = toolBuilder.addTextbox("");
            toolBuilder.addErrorLabel();
            Button setButton = toolBuilder.addButton("Set");

            currentSrs.Text = getSrsName();

            setButton.Click += (o, e2) =>
            {
                if (value.Text == "")
                {
                    toolBuilder.setError("Please enter value");
                    return;
                }
                try
                {
                    SRS = Proj4CSharp.Proj4CSharp.ProjectionFactoryFromName("EPSG:"+value.Text);
                }
                catch (Exception ex)
                {
                    toolBuilder.setError("Invalid SRS");
                    return;
                }
                toolBuilder.setError("");
                currentSrs.Text = getSrsName();
            };
        }
    }
}
