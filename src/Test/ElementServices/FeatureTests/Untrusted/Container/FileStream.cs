// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.IO;
using System.IO.Packaging;
using System.Threading; 
using System.Windows.Threading;

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;

using Microsoft.Test.Container;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.Container
{
    /******************************************************************************
    * CLASS:          BVT_FileStream
    ******************************************************************************/
    /// <summary>
    /// test class for Container BVT_FileStream BVT cases
    /// These functions test the Container filestream buffer's ability to 
    /// read, write, seek, and get its length.
    /// </summary>
    /// <remarks>
    /// class inherits from TestCase, where ContextEnteringSupport will automatically create
    /// and enter a context when the constructor is called, and will automatically exit and 
    /// dispose the context when the destructor is called.
    /// </remarks>
    [Test(0, "Container.FileStream", TestCaseSecurityLevel.FullTrust, "BVT_FileStream")]
    public class BVT_FileStream : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion


        #region Constructor
        [Variation("FileStream1")]
        [Variation("FileStream2")]
        [Variation("FileStream3")]
        [Variation("FileStream4", Keywords = "MicroSuite")]
        [Variation("FileStream5")]
        [Variation("FileStream6")]
        [Variation("FileStream7")]
        [Variation("FileStream8")]
        [Variation("FileStream9")]
        [Variation("FileStream10")]
        [Variation("FileStream11")]
        [Variation("FileStream12")]
        [Variation("FileStream13")]

        /******************************************************************************
        * Function:          BVT_FileStream Constructor
        ******************************************************************************/
        public BVT_FileStream(string arg): base(TestCaseType.ContextEnteringSupport)
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
                case "FileStream1":
                    FileStream1();
                    break;
                case "FileStream2":
                    FileStream2();
                    break;
                case "FileStream3":
                    FileStream3();
                    break;
                case "FileStream4":
                    FileStream4();
                    break;
                case "FileStream5":
                    FileStream5();
                    break;
                case "FileStream6":
                    FileStream6();
                    break;
                case "FileStream7":
                    FileStream7();
                    break;
                case "FileStream8":
                    FileStream8();
                    break;
                case "FileStream9":
                    FileStream9();
                    break;
                case "FileStream10":
                    FileStream10();
                    break;
                case "FileStream11":
                    FileStream11();
                    break;
                case "FileStream12":
                    FileStream12();
                    break;
                case "FileStream13":
                    FileStream13();
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
        * Function:          FileStream1
        ******************************************************************************/
        /// <summary>
        /// Basic container Read/Write test. Writes 1024 bytes to a stream and then reads and verifies the content.
        /// </summary>
        /// <remarks>
        /// The test passes if each byte of the stream is what was written to it and the stream has the expected length.
        /// </remarks>        
        public void FileStream1()
        {
            GlobalLog.LogStatus("entering FileStream1");

            TestContainer   tc = new TestContainer();
            Stream      strm   = null;
            FileInfo    fInfo = null;
            int         i;
            byte[]      buffer;

            // ***** Start Test Cases Initialization ****

            // Also investigate where this fle is created - does container have its own storage space?
            // ANSWER: container.xmf is created in the local directory (same place as coretests.exe)
            // 
            // Clean existing file. 
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
                GlobalLog.LogStatus("container.xmf already existed (a previous testcase didn't clean up after itself)!- deleting file", ConsoleColor.Red);
            }
            
            strm = tc.CreateStream("container.xmf"); //create the container
            
            // init the buffer
            buffer = new byte[1024];
            for(i = 0; i < 1024; i++)
                buffer[i] = (byte)i;
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            strm.Write( buffer, 0, 1024 );
            // ***** End Avalon Test ****
            #region validation
            
            tc.CloseStream();
            strm = tc.OpenStream(); //open the existing container
            buffer = new byte[1024];

            // read the buffer back
#pragma warning disable CA2022 // Avoid inexact read
            strm.Read( buffer, 0, 1024 );
#pragma warning restore CA2022

            // check the content
            try
            {
                for(i = 0; i < 1024; i++)
                {
                    if ( buffer[i] != (byte)i ) // if a byte in buffer does not match
                    {//clean up and bail
                        throw new  Microsoft.Test.TestValidationException("Error in byte " + i + " value " + buffer[i]);
                    }
                }
                // all of the buffer was ok?
                if ( i != 1024 )
                {//clean up and bail
                    throw new  Microsoft.Test.TestValidationException("the buffer is the wrong size!\nexpected: 1024\nactual: " + i);
                }
                
                GlobalLog.LogStatus("end validation");
            }
            
            #endregion validation


            // ***** Start Shut Down ****
            finally 
            {
                tc.CloseStream();
            }

            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****


            GlobalLog.LogStatus("exiting FileStream1");
        }

        /******************************************************************************
        * Function:          FileStream2
        ******************************************************************************/
        /// <summary>
        /// Container Read/Write test with offset. Writes 1024-50 bytes to a stream and then
        /// reads and verifies the content. Use offset 50 in the buffer when call Stream.Write
        /// </summary>
        /// <remarks>
        /// Test passes if the first 50 bytes are not altered and the remaining bytes are correctly written
        /// My first port! t-mtung
        /// </remarks>
        public void FileStream2()
        {
            GlobalLog.LogStatus("entering FileStream2");   
            TestContainer tc = new TestContainer();
            Stream      strm   = null;
            FileInfo    fInfo = null;
            int         i;
            byte[]      buffer;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
                GlobalLog.LogStatus("container.xmf already existed. deleting file", ConsoleColor.Red);
            }
            GlobalLog.LogStatus("Create test container.");
            strm = tc.CreateStream("container.xmf");
            // init the buffer
            buffer = new byte[1024];
            for(i = 0; i < 1024; i++)
                buffer[i] = (byte)i;
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            strm.Write( buffer, 50, 1024-50 );
            // ***** End Avalon Test ****
            
            #region validation
            tc.CloseStream();
            strm = tc.OpenStream();
            // init the buffer with 0s
            buffer = new byte[1024];
            for(i = 0; i < 1024; i++)
                buffer[i] = 0;

#pragma warning disable CA2022 // Avoid inexact read
            // read the buffer back
            strm.Read( buffer, 50, 1024-50 );
#pragma warning restore CA2022
            // check the content
            try
            {

                for(i = 0; i < 1024; i++)
                {
                    if ( i < 50 )
                    {
                        // values before offset 50 should be intact = 0
                        if ( buffer[i] != 0 )
                        {
                            throw new Microsoft.Test.TestValidationException("Error in byte " + i + " value " + buffer[i]);
                        }
                    }
                    else
                    {
                        // values after offset 50 should be read from file
                        if ( buffer[i] != (byte)i )
                        {
                            throw new Microsoft.Test.TestValidationException("Error in byte " + i + " value " + buffer[i]);
                    
                        }
                    }
                }
                // all the buffer was ok?
                if ( i != 1024 )
                {
                    throw new Microsoft.Test.TestValidationException("The buffer is the wrong size! Expected 1024 bytes, got "+i+" bytes.");

                }
            }
                #endregion


                // ***** Start Shut Down ****
            finally
            {
                tc.CloseStream();
            }
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
            GlobalLog.LogStatus("exiting FileStream2");
        }
  
        /******************************************************************************
        * Function:          FileStream3
        ******************************************************************************/
        /// <summary>
        /// Container Seek from beginning test. Writes 1024 bytes to a stream and then
        /// Seeks to offset 50 and reads and verifies the content.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        public void FileStream3()
        {
            TestContainer tc = new TestContainer();
            Stream      strm   = null;
            FileInfo  fInfo = null;
            int         i;
            byte[]      buffer;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create test container.");
            strm = tc.CreateStream("container.xmf");
            // init the buffer
            buffer = new byte[1024];
            for(i = 0; i < 1024; i++)
                buffer[i] = (byte)i;
            strm.Write( buffer, 0, 1024 );
            tc.CloseStream();

            // open the container again
            strm = tc.OpenStream();
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            strm.Seek( 50, SeekOrigin.Begin );
            // ***** End Avalon Test ****
            #region validation
            try
            {
                // init the buffer with 0s
                buffer = new byte[1024];
                for(i = 0; i < 1024; i++)
                    buffer[i] = 0;

#pragma warning disable CA2022 // Avoid inexact read
                // read the buffer back
                strm.Read( buffer, 0, 1024-50 );
#pragma warning restore CA2022
                // check the content
                for(i = 0; i < 1024; i++)
                {
                    if ( i < (1024 - 50) )
                    {
                        // first 1024 - 50 values are from the file
                        if ( buffer[i] != (byte)(i + 50) )
                        {
                            throw new Microsoft.Test.TestValidationException("Error in byte " + i + " value " + buffer[i]);
                        }
                    }
                    else
                    {
                        // last 50 values should be 0s
                        if ( buffer[i] != 0 )
                        {
                            throw new Microsoft.Test.TestValidationException("Error in byte " + i + " value " + buffer[i]);
                        }
                    }
                }
                // all the buffer was ok?
                if ( i != 1024 )
                {
                    throw new Microsoft.Test.TestValidationException("Buffer is wrong size!");
                }
                
            }
                #endregion

                // ***** Start Shut Down ****
            finally
            {
                tc.CloseStream();
            }
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          FileStream4
        ******************************************************************************/
        /// <summary>
        /// Container Seek from end test. Writes 1024 bytes to a stream and then
        /// Seeks to offset 50 and reads and verifies the content.
        /// </summary>
        public void FileStream4()
        {
            TestContainer tc = new TestContainer();
            Stream      strm   = null;
            FileInfo  fInfo = null;
            int         i;
            byte[]      buffer;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create test container.");
           
            strm = tc.CreateStream("container.xmf");
            // init the buffer
            buffer = new byte[1024];
            for(i = 0; i < 1024; i++)
                buffer[i] = (byte)i;
            strm.Write( buffer, 0, 1024 );
            tc.CloseStream();

            // open the container again
            strm = tc.OpenStream();
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            strm.Seek( -(1024 - 50), SeekOrigin.End );
            // ***** End Avalon Test ****

            #region validation  
            try
            {
                // init the buffer with 0s
                buffer = new byte[1024];
                for(i = 0; i < 1024; i++)
                    buffer[i] = 0;

#pragma warning disable CA2022 // Avoid inexact read
                // read the buffer back
                strm.Read( buffer, 0, 1024-50 );
#pragma warning restore CA2022
                // check the content
                for(i = 0; i < 1024; i++)
                {
                    if ( i < (1024 - 50) )
                    {
                        // first 1024 - 50 values are from the file
                        if ( buffer[i] != (byte)(i + 50) )
                        {
                            throw new Microsoft.Test.TestValidationException( "Error in byte " + i + " value " + buffer[i]);
                        }
                    }
                    else
                    {
                        // last 50 values should be 0s
                        if ( buffer[i] != 0 )
                        {
                            throw new Microsoft.Test.TestValidationException( "Error in byte " + i + " value " + buffer[i]);
                        }
                    }
                }
                // all the buffer was ok?
                if ( i != 1024 )
                {
                    throw new Microsoft.Test.TestValidationException( "Buffer is wrong size!");
                }
            }
                #endregion 

                // ***** Start Shut Down ****
            finally
            {
               tc.CloseStream();
            }
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          FileStream5
        ******************************************************************************/
        /// <summary>
        /// Container Seek from current position. Writes 1024 bytes to a stream and then
        /// Seeks to offset 50 and reads and verifies the content.
        /// </summary>
        public void FileStream5()
        {
            TestContainer tc = new TestContainer();
            Stream      strm   = null;
            FileInfo  fInfo = null;
            int         i;
            byte[]      buffer;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            
            strm = tc.CreateStream("container.xmf");
            // init the buffer
            buffer = new byte[1024];
            for(i = 0; i < 1024; i++)
                buffer[i] = (byte)i;
            strm.Write( buffer, 0, 1024 );
            tc.CloseStream();

            // open the container again
            
            strm = tc.OpenStream();
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            // Seek 25 from current offset twice should seek 50 forward
            // since the current offset in the beginning is 0, this
            // should bring us to offset 50
            strm.Seek( 25, SeekOrigin.Current );
            strm.Seek( 25, SeekOrigin.Current );
            // ***** End Avalon Test ****

            #region validation 
            try
            {
                // init the buffer with 0s
                buffer = new byte[1024];
                for(i = 0; i < 1024; i++)
                    buffer[i] = 0;

#pragma warning disable CA2022 // Avoid inexact read
                // read the buffer back
                strm.Read( buffer, 0, 1024-50 );
#pragma warning restore CA2022
                // check the content
                for(i = 0; i < 1024; i++)
                {
                    if ( i < (1024 - 50) )
                    {
                        // first 1024 - 50 values are from the file
                        if ( buffer[i] != (byte)(i + 50) )
                        {
                            throw new Microsoft.Test.TestValidationException( "Error in byte " + i + " value " + buffer[i]);
                        }
                    }
                    else
                    {
                        // last 50 values should be 0s
                        if ( buffer[i] != 0 )
                        {
                            throw new Microsoft.Test.TestValidationException( "Error in byte " + i + " value " + buffer[i]);
                    
                        }
                    }
                }
                // all the buffer was ok?
                if ( i != 1024 )
                {
                    throw new Microsoft.Test.TestValidationException("Buffer is wrong size!");
                }
            }
                #endregion

                // ***** Start Shut Down ****
            finally
            {
                tc.CloseStream();
            }
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          FileStream6
        ******************************************************************************/
        /// <summary>
        /// Container Set Position property test. Writes 1024 bytes to a stream and then
        /// Sets the Position to 50 and reads and verifies the content.
        /// </summary>
        public void FileStream6()
        {
            TestContainer tc = new TestContainer();
            Stream      strm   = null;
            FileInfo  fInfo = null;
            int         i;
            byte[]      buffer;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create test container.");
            strm = tc.CreateStream("container.xmf");
            // init the buffer
            buffer = new byte[1024];
            for(i = 0; i < 1024; i++)
                buffer[i] = (byte)i;
            strm.Write( buffer, 0, 1024 );
            tc.CloseStream();

            // open the container again
            strm = tc.OpenStream();
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            strm.Position = 50;
            // ***** End Avalon Test ****

            #region validation 
            try 
            {
                // init the buffer with 0s
                buffer = new byte[1024];
                for(i = 0; i < 1024; i++)
                    buffer[i] = 0;

#pragma warning disable CA2022 // Avoid inexact read
                // read the buffer back
                strm.Read( buffer, 0, 1024-50 );
#pragma warning restore CA2022
                // check the content
                for(i = 0; i < 1024; i++)
                {
                    if ( i < (1024 - 50) )
                    {
                        // first 1024 - 50 values are from the file
                        if ( buffer[i] != (byte)(i + 50) )
                        {
                            throw new Microsoft.Test.TestValidationException( "Error in byte " + i + " value " + buffer[i]);
                        }
                    }
                    else
                    {
                        // last 50 values should be 0s
                        if ( buffer[i] != 0 )
                        {
                            throw new Microsoft.Test.TestValidationException( "Error in byte " + i + " value " + buffer[i]);
                        }
                    }
                }
                // all the buffer was ok?
                if ( i != 1024 )
                {
                    throw new Microsoft.Test.TestValidationException("Buffer is wrong size!");
                }
            }
                #endregion

                // ***** Start Shut Down ****
            finally
            {
                tc.CloseStream();
            }
            // remove the container file
          
                fInfo.Delete();
            
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          FileStream7
        ******************************************************************************/
        /// <summary>
        /// Container Get Position property test. Writes 1024 bytes to a stream, then
        /// Seeks to offset 50 and verifies the position.
        /// </summary>
        public void FileStream7()
        {
            TestContainer tc = new TestContainer();
            Stream      strm   = null;
            FileInfo  fInfo = null;
            long        i;
            byte[]      buffer;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create test container.");
            strm = tc.CreateStream("container.xmf");
            // init the buffer
            buffer = new byte[1024];
            for(i = 0; i < 1024; i++)
                buffer[i] = (byte)i;
            strm.Write( buffer, 0, 1024 );
            tc.CloseStream();
            // open the container again
            strm = tc.OpenStream();
            strm.Seek( 50, SeekOrigin.Begin );
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            i = strm.Position;
            // ***** End Avalon Test ****

            #region validation 
            try
            {
                if ( i != 50 )
                {
                    throw new Microsoft.Test.TestValidationException("Position expected 50, got " + i );
                }
            }
                #endregion

                // ***** Start Shut Down ****
            finally
            {
                tc.CloseStream();
            }
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          FileStream8
        ******************************************************************************/
        /// <summary>
        /// Container Get Length property test. Writes 1024 bytes to a stream, then
        /// Gets the Length property and verifies the value.
        /// </summary>
        public void FileStream8()
        {
            TestContainer tc = new TestContainer();
            Stream      strm   = null;
            FileInfo  fInfo = null;
            long        i;
            byte[]      buffer;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create test container.");
            strm = tc.CreateStream("container.xmf");
            // init the buffer
            buffer = new byte[1024];
            for(i = 0; i < 1024; i++)
                buffer[i] = (byte)i;
            strm.Write( buffer, 0, 1024 );
            tc.CloseStream();

            // open the container again
            strm = tc.OpenStream();
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            i = strm.Length;
            // ***** End Avalon Test ****

            #region validation 
            try
            {
                if ( i != 1024 )
                {
                    throw new Microsoft.Test.TestValidationException( "Length expected 1024, got " + i );
                }
            }
                #endregion

                // ***** Start Shut Down ****
            finally
            {
                tc.CloseStream();
            }
                // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          FileStream9
        ******************************************************************************/
        /// <summary>
        /// Test container close befor closing the stream. 
        /// Writes 1024 bytes to a stream, then closes the container.
        /// Closes the stream then gets the Length property and verifies the value.
        /// This is a repro for 

        public void FileStream9()
        {
            TestContainer tc = new TestContainer();
            
            Stream      strm   = null;
            FileInfo  fInfo = null;
            long        i;
            byte[]      buffer;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create test container.");
            strm = tc.CreateStream("container.xmf");
            // init the buffer
            buffer = new byte[1024];
            for(i = 0; i < 1024; i++)
                buffer[i] = (byte)i;
            strm.Write( buffer, 0, 1024 );
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            // first close the container. then close the stream
            // as per 
            tc.CloseStream();
            // ***** End Avalon Test ****

            GlobalLog.LogStatus("Validate content.");
            #region validation 
            // open the container again and validate the content was written
            try
            {
                strm = tc.OpenStream();

                i = strm.Length;

                if ( i != 1024 )
                {
                    throw new Microsoft.Test.TestValidationException( "Length expected 1024, got " + i );
                }
            }
                #endregion

                // ***** Start Shut Down ****
            finally
            {
                tc.CloseStream();
            }
                // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }   

        /******************************************************************************
        * Function:          FileStream10
        ******************************************************************************/
        /// <summary>
        /// Basic container Read/Write test. Writes 1024 bytes to a stream and then
        /// reads and verifies the content. Uses read and write with non zero offset (24).
        /// </summary>
        public void FileStream10()
        {
            TestContainer tc = new TestContainer();
            
            Stream      strm   = null;
            FileInfo  fInfo = null;
            int         i;
            byte[]      buffer;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create test container.");
            strm = tc.CreateStream("container.xmf");
            // init the buffer
            buffer = new byte[1024];
            for(i = 0; i < 1024; i++)
                buffer[i] = (byte)i;
            // ***** End Test Cases Initialization ****

            // ***** Avalon Test ****
            strm.Write( buffer, 24, 1000 );
            // ***** End Avalon Test ****

            #region validation 
            try
            {
                tc.CloseStream();
                strm = tc.OpenStream();
                buffer = new byte[1024];
                // fill the buffer with different data
                for(i = 0; i < 1024; i++)
                    buffer[i] = (byte)(-i);
#pragma warning disable CA2022 // Avoid inexact read
                // read the buffer back
                strm.Read( buffer, 24, 1000 );
#pragma warning restore CA2022
                // check the content
                for(i = 0; i < 24; i++)
                {
                    if ( buffer[i] != (byte)(-i) )
                    {
                        throw new Microsoft.Test.TestValidationException("Error in byte " + i + " value " + buffer[i]);
                    }
                }
                // all of the buffer was ok?
                if ( i == 24 )
                {
                    for(i = 24; i < 1024; i++)
                    {
                        if ( buffer[i] != (byte)i )
                        {
                            throw new Microsoft.Test.TestValidationException("Error in byte " + i + " value " + buffer[i]);
                        }
                    }
                    // all of the buffer was ok?
                    if ( i != 1024 )
                    {
                   
                        throw new Microsoft.Test.TestValidationException("Buffer is wrong size!");
                    }
                }
            }
                #endregion

                // ***** Start Shut Down ****
            finally
            {
                tc.CloseStream();
            }
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          FileStream11
        ******************************************************************************/
        /// <summary>
        /// Test Write with non zero offset. Use offset equal to the buffer size.
        /// Validation is done by checking the lenght of the stream written.
        /// </summary>
        public void FileStream11()
        {
            TestContainer tc = new TestContainer();
            
            Stream      strm   = null;
            FileInfo  fInfo = null;
            long        i;
            byte[]      buffer;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create test container.");
            strm = tc.CreateStream("container.xmf");
            // init the buffer
            buffer = new byte[1024];
            for(i = 0; i < 1024; i++)
                buffer[i] = (byte)i;
            // ***** End Test Cases Initialization ****
            // ***** Avalon Test ****
            strm.Write( buffer, 1024, 0 );
            // ***** End Avalon Test ****

            #region validation 
            try
            {
                tc.CloseStream();

                // open the container again
                strm = tc.OpenStream();

                i = strm.Length;
                if ( i !=0)
                {
                    throw new Microsoft.Test.TestValidationException( "Length expected 0, got " + i );
                }
            }
                #endregion

                // ***** Start Shut Down ****
            finally
            {
                tc.CloseStream();
            }
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          FileStream12
        ******************************************************************************/
        /// <summary>
        /// Test Write with non zero offset. Use offset equal to the buffer size 
        /// and negative length. Validation is done by checking that exception occured
        /// Review with Roger Cheng Should Container write allow negative seek?
        /// ANSWER: by-design new behavior. Negative numbers used to be accepted with 
        /// disastrous results
        /// </summary>
        public void FileStream12()
        {
            TestContainer tc = new TestContainer();
            
            Stream      strm   = null;
            FileInfo  fInfo = null;
            long        i;
            byte[]      buffer;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create test container.");
            strm = tc.CreateStream("container.xmf");
            // init the buffer
            buffer = new byte[1024];
            for(i = 0; i < 1024; i++)
                buffer[i] = (byte)i;
            // ***** End Test Cases Initialization ****
            // ***** Avalon Test ****
            try
            {
                strm.Write( buffer, 1024, -23 );
                // ***** End Avalon Test ****

              
                throw new Microsoft.Test.TestValidationException("Expected exception did not occur.");
            }
            catch(ArgumentOutOfRangeException e)
            {//just catching the appropriate exception type is good enough
                if (e.Message=="Non-negative number required.")
                {}// Do I NEED to use e just because I declared it?
                //PASSED! - Do nothing
            }
                // ***** Start Shut Down ****
            finally
            {
                tc.CloseStream();
            }
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }

        /******************************************************************************
        * Function:          FileStream13
        ******************************************************************************/
        /// <summary>
        /// Container Read test. Writes 1024 bytes to a stream and then
        /// reads and verifies the content. Uses read with offset equals the 
        /// size fo the stream. Verify the buffer was intact.
        /// </summary>
        public void FileStream13()
        {
            TestContainer tc = new TestContainer();
            
            Stream      strm   = null;
            FileInfo  fInfo = null;
            int         i;
            byte[]      buffer;

            // ***** Start Test Cases Initialization ****
            // Clean existing file.
            fInfo = new FileInfo( "container.xmf" );
            if ( fInfo.Exists )
            {   // remove the container file
                fInfo.Delete();
            }
            GlobalLog.LogStatus("Create test container.");
            strm = tc.CreateStream("container.xmf");
            // init the buffer
            buffer = new byte[1024];
            for(i = 0; i < 1024; i++)
                buffer[i] = (byte)i;
            strm.Write( buffer, 0, 1024 );
            tc.CloseStream();
            strm = tc.OpenStream();
            buffer = new byte[1024];
            // fill the buffer with different data
            for(i = 0; i < 1024; i++)
                buffer[i] = (byte)(-i);
            // read the buffer back
            // ***** End Test Cases Initialization ****

#pragma warning disable CA2022 // Avoid inexact read
            // ***** Avalon Test ****
            strm.Read( buffer, 1024, 0 );
#pragma warning restore CA2022
            // ***** End Avalon Test ****

            #region validation 
            // check the content
            try
            {
                for(i = 0; i < 1024; i++)
                {
                    if ( buffer[i] != (byte)(-i) )
                    {
                   
                        throw new Microsoft.Test.TestValidationException("Error in byte " + i + " value " + buffer[i]);
                    }
                }
                // all of the buffer was ok?
                if ( i != 1024 )
                {
                
                    throw new Microsoft.Test.TestValidationException("Buffer is wrong size!");
                }
            }
                #endregion

                // ***** Start Shut Down ****
            finally
            {
                tc.CloseStream();
            }
            // remove the container file
            fInfo.Delete();
            // ***** End Shut Down  ****
        }
        #endregion
    }
}

