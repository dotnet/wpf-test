// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Tests Serialization and Resource Lookup
//
//

using System;
using System.IO;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Markup;
using System.Xml;

namespace DRT
{
    internal sealed class DrtSerializer : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            int ret = (new DrtSerializer()).Run(args);
            
            if (RefreshMode)
            {
                Console.WriteLine("\nNOTE: You will need to copy the Out_DrtSerializer_*.xaml files to Master_DrtSerializer_*.xaml in your source path");
            }

            return ret;
        }

        private DrtSerializer()
        {
            TeamContact = "WPF";
            Contact = "Microsoft";
            DrtName = "DrtMarkupSerializer";
            WindowTitle = "DrtMarkupSerializer";
            
            Suites = new DrtTestSuite[]{
                        new SerializationTestSuite(this, "DrtSerializer_1.xaml", "CustomFramework"),
                        new SerializationTestSuite(this, "DrtSerializer_2.xaml", "MIL"),
                        new SerializationTestSuite(this, "DrtSerializer_3.xaml", "Controls"),
                        new StylesSerializationTestSuite(this),
                        new SerializationTestSuite(this, "DrtSerializer_5.xaml", "Templates"),
                        new ResourceLookupTestSuite(this),
                        new ErrorValidationTestSuite(this),
                        };
        }

        protected override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            if ("refresh".Equals(arg))
            {
                RefreshMode = true;
                return true;
            }

            return base.HandleCommandLineArgument(arg, option, args, ref k);
        }

        public static bool RefreshMode = false;
        public const string XamlPath = @"DrtFiles\MarkupSerializer\";
    }

    internal class SerializationTestSuite : DrtTestSuite
    {
        internal SerializationTestSuite(DrtSerializer drt, String sourceFileName, String drtName): base(drtName)
        {
            this._drt = drt;
            this._sourceFileName = sourceFileName;
        }

        public override DrtTest[] PrepareTests()
        {
            Stream stream = null;

            try
            {
                // Prepare to load xaml content
                stream = File.OpenRead(DrtSerializer.XamlPath + _sourceFileName);
                
                // Load xaml markup
                _sourceTree = XamlReader.Load(stream, new ParserContext());
                
                // Assign it as Root Visual
                _drt.Show((Visual)_sourceTree);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(TestSerialization)
                        };
        }

        protected void TestSerialization()
        {
            StreamWriter writer = File.CreateText(DrtSerializer.XamlPath + "Out_" + _sourceFileName);
            XmlWriter xmlWriter = new XmlTextWriter(writer);
            
            try
            {
                // Save serialized xaml markup to file
                int startT = Environment.TickCount;
                XamlWriter.Save(_sourceTree, xmlWriter);
                int endT = Environment.TickCount;

                if (_drt.Verbose)
                    Console.WriteLine("Serialization Time = " + (endT-startT) + "ms");
            }
            finally
            {
                xmlWriter.Close();
                writer.Close();
            }

            // Skip verification in refresh mode
            if (!DrtSerializer.RefreshMode)
            {
                // Verify serialized content
                if (!CompareFiles(DrtSerializer.XamlPath + "Master_" + _sourceFileName, DrtSerializer.XamlPath + "Out_" + _sourceFileName))
                {
                    throw new Exception("Verification failed for " + _sourceFileName);
                }

                Stream stream = null;
                
                try
                {
                    // Prepare to load serialized xaml markup
                    stream = File.OpenRead(DrtSerializer.XamlPath + "Out_" + _sourceFileName);
                    
                    // Load serialized xaml markup
                    object serializedTree = XamlReader.Load(stream, new ParserContext());
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
        }

        private static bool CompareFiles(string fileName1, string fileName2)
        {
            Stream stream1 = null;
            Stream stream2 = null;
            
            try
            {
                stream1 = File.OpenRead(fileName1);
                stream2 = File.OpenRead(fileName2);
                
                return Microsoft.Test.Markup.CompareResult.Equivalent == Microsoft.Test.Markup.XamlComparer.Compare(stream1, stream2).Result;
            }
            finally
            {
                if (stream1 != null)
                    stream1.Close();
                
                if (stream2 != null)
                    stream2.Close();
            }
        }

        protected DrtSerializer          _drt;
        protected string                    _sourceFileName;
        protected object                   _sourceTree;
    }
    

    // Tests serialization of styles
    internal sealed class StylesSerializationTestSuite : SerializationTestSuite
    {
        internal StylesSerializationTestSuite(DrtSerializer drt): base(drt, "DrtSerializer_4.xaml", "Styles")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            Stream stream = null;

            try
            {
                // Prepare to load xaml content
                stream = File.OpenRead(DrtSerializer.XamlPath + _sourceFileName);
                
                // Load xaml markup
                _sourceTree = XamlReader.Load(stream, new ParserContext());
                
                // Attach some PropertySetters to the style
                Style s = new Style(typeof(Button));
                Setter setter = new Setter();
                setter.Property = Control.VerticalContentAlignmentProperty;
                setter.Value = VerticalAlignment.Top;
                s.Setters.Add(setter);

                setter = new Setter();
                setter.Property = Control.BackgroundProperty;
                GradientStopCollection stopColl = new GradientStopCollection();
                GradientStop stop = new GradientStop(Colors.AliceBlue, 0.3);
                stopColl.Add(stop);
                stop = new GradientStop(Colors.AntiqueWhite, 0.3);
                stopColl.Add(stop);
                stop = new GradientStop(Colors.Aqua, 0.3);
                stopColl.Add(stop);
                setter.Value = new LinearGradientBrush(stopColl);
                s.Setters.Add(setter);

                // Stick the style into a ResourceDictionary
                DockPanel dockPanel = (DockPanel)_sourceTree;
                dockPanel.Resources["MyButtonStyleWithSetters"] = s;

                // Create a button and add a ResourceReference for the above style
                Button b = new Button();
                b.SetResourceReference(FrameworkElement.StyleProperty, "MyButtonStyleWithSetters");

                // Add the button to the dockPanel
                dockPanel.Children.Add(b);

                // Assign it as Root Visual
                _drt.Show((Visual)_sourceTree);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(TestSerialization)
                        };
        }
    }
    
    // Tests resource lookup and invalidation within the tree
    internal sealed class ResourceLookupTestSuite : DrtTestSuite
    {
        internal ResourceLookupTestSuite(DrtSerializer drt): base("Resources")
        {
            this.drt = drt;
        }

        public override DrtTest[] PrepareTests()
        {
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(TestResourceLookup)
                        };
        }

        private void TestResourceLookup()
        {
            // Check simple Resource References
            {
                MyFrameworkElement fe = new MyFrameworkElement();

                ResourceDictionary dict = new ResourceDictionary();
                dict["one"] = 1;
                dict["two"] = 2.0;
                fe.Resources = dict;

                MyFrameworkElement feChild = new MyFrameworkElement();

                feChild.SetResourceReference(GreekStandard.BetaProperty, "one");
                feChild.SetResourceReference(GreekStandard.DeltaProperty, "two");

                fe.AppendModelChild(feChild);

                Check(fe.Beta == 0);
                Check(fe.Delta == 0.0);

                Check(feChild.Beta == 1);
                Check(feChild.Delta == 2.0);
            }

            // Check implicit Style References
            {
                MyFrameworkElement fe = new MyFrameworkElement();

                Style s = new Style(typeof(MyFrameworkElement));
                s.Setters.Add (new Setter(GreekStandard.BetaProperty, 1));
                s.Setters.Add (new Setter(GreekStandard.DeltaProperty, 2.0));

                ResourceDictionary dict = new ResourceDictionary();
                dict[typeof(MyFrameworkElement)] = s;
                fe.Resources = dict;

                Check(fe.Beta == 1);
                Check(fe.Delta == 2.0);
            }

            // Check implicit Style Reference coupled with dynamic resource reference in styles
            {
                MyFrameworkElement fe = new MyFrameworkElement();

                Style s = new Style(typeof(MyFrameworkElement));
                s.Setters.Add (new Setter(GreekStandard.BetaProperty, new DynamicResourceExtension("one")));
                s.Setters.Add (new Setter(GreekStandard.DeltaProperty, new DynamicResourceExtension("two")));

                ResourceDictionary dict = new ResourceDictionary();
                dict["one"] = 1;
                dict["two"] = 2.0;
                dict[typeof(MyFrameworkElement)] = s;
                fe.Resources = dict;

                MyFrameworkElement feChild = new MyFrameworkElement();
                fe.AppendModelChild(feChild);

                Check(fe.Beta == 1);
                Check(fe.Delta == 2.0);

                Check(feChild.Beta == 1);
                Check(feChild.Delta == 2.0);
            }

            // Check impact of changing Resource Tables on resource references
            {
                MyFrameworkElement fe = new MyFrameworkElement();

                ResourceDictionary dict = new ResourceDictionary();
                dict["one"] = 1;
                dict["two"] = 2.0;
                fe.Resources = dict;

                MyFrameworkElement feChild = new MyFrameworkElement();

                feChild.SetResourceReference(GreekStandard.BetaProperty, "one");
                feChild.SetResourceReference(GreekStandard.DeltaProperty, "two");

                fe.AppendModelChild(feChild);

                Check(fe.Beta == 0);
                Check(fe.Delta == 0.0);

                Check(feChild.Beta == 1);
                Check(feChild.Delta == 2.0);

                // Insert a dictionary on the child
                dict = new ResourceDictionary();
                dict["one"] = 10;
                dict["two"] = 20.0;
                feChild.Resources = dict;

                Check(fe.Beta == 0);
                Check(fe.Delta == 0.0);

                Check(feChild.Beta == 10);
                Check(feChild.Delta == 20.0);

                // Replace the dictionary on the child
                dict = new ResourceDictionary();
                dict["one"] = 100;
                dict["two"] = 200.0;
                feChild.Resources = dict;

                Check(fe.Beta == 0);
                Check(fe.Delta == 0.0);

                Check(feChild.Beta == 100);
                Check(feChild.Delta == 200.0);

                // Replace with empty dictionary
                feChild.Resources = new ResourceDictionary();

                Check(fe.Beta == 0);
                Check(fe.Delta == 0.0);

                Check(feChild.Beta == 1);
                Check(feChild.Delta == 2.0);

                // Remove the dictionary on the child
                feChild.Resources = null;

                Check(fe.Beta == 0);
                Check(fe.Delta == 0.0);

                Check(feChild.Beta == 1);
                Check(feChild.Delta == 2.0);
            }

            // Check impact of modifying a Resource Table entry on resource references
            {
                MyFrameworkElement fe = new MyFrameworkElement();

                ResourceDictionary dict = new ResourceDictionary();
                dict["one"] = 1;
                fe.Resources = dict;

                MyFrameworkElement feChild = new MyFrameworkElement();

                feChild.SetResourceReference(GreekStandard.BetaProperty, "one");
                feChild.SetResourceReference(GreekStandard.DeltaProperty, "two");

                fe.AppendModelChild(feChild);

                Check(fe.Beta == 0);
                Check(fe.Delta == 0.0);

                Check(feChild.Beta == 1);
                Check(feChild.Delta == 0.0);

                // Insert an entry in the dictionary
                fe.Resources.Add("two", 2.0);

                Check(fe.Beta == 0);
                Check(fe.Delta == 0.0);

                Check(feChild.Beta == 1);
                Check(feChild.Delta == 2.0);

                // Remove an entry from the dictionary
                fe.Resources.Remove("one");

                Check(fe.Beta == 0);
                Check(fe.Delta == 0.0);

                Check(feChild.Beta == 0);
                Check(feChild.Delta == 2.0);

                // Replace an entry with another
                fe.Resources["two"] = 20.0;

                Check(fe.Beta == 0);
                Check(fe.Delta == 0.0);

                Check(feChild.Beta == 0);
                Check(feChild.Delta == 20.0);
            }

            // Check impact of changing Resource Tables on implicit style references
            {
                MyFrameworkElement fe = new MyFrameworkElement();

                Style s = new Style(typeof(MyFrameworkElement));
                s.Setters.Add (new Setter(GreekStandard.BetaProperty, 1));
                s.Setters.Add (new Setter(GreekStandard.DeltaProperty, 2.0));

                ResourceDictionary dict = new ResourceDictionary();
                dict[typeof(MyFrameworkElement)] = s;
                fe.Resources = dict;

                Check(fe.Beta == 1);
                Check(fe.Delta == 2.0);

                // Replace one dictionary with another
                s = new Style(typeof(MyFrameworkElement));
                s.Setters.Add (new Setter(GreekStandard.BetaProperty, 10));
                s.Setters.Add (new Setter(GreekStandard.DeltaProperty, 20.0));

                dict = new ResourceDictionary();
                dict[typeof(MyFrameworkElement)] = s;
                fe.Resources = dict;

                Check(fe.Beta == 10);
                Check(fe.Delta == 20.0);
            }

            // Check impact of modifying a Resource Table entry on implicit style references
            {
                MyFrameworkElement fe = new MyFrameworkElement();

                Style s = new Style(typeof(MyFrameworkElement));
                s.Setters.Add (new Setter(GreekStandard.BetaProperty, 1));
                s.Setters.Add (new Setter(GreekStandard.DeltaProperty, 2.0));

                ResourceDictionary dict = new ResourceDictionary();
                dict[typeof(MyFrameworkElement)] = s;
                fe.Resources = dict;

                Check(fe.Beta == 1);
                Check(fe.Delta == 2.0);

                // Replace an entry with another
                s = new Style(typeof(MyFrameworkElement));
                s.Setters.Add (new Setter(GreekStandard.BetaProperty, 10));
                s.Setters.Add (new Setter(GreekStandard.DeltaProperty, 20.0));

                fe.Resources[typeof(MyFrameworkElement)] = s;

                Check(fe.Beta == 10);
                Check(fe.Delta == 20.0);
            }

            // Check impact of changing Resource Tables on explicit style references
            {
                MyFrameworkElement fe = new MyFrameworkElement();
                fe.SetResourceReference(FrameworkElement.StyleProperty, typeof(MyFrameworkElement));

                Style s = new Style(typeof(MyFrameworkElement));
                s.Setters.Add (new Setter(GreekStandard.BetaProperty, 1));
                s.Setters.Add (new Setter(GreekStandard.DeltaProperty, 2.0));

                ResourceDictionary dict = new ResourceDictionary();
                dict[typeof(MyFrameworkElement)] = s;
                fe.Resources = dict;

                Check(fe.Beta == 1);
                Check(fe.Delta == 2.0);

                // Replace one dictionary with another
                s = new Style(typeof(MyFrameworkElement));
                s.Setters.Add (new Setter(GreekStandard.BetaProperty, 10));
                s.Setters.Add (new Setter(GreekStandard.DeltaProperty, 20.0));

                dict = new ResourceDictionary();
                dict[typeof(MyFrameworkElement)] = s;
                fe.Resources = dict;

                Check(fe.Beta == 10);
                Check(fe.Delta == 20.0);
            }

            // Check impact of modifying a Resource Table entry on explicit style references
            {
                MyFrameworkElement fe = new MyFrameworkElement();
                fe.SetResourceReference(FrameworkElement.StyleProperty, typeof(MyFrameworkElement));

                Style s = new Style(typeof(MyFrameworkElement));
                s.Setters.Add (new Setter(GreekStandard.BetaProperty, 1));
                s.Setters.Add (new Setter(GreekStandard.DeltaProperty, 2.0));

                ResourceDictionary dict = new ResourceDictionary();
                dict[typeof(MyFrameworkElement)] = s;
                fe.Resources = dict;

                Check(fe.Beta == 1);
                Check(fe.Delta == 2.0);

                // Replace an entry with another
                s = new Style(typeof(MyFrameworkElement));
                s.Setters.Add (new Setter(GreekStandard.BetaProperty, 10));
                s.Setters.Add (new Setter(GreekStandard.DeltaProperty, 20.0));

                fe.Resources[typeof(MyFrameworkElement)] = s;

                Check(fe.Beta == 10);
                Check(fe.Delta == 20.0);
            }

            // Check impact of changing Resource Tables on resource references within styles
            {
                MyFrameworkElement fe = new MyFrameworkElement();

                ResourceDictionary dict = new ResourceDictionary();
                dict["one"] = 1;
                dict["two"] = 2.0;
                fe.Resources = dict;

                MyFrameworkElement feChild = new MyFrameworkElement();
                fe.AppendModelChild(feChild);

                Style s = new Style(typeof(MyFrameworkElement));
                s.Setters.Add (new Setter(GreekStandard.BetaProperty, new DynamicResourceExtension("one")));
                s.Setters.Add (new Setter(GreekStandard.DeltaProperty, new DynamicResourceExtension("two")));

                dict = new ResourceDictionary();
                dict[typeof(MyFrameworkElement)] = s;
                feChild.Resources = dict;

                Check(fe.Beta == 0);
                Check(fe.Delta == 0.0);

                Check(feChild.Beta == 1);
                Check(feChild.Delta == 2.0);

                // Replace a dictionary with another
                dict = new ResourceDictionary();
                dict["one"] = 10;
                dict["two"] = 20.0;
                fe.Resources = dict;

                Check(fe.Beta == 0);
                Check(fe.Delta == 0.0);

                Check(feChild.Beta == 10);
                Check(feChild.Delta == 20.0);

                // Add some more children
                MyFrameworkElement feGrandChild = new MyFrameworkElement();
                feChild.AppendModelChild(feGrandChild);

                dict = new ResourceDictionary();
                dict["one"] = 100;
                dict["two"] = 200.0;
                feGrandChild.Resources = dict;

                Check(fe.Beta == 0);
                Check(fe.Delta == 0.0);

                Check(feChild.Beta == 10);
                Check(feChild.Delta == 20.0);

                Check(feGrandChild.Beta == 100);
                Check(feGrandChild.Delta == 200.0);
            }

            // Check impact of modifying a Resource Table entry on resource references within styles
            {
                MyFrameworkElement fe = new MyFrameworkElement();

                ResourceDictionary dict = new ResourceDictionary();
                dict["one"] = 1;
                dict["two"] = 2.0;
                fe.Resources = dict;

                MyFrameworkElement feChild = new MyFrameworkElement();
                fe.AppendModelChild(feChild);

                Style s = new Style(typeof(MyFrameworkElement));
                s.Setters.Add (new Setter(GreekStandard.BetaProperty, new DynamicResourceExtension("one")));
                s.Setters.Add (new Setter(GreekStandard.DeltaProperty, new DynamicResourceExtension("two")));

                dict = new ResourceDictionary();
                dict[typeof(MyFrameworkElement)] = s;
                feChild.Resources = dict;

                Check(fe.Beta == 0);
                Check(fe.Delta == 0.0);

                Check(feChild.Beta == 1);
                Check(feChild.Delta == 2.0);

                // Replace values within the dictionary
                fe.Resources["one"] = 10;
                fe.Resources["two"] = 20.0;

                Check(fe.Beta == 0);
                Check(fe.Delta == 0.0);

                Check(feChild.Beta == 10);
                Check(feChild.Delta == 20.0);
            }
        }

        private static void Check(bool condition)
        {
            if (!condition)
            {
                throw new Exception("FAILED Resource Test!");
            }
        }

        private DrtSerializer drt;
    }

    // Tests validation
    internal sealed class ErrorValidationTestSuite : DrtTestSuite
    {
        internal ErrorValidationTestSuite(DrtSerializer drt): base("Validation")
        {
            _drt = drt;
        }

        public override DrtTest[] PrepareTests()
        {
            _sourceTree = new ErrorValidationTestSuite.InternalClass();

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(TestSerialization)
                        };
        }

        private void TestSerialization()
        {
            bool succeeded = false;

            try
            {
                string s = XamlWriter.Save(_sourceTree);
            }
            catch( InvalidOperationException )
            {
                succeeded = true;
            }

            DRT.Assert( succeeded, "Serializing an internal type didn't generate an exception" );
        }

        internal class InternalClass : Button
        {
        }
        
        private DrtSerializer _drt;
        private object        _sourceTree;
    }
}

