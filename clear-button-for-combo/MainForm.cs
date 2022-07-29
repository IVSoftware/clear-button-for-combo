﻿using System;
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
        }
    }
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
}