using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clear_button_for_combo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            for (int i = 0; i < 3; i++)
            {
                var comboBoxClearUserControl = new ComboBoxClearUserControl
                {
                    Width = 250,
                };
                ComboBox comboBox = comboBoxClearUserControl;
                comboBox.FormattingEnabled = true;
                comboBox.Items.AddRange(new object[] {
                    "Apple",
                    "Orange",
                    "Banana"});
                //comboBox.Font = new Font(comboBox.Font.FontFamily, 16F);
                flowLayoutPanel.Controls.Add(comboBoxClearUserControl);
            }
        }
    }
    public class ComboBoxClearUserControl : UserControl
    {
        public ComboBoxClearUserControl()
        {
            _comboBoxClear = new ComboBoxClear();
            _comboBoxClear.SizeChanged += (sender, e) =>
            {
                Height = _comboBoxClear.Height;
            };
            Controls.Add(_comboBoxClear);
        }
        private ComboBoxClear _comboBoxClear;
        public static implicit operator ComboBox(ComboBoxClearUserControl comboBoxClearUserControl) =>
            comboBoxClearUserControl._comboBoxClear;
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            _comboBoxClear.Width = Width;   // Width of UC goes to CB
            Height = _comboBoxClear.Height; // Height of CB goes to UC
        }
        public class ComboBoxClear : ComboBox, IMessageFilter
        {
            public ComboBoxClear()
            {
                Margin = new Padding(); // This size must be exactly equal to UserControl size.
            }
            const int CB_DROP_HANDLE_WIDTH = 26;
            protected override void OnHandleCreated(EventArgs e)
            {
                base.OnHandleCreated(e);
                Application.AddMessageFilter(this);
                if (!(DesignMode || _isHandleInitialized))
                {
                    _isHandleInitialized = true;
                    _lblClear.Size = new Size(Height - 6, Height - 4);
                    _lblClear.Location =
                        new Point(
                            Location.X + Width - _lblClear.Width - CB_DROP_HANDLE_WIDTH,
                            Location.Y + 2
                    );
                    Parent.Controls.Add(_lblClear);
                    _lblClear.Click += (sender, e) =>
                    {
                        Text = String.Empty;
                    };
                    TextChanged += (sender, e) =>
                    {
                        _lblClear.Visible = Text.Any();
                    };
                }
            }
            bool _isHandleInitialized = false;
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    Application.RemoveMessageFilter(this);
                }
            }
            const int WM_PAINT = 0x000f;
            public bool PreFilterMessage(ref Message m)
            {
                switch (m.Msg)
                {
                    case WM_PAINT:
                        _lblClear.BringToFront();
                        _lblClear.Refresh();
                        break;
                }
                return false;
            }
            private Label _lblClear = new Label
            {
                BackColor = SystemColors.Window,
                BorderStyle = BorderStyle.None,
                Text = "✖",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Tahoma", 6),
                Padding = new Padding(),
                Margin = new Padding(),
                Visible = false,
            };
        }
    }
}
