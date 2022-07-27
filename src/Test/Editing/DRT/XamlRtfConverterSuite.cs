// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DRT
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows; 
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    internal class XamlRtfConverterSuite : DrtTestSuite
    {
        #region Constructors

        internal XamlRtfConverterSuite(): base("XamlRtfConverter")
        {
        }

        #endregion Constructors

        #region Public Methods

        // Create XamlRtfConverter testing tree
        private UIElement CreateTree()
        {
            StackPanel stackPanel = new StackPanel();

            _richTextBox = new RichTextBox();
            _richTextBox.Height = 380;
            _richTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            _richTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            stackPanel.Children.Add(_richTextBox);

            _textBox = new TextBox();
            _textBox.Height = 180;
            _textBox.Background = new SolidColorBrush(Color.FromRgb(0xf0, 0xf0, 0xf0));
            _textBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            _textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            stackPanel.Children.Add(_textBox);

            return stackPanel;
        }

        public override DrtTest[] PrepareTests()
        {
            DRT.Show(CreateTree());

            // Return the lists of tests to run against the tree
            return new DrtTest[]{
                new DrtTest(TestSimpleRtfToXaml),
                new DrtTest(VerifySimpleRtfToXaml),
                new DrtTest(TestSimpleXamlToRtf),
                new DrtTest(VerifySimpleXamlToRtf),
            };
        }

        #endregion Public Methods

        #region Private Methods

        // Convert the simple rtf to the xaml content
        private void TestSimpleRtfToXaml()
        {
            string xamlContent = RtfToXamlConvert(@"DrtFiles\Editing\rtfsimple.rtf");

            if (xamlContent == string.Empty || xamlContent.Length == 0)
            {
                DRT.Assert(false, "RtfToXamlConvert is failed with rtfsimple.rtf file. XamlContent is empty\n");
            }
            else
            {
                // Set xaml data into Clipboard then paste into the RichTextBox control
                Clipboard.Clear();
                IDataObject dataObject = new DataObject("Xaml", xamlContent);
                Clipboard.SetDataObject(dataObject);
                _richTextBox.SelectAll();
                _richTextBox.Paste();

                // Set Xaml data on TextBox control
                _textBox.Text = xamlContent;
            }
        }

        // Verify the converting result from rtf to xaml content
        private void VerifySimpleRtfToXaml()
        {
            _richTextBox.SelectAll();
            _richTextBox.Copy();
            string plainText = Clipboard.GetText();

            if (plainText == string.Empty || plainText.Length == 0 || string.Compare(plainText, 0, "This is simple rtf testing.", 0, 27) != 0)
            {
                DRT.Assert(false, "RtfToXamlConvert is failed with rtfsimple.rtf file. \n Result:" + plainText + "\n Expect:This is simple rtf testing. ......\n");
            }

            string xamlResult = string.Empty;
            string xamlResultExpect = string.Empty;

            xamlResult = ReadXamlFileContent(@"DrtFiles\Editing\rtfsimple.rtf.xaml");
            xamlResultExpect = ReadXamlFileContent(@"DrtFiles\Editing\rtfsimple_expect.rtf.xaml");

            if (xamlResult == string.Empty || xamlResult.Length == 0 ||
                xamlResultExpect == string.Empty || xamlResultExpect.Length == 0 ||
                string.Compare(xamlResult, xamlResultExpect) != 0)
            {
                DRT.Assert(false, "RtfToXamlConvert is failed: xamlResult and xamlResultExpection file is different. Please check rtfsimple.rtf.xaml and rtfsimple_expect.rtf.xaml files.\n");
            }
        }

        // Convert the simple xaml to the rtf content
        private void TestSimpleXamlToRtf()
        {
            XamlToRtfConvert(@"DrtFiles\Editing\rtfsimple.rtf.xaml");
        }

        // Verify the converting result from xaml to rtf content
        private void VerifySimpleXamlToRtf()
        {
            string rtfResult = string.Empty;
            string rtfResultExpect = string.Empty;

            rtfResult = ReadXamlFileContent(@"DrtFiles\Editing\rtfsimple.rtf.xaml.rtf");
            rtfResultExpect = ReadXamlFileContent(@"DrtFiles\Editing\rtfsimple_expect.rtf.xaml.rtf");

            if (rtfResult == string.Empty || rtfResult.Length == 0 ||
                rtfResultExpect == string.Empty || rtfResultExpect.Length == 0 ||
                string.Compare(rtfResult, rtfResultExpect) != 0)
            {
                DRT.Assert(false, "XamlToRtfConvert is failed: rtfResult and rtfResultExpection file is different. Please check rtfsimple.rtf.xaml.rtf and rtfsimple_expect.rtf.xaml.rtf files. \n");
            }
        }

        // Read the xaml file then return content as string
        private string ReadXamlFileContent(string fileName)
        {
            string xamlContent = string.Empty;

            try
            {
                using (StreamReader xamlReader = new StreamReader(fileName))
                {
                    xamlContent = xamlReader.ReadToEnd();
                }
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("ERROR:" + fileName + " file is not acessible.\n");
            }

            return xamlContent;
        }

        // Convert rtf content to the xaml content string
        private string RtfToXamlConvert(string rtfFile)
        {
            string xamlContent = string.Empty;

            try
            {
                FileStream fileOpenStream = new FileStream(rtfFile, FileMode.Open, FileAccess.Read);

                if (fileOpenStream.Length > 0)
                {
                    // Converting rtf to xaml
                    xamlContent = XamlRtfConverter.ConvertRtfToXaml(new BinaryReader(fileOpenStream));
                }

                fileOpenStream.Close();

                // Write the xaml output as utf8 file
                UTF8Encoding encoder = new UTF8Encoding();
                int nBytes = encoder.GetByteCount(xamlContent.ToString());
                byte[] bytes = encoder.GetBytes(xamlContent.ToString());
                FileStream fileWriteStream = new FileStream(rtfFile + ".xaml", FileMode.OpenOrCreate, FileAccess.Write);
                fileWriteStream.SetLength(0);
                fileWriteStream.Write(bytes, 0, nBytes);
                fileWriteStream.Close();
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("ERROR:" + rtfFile + " file is not acessible.\n");
            }

            return xamlContent;
        }

        // Convert xaml content to the rtf content string
        private string XamlToRtfConvert(string xamlFile)
        {
            string xamlContent = string.Empty;
            string rtfContent = string.Empty;

            xamlContent = ReadXamlFileContent(xamlFile);

            if (xamlContent == string.Empty || xamlContent.Length == 0)
            {
                DRT.Assert(false, "XamlToRtfConvert is failed: " + xamlFile + " content is empty \n");
            }

            rtfContent = XamlRtfConverter.ConvertXamlToRtf(xamlContent);

            if (rtfContent == string.Empty || rtfContent.Length == 0)
            {
            }
            else
            {
                Encoding inEncoding = Encoding.Unicode;
                Encoding outEncoding = Encoding.GetEncoding(XamlRtfConverter.RtfCodePage);
                byte[] bytesUnicode = inEncoding.GetBytes(rtfContent);
                byte[] bytes1252 = Encoding.Convert(inEncoding, outEncoding, bytesUnicode);

                // Write the rtf output as 1252 file
                FileStream fileWriteStream = new FileStream(xamlFile + ".rtf", FileMode.OpenOrCreate, FileAccess.Write);
                fileWriteStream.SetLength(0);
                fileWriteStream.Write(bytes1252, 0, bytes1252.Length);
                fileWriteStream.Close();
            }

            return rtfContent;
        }

        #endregion Private Methods

        #region Private Fields

        private RichTextBox _richTextBox;
        private TextBox _textBox;

        #endregion Private Fields
    }
}