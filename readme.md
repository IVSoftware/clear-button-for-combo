# clear-button-for-combo
Float an [x] over a ComboBox to clear it when text is present.

 Suppose that ComboBoxClear implements IMessageFilter to install a low-level hook for WM_PAINT that can be used to refresh _lblClear and keep it on top. Then, instead of adding _lblClear to the Controls collection of the ComboBox (where it's never going to get on top of the editor control) put it in Parent.Controls instead.

This is working for me. "Your mileage may vary" like they used to say.

![screenshot](https://github.com/IVSoftware/clear-button-for-combo/blob/master/clear-button-for-combo/ReadMe/screenshot.png)

***

    public class ComboBoxClear : ComboBox, IMessageFilter
    {
        public ComboBoxClear()
        {
            Application.AddMessageFilter(this);
        }
        const int CB_DROP_HANDLE_WIDTH = 20;
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if(!(DesignMode || _isHandleInitialized))
            {
                _isHandleInitialized = true;
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
            if(disposing)
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
            Size = new Size(28, 28),
            Text = "✖",
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Tahoma", 6),
            Padding = new Padding(),
            Margin = new Padding(),
            Visible = false,
        };
    }