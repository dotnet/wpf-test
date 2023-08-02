// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Common functionality for layout test suites.
//
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Common functionality for layout test suites.
    // ----------------------------------------------------------------------
    internal abstract class LayoutSuite : DrtTestSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        protected LayoutSuite(string suiteName) : base(suiteName)
        {
            _error = new System.Text.StringBuilder();
        }

        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        static LayoutSuite()
        {
            _assembly = System.Reflection.Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
            Type typeLayoutDump = _assembly.GetType("MS.Internal.LayoutDump");
            _miDumpLayout = typeLayoutDump.GetMethod("DumpLayoutTree", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            _typeFlowDocumentView = _assembly.GetType("MS.Internal.Documents.FlowDocumentView");
        }

        // ------------------------------------------------------------------
        // Retrieve IContentHost from FDSV
        // ------------------------------------------------------------------
        internal static IContentHost GetIContentHost(FlowDocumentScrollViewer fdsv, DrtBase drt)
        {
            IContentHost ich = null;
            DependencyObject fdv = drt.FindVisualByType(_typeFlowDocumentView, fdsv);
            if (fdv != null && VisualTreeHelper.GetChildrenCount(fdv) > 0)
            {
                ich = VisualTreeHelper.GetChild(fdv, 0) as IContentHost;
            }
            return ich;
        }

        // ------------------------------------------------------------------
        // Retrieve IContentHost from FDSV
        // ------------------------------------------------------------------
        protected IContentHost GetIContentHost(FlowDocumentScrollViewer fdsv)
        {
            return GetIContentHost(fdsv, DRT);
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

            TextElement.SetFontFamily(_contentRoot, new FontFamily(new Uri("pack://application:,,,/", UriKind.Absolute), "./DrtFiles/Fonts/#Arial"));
            TextElement.SetFontSize(_contentRoot, 10.0 * 96.0 / 72.0);
            TextElement.SetFontStyle(_contentRoot, FontStyles.Normal);
            TextElement.SetFontStretch(_contentRoot, FontStretches.Normal);
            TextElement.SetFontWeight(_contentRoot, FontWeights.Normal);

            DRT.Show(root);

            // Return the lists of tests to run against the tree
            return CreateTests();
        }

        // ------------------------------------------------------------------
        // Create collection of tests.
        // ------------------------------------------------------------------
        protected abstract DrtTest[] CreateTests();

        // ------------------------------------------------------------------
        // Called when the suite is completed.
        // ------------------------------------------------------------------
        public override void ReleaseResources()
        {
            string error = _error.ToString();
            DRT.Assert(String.IsNullOrEmpty(error), "\r\n" + error);
        }

        // ------------------------------------------------------------------
        // Load content from xaml file.
        // ------------------------------------------------------------------
        protected void LoadContentFromXaml()
        {
            UIElement content = LoadFromXaml(this.TestName + ".xaml") as UIElement;
            DRT.Assert(content != null, "Expecting UIElement as root.");
            _contentRoot.Child = content;
        }

        // ------------------------------------------------------------------
        // Load from xaml file.
        // ------------------------------------------------------------------
        protected object LoadFromXaml(string xamlFile)
        {
            object content = null;
            System.IO.Stream stream = null;
            try
            {
                stream = System.IO.File.OpenRead(DrtFilesDirectory + xamlFile);
                content = System.Windows.Markup.XamlReader.Load(stream);
            }
            finally
            {
                // done with the stream
                if (stream != null)
                {
                    stream.Close();
                }
            }
            DRT.Assert(content != null, "Failed to load xaml file '{0}'", this.DrtFilesDirectory + xamlFile);
            return content;
        }

        // ------------------------------------------------------------------
        // Dump layout details.
        // ------------------------------------------------------------------
        protected void DumpLayoutTree(bool append, bool verify)
        {
            string masterFile = MasterXmlFileName;
            string testFile = TestXmlFileName;

            switch (((DrtLayoutBase)DRT).DumpMode)
            {
                case DrtLayoutBase.DumpModeType.Drt:
                    DumpLayoutTree(_contentRoot.Child, testFile, append);
                    if (verify && !CompareDumps(masterFile, testFile))
                    {
                        _error.AppendLine(String.Format("\t{0}: Layout dump failed. Run '{1} -suite {2} -dump diff' to see differences.", this.TestName, AppDomain.CurrentDomain.FriendlyName, this.Name));
                    }
                    break;

                case DrtLayoutBase.DumpModeType.Diff:
                    DumpLayoutTree(_contentRoot.Child, testFile, append);
                    if (verify && !CompareDumps(masterFile, testFile))
                    {
                        DiffDumps(masterFile, testFile);
                    }
                    break;

                case DrtLayoutBase.DumpModeType.Update:
                    DumpLayoutTree(_contentRoot.Child, masterFile, append);
                    break;
            }
        }

        // ------------------------------------------------------------------
        // OpenDumpFile
        //
        //      dumpFile - full path to dump file
        //      append - append dump to existing file?
        // ------------------------------------------------------------------
        protected XmlTextWriter OpenDumpFile(string dumpFile, bool append)
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
                //  It has to do
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
                return null;
            }
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;

            return writer;
        }


        // ------------------------------------------------------------------
        // DumpHostedElement
        //
        //      name - Name of hosted element
        //      type - Type of hosted element
        //      contentHost - ContehtHost to dump out to
        //      source - source
        //      writer - Writer to dump out to
        // ------------------------------------------------------------------
        protected void DumpHostedElement(string name, Type type, IContentHost contentHost, DependencyObject source, XmlTextWriter writer)
        {
            ContentElement element = (ContentElement) DRT.FindElementByID(name, source);

            DRT.Assert(element.GetType() == type);

            ReadOnlyCollection<Rect> rectangles = contentHost.GetRectangles(element);

            writer.WriteStartElement("Element");
            writer.WriteAttributeString("Name", name);

            foreach(Rect rect in rectangles)
            {
                writer.WriteStartElement("Rectangle");
                writer.WriteAttributeString("Left",   rect.Left.ToString  ("F", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Top",    rect.Top.ToString   ("F", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Width",  rect.Width.ToString ("F", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Height", rect.Height.ToString("F", CultureInfo.InvariantCulture));
                writer.WriteEndElement();
            }

            writer.WriteEndElement();           
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
            XmlTextWriter writer = OpenDumpFile(dumpFile, append);

            _miDumpLayout.Invoke(null, new object[] { writer, "UIElmentDump", root });

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
                _error.AppendLine(String.Format("\t{0}: Failed to compare dumps: {1}.", this.TestName, e.Message));
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
        protected void VerifyLayoutCreate()
        {
            DumpLayoutTree(false, false);
        }

        // ------------------------------------------------------------------
        // Verify content and append to dump file.
        // ------------------------------------------------------------------
        protected void VerifyLayoutAppend()
        {
            DumpLayoutTree(true, false);
        }

        // ------------------------------------------------------------------
        // Verify content and finalize dump file.
        // ------------------------------------------------------------------
        protected void VerifyLayoutFinalize()
        {
            DumpLayoutTree(true, true);
        }


        // ------------------------------------------------------------------
        // Verify content and finalize dump file.
        // ------------------------------------------------------------------
        protected void VerifyLayoutCreateAndFinalize()
        {
            DumpLayoutTree(false, true);
        }

        // ------------------------------------------------------------------
        // Unique name of the test.
        // ------------------------------------------------------------------
        protected string TestName
        {
            get { return this.Name + _testName; }
        }

        // ------------------------------------------------------------------
        // Location of all DRT related files.
        // ------------------------------------------------------------------
        protected virtual string DrtFilesDirectory
        {
            get { return DRT.BaseDirectory + "DrtFiles\\FlowLayout\\"; }
        }

        // ------------------------------------------------------------------
        // The name of the current test.
        // ------------------------------------------------------------------
        protected string _testName;

        // ------------------------------------------------------------------
        // Error string.
        // ------------------------------------------------------------------
        protected System.Text.StringBuilder _error;

        // ------------------------------------------------------------------
        // Placeholder for content.
        // ------------------------------------------------------------------
        protected Border _contentRoot;

        // ------------------------------------------------------------------
        // Method info for dump layout function.
        // ------------------------------------------------------------------
        protected static System.Reflection.MethodInfo _miDumpLayout;

        // ------------------------------------------------------------------
        // Type for FlowDocumentView.
        // ------------------------------------------------------------------
        protected static Type _typeFlowDocumentView;

        // ------------------------------------------------------------------
        // Framework assembly.
        // ------------------------------------------------------------------
        protected static System.Reflection.Assembly _assembly;
        
        // ------------------------------------------------------------------
        // Master XML file name
        // ------------------------------------------------------------------
        protected string MasterXmlFileName
        {
            get
            {
                Version ver = Environment.OSVersion.Version;
                if (ver.Major > 6 || (ver.Major == 6 && ver.Minor > 1))
                {
                    string MasterPath = DrtFilesDirectory + "Win8\\Master" + this.TestName + ".xml";
                    if (File.Exists(MasterPath))
                        return MasterPath;
                }
                return DrtFilesDirectory + "Master" + this.TestName + ".xml";
            }
        }

        // ------------------------------------------------------------------
        // Test XML file name
        // ------------------------------------------------------------------
        protected string TestXmlFileName
        {
            get { return DrtFilesDirectory + "Test" + this.TestName + ".xml"; }
        }
    }
}
