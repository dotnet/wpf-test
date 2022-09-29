// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description:
//  This class represents a single test case in the DRT that tests serialization
//  of DRX classes.
//
//

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Markup;
using System.Xml;



namespace MS.Internal.WppDrt.EDocsUx
{
    internal delegate int SerializationTestCasePropertyComparer(object obj);

    /// <summary>
    ///   This class represents a single serialization test case.
    /// </summary>
    internal class SerializationTestCase
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------
        #region Constructors
        
        /// <summary>
        ///   Constructor for the SerializationTestCase class.
        /// </summary>
        /// <param name="testName">
        ///   Human-readable name for this test case.
        /// </param>
        /// <param name="inputFile">
        ///   Path to input file containing the XAML representation of an object
        ///   to be deserialized.
        /// </param>
        /// <param name="comparer">
        ///   Delegate method to compare the properties of the deserialized object
        ///   with their expected values.
        /// </param>
        /// <param name="baselineFile">
        ///   Path to the file against which the result of serializing the object
        ///   will be compared.
        /// </param>
        internal SerializationTestCase(
            string testName,
            string inputFile,
            SerializationTestCasePropertyComparer comparer,
            string baselineFile
            )
        {
            _testName     = testName;
            _inputFile    = inputFile;
            _comparer     = comparer;
            _baselineFile = baselineFile;
        }

        #endregion Constructors

        //------------------------------------------------------
        //
        //  Internal methods
        //
        //------------------------------------------------------
        #region Internal methods

        /// <summary>
        ///   Execute this serialization test case.
        /// </summary>
        /// <param name="host">
        ///   Object that provides services needed by serialization test cases.
        /// </param>
        /// <returns>
        ///   0 on success, 1 on failure
        /// </returns>
        internal int Run(ISerializationTestCaseHost host)
        {
            int result = 0;
            object obj;         // The deserialized object.            

            Console.WriteLine("Test: " + _testName);

            // Load the input file to deserialize the object.
            string inputFile = host.BaselineDirectory + @"\" + _inputFile;

            using (FileStream fs = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            {
                obj = XamlReader.Load(fs);
            }

            // Re-serialize the resulting object.
            StringBuilder sb = new StringBuilder();
            TextWriter writer = new StringWriter(sb);
            XmlTextWriter xmlWriter = new XmlTextWriter(writer);

            XamlWriter.Save(obj, xmlWriter);

            // Compare the properties of the deserialized object to their
            // expected values. We do this -after- re-serializing, so that in case
            // of a mismatch, the re-serialized string is available for display
            // in the error message.
            if (_comparer != null && _comparer(obj) != 0)
            {
                Console.WriteLine("\nERROR: Deserialized object properties do not have their expected values.\n");
                Console.WriteLine("Deserialized object is: ");
                Console.WriteLine(sb.ToString());
                Console.WriteLine("To find out which properties have unexpected values, look at method");
                Console.WriteLine("Compare" + obj.GetType().Name + "Properties in file DrtDRXSerialization.cs.\n");
                result = 1;
                goto end;
            }

            string baselineFile = host.BaselineDirectory + @"\" + _baselineFile;

            switch (host.Mode)
            {
                // "RunDrt" (normal) run mode: Compare the serialized object with the contents
                // of a baseline file.
                case SerializationTestCaseRunMode.RunDrt:
                    Console.WriteLine("Comparing with baseline file: " + baselineFile + "...");
                    try
                    {
                        using (StreamReader sr = new StreamReader(baselineFile))
                        {
                            string baselineString = sr.ReadToEnd();
                            Microsoft.Test.Markup.XmlCompareResult xmlCompareResult = Microsoft.Test.Markup.XamlComparer.Compare(baselineString, sb.ToString());
                            if (xmlCompareResult.Result == Microsoft.Test.Markup.CompareResult.Different)
                            {
                                Console.WriteLine("\nERROR: Serialized object does not match baseline file.\n");
                                Console.WriteLine("Serialized object is: ");
                                Console.WriteLine(sb.ToString());
                                Console.WriteLine("Baseline file is: ");
                                Console.WriteLine(baselineString);
                                Console.WriteLine("\nConsider running 'DrtDRXSerialization -rebaseline' to update the\n");
                                Console.WriteLine("\nbaseline file if you have made changes that affect serialization.\n");
                                result = 1;
                            }
                        }
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        Console.WriteLine(
                            "ERROR: The baseline file " + baselineFile + "does not exist."
                            );
                        result = 1;
                    }
                    Console.WriteLine("Testing round-tripping...");

                    // Write out the serialized form of the object that we just created. Use try/catch
                    // to ensure that the temporary file is always deleted.
                    string tempFile = baselineFile + "Temp";

                    try
                    {
                        using (StreamWriter sw = new StreamWriter(tempFile))
                        {
                            sw.Write(sb.ToString());
                        }

                        // Read in the newly created file and compare properties.
                        using (FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.Read))
                        {
                            obj = XamlReader.Load(fs);
                        }

                        if (_comparer != null && _comparer(obj) != 0)
                        {
                            Console.WriteLine("\nERROR: After round-trip, Deserialized object properties do not have their expected values.\n");
                            Console.WriteLine("To find out which properties have unexpected values, look at method");
                            Console.WriteLine("Compare" + obj.GetType().Name + "Properties in file DrtDRXSerialization.cs.\n");
                            result = 1;
                            goto end;
                        }
                    }
                    finally
                    {
                        File.Delete(tempFile);
                    }
                    break;

                // "Rebaseline" mode: Replace the contents of the baseline file.
                case SerializationTestCaseRunMode.Rebaseline:
                    Console.WriteLine("Creating baseline file: " + baselineFile);
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(baselineFile))
                        {
                           sw.Write(sb.ToString());
                        }
                    }
                    catch (System.UnauthorizedAccessException)
                    {
                        Console.WriteLine(
                            "ERROR: Cannot write to baseline file " + baselineFile + ".\n" +
                            "Have you have checked out the baseline files in " + host.BaselineDirectory + "?"
                            );
                        result = 1;
                    }
                    break;

                default:
                    Debug.Assert(false);
                    Console.WriteLine("Unrecognized run mode: " + host.Mode);
                    result = 1;
                    break;
            }

        end:
            if (host.Mode == SerializationTestCaseRunMode.RunDrt)
            {
                if (result == 0)
                    Console.WriteLine("Passed");
                else
                    Console.WriteLine("FAILED: " + _testName);
            }

            return result;
        }

        #endregion Internal methods

        //------------------------------------------------------
        //
        //  Private fields
        //
        //------------------------------------------------------
        #region Private fields

        // Human-readable name for this test case.
        private string _testName;

        // Path to input file containing the XAML representation of an object
        // to be deserialized.
        private string _inputFile;

        // Delegate method to compare properties of the deserialized object with
        // their expected values.
        private SerializationTestCasePropertyComparer _comparer;

        // Path to file against which the result of the serializating the object
        /// will be compared.
        private string _baselineFile;

        #endregion Private fields
    }
}


