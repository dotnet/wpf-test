// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace WFCTestLib.Util
{
    // <doc>
    // <desc>
    //  Dialog with listbox for viewing debug strings.
    // </desc>
    // </doc>
    public class DebugWindow : Form
    {
        // <doc>
        // <desc>
        //  The delegate toe be invoked when the DebugWindow is disposed.
        // </desc>
        // </doc>
        internal MethodInvoker notifyWhenDisposed;

        // <doc>
        // <desc>
        //  Constructs a DebugWindow object
        // <desc>
        // </doc>
        public DebugWindow() : this(null)
        {
        }

        // <doc>
        // <desc>
        //  Constructs a DebugWindow object. The delegate provided is
        //  invoked when the DebugWindow is disposed.
        // </desc>
        // <param term="notifyWhenDisposed">
        //  The delegate to be invoked when the DebugWindow is disposed.
        // </param>
        // </doc>
        public DebugWindow(MethodInvoker notifyWhenDisposed) : base()
        {
            this.notifyWhenDisposed = notifyWhenDisposed;
            InitForm();
        }

        // <doc>
        // <desc>
        //  Add a string to the list of items
        // </desc>
        // <param term="s">
        //  The string to add to the listbox.
        // </param>
        // </doc>
        public virtual void AddItem(String s)
        {
            listbox.Items.Add(s);
            listbox.SelectedIndex = (listbox.Items.Count - 1);
        }

        // <doc>
        // <desc>
        //  Disposes the components of this form. Invokes the
        //  notifyWhenDisposed delegate.
        // </desc>
        // </doc>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (notifyWhenDisposed != null)
                notifyWhenDisposed();
            components.Dispose();
        }

        // <doc>
        // <desc>
        //  The event handler called when the Clear button is clicked
        //  in the DebugWindow. The contents of the listbox are cleared.
        // </desc>
        // <param term="sender">
        //  The object that initiated this event
        // </param>
        // <param term="e">
        //  The Event
        // </param>
        // </doc>
        private void BClear_click(Object sender, EventArgs e)
        {
            listbox.Items.Clear();
        }

        /**
         * NOTE: The following code is required by the Visual J++ form
         * designer.  It can be modified using the form editor.  Do not
         * modify it using the code editor.
         */
        internal Container components = new Container();
        internal ListBox listbox = new ListBox();
        internal Button bClear = new Button();

        private void InitForm()
        {
            this.Text = ("DebugWindow");
//            this.AutoScaleBaseSize = new Size(5, 13);
            this.ClientSize = new Size(308, 327);

            listbox.Dock = DockStyle.Fill;
            listbox.Size = new Size(308, 303);
            listbox.TabIndex = (0);
            listbox.Text = ("listBox1");
            listbox.UseTabStops = (true);

            bClear.Dock = DockStyle.Bottom;
            bClear.Location = new Point(0, 304);
            bClear.Size = new Size(308, 23);
            bClear.TabIndex = (1);
            bClear.Text = ("clear listbox");
            bClear.Click += new EventHandler(this.BClear_click);

            this.Controls.AddRange(new Control[] {
                                    bClear,
                                    listbox});
        }
    }
}
