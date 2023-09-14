using System;
using System.Xml;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.UnitTests
{
    [TargetType(typeof(VirtualizingStackPanel))]
    public class ScrollOwnerTest : IUnitTest
    {
        public TestResult Perform(object testElement, XmlElement variation)
        {

            VirtualizingStackPanel virtualizingStackPanel = new VirtualizingStackPanel();

            ScrollViewer scv = new ScrollViewer();
            virtualizingStackPanel.ScrollOwner = scv;

            if (virtualizingStackPanel.ScrollOwner == scv)
            {
                TestLog.Current.LogEvidence("Pass: VirtualizingStackPanel.ScrollOwner Get/Set");
                return TestResult.Pass;
            }
            else
            {
                TestLog.Current.LogStatus("VirtualizingStackPanel.ScrollOwner  " + virtualizingStackPanel.ScrollOwner.GetType().ToString());
                TestLog.Current.LogEvidence("Fail: VirtualizingStackPanel.ScrollOwner Get/Set");
                return TestResult.Fail;
            }
        }
    }
}


