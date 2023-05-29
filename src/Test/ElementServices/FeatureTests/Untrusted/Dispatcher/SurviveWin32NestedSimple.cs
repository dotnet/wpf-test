// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Media.Animation;
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
using Microsoft.Test.Threading;
using System.Runtime.InteropServices;
using Avalon.Test.CoreUI.Source;
using Microsoft.Test.Win32;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.Framework.Dispatchers;
using System.Windows.Controls;
using Microsoft.Win32;
using Avalon.Test.CoreUI.CoreInput.Common;
using Microsoft.Test;
using Microsoft.Test.Discovery;

namespace Avalon.Test.CoreUI.Threading
{
    /******************************************************************************
    * CLASS:          SimpleNestedMessageLoop
    ******************************************************************************/
    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>HwndDispatcherMultipleThreads.cs</filename>
    ///</remarks>
    [TestDefaults]
    public class SurviveWin32NestedSimple : TestCaseBase, IHostedTest
    {
        ITestContainer _testContainer = null;
            
        /// <summary>
        /// Represents the current ITestContainer for this IHostedTest.
        /// </summary>
        public ITestContainer TestContainer
        {
            get
            {
                return _testContainer;
            }
            set
            {
                _testContainer = value;
            }
        }

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public SurviveWin32NestedSimple() 
        {

        }
        

        /// <summary>
        /// Start a nested pump on a queue item. Later click a button and wait for the click on the click event exit the win32
        /// nested dispatcher
        /// </summary>
        public void ClickNested()
        {
            MouseHelper.MoveOnVirtualScreenMonitor();
            OnStartup();
            TestContainer.RequestStartDispatcher();
        }     


        // <summary>
        // </summary>        
        void OnStartup() 
        {

            StackPanel panel = new StackPanel();


            _b = new Button();
            _b.Click+= new RoutedEventHandler(ClickHere);

            _b.Content = "Click Here";
            _b.Width = 100;
            _b.Height = 100;
            panel.Children.Add(_b);

            CoreLogger.LogStatus("Creating Avalon Tree (Window->StackPanel->Button)");
            _surface = TestContainer.DisplayObject(panel, 10,10,500,500);
            CoreLogger.LogStatus("window.Show called");
            DispatcherHelper.EnqueueBackgroundCallback(new DispatcherOperationCallback(NestedWin32Loop), true);
            CoreLogger.LogStatus("Posting to Start Input");
            DispatcherHelper.EnqueueBackgroundCallback(new DispatcherOperationCallback (StartInput), null);

        }

        /// <summary>
        /// </summary>        
        /// <remarks>
        /// </remarks>
        void Exit()
        {
            if (!_isPassedClickOne)
            {
                CoreLogger.LogTestResult(false,"");
            }
            if (!_isPassedClickTwo)
            {
                CoreLogger.LogTestResult(false,"");
            }
            if (_dispatcherWrapper.DeepestNestingLevel==0)
            {
                CoreLogger.LogTestResult(false,"Nested Pump was not called");
            }

            TestContainer.EndTest();

        }



        Button _b;

        object NestedWin32Loop(object doInputNeeded)
        {
            _dispatcherWrapper = new DispatcherWrapper(Dispatcher.CurrentDispatcher,DispatcherType.Avalon);
            _dispatcherWrapper.PushNestedLoop(DispatcherType.Win32);

            if ((bool)doInputNeeded)
            {
                CoreLogger.LogStatus("Simulating input... Click on the button");
                DispatcherOperation op = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback (StartInput), null);
            }
            return null;
        }

        object StartInput(object o)
        {
            CoreLogger.LogStatus("Simulating input... Click on the button");
            MouseHelper.Click(_b);
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        private object ExitNested(object o)
        {
            _dispatcherWrapper.PopNestedLoop();
            return null;         
        }


        /// <summary></summary>
        protected void ClickHere(object o, RoutedEventArgs args)
        {   
            CoreLogger.LogStatus("Getting Click Event on the Button");   
            if (_count == 0)
            {
                _isPassedClickOne = true;

                DispatcherOperation op = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(ExitNested), null);
            }
            else
            {
                _isPassedClickTwo = true;
                Exit();
            }
            _count++;
        }

        int _count = 0;
        bool _isPassedClickOne = false;
        bool _isPassedClickTwo = false;

        DispatcherWrapper _dispatcherWrapper = null;

        /// <summary>
        /// Start a nested pump on a queue item. Later start an animation async and validates that the Repeated event on the animation is fired three times.
        /// Stop the win32 nested pump and listen for the Repeated event is called three more times.
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///      <li>Create a Win32Dispatcher</li>
        ///  </ol>
        ///     <filename>HwndDispatcherMultipleThreads.cs</filename>
        /// </remarks>
        public void RunWithAnimation()
        {
            MouseHelper.MoveOnVirtualScreenMonitor();


            CoreLogger.LogStatus("Posting Nested Win32 on Background priority");
            DispatcherHelper.EnqueueBackgroundCallback(new DispatcherOperationCallback(NestedWin32Loop), false);
            CoreLogger.LogStatus("Posting adding an animation on the tree on SystemIdle priority");
            DispatcherHelper.EnqueueBackgroundCallback(new DispatcherOperationCallback(ShowAnimationSurface), null);


            
            CoreLogger.LogStatus("App.Run");
            TestContainer.RequestStartDispatcher();

        } 



        void AnimationRepeatHandler(object o, EventArgs args)
        {
          if ((((Clock)o).CurrentIteration > _count))
          {
            _count++;
            CoreLogger.LogStatus("Repeated Event Called: " + _count.ToString());
            if (_count == 3)
            {
                _isPassedClickOne = true;
                CoreLogger.LogStatus("Posting an exit for the nested pump");
                _dispatcherWrapper.PopNestedLoop();
            }  

            if (_count == 6)
            {
                _isPassedClickTwo = true;
                Exit();

            }
          }
        }


        object ShowAnimationSurface(object o)
        {


            Canvas body = new Canvas();
            body.Background     = Brushes.Purple;
            
            TextBox textbox = new TextBox();
            body.Background     = Brushes.Lavender;
            textbox.Width       = 100d;
            textbox.Height      =  50d;
            
            body.Children.Add(textbox);

            DoubleAnimation anim = new DoubleAnimation();
            anim.From               = 100d;
            anim.To                 = 200d;
            anim.BeginTime          = TimeSpan.FromMilliseconds(4000);
            anim.Duration           = new Duration(TimeSpan.FromMilliseconds(2000));
            anim.FillBehavior       = FillBehavior.HoldEnd;
            anim.RepeatBehavior     = new RepeatBehavior(7);


            anim.CurrentTimeInvalidated += new EventHandler(AnimationRepeatHandler);

            
            textbox.BeginAnimation(TextBox.WidthProperty, anim);

            _surface = TestContainer.DisplayObject(body, 10,10,500,500);
            
            return null;
        }               

        Surface _surface = null;
    }       
}

