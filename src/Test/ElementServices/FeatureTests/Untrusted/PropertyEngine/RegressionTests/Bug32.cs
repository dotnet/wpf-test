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
    [Test(0, "PropertyEngine.RegressionTests", TestCaseSecurityLevel.FullTrust, "Bug32", Versions="4.7+")]
    public class Bug32 : TestCase
    {
        #region Constructor
        /// <summary>
        ///  32: Inherited properties do not propagate
        /// </summary>
        public Bug32()
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
            Avalon.Test.CoreUI.UtilityHelper.Utilities.StartRunAllTests("Bug32");
            Avalon.Test.CoreUI.UtilityHelper.Utilities.PrintTitle("Repro/Validate Bug 32");

            // Definitions (for terms used in the discussion):
            // o Element E is a "self-inheritance parent" if E has an inheritable
            //      property that has been set locally or by a style/template
            // o The "inheritance parent" of element E is the closest ancestor of E
            //      (possibly E itself) that is a self-inheritance parent.
            // o The "proper descendants" of element E are the descendants of E,
            //      except for E itself.
            // o The "proper ancestors" of E are the ancestors of E except for E itself.
            //
            // The bug arises when attaching a subtree with root R to a parent P, when
            //  1. There are nodes E1, E2, E3, E4 where E1 is a descendant of R, E2 is a
            //      proper descendant of E1, E3 and E4 are proper descendants of E2, but
            //      neither E3 and E4 are descendants of the other.
            //  2. E1 is a self-inheritance parent.
            //  3. E1 is the inheritance parent of E2 and E4.
            //  4. E3 is a self-inheritance parent.
            //  5. E2 declares a style by dynamic resource reference or implicit lookup.
            //  6. The style has not yet resolved, but does resolve upon attaching R to P.
            //  7. The new style sets one or more inheritable properties.
            //  8. P has inheritable properties with non-default values
            //
            //              P       property values P1=v1, P2=v2;  resources include style S that sets P1=v1'
            //              .            new link
            //             R=E1     (ip=self)
            //              |
            //              E2      Style={DynamicResource S}
            //             /  \
            //            E3  E4    ip(E3)=self  ip(E4)=E1
            //
            // In this situation the correct behavior is for P's property values to
            // propagate through the subtree under R (including E3 and E4), except for
            // values set by the new style;  the style's values should propagate starting
            // at E2 (including E3 and E4).
            //
            //  31 complains that E3 and E4 get P's value instead of the style's value,
            // for properties common to both.   32 complains that E3 gets
            // the default value for a property offered by P but not by the style
            // (although E4 gets P's value correctly).
            //
            // Referring to the picture, E3 and E4 should both have P1=v1', P2=v2.
            // 31:  E3 and E4 have P1=v1  (but P2=v2, correctly)
            // 32:  E3 has P2=default  (but E4 has P2=v2, and both have P1=v1')




            string hello = "hello";     // value for property offered by P but not by the style

            Grid mainGrid = new Grid(); // P
            mainGrid.SetValue(Control.ForegroundProperty, Brushes.Green);   // common property - (8)
            mainGrid.SetValue(FrameworkElement.DataContextProperty, hello); // offered by P, not by style - (8)

            // create the Style for the resource reference to resolve to
            Style gridStyle = new Style(typeof(Grid));
            gridStyle.Setters.Add(new Setter(Control.ForegroundProperty, Brushes.Red));  // common property - (7)
            mainGrid.Resources.Add("MyGridStyle", gridStyle);

            // create the subtree top-down
            Grid root = new Grid();     // R = E1
            root.SetValue(Control.FontSizeProperty, 12.0);  // makes E1 a self-inheritance parent - (2)

            Grid grid = new Grid();     // E2
            grid.SetResourceReference(Control.StyleProperty, "MyGridStyle");  // (5)
            root.Children.Add(grid);

            StackPanel panel = new StackPanel();    // container
            grid.Children.Add(panel);

            TextBlock tb1 = new TextBlock();        // E4
            panel.Children.Add(tb1);

            TextBlock tb2 = new TextBlock();        // E3
            tb2.FontWeight = FontWeights.SemiBold;  // makes E3 a self-inheritance parent - (4)
            panel.Children.Add(tb2);

            // attach the subtree
            mainGrid.Children.Add(root);

            // verify that the inheritable properties have the correct value
            Utilities.Assert(tb1.Foreground == Brushes.Red, "tb1.Foreground should be set from dynamic style");
            Utilities.Assert(tb2.Foreground == Brushes.Red, "tb2.Foreground should be set from dynamic style");
            Utilities.Assert(Object.Equals(tb1.DataContext, hello), "tb1.DataContext should propagate from parent tree");
            Utilities.Assert(Object.Equals(tb2.DataContext, hello), "tb2.DataContext should propagate from parent tree");

            // if the asserts don't fail, the test passes
            return TestResult.Pass;
        }
        #endregion
    }
}



