// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

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
    /// Add instance handler for bubble event onto content element and raise event
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\ContentElement
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  RaiseEventOnContentElement.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Create a RoutedEvent</li>
    /// <li>Create a new Content Element</li>
    /// <li>Add 3 handlers onto the content element, HandedEventToo=false</li>
    /// <li>Raise Event and check the times handle runs</li>
    /// <li>Check Argument has been set correctly in every handler raised</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.ContentElement", "RaiseEventCheckArgsOnContentElement")]
    public class RaiseEventCheckArgsOnContentElement : TestCase
    {
        #region data
        private int _executedCount;
        CustomContentElement _myContentElement = null;
        RoutedEvent _bubbleEvent = null;
        RoutedEventArgs _myArgs = null;
        #endregion data


        #region Constructor
        public RaiseEventCheckArgsOnContentElement() :base(TestCaseType.ContextSupport)
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
            CoreLogger.LogStatus ("This is a test case adding instance handlers onto ContentElement");

            Dispatcher context = MainDispatcher;
            

            using (CoreLogger.AutoStatus ("Creating a new content element"))
            {
                _myContentElement = new CustomContentElement();
            }

            using (CoreLogger.AutoStatus ("Creating a new Bubble Event Name"))
            {
                _bubbleEvent = EventManager.RegisterRoutedEvent ("BubbleRoutedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomContentElement));
            }

            using (CoreLogger.AutoStatus ("Adding Handlers"))
            {
                _myContentElement.AddHandler (_bubbleEvent, new RoutedEventHandler (MyHandler), true);
                _myContentElement.AddHandler (_bubbleEvent, new RoutedEventHandler (MyHandler), true);
                _myContentElement.AddHandler (_bubbleEvent, new RoutedEventHandler (MyHandler), true);
            }

            using (CoreLogger.AutoStatus ("Raising Event"))
            {
                _myArgs = new RoutedEventArgs(_bubbleEvent, _myContentElement);
                _myContentElement.RaiseEvent(_myArgs);
            }

            using (CoreLogger.AutoStatus ("Checking times handled"))
            {
                if (3 != _executedCount)
                    throw new Microsoft.Test.TestValidationException ("Should be handed only once, actually run: " + _executedCount.ToString () + "times");
            }
            using (CoreLogger.AutoStatus ("Checking handled args"))
            {
                if (_myArgs.Source != _myContentElement)
                    throw new Microsoft.Test.TestValidationException ("Args.Source has not been set correctly");

                if (_myArgs.OriginalSource != _myContentElement)
                    throw new Microsoft.Test.TestValidationException ("Args.OriginalSource has not been set correctly");

                if (_myArgs.RoutedEvent != _bubbleEvent)
                    throw new Microsoft.Test.TestValidationException ("Args.RoutedEvent has not been set correctly");

                if (_myArgs.Handled != false)
                    throw new Microsoft.Test.TestValidationException ("Args.Handled has not been set correctly");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        private void MyHandler (object sender, RoutedEventArgs args)
        {
            _executedCount++;
            if (_myArgs.Source != _myContentElement)
                throw new Microsoft.Test.TestValidationException ("Args.Source has not been set correctly");

            if (_myArgs.OriginalSource != _myContentElement)
                throw new Microsoft.Test.TestValidationException ("Args.OriginalSource has not been set correctly");

            if (_myArgs.RoutedEvent != _bubbleEvent)
                throw new Microsoft.Test.TestValidationException ("Args.RoutedEvent has not been set correctly");

            if (_myArgs.Handled != false)
                throw new Microsoft.Test.TestValidationException ("Args.Handled has not been set correctly");
        }
        #endregion 
    }
}
