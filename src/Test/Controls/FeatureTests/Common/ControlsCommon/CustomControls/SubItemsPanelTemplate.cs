using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Controls;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Sub-class of ItemsPanelTemplate for control property test.
    /// </summary>
    public class SubItemsPanelTemplate : ItemsPanelTemplate
    {
        private bool isTest;
        public bool IsTest
        {
            set { isTest = value; }
            get { return isTest; }
        }
    }
}
