// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Packaging;
using System.Threading;
using System.Xml;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls;

using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Serialization;
using Avalon.Test.CoreUI.Common;

using Microsoft.Test.Threading;
using Microsoft.Test.Win32;
using Microsoft.Test.Logging;
using Microsoft.Test.Serialization;
using Microsoft.Test.Windows;
using Microsoft.Test.Markup;
using Microsoft.Test.Integration;

namespace Avalon.Test.Xaml.Markup
{
    /// <summary>
    ///     
    ///</summary>
    /// <remarks>
    /// </remarks>
    public class XamlTestRunner
    {
        #region Constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        public XamlTestRunner() { }
        #endregion Constructor

        /// <summary>
        /// Common entry method of TestExtenderTests
        /// </summary>
        /// <param name="item"></param>
        public void EntryMethod(ContentItem item)
        {
            if (item == null) throw new ArgumentNullException();
                
            
            ActionForXamlInfo actionForXamlInfo = null;

            if (item.Content is String)
            {
                string foo = item.Content as string;

                if (String.IsNullOrEmpty(foo))
                {
                    throw new ArgumentException("item.Content cannot be null or empty.");
                }

                actionForXamlInfo = new ActionForXamlInfo();
                actionForXamlInfo.ActionForXaml = foo;
                actionForXamlInfo.XamlFile = GetXamlFile();
            }            
            else if (item.Content is ActionForXamlInfo)
            {
                 actionForXamlInfo = (ActionForXamlInfo)item.Content;
            }
            else
            {
                throw new ArgumentException("item.Content must be a string or ActionForXamlInfo");
            }
            
            
            switch (GetAction(actionForXamlInfo))
            {
                case ActionTypeForXaml.Load:
                    
                    RunLoadCase(actionForXamlInfo);
                    break;

                case ActionTypeForXaml.AsyncLoad:
                case ActionTypeForXaml.AsyncLoadStream:
                case ActionTypeForXaml.AsyncLoadXmlReader:

                    RunAsyncLoadCase(actionForXamlInfo);

                    break;

                case ActionTypeForXaml.CompileVisualBasic:
                case ActionTypeForXaml.CompileCSharp:
                    
                    RunCompileCase(actionForXamlInfo);

                    break;

                case ActionTypeForXaml.TestBamlReaderWriter:

                    RunBamlReaderWriterCase(actionForXamlInfo);
                    break;

                case ActionTypeForXaml.Serialization:

                    SerializationRoundTrip s = new SerializationRoundTrip();
                    s.RunTestCase(actionForXamlInfo);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            if (TestLog.Current != null)
            {
                TestLog.Current.Result = TestResult.Pass;
            }


        }


        static ActionTypeForXaml GetAction(ActionForXamlInfo info)
        {
            ActionTypeForXaml type = (ActionTypeForXaml)Enum.Parse(typeof(ActionTypeForXaml), info.ActionForXaml, true);
            return type;
        }

        static string GetXamlFile()
        {
            string xamlFile = Microsoft.Test.Integration.CommonStorage.Current.Get("FileName") as string;

            if (String.IsNullOrEmpty(xamlFile))
            {
                throw new InvalidOperationException("The FileName stored in CommonStorage is null or empty.");
            }

            if (!File.Exists(xamlFile))
            {
                throw new FileNotFoundException("File cannot be found", xamlFile);
            }

            return xamlFile;

        }


        /// <summary>
        /// It take a file and pass it to the xaml reader.
        /// It requires as input to have a FileName (string) object stored
        /// in the CommonStorage.  
        /// 
        /// </summary>
        static public void RunLoadCase(ActionForXamlInfo info)
        {
            string xamlFile = info.XamlFile;            
            xamlFile = Helper.PreprocessSupportFile(xamlFile);            
            object root = ParserUtil.ParseXamlFile(xamlFile);
            if (root == null)
            {
                GlobalLog.LogEvidence("root was null");
                TestLog.Current.Result = TestResult.Fail;
                throw new Microsoft.Test.TestValidationException("Root cannot be null");
            }
            if ((root as UIElement) != null)
            {
                (new SerializationHelper()).DisplayTree(root as UIElement);
            }
            TestLog.Current.Result = TestResult.Pass;
        }


        #region Public Methods
        /// <summary>
        /// Entry method for the compile-mode test cases.
        /// </summary>
        public void RunCompileCase(ActionForXamlInfo info)
        {
            //// Grab the current TestCaseInfo to get the
            //// xaml file, host type, etc.            
            //string supportingAssembliesString = info.SupportingAssemblies;
            //List<string> supportingAssemblies = SplitString(supportingAssembliesString);

            //if (GetAction(info) == ActionTypeForXaml.CompileCSharp)
            //{
            //    CoreLogger.LogStatus("Compiling Xaml with CSharp target.");
            //    this.RunCompileCase(info.XamlFile, "Application", supportingAssemblies, Languages.CSharp, null, false);
            //}
            //else if (GetAction(info) == ActionTypeForXaml.CompileVisualBasic)
            //{
            //    // Don't compile with VB if the Xaml contains inline code (assumed to be C#)
            //    if (!XamlHasInlineCode(info.XamlFile))
            //    {
            //        CoreLogger.LogStatus("Compiling Xaml with VisualBasic target.");
            //        this.RunCompileCase(info.XamlFile, "Application", supportingAssemblies, Languages.VisualBasic, null, true);
            //    }
            //    else
            //    {
            //        CoreLogger.LogStatus("Skipping VisualBasic compilation, Xaml contains inline code assumed to be C#");
            //    }
            //}
            //else
            //{
            //    throw new Microsoft.Test.TestValidationException("ActionForXaml value '" + info.ActionForXaml + "' is not recognized.");
            //}
        }

        /// <summary>
        /// HACK: Open the input Xaml file and report the existance of x:Code tags.
        /// This should be removed when we have command line parameters/xml.tests tags
        /// that specify the target language.
        /// </summary>
        /// <param name="xamlFile"></param>
        private bool XamlHasInlineCode(string xamlFile)
        {
            xamlFile = Helper.PreprocessSupportFile(xamlFile);

            StreamReader reader = new StreamReader((string)(Environment.CurrentDirectory) + @"\" + xamlFile);
            String s = reader.ReadToEnd();
            reader.Close();

            if (s.Contains("x:Code")) return true;
            return false;
        }

        /// <summary>
        /// Convenience wrapper the cleasn up and handles failures
        /// that occur in the separate process
        /// </summary>
        /// <param name="xamlFile"></param>
        /// <param name="hostType"></param>
        internal void RunCompileCase(string xamlFile, string hostType)
        {
            // RunCompileCase(xamlFile, hostType, null, Languages.CSharp, null, false);
        }

        /// <summary>
        /// Convenience wrapper the cleasn up and handles failures
        /// that occur in the separate process
        /// </summary>
        /// <param name="xamlFile"></param>
        /// <param name="hostType"></param>
        /// <param name="additionalAppMarkup"></param>
        internal void RunCompileCase(string xamlFile, string hostType, string additionalAppMarkup)
        {
           //  RunCompileCase(xamlFile, hostType, null, Languages.CSharp, additionalAppMarkup, false);
        }

        /// <summary>
        /// Convenience wrapper that cleans up and handles failures
        /// that occur in the separate process.
        /// </summary>
        private void RunCompileCase(string xamlFile, string hostType, List<string> supportingAssemblies, Object language, string additionalAppMarkup, bool debugBaml)
        {
            //xamlFile = Helper.PreprocessSupportFile(xamlFile);

            ////
            //// Cleanup old compile directories and files if necessary.
            //// 
            //CompilerHelper compiler = new CompilerHelper();
            //compiler.CleanUpCompilation();
            //compiler.AddDefaults();

            ////
            //// Compile xaml into Avalon app.
            ////    
            //compiler.CompileApp(new string[]{xamlFile}, hostType, null, null, supportingAssemblies, language, additionalAppMarkup, null, null, debugBaml);

            ////
            //// Run compiled Avalon app.
            ////
            //compiler.RunCompiledApp();
        }



        /// <summary>
        /// Entry Method for test cases of type BamlReaderWriter.
        /// It does the following things:
        /// 1. Compiles the given XAML to BAML. 
        /// 2. Creates an object tree from this BAML using LoadBaml()
        /// 3. Uses BamlHelper.CopyBaml to read this BAML using BamlReaderWrapper and 
        ///    create a new BAML using BamlWriterWrapper, with the same contents
        /// 4. Creates an object tree from this second BAML using LoadBaml()
        /// 5. Compares the object trees in Step 2 and Step 4, recursively, to make sure that 
        ///    they are the same. If they are, it passes the test. If not, the test fails.
        /// </summary>
        public void RunBamlReaderWriterCase(ActionForXamlInfo info)
        {
//            // Get the test case. 
//            string xamlFile = info.XamlFile;
//            xamlFile = Helper.PreprocessSupportFile(xamlFile);
//            string hostType = "Application";

//            CompilerHelper compiler = new CompilerHelper();
//            string objPath = Environment.CurrentDirectory + "\\obj\\release\\";
//            string binPath = Path.GetDirectoryName(compiler.CompiledExecutablePath) + "\\";
//            string executableFileName = Path.GetFileName(compiler.CompiledExecutablePath);
//            string origBamlName = Path.GetFileName(compiler.BamlPath);
//            string newBamlName = "__new" + origBamlName;


//            //
//            // Cleanup old compile directories and files if necessary.
//            // 
//            compiler.CleanUpCompilation();
//            compiler.AddDefaults();

//            //
//            // Compile xaml into Avalon app.
//            //
//            CoreLogger.LogStatus("Starting compilation for TestBamlReaderWriter...");
//            // Turn on the last param (debugBaml), which puts debugging info (line,position)
//            // in Baml. We have this to make sure the debugging info records don't mess with
//            // Baml round-tripping.
//            compiler.CompileApp(new string[] { xamlFile }, hostType, null, null, null, Languages.CSharp, null, null, null, true /*debugBaml*/);

//            // Compilation is done. Now proceed
////            Stream origBamlIn = null, newBamlIn = null;
//            Object root = null, root2 = null;
            
//            CreateContext();

//            // Load compiled app in current AppDomain, when needed. 
//            // This is necessary for the parser to find the generated
//            // root type from the compiled executable.            
//            ResolveEventHandler resolveEventHandler = delegate {
//                CoreLogger.LogStatus("Loading assembly " + Path.GetFileName(compiler.CompiledExecutablePath));
//                return Assembly.LoadFile(compiler.CompiledExecutablePath);
//            };
//            AppDomain.CurrentDomain.AssemblyResolve += resolveEventHandler;


//            // Load the first BAML, i.e. the one generated from XAML using compilation
//            root = BamlHelper.LoadBaml(objPath + origBamlName);

//            // Read the BAML using BamlReaderWrapper and write it into another BAML
//            CoreLogger.LogStatus("Converting from one BAML to another");
//            BamlHelper.CopyBaml(objPath + origBamlName, objPath + newBamlName);

//            // Load the new BAML, generated in the previous step
//            root2 = BamlHelper.LoadBaml(objPath + newBamlName);

//            AppDomain.CurrentDomain.AssemblyResolve -= resolveEventHandler;

//            // Compare the trees
//            CoreLogger.LogStatus("Comparing the object trees");

//            // If the roots are pages, insert them in a Window before comparing.
//            // That is necessary because some of Page's properties cannot be read when
//            // it is not a child of a Window.
//            if (root is Page && root2 is Page)
//            {
//                Window window = SerializationHelper.CreateWindow();
//                window.Content = root;

//                window = SerializationHelper.CreateWindow();
//                window.Content = root2;
//            }

//            TreeCompareResult result = TreeComparer.CompareLogical(root, root2);
//            if (CompareResult.Different == result.Result)
//            {
//                throw new Microsoft.Test.TestValidationException("The two object trees are different.");
//            }
     
            // Compare the Baml files node by node.
            /* 
                     Avalon namespace is fixed.
                     If you want to see the bug, uncomment this and run the test.      
            ArrayList diff = BamlHelper.CompareBamlFiles(objPath + origBamlName, objPath + newBamlName);
            if (diff.Count != 0)
            {
                throw new Microsoft.Test.TestValidationException("Original Baml and newly created Baml differ in atleast one Baml node.");
            }
            */ 
        }


        /// <summary>
        /// Entry point for test cases of type AsyncLoad.
        /// 
        /// This method in turn calls another method that does the following:
        /// 1. Load the Xaml file synchronously, and display the resultant tree.
        /// 2. Load the Xaml file async multiple times (using different parameters for 
        ///    a simulated file server). Each time:
        ///        - Check that async is truly happening, by ensuring a minimum number
        ///          of async loops (see below for details).    
        ///        - Display the resultant tree. 
        ///        - Compare the async tree with the sync tree and make sure they are 
        ///          identical.
        /// 
        /// Here's how a Xaml file is loaded async:
        /// 1. Load the Xaml file into a DOM tree
        /// 2. Add x:SynchronousMode="Async" and x:AsyncRecords attributes 
        ///    to the root of the tree
        /// 3. Save the tree into a temporary Xaml file.
        /// 4. Load the temporary Xaml file using XamlReader.Load().
        /// 
        /// Here's how async loop counting works:
        ///    Attach an event handler to the Dispatcher.
        ///    When dispatcher completes any operation, the event handler is called.
        ///    Event handler checks whether the operation was a parser work item for Async parsing. 
        ///    If yes, it keeps a count of such work items (loops).
        /// 
        /// Parser should go thru a reasonably high number of such loops 
        /// before completing Async parsing.
        /// </summary>
        public object RunAsyncLoadCase(object o)
        {

            ActionForXamlInfo info = (ActionForXamlInfo)o;

            DispatcherOperation operation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, 
                new DispatcherOperationCallback(AsyncLoadCaseCallback),
                info);
            Dispatcher.Run();

            return null;
        }



        /// <summary>
        /// It takes a comma-separated list of assembly names, and 
        /// loads those assemblies into the current AppDomain.
        /// Assembly names must be without extensions. 
        /// ".dll" will be appended automatically.
        /// Given assemblies must be present in current directory.
        /// This function uses Assembly.LoadFrom underneath.
        /// </summary>
        /// <param name="supportingAssemblies">Comma-separated list of assembly names</param>
        public static void LoadAssemblies(string supportingAssemblies)
        {
            List<string> assemblyNames = SplitString(supportingAssemblies);

            if (null == assemblyNames) return;

            foreach (string assemblyName in assemblyNames)
            {
                Assembly.LoadFrom(assemblyName + ".dll");  
            }
        }
        #endregion Public Methods

        #region Private Methods
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        private  IntPtr ApplicationFilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Quit the application if the source window is closed.
            if((msg == NativeConstants.WM_CLOSE) )
            {
                _dispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

                handled = true;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// For details, please check the comments on RunAsyncLoadCase(), which calls
        /// this method.
        /// </summary>
        /// <returns></returns>
        private object AsyncLoadCaseCallback(object o)
        {
            ActionForXamlInfo info = (ActionForXamlInfo)o;
            
            const int asyncRecords = 2; // Value for x:AsyncRecords property.
            const string tempFilename = "__AsyncLoadTempFile.xaml";

            // Get the test case info. 

            LoadAssemblies(info.SupportingAssemblies);
            string xamlFile = Helper.PreprocessSupportFile(info.XamlFile);

            //Create a SerializationHelper.
            SerializationHelper helper = new SerializationHelper();

            // Sync load the xaml first and display the resultant tree
            CoreLogger.LogStatus("Synchronous loading the xaml file.");
            int startTime = Environment.TickCount;
            _syncroot = ParserUtil.ParseXamlFile(xamlFile);            
            CoreLogger.LogStatus("Displaying the synchronous tree, and calling the verification routine, if any.");
            helper.DisplayTree(_syncroot, "Synchronous loaded", true /*continueDispatcher*/);
            int endTime = Environment.TickCount;
            CoreLogger.LogStatus("Finished synchronous loading the xaml file.");
            CoreLogger.LogStatus("Time taken for synchronous loading and display = " + (endTime - startTime) + " milliseconds.");

            // Load the Xaml file into a DOM tree
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(xamlFile);

            // Add x:SynchronousMode="Async" and x:AsyncRecords 
            // to the root element
            XmlElement rootElement = doc.DocumentElement;
            rootElement.SetAttribute("SynchronousMode", "http://schemas.microsoft.com/winfx/2006/xaml", "Async");
            rootElement.SetAttribute("AsyncRecords", "http://schemas.microsoft.com/winfx/2006/xaml", asyncRecords.ToString());

            // Save the DOM tree into a temporary Xaml file
            doc.Save(tempFilename);
            GlobalLog.LogFile(tempFilename);

            // Use a simulated file server to stream the temporary Xaml file (async file)
            // through a network, and then parse the Xaml using the client-side 
            // network stream.
            //
            // The simulated server streams some bytes then pauses for some time, then
            // streams few more bytes, then pauses again, and so on.
            // The server uses two user-settable parameters to control the above behavior:
            // 1. Chuck size: Number of bytes streamed at a time.
            // 2. Sleep time: Amount of time to pause between 2 chunks            
            // 
            // We want to parse the Xaml streams having different characteristics, so
            // we choose different values for the simulated server parameters mentioned 
            // above, and then use the resultant streams.
            long tempFileSize = (new FileInfo(tempFilename)).Length;
            SimulatedServer server = new SimulatedServer(tempFilename);
            server.UseRandomChunkSize(10 /*seed*/, (int)tempFileSize / 4 /*maxChunkSize*/);
            server.UseRandomSleepTime(10 /*seed*/, 50 /*maxSleepTime in milliseconds*/);
            CoreLogger.LogStatus("Async loading the xaml file from a file server.");
            Stream stream = server.ServeFile();
            ProcessAsyncStream(stream, info);
            stream.Close();

            // Before doing async again, we need to rebuild the sync tree
            // This is because when the TreeComparer compares sync and async trees, 
            // it queries for different properties on both trees, and in some cases
            // (such as TextBlock.ContentStart), querying a property value changes
            // the values for some other properties. Thus, the same sync tree can't be
            // used again for comparison with another async tree.
            CoreLogger.LogStatus("Synchronous loading the xaml file again.");
            _syncroot = ParserUtil.ParseXamlFile(xamlFile);
            helper.DisplayTree(_syncroot, "Synchronous loaded", true /*continueDispatcher*/);

            server = new SimulatedServer(tempFilename);
            server.UseFixedChunkSize((int)tempFileSize / 10);
            server.UseFixedSleepTime(100 /*milliseconds*/);
            CoreLogger.LogStatus("Async loading the xaml file from a file server.");
            stream = server.ServeFile();
            ProcessAsyncStream(stream, info);
            stream.Close();

            // Rebuild sync tree, for reasons given above.
            CoreLogger.LogStatus("Synchronous loading the xaml file again.");
            _syncroot = ParserUtil.ParseXamlFile(xamlFile);
            helper.DisplayTree(_syncroot, "Synchronous loaded", true /*continueDispatcher*/);

            server = new SimulatedServer(tempFilename);
            server.UseFixedChunkSize((int)tempFileSize);
            server.UseFixedSleepTime(0 /*milliseconds*/);
            CoreLogger.LogStatus("Async loading the xaml file from a file server.");
            stream = server.ServeFile();
            ProcessAsyncStream(stream, info);
            stream.Close();

            // Shut down the dispatcher
            Dispatcher.CurrentDispatcher.InvokeShutdown();

            return null;
        }

        /// <summary>
        /// For details, please check the comments on AsyncLoadCaseCallback(), which 
        /// calls this method.
        /// </summary>
        /// <returns></returns>
        private void ProcessAsyncStream(Stream stream, ActionForXamlInfo info)
        {

            ActionTypeForXaml action = GetAction(info);

            
            const int minLoopsRequired = 4; // Minimum Async loops parser should go through
            s_asyncLoopCount = 1;

            // Every time a DispatcherOperation is completed, our event handler should be called.
            // We must attach the event handler only once, otherwise it gets called multiple 
            // times, resulting in an error in our async loop counting logic.
            // So we keep a flag indicating whether the event handler has already been 
            // attached.
            if (!_dispatcherEventHandlerAttached)
            {
                DispatcherHelper.GetHooks().OperationCompleted += new DispatcherHookEventHandler(IncrementAsyncLoopCount);
                _dispatcherEventHandlerAttached = true;
            }

            // Load the async Xaml coming from the stream
            int startTime = Environment.TickCount;
            XamlReader xamlReader = new XamlReader();
            xamlReader.LoadCompleted += new AsyncCompletedEventHandler(SignalAsyncCompleted);
            _asyncCompleted = false;

    
            if (action == ActionTypeForXaml.AsyncLoadStream)
            {
                CoreLogger.LogStatus("LoadAsync only Stream param");                
                _asyncRoot = xamlReader.LoadAsync(stream);
            }
            else if (action == ActionTypeForXaml.AsyncLoadXmlReader)
            {
                CoreLogger.LogStatus("LoadAsync XmlReader param");                
                XmlTextReader reader = new XmlTextReader(stream);
                _asyncRoot = xamlReader.LoadAsync(reader);
            }
            else
            {
                CoreLogger.LogStatus("LoadAsync Stream and ParserContext param");                                
                ParserContext pc = new ParserContext();
                pc.BaseUri = PackUriHelper.Create(new Uri("siteoforigin://"));

                _asyncRoot = xamlReader.LoadAsync(stream, pc);
            }


            // Display the async tree, and call the verification routine, if any.
            // DisplayTree() doesn't return until the whole tree is built.
            CoreLogger.LogStatus("Displaying the async tree, and calling the verification routine, if any.");
            (new SerializationHelper()).DisplayTree(_asyncRoot, "Asynchronously loaded", true /*continueDispatcher*/);
            int endTime = Environment.TickCount;

            // We know that async loading is complete since DisplayTree() has returned.
            CoreLogger.LogStatus("Finished async loading the xaml file.");
            CoreLogger.LogStatus("Time taken for async loading and display = " + (endTime - startTime) + " milliseconds.");

            // Check whether XamlReader's LoadCompleted event was fired, as it should be.
            if (!_asyncCompleted)
            {
                throw new Microsoft.Test.TestValidationException("Async loading was complete, " +
                    "but XamlReader's LoadCompleted event was not fired.");
            }

            // Check whether Parser went through enough number of Async loops
            if (s_asyncLoopCount < minLoopsRequired)
                throw new Microsoft.Test.TestValidationException("Parser made only " + s_asyncLoopCount + " async loops, while we expected " + minLoopsRequired);
            else
                CoreLogger.LogStatus("Parser made " + s_asyncLoopCount + " async loops.");

            // Compare the async tree with the sync tree 
            // and make sure they are the same.
            CoreLogger.LogStatus("Comparing sync and async trees.");
            TreeCompareResult result = TreeComparer.CompareLogical(_syncroot, _asyncRoot);
            if (CompareResult.Different == result.Result)
            {
                throw new Microsoft.Test.TestValidationException("Failure occurred while comparing object trees. " +
                    "First tree root (synced) is " + _syncroot +
                    ", while the second tree root (async) is " + _asyncRoot);
            }
        }

        /// <summary>
        /// Event handler used by test cases of type AsyncLoad. 
        /// If the completed DispatcherOperation is a parser work item for Async parsing, 
        /// increment the Async loop count by 1.
        /// </summary>
        private void IncrementAsyncLoopCount(object sender, DispatcherHookEventArgs e)
        {
            bool nameFound = false;

            if (System.Environment.Version.Major < 4)
            {
                nameFound = ("System.Windows.Markup.TreeBuilder.Dispatch" == DispatcherHelper.GetNameFromDispatcherOperation(e.Operation));
            }
            else
            {
                nameFound = ("System.Windows.Markup.XamlReader.Dispatch" == DispatcherHelper.GetNameFromDispatcherOperation(e.Operation));
            }

            if (nameFound)
            {
                s_asyncLoopCount++;
                // Serialize the partial tree root

                // This operation takes too much time (about 1 sec) to execute it around 30 times
                // We should call this once in every 10 times
                if (s_asyncLoopCount % 10 == 0)
                {
                    CoreLogger.LogStatus("Serialize the partial tree root.");
                    SerializationHelper.SerializeObjectTree(_asyncRoot);
                }
            }
        }

        /// <summary>
        /// Event handler used by test cases of type AsyncLoad. 
        /// </summary>
        private void SignalAsyncCompleted(Object sender, AsyncCompletedEventArgs e)
        {
            CoreLogger.LogStatus("XamlReader's LoadCompleted event fired.");

            // We didn't cancel async loading.
            if (e.Cancelled)
            {
                throw new Microsoft.Test.TestValidationException("LoadCompleted event's args say async loading " +
                    "was cancelled, but it wasn't.");
            }

            // e.Error stores the exception, if one has occurred.
            if (e.Error != null)
            {
                CoreLogger.LogStatus("Async loading threw the following exception: ");
                throw e.Error;
            }

            // If everything's fine, signal that async loading is completed.
            _asyncCompleted = true;
        }

        #region AsyncLoad related variables
        private static int s_asyncLoopCount = 1; // Parser makes one loop before going async
        private bool _asyncCompleted = false;
        private bool _dispatcherEventHandlerAttached = false;
        private object _asyncRoot = null;
        private object _syncroot = null;
        private delegate void NoParamNoReturnValueCallback();
        #endregion AsyncLoad related variables

        /// <summary>
        /// Splits a comma-delimited string and creates a List of strings from it
        /// </summary>
        /// <param name="s">Comma-delimited string</param>
        /// <returns>List of string parts</returns>
        private static List<string> SplitString(string s)
        {
            if ((null == s) || (String.Empty == s)) return null;

            string[] parts = s.Split(new Char[] { ',' });
            List<string> partsList = new List<string>(parts.Length);
            for (int i = 0; i < parts.Length; i++)
            {
                partsList.Add(parts[i]);
            }
            return partsList;
        }
        #endregion Private Methods

        #region Context
        Dispatcher _dispatcher; 
        
        /// <summary>
        /// Creating Dispatcher
        /// </summary>
        private void CreateContext()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;          
        }        
        #endregion Context

    }
}

