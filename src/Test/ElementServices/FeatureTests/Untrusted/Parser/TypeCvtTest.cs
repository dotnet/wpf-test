// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
 * Description:
 *      Test TypeConverter usage in avalon. For details reffer to spec
 *      at http://avalon/coreui/Parser/ParserSpec.htm.
 * 
 * Scenario:
 *      Create new type MyType. 
 *      Associate custom type converter MyTypeCvt with the new type.
 *      Register dynamic property TypeCvtTest.TestProperty of the type MyType
 *      Load sample file in parses which sets the property to value "TestString"
 *      Converter verifies all passed parameters.
 *      Main program verifies the generated tree and checks correct property value.
 * 
 * Sample file content:
 *  <FlowPanel xmlns="using:Avalon.Base">
 *      <Control
 *          xmlns="using:TypeConverter#TypeCvt"
 *          TypeCvtTest.TestProperty="TestString">
 *              Text Example.
 *      </Control>
 *      </FlowPanel>
 */
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.ComponentModel;
using System.IO.Packaging;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;

using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

using System.Windows.Markup;
using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.Source;
using Microsoft.Test.Win32;
using MS.Internal;

namespace Avalon.Test.CoreUI.Parser
{

    /// <summary>
    /// </summary>
    public class MyButton : Button
    {
        /// <summary>
        /// </summary>
        public MyEnum FooMyEnum
        {
            get
            {
                return _fooMyEnum;
            }

            set
            {
                _fooMyEnum = value;
                this.Content = _fooMyEnum.ToString();
            }

        }

        MyEnum _fooMyEnum;
    }

    /// <summary>
    /// </summary>
    [TypeConverter(typeof(MyEnumCvt))]
    public enum MyEnum
    {
        /// <summary>
        /// </summary>
        One,
        /// <summary>
        /// </summary>
        Two,
        /// <summary>
        /// </summary>
        Three
        
    }

    

    /// <summary>
    /// The type converter class used in this test.
    /// </summary>
    public class MyEnumCvt : TypeConverter
    {
        /// <summary>
        /// To define type converter class we are required to override this 
        /// method
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext tpx, System.Globalization.CultureInfo culture, object obj)
        {
            if (obj is String)
            {
                string s = (string)obj;
                if (String.Compare(s,"Uno") == 0)
                {
                    return MyEnum.One;
                }
                if (String.Compare(s,"Dos") == 0)
                {
                    return MyEnum.Two;
                }
                if (String.Compare(s,"Tres") == 0)
                {
                    return MyEnum.Three;
                }
                if (Enum.IsDefined(typeof(MyEnum),obj))
                {
                    return Enum.Parse(typeof(MyEnum),s, true);
                }
            }
            if (obj is Enum)
            {
                return obj.ToString();
            }
            throw new Exception("Invalid Value");

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tdc"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext tdc, Type t)
        {
            return true;
        }
    }




    /// <summary>
    ///   MyType class defines new type. The [] statement associates 
    ///   MyTypeCvt class as a CLR TypeConverter for MyType.
    /// </summary>
    [TypeConverter(typeof(MyTypeCvt))]
    public class MyType
    {
        /// <summary>
        /// This string is used to check the final result.
        /// At the end of the test the main program examines the element 
        /// property and thecks this string for the value "fine"
        /// 
        /// All problems detected by the type converter class will be 
        /// reported into this variable.
        /// </summary>
        public string myString = "not initialized";
    }

    /// <summary>
    /// The type converter class used in this test.
    /// </summary>
    public class MyTypeCvt : TypeConverter
    {
        /// <summary>
        /// To define type converter class we are required to override this 
        /// method
        /// </summary>
        /// <param name="tpx">
        /// Provides link with the Mapper and XmlParserContext objects used
        /// by the parser via GetService call.
        /// </param>
        /// <param name="culture">CultureInfo supplied</param>
        /// <param name="obj">The value to be converted</param>
        /// <returns>
        /// Returns an instance of MyType object with myString set depending on
        /// success of the parameter test.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext tpx, System.Globalization.CultureInfo culture, object obj)
        {
            // Create MyType instance to be returned.
            MyType m = new MyType();
            // Try to convert the passed value to string. Note that parser 
            // should use string as a type.
            string str = obj as string;
            // Set initial test status to "bad"
            m.myString = "bad";
            if ( culture == null ) // check culture parameter
                m.myString = "culture is null";
            else if ( obj == null ) // check value supplied
                m.myString = "value is null";
            else if ( str == null ) // check value type supplied
                m.myString = "values is of type " + obj.GetType().ToString();
            else if ( str == "Test String" ) // finally check if the correct value is passed
                m.myString = "fine"; // Horay test successfull
            else // unexpected value
                m.myString = "value is " + str;
            return m;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tdc"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext tdc, Type t)
        {
            return true;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class TypeCvtTest : FrameworkElement
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TypeCvtTest()
        {
        }
        #region TestProperty DependencyProperty
        /// <summary>
        /// TypeCvtTest Property - string
        /// </summary>
        public static readonly DependencyProperty TestPropertyDP = DependencyProperty.RegisterAttached("TestProperty", typeof(MyType), typeof(TypeCvtTest));

        /// <summary>
        /// 
        /// </summary>
        public MyType TestProperty
        {
            get
            {
                return _testproperty;
            }
            set
            {
                SetValue(TestPropertyDP, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public MyType _testproperty = null;
        #endregion TestProperty DependencyProperty
    }
    /// <summary>
    /// This class contains the main testcase code in the CvtTest method.
    /// In addition it defines new DinamicProperty TestProperty to be
    /// used for this test.
    /// </summary>
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCasePriority("0")]
    [TestCaseArea(@"Parser\Simple")]
    [TestCaseMethod("CvtTest")]
    [TestCaseTimeout("180")]
    [TestCaseSupportFile("testcvt.xaml")]
    public class CvtTestBVT
    {
        /// <summary>
        /// Main test method
        /// </summary>
        public void CvtTest()
        {
            CreateContext();

            CoreLogger.LogStatus( "Open file" );
            // Open xaml file
            Stream stream = File.OpenRead("testcvt.xaml");
            // Invoke the parser to create a tree
            ParserContext pc = new ParserContext();
            pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
            UIElement root = (UIElement)System.Windows.Markup.XamlReader.Load(stream, pc);
            // Close the xaml file.
            stream.Close();
            
            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");

            TypeCvtTest foo = rootPage.Content as TypeCvtTest;

            if (foo == null)
                throw new Microsoft.Test.TestValidationException("Expecting a PanelFlow");
            // Try to get the property
            MyType mtp = (MyType)foo.GetValue(TypeCvtTest.TestPropertyDP);
            if ( mtp != null )
            {
                if ( mtp.myString == "fine" )
                {
                    // Horay we passed the test
                    CoreLogger.LogStatus("Test PASSED" );
                }
                else
                {
                    // Hmm, value is not correct, may be there was an error in 
                    // the converter code. Report the value to the log file.
                    throw new Microsoft.Test.TestValidationException("Property expected value 'fine', got '" + mtp.myString + "'" );
                }
            }
            else
                // We can not find the property. Oops.
                throw new Microsoft.Test.TestValidationException("Property is null" );

            // Close Avalon gracefuly.
            DisposeContext();
            return;
        }
        #region Context
        /// <summary>
        /// Creating Dispatcher
        /// </summary>
        private void CreateContext()
        {
            s_dispatcher = Dispatcher.CurrentDispatcher;    
        }
        /// <summary>
        /// Disposing Dispatcher here
        /// </summary>
        private void DisposeContext()
        {

        }

        /// <summary>
        /// Intercept window messages looking for a close.  If found, stop the dispatcher,
        /// causing the process to stop
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private static IntPtr ApplicationFilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Quit the application if the source window is closed.
            if((msg == NativeConstants.WM_CLOSE) )
            {
                s_dispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                
            }
            handled = false;
            return IntPtr.Zero;
        }


        #endregion Context
        #region Defined
        // UiContext defined here

        static Dispatcher s_dispatcher;
        #endregion Defined
    }
}
