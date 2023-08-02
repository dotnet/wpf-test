using System;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel
{
    public partial class TestUserControl
    {

        public TestUserControl()
        {
            InitializeComponent();
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            TestLog.Current.LogStatus("Click event fired");
            IsClickEventFired = true;
        }

        public bool IsClickEventFired
        {
            get
            {
                return isClickEventFired;
            }
            set
            {
                isClickEventFired = true;
            }
        }

        public new Thickness Margin
        {
            get
            {
                return TESTBUTTON.Margin;
            }
        }

        private bool isClickEventFired = false;
    }
}
