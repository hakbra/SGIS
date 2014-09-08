using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SGIS
{
    class ToolBuilder
    {
        private TableLayoutPanel panel;
        private Label errorLabel;

        public ToolBuilder(TableLayoutPanel p, string title)
        {
            panel = p;
            panel.Controls.Clear();

            Label titleLabel = new Label();
            titleLabel.Font = new Font(titleLabel.Font, FontStyle.Bold);
            titleLabel.Text = title;
            titleLabel.Anchor = AnchorStyles.None;
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;

            Label lineLabel = new Label();
            lineLabel.Dock = DockStyle.Top;
            lineLabel.Height = 1;
            lineLabel.BorderStyle = BorderStyle.FixedSingle;

            addControl(titleLabel);
            panel.Controls.Add(lineLabel);
        }

        public TextBox addTextbox(string caption)
        {
            return addTextbox(caption, "");
        }
        public TextBox addTextbox(string caption, string value)
        {
            Label textboxLabel = new Label();
            textboxLabel.Text = caption;
            textboxLabel.Anchor = AnchorStyles.None;
            textboxLabel.TextAlign = ContentAlignment.MiddleLeft;

            TextBox textbox = new TextBox();
            textbox.Anchor = AnchorStyles.None;
            textbox.Text = value;

            addControl(textboxLabel);
            addControl(textbox);

            return textbox;
        }

        public Button addButton(string caption, Action<Layer> action)
        {
            Button button = new Button();
            button.Text = caption;
            button.Anchor = AnchorStyles.None;

            addControl(button);

            button.Click += (o, e) =>
            {
                Layer l = (Layer)SGIS.app.getLayerList().SelectedItem;
                if (l == null)
                    return;
                setError("");
                action(l);
                if (errorLabel == null || errorLabel.Text == "")
                    panel.Controls.Clear();
            };

            return button;
        }

        public Button addButton(string caption)
        {
            Button button = new Button();
            button.Text = caption;
            button.Anchor = AnchorStyles.None;

            addControl(button);

            return button;
        }
        public Label addLabel(string text)
        {
            Label label = new Label();
            label.Anchor = AnchorStyles.None;
            label.Text = text;

            addControl(label);

            return label;
        }

        public Label addErrorLabel()
        {
            Label errorLabel = new Label();
            errorLabel.ForeColor = Color.Red;
            errorLabel.Anchor = AnchorStyles.None;

            addControl(errorLabel);

            this.errorLabel = errorLabel;
            return errorLabel;
        }

        public void addControl(Control c)
        {
            panel.Controls.Add(c);
            c.Width = (int) (panel.Width * 0.75);
        }

        public void setError(string s)
        {
            if (errorLabel == null)
                return;
            errorLabel.Text = s;

            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(errorLabel, s);
        }
    }
}
