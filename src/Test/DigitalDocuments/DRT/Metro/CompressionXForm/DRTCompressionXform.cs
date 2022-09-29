// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description:
//  The set of Developer Regression Tests for CF compression transform.
//
//

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Text;
using System.Security.RightsManagement; //RM

namespace DRT
{
    /// <summary>
    /// Compression DRT class. All tests live as methods within this class.
    /// </summary>
    public sealed class CompressionXformTestHarness : DrtBase
    {
        //This is used to switch between default mode and RM mode.
        //RM is special mode where users need to install windows client RM and set it up before running this DRT.

        [STAThread]
        public static int Main(string[] args)
        {
            DrtBase drt = new CompressionXformTestHarness();
            return drt.Run(args);
        }

        private CompressionXformTestHarness()
        {
            WindowTitle = CompressionXformTestSuite.Title;
            TeamContact = "WPF";
            Contact = "Microsoft";
            DrtName = CompressionXformTestSuite.Title;
            Suites = new DrtTestSuite[]
            {
                new CompressionXformTestSuite(),
                null            // list terminator - optional
            };
        }
    }

    public sealed class CompressionXformTestSuite : DrtTestSuite
    {
        #region Statics
        public static string Title = "DrtCompressionXform";
        static string s_fileName = Title + ".cf";
        static int s_upperLimit = 80000;      // default to limit DRT run-time

        static string s_bigFile = @"drtfiles\compressionXform\big.xaml";
        static string s_mediumFile = @"drtfiles\compressionXform\medium.xaml";
        static string s_uncompressibleFile = @"drtfiles\compressionXform\smiley.jpg";
        #endregion

        private bool _rmClientSetup = false;
        private bool _stress = false;

        // these are used for RM setup
        private static SecureEnvironment s_secureEnvironment;
        private static readonly string s_sc_appManifest = "<manifest></manifest>";

        /// <summary>
        /// Override this in derived classes to handle command-line arguments one-by-one.
        /// </summary>
        /// <param name="arg">current argument</param>
        /// <param name="option">if there was a leading "-" or "/" to arg</param>
        /// <param name="args">the array of command line arguments</param>
        /// <param name="k">current index in the argument array.  passed by ref so you can increase it to "consume" arguments to options.</param>
        /// <returns>True if handled</returns>
        public override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            if (option)
            {
                switch (arg)
                {
                    case "rmclient":
                        s_secureEnvironment = SecureEnvironment.Create
                                                    (s_sc_appManifest, AuthenticationType.Windows, UserActivationMode.Permanent);
                        break;

                    case "stress":
                        _stress = true;
                        break;

                    // we don't recognize so pass to base class for default handling
                    default:
                        return base.HandleCommandLineArgument(arg, option, args, ref k);
                }

                return true;
            }

            return false;
        }

        public CompressionXformTestSuite()
            : base(Title)
        {
            TeamContact = "Microsoft"; 
            Contact = "Microsoft";    
        }

        public override DrtTest[] PrepareTests()
        {
            // return the lists of tests to run against the tree
            return new DrtTest[]
            {
                new DrtTest( ExerciseTransformVersioning ),
                new DrtTest( ExerciseCompressionXform ),
                null        // list terminator - optional
            };
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
                                                            s_secureEnvironment,
                                                            out authorUseLicense
                                                            );
    
                //
                // Attempt to bind the use license to the secure environment.
                //
                cryptoProvider = authorUseLicense.Bind(s_secureEnvironment);
        }

        void Create(string streamName, string sourceFile, int dataSize, bool defineDataSpaces)
        {
//            String transformName = "compression." + streamName;

            if (_rmClientSetup)
            {
                UseLicense authorUseLicense;
                PublishLicense publishLicense;
                CryptoProvider cryptoProvider;

                SetUpPublishingInformation(out authorUseLicense,out publishLicense,out cryptoProvider);

                EncryptedPackageEnvelope myFile = EncryptedPackageEnvelope.Create(s_fileName, publishLicense, cryptoProvider);
                StorageInfo rootStorage = myFile.StorageInfo;
                if (defineDataSpaces)
                {
                    // Create a stream in one data space 

                    StreamInfo testSI = null;
                    if (rootStorage.StreamExists(streamName))
                        testSI = rootStorage.GetStreamInfo(streamName);
                    else
                    {
                        //second rootStorage: true is for normal compression (false is no compression)
                        testSI = rootStorage.CreateStream(streamName,CompressionOption.Normal, EncryptionOption.None);
                        DRT.Assert(testSI.CompressionOption == CompressionOption.Normal &&
                            testSI.EncryptionOption == EncryptionOption.None, "CompressionOption/EncryptionOption failure - getting compression and encryption options failed");
                    }
                
                    Stream testStream = testSI.GetStream();
                    FileToStream(sourceFile, testStream, dataSize);
                    testStream.Flush();
                }
                else
                {
                    StreamInfo testSI = null;
                    if (rootStorage.StreamExists(streamName))
                        testSI = rootStorage.GetStreamInfo(streamName);
                    else
                        testSI = rootStorage.CreateStream(streamName);
                    Stream testStream = testSI.GetStream();
                    FileToStream(sourceFile, testStream, dataSize);
                    testStream.Flush();
                }
                myFile.Close();
            }
            else
            {
                StorageRootWrapper myFile = StorageRootWrapper.Open(s_fileName, FileMode.OpenOrCreate);
                
                if (defineDataSpaces)
                {
                    // Create a stream in one data space 

                    StreamInfo testSI = null;
                    if (myFile.StreamExists(streamName))
                        testSI = myFile.GetStreamInfo(streamName);
                    else
                    {
                        //second argument: true is for normal compression (false is no compression)
                        testSI = myFile.CreateStream(streamName,CompressionOption.Normal, EncryptionOption.None);
                        DRT.Assert(testSI.CompressionOption == CompressionOption.Normal &&
                            testSI.EncryptionOption == EncryptionOption.None, "CompressionOption/EncryptionOption failure - getting compression and encryption options failed");
                    }

                    Stream testStream = testSI.GetStream();
                    FileToStream(sourceFile, testStream, dataSize);
                    testStream.Flush();
                }
                else
                {
                    StreamInfo testSI = null;
                    if (myFile.StreamExists(streamName))
                        testSI = myFile.GetStreamInfo(streamName);
                    else
                        testSI = myFile.CreateStream(streamName);
                    Stream testStream = testSI.GetStream();
                    FileToStream(sourceFile, testStream, dataSize);
                    testStream.Flush();
                }
                myFile.Close();
            }
        }

        void Exercise(string streamName, string sourceFile, int dataSize, bool expectTransforms)
        {
            // Re-open
            if (_rmClientSetup)
            {
                EncryptedPackageEnvelope myFile = EncryptedPackageEnvelope.Open(s_fileName);
                StorageInfo rootStorage = myFile.StorageInfo;
                
                // Verify first test string
                StreamInfo si = null;
                if (rootStorage.StreamExists(streamName))
                {
                    si =  rootStorage.GetStreamInfo( streamName);
                    DRT.Assert(!expectTransforms || si.CompressionOption != CompressionOption.NotCompressed, "CompressionOption failure - getting compression option failed");
                }
                else
                    si =  rootStorage.CreateStream( streamName);
                Stream s = si.GetStream();

                // Check stream content for test stream 1
                CompareStreamAndFile(s, sourceFile, dataSize);
                s.Close();

                // Shut down
                myFile.Close();
            }
            else
            {
                StorageRootWrapper myFile = StorageRootWrapper.Open(s_fileName, FileMode.Open);

                // Verify first test string
                StreamInfo si = null;
                if (myFile.StreamExists(streamName))
                {
                    si =  myFile.GetStreamInfo( streamName);
                    DRT.Assert(!expectTransforms || si.CompressionOption != CompressionOption.NotCompressed, "CompressionOption failure - getting compression option failed");
                }
                else
                    si =  myFile.CreateStream( streamName);
                Stream s = si.GetStream();

                // Check stream content for test stream 1
                CompareStreamAndFile(s, sourceFile, dataSize);
                s.Close();

                // Shut down
                myFile.Close();
            }
        }

        void Modify(string sourceFile, string streamName, Stream shadowStream)
        {
            // Verify first test string
            StreamInfo si = null;

            EncryptedPackageEnvelope myFile = null;
            StorageRootWrapper rootWrapper= null;

            // Re-open
            if (_rmClientSetup)
            {
                myFile = EncryptedPackageEnvelope.Open(s_fileName, FileAccess.ReadWrite);
                StorageInfo rootStorage = myFile.StorageInfo;
                if (rootStorage.StreamExists(streamName))
                    si = rootStorage.GetStreamInfo( streamName);
                else
                    si = rootStorage.CreateStream( streamName);
            }
            else
            {
                rootWrapper = StorageRootWrapper.Open(s_fileName, FileMode.Open);
                if (rootWrapper.StreamExists(streamName))
                    si = rootWrapper.GetStreamInfo( streamName);
                else
                    si = rootWrapper.CreateStream( streamName);
            }
            
            Stream s = si.GetStream();
            shadowStream.Seek(0, SeekOrigin.Begin);

            // overwrite
            s.Seek(22, SeekOrigin.Begin);       shadowStream.Seek(22, SeekOrigin.Begin);
            s.WriteByte(0xFF);                  shadowStream.WriteByte(0xFF);
            s.Seek(-9000, SeekOrigin.End);      shadowStream.Seek(-9000, SeekOrigin.End);
            for (int i = 0; i < 100; i++)
            {
                s.WriteByte(0xFF);              shadowStream.WriteByte(0xFF);
            }
            s.Flush();                          shadowStream.Flush();

            // Check stream content for test stream 1
            s.Seek(0, SeekOrigin.Begin);        shadowStream.Seek(0, SeekOrigin.Begin);
            CompareStreams(s, shadowStream);

            // modify
            s.SetLength(99);                    shadowStream.SetLength(99);

            // StreamInfo stream does not update Position on truncating SetLength
            s.Position = s.Length;              shadowStream.Position = shadowStream.Length;

            s.WriteByte(0xFF);                  shadowStream.WriteByte(0xFF);

            // Shut down
            if (_rmClientSetup)
                myFile.Close();
            else
                rootWrapper.Close();
        }

        Stream Read(string sourceFile, string streamName)
        {
            MemoryStream shadowStream = new MemoryStream();

            StreamInfo si = null;

            // Re-open
            EncryptedPackageEnvelope myFile = null;
            StorageRootWrapper rootWrapper= null;

            if (_rmClientSetup)
            {
                myFile = EncryptedPackageEnvelope.Open(s_fileName, FileAccess.Read);
                StorageInfo rootStorage = myFile.StorageInfo;
                if (rootStorage.StreamExists(streamName))
                    si = rootStorage.GetStreamInfo( streamName);
                else
                    si = rootStorage.CreateStream( streamName);
            }
            else
            {
                rootWrapper = StorageRootWrapper.Open(s_fileName, FileMode.Open, FileAccess.Read);
                if (rootWrapper.StreamExists(streamName))
                    si = rootWrapper.GetStreamInfo( streamName);
                else
                    si = rootWrapper.CreateStream( streamName);
            }
            
            Stream s = si.GetStream();

            int readSize = 0x1000;
            BinaryReader r = new BinaryReader(s);
            byte[] buf = new Byte[readSize];

            while (true)
            {
                int read = r.Read(buf, 0, buf.Length);

                // stream exhausted
                if (read <= 0)
                    break;

                shadowStream.Write(buf, 0, read);
            }

            // Shut down
            if (_rmClientSetup)
                myFile.Close();
            else
                rootWrapper.Close();

            return shadowStream;
        }

        void ReadAndCompare(string sourceFile, string streamName)
        {
            // Verify first test string
            StreamInfo si = null;

            // Re-open
            EncryptedPackageEnvelope myFile = null;
            StorageRootWrapper rootWrapper= null;

            if (_rmClientSetup)
            {
                myFile = EncryptedPackageEnvelope.Open(s_fileName, FileAccess.Read);
                StorageInfo rootStorage = myFile.StorageInfo;
                DRT.Assert(rootStorage.StreamExists(streamName));
                si = rootStorage.GetStreamInfo( streamName);
            }
            else
            {
                rootWrapper = StorageRootWrapper.Open(s_fileName, FileMode.Open, FileAccess.Read);
                DRT.Assert(rootWrapper.StreamExists(streamName));
                si = rootWrapper.GetStreamInfo( streamName);
            }
            
            Stream s = si.GetStream();

            // Check stream content for test stream 1
            CompareStreamAndFile(s, sourceFile);
                //           s.Close();

            // Shut down
            if (_rmClientSetup)
                myFile.Close();
            else
                rootWrapper.Close();
        }

        void ExerciseCompressionXform()
        {
            Console.WriteLine("====<Exercise CompressionXform>====");

            // Create a structure in a test fileName
            FileInfo testFileInfo = new FileInfo(s_fileName);
            if (testFileInfo.Exists)
            {
                Console.WriteLine("Deleting test fileName from past test run");
                testFileInfo.Delete();
            }

            Console.WriteLine("----------------------------------");
            Console.WriteLine("Step 1: Exercise a sequence of data sizes");
            Console.WriteLine("----------------------------------");

            // support stress for non-drt scenario (takes too long)
            int multiplier;
            int iterations;
            int initial;
            if (_stress)
            {
                initial = 1;
                multiplier = 197;
                iterations = 15;
            }
            else
            {
                initial = 64;
                multiplier = 37;
                iterations = 1;
            }

            int i = 1;
            while (i < 11)
            {
                for (int j = iterations; j > 0; j--)
                {
                    int dataLimit = Math.Min(s_upperLimit, initial * multiplier * i * j);

                    Console.WriteLine("Test: {0} DataLimit: {1}", i, dataLimit);
                    Create(i.ToString(), s_bigFile, dataLimit, true);
                    Exercise(i.ToString(), s_bigFile, dataLimit, true);
                }
                i++;
            }

            Console.WriteLine("----------------------------------");
            Console.WriteLine("Step 3: Add non-compressible jpeg");
            Console.WriteLine("----------------------------------");
            String streamName = "uncompressibleFile";
            Create(streamName, s_uncompressibleFile, -1, true);
            Exercise(streamName, s_uncompressibleFile, -1, true);

            Console.WriteLine("----------------------------------");
            Console.WriteLine("Step 4: Open file in read-only mode");
            Console.WriteLine("----------------------------------");
            ReadAndCompare(s_uncompressibleFile, streamName);

            Console.WriteLine("----------------------------------");
            Console.WriteLine("Step 5: Modify transformed streams in-place");
            Console.WriteLine("----------------------------------");
            Stream shadowStream = Read(s_mediumFile, streamName);
            Modify(s_mediumFile, streamName, shadowStream);
            {
                // Verify with shadow
                StreamInfo si = null;

                 // Re-open
                EncryptedPackageEnvelope myFile = null;
                StorageRootWrapper rootWrapper= null;

                if (_rmClientSetup)
                {
                    myFile = EncryptedPackageEnvelope.Open(s_fileName, FileAccess.Read);
                    StorageInfo rootStorage = myFile.StorageInfo;
                    DRT.Assert(rootStorage.StreamExists(streamName));
                    si = rootStorage.GetStreamInfo( streamName);
                }
                else
                {
                    rootWrapper = StorageRootWrapper.Open(s_fileName, FileMode.Open, FileAccess.Read);
                    DRT.Assert(rootWrapper.StreamExists(streamName));
                    si = rootWrapper.GetStreamInfo( streamName);
                }   
            
                Stream s = si.GetStream();

                // Check stream content for test stream 1
                s.Seek(0, SeekOrigin.Begin); shadowStream.Seek(0, SeekOrigin.Begin);
                CompareStreams(s, shadowStream);

                // Shut down
                if (_rmClientSetup)
                    myFile.Close();
                else
                    rootWrapper.Close();
            }



            Console.WriteLine("====<Data space routines complete.>====");
        }

        private static string s_featureName = "My.Feature";

        private static DrtVersionPair s_v1 = new DrtVersionPair(1, 0);
        private static DrtVersionPair s_v2 = new DrtVersionPair(2, 0);
        private static DrtVersionPair s_v3 = new DrtVersionPair(3, 0);
        private static DrtVersionPair s_v4 = new DrtVersionPair(4, 0);

        // writer, reader, updater
        private static DrtFormatVersion s_fv1 = new DrtFormatVersion(s_featureName, s_v1, s_v1, s_v1);
        private static DrtFormatVersion s_fv2 = new DrtFormatVersion(s_featureName, s_v2, s_v2, s_v2);
        private static DrtFormatVersion s_fv3 = new DrtFormatVersion(s_featureName, s_v3, s_v3, s_v3);
        private static DrtFormatVersion s_fv4 = new DrtFormatVersion(s_featureName, s_v4, s_v4, s_v4);

        /// <summary>
        /// Ensure that versioning works as expected
        /// </summary>
        void ExerciseTransformVersioning()
        {
            // Invariant: ReaderVersion <= UpdaterVersion <= WriterVersion

            Console.WriteLine("\nVersionTesting begins:\n");
            // fv1 opening fv1 should be fine - important for v1
            ExerciseVersionCompatibility(s_fv1, s_fv1, false, false, false);

            // fv1 opening fv2 should puke - important for v2
            ExerciseVersionCompatibility(s_fv2, s_fv1, true, false, false);

            // fv2 opening fv1 should be fine - nice but not necessary
            ExerciseVersionCompatibility(s_fv1, s_fv2, false, false, false);

#if false   // These are only interesting for v2. No need to update test harness for these for v1.
            // writes _v2, can read _v1 and is updatable by _v2
            DrtFormatVersion fvMixed2 = new DrtFormatVersion(_featureName, _v2, _v1, _v2);

            // writes _v3, can read _v1 and is updatable by _v2
            DrtFormatVersion fvMixed3 = new DrtFormatVersion(_featureName, _v3, _v1, _v2);

            ExerciseVersionCompatibility(_fv1, fvMixed2, false, false, false);    // should succeed to open and manipulate v1
            ExerciseVersionCompatibility(_fv2, fvMixed2, false, false, false);    // should succeed to open and manipulate v2
            ExerciseVersionCompatibility(_fv3, fvMixed2, true, true, true);       // should fail to access v3
#endif
            Console.WriteLine("\nVersionTesting complete\n");

        }

        private void 
        ExerciseVersionCompatibility(DrtFormatVersion fileVersion, DrtFormatVersion codeVersion, bool expectVersionConflict, bool readFails, bool writeFails)
        {
            Stream idStream = CreateVersionHoldingStream(fileVersion, true);
            Stream idStreamReadOnly = CreateVersionHoldingStream(fileVersion, false);
            Stream readOnlyEmptyStream = new MemoryStream(new byte[0]{}, false);

            // This is the stream who's format is versioned.  For DrtCompressionXform it would be transformed.
            MemoryStream dataStream = new MemoryStream();

            // Scenario 0: Write data stream
            PossibleNotSupportedException(FileAccess.Write, codeVersion, idStream, dataStream, false || writeFails, expectVersionConflict);
            DRT.Assert(writeFails || idStream.Length > 0);                  // write should have triggered the version write

            // instance data is read-only and empty - should throw NotSupportedException because we cannot write to idStream
            PossibleNotSupportedException(FileAccess.Write, codeVersion, readOnlyEmptyStream, dataStream, true, expectVersionConflict);      // exception expected

            // read-only stream - should simply read and compare (but can throw if writer needs to be updated)
            // fails for v2 updating v1 scenario - no need to test because our code never has a writable data stream and read-only
            // instance-data stream.
 //           PossibleNotSupportedException(FileAccess.Write, codeVersion, idStreamReadOnly, dataStream, writeFails, expectVersionConflict);

            // Scenario 1: Read from data stream
            PossibleNotSupportedException(FileAccess.Read, codeVersion, idStream, dataStream, false || readFails, expectVersionConflict);      // no exception expected

            // instance data is read-only and empty - Conditionally throws FormatVersionException when no Version data found
            // because we have no way to distinguish new empty stream and a opened stream.
            PossibleNotSupportedException(FileAccess.Read, codeVersion, readOnlyEmptyStream, dataStream, false || readFails, dataStream.Length > 0);      // exception expected

            // read-only stream - should simply read and compare
            PossibleNotSupportedException(FileAccess.Read, codeVersion, idStreamReadOnly, dataStream, false || readFails, expectVersionConflict);   // no exception expected

            // Scenario 2: Read/Write to data stream
            PossibleNotSupportedException(FileAccess.ReadWrite, codeVersion, idStream, dataStream, false, expectVersionConflict);

            // instance data is read-only and empty - should throw
            PossibleNotSupportedException(FileAccess.ReadWrite, codeVersion, readOnlyEmptyStream, dataStream, true, true);

            // read-only stream - should simply read and compare
            PossibleNotSupportedException(FileAccess.ReadWrite, codeVersion, idStream, dataStream, false, expectVersionConflict);
        }

        private void PossibleNotSupportedException(FileAccess access, DrtFormatVersion fv, Stream idStream, Stream dataStream, 
            bool expectingException, bool expectVersionException)
        {
            bool exceptionCaught = false;
            bool formatExceptionCaught = false;
            try
            {
                switch (access)
                {
                    case FileAccess.Read:
                        ReadData(fv, idStream, dataStream); break;

                    case FileAccess.Write:
                        WriteData(fv, idStream, dataStream); break;

                    case FileAccess.ReadWrite:
                        ReadWriteData(fv, idStream, dataStream); break;
                }
            }
            catch (NotSupportedException nse)
            {
                exceptionCaught = true;
                Console.WriteLine("NotSupportedException caught: " + nse.Message);
            }
            catch (FileFormatException ffe)
            {
                formatExceptionCaught = true;
                Console.WriteLine("FileFormatException caught: " + ffe.Message);
            }

            // can't get two exceptions so favor one over the other
            DRT.Assert(exceptionCaught == expectingException || formatExceptionCaught, (exceptionCaught ? "Not " : "")
                + "Expecting exception but " + (exceptionCaught ? "one" : "none") + " thrown");

            DRT.Assert(formatExceptionCaught == expectVersionException || exceptionCaught, (formatExceptionCaught ? "Not " : "")
                    + "Expecting Version exception but " + (formatExceptionCaught ? "one" : "none") + " thrown");
        }

        /// <summary>
        /// Write Data
        /// </summary>
        /// <param name="fv">current code version</param>
        /// <param name="instanceDataStream">instance stream where version should live or be persisted</param>
        /// <param name="dataStream">data stream</param>
        private void WriteData(DrtFormatVersion fv, Stream instanceDataStream, Stream dataStream)
        {
            Stream versionedStream = new DrtVersionedStream(dataStream, new DrtVersionedStreamOwner(instanceDataStream, fv));

            // write to the data stream and see if the version info is updated/compared
            versionedStream.WriteByte(0);
        }

        /// <summary>
        /// Read Data
        /// </summary>
        /// <param name="fv">current code version</param>
        /// <param name="instanceDataStream">instance stream where version should live or be persisted</param>
        /// <param name="dataStream">data stream</param>
        private void ReadData(DrtFormatVersion fv, Stream instanceDataStream, Stream dataStream)
        {
            Stream versionedStream = new DrtVersionedStream(dataStream, new DrtVersionedStreamOwner(instanceDataStream, fv));

            // read from the data stream and see if the version info is updated/compared
            versionedStream.ReadByte();
        }

        /// <summary>
        /// Read/Write Data
        /// </summary>
        /// <param name="fv">current code version</param>
        /// <param name="instanceDataStream">instance stream where version should live or be persisted</param>
        /// <param name="dataStream">data stream</param>
        private void ReadWriteData(DrtFormatVersion fv, Stream instanceDataStream, Stream dataStream)
        {
            Stream versionedStream = new DrtVersionedStream(dataStream, new DrtVersionedStreamOwner(instanceDataStream, fv));

            // trigger a version compare
            versionedStream.ReadByte();
            versionedStream.WriteByte(0);
        }

        private Stream CreateVersionHoldingStream(DrtFormatVersion version, bool writable)
        {
            MemoryStream temp = new MemoryStream();
            DrtVersionedStreamOwner tempUpdater = new DrtVersionedStreamOwner(temp, version);
            tempUpdater.WriteAttempt(); // force FormatVersion persistence //PersistVersion();
            return new MemoryStream(temp.GetBuffer(), 0, (int)temp.Length, writable);
        }

#region Utility
        void CompareStreams(Stream s1, Stream s2)
        {
            int blockNumber = 0;
            int readSize = 100;
            BinaryReader r1 = new BinaryReader(s1);
            BinaryReader r2 = new BinaryReader(s2);
            byte[] buf1 = new Byte[readSize];
            byte[] buf2 = new Byte[readSize];

            while (true)
            {
                int read1 = r1.Read(buf1, 0, buf1.Length);
                int read2 = r2.Read(buf2, 0, buf2.Length);
                DRT.Assert(read1 == read2,"Decode failure - read count is different: " + read1.ToString() + " vs " + read2.ToString());

                int i = 0;
                while (i < read1 && i < read2)
                {
                    DRT.Assert(buf1[i] == buf2[i], "Decode failure in block: " + blockNumber.ToString() + " position: " + i.ToString());
                    ++i;
                }

                // stream exhausted
                if (read1 <= 0)
                    break;

                ++blockNumber;
            }
        }

        void CompareStreams(Stream s1, Stream s2, int limit)
        {
            int retVal = 0;
            int blockNumber = 0;
            int readSize = 100;
            BinaryReader r1 = new BinaryReader(s1);
            BinaryReader r2 = new BinaryReader(s2);
            byte[] buf1 = new Byte[readSize];
            byte[] buf2 = new Byte[readSize];

            while (retVal == 0 && limit > 0)
            {
                int read1 = r1.Read(buf1, 0, Math.Min(limit, buf1.Length));
                int read2 = r2.Read(buf2, 0, Math.Min(limit, buf2.Length));
                DRT.Assert(read1 == read2, "Decode failure - read count is different: " + read1.ToString() + " vs " + read2.ToString());

                int i = 0;
                while (i < read1 && i < read2)
                {
                    DRT.Assert (buf1[i] == buf2[i], "Decode failure in block: " + blockNumber.ToString() + " - data is different at position: " + i.ToString());
                    ++i;
                }

                // stream exhausted
                if (read1 <= 0)
                    break;

                ++blockNumber;
                limit -= read1;
            }
        }

        void CompareStreamAndFile(Stream s, string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            CompareStreams(s, fs);
            fs.Close();
        }
        void CompareStreamAndFile(Stream s, string fileName, int limit)
        {
            if (limit == -1)
            {
                CompareStreamAndFile(s, fileName);
                return;
            }

            FileStream fs = new FileStream(fileName, FileMode.Open);
            CompareStreams(s, fs, limit);
            fs.Close();
        }

        void FileToStream(string fileName, Stream s)
        {
            //            s.Seek(0, SeekOrigin.Begin);
            byte[] buf = new Byte[100];
            FileStream fs = new FileStream(fileName, FileMode.Open);

            int read;
            while ((read = fs.Read(buf, 0, buf.Length)) > 0)
            {
                s.Write(buf, 0, read);
            }
            s.Flush();
            fs.Close();
        }

        void FileToStream(string fileName, Stream s, int limit)
        {
            if (limit == -1)
            {
                FileToStream(fileName, s);
                return;
            }

            //            s.Seek(0, SeekOrigin.Begin);
            byte[] buf = new Byte[1024];
            FileStream fs = new FileStream(fileName, FileMode.Open);

            int read;
            while ((limit > 0) && (read = fs.Read(buf, 0, Math.Min(limit, buf.Length))) > 0)
            {
                s.Write(buf, 0, read);
                limit -= read;
            }
            s.Flush();
            fs.Close();
        }

        Uri CreatePartUri(string partUri)
        {
            return PackUriHelper.CreatePartUri(new Uri(partUri, UriKind.Relative));
        }
        #endregion

        public override void ReleaseResources() 
        { 
            // clean up - delete fileName
            Console.WriteLine("\nDeleting container fileName: " + s_fileName);
            FileInfo fi = new FileInfo(s_fileName);
//            if (fi.Exists)
//               fi.Delete();
        }
    }

}

