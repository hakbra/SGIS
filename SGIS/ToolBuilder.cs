using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SGIS
{
    // convenience class for creating tool interface
    class ToolBuilder
    {
        private TableLayoutPanel panel;
        private Label errorLabel;
        private List<ComboBox> layerSelects = new List<ComboBox>();
        private Label currentLabel;
        public bool autoClose = true;
        public Action<Layer> resetAction;
        private Button cancelButton = new Button();

        // constructor sets target panel and adds reset-function to events
        public ToolBuilder(TableLayoutPanel p)
        {
            panel = p;
            SGIS.App.Layers.ListChanged += reset;
            SGIS.App.SelectionChanged += reset;
            SGIS.App.getLayerList().SelectedIndexChanged += reset;
            cancelButton.Click += (o, e) =>
            {
                clear();
            };
        }

        // function called when resetting interface, e.g. when selected layer is changed
        public void reset(object sender, EventArgs e)
        {
            // get selected layer
            Layer layer = (Layer)SGIS.App.getLayerList().SelectedItem;
            // if current tool has a label showing currently selected layer
            if (currentLabel != null)
            {
                string layerName = "No layer selected";
                if (layer != null)
                {
                    layerName = "Layer: " + layer.Name;
                    if (layerName.Length > 20) // shorten layer name if too long
                        layerName = layerName.Substring(0, 20) + "...";
                }
                currentLabel.Text = layerName;
            }
            // if tool has its own reset function, call it
            if (resetAction != null)
                resetAction(layer);
        }

        // convenience function for calling reset function
        public void reset()
        {
            reset(null, null);
        }

        // clear current tool interface
        public void clear()
        {
            panel.Controls.Clear();
            errorLabel = null;
            layerSelects.Clear();
            autoClose = true;
            resetAction = null;
            SGIS.App.AcceptButton = null;
            SGIS.App.CancelButton = cancelButton;
        }
        // convenience function for adding header with current layer label
        public void addHeader(string title)
        {
            addHeader(title, true);
        }
        // function for adding tool header and potential current layer label
        public void addHeader(string title, bool currentLayer)
        {
            clear();
            // label for tool title
            Label titleLabel = new Label();
            titleLabel.Font = new Font(titleLabel.Font, FontStyle.Bold);
            titleLabel.Text = title;
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;

            // line for separating header and content
            Label lineLabel = new Label();
            lineLabel.Dock = DockStyle.Top;
            lineLabel.Height = 1;
            lineLabel.BorderStyle = BorderStyle.FixedSingle;

            addControl(titleLabel);
            if (currentLayer)
                currentLabel = addLabel("Current layer");
            panel.Controls.Add(lineLabel);

            // call reset, mainly to get correct current layer
            reset();
        }

        // convenience function for adding textbox
        public TextBox addTextbox(string value)
        {
            TextBox textbox = new TextBox();
            textbox.Text = value;
            addControl(textbox);
            textbox.Focus();
            textbox.Select(textbox.Text.Length, 0);

            return textbox;
        }
        // convenience function for adding empty textbox with caption
        public TextBox addTextboxWithCaption(string caption)
        {
            return addTextboxWithCaption(caption, "");
        }
        // convenience function for adding textbox with caption
        public TextBox addTextboxWithCaption(string caption, string value)
        {
            // label for textbox caption
            addLabel(caption);
            return addTextbox(value);
        }

        // convenience function for adding button with simple error checking, and potentially an action to be executed
        // for example it is checking that a layer is selected
        public Button addButton(string caption, Action<Layer> action)
        {
            Button button = new Button();
            button.Text = caption;
            SGIS.App.AcceptButton = button;

            addControl(button);

            button.Click += (o, e) =>
            {
                // get current layer
                Layer l = (Layer)SGIS.App.getLayerList().SelectedItem;
                // do nothing if no layer is selected (layerlist is empty)
                if (l == null)
                    return;
                // check that each select layer dropdown is not null, and that the layer is not deleted (not possible?)
                foreach (ComboBox cb in layerSelects)
                {
                    Layer selectedLayer = (Layer)cb.SelectedItem;
                    if (selectedLayer == null)
                    {
                        setError("Select layer");
                        return;
                    }
                    if (!SGIS.App.Layers.Contains(selectedLayer))
                    {
                        setError(selectedLayer.Name + " is deleted");
                        return;
                    }

                }
                setError("");
                try
                {
                    // perform button action
                    action(l);
                }
                // simple error handling, very rarely happens
                catch (NetTopologySuite.Geometries.TopologyException ex)
                {
                    MessageBox.Show(ex.Message, "Topology Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // clear interface if no errors and autoClose is true
                if ((errorLabel == null || errorLabel.Text == "") && autoClose)
                    panel.Controls.Clear();
            };

            return button;
        }

        // convenience function for adding a simple button
        public Button addButton(string caption)
        {
            Button button = new Button();
            button.Text = caption;

            addControl(button);

            return button;
        }
        // convenience function for adding a simple label
        public Label addLabel(string text)
        {
            Label label = new Label();
            label.Text = text;
            label.TextAlign = ContentAlignment.MiddleLeft;

            addControl(label);

            return label;
        }

        // convenience function for adding an error label, only one may exist in a tool interface
        public Label addErrorLabel()
        {
            Label errorLabel = new Label();
            errorLabel.ForeColor = Color.Red; // red text color

            addControl(errorLabel);

            this.errorLabel = errorLabel;
            return errorLabel;
        }

        // function for adding any control and setting correct style and width
        public void addControl(Control c)
        {
            panel.Controls.Add(c);
            c.Anchor = AnchorStyles.None;
            c.Width = (int)(panel.Width * 0.75);
        }

        // convenience function for setting the error text. Will also add toolTip for long error messages
        public void setError(string s)
        {
            if (errorLabel == null)
                return;
            errorLabel.Text = s;

            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(errorLabel, s);
        }

        // convenience function for adding dropdown for selecting layer
        internal ComboBox addLayerSelect(string p)
        {
            addLabel(p);

            // create new dropdown
            ComboBox cb = new ComboBox();
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
            // add all layers to dropdown
            foreach (Layer l in SGIS.App.Layers)
                cb.Items.Add(l);
            // default to select first layer
            if (cb.Items.Count > 0)
                cb.SelectedIndex = 0;
            // on opening dropdown, repopulate dropdown in case new layers are added or removed
            cb.DropDown += (o, e) =>
            {
                Layer l = (Layer) cb.SelectedItem;
                cb.Items.Clear();

                foreach (Layer layer in SGIS.App.Layers)
                {
                    cb.Items.Add(layer);
                    // make sure old selected layer is still selected
                    if (layer == l)
                        cb.SelectedItem = l;
                }
                // if there are layers and none are selected, select the first
                if (cb.SelectedIndex == -1 && cb.Items.Count > 0)
                    cb.SelectedIndex = 0;
            };

            // add dropdown to panel
            addControl(cb);
            // add dropdown to layerSelect list
            layerSelects.Add(cb);

            return cb;
        }
    }
}
