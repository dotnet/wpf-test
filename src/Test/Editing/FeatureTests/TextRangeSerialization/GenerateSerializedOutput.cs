// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

using Microsoft.Test.Discovery;
using Microsoft.Test.Threading;

using Test.Uis.Loggers;
using Test.Uis.TestTypes;

namespace Microsoft.Test.Editing
{
    /// <summary>
    /// This test case tests generates serialized output in Xaml and XamlPackage format. 
    /// Those files are used by SerializationVersionToleranceTest to perform version tolerance testing of TextRange serialization.
    /// The output files are created in the running directory.
    /// </summary>
    [Test(3, "TextOM", "GenerateSerializedOutput", MethodParameters = "/TestCaseType:GenerateSerializedOutput", SupportFiles = @"FeatureTests\Editing\Serialization.xaml, FeatureTests\Editing\colors.PNG")]    
    public class GenerateSerializedOutput : CustomTestCase
    {
        public override void RunTestCase()
        {
            DoTest(DataFormats.Xaml);
            DoTest(DataFormats.XamlPackage);

            Logger.Current.ReportSuccess();
        }

        private void DoTest(string dataFormat)
        {
            InitializeRichTextBox(dataFormat);
            MainWindow.Content = _rtb;
            DispatcherHelper.DoEvents(5000); // Extra wait for the images to complete render
            TextRange tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            SaveSerializedOutput(tr, dataFormat);
        }

        private void SaveSerializedOutput(TextRange tr, string dataFormat)
        {            
            MemoryStream mStream = new MemoryStream();
            tr.Save(mStream, dataFormat);
            mStream.Seek(0, SeekOrigin.Begin);
            Logger.Current.TestLog.LogFile("SerializedOutput." + dataFormat, mStream);
            mStream.Close();
        }

        private void InitializeRichTextBox(string dataFormat)
        {
            using (FileStream fileStream = new FileStream("Serialization.xaml", FileMode.Open))
            {
                _rtb = (RichTextBox)XamlReader.Load(fileStream);                
            }

            if (_rtb == null)
            {
                throw new ApplicationException("Required input file Serialization.xaml doesnt have proper contents");
            }

            // Add InlineUIContainer            
            Paragraph inlineUIContainerParagraph = new Paragraph(new InlineUIContainer(GetImage()));
            _rtb.Document.Blocks.InsertBefore(_rtb.Document.Blocks.LastBlock, inlineUIContainerParagraph);

            // Add BlockUIContainer            
            BlockUIContainer blockUIContainer = new BlockUIContainer(GetImage());
            _rtb.Document.Blocks.InsertBefore(_rtb.Document.Blocks.LastBlock, blockUIContainer);

            _rtb.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            // Perform round trip with the specified data format
            TextRange tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            MemoryStream mStream = new MemoryStream();
            tr.Save(mStream, dataFormat);
            mStream.Seek(0, SeekOrigin.Begin);
            // Important: Need to create new RichTextBox because we are only testing serialization of RichTextBox contents.
            // The format we are going to use to test version tolerance is:
            // 1. Create RichTextBox by a programmatic constructor
            // 2. Set FontFamily, FontSize and Language to avoid false test failures due to localized runs
            // 3. Round trip (load & save) the contents of RichTextBox.
            _rtb = new RichTextBox();
            _rtb.FontSize = 11;
            _rtb.FontFamily = new System.Windows.Media.FontFamily("Palatino Linotype");
            _rtb.Language = XmlLanguage.GetLanguage(new System.Globalization.CultureInfo("en-US").Name);
            tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            tr.Load(mStream, dataFormat);
            mStream.Close();
        }

        private Image GetImage()
        {
            Image image = new Image();
            image.Height = 50;
            image.Width = 50;
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("colors.png", UriKind.Relative);
            bitmapImage.EndInit();
            image.Source = bitmapImage;

            return image;
        }        

        private RichTextBox _rtb = null;
    }
}