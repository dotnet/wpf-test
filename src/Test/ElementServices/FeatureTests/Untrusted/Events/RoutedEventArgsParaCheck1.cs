// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Events;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.Events
{
    /// <summary>
    /// Check the invalid inputs for RoutedEventArgs.
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Boundary
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  RoutedEventArgsParaCheck1.cs
    /// <para />
    /// <ol>Scenarios covered:
    ///     Create an subclass of RouteEventArgs
    ///     SetSource with null Event
    ///     Set handled before RoutedEvent is set
    ///     Check InvokeEventHandler for null RouteEvent, Null handler and null type
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
   [Test(0, "Events.Boundary", "RoutedEventArgsParaCheck1")]
    public class RoutedEventArgsParaCheck1 : TestCase
    {
        #region Data
        private class MyContentElement : ContentElement
        {
            public MyContentElement ():base() { }
        }
        #endregion data

        #region Constructor
        public RoutedEventArgsParaCheck1() :base(TestCaseType.ContextSupport)
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            CoreLogger.LogStatus("This is a test case checking the exception for RoutedEventArgs");

            Dispatcher context = MainDispatcher;

            MyTmpRoutedEventArgs myArgs = null;

            using (CoreLogger.AutoStatus("Create a MyTmpRoutedEventArgs"))
            {
                myArgs = new MyTmpRoutedEventArgs ();
            }
            using (CoreLogger.AutoStatus("Check invalid input for InvokdEventHandler and handled"))
            {
                myArgs.CheckHandledAndInvokdEventHandler ();
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Memebers
        private class MyTmpRoutedEventArgs : RoutedEventArgs
        {
            public MyTmpRoutedEventArgs () : base() { }
            public void CheckHandledAndInvokdEventHandler ()
            {
                Exception ExpThrown = null;
                
                using (CoreLogger.AutoStatus("RoutedEventArgs.InvokeEventHandler set Handledness with null RoutedEventEvent"))
                {
                    ExpThrown = null;
                    try
                    {
                        Handled = true; 
                    }
                    catch (InvalidOperationException e)
                    {
                        ExpThrown = e;
                    }
                    catch (Exception) { }
                }

                using (CoreLogger.AutoStatus("Validating the Argument exception is thrown"))
                {
                    if (null == ExpThrown)
                        throw new Microsoft.Test.TestValidationException ("Null checking for RoutedEventArgs.Handled: No RoutedEvent set error");
                }

                using (CoreLogger.AutoStatus("RoutedEventArgs.InvokeEventHandler with null RoutedEventEvent"))
                {
                    ExpThrown = null;
                    try
                    {
                        InvokeEventHandler (new RoutedEventHandler (OnRoutedEvent), typeof(CustomControl));
                    }
                    catch (InvalidOperationException e)
                    {
                        ExpThrown = e;
                    }
                    catch (Exception) { }
                }

                using (CoreLogger.AutoStatus("Validating the Argument exception is thrown"))
                {
                    if (null == ExpThrown)
                        throw new Microsoft.Test.TestValidationException ("Null checking for RoutedEventArgs.InvokeEventHandler(): No RoutedEvent set error");
                }

                using (CoreLogger.AutoStatus("Register a RoutedEvent and set it to the RoutedEventArgs"))
                {
                    RoutedEvent routedEvent = EventManager.RegisterRoutedEvent ("BubbleRoutedEventIDjksldiu9873)@**%", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MyContentElement));
                    this.RoutedEvent = routedEvent;
                }

                using (CoreLogger.AutoStatus("RoutedEventArgs.InvokeEventHandler with null handler"))
                {
                    ExpThrown = null;
                    try
                    {
                        InvokeEventHandler (null, typeof(CustomControl));
                    }
                    catch (ArgumentNullException e)
                    {
                        ExpThrown = e;
                    }
                    catch (Exception) { }
                }

                using (CoreLogger.AutoStatus("Validating the Argument exception is thrown"))
                {
                    if (null == ExpThrown)
                        throw new Microsoft.Test.TestValidationException ("Null checking for RoutedEventArgs.InvokeEventHandler(): null handler, error");
                }

                using (CoreLogger.AutoStatus("RoutedEventArgs.InvokeEventHandler with null type"))
                {
                    ExpThrown = null;
                    try
                    {
                        InvokeEventHandler (new RoutedEventHandler (OnRoutedEvent), null);
                    }
                    catch (ArgumentNullException e)
                    {
                        ExpThrown = e;
                    }
                    catch (Exception) { }
                }

                using (CoreLogger.AutoStatus("Validating the Argument exception is thrown"))
                {
                    if (null == ExpThrown)
                        throw new Microsoft.Test.TestValidationException ("Null checking for RoutedEventArgs.InvokeEventHandler(): null type, error");
                }
            }
            private void OnRoutedEvent (object sender, RoutedEventArgs args)
            {
            }
        }
        #endregion
    }
}
