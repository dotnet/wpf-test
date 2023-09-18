// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 15 $ $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/uis/forms/BVT/DataTransfer/Clipboard/ClipboardTest.cs $")]
namespace DataTransfer
{
    #region Namespance
    using System;

    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Markup;
    using System.IO;
    using System.Drawing;
    using System.Collections;
    using System.Collections.Generic;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Test.Uis.TestTypes;
    using Microsoft.Test.Imaging;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    #endregion Namespance

    /// <summary>
    /// This case Test the basic Functionality of Copy/paste image
    /// </summary>
    [Test(0, "Clipboard", "XamlPackageBVTTest", MethodParameters = "/TestCaseType=XamlPackageBVTTest /Pri=0", Timeout = 240, Keywords = "MicroSuite")]
    [TestOwner("Microsoft"), TestWorkItem("16, 17"), TestTactics("64"), TestBugs("14, 23, 390, 21, 22"), TestLastUpdatedOn("Aug 01,2006")]
    public class XamlPackageBVTTest : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            Log("DoRunCombination - Setup the controls....");
            double size = 200;
            Panel panel = new DockPanel();
            TestElement = panel;
            _box = new RichTextBox();
            _box.Height = size;
            _box.Width = size;
            _sourceWrapper = new UIElementWrapper(_box);
            panel.Children.Add(_box);

            _box = new RichTextBox();
            _box.Height = size;
            _box.Width = size;
            _targetWrapper = new UIElementWrapper(_box);
            DataObject.AddPastingHandler(_targetWrapper.Element, new DataObjectPastingEventHandler(SetPasteFormate));
            panel.Children.Add(_box);

            _box = new RichTextBox();
            _box.Height = size;
            _box.Width = size;
            panel.Children.Add(_box);
            Clipboard.Clear();
            QueueDelegate(SetSourceContent);
        }

        void SetPasteFormate(object obj, DataObjectPastingEventArgs args)
        {
            DataObject dObject = args.DataObject as DataObject;

            string[] formats = dObject.GetFormats();
            for (int i = 0; i < formats.Length; i++)
            {
                if (formats[i] == _pastedFormat)
                {
                    args.FormatToApply = _pastedFormat;
                }
            }
        }

        void SetSourceContent()
        {
            Log("SetSourceContent - Set the image and/or text to the source control ...");
            string source;
            if (_inmageFormat.Contains("\\") || _inmageFormat.Contains("/"))
            {
                //load the image fine from a web server
                source = "Source=" + "\"" + _inmageFormat + "\"";
            }
            else
            {
                //load the image from local file.
                source = "Source=" + "\"" + XamlUtils.CurrentDirectory + "/" + _inmageFormat + "\"";
            }
            string xaml = _contentOrder.Replace("<Image />", XamlPackageTestData.ImageXaml);
            xaml = xaml.Replace("Source", source);

            Log("Xaml[" + xaml + "]");

            _paragraph = XamlUtils.ParseToObject(xaml) as Paragraph;
            BlockCollection blocks = ((RichTextBox)_sourceWrapper.Element).Document.Blocks;
            blocks.Clear();
            blocks.Add(_paragraph);
            System.Threading.Thread.Sleep(300);
            QueueDelegate(TakeSourceImage);
        }

        void TakeSourceImage()
        {
            Log("TakeSourceImage - Take the image of the source control...");
            ((RichTextBox)_sourceWrapper.Element).Focus();
            _sourceImage = BitmapCapture.CreateBitmapFromElement(_sourceWrapper.Element);
            _sourceWrapper.SelectAll();
            QueueDelegate(PasteMethod);
        }

        private void PasteMethod()
        {
            Clipboard.Clear();
            //Here we copy twice because the clipboard tends to not copy content after multiple copy operations
            //So in case one copy is missed, the second does put the content in the clicpboard.
            ((RichTextBox)_sourceWrapper.Element).Copy();
            ((RichTextBox)_sourceWrapper.Element).Copy();
            ((RichTextBox)_targetWrapper.Element).SelectAll();
            ((RichTextBox)_targetWrapper.Element).Paste();
            QueueDelegate(TakeTargetImage);
        }

        void TakeTargetImage()
        {
            Log("TakeTargetImage - take the image of the target RichTextbox ...");
            _targetImage = BitmapCapture.CreateBitmapFromElement(_targetWrapper.Element);

            VerifyImage();
            QueueDelegate(VerifyingTextTree);
        }

        private void VerifyingTextTree()
        {
            VerifyTextTree();
            QueueDelegate(VerifyingProperties);
        }

        private void VerifyingProperties()
        {
            if (_pastedFormat != DataFormats.Rtf)
            {
                VerifyProperties(_targetWrapper.Element);
            }
            QueueDelegate(GeneralVerifications);
        }

        private void GeneralVerifications()
        {
            //make sure that there is an image in the target.
            VerifyImageInCollection();

            //Ohter verifications
            OtherVerifications();

            //Clear the source and target controls
            ((RichTextBox)_sourceWrapper.Element).Document.Blocks.Clear();
            _targetWrapper.Clear();

            QueueDelegate(AddInline);
        }

        private void AddInline()
        {
            InlineUIContainer inlinec = new InlineUIContainer(CreateImageControlFromBitmap());
            ((RichTextBox)_sourceWrapper.Element).Document.Blocks.Add(new Paragraph(inlinec));
            _box.Focus();
            QueueDelegate(TakeOnFlyImage);
        }

        void VerifyMultipleImagesPaste()
        {
            _box.Document.Blocks.Clear();

            int imageCount;
            _box.Paste();
            _box.Paste();
            TextRange range = new TextRange(_box.Document.ContentStart, _box.Document.ContentEnd);
            imageCount = TextOMUtils.EmbeddedObjectCountInRange(range, typeof(System.Windows.Controls.Image));
            Verifier.Verify(imageCount >= 2, "Failed - image Count expected[2], Actual[" + imageCount + "]");
        }

        void TakeOnFlyImage()
        {
            Log("TakeOnFlyImage - take the clip of the on fly image...");

            _sourceImage = Microsoft.Test.Imaging.BitmapCapture.CreateBitmapFromElement(_sourceWrapper.Element);

            _sourceWrapper.SelectAll();
            ((RichTextBox)_sourceWrapper.Element).Copy();

            _targetWrapper.SelectAll();
            ((RichTextBox)_targetWrapper.Element).Document.Blocks.Clear();
            ((RichTextBox)_targetWrapper.Element).Paste();

            QueueDelegate(VerifyPastedOnFlyImage);
        }

        void VerifyPastedOnFlyImage()
        {
            Log("VerifyPastedOnFlyImage - Verify if the on fly image is pasted...");
            _targetImage = Microsoft.Test.Imaging.BitmapCapture.CreateBitmapFromElement(_targetWrapper.Element);

            VerifyImage();

            QueueDelegate(NextCombination);
        }

        void verifyClipboardFormats()
        {
        }
        
        void VerifyTextTree()
        {
            TextPointer sourcePointer, targetPointer, sourceEnd;
            sourcePointer = _sourceWrapper.Start.GetInsertionPosition(LogicalDirection.Forward);
            targetPointer = _targetWrapper.Start.GetInsertionPosition(LogicalDirection.Forward);
            sourceEnd = sourcePointer.DocumentEnd;

            if (_pastedFormat == DataFormats.Rtf)
            {
                //an extra span is added in rtf
                while (sourcePointer.CompareTo(sourceEnd) != 0)
                {
                    Verifier.Verify(sourcePointer.GetPointerContext(LogicalDirection.Forward) == targetPointer.GetPointerContext(LogicalDirection.Forward),
                        "Failed: TextPointerContext won't match!");
                    sourcePointer = sourcePointer.GetNextContextPosition(LogicalDirection.Forward);
                    targetPointer = targetPointer.GetNextContextPosition(LogicalDirection.Forward);
                    while (targetPointer.Parent.ToString() != sourcePointer.Parent.ToString())
                    {
                        targetPointer = targetPointer.GetNextContextPosition(LogicalDirection.Forward);
                    }
                }

            }
            else
            {
                while (sourcePointer.CompareTo(sourceEnd) != 0)
                {
                    Verifier.Verify(sourcePointer.GetPointerContext(LogicalDirection.Forward) == targetPointer.GetPointerContext(LogicalDirection.Forward),
                        "Failed: TextPointerContext won't match!");
                    sourcePointer = sourcePointer.GetNextContextPosition(LogicalDirection.Forward);
                    targetPointer = targetPointer.GetNextContextPosition(LogicalDirection.Forward);

                }
            }

            //targetPointer must to go the end
            Verifier.Verify(0 == targetPointer.CompareTo(targetPointer.DocumentEnd),
                "Failed: targetPointer is not at the Document end!");
        }

        void VerifyImageInCollection()
        {
            int images = 0;
            foreach (Inline inline in _paragraph.Inlines)
            {

                if (inline is InlineUIContainer && ((InlineUIContainer)inline).Child is System.Windows.Controls.Image)
                {
                    images++;
                }
            }
            Verifier.Verify(images >= 1, "Failed: wrong image in the source target!");
        }

        void VerifyImage()
        {
            ComparisonCriteria criteria;
            ComparisonOperation operation;
            ComparisonResult result;

            criteria = new ComparisonCriteria();
            criteria.MaxColorDistance = 0.05f;

            operation = new ComparisonOperation();
            operation.Criteria = criteria;
            operation.MasterImage = _sourceImage;
            operation.SampleImage = _targetImage;
            result = operation.Execute();
            //in case of rtf the opacity and gradient is not added - hence image compare fails.
            if ((!result.CriteriaMet) && (_pastedFormat != DataFormats.Rtf.ToString()))
            {
                Logger.Current.LogImage(_sourceImage, "sourceImage");
                Logger.Current.LogImage(_targetImage, "targetImage");
                Bitmap _differences;
                ComparisonOperationUtils.AreBitmapsEqual(_sourceImage, _targetImage, out _differences);
                Logger.Current.LogImage(_differences, "diffImage");
                Verifier.Verify(false, "Failed, Image comparing failed!");
            }
        }

        void VerifyProperties(UIElement element)
        {
            string xaml = XamlWriter.Save(element);
            string[] array = XamlPackageTestData.TestedProperties;
            foreach (string str in array)
            {
                Verifier.Verify(xaml.Contains(str), "Failed: Expected[" + str + "] to be in the xaml!");
            }
        }

        void OtherVerifications()
        {
            switch (_extraCounter)
            {
                case 0:
                    VerifyMultipleImagesPaste();
                    break;
                case 1:
                    //When Regression_Bug21 is fixed, enable the following line.
                    //CorruptedXamlPackage();
                    break;
                case 2:
                    //when Regression_Bug22 is fixed, enabled the following line.
                    //EmptyPackageOnClipboard();
                    break;
                case 3:
                    EmptyPackageOnClipboardFlush();
                    break;
                default:
                    break;
            }
            _extraCounter++;
        }

        void EmptyPackageOnClipboard()
        {
            DataObject dobject;
            MemoryStream stream;
            stream = new MemoryStream();
            dobject = new DataObject(DataFormats.XamlPackage, stream);
            Clipboard.Clear();
            Clipboard.SetDataObject(dobject);

            //We should not crash here.
            QueueDelegate(new SimpleHandler(_box.Paste));
        }
        void EmptyPackageOnClipboardFlush()
        {
            DataObject dobject;
            MemoryStream stream;
            stream = new MemoryStream();
            dobject = new DataObject(DataFormats.XamlPackage, stream);
            Clipboard.Clear();
            Clipboard.SetDataObject(dobject, true);

            //We should not crash here.
            QueueDelegate(new SimpleHandler(_box.Paste));
        }

        void CorruptedXamlPackage()
        {
            DataObject dobject;
            MemoryStream mStream;
            StreamWriter sWriter;
            mStream = Clipboard.GetData(DataFormats.XamlPackage) as MemoryStream;
            sWriter = new StreamWriter(mStream);
            sWriter.WriteLine("junck");
            sWriter.Flush();
            dobject = new DataObject(DataFormats.XamlPackage, mStream);
            Clipboard.Clear();
            Clipboard.SetDataObject(dobject);

            //we should not crash when paste corrupted xamlpackage.
            QueueDelegate(new SimpleHandler(_box.Paste));
        }

        System.Windows.Controls.Image CreateImageControlFromBitmap()
        {
            Stream bitmapstrem = new MemoryStream();

            //save the source image into the memory stream.
            _sourceImage.Save(bitmapstrem, System.Drawing.Imaging.ImageFormat.Png);

            System.Windows.Media.Imaging.BitmapDecoder bd = System.Windows.Media.Imaging.BitmapDecoder.Create(
                bitmapstrem, System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat, System.Windows.Media.Imaging.BitmapCacheOption.OnLoad);
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = bd.Frames[0];
            return image;
        }

        #endregion Main flow.

        #region Private fields.

        private UIElementWrapper _sourceWrapper;
        private UIElementWrapper _targetWrapper;
        private string _pastedFormat = string.Empty;
        private string _contentOrder = string.Empty;
        private string _inmageFormat = string.Empty;
        private Bitmap _sourceImage;
        private Bitmap _targetImage;
        private Paragraph _paragraph;
        private RichTextBox _box;
        int _extraCounter = 0;


        #endregion Private fields.
    }

    /// <summary>
    /// data for xamlpackage test.
    /// </summary>
    public class XamlPackageTestData
    {
        /// <summary>
        /// return an array of imagefile names.
        /// </summary>
        public static object[] ImageFiles
        {
            get
            {
                return new object[] {
                    "XamlPackageImage.bmp",
                    "XamlPackageImage.jpg",
                    "XamlPackageImage.gif",
                    "XamlPackageImage.tif",
                   // "http://wpfapps/testscratch/Editing/TestCaseData/XamlpackageImage.PNG",
                };
            }
        }

        /// <summary>
        /// return an array of xaml content for image.
        /// </summary>
        public static object[] XamlCombinations
        {
            get
            {
                return new object[] {
                    "<Paragraph xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xml:space='preserve' ><Run>a</Run><Image /></Paragraph>",
                    "<Paragraph xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xml:space='preserve'><Image /><Run>a</Run></Paragraph>",
                    "<Paragraph xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xml:space='preserve'><Run>a</Run><Image /><Run>a</Run></Paragraph>",
                    "<Paragraph xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xml:space='preserve'><Image /></Paragraph>",
                    "<Paragraph xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xml:space='preserve'><Image /><Image /></Paragraph>", 
                };
            }
        }

        /// <summary>
        /// return the image xaml for testing 
        /// </summary>
        public static string ImageXaml
        {
            get
            {
                string xaml = "<Image  Opacity=\"0.9\" Height=\"100\" Width=\"100\" Source RenderTransformOrigin=\"0.5, 0.5\" SnapsToDevicePixels=\"True\" ToolTip=\"ClassicStyles\">" +
                                    "<Image.OpacityMask>" +
                                        "<RadialGradientBrush>" +
                                            "<RadialGradientBrush.GradientStops>" +
                                                "<GradientStopCollection>" +
                                                    "<GradientStop Color=\"Green\" Offset=\"0\" />" +
                                                    "<GradientStop Color=\"Transparent\" Offset=\"1\" />" +
                                                "</GradientStopCollection>" +
                                            "</RadialGradientBrush.GradientStops>" +
                                        "</RadialGradientBrush>" +
                                    "</Image.OpacityMask>" +
                                    "<Image.LayoutTransform>" +
                                    "<RotateTransform Angle=\"10\" />" +
                                    "</Image.LayoutTransform>" +
                                    "<Image.RenderTransform>" +
                                        "<RotateTransform Angle=\"30\" />" +
                                    "</Image.RenderTransform>" +
                                    "<Image.ContextMenu>" +
                                        "<ContextMenu><MenuItem Header=\"Item 1\" /></ContextMenu>" +
                                    "</Image.ContextMenu>" +
                              "</Image>";
                return xaml;
            }
        }

        /// <summary>
        /// return an array of interesting properties
        /// </summary>
        public static string[] TestedProperties
        {
            get
            {
                string[] properties ={  "<Image.OpacityMask>", 
                                    "<RadialGradientBrush>",
                                    "<RadialGradientBrush.GradientStops>",
                                    "<GradientStop Color=\"#FF008000\" Offset=\"0\" />",
                                    "<GradientStop Color=\"#00FFFFFF\" Offset=\"1\" />",
                                    "<RotateTransform Angle=\"10\" />",
                                    "<Image.RenderTransform>",
                                    "<RotateTransform Angle=\"30\" />",
                                    "Opacity=\"0.9\"",
                                    "RenderTransformOrigin=\"0.5,0.5\"",
                                    "SnapsToDevicePixels=\"True\"",
                                    "ToolTip=\"ClassicStyles\"",
                                 };

                return properties;
            }
        }

        /// <summary>
        /// return a array of data formats for application to paste.
        /// </summary>
        public static object[] PasteFormats
        {
            get
            {
                return new object[] {
                    //Regression_Bug23 is fixed, enable to XamlPackage formate.
                    DataFormats.XamlPackage, 
                    DataFormats.Rtf,
                };
            }
        }
    }
}
