// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*************************************************
 *  This file test:
 *  1. DataObject()     - take no parameter
 *  2. GetForamts()     - returns string array
 *  3. GetData(string)  - GetData from string array of GetFormats - all returns "System.String,UnicodeText,Text"
 *  4. GetDataPresent(string) - return bool value
 *  5. SetData(object)  - with 21 different types of DataFormats (Bitmap, Text, Rtf, ...) see DataFormats.cs from dev tree
 *  Command Line: exe.exe /TestCaseType=DataObjectAPI /SetDataParam=obj /SetDataFormatsType=bitmap  /ExpectedGetFormat=sut
 *  SetDataParam:       obj
 *  SetDataFormatsType: bitmap, commaseparatedvalue, ... all the DataFormat type in lower_case
 *  ExpectedGetFormat:  sut
 *
 * ************************************************/
[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 16 $ $Source: //depot/private/wcp_dev_platform/windowstest/client/wcptests/uis/forms/BVT/DataTransfer/DataObject/DataObjectAPI.cs $")]

namespace DataTransfer
{
    #region Namespaces.

    using System.Windows.Media.Imaging;
    using Shapes = System.Windows.Shapes;
    using System;
    using System.Drawing;
    using System.IO;    
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Test.Uis.TestTypes;
    using IComDataObject = System.Runtime.InteropServices.ComTypes;
    using Test.Uis.Data;
    using System.ComponentModel;
    using Microsoft.Test.Imaging;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.

    /// <summary>
    /// This class test the DataObjectPasteEventArgs class.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("28"), TestBugs("359"), TestWorkItem("")]
    public class DataObjectPastingEventArgsAPITest : CombinedTestCase
    {
        /// <summary>start to run the test scenario</summary>
        public override void RunTestCase()
        {
            DataObjectPastingEventArgs args;
            DataObject dobj; 
            Log("Test the Constructor of DataObjectPastingEventArgs...");
            //Invalid Parameter.
            try
            {
                //null data
                new DataObjectPastingEventArgs(null, true, DataFormats.Text);
                throw new Exception("Did not get the expected Exception for null data value!");
            }
            catch (ArgumentNullException e)
            {
                Log("Exception message[" + e.Message + "]");
            }

            try
            {
                //null format
                new DataObjectPastingEventArgs(new DataObject("abc"), true, null);
                throw new Exception("Did not get the expected Exception for null format!");
            }
            catch (ArgumentNullException e)
            {
                Log("Exception message[" + e.Message + "]");
            }

            try
            {
                //no existing format
                new DataObjectPastingEventArgs(new DataObject("abc"), true, "junckformat");
                throw new Exception("Did not get the expected Exception for non existing format!");
            }
            catch (ArgumentException e)
            {
                Log("Exception message[" + e.Message + "]");
            }
            try
            {
                //no existing format
                new DataObjectPastingEventArgs(new DataObject("abc"), true, string.Empty);
                throw new Exception("Did not get the expected Exception for non existing format!");
            }
            catch (ArgumentException e)
            {
                Log("Exception message[" + e.Message + "]");
            }
            dobj = new DataObject("abc");
            args = new DataObjectPastingEventArgs(dobj, true, DataFormats.Text);
            Verifier.Verify(args.IsDragDrop, "IsDragDrop value is changed! Expected[true], actual[false]!");
            Verifier.Verify(!args.CommandCancelled, "default value of CommandCancelled should be false!");

            Log("Test CancelCommand method...");
            //CommandCancelled
            args.CancelCommand();
            Verifier.Verify(args.CommandCancelled, "true value expected after calling CommandCancelled()method!");

            Verifier.Verify(args.FormatToApply == DataFormats.Text, "Expected FormattoApply[" + DataFormats.Text + "], actaul[" + args.FormatToApply + "]");

            Log("Test FormatToApply property...");
            args.FormatToApply = DataFormats.UnicodeText;
            Verifier.Verify(args.FormatToApply == DataFormats.UnicodeText, "Expected FormattoApply[" + DataFormats.UnicodeText + "], actaul[" + args.FormatToApply + "]");

            try
            {
                args.FormatToApply = "NonExistingFormat";
                throw new Exception("Expected ArgumentException!");
            }
            catch (ArgumentException e)
            {
                Log("Exception message[" + e.Message + "]");
            }
            try
            {
                args.FormatToApply = null;
                throw new Exception("Expected ArgumentNullException!");
            }
            catch (ArgumentException e)
            {
                Log("Exception message[" + e.Message + "]");
            }

            Log("Test DataObject property...");
            Verifier.Verify(dobj == args.DataObject, "DataObject return is not the one set form Construcotr!");
            try
            {
                args.DataObject = null;
                throw new Exception("Expect ArgumentNullException!");
            }
            catch (ArgumentNullException e)
            {
                Log("Exception message[" + e.Message + "]");
            }
            try
            {
                args.DataObject = new DataObject();
                throw new Exception("Expect ArgumentException!");
            }
            catch (ArgumentException e)
            {
                Log("Exception message[" + e.Message + "]");
            }

            dobj = new DataObject("SetDataObject");
            
            args.DataObject = dobj;
            Verifier.Verify(args.FormatToApply == DataFormats.StringFormat, "Expected format[" + DataFormats.StringFormat + "], actual[" + args.FormatToApply + "]");
           
            EndTest();
        }
    }
    /// <summary>
    /// Performs data-driven tests on the DataObject API.
    /// </summary>
    [Test(2, "DataObject", "DataObjectAPI1", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=enhancedmetafile")]
    [Test(2, "DataObject", "DataObjectAPI2", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=filedrop")]
    [Test(2, "DataObject", "DataObjectAPI3", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=html")]
    [Test(2, "DataObject", "DataObjectAPI4", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=locale")]
    [Test(2, "DataObject", "DataObjectAPI5", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=metafilepicture")]
    [Test(2, "DataObject", "DataObjectAPI6", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=oemtext")]
    [Test(2, "DataObject", "DataObjectAPI7", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=palette")]
    [Test(2, "DataObject", "DataObjectAPI8", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=pendata")]
    [Test(2, "DataObject", "DataObjectAPI9", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=riff")]
    [Test(2, "DataObject", "DataObjectAPI10", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=rtf")]
    [Test(2, "DataObject", "DataObjectAPI11", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=serializable")]
    [Test(2, "DataObject", "DataObjectAPI12", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=stringformat")]
    [Test(2, "DataObject", "DataObjectAPI13", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=symboliclink")]
    [Test(2, "DataObject", "DataObjectAPI14", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=text")]
    [Test(2, "DataObject", "DataObjectAPI15", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=tiff")]
    [Test(2, "DataObject", "DataObjectAPI16", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=unicodetext")]
    [Test(2, "DataObject", "DataObjectAPI17", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=waveaudio")]
    [Test(2, "DataObject", "DataObjectAPI18", MethodParameters = "/TestCaseType=DataObjectAPI /SetDataParam=obj /ExpectedGetFormat=sut /SetDataFormatsType=xml")]
    [TestOwner("Microsoft"), TestTactics("28,149,150,151,152,153,153,154,155,156,157,158,159,160,161,162,163,164,165"), TestBugs(""), TestWorkItem("")]
    public class DataObjectAPI : CombinedTestCase
    {
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            GetDataInvalidTest();
            SetDataInvalidTest();
            EndTest();
        }

        void SetDataInvalidTest()
        {
            Log("Invalid test for DataObject.SetData()...");
            DataObject dobj = new DataObject();
            string[] formats = new string[3];
            object[] objs = new object[2];
            Type[] types = new Type[2];

            types[0] = null;
            types[1] = typeof(System.String);


            formats[0] = null;
            formats[1] = string.Empty;
            formats[2] = DataFormats.Text;

            objs[0] = null;
            objs[1] = "abc";

            //those loop makes the combinations of invalid input. 
            //test DataObject.SetData(string fromat, object dagta) & DataObject.SetData(string fromat, object dagta, bool autocovert)
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        try
                        {
                            Log("i=" + i.ToString() + ", j=" + j.ToString() + ", k=" + k.ToString());
                            if (k == 0)
                            {
                                dobj.SetData(formats[i], objs[j]);
                            }
                            else
                            {
                                dobj.SetData(formats[i], objs[j], true);
                            }
                            //Note: i ==2 && j==1 is a valid combination.  
                            if (!(i == 2 && j == 1))
                            {
                                throw new Exception("No invalid input value exception is thrown");
                            }
                        }
                        catch (Exception e)
                        {
                            if (!(e is ArgumentNullException || e is ArgumentException))
                            {
                                throw new Exception("Did not get the expected exception!");
                            }
                        }
                    }
                }
            }

            //Test Dataobject.SetData(Type format, object data)
            for (int m = 0; m < 2; m++)
            {
                for (int n = 0; n < 2; n++)
                {
                    try
                    {
                        Log("m=" + m.ToString() + ", n=" + n.ToString());
                        dobj.SetData(types[m], objs[n]);
                        if (!(m == 1 && n == 1))
                        {
                            throw new Exception("No invalid input value exception is thrown");
                        }
                    }
                    catch (Exception e)
                    {
                        if (!(e is ArgumentNullException || e is ArgumentException))
                        {
                            throw new Exception("Did not get the expected exception!");
                        }
                    }
                }
            }
        }

        void GetDataInvalidTest()
        {
            Log("Invalid test for DataObject.GetData()...");
            DataObject DO = new DataObject();

            try
            {
                DO.GetData((string)null);
                throw new Exception("DataObject.GetData accepts null string.");
            }
            catch (ArgumentNullException e)
            {
                Logger.Current.Log(e.Message);
                Logger.Current.Log("DataObject.GetData rejects null string");
            }
            //DataObject.GetData(Type format)
            //null
            try
            {
                DO.GetData((Type)null);
                throw new Exception("DataObject.GetData accepts null string as type.");
            }
            catch (ArgumentNullException e)
            {
                Logger.Current.Log(e.Message);
                Logger.Current.Log("DataObject.GetData rejects null string as type");
            }
            //DataObject.GetData(string)
            //empty string.
            try
            {
                DO.GetData(string.Empty);
                throw new Exception("DataObject.GetData accepts string.Empty.");
            }
            catch (ArgumentException e)
            {
                Logger.Current.Log(e.Message);
                Logger.Current.Log("DataObject.GetData rejects string.Empty");
            }
            //DataObject.GetDataPresent(string format)
            try
            {
                //null value
                DO.GetDataPresent((string)null);
                throw new Exception("DataObject.GetDataPresent accepts null!");
            }
            catch (ArgumentNullException e)
            {
                Logger.Current.Log(e.Message);
                Logger.Current.Log("DataObject.GetDataPresent reject null value!");
            }
            //DataObject.GetDataPresent(string format)
            try
            {
                //sring.empty
                DO.GetDataPresent(string.Empty);
                throw new Exception("DataObject.GetDataPresent accepts string.Empty!");
            }
            catch (ArgumentException e)
            {
                Logger.Current.Log(e.Message);
                Logger.Current.Log("DataObject.GetDataPresent reject string.Empty!");
            }
            //DataObject.GetDataPresent(string format, AutoConvert)
            try
            {
                //null value
                DO.GetDataPresent(null, true);
                throw new Exception("DataObject.GetDataPresent accepts null!");
            }
            catch (ArgumentNullException e)
            {
                Logger.Current.Log(e.Message);
                Logger.Current.Log("DataObject.GetDataPresent reject null value!");
            }
            //DataObject.GetDataPresent(string format, AutoConvert)
            try
            {
                //sring.empty
                DO.GetDataPresent(string.Empty, true);
                throw new Exception("DataObject.GetDataPresent accepts string.Empty!");
            }
            catch (ArgumentException e)
            {
                Logger.Current.Log(e.Message);
                Logger.Current.Log("DataObject.GetDataPresent reject string.Empty!");
            }
        }
    }

    /// <summary>
    /// Verifies that XML text is copied correctly.
    /// </summary>
    [Test(0, "DataObject", "DataObjectReproRegression_Bug360", MethodParameters = "/TestCaseType=DataObjectReproRegression_Bug360")]
    [TestOwner("Microsoft"), TestTactics("176"), TestBugs("360")]
    public class DataObjectReproRegression_Bug360 : Test.Uis.TextEditing.TextBoxTestCase
    {
        #region Main flow.
        private const string ExpectedText = "-\xFF77-";

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Log("Cutting and pasting expected text: [" + ExpectedText + "]");
            TestTextBox.Text = ExpectedText;
            MouseInput.MouseClick(TestTextBox);
            KeyboardInput.TypeString("+{HOME}^x");
            QueueDelegate(DoPaste);
        }

        private void DoPaste()
        {
            Verifier.Verify((Clipboard.GetDataObject()).GetData(DataFormats.UnicodeText).ToString() == ExpectedText, "Clipboard match." + (Clipboard.GetDataObject()).GetData(DataFormats.UnicodeText).ToString(), true);
            KeyboardInput.TypeString("^v");
            QueueDelegate(CheckText);
        }

        private void CheckText()
        {
            Log("Expected text: [" + ExpectedText + "]");
            Log("Existing text: [" + TestTextBox.Text + "]");
            Verifier.Verify(TestTextBox.Text == ExpectedText, "Texts match.", true);
            Logger.Current.ReportSuccess();
        }
        #endregion Main flow.
    }

    /// <summary>DataObject APIs testing</summary>
    [Test(0, "DataObject", "DataObjectAPITest1", MethodParameters = "/TestCaseType=DataObjectAPITest /caseIndex=1")]
    [Test(0, "DataObject", "DataObjectAPITest2", MethodParameters = "/TestCaseType=DataObjectAPITest /caseIndex=3")]
    [Test(0, "DataObject", "DataObjectAPITest3", MethodParameters = "/TestCaseType=DataObjectAPITest /caseIndex=4")]
    [Test(0, "DataObject", "DataObjectAPITest4", MethodParameters = "/TestCaseType=DataObjectAPITest /caseIndex=5")]
    [Test(0, "DataObject", "DataObjectAPITest5", MethodParameters = "/TestCaseType=DataObjectAPITest /caseIndex=8")]
    [Test(3, "DataObject", "DataObjectAPITest6", MethodParameters = "/TestCaseType=DataObjectAPITest /caseIndex=0")]
    [Test(2, "DataObject", "DataObjectAPITest7", MethodParameters = "/TestCaseType=DataObjectAPITest /caseIndex=2")]
    [Test(2, "DataObject", "DataObjectAPITest8", MethodParameters = "/TestCaseType=DataObjectAPITest /caseIndex=6")]
    [Test(2, "DataObject", "DataObjectAPITest9", MethodParameters = "/TestCaseType=DataObjectAPITest /caseIndex=7")]
    [Test(2, "DataObject", "DataObjectAPITest10", MethodParameters = "/TestCaseType=DataObjectAPITest /caseIndex=8")]
    [TestOwner("Microsoft"), TestTactics("166, 167, 168, 169, 170, 171, 172, 173, 174, 175"), TestBugs("361, 354, 362")]
    [WindowlessTest(true)]
    public class DataObjectAPITest : CustomTestCase
    {
        #region Test case data.
        delegate void CompareCallback(object master, object sample);
        private int _caseIndex; 

        struct TestCaseData
        {
            public string DataFormatName;
            public object DataContent;
            public CompareCallback DataVerifier;

            public TestCaseData(string dataFormatName, object dataContent, CompareCallback dataVerifier)
            {
                this.DataContent = dataContent;
                this.DataFormatName = dataFormatName;
                this.DataVerifier = dataVerifier;
            }

            public static TestCaseData[] Cases = new TestCaseData[] {
                new TestCaseData(DataFormats.Bitmap, new Bitmap("Colors.png"), new CompareCallback(BitmapVerifier)),
                new TestCaseData(DataFormats.Bitmap, CreateSimpleBitmap(), new CompareCallback(BitmapVerifier)),
                new TestCaseData(DataFormats.Text, "blah", new CompareCallback(TextVerifier)),
                new TestCaseData(DataFormats.UnicodeText, "a\x0204\x0243\x0134\n\n\n~~\b", new CompareCallback(TextVerifier)),
                new TestCaseData(DataFormats.Html, "<HTML><BODY><BOLD>abc def</BOLD></BODY></HTML>", new CompareCallback(TextVerifier)),
                new TestCaseData(DataFormats.Xaml,
                    "<?Mapping XmlNamespace=\"medians\" ClrNamespace=\"System.Windows.Media\" Assembly=\"PresentationCore\" ?>" +
                    "<TextBox xmlns=\"http://schemas.microsoft.com/2005/xaml/\" xmlns:mil=\"medians\" xmlns:def=\"Definition\"" +
                    "AcceptsReturn=\"True\" Height=\"200\" Width=\"200\" Background=\"LightCyan\" ScrollerVisibilityY=\"Auto\"" +
                    "ScrollerVisibilityX=\"Auto\">I am TextBox.</TextBox>",
                    new CompareCallback(TextVerifier)),
                new TestCaseData(DataFormats.StringFormat, "StringFormat", new CompareCallback(TextVerifier)),
                //FileDrop with 1 file
                new TestCaseData(DataFormats.FileDrop, new string[]{"FileList.txt"}, new CompareCallback(FileDropVerifier)),
                //FileDrop with 4 files
                new TestCaseData(DataFormats.FileDrop, new string[]{"test1.txt","test2.txt","test3.txt", "test4.txt"},
                    new CompareCallback(FileDropVerifier)),
                //FileDrop with max file name length.
                new TestCaseData(DataFormats.FileDrop,
                    new string[] { "abcdefghijklmnopqrstuvwxyz`1234567890-=~!@#$%^&*()_+ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz`1234567890-=~!@#$%^&*()_+ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz`1234567890-=~!@#$%^&*()_+ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstu.doc" },
                    new CompareCallback(FileDropVerifier)),
            };

            #region Helper methods.

            private static void BitmapVerifier(object master, object sample)
            {
                Bitmap masterBitmap;
                Bitmap sampleBitmap;
                Bitmap differentBitmap;
                bool bitmapsMatch;

                masterBitmap = (Bitmap)master;
                if (sample is BitmapSource)
                {
                    // Convert BimapSource to System.Drawing.Bitmap to get Win32 HBITMAP.
                    BitmapEncoder bitmapEncoder;
                    Stream bitmapSteram;
                    bitmapEncoder = new BmpBitmapEncoder();
                    bitmapEncoder.Frames.Add(BitmapFrame.Create(sample as BitmapSource));

                    bitmapSteram = new MemoryStream();
                    bitmapEncoder.Save(bitmapSteram);
                    sampleBitmap = new System.Drawing.Bitmap(bitmapSteram);
                }
                else
                {
                    sampleBitmap = (Bitmap)sample;
                }
                
                bitmapsMatch = Microsoft.Test.Imaging.ComparisonOperationUtils.AreBitmapsEqual(
                    masterBitmap, sampleBitmap, out differentBitmap);
                if (!bitmapsMatch)
                {
                    Logger.Current.LogImage(masterBitmap, "original");
                    Logger.Current.LogImage(sampleBitmap, "clipboardBitmap");
                    Logger.Current.LogImage(differentBitmap, "differences");
                }
                Verifier.Verify(bitmapsMatch, "Bitmaps are equal.", true);
            }
            private static void TextVerifier(object master, object sample)
            {
                string masterText;
                string sampleText;

                masterText = (string)master;
                sampleText = (string)sample;
                Logger.Current.Log("masterText[" + masterText + "]");
                Logger.Current.Log("sampleText[" + sampleText + "]");
                Verifier.Verify(masterText == sampleText, "Text are equal.", true);
            }

            private static void FileDropVerifier(object master, object sample)
            {
                string[] masterFile;
                string[] sampleFile;

                masterFile = (string[])master;
                sampleFile = (string[])sample;
                for (int i = 0; i < masterFile.Length; i++)
                {
                    Logger.Current.Log("masterFile" + i + "[" + masterFile[i] + "]");
                    Logger.Current.Log("sampleFile" + i + "[" + sampleFile[i] + "]");
                    Verifier.Verify(masterFile[i] == sampleFile[i], "Text are equal.", true);
                }
            }
            private static Bitmap CreateSimpleBitmap()
            {
                Bitmap result;
                result = new Bitmap(10, 10);
                using (Graphics graphics = Graphics.FromImage(result))
                {
                    graphics.FillRectangle(System.Drawing.Brushes.Red, 0, 0, 10, 10);
                    graphics.DrawRectangle(System.Drawing.Pens.Blue, 2, 2, 5, 5);
                }
                return result;
            }

            #endregion Helper methods.
        }

        #endregion Test case data.

        #region Main flow.

        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            DataObject originalDataObject;
            IDataObject clipboardData;
            object originalContent;
            object clipboardContent;

            _caseIndex= ConfigurationSettings.Current.GetArgumentAsInt("caseIndex", true);

            originalContent = TestCaseData.Cases[_caseIndex].DataContent;
            originalDataObject = new DataObject();
            originalDataObject.SetData(TestCaseData.Cases[_caseIndex].DataFormatName, originalContent);
            Clipboard.SetDataObject(originalDataObject);
            clipboardData = Clipboard.GetDataObject();
            clipboardContent = clipboardData.GetData(TestCaseData.Cases[_caseIndex].DataFormatName);
            if (TestCaseData.Cases[_caseIndex].DataVerifier == null)
            {
                Verifier.Verify(clipboardContent.Equals(originalContent), "Content matches after retrieving from clipboard.", true);
            }
            else
            {
                TestCaseData.Cases[_caseIndex].DataVerifier(originalContent, clipboardContent);
            }
          
            //Test all the constructor
            DataObjectConstructorTest.RunTest();

            Logger.Current.ReportSuccess();
            //QueueHelper.Current.QueueDelegate(new SimpleHandler(RunNextTest));
        }

        private void RunNextTest()
        {
            _caseIndex++;
            if (_caseIndex == TestCaseData.Cases.Length)
            {
                Log("Test is completed at:[" + _caseIndex + "]");
                Logger.Current.ReportSuccess();
            }
            else
            {
                Log("Run next test case:[" + _caseIndex + "]");
                RunTestCase();
            }
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Verifies that the advising APIs in the OLE view of
    /// DataObject work as expected.
    /// </summary>
    /// <remarks>
    /// The full specification of the OLE data capabilities
    /// can be found at
    /// http://msdn.microsoft.com/library/en-us/com/html/8a002deb-2727-456c-8078-a9b0d5893ed4.asp
    /// </remarks>
    [Test(2, "DataObject", "DataObjectAdvising", MethodParameters = "/TestCaseType=DataObjectAdvising")]
    [TestOwner("Microsoft"), TestTactics("177"),
     TestBugs("363,364,365,366,367,368,369,370,371,372,373,352")]
    public class DataObjectAdvising : CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            const string TextData = "Text data sample.";

            DataObject dataObject;              // Avalon data object.
            Win32.IOleDataObject oleData;       // OLE view of data object.

            Log("Setting an Avalon object on the Clipboard...");
            dataObject = new DataObject();
            dataObject.SetData(TextData);
            oleData = GetOleDataObject(dataObject);

            VerifyQueryGetData(oleData);
            VerifyGetDataHere(oleData, TextData);
            VerifyCanonicalFormat(oleData);
            VerifySetData(oleData);
            VerifyFormatEnumeration();
            VerifyAdvising(oleData);
            VerifyGetData();
            VerifyGetDataMetafile();
            VerifyTransfers();
            Logger.Current.ReportSuccess();
        }

        private void VerifyAdvising(Win32.IOleDataObject oleData)
        {
            object adviseEnumerator;
            int pdwConnection;
            AdviseSink sink;
            Win32.FORMATETC format;

            sink = new AdviseSink();
            format = new Win32.FORMATETC();

            SetupStandardTextRequest(format, null, Win32.TYMED_HGLOBAL);
            VerifyExpectedHResult(
                Win32.SafeOleDAdvise(oleData, format, 0, sink, out pdwConnection), Win32.OLE_E_ADVISENOTSUPPORTED, "OleDAdvise");
            VerifyExpectedHResult(
                Win32.SafeOleDUnadvise(oleData, pdwConnection), Win32.OLE_E_ADVISENOTSUPPORTED, "OleDUnadvise");
            VerifyExpectedHResult(
                Win32.SafeOleEnumDAdvise(oleData, out adviseEnumerator), Win32.OLE_E_ADVISENOTSUPPORTED, "OleEnumDAdvise");
        }

        private void VerifyCanonicalFormat(Win32.IOleDataObject oleData)
        {
            Win32.FORMATETCStruct format;
            Win32.FORMATETCStruct outFormat;

            format = new Win32.FORMATETCStruct();
            outFormat = new Win32.FORMATETCStruct();

            // See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/com/html/3a5009a0-8d92-483a-b055-8a97f326dccd.asp?frame=true
            Log("Verify OleGetCanonicalFormatEtc...");
            format = new Win32.FORMATETCStruct();
            format.cfFormat = (short)Win32.CF_TEXT;
            format.dwAspect = Win32.DVASPECT_CONTENT;
            format.lindex = -1;
            format.tymed = Win32.TYMED_HGLOBAL;
            CheckHResult(
                oleData.OleGetCanonicalFormatEtc(ref format, ref outFormat), "OleGetCanonicalFormatEtc");
            Verifier.Verify(outFormat.cfFormat == (short)Win32.CF_TEXT,
                "Canonical format is ANSI Text [" + format.cfFormat + "]", true);
            Verifier.Verify(outFormat.dwAspect == Win32.DVASPECT_CONTENT,
                "Canonical format has content aspect [" + format.dwAspect + "]", true);
            Verifier.Verify(outFormat.lindex == -1,
                "Canonical format has unused index [" + format.lindex + "]", true);

            Log("Verifying that aliasing doesn't cause a failure...");
            CheckHResult(
                oleData.OleGetCanonicalFormatEtc(ref format, ref format), "OleGetCanonicalFormatEtc");
            Verifier.Verify(format.cfFormat == (short)Win32.CF_TEXT,
                "Canonical format is ANSI Text [" + format.cfFormat + "]", true);

            Log("Verifying that FORMATETC.lindex != -1 is rejected...");
            format.lindex = 2;
            /*
            VerifyExpectedHResult(oleData.OleGetCanonicalFormatEtc(ref format, ref outFormat),
                Win32.DV_E_LINDEX, "OleGetCanonicalFormatEtc");
            */

            Log("Verifying that FORMATETC.ptd gets nulled out...");
            format.lindex = -1;
            format.ptd = Win32.SafeGetDummyTargetDevice();
            /*
            VerifyExpectedHResult(oleData.OleGetCanonicalFormatEtc(ref format, ref format),
                Win32.DATA_S_SAMEFORMATETC, "OleGetCanonicalFormatEtc");
            Verifier.Verify(format.ptd == IntPtr.Zero,
                "Target device was null'ed out by OleGetCanonicalFormatEtc.", true);
            */
        }

        private void VerifyFormatEnumeration()
        {
            DataObject dataObject;              // Data object to hold data.
            Win32.IEnumFORMATETC bookmark;      // Bookmarked position in enumeration.
            Win32.IEnumFORMATETC enumFormatEtc; // Enumerator for formats.
            Win32.IOleDataObject oleData;       // OLE IDataObject interface for data object.
            int fetched;                        // Count of formats returned.
            short firstFormat;                  // First format in consecutive requests.

            // Populate a data object with multiple formats and get the OLE interface.
            dataObject = new DataObject();
            dataObject.SetData(DataFormats.Text, "DataFormats.Text sample.");
            dataObject.SetData(DataFormats.UnicodeText, "DataFormats.UnicodeText sample.");
            oleData = GetOleDataObject(dataObject);

            Log("Verifying OleEnumFormatEtc...");
            VerifyExpectedHResult(
                Win32.SafeOleEnumFormatEtc(oleData, Win32.DATADIR_SET, out enumFormatEtc), Win32.E_NOTIMPL, "OleEnumFormatEtc");
            Log("Enumerator for setting formats is not implemented, as expected.");
            CheckHResult(
                Win32.SafeOleEnumFormatEtc(oleData, Win32.DATADIR_GET, out enumFormatEtc), "OleEnumFormatEtc");
            Verifier.Verify(enumFormatEtc != null, "FORMATETC enumerator returned successfully.", true);

            Log("Verifying multiple requests...");
            Win32.FORMATETCStruct[] rgelt = new Win32.FORMATETCStruct[20];
            VerifyExpectedHResult(
                Win32.SafeEnumFormatEtcNext(enumFormatEtc, 20, rgelt, out fetched), Win32.S_FALSE, "EnumFormatEtcNext");
            Verifier.Verify(fetched < 20, "Less than 20 formats for a plain string returned.", true);

            Log("Verifying enumerator reset...");
            CheckHResult(enumFormatEtc.Reset(), "IEnumFormatEtc.Reset");

            Log("Verifying enumerator fetching and cloning...");
            CheckHResult(enumFormatEtc.Clone(out bookmark), "IEnumFormatEtc.Clone");
            CheckHResult(
                Win32.SafeEnumFormatEtcNext(enumFormatEtc, 1, rgelt), "IEnumFormatEtc.Next");
            Verifier.Verify(rgelt[0].cfFormat != 0, "Clipboard format has been assigned.");
            firstFormat = rgelt[0].cfFormat;

            // Verify that the next format is different.
            CheckHResult(
                Win32.SafeEnumFormatEtcNext(enumFormatEtc, 1, rgelt), "IEnumFormatEtc.Next");
            Verifier.Verify(rgelt[0].cfFormat != firstFormat, "Second clipboard format is different.");

            // Reset the format we read, and try reading through the previous bookmark.
            rgelt[0].cfFormat = 0;
            CheckHResult(
                Win32.SafeEnumFormatEtcNext(bookmark, 1, rgelt), "IEnumFormatEtc.Next");
            Verifier.Verify(rgelt[0].cfFormat == firstFormat, "Bookmark re-reads the original format.");
        }

        private void VerifyGetData()
        {
            DataObject dataObject;          // Managed data obejct.
            Win32.IOleDataObject oleData;   // OLE IDataObject interface.
            Win32.FORMATETC format;         // Content format requested.
            Win32.STGMEDIUM medium;         // Storage medium.

            // Set up a data object and get its data.
            dataObject = new DataObject();
            dataObject.SetData(TextSample);
            oleData = GetOleDataObject(dataObject);

            format = new Win32.FORMATETC();
            medium = new Win32.STGMEDIUM();

            Log("OleGetData uses the FORMATETC structure to indicate exactly what kind of data the caller is requesting...");
            SetupStandardTextRequest(format, medium, Win32.TYMED_HGLOBAL);
            CheckHResult(Win32.SafeOleGetData(oleData, format, medium), "OleGetData");
            try
            {
                string dataInHandle = GetStringFromHGlobal(medium.unionmember);
                Verifier.Verify(dataInHandle == TextSample,
                    "Data in handle [" + dataInHandle + "] matches sample data [" + TextSample + "]",
                    true);
            }
            finally
            {
                Win32.SafeReleaseStgMedium(medium);
            }

            SetupStandardTextRequest(format, medium, Win32.TYMED_ISTREAM | Win32.TYMED_HGLOBAL);
            CheckHResult(Win32.SafeOleGetData(oleData, format, medium), "OleGetData");
            try
            {
                Log("Expecting medium.tymed to be one TYMED_HGLOBAL ( " + Win32.TYMED_HGLOBAL + "), not OR'ed with STREAM");
                Log("Actual value returned is (" + medium.tymed + ")");
                Verifier.Verify(medium.tymed == Win32.TYMED_HGLOBAL);
            }
            finally
            {
                Win32.SafeReleaseStgMedium(medium);
            }
        }

        private void VerifyGetDataMetafile()
        {
            System.Drawing.Imaging.Metafile metafile;
            DataObject dataObject;
            Win32.IOleDataObject oleData;
            Win32.FORMATETC format;         // Content format requested.
            Win32.STGMEDIUM medium;         // Storage medium.

            // Create a sample metafile.
            metafile = new System.Drawing.Imaging.Metafile(Win32.GetDC(IntPtr.Zero),
                System.Drawing.Imaging.EmfType.EmfOnly);
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(metafile))
            {
                graphics.FillRectangle(Brushes.Red, 0, 0, 30, 30);
            }

            Log("Retrieving data object with metafile...");
            dataObject = new DataObject();
            dataObject.SetData(DataFormats.EnhancedMetafile, metafile);
            oleData = GetOleDataObject(dataObject);

            Log("Verifying that the metafile can be retrieved...");
            format = new Win32.FORMATETC();
            medium = new Win32.STGMEDIUM();
            format.cfFormat = (short)Win32.CF_ENHMETAFILE;
            format.lindex = -1;

            format.tymed = Win32.TYMED_ENHMF;
            format.lindex = -1;
            CheckHResult(oleData.OleGetData(format, medium), "OleGetData");
            try
            {
                Verifier.Verify(medium.unionmember != IntPtr.Zero,
                    "Returned metafile handle is non-null.", true);
            }
            finally
            {
                Win32.SafeReleaseStgMedium(medium);
            }
        }

        private void VerifyGetDataHere(Win32.IOleDataObject oleData, string expectedText)
        {
            Win32.FORMATETC format;             // Content format requested.
            Win32.STGMEDIUM pMedium;            // Storage medium.

            format = new Win32.FORMATETC();
            pMedium = new Win32.STGMEDIUM();

            Log("Verify OleGetDataHere...");

            Log("Checking OleGetDataHere with invalid type of media (TYMED_ENHMF)...");
            SetupStandardTextRequest(format, pMedium, Win32.TYMED_ENHMF);
            VerifyExpectedHResult(
                Win32.SafeOleGetDataHere(oleData, format, pMedium), Win32.DV_E_TYMED, "OleGetDataHere");

            Log("Checking OleGetDataHere rejects calls with or'ed media (TYMED_HGLOBAL | TYMED_ISTREAM)...");
            SetupStandardTextRequest(format, pMedium, Win32.TYMED_ISTREAM | Win32.TYMED_HGLOBAL);
            VerifyExpectedHResult(
                Win32.SafeOleGetDataHere(oleData, format, pMedium), Win32.DV_E_TYMED, "OleGetDataHere");

            Log("Checking OleGetDataHere succeeds for regular cases...");
            SetupStandardTextRequest(format, pMedium, Win32.TYMED_HGLOBAL);
            pMedium.unionmember = Win32.GlobalAlloc(0, 1024);
            try
            {
                string dataInHandle;

                CheckHResult(Win32.SafeOleGetDataHere(oleData, format, pMedium), "OleGetDataHere");
                dataInHandle = GetStringFromHGlobal(pMedium.unionmember);
                Verifier.Verify(dataInHandle == expectedText,
                    "Data in handle [" + dataInHandle + "] matches sample data [" + expectedText + "]",
                    true);
                    
                // Work around Regression_Bug352, which shrinks our buffer and causes the next GetDataHere to fial.
                Win32.SafeReleaseStgMedium(pMedium);
                pMedium.tymed = Win32.TYMED_HGLOBAL;
                pMedium.unionmember = Win32.GlobalAlloc(0, 1024);
                
                format.cfFormat = (short)Win32.CF_UNICODETEXT;
                Log("Verifying it also works for UnicodeText...");
                CheckHResult(oleData.OleGetDataHere(format, pMedium), "OleGetDataHere");
                dataInHandle = GetStringFromHGlobal(pMedium.unionmember, true);
                Verifier.Verify(dataInHandle == expectedText,
                    "Data in handle [" + dataInHandle + "] matches sample data [" + expectedText + "]",
                    true);
            }
            finally
            {
                Win32.SafeReleaseStgMedium(pMedium);
            }
        }

        private void VerifyQueryGetData(Win32.IOleDataObject oleData)
        {
            Win32.FORMATETC format; // Content format requested.

            Log("Querying for text availability...");
            format = new Win32.FORMATETC();
            SetupStandardTextRequest(format, null, Win32.TYMED_HGLOBAL);
            CheckHResult(Win32.SafeOleQueryGetData(oleData, format), "QueryGetData");
        }

        private void VerifySetData(Win32.IOleDataObject oleData)
        {
            Win32.FORMATETC format;             // Content format requested.
            Win32.STGMEDIUM pMedium;            // Storage medium.

            format = new Win32.FORMATETC();
            pMedium = new Win32.STGMEDIUM();
            SetupStandardTextRequest(format, pMedium, Win32.TYMED_HGLOBAL);

            Log("Verify OleSetData...");
            VerifyFailedHResult(
                Win32.SafeOleSetData(oleData, format, pMedium, true), "OleSetData");
            Log("OleSetData was not implemented, as expected.");
        }

        private void VerifyStreamString(string text, bool useUtf16, bool askUnicode)
        {
            // Verify that streams appear correctly.
            DataObject dataObject;
            Win32.IOleDataObject oleData;
            Stream stream;
            Win32.FORMATETC format;
            Win32.STGMEDIUM medium;
            string dataInHandle;

            format = new Win32.FORMATETC();
            medium = new Win32.STGMEDIUM();
            dataObject = new DataObject();

            Log("Verifying reading strings from streams (putting UTF16: " +
                useUtf16 + ", asking for UTF16: " + askUnicode + ")");

            // Populate a memory stream.
            stream = CreateMemoryStreamForText(TextSample, useUtf16);
            if (useUtf16)
            {
                dataObject.SetData(DataFormats.UnicodeText, stream);
            }
            else
            {
                dataObject.SetData(DataFormats.Text, stream);
            }

            oleData = GetOleDataObject(dataObject);
            SetupStandardTextRequest(format, medium, Win32.TYMED_HGLOBAL);
            if (askUnicode)
            {
                format.cfFormat = (short)Win32.CF_UNICODETEXT;
            }

            CheckHResult(oleData.OleGetData(format, medium), "OleGetData");
            dataInHandle = GetStringFromHGlobal(medium.unionmember, askUnicode);
            Verifier.Verify(dataInHandle == TextSample,
                "Data in handle [" + dataInHandle + "] matches sample data [" + TextSample + "]",
                true);

            Win32.SafeReleaseStgMedium(medium);
        }

        private void VerifyStringForFormat(int clipboardFormat, bool isUnicode)
        {
            // Verify that strings appear correctly.
            DataObject dataObject;
            Win32.IOleDataObject oleData;
            Win32.FORMATETC format;
            Win32.STGMEDIUM medium;
            string dataInHandle;
            DataFormat dataFormat;

            dataFormat = DataFormats.GetDataFormat(clipboardFormat);

            Log("Verifying string transfer for format: " +
                dataFormat.Name + " (" + dataFormat.Id + ")");

            // Create a data object with the data, and get its OLE interface.
            dataObject = new DataObject();
            dataObject.SetData(dataFormat.Name, TextSample);
            oleData = GetOleDataObject(dataObject);

            // Request the data from the OLE interface.
            format = new Win32.FORMATETC();
            medium = new Win32.STGMEDIUM();
            SetupStandardTextRequest(format, medium, Win32.TYMED_HGLOBAL);
            format.cfFormat = (short)clipboardFormat;
            CheckHResult(oleData.OleGetData(format, medium), "OleGetData");

            // Verify that the data matches our expectations.
            dataInHandle = GetStringFromHGlobal(medium.unionmember, isUnicode);
            Verifier.Verify(dataInHandle == TextSample,
                "Data in handle [" + dataInHandle + "] matches sample data [" + TextSample + "]",
                true);

            // Clean up the callee-allocated memory.
            Win32.SafeReleaseStgMedium(medium);
        }

        /// <summary>Verifies that data can be transferred correctly.</summary>
        private void VerifyTransfers()
        {
            VerifyStreamString(TextSample, false, false);
            VerifyStreamString(TextSample, true, true);

            // Verify that non-UTF16 formats are shown correctly.
            VerifyStringForFormat((int)Win32.CF_TEXT, false);
            VerifyStringForFormat(DataFormats.GetDataFormat(DataFormats.Html).Id, false);
            VerifyStringForFormat(DataFormats.GetDataFormat(DataFormats.Rtf).Id, false);
            VerifyStringForFormat(DataFormats.GetDataFormat(DataFormats.Xaml).Id, false);
            VerifyStringForFormat((int)Win32.CF_OEMTEXT, false);

            // Verify that UTF16 formats are shown correctly.
            VerifyStringForFormat((int)Win32.CF_UNICODETEXT, true);
        }

        #endregion Main flow.

        #region Helper methods.

        /// <summary>
        /// Checks that the given HRESULT for an operation succeeded.
        /// </summary>
        /// <param name="hresult">Result value to check.</param>
        /// <returns>
        /// true if hresult is S_OK, false if S_FALSE; an exception for
        /// any other value.
        /// </returns>
        private bool CheckHResult(int hresult)
        {
            return CheckHResult(hresult, null);
        }

        /// <summary>
        /// Checks that the given HRESULT for an operation succeeded.
        /// </summary>
        /// <param name="hresult">Result value to check.</param>
        /// <param name="operation">Operation that returned hresult.</param>
        /// <returns>
        /// true if hresult is S_OK, false if S_FALSE; an exception for
        /// any other value.
        /// </returns>
        private bool CheckHResult(int hresult, string operation)
        {
            string errorDescription;

            if (hresult == Win32.S_OK)
            {
                return true;
            }
            else if (hresult == Win32.S_FALSE)
            {
                return false;
            }
            else if (hresult == Win32.E_NOTIMPL)
            {
                Log(operation + " is not implemented.");
                return false;
            }
            else
            {
                errorDescription = "COM/OLE call failed with HRESULT " +
                    String.Format("{0:X}", hresult);
                if (operation != null)
                {
                    errorDescription += " during call to " + operation;
                }
                throw new Exception(errorDescription);
            }
        }

        /// <summary>
        /// Given the specified string, returns a stream with its
        /// encoded characters.
        /// </summary>
        /// <param name="p">String to return in stream.</param>
        /// <param name="useUtf16">Whether to use UTF-16 rather than ASCII encoding.</param>
        /// <returns>Stream with string contents.</returns>
        private Stream CreateMemoryStreamForText(string p, bool useUtf16)
        {
            byte[] bytes;                   // Byte representation of string.
            MemoryStream result;            // Stream with text.
            System.Text.Encoding encoding;  // ASCII or Unicode encoding.

            // Get the bytes that encode the string.
            encoding = (useUtf16)? System.Text.Encoding.Unicode : System.Text.Encoding.ASCII;
            bytes = encoding.GetBytes(p + "\0");

            result = new MemoryStream(bytes.Length);
            result.Write(bytes, 0, bytes.Length);

            return result;
        }

        private static BitmapSource CreateSampleBitmap()
        {
            return RenderBrushToImageData(System.Windows.Media.Brushes.Red, 30, 30, new Rect(0, 0, 30, 30), 96, 96);
        }

        /// <summary>
        /// Given the specified DataObject, returns its OLE interface.
        /// </summary>
        /// <param name="dataObject">The DataObject to get an interface for.</param>
        /// <returns>The OLE interface of the specified object.</returns>
        /// <remarks>
        /// This has the side-effect of placing the data object on the
        /// clipboard.
        /// </remarks>
        private Win32.IOleDataObject GetOleDataObject(DataObject dataObject)
        {
            Win32.IOleDataObject result;

            if (dataObject == null)
            {
                throw new ArgumentNullException("dataObject");
            }

            Clipboard.SetDataObject(dataObject);

            result = null;
            CheckHResult(Win32.OleGetClipboard(ref result), "OleGetClipboard");

            return result;
        }

        private static string GetStringFromHGlobal(IntPtr handle)
        {
            return GetStringFromHGlobal(handle, false);
        }

        private static string GetStringFromHGlobal(IntPtr handle, bool isUnicode)
        {
            IntPtr buffer;

            if (handle == IntPtr.Zero)
            {
                throw new ArgumentNullException("handle");
            }

            buffer = Win32.SafeGlobalLock(handle);
            if (buffer == IntPtr.Zero)
            {
                throw new System.ComponentModel.Win32Exception();
            }
            try
            {
                return Win32.ReadNullTerminatedString(buffer, isUnicode);
            }
            finally
            {
                Win32.SafeGlobalUnlock(handle);
            }
        }

        private static BitmapSource RenderBrushToImageData(System.Windows.Media.Brush b, int width, int height,
            Rect realizationSize, double dpiX, double dpiY)
        {
            // Create image data
            RenderTargetBitmap id = new RenderTargetBitmap(
                width, height, dpiX, dpiY, System.Windows.Media.PixelFormats.Pbgra32);

            // Create a visual to render
            System.Windows.Media.DrawingVisual myDrawingVisual = new System.Windows.Media.DrawingVisual();

            // draw a rectangle using the given brush
            System.Windows.Media.DrawingContext myDrawingContext = myDrawingVisual.RenderOpen();
            myDrawingContext.DrawRectangle(b, null, realizationSize);
            myDrawingContext.Close();

            // Render into Bitmap
            id.Render(myDrawingVisual);

            // We need this here since TR is asuming Bgra32 not Pbgra32 - we do our own format conversions
            FormatConvertedBitmap fcb = new FormatConvertedBitmap(
                id, System.Windows.Media.PixelFormats.Bgra32, null, 0.0);

            return fcb;
        }

        /// <summary>
        /// Sets up the given arguments to the medium type for a simple text request.
        /// </summary>
        private static void SetupStandardTextRequest(Win32.FORMATETC format,
            Win32.STGMEDIUM medium, int mediumType)
        {
            if (format != null)
            {
                format.cfFormat = (short)Win32.CF_TEXT;
                format.dwAspect = Win32.DVASPECT_CONTENT;
                format.lindex = -1;
                format.tymed = mediumType;
            }

            if (medium != null)
            {
                medium.tymed = mediumType;
                medium.unionmember = IntPtr.Zero;
                medium.pUnkForRelease = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Checks that the given HRESULT is as expected.
        /// </summary>
        /// <param name="hresult">Result value to check.</param>
        /// <param name="expectedResult">Expected result value.</param>
        /// <param name="operation">Operation that returned hresult.</param>
        private void VerifyExpectedHResult(int hresult, int expectedResult, string operation)
        {
            if (hresult != expectedResult)
            {
                string errorDescription;

                errorDescription = "COM/OLE call failed with unexpected HRESULT " +
                    String.Format("{0:X}", hresult) + " (expecting " +
                    String.Format("{0:X}", expectedResult) + ")";
                if (operation != null)
                {
                    errorDescription += " during call to " + operation;
                }
                throw new Exception(errorDescription);
            }
        }

        /// <summary>
        /// Checks that the given HRESULT is a failure code.
        /// </summary>
        /// <param name="hresult">Result value to check.</param>
        /// <param name="operation">Operation that returned hresult.</param>
        private void VerifyFailedHResult(int hresult, string operation)
        {
            // From winerror.h: # define SUCCEEDED(hr) (((HRESULT)(hr)) >= 0)
            if (hresult >= 0)
            {
                string errorDescription;

                errorDescription = "COM/OLE call succeeded with HRESULT " +
                    String.Format("{0:X}", hresult) + " (expecting failure)";
                if (operation != null)
                {
                    errorDescription += " during call to " + operation;
                }
                throw new Exception(errorDescription);
            }
        }

        #endregion Helper methods.

        #region Private data.

        private const string TextSample = "Text sample.";

        #endregion Private data.

        #region Inner types.

        /// <summary>Empty implementation of the OLE IAdviseSink interface.</summary>
        class AdviseSink: Win32.IAdviseSink
        {
            /// <summary>
            /// Called by the server to notify a data object's currently
            /// registered advise sinks that data in the object has changed.
            /// </summary>
            /// <param name='pFormatetc'></param>
            /// <param name='pStgmed'></param>
            public void OnDataChange(Win32.FORMATETC pFormatetc, Win32.STGMEDIUM pStgmed) { }

            /// <summary>
            /// Notifies an object's registered advise sinks that its view
            /// has changed.
            /// </summary>
            /// <param name='dwAspect'>The aspect, or view, of the object.</param>
            /// <param name='lindex'>The portion of the view that has changed.</param>
            public void OnViewChange(int dwAspect, int lindex) { }

            /// <summary>
            /// Called by the server to notify all registered advisory sinks
            /// that the object has been renamed.
            /// </summary>
            /// <param name='pmk'>
            /// Pointer to the IMoniker interface on the new full moniker of
            /// the object.
            /// </param>
            public void OnRename(object pmk) { }

            /// <summary>
            /// Called by the server to notify all registered advisory sinks
            /// that the object has been saved.
            /// </summary>
            public void OnSave() { }

            /// <summary>
            /// Called by the server to notify all registered advisory sinks
            /// that the object has changed from the running to the loaded
            /// state.
            /// </summary>
            public void OnClose() { }
        }

        #endregion Inner types.
    }

    /// <summary>
    /// Verifies that the DataObject cannot be used to populate
    /// the Clipboard with malicious data.
    /// </summary>
    [Test(2, "DataObject", "DataObjectLockDown1", MethodParameters = "/TestCaseType=DataObjectLockDown",Disabled=true)]
    [Test(2, "PartialTrust", TestCaseSecurityLevel.FullTrust, "DataObjectLockDown2", MethodParameters = "/TestCaseType=DataObjectLockDown /XbapName=EditingTestDeploy")]
    [TestOwner("Microsoft"), TestTactics("178"), TestBugs("374,375,376"), TestWorkItem("18,19")]
    public class DataObjectLockDown: ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Reads the values to run a combination.</summary>
        protected override bool DoReadCombination(System.Collections.Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);

            // UseDragDrop does not care about flushing data object.
            result = result && !(_useDragDrop && _flush);

            return result;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _dataForTransfer = GetDataForCombination();
            GetExpectedResults(out _securityExceptionExpected, out _successExpected);

            if (_useDragDrop)
            {
                RunDragDrop();
            }
            else
            {
                RunClipboard();
            }
        }

        private void RunClipboard()
        {
            string[] formats;
            IDataObject clipboardDataObject;

            Clipboard.Clear();
            try
            {
                Clipboard.SetDataObject(_dataForTransfer, _flush);
                if (_securityExceptionExpected)
                {
                    throw new Exception("Security exception expected but none thrown.");
                }

                clipboardDataObject = ClipboardWrapper.GetDataObject();
                formats = DataObjectWrapper.GetFormats(clipboardDataObject);
                Verifier.Verify(_successExpected == (formats.Length > 0),
                    "Expected setting to work: " + _successExpected + ", found DataObject on " +
                    "clipboard with format count: " + formats.Length, true);
            }
            catch(System.Security.SecurityException securityException)
            {
                if (!_securityExceptionExpected)
                {
                    throw new Exception("Unexpected security exception.", securityException);
                }
                Log("Security exception thrown as expected.");
            }
            QueueDelegate(NextCombination);
        }

        private void RunDragDrop()
        {
            StackPanel panel;

            panel = new StackPanel();
            _sourceElement = new Shapes.Rectangle();
            _destinationElement = new Shapes.Rectangle();

            _sourceElement.Width = _destinationElement.Width = 50d;
            _sourceElement.Height = _destinationElement.Height = 50d;

            _sourceElement.Fill = System.Windows.Media.Brushes.LightGreen;
            _destinationElement.Fill = System.Windows.Media.Brushes.DarkGreen;

            _destinationElement.AllowDrop = true;
            _destinationElement.Drop += new DragEventHandler(DestinationElementDrop);

            panel.Children.Add(_sourceElement);
            panel.Children.Add(_destinationElement);

            TestElement = panel;
            QueueDelegate(PrepareMouseDrag);
        }

        private void PrepareMouseDrag()
        {
            MouseInput.MouseMove(_sourceElement);
            MouseInput.MouseDown();
            QueueDelegate(StartDrag);
        }

        private void StartDrag()
        {
            try
            {
                _mainDispatcher = Dispatcher.CurrentDispatcher;
                _elementDropped = false;
                MouseInput.MouseDragInOtherThread(
                    ElementUtils.GetScreenRelativeCenter(_sourceElement),
                    ElementUtils.GetScreenRelativeCenter(_destinationElement), true,
                    TimeSpan.FromSeconds(2), null, null);
                _effects = DragDrop.DoDragDrop(_sourceElement, _dataForTransfer, DragDropEffects.All);
            }
            catch (System.Security.SecurityException)
            {
                _securityExceptionExpected = true;
            }

            if (_successExpected || !_securityExceptionExpected)
            {
                Verifier.Verify(_elementDropped, "Element was dropped successfully.", true);
            }
            else if (_securityExceptionExpected || !_successExpected)
            {
                Verifier.Verify(_effects == DragDropEffects.None,
                    "The effects is " + _effects + ", expecting None for non-successful drag-drop.", true);
            }
            QueueDelegate(NextCombination);
        }

        private void DestinationElementDrop(object sender, DragEventArgs e)
        {
            IDataObject dragDataObject;
            string[] formats;

            dragDataObject = e.Data;
            formats = DataObjectWrapper.GetFormats(dragDataObject);

            Verifier.Verify(_successExpected == (formats.Length > 0),
                "Expected dragging to work: " + _successExpected + ", found DataObject on " +
                "DragDrop operation with format count: " + formats.Length, true);
            _elementDropped = true;
        }

        #endregion Main flow.

        #region Helper methods.

        private object GetDataForCombination()
        {
            return _contentData.Data;
        }

        private void GetExpectedResults(out bool securityExceptionExpected, out bool successExpected)
        {
            securityExceptionExpected = false;

            // To succeed, an operation needs to be safe, or it needs to be fully trusted.
            successExpected = _contentData.IsSafeToCopy || !IsTestRunningInPartialTrust;

            successExpected = successExpected && !(_useDragDrop && IsTestRunningInPartialTrust);

            //In PartialTrust, clipboard cannot be set using SetDataObject
            securityExceptionExpected = (IsTestRunningInPartialTrust && !_useDragDrop);            
        }

        /// <summary>
        /// Determines whether the current test is running in partial trust.
        /// </summary>        
        private bool IsTestRunningInPartialTrust
        {
            get
            {
                if (!_isTestRunningInPartialTrust.HasValue)
                {
                    try
                    {
                        new System.Security.Permissions.SecurityPermission(
                            System.Security.Permissions.PermissionState.Unrestricted).Demand();                            
                        _isTestRunningInPartialTrust = false;
                    }
                    catch(System.Security.SecurityException)
                    {
                        _isTestRunningInPartialTrust = true;
                    }
                }
                return _isTestRunningInPartialTrust.Value;
            }
        }

        #endregion Helper methods.

        #region Private data.

        /// <summary>Data to try to transfer onto the clipboard.</summary>
        private TransferContentData _contentData = null;

        /// <summary>Whether the content should be immediately flushed.</summary>
        private bool _flush = false;

        /// <summary>Whether drag/drop should be used (as opposed to Clipboard).</summary>
        private bool _useDragDrop = false;

        /// <summary>Whether the current test is running in partial trust.</summary>
        private Nullable<bool> _isTestRunningInPartialTrust;

        /// <summary>Destination element in drag/drop operation.</summary>
        private Shapes.Rectangle _destinationElement;

        /// <summary>Source element in drag/drop operation.</summary>
        private Shapes.Rectangle _sourceElement;

        /// <summary>Dispatcher on main application thread.</summary>
        private Dispatcher _mainDispatcher;

        /// <summary>Whether we expect a security exception for this combination.</summary>
        private bool _securityExceptionExpected;

        /// <summary>Whether we expect success for this combination.</summary>
        private bool _successExpected;

        /// <summary>Data to be transferred.</summary>
        private object _dataForTransfer;

        /// <summary>Whether the element was dropped in a drag/drop operation.</summary>
        private bool _elementDropped;

        /// <summary>Effects of the drag-drop operation.</summary>
        private DragDropEffects _effects;

        // Data to try:
        // - every data format (on its own)
        // - every data format (in a data object)
        // - a custom data object (with a string - assume it can profile & lie in critical calls)
        // - a custom type that can type-convert into a dangerous DataObject
        // - more dangerous cases:
        //   - for images, fuzzed images
        //   - for xaml, content that loads dangerous objects
        //   - for rtf, fuzzed content
        //   - for xml, maybe stuff that reaches out to the web? (invasion of privacy case)
        // Vector:
        // - copy / paste vrs. drag / drop
        //   - can I add evil xaml during extensibility? -> to extensibility case.

        #endregion Private data.
    }

    /// <summary>
    /// Description of content that can be transferred through the clipboard
    /// or drag/drop.
    /// </summary>
    class TransferContentData
    {
        #region Constructors.

        /// <summary>Initializes a new TransferContentData instance.</summary>
        private TransferContentData(string name, object data, string primaryDataFormat,
            string[] supportedDataFormats, bool isSafeToCopy)
        {
            this.Name = name;
            this.Data = data;
            this.PrimaryDataFormat = primaryDataFormat;
            this.SupportedDataFormats = supportedDataFormats;
            this.IsSafeToCopy = isSafeToCopy;
        }

        #endregion Constructors.

        #region Public properties.

        /// <summary>A name for the content, for debugging purposes.</summary>
        public object Data;

        /// <summary>A name for the content, for debugging purposes.</summary>
        public string PrimaryDataFormat;

        /// <summary>A name for the content, for debugging purposes.</summary>
        public string[] SupportedDataFormats;

        /// <summary>A name for the content, for debugging purposes.</summary>
        public bool IsSafeToCopy;

        /// <summary>A name for the content, for debugging purposes.</summary>
        public string Name;

        /// <summary>Values for testing content in data transfer operations.</summary>
        public static TransferContentData[] Values
        {
            get
            {
                if (s_values == null)
                {
                    s_values = new TransferContentData[] {
                        new TransferContentData("PlainString", "text", DataFormats.UnicodeText,
                            new string[] { DataFormats.UnicodeText, DataFormats.OemText, DataFormats.Text },
                            true),
                        new TransferContentData("XamlDataObject", TransferContentData.CreateXamlDataObject(), DataFormats.Xaml,
                            new string[] { DataFormats.Xaml },
                            false),
                    };
                }
                return s_values;
            }
        }

        #endregion Public properties.

        #region Public methods.

        /// <summary>Returns a string representation of this object.</summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion Public methods.

        #region Private methods.

        private static DataObject CreateXamlDataObject()
        {
            return DataObjectWrapper.CreateXamlDataObject();            
        }

        #endregion Private methods.

        #region Private fields.

        private static TransferContentData[] s_values;

        #endregion Private fields.
    }

    /// <summary>
    /// DataObject API testing for dev work item 4
    /// </summary>
    [Test(0, "DataObject", "DataObjectFormatsTest", MethodParameters = "/TestCaseType:DataObjectFormatsTest", Keywords = "Localization_Suite")]
    [TestOwner("Microsoft"), TestTactics("179"), TestBugs("377"), TestWorkItem("15")]
    public class DataObjectFormatsTest : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            DataObject dataObject = new DataObject();
            switch (_setAPI)
            {
                case "SetAudio":
                    SetAudioInvalidTest();
                    Stream setAudioStream = new MemoryStream();
                    dataObject.SetAudio(setAudioStream);
                    Verifier.Verify(dataObject.ContainsAudio(), "ContainsAudio for setAudioStream should be true.", true);
                    Verifier.Verify(dataObject.GetAudioStream().CanRead, "GetAudioStream().CanRead should be true.", true);

                    byte[] buff = new byte[5];
#pragma warning disable CA2022 // Avoid inexact read
                    setAudioStream.Read(buff, 0, 4);
#pragma warning restore CA2022
                    dataObject.SetAudio(buff);
                    Verifier.Verify(dataObject.ContainsAudio(), "ContainsAudio for byte[] should be true.", true);
                    Verifier.Verify(dataObject.GetAudioStream().CanRead, "GetAudioStream().CanRead for byte[] should be true.", true);
                    break;

                case "SetFileDropList":
                    SetFileDropListInvalidTest();
                    System.Collections.Specialized.StringCollection sc = new System.Collections.Specialized.StringCollection();
                    sc.AddRange(new string[] { "test1.txt", "test2.txt", "test3.txt" });
                    dataObject.SetFileDropList(sc);
                    Verifier.Verify(dataObject.ContainsFileDropList(), "ContainsFileDropList should be true.", true);
                    Verifier.Verify(dataObject.GetFileDropList().Count == 3,
                        "GetFileDropList().Count should be 3 : " + dataObject.GetFileDropList().Count, true);
                    break;

                case "SetImage":
                    SEtImageInvalidTest();
                    BitmapSource bitmapSource = BitmapFrame.Create(new Uri("w.gif", UriKind.RelativeOrAbsolute),
                        BitmapCreateOptions.None, BitmapCacheOption.Default);
                    dataObject.SetImage(bitmapSource);
                    Verifier.Verify(dataObject.ContainsImage(), "ContainsImage() should be true.", true);
                    bitmapSource = dataObject.GetImage();
                    Verifier.Verify(bitmapSource.Height == 50.0, "bitmapSource.Height is 50.0 : " + bitmapSource.Height, true);
                    break;

                case "SetText":
                    SetTextInvalidTest();
                    dataObject.SetText("Hello World");
                    Verifier.Verify(dataObject.ContainsText(), "ContainsText() should be true", true);
                    Verifier.Verify(dataObject.GetText() == "Hello World", "GetText() matched.", true);

                    dataObject.SetText("This is a test", TextDataFormat.Rtf);
                    Verifier.Verify(dataObject.ContainsText(TextDataFormat.Rtf), "ContainsText() should be true", true);
                    Verifier.Verify(dataObject.GetText(TextDataFormat.Rtf) == "This is a test", "GetText(Rtf) matched.", true);
                    break;
            }
            QueueDelegate(NextCombination);
        }

        void SEtImageInvalidTest()
        {
            DataObject dataobject = new DataObject();
            try
            {
                dataobject.SetImage(null);
                throw new Exception("No exception is thrown for invalid parameter for calling DataObject.SetImage(null)!");
            }
            catch (Exception e)
            {
                Verifier.Verify(e is ArgumentNullException, "Wrong exceptoin[" + e.ToString() + "] is caught!");
            }
        }

        void SetTextInvalidTest()
        {
            DataObject dataobject = new DataObject();
            try
            {
                dataobject.SetText(null);
                throw new Exception("No exception is thrown for invalid parameter for calling DataObject.SetText(null)!");
            }
            catch (Exception e)
            {
                Verifier.Verify(e is ArgumentNullException, "Wrong exceptoin[" + e.ToString() + "] is caught!");
            }
            try
            {
                dataobject.SetText(null, TextDataFormat.Text);
                throw new Exception("No exception is thrown for invalid parameter for calling  DataObject.SetText(null, Text)!");
            }
            catch (Exception e)
            {
                Verifier.Verify(e is ArgumentNullException, "Wrong exceptoin[" + e.ToString() + "] is caught!");
            }

            try
            {
                TextDataFormat invalidFormat = (TextDataFormat)1001 ;
                dataobject.SetText("abc", invalidFormat);
                throw new Exception("No exception is thrown for invalid parameter for calling  DataObject.SetText(null, invalidFormat)!");
            }
            catch (Exception e)
            {
                Verifier.Verify(e is InvalidEnumArgumentException, "Wrong exceptoin[" + e.ToString() + "] is caught!");
            }
        }

        void SetAudioInvalidTest()
        {
            DataObject dataobject = new DataObject();
            try
            {
                byte[] bytes =null;
                dataobject.SetAudio(bytes);
                throw new Exception("No exception is thrown for invalid parameter for calling DataObject.SetAudio(byte[])!");
            }
            catch (Exception e)
            {
                Verifier.Verify(e is ArgumentNullException, "Wrong exceptoin[" + e.ToString() + "] is caught!");
            }
            try
            {
                MemoryStream mstream = null;
                dataobject.SetAudio(mstream);
                throw new Exception("No exception is thrown for invalid parameter for calling DataObject.SetAudio(Stream)!");
            }
            catch (Exception e)
            {
                Verifier.Verify(e is ArgumentNullException, "Wrong exceptoin[" + e.ToString() + "] is caught!");
            }

            //no data.
            dataobject.SetAudio(new byte[] { });
        }

        void SetFileDropListInvalidTest()
        {
            System.Collections.Specialized.StringCollection sc = new System.Collections.Specialized.StringCollection();
            DataObject dataobject = new DataObject();
            //invalid data
            object[] objs = { null, sc.Add("-JunckTestVaueForFileDropList-"), new System.Collections.Specialized.StringCollection() };
            for(int i = 0; i<objs.Length; i++)
            {
                try
                {
                    dataobject.SetFileDropList(objs[0] as System.Collections.Specialized.StringCollection);
                    throw new Exception("No Exception is thrown for invalid Parameters!");
                }
                catch (Exception e)
                {
                    Verifier.Verify(e is ArgumentException || e is ArgumentNullException, "Wrong Exception[" + e.ToString() + "] is caught!");
                }
            }

        }
        #endregion Main flow

        #region Private data.
        private string _setAPI = "";
        #endregion Private data.
    }

    /// <summary>
    /// DataObjectConstructorTest
    /// </summary>
    public class DataObjectConstructorTest
    {
        /// <summary>
        /// 
        /// </summary>
        public static void RunTest()
        {
            DataObjectNoParameter();
            DataObjectOneParameter();
            DataObjecttwoParameters1();
            DataObjecttwoParameters2();
            DataObjectThreeParameters();
        }
 
        static void DataObjectThreeParameters()
        {
            Logger.Current.Log("Test DataObject(string format, object data, AutoConvert) constructor...");
            DataObject dataobject = null;
            DataObjectTestData data = null;
            bool autoconvert;
            for(int k = 0; k<2; k++)
            {
                for (int i = 0; i < DataObjectTestData.DataArray.Length; i++)
                {
                    autoconvert = (i==0)? true : false; 
                    data = (DataObjectTestData)DataObjectTestData.DataArray[i];
                    try
                    {
                        dataobject = new DataObject(data.Format, data.Data, autoconvert);
                        if (data.Format == null || data.Format == string.Empty || data.Data == null)
                        {
                            throw new Exception("Invalid argument is not checked!");
                        }
                    }
                    catch (Exception e)
                    {
                        if (!((data.Format == null || data.Format == string.Empty || data.Data == null) && e is ArgumentException))
                        {
                            throw e;
                        }
                    }
                    if (!(data.Format == null || data.Format == string.Empty || data.Data == null))
                    {
                        if (data.Format == DataFormats.Text || data.Format == DataFormats.UnicodeText || data.Format == DataFormats.StringFormat)
                        {
                            string str = dataobject.GetData("Text") as string;

                            Verifier.Verify(str == (string)data.Data, "string won't match! Expected[" + data.Data.ToString() + "], Actual[" + str + "]");

                            string[] formats = dataobject.GetFormats();
                            if (autoconvert)
                            {
                                Verifier.Verify(formats.Length == 3, "We expected 3 formats, Actual[" + formats.Length + "]");
                            }
                            else
                            {
                                Verifier.Verify(formats.Length == 1, "We expected 1 formats, Actual[" + formats.Length + "]");
                                formats = dataobject.GetFormats(true);
                            }
                     
                         }
                        Verifier.Verify(dataobject.GetDataPresent(data.Format), data.Format + " is not in the dataobject!");
                    }
                }
            }
        }

        static void DataObjecttwoParameters1()
        {
            Logger.Current.Log("Test DataObject(Type format, object data) constructor...");
            DataObject dataobject = null;
            DataObjectTestData data = null;
            for (int i = 0; i < DataObjectTestData.DataArray.Length; i++)
            {
                data = (DataObjectTestData)DataObjectTestData.DataArray[i];
                try
                {
                    dataobject = new DataObject(data.Type, data.Data);
                    if (data.Type == null  || data.Data == null)
                    {
                        throw new Exception("Invalid argument is not checked!");
                    }
                }
                catch (Exception e)
                {
                    if (!((data.Type == null || data.Data == null) && e is ArgumentException))
                    {
                        throw e;
                    }
                }
                if (!(data.Type == null || data.Data == null))
                {
                    if (data.Type.FullName == DataFormats.StringFormat)
                    {
                        string str = dataobject.GetData("Text") as string;
                        Verifier.Verify(str == (string)data.Data, "string won't match! Expected[" + data.Data.ToString() + "], Actual[" + str + "]");
                    }
                    Verifier.Verify(dataobject.GetDataPresent(data.Type.FullName), data.Format + " is not in the dataobject!");
                }
            }
        }

        static void DataObjecttwoParameters2()
        {
            Logger.Current.Log("Test DataObject(string format, object data) constructor...");
            DataObject dataobject = null;
            DataObjectTestData data = null; 
            for (int i = 0; i < DataObjectTestData.DataArray.Length; i++)
            {
                data = (DataObjectTestData)DataObjectTestData.DataArray[i];
                try
                {
                    dataobject = new DataObject(data.Format, data.Data);
                    if(data.Format == null || data.Format ==string.Empty || data.Data == null)
                    {
                        throw new Exception("Invalid argument is not checked!");
                    }
                }
                catch (Exception e)
                {
                    if (!((data.Format == null || data.Format == string.Empty || data.Data == null) && e is ArgumentException))
                    {
                        throw e; 
                    }
                }
                if (!(data.Format == null || data.Format == string.Empty || data.Data == null))
                {
                    if (data.Format == DataFormats.Text || data.Format == DataFormats.UnicodeText || data.Format == DataFormats.StringFormat)
                    {
                        string str = dataobject.GetData("Text") as string;
                        Verifier.Verify(str == (string)data.Data, "string won't match! Expected[" + data.Data.ToString() + "], Actual[" + str + "]");
                    }
                    Verifier.Verify(dataobject.GetDataPresent(data.Format), data.Format + " is not in the dataobject!");
                }
            }
        }

        static void DataObjectNoParameter()
        {
            Logger.Current.Log("Test DataObject() constructor...");
            DataObject dObj = new DataObject();
            string[] format1 = dObj.GetFormats();
            string[] format2 = dObj.GetFormats(false);
            string str = "Format1.Length[" + format1.Length + "], " + "Format2.Length[" + format2.Length + "]";
            Verifier.Verify(format1.Length == 0 && format2.Length == 0, "No format when use DataObject()! " + str);
        }

        static void DataObjectOneParameter()
        {
            Logger.Current.Log("Test DataObject(object data) constructor...");
            DataObject dataobject = null ; 
            for (int i = 0; i < DataObjectTestData.DataArray.Length; i++)
            {
                object data = ((DataObjectTestData)DataObjectTestData.DataArray[i]).Data;
                try
                {
                    dataobject = new DataObject(data);
                    
                    if (data == null)
                    {
                        throw new Exception("No null value is accepted for DataObject(object data)!");
                    }
                }
                catch(Exception e)
                {
                    if (!(data == null && e is ArgumentNullException))
                    {
                        throw e; 
                    }
                }

                //verify the dataobject creation.
                if (null != data)
                {
                    if(data is DataObject)
                    {
                        //Regression_Bug354
                        string[] fs1 = dataobject.GetFormats();
                        string[] fs2 = ((DataObject)data).GetFormats();
                        for (int n = 0; n < fs1.Length; n++)
                        {
                            Verifier.Verify(fs1[n] == fs2[n], "Formats won't match at index[" + n + "], fs1[" + fs1[n] + "], fs2[" + fs2[n] + "]");
                        }
                    }
                    else
                    {
                        string[] format1 = dataobject.GetFormats(false); 
                        Verifier.Verify(format1.Length == 1, "No format when use DataObject()! ");
                        Verifier.Verify(data.GetType().ToString() == format1[0], "Format won't match, Expected[" + data.ToString() + "], Actual[" + format1[0] + "]");
                        if (data is System.String)
                        {
                            format1 = dataobject.GetFormats();
                            string[] format2 = dataobject.GetFormats(true);
                            Verifier.Verify(format1.Length == format2.Length, "Formats count won't match, format1[" + format1.Length.ToString() + "], + format2[" + format2.Length.ToString() + "]");
                            for (int j = 0; j < format1.Length; j++)
                            {
                                Verifier.Verify(format1[j] == format2[j], "Formats won't match at index[" + j + "], format1[" + format1[j] + "], format2[" + format2[j] + "]");
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// DataObjectTestData
    /// </summary>
    public class DataObjectTestData
    {
        object _data;
        string _format;
        Type _type; 
        
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="format"></param>
        /// <param name="data"></param>
        public DataObjectTestData(string format, object data)
        {
            _data = data;
            
            _format = format;
            if (data != null)
            {
                _type=data.GetType();
            }
            else
            {
                _type=null; 
            }
        }
        
        /// <summary>
        /// data
        /// </summary>
        public object Data
        {
            get
            {
                return _data;
            }
        }

        /// <summary>
        /// format
        /// </summary>
        public string Format
        {
            get
            {
                return _format;
            }
        }

        /// <summary>
        /// Type format
        /// </summary>
        public Type Type
        {
            get
            {
                return _type; 
            }
        }

        /// <summary>
        /// data array.
        /// </summary>
        public static object[] DataArray = {
            new DataObjectTestData(null, null),
            new DataObjectTestData("a", null),
            new DataObjectTestData(null, "def"),
            new DataObjectTestData(null, 123),
            new DataObjectTestData(new Button().ToString(), new Button()),
            new DataObjectTestData(DataFormats.Text, "Text"),
            new DataObjectTestData(DataFormats.UnicodeText, "Text"),
            new DataObjectTestData(DataFormats.Xaml, "<Avalon>abc</Avalon>"),
            new DataObjectTestData(null, new DataObject("abc")),
        };
    }
}
