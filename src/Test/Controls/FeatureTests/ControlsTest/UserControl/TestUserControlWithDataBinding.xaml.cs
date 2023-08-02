using System;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel
{
    public partial class TestUserControlWithDataBinding
    {

        public TestUserControlWithDataBinding()
        {
            InitializeComponent();
        }

        public string Text
        {
            get
            {
                return TESTTESTBLOCK.Text;
            }
        }
    }
}
