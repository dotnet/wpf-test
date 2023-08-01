// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      Represents a baml fuzz test. Contains overrides for generating, 
 *      fuzzing, and loading baml files.
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.IO;
using System.Xml;
using System.Threading;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Markup;
using Microsoft.Test.MSBuildEngine;
using Microsoft.Test.Markup;
using Microsoft.Test.Serialization;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Parser;

namespace Avalon.Test.CoreUI.Parser.Security
{
    /// <summary>
    /// Represents a baml fuzz test. Contains overrides for generating, 
    /// fuzzing, and loading baml files.
    /// </summary>
    public class BamlFuzzTest : ParserFuzzTest
    {
        /// <summary>
        /// </summary>
        public BamlFuzzTest() : base()
        {
        }

        /// <summary>
        /// </summary>
        public BamlFuzzTest(XmlElement xmlElement) : base(xmlElement)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string CreateFile()
        {
            string tempXamlFile = base.CreateFile();

            CoreLogger.LogStatus("Converting xaml to baml...");
            string tempBamlFile = _ConvertXamlToBaml(tempXamlFile);

            return tempBamlFile;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationFilePath"></param>
        protected override void DoFuzz(string sourceFilePath, string destinationFilePath)
        {
            CoreLogger.LogStatus("Fuzzing baml...");

            // Get the records.
            BamlRecord[] records = _ReadBamlRecords(sourceFilePath);

            // Fuzz the records.
            foreach (BamlFuzzer fuzzer in this.Fuzzers)
            {
                fuzzer.FuzzBamlRecords(records);
            }

            // Save the records to the given destination path.
            _SaveBamlRecords(records, destinationFilePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fuzzedFile"></param>
        protected override void TestFuzzedFile(string fuzzedFile)
        {
            CoreLogger.LogStatus("Loading fuzzed baml...");
            try
            {
                BamlHelper.LoadBaml(fuzzedFile);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private string _ConvertXamlToBaml(string xamlFile)
        {
            // Generated BAML will have the same name as the XAML
            string bamlFile = xamlFile.Replace(".xaml", ".baml");
            string compiledFile = "obj\\release\\" + bamlFile;

            //
            // Cleanup old compile directories and files if necessary.
            // 
            CompilerHelper compiler = new CompilerHelper();
            compiler.CleanUpCompilation();

            if (File.Exists(bamlFile))
            {
                File.Delete(bamlFile);
            }

            //
            // Compile xaml into Avalon app.
            //
            compiler.CompileApp(xamlFile, "Application");

            CoreLogger.LogStatus("Check if the BAML exists");
            if (File.Exists(compiledFile))
            {
                File.Copy(compiledFile, bamlFile);
            }
            else
            {
                throw new Microsoft.Test.TestSetupException("Could not find '" + compiledFile + "'.");
            }

            return bamlFile;
        }

        /// <summary>
        /// Retreive an array of BamlRecords from a file
        /// </summary>
        private static BamlRecord[] _ReadBamlRecords(string sourcePath)
        {
            FileStream stream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader reader = new BinaryReader(stream, System.Text.Encoding.UTF8);
            ArrayList list = new ArrayList();

            try
            {
                while (true)
                {
                    list.Add(_GetNextBamlRecord(reader));
                }
            }
            catch (EndOfStreamException)
            {
                CoreLogger.LogStatus("Successfully read " + list.Count + " records from baml file");
                BamlRecord[] records = new BamlRecord[list.Count];
                list.CopyTo(records);
                return records;
            }
            finally
            {
                reader.Close();
                reader = null;
            }
        }

        private static BamlRecord _GetNextBamlRecord(BinaryReader reader)
        {
            Int16 type = reader.ReadInt16();
            switch ((BamlRecordType)type)
            {
                case BamlRecordType.ConstructorParametersEnd:
                case BamlRecordType.ConstructorParametersStart:
                case BamlRecordType.DocumentEnd:
                case BamlRecordType.ElementEnd:
                case BamlRecordType.KeyElementEnd:
                case BamlRecordType.PropertyArrayEnd:
                case BamlRecordType.PropertyComplexEnd:
                case BamlRecordType.PropertyIDictionaryEnd:
                case BamlRecordType.PropertyIListEnd:
                    return new FixedSizeBamlRecord(type, 0, new byte[0]);

                case BamlRecordType.ConstructorParameterType:
                case BamlRecordType.ElementStart:
                case BamlRecordType.RootElement:
                case BamlRecordType.PropertyComplexStart:
                case BamlRecordType.PropertyArrayStart:
                case BamlRecordType.PropertyIDictionaryStart:
                case BamlRecordType.PropertyIListStart:
                    return new FixedSizeBamlRecord(type, 2, reader.ReadBytes(2));

                case BamlRecordType.ConnectionId:
                    return new ConnectionIdBamlRecord(reader.ReadBytes(4));

                case BamlRecordType.DeferableContentStart:
                case BamlRecordType.PropertyStringReference:
                case BamlRecordType.PropertyTypeReference:
                    return new FixedSizeBamlRecord(type, 4, reader.ReadBytes(4));

                // It turns out that these two types actually read (in order):
                //      2 bytes : _typeId (Int16)
                //      4 bytes : _valuePosition (Int32)
                //      1 byte  : _shared (Boolean)
                //      1 byte  : _sharedSet (Boolean)
                //
                // This doesn't match what "RecordSize" returns for these types in Avalon,
                //  but what do you do?
                case BamlRecordType.DefAttributeKeyType:
                case BamlRecordType.KeyElementStart:
                    return new FixedSizeBamlRecord(type, 8, reader.ReadBytes(8));

                default:
                    Int32 size = reader.ReadInt32();
                    return new VariableSizeBamlRecord(type, size, reader.ReadBytes(size - 4));
            }
        }

        /// <summary>
        /// Save an array of BamlRecords to a file
        /// </summary>
        /// <param name="records">The array of BamlRecords to save</param>
        /// <param name="destinationPath">The path where the BamlRecords should be saved</param>
        private static void _SaveBamlRecords(BamlRecord[] records, string destinationPath)
        {
            FileStream stream = new FileStream(destinationPath, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream, System.Text.Encoding.UTF8);

            foreach (BamlRecord record in records)
            {
                writer.Write((byte[])record);
            }
            writer.Close();
        }
    }
}

