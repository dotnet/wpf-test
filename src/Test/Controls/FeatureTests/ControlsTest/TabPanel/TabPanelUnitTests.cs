using System;
using System.Windows.Input;
using System.Windows.Controls;
using System.Xml;
using System.Windows.Controls.Primitives;
using System.Windows.Automation.Provider;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;
using System.Threading;
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Utilities;
using System.Windows;

namespace Avalon.Test.ComponentModel.UnitTests
{

    [TargetType(typeof(TabPanel))]
    public class TabPanelPublicAPIsTest : IUnitTest
    {
        public TabPanelPublicAPIsTest()
        {
        }
        /// <summary>
        /// Test Popup Public APIs.
        /// </summary>
        /// <param name="tabpanel"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            tabpanel = obj as TabPanel;
            margin = tabpanel.Margin;
            tabpanel.Margin = new Thickness(88);
            if (88 == tabpanel.Margin.Bottom
                && 88 == tabpanel.Margin.Left
                && 88 == tabpanel.Margin.Right
                && 88 == tabpanel.Margin.Top
                )
            {
                RestoreState();
                return TestResult.Pass;
            }
            else
            {
                RestoreState();
                return TestResult.Fail;
            }
        }
        private void RestoreState()
        {
            tabpanel.Margin = margin;
        }
        TabPanel tabpanel;
        Thickness margin;
    }
}


