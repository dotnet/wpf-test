using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Controls;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Sub-class of ViewBase for control property test.
    /// </summary>
    public class SubViewBase : ViewBase
    {
        private bool isTest;
        public bool IsTest
        {
            set { isTest = value; }
            get { return isTest; }
        }
    }
}
