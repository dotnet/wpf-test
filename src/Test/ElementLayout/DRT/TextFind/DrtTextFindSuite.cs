// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Common functionality for text find test suites.
//
//

using System;
using System.IO;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Common functionality for text find test suites.
    // ----------------------------------------------------------------------
    internal abstract class DrtTextFindSuite : DrtTestSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        protected DrtTextFindSuite(string testName, string contactName) : base(testName)
        {
            Contact = contactName;
        }

        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        static DrtTextFindSuite()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
            Type typeLayoutDump = assembly.GetType("MS.Internal.LayoutDump");
            _miDumpLayout = typeLayoutDump.GetMethod("DumpLayoutTree", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        }

        // ------------------------------------------------------------------
        // Initialize tests.
        // ------------------------------------------------------------------
        public override DrtTest[] PrepareTests()
        {
            // Initialize the suite here. This includes loading the tree.
            _contentRoot = new Border();
            _contentRoot.Width = 800;
            _contentRoot.Height = 600;
            Border root = new Border();
            root.Background = Brushes.White;
            root.Child = _contentRoot;
            DRT.Show(root);

            // Prepare the lists of tests to run against the tree
            DrtTest verifyLayoutCreate = new DrtTest(VerifyLayoutCreate);
            DrtTest verifyLayoutAppend = new DrtTest(VerifyLayoutAppend);
            DrtTest verifyLayoutFinalize = new DrtTest(VerifyLayoutFinalize);

            DrtTest[] tests = CreateTests();
            DrtTest[] testsWithVerification = new DrtTest[tests.Length * 2];

            for (int i = 0; i < testsWithVerification.Length; ++i)
            {
                if ((i % 2) == 0)
                {
                    testsWithVerification[i] = tests[i/2];
                }
                else
                {
                    if      (i == 1)                                    testsWithVerification[i] = verifyLayoutCreate;
                    else if (i == (testsWithVerification.Length - 1))   testsWithVerification[i] = verifyLayoutFinalize;
                    else                                                testsWithVerification[i] = verifyLayoutAppend;
                }
            }

            return (testsWithVerification);
        }

        // ------------------------------------------------------------------
        // Create collection of tests.
        // ------------------------------------------------------------------
        protected abstract DrtTest[] CreateTests();

        // ------------------------------------------------------------------
        // Load content from xaml file.
        // ------------------------------------------------------------------
        protected void LoadContentFromXaml(string xamlFileName)
        {
            UIElement content = null;
            System.IO.Stream stream = null;
            string fileName = this.DrtFilesDirectory + xamlFileName + ".xaml";
            try
            {
                stream = System.IO.File.OpenRead(fileName);
                content = System.Windows.Markup.XamlReader.Load(stream) as UIElement;
            }
            finally
            {
                // done with the stream
                if (stream != null) { stream.Close(); }
            }
            DRT.Assert(content != null, "{0}: Failed to load xaml file '{1}'", this.TestName, fileName);
            _contentRoot.Child = content;
        }

        // ------------------------------------------------------------------
        // Dump layout details.
        // ------------------------------------------------------------------
        protected void DumpLayoutTree(bool append, bool verify)
        {
            string masterFile = DrtFilesDirectory + "Master" + this.TestName + ".xml";
            string testFile = DrtFilesDirectory + "Test" + this.TestName + ".xml";
            string win8MasterFile = DrtFilesDirectory + "Win8\\Master" + this.TestName + ".xml";
            Version ver = Environment.OSVersion.Version;
            if ((ver.Major > 6 || (ver.Major == 6 && ver.Minor > 1)) && File.Exists(win8MasterFile))
            {
                masterFile = win8MasterFile;
            }

            switch (((DrtTextFindBase)DRT).DumpMode)
            {
                case DrtTextFindBase.DumpModeType.Drt:
                    DumpLayoutTree(_contentRoot.Child, testFile, append);
                    if (verify && !CompareDumps(masterFile, testFile))
                    {
                        DRT.Assert(false, "{0}: Layout dump failed. Run '{1} -suite {2} -diff' to see differences.", this.TestName, AppDomain.CurrentDomain.FriendlyName, this.Name);
                    }
                    break;

                case DrtTextFindBase.DumpModeType.Diff:
                    DumpLayoutTree(_contentRoot.Child, testFile, append);
                    if (verify && !CompareDumps(masterFile, testFile))
                    {
                        DiffDumps(masterFile, testFile);
                    }
                    break;

                case DrtTextFindBase.DumpModeType.Dump:
                    DumpLayoutTree(_contentRoot.Child, masterFile, append);
                    break;

                case DrtTextFindBase.DumpModeType.View:
                    System.Threading.Thread.Sleep(500); //delay 0.5 sec
                    break;
            }
        }

        // ------------------------------------------------------------------
        // Dump content of the specified layout tree.
        //
        //      root - root of the layout tree
        //      dumpFile - full path to dump file
        //      append - append dump to existing file?
        // ------------------------------------------------------------------
        private void DumpLayoutTree(UIElement root, string dumpFile, bool append)
        {
            Stream stream = null;
            XmlTextWriter writer = null;
            FileMode mode = append ? FileMode.Append : FileMode.Create;

            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // This is hacky, but needed to unblock random failures of File.Open
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            bool opened = false;
            int cFailed = 0;
            while (!opened && cFailed < 10)
            {
                try
                {
                    stream = File.Open(dumpFile, mode);
                    writer = new XmlTextWriter(stream, new System.Text.ASCIIEncoding());
                    opened = true;
                }
                // I have seen this exception on some longhorn builds. It has to do
                // with some timing constrains related to file cache. I it fails,
                // retry the same operation up to 10 times.
                // Catch and handle exception to avoid random failures of DRTs.
                catch (System.IO.IOException)
                {
                    GC.Collect();
                    System.Threading.Thread.Sleep(1000);
                    ++cFailed;
                }
            }
            if (!opened)
            {
                DRT.Assert(false, "{0}: Aborting dump process due to failure to open dump file: {1}", this.TestName, dumpFile);
                return;
            }
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            _miDumpLayout.Invoke(null, new object[] { writer, "UIElmentDump_" + (++_dumpCounter), root });
            writer.Flush();
            writer.Close();
        }

        // ------------------------------------------------------------------
        // Compare test dump file with master dump file.
        //
        //      masterFile - full path to master dump file
        //      testFile - full path to test dump file
        //
        // Returns: 'true' if dump files match, 'false' otherwise.
        // ------------------------------------------------------------------
        private bool CompareDumps(string masterFile, string testFile)
        {
            bool result = false;
            try
            {
                StreamReader master = File.OpenText(masterFile);
                StreamReader test = File.OpenText(testFile);
                string masterContent = master.ReadToEnd();
                string testContent = test.ReadToEnd();
                result = (masterContent.CompareTo(testContent) == 0);
            }
            // Exception may be thrown when master file is not found. Even if we
            // continue DRT will fail with appropriate message.
            // Catch and handle IOException, because it is useful when adding new DRTs.
            catch (System.IO.IOException e)
            {
                DRT.Assert(false, "{0}: Failed to compare dumps: {1}.", this.TestName, e.ToString());
            }
            return result;
        }

        // ------------------------------------------------------------------
        // Run windiff tool to view differences between master and test dump file.
        //
        //      masterFile - full path to master dump file
        //      testFile - full path to test dump file
        // ------------------------------------------------------------------
        private void DiffDumps(string masterFile, string testFile)
        {
            System.Diagnostics.Process.Start("windiff", masterFile + " " + testFile);
        }

        // ------------------------------------------------------------------
        // Create dump file and verify content.
        // ------------------------------------------------------------------
        private void VerifyLayoutCreate()
        {
            DumpLayoutTree(false, false);
        }

        // ------------------------------------------------------------------
        // Verify content and append to dump file.
        // ------------------------------------------------------------------
        private void VerifyLayoutAppend()
        {
            DumpLayoutTree(true, false);
        }

        // ------------------------------------------------------------------
        // Verify content and finalize dump file.
        // ------------------------------------------------------------------
        private void VerifyLayoutFinalize()
        {
            DumpLayoutTree(true, true);
        }

        // ------------------------------------------------------------------
        // Unique name of the test.
        // ------------------------------------------------------------------
        protected string TestName
        {
            get { return this.Name; }
        }

        // ------------------------------------------------------------------
        // Location of all DRT related files.
        // ------------------------------------------------------------------
        protected string DrtFilesDirectory
        {
            get { return DRT.BaseDirectory + "DrtFiles\\TextFind\\"; }
        }

        // ------------------------------------------------------------------
        // Placeholder for content.
        // ------------------------------------------------------------------
        private Border _contentRoot;

        // ------------------------------------------------------------------
        // Placeholder for content.
        // ------------------------------------------------------------------
        protected Border ContentRoot
        {
            get { return (_contentRoot); }
        }

        // ------------------------------------------------------------------
        // Method info for dump layout function.
        // ------------------------------------------------------------------
        protected static System.Reflection.MethodInfo _miDumpLayout;

        private int _dumpCounter;
    }
}
