# clear-button-for-combo
The objective is to float an [x] over a ComboBox to clear it when text is present.

This works reliably (for example, in a challenging environment like a `FlowLayoutControl`) when `ComboBoxClear` and the `_lblClear` share a deterministic parent `Control` (it doesn't need to be a `UserControl` specifically).

There is a low-level hook of the `WM_PAINT` message that I left in the code even though it proved to be unnecessary in the end. A `ComboBox` is not going to fire a `Paint` message or hit an `OnPaint` override if one were to be added, but if you really needed to do something like that this shows how.


![screenshot](https://github.com/IVSoftware/clear-button-for-combo/blob/master/clear-button-for-combo/ReadMe/flow-layout-panel.png)

***

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

***
**TESTBENCH**

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
