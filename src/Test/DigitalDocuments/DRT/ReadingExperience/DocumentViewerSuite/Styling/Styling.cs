// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: DRT which tests the Styling of the DocumentViewer controls
//
//
using DRT;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.Threading;
using System;
using System.IO;
using System.Runtime.InteropServices;

using System.Text;


namespace DRTDocumentViewerSuite
{
   
    /// <summary>
    /// The StylingSuite is responsible for running the individual DocumentViewer Styling tests.
    /// </summary>
    public sealed class StylingSuite : DrtTestSuite
    {
        public StylingSuite() : base("Styling")
        {
            TeamContact = "WPF";
            Contact = "Microsoft";
        }       

        public override DrtTest[] PrepareTests()
        {
            //Instantiate our content tree here
            _documentViewer = new DocumentViewer();
            //Give DocumentViewer our custom style
            _documentViewer.Style = CreateAlternateStyle();
            _documentViewer.Document = LoadXaml("DrtFiles\\DocumentViewerSuite\\Styling\\fixedtest.xaml");

            if (_documentViewer.Document != null)
            {
                ((DynamicDocumentPaginator)_documentViewer.Document.DocumentPaginator).PaginationCompleted += new EventHandler(OnPaginationCompleted);
                //If the content is already paginated (as is the case for certain Fixed content)
                if (_documentViewer.Document.DocumentPaginator.IsPageCountValid)
                {
                    OnPaginationCompleted(_documentViewer.Document, EventArgs.Empty);
                }
            }
            DRT.Show(_documentViewer);

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                new DrtTest( WaitForPaginationCompleted ),
                new DrtTest( TestStyle ),
                new DrtTest( TestZoom ),
                new DrtTest( WaitForPaginationCompleted ),
                new DrtTest( TestHorizontalOffset ),
                new DrtTest( TestVerticalOffset ),
                new DrtTest( TestGridColumnCount ),
                new DrtTest( WaitForPaginationCompleted ),
                new DrtTest( TestShowPageBorders ),
                new DrtTest( TestHorizontalPageSpacing ),
                new DrtTest( TestVerticalPageSpacing ),
                };
        }


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------
        #region Private Methods

        /// <summary>
        /// Load Xaml content from file.
        /// </summary>
        /// <param name="filename">Relative filename to load.</param>
        private System.Windows.Documents.IDocumentPaginatorSource LoadXaml(string filename)
        {
            System.IO.FileStream fileStream = new System.IO.FileStream(filename, System.IO.FileMode.Open);

            object newXaml = System.Windows.Markup.XamlReader.Load(fileStream);
            // Leave fileStream open due to async loading.
            //  fileStream.Close();

            if (newXaml is System.Windows.Documents.IDocumentPaginatorSource)
            {
                return (System.Windows.Documents.IDocumentPaginatorSource)newXaml;
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "DocumentViewer can only accept children of type IDocumentPaginatorSource.");
                return null;
            }
        }

        // Run this test until pagination completes.
        private void WaitForPaginationCompleted()
        {
            if (!_paginationCompleted)
            {
                DRT.Pause(_paginationDelay);
                DRT.ResumeAt(WaitForPaginationCompleted);
            }
        }

        private void OnPaginationCompleted(object sender, EventArgs args)
        {
            _paginationCompleted = true;
        }       

        /// <summary>
        /// Finds an element with the given Name.
        /// </summary>
        /// <param name="id">The Name to search for</param>
        /// <param name="root">The root of the tree to search</param>
        /// <returns>The FrameworkElement that was found by the search, or null if no match was found.</returns>
        private FrameworkElement FindElementByID(string id, DependencyObject root)
        {
            FrameworkElement fe = root as FrameworkElement;

            //Check to see if this node has the Name we're looking for.
            if (fe != null && fe.Name == id)
            {
                return fe;
            }

            //now check our children.
            int count = VisualTreeHelper.GetChildrenCount(root);
            for(int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(root, i);
                FrameworkElement r = FindElementByID(id, child);
                if (r != null)
                {
                    return r;
                }
            }

            return null;
        }

        /// <summary>
        /// Test DocumentViewer's Style to ensure it's well-formed
        /// </summary>
        private void TestStyle()
        {
            int result = 0;

            //Walk our visual tree from the root and verify
            //that our ContentHost has been filled properly.
            ScrollViewer contentArea = _documentViewer.Template.FindName(_contentHostName, _documentViewer) as ScrollViewer;

            //Investigate our contentArea, make sure it's
            //what it's supposed to be and that DocumentViewer has put in a ScrollViewer
            if (contentArea == null)
            {
                result = 1;
                Console.WriteLine("No ContentHost element found.");
            }
            else
            {
                
                //DocumentViewer should have put a DocumentGrid with Name "DocumentGrid"
                //inside of the Content Area.
                if ((contentArea.Content != null) && (contentArea.Content is FrameworkElement) && (((FrameworkElement)contentArea.Content).Name == "DocumentGrid"))
                {
                    Console.WriteLine("ContentHost is OK.");
                }
                else
                {
                    result = 2;
                    Console.WriteLine("ContentHost does not contain a DocumentGrid.");
                }
            
            }

            DRT.Assert(result == 0, "DocumentViewer's VisualTree is invalid!");

        }

        /// <summary>
        /// Test the Zoom bindings used DocumentViewer's Style.
        /// </summary>
        private void TestZoom()
        {
            int result = 0;

            // Grab a reference to the zoomslider
            Slider zoomSlider = FindElementByID("ZoomSlider", _documentViewer) as Slider;

            // Ensure zoomslider was found in style.
            if (zoomSlider == null)
            {
                result = 1;
                Console.WriteLine("ZoomSlider is not in the Style's Visual Tree.");
            }
            else
            {
                //Update ZoomSliders's Value and make sure DocumentViewer follows suit.
                zoomSlider.Value = 200.0;

                if (_documentViewer.Zoom != zoomSlider.Value)
                {
                    result = 2;
                    Console.WriteLine("Data binding ZoomSlider->DocumentViewer failed.");
                }

                //Now update DocumentViewer's ZoomPercentage and see if ZoomSlider is updated.
                _documentViewer.Zoom = 100.0;

                if (zoomSlider.Value != _documentViewer.Zoom)
                {
                    result = 2;
                    Console.WriteLine("Data binding DocumentViewer->ZoomSlider failed.");
                }
            }
            DRT.Assert(result == 0, "Data binding failed for ZoomPercentage.");
        }

        /// <summary>
        /// Test the GridColumnCount bindings used DocumentViewer's Style.
        /// </summary>
        private void TestGridColumnCount()
        {
            int result = 0;

            // Grab a reference to the columnslider
            Slider columnSlider = FindElementByID("ColumnSlider", _documentViewer) as Slider;

            // Ensure columnSlider was found in style.
            if (columnSlider == null)
            {
                result = 1;
                Console.WriteLine("ColumnSlider is not in the Style's Visual Tree.");
            }
            else
            {
                //Update ColumnSliders's value and make sure DocumentViewer's is updated as well.
                columnSlider.Value = 3;

                if (_documentViewer.MaxPagesAcross != columnSlider.Value)
                {
                    result = 2;
                    Console.WriteLine("Data binding ColumnSlider->DocumentViewer failed.");
                }

                //Now change DocumentViewer's GridColumnCount and make sure ColumnSlider gets changed as well.
                _documentViewer.MaxPagesAcross = 2;

                if (columnSlider.Value != _documentViewer.MaxPagesAcross)
                {
                    result = 2;
                    Console.WriteLine("Data binding DocumentViewer->ColumnSlider failed.");
                }
            }
            DRT.Assert(result == 0, "Data binding failed for GridColumnCount.");
        }

        /// <summary>
        /// Test the HorizontalOffset bindings used DocumentViewer's Style.
        /// </summary>
        private void TestHorizontalOffset()
        {
            int result = 0;

            // Grab a reference to the horzOffsetTextBox
            TextBox horzOffsetTextBox = FindElementByID("HorzOffsetTextBox", _documentViewer) as TextBox;

            // Ensure horzOffsetTextBox was found in style.
            if (horzOffsetTextBox == null)
            {
                result = 1;
                Console.WriteLine("HorzOffsetTextBox is not in the Style's Visual Tree.");
            }
            else
            {
                //Update HorzOffsetTextBox's value and make sure DocumentViewer's is updated as well.
                horzOffsetTextBox.Text = (20.0).ToString();

                if (_documentViewer.HorizontalOffset != double.Parse(horzOffsetTextBox.Text))
                {
                    result = 2;
                    Console.WriteLine("Data binding HorzOffsetTextBox->DocumentViewer failed.");
                }

                //Now change DocumentViewer's HorizontalOffset and make sure HorzOffsetTextBox gets changed as well.
                _documentViewer.HorizontalOffset = 0.0;

                if (double.Parse(horzOffsetTextBox.Text) != _documentViewer.HorizontalOffset)
                {
                    result = 2;
                    Console.WriteLine("Data binding DocumentViewer->HorzOffsetTextBox failed.");
                }
            }
            DRT.Assert(result == 0, "Data binding failed for HorizontalOffset.");
        }

        /// <summary>
        /// Test the VerticalOffset bindings used DocumentViewer's Style.
        /// </summary>
        private void TestVerticalOffset()
        {
            int result = 0;

            // Grab a reference to the vertOffsetTextBox
            TextBox vertOffsetTextBox = FindElementByID("VertOffsetTextBox", _documentViewer) as TextBox;

            // Ensure vertOffsetTextBox was found in style.
            if (vertOffsetTextBox == null)
            {
                result = 1;
                Console.WriteLine("VertOffsetTextBox is not in the Style's Visual Tree.");
            }
            else
            {
                //Update VertOffsetTextBox's value and make sure DocumentViewer's is updated as well.
                vertOffsetTextBox.Text =(20.0).ToString();

                if (_documentViewer.VerticalOffset != double.Parse(vertOffsetTextBox.Text))
                {
                    result = 2;
                    Console.WriteLine("Data binding VertOffsetTextBox->DocumentViewer failed.");
                }

                //Now change DocumentViewer's VerticalOffset and make sure VertOffsetTextBox gets changed as well.
                _documentViewer.VerticalOffset = 0.0;

                if (double.Parse(vertOffsetTextBox.Text) != _documentViewer.VerticalOffset)
                {
                    result = 2;
                    Console.WriteLine("Data binding DocumentViewer->VertOffsetTextBox failed.");
                }
            }
            DRT.Assert(result == 0, "Data binding failed for VerticalOffset.");
        }

        /// <summary>
        /// Test the ShowPageBorders bindings used DocumentViewer's Style.
        /// </summary>
        private void TestShowPageBorders()
        {
            int result = 0;

            // Grab a reference to the pageBordersCheckBox
            CheckBox pageBordersCheckBox = FindElementByID("PageBordersCheckBox", _documentViewer) as CheckBox;

            // Ensure pageBordersCheckBox was found in style.
            if (pageBordersCheckBox == null)
            {
                result = 1;
                Console.WriteLine("PageBordersCheckBox is not in the Style's Visual Tree.");
            }
            else
            {
                //Update PageBordersCheckBox's value and make sure DocumentViewer's is updated as well.
                pageBordersCheckBox.IsChecked = false;

                if (_documentViewer.ShowPageBorders != pageBordersCheckBox.IsChecked)
                {
                    result = 2;
                    Console.WriteLine("Data binding PageBordersCheckBox->DocumentViewer failed.");
                }

                //Now change DocumentViewer's ShowPageBorders and make sure PageBordersCheckBox gets changed as well.
                _documentViewer.ShowPageBorders = true;

                if (pageBordersCheckBox.IsChecked != _documentViewer.ShowPageBorders)
                {
                    result = 2;
                    Console.WriteLine("Data binding DocumentViewer->PageBordersCheckBox failed.");
                }
            }
            DRT.Assert(result == 0, "Data binding failed for ShowPageBorders.");
        }

        /// <summary>
        /// Test the HorizontalPageSpacing bindings used DocumentViewer's Style.
        /// </summary>
        private void TestHorizontalPageSpacing()
        {
            int result = 0;

            // Grab a reference to the horzSpacingSlider
            Slider horzSpacingSlider = FindElementByID("HorzSpacingSlider", _documentViewer) as Slider;

            // Ensure horzSpacingSlider was found in style.
            if (horzSpacingSlider == null)
            {
                result = 1;
                Console.WriteLine("HorzSpacingSlider is not in the Style's Visual Tree.");
            }
            else
            {
                //Update HorzSpacingSlider's Value and make sure DocumentViewer follows suit.
                horzSpacingSlider.Value = 0.0;

                if (_documentViewer.HorizontalPageSpacing != horzSpacingSlider.Value)
                {
                    result = 2;
                    Console.WriteLine("Data binding HorzSpacingSlider->DocumentViewer failed.");
                }

                //Now update DocumentViewer's HorizontalPageSpacing and see if HorzSpacingSlider is updated.
                _documentViewer.HorizontalPageSpacing = 20.0;

                if (horzSpacingSlider.Value != _documentViewer.HorizontalPageSpacing)
                {
                    result = 2;
                    Console.WriteLine("Data binding DocumentViewer->HorzSpacingSlider failed.");
                }
            }
            DRT.Assert(result == 0, "Data binding failed for HorizontalPageSpacing.");
        }

        /// <summary>
        /// Test the VerticalPageSpacing bindings used DocumentViewer's Style.
        /// </summary>
        private void TestVerticalPageSpacing()
        {
            int result = 0;

            // Grab a reference to the horzSpacingSlider
            Slider vertSpacingSlider = FindElementByID("VertSpacingSlider", _documentViewer) as Slider;

            // Ensure vertSpacingSlider was found in style.
            if (vertSpacingSlider == null)
            {
                result = 1;
                Console.WriteLine("VertSpacingSlider is not in the Style's Visual Tree.");
            }
            else
            {
                //Update VertSpacingSlider's Value and make sure DocumentViewer follows suit.
                vertSpacingSlider.Value = 0.0;

                if (_documentViewer.VerticalPageSpacing != vertSpacingSlider.Value)
                {
                    result = 2;
                    Console.WriteLine("Data binding VertSpacingSlider->DocumentViewer failed.");
                }

                //Now update DocumentViewer's VerticalPageSpacing and see if VertSpacingSlider is updated.
                _documentViewer.VerticalPageSpacing = 20.0;

                if (vertSpacingSlider.Value != _documentViewer.VerticalPageSpacing)
                {
                    result = 2;
                    Console.WriteLine("Data binding DocumentViewer->VertSpacingSlider failed.");
                }
            }
            DRT.Assert(result == 0, "Data binding failed for VerticalPageSpacing.");
        }

        /// <summary>
        /// Create our own style.
        /// </summary>
        /// <returns>The Style we built</returns>
        private Style CreateAlternateStyle()
        {
            // The new Style for DocumentViewer
            Style style;

            Binding binding; // Used to for databinding properties.

            //Create a Slider and bind to control GridColumnCount.
            FrameworkElementFactory fefColumnSlider = new FrameworkElementFactory(typeof(Slider));
            fefColumnSlider.SetValue(Slider.OrientationProperty, Orientation.Vertical);
            fefColumnSlider.SetValue(Slider.TickPlacementProperty, TickPlacement.Both);
            fefColumnSlider.SetValue(Slider.TickFrequencyProperty, 1.0);
            fefColumnSlider.SetValue(Slider.MaximumProperty, 6.0);
            fefColumnSlider.SetValue(Slider.MinimumProperty, 1.0);
            fefColumnSlider.SetValue(Slider.LargeChangeProperty, 2.0);
            fefColumnSlider.SetValue(Slider.SmallChangeProperty, 1.0);
            fefColumnSlider.SetValue(Slider.SelectionStartProperty, 1.0);
            fefColumnSlider.SetValue(Slider.SelectionEndProperty, 6.0);
            fefColumnSlider.SetValue(FrameworkElement.NameProperty, "ColumnSlider");
            binding = new Binding("MaxPagesAcross");
            binding.Mode = BindingMode.TwoWay;
            binding.RelativeSource = RelativeSource.TemplatedParent;
            fefColumnSlider.SetBinding(Slider.ValueProperty, binding);

            //Create a Slider and binding to control ZoomPercentage.
            FrameworkElementFactory fefZoomSlider = new FrameworkElementFactory(typeof(Slider));
            fefZoomSlider.SetValue(Slider.TickPlacementProperty, TickPlacement.Both);
            fefZoomSlider.SetValue(Slider.TickFrequencyProperty, 50.0);
            fefZoomSlider.SetValue(Slider.MaximumProperty, 300.0);
            fefZoomSlider.SetValue(Slider.MinimumProperty, 100.0);
            fefZoomSlider.SetValue(Slider.LargeChangeProperty, 2.0);
            fefZoomSlider.SetValue(Slider.SmallChangeProperty, 1.0);
            fefZoomSlider.SetValue(Slider.SelectionStartProperty, 1.0);
            fefZoomSlider.SetValue(Slider.SelectionEndProperty, 6.0);
            fefZoomSlider.SetValue(FrameworkElement.NameProperty, "ZoomSlider");
            binding = new Binding("Zoom");
            binding.Mode = BindingMode.TwoWay;
            binding.RelativeSource = RelativeSource.TemplatedParent;
            fefZoomSlider.SetBinding(Slider.ValueProperty, binding);

            //Create a textbox and binding to control HorizontalOffset.
            FrameworkElementFactory fefHorzOffsetTextBox = new FrameworkElementFactory(typeof(TextBox));
            fefHorzOffsetTextBox.SetValue(FrameworkElement.NameProperty, "HorzOffsetTextBox");
            binding = new Binding("HorizontalOffset");
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.RelativeSource = RelativeSource.TemplatedParent;
            fefHorzOffsetTextBox.SetBinding(TextBox.TextProperty, binding);

            //Create a textbox and bind to control VerticalOffset.
            FrameworkElementFactory fefVertOffsetTextBox = new FrameworkElementFactory(typeof(TextBox));
            fefVertOffsetTextBox.SetValue(FrameworkElement.NameProperty, "VertOffsetTextBox");
            binding = new Binding("VerticalOffset");
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.RelativeSource = RelativeSource.TemplatedParent;
            fefVertOffsetTextBox.SetBinding(TextBox.TextProperty, binding);

            //Create a checkbox and bind to control ShowPageBorders.
            FrameworkElementFactory fefPageBordersCheckBox = new FrameworkElementFactory(typeof(CheckBox));
            fefPageBordersCheckBox.SetValue(FrameworkElement.NameProperty, "PageBordersCheckBox");
            binding = new Binding("ShowPageBorders");
            binding.Mode = BindingMode.TwoWay;
            binding.RelativeSource = RelativeSource.TemplatedParent;
            fefPageBordersCheckBox.SetBinding(CheckBox.IsCheckedProperty, binding);

            //Create a Slider and bind to control HorizontalSpacing.
            FrameworkElementFactory fefHorzSpacingSlider = new FrameworkElementFactory(typeof(Slider));
            fefHorzSpacingSlider.SetValue(Slider.TickPlacementProperty, TickPlacement.Both);
            fefHorzSpacingSlider.SetValue(Slider.TickFrequencyProperty, 10.0);
            fefHorzSpacingSlider.SetValue(Slider.MaximumProperty, 30.0);
            fefHorzSpacingSlider.SetValue(Slider.MinimumProperty, 0.0);
            fefHorzSpacingSlider.SetValue(Slider.LargeChangeProperty, 10.0);
            fefHorzSpacingSlider.SetValue(Slider.SmallChangeProperty, 5.0);
            fefHorzSpacingSlider.SetValue(Slider.SelectionStartProperty, 0.0);
            fefHorzSpacingSlider.SetValue(Slider.SelectionEndProperty, 30.0);
            fefHorzSpacingSlider.SetValue(FrameworkElement.NameProperty, "HorzSpacingSlider");
            binding = new Binding("HorizontalPageSpacing");
            binding.Mode = BindingMode.TwoWay;
            binding.RelativeSource = RelativeSource.TemplatedParent;
            fefHorzSpacingSlider.SetBinding(Slider.ValueProperty, binding);

            //Create a Slider and bind to control VerticalSpacing.
            FrameworkElementFactory fefVertSpacingSlider = new FrameworkElementFactory(typeof(Slider));
            fefVertSpacingSlider.SetValue(Slider.OrientationProperty, Orientation.Vertical);
            fefVertSpacingSlider.SetValue(Slider.TickPlacementProperty, TickPlacement.Both);
            fefVertSpacingSlider.SetValue(Slider.TickFrequencyProperty, 10.0);
            fefVertSpacingSlider.SetValue(Slider.MaximumProperty, 30.0);
            fefVertSpacingSlider.SetValue(Slider.MinimumProperty, 0.0);
            fefVertSpacingSlider.SetValue(Slider.LargeChangeProperty, 10.0);
            fefVertSpacingSlider.SetValue(Slider.SmallChangeProperty, 5.0);
            fefVertSpacingSlider.SetValue(Slider.SelectionStartProperty, 0.0);
            fefVertSpacingSlider.SetValue(Slider.SelectionEndProperty, 30.0);
            fefVertSpacingSlider.SetValue(Control.NameProperty, "VertSpacingSlider");
            binding = new Binding("VerticalPageSpacing");
            binding.Mode = BindingMode.TwoWay;
            binding.RelativeSource = RelativeSource.TemplatedParent;
            fefVertSpacingSlider.SetBinding(Slider.ValueProperty, binding);

            // StackPanel to hold controls
            FrameworkElementFactory fefStackPanel = new FrameworkElementFactory(typeof(StackPanel));
            fefStackPanel.SetValue(Grid.RowProperty, 0);
            fefStackPanel.SetValue(Grid.ColumnProperty, 1);
            fefStackPanel.AppendChild(fefColumnSlider);
            fefStackPanel.AppendChild(fefZoomSlider);
            fefStackPanel.AppendChild(fefHorzOffsetTextBox);
            fefStackPanel.AppendChild(fefVertOffsetTextBox);
            fefStackPanel.AppendChild(fefPageBordersCheckBox);
            fefStackPanel.AppendChild(fefHorzSpacingSlider);
            fefStackPanel.AppendChild(fefVertSpacingSlider);

            // ScrollViewer set as the content area.
            FrameworkElementFactory fefScrollViewer = new FrameworkElementFactory(typeof(ScrollViewer), _contentHostName);            
            fefScrollViewer.SetValue(Control.BackgroundProperty, System.Windows.Media.Brushes.Purple);            
            fefScrollViewer.SetValue(ScrollViewer.CanContentScrollProperty, true);
            fefScrollViewer.SetValue(Grid.RowProperty, 0);
            fefScrollViewer.SetValue(Grid.ColumnProperty, 0);

            // Grid to layout the entire style.
            FrameworkElementFactory fefGrid = new FrameworkElementFactory(typeof(Grid));
            fefGrid.SetValue(Control.NameProperty, "ContentGrid");
            FrameworkElementFactory fefRowDef = new FrameworkElementFactory(typeof(RowDefinition));
            fefRowDef.SetValue(RowDefinition.HeightProperty, new GridLength(1, GridUnitType.Star));
            fefGrid.AppendChild(fefRowDef);
            FrameworkElementFactory fefColumnDef = new FrameworkElementFactory(typeof(ColumnDefinition));
            fefColumnDef.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));
            fefGrid.AppendChild(fefColumnDef);
            fefColumnDef = new FrameworkElementFactory(typeof(ColumnDefinition));
            fefColumnDef.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Auto));
            fefGrid.AppendChild(fefColumnDef);
            fefGrid.AppendChild(fefStackPanel);
            fefGrid.AppendChild(fefScrollViewer);

            //Build DocumentViewer's style
            style = new Style(typeof(DocumentViewer));
            ControlTemplate template = new ControlTemplate(typeof(DocumentViewer));
            template.VisualTree = fefGrid;
            style.Setters.Add(new Setter(Control.TemplateProperty, template));

            return style;
        }
        #endregion Private Methods

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------
        #region Private Fields

        private DocumentViewer _documentViewer;

        private bool _paginationCompleted;
        // Amount of time (msec) to delay while waiting for pagination to complete.
        private const int _paginationDelay = 500;

        private const string _contentHostName = "PART_ContentHost";

        #endregion Private Fields

    }
}


