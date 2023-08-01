// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Container;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.Container
{ 
    /******************************************************************************
    * CLASS:          BVT_NamingSupport
    ******************************************************************************/
    /// <summary>
    /// Description:
    /// BVT tests for naming support in container code. 
    /// Covers both streams and storages. 
    /// </summary>
    /// <remarks>
    /// class inherits from TestCase, where ContextEnteringSupport will automatically create
    /// and enter a context when the constructor is called, and will automatically exit and 
    /// dispose the context when the destructor is called.
    /// </remarks>
    [Test(0, "Container.NamingSupport", TestCaseSecurityLevel.FullTrust, "BVT_NamingSupport")]
    public class BVT_NamingSupport : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion


        #region Constructor

        [Variation("NamingSupport1", Disabled=true)]
        [Variation("NamingSupport2", Disabled=true)]
        [Variation("NamingSupport3", Disabled=true)]

        /******************************************************************************
        * Function:          BVT_NamingSupport Constructor
        ******************************************************************************/
        public BVT_NamingSupport(string arg): base(TestCaseType.ContextEnteringSupport)
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
                case "NamingSupport1":
                    NamingSupport1();
                    break;
                case "NamingSupport3":
                    NamingSupport2();
                    break;
                case "NamingSupport2":
                    NamingSupport3();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public and Protected Members
        /// <summary>
        /// Variable test names for storages, fixed name for streams.
        /// </summary>
        /// <remarks>
        /// Streams of a fixed name are created under storages of different test names. 
        /// Many validations are performed, such as property names and stream I/O.
        /// </remarks>
        public void NamingSupport1 ()
        {
            for (int i = 0; i < _strings.Length; i++)
            {
                TestNamedStoragesAndStreams (_strings[i], "TestStream", true);
            }

            AutoData autoData = new AutoData ();
            for (int i = 0; i < 10; i++)
            {
                TestNamedStoragesAndStreams (autoData.GetTestString (i), "TestStream", true);
            }
        }


        /******************************************************************************
        * Function:          NamingSupport2
        ******************************************************************************/
        /// <summary>
        /// Fixed name for storages, variable names for streams.
        /// </summary>
        /// <remarks>
        /// Streams of different test names are created under storages of a fixed name. 
        /// Many validations are performed, such as property names and stream I/O.
        /// </remarks>
        public void NamingSupport2 ()
        {
            for (int i = 0; i < _strings.Length; i++)
            {
                TestNamedStoragesAndStreams ("TestStorage", _strings[i], true);
            }

            AutoData autoData = new AutoData ();
            for (int i = 0; i < 10; i++)
            {
                TestNamedStoragesAndStreams ("TestStorage", autoData.GetTestString (i), true);
            }
        }

        /******************************************************************************
        * Function:          NamingSupport3
        ******************************************************************************/
        /// <summary>
        /// Variable names for storages, variable names for streams.
        /// </summary>
        /// <remarks>
        /// Streams of different test names are created under storages of the same test name. 
        /// Many validations are performed, such as property names and stream I/O.
        /// </remarks>
        [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
        public void NamingSupport3 ()
        {
            for (int i = 0; i < _strings.Length; i++)
            {
                TestNamedStoragesAndStreams (_strings[i], _strings[i], true);
            }

            AutoData autoData = new AutoData ();

            for (int i = 0; i < 10; i++)
            {
                TestNamedStoragesAndStreams (autoData.GetTestString (i), autoData.GetTestString (i), true);
            }
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          TestNamedStoragesAndStreams
        ******************************************************************************/
        /// <summary>
        /// This function is shared by various test cases. 
        /// Depending on the parameters passed, it creates a structure of storages and streams,
        /// inside a new container.
        /// It then performs a number of validations based on this structure.
        /// </summary>
        /// <param name="storageName">Name for the test storage to be created.</param>
        /// <param name="streamName">Name for the test stream to be created.</param>
        /// <param name="createAtRootLevel">Whether to create the test storage at root level or a level below that.</param>
        private void TestNamedStoragesAndStreams (String storageName, String streamName, bool createAtRootLevel)
        {
            StorageInfo parentStorage = null;

            // Create a new container.
            String filePath = Path.GetTempFileName ();
            StorageRootWrapper root = StorageRootWrapper.Open (filePath, FileMode.Create);

            if (createAtRootLevel)
            {
                parentStorage = root;
            }
            else
            {
                // Create a first level storage directly below root
                // Our test storage and stream will be created below this storage.
                StorageInfo childOfRoot = root.CreateSubStorage("ChildOfRoot");
                parentStorage = childOfRoot;
            }

            // Create a test storage under parentStorage. 
            StorageInfo testStorageInfo = parentStorage.CreateSubStorage(storageName);

            // Make sure the test storage is created.
            if (!parentStorage.SubStorageExists(storageName))
            {
                throw new Microsoft.Test.TestValidationException ("The property \"Exists\" of the storage does not match the expected value");
            }

            // Create a test stream under the test storage.
            StreamInfo testStreamInfo = testStorageInfo.CreateStream(streamName);
            Stream testStream = testStreamInfo.GetStream ();

            // Make sure the test stream is created.
            if (!testStorageInfo.StreamExists(streamName))
            {
                throw new Microsoft.Test.TestValidationException ("The property \"Exists\" of the stream does not match the expected value");
            }

            // Try to create another stream with the same name. Make sure it fails.
            try
            {
                StreamInfo anotherStreamInfo = testStorageInfo.CreateStream(streamName);
                throw new Microsoft.Test.TestValidationException ("Exception expected, but not thrown, during attempted creation of a duplicate stream.");
            }
            catch (IOException) { }

            // Write some data into the stream.
            StreamWriter writer = new StreamWriter (testStream);

            writer.WriteLine ("Hello World");
            writer.Flush ();
            writer.Close ();
            testStream.Close ();

            // Close everything.
            root.Close ();

            // Now open all the storages and streams again. 
            // This makes sure that everything persisted correctly when we closed the container
            // in the previous step.
            root = StorageRootWrapper.Open (filePath, FileMode.Open, FileAccess.ReadWrite);
            if (createAtRootLevel)
            {
                parentStorage = root;
            }
            else
            {
                // Open the first level storage "ChildOfRoot", created before.
                // Our test storage and stream will be below this storage.
                StorageInfo childOfRoot = root.GetSubStorageInfo("ChildOfRoot");
                parentStorage = childOfRoot;
            }

            testStorageInfo = parentStorage.GetSubStorageInfo(storageName);
            testStreamInfo = testStorageInfo.GetStreamInfo(streamName);

            // Now perform a series of validations
            if (!parentStorage.SubStorageExists(storageName))
            {
                throw new Microsoft.Test.TestValidationException ("The property \"Exists\" of the storage does not match the expected value");
            }

            if (!testStorageInfo.StreamExists(streamName))
            {
                throw new Microsoft.Test.TestValidationException ("The property \"Exists\" of the stream does not match the expected value");
            }

            // Try to create another stream with the same name. Make sure it fails.
            try
            {
                StreamInfo anotherStreamInfo = testStorageInfo.CreateStream(streamName);
                throw new Microsoft.Test.TestValidationException ("Exception expected, but not thrown, during attempted creation of a duplicate stream.");
            }
            catch (IOException) { }

            // The array of storages returned by the parentStorage should contain our 
            // storage object with correct "Name" property.       
            StorageInfo[] storages = parentStorage.GetSubStorages();
            if (!(storages[0].Name.Equals (storageName)))
            {
                throw new Microsoft.Test.TestValidationException ("The property \"Name\" of the storage does not match the expected value");
            }

            // Array of streams returned by the parentStorage should be empty.
            StreamInfo[] parentStorageStreams = parentStorage.GetStreams();
            if (parentStorageStreams.Length != 0)
            {
                throw new Microsoft.Test.TestValidationException("Array of streams returned by the parentStorage should be empty.");
            }

            // The array of streams returned by the testStorage should contain our
            // stream with correct "Name" property.
            StreamInfo[] streams = testStorageInfo.GetStreams();
            if (!(streams[0].Name.Equals (streamName)))
            {
                throw new Microsoft.Test.TestValidationException ("The property \"Name\" of the stream does not match the expected value");
            }

            // Read the previously written data from the testStream and see if it matches
            // the written data.
            testStream = testStreamInfo.GetStream (FileMode.Open, FileAccess.ReadWrite);

            StreamReader reader = new StreamReader (testStream);

            if (!(reader.ReadLine ().Equals ("Hello World")))
            {
                throw new Microsoft.Test.TestValidationException ("Stream I/O error.");
            }

            reader.Close ();
            testStream.Close ();

            // Delete the test storage and stream. 
            // Also, try deleting them again.
            // This should NOT throw exceptions, as per the design. 
            // It should silently ignore the second request.
            testStorageInfo.DeleteStream(streamName);
            testStorageInfo.DeleteStream(streamName);
            parentStorage.DeleteSubStorage(storageName);
            parentStorage.DeleteSubStorage(storageName);                            

            // Clean-up
            root.Close ();
            DeleteFile(filePath);
        }

        /// <summary>
        ///  Internal utility function to delete the file specified by the given path.
        /// </summary>
        /// <param name="filePath">Full path of the file to be deleted.</param>
        private void DeleteFile(String filePath)
        {
            FileInfo fileInfo = new FileInfo (filePath);

            if (fileInfo.Exists)
            {
                fileInfo.Delete ();
            }

            // If the file still exists, for some reason, fail the test.
            if (File.Exists (filePath))
            {
                throw new Microsoft.Test.TestSetupException("File " + filePath + " not deleted.");
            }
        }

        // Valid names
        String[] _strings = { "a", 
            "Hello World", 
            "Hello ' \" - + = World",
            "Hello `~@#$%^&*()[]_ World",
            "0123456789 , 0123456789 ; 0123",              // Length = 30
            "abcdefghij . ABCDEFGHIJ ? {}<>",              // Length = 30
            "0123456789 , 0123456789 ; 01234",             // Length = 31
            "abcdefghij . ABCDEFGHIJ ? {}<>|",             // Length = 31
        };

        // Invalid names - either contain one of the 4 special characters ( \ : / ! ) 
        // or are more than 31 characters long
        String[] _badNames = {
            "H:llo World", // One special characters, length <= 31
            "H/llo World", // One special character, length <= 31
            "H!llo World", // One special character, length <= 31
            "H:l/o! World!", // Multiple special characters, length <= 31
            "0123456789 : 0123456789 ! 0123", // Special characters, length = 30
            "abcdefghij / ABCDEFGHIJ : {}<>", // Special characters, length = 30
            "0123456789 : 0123456789 ! 01234", // Special characters, length = 31
            "abcdefghij / ABCDEFGH : {}<>:||", // Special characters, length = 31
            "0123456789 0123456789 0123456789", // No special characters, length = 32
            "0123456789 0123456789 0123456789a", // No special characters, length = 33
            "0123456789 0123456789 0123456789 0123456789 012345", // No special characters, length = 50
            "0123456789 :/!3456789 0123456789", // Special characters, length = 32
            "012345678| 0123:56/89 !12`45#789a", // Special characters, length = 33
            "ABCDE: 0123/ :/! | !@#$%^&* ()-=_+[]{};'\"'<>,.?`~a" // Special characters, length = 50
        };
        #endregion
    }
}
