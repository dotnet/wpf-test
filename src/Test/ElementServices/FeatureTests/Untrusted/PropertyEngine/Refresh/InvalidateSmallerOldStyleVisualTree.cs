// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshInvalidatePreviousSmallerOldStyleVisualTreeTest
{
    /******************************************************************************
    * CLASS:          InvalidatePreviousSmallerOldStyleVisualTree
    ******************************************************************************/
    [Test(0, "PropertyEngine.Style", TestCaseSecurityLevel.FullTrust, "InvalidatePreviousSmallerOldStyleVisualTree")]
    public class InvalidatePreviousSmallerOldStyleVisualTree : AvalonTest
    {
        #region Constructor
        /******************************************************************************
        * Function:          InvalidatePreviousSmallerOldStyleVisualTree Constructor
        ******************************************************************************/
        public InvalidatePreviousSmallerOldStyleVisualTree()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            CV01.Run();

            //Any test failures will be caught by Asserts or Exceptions.
            return TestResult.Pass;
        }
        #endregion


        #region Public Static Members
        /******************************************************************************
        * Function:          ValidateVisualTreeChange
        ******************************************************************************/
        /// <summary>
        /// We must make sure that the visual trees are actually being changed, 
        /// else we are not verify the fix was actually made.
        /// </summary>
        /// <remarks>
        /// All tests may be verified with this method.
        /// </remarks>
        /// <param name="btn">Element whose visual tree should have changed.</param>
        public static void ValidateVisualTreeChange(Button btn)
        {
            // Either a visual tree with text Name="validationContent" was assigned or
            // the button's content is changed.
            Utilities.PrintStatus("1. Verify style changed.");
            FrameworkElement fe = FindElementById(btn, "validationContent");

            TextBlock txt = fe as TextBlock;
            string content = null;
            if (txt != null)
            {
                content = txt.Text;
            }
            else
            {
                content = btn.Content as string;
            }

            Utilities.Assert(content == "Changed", "(validate text)");
        }

        /******************************************************************************
        * Function:          RoundtripCV
        ******************************************************************************/
        /// <summary>
        /// Make sure that the parsing/serialization performs correctly.
        /// </summary>
        /// <param name="root"></param>
        public static void RoundtripCV(UIElement root)
        {
            Utilities.PrintStatus("Verify application structure.");

            Utilities.PrintStatus("1. Root element is a StackPanel.");
            StackPanel sp = root as StackPanel;
            Utilities.Assert(sp != null, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("2. 1 child elements present.");
            Utilities.Assert(sp.Children.Count == 1, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("3. 1 Button.");
            Button btn = sp.Children[0] as Button;
            Utilities.Assert(btn != null, "Children[0] (button)");
            Utilities.PrintStatus("");

            ValidateVisualTreeChange(btn);
        }

        /******************************************************************************
        * Function:          ValidateVisualTreeChange
        ******************************************************************************/
        /// <summary>
        /// Finds the first element in the given scope with the specified id
        /// in the visual tree.
        /// </summary>
        /// <param name='scope'>Scope from which to begin search.</param>
        /// <param name='id'>Name of framework element.</param>
        /// <returns>The element found, null otherwise.</returns>
        /// <remarks>
        /// Note that this is not what the end-user would expect to use.
        /// However, it is a valid search operation, and it is required
        /// to work around a Window 




        public static FrameworkElement FindElementById(DependencyObject scope, string id)
        {
            if (scope == null)
            {
                throw new ArgumentNullException("scope");
            }

            FrameworkElement result = scope as FrameworkElement;
            if (result != null && result.Name == id)
                return result;

            int count = VisualTreeHelper.GetChildrenCount(scope);
            for(int i = 0; i < count; i++)
            {
                // Common base class for Visual and Visual3D is DependencyObject
                DependencyObject visual = VisualTreeHelper.GetChild(scope, i);

                result = FindElementById(visual, id);
                if (result != null)
                    return result;
            }

            // ContainerVisual's may have further Visual elements to examine
            ContainerVisual cv = scope as ContainerVisual;
            if (cv != null)
                {
                foreach (Visual visual in cv.Children)
                {
                    result = FindElementById(visual, id);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          CV01
    ******************************************************************************/
    /// Verify that no exception is thrown when a new style has a smaller visual tree
    /// than the previous one.
    /// </summary>
    public class CV01
    {
        #region Static Methods
        /******************************************************************************
        * Function:          Run
        ******************************************************************************/
        public static void Run()
        {
            Utilities.PrintTitle("Default style to named style, shrinking visual tree, on load.");
            new PEApplicationCV01().Run();
        }
        #endregion


        /******************************************************************************
        * CLASS:          PEApplicationCV01
        ******************************************************************************/
        public class PEApplicationCV01 : PEApplication
        {
            #region Private Data
            private Button _btn;
            #endregion


            #region Protected Members
            /******************************************************************************
            * Function:          TestSetupUI
            ******************************************************************************/
            protected override void TestSetupUI(StackPanel parentPanel, Window rootWindow)
            {
               Style btnStyle = new Style(typeof(Button));
               FrameworkElementFactory btnFef = new FrameworkElementFactory(typeof(StackPanel));
               FrameworkElementFactory txtFef1 = new FrameworkElementFactory(typeof(TextBlock));
               txtFef1.SetValue(TextBlock.TextProperty, "Same");
               txtFef1.SetValue(TextBlock.NameProperty, "validationContent");
               btnFef.AppendChild(txtFef1);
               FrameworkElementFactory txtFef2 = new FrameworkElementFactory(typeof(TextBlock));
               txtFef2.SetValue(TextBlock.TextProperty, "Same");
               btnFef.AppendChild(txtFef2);
               ControlTemplate template1 = new ControlTemplate(typeof(Button));
               template1.VisualTree = btnFef;
               btnStyle.Setters.Add(new Setter(Button.TemplateProperty, template1));

               parentPanel.Resources[typeof(Button)] = btnStyle;

               Style btnStyleNamed = new Style(typeof(Button));
               btnFef = new FrameworkElementFactory(typeof(StackPanel));
               txtFef1 = new FrameworkElementFactory(typeof(TextBlock));
               txtFef1.SetValue(TextBlock.TextProperty, "Changed");
               txtFef1.SetValue(TextBlock.NameProperty, "validationContent");
               btnFef.AppendChild(txtFef1);
               ControlTemplate template = new ControlTemplate(typeof(Button));
               template.VisualTree = btnFef;
               btnStyleNamed.Setters.Add(new Setter(Button.TemplateProperty, template));

               parentPanel.Resources["btnStyle"] = btnStyleNamed;

               _btn = new Button();
               parentPanel.Children.Add(_btn);
               _btn.Style = (Style)_btn.FindResource("btnStyle");
            }

            /******************************************************************************
            * Function:          TestCompleted
            ******************************************************************************/
            protected override bool TestCompleted()
            {
               InvalidatePreviousSmallerOldStyleVisualTree.ValidateVisualTreeChange(_btn);
               return true;
            }
            #endregion
        }
    }
}


