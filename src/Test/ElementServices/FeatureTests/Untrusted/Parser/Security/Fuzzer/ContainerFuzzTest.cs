// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      Represents a xaml fuzz test. Contains overrides for generating, 
 *      fuzzing, and loading xaml files.
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.IO;
using System.IO.Packaging;
using System.Xml;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Runtime.InteropServices;

using Microsoft.Test.Container;
using Microsoft.Test.Markup;
using Microsoft.Test.Serialization;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Parser;


namespace Avalon.Test.CoreUI.Parser.Security
{

    /// <summary>
    /// </summary>
    public class ContainerFuzzTest : FuzzTest
    {
        /// <summary> 
        /// Represents a xaml fuzz test. Contains overrides for generating, 
        /// fuzzing, and loading xaml files.
        /// </summary>
        public ContainerFuzzTest()
            : base()
        {

        }

        /// <summary>
        /// </summary>
        public ContainerFuzzTest(XmlElement xmlElement)
            : base(xmlElement)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string CreateFile()
        {
            string tempContainerFile = GetRandomFileNameWithExtension("container");

//            StorageInfo strInfo = null;

            // Clean existing file.
            if (File.Exists(tempContainerFile))
                File.Delete(tempContainerFile);

            // Add substorage
            CoreLogger.LogStatus("Create test container.");
            StorageRootWrapper storageRoot = StorageRootWrapper.Open(tempContainerFile);
            _CreateStorage(storageRoot, 2, 2, 2);
            storageRoot.Close();

            return tempContainerFile;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationFilePath"></param>
        protected override void DoFuzz(string sourceFilePath, string destinationFilePath)
        {
            CoreLogger.LogStatus("Fuzzing container...");
            File.Copy(sourceFilePath, destinationFilePath, true);

            // Fuzz the records.
            foreach (ContainerFuzzer fuzzer in this.Fuzzers)
            {
                fuzzer.FuzzContainer(destinationFilePath, destinationFilePath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fuzzedFile"></param>
        protected override void TestFuzzedFile(string fuzzedFile)
        {

            CoreLogger.LogStatus("Testing container...");

            testPlanLogger.Log("<Title />");

            StorageRootWrapper stRoot = StorageRootWrapper.Open(fuzzedFile);
            try
            {
                DumpStorageInfo(stRoot, dumpDepth);
            }
            finally
            {
                stRoot.Close();
            }
        }

        // Generates fuzz and writes it to the given Stream.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamInfo"></param>
        protected void _WriteFuzzToStream(StreamInfo streamInfo)
        {
            _WriteFuzzToStream(streamInfo, false);
        }

        // Generates fuzz and writes it to the given Stream.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamInfo"></param>
        /// <param name="logToTestPlan"></param>
        protected void _WriteFuzzToStream(StreamInfo streamInfo, Boolean logToTestPlan)
        {
            if (logToTestPlan) {testPlanLogger.Log("<GetStreamFromStreamInfo Name=\"" + streamInfo.Name + "\" SeqNum=\"" + seqNum++ + " />");}

            Stream stream = streamInfo.GetStream();
            StreamWriter writer = new StreamWriter(stream);

            // put back call to GetFuzz
            // String byte[] bytes = new byte[100];
            // String random.NextBytes(bytes);
            // String System.Text.Encoding encoding = System.Text.Encoding.Unicode;
            // String fuzz = encoding.GetString(bytes);
            String fuzz = FuzzerBase.GetFuzz(new Random()); 

            if (logToTestPlan)
            {
                if (logToTestPlan) { testPlanLogger.Log("<WriteFuzz Name=\"" + streamInfo.Name + "\" SeqNum=\"" + seqNum++ + "\"FuzzBytes=\"" + fuzz + "\" />"); }
            } 
            writer.Write(fuzz);
            writer.Flush();
            writer.Close();
        }


        /// <summary>
        /// Walks through the storage tree of the given StorageInfo,
        /// and randomly inserts new streams and storages, deletes storages and 
        /// streams, and changes existing streams. 
        /// </summary>
        /// <param name="storageInfo"></param>
        /// <param name="depth"></param>
        protected virtual void DumpStorageInfo(StorageInfo storageInfo, int depth)
        {
            if (depth == 0)
            {
                return;
            }


            testPlanLogger.Log("<StorageInfo Name=" + storageInfo.Name + " >");
            testPlanLogger.Indent();
            // Sometimes creates new streams.
            if (random.Next(2) < 1)
            {
                String nameForChildStream = "RandomSubStream" + (_randomInjectionCount++).ToString();
                testPlanLogger.Log("<CreateStream Name=\"" + nameForChildStream + "\" SeqNum=\"" + seqNum++ + "\"/>");
                StreamInfo childStreamInfo = storageInfo.CreateStream(nameForChildStream);
                _WriteFuzzToStream(childStreamInfo, true);

            }

            // Sometimes create a substorage.
            if (random.Next(2) < 1)
            {
                String nameForSubStorage = "RandomSubStorage" + (_randomInjectionCount++).ToString();
                testPlanLogger.Log("<CreateSubstorage Name=\"" + nameForSubStorage + "\" SeqNum=\"" + seqNum++ + "\"/>");
                StorageInfo childStorage = storageInfo.CreateSubStorage(nameForSubStorage);
                _CreateStorage(childStorage, 2, 2, 2, true);
            }

            // Dump child storages.
            testPlanLogger.Log("<GetSubStorages SeqNum=\"" + seqNum++ + "\"/>");
            StorageInfo[] storages = storageInfo.GetSubStorages();
            for (int i = 0; i < storages.Length; i++)
            {
                string storageName = storages[i].Name;
                CoreLogger.LogStatus("  Child storage: " + storageName);

                DumpStorageInfo(storages[i], depth - 1);

                // Sometimes delete the substorage.
                if (random.Next(2) < 1)
                {
                    testPlanLogger.Log("<DeleteSubstorage Name=\"" + storageName + "\" SeqNum=\"" + seqNum++ + "\"/>");
                    storageInfo.DeleteSubStorage(storageName);
                    CoreLogger.LogStatus("  Child storage: " + storageName + " - deleted");
                }
            }

            // Dump child streams.
            testPlanLogger.Log("<GetStreams SeqNum=\"" + seqNum++ + "\"/>");
            StreamInfo[] streams = storageInfo.GetStreams();
            for (int i = 0; i < streams.Length; i++)
            {
                string streamName = streams[i].Name;
                CoreLogger.LogStatus("  Child stream: " + streamName);

                // Sometimes delete the stream. Other times, just change the stream.
                if (random.Next(2) < 1)
                {
                    testPlanLogger.Log("<DeleteStream  Name=\"" + streamName + "\" SeqNum=\"" + seqNum++ + "\"/>");
                    storageInfo.DeleteStream(streamName);
                    CoreLogger.LogStatus("  Child stream: " + streamName + " - deleted");
                }
                else
                {
                    _WriteFuzzToStream(streams[i], true);
                    CoreLogger.LogStatus("  Child stream: " + streamName + " - fuzzed");

                }
            }
            testPlanLogger.Outdent();
            testPlanLogger.Log("</StorageInfo>");
        }
        // Generates a sub-storages and streams rooted with the given
        // StorageInfo. The resultant storage tree may be saved as
        // a compound file (container).
        private void _CreateStorage(StorageInfo storageInfo, int maxStorageCount, int maxStreamCount, int depth)
        {
            _CreateStorage(storageInfo, maxStorageCount, maxStreamCount, depth, false);
        }

        private void _CreateStorage(StorageInfo storageInfo, int maxStorageCount, int maxStreamCount, int depth, Boolean logToTestPlan)
        {
            if (depth <= 0)
            {
                return;
            }
            if (logToTestPlan) {testPlanLogger.Log("<StorageInfo Name=\"" + storageInfo.Name + "\" >");};
            if (logToTestPlan) {testPlanLogger.Indent();};
            int storageCount = random.Next(maxStorageCount + 1);
            int streamCount = random.Next(maxStreamCount + 1);

            for (int i = 0; i < storageCount; i++)
            {
                String nameForSubStorage = "SubStorage" + i.ToString() + "_Depth" + depth.ToString();
                if (logToTestPlan) { testPlanLogger.Log("<CreateSubStorage Name=\"" + nameForSubStorage + "\" SeqNum=\"" + seqNum++ + "\"/>"); }
                StorageInfo childStorage = storageInfo.CreateSubStorage(nameForSubStorage);
                _CreateStorage(childStorage, maxStorageCount, maxStreamCount, depth - 1, logToTestPlan);
            }

            for (int i = 0; i < streamCount; i++)
            {
                String nameForChildStream = "SubStream" + i.ToString() + "_Depth" + depth.ToString();
                if (logToTestPlan) { testPlanLogger.Log("<CreateStream  Name=\"" + nameForChildStream + "\" SeqNum=\"" + seqNum++ + "\"/>"); }
                StreamInfo childStreamInfo = storageInfo.CreateStream(nameForChildStream);
                _WriteFuzzToStream(childStreamInfo, logToTestPlan);
            }
            if (logToTestPlan) { testPlanLogger.Outdent(); };
            if (logToTestPlan) {testPlanLogger.Log("</StorageInfo>");};
            
        }

        // Checks if the given Exception is a kind that we expect from
        // the Parser when loading bogus xaml or baml streams.
        private bool _IsExpectedExceptionType(Exception ex)
        {
            return (ex is IOException)
                || (ex is FileFormatException)
                || (ex is COMException && ex.Message.Contains("80030109"))
                || (ex is COMException && ex.Message.Contains("Failed call to") && ex.Message.Contains("IStorage.CreateStream"))
                || (ex is COMException && ex.Message.Contains("8003001E"))
                || (ex is COMException && ex.Message.Contains("Compound File API failure"))
                || (ex is COMException && ex.Message.Contains("Call to '") && ex.Message.Contains("' failed."))
                || (ex is COMException && ex.Message.Contains("Failed call to '") && ex.Message.Contains("IStorage.OpenStream"))
                || (ex is COMException && ex.Message.Contains("Failed call to '") && ex.Message.Contains("IStorage::OpenStorage"))
                || (ex is COMException && ex.Message.Contains("Failed call to '") && ex.Message.Contains("IStorage.OpenStorage"))
                || (ex is COMException && ex.Message.Contains("Failed call to '") && ex.Message.Contains("IStorage.CreateStorage"));
        }

        /// <summary>
        /// Checks if the given exception is an expected one for corrupted containers.
        /// </summary>
        protected override bool IsExceptionOkay(Exception ex)
        {
            if (!_IsExpectedExceptionType(ex))
                return false;

            ex = this.GetInnermostException(ex);

            return _IsExpectedExceptionType(ex);
        }

        /// <summary>
        /// keeps track of streams that are injected into storageinfo
        /// </summary>
        protected int _randomInjectionCount = 0;

        /// <summary>
        /// 
        /// </summary>
        protected int seqNum = 0;
        /// <summary>
        /// 
        /// </summary>
        protected int dumpDepth = 3;
    }
}

