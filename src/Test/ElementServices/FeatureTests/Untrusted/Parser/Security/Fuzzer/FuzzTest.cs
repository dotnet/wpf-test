// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      Storage for Fuzz test settings/parameters
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.IO;
using System.Xml;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Markup;

using Microsoft.Test.Markup;
using Microsoft.Test.Serialization;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Utilities;
using Avalon.Test.CoreUI.Parser;
using System.Diagnostics;

namespace Avalon.Test.CoreUI.Parser.Security
{
    /// <summary>
    /// Storage for Fuzz test settings/parameters
    /// </summary>
    public abstract class FuzzTest
    {
        /// <summary>
        /// Intantiates new FuzzTest objects using test descriptions in the given xml file.
        /// </summary>
        public static List<FuzzTest> LoadTests(string xmlFile)
        {
            if (xmlFile == null)
            {
                throw new ArgumentNullException("xmlFile");
            }

            List<FuzzTest> list = new List<FuzzTest>();
            XmlDocument doc = new XmlDocument();

            // Load xml file.
            doc.Load(xmlFile);

            // Get FuzzTests node.
            XmlNode fuzzTestsNode = doc.SelectSingleNode("FuzzTests");
            if (fuzzTestsNode == null)
            {
                throw new ArgumentException("xmlFile", "XmlElement 'FuzzTests' does not exist in " + xmlFile);
            }

            // Get FuzzTest nodes.
            XmlNodeList fuzzTestNodes = fuzzTestsNode.ChildNodes;
            if (fuzzTestNodes.Count < 1)
            {
                throw new ArgumentException("xmlFile", "Could not find any test definitions under 'FuzzTests'.");
            }

            // For each FuzzTest node, create an FuzzTest instance
            // and add it to the list.
            foreach (XmlNode node in fuzzTestNodes)
            {
                if (!(node is XmlElement))
                    continue;

                XmlElement element = (XmlElement)node;

                string typeName = "Avalon.Test.CoreUI.Parser.Security." + element.Name;
                InternalObject internalObject = InternalObject.CreateInstance(typeof(FuzzTest).Assembly.FullName, typeName, new object[] { element });
                FuzzTest test = (FuzzTest)internalObject.Target;

                list.Add(test);
            }

            return list;
        }

        /// <summary>
        /// Creates an default test object.
        /// </summary>
        public FuzzTest()
        {
            seed = DateTime.Now.Millisecond;
        }

        /// <summary>
        /// Create a test object based on the xml description.
        /// </summary>
        /// <param name="xmlElement">An xml element describing the test.</param>
        public FuzzTest(XmlElement xmlElement)
        {
            XmlAttributeCollection attribs = xmlElement.Attributes;

            if (attribs["SelectFuzzerRandomly"] != null)
                selectFuzzerRandomly = Convert.ToBoolean(attribs["SelectFuzzerRandomly"].Value);

            if (attribs["MaxIterations"] != null)
                _maxFiles = Convert.ToInt32(attribs["MaxIterations"].Value);

            if (attribs["seed"] != null)
                seed = Convert.ToInt32(attribs["seed"].Value);
            else
                seed = DateTime.Now.Millisecond;

            random = new Random(seed);

            this.Log("/seed=" + seed.ToString(), true);
            
            for (XmlNode node = xmlElement.FirstChild; node != null; node = node.NextSibling)
            {
                XmlElement element = node as XmlElement;
                if (element == null)
                {
                    continue;
                }
                _fuzzers.Add(FuzzerBase.Create(element, random));
            }
        }

        /// <summary>
        /// Runs the test using its current options.
        /// </summary>
        public void Run()
        {
            //
            // Clean current directory.
            //
            this.CleanupCore();

            //
            // Loop until we've run the max iterations.  Generate and test files
            // in the Dispatcher. We need to stop and restart the dispatcher periodically
            // to avoid memory leaks. (Objects that are instantiated sometimes are linked
            // to the current dispatcher. If the dispatcher were never recycled, those objects
            // would never be garbage collected.)
            //
            DispatcherOperation op = null;

            while (!_HasReachedMaxIterations())
            {
                op = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(_RunFuzzer), null);
                op.Wait();
            }
        }

        // Check if the maximum number of iterations (the number of generated, 
        // fuzzed, and loaded files) has been reached.
        private bool _HasReachedMaxIterations()
        {
            return (_generatedCount >= _maxFiles) && (_maxFiles >= 0);
        }

        // Callback that executes as a DispatcherOperation.
        // This runs a subset of iterations before returning.
        private object _RunFuzzer(object obj)
        {
            string originalFile = "";
            string fuzzedFile = "";

            _DeleteOldFiles();

            for (int i = 0; i < s_iterationCount && !_HasReachedMaxIterations(); i++)
            {
                // Create xaml
                CoreLogger.LogStatus("\r\nStarting iteration " + _generatedCount++ + "...");

                _persistFiles.Clear();

                try
                {
                    // Create file
                    originalFile = this.CreateFile();
                    _persistFiles.Add(originalFile);

                    string baseFileName = Path.GetFileNameWithoutExtension(originalFile);
                    fuzzedFile = baseFileName + "_fuzzed" + Path.GetExtension(originalFile);
                    testPlanLogger = new IndentLogger(fuzzedFile + ".testplan.log", 2);
                    CoreLogger.LogStatus("\r\nCurrent fuzzed file: " + fuzzedFile + "...");
                    // Fuzz file
                    this.DoFuzz(originalFile, fuzzedFile);
                    _persistFiles.Add(fuzzedFile);
                    string beforeTestFuzzedFile = fuzzedFile + ".before.test";
                    File.Copy(fuzzedFile, beforeTestFuzzedFile, true);
                    _persistFiles.Add(beforeTestFuzzedFile);

                    _persistFiles.Add(testPlanLogger.GetFilename());
                    // Test file
                    _TestFuzzedFileInternal(fuzzedFile);
                   
                }
                catch (Exception ex)
                {
                    this.Log(ex.ToString() + "\r\n", false);
                }
                if (null != testPlanLogger)
                {
                    testPlanLogger.Close();
                }
                _oldFiles.AddRange(_persistFiles);

            }

            Dispatcher.CurrentDispatcher.InvokeShutdown();

            return null;
        }

        // Attempts to delete generated container files
        // that were left from old iterations.
        private void _DeleteOldFiles()
        {
            string[] oldFiles = new string[_oldFiles.Count];
            _oldFiles.CopyTo(oldFiles);
            for (int j = 0; j < oldFiles.Length; j++)
            {
                string file = oldFiles[j];

                try
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                        _oldFiles.Remove(file);
                    }
                }
                catch (IOException)
                {
                    Console.WriteLine("Couldn't delete old temporary file.");
                }
            }
        }

        // Calls the virtual TestFuzzedFile, which performs loading of the 
        // fuzzed file or anything else it wants to do.
        private void _TestFuzzedFileInternal(string fuzzedFile)
        {
            try
            {
                Trace.Listeners.Clear();
                Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
                this.TestFuzzedFile(fuzzedFile);
            }
            catch (Exception ex)
            {
                if (this.IsExceptionOkay(ex))
                {
                    CoreLogger.LogStatus(ex.GetType().Name);
                }
                else
                {
                    _PersistException(ex);
                }
            }
        }

        /// <summary>
        /// Subclasses may override this to do optional cleanup.
        /// For example, a xaml fuzz test might want to remove generated
        /// files from an old run in the same directory.
        /// </summary>
        protected virtual void CleanupCore()
        {
            string[] dirs = Directory.GetDirectories(@".\", savedFailureFilePrefix + "*");
            for (int i = 0; i < dirs.Length; i++)
            {
                Directory.Delete(dirs[i], true);
            }

        }

        /// <summary>
        /// Helper routine that generates a random filename with the given extension.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        protected string GetRandomFileNameWithExtension(string extension)
        {
            string fileName = Path.ChangeExtension("__" + Path.GetRandomFileName(), extension);

            return fileName;
        }

        /// <summary>
        /// Subclasses should override this to do custom fuzzing.
        /// </summary>
        /// <param name="sourceFilePath">The path to an unfuzzed source file.</param>
        /// <param name="destinationFilePath">The path where the fuzzed file should be saved.</param>
        protected abstract void DoFuzz(string sourceFilePath, string destinationFilePath);

        /// <summary>
        /// Subclasses should override this to create their domain-specific file.
        /// </summary>
        protected abstract string CreateFile();

        /// <summary>
        /// Subclasses should override this to test their domain-specific file, given as a stream.
        /// </summary>
        /// <param name="fuzzedFile"></param>
        protected abstract void TestFuzzedFile(string fuzzedFile);

        /// <summary>
        /// Subclasses should override this to determine if the caught exception 
        /// is acceptable or expected for the test of the fuzzed file.
        /// </summary>
        protected abstract bool IsExceptionOkay(Exception exception);

        /// <summary>
        /// Walks to the innnermost nested exception of the given exception.
        /// </summary>
        protected Exception GetInnermostException(Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            return ex;
        }

        // Saves the given exception to a new file.
        private void _PersistException(Exception ex)
        {
            CoreLogger.LogStatus(ex.GetType().Name + " caught!!", ConsoleColor.Red);

            string dirPath = @".\" + savedFailureFilePrefix + (_failureCount++).ToString();
            Directory.CreateDirectory(dirPath);
            dirPath += "\\";

            // Save exception to text file.
            this.Log("\r\n\r\n*** Failure", false);
            this.Log(dirPath + "\r\n", false);
            testPlanLogger.Log("<Exception>");
            testPlanLogger.Log(ex.ToString());
            testPlanLogger.Log("</Exception>");
            if (ex.InnerException != null)
            {
                testPlanLogger.Log("<InnerException>");
                testPlanLogger.Log(ex.InnerException.ToString());
                testPlanLogger.Log("</InnerException>");
            }
            testPlanLogger.Close();

           
            // Copy every file in PersistFiles to the new failure directory.
            for (int i = 0; i < _persistFiles.Count; i++)
            {
                string fileName = _persistFiles[i];

                File.Copy(fileName, dirPath + fileName);

                this.Log(fileName, false);
            }

            this.Log(ex.ToString() + "\r\n\r\n", false);
        }

        /// <summary>
        /// Enables convenient logging of test parameters to facilitate debugging.
        /// For example, the seed could be persisted so a test may be run again.
        /// </summary>
        protected void Log(string str, bool shouldCreate)
        {
            FileMode fileMode = FileMode.Append;

            if (shouldCreate)
                fileMode = FileMode.Create;

            FileStream fileStream = File.Open(s_logPath, fileMode, FileAccess.Write, FileShare.Read);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.WriteLine(str);
            streamWriter.Close();
        }

        /// <summary>
        /// Directs the fuzz test to choose supported fuzzers randomly. This property
        /// enables a single test description to run all fuzzers.
        /// </summary>
        protected bool selectFuzzerRandomly = false;

        /// <summary>
        /// The seed used for all random number generation. Test descriptions may
        /// specify a seed to cause the same fuzzing to occur more than once.
        /// </summary>
        protected int seed = -1;
        
        /// <summary>
        /// The objects that perform the fuzzing.
        /// </summary>
        public List<FuzzerBase> Fuzzers
        {
            get { return _fuzzers; }
        }

        /// <summary>
        /// Names of files that should be persisted in case of failure.
        /// </summary>
        protected List<string> PersistFiles
        {
            get { return _persistFiles; }
        }

        /// <summary>
        /// Prefix used for the filenames of fuzzed files that are saved after failing.
        /// </summary>
        protected static readonly string savedFailureFilePrefix = "__failedFuzz";

        /// <summary>
        /// Global random number generator.
        /// </summary>
        protected readonly Random random;

        /// <summary>
        /// Logs XML representation of sequence of testing actions attempted.
        /// </summary>
        protected IndentLogger testPlanLogger;


        private int _failureCount = 0;
        private int _maxFiles = 1;
        private static readonly string s_logPath = "__fuzzLog.txt";
        private static int s_iterationCount = 5;
        private int _generatedCount = 0;
        private List<FuzzerBase> _fuzzers = new List<FuzzerBase>();
        private List<string> _persistFiles = new List<string>();
        private List<string> _oldFiles = new List<string>();
    }
}

