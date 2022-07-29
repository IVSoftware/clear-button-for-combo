using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
                var comboBoxClearUserControl = new ComboBoxClearControl
                {
                    Width = 250,
                };
                ComboBox comboBox = comboBoxClearUserControl;
                comboBox.FormattingEnabled = true;
                comboBox.Items.AddRange(new object[] {
                    "Apple",
                    "Orange",
                    "Banana"});
                comboBox.Font = new Font(comboBox.Font.FontFamily, 16F);
                flowLayoutPanel.Controls.Add(comboBoxClearUserControl);
            }
        }
    }
    public class ComboBoxClearControl : Control
    {
        public ComboBoxClearControl()
        {
            _comboBoxClear = new ComboBoxClear();
            _comboBoxClear.SizeChanged += (sender, e) => Height = _comboBoxClear.Height;
            Controls.Add(_comboBoxClear);
        }
        private ComboBoxClear _comboBoxClear;
        public static implicit operator ComboBox(ComboBoxClearControl comboBoxClearUserControl) =>
            comboBoxClearUserControl._comboBoxClear;
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            _comboBoxClear.Width = Width;   // Width of C goes to CB
            Height = _comboBoxClear.Height; // Height of CB goes to C
        }
        public class ComboBoxClear : ComboBox, IMessageFilter
        {
            public ComboBoxClear() => Margin = new Padding(); 

            const int CB_DROP_HANDLE_WIDTH = 26;
            bool _isHandleInitialized = false;
            protected override void OnHandleCreated(EventArgs e)
            {
                base.OnHandleCreated(e);
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
                    _lblClear.BringToFront();
                    _lblClear.Click += (sender, e) => Text = String.Empty;
                    TextChanged += (sender, e) => _lblClear.Visible = Text.Any();
                    Application.AddMessageFilter(this);
                }
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
            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == WM_PAINT)
                { } // Paint something if needed
                return false;
            }
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    Application.RemoveMessageFilter(this);
                }
            }
            const int WM_PAINT = 0x000f;
        }
    }
}
