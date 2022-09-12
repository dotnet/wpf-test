// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Common functionality for test suites using dump files.
//

using System;                   // string
using System.IO;                // Stream
using System.Xml;               // XmlTextWriter

namespace DRT
{
    /// <summary>
    /// Common functionality for test suites using dump files.
    /// </summary>
    internal abstract class DumpTestSuite : FlowTestSuite
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="suiteName">Name of the suite.</param>
        protected DumpTestSuite(string suiteName) : 
            base(suiteName)
        {
        }

        /// <summary>
        /// Dump content to XmlTextWriter.
        /// </summary>
        protected abstract void DumpContent(XmlTextWriter writer);

        /// <summary>
        /// Create dump file.
        /// </summary>
        protected void DumpCreate()
        {
            DumpToFile(false, false);
        }

        /// <summary>
        /// Append to dump file.
        /// </summary>
        protected void DumpAppend()
        {
            DumpToFile(true, false);
        }

        /// <summary>
        /// Append to dump file and verify content.
        /// </summary>
        protected void DumpFinalizeAndVerify()
        {
            DumpToFile(true, true);
        }

        /// <summary>
        /// Create dump file and verify content
        /// </summary>
        protected void DumpCreateAndVerify()
        {
            DumpToFile(false, true);
        }

        /// <summary>
        /// Dump content to file.
        /// </summary>
        /// <param name="append">Append to existing file.</param>
        /// <param name="verify">Verify content.</param>
        private void DumpToFile(bool append, bool verify)
        {
            string masterFileName = "Master" + this.Name + ".xml";
            string masterFile = DrtFilesDirectory + masterFileName;
            string testFile = DrtFilesDirectory + "Test" + this.Name + ".xml";

            switch (((DrtFlowBase)DRT).Mode)
            {
                case DrtFlowBase.ExecutionMode.Drt:
                    DumpToFile(testFile, append, verify);
                    if (verify && !CompareDumps(masterFile, testFile))
                    {
                        DRT.Assert(false, "Dump failed. Run '{0} -suite {1} -diff' to see differences.'", AppDomain.CurrentDomain.FriendlyName, this.Name);
                    }
                    break;

                case DrtFlowBase.ExecutionMode.Diff:
                    DumpToFile(testFile, append, verify);
                    if (verify && !CompareDumps(masterFile, testFile))
                    {
                        System.Diagnostics.Process.Start("windiff", masterFile + " " + testFile);
                    }
                    break;

                case DrtFlowBase.ExecutionMode.Update:
                    DumpToFile(masterFile, append, verify);
                    if (verify)
                    {
                        string arch = Environment.GetEnvironmentVariable("build.arch");
                        if (arch != null && arch == "x86")
                        {
                            string objRoot = Environment.GetEnvironmentVariable("OBJECT_ROOT");
                            string sdxRoot = Environment.GetEnvironmentVariable("SDXROOT");
                            string altDir = Environment.GetEnvironmentVariable("build.type");
                            if (objRoot != null && sdxRoot != null && altDir != null)
                            {
                                string source = objRoot + "\\windows\\DevTest\\WCP\\obj" + altDir + "\\i386\\" + masterFile;
                                string target = sdxRoot + "\\" + ((DrtFlowBase)DRT).DrtSDDirectory + "Masters\\" + masterFileName;
                                System.Diagnostics.Process.Start("cmd", "/C copy " + source + " " + target);
                            }
                        }
                    }
                    break;

                default: // Skip it
                    break;
            }
        }

        /// <summary>
        /// Dump content to file.
        /// </summary>
        /// <param name="dumpFile">Full path to dump file.</param>
        /// <param name="append">Append to existing file.</param>
        /// <param name="verify">Verify content.</param>
        private void DumpToFile(string dumpFile, bool append, bool verify)
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
                DRT.Assert(false, "Aborting dump process due to failure to open dump file: {0}", dumpFile);
                return;
            }
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            // Initialize writer
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            DumpContent(writer);
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// Compare test dump file with master dump file.
        /// </summary>
        /// <param name="masterFile">Full path to master dump file.</param>
        /// <param name="testFile">Full path to test dump file.</param>
        /// <returns>'true' if dump files match, 'false' otherwise.</returns>
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
                DRT.Assert(false, "Failed to compare dumps: {0}.", e.Message);
            }
            return result;
        }
    }
}
