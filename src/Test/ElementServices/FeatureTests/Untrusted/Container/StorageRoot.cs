// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.IO.Packaging;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

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
    * CLASS:          BVT_StorageRoot
    ******************************************************************************/
    /// <summary>
    /// test class for Container BVT_StorageRoot BVT cases
    /// </summary>
    /// <remarks>
    /// class inherits from TestCase, where ContextEnteringSupport will automatically create
    /// and enter a context when the constructor is called, and will automatically exit and 
    /// dispose the context when the destructor is called.
    /// </remarks>
    [Test(1, "Container.StorageRoot", TestCaseSecurityLevel.FullTrust, "BVT_StorageRoot")]
    public class BVT_StorageRoot : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion


        #region Constructor
        [Variation("StorageRoot1", Priority=0)]
        [Variation("StorageRoot2")]
        [Variation("StorageRoot3", Priority=0)]
        [Variation("StorageRoot4", Priority=0)]
        [Variation("StorageRoot5", Priority=0)]
        [Variation("StorageRoot6", Priority=0)]
        [Variation("StorageRoot7")]
        [Variation("StorageRoot8")]
        [Variation("StorageRoot9")]
        [Variation("StorageRoot10")]
        [Variation("StorageRoot11")]
        [Variation("StorageRoot12")]
        [Variation("StorageRoot13")]
        [Variation("StorageRoot14")]
        [Variation("StorageRoot15")]
        [Variation("StorageRoot16")]
        [Variation("StorageRoot17")]
        [Variation("StorageRoot18")]
        [Variation("StorageRoot19")]
        [Variation("StorageRoot20")]
        [Variation("StorageRoot21")]

        /******************************************************************************
        * Function:          BVT_StorageRoot Constructor
        ******************************************************************************/
        public BVT_StorageRoot(string arg): base(TestCaseType.ContextEnteringSupport)
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
                case "StorageRoot1":
                    StorageRoot1();
                    break;
                case "StorageRoot2":
                    StorageRoot2();
                    break;
                case "StorageRoot3":
                    StorageRoot3();
                    break;
                case "StorageRoot4":
                    StorageRoot4();
                    break;
                case "StorageRoot5":
                    StorageRoot5();
                    break;
                case "StorageRoot6":
                    StorageRoot6();
                    break;
                case "StorageRoot7":
                    StorageRoot7();
                    break;
                case "StorageRoot8":
                    StorageRoot8();
                    break;
                case "StorageRoot9":
                    StorageRoot9();
                    break;
                case "StorageRoot10":
                    StorageRoot10();
                    break;
                case "StorageRoot11":
                    StorageRoot11();
                    break;
                case "StorageRoot12":
                    StorageRoot12();
                    break;
                case "StorageRoot13":
                    StorageRoot13();
                    break;
                case "StorageRoot14":
                    StorageRoot14();
                    break;
                case "StorageRoot15":
                    StorageRoot15();
                    break;
                case "StorageRoot16":
                    StorageRoot16();
                    break;
                case "StorageRoot17":
                    StorageRoot17();
                    break;
                case "StorageRoot18":
                    StorageRoot18();
                    break;
                case "StorageRoot19":
                    StorageRoot19();
                    break;
                case "StorageRoot20":
                    StorageRoot20();
                    break;
                case "StorageRoot21":
                    StorageRoot21();
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
        * Function:          StorageRoot1
        ******************************************************************************/
        /// <summary>
        /// Simplest BVT of all: create a container
        /// very basic validation is done: code checks if the actual 
        /// file is created
        /// </summary>        
        public void StorageRoot1()
        {
            StorageRootWrapper stRoot = null;
            FileInfo  fInfo = null;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            // ***** End Test Cases Initialization ****
            // ***** Avalon Test ****
            GlobalLog.LogStatus("Test simple container creation.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            stRoot.Close();
            // ***** End Avalon Test ****
            // ***** Start Validation Section ****
            // check if the file exists
            fInfo = new FileInfo( "container.xmf" );
            if ( !fInfo.Exists )
            {
                throw new Microsoft.Test.TestValidationException( "Container file not created" );
            }
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRoot2
        ******************************************************************************/
        /// <summary>
        /// create a New container
        /// very basic validation is done: code checks if the actual 
        /// file is created
        /// </summary>
        public void StorageRoot2()
        {
            StorageRootWrapper stRoot = null;
            FileInfo  fInfo = null;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            // ***** End Test Cases Initialization ****
            // ***** Avalon Test ****
            GlobalLog.LogStatus("Test simple NEW container creation.");
            stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.CreateNew );
            stRoot.Close();
            // ***** End Avalon Test ****
            // ***** Start Validation Section ****
            // check if the file exists
            fInfo = new FileInfo( "container.xmf" );
            if ( !fInfo.Exists )
            {
                throw new Microsoft.Test.TestValidationException( "Container file not created" );
            }
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRoot3
        ******************************************************************************/
        /// <summary>
        /// create new container when file exists
        /// This is expected to fail, code catches and verifyes the exception thrown.
        /// </summary>
        public void StorageRoot3()
        {
            StorageRootWrapper  stRoot      = null;
            FileInfo          fInfo       = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( !fInfo.Exists )
            {
                // create empty container if file does not exist
                GlobalLog.LogStatus("Create simple container.");
                stRoot = StorageRootWrapper.Open( "container.xmf" );
                stRoot.Close();

                // check if it is created succesfuly
                fInfo = new FileInfo( "container.xmf" );
                if ( !fInfo.Exists )
                {
                    throw new Microsoft.Test.TestValidationException( "Container file not created" );
                }
            }
            // ***** End Test Cases Initialization ****
            // there is a container file, so this is expected to fail
            GlobalLog.LogStatus("Test simple NEW container creation, should fail container exist.");
            try
            {
                // ***** Avalon Test ****
                // this should fail
                stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.CreateNew );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                stRoot.Close();
                bResult = false;
                resultMsg = "StorageRootWrapper.Open succeeded when it should have failed.";
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
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRoot4
        ******************************************************************************/
        /// <summary>
        /// Open a container. Default parameters
        /// Basic validation is done to check if the file exists.
        /// </summary>
        public void StorageRoot4()
        {
            StorageRootWrapper stRoot = null;
            FileInfo  fInfo = null;

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            // ***** End Test Cases Initialization ****
            // ***** Avalon Test ****
            // Default open call, should create a container
            GlobalLog.LogStatus("Test simple container creation.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            stRoot.Close();
            // ***** End Avalon Test ****
            // ***** Start Validation Section ****
            // check the file exists
            fInfo = new FileInfo( "container.xmf" );
            if ( !fInfo.Exists )
            {
                throw new Microsoft.Test.TestValidationException( "Container file does not exist" );
            }
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          StorageRoot5
        ******************************************************************************/
        /// <summary>
        /// Open existing container. Container is created beforhead with one stream inside.
        /// Validation code checks if the stream is present.
        /// </summary>
        public void StorageRoot5()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            FileInfo    fInfo = null;

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }

            // Default open call, should create a container
            // Also create one stream in the container to be used in 
            // later test
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            // create the stream
            stInfo = stRoot.CreateStream("Test1");
            stRoot.Close();
            
            // check if the file exists
            fInfo = new FileInfo( "container.xmf" );
            if ( !fInfo.Exists )
            {
                throw new Microsoft.Test.TestValidationException( "Container not created" );
            }
            // ***** End Test Cases Initialization ****
            // ***** Avalon Test ****
            // Check the Open flag on existing container
            GlobalLog.LogStatus("Test container open.");
            stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.Open );
            // ***** End Avalon Test ****
            // ***** Start Validation Section ****

            // search if Test1 stream exists, so we have not overriden 
            // the container file.
            if (!stRoot.StreamExists("Test1"))
            {
                throw new Microsoft.Test.TestValidationException( "Stream does not exist.");
            }
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRoot6
        ******************************************************************************/
        /// <summary>
        /// Open a existing container with OpenOrCreate. Container is created 
        /// beforhead with one stream inside. Validation code checks if the stream 
        /// is present.
        /// </summary>
        public void StorageRoot6()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;
            FileInfo  fInfo = null;

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {
                // remove the container file
                fInfo.Delete();
            }
            // Default open call, should create a container
            // Also create one stream in the container to be used in 
            // later tests
            GlobalLog.LogStatus("Test simple container creation.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            // create the stream
            stInfo = stRoot.CreateStream("Test1");
            stRoot.Close();
            
            // check if the file exists
            fInfo = new FileInfo( "container.xmf" );
            if ( !fInfo.Exists )
            {
                throw new Microsoft.Test.TestValidationException( "Container file does not exist" );
             
            }
            // ***** End Test Cases Initialization ****
            // ***** Avalon Test ****
            // Check the OpenOrCreate flag on existing container
            GlobalLog.LogStatus("Test container Open or Create with existing container.");
            stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.OpenOrCreate );
            // ***** End Avalon Test ****
            // ***** Start Validation Section ****

            // search if Test1 stream exists, so we have not overriden 
            // the container file.
            if (!stRoot.StreamExists("Test1"))
            {
                throw new Microsoft.Test.TestValidationException("Stream does not exist.");
            }
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRoot7
        ******************************************************************************/
        /// <summary>
        /// Open a non existing container. This is expected to fail.
        /// Validation code Catches and validates the exception thrown.
        /// </summary>
        public void StorageRoot7()
        {
            StorageRootWrapper  stRoot      = null;
            FileInfo          fInfo       = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            // ***** End Test Cases Initialization ****

            // Check the Open flag with no container
            GlobalLog.LogStatus("Test container open with no container.");
            try
            {
                // ***** Avalon Test ****
                // this is expected to fail
                stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.Open );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                stRoot.Close();
                bResult = false;
                resultMsg = "StorageRootWrapper.Open succeeded when it should have failed.";
            }
            catch (FileNotFoundException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected FileNotFoundException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRoot8
        ******************************************************************************/
        /// <summary>
        /// Open with OpenOrCreate and no container. 
        /// This is expected to create a new container.
        /// Validation code checks if the file is there.
        /// </summary>
        public void StorageRoot8()
        {
            StorageRootWrapper stRoot = null;
            FileInfo  fInfo = null;

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            // ***** End Test Cases Initialization ****
            // ***** Avalon Test ****
            // Check the OpenOrCreate flag with no container
            GlobalLog.LogStatus("Test container Open or Create with no container.");
            stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.OpenOrCreate );
            stRoot.Close();
            // ***** End Avalon Test ****
            // ***** Start Validation Section ****
            // check the file exists
            fInfo = new FileInfo( "container.xmf" );
            if ( !fInfo.Exists )
            {
                throw new Microsoft.Test.TestValidationException("Container file does not exist" );
            }
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          StorageRoot9
        ******************************************************************************/
        /// <summary>
        /// Open a container. Open existing container, readonly
        /// Validation is done by trying to create a stream, which is 
        /// expected to fail, since the dile is opened only for read.
        /// </summary>
        public void StorageRoot9()
        {
            StorageRootWrapper  stRoot      = null;
            StreamInfo          stInfo      = null;
            FileInfo          fInfo       = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }

            // Default open call, should create a container
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            stRoot.Close();
            
            // check if the file exists
            fInfo = new FileInfo( "container.xmf" );
            if ( !fInfo.Exists )
            {
                throw new Microsoft.Test.TestValidationException( "Container not created" );
            }
            // ***** End Test Cases Initialization ****
            // ***** Avalon Test ****
            // Check the Open flag on existing container
            GlobalLog.LogStatus("Test container open, FileAccess.Read.");
            stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.Open, FileAccess.Read );
            // ***** End Avalon Test ****
            // ***** Start Validation Section ****

            // Try to create a stream, since the file is opened readonly
            // we expect this to fail
            try
            {
                stInfo = stRoot.CreateStream("Test1");
                bResult = false;
                resultMsg = "CreateStream succeeded when it should have failed.";
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
        * Function:          StorageRoot10
        ******************************************************************************/
        /// <summary>
        /// Open a container. Open existing container with OpenOrCreate, readonly
        /// Validation is done by trying to create a stream, which is 
        /// expected to fail, since the dile is opened only for read.
        /// </summary>
        public void StorageRoot10()
        {
            StorageRootWrapper  stRoot      = null;
            StreamInfo          stInfo      = null;
            FileInfo          fInfo       = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {
                // remove the container file
                fInfo.Delete();
            }
            // Default open call, should create a container
            GlobalLog.LogStatus("Test simple container creation.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            stRoot.Close();
            
            // check if the file exists
            fInfo = new FileInfo( "container.xmf" );
            if ( !fInfo.Exists )
            {
                throw new Microsoft.Test.TestValidationException( "Container file does not exist" );
            
            }
            // ***** End Test Cases Initialization ****
            // ***** Avalon Test ****
            // Check the OpenOrCreate flag on existing container
            GlobalLog.LogStatus("Test container Open or Create with existing container FileAccess.Read.");
            stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.OpenOrCreate, FileAccess.Read );
            // ***** End Avalon Test ****
            // ***** Start Validation Section ****

            // Try to create a stream, since the file is opened readonly
            // we expect this to fail
            try
            {
                stInfo = stRoot.CreateStream("Test1");
                bResult = false;
                resultMsg = "CreateStream succeeded when it should have failed.";
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
        * Function:          StorageRoot11
        ******************************************************************************/
        /// <summary>
        /// Open a container. Open new container with OpenOrCreate, readonly
        /// This is an invalid call, Create with ReadOnly is not compatible.
        /// </summary>
        /// <remarks>StorageRootWrapper.open no longer accepts the invalid combination
        /// rogerch says that this is no longer allowed. it causes non-intuitive error
        /// </remarks>
        public void StorageRoot11()
        {
            StorageRootWrapper  stRoot      = null;
            FileInfo          fInfo       = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {
                // remove the container file
                fInfo.Delete();
            }
            // ***** End Test Cases Initialization ****
            // ***** Avalon Test ****
            // Check the OpenOrCreate flag on existing container
            GlobalLog.LogStatus("Test container Open or Create with no container, FileAccess.Read.");

            try
            {
                //this will fail
                stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.OpenOrCreate, FileAccess.Read );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****

                // Try to create a stream, since the file is opened readonly
                // we expect this to fail
                //stInfo = new StreamInfo( stRoot, "Test1" ).

                // this is expected to fail
                //fStream = stInfo.Create(FileMode.CreateNew).
                //fStream.Close().
                bResult = false;
                resultMsg = "StorageRootWrapper.Open succeeded when it should have failed.";
                stRoot.Close();
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
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRoot12
        ******************************************************************************/
        /// <summary>
        /// Open a container. Open new container with OpenOrCreate, WriteOnly
        /// Basic validation is done to check if the file exists.
        /// </summary>
        public void StorageRoot12()
        {
            StorageRootWrapper  stRoot      = null;
            FileInfo          fInfo       = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {
                // remove the container file
                fInfo.Delete();
            }
            // ***** End Test Cases Initialization ****
            try
            {
                // ***** Avalon Test ****
                // Check the OpenOrCreate,Write flag on existing container
                // this is expected to fail
                GlobalLog.LogStatus("Test container Open or Create with no container, FileAccess.Write.");
                stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.OpenOrCreate, FileAccess.Write );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                bResult = false;
                resultMsg = "StorageRootWrapper.Open succeeded when it should have failed.";
                stRoot.Close();
            }
            catch (NotSupportedException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected NotSupportedException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRoot13
        ******************************************************************************/
        /// <summary>
        /// Open existing container with FileShare.None.
        /// Container is created beforhead with one stream inside.
        /// Validation code checks if the stream is present.
        /// </summary>
        public void StorageRoot13()
        {
            StorageRootWrapper stRoot = null;
            StreamInfo  stInfo = null;            
            FileInfo    fInfo = null;

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }

            // Default open call, should create a container
            // Also create one stream in the container to be used in 
            // later tests
            GlobalLog.LogStatus("Test simple container creation.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            // create the stream
            stInfo = stRoot.CreateStream( "Test1" );
            stRoot.Close();
            
            // check if the file exists
            fInfo = new FileInfo( "container.xmf" );
            if ( !fInfo.Exists )
            {
                throw new Microsoft.Test.TestValidationException(  "Container not created" );
              
            }
            // ***** End Test Cases Initialization ****
            // ***** Avalon Test ****
            // Check the Open flag on existing container
            GlobalLog.LogStatus("Test container open, FileAccess.Read, FileShare.None.");
            stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.Open, FileAccess.Read, FileShare.None );
            // ***** End Avalon Test ****
            // ***** Start Validation Section ****

            // search if Test1 stream exists, so we have not overriden 
            // the container file.
            if (!stRoot.StreamExists("Test1"))
            {
                throw new Microsoft.Test.TestValidationException(  "Stream does not exist.");
            }
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            stRoot.Close();
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRoot14
        ******************************************************************************/
        /// <summary>
        /// Test close the container twice. 
        /// Validation code verifiyes no exception is thrown. 
        /// All exceptions are catched by the test framework and are treeted as errors, 
        /// so no real   catch is necesery.
        /// </summary>
        public void StorageRoot14()
        {
            StorageRootWrapper stRoot = null;
            FileInfo  fInfo = null;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            try
            {
                stRoot = StorageRootWrapper.Open( "container.xmf" );
                GlobalLog.LogStatus("Test close twice.");
                stRoot.Close();
                // ***** End Test Cases Initialization ****
                // ***** Avalon Test ****
                stRoot.Close();
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
            }
            catch (Exception e)
            {

                throw new Microsoft.Test.TestValidationException( e.Message);
            }
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            // remove the container file
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {
                fInfo.Delete();
            }
            // ***** End Shut Down  ****
        }

   
        /******************************************************************************
        * Function:          StorageRoot15
        ******************************************************************************/
        /// <summary>
        /// Open an invalid file. A filename "b:\container.xmf" is passed to the call.
        /// Usually B drive does not exist so the call should fail.
        /// This is expected to fail. Validation is done by catching the exception.
        /// </summary>
        public void StorageRoot15()
        {
            StorageRootWrapper  stRoot      = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // ***** End Test Cases Initialization ****
            try
            {
                // ***** Avalon Test ****
                GlobalLog.LogStatus("Open an invalid file.");
                stRoot = StorageRootWrapper.Open( "b:\\container.xmf" );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                bResult = false;
                resultMsg = "StorageRootWrapper.Open succeeded when it should have failed.";
            }
            catch (IOException e)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected IOException occurred: " + e.Message;
            }
            GlobalLog.LogEvidence(bResult + " " + resultMsg);
            // check if the file exists
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRoot16
        ******************************************************************************/
        /// <summary>
        /// Open existing container with FileShare.Inheritable parameter.
        /// Container file is created beforhead.
        /// This is expected to fail. The appropriate exception is catched.
        /// </summary>
        public void StorageRoot16()
        {
            StorageRootWrapper  stRoot      = null;
            FileInfo          fInfo       = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }

            // Default open call, should create a container
            GlobalLog.LogStatus("Test simple container creation.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            stRoot.Close();
            
            // check if the file exists
            fInfo = new FileInfo( "container.xmf" );
            if ( !fInfo.Exists )
            {
                throw new Microsoft.Test.TestValidationException( "Container not created" );
            }
            // ***** End Test Cases Initialization ****
            try
            {
                // ***** Avalon Test ****
                // Check the Open flag on existing container
                GlobalLog.LogStatus("Open existing container with FileShare.Inheritable parameter.");
                stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.Open, FileAccess.Read, FileShare.Inheritable );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                bResult = false;
                resultMsg = "StorageRootWrapper.Open succeeded when it should have failed.";
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
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRoot17
        ******************************************************************************/
        /// <summary>
        /// Open existing container with FileMode.Append parameter.
        /// Container file is created beforhead.
        /// This is expected to fail. The appropriate exception is catched.
        /// </summary>
        public void StorageRoot17()
        {
            StorageRootWrapper  stRoot      = null;
            FileInfo          fInfo       = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }

            // Default open call, should create a container
            GlobalLog.LogStatus("Test simple container creation.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            stRoot.Close();
            
            // check if the file exists
            fInfo = new FileInfo( "container.xmf" );
            if ( !fInfo.Exists )
            {
               throw new Microsoft.Test.TestValidationException(  "Container not created" );
            }
            // ***** End Test Cases Initialization ****
            try
            {
                // ***** Avalon Test ****
                // Check the Open flag on existing container
                GlobalLog.LogStatus("Open existing container with FileMode.Append parameter.");
                stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.Append );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                bResult = false;
                resultMsg = "StorageRootWrapper.Open succeeded when it should have failed.";
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
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRoot18
        ******************************************************************************/
        /// <summary>
        /// Open existing container with FileMode.Truncate parameter.
        /// Container file is created beforhead.
        /// This is expected to fail. The appropriate exception is catched.
        /// </summary>
        public void StorageRoot18()
        {
            StorageRootWrapper  stRoot      = null;
            FileInfo          fInfo       = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }

            // Default open call, should create a container
            GlobalLog.LogStatus("Test simple container creation.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            stRoot.Close();
            
            // check if the file exists
            fInfo = new FileInfo( "container.xmf" );
            if ( !fInfo.Exists )
            {
                throw new Microsoft.Test.TestValidationException(  "Container not created" );
            }
            // ***** End Test Cases Initialization ****
            try
            {
                // ***** Avalon Test ****
                // Check the Open flag on existing container
                GlobalLog.LogStatus("Open existing container with FileMode.Truncate parameter.");
                stRoot = StorageRootWrapper.Open( "container.xmf", FileMode.Append );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                bResult = false;
                resultMsg = "StorageRootWrapper.Open succeeded when it should have failed.";
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
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRoot19
        ******************************************************************************/
        /// <summary>
        /// Open existing container with invalid FileMode parameter.
        /// Container file is created beforhead.
        /// This is expected to fail. The appropriate exception is catched.
        /// </summary>
        public void StorageRoot19()
        {
            StorageRootWrapper  stRoot      = null;
            FileInfo          fInfo       = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }

            // Default open call, should create a container
            GlobalLog.LogStatus("Test simple container creation.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            stRoot.Close();
            
            // check if the file exists
            fInfo = new FileInfo( "container.xmf" );
            if ( !fInfo.Exists )
            {
                throw new Microsoft.Test.TestValidationException(  "Container not created" );
            }
            // ***** End Test Cases Initialization ****
            try
            {
                // ***** Avalon Test ****
                // Check the Open flag on existing container
                GlobalLog.LogStatus("Open existing container with invalid FileMode parameter.");
                stRoot = StorageRootWrapper.Open( "container.xmf", (FileMode)0 );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                bResult = false;
                resultMsg = "StorageRootWrapper.Open succeeded when it should have failed.";
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
            fInfo.Delete();
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRoot20
        ******************************************************************************/
        /// <summary>
        /// Try to open a container passing a null string.
        /// Validation is done catching the appropriate exception. This call is expected to fail.
        /// </summary>
        public void StorageRoot20()
        {
            StorageRootWrapper stRoot = null;
            FileInfo  fInfo = null;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            // ***** End Test Cases Initialization ****
            try
            {
                // ***** Avalon Test ****
                stRoot = StorageRootWrapper.Open( null );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
            }
            catch (ArgumentNullException e)
            {
                if (e.ParamName != "Path")
                   throw new Microsoft.Test.TestValidationException(e.Message);              
            }
            // ***** End Validation Section  ****
            // ***** Start Shut Down ****
            // ***** End Shut Down  ****
        }


        /******************************************************************************
        * Function:          StorageRoot21
        ******************************************************************************/
        /// <summary>
        /// Open a container with invalid flags. Repro for 


        public void StorageRoot21()
        {
            StorageRootWrapper  stRoot      = null;
            FileInfo          fInfo       = null;
            bool                bResult     = false;
            string              resultMsg   = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Cleanup container file if it exists
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }

            // Default open call, should create a container
            // Also create one stream in the container to be used in 
            // later test
            GlobalLog.LogStatus("Create simple container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            stRoot.Close();
            
            // check if the file exists
            fInfo = new FileInfo( "container.xmf" );
            if ( !fInfo.Exists )
            {
                throw new Microsoft.Test.TestValidationException(  "Container not created" );
            }
            // ***** End Test Cases Initialization ****

            // Check the Open flag with no container
            GlobalLog.LogStatus("Test container open with invalid flags.");
            try
            {
                // ***** Avalon Test ****
                // this is expected to fail
                stRoot =  StorageRootWrapper.Open("container.xmf", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                bResult = false;
                resultMsg = "StorageRootWrapper.Open succeeded when it should have failed.";
                stRoot.Close();
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
            fInfo.Delete();
            // ***** End Shut Down  ****
        }
        #endregion
    }
}
