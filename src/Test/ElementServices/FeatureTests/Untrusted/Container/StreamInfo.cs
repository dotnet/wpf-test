// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.IO;
using System.IO.Packaging;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;

using Microsoft.Test.Container;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.Container
{
    /******************************************************************************
    * CLASS:          BVT_StreamInfo
    ******************************************************************************/
    /// <summary>
    /// Cases for the StreamInfo class
    /// </summary>    
    /// <remarks>
    /// class inherits from TestCase, where ContextEnteringSupport will automatically create
    /// and enter a context when the constructor is called, and will automatically exit and 
    /// dispose the context when the destructor is called.
    /// </remarks>
    [Test(1, "Container.StreamInfo", TestCaseSecurityLevel.FullTrust, "BVT_StreamInfo")]
    public class BVT_StreamInfo : TestCase
    {
        #region Private Data
        private string _resultMsg = "Unknown failure";
        private string _testName = "";
        #endregion


        #region Constructor
        [Variation("StreamInfo1")]
        [Variation("StreamInfo2")]
        [Variation("StreamInfo3")]
        [Variation("StreamInfo4")]
        [Variation("StreamInfo5")]
        [Variation("StreamInfo6")]
        [Variation("StreamInfo7", Priority=0)]
        [Variation("StreamInfo8")]
        [Variation("StreamInfo9", Priority=0)]
        [Variation("StreamInfo10")]
        [Variation("StreamInfo11")]
        [Variation("StreamInfo12")]
        [Variation("StreamInfo13", Priority=0)]
        [Variation("StreamInfo14")]
        [Variation("StreamInfo15")]
        [Variation("StreamInfo16")]
        [Variation("StreamInfo17")]
        [Variation("StreamInfo18")]
        [Variation("StreamInfo19")]
        [Variation("StreamInfo20")]
        [Variation("StreamInfo21")]
        [Variation("StreamInfo22")]
        [Variation("StreamInfo23", Priority=0)]
        [Variation("StreamInfo24")]
        [Variation("StreamInfo25")]
        [Variation("StreamInfo26")]

        /******************************************************************************
        * Function:          BVT_StreamInfo Constructor
        ******************************************************************************/
        public BVT_StreamInfo(string arg): base(TestCaseType.ContextEnteringSupport)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            switch (_testName)
            {
                case "StreamInfo1":
                    StreamInfo1();
                    break;
                case "StreamInfo2":
                    StreamInfo2();
                    break;
                case "StreamInfo3":
                    StreamInfo3();
                    break;
                case "StreamInfo4":
                    StreamInfo4();
                    break;
                case "StreamInfo5":
                    StreamInfo5();
                    break;
                case "StreamInfo6":
                    StreamInfo6();
                    break;
                case "StreamInfo7":
                    StreamInfo7();
                    break;
                case "StreamInfo8":
                    StreamInfo8();
                    break;
                case "StreamInfo9":
                    StreamInfo9();
                    break;
                case "StreamInfo10":
                    StreamInfo10();
                    break;
                case "StreamInfo11":
                    StreamInfo11();
                    break;
                case "StreamInfo12":
                    StreamInfo12();
                    break;
                case "StreamInfo13":
                    StreamInfo13();
                    break;
                case "StreamInfo14":
                    StreamInfo14();
                    break;
                case "StreamInfo15":
                    StreamInfo15();
                    break;
                case "StreamInfo16":
                    StreamInfo16();
                    break;
                case "StreamInfo17":
                    StreamInfo17();
                    break;
                case "StreamInfo18":
                    StreamInfo18();
                    break;
                case "StreamInfo19":
                    StreamInfo19();
                    break;
                case "StreamInfo20":
                    StreamInfo20();
                    break;
                case "StreamInfo21":
                    StreamInfo21();
                    break;
                case "StreamInfo22":
                    StreamInfo22();
                    break;
                case "StreamInfo23":
                    StreamInfo23();
                    break;
                case "StreamInfo24":
                    StreamInfo24();
                    break;
                case "StreamInfo25":
                    StreamInfo25();
                    break;
                case "StreamInfo26":
                    StreamInfo26();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public and Protected Members
        /******************************************************************************
        * Function:          StreamInfo1
        ******************************************************************************/
        /// <summary>
        /// Try to create a stream twice. Validation is done by catching the appropriate
        /// exception. This call is expected to fail.
        /// </summary>        
        public void StreamInfo1()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;            
            FileInfo  fInfo = null;
            bool        bResult = false;
            string      resultMsg = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            GlobalLog.LogStatus("Create a stream.");
            stInfo = stRoot.CreateStream( "TestStream" );
            stRoot.Close();
            // ***** End Avalon Test ****

            // ***** Start Validation Section ****
            try
            {
                stRoot = StorageRootWrapper.Open( "container.xmf" );
                // this is expected to fail
                stInfo = stRoot.CreateStream( "TestStream" );

                bResult = false;
                resultMsg = "StorageRootWrapper.CreateStream succeeded when it should have failed.";
            }
            catch (IOException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected IOException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence( bResult + " " + resultMsg );
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          StreamInfo2
        ******************************************************************************/
        /// <summary>
        /// Create a stream via StorageInfo.CreateStream, and then StreamInfo.GetStream(FileMode.Create). 
        /// Validation is done by trying to create the stream again, which should fail.
        /// </summary>
         public void StreamInfo2()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            Stream      fStream = null;
            FileInfo  fInfo = null;
            bool        bResult = false;
            string      resultMsg = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            GlobalLog.LogStatus("Create a stream.");
            stInfo = stRoot.CreateStream( "TestStream" );
            fStream = stInfo.GetStream(FileMode.Create);
            fStream.Close();
            stRoot.Close();
            // ***** End Avalon Test ****

            // ***** Start Validation Section ****
            try
            {
                stRoot = StorageRootWrapper.Open( "container.xmf" );
                // this is expected to fail
                stInfo = stRoot.CreateStream( "TestStream" );

                bResult = false;
                resultMsg = "StorageRootWrapper.CreateStream succeeded when it should have failed.";
            }
            catch (IOException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected IOException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StreamInfo13
        ******************************************************************************/
        /// <summary>
        /// Create new streams via StorageInfo.CreateStream. 
        /// Validation is done by making sure StorageInfo.StreamExists returns true.
        /// Also, validates the CompressionOption and EncryptionOption overload of CreateStream.
        /// </summary>
        public void StreamInfo3()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo stInfo = null;
            FileInfo fInfo = null;
            
            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo("container.xmf");
            if (fInfo.Exists)
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open("container.xmf");
            fInfo = new FileInfo("container.xmf");
            // check if the file exists
            if (!fInfo.Exists)
            {
                GlobalLog.LogEvidence("ERROR: Container file not created");
                return;
            }
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            GlobalLog.LogStatus("Create new stream.");
            stInfo = stRoot.CreateStream("TestStream");
            // ***** End Avalon Test ****

            // ***** Avalon Test ****
            GlobalLog.LogStatus("Create new stream.");
            stInfo = stRoot.CreateStream("TestStream2", CompressionOption.Fast, EncryptionOption.None);
            // ***** End Avalon Test ****

            stRoot.Close();

            // ***** Start Validation Section ****
            stRoot = StorageRootWrapper.Open("container.xmf");
            
            // Verify stream that has default options.
            if (!stRoot.StreamExists("TestStream")) throw new Microsoft.Test.TestValidationException("FAILED");
            stInfo = stRoot.GetStreamInfo("TestStream");
            if(stInfo.CompressionOption != CompressionOption.NotCompressed) throw new Microsoft.Test.TestValidationException("FAILED");
            if(stInfo.EncryptionOption != EncryptionOption.None) throw new Microsoft.Test.TestValidationException("FAILED");

            // Verify stream that has explicit compression and encryption options.
            // Limited implementation in Avalon V1 means that we can only verify that
            // CompressionOption is NotCompressed or not NotCompressed.
            if (!stRoot.StreamExists("TestStream")) throw new Microsoft.Test.TestValidationException("FAILED");
            stInfo = stRoot.GetStreamInfo("TestStream2");
            if (stInfo.CompressionOption == CompressionOption.NotCompressed) throw new Microsoft.Test.TestValidationException("FAILED");
            if (stInfo.EncryptionOption != EncryptionOption.None) throw new Microsoft.Test.TestValidationException("FAILED");
            
            GlobalLog.LogEvidence("PASS: Test Passed");

            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StreamInfo4
        ******************************************************************************/
        /// <summary>
        /// Create new stream via StorageInfo.CreateStream, and then StreamInfo.GetStream(FileMode.Create). 
        /// Validation is done by making sure StorageInfo.StreamExists returns true.
        /// </summary>
        public void StreamInfo4()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            Stream      fStream = null;
            FileInfo  fInfo = null;
            bool        bResult = false;
            string      resultMsg = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            GlobalLog.LogStatus("Create new stream.");
            stInfo = stRoot.CreateStream( "TestStream" );
            fStream = stInfo.GetStream(FileMode.Create);
            fStream.Close();
            stRoot.Close();
            // ***** End Avalon Test ****

            // ***** Start Validation Section ****
            stRoot = StorageRootWrapper.Open("container.xmf");
            if(stRoot.StreamExists( "TestStream" ))
            {
                bResult = true;                
                resultMsg = "Test passed";
            }
            else
            {
                bResult = false;
                resultMsg = "CreateStream() followed by GetStream(FileMode.Create) makes StreamExists return false";
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        
        /******************************************************************************
        * Function:          StreamInfo5
        ******************************************************************************/
        /// <summary>
        /// GetStream with FileMode.Open. A container with one stream is created beforehead.
        /// Validation is done by making sure returned stream is not null.
        /// </summary>
        public void StreamInfo5()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            Stream      fStream = null;
            FileInfo  fInfo = null;
            bool        bResult = false;
            string      resultMsg = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }

            GlobalLog.LogStatus("Create a stream");
            stInfo = stRoot.CreateStream( "TestStream" );
            stRoot.Close();
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            stRoot = StorageRootWrapper.Open("container.xmf");
            GlobalLog.LogStatus("Open the stream.");
            stInfo = stRoot.GetStreamInfo( "TestStream" );
            fStream = stInfo.GetStream(FileMode.Open);
            // ***** End Avalon Test ****
            // ***** Start Validation Section  ****
            if ( fStream != null )
            {
                fStream.Close();
                bResult = true;
                resultMsg = "Test passed";
            }
            else
            {
                bResult = false;
                resultMsg = "GetStream(FileMode.Open) returns null even if the stream exists.";
            }

            GlobalLog.LogEvidence( bResult + " " + resultMsg );
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          StreamInfo6
        ******************************************************************************/
        /// <summary>
        /// GetStream on existing stream with FileMode.CreateNew - should fail, since CreateNew is not supported. 
        /// Validation is done by catching the exception.
        /// </summary>
        public void StreamInfo6()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            Stream      fStream = null;
            FileInfo  fInfo = null;
            bool        bResult = false;
            string      resultMsg = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created");
                return;
            }
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            GlobalLog.LogStatus("Create a new stream.");
            stInfo = stRoot.CreateStream("TestStream");
            // ***** End Avalon Test ****
            try
            {
                // ***** Start Validation Section ****
                fStream = stInfo.GetStream(FileMode.CreateNew);
                fStream.Close();
                stRoot.Close();

                bResult = false;
                resultMsg = "GetStream(FileMode.CreateNew) doesn't throw, even if the stream already exists.";
            }
            catch (ArgumentException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected ArgumentException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StreamInfo7
        ******************************************************************************/
        /// <summary>
        /// Verify that GetStream(OpenOrCreate) acts as GetStream(Open) if the stream exists.
        /// A container is created with a stream in which "This is a test" is written. 
        /// The stream is then opened with GetStream(OpenOrCreate). 
        /// Validation is done by verifying the stream content. Stream should be preserved 
        /// and not overwritten.
        /// </summary>
        public void StreamInfo7()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            Stream      fStream = null;
            StreamWriter wrt   = null;
            StreamReader rdr   = null;
            FileInfo  fInfo = null;
            bool        bResult = false;
            string      resultMsg = "Unknown failure";
            string      line;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created");
                return;
            }

            GlobalLog.LogStatus("Create a new stream");
            stInfo = stRoot.CreateStream( "TestStream" );
            fStream = stInfo.GetStream();
            wrt = new StreamWriter( fStream );
            wrt.WriteLine("This is a test");
            wrt.Close();
            fStream.Close();
            stRoot.Close();
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            stRoot = StorageRootWrapper.Open("container.xmf");
            GlobalLog.LogStatus("Open the stream.");
            stInfo = stRoot.GetStreamInfo( "TestStream" );
            fStream = stInfo.GetStream(FileMode.OpenOrCreate);
            // ***** End Avalon Test ****

            // ***** Start Validation Section ****
            rdr = new StreamReader( fStream );
            line = rdr.ReadLine();
            if ( line == "This is a test" )
            {
                bResult = true;
                resultMsg = "Test passed";
            }
            else
            {
                bResult = false;
                resultMsg = "GetStream(OpenOrCreate) doesn't act like GetStream(Open) even if the stream exists.";
            }
            rdr.Close();
            fStream.Close();

            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          StreamInfo8
        ******************************************************************************/
        /// <summary>
        /// Open an existing stream, readonly. 
        /// Validation is done by trying to write to the stream, and verifying that 
        /// an exception is thrown.
        /// </summary>
        public void StreamInfo8()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo      = null;
            Stream      fStream     = null;
            StreamWriter wrt        = null;
            FileInfo  fInfo       = null;
            bool        bResult     = false;
            string      resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }
            stInfo = stRoot.CreateStream( "TestStream" );
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            GlobalLog.LogStatus("Open the stream.");
            fStream = stInfo.GetStream(FileMode.Open, FileAccess.Read);
            // ***** End Avalon Test ****
            // ***** Start Validation Section  ****
            try
            {
                wrt = new StreamWriter( fStream );
                wrt.WriteLine( "This is a test" );
                wrt.Close();

                bResult = false;
                resultMsg = "Stream opened in ReadOnly mode allows writes.";
            }
            catch (ArgumentException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected ArgumentException occurred: " + e.Message;
            }
            fStream.Close();

            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StreamInfo9
        ******************************************************************************/
        /// <summary>
        /// Check the Name property
        /// Validation is done by comparing the property value with expected value
        /// </summary>
        public void StreamInfo9()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            string      stName = null;
            FileInfo  fInfo = null;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created");
                return;
            }
            stInfo = stRoot.CreateStream( "TestStream" );
            // ***** End Test Cases Initialization ****
            // ***** Avalon Test ****
            GlobalLog.LogStatus("Checking Name property.");
            stName = stInfo.Name;
            // ***** End Avalon Test ****
            // ***** Start Validation Section  ****
            if (stName != "TestStream")
            {
                GlobalLog.LogEvidence("FAIL: StreamInfo.Name property not as expected");
            }
            else
            {
                GlobalLog.LogEvidence("PASS: Test passed");
            }
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StreamInfo10
        ******************************************************************************/
        /// <summary>
        /// Create a stream with GetStream(FileMode.Create, FileAccess.Read). Validation is done 
        /// by making sure that StreamExists returns true.
        /// </summary>
        public void StreamInfo10()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            Stream      fStream = null;
            FileInfo  fInfo = null;
            bool        bResult = false;
            string      resultMsg = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created");
                return;
            }
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            GlobalLog.LogStatus("Create a stream.");
            stInfo = stRoot.CreateStream( "TestStream" );
            // ***** End Avalon Test ****

            // ***** Start Validation Section ****
            try
            {            
                fStream = stInfo.GetStream(FileMode.Create, FileAccess.Read);            
                fStream.Close();
    
                bResult = false;
                resultMsg = "StreamInfo.GetStream(FileMode.Create, FileAccess.Read) was expected to fail";
            }
            catch (ArgumentException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected ArgumentException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StreamInfo11
        ******************************************************************************/
        /// <summary>
        /// Open an existing stream with FileAccess.Write. 
        /// Validation is done by trying to read from the stream, 
        /// and making sure it fails.
        /// </summary>
        public void StreamInfo11()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo  = null;
            Stream      fStream = null;
            StreamWriter wrt    = null;
            StreamReader rdr    = null;
            FileInfo  fInfo   = null;
            bool        bResult = false;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }
            stInfo = stRoot.CreateStream( "TestStream" );
            fStream = stInfo.GetStream();
            wrt = new StreamWriter( fStream );
            wrt.WriteLine( "This is a test" );
            wrt.Close();
            fStream.Close();
            stRoot.Close();

            stRoot = StorageRootWrapper.Open( "container.xmf" );
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            GlobalLog.LogStatus("Open a stream.");
            stInfo = stRoot.GetStreamInfo( "TestStream" );
            try
            {
                fStream = stInfo.GetStream(FileMode.Open, FileAccess.Write);
                // ***** End Avalon Test ****

                // ***** Start Validation Section  ****                
                rdr = new StreamReader( fStream );
                string line = rdr.ReadLine();
                rdr.Close();
                bResult = false;
                _resultMsg = "WriteOnly stream should not allow reading.";
            }
            catch (ArgumentException e)
            {
                bResult = true;
                _resultMsg = "Test passed:  the expected ArgumentException occurred: " + e.Message;
            }
            fStream.Close();
            GlobalLog.LogEvidence(bResult + " " + _resultMsg);
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          StreamInfo12
        ******************************************************************************/
        /// <summary>
        /// Open a stream with FileMode.Open, FileAccess=0.
        /// FileAccess = 0 is invalid argument, this call is expected to fail.
        /// Validation is done by checking and verifying the exception thrown.
        /// </summary>
        public void StreamInfo12()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            Stream      fStream = null;
            FileInfo  fInfo = null;
            bool        bResult = false;
            string      resultMsg = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }
            GlobalLog.LogStatus("Create test stream.");
            stInfo = stRoot.CreateStream( "TestStream" );
            // ***** End Test Cases Initialization ****

            try
            {
                // ***** Avalon Test ****
                GlobalLog.LogStatus("Open a stream.");
                fStream = stInfo.GetStream(FileMode.Open, 0);
                // ***** End Avalon Test ****

                // ***** Start Validation Section ****
                fStream.Close();
                bResult = false;
                resultMsg = "StreamInfo.GetStream(FileMode.Open, 0) was expected to fail";
            }
            catch (ObjectDisposedException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected ObjectDisposedException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StreamInfo13
        ******************************************************************************/
        /// <summary>
        /// Delete a stream. A container with one stream is created beforehead.
        /// Validation is done by checking that StreamExists returns false.
        /// </summary>
        public void StreamInfo13()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;            
            FileInfo  fInfo = null;
            bool        bResult = false;
            string      resultMsg = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created");
                return;
            }
            stInfo = stRoot.CreateStream( "TestStream" );
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            GlobalLog.LogStatus("Delete a stream.");
            stRoot.DeleteStream( "TestStream" );
            // ***** End Avalon Test ****
            // ***** Start Validation Section  ****
            if ( !stRoot.StreamExists("TestStream") )
            {
                bResult = true;
                resultMsg = "Test passed";
            }
            else
            {
                bResult = false;
                resultMsg = "Stream exists even after deleting.";
            }

            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }



        /******************************************************************************
        * Function:          StreamInfo14
        ******************************************************************************/
        /// <summary>
        /// Create a stream with GetStream(FileMode.Create), while the stream is still open. 
        /// Validation is done by checking the Length property on the stream.
        /// </summary>
        public void StreamInfo14()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            Stream      fStream = null;
            FileInfo  fInfo = null;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created");
                return;
            }
            stInfo = stRoot.CreateStream( "TestStream" );
            fStream = stInfo.GetStream();
            fStream.Write( new Byte[512], 0, 512 );
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            GlobalLog.LogStatus("Try to create the stream again.");
            fStream = stInfo.GetStream(FileMode.Create);
            fStream.Close();
            // ***** End Avalon Test ****

            // ***** Start Validation Section ****
            fStream = stInfo.GetStream();
            if ( fStream.Length == 0 )
            {
                GlobalLog.LogEvidence("PASS: Test passed");
            }
            else
            {
                GlobalLog.LogEvidence("FAIL: Can't use GetStream(FileMode.Create), while the stream is open.");
            }
            fStream.Close();
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }



        /******************************************************************************
        * Function:          StreamInfo15
        ******************************************************************************/
        /// <summary>
        /// Try the GetStreamInfo operation while the stream is open for writing. 
        /// Validation is done by comparing the Name property of the StreamInfo with expected value.
        /// </summary>
        public void StreamInfo15()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            Stream      fStream = null;
            FileInfo  fInfo = null;            

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }
            stInfo = stRoot.CreateStream( "TestStream" );
            fStream = stInfo.GetStream();
            fStream.Write( new Byte[512], 0, 512 );
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            GlobalLog.LogStatus("Try GetStreamInfo.");
            stInfo = stRoot.GetStreamInfo( "TestStream" );
            // ***** End Avalon Test ****

            // ***** Start Validation Section ****
            if ( stInfo.Name == "TestStream" )
            {
                GlobalLog.LogEvidence("PASS: Test passed");
            }
            else
            {
                GlobalLog.LogEvidence("FAIL: GetStreamInfo() has issues if the stream is open.");
            }
            fStream.Close();
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StreamInfo16
        ******************************************************************************/
        /// <summary>
        /// Create a stream with the same name as existing storage. This is expected to fail.
        /// Validation is done by catching the appropriate exception.
        /// </summary>
        public void StreamInfo16()
        {
            StorageRootWrapper stRoot = null;
            StorageInfo strInfo = null;
            StreamInfo  stInfo = null;
            FileInfo  fInfo = null;
            bool        bResult = false;
            string      resultMsg = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created");
                return;
            }
            strInfo = stRoot.CreateSubStorage( "TestName" );
            // ***** End Test Cases Initialization ****

            try
            {
                // ***** Avalon Test ****
                GlobalLog.LogStatus("Create a stream.");
                stInfo = stRoot.CreateStream( "TestName" );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                bResult = false;
                resultMsg = "It is possible to create a stream and a substorage with the same name, at the same level.";
            }
            catch (InvalidOperationException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected InvalidOperationException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          StreamInfo17
        ******************************************************************************/
        /// <summary>
        /// Open a stream with FileMode.Append. This is not supported.
        /// Validation is done by catching the appropriate exception.
        /// </summary>
        public void StreamInfo17()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            Stream      fStream = null;
            FileInfo  fInfo = null;
            bool        bResult = false;
            string      resultMsg = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created");
                return;
            }

            stInfo = stRoot.CreateStream( "TestStream" );
            // ***** End Test Cases Initialization ****
            try
            {            
                // ***** Avalon Test ****
                GlobalLog.LogStatus("Open a stream.");
                fStream = stInfo.GetStream(FileMode.Append);
                fStream.Close();
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                bResult = false;
                resultMsg = "GetStream(FileMode.Append) should throw, since FileMode.Append is not supported.";
            }
            catch (ArgumentException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected ArgumentException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          StreamInfo18
        ******************************************************************************/
        /// <summary>
        /// Open a stream with FileMode.Truncate. This is not supported.
        /// Validation is done by catching the appropriate exception.
        /// </summary>
        public void StreamInfo18()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            Stream      fStream = null;
            FileInfo  fInfo = null;
            bool        bResult = false;
            string      resultMsg = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created");
                return;
            }

            stInfo = stRoot.CreateStream( "TestStream" );
            // ***** End Test Cases Initialization ****
            try
            {            
                // ***** Avalon Test ****
                GlobalLog.LogStatus("Open a stream.");
                fStream = stInfo.GetStream(FileMode.Truncate);
                fStream.Close();
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                bResult = false;
                resultMsg = "GetStream(FileMode.Truncate) should throw, since FileMode.Truncate is not supported.";
            }
            catch (ArgumentException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected ArgumentException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StreamInfo19
        ******************************************************************************/
        /// <summary>
        /// Open a stream with invalid FileMode value of 0. This is expected to fail.
        /// Validation is done by catching the appropriate exception.
        /// </summary>
        public void StreamInfo19()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            Stream      fStream = null;
            FileInfo  fInfo = null;
            bool        bResult = false;
            string      resultMsg = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }
            stInfo = stRoot.CreateStream( "TestStream" );
            // ***** End Test Cases Initialization ****

            try
            {
                // ***** Avalon Test ****
                GlobalLog.LogStatus("Open a stream.");
                fStream = stInfo.GetStream((FileMode)0);
                fStream.Close();
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                bResult = false;
                resultMsg = "GetStream((FileMode)0) should throw, but it doesn't.";
            }
            catch (ArgumentException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected ArgumentException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StreamInfo20
        ******************************************************************************/
        /// <summary>
        /// A stream is created in a container beforehand.
        /// Open the stream with FileMode.Create with the container opened in Readonly mode.
        /// This is expected to fail.
        /// Validation is done by catching the appropriate exception.
        /// </summary>
        public void StreamInfo20()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            Stream      fStream = null;
            FileInfo  fInfo = null;
            bool        bResult = false;
            string      resultMsg = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }

            stInfo = stRoot.CreateStream( "TestStream" );
            stRoot.Close();
            stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.Open, FileAccess.Read );
            // ***** End Test Cases Initialization ****

            try
            {
                // ***** Avalon Test ****
                GlobalLog.LogStatus("Open a stream.");
                stInfo = stRoot.GetStreamInfo( "TestStream" );
                fStream = stInfo.GetStream(FileMode.Create);
                fStream.Close();
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                bResult = false;
                resultMsg = "GetStream(FileMode.Create) doesn't fail even if container is Read-only";
            }
            catch (IOException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected IOException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StreamInfo21
        ******************************************************************************/
        /// <summary>
        /// Create a stream with StorageInfo.CreateStream on a Readonly container.
        /// This is expected to fail.
        /// Validation is done by catching the appropriate exception.
        /// </summary>
        public void StreamInfo21()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;            
            FileInfo  fInfo = null;
            bool        bResult = false;
            string      resultMsg = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }
            stRoot.Close();
            stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.Open, FileAccess.Read );
            // ***** End Test Cases Initialization ****

            try
            {
                // ***** Avalon Test ****
                stInfo = stRoot.CreateStream( "TestStream" );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                bResult = false;
                resultMsg = "StorageInfo.CreateStream doesn't fail even if the container is opened Read-only.";
            }
            catch (IOException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected IOException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StreamInfo22
        ******************************************************************************/
        /// <summary>
        /// Open a stream with FileMode.Open and FileAccess.ReadWrite on Readonly container.
        /// This is expected to fail.
        /// Validation is done by catching the appropriate exception.
        /// </summary>
        public void StreamInfo22()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            Stream      fStream = null;
            FileInfo  fInfo = null;
            bool        bResult = false;
            string      resultMsg = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }
            stInfo = stRoot.CreateStream( "TestStream" );
            stRoot.Close();

            stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.Open, FileAccess.Read );
            stInfo = stRoot.GetStreamInfo( "TestStream" );
            // ***** End Test Cases Initialization ****

            try
            {
                // ***** Avalon Test ****
                GlobalLog.LogStatus("Open a stream.");
                fStream = stInfo.GetStream(FileMode.Open, FileAccess.ReadWrite);
                fStream.Close();
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                bResult = false;
                resultMsg = "StreamInfo.GetStream() allows ReadWrite access even if the container is opened Read-only.";
            }
            catch (IOException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected IOException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence( bResult + " " + resultMsg );
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }



        /******************************************************************************
        * Function:          StreamInfo23
        ******************************************************************************/
        /// <summary>
        /// Open a stream which is already open. A new handle should be returned with Position at 0.
        /// Validation is done by checking the Position property of the handle.
        /// </summary>
        public void StreamInfo23()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            Stream      fStream = null;
            FileInfo  fInfo = null;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }
            stInfo = stRoot.CreateStream( "TestStream" );
            fStream = stInfo.GetStream();
            fStream.Write( new Byte[512], 0, 512 );
            fStream.Close();
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            GlobalLog.LogStatus("open a stream twice.");                        
            fStream = stInfo.GetStream(FileMode.Open);
#pragma warning disable CA2022 // Avoid inexact read
            fStream.Read(new Byte[256], 0, 256);
#pragma warning restore CA2022
            // Stream position is 256
            // this shoult return a new copy with position = 0
            fStream = stInfo.GetStream(FileMode.Open);
            // ***** End Avalon Test ****

            // ***** Start Validation Section ****
            if ( fStream.Position == 0 )
            {
                GlobalLog.LogEvidence("PASS: Test passed");
            }
            else
            {
                GlobalLog.LogEvidence("FAIL: Opening a stream which is already open doesn't return a new handle.");
            }

            fStream.Close();
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }



        /******************************************************************************
        * Function:          StreamInfo24
        ******************************************************************************/
        /// <summary>
        /// Try to create a stream with empty string ("") as name.
        /// This is expected to fail.
        /// Validation is done by catching the appropriate exception.
        /// </summary>
        public void StreamInfo24()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            FileInfo  fInfo = null;            
            bool        bResult = false;
            string      resultMsg = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }
            // ***** End Test Cases Initialization ****
            try
            {
                // ***** Avalon Test ****
                GlobalLog.LogStatus("Create a stream.");
                stInfo = stRoot.CreateStream(String.Empty);
                // ***** End Avalon Test ****

                // ***** Start Validation Section ****
                bResult = false;
                resultMsg = "Empty string was allowed as name for a stream";
            }
            catch( ArgumentException e )
            {
                if (e.ParamName == "streamName")
                {
                    bResult = true;
                    resultMsg = "Test passed";
                }
                else
                    throw e;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StreamInfo25
        ******************************************************************************/
        /// <summary>
        /// Delete a stream while the stream is still open. 
        /// Validation is done by checking that StreamExists returns false.
        /// </summary>
        public void StreamInfo25()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            Stream fStream = null;
            FileInfo  fInfo = null;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }
            stInfo = stRoot.CreateStream( "TestStream" );
            fStream = stInfo.GetStream(FileMode.Open);
            // this will open a stream, which we never close
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            GlobalLog.LogStatus("delete a stream.");
            stRoot.DeleteStream( "TestStream" );
            // ***** End Avalon Test ****

            // ***** Start Validation Section ****
            if (!stRoot.StreamExists("TestStream"))
            {
                GlobalLog.LogEvidence("PASS: Test passed");
            }
            else
            {
                GlobalLog.LogEvidence("FAIL: Deleting a stream which is open silently fails.");
            }
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          StreamInfo26
        ******************************************************************************/
        /// <summary>
        /// Delete a stream from a Read only container. This is expected to fail.
        /// Validation is done by catching the appropriate exception.
        /// </summary>
        public void StreamInfo26()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            FileInfo  fInfo = null;
            bool        bResult = false;
            string      resultMsg = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            // check if the file exists
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }
            stInfo = stRoot.CreateStream( "TestStream" );
            stRoot.Close();

            stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.Open, FileAccess.Read );
            // ***** End Test Cases Initialization ****
            try
            {
                // ***** Avalon Test ****
                GlobalLog.LogStatus("delete a stream.");
                stRoot.DeleteStream( "TestStream" );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                bResult = false;
                resultMsg = "StorageInfo.DeleteStream() was successful, even if the container was opened as Read-only.";
            }
            catch (UnauthorizedAccessException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected UnauthorizedAccessException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****

            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }
        #endregion
    }
}
