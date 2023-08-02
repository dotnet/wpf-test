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
    ///     <filename>MultiplePropertyTriggersSimple.cs</filename>
    ///</remarks>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]    
    public class MultiplePropertyTriggersSimple : TestCase
    {
        

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public MultiplePropertyTriggersSimple() :base(TestCaseType.HwndSourceSupport)
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
	 ///     <filename>MultiplePropertyTriggersSimple.cs</filename>
        /// </remarks>
        [CoreTestsLoader(CoreTestsTestType.MethodBase)]
        [TestCasePriority("0")]
        [TestCaseArea(@"PropertyEngine\TriggerBase\MultiTrigger")]
        [TestCaseMethod("RunTest")]
        [TestCaseParams("OnCode")]
        [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
        public void OnCode()
        {
            Style style;
            MultiTrigger trigger;
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
                _button.Foreground = Brushes.Blue;

                

                panel.Children.Add(_button);

                AddHello2Panel(panel);

            }


            CoreLogger.AutoStatus("Creating a style with a trigger");
                style = new Style(typeof(CheckBox), _button.Style);

                trigger = new MultiTrigger();

                if (trigger.Conditions.Count != 0)
                    throw new Microsoft.Test.TestValidationException("The Conditions.Count is not 0");


                Condition c1 = new Condition();
                trigger.Conditions.Add(c1);

                if (trigger.Conditions.Count != 1)
                    throw new Microsoft.Test.TestValidationException("The Conditions.Count is not 1");

            CoreLogger.AutoStatus("Creating a style with a trigger 111");
                
                c1.Property = CheckBox.IsCheckedProperty;             
                c1.Value = (Nullable<bool>)true;;
           CoreLogger.AutoStatus("Creating a style with a trigger 1.5");

                if (c1.Property != CheckBox.IsCheckedProperty)
                        throw new Microsoft.Test.TestValidationException("The property is not the expected");
           CoreLogger.AutoStatus("Creating a style with a trigger 1.6");


                if (((bool?)(c1.Value)) != true)
                        throw new Microsoft.Test.TestValidationException("The value is not the expected");

            CoreLogger.AutoStatus("Creating a style with a trigger 222");


                Condition c2 = new Condition();
                c2.Property = Control.ForegroundProperty;             
                c2.Value = Brushes.Red;
                trigger.Conditions.Add(c2);


                
                trigger.Setters.Add(new Setter(Control.BackgroundProperty,Brushes.Indigo));
                

                style.Triggers.Add(trigger);

                _button.Style = style;

            Source.RootVisual = border;

 

            //Pending further checkin here.
             Dispatcher.Run();
            //_isPass = true;

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

                if (_button.Background == Brushes.Indigo)
                    throw new Microsoft.Test.TestValidationException("Expecting the default Indigo");

                _button.Foreground = Brushes.Red;

                
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(validation), null);
                return null;   
            }


            object validation(object o)
            {
                using(CoreLogger.AutoStatus("Validating the Trigger works correctly"))
                {
                    if (_button.Background  != Brushes.Indigo)
                        throw new Microsoft.Test.TestValidationException("The property trigger was not fired on Background");

                }



               Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(lastValidation), null);

               _button.IsChecked = false;

                return null;   
            }




            object lastValidation(object o)
            {

                using(CoreLogger.AutoStatus("Validating the Trigger works correctly"))
                {
                    if (_button.Background == Brushes.Indigo)
                        throw new Microsoft.Test.TestValidationException("The property trigger was not fired on Background");

                    if (_button.Foreground != Brushes.Red)
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










