using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Controls;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Sub-class of StyleSelector for control property test.
    /// </summary>
    public class SubStyleSelector : StyleSelector
    {
        private bool isTest;
        public bool IsTest
        {
            set { isTest = value; }
            get { return isTest; }
        }
    }
}
