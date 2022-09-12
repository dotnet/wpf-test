// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description:
//  The class that executes the set of Developer Regression Tests for serialization
//  of DRX classes.
//
//

using DRT;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Navigation;



namespace MS.Internal.WppDrt.EDocsUx
{
    /// <summary>
    ///   This class executes the DRTs that test serialization of DRX classes.
    /// </summary>
    internal class DrtDRXSerialization : DrtBase
    {

        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------
        #region Constructors
        public DrtDRXSerialization() : base()
        {
            DrtName = "DRXSerialization";
            WindowTitle = "DRX Serialization DRT";
            TeamContact = "WPF";
            Contact = "Microsoft";
            Suites = new DrtTestSuite[] {
                new DRXSerializationTestSuite()
                };
        }
        #endregion Constructors

        //------------------------------------------------------
        //
        //  Main method
        //
        //------------------------------------------------------
        #region Main method

        /// <summary>
        ///   Main routine for the DRT that tests serialization of DRX classes.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            return (new DrtDRXSerialization()).Run(args);
        }

        #endregion Main method

        // Print a description of command line arguments.  Derived classes should
        // override this to describe their own arguments, and then call
        // base.PrintOptions() to get the DrtBase description.
        protected override void PrintOptions()
        {
            Console.WriteLine(
               "Usage: DrtDRXSerialization [option ...]\n" +
               "Options:\n" +
               "    -rebaseline  Regenerate the baseline comparison files against which\n" +
               "                   the serialization results are compared (default: off)" +
               "                   This must be run from within razzle."
               );
            base.PrintOptions();
        }
    }

    /// <summary>
    /// The DRXSerializationTestSuite handles the execution of our Serialization test cases.
    /// </summary>
    public sealed class DRXSerializationTestSuite : DrtTestSuite, ISerializationTestCaseHost
    {
        public DRXSerializationTestSuite() : base("DRXSerializationTestSuite")
        {
        }

        /// <summary>
        /// Parse any command-line arguments here
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="option"></param>
        /// <param name="args"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            bool handled = false;

            if (option)
            {
                switch (arg)
                {
                    case "rebaseline":
                        _mode = SerializationTestCaseRunMode.Rebaseline;
                        _baselineDirectory = _baselineOutputDirectory;
                        handled = true;
                        break;

                    default:
                        handled = false;
                        break;
                }
            }

            return handled;
        }

        //------------------------------------------------------
        //
        //  ISerializationTestCaseHost implementation
        //
        //------------------------------------------------------
        #region ISerializationTestCaseHost implementation

        SerializationTestCaseRunMode ISerializationTestCaseHost.Mode
        {
            get { return _mode; }
        }

        string ISerializationTestCaseHost.BaselineDirectory
        {
            get { return _baselineDirectory; }
        }

        #endregion ISerializationTestCaseHost implementation

        public override DrtTest[] PrepareTests()
        {

            // return the lists of tests to run against the tree            
            return new DrtTest[]{                
                new DrtTest( RunAll),                                        
                };
        }

        //------------------------------------------------------
        //
        //  Private methods
        //
        //------------------------------------------------------
        #region Private methods

        private void RunAll()
        {

            int result = (int)RunTests(this);

            if (_mode == SerializationTestCaseRunMode.RunDrt)
            {
                DRT.Assert(result == 0, "FAILED one or more serialization tests.");
            }
        }

        private object RunTests(object arg)
        {
            int result = 0;

            // Define the test cases.
            SerializationTestCaseGroup[] testCaseGroups = {
                new SerializationTestCaseGroup(
                    typeof(DocumentViewer), 
                    _knownDocumentViewerProperties, 
                    new SerializationTestCase[] {
                        new SerializationTestCase(
                            "Default properties", 
                            "DocumentViewerDefaultInput.xaml", 
                            null, 
                            "DocumentViewerDefault.xaml"
                            ),
                        new SerializationTestCase(
                            "Non-default properties",
                            "DocumentViewerNonDefaultInput.xaml", 
                            CompareDocumentViewerProperties, 
                            "DocumentViewerNonDefault.xaml"
                            )
                        }
                    ), 
                };

            if (_mode == SerializationTestCaseRunMode.Rebaseline)
            {
                Console.WriteLine("Regenerating baseline comparison files...");
                Console.WriteLine("Ensure the baseline file you want to rebaseline is opened for edit!");
                Console.WriteLine("Also check that you are executing this from within razzle.");
                Console.WriteLine("After this is complete, you will need to recompile DrtDRXSerialization");
                Console.WriteLine("To test the changes.");
                Console.WriteLine("(if you have any problems please contact DRXDev)");
            }

            // Execute the test cases.
            for (int i = 0; i < testCaseGroups.Length; ++i)
            {
                result += testCaseGroups[i].Run(this);
            }

            return result;
        }

        #region Property comparer delegates.
        private static int CompareDocumentViewerProperties(object obj)
        {
            int result = 0;

            DocumentViewer dv = (DocumentViewer)obj;

            if ((dv.HorizontalOffset != 0.0) ||
                (dv.VerticalOffset != 0.0) ||
                (dv.HorizontalPageSpacing != 20.0) ||
                (dv.VerticalPageSpacing != 20.0) ||
                (dv.ShowPageBorders) ||
                (dv.Zoom != 100.0))
            {
                result = 1;
            }

            return result;
        }
        #endregion Property comparer delegates

        #endregion Private methods

        //------------------------------------------------------
        //
        //  Private fields
        //
        //------------------------------------------------------
        #region Private fields

        private SerializationTestCaseRunMode _mode = SerializationTestCaseRunMode.RunDrt;

        // Directory from which baseline comparison files are read or to which they are
        // written, depending on the run mode. By default, we run in "RunDrt" mode, so
        // files are read from the input directory.
        private string _baselineDirectory = _baselineInputDirectory;

        // Directory from which baseline comparison files are read when running in
        // "RunDrt" (normal) mode.
        private static readonly string _baselineInputDirectory = @"DrtFiles\DRXSerialization";

        // Directory to which baseline comparison files are written when running in
        // "Rebaseline" mode.
        // Default value set using Environment Variable.
        private static readonly string _baselineOutputDirectory = System.Environment.GetEnvironmentVariable("SDXROOT") + 
                 @"\windows\wcp\devtest\drts\DRXSerialization\DrtFiles";
        
        #region Known properties

        // List of known public, R/W properties in each type that are not hidden
        // from serialization.

        // WARNING: These elements of each of these arrays must appear in alphabetical order
        // or the DRT will fail.

        private static readonly string[] _knownDocumentViewerProperties = {                             
            "HorizontalOffset",
            "HorizontalPageSpacing",
            "MaxPagesAcross",
            "ShowPageBorders",
            "VerticalOffset",
            "VerticalPageSpacing",
            "Zoom",
        };

        private static readonly string[] _knownZoomControlProperties = {
            "DocumentType",
            "MaxZoom",
            "MinZoom",
            "PageFit",
            "Zoom",
        };

        #endregion Known properties

        #endregion Private fields
    }
}

