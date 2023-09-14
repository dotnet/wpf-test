// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.IO;
using System.Windows.Input;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Automation.Peers;
using System.Windows.Interop;
using Microsoft.Test.Logging;
using System.Reflection;


namespace Microsoft.Test.Layout
{
    
        public class TestHelperW
        {
            #region Helper Functions
            //Find the visual which has Find command.
            private static bool HasSwitchCommand(DependencyObject vis)
            {
                ICommandSource source = vis as ICommandSource;
                if (source == null)
                {
                    return false;
                }

                return source.Command == FlowDocumentReader.SwitchViewingModeCommand;

            }

            private static bool HasFindCommand(DependencyObject vis)
            {
                ICommandSource source = vis as ICommandSource;
                if (source == null)
                {
                    return false;
                }
                return source.Command == ApplicationCommands.Find;

            }
            //Find the visual which has FindToolBar.
            private static bool HasFindToolBar(DependencyObject vis)
            {
                object o = vis as Object;
                if (o == null)
                {
                    return false;
                }
                return o.GetType().Name == "FindToolBar";
            }

            private static bool HasTextBox(DependencyObject vis)
            {
                object o = vis as Object;
                if (o == null)
                {
                    return false;
                }
                return o is TextBox;
            }

            private static bool HasFindNextButton(DependencyObject vis)
            {
                object o = vis as Object;
                if (o == null)
                {
                    return false;
                }
                if (o is Button)
                {
                    Button obj = o as Button;
                    if (obj.Name == "FindNextButton")
                    {
                        return true;
                    }
                }
                return false;
            }

            private static bool HasFindPreviousButton(DependencyObject vis)
            {
                object o = vis as Object;
                if (o == null)
                {
                    return false;
                }
                if (o is Button)
                {
                    Button obj = o as Button;
                    if (obj.Name == "FindPreviousButton")
                    {
                        return true;
                    }
                }
                return false;
            }

            // find the first visual child which can pass the predicate
            private static DependencyObject FindChildFromVisualTree(DependencyObject vis, Predicate<DependencyObject> pred)
            {
                if (pred(vis))
                {
                    return vis;
                }

                int count = VisualTreeHelper.GetChildrenCount(vis);
                for (int i = 0; i < count; i++)
                {
                    DependencyObject v = VisualTreeHelper.GetChild(vis, i);
                    DependencyObject tmp = FindChildFromVisualTree(v, pred);
                    if (tmp != null)
                    {
                        return tmp;
                    }
                }
                return null;

            }
            // find the visual that has the find command.
            public static DependencyObject FindtheFindButtonVisual(DependencyObject vis)
            {
                return FindChildFromVisualTree(vis, HasFindCommand);
            }

            // find the visual that has the findtoolbar name.
            public static DependencyObject FindtheFindToolBar(DependencyObject vis)
            {
                return FindChildFromVisualTree(vis, HasFindToolBar);
            }

            public static DependencyObject FindtheTextBox(DependencyObject vis)
            {
                return FindChildFromVisualTree(vis, HasTextBox);
            }

            public static DependencyObject FindtheFindNextButton(DependencyObject vis)
            {
                return FindChildFromVisualTree(vis, HasFindNextButton);
            }

            public static DependencyObject FindtheFindPreviousButton(DependencyObject vis)
            {
                return FindChildFromVisualTree(vis, HasFindPreviousButton);
            }

            public static DependencyObject FindSwitchViewingModeVisual(DependencyObject vis)
            {
                return FindChildFromVisualTree(vis, HasSwitchCommand);
            }
            public static TextRange tr;

            public static int GetSubStringCount(string x, FlowDocument fd)
            {

                TextContainerW tw = TextContainerW.FromTextPointer(fd.ContentStart);
                tr = new TextRange(tw.StartPosition, tw.EndPosition);
                string subString = x;
                int count = 0;
                string str = tr.Text.ToLower();
                int startLocation = str.IndexOf(subString);
                int endLocation = str.LastIndexOf(subString);
                while (startLocation != -1)
                {
                    count++;
                    startLocation += 1;
                    startLocation = str.IndexOf(subString, startLocation);

                }
                return count;
            }
            #endregion

            // make sure that reader has the right mode of viewer inside
            public static bool HasView(FlowDocumentReader reader, FlowDocumentReaderViewingMode mode)
            {
                ViewerInfo info = FindView(reader);
                if (info == null)
                {
                    GlobalLog.LogStatus("Viewer not exist");
                    return false;
                }
                return info.Mode == mode;
            }

            // Find the viewer inside reader
            private static ViewerInfo FindView(FlowDocumentReader reader)
            {
                switch (reader.ViewingMode)
                {
                    case FlowDocumentReaderViewingMode.TwoPage:
                    case FlowDocumentReaderViewingMode.Page:
                        {
                            FlowDocumentPageViewer v = (FlowDocumentPageViewer)FindChildFromVisualTree(reader, delegate(DependencyObject visual)
                                { return visual is FlowDocumentPageViewer; });
                            if (v == null)
                                return null;
                            return new FlowDocumentPageViewerInfo(v);
                        }
                    case FlowDocumentReaderViewingMode.Scroll:
                        {
                            FlowDocumentScrollViewer v = (FlowDocumentScrollViewer)FindChildFromVisualTree(reader, delegate(DependencyObject visual)
                            { return visual is FlowDocumentScrollViewer; });
                            if (v == null)
                                return null;
                            return new FlowDocumentScrollViewerInfo(v);
                        }
                    default:
                        throw new ApplicationException("Unexpected ViewingMode: " + reader.ViewingMode);
                }
            }

            #region ViewerInfo

            // view info intereface for all views inside reader
            interface ViewerInfo
            {
                double Zoom { get; }

                double MinZoom { get; }

                double MaxZoom { get; }

                FlowDocumentReaderViewingMode Mode { get; }
            }

            // viewer info for FlowDocumentPageViewer
            class FlowDocumentPageViewerInfo : ViewerInfo
            {
                private FlowDocumentPageViewer _viewer;

                public FlowDocumentPageViewerInfo(FlowDocumentPageViewer viewer)
                {
                    _viewer = viewer;
                }

                public double Zoom
                {
                    get { return _viewer.Zoom; }
                }

                public double MinZoom
                {
                    get { return _viewer.MinZoom; }
                }

                public double MaxZoom
                {
                    get { return _viewer.MaxZoom; }
                }

                public FlowDocumentReaderViewingMode Mode
                {
                    get
                    {
                        int count = LayoutUtility.GetChildCountFromVisualTree(_viewer, typeof(DocumentPageView));
                        if (count == 1)
                            return FlowDocumentReaderViewingMode.Page;
                        if (count == 2)
                            return FlowDocumentReaderViewingMode.TwoPage;
                        throw new ApplicationException("FlowDocumentPageViewer has illegal number of pages: " + count);
                    }
                }
            }

            // viewer info for FlowDocumentScrollViewer
            class FlowDocumentScrollViewerInfo : ViewerInfo
            {
                private FlowDocumentScrollViewer _viewer;

                public FlowDocumentScrollViewerInfo(FlowDocumentScrollViewer viewer)
                {
                    _viewer = viewer;
                }

                public double Zoom
                {
                    get { return _viewer.Zoom; }
                }

                public double MinZoom
                {
                    get { return _viewer.MinZoom; }
                }

                public double MaxZoom
                {
                    get { return _viewer.MaxZoom; }
                }

                public FlowDocumentReaderViewingMode Mode
                {
                    get { return FlowDocumentReaderViewingMode.Scroll; }
                }
            }

            #endregion
        }
        
    
}
