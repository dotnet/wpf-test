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
using System.Windows.Markup;


namespace Avalon.Test.Framework.Triggers.Properties
{
    ///<summary>
    ///</summary>"
    ///<remarks>
    ///     <filename>PropertyTriggerMultipleSetSimple.cs</filename>
    ///</remarks>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]    
    public class PropertyTriggerMultipleSetSimple : TestCase
    {
        

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public PropertyTriggerMultipleSetSimple() :base(TestCaseType.HwndSourceSupport)
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
        /// Adds 1 Trigger to a Checkbutton to change two different properties Background and ForeGround on a Xaml usiing Parser.Load
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///     <li></li>
        ///  </ol>
	 ///     <filename>PropertyTriggerMultipleSetSimple.cs</filename>
        /// </remarks>
        [CoreTestsLoader(CoreTestsTestType.MethodBase)]
        [TestCasePriority("0")]
        [TestCaseArea(@"PropertyEngine\TriggerBase\Trigger\Simple")]
        [TestCaseMethod("RunTest")]
        [TestCaseParams("OnXaml")]
        [TestCaseSupportFile("PropertySimpleCheckBoxMultipleSet.xaml")]
        [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
        public void OnXaml()
        {

            FileStream fs = null;

            Border border;

            if (!File.Exists("PropertySimpleCheckBoxMultipleSet.xaml"))
                throw new Microsoft.Test.TestSetupException("The file SimpleCheckButton.xaml was not found");

            try
            {
                using(CoreLogger.AutoStatus("Creating a tree"))
                {
               
                    fs = new FileStream("PropertySimpleCheckBoxMultipleSet.xaml", FileMode.Open);

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
        /// Adds 1 Trigger to a Checkbutton to change two different properties Background and ForeGround on Code
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///     <li></li>
        ///  </ol>
	 ///     <filename>PropertyTriggerMultipleSetSimple.cs</filename>
        /// </remarks>
        [CoreTestsLoader(CoreTestsTestType.MethodBase)]
        [TestCasePriority("0")]
        [TestCaseArea(@"PropertyEngine\TriggerBase\Trigger\Simple")]
        [TestCaseMethod("RunTest")]
        [TestCaseParams("OnCode")]
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
                trigger.Setters.Add(new Setter(Control.ForegroundProperty,Brushes.Blue));

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
                        throw new Microsoft.Test.TestValidationException("The property trigger was not fired on Background");

                    if (_button.Foreground != Brushes.Blue)
                        throw new Microsoft.Test.TestValidationException("The property trigger was not fired on Foreground");



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











