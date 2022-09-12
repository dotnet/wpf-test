// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//#define LayoutDumpDebug //  un-comment this to enable "traditional" layout dump logging.

//
//
//
// Description: Common functionality for layout test suites.
//
//

using System;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Common functionality for layout test suites.
    // ----------------------------------------------------------------------
    internal abstract class DrtSelectionSuite : DrtTestSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        protected DrtSelectionSuite(string testName, string contactName) : base(testName)
        {
            Contact = contactName;
        }

        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        static DrtSelectionSuite()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
            s_typeITextContainer = assembly.GetType("System.Windows.Documents.ITextContainer");
            s_typeITextView = assembly.GetType("System.Windows.Documents.ITextView");
            s_typeTextBoxBase = assembly.GetType("System.Windows.Controls.Primitives.TextBoxBase");

            //  disabling backgound calc
            Type typeBackgroundFormatInfo = assembly.GetType("MS.Internal.PtsHost.BackgroundFormatInfo");
            System.Reflection.FieldInfo fieldInfo = typeBackgroundFormatInfo.GetField("_isBackgroundFormatEnabled", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            fieldInfo.SetValue(null, false);

#if LayoutDumpDebug
            Type typeLayoutDump = assembly.GetType("MS.Internal.LayoutDump");
            s_miDumpLayout = typeLayoutDump.GetMethod("DumpLayoutTree", BindingFlags.Static | BindingFlags.NonPublic);
#endif
        }

        // ------------------------------------------------------------------
        // Initialize tests.
        // ------------------------------------------------------------------
        public override DrtTest[] PrepareTests()
        {
            // Initialize the suite here. This includes loading the tree.
            double drtCanvasWidth = 800;
            double drtCanvasHeight = 600;

            _contentRoot = new Border();
            _contentRoot.Width = drtCanvasWidth;
            _contentRoot.Height = drtCanvasHeight;
            
            _selectionRenderer = new SelectionRenderer();
            _selectionRenderer.Width = drtCanvasWidth;
            _selectionRenderer.Height = drtCanvasHeight;

            Grid root = new Grid();
            root.Background = Brushes.White;
            root.Children.Add(_contentRoot);
            root.Children.Add(_selectionRenderer);
            DRT.Show(root);

            // Prepare the lists of tests to run against the tree
            DrtTest verifySelectionCreate = new DrtTest(VerifySelectionCreate);
            DrtTest verifySelectionAppend = new DrtTest(VerifySelectionAppend);
            DrtTest verifySelectionFinalize = new DrtTest(VerifySelectionFinalize);

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
                    if      (i == 1)                                    testsWithVerification[i] = verifySelectionCreate;
                    else if (i == (testsWithVerification.Length - 1))   testsWithVerification[i] = verifySelectionFinalize;
                    else                                                testsWithVerification[i] = verifySelectionAppend;
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
            ClearSelectionGeometry();
        
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

            // Set a standard Font not based on system metrics
            TextElement.SetFontFamily(_contentRoot, new FontFamily(new Uri("pack://application:,,,/", UriKind.Absolute), "#Arial"));
            TextElement.SetFontSize(_contentRoot, 10.0 * 96.0 / 72.0);
            TextElement.SetFontStyle(_contentRoot, FontStyles.Normal);
            TextElement.SetFontWeight(_contentRoot, FontWeights.Normal);
        }

        // ------------------------------------------------------------------
        //  Helper to invoke GetTightBoundingGeometryFromTextPositions on 
        //  ITextView associated with element.
        // ------------------------------------------------------------------
        protected void CalcualteSelectionGeometry(IServiceProvider element, double percentStartPosition, double percentEndPosition)
        {
            DRT.Assert(element != null);
            DRT.Assert(0 <= percentStartPosition && percentStartPosition < percentEndPosition && percentEndPosition <= 100);

            //
            //  calculate and instantiate ITextPointers for the selection range
            //
            object iTextContainer = element.GetService(s_typeITextContainer);

            int cch = (int)s_typeITextContainer.InvokeMember(
                "SymbolCount",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                null,
                iTextContainer,
                new object[] { });

            int cpStart = (int)(percentStartPosition * cch * 0.01);
            int cpEnd = (int)(percentEndPosition * cch * 0.01);

            DRT.Assert(0 <= cpStart && cpStart < cpEnd && cpEnd <= cch,
                "percentStartPosition and / or percentEndPosition values must be changed to produce sensible CP's");

            object iTextPointerStart = s_typeITextContainer.InvokeMember(
                "CreatePointerAtOffset",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null,
                iTextContainer,
                new object[] { cpStart, System.Windows.Documents.LogicalDirection.Forward });

            object iTextPointerEnd = s_typeITextContainer.InvokeMember(
                "CreatePointerAtOffset",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null,
                iTextContainer,
                new object[] { cpEnd, System.Windows.Documents.LogicalDirection.Forward });

            //
            //  call ITextView.CalcualteSelectionGeometry()
            //                
            object iTextView = element.GetService(s_typeITextView);

            _geometry = (Geometry)s_typeITextView.InvokeMember(
                "GetTightBoundingGeometryFromTextPositions",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null,
                iTextView,
                new object[] { iTextPointerStart, iTextPointerEnd });

            DRT.Assert(_geometry != null);

            if (!(_geometry is PathGeometry))
            {
                _geometry = Geometry.Combine(new PathGeometry(), _geometry, GeometryCombineMode.Union, null, 1e-4, ToleranceType.Absolute);
            }

            if (_geometry != null)
            {
                _geometry.Transform = CalculateTranslateTransform(_contentRoot, element);
            }

            _selectionRenderer.SetTestSelectionGeometry(_geometry);

            if (((DrtSelectionBase)DRT).DumpMode == DrtSelectionBase.DumpModeType.View)
            {
                string masterFile = DrtFilesDirectory + "Master" + this.TestName + ".txt";
                ShowSelectionsDiff(_geometry, masterFile);
            }
        }

        // ------------------------------------------------------------------
        //  Helper to invoke GetTightBoundingGeometryFromTextPositions on 
        //  ITextView associated with TextBoxBase element.
        // ------------------------------------------------------------------
        protected void CalcualteSelectionGeometry(TextBoxBase element, double percentStartPosition, double percentEndPosition)
        {
            DRT.Assert(element != null);

            object serviceProvider = s_typeTextBoxBase.InvokeMember(
                "RenderScope",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, 
                null, 
                element, 
                new object[] {});

            DRT.Assert(serviceProvider is IServiceProvider);

            CalcualteSelectionGeometry((IServiceProvider)serviceProvider, percentStartPosition, percentEndPosition);
        }

        protected void ClearSelectionGeometry()
        {
            _selectionRenderer.ClearSelectionGeometry();
        }

        // ------------------------------------------------------------------
        // Dump selection details.
        // ------------------------------------------------------------------
        protected void DumpSelection(bool append, bool verify)
        {
            string masterFile = DrtFilesDirectory + "Master" + this.TestName + ".txt";
            string testFile = DrtFilesDirectory + "Test" + this.TestName + ".txt";

            if (!append) { _appendCounter = 0; }
            _appendCounter++;

            if (verify) { ClearSelectionGeometry(); }

            switch (((DrtSelectionBase)DRT).DumpMode)
            {
                case DrtSelectionBase.DumpModeType.Drt:
                    DumpSelection(_geometry, testFile, append);
                    if (verify && !CompareDumps(masterFile, testFile))
                    {
                        DRT.Assert(false, "{0}: Selection dump failed. Run '{1} -suite {2} -diff' to see differences.", this.TestName, AppDomain.CurrentDomain.FriendlyName, this.Name);
                    }
                    break;

                case DrtSelectionBase.DumpModeType.Diff:
                    DumpSelection(_geometry, testFile, append);
                    if (verify && !CompareDumps(masterFile, testFile))
                    {
                        DiffDumps(masterFile, testFile);
                    }
                    break;

                case DrtSelectionBase.DumpModeType.Dump:
                    DumpSelection(_geometry, masterFile, append);
                    break;

                case DrtSelectionBase.DumpModeType.View:
                    System.Threading.Thread.Sleep(((DrtSelectionBase)DRT).ViewDelay); 
                    break;
            }
        }

        // ------------------------------------------------------------------
        // Dump content of the specified layout tree.
        //
        //      geometry - geometry
        //      dumpFile - full path to dump file
        //      append - append dump to existing file?
        // ------------------------------------------------------------------
        private void DumpSelection(Geometry geometry, string dumpFile, bool append)
        {
            StreamWriter sw = null;

            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // This is hacky, but needed to unblock random failures of File.Open
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            bool opened = false;
            int cFailed = 0;
            while (!opened && cFailed < 10)
            {
                try
                {
                    if (!append && File.Exists(dumpFile))
                    {
                        File.Delete(dumpFile);
                    }

                    sw = File.AppendText(dumpFile);
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

            if (geometry != null)
            {
                sw.WriteLine("Dump # " + _appendCounter);
                string str = ((IFormattable)geometry).ToString(c_nformat, CultureInfo.InvariantCulture);
                sw.WriteLine(str);
            }
            sw.Flush();
            sw.Close();
        }

        // ------------------------------------------------------------------
        // Visualizes seleciton geometry difference.
        // ------------------------------------------------------------------
        private void ShowSelectionsDiff(Geometry geometry, string masterFile)
        {
            StreamReader sr = null;

            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // This is hacky, but needed to unblock random failures of File.Open
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            bool opened = false;
            int cFailed = 0;
            while (!opened && cFailed < 10)
            {
                try
                {
                    if (!File.Exists(masterFile))
                    {
                        //  no master file exists, thus nothing to compare
                        return;
                    }

                    sr = File.OpenText(masterFile);
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
                DRT.Assert(false, "{0}: Aborting dump process due to failure to open master file: {1}", this.TestName, masterFile);
                return;
            }
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            if (geometry != null)
            {
                //  there are two lines for each geometry dump: 
                //  Dump # 1
                //  F1M0.00,0.00L47.43,0.00 47.43,15.33 0.00,15.33 0.00,0.00z
                //  Dump # 2
                //  F1M0.00,0.00L47.43,0.00 47.43,15.33 0.00,15.33 0.00,0.00z

                //  so skip lines in pairs until we are at the right line...
                for (int i = 1; i < _appendCounter; ++i)
                {
                    sr.ReadLine();
                    sr.ReadLine();
                }
                
                //  skip "Dump # 153" line too
                sr.ReadLine();
                
                //  try to parse the current line into Geometry
                string masterGeometryString = sr.ReadLine();
                Geometry masterGeometry = null;
                
                try
                {
                    masterGeometry = (Geometry)(Geometry.Parse(masterGeometryString));
                    DRT.Assert(masterGeometry != null);
                }
                catch (Exception)
                {
                }
                
                //  convert the given geometry to string
                string currentGeometryString = ((IFormattable)geometry).ToString(c_nformat, CultureInfo.InvariantCulture);

                if (masterGeometryString != currentGeometryString)
                {
                    _selectionRenderer.SetDiffSelectionGeometry(masterGeometry, geometry);
                }
            }
            sr.Close();
        }

        private Transform CalculateTranslateTransform(object parent, object child)
        {
            Transform result = null;

            GeneralTransform transform = null;
            Visual vParent = parent as Visual;
            Visual vChild = child as Visual;

            if (vParent != null && vChild != null)
            {
                transform = vChild.TransformToAncestor(vParent);
            }

            if (transform != null)
            {
                Point ancestorOffset = new Point();
                transform.TryTransform(ancestorOffset, out ancestorOffset);
                result = new TranslateTransform(ancestorOffset.X, ancestorOffset.Y);
            }

            return (result);
        }

#if LayoutDumpDebug
        // ------------------------------------------------------------------
        // Dump layout details.
        // ------------------------------------------------------------------
        protected void DumpLayoutTree(bool append, bool verify)
        {
            string masterFile = DrtFilesDirectory + "Master" + this.TestName + ".xml";
            string testFile = DrtFilesDirectory + "Test" + this.TestName + ".xml";

            switch (((DrtSelectionBase)DRT).DumpMode)
            {
                case DrtSelectionBase.DumpModeType.Drt:
                    DumpLayoutTree(_contentRoot.Child, testFile, append);
                    if (verify && !CompareDumps(masterFile, testFile))
                    {
                        DRT.Assert(false, "{0}: Layout dump failed. Run '{1} -suite {2} -diff' to see differences.", this.TestName, AppDomain.CurrentDomain.FriendlyName, this.Name);
                    }
                    break;

                case DrtSelectionBase.DumpModeType.Diff:
                    DumpLayoutTree(_contentRoot.Child, testFile, append);
                    if (verify && !CompareDumps(masterFile, testFile))
                    {
                        DiffDumps(masterFile, testFile);
                    }
                    break;

                case DrtSelectionBase.DumpModeType.Dump:
                    DumpLayoutTree(_contentRoot.Child, masterFile, append);
                    break;

                case DrtSelectionBase.DumpModeType.View:
                    System.Threading.Thread.Sleep(((DrtSelectionBase)DRT).ViewDelay);
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
            s_miDumpLayout.Invoke(null, new object[] { writer, "UIElmentDump_" + (++_dumpCounter), root });
            writer.Flush();
            writer.Close();
        }
#endif // LayoutDumpDebug

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
        private void VerifySelectionCreate()
        {
#if LayoutDumpDebug
            DumpLayoutTree(false, false);
#else
            DumpSelection(false, false);
#endif
        }

        // ------------------------------------------------------------------
        // Verify content and append to dump file.
        // ------------------------------------------------------------------
        private void VerifySelectionAppend()
        {
#if LayoutDumpDebug
            DumpLayoutTree(true, false);
#else
            DumpSelection(true, false);
#endif
        }

        // ------------------------------------------------------------------
        // Verify content and finalize dump file.
        // ------------------------------------------------------------------
        private void VerifySelectionFinalize()
        {
#if LayoutDumpDebug
            DumpLayoutTree(true, true);
#else
            DumpSelection(true, true);
#endif
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
            get { return DRT.BaseDirectory + "DrtFiles\\Selection\\"; }
        }

        // ------------------------------------------------------------------
        // Placeholder for content.
        // ------------------------------------------------------------------
        protected Border ContentRoot
        {
            get { return (_contentRoot); }
        }

        // ------------------------------------------------------------------
        // Private Data
        // ------------------------------------------------------------------
        private Border _contentRoot;
        private SelectionRenderer _selectionRenderer;
        private Geometry _geometry;
        private static readonly Type s_typeITextContainer;
        private static readonly Type s_typeITextView;
        private static readonly Type s_typeTextBoxBase;
        private int _appendCounter;
        private const string c_nformat = "0.00;-0.00;0.00";

#if LayoutDumpDebug
        private static readonly MethodInfo s_miDumpLayout;
        private int _dumpCounter;
#endif 

        // ------------------------------------------------------------------
        // Helper Selection Renderer class
        // ------------------------------------------------------------------
        private class SelectionRenderer : FrameworkElement
        {
            protected override void OnRender(DrawingContext dc)
            {
                double opacity = 0.2;
                double penStroke = 1.0;
                Brush selectionBrush;
                Pen selectionOutlinePen;
                
                if (_testSelectionGeometry != null)
                {
                    Color c = Color.FromArgb(0xff, 0x33, 0x33, 0xff);
                    selectionBrush = new SolidColorBrush(c);
                    selectionBrush.Opacity = opacity;
                    selectionBrush.Freeze();
                    selectionOutlinePen = new Pen(new SolidColorBrush(c), penStroke);
                    selectionOutlinePen.Freeze();
                    dc.DrawGeometry(selectionBrush, selectionOutlinePen, _testSelectionGeometry);
                }

                if (_expectedSelectionGeometry != null)
                {
                    Color c = Color.FromArgb(0xff, 0x33, 0xff, 0x33);
                    selectionBrush = new SolidColorBrush(c);
                    selectionBrush.Opacity = opacity;
                    selectionBrush.Freeze();
                    selectionOutlinePen = new Pen(new SolidColorBrush(c), penStroke);
                    selectionOutlinePen.Freeze();
                    dc.DrawGeometry(selectionBrush, selectionOutlinePen, _expectedSelectionGeometry);
                }

                if (_actualSelectionGeometry != null)
                {
                    Color c = Color.FromArgb(0xff, 0xff, 0x33, 0x33);
                    selectionBrush = new SolidColorBrush(c);
                    selectionBrush.Opacity = opacity;
                    selectionBrush.Freeze();
                    selectionOutlinePen = new Pen(new SolidColorBrush(c), penStroke);
                    selectionOutlinePen.Freeze();
                    dc.DrawGeometry(selectionBrush, selectionOutlinePen, _actualSelectionGeometry);
                }

            }

            internal void SetTestSelectionGeometry(Geometry g)
            {
                _testSelectionGeometry = g;
                _expectedSelectionGeometry = null;
                _actualSelectionGeometry = null;
                InvalidateVisual();
            }

            internal void SetDiffSelectionGeometry(Geometry expected, Geometry actual)
            {
                _expectedSelectionGeometry = expected;
                _actualSelectionGeometry = actual;
                _testSelectionGeometry = null;
                InvalidateVisual();
            }
            
            internal void ClearSelectionGeometry()
            {
                _expectedSelectionGeometry = null;
                _actualSelectionGeometry = null;
                _testSelectionGeometry = null;
                InvalidateVisual();
            }
            
            private Geometry _testSelectionGeometry;        //  whatever geometry we got during normal drt run
            private Geometry _expectedSelectionGeometry;    //  expected master geometry - used when showing diff
            private Geometry _actualSelectionGeometry;      //  actual bad geometry - used when showing diff
        }
    }
}
