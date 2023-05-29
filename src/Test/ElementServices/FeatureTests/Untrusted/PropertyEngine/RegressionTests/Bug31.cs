// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;  //For DataBind
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;

using Avalon.Test.CoreUI;
//using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.PropertyEngine.RegressionTests
{
    [Test(0, "PropertyEngine.RegressionTests", TestCaseSecurityLevel.FullTrust, "Bug31", Versions="4.7+")]
    public class Bug31 : TestCase
    {
        #region Constructor
        /// <summary>
        /// Dynamically created FlowDocument elements does not reflect dynamic style setters
        /// </summary>
        public Bug31()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Repro the 

        TestResult StartTest()
        {
            Avalon.Test.CoreUI.UtilityHelper.Utilities.StartRunAllTests("Bug31");
            Avalon.Test.CoreUI.UtilityHelper.Utilities.PrintTitle("Repro/Validate Bug 31");

            // The 











            Grid mainGrid = new Grid();
            mainGrid.SetValue(Control.ForegroundProperty, Brushes.Green);   // for (5)
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            mainGrid.Children.Add(fdsv);

            // create the Styles for the resource references to resolve to
            Style controlStyle = new Style(typeof(Control));
            controlStyle.Setters.Add(new Setter(Control.ForegroundProperty, Brushes.Red));  // (4)
            mainGrid.Resources.Add("MyControlStyle", controlStyle);
            Style sectionStyle = new Style(typeof(Section));
            sectionStyle.Setters.Add(new Setter(Control.ForegroundProperty, Brushes.Red));  // (4)
            mainGrid.Resources.Add("MySectionStyle", sectionStyle);

            // create the elements (with resource references) and their parents
            Control control = new Control();
            control.SetResourceReference(Control.StyleProperty, "MyControlStyle");  // (2)
            Panel controlParent = new DockPanel();
            controlParent.Children.Add(control);
            Section section = new Section(new Paragraph(new Run("abcd")));
            section.SetResourceReference(Section.StyleProperty, "MySectionStyle");  // (2)
            FlowDocument sectionParent = new FlowDocument(section);

            // add the parents to the tree
            mainGrid.Children.Add(controlParent);   // (3)
            fdsv.Document = sectionParent;          // (3)

            // verify that the inheritable property (Foreground) has the correct value
            Utilities.Assert(control.Foreground == Brushes.Red, "Control.Foreground should be set from dynamic style");
            Utilities.Assert(section.Foreground == Brushes.Red, "Section.Foreground should be set from dynamic style");

            // if the asserts don't fail, the test passes
            return TestResult.Pass;
        }
        #endregion
    }
}


