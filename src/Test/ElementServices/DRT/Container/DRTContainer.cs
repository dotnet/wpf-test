// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***************************************************************************\
*
*
* Description:
*   The set of Developer Regression Tests for the container.
*
*/

using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.RightsManagement;

using MS.Utility;

namespace DRT
{

/// <summary>
/// Container DRT class, all tests live as methods within this class.
/// </summary>
class ContainerTests
{
    const string testFileName = "DRT01.container";
    private static SpewWriter Log = new SpewWriter();
    private static readonly string sc_appManifest = "<manifest></manifest>";

    private static SecureEnvironment _secureEnvironment;

    //This is used to switch between default mode and RM mode.
    //RM is special mode where users need to install windows client RM and set it up before running this DRT.
    private static bool   _rmClientSetup = false;



    /// <summary>
    /// The main DRT test method that will in turn call every container DRT in
    /// turn
    /// </summary>
    /// <returns>0 if all are successful, 1 if there is at least one failure</returns>
    [STAThread]
    public static int Main(string[] cmdParams)
    {
        if( cmdParams.Length == 1 && cmdParams[0] == "/verbose" )
            Log.Verbose = true;
        if( cmdParams.Length == 1 && cmdParams[0] == "/rmclient" )
            _rmClientSetup = true;

         if( cmdParams.Length == 2 ) 
        {
            if ( cmdParams[0] == "/verbose" || cmdParams[1] == "/verbose" )
                Log.Verbose = true;
            if ( cmdParams[1] == "/rmclient"  || cmdParams[1] == "/rmclient"  )
                _rmClientSetup = true;
        }
        
        int result = 0;

        Log.WriteLine();
        Log.WriteLine();
        Log.Banner("Container", "Microsoft");


        if (_rmClientSetup)
        {
            Log.WriteLine("====Securing the environment for Encrypted Package Rights Management.>====");
            _secureEnvironment = SecureEnvironment.Create
                                            (sc_appManifest, AuthenticationType.Windows, UserActivationMode.Permanent);
        }
        else
        {
            _secureEnvironment = null;
        }

                                                     Log.WriteLine();  // Do linefeed between tests
        result += HelloWorldDRT();      Log.WriteLine();  // Do linefeed between tests
        result += BasicAccess();            Log.WriteLine();  // Do linefeed between tests
        result += StreamOptionTests();            Log.WriteLine();  // Do linefeed between tests
        result += StorageMethods();     Log.WriteLine();  // Do linefeed between tests
        result += StreamPM();           Log.WriteLine();  // Do linefeed between tests
        result += RegressionTests();    Log.WriteLine();  // Do linefeed between tests

        if( result > 0)
        {
            Log.AlwaysWriteLine("One or more container DRTs failed");
            result = 1;
        }
        else
        {
            Log.AlwaysWriteLine("All container DRTs passed");
        }

        Log.WriteLine();
        Log.WriteLine();

        return result;
    }


    /// <summary>
    /// Simple "Hello World" test.  This creates a file, writes a string to
    /// it, closes the file, opens it again, and reads back the string.
    /// </summary>
    /// <returns></returns>
    static int HelloWorldDRT()
    {
        Log.WriteLine("====<Initiating HelloWorldDRT>====");
        // If the DRT test file exists, delete it.
        FileInfo    testFileInfo= new FileInfo(testFileName);

        if( testFileInfo.Exists )
        {
            Log.WriteLine("Deleting test file from past test run");
            testFileInfo.Delete();
        }

        if (_rmClientSetup)
        {
            // Create DRT test file
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            EncryptedPackageEnvelope myFile = EncryptedPackageEnvelope.Create(testFileName, publishLicense, cryptoProvider);
            StorageInfo rootStorage = myFile.StorageInfo;

            StreamInfo  myData      = rootStorage.CreateStream( "Hello World stream" );

            Stream      dataStream  = myData.GetStream();
            StreamWriter writer     = new StreamWriter(dataStream); // Frameworks class

            writer.WriteLine("Hello World!");

            writer.Close();
            myFile.Close(); // Implicitly shuts down everything else relating to this container
            // Verify file exists

            testFileInfo= new FileInfo(testFileName);
            if( !testFileInfo.Exists )
            {
                Log.WriteLine("Creation test failed - file's not there");
                return 1;
            }

            // Open it back up
            myFile = EncryptedPackageEnvelope.Open(testFileName);
            rootStorage = myFile.StorageInfo;

            myData = rootStorage.GetStreamInfo( "Hello World stream" );
            dataStream = myData.GetStream();
            StreamReader reader = new StreamReader(dataStream);

            if( "Hello World!" != reader.ReadLine() )
            {
                Log.WriteLine("Data verification failed");
                return 1;
            }

            reader.Close();
            myFile.Close(); // Implicitly shuts down everything else relating to this container
        }
        else
        {
            // Create DRT test file
            Log.WriteLine("Creating the file " + testFileName);
            StorageRootWrapper rootStorage = StorageRootWrapper.Open(testFileName, FileMode.CreateNew);

            StreamInfo  myData      = rootStorage.CreateStream( "Hello World stream" );

            Stream      dataStream  = myData.GetStream(FileMode.Open, FileAccess.Write);
            StreamWriter writer     = new StreamWriter(dataStream); // Frameworks class

            writer.WriteLine("Hello World!");
            writer.Close();

            rootStorage.Close(); // Implicitly shuts down everything else relating to this container
            // Verify file exists

            testFileInfo= new FileInfo(testFileName);
            if( !testFileInfo.Exists )
            {
                Log.WriteLine("Creation test failed - file's not there");
                return 1;
            }

            Log.WriteLine("Opening the file " + testFileName);
            // Open it back up
            rootStorage = StorageRootWrapper.Open(testFileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            myData = rootStorage.GetStreamInfo( "Hello World stream" );
            dataStream = myData.GetStream(FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(dataStream);

            if( "Hello World!" != reader.ReadLine() )
            {
                Log.WriteLine("Data verification failed");
                return 1;
            }

            reader.Close();
            rootStorage.Close(); // Implicitly shuts down everything else relating to this container
        }

        // All is good, verify that file can be deleted
        testFileInfo.Delete();

        // Everything passed, we rock.
        Log.WriteLine("====<HelloWorldDRT completed>====");

        return 0;
    }

     /// <summary>
    /// Simple basic access tests  This creates a file, creates storage and a stream in it. Make sure they exist and can be
    /// deleted.
    /// </summary>
    /// <returns></returns>
    static int BasicAccess()
    {
        Log.WriteLine("====<Initiating Basic Access test>====");

        // Create a structure in a test file
        FileInfo testFileInfo = new FileInfo(testFileName);
        if( testFileInfo.Exists )
        {
            Log.WriteLine("Deleting test file from past test run");
            testFileInfo.Delete();
        }
        
        if (_rmClientSetup)
        {
            // Create DRT test file
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            EncryptedPackageEnvelope myFile = EncryptedPackageEnvelope.Create(testFileName, publishLicense, cryptoProvider);
            StorageInfo rootStorage = myFile.StorageInfo;
         
            StorageInfo strInfo = rootStorage.CreateSubStorage("TestSubStorage");
            StreamInfo stInfo = rootStorage.CreateStream("TestStream");
         
            if (! rootStorage.SubStorageExists("TestSubStorage"))
            {
                Log.WriteLine("Created SubStorage does not exist!");
                return 1;
            }
            if (! rootStorage.StreamExists("TestStream"))
            {
                Log.WriteLine("Created Stream does not exist!");
                return 1;
            }
            rootStorage.DeleteSubStorage("TestSubStorage");
            rootStorage.DeleteStream("TestStream");
            if (rootStorage.SubStorageExists("TestSubStorage"))
            {
                Log.WriteLine("Deleted SubStorage still exists!");
                return 1;
            }
            if(rootStorage.StreamExists("TestStream"))
            {
                Log.WriteLine("Deleted Stream still exists!");
                return 1;
            }

            myFile.Close();
        }
        else
        {
            StorageRootWrapper  myFile = StorageRootWrapper.Open(testFileName, FileMode.CreateNew);

            StorageInfo strInfo = myFile.CreateSubStorage("TestSubStorage");
            StreamInfo stInfo = myFile.CreateStream("TestStream");

            if (! myFile.SubStorageExists("TestSubStorage"))
            {
                Log.WriteLine("Created SubStorage does not exist!");
                return 1;
             }
            if (! myFile.StreamExists("TestStream"))
            {
                Log.WriteLine("Created Stream does not exist!");
                return 1;
            }
            myFile.DeleteSubStorage("TestSubStorage");
            myFile.DeleteStream("TestStream");
            if (myFile.SubStorageExists("TestSubStorage"))
            {
                Log.WriteLine("Deleted SubStorage still exists!");
                return 1;
            }
            if(myFile.StreamExists("TestStream"))
            {
                Log.WriteLine("Deleted Stream still exists!");
                return 1;
            }

            myFile.Close();
        }

        // Need to re-establish FileInfo class even though it points to the same file.
        testFileInfo = new FileInfo(testFileName);
        testFileInfo.Delete();

        // Everything passed.
        Log.WriteLine("====<BasicAccess test completed>====");

        return 0;
    }

    ///Streamoption tests - Test CompressionOption and EncryptionOption while creating a stream.
     static int StreamOptionTests()
    {
        Log.WriteLine("====<Initiating Stream Option test>====");

        int retVal = 0;

        string rm1FileName = "RMTest1";
        string rm2FileName = "RMTest2";
        string rm3FileName = "RMTest3";
        string rm4FileName = "RMTest4";
        // Create a structure in a test file
        FileInfo testFileInfo = new FileInfo(rm1FileName);
        if( testFileInfo.Exists )
        {
            Log.WriteLine("Deleting test file from past test run");
            testFileInfo.Delete();
        }
        
        if (_rmClientSetup)
        {
            // Create DRT test file - Compression - Yes, Encryption- None
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            EncryptedPackageEnvelope myFile = EncryptedPackageEnvelope.Create(rm1FileName, publishLicense, cryptoProvider);
            StorageInfo rootStorage = myFile.StorageInfo;
         
            StorageInfo storageInfo = rootStorage.CreateSubStorage("TestSubStorage");
            StreamInfo myData = storageInfo.CreateStream("Stream1", CompressionOption.Normal, EncryptionOption.None);
            Stream    dataStream  = myData.GetStream();
            StreamWriter    writer     = new StreamWriter(dataStream); // Frameworks class
            writer.WriteLine("Hello World!");
            writer.Close();
            myFile.Close();

            // Create DRT test file - Compression - Yes, Encryption - RightsManagement
            myFile = EncryptedPackageEnvelope.Create(rm2FileName, publishLicense, cryptoProvider);
            rootStorage = myFile.StorageInfo;
         
            storageInfo = rootStorage.CreateSubStorage("TestSubStorage");
            myData = storageInfo.CreateStream("Stream1", CompressionOption.Fast, EncryptionOption.RightsManagement);
            dataStream  = myData.GetStream();
            writer     = new StreamWriter(dataStream); // Frameworks class
            writer.WriteLine("Hello World!");
            writer.Close();
            myFile.Close();

            // Create DRT test file - Compression - No, Encryption - RightsManagement
            myFile = EncryptedPackageEnvelope.Create(rm3FileName, publishLicense, cryptoProvider);
            rootStorage = myFile.StorageInfo;
         
            storageInfo = rootStorage.CreateSubStorage("TestSubStorage");
            myData = storageInfo.CreateStream("Stream1", CompressionOption.NotCompressed, EncryptionOption.RightsManagement);
            dataStream  = myData.GetStream();
            writer     = new StreamWriter(dataStream); // Frameworks class
            writer.WriteLine("Hello World!");
            writer.Close();
            myFile.Close();

            // Create DRT test file - Compression - No, Encryption -None
            myFile = EncryptedPackageEnvelope.Create(rm4FileName, publishLicense, cryptoProvider);
            rootStorage = myFile.StorageInfo;
         
            storageInfo = rootStorage.CreateSubStorage("TestSubStorage");
            myData = storageInfo.CreateStream("Stream1", CompressionOption.NotCompressed, EncryptionOption.None);
            dataStream  = myData.GetStream();
            writer     = new StreamWriter(dataStream); // Frameworks class
            writer.WriteLine("Hello World!");
            writer.Close();
            myFile.Close();
            
            //delete this file from these tests
            testFileInfo = new FileInfo(rm1FileName);
            if( testFileInfo.Exists )
            {
                Log.WriteLine("Deleting test file from past test run");
                testFileInfo.Delete();
            }
            else
                retVal = 1;

            //delete this file from these tests
            testFileInfo = new FileInfo(rm2FileName);
            if( testFileInfo.Exists )
            {
                Log.WriteLine("Deleting test file from past test run");
                testFileInfo.Delete();
            }
            else
                retVal = 1;

            //delete this file from these tests
            testFileInfo = new FileInfo(rm3FileName);
            if( testFileInfo.Exists )
            {
                Log.WriteLine("Deleting test file from past test run");
                testFileInfo.Delete();
            }
            else
                retVal = 1;

            //delete this file from these tests
            testFileInfo = new FileInfo(rm4FileName);
            if( testFileInfo.Exists )
            {
                Log.WriteLine("Deleting test file from past test run");
                testFileInfo.Delete();
            }
            else
                retVal = 1;
        }

        return retVal;
    }
    
    static int StorageMethods()
    {
        // Test storages
        string storageName1  = "Acura";
        string storageName1a = "RSX";

        Log.WriteLine("====<Initiating StorageMethods test>====");

        // Create a structure in a test file
        FileInfo testFileInfo = new FileInfo(testFileName);
        if( testFileInfo.Exists )
        {
            Log.WriteLine("Deleting test file from past test run");
            testFileInfo.Delete();
        }

        StorageInfo acuraStorage = null;
        StorageInfo workingStorage = null;

        EncryptedPackageEnvelope myFile = null;
        StorageRootWrapper  rootWrapper = null;

        if (_rmClientSetup)
        {
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            myFile = EncryptedPackageEnvelope.Create(testFileName, publishLicense, cryptoProvider);
            StorageInfo rootStorage = myFile.StorageInfo;

            // Start building things up
            // "\Acura"
            acuraStorage = rootStorage.CreateSubStorage(storageName1);

            // "\Acura\RSX"
            workingStorage = acuraStorage.CreateSubStorage( storageName1a );
        }
        else
        {
            rootWrapper = StorageRootWrapper.Open(testFileName, FileMode.CreateNew);

            // Start building things up
            // "\Acura"
            acuraStorage = rootWrapper.CreateSubStorage(storageName1);

            // "\Acura\RSX"
            workingStorage = acuraStorage.CreateSubStorage( storageName1a );
        }

        StorageInfo[] storages = acuraStorage.GetSubStorages();
        foreach( object o in storages)
        {
            if( o == null )
            {
                Log.WriteLine("Object retrieved from enumerator is null!");
                return 1;
            }
            
            if( ((StorageInfo)o).Name != storageName1a )
//                ((StorageInfo)o).Name != storageName1b )
            {
                Log.WriteLine("This is an unexpected storage name");
                return 1;
            }
        }

        StreamInfo[] streams = acuraStorage.GetStreams();
        foreach( object o in streams)
        {
            if( o == null )
            {
                Log.WriteLine("Object retrieved from enumerator is null!");
                return 1;
            }
            
            Log.WriteLine("Retrieved stream named {0}", ((StreamInfo)o).Name );
            Log.WriteLine("!!!!This is an unexpected event.");
            return 1;
        }

        acuraStorage.CreateSubStorage( "TL" );

        foreach( object o in acuraStorage.GetSubStorages() )
        {
            Log.WriteLine("{0}",((StorageInfo)o).Name);
        }

        if( 0 != acuraStorage.GetStreams().Length )
        {
            Log.WriteLine("Not expecting any streams in this enumerator");
            return 1;
        }

        // Close out the file
        if (_rmClientSetup)
            myFile.Close();
        else
            rootWrapper.Close();

        // Need to re-establish FileInfo class even though it points to the same file.
        testFileInfo = new FileInfo(testFileName);
        testFileInfo.Delete();

        // Everything passed, we rock.
        Log.WriteLine("====<StorageMethods test completed>====");
        return 0;
    }

    static int StreamPM()
    {
        Log.WriteLine("====<Initiating Stream property/method test routine.>====");

        // Create a structure in a test file
        FileInfo testFileInfo = new FileInfo(testFileName);
        if( testFileInfo.Exists )
        {
            Log.WriteLine("Deleting test file from past test run");
            testFileInfo.Delete();
        }
        
        string[]    testNames    = { "some", "random", "names" };
        string      streamData  = "StreamPM test";

        StorageInfo testStorage1 = null;
        StorageInfo testStorage2 = null;
        StreamInfo  testStream1  = null;


        EncryptedPackageEnvelope myFile = null;
        StorageRootWrapper  rootWrapper = null;

        if (_rmClientSetup)
        {
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            myFile = EncryptedPackageEnvelope.Create(testFileName, publishLicense, cryptoProvider);
            StorageInfo rootStorage = myFile.StorageInfo;

            testStorage1 = rootStorage.CreateSubStorage( testNames[0] );
        }
        else
        {
            rootWrapper = StorageRootWrapper.Open(testFileName, FileMode.CreateNew);
            testStorage1 = rootWrapper.CreateSubStorage( testNames[0] );
        }

        testStorage2 = testStorage1.CreateSubStorage( testNames[1] );
        testStream1  = testStorage2.CreateStream( testNames[2] );

        if( !testStorage2.StreamExists(testNames[2]))
        {
            Log.WriteLine("Created Stream does not exist.");
            return 1;
        }

        Stream testStream = testStream1.GetStream();

        StreamWriter writer     = new StreamWriter(testStream); // Frameworks class

        writer.WriteLine(streamData);
        writer.Close();

        // Compare the path with the name array it came from
        string namePath = testStream1.Name;

        System.Text.StringBuilder testNameBuilder = new System.Text.StringBuilder(testNames[0]);
        for( int i = 1; i < testNames.Length; i++ )
        {
            testNameBuilder.Append('\\');
            testNameBuilder.Append(testNames[i]);
        }
        string testPath = testNameBuilder.ToString();

        // Close down and re-open the container.
        if (_rmClientSetup)
            myFile.Close();
        else
            rootWrapper.Close();

        if (_rmClientSetup)
        {
            myFile = EncryptedPackageEnvelope.Open(testFileName);
            StorageInfo rootStorage  = myFile.StorageInfo;
            testStorage1 = rootStorage.GetSubStorageInfo( testNames[0] );
        }
        else
        {
            rootWrapper = StorageRootWrapper.Open(testFileName, FileMode.Open);
            testStorage1 = rootWrapper.GetSubStorageInfo( testNames[0] );
        }

        testStorage2 = testStorage1.GetSubStorageInfo( testNames[1] );
        testStream1  =  testStorage2.GetStreamInfo( testNames[2] );

        // Check the stream name again
        namePath  = testStream1.Name;

        if( !testStorage2.StreamExists( testNames[2] ) )
        {
            Log.WriteLine("Stream that should be there thinks it's not");
            return 1;
        }

        testStream = testStream1.GetStream( FileMode.Open, FileAccess.Read );

        // Try reading the stream
        StreamReader reader = new StreamReader(testStream);
        if( streamData != reader.ReadLine() )
        {
            Log.WriteLine("Read #1 failed");
            return 1;
        }

        StreamReader reader2 = new StreamReader(testStream1.GetStream()); // Clone
        if( streamData != reader2.ReadLine() )
        {
            Log.WriteLine("Read #2 failed");
            return 1;
        }

        reader.Close();
        reader2.Close();
        // Abort everything - things should not choke.

        // Close out the file
        if (_rmClientSetup)
            myFile.Close();
        else
            rootWrapper.Close();

        testFileInfo = new FileInfo(testFileName);
        testFileInfo.Delete();

        Log.WriteLine("====<Stream property/method test routine complete.>====");

        return 0;
    }

    static int RegressionTests()
    {
        int retVal = 0;

        Log.WriteLine("====<RegressionTests: Verify that fixed bugs stay fixed>====");

        retVal += RegressionTest1();
        retVal += RegressionTest2();
        retVal += RegressionTest3();
        retVal += RegressionTest4();
        retVal += RegressionTest5();
        retVal += RegressionTest7();
        retVal += RegressionTest8();
        retVal += RegressionTest9(); 
        retVal += RegressionTest10();
        retVal += RegressionTest11();
        retVal += RegressionTest12();
        retVal += RegressionTest13();
        retVal += RegressionTest14();

        // PS 
        retVal += RegressionTest15();
        
            
        if( 0 == retVal )
            Log.WriteLine("====<RegressionTests complete>====");
        else
            Log.WriteLine("====<One or more regression tests failed>====");

        return retVal;
    }

    static int RegressionTest1()
    {
        Log.WriteLine("BUG: Container opened ReadOnly, still allows stream creation");

        StreamInfo  stInfo = null;
        int         retVal = 1;

        EncryptedPackageEnvelope stRoot = null;
        StorageRootWrapper  rootWrapper = null;

        if (_rmClientSetup)
        {    
            // create empty container 
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            stRoot = EncryptedPackageEnvelope.Create(testFileName, publishLicense, cryptoProvider);
            stRoot.Close();

            stRoot = EncryptedPackageEnvelope.Open(testFileName, FileAccess.Read);
            StorageInfo storageInfo = stRoot.StorageInfo;

            try
            {
                stInfo = storageInfo.CreateStream( "Test2" );
            }
            catch ( IOException e)
            {
                if( "Cannot create a stream in a read-only package." == e.Message )
                {
                    retVal = 0; // We expect this to happen
                }
                else
                {
                    Log.WriteLine("    Expected one error, got another instead: {0}", e.Message);
                }
            }

        }
        else
        {  
            // create empty container 
            rootWrapper = StorageRootWrapper.Open(testFileName, FileMode.CreateNew);
            rootWrapper.Close();
            
            rootWrapper = StorageRootWrapper.Open(testFileName, FileMode.Open, FileAccess.Read);
            try
            {
                stInfo  = rootWrapper.CreateStream( "Test2" );
                Log.WriteLine("    Expected Exception");
            }
            catch (TargetInvocationException)
            {
                // IOException is expected here. Since we are using Reflection this is expected.
                retVal = 0; // We expect this to happen
            }
        }

        if (_rmClientSetup)
            stRoot.Close();
        else
            rootWrapper.Close();

        return retVal;
    }

    static int RegressionTest2()
    {
        Log.WriteLine("Container StorageRoot.Open throws COMException when file is not present");

        int retVal = 1;

       FileInfo  testFileInfo = new FileInfo(testFileName);
        testFileInfo.Delete();

        
        FileInfo fInfo = new FileInfo( testFileName );
        if ( fInfo.Exists )
        {	// remove the container file
            fInfo.Delete();
        }

        if (_rmClientSetup)
        {
            try
            {
                Log.WriteLine("    Expected IOException");
                EncryptedPackageEnvelope myFile = EncryptedPackageEnvelope.Open(testFileName, FileAccess.Read);
            }
            catch( IOException )
            {
                // IOException is expected here.
                retVal = 0;
            }
        }
        else
        {
            try
            {
                Log.WriteLine("    Expected Exception");
                StorageRootWrapper rootWrapper = StorageRootWrapper.Open(testFileName, FileMode.Open, FileAccess.Read);
            }
            catch( TargetInvocationException )
            {
                // IOException is expected here. Since we are using Reflection this is expected.
                retVal = 0;
            }
        }

        return retVal;
    }

    static int RegressionTest3()
    {
        Log.WriteLine("Container StreamInfo.Open throws COMException");

        int retVal = 1;
        StreamInfo  stInfo = null;
        FileInfo	fInfo = null;

        fInfo = new FileInfo( testFileName );
        if ( fInfo.Exists )
        {	// remove the container file
            fInfo.Delete();
        }

        EncryptedPackageEnvelope stRoot = null;
        StorageRootWrapper  rootWrapper = null;

        if (_rmClientSetup)
        {    
            // create empty container 
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            stRoot = EncryptedPackageEnvelope.Create(testFileName, publishLicense, cryptoProvider);
            StorageInfo rootStorage = stRoot.StorageInfo;

            try
            {
                stInfo = rootStorage.GetStreamInfo( "NonExistent" );
            }
            catch(IOException)
            {
                retVal = 0;
            }
        }
        else
        {
            rootWrapper = StorageRootWrapper.Open(testFileName, FileMode.CreateNew);
            try
            {
                StreamInfo streamInfo = rootWrapper.GetStreamInfo("NonExistent");
            }
            catch (TargetInvocationException)
            {
                // IOException is expected here. Since we are using Reflection this is expected.
                retVal = 0;
            }
        }
        
        if (_rmClientSetup)
            stRoot.Close();
        else
            rootWrapper.Close();

        return retVal;
    }

    static int RegressionTest4()
    {
        Log.WriteLine("Container StoregeInfo leaks unmanaged undelying resource");

        StreamInfo  stInfo = null;
        Stream		fStream = null;
        FileInfo	fInfo = null;

        fInfo = new FileInfo( testFileName );
        if ( fInfo.Exists )
        {	// remove the container file
            fInfo.Delete();
        }

        if (_rmClientSetup)
        {
            EncryptedPackageEnvelope stRoot = null;
            
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            stRoot = EncryptedPackageEnvelope.Create(testFileName, publishLicense, cryptoProvider);
            StorageInfo rootStorage = stRoot.StorageInfo;
            stInfo = rootStorage.CreateStream( "TestStream" );

            fStream = stInfo.GetStream();
            fStream.Close();

            stInfo = rootStorage.GetStreamInfo( "TestStream" );
            fStream = stInfo.GetStream(FileMode.OpenOrCreate);
            fStream.Close();

            stRoot.Close();
        }
        else
        {
            StorageRootWrapper  rootWrapper = null;
            rootWrapper = StorageRootWrapper.Open(testFileName, FileMode.CreateNew);
            stInfo = rootWrapper.CreateStream( "TestStream" );

            fStream = stInfo.GetStream();
            fStream.Close();

            stInfo = rootWrapper.GetStreamInfo( "TestStream" );
            fStream = stInfo.GetStream(FileMode.OpenOrCreate);
            fStream.Close();
            
            rootWrapper.Close();
        }

        return 0;
    }

    static int RegressionTest5()
    {
        Log.WriteLine("Container: Program is able to write in a Stream opened for Read (only)");

        int retVal = 1;
        StreamInfo  stInfo = null;
        Stream		fStream = null;
        StreamWriter wrt = null;
        FileInfo	fInfo = null;

        fInfo = new FileInfo( testFileName );
        if ( fInfo.Exists )
        {	// remove the container file
            fInfo.Delete();
        }

        EncryptedPackageEnvelope stRoot = null;
        StorageRootWrapper  rootWrapper = null;

        if (_rmClientSetup)
        {
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            stRoot = EncryptedPackageEnvelope.Create(testFileName, publishLicense, cryptoProvider);
            StorageInfo rootStorage = stRoot.StorageInfo;
            stInfo = rootStorage.CreateStream( "TestStream" );
            fStream = stInfo.GetStream();
            fStream.Close();
            stRoot.Close();

            stRoot = EncryptedPackageEnvelope.Open(testFileName);
            rootStorage = stRoot.StorageInfo;
            stInfo = rootStorage.GetStreamInfo( "TestStream" );
            fStream = stInfo.GetStream(FileMode.Open, FileAccess.Read);
        }
        else
        {
            rootWrapper = StorageRootWrapper.Open(testFileName, FileMode.CreateNew);
            stInfo = rootWrapper.CreateStream( "TestStream" );
            fStream = stInfo.GetStream();
            fStream.Close();
            rootWrapper.Close();

            rootWrapper = StorageRootWrapper.Open( testFileName );
            stInfo = rootWrapper.GetStreamInfo( "TestStream" );
            fStream = stInfo.GetStream(FileMode.Open, FileAccess.Read);
        }
        
        try
        {
            // this should fail
            wrt = new StreamWriter( fStream );
            wrt.WriteLine( "This is a test" );
            wrt.Close();
            Log.WriteLine("    Stream write expected to fail but did not.");
        }
        catch ( ArgumentException )
        {
            retVal = 0;
        }

        fStream.Close();

        if (_rmClientSetup)
            stRoot.Close();
        else
            rootWrapper.Close();

        return retVal;
    }

    static int RegressionTest6()
    {
        Log.WriteLine("Container: Program is able to read from stream opened for Write");

        int retVal = 1;
        StreamInfo  stInfo = null;
        Stream		fStream = null;
        StreamWriter wrt = null;
        StreamReader rdr = null;
        string line;


        EncryptedPackageEnvelope stRoot = null;
        StorageRootWrapper  rootWrapper = null;

        if (_rmClientSetup)
        {
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            stRoot = EncryptedPackageEnvelope.Create(testFileName, publishLicense, cryptoProvider);
            StorageInfo rootStorage = stRoot.StorageInfo;
            stInfo = rootStorage.CreateStream( "TestStream" );
            fStream = stInfo.GetStream();
            wrt = new StreamWriter( fStream );
            wrt.WriteLine( "This is a test" );
            wrt.Close();
            fStream.Close();
            stRoot.Close();

            stRoot = EncryptedPackageEnvelope.Open(testFileName);
            rootStorage = stRoot.StorageInfo;
            stInfo = rootStorage.GetStreamInfo( "TestStream" );
            fStream = stInfo.GetStream(FileMode.Open, FileAccess.Write);
        }
        else
        {
            rootWrapper = StorageRootWrapper.Open(testFileName, FileMode.CreateNew);
            stInfo = rootWrapper.CreateStream( "TestStream" );
            fStream = stInfo.GetStream();
            wrt = new StreamWriter( fStream );
            wrt.WriteLine( "This is a test" );
            wrt.Close();
            fStream.Close();
            rootWrapper.Close();

            rootWrapper = StorageRootWrapper.Open( testFileName );
            stInfo = rootWrapper.GetStreamInfo( "TestStream" );
            fStream = stInfo.GetStream(FileMode.Open, FileAccess.Write);
        }

        try
        {
            rdr = new StreamReader( fStream );
            line = rdr.ReadLine();
            rdr.Close();
            Log.WriteLine("    Stream read expected to fail but did not");
        }
        catch( ArgumentException )
        {
            retVal = 0;
        }

        fStream.Close();

        if (_rmClientSetup)
            stRoot.Close();
        else
            rootWrapper.Close();

        return retVal;
    }

    static int RegressionTest7()
    {
        Log.WriteLine("Container: StorageInfo.Exists throws COM exception Access Denied");

        int retVal = 1;
        StorageInfo strInfo = null;
        FileInfo	fInfo = null;

        fInfo = new FileInfo( testFileName );
        if ( fInfo.Exists )
            fInfo.Delete();

        if (_rmClientSetup)
        {
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            EncryptedPackageEnvelope stRoot = EncryptedPackageEnvelope.Create(testFileName, publishLicense, cryptoProvider);
            StorageInfo rootStorage = stRoot.StorageInfo;

            strInfo = rootStorage.CreateSubStorage( "TestStorage" );

            if ( !rootStorage.SubStorageExists("TestStorage"))
            {
                Log.WriteLine( "    Storage is missing." );
                retVal = 1;
            }
            else
            {
                retVal = 0;
            }
            stRoot.Close();
        }
        else
        {
            StorageRootWrapper rootWrapper = StorageRootWrapper.Open(testFileName, FileMode.CreateNew);

            strInfo = rootWrapper.CreateSubStorage( "TestStorage" );

            if ( !rootWrapper.SubStorageExists("TestStorage"))
            {
                Log.WriteLine( "    Storage is missing." );
                retVal = 1;
            }
            else
            {
                retVal = 0;
            }
            rootWrapper.Close();
        }

        return retVal;
    }

    static int RegressionTest8()
    {
        Log.WriteLine("Container: StorageInfo:CreateSubStorage does not create storage");

        int retVal = 1;
        StorageInfo strInfo = null;
        FileInfo	fInfo = null;

        fInfo = new FileInfo( testFileName );
        if ( fInfo.Exists )
            fInfo.Delete();

        if (_rmClientSetup)
        {
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            EncryptedPackageEnvelope stRoot = EncryptedPackageEnvelope.Create(testFileName, publishLicense, cryptoProvider);
            StorageInfo rootStorage = stRoot.StorageInfo;
            strInfo = rootStorage.CreateSubStorage("TestStorage");

            strInfo = null;
            stRoot.Close();
            stRoot = EncryptedPackageEnvelope.Open(testFileName);
            rootStorage = stRoot.StorageInfo;
            strInfo = rootStorage.GetSubStorageInfo( "TestStorage" );
            if ( rootStorage.SubStorageExists("TestStorage") )
            {
                retVal = 0;
            }
            else
            {
                Log.WriteLine( "    Storage missing, should have been created with CreateSubStorage" );
                retVal = 1;
            }
            stRoot.Close();
        }
        else
        {
            StorageRootWrapper rootWrapper = StorageRootWrapper.Open(testFileName,FileMode.CreateNew);
            strInfo = rootWrapper.CreateSubStorage("TestStorage");

            strInfo = null;
            rootWrapper.Close();

            rootWrapper = StorageRootWrapper.Open( testFileName );
            strInfo = rootWrapper.GetSubStorageInfo( "TestStorage" );
            if ( rootWrapper.SubStorageExists("TestStorage") )
            {
                retVal = 0;
            }
            else
            {
                Log.WriteLine( "    Storage missing, should have been created with CreateSubStorage" );
                retVal = 1;
            }
            rootWrapper.Close();
        }

        return retVal;
    }

    static int RegressionTest9()
    {
        Log.WriteLine("Container: StorageRoot.Close does not close the file after using StorageRoot.Exists");

        int retVal = 1;
        StreamInfo  stInfo = null;
        Stream      fStream  = null;
        FileInfo    fInfo = null;

        // Cleanup container file if it exists
        fInfo = new FileInfo( testFileName );
        if ( fInfo.Exists )
            fInfo.Delete();

        if (_rmClientSetup)
        {
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            EncryptedPackageEnvelope stRoot = EncryptedPackageEnvelope.Create(testFileName, publishLicense, cryptoProvider);
            StorageInfo rootStorage = stRoot.StorageInfo;
            stInfo = rootStorage.CreateStream( "Test1" );
            fStream = stInfo.GetStream();
            fStream.Close();
            stRoot.Close();

            stRoot = EncryptedPackageEnvelope.Open(testFileName);
            rootStorage = stRoot.StorageInfo;
            stInfo = rootStorage.GetStreamInfo( "Test1" );
            if ( rootStorage.StreamExists( "Test1" ) )
            {
                retVal = 0;
            }
            else
            {
                Log.WriteLine("Expected stream doesn't seem to exist");
            }
            stRoot.Close();
        }
        else
        {
            StorageRootWrapper rootWrapper = StorageRootWrapper.Open(testFileName,FileMode.CreateNew);
            stInfo = rootWrapper.CreateStream( "Test1" );
            fStream = stInfo.GetStream();
            fStream.Close();
            rootWrapper.Close();
			
            rootWrapper = StorageRootWrapper.Open( testFileName );
            stInfo = rootWrapper.GetStreamInfo( "Test1" );
            if ( rootWrapper.StreamExists( "Test1" ) )
            {
                retVal = 0;
            }
            else
            {
                Log.WriteLine("Expected stream doesn't seem to exist");
            }
            rootWrapper.Close();
        }
        
        fInfo = new FileInfo( testFileName );
        fInfo.Delete();

        return retVal;
    }

    static int RegressionTest10()
    {
        Log.WriteLine("StorageInfo.Exist returns true after StorageInfo object has been deleted.");

        int retVal = 1;

        FileInfo	fInfo = null;

        fInfo = new FileInfo("test1.xmf" );

        if ( fInfo.Exists )
        {	
        	fInfo.Delete();
        }

        if (_rmClientSetup)
        {
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            EncryptedPackageEnvelope stRoot = EncryptedPackageEnvelope.Create("test1.xmf", publishLicense, cryptoProvider);
            StorageInfo rootStorage = stRoot.StorageInfo;
        
            rootStorage.CreateSubStorage("Storage"); //S is upper case	
            rootStorage.DeleteSubStorage("Storage");  //this should delete StorageInfo
            if (! rootStorage.SubStorageExists("Storage"))
                    retVal = 0;
        
            stRoot.Close();
        }
        else
        {
            StorageRootWrapper rootWrapper = StorageRootWrapper.Open("test1.xmf", FileMode.CreateNew);
        
            rootWrapper.CreateSubStorage("Storage"); //S is upper case	
            rootWrapper.DeleteSubStorage("Storage");  //this should delete StorageInfo
            if (! rootWrapper.SubStorageExists("Storage"))
                    retVal = 0;
        
            rootWrapper.Close();
        }

        return retVal;        
    }

    static int RegressionTest11()
    {
        Log.WriteLine("Can't delete StorageRoot, StorageInfo and StreamInfo objects when Stream is in a substorage.");

        int retVal = 1;

        //Creating StorageRoot, StorageInfo and StreamInfo.

        FileInfo	fInfo = null;

        fInfo = new FileInfo("test1.xmf" );

        if ( fInfo.Exists )
        {	
        	fInfo.Delete();
        }

        if (_rmClientSetup)
        {
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            EncryptedPackageEnvelope stRoot = EncryptedPackageEnvelope.Create("test1.xmf", publishLicense, cryptoProvider);
            StorageInfo rootStorage = stRoot.StorageInfo;

            StorageInfo StoInfo = rootStorage.CreateSubStorage("Storage");    
            StreamInfo Stream = StoInfo.CreateStream("stream");
            Stream x = Stream.GetStream();
            x.Close();

            stRoot.Close();

            //Open and try to delete them.
            EncryptedPackageEnvelope sroot = EncryptedPackageEnvelope.Open("test1.xmf", FileAccess.ReadWrite);
            rootStorage = sroot.StorageInfo;
            rootStorage.DeleteSubStorage("Storage"); //Exception "Unable to delete"
            try
            {
                StoInfo = rootStorage.GetSubStorageInfo("Storage"); //Exception "Unable to delete"
                StoInfo.DeleteStream("stream");
            }
            catch( IOException )
            {
                retVal = 0; //expected condition 
            }
        }
        else
        {
            StorageRootWrapper rootWrapper = StorageRootWrapper.Open("test1.xmf",FileMode.CreateNew);

            StorageInfo StoInfo = rootWrapper.CreateSubStorage("Storage");    
            StreamInfo Stream = StoInfo.CreateStream("stream");
            Stream x = Stream.GetStream();
            x.Close();

            rootWrapper.Close();

            //Open and try to delete them.
            StorageRootWrapper sroot = StorageRootWrapper.Open("test1.xmf",FileMode.Open, FileAccess.ReadWrite);
            sroot.DeleteSubStorage("Storage"); //Exception "Unable to delete"
            try
            {
                StoInfo = sroot.GetSubStorageInfo("Storage"); //Exception "Unable to delete"
                StoInfo.DeleteStream("stream");
            }
            catch( TargetInvocationException )
            {
                retVal = 0; //expected condition
            }
        }

        return retVal;
    }

    static int RegressionTest12()
    {
        Log.WriteLine("Unable to specify non-zero offset to Stream.Read()");

        int retVal = 1;

        byte[] testData = { 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16 };
        byte[] verifyData={ 0,0,0,0,0,0,0,9,10,11,12,13,14,15,16,0 };
        byte[] readData = new byte[testData.Length];

        Stream testStream = null;

        FileInfo	fInfo = null;
        fInfo = new FileInfo("offsetcontainer.xmf" );
        if ( fInfo.Exists )
        {	
        	fInfo.Delete();
        }

        EncryptedPackageEnvelope stRoot = null;
        StorageRootWrapper rootWrapper = null;

        if (_rmClientSetup)
        {
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            stRoot = EncryptedPackageEnvelope.Create("offsetcontainer.xmf", publishLicense, cryptoProvider);
            StorageInfo rootStorage = stRoot.StorageInfo;
        
            testStream = (rootStorage.CreateStream( "testStream")).GetStream();        
        }
        else
        {
            rootWrapper = StorageRootWrapper.Open("offsetcontainer.xmf", FileMode.CreateNew);
            testStream = (rootWrapper.CreateStream( "testStream")).GetStream();        
        }

        testStream.Write( testData, 8, 8 );
        testStream.Seek( 0, SeekOrigin.Begin );
#pragma warning disable CA2022 // Avoid inexact read
        testStream.Read( readData, 7, 8 );
#pragma warning restore CA2022

        retVal = 0; // Innocent until proved guilty
        for( int i = 0; i < testData.Length; i++ )
        {
            if( readData[i] != verifyData[i] )
                retVal = 1;
        }

        if( 1 == retVal )
            Log.WriteLine("    Non-zero offset array read/write did not result as expected");

        if (_rmClientSetup)
            stRoot.Close();
        else
            rootWrapper.Close();

        return retVal;        
    }
    
    static int RegressionTest13()
    {
        Log.WriteLine("Container: STG_E_INVALIDFLAG on StorageRoot.Open should be special cased.");

        int retVal = 1;

        if (_rmClientSetup)
        {
            try
            {
                // Not supported in OLE32.DLL DocFile implementation
                EncryptedPackageEnvelope stRoot = EncryptedPackageEnvelope.Open("InvalidFlags.container", 
                    FileAccess.Read, 
                    FileShare.ReadWrite);
                stRoot.Close();
            }
            catch ( ArgumentException )
            {
                retVal = 0; // Expected condition
            }
        }
        else
        {
            try
            {
                // Not supported in OLE32.DLL DocFile implementation
                StorageRootWrapper rootWrapper = StorageRootWrapper.Open( "InvalidFlags.container", 
                    FileMode.Open,
                    FileAccess.Read, 
                    FileShare.ReadWrite);
                rootWrapper.Close();
            }
            catch ( TargetInvocationException )
            {
                retVal = 0; // Expected condition
            }
        }

        if( retVal == 1 )
            Log.WriteLine("    STG_E_INVALIDFLAG did not get translated into ArgumentException as expected");
        
        return retVal;
    }

    static int RegressionTest14()
    {
        Log.WriteLine("Container: Exception thrown if Stream.Close() is called after StorageRoot.Close()");

        int retVal = 1;

        Stream testStream = null;

        FileInfo	fInfo = null;
        fInfo = new FileInfo("test.container" );
        if ( fInfo.Exists )
        {	
        	fInfo.Delete();
        }

        if (_rmClientSetup)
        {
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            EncryptedPackageEnvelope stRoot = EncryptedPackageEnvelope.Create("test.container", publishLicense, cryptoProvider);
            StorageInfo rootStorage = stRoot.StorageInfo;
        
            StreamInfo stInfo = rootStorage.CreateStream( "fooStream" );
            testStream = stInfo.GetStream();

            stRoot.Close();
        }
        else
        {
            StorageRootWrapper rootWrapper = StorageRootWrapper.Open( "test.container", FileMode.CreateNew);
            StreamInfo stInfo = rootWrapper.CreateStream( "fooStream" );
            testStream = stInfo.GetStream();

            rootWrapper.Close();
        }

        try
        {
            testStream.Close();
            retVal = 0;
        }
        catch ( NullReferenceException )
        {
            Log.WriteLine("    Exception was thrown when test stream was explicitly closed after container close");
        }

        return retVal;
    }

    static int RegressionTest15()
    {

        int result = 0;

        // Note the following does not test the defunct metadata API, but a container 
        Log.WriteLine("Calling PropertySetDictionary.SaveToPart after PropertySetDictionary.LoadFromPart  with the same part, fails");

        StreamInfo si = null;
        
        FileInfo	fInfo = null;
        fInfo = new FileInfo("test.container" );
        if ( fInfo.Exists )
        {	
        	fInfo.Delete();
        }

        StorageRootWrapper rootWrapper = null;
        EncryptedPackageEnvelope sr = null;

        if (_rmClientSetup)
        {
            UseLicense authorUseLicense;
            PublishLicense publishLicense;
            CryptoProvider cryptoProvider;

            SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

            sr = EncryptedPackageEnvelope.Create("test.container", publishLicense, cryptoProvider);
            StorageInfo rootStorage = sr.StorageInfo;
        
            si = rootStorage.CreateStream( "teststream");
        }
        else
        {
            rootWrapper = StorageRootWrapper.Open("test.container", FileMode.CreateNew);
            si = rootWrapper.CreateStream( "teststream");
        }

        Stream s = si.GetStream();
        s.WriteByte ((byte)'a');
        s.Close ();

        if (_rmClientSetup)
            sr.Close ();
        else
            rootWrapper.Close();

        // sub test 1  open stream in Read mode and then in ReadWrite then Write and it should work 

        if (_rmClientSetup)
        {
            sr = EncryptedPackageEnvelope.Open("test.container", FileAccess.ReadWrite);
            StorageInfo rootStorage = sr.StorageInfo;
            si = rootStorage.GetStreamInfo( "teststream");
        }
        else
        {
            rootWrapper = StorageRootWrapper.Open ("test.container", FileMode.Open, FileAccess.ReadWrite);
            si = rootWrapper.GetStreamInfo( "teststream");
        }

        Stream s1= si.GetStream(FileMode.Open, FileAccess.Read);
        s1.Close();

        Stream s2= si.GetStream(FileMode.Open, FileAccess.ReadWrite);
        s2.WriteByte ((byte)'b');
        s2.Close ();

        if (_rmClientSetup)
            sr.Close ();
        else
            rootWrapper.Close();

        // sub test 2  open stream in ReadWrite and then in Read and then Write and expect to get an exception
        if (_rmClientSetup)
        {
            sr = EncryptedPackageEnvelope.Open("test.container", FileAccess.ReadWrite);
            StorageInfo rootStorage = sr.StorageInfo;
            si = rootStorage.GetStreamInfo( "teststream");
        }
        else
        {
            rootWrapper = StorageRootWrapper.Open ("test.container", FileMode.Open, FileAccess.ReadWrite);
            si = rootWrapper.GetStreamInfo( "teststream");
        }
        
        Stream s3= si.GetStream(FileMode.Open, FileAccess.ReadWrite);
        s3.Close();

        Stream s4= si.GetStream(FileMode.Open, FileAccess.Read);
        try
        {
            result ++;
            s4.WriteByte ((byte)'b');
        }
        catch (System.NotSupportedException)
        {
            result --;
        }
        s4.Close ();

        if (_rmClientSetup)
            sr.Close ();
        else
            rootWrapper.Close();

        // sub test 3  open stream in Read then write to it and expect to get an exception
        if (_rmClientSetup)
        {
            sr = EncryptedPackageEnvelope.Open("test.container", FileAccess.ReadWrite);
            StorageInfo rootStorage = sr.StorageInfo;
            si = rootStorage.GetStreamInfo( "teststream");
        }
        else
        {
            rootWrapper = StorageRootWrapper.Open ("test.container", FileMode.Open, FileAccess.ReadWrite);
            si = rootWrapper.GetStreamInfo( "teststream");
        }
        
        Stream s5= si.GetStream(FileMode.Open, FileAccess.Read);
        try
        {
            result ++;
            s5.WriteByte ((byte)'b');
        }
        catch (System.NotSupportedException)
        {
            result --;
        }
        s5.Close ();

        if (_rmClientSetup)
            sr.Close ();
        else
            rootWrapper.Close();

        return result;
    }

    internal static void
            SetUpPublishingInformation(
                out UseLicense authorUseLicense,
                out PublishLicense publishLicense,
                out CryptoProvider cryptoProvider
                )
    {
                UnsignedPublishLicense unsignedPublishLicense = new UnsignedPublishLicense();
                unsignedPublishLicense.Grants.Add(
                                                        new ContentGrant(
                                                            new ContentUser(
                                                                "somebody@somecompany.com",
                                                                AuthenticationType.Windows
                                                                ),
                                                            ContentRight.Owner
                                                            )
                                                        );

                publishLicense = unsignedPublishLicense.Sign(
                                                            _secureEnvironment,
                                                            out authorUseLicense
                                                            );

                //
                // Attempt to bind the use license to the secure environment.
                //
                cryptoProvider = authorUseLicense.Bind(_secureEnvironment);
    }

}

/// <summary>
/// The stream object that performs the actual work for AddXTransform
/// </summary>
internal class AddXStream : Stream
{
    byte delta;
    Stream baseStream;

    internal AddXStream( byte offset, Stream underlyingStream )
    {
        delta= offset;
        baseStream = underlyingStream;
    }

    // System.IO.Stream methods
    public override bool CanRead {get{return baseStream.CanRead;}}
    public override bool CanSeek {get{return baseStream.CanRead;}}
    public override bool CanWrite{get{return baseStream.CanRead;}}
    public override long Length  {get{return baseStream.Length;}}
    public override long Position{get{return baseStream.Position;} set{baseStream.Position = value;}}

    public override void Flush() { baseStream.Flush(); }
    public override long Seek( long offset, SeekOrigin origin ){ return baseStream.Seek( offset, origin );}
    public override void SetLength( long newLength ) {baseStream.SetLength( newLength );}

    public override int Read(
        byte[] buffer,
        int offset,
        int count)
    {
        int readCount = baseStream.Read( buffer, offset, count );
        int addMax = 0xFF - delta;
        
        for( int i = offset; i < readCount; i++ )
        {
            if( delta > buffer[i] )
            {
                buffer[i] = (byte)(buffer[i] + addMax);
            }
            else
            {
                buffer[i] -= delta;
            }
        }

        return readCount;
    }
    
    public override void Write(
        byte[] buffer,
        int offset,
        int count)
    {
        int addMax = 0xFF - delta;
        
        for( int i = offset; i < count; i++ )
        {
            if( addMax < buffer[i] )
            {
                buffer[i] = (byte)(buffer[i] - addMax);
            }
            else
            {
                buffer[i] += delta;
            }
        }
        baseStream.Write( buffer, offset, count );
    }

}

}
