// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for hosting UIElements in FlowDocument.
//

using System;
using System.Windows;                       // UIElement
using System.Windows.Controls;              // FlowDocumentPageViewer
using System.Windows.Controls.Primitives;   // IScrollInfo
using System.Windows.Documents;             // FlowDocument
using System.Windows.Media;                 // Visual
using System.Windows.Shapes;                // Rectangle

namespace DRT
{
#if TODO
<Border xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            Name="root" Background="cornsilk">
    <Border.Resources>
        <DataTemplate DataType="Magazine">
            <StackPanel>
                <TextBlock Width="100" Text="{Binding XPath=@Title}" />
                <TextBlock Width="70" Text="{Binding XPath=@Published}" />
            </StackPanel>
        </DataTemplate>
        <XmlDataProvider x:Key="Library" XPath="/root/*">
          <x:XData>
            <root xmlns="">
                <Magazine Title="Sports Illustrated" Published="Weekly" />
                <Magazine Title="People" Published="Weekly" />
                <Magazine Title="Time" Published="Weekly" />
                <Magazine Title="Wall Street Journal" Published="Daily" />
            </root>
          </x:XData>
        </XmlDataProvider>
    </Border.Resources>

    <FlowDocumentPageViewer>
        <FlowDocument>
            <Paragraph>Testing UIElement hosting with custom data templates.</Paragraph>
            <BlockUIContainer>
                <ListBox Name="lb2" Margin="10,10,10,10" DataContext="{Binding Source={StaticResource Library}}" ItemsSource="{Binding }" />
            </BlockUIContainer>
            <Paragraph>Testing UIElement hosting with custom data templates.</Paragraph>
        </FlowDocument>
    </FlowDocumentPageViewer>

</Border>
#endif

    /// <summary>
    /// Test suite for UIElement hosting.
    /// </summary>
    internal sealed class FlowDocUIHostingTestSuite : LayoutDumpTestSuite
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal FlowDocUIHostingTestSuite() :
            base("FlowDocUIHosting")
        {
            this.Contact = "Microsoft";
        }

        /// <summary>
        /// Returns collection of tests.
        /// </summary>
        protected override DrtTest[] CreateTests()
        {
            // Return the lists of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(NewDocument),                   new DrtTest(DumpCreate),
                new DrtTest(ScrollBlockUIElement),          new DrtTest(DumpAppend),
                new DrtTest(ReplaceBlockUIElement),         new DrtTest(DumpAppend),
                new DrtTest(ReplaceBlockUIContainer),       new DrtTest(DumpAppend),
                new DrtTest(InsertInlineUIContainer),       new DrtTest(DumpAppend),
                new DrtTest(ScrollInlineUIElement),         new DrtTest(DumpAppend),
                new DrtTest(ReplaceInlineUIElement),        new DrtTest(DumpAppend),
                new DrtTest(ReplaceInlineUIContainer),      new DrtTest(DumpAppend),
                new DrtTest(ReplaceWithText),               new DrtTest(DumpAppend),
                new DrtTest(InsertText),                    new DrtTest(DumpAppend),
                new DrtTest(InsertFigure),                  new DrtTest(DumpAppend),
                new DrtTest(ModifyFigure),                  new DrtTest(DumpAppend),
                new DrtTest(GoToFirstPage),                 new DrtTest(DumpAppend),
                new DrtTest(AppendBlockUIContainer),        new DrtTest(DumpAppend),
                new DrtTest(ModifyBlockUIContainer),        new DrtTest(DumpAppend),
                new DrtTest(GoToLastPage),                  new DrtTest(DumpAppend),
                new DrtTest(ReplaceDocument),               new DrtTest(DumpAppend),
                new DrtTest(LoadDataTemplateTest),          new DrtTest(DumpAppend),
                new DrtTest(ReplaceDocument),               new DrtTest(DumpFinalizeAndVerify),
            };
        }

        /// <summary>
        /// Creates FlowDocumentPageViewer with new document attached to it.
        /// </summary>
        private void NewDocument()
        {
            BlockUIContainer blockUI = new BlockUIContainer(CreateListBox());
            _document = new FlowDocument(new Paragraph(new Run("Block UIElement:")));
            _document.Blocks.Add(blockUI);

            _viewer = new FlowDocumentPageViewer();
            _viewer.Document = _document;
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Scroll Block UIElement.
        /// </summary>
        private void ScrollBlockUIElement()
        {
            // Retrive ScrollContentPresenter from MonthCalendar
            ListBox listBox = (ListBox)((BlockUIContainer)_document.Blocks.LastBlock).Child;
            IScrollInfo scroller = (IScrollInfo)DRT.FindVisualByType(typeof(VirtualizingStackPanel), listBox);
            // Simulate UIElement tree invalidation to hit optimization in the LayoutManager.
            Visual parent = (Visual)VisualTreeHelper.GetParent((DependencyObject)scroller);
            while (parent != _viewer)
            {
                if (parent is UIElement)
                {
                    ((UIElement)parent).InvalidateArrange();
                }
                parent = (Visual)VisualTreeHelper.GetParent(parent);
            }
            // Invalidate Arrange for ScrollContentPresenter
            scroller.MouseWheelDown();
        }

        /// <summary>
        /// Replace Block UIElement
        /// </summary>
        private void ReplaceBlockUIElement()
        {
            BlockUIContainer block = (BlockUIContainer)_document.Blocks.LastBlock;
            block.Child = CreateButton();
        }

        /// <summary>
        /// Replace BlockUIContainer
        /// </summary>
        private void ReplaceBlockUIContainer()
        {
            BlockUIContainer block = new BlockUIContainer(CreateRectangle());
            _document.Blocks.Remove(_document.Blocks.LastBlock);
            _document.Blocks.Add(block);
        }

        /// <summary>
        /// Insert InlineUIContainer
        /// </summary>
        private void InsertInlineUIContainer()
        {
            _document.Blocks.Remove(_document.Blocks.LastBlock);
            InlineUIContainer inlineUI = new InlineUIContainer(CreateListBox());
            Paragraph para = new Paragraph(new Run("Inline UIElement:"));
            para.Inlines.Add(inlineUI);
            _document.Blocks.Add(para);
        }

        /// <summary>
        /// Scroll Inline UIElement.
        /// </summary>
        private void ScrollInlineUIElement()
        {
            // Retrive ScrollContentPresenter from MonthCalendar
            ListBox listBox = (ListBox)((InlineUIContainer)((Paragraph)_document.Blocks.LastBlock).Inlines.LastInline).Child;
            IScrollInfo scroller = (IScrollInfo)DRT.FindVisualByType(typeof(VirtualizingStackPanel), listBox);
            // Simulate UIElement tree invalidation to hit optimization in the LayoutManager.
            Visual parent = (Visual)VisualTreeHelper.GetParent((DependencyObject)scroller);
            while (parent != _viewer)
            {
                if (parent is UIElement)
                {
                    ((UIElement)parent).InvalidateArrange();
                }
                parent = (Visual)VisualTreeHelper.GetParent(parent);
            }
            // Invalidate Arrange for ScrollContentPresenter
            scroller.MouseWheelDown();
        }

        /// <summary>
        /// Replace Inline UIElement
        /// </summary>
        private void ReplaceInlineUIElement()
        {
            InlineUIContainer inline = (InlineUIContainer)((Paragraph)_document.Blocks.LastBlock).Inlines.LastInline;
            inline.Child = CreateButton();
        }

        /// <summary>
        /// Replace InlineUIContainer
        /// </summary>
        private void ReplaceInlineUIContainer()
        {
            InlineUIContainer inline = new InlineUIContainer(CreateRectangle());
            ((Paragraph)_document.Blocks.LastBlock).Inlines.Remove(((Paragraph)_document.Blocks.LastBlock).Inlines.LastInline);
            ((Paragraph)_document.Blocks.LastBlock).Inlines.Add(inline);
        }

        /// <summary>
        /// Replace content with text
        /// </summary>
        private void ReplaceWithText()
        {
            Paragraph para;
            _document.Blocks.Clear();

            para = new Paragraph(new Run());
            para.Background = Brushes.Beige;
            ((Run)para.Inlines.FirstInline).Text = GenerateText();
            _document.Blocks.Add(para);

            para = new Paragraph(new Run());
            para.Background = Brushes.LightGreen;
            _document.Blocks.Add(para);

            para = new Paragraph(new Run());
            para.Background = Brushes.LightSkyBlue;
            ((Run)para.Inlines.FirstInline).Text = GenerateText();
            _document.Blocks.Add(para);

            para = new Paragraph(new Run());
            para.Background = Brushes.LightYellow;
            ((Run)para.Inlines.FirstInline).Text = GenerateText();
            _document.Blocks.Add(para);

            para = new Paragraph(new Run());
            para.Background = Brushes.LightCoral;
            ((Run)para.Inlines.FirstInline).Text = GenerateText();
            _document.Blocks.Add(para);
        }

        /// <summary>
        /// Insert more text on the first page of the document.
        /// </summary>
        private void InsertText()
        {
            Paragraph para = (Paragraph)_document.Blocks.FirstBlock.NextBlock;
            ((Run)para.Inlines.FirstInline).Text = GenerateText();
            para.Inlines.Add(new Run(GenerateText()));
            para.Inlines.Add(new Run(GenerateText()));
            para.Inlines.Add(new Run(GenerateText()));
        }

        /// <summary>
        /// Insert Figure on the first page of the document.
        /// </summary>
        private void InsertFigure()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Background = Brushes.LimeGreen;
            textBlock.Text = "Figure";
            textBlock.FontSize = 12;
            BlockUIContainer block = new BlockUIContainer(textBlock);
            Figure figure = new Figure(block);
            figure.VerticalAnchor = FigureVerticalAnchor.ParagraphTop;
            figure.HorizontalAnchor = FigureHorizontalAnchor.ColumnLeft;

            Paragraph para = (Paragraph)_document.Blocks.FirstBlock.NextBlock;
            para.Inlines.InsertAfter(para.Inlines.FirstInline, figure);
        }

        /// <summary>
        /// Modify Figure.
        /// </summary>
        private void ModifyFigure()
        {
            Paragraph para = (Paragraph)_document.Blocks.FirstBlock.NextBlock;
            Figure figure = (Figure)para.Inlines.FirstInline.NextInline;
            TextBlock textBlock = (TextBlock)((BlockUIContainer)figure.Blocks.FirstBlock).Child;
            textBlock.Text = GenerateText();
            textBlock.TextWrapping = TextWrapping.Wrap;
        }

        /// <summary>
        /// Go to first page.
        /// </summary>
        private void GoToFirstPage()
        {
            _viewer.FirstPage();
        }

        /// <summary>
        /// Append BlockUIConainer.
        /// </summary>
        private void AppendBlockUIContainer()
        {
            Border border = new Border();
            border.Width = 100;
            border.Height = 20;
            border.Background = Brushes.MediumBlue;
            BlockUIContainer block = new BlockUIContainer(border);
            _document.Blocks.Add(block);
        }

        /// <summary>
        /// Modify BlockUIConainer.
        /// </summary>
        private void ModifyBlockUIContainer()
        {
            Border border = (Border)((BlockUIContainer)_document.Blocks.LastBlock).Child;
            border.Width = 1000;
            border.Height = 1000;
        }

        /// <summary>
        /// Go to last page.
        /// </summary>
        private void GoToLastPage()
        {
            _viewer.LastPage();
        }

        /// <summary>
        /// Replace existing document with new one.
        /// </summary>
        private void ReplaceDocument()
        {
            _document = new FlowDocument(new Paragraph());
            _viewer.Document = _document;
        }

        /// <summary>
        /// Load test with custom data template for content of hosted UIElement.
        /// </summary>
        private void LoadDataTemplateTest()
        {
            this.Root.Child = (UIElement)LoadFromXaml("UIElementHostingTest1.xaml");
            _viewer = (FlowDocumentPageViewer)DRT.FindVisualByType(typeof(FlowDocumentPageViewer), this.Root.Child);
            _document = (FlowDocument)_viewer.Document;
        }

        /// <summary>
        /// Create ListBox.
        /// </summary>
        private ListBox CreateListBox()
        {
            ListBox listBox = new ListBox();
            listBox.Height = 200;
            for (int i = 0; i < 20; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = "Item #" + i.ToString();
                listBox.Items.Add(textBlock);
            }
            return listBox;
        }

        /// <summary>
        /// Create Button.
        /// </summary>
        private Button CreateButton()
        {
            Button button = new Button();
            button.Content = "This is button!";
            button.FontStyle = FontStyles.Italic;
            button.FontSize = 96;
            return button;
        }

        /// <summary>
        /// Create Rectangle.
        /// </summary>
        private Rectangle CreateRectangle()
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 100;
            rectangle.Height = 30;
            rectangle.Fill = Brushes.RoyalBlue;
            return rectangle;
        }

        /// <summary>
        /// Generate some text.
        /// </summary>
        private string GenerateText()
        {
            string s1 = "For those who love to watch movies, listen to music, and share digital photos, the computer is becoming an entertainment hub. Now you can control all of your digital media from one location - anywhere in your living room.";
            string s2 = "Wireless Optical Desktop 5000: Enjoy better photos with this all-in-one photo editing solution-photo keyboard, mouse and software all-in-one. Includes Microsoft Digital Image Standard 2006, powerful and easy to use software for organizing and managing photos.";
            string s3 = "Wireless Optical Desktop Elite: Get quick access to documents and Web pages, and easily enlarge and edit details with the new Magnifier. The included ergonomic mouse lets you maneuver with ease and accuracy.";
            string s4 = "Remote Keyboard for Windows® XP Media Center Edition: Launch digital media, surf the Web, watch movies, and send instant messages from up to 30 feet away with this wireless keyboard, mouse, and remote.";
            return s1 + s2 + s3 + s4;
        }

        private FlowDocument _document;
        private FlowDocumentPageViewer _viewer;
    }
}
