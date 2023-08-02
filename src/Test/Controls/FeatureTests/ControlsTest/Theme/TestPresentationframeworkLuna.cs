//
//	
//	   TestPresentationframeworkLuna.cs
//	   Find	all	concrete Controls under	System.Windows.Conrols and instantiate it
//	   This	will exercise code in PresentationFramework.Luna.dll. If no	exception test
//	   pass	
//	
//

using System;
using System.IO;
using System.Xml;
using System.Windows;
using System.Reflection;
using System.Threading;
using System.Collections;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls.Primitives;

using Microsoft.Windows.Themes;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;

namespace Avalon.Test.ComponentModel.UnitTests
{

    /// <summary>
    /// Create all controls under Luna theme.
    /// Use CMLoader to instantiate the controls specified in XTC, so no real work in this class
    /// </summary>
    public class PresentationframeworkLunaTest2 : IUnitTest
    {

        public TestResult Perform(object obj, XmlElement variation)
        {

            TestLog.Current.LogEvidence("Testing Control " + obj.GetType() + " under Luna theme");

            Control control = XMLParser.ParseObjectWithDefaultNS(variation.ChildNodes[0]) as Control;

            if (control == null)
            {
                throw new NullReferenceException("Unable to create control: " + variation.ChildNodes[0].Name);
            }

            Panel panel = (Avalon.Test.ComponentModel.Actions.WindowPositionResizeAction.GetParentWindow(obj as FrameworkElement)).Content as Panel;

            if (panel == null)
            {
                throw new InvalidCastException("Unable to retrieve top Panel");
            }

            panel.Children.Add(control);

            TestLog.Current.LogStatus(control.GetType().Name + " added to the TopPanel");

            QueueHelper.WaitTillQueueItemsProcessed();

            TestLog.Current.LogEvidence("Pass: Testing Control " + obj.GetType() + " under Luna theme");

            return TestResult.Pass;
        }
    }
}
