// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*************************************************
 *  This file test Clipboard APIs.
 * ************************************************/

namespace DataTransfer
{
    #region Namespaces.
    
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;        

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    
    #endregion Namespaces.

    /// <summary>
    /// Tests convenience methods of Clipboard class. 
    /// (Previously test case # 62)
    /// </summary>
    [Test(0, "Clipboard", "ClipboardAPITest", MethodParameters = "/TestCaseType=ClipboardAPITest", Timeout = 270, Keywords = "Localization_Suite,MicroSuite")]
    [TestOwner("Microsoft"), TestTactics("62"), TestWorkItem("15"), TestLastUpdatedOn("Feb 22, 2007")]
    public class ClipboardAPITest : CustomTestCase
    {
        static string[] s_formats = {DataFormats.Rtf, 
                DataFormats.PenData, 
                DataFormats.Bitmap,
                DataFormats.CommaSeparatedValue,
                DataFormats.Dib, 
                DataFormats.Dif,
                DataFormats.EnhancedMetafile, 
                DataFormats.FileDrop, 
                DataFormats.Html, 
                DataFormats.Locale, 
                DataFormats.MetafilePicture, 
                DataFormats.OemText, 
                DataFormats.Palette, 
                DataFormats.Rtf,
                DataFormats.Riff,
                DataFormats.Serializable,
                DataFormats.StringFormat,
                DataFormats.SymbolicLink,
                DataFormats.Text,
                DataFormats.Tiff,
                DataFormats.UnicodeText,
                DataFormats.WaveAudio,
                DataFormats.Xaml,
                DataFormats.XamlPackage,
                DataFormats.PenData,
            };

        /// <summary>Runs test case</summary>
        public override void RunTestCase()
        {
            CompleteAPITest();

            Logger.Current.ReportSuccess();
        }

        /// <summary>Test the Clipboard API one by one.</summary>
        void CompleteAPITest()
        {
            TestClearMethod();

            TestAudioMethods();

            TestSetDataMethod();

            TestSetFileDropListMethod();

            TestSetImageMothed();

            TestSetTextMethod();

            TestGetDataMethod();

            TestSetDataObjectMethod();

            TestIsCurrentMethod();
        }

        /// <summary>Test the SetDataObject method.</summary>
        void TestSetDataObjectMethod()
        {
            DataObject dobj1;
            DataObject dobj2;
            string data = "abc";

            //Invalid test.
            try
            {
                Clipboard.SetDataObject(null);
                throw new Exception("Clipboard.SetDataObject(object data) accepted a null data.");
            }
            catch (ArgumentException)
            {
                Logger.Current.Log("Clipboard.SetDataObject(object data) rejects null data. ");
            }
            try
            {
                Clipboard.SetDataObject(null, true);
                throw new Exception("Clipboard.SetDataObject(object data, bool copy) accepted a null data.");
            }
            catch (ArgumentException)
            {
                Logger.Current.Log("Clipboard.SetDataObject(object data, bool copy) rejects null data. ");
            }

            dobj1 = new DataObject(DataFormats.Text, data);
            Clipboard.SetDataObject(dobj1);
            dobj2 = Clipboard.GetDataObject() as DataObject;

            //Verify the data for before and after the clipboard seting.
            for (int i = 0; i < s_formats.Length; i++)
            {
                string str1, str2;
                str1 = dobj1.GetData(s_formats[i]) as string;
                str2 = dobj2.GetData(s_formats[i]) as string;
                if (s_formats[i] == DataFormats.OemText)
                {
                    //OemText format is different before and after a dataobject is set on the clipboard.
                }
                else
                {
                    if (s_formats[i] == DataFormats.Text
                       || s_formats[i] == DataFormats.UnicodeText)
                    {
                        Verifier.Verify(str1 == data, "Failed - returned data should be[" + data + "]!");
                    }

                    Verifier.Verify(str1 == str2, "Set object and gotten object should be the same for format[" + s_formats[i] + "]!");
                }
            }
        }

        /// <summary>Test IsCurrent method.</summary>
        void TestIsCurrentMethod()
        {
            DataObject dobj;
            //Invalid Test
            try
            {
                Clipboard.IsCurrent((DataObject)null);
                throw new Exception("Clipboard.IsCurrent(DataObject) accepted a null data.");
            }
            catch (ArgumentException)
            {
                Logger.Current.Log("Clipboard.IsCurrent(DataObject) rejects null data. ");
            }

            //No flush
            dobj = new DataObject(DataFormats.Text, "abc");
            Clipboard.SetDataObject(dobj, false);           //SetData to Clipboard without flush
            IDataObject dataObject = Clipboard.GetDataObject();     //GetData from Clipboard

            Verifier.Verify(Clipboard.IsCurrent(dobj) == true, "No flush. IsCurrent() should return true.", true);
            Verifier.Verify((string)dataObject.GetData(DataFormats.Text, true) == "abc", "dataObject should be the same with[abc]", true);

            //auto flush
            DataObject datao = new DataObject("def");
            Clipboard.SetDataObject(datao);
            Clipboard.GetDataObject();
            Verifier.Verify(Clipboard.IsCurrent(datao) == true, "Flushed IsCurrent should return true for current dataObject...", true);
            Verifier.Verify(Clipboard.IsCurrent(dobj) == false, "Flushed IsCurrent should return false for old dataObject...", true);
        }

        /// <summary>Test Clipbaord.GetData() method.</summary>
        void TestGetDataMethod()
        {
            string data = "abc";

            //Invalid parameter for Clipbaord.GetData.
            try
            {
                Clipboard.GetData(null);
                throw new Exception("format should not be a null value for Clipboard.GetData()!");
            }
            catch (Exception e)
            {
                Verifier.Verify(e is ArgumentNullException, "Get wrong exception! - " + e.ToString());
            }

            try
            {
                Clipboard.GetData(string.Empty);
                throw new Exception("format should not be empty string for Clipboard.GetData()!");
            }
            catch (Exception e)
            {
                Verifier.Verify(e is ArgumentException, "Get wrong exception! - " + e.ToString());
            }
            //GetData when Clipboard is empty.
            Clipboard.Clear();
            for (int i = 0; i < s_formats.Length; i++)
            {
                Verifier.Verify(Clipboard.GetData(s_formats[i]) == null, "Failed no data should be avaliable for[" + s_formats[i] + "] when clipboard is cleard!");
            }

            //Get data for avaliable formats
            Clipboard.SetData(DataFormats.Xaml, data);
            for (int i = 0; i < s_formats.Length; i++)
            {
                if (s_formats[i] == DataFormats.Xaml)
                {
                    string sdata = Clipboard.GetData(s_formats[i]) as string;
                    Verifier.Verify(sdata == data, "Failed - returned data should be[" + data + "]!");
                }
                else
                {
                    Verifier.Verify(Clipboard.GetData(s_formats[i]) == null, "Failed no data should be avaliable for[" + s_formats[i] + "] when clipboard is cleard!");
                }
            }

            //GetConverted data
            Clipboard.SetData(DataFormats.Text, data);

            for (int i = 0; i < s_formats.Length; i++)
            {
                if (s_formats[i] == DataFormats.Text
                    || s_formats[i] == DataFormats.UnicodeText
                    || s_formats[i] == DataFormats.OemText)
                {
                    string sdata = Clipboard.GetData(s_formats[i]) as string;
                    Verifier.Verify(sdata == data, "Failed - returned data should be[" + data + "]!");
                }
                //Locate should be something when the is a format on the clipboard. what is it?
                else if (s_formats[i] == DataFormats.Locale)
                {
                    object o = Clipboard.GetData(s_formats[i]);
                    Verifier.Verify(!(o is string) && o != null, "Failed - Locale data is not correct!");
                }
                else
                {
                    Verifier.Verify(Clipboard.GetData(s_formats[i]) == null, "Failed no data should be avaliable for[" + s_formats[i] + "] when only TextFormat is set!");
                }
            }
        }

        /// <summary>Test Clipboard.SetTextMethod.</summary>
        void TestSetTextMethod()
        {
            //Inlvalid Test:
            try
            {
                Log("Calling Clipboard.SetText(null)...");
                Clipboard.SetText(null);
                throw new Exception("Null value should not be accepted for Clipboard.SetText(text)!");
            }
            catch (Exception e)
            {
                Verifier.Verify(e is ArgumentException, e.ToString() + " is the expected exception!");
            }
            Clipboard.SetText("Hello World");
            Verifier.Verify(Clipboard.ContainsText(), "ContainsText() should be true", true);
            Verifier.Verify(Clipboard.GetText() == "Hello World", "GetText() matched.", true);

            //Invalid test:
            try
            {
                Log("Calling Clipboard.SetText(null, TextDataFormat.Text)...");
                Clipboard.SetText(null, TextDataFormat.Text);
                throw new Exception("Null value should not be accepted for Clipboard.SetText(text, format)!");
            }
            catch (ArgumentException e)
            {
                Log(e.Message);
            }
            try
            {
                Log("Calling Clipboard.SetText(\"This is a test\", (TextDataFormat)InvalidFormat)...");
                int InvalidFormat = 1001;
                Clipboard.SetText("This is a test", (TextDataFormat)InvalidFormat);
                throw new Exception("Invalid format should not be accepted for Clipboard.SetText(text, format)!");
            }
            catch (ArgumentException e)
            {
                Log(e.Message);
            }

            //Test Invalid paramepter for ClipBoard.ContainerTest(format)
            try
            {
                Log("Calling ClipBoard.ContainerTest(invalidformat)...");
                int format = 1001;
                Clipboard.ContainsText((TextDataFormat)format);
                throw new Exception("Invalid format should not be accepted for Clipboard.SetText(text, format)!");
            }
            catch (Exception e)
            {
                Verifier.Verify(e is System.ComponentModel.InvalidEnumArgumentException, "Failed - Get a wrong exception!");
            }

            Clipboard.SetText("This is a test", TextDataFormat.Rtf);
            Verifier.Verify(Clipboard.ContainsText(TextDataFormat.Rtf), "ContainsText() should be true", true);
            Verifier.Verify(Clipboard.GetText(TextDataFormat.Rtf) == "This is a test", "GetText(Rtf) matched.", true);

        }

        /// <summary>Test Clipboard.SetImage()</summary>
        void TestSetImageMothed()
        {
            //Invalid Test:
            try
            {
                Log("Calling Clipboard.SetImage(null)...");
                Clipboard.SetImage(null);
                throw new Exception("Clipboard.SetImage(null) should throw ArgumentNullException!");

            }
            catch (Exception e)
            {
                Verifier.Verify(e is ArgumentException, e.ToString() + " is the expected exception!");
            }

            //return false when no image is on the clipboard.
            Clipboard.SetData(DataFormats.Text, "abc");
            Verifier.Verify(!Clipboard.ContainsImage(), "Failed-ContainsImage() should be false when only text on the clipboard.");

            BitmapSource bitmapSource = BitmapFrame.Create(new Uri("w.gif", UriKind.RelativeOrAbsolute),
                BitmapCreateOptions.None, BitmapCacheOption.Default);
            Clipboard.SetImage(bitmapSource);
            Verifier.Verify(Clipboard.ContainsImage(), "ContainsImage() should be true.", true);
            bitmapSource = null;
            bitmapSource = Clipboard.GetImage();
            Verifier.Verify(bitmapSource.Height == 50.0, "bitmapSource.Height is 50.0 : " + bitmapSource.Height, true);

        }

        /// <summary>
        /// This method tests the Cliboard.Clear() method. We will make sure the following things:
        ///     1. No data or data formats are on the clipboard after it is cleared.
        ///     2. No side effect after more than one clear()s are called in a row. 
        /// </summary>
        void TestClearMethod()
        {
            string[] strs;
            string str;
            IDataObject dobj;

            //Clear() - No data or dataformats left on clipboard()
            Clipboard.SetData(DataFormats.Text, "abc");
            strs = ((IDataObject)Clipboard.GetDataObject()).GetFormats(true);

            //Note: autoconverter should be test somewhere else.
            Verifier.Verify(strs.Length >= 1, "Failed - No data format return after setting text on the Clipboard!");

            //Clear the clibpard and verify the data and dataformats are not avaliable.
            Clipboard.Clear();
            str = Clipboard.GetData(DataFormats.Text) as string;

            //Note: we expected null value instead of string.empty. 
            Verifier.Verify(str == null, "Failed - no data should be on clipboard after ClipBoard.Clear() is called!");

            dobj = Clipboard.GetDataObject() as IDataObject;
            //We should get a dataobject that contains no formats inside. 
            Verifier.Verify(dobj != null && dobj.GetFormats().Length == 0, "Failed - found data on the clipboard(through DataObject) after Clipboard.Clear() is called!");

            //Clear() more than once will have no side effect.
            Clipboard.Clear();
            Clipboard.Clear();
        }

        /// <summary>
        /// Test the Set/Get Audio from Clipboard.
        /// Methods tested:
        ///     SetAudio(Byte[] bytes)
        ///     SetAudio(Stream audioStream)
        ///     GetAudioStream()
        ///     ContainsAudio()
        /// </summary>
        void TestAudioMethods()
        {
            Stream audioStream;
            StreamWriter sWriter;
            //Invalid Test.
            try
            {
                Clipboard.SetAudio((byte[])null);
                throw new Exception("Expect an exception for Clipboard.SetAudio(null as byte[])!");
            }
            catch (Exception e)
            {
                Verifier.Verify(e is ArgumentNullException, "Unexpected exception is throw - " + e.ToString());
            }

            try
            {
                Clipboard.SetAudio((Stream)null);
                throw new Exception("Expect an exception for Clipboard.SetAudio(null Stream)!");
            }
            catch (Exception e)
            {
                Verifier.Verify(e is ArgumentNullException, "Unexpected exception is throw - " + e.ToString());
            }

            //Test Clipboard.ContainsAudio when no audio data is on Clipboard.
            Clipboard.SetData(DataFormats.Text, "abc");
            Verifier.Verify(!Clipboard.ContainsAudio(), "Failed - No audio data should be on the Clipboard when only text is set!");
            Clipboard.Clear();
            Verifier.Verify(!Clipboard.ContainsAudio(), "Failed - No audio data should be on the Clipboard after it is cleared");

            //GetAudioStream returns null value when no audio on the clipbard.
            Verifier.Verify(Clipboard.GetAudioStream() == null, "Failed - No Audio stream on clipboard when it is cleared.", true);


            //Test SetAudio() methods.
            audioStream = new MemoryStream();
            sWriter = new StreamWriter(audioStream);
            sWriter.Write("audio Data");
            audioStream.Seek(0, SeekOrigin.Begin);

            Clipboard.SetAudio(audioStream);
            Verifier.Verify(Clipboard.ContainsAudio(), "ContainsAudio for setAudioStream should be true.", true);

            //Regression_Bug19 - Editing: OutofmemoryException is thrown when trying to get the bad Audio data on the clipboard.
            //Enable the following code when the Regression_Bug19 is fixed.
            //Verifier.Verify(Clipboard.GetAudioStream().CanRead, "GetAudioStream().CanRead should be true.", true);

            audioStream = new FileStream("Beethoven.wma", FileMode.Open);
            Clipboard.SetAudio(audioStream);
            Verifier.Verify(Clipboard.ContainsAudio(), "ContainsAudio for setAudioStream should be true.", true);
            Verifier.Verify(Clipboard.GetAudioStream().CanRead, "GetAudioStream().CanRead should be true.", true);

            Clipboard.Clear();
            Verifier.Verify(!Clipboard.ContainsAudio(), "Failed: Clipboard should contains no audio format when it is cleared!");
            byte[] buff = new byte[5];
#pragma warning disable CA2022 // Avoid inexact read
            audioStream.Read(buff, 0, 4);
#pragma warning restore CA2022
            Clipboard.SetAudio(buff);
            Verifier.Verify(Clipboard.ContainsAudio(), "ContainsAudio for byte[] should be true.", true);
            Verifier.Verify(Clipboard.GetAudioStream().CanRead, "GetAudioStream().CanRead for byte[] should be true.", true);
        }

        /// <summary>Test Clipboard.SetData()</summary>
        void TestSetDataMethod()
        {
            object[] obs = { DataFormats.Text, null };
            for (int i = 0; i < obs.Length; i++)
            {
                for (int k = 0; k < obs.Length; k++)
                {
                    try
                    {
                        //when i == 0; k ==0 the parameter are valid.
                        if (!(i == 0 && k == 0))
                        {
                            Clipboard.SetData(obs[i] as string, obs[k]);
                            throw new Exception("Expected an excpetion for invalid parameter of Clipboard.SetData()");
                        }
                    }
                    catch (Exception e)
                    {
                        Verifier.Verify(e is ArgumentNullException, "Unexpected exception is throw - " + e.ToString());
                    }

                }
            }

            //Invalid Test for Clipboard.ContainsData(null)
            try
            {
                Log("Calling Clipboard.ContainsData(null) ...");
                Clipboard.ContainsData(null);
                throw new Exception("Format string can't be null!");
            }
            catch (Exception e)
            {
                Verifier.Verify(e is ArgumentNullException, "Wrong exception is caught! [" + e.ToString() + "]");
            }

            //Invalid Test for Clipboard.ContainsData(null)
            try
            {
                Log("Calling Clipboard.ContainsData(string.Empty) ...");
                Clipboard.ContainsData(string.Empty);
                throw new Exception("Format string can't be empty string!");
            }
            catch (Exception e)
            {
                Verifier.Verify(e is ArgumentException, "Wrong exception is caught! [" + e.ToString() + "]");
            }

            //ContainsData return false; 
            Verifier.Verify(!Clipboard.ContainsData(DataFormats.UnicodeText + "junckformat"), "ContainsData() should be false.", true);

            Clipboard.SetData(DataFormats.UnicodeText, "Hello World");
            Verifier.Verify(Clipboard.ContainsData(DataFormats.UnicodeText), "ContainsData() should be true.", true);
            Verifier.Verify(Clipboard.GetData(DataFormats.UnicodeText).ToString() == "Hello World", "Data matched.", true);
        }

        /// <summary>Test Clipboard.SetFileDropList() method</summary>
        void TestSetFileDropListMethod()
        {
            //Invalid Test:
            object[] objs = new object[3];
            objs[0] = null;
            objs[1] = new System.Collections.Specialized.StringCollection();
            objs[2] = new System.Collections.Specialized.StringCollection().Add("---junck----");

            for (int i = 0; i < objs.Length; i++)
            {
                try
                {
                    Clipboard.SetFileDropList(objs[i] as System.Collections.Specialized.StringCollection);
                    throw new Exception("Can't get the Exception for invalid parameters of Clipboard.SetFileDropList()!");
                }
                catch (Exception e)
                {
                    Verifier.Verify(e is ArgumentException || e is ArgumentNullException, "Get unexpected exception - " + e.ToString());
                }
            }

            Clipboard.Clear();

            //No format found scenario
            Verifier.Verify(!Clipboard.ContainsFileDropList(), "Failed - Clipboard.ContainsFileDropList() should be false after the clipboard is cleared.");

            System.Collections.Specialized.StringCollection sc = new System.Collections.Specialized.StringCollection();
            sc.AddRange(new string[] { "test1.txt", "test2.txt", "test3.txt" });
            Clipboard.SetFileDropList(sc);
            Verifier.Verify(Clipboard.ContainsFileDropList(), "ContainsFileDropList should be true.", true);
            Verifier.Verify(Clipboard.GetFileDropList().Count == 3,
                "GetFileDropList().Count should be 3 : " + Clipboard.GetFileDropList().Count, true);
        }
    }

    /// <summary>
    /// Performs Clipboard actions which are called from ClipboardFormatsTest. This class is
    /// not a test by itself. ClipboardFlushTest calls instances of this class.
    /// </summary>
    public class DoClipboardAction : CustomTestCase
    {
        /// <summary>Runs test case</summary>
        public override void RunTestCase()
        {
            string action;
            DataObject dobj;
            string setData = "TestData";

            action = Settings.GetArgument("Action").ToLower();

            //Set data without flush.
            if (action == "setdatanoflush")
            {
                dobj = new DataObject(DataFormats.Text, setData);
                Log("SetData with no flush");
                Clipboard.SetDataObject(dobj, false);
            }
            //Get data to verify no flush scenario, we should not see the data after app is killed.
            else if (action == "getdatanotflushed")
            {                
                string str = Clipboard.GetData(DataFormats.Text) as string;                
                Verifier.Verify(str == null, "Verifying GetData when data is set with no flush. " +
                    "Clipboard data [" + str + "], Expected []");                
            }
            //Set data with flush.
            else if (action == "setdataflush")
            {
                dobj = new DataObject(DataFormats.Text, setData);
                Log("SetData with flush = true");
                Clipboard.SetDataObject(dobj, true);
            }
            //Get data to verify the flush, we should see the set data on the clipboard after the app is gone.
            else if (action == "getdataflushed")
            {
                string str = Clipboard.GetData(DataFormats.Text) as string;                
                Verifier.Verify(str == setData, "Verifying GetData when data is set with flush = true. " +
                    "Clipboard data [" + str + "], Expected [" + setData + "]");
            }

            Logger.Current.ReportSuccess();
        }
    }

    /// <summary>
    /// Clipboard API testing. For testing the flush, we have to start 
    /// a app for several times. Hence we launch processes in this test case to perform
    /// the clipboard actions.
    /// </summary>
    [Test(0, "Clipboard", "ClipboardFlushTest", MethodParameters = "/TestCaseType=ClipboardFlushTest", Timeout = 270)]    
    [TestOwner("Microsoft"), TestTactics("63"), TestBugs("19, 389"), TestWorkItem("15"), TestLastUpdatedOn("Feb 22, 2007")]
    public class ClipboardFlushTest : CustomTestCase
    {
        #region Main flow.        

        int _timeout = 1000 * 60;

        /// <summary>Runs a specific combination.</summary>
        public override void  RunTestCase()
        {
            SetDataNoFlush();
            GetDataNotFlushed();
            SetDataFlush();
            GetDataFlushed();

            Logger.Current.ReportSuccess();
        }

        private void SetDataNoFlush()
        {
            Process process = new Process();
            process.StartInfo = CreateProcessStartInfo("/TestCaseType=DoClipboardAction /Action=SetDataNoFlush");

            Log("Launching process to SetData with no flush...");
            process.Start();
            try
            {
                process.WaitForExit(_timeout);
            }
            finally
            {
                if (!process.HasExited)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception e)
                    {
                        Log("Unable to kill test case process");
                        Log(e.ToString());
                    }
                }
            }
        }

        private void GetDataNotFlushed()
        {
            Process process = new Process();
            process.StartInfo = CreateProcessStartInfo("/TestCaseType=DoClipboardAction /Action=GetDataNotFlushed");

            Log("Launching process to GetData when data is set with no flush...");
            process.Start();
            try
            {
                process.WaitForExit(_timeout);
            }
            finally
            {
                if (!process.HasExited)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception e)
                    {
                        Log("Unable to kill test case process");
                        Log(e.ToString());
                    }
                }
            }
        }

        private void SetDataFlush()
        {
            Process process = new Process();
            process.StartInfo = CreateProcessStartInfo("/TestCaseType=DoClipboardAction /Action=SetDataFlush");

            Log("Launching process to SetData with flush = true...");
            process.Start();
            try
            {
                process.WaitForExit(_timeout);
            }
            finally
            {
                if (!process.HasExited)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception e)
                    {
                        Log("Unable to kill test case process");
                        Log(e.ToString());
                    }
                }
            }
        }

        private void GetDataFlushed()
        {
            Process process = new Process();
            process.StartInfo = CreateProcessStartInfo("/TestCaseType=DoClipboardAction /Action=GetDataFlushed");

            Log("Launching process to GetData when data is set with flush = true...");
            process.Start();
            try
            {
                process.WaitForExit(_timeout);
            }
            finally
            {
                if (!process.HasExited)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception e)
                    {
                        Log("Unable to kill test case process");
                        Log(e.ToString());
                    }
                }
            }
        }

        private ProcessStartInfo CreateProcessStartInfo(string arguments)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("EditingTest.exe", arguments);
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;

            return processStartInfo;
        }

        #endregion Main flow
    }
}
