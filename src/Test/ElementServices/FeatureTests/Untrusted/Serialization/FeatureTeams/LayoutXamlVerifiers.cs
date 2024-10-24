// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Windows.Controls;
using Avalon.Test.CoreUI.Common;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Collections;
using Avalon.Test.CoreUI.Parser;

using Microsoft.Test.Serialization.CustomElements;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// Cell resources verification
    /// </summary>
    public class LayoutXamlVerifiers
    {

        /// <summary>
        /// 
        /// </summary>
        public static void BorderVerify(UIElement uie)
        {
            CoreLogger.LogStatus("Inside LayoutXamlVerifiers.BorderVerify()...");
            
            StackPanel myPanel = uie as StackPanel;

            if (null == myPanel)
            {
                throw new Microsoft.Test.TestValidationException("Should be StackPanel");
            }

            UIElementCollection myChildren = myPanel.Children;
            CoreLogger.LogStatus(myPanel.Children.ToString());
            CoreLogger.LogStatus(myPanel.Children.Count.ToString());

//          Border myBorder = IDedObjects["border"] as Border.
//            if (null == myBorder)
//            {
//                throw new Microsoft.Test.TestValidationException("Should Have a border").
//            }
//            VerifyElement.VerifyDouble(((FrameworkElement)myBorder).Height, 100).
//            VerifyElement.VerifyDouble(((FrameworkElement)myBorder).Width, 100).
//            VerifyElement.VerifyThickness(((FrameworkElement)myBorder).Margin, new Thickness(10)).
//            VerifyElement.VerifyColor(((SolidColorBrush)(myBorder.Background)).Color, Colors.Orange).
//            VerifyElement.VerifyColor(((SolidColorBrush)(myBorder.BorderBrush)).Color, Colors.RoyalBlue).
//            VerifyElement.VerifyThickness(myBorder.BorderThickness, new Thickness(10, 5, 25, 11)).
        }
        /// <summary>
        /// 
        /// </summary>
        public static void CanvasVerify(UIElement uie)
        {
            CoreLogger.LogStatus("Inside LayoutXamlVerifiers.BorderVerify()...");

            CustomCanvas myPanel = uie as CustomCanvas;

            if (null == myPanel)
            {
                throw new Microsoft.Test.TestValidationException("Should be CustomCanvas");
            }

            UIElementCollection myChildren = myPanel.Children;

            if (myChildren.Count != 2)
            {
                throw new Microsoft.Test.TestValidationException("Should has 2 children");
            }

            Canvas canvas1 = (Canvas)LogicalTreeHelper.FindLogicalNode(uie, "Canvas1");
            Canvas canvas2 = (Canvas)LogicalTreeHelper.FindLogicalNode(uie, "Canvas2");

            if (null == canvas1 || null == canvas2)
            {
                throw new Microsoft.Test.TestValidationException("Child canvas is missed");
            }

            // location
            VerifyElement.VerifyDouble(Canvas.GetTop(canvas1), 25);
            VerifyElement.VerifyDouble(Canvas.GetRight(canvas1), 25 * 96);
            VerifyElement.VerifyDouble(Canvas.GetLeft(canvas2), 25);
            VerifyElement.VerifyDouble(Canvas.GetBottom(canvas2), 25);
            
            //backgroud
            VerifyElement.VerifyColor(((SolidColorBrush)(((Panel)canvas1).Background)).Color, Colors.Gray);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Panel)canvas2).Background)).Color, Colors.RoyalBlue);
            
            //size
            VerifyElement.VerifyDouble(((FrameworkElement)canvas1).Height, 100);
            VerifyElement.VerifyDouble(((FrameworkElement)canvas1).Width, 100);
            VerifyElement.VerifyDouble(((FrameworkElement)canvas2).Height, 100);
            VerifyElement.VerifyDouble(((FrameworkElement)canvas2).Width, 100);
        }

        /// <summary>
        /// Verification method for Dockpanel.xaml from layout group
        /// </summary>
        public static void DockPanelVerify(UIElement uie)
        {
            CoreLogger.LogStatus("Inside LayoutXamlVerifiers.DockPanelVerify()...");

            DockPanel myPanel = uie as DockPanel;

            if (null == myPanel)
            {
                throw new Microsoft.Test.TestValidationException("Should be DockPanel");
            }

            UIElementCollection myChildren = myPanel.Children;

            if (myChildren.Count != 5)
            {
                throw new Microsoft.Test.TestValidationException("Should has 5 children");
            }
            else
            {
                CoreLogger.LogStatus("Number of children OK");
            }

            

            DockPanel dockpanel1 = (DockPanel)LogicalTreeHelper.FindLogicalNode(uie, "DockPanel1");
            DockPanel dockpanel2 = (DockPanel)LogicalTreeHelper.FindLogicalNode(uie, "DockPanel2");
            DockPanel dockpanel3 = (DockPanel)LogicalTreeHelper.FindLogicalNode(uie, "DockPanel3");
            DockPanel dockpanel4 = (DockPanel)LogicalTreeHelper.FindLogicalNode(uie, "DockPanel4");
            DockPanel dockpanel5 = (DockPanel)LogicalTreeHelper.FindLogicalNode(uie, "DockPanel5");


            if (null == dockpanel1 
                || null == dockpanel2
                || null == dockpanel3
                || null == dockpanel4
                || null == dockpanel5)
            {
                throw new Microsoft.Test.TestValidationException("some Child Dockpanels are missed");
            }

            //Verify root panel
            VerifyElement.VerifyColor(((SolidColorBrush)(((Panel)myPanel).Background)).Color, Colors.Yellow);


            // backgroud for children
            VerifyElement.VerifyColor(((SolidColorBrush)(((Panel)dockpanel1).Background)).Color, Colors.Gray);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Panel)dockpanel2).Background)).Color, Colors.RoyalBlue);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Panel)dockpanel3).Background)).Color, Colors.Gray);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Panel)dockpanel4).Background)).Color, Colors.Gray);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Panel)dockpanel5).Background)).Color, Colors.LightGray);
            
            //size  for children
            VerifyElement.VerifyDouble(((FrameworkElement)dockpanel1).Height, 100);
            VerifyElement.VerifyDouble(((FrameworkElement)dockpanel2).Width, 200);
            VerifyElement.VerifyDouble(((FrameworkElement)dockpanel4).Height, 100);
            VerifyElement.VerifyDouble(((FrameworkElement)dockpanel3).Width, 50);

            //Verify Dock
            VerifyElement.VerifyDock(DockPanel.GetDock(dockpanel1), Dock.Top);
            VerifyElement.VerifyDock(DockPanel.GetDock(dockpanel2), Dock.Left);
            VerifyElement.VerifyDock(DockPanel.GetDock(dockpanel3), Dock.Right);
            VerifyElement.VerifyDock(DockPanel.GetDock(dockpanel4), Dock.Bottom);
            VerifyElement.VerifyDock(dockpanel5, true);
        }
        /// <summary>
        /// 
        /// </summary>
        public static void StackPanelVerify(UIElement uie)
        {
            CoreLogger.LogStatus("Inside LayoutXamlVerifiers.StackPanelVerify()...");

            StackPanel myPanel = uie as StackPanel;

            if (null == myPanel)
            {
                throw new Microsoft.Test.TestValidationException("Should be StackPanel");
            }

            UIElementCollection myChildren = myPanel.Children;

            VerifyElement.VerifyInt(myChildren.Count, 2);

            StackPanel stackPanel1 = (StackPanel)LogicalTreeHelper.FindLogicalNode(uie, "StackPanel1");
            StackPanel stackPanel2 = (StackPanel)LogicalTreeHelper.FindLogicalNode(uie, "StackPanel2");
            StackPanel stackPanel3 = (StackPanel)LogicalTreeHelper.FindLogicalNode(uie, "StackPanel3");
            StackPanel stackPanel4 = (StackPanel)LogicalTreeHelper.FindLogicalNode(uie, "StackPanel4");

            if (null == stackPanel1 || null == stackPanel2 || null == stackPanel3 || null == stackPanel4)
            {
                throw new Microsoft.Test.TestValidationException("Some child StackPanels are missing.");
            }
            VerifyElement.VerifyInt(stackPanel1.Children.Count, 1);
            VerifyElement.VerifyInt(stackPanel3.Children.Count, 1);
            //verify size
            //size  for children

            VerifyElement.VerifyDouble(((FrameworkElement)stackPanel1).Height, 100);
            VerifyElement.VerifyDouble(((FrameworkElement)stackPanel1).Width, 100);

            VerifyElement.VerifyDouble(((FrameworkElement)stackPanel2).Height, 50);
            VerifyElement.VerifyDouble(((FrameworkElement)stackPanel2).Width, 50);

            VerifyElement.VerifyDouble(((FrameworkElement)stackPanel3).Height, 100);
            VerifyElement.VerifyDouble(((FrameworkElement)stackPanel3).Width, 100);

            VerifyElement.VerifyDouble(((FrameworkElement)stackPanel4).Height, 50);
            VerifyElement.VerifyDouble(((FrameworkElement)stackPanel4).Width, 50);
            //background
            VerifyElement.VerifyColor(((SolidColorBrush)(((Panel)myPanel).Background)).Color, Colors.Crimson);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Panel)stackPanel1).Background)).Color, Colors.LawnGreen);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Panel)stackPanel2).Background)).Color, Colors.RoyalBlue);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Panel)stackPanel3).Background)).Color, Colors.Orange);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Panel)stackPanel4).Background)).Color, Colors.RoyalBlue);
            //alignments
            VerifyElement.VerifyInt((int)(stackPanel1.HorizontalAlignment), (int)(System.Windows.HorizontalAlignment.Center));
            VerifyElement.VerifyInt((int)(stackPanel3.VerticalAlignment), (int)(VerticalAlignment.Center));
        }
        /// <summary>
        /// 
        /// </summary>
        public static void LayoutImageVerify(UIElement uie)
        {
            CoreLogger.LogStatus("Inside LayoutXamlVerifiers.LayoutImageVerify()...");

            CustomCanvas myPanel = uie as CustomCanvas;

            if (null == myPanel)
            {
                throw new Microsoft.Test.TestValidationException("Should be CustomCanvas");
            }

            UIElementCollection myChildren = myPanel.Children;

            VerifyElement.VerifyInt(myChildren.Count, 1);

            Image myImage = myChildren[0] as Image;

            if (null == myImage)
            {
                throw new Microsoft.Test.TestValidationException("Should be Image");
            }

            VerifyElement.VerifyInt((int)(myImage.HorizontalAlignment), (int)(System.Windows.HorizontalAlignment.Center));
            VerifyElement.VerifyInt((int)(myImage.VerticalAlignment), (int)(VerticalAlignment.Center));
            VerifyElement.VerifyInt((int)(myImage.Stretch), (int)(Stretch.None));
            VerifyElement.VerifyDouble(((FrameworkElement)myImage).Height, 400);
            VerifyElement.VerifyDouble(((FrameworkElement)myImage).Width, 400);
            BitmapImage imgsrc = new BitmapImage(new Uri("avalon.png", UriKind.RelativeOrAbsolute));
            VerifyElement.VerifyInt(imgsrc.PixelHeight, ((BitmapSource)myImage.Source).PixelHeight);
            VerifyElement.VerifyInt(imgsrc.PixelWidth, ((BitmapSource)myImage.Source).PixelWidth);
        }
        /// <summary>
        /// 
        /// </summary>
        public static void LayoutGridVerify(UIElement uie)
        {
            CoreLogger.LogStatus("Inside LayoutXamlVerifiers.LayoutImageVerify()...");

            StackPanel myPanel = uie as StackPanel;

            if (null == myPanel)
            {
                throw new Microsoft.Test.TestValidationException("Should be StackPanel");
            }

            UIElementCollection myChildren = myPanel.Children;
            VerifyElement.VerifyInt(myChildren.Count, 2);

            Grid e1 = (Grid)LogicalTreeHelper.FindLogicalNode(uie, "E1");
            Grid e7 = (Grid)LogicalTreeHelper.FindLogicalNode(uie, "E7");

            if(null == e1 || null == e7)
            {
                throw new Microsoft.Test.TestValidationException("Missed Grid");
            }

            CoreLogger.LogStatus("Verify children of Grid ...");
            VerifyElement.VerifyInt(e1.Children.Count, 5);
            VerifyElement.VerifyInt(e1.Children.Count, 5);
            //size of grid panel
            CoreLogger.LogStatus("Verify size of Grid ...");
            VerifyElement.VerifyDouble(((FrameworkElement)e1).Height, 300);
            VerifyElement.VerifyDouble(((FrameworkElement)e1).Width, 200);

            VerifyElement.VerifyDouble(((FrameworkElement)e7).Height, 300);
            VerifyElement.VerifyDouble(((FrameworkElement)e7).Width, 200);
            // columns
            CoreLogger.LogStatus("Verify number of columns ...");
            VerifyElement.VerifyInt(e1.ColumnDefinitions.Count, 2);
            VerifyElement.VerifyInt(e7.ColumnDefinitions.Count, 2);
            VerifyElement.VerifyInt(e1.RowDefinitions.Count, 1);
            VerifyElement.VerifyInt(e7.RowDefinitions.Count, 1);
            //text
            CoreLogger.LogStatus("Verify text ...");

            TextBlock e2 = (TextBlock)LogicalTreeHelper.FindLogicalNode(uie, "E2");
            TextBlock e5 = (TextBlock)LogicalTreeHelper.FindLogicalNode(uie, "E5");
            TextBlock e6 = (TextBlock)LogicalTreeHelper.FindLogicalNode(uie, "E6");
            TextBlock e8 = (TextBlock)LogicalTreeHelper.FindLogicalNode(uie, "E8");
            TextBlock e11 = (TextBlock)LogicalTreeHelper.FindLogicalNode(uie, "E11");
            TextBlock e12 = (TextBlock)LogicalTreeHelper.FindLogicalNode(uie, "E12");

            if(null == e2 || null == e5 || null == e6 || null == e8|| null == e11|| null == e12)
            {
                throw new Microsoft.Test.TestValidationException("Missed textes");
            }
            CoreLogger.LogStatus("Verify text content ...");
            VerifyElement.VerifyString(e2.Text, "cell 1");
            VerifyElement.VerifyString(e5.Text, "cell 4");
            VerifyElement.VerifyString(e6.Text, "cell 5");
            VerifyElement.VerifyString(e8.Text, "cell 1");
            VerifyElement.VerifyString(e11.Text, "cell 4");
            VerifyElement.VerifyString(e12.Text, "cell 5");
            CoreLogger.LogStatus("Verify text height ...");
            VerifyElement.VerifyDouble(((FrameworkElement)e2).Height, 100);
            VerifyElement.VerifyDouble(((FrameworkElement)e5).Height, 50);
            VerifyElement.VerifyDouble(((FrameworkElement)e6).Height, 75);
            VerifyElement.VerifyDouble(((FrameworkElement)e8).Height, 50);
            VerifyElement.VerifyDouble(((FrameworkElement)e11).Height, 75);
            VerifyElement.VerifyDouble(((FrameworkElement)e12).Height, 50);
            CoreLogger.LogStatus("Verify images ...");

            Image e3 = (Image)LogicalTreeHelper.FindLogicalNode(uie, "E3");
            Image e4 = (Image)LogicalTreeHelper.FindLogicalNode(uie, "E4");
            Image e9 = (Image)LogicalTreeHelper.FindLogicalNode(uie, "E9");
            Image e10 = (Image)LogicalTreeHelper.FindLogicalNode(uie, "E10");

            if (null == e3 || null == e4 || null == e9 || null == e10)
            {
                throw new Microsoft.Test.TestValidationException("Missed images");
            }
            VerifyElement.VerifyInt(Grid.GetColumnSpan(e3), 1);
            VerifyElement.VerifyInt(Grid.GetColumnSpan(e4), 1);
            VerifyElement.VerifyInt(Grid.GetColumnSpan(e9), 1);
            VerifyElement.VerifyInt(Grid.GetColumnSpan(e10), 2);

        }
        /// <summary>
        /// 
        /// </summary>
        public static void LayoutTableVerify(UIElement uie)
        {
            CoreLogger.LogStatus("Inside LayoutXamlVerifiers.LayoutTableVerify()...");

            DockPanel myPanel = uie as DockPanel;

            if (null == myPanel)
            {
                throw new Microsoft.Test.TestValidationException("Should be DockPanel");
            }
            
            UIElementCollection myChildren = myPanel.Children;

            FlowDocumentScrollViewer myTextPanel = myChildren[0] as FlowDocumentScrollViewer;

            if (null == myTextPanel)
            {
                throw new Microsoft.Test.TestValidationException("Should be a FlowDocumentScrollViewer");
            }
            

            IEnumerator e = LogicalTreeHelper.GetChildren(myTextPanel.Document).GetEnumerator();
            e.MoveNext();

            Table myTable = (Table)e.Current;

            if (null == myTable)
            {
                throw new Microsoft.Test.TestValidationException("No Table");
            }

            CoreLogger.LogStatus("Verifying columns ...");

            TableColumnCollection myColumns = myTable.Columns;

            VerifyElement.VerifyInt(myColumns.Count, 3);

            TableColumn column1 = (TableColumn)LogicalTreeHelper.FindLogicalNode(uie, "Column1");
            TableColumn column2 = (TableColumn)LogicalTreeHelper.FindLogicalNode(uie, "Column2");
            TableColumn column3 = (TableColumn)LogicalTreeHelper.FindLogicalNode(uie, "Column3");

            VerifyElement.VerifyDouble(column1.Width.Value, 160);
            VerifyElement.VerifyColor(((SolidColorBrush)(column1.Background)).Color, Colors.Black);
            VerifyElement.VerifyDouble(column3.Width.Value, 160);
            VerifyElement.VerifyColor(((SolidColorBrush)(column3.Background)).Color, Colors.Black);
            VerifyElement.VerifyColor(((SolidColorBrush)(column2.Background)).Color, Colors.DarkGray);

            // header
            CoreLogger.LogStatus("Verifying header ...");

            TableRowGroup myHeader = myTable.RowGroups[0];

            VerifyElement.VerifyInt(myHeader.Rows.Count, 2);

            TableRow r1 = (TableRow)LogicalTreeHelper.FindLogicalNode(uie, "R1");
            TableRow r2 = (TableRow)LogicalTreeHelper.FindLogicalNode(uie, "R2");

            VerifyElement.VerifyBool(myHeader.Rows.Contains(r1), true);
            VerifyElement.VerifyColor(((SolidColorBrush)(r1.Background)).Color, Colors.DarkGray);
            VerifyElement.VerifyColor(((SolidColorBrush)(r2.Background)).Color, Colors.DarkGray);
            VerifyElement.VerifyInt(r1.Cells.Count, 1);
            VerifyElement.VerifyInt(r2.Cells.Count, 3);

            TableCell c1 = (TableCell)LogicalTreeHelper.FindLogicalNode(uie, "C1");
            TableCell c2 = (TableCell)LogicalTreeHelper.FindLogicalNode(uie, "C2");
            TableCell c3 = (TableCell)LogicalTreeHelper.FindLogicalNode(uie, "C3");
            TableCell c4 = (TableCell)LogicalTreeHelper.FindLogicalNode(uie, "C4");

            VerifyElement.VerifyBool(r1.Cells.Contains(c1), true);
            VerifyElement.VerifyBool(r2.Cells.Contains(c2), true);
            VerifyElement.VerifyBool(r2.Cells.Contains(c3), true);
            VerifyElement.VerifyBool(r2.Cells.Contains(c4), true);
            CoreLogger.LogStatus("Verifying cell ...");
            VerifyElement.VerifyColor(((SolidColorBrush)(c1.Foreground)).Color, Colors.Yellow);
            VerifyElement.VerifyColor(((SolidColorBrush)(c2.Foreground)).Color, Colors.Yellow);
            VerifyElement.VerifyColor(((SolidColorBrush)(c3.Foreground)).Color, Colors.Yellow);
            VerifyElement.VerifyColor(((SolidColorBrush)(c4.Foreground)).Color, Colors.Yellow);
            VerifyElement.VerifyInt(c1.ColumnSpan, 3);
            VerifyElement.VerifyColor(((SolidColorBrush)(c1.BorderBrush)).Color, Colors.White);
            VerifyElement.VerifyThickness(c1.Padding, new Thickness(6));
            VerifyElement.VerifyThickness(c2.BorderThickness, new Thickness(1));

            // footer
            CoreLogger.LogStatus("Verifying footer ...");
            TableRowGroup myFooter = myTable.RowGroups[2];

            VerifyElement.VerifyInt(myFooter.Rows.Count, 2);

            TableRow r3 = myFooter.Rows[0];
            TableRow r4 = myFooter.Rows[1];

            if (null == r3)
            {
                throw new Microsoft.Test.TestValidationException("Missed row  r3 in footer");
            }
            if (null == r4)
            {
                throw new Microsoft.Test.TestValidationException("Missed row r4 in footer");
            }
            VerifyElement.VerifyBool(myFooter.Rows.Contains(r3), true);
            VerifyElement.VerifyBool(myFooter.Rows.Contains(r4), true);
            VerifyElement.VerifyColor(((SolidColorBrush)(r3.Background)).Color, Colors.DarkGray);
            VerifyElement.VerifyColor(((SolidColorBrush)(r4.Background)).Color, Colors.DarkGray);
            VerifyElement.VerifyInt(r3.Cells.Count, 3);
            VerifyElement.VerifyInt(r4.Cells.Count, 1);

            TableCell c5 = r4.Cells[0];

            VerifyElement.VerifyBool(r4.Cells.Contains(c5), true);
            CoreLogger.LogStatus("Verifying cell ...");
            VerifyElement.VerifyColor(((SolidColorBrush)(c5.Foreground)).Color, Colors.Yellow);
            VerifyElement.VerifyInt(c5.ColumnSpan, 3);
            VerifyElement.VerifyColor(((SolidColorBrush)(c5.BorderBrush)).Color, Colors.White);
            VerifyElement.VerifyThickness(c5.Padding, new Thickness(6));
            VerifyElement.VerifyThickness(c5.BorderThickness, new Thickness(1));

            // body
            CoreLogger.LogStatus("Verifying body ...");
            TableRowGroup myBody = myTable.RowGroups[1];

            VerifyElement.VerifyInt(myBody.Rows.Count, 4);

            TableRow r5 = (TableRow)LogicalTreeHelper.FindLogicalNode(uie, "R5");
            TableRow r6 = (TableRow)LogicalTreeHelper.FindLogicalNode(uie, "R6");
            TableRow r7 = (TableRow)LogicalTreeHelper.FindLogicalNode(uie, "R7");
            TableRow r8 = (TableRow)LogicalTreeHelper.FindLogicalNode(uie, "R8");

            VerifyElement.VerifyBool(myBody.Rows.Contains(r5), true);
            VerifyElement.VerifyBool(myBody.Rows.Contains(r6), true);
            VerifyElement.VerifyBool(myBody.Rows.Contains(r7), true);
            VerifyElement.VerifyBool(myBody.Rows.Contains(r8), true);

            VerifyElement.VerifyInt(r5.Cells.Count, 3);
            VerifyElement.VerifyInt(r8.Cells.Count, 1);

            TableCell c6 = (TableCell)LogicalTreeHelper.FindLogicalNode(uie, "C6");

            VerifyElement.VerifyBool(r5.Cells.Contains(c6), true);

            CoreLogger.LogStatus("Verifying cell ...");
            VerifyElement.VerifyColor(((SolidColorBrush)(c6.Foreground)).Color, Colors.White);
            VerifyElement.VerifyColor(((SolidColorBrush)(c6.BorderBrush)).Color, Colors.White);
            VerifyElement.VerifyThickness(c6.Padding, new Thickness(6));
            VerifyElement.VerifyThickness(c6.BorderThickness, new Thickness(1));
            TextRange range = new TextRange((c6.ContentStart), (c6.ContentEnd));
            VerifyElement.VerifyString(range.Text, "Cell 2");
        }
        /// <summary>
        ///  Verifier for LayoutTextPanel.xaml
        /// </summary>
        public static void LayoutTextPanelVerify(UIElement uie)
        {
            CoreLogger.LogStatus("Inside LayoutXamlVerifiers.LayoutTextPanelVerify()...");

            FlowDocumentScrollViewer fdsv = uie as FlowDocumentScrollViewer;
            FlowDocument myPanel = fdsv.Document;

            if (null == myPanel)
            {
                throw new Microsoft.Test.TestValidationException("Should be FlowDocumentScrollViewer");
            }

            CoreLogger.LogStatus("Verifying root element ...");
            VerifyElement.VerifyInt((int)(myPanel.Typography.Capitals), (int)(FontCapitals.Unicase));
            VerifyElement.VerifyString(fdsv.FontFamily.Source, "Palatino Linotype");
            VerifyElement.VerifyBool((fdsv.FontSize == 17), true);
            VerifyElement.VerifyColor(((SolidColorBrush)(fdsv.Foreground)).Color, Colors.Gray);
            VerifyElement.VerifyDouble(myPanel.LineHeight, 25);
            VerifyElement.VerifyBool(myPanel.Typography.StandardLigatures, true);
            //VerifyElement.VerifyInt((int)myPanel.TextTrimming, (int)TextTrimming.CharacterEllipsis);
            
            CoreLogger.LogStatus("Root Textpanel OK ...");

            //Verify the first element
            CoreLogger.LogStatus("Verifying first element ...");

            Paragraph e1 = (Paragraph)LogicalTreeHelper.FindLogicalNode(uie, "E1");

            VerifyElement.VerifyBool((null == e1), false);
            //VerifyElement.VerifyBool(e1.TextDecorations.Equals(System.Windows.TextDecorations.Underline), true).
            //VerifyElement.VerifyInt((int)e1.TextWrapping, (int)TextWrapping.NoWrap);
            VerifyElement.VerifyString((new TextRange(e1.ContentStart, e1.ContentEnd)).Text, "WONDERWALL");
            //Verify the second element
            CoreLogger.LogStatus("Verifying the second element ...");

            Paragraph e2 = (Paragraph)LogicalTreeHelper.FindLogicalNode(uie, "E2");

            VerifyElement.VerifyBool((null == e2), false);

            //VerifyElement.VerifyDouble(e2.TextIndent, 25).
            VerifyElement.VerifyString((new TextRange(e2.ContentStart, e2.ContentEnd)).Text, "-by oasis");

            Paragraph e3 = (Paragraph)LogicalTreeHelper.FindLogicalNode(uie, "E3");
            Paragraph e4 = (Paragraph)LogicalTreeHelper.FindLogicalNode(uie, "E4");
            Section e5 = (Section)LogicalTreeHelper.FindLogicalNode(uie, "E5");
            Inline e6 = (Inline)LogicalTreeHelper.FindLogicalNode(uie, "E6");
            Paragraph e7 = (Paragraph)LogicalTreeHelper.FindLogicalNode(uie, "E7");
            
            if(null == e3 || null == e4 || null == e5 || null == e6 || null == e7) 
            {
                throw new Microsoft.Test.TestValidationException("Some text component missed");
            }
            VerifyElement.VerifyInt((int)e6.Typography.Variants, (int)FontVariants.Superscript);
        }

        /// <summary>
        ///  Verifier for LayoutTextPanelSimple.xaml
        /// </summary>
        public static void LayoutTextPanelSimpleVerify(UIElement uie)
        {
            CoreLogger.LogStatus("Inside LayoutXamlVerifiers.LayoutTextPanelSimpleVerify()...");

			FlowDocumentScrollViewer fdsv = uie as FlowDocumentScrollViewer;
            FlowDocument myPanel = fdsv.Document;

            if (null == myPanel)
            {
                throw new Microsoft.Test.TestValidationException("Should be TextPanel");
            }
            CoreLogger.LogStatus("Verifying root element ...");
            VerifyElement.VerifyInt((int)(myPanel.Typography.Capitals), (int)(FontCapitals.Unicase));
            VerifyElement.VerifyString(fdsv.FontFamily.Source, "Palatino Linotype");
            VerifyElement.VerifyBool((fdsv.FontSize == 17), true);
            //VerifyElement.VerifyInt((int)myPanel.FontStretch, (int)FontStretches.Normal).
            //VerifyElement.VerifyInt((int)myPanel.FontStyle, (int)FontStyles.Oblique).
            //VerifyElement.VerifyInt((int)myPanel.FontWeight, (int)FontWeights.Bold).
            VerifyElement.VerifyColor(((SolidColorBrush)(fdsv.Foreground)).Color, Colors.Gray);
            VerifyElement.VerifyDouble(myPanel.LineHeight, 25);
            VerifyElement.VerifyBool(myPanel.Typography.StandardLigatures, true);
            //VerifyElement.VerifyInt((int)myPanel.TextTrimming, (int)TextTrimming.CharacterEllipsis);
            
            CoreLogger.LogStatus("Root Textpanel OK ...");

            //Verify the first element
            CoreLogger.LogStatus("Verifying first element ...");

            Paragraph e1 = (Paragraph)LogicalTreeHelper.FindLogicalNode(uie, "E1");
            
            VerifyElement.VerifyBool((null == e1), false);
            //VerifyElement.VerifyInt((int)e1.TextWrapping, (int)TextWrapping.NoWrap);
        }
    }
}

