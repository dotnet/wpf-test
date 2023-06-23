using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Avalon.Test.ComponentModel
{
    public class TestCoercePanel : StackPanel
    {
        public int TestCoerce
        {
            get { return (int)GetValue(TestCoerceButton.TestCoerceProperty); }
            set { SetValue(TestCoerceButton.TestCoerceProperty, value); }
        }
    }
}
