// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.IO.Packaging;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Container;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test.Container;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Avalon.Test.CoreUI.Container
{
    /******************************************************************************
    * CLASS:          BVT_StorageRootThemePark
    ******************************************************************************/
    /// <summary>
    /// test class for Container BVT_StorageRootThemePark BVT cases
    /// </summary>
    /// <remarks>
    /// class inherits from TestCase, where ContextEnteringSupport will automatically create
    /// and enter a context when the constructor is called, and will automatically exit and 
    /// dispose the context when the destructor is called.
    /// </remarks>
    [Test(0, "Container.StorageRootThemePark", TestCaseSecurityLevel.FullTrust, "BVT_StorageRootThemePark")]
    public class BVT_StorageRootThemePark : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion


        #region Constructor
        [Variation("StorageRootThemePark1")]
        [Variation("StorageRootThemePark2")]
        [Variation("StorageRootThemePark3")]
        [Variation("StorageRootThemePark4")]
        [Variation("StorageRootThemePark5")]
        [Variation("StorageRootThemePark6")]
        [Variation("StorageRootThemePark7")]
        [Variation("StorageRootThemePark8")]
        [Variation("StorageRootThemePark9")]
        [Variation("StorageRootThemePark10")]

        /******************************************************************************
        * Function:          BVT_StorageRootThemePark Constructor
        ******************************************************************************/
        public BVT_StorageRootThemePark(string arg): base(TestCaseType.ContextEnteringSupport)
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
                case "StorageRootThemePark1":
                    StorageRootThemePark1();
                    break;
                case "StorageRootThemePark2":
                    StorageRootThemePark2();
                    break;
                case "StorageRootThemePark3":
                    StorageRootThemePark3();
                    break;
                case "StorageRootThemePark4":
                    StorageRootThemePark4();
                    break;
                case "StorageRootThemePark5":
                    StorageRootThemePark5();
                    break;
                case "StorageRootThemePark6":
                    StorageRootThemePark6();
                    break;
                case "StorageRootThemePark7":
                    StorageRootThemePark7();
                    break;
                case "StorageRootThemePark8":
                    StorageRootThemePark8();
                    break;
                case "StorageRootThemePark9":
                    StorageRootThemePark9();
                    break;
                case "StorageRootThemePark10":
                    StorageRootThemePark10();
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
        * Function:          StorageRootThemePark1
        ******************************************************************************/
        /// <summary>
        /// Simplest BVT of all: create a container
        /// very basic validation is done: code checks if the actual 
        /// file is created
        /// </summary>        
        public void StorageRootThemePark1()
        {
            StorageRootWrapper stRoot = null;
            FileStream  fStream = null;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            ThemeParkUtils.CleanStorage();
            // ***** End Test Cases Initialization ****
            // ***** Avalon Test ****
            GlobalLog.LogStatus("Test simple container creation.");
            fStream = ThemeParkUtils.GetIsolatedFileStream("container.xmf", FileMode.OpenOrCreate);
            stRoot = StorageRootWrapper.CreateOnStream( fStream, FileMode.Create );
            stRoot.Close();
            // ***** End Avalon Test ****
            // ***** Start Validation Section ****
            // check if the file exists
            if ( !ThemeParkUtils.ContainerFileExists() )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }
            GlobalLog.LogEvidence("PASS: FileStream1 passes" );
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            // remove the container file
            fStream.Close();
            ThemeParkUtils.CleanStorage();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRootThemePark2
        ******************************************************************************/
        /// <summary>
        /// Open a container. Open a new container with Create, on readonly stream
        /// This is an invalid call, Create with ReadOnly is not compatible.
        /// </summary>
        public void StorageRootThemePark2()
        {
            StorageRootWrapper  stRoot      = null;
            FileStream          ifStream    = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            ThemeParkUtils.CleanStorage();
            // ***** End Test Cases Initialization ****
            GlobalLog.LogStatus("Test container Open or Create with no container, FileAccess.Read.");
            // Try to create a stream, since the file is opened readonly
            // we expect this to fail
            try
            {
                // ***** Avalon Test ****
                ifStream = ThemeParkUtils.GetIsolatedFileStream("container.xmf", FileMode.OpenOrCreate, FileAccess.Read );
                stRoot = stRoot = StorageRootWrapper.CreateOnStream( ifStream, FileMode.Create );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                stRoot.Close();
                bResult = false;
                resultMsg = "stinfo.Create was expected to fail";
            }
            catch (ArgumentException e)
            {
                bResult = true;
                resultMsg = "FileStream2 passed:  the expected ArgumentException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg );
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            ifStream.Close();
            // remove the container file
            ThemeParkUtils.CleanStorage();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRootThemePark3
        ******************************************************************************/
        /// <summary>
        /// create new container when file exists
        /// This is expected to fail, code catches and verifyes the exception thrown.
        /// UPDATE: This is not expected to fail.
        /// </summary>
        public void StorageRootThemePark3()
        {
            StorageRootWrapper stRoot = null;
            FileStream  fStream = null;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            ThemeParkUtils.CleanStorage();
            
            // create empty container if file does not exist
            GlobalLog.LogStatus("Create simple container.");
            fStream = ThemeParkUtils.GetIsolatedFileStream("container.xmf", FileMode.OpenOrCreate);
            stRoot = StorageRootWrapper.CreateOnStream( fStream, FileMode.Create );
            stRoot.Close();
            fStream.Close();

            // check if it is created succesfuly
            if ( !ThemeParkUtils.ContainerFileExists() )
            {
                GlobalLog.LogEvidence("ERROR: Container file not created" );
                return;
            }
            // ***** End Test Cases Initialization ****
            // there is a container file, so this is expected to fail
            fStream = ThemeParkUtils.GetIsolatedFileStream("container.xmf", FileMode.OpenOrCreate);
            GlobalLog.LogStatus("Test simple NEW container creation, should fail container exist.");
            try
            {
                // ***** Avalon Test ****
                // this should fail
                stRoot = StorageRootWrapper.CreateOnStream( fStream, FileMode.Create );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                stRoot.Close();
            }
            catch (IOException e)
            {
                throw new Microsoft.Test.TestValidationException(e.Message);
            }
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            // remove the container file
            fStream.Close();
            ThemeParkUtils.CleanStorage();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRootThemePark4
        ******************************************************************************/
        /// <summary>
        /// Open a container. Open a new container with Create, on writeonly stream
        /// This is an invalid call, Create with WriteOnly is not compatible.
        /// </summary>
        public void StorageRootThemePark4()
        {
            StorageRootWrapper  stRoot      = null;
            FileStream          ifStream    = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            ThemeParkUtils.CleanStorage();
            // ***** End Test Cases Initialization ****
            GlobalLog.LogStatus("Test container Open or Create with no container, FileAccess.Read.");
            // Try to create a stream, since the file is opened readonly
            // we expect this to fail
            try
            {
                // ***** Avalon Test ****
                ifStream = ThemeParkUtils.GetIsolatedFileStream("container.xmf", FileMode.OpenOrCreate, FileAccess.Write );
                stRoot = StorageRootWrapper.CreateOnStream( ifStream, FileMode.Create );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                stRoot.Close();
                bResult = false;
                resultMsg = "StorageRootWrapper.CreateOnStream succeeded when it should have failed.";
            }
            catch (ArgumentException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected ArgumentException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg );
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            ifStream.Close();
            // remove the container file
            ThemeParkUtils.CleanStorage();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRootThemePark5
        ******************************************************************************/
        /// <summary>
        /// Open existing container. Container is created beforhead with one stream inside.
        /// Validation code checks if the stream is present.
        /// </summary>
        public void StorageRootThemePark5()
        {
            StorageRootWrapper  stRoot      = null;
            StreamInfo          stInfo      = null;
            FileStream          ifStream    = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            ThemeParkUtils.CleanStorage();

            // Default open call, should create a container
            // Also create one stream in the container to be used in 
            // later test
            GlobalLog.LogStatus("Create simple container.");
            ifStream = ThemeParkUtils.GetIsolatedFileStream("container.xmf", FileMode.OpenOrCreate);
            stRoot = StorageRootWrapper.CreateOnStream( ifStream, FileMode.Create );
            // create the stream
            stInfo = stRoot.CreateStream( "Test1" );
            stRoot.Close();
            ifStream.Close();
            
            // check if the file exists
            
            if ( !ThemeParkUtils.ContainerFileExists() )
            {
                GlobalLog.LogEvidence("FAIL: Container not created");
                return;
            }
            // ***** End Test Cases Initialization ****
            // ***** Avalon Test ****
            // Check the Open flag on existing container
            GlobalLog.LogStatus("Test container open.");
            ifStream = ThemeParkUtils.GetIsolatedFileStream("container.xmf", FileMode.OpenOrCreate);
            stRoot = StorageRootWrapper.CreateOnStream( ifStream, FileMode.Open );
            // ***** End Avalon Test ****
            // ***** Start Validation Section ****

            // search if Test1 stream exists, so we have not overriden 
            // the container file.
            if (stRoot.StreamExists("Test1"))
            {
                bResult = true;
                resultMsg = "Test passed";
            }
            else
            {
                bResult = false;
                resultMsg = "Stream does not exist.";
            }
            GlobalLog.LogEvidence( bResult + " " + resultMsg );
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            stRoot.Close();
            ifStream.Close();
            // remove the container file
            ThemeParkUtils.CleanStorage();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRootThemePark6
        ******************************************************************************/
        /// <summary>
        ///  Create container on closed stream. This is expected to fail.
        /// Validation code catches and validates the exception thrown.
        /// </summary>
        public void StorageRootThemePark6()
        {
            StorageRootWrapper  stRoot      = null;
            FileStream          fStream     = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            ThemeParkUtils.CleanStorage();
            // ***** End Test Cases Initialization ****

            // Check the Open flag with no container
            GlobalLog.LogStatus("Test container open with invalid container.");
            fStream = ThemeParkUtils.GetIsolatedFileStream("container.xmf", FileMode.OpenOrCreate);
            fStream.Close();
            try
            {
                // ***** Avalon Test ****
                // this is expected to fail
                stRoot = StorageRootWrapper.CreateOnStream( fStream, FileMode.Create );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                stRoot.Close();
                bResult = false;
                resultMsg = "StorageRootWrapper.CreateOnStream succeeded when it should have failed.";
            }
            catch (ArgumentException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected ArgumentException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            // remove the container file
            fStream.Close();
            ThemeParkUtils.CleanStorage();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRootThemePark7
        ******************************************************************************/
        /// <summary>
        /// Open an invalid stream as container. This is expected to fail.
        /// Validation code catches and validates the exception thrown.
        /// </summary>
        public void StorageRootThemePark7()
        {
            StorageRootWrapper  stRoot      = null;
            FileStream          fStream     = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            ThemeParkUtils.CleanStorage();
            fStream = ThemeParkUtils.GetIsolatedFileStream("container.xmf", FileMode.OpenOrCreate);
            fStream.Write( new Byte[512], 0, 512 );
            fStream.Close();
            // ***** End Test Cases Initialization ****

            // Check the Open flag with no container
            GlobalLog.LogStatus("Test container open with invalid container.");
            fStream = ThemeParkUtils.GetIsolatedFileStream("container.xmf", FileMode.OpenOrCreate);
            try
            {
                // ***** Avalon Test ****
                // this is expected to fail
                stRoot = StorageRootWrapper.CreateOnStream( fStream, FileMode.Open );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                stRoot.Close();
                bResult = false;
                resultMsg = "StorageRootWrapper.CreateOnStream succeeded when it should have failed.";
            }
            catch (IOException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected IOException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            // remove the container file
            fStream.Close();
            ThemeParkUtils.CleanStorage();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRootThemePark8
        ******************************************************************************/
        /// <summary>
        /// Open existing container with FileMode.Append parameter.
        /// Container file is created beforhead.
        /// This is expected to fail. The appropriate exception is catched.
        /// </summary>
        public void StorageRootThemePark8()
        {
            StorageRootWrapper  stRoot      = null;
            FileStream          fStream     = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            ThemeParkUtils.CleanStorage();

            // Default open call, should create a container
            GlobalLog.LogStatus("Test simple container creation.");
            fStream = ThemeParkUtils.GetIsolatedFileStream("container.xmf", FileMode.OpenOrCreate );
            stRoot = StorageRootWrapper.CreateOnStream( fStream, FileMode.Create );
            stRoot.Close();
            fStream.Close();
            
            // check if the file exists
            
            if ( !ThemeParkUtils.ContainerFileExists() )
            {
                GlobalLog.LogEvidence("FAIL: Container not created");
                return;
            }
            // ***** End Test Cases Initialization ****
            try
            {
                // ***** Avalon Test ****
                // Check the Open flag on existing container
                GlobalLog.LogStatus("Open existing container with FileMode.Append parameter.");
                fStream = ThemeParkUtils.GetIsolatedFileStream("container.xmf", FileMode.OpenOrCreate );
                stRoot = StorageRootWrapper.CreateOnStream( fStream, FileMode.Append );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                bResult = false;
                resultMsg = "StorageRootWrapper.CreateOnStream succeeded when it should have failed.";
            }
            catch (ArgumentException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected ArgumentException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            // remove the container file
            fStream.Close();
            ThemeParkUtils.CleanStorage();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRootThemePark9
        ******************************************************************************/
        /// <summary>
        ///  Open a container. Open existing container, readonly
        /// Validation is done by trying to create a stream, which is 
        /// expected to fail, since the dile is opened only for read.
        /// </summary>
        public void StorageRootThemePark9()
        {
            StorageRootWrapper  stRoot      = null;
            StreamInfo          stInfo      = null;            
            FileStream          ifStream    = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            ThemeParkUtils.CleanStorage();

            // Default open call, should create a container
            GlobalLog.LogStatus("Create simple container.");
            ifStream = ThemeParkUtils.GetIsolatedFileStream("container.xmf", FileMode.OpenOrCreate);
            stRoot = StorageRootWrapper.CreateOnStream( ifStream, FileMode.Create );
            stRoot.Close();
            ifStream.Close();
            
            // check if the file exists
            
            if ( !ThemeParkUtils.ContainerFileExists() )
            {
                GlobalLog.LogEvidence("FAIL: Container not created");
                return;
            }
            // ***** End Test Cases Initialization ****
            // ***** Avalon Test ****
            // Check the Open flag on existing container
            GlobalLog.LogStatus("Test container open, FileAccess.Read.");
            ifStream = ThemeParkUtils.GetIsolatedFileStream("container.xmf", FileMode.Open, FileAccess.Read );
            stRoot = StorageRootWrapper.CreateOnStream( ifStream, FileMode.Open );
            // ***** End Avalon Test ****
            // ***** Start Validation Section ****

            // Try to create a stream, since the file is opened readonly
            // we expect this to fail
            try
            {
                stInfo = stRoot.CreateStream("Test1");
                bResult = false;
                resultMsg = "stInfo.Create was expected to fail";
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
            ifStream.Close();
            // remove the container file
            ThemeParkUtils.CleanStorage();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRootThemePark10
        ******************************************************************************/
        /// <summary>
        /// Open a container passing null as Stream
        /// This is an invalid call, Create with ReadOnly is not compatible.
        /// </summary>
        public void StorageRootThemePark10()
        {
            StorageRootWrapper  stRoot      = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            ThemeParkUtils.CleanStorage();
            // ***** End Test Cases Initialization ****
            GlobalLog.LogStatus("Test container Open or Create with no container, FileAccess.Read.");
            // Try to create a stream, since the file is opened readonly
            // we expect this to fail
            try
            {
                // ***** Avalon Test ****
                stRoot = StorageRootWrapper.CreateOnStream( null, FileMode.Create );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                stRoot.Close();
                bResult = false;
                resultMsg = "stinfo.Create was expected to fail";
            }
            catch (ArgumentNullException e )
            {
                if ( e.ParamName == "baseStream" )
                {
                    bResult = true;
                    resultMsg = "Test passed";
                }
                else
                    throw e;
            }
            GlobalLog.LogEvidence( bResult + " " + resultMsg );
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            // remove the container file
            ThemeParkUtils.CleanStorage();
            // ***** End Shut Down  ****
        }
        #endregion
    }
}

