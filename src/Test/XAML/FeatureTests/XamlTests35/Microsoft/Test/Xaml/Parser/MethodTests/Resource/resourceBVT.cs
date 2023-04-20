// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.IO.Packaging;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;
using Microsoft.Test.Serialization;
using Microsoft.Test.Discovery;
using Microsoft.Test.Xaml.Types;
using Microsoft.Test.Xaml.Utilities;
using Microsoft.Test.Xaml.Parser;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Resource
{ /// <summary>
    /// Parser Test
    /// </summary>
    /// <remarks>
    /// This is parser BVT test that parse XAML using Resource and Styling.
    /// Test cases are:
    ///         - LoadXml with Resource
    ///         - LoadXml with Style
    ///         - LoadXml with Style Visual Tree
    ///         - LoadXml with Style Property Trigger
    ///         - LoadXml Resource Tag
    ///         - LoadXml with Resource and Style tag.
    ///         - Testing Resource and Resource Dictionary.
    ///         - Parsing Implicit Resource Styling.
    ///         - Parsing Explicit Resource Styling.
    ///         - Parsing Resource and Style with Comment tag inside.
    ///         - Parsing Style BasedOn
    ///         - Parsing Style *typeof
    ///         - Parsing Style *Alias
    ///         - Style parser should enforce a TargetType for Styles.
    ///         - Loading External Resources and Styling.
    ///         - Loading Custom controls in Styles.
    /// </remarks>
    public class ResourceBVT
    {
        #region RunTest
        /// <summary>
        /// Test case Entry point
        /// </summary>
        /// 
        //[Test(1, @"Parser\Resource", TestParameters = "Params=SERIALIZERES", Description = "Serialize Resource tag", SupportFiles = @"FeatureTests\ElementServices\resourcetest.xaml", Area = "XAML", Timeout=240)]
        public void RunTest()
        {
            string strParams = DriverState.DriverParameters["TestParams"];

            GlobalLog.LogStatus("Core:ResourceBVT Started ..." + "\n");// Start ParserBVT test

            // use CultureInfo.InvariantCulture to ensure argument string does not fail in languages such as Turkish
            strParams = strParams.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            switch (strParams)
            {
                case "PARSERES":
                    TestResourceParser();
                    break;

                case "PARSESTYLE":
                    TestStyleParser();
                    break;

                case "PARSERESOURCE":
                    TestResource();
                    break;

                case "PARSERESSTYLE":
                    TestResourceStyle();
                    break;

                case "PARSESTYLEVISUAL":
                    TestStyleVisualParser();
                    break;

                case "STYLEPROPTRIG":
                    TestStylePropVisualTrigger();
                    break;

                case "RESOURCEDIC":
                    TestResourceDic();
                    break;

                case "IMPLICIT":
                    TestResourceImp();
                    break;

                case "EXPLICIT":
                    TestResourceExp();
                    break;

                case "SERIALIZERES":
                    TestResourceSerialized();
                    break;

                case "COMMENT":
                    TestResourceComment();
                    break;

                case "BASEDON":
                    TestStyleBasedOn();
                    break;

                case "TYPEOF":
                    TestStyleTypeof();
                    break;

                case "ALIAS":
                    TestStyleAlias();
                    break;

                case "EXTERNAL":
                    TestExternalStyle();
                    break;
                case "CUSTOM":
                    CustomControlStyle();
                    break;
                default:
                    GlobalLog.LogStatus("ResourceBVT.RunTest was called with an unsupported parameter.");
                    throw new Microsoft.Test.TestSetupException("Parameter is not supported");
            }
        }
        #endregion RunTest
        /// <summary>
        ///
        /// </summary>
        public ResourceBVT()
        {
        }
        #region TestResourceParser
        /// <summary>
        /// TestResourceParser for ResourceBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(Resource tag) using Stream
        /// Verify:
        /// - Parse without error.
        /// - Render Control using OnRender Event to verify.
        /// </summary>
        void TestResourceParser() // Parse through resourcetest.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:ResourceBVT.TestResourceParser Started ..." + "\n");
            CreateContext();

            UIElement root = StreamParser(_resourceXaml);
            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");
            PanelFlow foo = rootPage.Content as PanelFlow;
            GlobalLog.LogStatus("Verifying Resource Test...");
            if (foo == null)
                throw new Microsoft.Test.TestValidationException("Expecting a PanelFlow");
            IEnumerable pfEnumerable = foo.Children;
            IEnumerator pf = pfEnumerable.GetEnumerator();
            for (int i = 0; i < 4; i++)
            {
                pf.MoveNext();
            }

            //Tree is already displayed as a result of loading in this Framework
            //_helper.DisplayTree(root);

            DisposeContext();

            GlobalLog.LogStatus("TestStreamParser Exit without error. Test Pass!");
        }
        #endregion TestResourceParser
        #region TestStyleParser
        /// <summary>
        /// TestStyleParser for ResourceBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(Resource/Style tag) using Stream
        /// Verify:
        /// - Parse without error.
        /// - Render Control using OnRender Event to verify.
        /// </summary>
        void TestStyleParser() // Parse through styletest.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:ResourceBVT.TestStyleParser Started ..." + "\n");
            CreateContext();

            UIElement root = StreamParser(_styleXaml);
            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");
            DockPanel foo = rootPage.Content as DockPanel;

            GlobalLog.LogStatus("Verifying Style on button...");
            if (foo == null)
                throw new Microsoft.Test.TestValidationException("Expecting a DockPanel");

            foreach (object currentChild in LogicalTreeHelper.GetChildren(foo))
            {
                string id = ((Button)currentChild).Name;

                if (id == "B1")
                {
                    Button b = (Button)currentChild;
                    System.Windows.Media.Color green, blue;

                    green = blue = new System.Windows.Media.Color();
                    green = ((SolidColorBrush)b.Background).Color;
                    blue = ((SolidColorBrush)b.Foreground).Color;
                    if (!(green.R == 0 && green.G == 255 && green.B == 0))
                    {
                        throw new Microsoft.Test.TestValidationException("'GreenBrush' resource had unexpected value at Page level.  " + "'R' = " + green.R + " 'G' = " + green.G + " 'B' = " + green.B);
                    }

                    if (!(blue.R == 0 && blue.G == 0 && blue.B == 255))
                    {
                        throw new Microsoft.Test.TestValidationException("'BlueBrush' resource had unexpected value at Page level.  " + "'R' = " + blue.R + " 'G' = " + blue.G + " 'B' = " + blue.B);
                    }
                }
            }

            //Tree is already displayed as a result of loading in this Framework
            //_helper.DisplayTree(root);

            DisposeContext();
            GlobalLog.LogStatus("TestStyleParser Exit without error. Test Pass!");
        }
        #endregion TestStyleParser
        #region TestStyleVisualParser
        /// <summary>
        /// TestStyleVisualParser for ResourceBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(Resource/Style tag) using Stream
        /// Verify:
        /// - Parse without error.
        /// - Render Control using OnRender Event to verify.
        /// </summary>
        void TestStyleVisualParser() // Parse through stylevisual.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:ResourceBVT.TestStyleVisualParser Started ..." + "\n");
            CreateContext();
            UIElement root = StreamParser(_styleVisualXaml);
            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");
            DockPanel foo = rootPage.Content as DockPanel;

            GlobalLog.LogStatus("Verifying Style on button...");
            if (foo == null)
                throw new Microsoft.Test.TestValidationException("Expecting a DockPanel");

            IEnumerator ienum = LogicalTreeHelper.GetChildren(foo).GetEnumerator();

            ienum.MoveNext();

            Button b = (Button)ienum.Current;
            ControlTemplate template = (ControlTemplate)b.GetValue(Control.TemplateProperty);

            DockPanel templateRoot = template.LoadContent() as DockPanel;
            if (typeof(Button) != (templateRoot.Children[0]).GetType())
                throw new Microsoft.Test.TestValidationException("VisualTree is not parsing correctly..." + (templateRoot.Children[0]).GetType());

            //Tree is already displayed as a result of loading in this Framework
            //_helper.DisplayTree(root);

            DisposeContext();
            GlobalLog.LogStatus("TestStyleVisualParser Exit without error. Test Pass!");
        }
        #endregion TestStyleVisualParser
        #region TestStylePropVisualTrigger
        /// <summary>
        /// TestStylePropVisualTrigger for ResourceBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(Resource/Style tag) using Stream
        /// Verify:
        /// - Parse without error.
        /// - Render Control using OnRender Event to verify.
        /// </summary>
        void TestStylePropVisualTrigger() // Parse through styleproptrig.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:ResourceBVT.TestStylePropVisualTrigger Started ..." + "\n");
            CreateContext();

            UIElement root = StreamParser(_stylePropXaml);
            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");
            DockPanel foo = rootPage.Content as DockPanel;

            GlobalLog.LogStatus("Verifying Style Property Trigger...");
            if (foo == null)
                throw new Microsoft.Test.TestValidationException("Expecting a DockPanel");

            IEnumerator ienum = LogicalTreeHelper.GetChildren(foo).GetEnumerator();

            ienum.MoveNext();

            Canvas c = (Canvas)ienum.Current;

            if (c == null)
                throw new Microsoft.Test.TestValidationException("Expecting a Canvas");

            IEnumerator i = LogicalTreeHelper.GetChildren(c).GetEnumerator();

            i.MoveNext();

            Button b1 = (Button)i.Current;

            if (b1 == null)
                throw new Microsoft.Test.TestValidationException("Expecting a Rectangle");

            System.Windows.Media.Color red, white;

            red = white = new System.Windows.Media.Color();
            red = ((SolidColorBrush)(b1.Background)).Color;

            // Verify Red Color.
            if (!(red.R == 255 && red.G == 0 && red.B == 0))
            {
                throw new Microsoft.Test.TestValidationException("'RedBrush' resource had unexpected value at Page level.  " + "'R' = " + red.R + " 'G' = " + red.G + " 'B' = " + red.B);
            }

            GlobalLog.LogStatus(b1.Background.ToString());
            ienum.MoveNext();

            Button b2 = (Button)ienum.Current;

            if (b2 == null)
                throw new Microsoft.Test.TestValidationException("Expecting a Rectangle");

            white = ((SolidColorBrush)(b2.Background)).Color;

            // Verify White Color.
            if (!(white.R == 255 && white.G == 255 && white.B == 255))
            {
                throw new Microsoft.Test.TestValidationException("'WhiteBrush' resource had unexpected value at Page level.  " + "'R' = " + white.R + " 'G' = " + white.G + " 'B' = " + white.B);
            }

            //Tree is already displayed as a result of loading in this Framework
            //_helper.DisplayTree(root);

            DisposeContext();
            GlobalLog.LogStatus("TestStyleVisualParser Exit without error. Test Pass!");
        }
        #endregion TestStylePropVisualTrigger
        #region TestResource
        /// <summary>
        /// TestResource for ResourceBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(Resource tag) using Stream
        /// Verify:
        /// - Parse without error.
        /// - Render Control using OnRender Event to verify.
        /// </summary>
        void TestResource() // Parse through resource.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:ResourceBVT.TestResource Started ..." + "\n");
            CreateContext();

            UIElement root = StreamParser(_resourceTest);
            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");
            DockPanel foo = rootPage.Content as DockPanel;

            if (foo == null)
                throw new Microsoft.Test.TestValidationException("Expecting a DockPanel");

            GlobalLog.LogStatus("Verifying Resource parse successfully...");
            VerifyPageResources(foo);

            //Tree is already displayed as a result of loading in this Framework
            //_helper.DisplayTree(root);


            DisposeContext();
            GlobalLog.LogStatus("TestResource Exit without error. Test Pass!");
        }
        #endregion TestResource
        #region TestResourceStyle
        /// <summary>
        /// TestResourceStyle for ResourceBVT case
        /// Resource Parsing implicit name for style resource
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(Resource + Style tag) using Stream
        /// Verify:
        /// - Parse without error.
        /// - Render Control using OnRender Event to verify.
        /// </summary>
        void TestResourceStyle() // Parse through resourcestyle.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:ResourceBVT.TestResourceStyle Started ..." + "\n");
            CreateContext();

            UIElement root = StreamParser(_resStyleXaml);
            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");
            DockPanel foo = rootPage.Content as DockPanel;

            GlobalLog.LogStatus("Verifying Resource and Style parsing....");
            if (foo == null)
                throw new Microsoft.Test.TestValidationException("Expecting a DockPanel");

            IEnumerator ienum = LogicalTreeHelper.GetChildren(foo).GetEnumerator();

            while (ienum.MoveNext())
            {
                string id = ((Button)ienum.Current).Name;

                if (id == "B1")
                {
                    Button b = (Button)ienum.Current;
                    System.Windows.Media.Color green, blue;

                    green = blue = new System.Windows.Media.Color();
                    green = ((SolidColorBrush)b.Background).Color;
                    blue = ((SolidColorBrush)b.Foreground).Color;
                    if (!(green.R == 0 && green.G == 255 && green.B == 0))
                    {
                        throw new Microsoft.Test.TestValidationException("'GreenBrush' resource had unexpected value at Page level.  " + "'R' = " + green.R + " 'G' = " + green.G + " 'B' = " + green.B);
                    }

                    if (!(blue.R == 0 && blue.G == 0 && blue.B == 255))
                    {
                        throw new Microsoft.Test.TestValidationException("'BlueBrush' resource had unexpected value at Page level.  " + "'R' = " + blue.R + " 'G' = " + blue.G + " 'B' = " + blue.B);
                    }
                }

                if (id == "res1")
                {
                    Button b = (Button)ienum.Current;
                    System.Windows.Media.Color red = new System.Windows.Media.Color();

                    red = ((SolidColorBrush)((Button)ienum.Current).Background).Color;
                    if (!(red.R == 255 && red.G == 0 && red.B == 0))
                    {
                        throw new Microsoft.Test.TestValidationException("'RedBrush' resource had unexpected value at Page level.  " + "'R' = " + red.R + " 'G' = " + red.G + " 'B' = " + red.B);
                    }
                }
            }

            //Tree is already displayed as a result of loading in this Framework
            //_helper.DisplayTree(root);

            DisposeContext();
            GlobalLog.LogStatus("TestResourceStyle Exit without error. Test Pass!");
        }
        #endregion TestResourceStyle
        #region TestResourceSerialized
        /// <summary>
        /// TestResourceSerialized for ResourceBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(Resource tag) using Stream
        /// - Serialize XAML (resourcetest.xaml).
        /// Verify:
        /// - Parse without error.
        /// - Render Control using OnRender Event to verify.
        /// </summary>
        void TestResourceSerialized() // Parse through resourcetest.xaml
        {
            try
            {
                //Create Context here.
                GlobalLog.LogStatus("Core:ResourceBVT.TestResourceSerialized Started ..." + "\n");

                UIElement root = StreamParser(_resourceXaml);

                CreateTempFile(root);

                UIElement el = StreamParser("tempFile.xaml");

                DisposeContext();

                GlobalLog.LogStatus("TestResourceSerialized Exit without error. Test Pass!");
            }
            finally
            {
                GlobalLog.LogStatus("Clean up temp file....");
                if (File.Exists("tempFile.xaml"))
                    File.Delete("tempFile.xaml");
            }
        }
        #endregion TestResourceSerialized
        #region TestResourceDic
        /// <summary>
        /// TestResourceDic for ResourceBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(Resource tag) using Stream
        /// Verify:
        /// - Parse without error.
        /// - Render Control using OnRender Event to verify.
        /// </summary>
        void TestResourceDic() // Parse through resourcedic.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:ResourceBVT.TestResourceDic Started ..." + "\n");
            CreateContext();

            UIElement root = StreamParser(_resDicXaml);
            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");
            DockPanel foo = rootPage.Content as DockPanel;

            GlobalLog.LogStatus("Verifying Resource Test...");

            // Get the DockPanel
            IEnumerator c = LogicalTreeHelper.GetChildren(foo).GetEnumerator();

            c.MoveNext();

            DockPanel dp = (DockPanel)c.Current;

            // Get the border (child of dock panel)
            c = LogicalTreeHelper.GetChildren(dp).GetEnumerator();
            c.MoveNext();

            Border b = (Border)c.Current;

            // Verify the background on the border (which comes from the resources)
            SolidColorBrush background = (SolidColorBrush)b.Background;

            if (background.Color.ToString().Equals("#00FF0000") == false)
            {
                throw new Microsoft.Test.TestValidationException("Incorrect evaluation of ResourceValueExpression for Border.Background property");
            }

            // Get the Button child of the dock panel
            c.MoveNext();

            System.Windows.Controls.Button button = c.Current as System.Windows.Controls.Button;

            // Get the element's content, which should be a TestChangeable object
            ChangeableTest ct = button.Content as ChangeableTest;

            if (ct.IsFrozen)
            {
                throw new Microsoft.Test.TestValidationException("Freezable type is not supposed to be frozen automatically when used as a resource");
            }

            if (ct.IsChangeTest != 1)
            {
                GlobalLog.LogStatus("ct.IsChangeTest is: " + ct.IsChangeTest + ". Should be 1.");
                throw new Microsoft.Test.TestValidationException("ct incorrect value");
            }

            // Create a new resource dictionary and add it to DockPanel
            ResourceDictionary rd = new ResourceDictionary();

            rd.Add("DaBrush", new SolidColorBrush(Colors.Green));
            rd.Add("ValueChange", new ChangeableTest(2));
            foo.Resources = rd;

            // Check background against this new resource dictionary
            background = (SolidColorBrush)b.Background;
            if (background.Color.ToString().Equals("#FF008000") == false)
            {
                GlobalLog.LogStatus(background.Color.ToString());
                throw new Microsoft.Test.TestValidationException("Incorrect evaluation of ResourceValueExpression for Border.Background property");
            }

            // And check the freezable against this new resource dictionary
            // (to ensure that the invalidation happened and the cache was cleared).
            ct = button.Content as ChangeableTest;
            if (ct.IsChangeTest != 2)
            {
                GlobalLog.LogStatus("ct is {0}, should be 2, ct.IsChangeTest:" + ct.IsChangeTest);
                throw new Microsoft.Test.TestValidationException("ct incorrect value");
            }

            //Tree is already displayed as a result of loading in this Framework
            //_helper.DisplayTree(root);

            DisposeContext();
            GlobalLog.LogStatus("TestResourceDic Exit without error. Test Pass!");
        }
        #endregion TestResourceDic
        #region TestResourceImp
        /// <summary>
        /// TestResourceImp for ResourceBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(Resource tag) using Stream
        /// Verify:
        /// - Parse without error.
        /// - Render Control using OnRender Event to verify.
        /// </summary>
        void TestResourceImp() // Parse through resourceimplicit.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:ResourceBVT.TestResourceImp Started ..." + "\n");
            CreateContext();

            UIElement root = StreamParser(_resImpXaml);
            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");
            DockPanel foo = rootPage.Content as DockPanel;

            GlobalLog.LogStatus("Verifying Resource Test...");

            // Get the DockPanel
            IEnumerator c = LogicalTreeHelper.GetChildren(foo).GetEnumerator();

            c.MoveNext();

            DockPanel dp = (DockPanel)c.Current;

            // Get the TextBlock (child of dock panel)
            c = LogicalTreeHelper.GetChildren(dp).GetEnumerator();
            c.MoveNext();

            Button b = (Button)c.Current;

            // Verify the foreground on the TextBlock (which comes from the resources)
            SolidColorBrush foreground = (SolidColorBrush)b.Foreground;

            if (foreground.Color.ToString().Equals("#FFFF0000") == false)
            {
                throw new Microsoft.Test.TestValidationException("TestResourceImp FAIL!!! Incorrect evaluation of ResourceValueExpression for Text.Foreground property..." + foreground.Color.ToString());
            }

            //Tree is already displayed as a result of loading in this Framework
            //_helper.DisplayTree(root);

            DisposeContext();
            GlobalLog.LogStatus("TestResourceImp Exit without error. Test Pass!");
        }
        #endregion TestResourceImp
        #region TestResourceExp
        /// <summary>
        /// TestResourceExp for ResourceBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(Resource tag) using Stream
        /// Verify:
        /// - Parse without error.
        /// - Render Control using OnRender Event to verify.
        /// </summary>
        void TestResourceExp() // Parse through resourceexplicit.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:ResourceBVT.TestResourceExp Started ..." + "\n");
            CreateContext();

            UIElement root = StreamParser(_resExpXaml);
            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");
            DockPanel foo = rootPage.Content as DockPanel;

            GlobalLog.LogStatus("Verifying Resource Test...");

            // Get the DockPanel
            IEnumerator c = LogicalTreeHelper.GetChildren(foo).GetEnumerator();

            c.MoveNext();

            DockPanel dp = (DockPanel)c.Current;

            // Get the button (child of dock panel)
            c = LogicalTreeHelper.GetChildren(dp).GetEnumerator();
            c.MoveNext();

            Button b = (Button)c.Current;

            // Verify the background on the button (which comes from the resources)
            SolidColorBrush background = (SolidColorBrush)b.Background;

            if (background.Color.ToString().Equals("#FFFF0000") == false)
            {
                throw new Microsoft.Test.TestValidationException("TestResourceExp FAIL!!!! Incorrect evaluation of ResourceValueExpression for Button.Background property..." + background.Color.ToString());
            }

            //Tree is already displayed as a result of loading in this Framework
            //_helper.DisplayTree(root);

            DisposeContext();
            GlobalLog.LogStatus("TestResourceExp Exit without error. Test Pass!");
        }
        #endregion TestResourceExp
        #region TestResourceComment
        /// <summary>
        /// TestResourceComment for ResourceBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(Resource tag) using Stream
        /// Verify:
        /// - Parse without error.
        /// - Render Control using OnRender Event to verify.
        /// </summary>
        void TestResourceComment() // Parse through resourcecomment.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:ResourceBVT.TestResourceComment Started ..." + "\n");
            CreateContext();

            UIElement root = StreamParser(_resCommentXaml);
            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");
            DockPanel foo = rootPage.Content as DockPanel;

            GlobalLog.LogStatus("Verifying Resource Test...");

            IEnumerator ienum = LogicalTreeHelper.GetChildren(foo).GetEnumerator();
            System.Windows.Media.Color green, blue;

            green = blue = new System.Windows.Media.Color();
            while (ienum.MoveNext())
            {
                string id = ((Button)ienum.Current).Name;

                if (id == "b1")
                {
                    green = ((SolidColorBrush)((Button)ienum.Current).Background).Color;
                    blue = ((SolidColorBrush)((Button)ienum.Current).Foreground).Color;
                    GlobalLog.LogStatus("Verifying if Button Background is Green...");

                    // Verify Green Color.
                    if (!(green.R == 0 && green.G == 255 && green.B == 0))
                    {
                        throw new Microsoft.Test.TestValidationException("'GreenBrush' resource had unexpected value at Page level.  " + "'R' = " + green.R + " 'G' = " + green.G + " 'B' = " + green.B);
                    }

                    GlobalLog.LogStatus("Verifying if Button Foreground is Blue...");

                    // Verify Blue Color.
                    if (!(blue.R == 0 && blue.G == 0 && blue.B == 255))
                    {
                        throw new Microsoft.Test.TestValidationException("'BlueBrush' resource had unexpected value at Page level.  " + "'R' = " + blue.R + " 'G' = " + blue.G + " 'B' = " + blue.B);
                    }
                }
            }

            //Tree is already displayed as a result of loading in this Framework
            //_helper.DisplayTree(root);

            DisposeContext();
            GlobalLog.LogStatus("TestResourceComment Exit without error. Test Pass!");
        }
        #endregion TestResourceComment
        #region TestStyleBasedOn
        /// <summary>
        /// TestStyleBasedOn for ResourceBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(Resource tag) using Stream
        /// Verify:
        /// - Parse without error.
        /// - Render Control using OnRender Event to verify.
        /// </summary>
        void TestStyleBasedOn() // Parse through resourcebasedon.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:ResourceBVT.TestStyleBasedOn Started ..." + "\n");
            CreateContext();

            UIElement root = StreamParser(_styleBasedOnXaml);
            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");
            DockPanel foo = rootPage.Content as DockPanel;

            GlobalLog.LogStatus("Verifying Resource Test...");

            IEnumerator ienum = LogicalTreeHelper.GetChildren(foo).GetEnumerator();
            System.Windows.Media.Color blue, limegreen, yellow, red, gold;

            blue = limegreen = yellow = red = gold = new System.Windows.Media.Color();
            while (ienum.MoveNext())
            {
                string id = ((FrameworkElement)ienum.Current).Name;

                switch (id)
                {
                    case "b":
                        red = ((SolidColorBrush)((Button)ienum.Current).Background).Color;
                        break;

                    case "c":
                        blue = ((SolidColorBrush)((CheckBox)ienum.Current).Background).Color;
                        break;

                    case "r":
                        red = ((SolidColorBrush)((Button)ienum.Current).Background).Color;
                        yellow = ((SolidColorBrush)((Button)ienum.Current).Foreground).Color;
                        break;

                    case "t":
                        gold = ((SolidColorBrush)((TextBlock)ienum.Current).Foreground).Color;
                        break;

                    case "r1":
                        limegreen = ((SolidColorBrush)((System.Windows.Shapes.Rectangle)ienum.Current).Fill).Color;
                        break;

                    case "r2":
                        blue = ((SolidColorBrush)((System.Windows.Shapes.Rectangle)ienum.Current).Fill).Color;
                        break;

                    default:
                        throw new Microsoft.Test.TestValidationException("Fail!!! Unexpected element Name encountered - " + id + ".");
                }
            }

            // Verify Blue Color.
            if (blue.ToString().Equals("#FF0000FF") == false)
            {
                GlobalLog.LogStatus("Blue: " + blue.ToString());
                throw new Microsoft.Test.TestValidationException("Incorrect Blue Color property");
            }

            // Verify Red Color.
            if (red.ToString().Equals("#FFFF0000") == false)
            {
                GlobalLog.LogStatus("Red: " + red.ToString());
                throw new Microsoft.Test.TestValidationException("Incorrect Red Color property");
            }

            // Verify Yellow Color.
            if (yellow.ToString().Equals("#FFFFFF00") == false)
            {
                GlobalLog.LogStatus("Yellow: " + yellow.ToString());
                throw new Microsoft.Test.TestValidationException("Incorrect Yellow Color property");
            }

            // Verify Gold Color.
            if (gold.ToString().Equals("#FFFFD700") == false)
            {
                GlobalLog.LogStatus("Gold: " + gold.ToString());
                throw new Microsoft.Test.TestValidationException("Incorrect Gold Color property");
            }

            // Verify LimeGreen Color.
            if (limegreen.ToString().Equals("#FF32CD32") == false)
            {
                GlobalLog.LogStatus("LimeGreen: " + limegreen.ToString());
                throw new Microsoft.Test.TestValidationException("Incorrect LimeGreen Color property");
            }

            //Tree is already displayed as a result of loading in this Framework
            //_helper.DisplayTree(root);

            DisposeContext();
            GlobalLog.LogStatus("TestStyleBasedOn Exit without error. Test Pass!");
        }
        #endregion TestStyleBasedOn
        #region TestStyleTypeof
        /// <summary>
        /// TestStyleTypeof for ResourceBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(Resource tag) using Stream
        /// Verify:
        /// - Parse without error.
        /// - Render Control using OnRender Event to verify.
        /// </summary>
        void TestStyleTypeof() // Parse through resourcetypeof.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:ResourceBVT.TestStyleTypeof Started ..." + "\n");
            CreateContext();

            UIElement root = StreamParser(_styleTypeOfXaml);
            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");
            DockPanel foo = rootPage.Content as DockPanel;

            GlobalLog.LogStatus("Verifying Resource Test...");

            IEnumerator ienum = LogicalTreeHelper.GetChildren(foo).GetEnumerator();
            System.Windows.Media.Color green, blue, yellow;

            green = blue = yellow = new System.Windows.Media.Color();
            while (ienum.MoveNext())
            {
                string id = ((FrameworkElement)ienum.Current).Name;

                switch (id)
                {
                    case "b1":
                        blue = ((SolidColorBrush)((Button)ienum.Current).Background).Color;
                        break;

                    case "b2":
                        yellow = ((SolidColorBrush)((RadioButton)ienum.Current).Background).Color;
                        break;

                    case "b3":
                        green = ((SolidColorBrush)((Border)ienum.Current).Background).Color;
                        break;

                    case "r1":
                        blue = ((SolidColorBrush)((System.Windows.Shapes.Rectangle)ienum.Current).Fill).Color;
                        break;

                    default:
                        throw new Microsoft.Test.TestValidationException("Fail!!! Unexpected element Name encountered - " + id + ".");
                }
            }

            // Verify Green Color.
            if (green.ToString().Equals("#0000FF00") == false)
            {
                GlobalLog.LogStatus("Green: " + green.ToString());
                throw new Microsoft.Test.TestValidationException("Incorrect Green Color property");
            }

            // Verify Blue Color.
            if (blue.ToString().Equals("#FF0000FF") == false)
            {
                GlobalLog.LogStatus("Blue: " + blue.ToString());
                throw new Microsoft.Test.TestValidationException("Incorrect Blue Color property");
            }

            // Verify Yellow Color.
            if (yellow.ToString().Equals("#FFFFFF00") == false)
            {
                GlobalLog.LogStatus("Yellow: " + yellow.ToString());
                throw new Microsoft.Test.TestValidationException("Incorrect Yellow Color property");
            }

            //Tree is already displayed as a result of loading in this Framework
            //_helper.DisplayTree(root);

            DisposeContext();

            GlobalLog.LogStatus("TestStyleTypeof Exit without error. Test Pass!");
        }
        #endregion TestStyleTypeof
        #region TestStyleAlias
        /// <summary>
        /// TestStyleAlias for ResourceBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(Resource tag) using Stream
        /// Verify:
        /// - Parse without error.
        /// - Render Control using OnRender Event to verify.
        /// </summary>
        void TestStyleAlias() // Parse through stylealias.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:ResourceBVT.TestStyleAlias Started ..." + "\n");
            CreateContext();

            UIElement root = StreamParser(_styleAliasXaml);
            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");

            Border foo = rootPage.Content as Border;

            GlobalLog.LogStatus("Verifying Styling Test...");

            IEnumerator ienum = LogicalTreeHelper.GetChildren(foo).GetEnumerator();

            ienum.MoveNext();

            StackPanel stackPanel = (StackPanel)ienum.Current;
            IEnumerator i = LogicalTreeHelper.GetChildren(stackPanel).GetEnumerator();

            i.MoveNext();

            ListBox lb = (ListBox)i.Current;
            ControlTemplate template = (ControlTemplate)lb.GetValue(Control.TemplateProperty);
            Border border = (Border)template.LoadContent();
            Canvas canvas = (Canvas)border.Child;
            ScrollViewer scrollview = (ScrollViewer)canvas.Children[1];

            if (typeof(ScrollViewer) != scrollview.GetType())
                throw new Microsoft.Test.TestValidationException("FAIL!!!! Did not get System.Windows.Controls.ScrollViewer...." + scrollview.GetType());

            //Tree is already displayed as a result of loading in this Framework
            //_helper.DisplayTree(root);

            DisposeContext();
            GlobalLog.LogStatus("TestStyleAlias Exit without error. Test Pass!");
        }
        #endregion TestStyleAlias
        #region ExternalStyle
        /// <summary>
        /// Loading External Resources and Styling
        /// scenario:
        ///     - Load ExternalStyle.xaml
        ///     - Load ResourceExternal.xaml
        ///     - use ResourceExternal on ExternalStyle
        /// Verify:
        ///     - Resource and Style applied.
        /// </summary>
        void TestExternalStyle()
        {
            GlobalLog.LogStatus("Core:ExternalStyle Started ..." + "\n");

            GlobalLog.LogStatus("Parse Page XAML using Stream...." + _externalXaml);
            DockPanel foo = null;
            IXamlTestParser parser = XamlTestParserFactory.Create();
            foo = parser.LoadXaml(_externalXaml, null) as DockPanel;

            if (foo == null)
                throw new Microsoft.Test.TestValidationException("Expecting a DockPanel");
            FrameworkElement r = null;
            // see if it loads
            GlobalLog.LogStatus("Parse Resource XAML using Stream...." + _externalResource);

            r = (FrameworkElement)parser.LoadXaml(_externalResource, null);

            foo.Resources = ((DockPanel)(((Page)r).Content)).Resources;
            GlobalLog.LogStatus("Verifying External Resource and Style Test...");
            char numericListSeparator = XamlTestHelper.GetNumericListSeparatorByCulture(CultureInfo.InvariantCulture);
            Button border1 = (Button)LogicalTreeHelper.FindLogicalNode(foo, "border1");
            if (border1 != null)
            {
                System.Windows.Media.Color white = new System.Windows.Media.Color();
                white = ((SolidColorBrush)border1.Background).Color;
                if (white != Colors.White)
                    throw new Microsoft.Test.TestValidationException("Test Fail!!!!!!!!!" + white.ToString() + " R " + white.R + " B " + white.B + " G " + white.G);
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Test Fail! Could not find Border...");
            }
            Button dock1 = (Button)LogicalTreeHelper.FindLogicalNode(foo, "dock1");
            if (dock1 != null)
            {
                object d = dock1.GetValue(DockPanel.DockProperty);
                if ((d.ToString() != "Bottom") || (dock1.Margin.ToString() != "10" + numericListSeparator + "10" + numericListSeparator + "10" + numericListSeparator + "10"))
                    throw new Microsoft.Test.TestValidationException("Test Fail!!! DockPanel does not apply style... Margin " + dock1.Margin + " Dock " + d.ToString());
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Test Fail! Could not Find DockPanel Name=dock1...");
            }
            Button b1 = (Button)LogicalTreeHelper.FindLogicalNode(foo, "b1");
            if (b1 != null)
            {
                if (b1.Margin.ToString() != "0" + numericListSeparator + "0" + numericListSeparator + "20" + numericListSeparator + "20")
                    throw new Microsoft.Test.TestValidationException("Test Fail!!! Button does not apply style... Margin " + b1.Margin);
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Test Fail! Could not Find Button Name=b1...");
            }
            Button Logo = (Button)LogicalTreeHelper.FindLogicalNode(foo, "Logo");
            if (Logo != null)
            {
                object d = Logo.GetValue(DockPanel.DockProperty);
                if (d.ToString() != "Left")
                    throw new Microsoft.Test.TestValidationException("Test Fail!!! Canvas does not apply style... Dock " + d.ToString());
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Test Fail! Could not Find Canvas Name=Logo...");
            }
            Button button1 = (Button)LogicalTreeHelper.FindLogicalNode(foo, "button1");
            if (button1 != null)
            {
                object d = button1.GetValue(DockPanel.DockProperty);
                if ((d.ToString() != "Top") || (button1.Margin.ToString() != "10" + numericListSeparator + "10" + numericListSeparator + "10" + numericListSeparator + "10") || (button1.HorizontalAlignment.ToString() != "Center"))
                    throw new Microsoft.Test.TestValidationException("Test Fail!!! FlowPanel does not apply style... Dock " + d.ToString() + " HA " + button1.HorizontalAlignment.ToString() + " M " + button1.Margin.ToString());
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Test Fail! Could not Find Button Name=button1...");
            }

            //Tree is already displayed as a result of loading in this Framework
            //_helper.DisplayTree(root);

            DisposeContext();
            GlobalLog.LogStatus("ExternalStyle Test Pass!...");
        }
        #endregion ExternalStyle
        #region CustomControlStyle
        /// <summary>
        /// CustomControlStyle for ResourceBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(Resource/Style tag) using Stream
        /// Verify:
        /// - Parse without error.
        /// - Render Control using OnRender Event to verify.
        /// </summary>
        void CustomControlStyle() // Parse through CustomControlStyle.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:ResourceBVT.CustomControlStyle Started ..." + "\n");
            CreateContext();

            UIElement root = StreamParser(_customControl);

            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");

            PanelFlow foo = rootPage.Content as PanelFlow;

            GlobalLog.LogStatus("Verifying Style on button...");
            if (foo == null)
                throw new Microsoft.Test.TestValidationException("Expecting a PanelFlow");
            IEnumerable pfEnumerable = foo.Children;
            IEnumerator pf = pfEnumerable.GetEnumerator();
            pf.MoveNext();
            DockPanel dp = (DockPanel)pf.Current;
            IEnumerator ienum = LogicalTreeHelper.GetChildren(dp).GetEnumerator();
            ienum.MoveNext();
            Item i = (Item)ienum.Current;
            ienum.MoveNext();
            ienum.MoveNext();
            ienum.MoveNext();
            Microsoft.Test.Xaml.Types.Label l = (Microsoft.Test.Xaml.Types.Label)ienum.Current;
            FakeSolidColorBrush a = (FakeSolidColorBrush)i.GetValue(Item.SolidBackgroundBrushProperty);
            if (a.Color != "Red")
                throw new Microsoft.Test.TestValidationException("Expecting Red Color...., but getting " + a.Color);
            if (a.Opacity != 0.8)
                throw new Microsoft.Test.TestValidationException("Expecting 0.8...., but getting " + a.Opacity);
            if (l.Text != "Style here")
                throw new Microsoft.Test.TestValidationException("Expecting Correct Text...., but getting " + l.Text);

            //Tree is already displayed as a result of loading in this Framework
            //_helper.DisplayTree(root);

            DisposeContext();
            GlobalLog.LogStatus("TestStyleParser Exit without error. Test Pass!");
        }
        #endregion CustomControlStyle
        #region CreateTempFile
        /// <summary>
        ///
        /// </summary>
        /// <param name="root"></param>
        void CreateTempFile(UIElement root)
        {
            GlobalLog.LogStatus("Creating xaml file from SaveAsXml...");

            string outer = SerializationHelper.SerializeObjectTree(root);

            // create a new file for comparison
            GlobalLog.LogStatus("Creating a new temporary file for comparison..." + "\n");

            FileStream fs = new FileStream("tempFile.xaml", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            // create a xaml with the expected markup
            GlobalLog.LogStatus("Creating a xaml file..." + "\n");
            sw.Write(outer);
            sw.Close();
            fs.Close();
        }
        #endregion CreateTempFile
        #region VerifyResourceParsing
        /// <summary>
        ///
        /// </summary>
        /// <param name="dp"></param>
        void VerifyPageResources(DockPanel dp)
        {
            IEnumerator ienum = LogicalTreeHelper.GetChildren(dp).GetEnumerator();
            System.Windows.Media.Color green, blue, red, yellow;

            green = blue = red = yellow = new System.Windows.Media.Color();
            while (ienum.MoveNext())
            {
                string id = ((Button)ienum.Current).Name;

                switch (id)
                {
                    case "b1":
                        yellow = ((SolidColorBrush)((Button)ienum.Current).Background).Color;
                        break;

                    case "b2":
                        red = ((SolidColorBrush)((Button)ienum.Current).Background).Color;
                        break;

                    case "b3":
                        green = ((SolidColorBrush)((Button)ienum.Current).Background).Color;
                        break;

                    case "b4":
                        blue = ((SolidColorBrush)((Button)ienum.Current).Background).Color;
                        break;

                    default:
                        throw new Microsoft.Test.TestValidationException("Fail!!! Unexpected element Name encountered - " + id + ".");
                }
            }

            // Verify Green Color.
            if (!(green.R == 0 && green.G == 255 && green.B == 0))
            {
                throw new Microsoft.Test.TestValidationException("'GreenBrush' resource had unexpected value at Page level.  " + "'R' = " + green.R + " 'G' = " + green.G + " 'B' = " + green.B);
            }

            // Verify Blue Color.
            if (!(blue.R == 0 && blue.G == 0 && blue.B == 255))
            {
                throw new Microsoft.Test.TestValidationException("'BlueBrush' resource had unexpected value at Page level.  " + "'R' = " + blue.R + " 'G' = " + blue.G + " 'B' = " + blue.B);
            }

            // Verify Red Color.
            if (!(red.R == 255 && red.G == 0 && red.B == 0))
            {
                throw new Microsoft.Test.TestValidationException("'RedBrush' resource had unexpected value at Page level.  " + "'R' = " + red.R + " 'G' = " + red.G + " 'B' = " + red.B);
            }

            // Verify Yellow Color.
            if (!(yellow.R == 255 && yellow.G == 255 && yellow.B == 0))
            {
                throw new Microsoft.Test.TestValidationException("'YellowBrush' resource had unexpected value at Page level.  " + "'R' = " + yellow.R + " 'G' = " + yellow.G + " 'B' = " + yellow.B);
            }
        }
        #endregion VerifyResourceParsing
        #region LoadXml
        /// <summary>
        /// StreamParser is used to LoadXml(xaml).
        /// </summary>
        public UIElement StreamParser(string filename)
        {
            UIElement root = null;
            // see if it loads
            GlobalLog.LogStatus("Parse XAML using Stream...." + filename);
            IXamlTestParser parser = XamlTestParserFactory.Create();
            root = (UIElement)parser.LoadXaml(filename, null);
            return root;
        }
        #endregion LoadXml
        #region Defined

    readonly SerializationHelper _helper = new SerializationHelper();
        // UiContext defined here
        Dispatcher _dispatcher;

        #endregion Defined
        #region filenames
        string _resourceXaml = "resourcetest.xaml";
        string _resourceTest = "resource.xaml";
        string _styleXaml = "styletest.xaml";
        string _styleVisualXaml = "stylevisual.xaml";
        string _resStyleXaml = "resourcestyle.xaml";
        string _resDicXaml = "resourcedic.xaml";
        string _stylePropXaml = "styleproptrig.xaml";
        string _resExpXaml = "resourceexplicit.xaml";
        string _resImpXaml = "resourceimplicit.xaml";
        string _resCommentXaml = "resourcecomment.xaml";
        string _styleBasedOnXaml = "resourcebasedon.xaml";
        string _styleTypeOfXaml = "resourcetypeof.xaml";
        string _styleAliasXaml = "stylealias.xaml";
        string _externalXaml = "ExternalStyle.xaml";
        string _externalResource = "ResourceExternal.xaml";
        string _customControl = "CustomControlStyle.xaml";
        #endregion filenames
        #region Context
        /// <summary>
        /// Creating UIContext
        /// </summary>
        private void CreateContext()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// Disposing UIContext here
        /// </summary>
        private void DisposeContext()
        {

        }
        #endregion Context
        #region FindElement
        private FrameworkElement FindElement(Type type, string id, UIElement root)
        {
            Visual visual = root as Visual;

            if (visual == null)
                return null;

            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");

            FrameworkElement fe = rootPage.Content as FrameworkElement;

            if ((fe.GetType().Equals(type)) && (fe.Name == id))
                return fe;

            int count = VisualTreeHelper.GetChildrenCount(visual);

            for (int i = 0; i < count; i++)
            {
                UIElement child = (UIElement)VisualTreeHelper.GetChild(visual, i);
                FrameworkElement feRet = FindElement(type, id, child);

                if (feRet != null)
                    return feRet;
            }

            return null;
        }
        #endregion
    }
}

