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
    * CLASS:          BVT_StorageInfo
    ******************************************************************************/
    [Test(0, "Container.StorageInfo", TestCaseSecurityLevel.FullTrust, "BVT_StorageInfo")]
    public class BVT_StorageInfo : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion


        #region Constructor

        [Variation("StorageInfo1")]
        [Variation("StorageInfo2")]
        [Variation("StorageInfo3")]
        [Variation("StorageInfo4", Keywords = "MicroSuite")]
        [Variation("StorageInfo5")]
        [Variation("StorageInfo6")]
        [Variation("StorageInfo7")]
        [Variation("StorageInfo8")]
        [Variation("StorageInfo9")]

        /******************************************************************************
        * Function:          BVT_StorageInfo Constructor
        ******************************************************************************/
        public BVT_StorageInfo(string arg): base(TestCaseType.ContextEnteringSupport)
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
                case "StorageInfo1":
                    StorageInfo1();
                    break;
                case "StorageInfo2":
                    StorageInfo2();
                    break;
                case "StorageInfo3":
                    StorageInfo3();
                    break;
                case "StorageInfo4":
                    StorageInfo4();
                    break;
                case "StorageInfo5":
                    StorageInfo5();
                    break;
                case "StorageInfo6":
                    StorageInfo6();
                    break;
                case "StorageInfo7":
                    StorageInfo7();
                    break;
                case "StorageInfo8":
                    StorageInfo8();
                    break;
                case "StorageInfo9":
                    StorageInfo9();
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
        * Function:          StorageInfo1
        ******************************************************************************/
        /// <summary>
        /// Create storage using StorageInfo.CreateSubStorage(). 
        /// Validation is done by checking that StorageInfo.SubStorageExists()
        /// returns true.
        /// </summary>       
        public void StorageInfo1()
        {
           StorageRootWrapper stRoot = null;
           StorageInfo strInfo = null;
           FileInfo   fInfo = null;

           // ***** Start Test Cases Initialization ****
           // Clean existing file.
           fInfo = new FileInfo( "container.xmf" );
           if ( fInfo.Exists )
           {    // remove the container file
               fInfo.Delete();
           }
           GlobalLog.LogStatus("Create test container.");
           stRoot = StorageRootWrapper.Open( "container.xmf" );
           fInfo = new FileInfo( "container.xmf" );
           if ( !fInfo.Exists )
           {
               GlobalLog.LogEvidence( "FAIL: Container file not created" );
               return;
           }
           // ***** End Test Cases Initialization ****
           // ***** Avalon Test ****
           strInfo = stRoot.CreateSubStorage("TestStorage");
           stRoot.Close();
           stRoot = StorageRootWrapper.Open("container.xmf");
           // ***** End Avalon Test ****
           // ***** Start Validation Section ****
           if (stRoot.SubStorageExists("TestStorage"))
               GlobalLog.LogEvidence("PASS: Test passes" );
           else
               GlobalLog.LogEvidence("FAIL: Storage does not exist" );
           // ***** End Validation Section  ****
           // ***** Start Shut Down ****
           stRoot.Close();
           // remove the container file
           fInfo.Delete();
           // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          StorageInfo2
        ******************************************************************************/
        /// <summary>
        /// Delete an empty substorage using StorageInfo.DeleteSubStorage().
        /// Validation is done via making sure StorageInfo.SubStorageExists() returns false.
        /// </summary>
        public void StorageInfo2()
        {
           StorageRootWrapper stRoot = null;
           StorageInfo strInfo = null;
           FileInfo   fInfo = null;

           // ***** Start Test Cases Initialization ****
           // Clean existing file.
           fInfo = new FileInfo( "container.xmf" );
           if ( fInfo.Exists )
           {    // remove the container file
               fInfo.Delete();
           }
           GlobalLog.LogStatus("Create test container.");
           stRoot = StorageRootWrapper.Open( "container.xmf" );
           fInfo = new FileInfo( "container.xmf" );
           if ( !fInfo.Exists )
           {
               GlobalLog.LogEvidence( "FAIL: Container file not created" );
               return;
           }
           strInfo = stRoot.CreateSubStorage("TestStorage");
           stRoot.Close();
           stRoot = StorageRootWrapper.Open( "container.xmf" );
           // ***** End Test Cases Initialization ****
           // ***** Avalon Test ****
           stRoot.DeleteSubStorage("TestStorage");
           // ***** End Avalon Test ****
           // ***** Start Validation Section ****
           if (!stRoot.SubStorageExists("TestStorage"))
               GlobalLog.LogEvidence("PASS: Test passes" );
           else
               GlobalLog.LogEvidence("FAIL: Storage exist - should have been deleted" );
           // ***** End Validation Section  ****
           // ***** Start Shut Down ****
           stRoot.Close();
           // remove the container file
           fInfo.Delete();
           // ***** End Shut Down  ****
       }


        /******************************************************************************
        * Function:          StorageInfo3
        ******************************************************************************/
        /// <summary>
        /// Delete a non-empty substorage using StorageInfo.DeleteSubStorage().
        /// A sub-sub-storage is created in the sub-storage to test non-empty substorage deletion.
        /// Validation is done by checking that StorageInfo.SubStorageExists() returns false.
        /// Substorage deletion is recursive by default.
        /// </summary>
        public void StorageInfo3()
        {
           StorageRootWrapper stRoot = null;
           StorageInfo strInfo = null;
           StorageInfo strInfo1 = null;
           FileInfo   fInfo = null;
           // ***** Start Test Cases Initialization ****
           // Clean existing file.
           fInfo = new FileInfo( "container.xmf" );
           if ( fInfo.Exists )
           {    // remove the container file
               fInfo.Delete();
           }
           GlobalLog.LogStatus("Create test container.");
           stRoot = StorageRootWrapper.Open( "container.xmf" );
           fInfo = new FileInfo( "container.xmf" );
           if ( !fInfo.Exists )
           {
               throw new Microsoft.Test.TestValidationException( "Container file not created" );
           }
           strInfo = stRoot.CreateSubStorage("TestStorage");
           strInfo1 = strInfo.CreateSubStorage( "TestSubStorage" );
           stRoot.Close();
           stRoot = StorageRootWrapper.Open( "container.xmf" );
           // ***** End Test Cases Initialization ****

           // ***** Avalon Test ****            
           stRoot.DeleteSubStorage("TestStorage");
           // ***** End Avalon Test ****
           // ***** Start Validation Section ****
           if(stRoot.SubStorageExists("TestStorage"))
           {                
               throw new Microsoft.Test.TestValidationException("StorageInfo.DeleteSubStorage() cannot delete non-empty substorages.");                  
           }
           // ***** End Validation Section  ****
           // ***** Start Shut Down ****
           stRoot.Close();
           // remove the container file
           fInfo.Delete();
           // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          StorageInfo4
        ******************************************************************************/
        /// <summary>
        /// Delete a non-empty substorage using StorageInfo.DeleteSubStorage().
        /// A stream is created in the substorage to test non-empty substorage deletion.
        /// Validation is done by checking that StorageInfo.SubStorageExists() returns false.
        /// Substorage deletion is recursive by default.
        /// </summary>
        public void StorageInfo4()
        {
           StorageRootWrapper stRoot = null;
           StorageInfo strInfo = null;
           StreamInfo  strmInfo = null;
           FileInfo   fInfo = null;

           // ***** Start Test Cases Initialization ****
           // Clean existing file.
           fInfo = new FileInfo( "container.xmf" );
           if ( fInfo.Exists )
           {    // remove the container file
               fInfo.Delete();
           }
           GlobalLog.LogStatus("Create test container.");
           stRoot = StorageRootWrapper.Open( "container.xmf" );
           fInfo = new FileInfo( "container.xmf" );
           if ( !fInfo.Exists )
           {
               GlobalLog.LogEvidence( "FAIL: Container file not created" );
               return;
           }
           strInfo = stRoot.CreateSubStorage("TestStorage");
           strmInfo = strInfo.CreateStream( "TestStream" );
           stRoot.Close();
           stRoot = StorageRootWrapper.Open( "container.xmf" );
           // ***** End Test Cases Initialization ****
           // ***** Avalon Test ****
           stRoot.DeleteSubStorage("TestStorage");
           // ***** End Avalon Test ****
           // ***** Start Validation Section ****
           if(stRoot.SubStorageExists("TestStorage"))
           {             
               throw new Microsoft.Test.TestValidationException("StorageInfo.DeleteSubStorage() cannot delete non-empty substorages." );               
           }
           // ***** End Validation Section  ****
           // ***** Start Shut Down ****
           stRoot.Close();
           // remove the container file
           fInfo.Delete();
           // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          StorageInfo5
        ******************************************************************************/
        /// <summary>
        /// Test StorageInfo.SubStorageExists() method on a non-existing substorage.
        /// Validation is done by making sure that the return value is false.
        /// </summary>
        public void StorageInfo5()
        {
           StorageRootWrapper stRoot = null;           
           FileInfo   fInfo = null;
           bool     bExists;

           // ***** Start Test Cases Initialization ****
           // Clean existing file.
           fInfo = new FileInfo( "container.xmf" );
           if ( fInfo.Exists )
           {    // remove the container file
               fInfo.Delete();
           }
           GlobalLog.LogStatus("Create test container.");
           stRoot = StorageRootWrapper.Open( "container.xmf" );
           fInfo = new FileInfo( "container.xmf" );
           if ( !fInfo.Exists )
           {
               GlobalLog.LogEvidence( "FAIL: Container file not created" );
               return;
           }
           // ***** End Test Cases Initialization ****
           // ***** Avalon Test ****
           bExists = stRoot.SubStorageExists("TestStorage");
           // ***** End Avalon Test ****
           // ***** Start Validation Section ****
           if ( !bExists )
               GlobalLog.LogEvidence("PASS: Test passes" );
           else
               GlobalLog.LogEvidence("FAIL: StorageInfo.SubStorageExists() returns true even if the substorage does not exist." );
           // ***** End Validation Section  ****
           // ***** Start Shut Down ****
           stRoot.Close();
           // remove the container file
           fInfo.Delete();
           // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          StorageInfo6
        ******************************************************************************/
        /// <summary>
        /// Test GetStreams() and GetSubStorages() methods on an empty storage.
        /// Validation is done by checking that both of them return empty arrays.
        /// </summary>
        public void StorageInfo6()
        {
           StorageRootWrapper stRoot = null;
           StorageInfo strInfo = null;
           FileInfo   fInfo = null;
           bool     bResult = false;
           string       failMsg = "Unknown failure";

           // ***** Start Test Cases Initialization ****
           // Clean existing file.
           fInfo = new FileInfo( "container.xmf" );
           if ( fInfo.Exists )
           {    // remove the container file
               fInfo.Delete();
           }
           GlobalLog.LogStatus("Create test container.");
           stRoot = StorageRootWrapper.Open( "container.xmf" );
           fInfo = new FileInfo( "container.xmf" );
           if ( !fInfo.Exists )
           {
               GlobalLog.LogEvidence( "FAIL: Container file not created" );
               return;
           }
           strInfo = stRoot.CreateSubStorage("TestStorage");
           stRoot.Close();
           stRoot = StorageRootWrapper.Open( "container.xmf" );
           strInfo = stRoot.GetSubStorageInfo("TestStorage");
           // ***** End Test Cases Initialization ****
           StreamInfo[] streams = strInfo.GetStreams();
           StorageInfo[] storages = strInfo.GetSubStorages();

           if((streams.Length != 0) || (storages.Length != 0))                   
           {                   
               bResult = false;               
               failMsg = "GetStreams() and/or GetSubStorages() on empty storage don't return empty arrays." ;         
           }
           else                   
           {                       
               bResult = true;               
               failMsg = "Test Passed";               
           }
           // ***** End Validation Section  ****
           GlobalLog.LogEvidence( bResult + " " + failMsg );
           // ***** Start Shut Down ****
           stRoot.Close();
           // remove the container file
           fInfo.Delete();
           // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          StorageInfo7
        ******************************************************************************/
        /// <summary>
        /// Test GetStreams() and GetSubStorages() methods on a storage with one sub storage and one stream.
        /// Validation is done by counting and verifying the elements in the arrays returned.
        /// </summary>
        public void StorageInfo7()
        {
           StorageRootWrapper stRoot = null;
           StorageInfo strInfo = null;
           StorageInfo strInfo1 = null;
           StreamInfo  strmInfo = null;
           FileInfo   fInfo = null;
           bool     bResult = false;
           string       failMsg = "Unknown failure";

           // ***** Start Test Cases Initialization ****
           // Clean existing file.
           fInfo = new FileInfo( "container.xmf" );
           if ( fInfo.Exists )
           {    // remove the container file
               fInfo.Delete();
           }
           GlobalLog.LogStatus("Create test container.");
           stRoot = StorageRootWrapper.Open( "container.xmf" );
           fInfo = new FileInfo( "container.xmf" );
           if ( !fInfo.Exists )
           {
               GlobalLog.LogEvidence( "FAIL: Container file not created" );
               return;
           }
           strInfo = stRoot.CreateSubStorage("TestStorage");
           strInfo1 = strInfo.CreateSubStorage( "TestSubStorage" );
           strmInfo = strInfo.CreateStream( "TestStream" );
           stRoot.Close();
           stRoot = StorageRootWrapper.Open( "container.xmf" );
           strInfo = stRoot.GetSubStorageInfo("TestStorage");

           StreamInfo[] streams = strInfo.GetStreams();
           StorageInfo[] storages = strInfo.GetSubStorages();
           // ***** End Test Cases Initialization ****

           // ***** Start Validation Section ****
           if ((streams.Length != 1) || (storages.Length != 1))
           {
                bResult = false;
                failMsg = "GetStreams() and GetSubStorages() were expected to return arrays with 1 element each. That didn't happen.";
           }
           else
           {               
               StreamInfo si = streams[0];           
               StorageInfo sti = storages[0];

               if (si.Name != "TestStream")
               {
                   bResult = false;
                   failMsg = "One of the streams returned by StorageInfo.GetStreams() has a different name than expected.";
               }
               else
               {
                   if (sti.Name != "TestSubStorage")
                   {
                       bResult = false;
                       failMsg = "One of the substorages returned by StorageInfo.GetSubStorages() has a different name than expected.";
                   }
                   else
                   {
                       bResult = true;
                       failMsg = "Test passed";
                   }
               }
           }
           // ***** End Validation Section  ****
           GlobalLog.LogEvidence( bResult + " " + failMsg );
           // ***** Start Shut Down ****
           stRoot.Close();
           // remove the container file
           fInfo.Delete();
           // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          StorageInfo8
        ******************************************************************************/
        /// <summary>
        /// Test Name property on a storage.
        /// Validation is done by comparing the value with expected result.
        /// </summary>
        public void StorageInfo8()
        {
           StorageRootWrapper stRoot = null;
           StorageInfo strInfo = null;
           FileInfo   fInfo = null;
           string       name;

           // ***** Start Test Cases Initialization ****
           // Clean existing file.
           fInfo = new FileInfo( "container.xmf" );
           if ( fInfo.Exists )
           {    // remove the container file
               fInfo.Delete();
           }
           GlobalLog.LogStatus("Create test container.");
           stRoot = StorageRootWrapper.Open( "container.xmf" );
           fInfo = new FileInfo( "container.xmf" );
           if ( !fInfo.Exists )
           {
               GlobalLog.LogEvidence( "FAIL: Container file not created" );
               return;
           }
           strInfo = stRoot.CreateSubStorage("TestStorage");
           stRoot.Close();
           stRoot = StorageRootWrapper.Open( "container.xmf" );
           strInfo = stRoot.GetSubStorageInfo("TestStorage");
           // ***** End Test Cases Initialization ****
           // ***** Start Avalon Test ****
           name = strInfo.Name;
           // ***** End Avalon Test ****
           // ***** Start Validation Section ****
           if ( name == "TestStorage" )
           {
               GlobalLog.LogEvidence("PASS: Test passes" );
           }
           else
           {
               GlobalLog.LogEvidence("FAIL: Value of StorageInfo.Name property is different from expected." );
           }
           // ***** End Validation Section  ****
           // ***** Start Shut Down ****
           stRoot.Close();
           // remove the container file
           fInfo.Delete();
           // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          StorageInfo9
        ******************************************************************************/
        /// <summary>
        /// Try to create a storage with the same name as an existing stream.
        /// Validation is done by catching the exception - this call is expected to fail.
        /// </summary>
        public void StorageInfo9()
        {
            StorageRootWrapper stRoot   = null;
            StorageInfo strInfo         = null;
            StreamInfo  stInfo          = null;
            FileInfo  fInfo           = null;
            bool        bResult         = false;
            string      resultMsg       = "Unknown failure";

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {    // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create test container.");
            stRoot = StorageRootWrapper.Open( "container.xmf" );
            fInfo = new FileInfo( "container.xmf" );
            if ( !fInfo.Exists )
            {
                GlobalLog.LogEvidence( "FAIL: Container file not created" );
                return;
            }
            stInfo = stRoot.CreateStream( "TestName" );
            // ***** End Test Cases Initialization ****
            try
            {
                // ***** Start Avalon Test ****
                strInfo = stRoot.CreateSubStorage( "TestName" );
                // ***** End Avalon Test ****
                // ***** Start Validation Section ****
                throw new Microsoft.Test.TestValidationException( "Inside a storage, it's possible to create a substorage with the same name as an existing stream. This should not be allowed.");
            }
            catch (InvalidOperationException)
            {
                bResult = true;
                resultMsg = "Test passed:  the expected InvalidOperationException occurred.";
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
