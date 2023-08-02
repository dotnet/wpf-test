// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable 618
using Microsoft.Test.Logging;
using Microsoft.Test.Hosting;
using Microsoft.Test.UIAutomaion;
using System;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using UIAutomation;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Shim to allow Flow/Fixed TextRange/TextRangePattern UIAutomation testcases
    /// to run within the UIAutomation framework.
    /// </summary>
    [Serializable]
    public class UiaShim : UiaSimpleTestcase
    {
        string _xeshim;

        public string XmlElementShim
        {
            get { return _xeshim; }
            set { _xeshim = value; }
        }

        public override void Init(object target)
        {
            FlowDocument fd = target as FlowDocument;
            if (fd != null)
            {
                // FlowDocument Page width/height default to NaN, which allow dynamic
                // repagination based upon viewer size. Since we want our cases to be
                // interchangable with D2, if the FlowDocument did not explicitly set the page
                // width and height, we need to match XPS serialization's default size of A4.
                if (double.IsNaN(fd.PageWidth))
                {
                    fd.PageWidth = 8.5 * 96;
                }
                if (double.IsNaN(fd.PageHeight))
                {
                    fd.PageHeight = 11 * 96;
                }

                XmlDocument xd = new XmlDocument();
                xd.LoadXml(_xeshim);
                XmlElement xe = xd.DocumentElement;
                if (xe["VIEWERLAYOUT"] != null)
                {
                    if (xe["VIEWERLAYOUT"].InnerText == "TwoPages")
                    {
                        if (fd.Parent.GetType().ToString() == "MS.Internal.AppModel.RootBrowserWindow")
                        {
                            NavigationWindow nw = fd.Parent as NavigationWindow;
                            FlowDocumentReader fdr = (FlowDocumentReader)(VisualTreeUtils.FindPartByType(nw, typeof(FlowDocumentReader)))[0];
                            fdr.ViewingMode = System.Windows.Controls.FlowDocumentReaderViewingMode.TwoPage;
                        }
                    }
                }
            }
            else
            {
                TestLog.Current.LogEvidence("Target was not a FlowDocument, unable to fix page size.");
            }
        }

        public override void DoTest(AutomationElement target)
        {


            // We used the XmlElementShim property to store the VARIATION node
            // as a string. We now need to convert it back to the XmlElement
            // form that IUIAutomationTest expects.
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(_xeshim);
            XmlElement xe = xd.DocumentElement;

            Assembly asm = Assembly.LoadWithPartialName("UIAutoFTUtils");
            if (asm != null)
            {
                Type t = asm.GetType("UIAutomation." + xe.GetAttribute("RealClass"), false);
                if (t != null)
                {
                    if (t.GetInterface("UIAutomation.IUIAutomationTest") != null)
                    {
                        IUIAutomationTest testToRun = (IUIAutomationTest)Activator.CreateInstance(t);
                        if (testToRun.Perform(target, xe))
                        {
                            TestLog.Current.Result = TestResult.Pass;
                        }
                        else
                        {
                            TestLog.Current.Result = TestResult.Fail;
                        }
                    }
                }
            }
        }

        public override void Validate(object target)
        {
        }

    }
    static class VisualTreeUtils
    {
        /// <summary>
        /// Used to get an item of a specific type in the visual tree.
        /// So to find the 2nd item of type Button it would be something like.
        /// FindPartByType(visual,typeof(Button),1);
        /// </summary>
        /// <param name="vis">The visual who's tree you want to search.</param>
        /// <param name="visType">The type of object you want to find.</param>
        /// <param name="index">The count of the item as it is found in the tree.</param>
        /// <returns>The object of type visType found in vis that is the index item</returns>
        public static object FindPartByType(System.Windows.Media.Visual vis, System.Type visType, int index)
        {
            if (vis != null)
            {
                System.Collections.ArrayList parts = FindPartByType(vis, visType);

                if (index >= 0 && index < parts.Count)
                {
                    return parts[index];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns an ArrayList of all of the items of the specified type in the visual tree.
        /// </summary>
        /// <param name="vis">The visual who's tree you want to search.</param>
        /// <param name="visType">The type of object you want to find.</param>
        /// <returns>An ArrayList containing all of the objects of type visType found in the tree of vis.</returns>
        public static System.Collections.ArrayList FindPartByType(System.Windows.Media.Visual vis, System.Type visType)
        {
            System.Collections.ArrayList parts = new System.Collections.ArrayList();

            if (vis != null)
            {
                parts = FindPartByTypeRecurs(vis, visType, parts);
            }

            return parts;
        }

        private static System.Collections.ArrayList FindPartByTypeRecurs(DependencyObject vis, System.Type visType, System.Collections.ArrayList parts)
        {
            if (vis != null)
            {
                if (vis.GetType() == visType)
                {
                    parts.Add(vis);
                }

                int count = VisualTreeHelper.GetChildrenCount(vis);

                for(int i = 0; i < count; i++)
                {
                    DependencyObject curVis = VisualTreeHelper.GetChild(vis,i);
                    parts = FindPartByTypeRecurs(curVis, visType, parts);
                }
            }

            return parts;
        }

        /// <summary>
        /// tranverse the visual tree to get a visua by Name.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ele"></param>
        /// <returns></returns>
        public static DependencyObject FindPartByName(DependencyObject ele, string name)
        {
            DependencyObject result;
            if (ele == null)
            {
                return null;
            }
            if (name.Equals(ele.GetValue(FrameworkElement.NameProperty)))
            {
                return ele;
            }
            int count = VisualTreeHelper.GetChildrenCount(ele);
            for(int i = 0; i < count; i++)
            {
                DependencyObject vis = VisualTreeHelper.GetChild(ele,i);
                if ((result = FindPartByName(vis, name)) != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}
