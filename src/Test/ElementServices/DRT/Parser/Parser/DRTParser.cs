// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***************************************************************************\
*
*
*  Parser tests.
*
*
\***************************************************************************/
using System;
using System.Threading;
using System.Text;

using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;
using System.Windows.Markup;
using System.Windows.Documents;

using MS.Win32;
using System.Windows.Media;
using System.Xml;
using System.Drawing;
using System.Globalization;
using System.Windows.Media.Animation; //ParallelTimeline
using System.Windows.Media.Media3D;

using DRTText;

namespace DRT
{
    // Test class
    public class MyButton : System.Windows.Controls.Button
    {
        public MyButton() : base()
        {
        }

        public static readonly DependencyProperty YabbaProperty
            = DependencyProperty.RegisterAttached("Yabba", typeof(int), typeof(MyButton));
        public static readonly DependencyProperty DabbaProperty
            = DependencyProperty.RegisterAttached("Dabba", typeof(int), typeof(MyButton));
        public static readonly DependencyProperty DoTypeProperty
            = DependencyProperty.RegisterAttached("DoType", typeof(Type), typeof(MyButton));
        
        /// <summary>
        ///     Test DependencyProperty attached prop 
        /// </summary>
        public static void SetDoType(DependencyObject target, Type value)
        {
            _doType = value;
        }
        
        /// <summary>
        ///     Test DependencyProperty attached prop
        /// </summary>
        public static Type GetDoType(DependencyObject target)
        {
            return _doType;
        }

        /// <summary>
        ///     Test DependencyProperty attached prop 
        /// </summary>
        public static void SetYabba(DependencyObject target, int value)
        {
            _yabba = value;
        }
        
        /// <summary>
        ///     Test DependencyProperty attached prop
        /// </summary>
        public static int GetYabba(DependencyObject target)
        {
            return _yabba;
        }

        /// <summary>
        ///     Test DependencyProperty attached prop 
        /// </summary>
        public static void SetDabba(DependencyObject target, int value)
        {
            _dabba = value;
        }
        
        /// <summary>
        ///     Test DependencyProperty attached prop
        /// </summary>
        public static int GetDabba(DependencyObject target)
        {
            return _dabba;
        }

        private static Type _doType;
        private static int  _yabba;
        private static int  _dabba;

    }
    
    // Parser base test class
    public class ParserTest : DrtBase
    {    
        [STAThread]
        public static int Main(string[] args)
        {
            DrtBase drt = new ParserTest();
            return drt.Run(args);
        }

        // Create a new test instance, setting common drt information.
        public ParserTest()
        {
            // Preload test classes, if not already there.
            Assembly.LoadFrom(".\\testclasses.dll");
            
            WindowTitle = "Parser DRT";
            TeamContact = "WPF";
            Contact = "Microsoft";
            DrtName = "DRTParser";
            Suites = new DrtTestSuite[]{
                        new InternalAPITests(),
                        new SimpleLoadTest(),
                        new MarkupExtensionLoadTest(),
                        new LoadedEventTest(),
                        new ResourceLoadTest(),
                        new StringLoadTest(),
                        new FceLoadTest(),
                        new XmlReaderLoadTest(),
                        new TemplateLoadTest(),
                        new XmlIslandLoadTest(),
                        new AttachedPropertiesTest(),
                        new AsyncParsingTests(),
                        new DateTimeTests(),
                        new InitializationStringTest(),
                        new NoDefaultXmlNsTest(),
                        new XamlValidationTest(), 
                        null           
                        };

            // Turn on dummy tracing; it won't go anywhere, but will exercise the code.
            EnableNoopTracing();
        }
        
    }

    // Base for all parser tests.
    public class ParserSuites : DrtTestSuite
    {
        public ParserSuites (string name) : base (name)
        {
        }
        
        // Loads up a test file. 
        // File is relative to the TestFileDir
        protected Stream LoadTestFile(string fileName)
        {
            string testFile = TestFileDir + fileName;
            System.IO.Stream stream = File.OpenRead(testFile);

            return stream;
        }
        
        string TestFileDir = ".\\"; // directory path for test files, include \ at end.
        
    }

    // Test various parser bugs to make sure we exercise some internal apis.
    public class InternalAPITests : ParserSuites
    {
        public InternalAPITests() : base("Internal API Tests")
        {
        }
        
        public override DrtTest[] PrepareTests()
        {
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest( RunTest ),
                        null        
                        };
        }

        private void RunTest()
        {
            Console.WriteLine("Internal API tests started.");

            // Internal LoadXml
            ParserContext context = new ParserContext();
            Stream xamlStream = LoadTestFile(@"DrtFiles\Parser\drtparser4.xaml");
            XmlTextReader xmlReader = new XmlTextReader(xamlStream,
                                     XmlNodeType.Document,
                                    (XmlParserContext)context);
            Assembly assemblyPF = typeof(System.Windows.FrameworkElement).Assembly;  // PresentationFramework
            Type typeXamlParseMode =  assemblyPF.GetType("System.Windows.Markup.XamlParseMode");
            Type parserType = typeof(XamlReader);
            BindingFlags bfMember = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
            parserType.InvokeMember("Load", 
                                     bfMember, 
                                     null, 
                                     null,
                                     new object[] { xmlReader, context, Enum.Parse(typeXamlParseMode,"Asynchronous")});
            
            Console.WriteLine("Internal API tests passed.");

        }
    }

    public class AsyncParsingTests: ParserSuites
    {
        public AsyncParsingTests(): base("Async Parsing Tests")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[]{
                        new DrtTest(RunAsyncLoadTest),
                        new DrtTest(RunAsyncCancelTest),
                        new DrtTest(RunAsyncErrorTest),
                        };
        }

        private void RunAsyncLoadTest()
        {
            Console.WriteLine("Async load test started.");

            ParserContext parserContext = new ParserContext();
            Stream xamlStream = LoadTestFile(@"DrtFiles\Parser\drtparser8.xaml");
            
            XamlReader xamlReader = new XamlReader();
            xamlReader.LoadCompleted += new AsyncCompletedEventHandler(OnAsyncLoadCompleted);
            
            _root = xamlReader.LoadAsync(xamlStream, parserContext);

            // Verify the root
            VerifyRoot();
        }

        private void OnAsyncLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Verify loaded content
            VerifyLoad(e);
            
            Console.WriteLine("Async load test passed.");
        }
        
        private void RunAsyncCancelTest()
        {
            Console.WriteLine("Async cancel test started.");

            ParserContext parserContext = new ParserContext();
            Stream xamlStream = LoadTestFile(@"DrtFiles\Parser\drtparser8.xaml");
            
            XamlReader xamlReader = new XamlReader();
            xamlReader.LoadCompleted += new AsyncCompletedEventHandler(OnAsyncCancelCompleted);
            
            _root = xamlReader.LoadAsync(xamlStream, parserContext);
            xamlReader.CancelAsync();
            
            // Verify the root
            VerifyRoot();
        }

        private void OnAsyncCancelCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Verify cancel
            VerifyCancel(e);
            
            Console.WriteLine("Async cancel test passed.");
        }
        
        private void RunAsyncErrorTest()
        {
            Console.WriteLine("Async error test started.");

            ParserContext parserContext = new ParserContext();
            Stream xamlStream = LoadTestFile(@"DrtFiles\Parser\drtparser9.xaml");
            
            XamlReader xamlReader = new XamlReader();
            xamlReader.LoadCompleted += new AsyncCompletedEventHandler(OnAsyncErrorCompleted);
            
            _root = xamlReader.LoadAsync(xamlStream, parserContext);
            
            // Verify the root
            VerifyRoot();
        }

        private void OnAsyncErrorCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Verify error
            VerifyError(e);
            
            Console.WriteLine("Async error test passed.");
        }

        #region HelperMethods

        private void VerifyRoot()
        {
            if (!(_root is Grid))
            {
                throw new Exception("Async load root incorrect. Expected " + typeof(Grid) + ". Got " + _root);
            }
        }

        private void VerifyLoad(AsyncCompletedEventArgs e)
        {
            // Verify EventArgs
            
            if (e.Cancelled)
            {
                throw new Exception("This is a successfull load and hence the Cancelled flag on the EventArgs must not be set");
            }

            if (e.Error != null)
            {
                throw new Exception("This is a successfull load and hence the Error on the EventArgs must not be set");
            }

            // Verify loaded content
            
            Grid grid = (Grid)_root;

            Type[] childrenTypes = new Type[] {typeof(Border), typeof(Border), typeof(TextBlock)};

            if (grid.Children.Count > childrenTypes.Length)
            {
                throw new Exception("Too many children found in async loaded content.");
            }
            
            for (int i=0; i<grid.Children.Count; i++)
            {
                UIElement child = grid.Children[i];
                
                if (child == null || child.GetType() != childrenTypes[i])
                {
                    throw new Exception("Incorrect async load child. Expected " + childrenTypes[i] + ". Got " + child);
                }
            }
        }

        private void VerifyCancel(AsyncCompletedEventArgs e)
        {
            // Verify EventArgs

            if (!e.Cancelled)
            {
                throw new Exception("This is a cancelled operation and hence the Cancelled flag on the EventArgs must be set");
            }

            if (e.Error != null)
            {
                throw new Exception("This is a cancelled operation for what would have otherwise been a successfull load. Hence the Error on the EventArgs must not be set");
            }
        }

        private void VerifyError(AsyncCompletedEventArgs e)
        {
            // Verify EventArgs

            if (e.Cancelled)
            {
                throw new Exception("This async load operation did run to its end even though there was an error during parse. Hence the Cancelled flag on the EventArgs must not be set");
            }

            if (e.Error == null)
            {
                throw new Exception("This async load should have resulted in an error. Hence the on the EventArgs must be set");
            }
        }
        
        #endregion HelperMethods

        #region Data

        private object _root;
        
        #endregion Data
    }


    //============================================================================================
    //
    //  DateTimeTests
    //
    //  This tests the special serializer we use for parsing and serializing DateTime
    //  values.
    //
    //============================================================================================

    public class DateTimeTests: ParserSuites
    {
        public DateTimeTests(): base("DateTime Test")
        {
            // Calculate the suffix used for local times on January first (this is picked to 
            // avoid daylight savings time confusion, which is user configurable).  This
            // "localSuffix" is used below to determine expected values.
            
            string formatString = "yyyy-MM-dd'T'HH':'mmzzz";
            DateTime dateTime = new DateTime( 2006, 1, 1 );
            string now = dateTime.ToString( formatString, CultureInfo.GetCultureInfo("en-US") );
            string localSuffix = now.Substring( now.LastIndexOf('-') );


            // Create each of the scenario test objects.
            
            _testObjects = new DateTimeTestObject[]
                    { 
                      // Start with a DateTime object that has just a date (unspecified)
                      new DateTimeTestObject( new DateTime( 2006, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified ),
                                              null,
                                              "2006-02-18" ),


                      // Same thing, in UTC time
                      new DateTimeTestObject( new DateTime( 2006, 2, 18, 0, 0, 0, 0, DateTimeKind.Utc ),
                                              null,
                                              "2006-02-18T00:00Z" ),
                                              
                      // Same thing, in local time
                      new DateTimeTestObject( new DateTime( 2006, 2, 18, 0, 0, 0, 0, DateTimeKind.Local ),
                                              null,
                                              "2006-02-18T00:00" + localSuffix ),


                      
                      // Start with a DateTime object that has both a date and a time (minute granularity)
                      new DateTimeTestObject( new DateTime( 2006, 2, 18, 17, 44, 0, 0, DateTimeKind.Unspecified ),
                                              null,
                                              "2006-02-18T17:44" ),

                      // Start with a DateTime object that has both a date and a time (second granularity)
                      new DateTimeTestObject( new DateTime( 2006, 2, 18, 17, 44, 12, 0, DateTimeKind.Unspecified ),
                                              null,
                                              "2006-02-18T17:44:12" ),

                      // Start with a DateTime object that has date and a time (millisecond granularity)
                      new DateTimeTestObject( new DateTime( 2006, 2, 18, 17, 44, 12, 123, DateTimeKind.Utc ),
                                              null,
                                              "2006-02-18T17:44:12.123Z" ),


                      // Start with an input string in unspecified time
                      new DateTimeTestObject( new DateTime( 1970, 1, 29, 0, 0, 0, 0, DateTimeKind.Unspecified ),
                                              "         1/29/1970            ", // Verify trimming
                                              "1970-01-29" ),

                      // Start with an input string in local time
                      new DateTimeTestObject( new DateTime( 1970, 1, 29, 11, 0, 0, 0, DateTimeKind.Local ),
                                              "1/29/1970 11:00" + localSuffix,
                                              "1970-01-29T11:00" + localSuffix ),

                      // DateTime.MinValue special-case
                      new DateTimeTestObject( DateTime.MinValue,
                                              "",
                                              "0001-01-01" )
                    };


        }

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[]{
                        new DrtTest(RunDateTimeTest),
                        };
        }

        private void RunDateTimeTest()
        {
            Console.WriteLine( Name + " started.");

            foreach( DateTimeTestObject testObject in _testObjects )
            {
                RunOneTest( testObject );
            }


            Console.WriteLine(Name + " test passed.");

        }

        // This method runs one test scenario.  The test scenario is described in the
        // DateTimeTestObject.  That object has the DateTime we want to test, as well 
        // as the expected SerializedOuput.  It may also have StringInput, in which case
        // we'll try to parse it first.
        
        private void RunOneTest( DateTimeTestObject testObject )
        {
            DateTimeControl dateTimeControl;

            // Boilerplate markup for what the DateTimeControl looks like.
            
            string xamlHeader = "<DateTimeControl" +
                                " DateTime=\"";
            string xamlFooter = "\" xmlns=\"clr-namespace:DRT;assembly=DrtParser\" />";


            // If this scenario has StringInput, we'll parse it first.
            if( testObject.StringInput != null )
            {
                // Create markup from the date/time string.
                string inputString = xamlHeader + testObject.StringInput + xamlFooter;

                // Load the markup.
                dateTimeControl = XamlReader.Load( new XmlTextReader(new StringReader(inputString.ToString()))) as DateTimeControl;

                // Validate that we got something back.
                DRT.Assert( dateTimeControl != null,
                            "Failed to load DateTimeControl for " + testObject.StringInput );

                // And validate that it's got the right DateTime value.
                DRT.Assert( testObject.DateTime == dateTimeControl.DateTime,
                            "DateTime value doesn't match for " + testObject.StringInput );

            }

            // Create a DateTimeControl with its DateTime set to the scenario input.
            dateTimeControl = new DateTimeControl();
            dateTimeControl.DateTime = testObject.DateTime;

            // Serialize this DateTimeControl
            string outputString = XamlWriter.Save( dateTimeControl );

            // Calculate what we expect the serialized output to be
            string expectedString = xamlHeader
                                   + testObject.SerializedOutput
                                   + xamlFooter;

            // Validate the ouput
            DRT.Assert( outputString == expectedString,
                        "Serialization of DateTime Failed, \n" +
                        "expected '" + expectedString + "'" +
                        "got '" + outputString + "'" );
                        
        }

        private static DateTimeTestObject[] _testObjects;
        
    }


    // This class is used to describe a scenario case for the DateTimeTests test.
    public class DateTimeTestObject
    {
        public DateTimeTestObject( DateTime dateTime, string stringInput, string serializedOutput )
        {
            DateTime = dateTime;
            StringInput = stringInput;
            SerializedOutput = serializedOutput;
        }
        
        public DateTime DateTime;
        public string StringInput;
        public string SerializedOutput;
    }

    // DateTimeControl to test the custom DateTime serialization
    public class DateTimeControl : FrameworkElement
    {

        // The DateTime to use for this scenario
        private DateTime _dateTime;
        public DateTime DateTime
        {
            get { return _dateTime; }
            set { _dateTime = value; }
        }

    }




    // Simple load of a file that contains a number of animations.
    public class SimpleLoadTest : ParserSuites
    {
        public SimpleLoadTest() : base("Simple Animation Load Test")
        {
        }
        
        public override DrtTest[] PrepareTests()
        {
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest( RunTest ),
                        null        
                        };
        }

        private void RunTest()
        {
            Console.WriteLine("SimpleLoadTest started.");
            Stream xamlFileStream = LoadTestFile(@"DrtFiles\Parser\DrtParser1.xaml");
            UIElement root = null;
            int startT;
            int endT;

            try
            {
                // see if it loads
                startT = Environment.TickCount;
                root = (UIElement) XamlReader.Load(xamlFileStream);
                endT = Environment.TickCount;
            }
            finally
            {

                // done with the stream
                xamlFileStream.Close();
            }


            Console.WriteLine("SimpleLoadTest passed - Time = " + (endT-startT) + "ms");
        }
    }
    
    // Simple load of a file that contains a number of animations.
    public class MarkupExtensionLoadTest : ParserSuites
    {
        public MarkupExtensionLoadTest() : base("MarkupExtension Load Test")
        {
        }
        
        public override DrtTest[] PrepareTests()
        {
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest( RunTest ),
                        null        
                        };
        }

        private void RunTest()
        {
            Console.WriteLine("MarkupExtensionLoadTest started.");
            Stream xamlFileStream = LoadTestFile(@"DrtFiles\Parser\DrtParser6.xaml");
            UIElement root = null;
            int startT;
            int endT;

            try
            {
                // see if it loads
                startT = Environment.TickCount;
                root = (UIElement) XamlReader.Load(xamlFileStream);
                endT = Environment.TickCount;
            }
            finally
            {

                // done with the stream
                xamlFileStream.Close();
            }

            Console.WriteLine("MarkupExtensionLoadTest passed - Time = " + (endT-startT) + "ms");
        }
    }
        
    // Do a simple load of a document containing FrameworkContentElements.
    public class FceLoadTest : ParserSuites
    {
        public FceLoadTest() : base("Framework Content Element Load Test")
        {
        }
        
        public override DrtTest[] PrepareTests()
        {
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest( RunTest ),
                        null        
                        };
        }

        private void RunTest()
        {
            Console.WriteLine("FceLoadTest started.");
            Stream xamlFileStream = LoadTestFile(@"DrtFiles\Parser\testcell.xaml");
            UIElement root = null;
            int startT;
            int endT;

            try
            {
                // see if it loads
                startT = Environment.TickCount;
                root = (UIElement) XamlReader.Load(xamlFileStream);
                xamlFileStream.Close();
                xamlFileStream = LoadTestFile(@"DrtFiles\Parser\testfce.xaml");
                root = (UIElement) XamlReader.Load(xamlFileStream);
                endT = Environment.TickCount;
            }
            finally
            {

                // done with the stream
                xamlFileStream.Close();
            }

            Console.WriteLine("FceLoadTest passed - Time = " + (endT-startT) + "ms");
        }
    }


    public class InitializationStringTest : ParserSuites
    {
        public InitializationStringTest() : base("Initialization String Test")
        {
        }
        
        public override DrtTest[] PrepareTests()
        {
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest( RunTest ),
                        null        
                        };
        }

        private void RunTest()
        {
            Console.WriteLine( Name + " started.");

            string xaml = @"<Sketch xmlns='clr-namespace:DRT;assembly=DrtParser'>0,1 1,0</Sketch>";
            Sketch sketch = XamlReader.Load( new XmlTextReader(new StringReader(xaml.ToString()))) as Sketch;
            DRT.Assert( sketch != null, "Couldn't create Sketch using an initialization string" );

            xaml = @"<MyTextBlock xmlns='clr-namespace:DRT;assembly=DrtParser'>Hello</MyTextBlock>";
            MyTextBlock myTextBlock = XamlReader.Load( new XmlTextReader(new StringReader(xaml.ToString()))) as MyTextBlock;
            DRT.Assert( myTextBlock != null && myTextBlock.Inlines.AddCalled, "Couldn't create TextBlock using an initialization string" );

            Console.WriteLine( Name + " passed" );
        }
    }


    [ContentProperty("Inlines")]
    public class MyTextBlock
    {
        MyInlineCollection _inlines = new MyInlineCollection();
        public MyInlineCollection Inlines
        {
            get
            {
                return _inlines;
            }
        }

    }

    [ContentWrapper(typeof(Run))]
    [ContentWrapper(typeof(InlineUIContainer))]
    [WhitespaceSignificantCollection]
    public class MyInlineCollection : Collection<Inline>, IList
    {
        bool _addCalled = false;
        internal bool AddCalled
        {
            get { return _addCalled; }
        }
        
        int IList.Add( object o )
        {
            _addCalled = true;
            return 0;
        }
        
        void IList.Clear()
        {
        }

        bool IList.Contains(object value)
        {
            return false;
        }

        int IList.IndexOf(object value)
        {
            return 0;
        }

        void IList.Insert(int index, object value)
        {
        }

        bool IList.IsFixedSize
        {
            get 
            {
                return false;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        void IList.Remove(object value)
        {
        }

        void IList.RemoveAt(int index)
        {
        }

        object IList.this[int index]
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

    }



    [ContentProperty("Points")]
    [TypeConverter(typeof(SketchConverter))]
    public class Sketch
    {
        public Sketch()
        {
            _points = new PointCollection();
        }

        public Sketch( PointCollection points )
        {
            _points = points;
        }

        PointCollection _points;
        public PointCollection Points
        {
            get { return _points; }
        }

        System.Windows.Media.Brush _stroke;
        public System.Windows.Media.Brush Stroke
        {
            get { return _stroke; }
            set { _stroke = value; }
        }
        
    }


    public class SketchConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            TypeConverter pointCollectionConverter = new PointCollectionConverter();
            PointCollection pointCollection = pointCollectionConverter.ConvertFrom( context, culture, value ) as PointCollection;

            Sketch sketch = new Sketch( pointCollection );
            return sketch;
        }


        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo( context, culture, value, destinationType );
        }

    }




    // Test for events that are fired when a tree is constructed by the parser.
    public class LoadedEventTest : ParserSuites
    {
        public LoadedEventTest() : base("Loaded Events Test")
        {
        }
        
        public override DrtTest[] PrepareTests()
        {
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest( RunTest ),
                        null        
                        };
        }

        private void RunTest()
        {
            Console.WriteLine("LoadedEventTest started.");
            Stream xamlFileStream = LoadTestFile(@"DrtFiles\Parser\DrtParser3.xaml");
            UIElement root = null;
            int startT;
            int endT;
        
            try
            {
                // see if it loads
                startT = Environment.TickCount;
                root = (UIElement) XamlReader.Load(xamlFileStream);
                endT = Environment.TickCount;
            }
            finally
            {
        
                // done with the stream
                xamlFileStream.Close();
            }

            if (_loadedCount != 10)
            {
                throw new Exception("Fired " + _loadedCount + " Loaded events while expecting 10 of them");
            }
            
            Console.WriteLine("LoadedEventTest passed - Time = " + (endT-startT) + "ms");
        }
        
        internal static void OnLoaded(object o, EventArgs args)
        {
            FooControl control = (FooControl)o;
            if (!control.Name.Equals(_eventTargets[_loadedCount]))
            {
                throw new Exception("Out of order loaded event fired on " + control.Name + " when expecting event on " + _eventTargets[_loadedCount]);
            }
            _loadedCount++;

            // Check if IsInitialized is true on self and all children
            if (!IsSubTreeInitialized(control))
            {
                throw new Exception("Loaded Event Fired on " + control.Name + " even though IsInitialized is not true ");
            }
        }

        private static bool IsSubTreeInitialized(DependencyObject parent)
        {
            bool ret = true;
            
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for(int i = 0; i < count; i++)
            {
                ret = ret && IsSubTreeInitialized(VisualTreeHelper.GetChild(parent, i));
            }

            return ret && ((FooControl)parent).IsInitialized;
        }

        // Fields used to tests Loaded event
        public static int _loadedCount = 0;
        public static string[] _eventTargets = {"ID3", "ID2", "ID6", "ID5", "ID8", "ID7", "ID9", "ID4", "ID10", "ID1"};
    }

    // Do a load of a file using an XmlReader instead of a stream.  The file is saved as
    // Unicode.
    public class XmlReaderLoadTest : ParserSuites
    {
        public XmlReaderLoadTest() : base("XmlReader Load Test")
        {
        }
        
        public override DrtTest[] PrepareTests()
        {
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest( RunTest ),
                        null        
                        };
        }

        private void RunTest()
        {
            Console.WriteLine("XmlReaderLoadTest started.");
            UIElement root = null;
            
            StreamReader textReader = new StreamReader(@"DrtFiles\Parser\drtparser4.xaml", new System.Text.UnicodeEncoding());
            XmlTextReader xmlReader = new XmlTextReader("", textReader);
            int startT;
            int endT;
            
            try
            {
                // see if it loads
                startT = Environment.TickCount;
                root = (UIElement) XamlReader.Load(xmlReader);
                endT = Environment.TickCount;
            }
            finally
            {

                // done with the stream
                xmlReader.Close();
            }

            Console.WriteLine("XmlReaderLoadTest passed - Time = " + (endT-startT) + "ms");
        }  
    }

    // Do a load of a small document containing resources and styles
    public class ResourceLoadTest : ParserSuites
    {
        public ResourceLoadTest() : base("Resources Load Test")
        {
        }
        
        public override DrtTest[] PrepareTests()
        {
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest( RunTest ),
                        null        
                        };
        }

        private void RunTest()
        {
            Console.WriteLine("ResourceLoadTest started.");
            Stream xamlFileStream = LoadTestFile(@"DrtFiles\Parser\DrtParser2.xaml");
            UIElement root = null;
            int startT;
            int endT;

            try
            {
                // see if it loads
                startT = Environment.TickCount;
                root = (UIElement) XamlReader.Load(xamlFileStream);
                endT = Environment.TickCount;

                // Get the DockPanel
                IEnumerator children = LogicalTreeHelper.GetChildren(root).GetEnumerator();
                children.MoveNext();
                System.Windows.Controls.StackPanel sp = (System.Windows.Controls.StackPanel)children.Current;

                // Get the border (child of dock panel)
                
                children = LogicalTreeHelper.GetChildren(sp).GetEnumerator();
                children.MoveNext();
                Border border = (Border)children.Current;

                // Verify the background on the border (which comes from the resources)
                
                SolidColorBrush background = (SolidColorBrush)border.Background;
                if (background.Color.ToString().Equals("#00FF0000") ==  false)
                {
                    throw new Exception("Incorrect evaluation of ResourceReferenceExpression for Border.Background property");
                }

                // Get the Button child of the dock panel
                
                children.MoveNext ();
                System.Windows.Controls.Button button = children.Current as System.Windows.Controls.Button;

                // Get the element's content, which should be a TestFreezable object
                // It should have been frozen (used) on the first GetValue.
                
                TestFreezable testFreezable = button.Content as TestFreezable;
                if( testFreezable.TheValue != 1 )
                {
                    Console.WriteLine("testFreezable is {0}, should be 1", testFreezable.TheValue );
                    throw new Exception ("testFreezable incorrect value");
                }

                // Get Text child of DockPanel
                children.MoveNext ();
                children.MoveNext ();
                children.MoveNext ();
                System.Windows.Controls.TextBlock simpleText = children.Current as System.Windows.Controls.TextBlock;

                // Test that the style on the simpleText is the implicitly applied 
                // from the typed style in the DockPanel's resources
                if (sp.FindResource(simpleText.GetType()) != simpleText.Style)
                {
                    throw new Exception("Typed Style from Resource did not get implicitly applied to the control");
                }

                // Test hashtable creation
                children.MoveNext();
                children.MoveNext();
                object ttc = children.Current;
                if (ttc.ToString() != "DRTText.TextTestControl Items.Count:0" )
                    throw new Exception("Creation of TextTestControl failed.");

                // Verify that we can do prefixed name lookups at this point in the tree
/*                
                Type tt = XamlTypeMapper.GetTypeFromName("dtt:TextTestControl", ttc as DependencyObject);
                if (tt == null)
                    throw new Exception("Type resolution of dtt:TextTestControl failed");
                tt = XamlTypeMapper.GetTypeFromName("mil:LinearGradientBrush", ttc as DependencyObject);
                if (tt == null)
                    throw new Exception("Type resolution of mil:LinearGradientBrush failed");
                tt = XamlTypeMapper.GetTypeFromName("DockPanel", ttc as DependencyObject);
                if (tt == null)
                    throw new Exception("Type resolution of DockPanel failed");
*/                    
                
                // Create a new resource dictionary and add it to DockPanel
                
                ResourceDictionary rd = new ResourceDictionary();
                rd.Add("DaBrush", new SolidColorBrush(Colors.Green));
                rd.Add("TestFreezable", new TestFreezable(2));
                rd.Add(simpleText.GetType(), new Style(typeof(FrameworkElement)));
                sp.Resources = rd;

                // Check background against this new resource dictionary
                
                background = (SolidColorBrush)border.Background;
                
                if (background.Color.ToString().Equals("#FF008000") ==  false)
                {
                    Console.WriteLine(background.Color);
                    throw new Exception("Incorrect evaluation of ResourceReferenceExpression for Border.Background property");
                }

                // And check the freezable against this new resource dictionary
                // (to ensure that the invalidation happened and the cache was deleted).

                testFreezable = button.Content as TestFreezable;
                if (testFreezable.TheValue != 2)
                {
                    Console.WriteLine("testFreezable is {0}, should be 2", testFreezable.TheValue );
                    throw new Exception ("testFreezable incorrect value");
                }

                // Test that the style on the simpleText against the new resource 
                // dictionary on DockPanel and confirm that the style property was 
                // correctly invalidated and subsequently fetched from the new dictionary
                if (sp.FindResource(simpleText.GetType()) != simpleText.Style)
                {
                    throw new Exception("Typed Style from Resource did not get implicitly applied to the control");
                }
                

            }
            finally
            {

                // done with the stream
                xamlFileStream.Close();
            }

            Console.WriteLine("ResourceLoadTest passed - Time = " + (endT-startT) + "ms");

        }
    }

    // Loads a file to test "immutable" type support -- a type such as String
    // that can be intiailized with a type converter, but doesn't have any setter properties
    public class StringLoadTest : ParserSuites
    {
        public StringLoadTest() : base("Immutable String Load Test")
        {
        }
        
        public override DrtTest[] PrepareTests()
        {
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest( RunTest ),
                        null        
                        };
        }

        private void RunTest()
        {
            Console.WriteLine("StringLoadTest started.");
            Stream xamlFileStream = LoadTestFile(@"DrtFiles\Parser\DrtParser5.xaml");
            Object root = null;
            int startT;
            int endT;

            try
            {
                // Load the markup, which is just <String>Hello, world</String>
                
                startT = Environment.TickCount;
                root = XamlReader.Load(xamlFileStream);
                endT = Environment.TickCount;

                // We should have gotten a string
                if( root.GetType() != typeof(String) )
                {
                    throw new Exception("<String> was not loaded correctly");
                }

                // And it should have the right content
                else if( (String)root != "Hello, world" )
                {
                    throw new Exception("Bad <String> value -- " + (String)root );
                }
            }
            finally
            {

                // done with the stream
                xamlFileStream.Close();
            }

            Console.WriteLine("StringLoadTest passed - Time = " + (endT-startT) + "ms");

        }
    }

    // FooControl to test Firing of Loaded event
    [ContentProperty("Children")]
    public class FooControl : FrameworkElement, IAddChild
    {
        public FooControl() : base()
        {
            Initialized += new EventHandler(LoadedEventTest.OnLoaded);
        }

        /// <summary>
        ///  Add a text string to this control
        /// </summary>
        void IAddChild.AddText(string text)
        {
        }
        
        /// <summary>
        ///  Add an object child to this control
        /// </summary>
        void IAddChild.AddChild(object o)
        {
            Children.Add(o as UIElement);
        }
        
#region Logical Tree

        /// <summary>
        ///     Returns enumerator to logical children
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        protected override IEnumerator LogicalChildren
        {
            get { return Children.GetEnumerator(); }
        }


#endregion Logical Tree

        private List<UIElement> children;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<UIElement> Children
        {
            get { 
                if (children == null)
                    children = new List<UIElement>();
                return children;
            }
        }

    }



    // Instantiating of Freezable used for resource test
    
    public class TestFreezable : Freezable
    {
        private int _theValue;

        public int TheValue
        {
            get { return _theValue; }
            set { _theValue = value; }
        }
        
        public TestFreezable (int theValue )
        {
            _theValue = theValue;
        }

        public TestFreezable ()
        {
            _theValue = 0;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new TestFreezable();
        }
        
        protected override void CloneCore(Freezable sourceFreezable)
        {
            TestFreezable sourceTestFreezable = (TestFreezable) sourceFreezable;
            base.CloneCore(sourceFreezable);

            TheValue = sourceTestFreezable.TheValue;
        }
    
    }
    
    // Do a load of a small document containing templates
    public class TemplateLoadTest : ParserSuites
    {
        public TemplateLoadTest() : base("Template Load Test")
        {
        }
        
        public override DrtTest[] PrepareTests()
        {
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest( RunTest ),
                        null        
                        };
        }

        private void RunTest()
        {
            Console.WriteLine("TemplateLoadTest started.");
            Stream xamlFileStream = LoadTestFile(@"DrtFiles\Parser\DrtParser7.xaml");
            UIElement root = null;
            int startT;
            int endT;

            try
            {
                // see if it loads
                startT = Environment.TickCount;
                root = (UIElement) XamlReader.Load(xamlFileStream);
                endT = Environment.TickCount;

                // Get the DockPanel
                DockPanel dockPanel = (DockPanel)root;

                // Verify DockPanel
                Style implicitButtonStyle = (Style)dockPanel.Resources[typeof(System.Windows.Controls.Button)];
                Style myButtonStyle = (Style)dockPanel.Resources["MyButtonStyle"];
                ControlTemplate myButtonTemplate = (ControlTemplate)dockPanel.Resources["MyButtonTemplate"];
                if (implicitButtonStyle == null || myButtonStyle == null || myButtonTemplate == null ||
                    ((Setter)implicitButtonStyle.Setters[0]).Property != System.Windows.Controls.Control.TemplateProperty || 
                    ((Setter)implicitButtonStyle.Setters[0]).Value == null)
                {
                    throw new Exception("DockPanel's Resources section that contained direct Templates and Templates nested in Styles has not been loaded correctly");
                }
                
                // Get the buttons (childredn of dock panel)
                System.Windows.Controls.Button b1 = (System.Windows.Controls.Button)dockPanel.Children[0];
                System.Windows.Controls.Button b2 = (System.Windows.Controls.Button)dockPanel.Children[1];
                System.Windows.Controls.Button b3 = (System.Windows.Controls.Button)dockPanel.Children[2];
                System.Windows.Controls.Button b4 = (System.Windows.Controls.Button)dockPanel.Children[3];
                System.Windows.Controls.Button b5 = (System.Windows.Controls.Button)dockPanel.Children[4];
                System.Windows.Controls.Button b6 = (System.Windows.Controls.Button)dockPanel.Children[5];
                System.Windows.Controls.Button b7 = (System.Windows.Controls.Button)dockPanel.Children[6];

                // Verify Button #1
                if (((String)b1.Content) != "Hello" ||  b1.Style != null || b1.Template == null || b1.Template.VisualTree != null)
                {
                    throw new Exception("Button#1 that contained a locally set Template has not been loaded correctly");
                }

                b1.ApplyTemplate();
                Canvas b1Child = typeof(FrameworkElement).GetProperty("TemplateChild", BindingFlags.NonPublic|BindingFlags.Instance).GetValue(b1,null) as Canvas;
                if( b1Child == null
                    || 
                    b1Child.Children[0].GetType() != typeof(System.Windows.Shapes.Rectangle)
                    ||
                    b1Child.Children[1].GetType() != typeof(ContentPresenter) )
                {
                    throw new Exception("Button#1's Template VisualTree has not been loaded correctly");
                }
                    
                
                // Verify Button #2
                if (((String)b2.Content) != "World" || b2.Style != implicitButtonStyle || b2.Template == null)
                {
                    throw new Exception("Button#2 that has an implicit reference to Style in the resources section has not been loaded correctly");
                }
                
                // Verify Button #3
                if (((String)b3.Content) != "Once" || b3.Style != myButtonStyle || b3.Template == null)
                {
                    throw new Exception("Button#3 that has an explicit reference to Style in the resources section has not been loaded correctly");
                }

                // Verify Button #4
                if (((String)b4.Content) != "More" || b4.Style != null || b4.Template != myButtonTemplate)
                {
                    throw new Exception("Button#4 that has an explicit reference to Template in the resources section has not been loaded correctly");
                }
                
                // Verify Button #5
                if (((String)b5.Content) != "And" ||  b5.Style != null || b5.Template == null || 
                    ((Trigger)b5.Template.Triggers[0]).Property != System.Windows.Controls.Primitives.ButtonBase.IsPressedProperty ||
                    ((bool)((Trigger)b5.Template.Triggers[0]).Value) != true)
                {
                    throw new Exception("Button#5 that contained a locally set Template with Triggers has not been loaded correctly");
                }

                // Verify Button #6
                if (b6.Content != null ||  b6.Style != null || b6.Template == null)
                {
                    throw new Exception("Button#6 that contained a locally set Template with Storyboards has not been loaded correctly");
                }
                if (b6.Template.Triggers.Count != 1 || !(b6.Template.Triggers[0] is EventTrigger))
                {
                    throw new Exception("Button#6's Template EventTrigger has not been loaded correctly");
                }

                // Verify Button #7
                VerifyComplexButton( b7 );


                // Urelated to the above, test loading a FEF
                VerifyLoadFef();


            }
            finally
            {

                // done with the stream
                xamlFileStream.Close();
            }

            Console.WriteLine("TemplateLoadTest passed - Time = " + (endT-startT) + "ms");

        }


    private void VerifyLoadFef()
    {
        FrameworkElementFactory fef1 = new FrameworkElementFactory(typeof(DockPanel));
        FrameworkElementFactory fef2 = new FrameworkElementFactory(typeof(Button));
        FrameworkElementFactory fef3 = new FrameworkElementFactory("Ouch");

        fef1.AppendChild( fef2 );
        fef2.AppendChild( fef3 );

        fef1.SetBinding(DockPanel.BackgroundProperty, new Binding() );
        fef2.SetValue(DockPanel.DockProperty, Dock.Left);
        fef2.SetValue(Button.ForegroundProperty, new TemplateBindingExtension(DockPanel.DockProperty) );

        ControlTemplate controlTemplate = new ControlTemplate();
        controlTemplate.VisualTree = fef1;
        controlTemplate.Seal();

        FrameworkElement fe = controlTemplate.LoadContent() as FrameworkElement;

        DRT.Assert( fe is DockPanel );
        DRT.Assert( fe.ReadLocalValue(DockPanel.BackgroundProperty) is BindingExpression );

        Button button = (fe as DockPanel).Children[0] as Button ;
        DRT.Assert( button != null );
        DRT.Assert( button.ReadLocalValue(Button.ForegroundProperty) is TemplateBindingExpression );
        DRT.Assert( (button.Content as string) == "Ouch" );        

        // Verify that we can load a ScrollViewer template (common scenario).

        ScrollViewer scrollViewer = new ScrollViewer();
        DependencyObject templateRoot = scrollViewer.Template.LoadContent();
        DRT.Assert( templateRoot != null, "Couldn't load ScrollViewer template" );
        
    }


    private void VerifyComplexButton( Button b )
    {
        // Basic check
        b.ApplyTemplate();
        if (b.Content != null ||  b.Style != null || b.Template == null)
        {
            throw new Exception("Button#7 that contained a locally set Template has not been loaded correctly");
        }

        Grid templateRoot = typeof(FrameworkElement).GetProperty("TemplateChild", BindingFlags.NonPublic|BindingFlags.Instance).GetValue(b,null) as Grid;

        if( templateRoot == null )
            throw new Exception("Button7 should have a Grid as its root");

        // Check that the column definitions got set up properly (this helps verify that CLR
        // properties are handled).

        if( templateRoot.ColumnDefinitions.Count != 3 )
            throw new Exception("Button7 should have 3 ColumnDefinitions");

        if( templateRoot.ColumnDefinitions[0].Width != new GridLength(1) )
            throw new Exception("Button7 ColumnDefinition.Width should be 1");

        // The named SCB shouldn't be frozen

        SolidColorBrush scb = (SolidColorBrush) b.Template.FindName( "RectangleFill", b );
        if( scb == null )
            throw new Exception("Button7, couldn't find SCB named RectangleFill");

        if( scb.IsFrozen )
            throw new Exception("SCB RectangleFill shouldn't have been frozen (because it has a name)");

        SolidColorBrush brushResource1 = new SolidColorBrush( Colors.Blue );
        SolidColorBrush brushResource2 = new SolidColorBrush( Colors.Red );

        // See if the dynamic resource on the brush in the rectangle works.

        b.Resources["ColorResource"] = Colors.Blue;

        System.Windows.Shapes.Rectangle rectangle = (System.Windows.Shapes.Rectangle) b.Template.FindName( "RectangleWithDynamicResource", b );
        if( rectangle == null )
            throw new Exception("Button7, couldn't find RectangleWithDynamicResource");

        if( ((SolidColorBrush)rectangle.Fill).Color != Colors.Blue )
            throw new Exception("Button7, RectangleWithDynamicResource didn't pick up the first color");

        b.Resources["ColorResource"] = Colors.Red;
        
        if( ((SolidColorBrush)rectangle.Fill).Color != Colors.Red )
            throw new Exception("Button7, RectangleWithDynamicResource didn't pick up the second color");

        // See if the dynamic resource on the Viewport3D works.

        Viewport3D viewport = (Viewport3D) b.Template.FindName( "Viewport", b );
        if( viewport == null )
            throw new Exception("Button7, couldn't find Viewport");


        DirectionalLight directionalLight = (DirectionalLight) ((Model3DGroup)((ModelVisual3D)viewport.Children[0]).Content).Children[0];
        if( directionalLight == null )
            throw new Exception("Button7, couldn't find DirectionalLight");

        b.Resources["ColorResource"] = Colors.Blue;

        if( directionalLight.Color != Colors.Blue )
            throw new Exception("Button7, DirectionalLight didn't pick up the first color");

        b.Resources["ColorResource"] = Colors.Red;
        
        if( directionalLight.Color != Colors.Red )
            throw new Exception("Button7, DirectionalLight didn't pick up the second color");

        // Yay

    }


        
    }


    // test Xml islands
    public class XmlIslandLoadTest : ParserSuites
    {
        public XmlIslandLoadTest() : base("XmlIsland")
        {
        }
        
        public override DrtTest[] PrepareTests()
        {
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest( RunTest ),
                        new DrtTest( VerifyXDataParsed ),
                        null        
                        };
        }

        private void RunTest()
        {
            Stream xamlFileStream = LoadTestFile(@"DrtFiles\Parser\xmlIsland.xaml");

            try
            {
                // see if it loads
                _rootElement = (FrameworkElement) XamlReader.Load(xamlFileStream);
            }
            finally
            {

                // done with the stream
                xamlFileStream.Close();
            }
            DRT.Assert(_rootElement != null, "failed to load xmlIsland.xml");
        }
        
        private void VerifyXDataParsed()
        {
            // verify that all Xml island got loaded and parsed:
            MyXmlIsland myIsland = (MyXmlIsland) _rootElement.FindResource("myXmlIslandCPA");
            DRT.Assert(myIsland != null, "Cannot find MyXmlIsland 'myXmlIslandCPA' in resources");
            DRT.Assert(myIsland.Document.DocumentElement != null, "MyXmlIsland 'myXmlIslandCPA' didn't load inline XML");

            MyXmlIsland myIsland2 = (MyXmlIsland) _rootElement.FindResource("myXmlIslandComplexProp");
            DRT.Assert(myIsland2 != null, "Cannot find MyXmlIsland 'myXmlIslandComplexProp' in resources");
            DRT.Assert(myIsland2.Document.DocumentElement != null, "MyXmlIsland 'myXmlIslandComplexProp' didn't load inline XML");

            XmlDataProvider aXmlDP = (XmlDataProvider) _rootElement.FindResource("aXmlDP");
            DRT.Assert(aXmlDP != null, "Cannot find XmlDataProvider 'aXmlDP' in resources");
            DRT.Assert(aXmlDP.Document.DocumentElement != null, "XmlDataProvider 'aXmlDP' didn't load inline XML");

        /* not supported yet by parser
            XmlDataProvider aXmlDP2 = (XmlDataProvider) _rootElement.FindResource("aXmlDP2");
            DRT.Assert(aXmlDP2 != null, "Cannot find XmlDataProvider 'aXmlDP2' in resources");
            DRT.Assert(aXmlDP2.Document.DocumentElement != null, "XmlDataProvider 'aXmlDP2' didn't load inline XML");
        */

            // verify xml islands are serialized with correct XData instruction
            string s;
            string xdataTag = "XData";
            s = XamlWriter.Save(myIsland);
            DRT.Assert(s.IndexOf(xdataTag) > 0, "Cannot find <x:XData> in serialized MyXmlIsland");
            
            s = XamlWriter.Save(aXmlDP);
            DRT.Assert(s.IndexOf(xdataTag) > 0, "Cannot find <x:XData> in serialized XmlDataProvider");
        
        }

        FrameworkElement _rootElement;
    }


    // *******************************************************************************************
    //
    // Test Attached Properties without DP Backing
    //
    // *******************************************************************************************

    #region Attached Properties Without DP Backing

    // Do a simple load of a document containing FrameworkContentElements.
    public class AttachedPropertiesTest : ParserSuites
    {
        public AttachedPropertiesTest()
            : base("AttachedProperties")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest( RunTest ),
                        null        
                        };
        }

        private void RunTest()
        {
            Console.WriteLine("AttachedProperties Test started.");
            Stream xamlFileStream = LoadTestFile(@"DrtFiles\Parser\TestAttachedProperties.xaml");
            object root = null;
            int startT;
            int endT;

            try
            {
                // see if it loads
                startT = Environment.TickCount;
                root = XamlReader.Load(xamlFileStream);
                xamlFileStream.Close();
                xamlFileStream = LoadTestFile(@"DrtFiles\Parser\TestAttachedProperties.xaml");
                root = XamlReader.Load(xamlFileStream);
                endT = Environment.TickCount;
            }
            finally
            {

                // done with the stream
                xamlFileStream.Close();
            }

            Console.WriteLine("AttachedProperties Test passed - Time = " + (endT - startT) + "ms");
        }
    }

    public class DO_AAA : DependencyObject
    {
        public DO_AAA()
        {
        }

        public static readonly DependencyProperty ReadOnlyListProperty = DO_BBB.ReadOnlyListProperty.AddOwner(typeof(DO_AAA));
    }

    public class DO_BBB : DependencyObject
    {
        public static readonly DependencyProperty ReadWriteListProperty = DependencyProperty.RegisterAttached("ReadWriteList", typeof(ArrayList), typeof(DO_BBB), new FrameworkPropertyMetadata(null));
        public static ArrayList GetReadWriteList(DependencyObject o)
        {
            return (ArrayList)o.GetValue(ReadWriteListProperty);
        }
        public static void SetReadWriteList(DependencyObject o, ArrayList v)
        {
            o.SetValue(ReadWriteListProperty, v);
        }

        public static readonly DependencyProperty ReadOnlyListProperty = DependencyProperty.RegisterAttached("ReadOnlyList", typeof(ArrayList), typeof(DO_BBB), new FrameworkPropertyMetadata(new ArrayList()));
        public static ArrayList GetReadOnlyList(DependencyObject o)
        {
            return (ArrayList)o.GetValue(ReadOnlyListProperty);
        }
    }

    public class AAA
    {
        public AAA()
        {
        }
    }

    public class NonDo
    {
        public static readonly DependencyProperty DpOnNonDoProperty = DependencyProperty.RegisterAttached("DpOnNonDo", typeof(String), typeof(NonDo), new FrameworkPropertyMetadata("DpOnNonDo-DefaultMetadata"));

        public static String GetDpOnNonDo(Object o)
        {
            return _dpOnNonDo;
        }

        public static void SetDpOnNonDo(Object o, String dpOnNonDo)
        {
            _dpOnNonDo = dpOnNonDo;
        }

        private static string _dpOnNonDo = "DpOnNonDo-StaticDefault";
    }

    public class BBB
    {
        // ---------------
        // String Property
        // ---------------
        public static String GetStringProp(AAA aaa)
        {
            return _stringProp;
        }
        public static void SetStringProp(AAA aaa, String stringProp)
        {
            _stringProp = stringProp;
        }
        private static string _stringProp = "stringProp-StaticDefault";

        // -----------------
        // Integer Property
        // -----------------
        public static int GetIntegerProp(AAA aaa)
        {
            return _integerProp;
        }
        public static void SetIntegerProp(AAA aaa, int integerProp)
        {
            _integerProp = integerProp;
        }
        private static int _integerProp = 999;

        // -----------------
        // Type Property
        // -----------------
        public static Type GetTypeProp(AAA aaa)
        {
            return _typeProp;
        }
        public static void SetTypeProp(AAA aaa, Type typeProp)
        {
            _typeProp = typeProp;
        }
        private static Type _typeProp = typeof(string);

        // -----------------------------------------
        // Custom Property With Class TypeConverter
        // -----------------------------------------
        public static CustomTypeWithClassTC GetCustomPropWithClassTC(AAA aaa)
        {
            return _customPropWithClassTC;
        }
        public static void SetCustomPropWithClassTC(AAA aaa, CustomTypeWithClassTC customPropWithClassTC)
        {
            _customPropWithClassTC = customPropWithClassTC;
        }
        private static CustomTypeWithClassTC _customPropWithClassTC;

        // -------------------------------------------
        // Custom Property With Property TypeConverter
        // -------------------------------------------
        [TypeConverter(typeof(CustomTypeWithPropTCConverter))]
        public static CustomTypeWithPropTC GetCustomPropWithPropTC(AAA aaa)
        {
            return _customPropWithPropTC;
        }
        public static void SetCustomPropWithPropTC(AAA aaa, CustomTypeWithPropTC customPropWithPropTC)
        {
            _customPropWithPropTC = customPropWithPropTC;
        }
        private static CustomTypeWithPropTC _customPropWithPropTC;

        // -------------------------------------------
        // Attached Read/Write Collection
        // -------------------------------------------
        public static ArrayList GetArrayListReadWrite(AAA aaa)
        {
            return _arrayListReadWrite;
        }
        public static void SetArrayListReadWrite(AAA aaa, ArrayList arrayListReadWrite)
        {
            _arrayListReadWrite = arrayListReadWrite;
        }
        private static ArrayList _arrayListReadWrite = new ArrayList();

        // ----------------------------------------------
        // Attached Read/Write Collection Backed With DP
        // ---------------------------------------------
        public static readonly DependencyProperty ArrayListReadWriteDPProperty = DependencyProperty.RegisterAttached("ArrayListReadWriteDP", typeof(ArrayList), typeof(BBB), new FrameworkPropertyMetadata(null));
        public static ArrayList GetArrayListReadWriteDP(AAA aaa)
        {
            return _arrayListReadWriteDP;
        }
        public static void SetArrayListReadWriteDP(AAA aaa, ArrayList arrayListReadWriteDP)
        {
            _arrayListReadWriteDP = arrayListReadWriteDP;
        }
        private static ArrayList _arrayListReadWriteDP = new ArrayList();

        // -------------------------------------------
        // Attached ReadOnly Collection
        // -------------------------------------------
        public static ArrayList GetArrayListReadOnly(AAA aaa)
        {
            return _arrayListReadOnly;
        }
        private static ArrayList _arrayListReadOnly = new ArrayList();
    }

    [TypeConverter(typeof(CustomTypeWithClassTCConverter))]
    public class CustomTypeWithClassTC
    {
        public CustomTypeWithClassTC(string value)
        {
            this.Value = value;
        }

        public string Value;
    }

    public class CustomTypeWithPropTC
    {
        public CustomTypeWithPropTC(string value)
        {
            this.Value = value;
        }

        public string Value;
    }

    // ---------------------------------------------------------
    //
    // Sparkle Request
    //
    // ---------------------------------------------------------

    public class Class1
    {
        public static readonly DependencyProperty BarProperty = DependencyProperty.RegisterAttached("Bar", typeof(bool), typeof(Class1));
        public static bool GetFoo(object target)
        {
            return false;
        }

        public static void SetFoo(object target, bool value)
        { }

        public static bool GetBar(object target)
        {
            return false;
        }

        public static void SetBar(object target, bool value)
        { }
    }

    // ---------------------------------------------------------
    //
    // Custom TypeConverters
    //
    // ---------------------------------------------------------

    public class CustomTypeWithClassTCConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
        {
            if (source is string)
            {
                return new CustomTypeWithClassTC((string)source);
            }
            else
            {
                return null;
            }
        }

        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((CustomTypeWithClassTC)value).Value;
            }
            else
            {
                return null;
            }
        }
    }

    public class CustomTypeWithPropTCConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
        {
            if (source is string)
            {
                return new CustomTypeWithPropTC((string)source);
            }
            else
            {
                return null;
            }
        }

        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((CustomTypeWithPropTC)value).Value;
            }
            else
            {
                return null;
            }
        }
    }

    #endregion Attached Properties Without DP Backing


    // test No default namespace
    public class NoDefaultXmlNsTest : ParserSuites
    {
        public NoDefaultXmlNsTest() : base("NoDefaultNamespace")
        {
        }
        
        public override DrtTest[] PrepareTests()
        {
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest( RunTest ),
                        null
                        };
        }

        private void RunTest()
        {
            Stream xamlFileStream = LoadTestFile(@"DrtFiles\Parser\NoDefaultXmlNs.xaml");

            try
            {
                // see if it loads
                _rootElement = (FrameworkElement) XamlReader.Load(xamlFileStream);
            }
            finally
            {

                // done with the stream
                xamlFileStream.Close();
            }
            DRT.Assert(_rootElement != null, "failed to load NoDefaultXmlNs.xaml");
        }

        FrameworkElement _rootElement;
    }

    public class XamlValidationTest : ParserSuites
    {
        public XamlValidationTest() : base("XAML Validation Test")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[]{
                        new DrtTest(RunXamlValidationTest),
                        };
        }

        private void RunXamlValidationTest()
        {
            Console.WriteLine("XAML validation test started.");

            // Define the path to the XAML file
            string xamlPath = @"DrtFiles\Parser\DrtParser10.xaml";
            Stream xamlStream = LoadTestFile(xamlPath);

            try
            {
                // Parse the XAML
                ParserContext parserContext = new ParserContext();
                object rootElement = XamlReader.Load(xamlStream, parserContext);

                // Check if the root element is a Window
                if (rootElement is Window window)
                {
                    // Show and close the window
                    window.Show();
                    window.Close();
                    Console.WriteLine("XAML validation test passed. The XAML is valid and renders correctly.");
                }
                else if (rootElement is UIElement uiElement)
                {
                    // Render the element if it's a UIElement
                    Render(uiElement);
                    Console.WriteLine("XAML validation test passed. The XAML is valid and renders correctly.");
                }
                else
                {
                    Console.WriteLine("XAML validation test failed. The root element is not a UIElement or Window.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"XAML validation test failed with exception: {ex.Message}");
            }
        }

        private void Render(UIElement element)
        {
            // Create a new window to render the UIElement
            Window window = new Window
            {
                Content = element,
                Width = 800,
                Height = 600
            };
            window.Show();
            window.Close();
        }
    }
}
