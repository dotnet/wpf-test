using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Sub-class of Sytle for control property test.
    /// </summary>
    public class SubStyle : Style
    {
        private bool isTest;
        public bool IsTest
        {
            set { isTest = value; }
            get { return isTest; }
        }
    }
}
