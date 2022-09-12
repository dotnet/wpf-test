// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for standalone application mode. 
//

using System;                               // string
using System.IO;                            // Stream
using System.Windows;                       // RoutedEventArgs
using System.Windows.Controls;              // Border, FlowDocumentPageViewer
using System.Windows.Controls.Primitives;   // DocumentViewerBase
using System.Windows.Documents;             // DocumentPaginator
using System.Windows.Threading;             // DispatcherPriority
using System.Windows.Media;                 // Brushes
using System.Windows.Markup;                // XamlReader

namespace DRT
{
    /// <summary>
    /// Test suite for standalone application mode.
    /// </summary>
    internal sealed class ApplicationTestSuite : DrtTestSuite
    {
        //-------------------------------------------------------------------
        //
        //  Constructors
        //
        //-------------------------------------------------------------------

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="testSuites">Collection of available test suites.</param>
        internal ApplicationTestSuite(DrtTestSuite[] testSuites) : 
            base("Application")
        {
            this.Contact = "Microsoft";
            _testSuites = testSuites;
        }

        #endregion Constructors

        //-------------------------------------------------------------------
        //
        //  Protected Methods
        //
        //-------------------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// Initialize test.
        /// </summary>
        public override DrtTest[] PrepareTests()
        {
            // Main menu
            Menu mainMenu = new Menu();
            MenuItem miViewer = new MenuItem();
            miViewer.Header = "Viewer";
            mainMenu.Items.Add(miViewer);
            MenuItem miContent = new MenuItem();
            miContent.Header = "Content";
            mainMenu.Items.Add(miContent);
            MenuItem miDrt = new MenuItem();
            miDrt.Header = "DRT";
            mainMenu.Items.Add(miDrt);
            _miNextTest = new MenuItem();
            _miNextTest.Header = "";
            _miNextTest.Click += new RoutedEventHandler(HandleNextTestClick);
            _miNextTest.FontStyle = FontStyles.Italic;
            mainMenu.Items.Add(_miNextTest);

            // Viewer menu
            MenuItem mi = new MenuItem();
            mi.Header = "[Empty]";
            mi.Click += new RoutedEventHandler(HandleViewerEmptyClick);
            miViewer.Items.Add(mi);
            mi = new MenuItem();
            mi.Header = "FlowDocumentReader";
            mi.Click += new RoutedEventHandler(HandleViewerFlowDocumentReaderClick);
            miViewer.Items.Add(mi);
            mi = new MenuItem();
            mi.Header = "FlowDocumentPageViewer";
            mi.Click += new RoutedEventHandler(HandleViewerFlowDocumentPageViewerClick);
            miViewer.Items.Add(mi);
            mi = new MenuItem();
            mi.Header = "FlowDocumentScrollViewer";
            mi.Click += new RoutedEventHandler(HandleViewerFlowDocumentScrollViewerClick);
            miViewer.Items.Add(mi);

            // Content menu
            mi = new MenuItem();
            mi.Header = "[Empty]";
            mi.Click += new RoutedEventHandler(HandleContentEmptyClick);
            miContent.Items.Add(mi);
            mi = new MenuItem();
            mi.Header = "Plain FlowDocument";
            mi.Click += new RoutedEventHandler(HandleContentFlowDocumentPlainClick);
            miContent.Items.Add(mi);
            mi = new MenuItem();
            mi.Header = "Complex FlowDocument";
            mi.Click += new RoutedEventHandler(HandleContentFlowDocumentComplexClick);
            miContent.Items.Add(mi);

            PopulateDrtMenu(miDrt);

            _documentRoot = new Border();
            _documentRoot.BorderThickness = new Thickness(2);
            _documentRoot.BorderBrush = Brushes.DarkGray;
            _documentRoot.Background = Brushes.Beige;

            DockPanel root = new DockPanel();
            root.Background = Brushes.DarkGray;
            root.Children.Add(mainMenu);
            root.Children.Add(_documentRoot);
            DockPanel.SetDock(mainMenu, Dock.Top);
            DRT.Show(root);

            // Return the lists of tests to run against the tree
            return new DrtTest[] { new DrtTest(RunApp) };
        }

        #endregion Protected Methods

        //-------------------------------------------------------------------
        //
        //  Menu Handlers
        //
        //-------------------------------------------------------------------

        #region Menu Handlers

        /// <summary>
        /// Viewer -> [Empty]
        /// </summary>
        private void HandleViewerEmptyClick(object sender, RoutedEventArgs e)
        {
            ClearDrtMode();
            if (_viewer != null)
            {
                SetViewerDocument(null);
                _viewer = null;
            }
            _documentRoot.Child = null;
        }

        /// <summary>
        /// Viewer -> FlowDocumentReader
        /// </summary>
        private void HandleViewerFlowDocumentReaderClick(object sender, RoutedEventArgs e)
        {
            ClearDrtMode();
            if (_viewer != null)
            {
                SetViewerDocument(null);
            }
            _viewer = new FlowDocumentReader();
            _documentRoot.Child = _viewer;
            SetViewerDocument(_document);
        }

        /// <summary>
        /// Viewer -> FlowDocumentPageViewer
        /// </summary>
        private void HandleViewerFlowDocumentPageViewerClick(object sender, RoutedEventArgs e)
        {
            ClearDrtMode();
            if (_viewer != null)
            {
                SetViewerDocument(null);
            }
            _viewer = new FlowDocumentPageViewer();
            _documentRoot.Child = _viewer;
            SetViewerDocument(_document);
        }

        /// <summary>
        /// Viewer -> FlowDocumentScrollViewer
        /// </summary>
        private void HandleViewerFlowDocumentScrollViewerClick(object sender, RoutedEventArgs e)
        {
            ClearDrtMode();
            if (_viewer != null)
            {
                SetViewerDocument(null);
            }
            _viewer = new FlowDocumentScrollViewer();
            _documentRoot.Child = _viewer;
            SetViewerDocument(_document);
        }

        /// <summary>
        /// Content -> [Empty]
        /// </summary>
        private void HandleContentEmptyClick(object sender, RoutedEventArgs e)
        {
            ClearDrtMode();
            _document = null;
            if (_viewer != null)
            {
                SetViewerDocument(null);
            }
        }

        /// <summary>
        /// Content -> Plain FlowDocument
        /// </summary>
        private void HandleContentFlowDocumentPlainClick(object sender, RoutedEventArgs e)
        {
            ClearDrtMode();
            _document = LoadFromXaml("FlowDocumentPlain.xaml") as FlowDocument;
            if (_viewer != null)
            {
                SetViewerDocument(_document);
            }
        }

        /// <summary>
        /// Content -> Complex FlowDocument
        /// </summary>
        private void HandleContentFlowDocumentComplexClick(object sender, RoutedEventArgs e)
        {
            ClearDrtMode();
            _document = LoadFromXaml("FlowDocumentComplex.xaml") as FlowDocument;
            if (_viewer != null)
            {
                SetViewerDocument(_document);
            }
        }

        /// <summary>
        /// DRT -> [Suite]
        /// </summary>
        private void HandleDrtSuiteClick(object sender, RoutedEventArgs e)
        {
            int index;
            string menuHeader;

            _document = null;
            if (_viewer != null)
            {
                SetViewerDocument(null);
                _viewer = null;
            }
            _documentRoot.Child = null;

            if (e.Source is MenuItem)
            {
                menuHeader = ((MenuItem)e.Source).Header as string;
                for (index = 0; index < _testSuites.Length; index++)
                {
                    if (menuHeader == _testSuites[index].Name)
                    {
                        if (_testSuites[index] is FlowTestSuite)
                        {
                            _currentTests = _testSuites[index].PrepareTests();
                            _currentTestIndex = 0;
                            _documentRoot.Child = ((FlowTestSuite)_testSuites[index]).Root;
                            HandleNextTestClick(null, e);
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// DRT -> [Suite] -> [Next Test]
        /// </summary>
        private void HandleNextTestClick(object sender, RoutedEventArgs e)
        {
            _currentTests[_currentTestIndex].Invoke();
            DRT.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(VerifyDrtTest), null);
            _miNextTest.IsEnabled = false;
        }

        #endregion Menu Handlers

        //-------------------------------------------------------------------
        //
        //  Private Methods
        //
        //-------------------------------------------------------------------

        #region Private Methods

        /// <summary>
        /// Run the application.
        /// </summary>
        private void RunApp()
        {
        }

        /// <summary>
        /// Pupulate DRT menu.
        /// </summary>
        /// <param name="miDrt"></param>
        private void PopulateDrtMenu(MenuItem miDrt)
        {
            int index;
            MenuItem mi;

            for (index = 0; index < _testSuites.Length; index++)
            {
                _testSuites[index].DRT = DRT;
                mi = new MenuItem();
                mi.Header = _testSuites[index].Name;
                mi.Click += new RoutedEventHandler(HandleDrtSuiteClick);
                miDrt.Items.Add(mi);
            }
            ClearDrtMode();
        }

        /// <summary>
        /// Sets document on the current viewer.
        /// </summary>
        /// <param name="document">New content for the viewer.</param>
        private void SetViewerDocument(object document)
        {
            if (_viewer != null)
            {
                if (_viewer is FlowDocumentReader)
                {
                    ((FlowDocumentReader)_viewer).Document = document as FlowDocument;
                }
                else if (_viewer is FlowDocumentScrollViewer)
                {
                    ((FlowDocumentScrollViewer)_viewer).Document = document as FlowDocument;
                }
                else if (_viewer is DocumentViewerBase)
                {
                    ((DocumentViewerBase)_viewer).Document = document as IDocumentPaginatorSource;
                }
            }
        }

        /// <summary>
        /// Verify test execution.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private object VerifyDrtTest(object arg)
        {
            if (_currentTests != null)
            {
                _currentTests[_currentTestIndex + 1].Invoke();
                _currentTestIndex += 2;
                _miNextTest.IsEnabled = _currentTestIndex < _currentTests.Length;
                if (_currentTestIndex < _currentTests.Length)
                {
                    _miNextTest.Header = "Next: " + _currentTests[_currentTestIndex].Method.Name;
                }
                else
                {
                    _miNextTest.Header = "";
                }
            }
            return null;
        }

        /// <summary>
        /// Clear state used by DRT mode.
        /// </summary>
        private void ClearDrtMode()
        {
            _miNextTest.IsEnabled = false;
            _currentTests = null;
            _currentTestIndex = 0;
        }

        /// <summary>
        /// Load content from xaml file.
        /// </summary>
        private object LoadFromXaml(string xamlFileName)
        {
            object content = null;
            Stream stream = null;
            string fileName = ((DrtFlowBase)DRT).DrtFilesDirectory + xamlFileName;
            try
            {
                stream = File.OpenRead(fileName);
                content = XamlReader.Load(stream);
            }
            finally
            {
                // done with the stream
                if (stream != null) { stream.Close(); }
            }
            return content;
        }

        #endregion Private Methods

        //-------------------------------------------------------------------
        //
        //  Private Fields
        //
        //-------------------------------------------------------------------

        #region Private Fields

        private Border _documentRoot;           // Client area root.
        private FlowDocument _document;         // Document displayed in app mode
        private FrameworkElement _viewer;       // Viewer associated with document
        private MenuItem _miNextTest;           // MenuItem for execution of the next test in DRT suite
        private DrtTestSuite[] _testSuites;     // Available test suites
        private DrtTest[] _currentTests;        // Tests for currently selected test suite
        private int _currentTestIndex;          // Index of currently executed test

        #endregion Private Fields
    }
}
