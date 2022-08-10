// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test of basic 3D functionality.
// For more interesting interactive mode (not DRT) use "-i"
//
//
//

using System;
using System.Xml;
using System.Reflection;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Markup;
using System.Windows.Input;

using System.Windows.Data;
using System.ComponentModel;
using CultureInfo = System.Globalization.CultureInfo;

using Tests;

// Loose resources should be declared in the .proj file.  Since this project does
// not have a .proj file the required attributes can be added manually. 
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"DrtFiles/DrtBasic3D/combined.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"DrtFiles/DrtBasic3D/ambient.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"DrtFiles/DrtBasic3D/diffuse.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"DrtFiles/DrtBasic3D/emissive.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"DrtFiles/DrtBasic3D/specular.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"DrtFiles/DrtBasic3D/msnbackground.bmp")]

namespace Microsoft.Samples
{
    public class DrtBasic3D : Window
    {
        public DrtBasic3D(bool interactive)
        {
            _spareComboBox = new ComboBox();
            _suite = new TestSuite();

            //
            // Set up Window
            //
            Width = 800;
            Height = 600;
            Title = "DrtBasic3D";

            // Build controls.
            _control1 = new Viewport3D();
            _control2 = new Viewport3D();
            _comboBox = new ComboBox();
            for (int i = 0; i < _suite.Count; ++i)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = _suite[i].ToString();
                _comboBox.Items.Add(item);
            }
            _comboBox.SelectionChanged += new SelectionChangedEventHandler(this.TestCaseChanged);
            _comboBox.SelectedIndex = 0;

            Button buttonRunAll = new Button();
            buttonRunAll.Content = "Run All";
            buttonRunAll.Click += this.RunAllTests;
            Button buttonRunNext = new Button();
            buttonRunNext.Content = "Next Test";
            buttonRunNext.Click += this.RunNextTest;
            Button buttonDumpXaml = new Button();
            buttonDumpXaml.Content = "Dump Xaml";
            buttonDumpXaml.Click += this.DumpXaml;
            Button buttonSaveXaml = new Button();
            buttonSaveXaml.Content = "Save Xaml";
            buttonSaveXaml.Click += this.SaveXaml;
            Button buttonRoundtripXaml = new Button();
            buttonRoundtripXaml.Content = "Roundtrip Xaml";
            buttonRoundtripXaml.Click += this.RoundtripXaml;
            CheckBox trackballEnabled = new CheckBox();
            trackballEnabled.Content = "Trackball";
            trackballEnabled.IsChecked = true; // Check this box before adding CB because trackball hasn't been constructed yet
            trackballEnabled.Checked += this.TrackballCBChecked;
            trackballEnabled.Unchecked += this.TrackballCBUnchecked;
            CheckBox checkClip = new CheckBox();
            checkClip.Content = "Clip To Bounds";
            
            // NOTE:  We intentionally set the checked state to true before hooking
            //        up the changed handlers to test that Viewport3Ds clip by default.
            checkClip.IsChecked = true;
            checkClip.Checked += this.ClipToBoundsCBChecked;
            checkClip.Unchecked += this.ClipToBoundsCBUnchecked;

            DockPanel dpanel = new DockPanel();
            Grid viewportPanel = new Grid();
            viewportPanel.Background = Brushes.DarkBlue;
            _viewportPanel = viewportPanel;
            viewportPanel.ColumnDefinitions.Add( new ColumnDefinition() );
            viewportPanel.ColumnDefinitions.Add( new ColumnDefinition() );

            if (!interactive)
            {
                // Keep sizes fixed for hit testing tests.  Otherwise we'd be dependent on the theme/layout/other controls.
                _control1.Width = 392;
                _control1.Height = 525;
                _control2.Width = 392;
                _control2.Height = 525;
            }
            
            Grid.SetColumn(_control1,0);
            Grid.SetColumn(_control2,1);
            viewportPanel.Margin = new Thickness(4);
            viewportPanel.Children.Add(_control1);
            viewportPanel.Children.Add(_control2);

            _buttonDock = new DockPanel();
            DockPanel.SetDock(_buttonDock,Dock.Bottom);
            _buttonDock.Children.Add(_comboBox);
            _buttonDock.Children.Add(trackballEnabled);
            _buttonDock.Children.Add(checkClip);
            _buttonDock.Children.Add(buttonDumpXaml);
            _buttonDock.Children.Add(buttonSaveXaml);
            _buttonDock.Children.Add(buttonRoundtripXaml);
            _buttonDock.Children.Add(buttonRunNext);
            _buttonDock.Children.Add(buttonRunAll);
            _buttonDock.Margin = new Thickness(4);
            foreach (Control control in _buttonDock.Children)
            {
                DockPanel.SetDock(control,Dock.Left);
                control.Margin = new Thickness(4);
            }

            // Textual label that the tests can update
            _animatedText = new TextBlock();
            _animatedText.Text = "Hello";
            DockPanel.SetDock(_animatedText,Dock.Bottom);
            _buttonDock.Children.Add(_animatedText);
            
            // Spare combobox that tests can use
            DockPanel.SetDock(_spareComboBox,Dock.Bottom);
            _buttonDock.Children.Add(_spareComboBox);
            
            dpanel.Children.Add(_buttonDock);
            dpanel.Children.Add(viewportPanel);
            Content = dpanel;

            _trackball = new Trackball();
            _trackball.Attach(this);
            _trackball.Slaves.Add(_control1);
            _trackball.Slaves.Add(_control2);
            _trackball.Enabled = trackballEnabled.IsChecked == true;

            this.MouseLeftButtonUp += new MouseButtonEventHandler(OnWindowClicked);
        }

        private void OnWindowClicked(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine( "Sender is " + sender.ToString() );
            Point p1 = Mouse.GetPosition(_control1);
            Console.WriteLine("Trying left  ({0})", p1);
            VisualTreeHelper.HitTest(
                _control1,
                null, 
                new HitTestResultCallback(HTResult),
                new PointHitTestParameters(p1)
            );

            Point p2 = e.GetPosition(_control2);
            Console.WriteLine("Trying right ({0})", p2);
            VisualTreeHelper.HitTest(
                _control2,
                null,
                new HitTestResultCallback(HTResult),
                new PointHitTestParameters(p2)
            );
        }

        public HitTestResultBehavior HTResult(System.Windows.Media.HitTestResult result2d)
        {
            RayHitTestResult rayResult = result2d as RayHitTestResult;

            if (rayResult != null)
            {
                RayMeshGeometry3DHitTestResult rayMeshResult = rayResult as RayMeshGeometry3DHitTestResult;

                if (rayMeshResult != null)
                {
                    Console.WriteLine("\tHit {0}/{1}", rayResult.VisualHit.GetType().Name, rayResult.ModelHit.GetType().Name);
                    Console.WriteLine("\t\tPointHit: {0}", rayResult.PointHit);
                    Console.WriteLine("\t\tDistanceToRayOrigin: {0}", rayResult.DistanceToRayOrigin);
                    Console.WriteLine("\t\tVertexIndex1: {0} (%{1})", rayMeshResult.VertexIndex1, rayMeshResult.VertexWeight1 * 100);
                    Console.WriteLine("\t\tVertexIndex2: {0} (%{1})", rayMeshResult.VertexIndex2, rayMeshResult.VertexWeight2 * 100);
                    Console.WriteLine("\t\tVertexIndex3: {0} (%{1})", rayMeshResult.VertexIndex3, rayMeshResult.VertexWeight3 * 100);
                    Console.WriteLine("\t\tMeshHit: {0}", rayMeshResult.MeshHit);
                    Console.WriteLine(String.Empty);
                }
            }

            return HitTestResultBehavior.Continue;
        }

        /// <summary>
        /// Callback for toggling the trackball
        /// </summary>
        void TrackballCBChecked(object sender, RoutedEventArgs e)
        {
            _trackball.Enabled = true;
        }
        void TrackballCBUnchecked(object sender, RoutedEventArgs e)
        {
            _trackball.Enabled = false;
        }


        /// <summary>
        /// Callback for toggling the trackball
        /// </summary>
        void ClipToBoundsCBChecked(object sender, RoutedEventArgs e)
        {
            _control1.ClipToBounds = true;
            _control2.ClipToBounds = true;
        }
        void ClipToBoundsCBUnchecked(object sender, RoutedEventArgs e)
        {
            _control1.ClipToBounds = false;
            _control2.ClipToBounds = false;
        }

        /// <summary>
        /// Callback for changing the test case via the combobox.
        /// </summary>
        void TestCaseChanged(object sender, SelectionChangedEventArgs e)
        {
            
            TestCase testcase = _suite[(sender as ComboBox).SelectedIndex];
            testcase.BuildSceneLeft(_control1);
            testcase.BuildSceneRight(_control2);
        }

        /// <summary>
        /// Serializes the left and right Viewport3D's to xaml, then deserializes them.
        /// Returns true if it worked.
        /// </summary>
        bool RoundtripXaml()
        {
            ArrayList newControls = new ArrayList();
            foreach (Viewport3D vp in _viewportPanel.Children)
            {
                StringBuilder sb = new StringBuilder();
                TextWriter writer = new StringWriter(sb, CultureInfo.GetCultureInfo("en-us"));
                XmlTextWriter xmlWriter = new XmlTextWriter(writer);
                xmlWriter.Formatting = Formatting.Indented; // make the writer put tags on separate lines, indented
                XamlDesignerSerializationManager manager = new XamlDesignerSerializationManager(xmlWriter);
                manager.XamlWriterMode = XamlWriterMode.Expression;
                
                XamlWriter.Save(vp, manager);
                
                xmlWriter.Close();
                writer.Close();

                string s = sb.ToString();
                byte[] ba = new byte[ s.Length ];
                int i = 0;
                foreach (char c in s)
                {
                    ba[i++] = (byte) c;
                }
                MemoryStream stream = new MemoryStream(ba);
                ParserContext pc = new ParserContext();
                try
                {
                    UIElement root = (UIElement) XamlReader.Load(stream, pc);
                    Viewport3D viewport = root as Viewport3D;
                    if (viewport == null)
                    {
                        Console.WriteLine( "Xaml round trip parsed, but returned wrong element!" );
                        return false;
                    }

                    // Do some simple tests to verify that the roundtripped model is the same
                    // thing that we started with.
                    if (viewport.Children.Count != vp.Children.Count)
                    {
                        Console.WriteLine( "Xaml round trip parsed, but returned wrong number of children!" );
                        return false;
                    }
                }
                catch( XamlParseException exception )
                {
                    Console.WriteLine( "Xaml round trip failing with " + exception );
                    return false;
                }
            }
            return true;
        }

        void RoundtripXaml(object o, RoutedEventArgs e)
        {
            RoundtripXaml();
        }
    
        /// <summary>
        /// Writes the xaml for the main content of our app to a textwriter.
        /// </summary>
        void WriteXaml(TextWriter tw)
        {
            // With the event on the trackball checkbox the
            // SaveAsXml() throws an exception which can be ignored
            // and the program will continue.  Here, I just remove the
            // checkbox's event handler.
            foreach (FrameworkElement element in _buttonDock.Children)
            {
                CheckBox checkbox = element as CheckBox;
                if (checkbox != null)
                {
                    checkbox.Checked -= TrackballCBChecked;
                    checkbox.Unchecked -= TrackballCBUnchecked;
                    checkbox.Checked -= ClipToBoundsCBChecked;
                    checkbox.Unchecked -= ClipToBoundsCBUnchecked;
                }
            }
            tw.WriteLine(XamlWriter.Save(Content));
        }

        /// <summary>
        /// Dumps the xaml for our main window to the console.
        /// </summary>
        void DumpXaml(object o, RoutedEventArgs e)
        {
            WriteXaml(Console.Out);
            
        }

        /// <summary>
        /// Saves the xaml for the main window to save.xaml.
        /// </summary>
        void SaveXaml(object o, RoutedEventArgs e)
        {
            StreamWriter sw = new StreamWriter("save.xaml");
            WriteXaml(sw);
            sw.Close();
        }
    
        /// <summary>
        /// Callback for button that automatically runs all events.
        /// </summary>
        void RunAllTests(object o, RoutedEventArgs e)
        {
            RunAllTests();
        }
    
        void RunAllTests()
        {
            // Set up state for tick handler which helps RunAllTests
            _count=0;
            // Push a work item onto the queue to move to the next test.
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(MoveToNextTest), null);
        }

        private bool _testForever = false;
        private bool _timeHittesting = false;
        private static Stopwatch _stopwatch = new Stopwatch();
        
        private int _count;
        private int _failedCount = 0;
        private string _failedCaseNames = "";

        private object MoveToNextTest(object arg)
        {
            if (_count == _suite.Count)
            {
                _count = 0;
            }
            _comboBox.SelectedIndex = _count;
            ++_count;

            // Push a work item onto the queue to execute our next test.  This is the
            // same queue that processes paint messages so we are gauranteed to
            // be rendered.
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(DoCurrentTest), null);
        
            return null;
        }

        private object DoCurrentTest(object arg)
        {
            if (_timeHittesting)
            {
                const double DELTA = 0.1;
                for (double u = 0; u < 1; u += DELTA)
                {
                    for (double v = 0; v < 1; v += DELTA)
                    {
                        VisualTreeHelper.HitTest(_control1,new Point(_control1.Width*u,_control1.Height*v));
                        VisualTreeHelper.HitTest(_control2,new Point(_control2.Width*u,_control2.Height*v));
                    }
                }
            }
            
            TestCase testcase = _suite[_count - 1];
            if (!testcase.DoTest() || (testcase.DoRoundtrip() && !RoundtripXaml()) )
            {
                _failedCount++;
                if (_failedCount > 1)
                {
                    _failedCaseNames += ", " + testcase;
                }
                else
                {
                    _failedCaseNames += " " + testcase;
                }
            }
            if (_count < _suite.Count || _testForever)
            {                
                // Push a work item onto the queue to move to the next test.
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(MoveToNextTest), null);
            }
            else
            {
                if (_failedCount == 0) 
                {
                    Console.WriteLine("DrtBasic3D successfully completed " + (_count - _failedCount) + " of " + _suite.Count + " tests.");
                }
                else
                {
                    Console.WriteLine("DrtBasic3D failed " + _failedCount + " of " + _suite.Count + " tests.  FAILED TESTS:" + _failedCaseNames);
                }

                Application.Current.Shutdown(-_failedCount);
            }

            return null;
        }

        /// <summary>
        /// Callback for button that runs the next test.
        /// </summary>
        void RunNextTest(object sender, RoutedEventArgs e)
        {
            int index = _comboBox.SelectedIndex;
            ++index;
            if (index >= _comboBox.Items.Count)
                index = 0;
            _comboBox.SelectedIndex = index;
        }

        /// <summary>
        /// Prints out the names of all the main (no plusses in the name), public, non-abstract Media3D classes.
        /// </summary>
        static void PrintMedia3D()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(PerspectiveCamera));
            Module[] ms = assembly.GetModules();
            List<Type> types = new List<Type>();
            foreach (Module m in ms)
            {
                Type[] ts = m.GetTypes();
                foreach (Type t in ts)
                {
                    if (t.ToString().Contains("Media3D") &&
                        !t.ToString().Contains("+") &&
                        !t.IsAbstract && t.IsPublic)
                        types.Add(t);
                }
            }
            foreach (Type i in types)
            {
                Console.WriteLine(i);
            }
        }

        [STAThread]
        public static int Main(string [] args)
        {
            bool interactive = ((IList)args).Contains("-i");
            
            Application application = new Application();

            DrtBasic3D basic = new DrtBasic3D(interactive);

            _stopwatch.Start();
            if (((IList)args).Contains("-cover"))
            {
                DrtBasic3D.PrintMedia3D();
                return 0;
            }
            if (((IList)args).Contains("-forever"))
            {
                basic._testForever = true;
            }
            if (((IList)args).Contains("-tier"))
            {
                Console.WriteLine("Graphics Tier is " + RenderCapability.Tier);
            }
            if (((IList)args).Contains("-hit"))
            {
                basic._timeHittesting = true;
            }
            if (((IList)args).Contains("-help") || ((IList)args).Contains("-h"))
            {
                Console.Write("DrtBasic3D [-auto] [-h(elp)]\nInteractive program to test basic 3D features.\n-tier : Print graphics tier\n" +
                              "-auto : Automatically run tests\n-help : Show this usage info.\n");
                return 0;
            }
            if (!interactive)
            {
                // Running in drt mode, re-direct output to DrtBasic3D.log
                // 
                StreamWriter logOutput = new StreamWriter("DrtBasic3D.log", false /* do not append - re-write if the file exists */);
                logOutput.AutoFlush = true;
                Console.SetOut(logOutput);
                Console.WriteLine("DrtBasic3D contacts are Microsoft.  Use -i for interactive demo, -h for help.  ");
                basic.RunAllTests();
            }

            basic.Show();
            
            int result = application.Run();
            _stopwatch.Stop();
            
            Console.Out.Close();

            // Recover the standard output stream so that a small note can be displayed.
            StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput());
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
            Console.WriteLine("DrtBasic3D " + (result == 0 ? "SUCCEEDED" : "FAILED (check DrtBasic3D.log)"));
            Console.WriteLine("Elapsed time is " + _stopwatch.ElapsedMilliseconds);

            return result;
        }

        private Trackball _trackball;
        private ComboBox _comboBox;
        static private ComboBox _spareComboBox;
        private TestSuite _suite;
        private Viewport3D _control1,_control2;
        private Panel _viewportPanel;
        private DockPanel _buttonDock;
        static private SolidColorBrush _brush;

        // This is a brush that changes programatically
        static public Brush ChangingBrush
        {
            get
            {
                if (_brush == null)
                {
                    _brush = new SolidColorBrush(Colors.White);
                }
                return _brush;
            }
        }

        static public ComboBox ComboBox
        {
            get
            {
                return _spareComboBox;
            }
        }
        
        private void UpdateReadout(object sender, EventArgs e)
        {
            try
            {
                if (_do != null && _dp != null)
                {
                    _animatedText.Text =
                        _do.GetValue(_dp).ToString() + " vs. " +
                        ((Animatable)_do).GetAnimationBaseValue(_dp).ToString();
                }

                // Animate brush "by hand" if it exists
                if (_brush != null)
                {
                    _brush.Color = (_brush.Color == Colors.White) ?
                                   Colors.Black :
                                   Colors.White;
                }
            }
            catch
            {
                // No biggie.
            }
        }

        static public void SetDPDO( DependencyObject dobj, DependencyProperty dp )
        {
            _do = dobj;
            _dp = dp;
        }
        private TextBlock _animatedText;
        static private DependencyObject _do;
        static private DependencyProperty _dp;

        private const int _delayInMilliseconds = 100;
    }
}

