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
            Form options = new Form();

            FlowLayoutPanel optionsFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            GroupBox srsOptionsGroup = new System.Windows.Forms.GroupBox();
            Button closeButton = new System.Windows.Forms.Button();
            FlowLayoutPanel srsOptionsFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            TextBox srsTextBox = new System.Windows.Forms.TextBox();
            Button setSrsButton = new System.Windows.Forms.Button();
            ComboBox srsType = new ComboBox();
            Label currentSRS = new Label();
            Label errorLabel = new Label();

            optionsFlowLayout.SuspendLayout();
            srsOptionsGroup.SuspendLayout();
            srsOptionsFlowLayout.SuspendLayout();
            SuspendLayout();
            // 
            // optionsFlowLayout
            // 
            optionsFlowLayout.Controls.Add(srsOptionsGroup);
            optionsFlowLayout.SetFlowBreak(srsOptionsGroup, true);
            optionsFlowLayout.Controls.Add(closeButton);
            optionsFlowLayout.Location = new System.Drawing.Point(0, 0);
            optionsFlowLayout.AutoSize = true;
            optionsFlowLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            // 
            // srsOptionsGroup
            // 
            srsOptionsGroup.Controls.Add(srsOptionsFlowLayout);
            srsOptionsGroup.Location = new System.Drawing.Point(3, 3);
            srsOptionsGroup.Text = "Spatial Reference System";
            // 
            // currentSRS
            currentSRS.Text = "Placeholder";
            currentSRS.Height = 20;
            // closeButton
            // 
            closeButton.Text = "Close";
            closeButton.Height = 20;
            // errorLabel
            errorLabel.Height = 20;
            // 
            // srsOptionsFlowLayout
            // 
            srsOptionsFlowLayout.Controls.Add(currentSRS);
            srsOptionsFlowLayout.SetFlowBreak(currentSRS, true);
            srsOptionsFlowLayout.Controls.Add(srsType);
            srsOptionsFlowLayout.Controls.Add(srsTextBox);
            srsOptionsFlowLayout.SetFlowBreak(srsTextBox, true);
            srsOptionsFlowLayout.Controls.Add(setSrsButton);
            srsOptionsFlowLayout.SetFlowBreak(setSrsButton, true);
            srsOptionsFlowLayout.Controls.Add(errorLabel);
            srsOptionsFlowLayout.Location = new System.Drawing.Point(5, 20);
            srsOptionsFlowLayout.AutoSize = true;
            srsOptionsFlowLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            // srsDropDown
            srsType.DropDownStyle = ComboBoxStyle.DropDownList;
            srsType.Items.Add("EPSG");
            srsType.SelectedIndex = 0;
            srsType.Width = 60;
            srsType.Height = 20;

            // 
            // srsTextBox
            // 
            // 
            // setSrsButton
            // 
            setSrsButton.Text = "Set";
            setSrsButton.Dock = DockStyle.Fill;
            // 
            // Form1
            // 
            options.AutoSize = true;
            options.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            
            options.Controls.Add(optionsFlowLayout);
            options.Text = "Options";
            optionsFlowLayout.ResumeLayout(false);
            srsOptionsGroup.ResumeLayout(false);
            srsOptionsFlowLayout.ResumeLayout(false);
            srsOptionsFlowLayout.PerformLayout();
            options.ResumeLayout(false);

            options.Show();
        }
    }
}
