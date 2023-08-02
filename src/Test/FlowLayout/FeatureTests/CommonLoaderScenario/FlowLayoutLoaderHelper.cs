// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.Layout.PropertyDump;
using MTI = Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.RenderingVerification;

namespace Microsoft.Test.Layout
{    
    public class FlowLayoutLoaderHelper: AvalonTest
    {
        public bool finalResult = false;
        
        private Window _testWin;
        private string _xamlContent;
        private string _container = "sv";
        private string _verification = "vscan";
        private int _pgNum = 0;
        private double _desiredWidth;
        private double _desiredHeight;
        private bool _columnWidthFlexible = true;
        private double _columnWidth = -1;
        private Thickness _pagePadding = new Thickness(-1);
        private double _columnGap = -1;
        private double _columnRuleWidth = -1;
        private bool _background = false;
        private bool _optimal = false;
        private bool _hyphenation = false;
        private string _masterDimensions = "";
        private object _testCaseObject;
       
        public FlowLayoutLoaderHelper(object testCaseObject)
        {
            this._testCaseObject = testCaseObject;
            LoadArguments();
        }

        /// <summary>
        /// Grabs parameters to run this test
        /// </summary>
        private void LoadArguments()
        {
            if (DriverState.DriverParameters["content"] != null)
            {
                _xamlContent = DriverState.DriverParameters["content"].ToLowerInvariant();
            }

            if (DriverState.DriverParameters["container"] != null)
            {
                _container = DriverState.DriverParameters["container"].ToLowerInvariant();
            }

            if (DriverState.DriverParameters["verification"] != null)
            {
                _verification = DriverState.DriverParameters["verification"].ToLowerInvariant();
            }

            if (DriverState.DriverParameters["width"] != null)
            {
                _desiredWidth = Double.Parse(DriverState.DriverParameters["width"].ToLowerInvariant());
            }

            if (DriverState.DriverParameters["height"] != null)
            {
                _desiredHeight = Double.Parse(DriverState.DriverParameters["height"].ToLowerInvariant());
            }

            if (DriverState.DriverParameters["cwf"] != null)
            {
                if (DriverState.DriverParameters["cwf"].ToLowerInvariant() == "true")
                {
                    _columnWidthFlexible = true;
                }
                else
                {
                    _columnWidthFlexible = false;
                }
            }

            if (DriverState.DriverParameters["pp"] != null)
            {
                string[] thick = DriverState.DriverParameters["pp"].Split(new Char[] { ':', ';' });
                _pagePadding = new Thickness(Double.Parse(thick[0]), Double.Parse(thick[1]), Double.Parse(thick[2]), Double.Parse(thick[3]));
            }

            if (DriverState.DriverParameters["cw"] != null)
            {
                _columnWidth = Double.Parse(DriverState.DriverParameters["cw"].ToLowerInvariant());
            }

            if (DriverState.DriverParameters["cg"] != null)
            {
                _columnGap = Double.Parse(DriverState.DriverParameters["cg"].ToLowerInvariant());
            }

            if (DriverState.DriverParameters["crw"] != null)
            {
                _columnRuleWidth = Double.Parse(DriverState.DriverParameters["crw"].ToLowerInvariant());
            }

            if (DriverState.DriverParameters["pgnum"] != null)
            {
                _pgNum = Int32.Parse(DriverState.DriverParameters["pgnum"].ToLowerInvariant());
            }

            if (DriverState.DriverParameters["bg"] != null)
            {
                if (DriverState.DriverParameters["bg"].ToLowerInvariant() == "true")
                {
                    _background = true;
                }
            }

            if (DriverState.DriverParameters["optimal"] != null)
            {
                if (DriverState.DriverParameters["optimal"].ToLowerInvariant() == "true")
                {
                    _optimal = true;
                }
            }

            if (DriverState.DriverParameters["hyphenation"] != null)
            {
                if (DriverState.DriverParameters["hyphenation"].ToLowerInvariant() == "true")
                {
                    _hyphenation = true;
                }
            }

            if (DriverState.DriverParameters["masterdimensions"] != null)
            {
                _masterDimensions = DriverState.DriverParameters["masterdimensions"].ToLowerInvariant();
            }
        }

        /// <summary>
        /// Creates a window and loads it with the provided xaml content
        /// The ContentRendered event handler will continue the test
        /// </summary>
        public void RunLayoutVerificationTest()
        {           
            _testWin = new Window();
            _testWin.Height = 650;
            _testWin.Width = 820;
            _testWin.Left = 0;
            _testWin.Top = 0;
            _testWin.Title = "FlowLayoutLoader: " + _xamlContent;
            _testWin.ContentRendered += new EventHandler(testWin_ContentRendered);
            _testWin.Content = LoadContent();
            if (_verification == "pdump")
            {                
                // Setting Window.Content size to ensure same size of root element over all themes.  
                // Different themes have diffent sized window chrome which will cause property dump 
                // and vscan failures even though the rest of the content is the same.
                // 784x564 is the content size of a 800x600 window in Aero them.
                ((FrameworkElement)_testWin.Content).Height = 564;
                ((FrameworkElement)_testWin.Content).Width = 784;
                _testWin.SizeToContent = SizeToContent.WidthAndHeight;
            }
            _testWin.Show();

            WaitForPriority(DispatcherPriority.ApplicationIdle);
        }

        public void CloseWindow()
        {
            _testWin.Close();
        }

        /// <summary>
        /// Loads the content of the supplied xaml file into a 
        /// bottomless or paginated format depending on test parameters
        /// and returns the content as a Border element.
        /// Layout properties are set depending on test parameters
        /// </summary>
        private Border LoadContent()
        {
            GlobalLog.LogStatus("Loading content...");
            Border layoutBorder = new Border();
            layoutBorder.BorderThickness = new Thickness(1);
            Grid mainPanel = new Grid();
            if (_verification != "pdump")
            {
                mainPanel.Width = _testWin.Width - 10;
                mainPanel.Height = _testWin.Height - 35;
            }
            else //match to window content width/height so content is not clipped.
            {
                mainPanel.Width = 784 - 10;
                mainPanel.Height = 564 - 35;
            }

            ((IAddChild)layoutBorder).AddChild(mainPanel);

            //Serialize content in partial xaml file
            _xamlContent = System.IO.Path.Combine(System.Environment.CurrentDirectory, _xamlContent);

            FlowDocument fd = new FlowDocument();
            fd.IsOptimalParagraphEnabled = _optimal;
            fd.IsHyphenationEnabled = _hyphenation;

            if (_background)
            {
                fd.Background = Brushes.Wheat;
            }

            if (fd.IsColumnWidthFlexible != _columnWidthFlexible)
            {
                fd.IsColumnWidthFlexible = _columnWidthFlexible;
            }

            if (_columnWidth > 0)
            {
                fd.ColumnWidth = _columnWidth;
            }

            if (_pagePadding.Left > -1)
            {
                fd.PagePadding = _pagePadding;
            }

            if (_columnGap > 0)
            {
                fd.ColumnGap = _columnGap;
            }

            if (_columnRuleWidth > 0)
            {
                fd.ColumnRuleWidth = _columnRuleWidth;
                fd.ColumnRuleBrush = Brushes.Black;
            }

            AddPartialXamlContent(fd);

            if (_container == "sv")
            {
                layoutBorder.BorderBrush = Brushes.Black;

                GlobalLog.LogStatus("Using FlowDocumentScrollViewer");
                FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
                fdsv.Document = fd;
                fdsv.VerticalScrollBarVisibility = fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;               

                if (_desiredWidth != 0)
                {
                    fdsv.Width = _desiredWidth;
                }

                if (_desiredHeight != 0)
                {
                    fdsv.Height = _desiredHeight;
                }

                ((IAddChild)mainPanel).AddChild(fdsv);
            }
            else if (_container == "fd")
            {
                layoutBorder.BorderBrush = Brushes.Red;
                GlobalLog.LogStatus("Using a custom Paginator with FlowDocument");

                if (_desiredHeight == 0)
                {
                    _desiredHeight = mainPanel.Height - 2;
                }

                if (_desiredWidth == 0)
                {
                    _desiredWidth = mainPanel.Width - 2;
                }

                fd.PageWidth = _desiredWidth;
                fd.PageHeight = _desiredHeight;

                DocumentPageView dpv = new DocumentPageView();
                dpv.DocumentPaginator = ((IDocumentPaginatorSource)fd).DocumentPaginator;
                dpv.PageNumber = _pgNum;

                ((IAddChild)mainPanel).AddChild(dpv);
            }
            return layoutBorder;
        }

        /// <summary>
        /// Load content from a specified xaml file and add it to the current test application's content.
        /// </summary>
        /// <param name="fd">The FlowDocument which the content will be added to.</param>
        private void AddPartialXamlContent(FlowDocument fd)
        {
            ParserContext pc = new ParserContext();
            pc.BaseUri = new Uri("pack://siteoforigin:,,,/");
            object o = XamlReader.Load(File.OpenRead(_xamlContent), pc);

            if (o is ContentElement)
            {
                ((IAddChild)fd).AddChild((ContentElement)o);
            }
            else
            {
                ((IAddChild)fd).AddChild((FrameworkElement)o);
            }
        }

        /// <summary>
        /// Calls specified verification methods
        /// </summary>
        private void DelayVerification()
        {
            if (_verification == "vscan")
            {              
                GlobalLog.LogStatus("Verifying using VScan");
                VScanCommon tool = new VScanCommon(_testWin, _testCaseObject, DriverState.TestName);
               
                //If a particular master dimension is specified, than vscan will try to locate a master that was captured
                //under the same specified condition if one exists.
                if (_masterDimensions != "")
                {
                    string[] mDim = _masterDimensions.Split(new Char[] { ':', ';' });

                    foreach (string dim in mDim)
                    {
                        switch (dim)
                        {
                            case "wpfculture":
                                {
                                    tool.Index.AddCriteria(MasterMetadata.CurrentWPFUICultureDimension, 2);
                                    break;
                                }                           
                            case "theme":
                                {
                                    tool.Index.AddCriteria(MasterMetadata.ThemeDimension, 1);
                                    break;
                                }
                            case "os":
                                {
                                    tool.Index.AddCriteria(MasterMetadata.OsVersionDimension, 1);
                                    break;
                                }
                            case "dpi":
                                {
                                    tool.Index.AddCriteria(MasterMetadata.DpiDimension, 1);
                                    break;
                                }
                            default:
                                {
                                    TestLog.Current.LogEvidence("Did not recognize the master dimension specification");
                                    break;
                                }
                        }
                    }
                }
                if (_masterDimensions == "dpi")
                {
                    //Because of the differences due to high dpi we need to use a high dpi master
                    //Because we have the high dpi master, we don't want to use the special high dpi tolerance threshhold.
                    string tolerance = "<Tolerances>" + tool.DefaultStrictTolerance + "</Tolerances>";
                    tool.ResizeWindowForDpi = false;
                    finalResult = tool.CompareImage(null, tolerance, DriverState.TestName);
                }
                else
                {
                    finalResult = tool.CompareImage();
                }
            }
            else if (_verification == "pdump")
            {               
                GlobalLog.LogStatus("Property Dump Verification");
                PropertyDumpHelper helper = new PropertyDumpHelper((Visual)_testWin.Content);                
                finalResult = helper.CompareLogShow(new Arguments(_testCaseObject));
            }
        }

        /// <summary>
        /// Window ContentRendered event fires, move the mouse out of the way, and do verification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void testWin_ContentRendered(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("Content has rendered");
            GlobalLog.LogStatus("Moving the mouse out of the way");
            MTI.Input.MoveTo(new Point(0, 0));

            GlobalLog.LogStatus("Wait a few seconds to make sure all content is visible");
            WaitFor(1000);

            DelayVerification();
        }         
    }    
}
