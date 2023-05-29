// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Avalon.Test.CoreUI.Common;
using System.Runtime.InteropServices;
using Avalon.Test.CoreUI.Source;
using Microsoft.Test.Win32;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.Framework.Dispatchers;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.IO;
using System.IO.Packaging;
using System.Windows.Markup;
using Microsoft.Test.Serialization;
using System.Xml;
using Microsoft.Test.Threading;
using System.Windows.Data;
namespace Avalon.Test.Framework.Triggers.Properties
{


    /// <summary>
    /// </summary>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]       
    public class ConditionCollectionTest 
    {
        


        /// <summary>
        /// </summary>
        [TestCase("2", @"PropertyEngine",TestCaseSecurityLevel.PartialTrust, "ConditionCollection.IsSealed from Code and Condition ctor with BindingBase param")]
        public void TestCondition()
        {
            Button bu = new Button();
            bu.Content ="foo";
            bu.Background = Brushes.Red;
            
            Binding b = new Binding();
            b.Path = new PropertyPath("Background");
            b.Source = bu;

            Page p = new Page();
            p.Content = bu;
            Style s = new Style(typeof(Button), bu.Style);

            Condition c = new Condition(b,Brushes.Red);

            MultiDataTrigger trigger = new MultiDataTrigger();
            trigger.Conditions.Add(c);

            Setter setter = new Setter(Control.ForegroundProperty, Brushes.Red);
            trigger.Setters.Add(setter);
            


            s.Triggers.Add(trigger);
            bu.Style = s;            

            SerializationHelper sh = new SerializationHelper();
            sh.DisplayTree(p);
            
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ValidateSetter), bu);


            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ValidateIsSealed), p);

            DispatcherHelper.RunDispatcher();

        }

        object ValidateSetter(object o)
        {
            Button b = (Button)o;

            bool passed = false;
            string comment = "The Setter was not applied.";

            if (b.Foreground == Brushes.Red)
            {
                passed = true;
                comment = "";
            }
            
            CoreLogger.LogTestResult(passed, comment);

            
            return null;
        }


        /// <summary>
        /// </summary>
        [TestCase("2", @"PropertyEngine",TestCaseSecurityLevel.PartialTrust, "ConditionCollection.IsSealed from Code")]
        public void TestIsSealedCode()
        {
            ConditionCollection cc = new ConditionCollection();

            bool passed = false;
            string comment = "The Collection should not be sealed";

            if (!cc.IsSealed)
            {
                passed = true;
                comment = "";
            }
            
            CoreLogger.LogTestResult(passed, comment);
        }


        /// <summary>
        /// </summary>
        [TestCase("2", @"PropertyEngine",TestCaseSecurityLevel.PartialTrust, "ConditionCollection.IsSealed from Xaml")]
        public void TestIsSealedXaml()
        {



            string str = "<Page xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Button Foreground=\"Red\">" ;
            str += "    <Button.Style>";
            str += "        <Style>";
            str += "            <Style.Triggers>";
            str += "                <MultiTrigger>";
            str += "                    <MultiTrigger.Conditions>";
            str += "                        <Condition Property=\"Button.Foreground\" Value=\"Red\"/>	";
            str += "                    </MultiTrigger.Conditions>";
            str += "                    <Setter Property=\"Button.Background\" Value=\"Red\"/>";
            str += "                </MultiTrigger>";
            str += "            </Style.Triggers>";
            str += "        </Style>";
            str += "    </Button.Style>		";
            str += "</Button></Page>";

            StringReader s = new StringReader(str);
            XmlTextReader xmlReader = new XmlTextReader(s);
            object root = XamlReader.Load(xmlReader);

            SerializationHelper sh = new SerializationHelper();
            sh.DisplayTree(root);

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ValidateIsSealed), root);


            DispatcherHelper.RunDispatcher();


        }

        object ValidateIsSealed(object o)
        {
                    Page p = (Page)o;
                    Button b = (Button)p.Content;

                    ConditionCollection cc = null;

                    if (b.Style.Triggers[0] is MultiDataTrigger)
                    {
                        cc = ((MultiDataTrigger)b.Style.Triggers[0]).Conditions;
                    }
                    else
                    {
                        cc = ((MultiTrigger)b.Style.Triggers[0]).Conditions;
                    }


                    


                    bool passed = false;
                    string comment = "The Collection should be sealed";

                    if (cc.IsSealed)
                    {
                        passed = true;
                        comment = "";
                    }
                    
                    CoreLogger.LogTestResult(passed, comment);



                    DispatcherHelper.ShutDown();

                    return null;
        }
        



    }




    ///<summary>
    ///</summary>"
    ///<remarks>
    ///     <filename>PropertyTriggerCodeonButton.cs</filename>
    ///</remarks>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]    
    public class PropertyTriggerCodeonButton : TestCase
    {
        

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public PropertyTriggerCodeonButton() :base(TestCaseType.HwndSourceSupport)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public override void Run()
        {
            if(TestCaseInfo.GetCurrentInfo().Params == "OnCode")
            {
                OnCode();
            }
            else if(TestCaseInfo.GetCurrentInfo().Params == "OnXaml")
            {
                OnXaml();
            }
            else
            {
                throw new Microsoft.Test.TestSetupException("Test Case is not supported");
            }
            
        }


        /// <summary>
        /// Creating a CheckButton (all the tree on code) and adding a property trigger for IsChecked
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///     <li></li>
        ///  </ol>
	 ///     <filename>PropertyTriggerCodeonButton.cs</filename>
        /// </remarks>
        [CoreTestsLoader(CoreTestsTestType.MethodBase)]
        [TestCasePriority("0")]
        [TestCaseArea(@"PropertyEngine\TriggerBase\Trigger\Simple")]
        [TestCaseMethod("RunTest")]
        [TestCaseParams("OnXaml")]
        [TestCaseDisabled("0")]
        [TestCaseSupportFile("PropertySimpleCheckButton.xaml")]
        [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
        public void OnXaml()
        {

            FileStream fs = null;

            Border border;

            if (!File.Exists("PropertySimpleCheckButton.xaml"))
                throw new Microsoft.Test.TestSetupException("The file SimpleCheckButton.xaml was not found");

            try
            {
                using(CoreLogger.AutoStatus("Creating a tree"))
                {
               
                    fs = new FileStream("PropertySimpleCheckButton.xaml", FileMode.Open);

                }
                ParserContext pc = new ParserContext();
                pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                Page rootPage = System.Windows.Markup.XamlReader.Load(fs, pc) as Page;
                if (null == rootPage)
                    throw new Microsoft.Test.TestSetupException("Page cast filed");

                border = rootPage.Content as Border;

                _button = ((StackPanel)border.Child).Children[0] as CheckBox;

                AddHello2Panel(((StackPanel)border.Child));
            
                Source.RootVisual = border;


                Dispatcher.Run();

            }
            finally
            {
                fs.Close();

            }

            
            if (!_isPass)
                throw new Microsoft.Test.TestValidationException("The test case fails");

                        
        }


        


        /// <summary>
        /// Creating a CheckButton (all the tree on code) and adding a property trigger for IsChecked
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///     <li></li>
        ///  </ol>
	 ///     <filename>PropertyTriggerCodeonButton.cs</filename>
        /// </remarks>
        [CoreTestsLoader(CoreTestsTestType.MethodBase)]
        [TestCasePriority("0")]
        [TestCaseArea(@"PropertyEngine\TriggerBase\Trigger\Simple")]
        [TestCaseMethod("RunTest")]
        [TestCaseParams("OnCode")]
        [TestCaseDisabled("0")]
        [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
        public void OnCode()
        {
            Style style;
            Trigger trigger;
            StackPanel panel ;
            Border border;

            using(CoreLogger.AutoStatus("Creating a tree"))
            {
                border = new Border();

                border.Background = Brushes.Linen;

                panel = new StackPanel();

                border.Child = panel;

                _button = new CheckBox();

                _button.Content = "Move the mouse here!!!!";

                panel.Children.Add(_button);

                AddHello2Panel(panel);

            }


            using(CoreLogger.AutoStatus("Creating a style with a trigger"))
            {
                style = new Style(typeof(CheckBox), _button.Style);

                trigger = new Trigger();

                trigger.Property = CheckBox.IsCheckedProperty;

                trigger.Value = (Nullable<bool>)true;;

                trigger.Setters.Add(new Setter(Control.BackgroundProperty,Brushes.Indigo));

                style.Triggers.Add(trigger);

                _button.Style = style;

            }

            Source.RootVisual = border;



            Dispatcher.Run();

            if (!_isPass)
                throw new Microsoft.Test.TestValidationException("The test case fails");

                        
        }

        void AddHello2Panel(Panel panel)
        {
                Avalon.Test.CoreUI.Source.HelloElement hello = new Avalon.Test.CoreUI.Source.HelloElement();
                hello.RenderedSourcedHandlerEvent += new HelloElement.RenderHandler(onPainted);

                hello.Source = Source;

                panel.Children.Add(hello);

        }


            void onPainted(UIElement target, HwndSource Source)
            {
                target.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(simulateClick), null);
                
            }

            object simulateClick(object o)
            {
            
                using(CoreLogger.AutoStatus("Changing the CheckBox State on Code"))
                {
                    _button.IsChecked = true;
                }

                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(validation), null);
                return null;   
            }


            object validation(object o)
            {

                using(CoreLogger.AutoStatus("Validating the Trigger works correctly"))
                {
                    if (_button.Background != Brushes.Indigo)
                        throw new Microsoft.Test.TestValidationException("The property trigger was not fired");

                }
                _isPass = true;
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(exitDispatcher), null);

                return null;   
            }



            object exitDispatcher(object o)
            {

                Source.Dispose();
                Microsoft.Test.Threading.DispatcherHelper.ShutDown();
              
                return null;   
            }


            CheckBox _button = null;
            bool _isPass = false;

    }
        
 }









