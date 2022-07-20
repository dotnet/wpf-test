// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description:
//  This class provides the DRT for the DrtEncryptedPackageCoreProperties class,
//  which provides access to the core properties (a subset of the standard OLE
//  properties) in a compound file representing an RM-protected Metro document.
//
//

using DRT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Text;
using System.Windows;
using System.Security.RightsManagement;

using MS.Internal.IO.Packaging;

namespace DRT
{
    public class DrtEncryptedPackageCoreProperties : DrtBase
    {
        [STAThread]
        static int Main(string[] args)
        {
            DrtBase drt = new DrtEncryptedPackageCoreProperties();

            return drt.Run(args);
        }

        private DrtEncryptedPackageCoreProperties()
        {
            WindowTitle = "DrtEncryptedPackageCoreProperties";
            DrtName = "DrtEncryptedPackageCoreProperties";
            WindowSize = new Size(800, 750);
            TeamContact = "WPF";
            Contact = "Microsoft";
            Suites = new DrtTestSuite[] {
                        new StorageBasedPackagePropertiesTestSuite(),
                        };
        }        
    }

    /// <summary>
    /// </summary>
    public sealed class StorageBasedPackagePropertiesTestSuite : DrtTestSuite
    {
        /// <summary>
        /// </summary>
        public StorageBasedPackagePropertiesTestSuite()
            : base("StorageBasedPackagePropertiesTestSuite")
        {
        }

        /// <summary>
        /// </summary>
        public override DrtTest[] PrepareTests()
        {
            //
            // Ensure that the directory to hold the test files exists.
            //
            DirectoryInfo di = new DirectoryInfo(sc_testFileDirectory);
            di.Create();

            return new DrtTest[] {
                new DrtTest(TestProperties)
                };
        }

        #region Test methods

        private void TestProperties()
        {
            foreach (TestDefinition test in _testDefinitions)
            {
                string pathName = sc_testFileDirectory + test.fileName;
                File.Copy(pathName, sc_tempTestFile, true /* => overwrite */);

                using (EncryptedPackageEnvelope ep = EncryptedPackageEnvelope.Open(sc_tempTestFile, FileAccess.ReadWrite))
                {
                    using (PackageProperties props = ep.PackageProperties)
                    {
                        string title = props.Title;
                        DRT.Assert(
                                title == test.title,
                                "Incorrect Title property in test file " + pathName + "\n" +
                                "Expected " + StringOrNull(test.title) + "; got " + StringOrNull(title) + ".\n"
                                );

                        string subject = props.Subject;
                        DRT.Assert(
                                subject == test.subject,
                                "Incorrect Subject property in test file " + pathName + "\n" +
                                "Expected " + StringOrNull(test.subject) + "; got " + StringOrNull(subject) + ".\n"
                                );
                                
                        //
                        // Test deleting a property.
                        //
                        if (subject != null)
                        {
                            props.Subject = null;
                            
                            //
                            // It should be gone now.
                            //
                            subject = props.Subject;
                            DRT.Assert(
                                subject == null,
                                "Failed to delete Subject property from test file " + pathName + "\n"
                                );
                        }
                        
                        //
                        // Test adding a property that doesn't exist (the test files don't have this
                        // property). Also take the opportunity to test DateTime properties.
                        //
                        DateTime now = DateTime.Now;
                        props.Modified = now;
                        
                        DateTime then = (DateTime)props.Modified;
                        DRT.Assert(
                            then == now,
                            "Failed to create Modified property in test file " + pathName + "\n"
                            );

                        //
                        // Test reading a string property that doesn't exist.
                        //
                        DRT.Assert(
                            props.Language == null,
                            "Failed to properly read a non-existent string property"
                            );

                        //
                        // Test reading a DateTime property that doesn't exist.
                        //
                        DRT.Assert(
                            props.LastPrinted == null,
                            "Failed to properly read a non-existent DateTime property"
                            );

                        //
                        // Test round-tripping a string property.
                        //
                        const string testKeywords = "one, two, three";
                        string prevKeywords = props.Keywords;   // Save any existing value.

                        props.Keywords = testKeywords;
                        
                        DRT.Assert(
                            props.Keywords == testKeywords,
                            "Failed to round-trip a string property"
                            );

                        props.Keywords = prevKeywords;
                    }
                }
            }
        }

        #endregion Test methods
        
        #region Private helper methods
        
        private string StringOrNull(string s)
        {
            return s == null ? "<null>" : s;
        }
        
        #endregion

        #region Private fields

        #endregion Private fields

        #region Constants

        //
        // Name of directory containing test files generated by this DRT.
        //
        private static readonly string sc_testFileDirectory = @"DrtFiles\EncryptedPackageCoreProperties\";
        
        //
        // Name of a temporary file to which each test file, in turn, is copied, so that we
        // don't overwrite the originals.
        //
        private static readonly string sc_tempTestFile = sc_testFileDirectory + "TempTestFile.xps";

        #endregion Constants

        #region Test data

        private class TestDefinition
        {
            internal
            TestDefinition(
                string fileName,
                string title,
                string subject
                )
            {
                this.fileName = fileName;
                this.title = title;
                this.subject = subject;
            }

            internal string fileName;
            internal string title;
            internal string subject;
        }

        private static readonly TestDefinition[] _testDefinitions =
        {
            new TestDefinition(
                    "EncryptedPackageWithProperties.xps",
                    "Encrypted Package With Properties",        // Title
                    "Testing EncryptedPackageCoreProperties"    // Subject
                    ),

            new TestDefinition(
                    "EncryptedPackageWithoutProperties.xps",
                    null,
                    null
                    )
        };

        #endregion Test data
    }
}
