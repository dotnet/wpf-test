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
    ///     <filename>MultipleVisualTriggerSimple.cs</filename>
    ///</remarks>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]    
    public class MultipleVisualTriggerSimple : TestCase
    {
        

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public MultipleVisualTriggerSimple() :base(TestCaseType.HwndSourceSupport)
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
        /// Add a MultiTrigger to a checkbutton... the trigger looks for Checked and ForeGround porperty.. After it is set both (validation)
        /// this test case change the background to Indigo color.
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///     <li></li>
        ///  </ol>
     ///     <filename>MultipleVisualTriggerSimple.cs</filename>
        /// </remarks>
        [CoreTestsLoader(CoreTestsTestType.MethodBase)]
        [TestCasePriority("0")]
        [TestCaseArea(@"PropertyEngine\TriggerBase\Trigger\Multiple")]
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

                trigger.Setters.Add( new Setter(Control.BackgroundProperty,Brushes.Indigo));

                style.Triggers.Add(trigger);


                trigger = new Trigger();

                trigger.Property = Control.ForegroundProperty;

                trigger.Value = Brushes.Yellow;

                trigger.Setters.Add( new Setter(Control.FontSizeProperty,48.0));

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
                    _button.Foreground = Brushes.Yellow;
                }

                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(validation), null);
                return null;   
            }


            object validation(object o)
            {

                using(CoreLogger.AutoStatus("Validating the Trigger works correctly"))
                {
                    if (_button.Background != Brushes.Indigo)
                        throw new Microsoft.Test.TestValidationException("The property trigger was not fired for Background");

                    if (_button.FontSize != 48)
                        throw new Microsoft.Test.TestValidationException("The property trigger was not fired for FontSize");

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










