using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;

using WpfControlToolkit;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Avalon.Test.ComponentModel.Utilities;


namespace Avalon.Test.ComponentModel.Actions
{
    public static class AnimatedStackPanelUnitTest
    {
        #region Public and Protected Members
        /******************************************************************************
        * Function:          VerifyOrientationAction
        ******************************************************************************/
        /// <summary>
        /// CustomControl test: the AnimatedStackPanel control.
        /// </summary>
        /// <param name="parent">The AnimatedStackPanel parent in Markup</param>
        /// <param name="variation">The Variation to be tested</param>
        /// <returns>A boolean indicating pass or fail</returns>
        public static bool VerifyOrientationAction(ItemsControl parent, Orientation expectedOrientation)
        {
            bool finalResult = false;

            AnimatedStackPanel asp = FindAnimatedStackPanel(parent);

            if (asp == null)
            {
                GlobalLog.LogEvidence("!!!ERROR in VerifyOrientationAction:  The AnimatedStackPanel was not found.");
                finalResult = false;
            }
            else
            {
                finalResult = Verify(asp, expectedOrientation);
            }

            return finalResult;
        }
        #endregion

        #region Private Members
        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verifies the finalResult of the test case, and returns a Pass/Fail finalResult.
        /// </summary>
        /// <param name="animatedStackPanel">The Reveal control to be verified.</param>
        /// <returns></returns>
        private static bool Verify(AnimatedStackPanel animatedStackPanel, Orientation expOrientation)
        {
            Orientation actOrientation = animatedStackPanel.Orientation;

            TestLog.Current.LogStatus("*************************************************");
            TestLog.Current.LogStatus("Orientation Expected: " + expOrientation);
            TestLog.Current.LogStatus("Orientation Actual:   " + actOrientation);
            TestLog.Current.LogStatus("*************************************************");

            if (actOrientation == expOrientation)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        /******************************************************************************
        * Function:          FindAnimatedStackPanel
        ******************************************************************************/
        /// <summary>
        /// Finds the first occurance of an AnimatedStackPanel.
        /// </summary>
        /// <param name="t">The Type of the object that is looked for.</param>
        /// <param name="root">The root element.</param>
        /// <returns>A FrameworkElement</returns>
        private static AnimatedStackPanel FindAnimatedStackPanel(ItemsControl parent)
        {
            DependencyObject dobj = FindElement(typeof(System.Windows.Controls.ItemsPresenter), parent);
            ItemsPresenter ip = (ItemsPresenter)dobj;
            
            ItemsPanelTemplate ipt = (ItemsPanelTemplate)parent.ItemsPanel;

            //The expected Name of the AnimatedStackPanel is specified in the .xtc via the parent's Tag property.
            //It must match the actual name.
            object ob = ipt.FindName((string)parent.Tag, ip);

            if (ob == null)
            {
                return null;
            }
            else
            {
                return (AnimatedStackPanel)ob;
            }
        }

        /******************************************************************************
        * Function:          FindElement
        ******************************************************************************/
        /// <summary>
        /// Finds the first occurance of an AnimatedStackPanel.
        /// </summary>
        /// <param name="t">The Type of the object that is looked for.</param>
        /// <param name="root">The root element.</param>
        /// <returns>A FrameworkElement</returns>
        private static FrameworkElement FindElement(Type t, DependencyObject root)
        {
            if (root == null) return null;
            FrameworkElement fe = root as FrameworkElement;
            
            if (fe != null && fe.GetType() == t)
            {
                return fe;
            }

            int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);
            Console.WriteLine("Type: " + fe.GetType());

            for(int i = 0; i < count; i++)
            {
                DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(root,i);
                Console.WriteLine("child: " + child.GetType());

                DependencyObject feRet = FindElement(t, child);
                if (feRet != null)
                {
                    return (FrameworkElement)feRet;
                }
            }
            return null;
        }
        #endregion
    }
}
