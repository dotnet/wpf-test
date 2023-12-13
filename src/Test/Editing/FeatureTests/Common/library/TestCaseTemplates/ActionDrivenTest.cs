// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Define Action driven test case framework

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/TestCaseTemplates/ActionDrivenTest.cs $")]

namespace Test.Uis.TestTypes
{
    #region Namespaces.

    using System;
    using System.Xml;
    using System.Collections;
    using System.Threading; 
    using System.Windows.Threading;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Documents;

    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Base class for ActionDrivenTest-based cases.
    /// </summary>
    /// <remarks>
    /// The ultimate goal for this class is to enable
    /// scriptable tests for user-interaction tests.
    /// This is the simpliest usage. More advanced usage
    /// will include inherriting from ActionDrivenTest,
    /// write your own function and create ActionItem (public)
    /// and call ActionManager.Current.ProcessActionItem
    /// This allows xml definied action items interleaving with
    /// action items created in code.
    /// </remarks>
    public class CustomActionDrivenTest : CustomTestCase
    {
        #region Constructors.

        /// <summary>
        /// Creates a new CustomActionDrivenTest instance.
        /// </summary>
        protected CustomActionDrivenTest(): base ()
        {
            string xamlPage = ConfigurationSettings.Current.GetArgument("XamlPage");

            if (!String.IsNullOrEmpty (xamlPage))
            {
                base.StartupPage = xamlPage;
            }
        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>
        /// When the test case runs, we retrieve the xml reader
        /// Child classes should *not* override this method
        /// </summary>
        public sealed override void RunTestCase()
        {

#if TRACING_RANDOM_FAILURES
            InputLogger.EnableInputLogging();
#endif
            //
            // We initialize this to false
            // This variable will be set true when there's
            // ActionItem processed. If this remains false
            // after the code goes through the while loop below
            // it means that the testxml doesn't contain any
            // ActionItem for this test case, in which case
            // it is reckoned as a test error and the tester
            // should be notified
            //
            bool bActionItemProcessedEver = false;

            this._xmlReader = ConfigurationSettings.Current.GetXmlBlockReader ();

            System.Diagnostics.Debug.Assert(this._xmlReader != null);

            //
            // start going through each of the Action element
            //
            while (this._xmlReader.Read())
            {
                switch (this._xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (this._xmlReader.Name == "Action")
                        {
                            bActionItemProcessedEver = true;
                            ProcessOneActionItemBlock();
                        }

                        break;
                    default:
                        break;
                }
            }

            //
            // see comment at the declaration of this variable
            // bActionItemProcessedEver = false means an error
            // in testxml. The tester should be notified by an exception
            //
            if (!bActionItemProcessedEver)
            {
                string message = String.Format("No ActionItem is found for this test case name [{0}], Please check testxml", ConfigurationSettings.Current.GetArgument("TestName"));

                throw new InvalidOperationException(message);
            }

            ActionDrivenTestRunTestCase();
            this._enumerator = ActionManager.Current.GetEnumerator();

            // start the sequence of executing each of the Action item
            QueueHelper.Current.QueueDelegate(new SimpleHandler(DispatchNextActionItem));
        }

        /// <summary>
        /// enumerator through the ActionItem list and dispatch every single one of them
        /// at this end of this method it posts another item to call itself so that it loops
        /// until all the ActionItems exhaust.
        /// </summary>
        private void DispatchNextActionItem()
        {
            if (this._thread != null)
            {
                this._thread.Join();

                //
                // we null out the temp variables
                // to facilitate executing ActionItem in another thread
                //
                this._thread = null;
                this._actionItemForAnotherThread = null;
            }


            if (this._enumerator.MoveNext())
            {
                ActionItem actionItem = this._enumerator.Current as ActionItem;

                Logger.Current.Log("Dispatching item named [{0}], ID = [{1}]", actionItem.Name, actionItem.ID);

                if (actionItem == null)
                {
                    string output = String.Format("Current element is not an ActionItem");

                    throw new InvalidOperationException(output);
                }

                if (!actionItem.UseWorkerThread)
                {
                    actionItem.InvokeAction();

                    //
                    // I have seen some random failures when we inject some input
                    // and the next Action Item is dispatched before the input is processed.
                    // this can be a race condition in which the kernel has not yet dispatched the
                    // item when Avalon is ready to dispatch the idle queued items. We sleep some time here so that
                    // we want to make sure that the kernel has got enough time to process the injected input
                    // please notice that input injection is asynchronise and there's no guarantee that it will
                    // happen in 1.2 seconds, but 1.2 seconds "should be" enough for the input to get through kernel
                    // and be dispatched
                    //

                    Thread.Sleep(1200);

                    QueueHelper.Current.QueueDelegate(new SimpleHandler(DispatchNextActionItem));
                }
                else
                {
                    this._actionItemForAnotherThread = actionItem;

                    if (this._thread != null)
                    {
                        throw new InvalidOperationException("Thread should be null when it gets here");
                    }
                    this._thread = new Thread(new ThreadStart(InvokeItemInWorkerThread));
                    this._thread.SetApartmentState(ApartmentState.STA);
                    this._thread.Start();
                    Thread.Sleep(0);
                }
            }
            else
            {
                if (!ConfigurationSettings.Current.GetArgumentAsBool("NoExit", false))
                {
                    QueueHelper.Current.QueueDelegate(new SimpleHandler(Logger.Current.ReportSuccess));
                }
            }
        }

        /// <summary>
        /// Child classes can override this
        /// as a substitute of CustomTestCase.RunTestCase
        /// </summary>
        protected virtual void ActionDrivenTestRunTestCase()
        {
        }

        #endregion Public methods.


        #region Private methods

        /// <summary>
        /// Invokes the action item.
        /// </summary>
        private object InvokeActionOnItem(object arg)
        {
            if (arg == null)
            {
                throw new ArgumentNullException("arg");
            }

            ActionItem item = arg as ActionItem;
            if (item == null)
            {
                throw new ArgumentException(
                    "Item should be of type ActionItem, not [" +
                    item.GetType() + "].", "arg");
            }
            item.InvokeAction();

            return null;
        }

        /// <summary>
        /// Warning: This method is a stub to call ActionItem.InvokeAction
        /// in a worker thread. This is using the main thread ui context to
        /// queue an item for next ActionItem execution
        /// </summary>
        private void InvokeItemInWorkerThread()
        {
            DispatcherOperationCallback callback;
            ActionItem arg;

            if (this._actionItemForAnotherThread != null)
            {
                Thread.Sleep(1000);

                // Perform the invocation in the correct Dispatcher.
                callback = new DispatcherOperationCallback(InvokeActionOnItem);
                arg = this._actionItemForAnotherThread;

                GlobalCachedObjects.Current.MainDispatcher.Invoke(DispatcherPriority.Normal, callback, arg);

                Thread.Sleep(1200);
            }

            (new QueueHelper(GlobalCachedObjects.Current.MainDispatcher)).QueueDelegate(new SimpleHandler(DispatchNextActionItem));
        }

        /// <summary>
        /// we get the xml block which is parsed here.
        /// any parsing logic for an Action block will have to be added here
        /// </summary>
        private void ProcessOneActionItemBlock ()
        {
            string nameAttribute = this._xmlReader.GetAttribute ("Name");
            string classNameAttribute = this._xmlReader.GetAttribute ("ClassName");
            string methodNameAttribute = this._xmlReader.GetAttribute ("MethodName");
            string invokeTypeAttribute = this._xmlReader.GetAttribute ("Type");
            string useWorkerThreadAttribute = this._xmlReader.GetAttribute("UseWorkerThread");
            string repeatCountAttribute = this._xmlReader.GetAttribute("RepeatCount");

            bool useWorkerThread = false;

            if (!String.IsNullOrEmpty(useWorkerThreadAttribute) && useWorkerThreadAttribute.ToLower() == "true")
            {
                useWorkerThread = true;
            }

            // array list for the arguments to call the method / Property
            ArrayList args = new ArrayList();

            InvokeType invokeType = (InvokeType)
                Enum.Parse(typeof(InvokeType), invokeTypeAttribute);

            while (this._xmlReader.Read())
            {
                if (this._xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (this._xmlReader.Name != "Param")
                    {
                        throw new InvalidOperationException("Action XML elements only support Param elements.");
                    }

                    // is there any argument needs to be retrieved from previous actions?
                    // WARNING: Numeric values are *not* allowed to be used as
                    // name. They are treated as steps if they are valid integers.
                    string retrieveFromReturnValue = this._xmlReader.GetAttribute("RetrieveFromReturnValue");
                    ParameterToMethodType paramType = ParameterToMethodType.Direct;
                    string val = String.Empty;
                    string convertToTypeName = this._xmlReader.GetAttribute("ConvertTo");

                    if (!String.IsNullOrEmpty(retrieveFromReturnValue))
                    {
                        paramType = ParameterToMethodType.RetrieveFromReturnValue;
                        val = retrieveFromReturnValue;
                    }
                    else
                    {
                        //
                        // Use the value directly off the Value attribute.
                        // Empty values are accepted as empty strings, but
                        // missing values are rejected.
                        // *null flags a null value.
                        //
                        val = this._xmlReader.GetAttribute("Value");

                        if (val == null)
                        {
                            throw new InvalidOperationException("Parameter " + args.Count + " has no value defined.");
                        }

                        //
                        // *null in xml means a real null
                        //
                        if (val == "*null")
                        {
                            val = null;
                        }
                    }

                    args.Add(new ParameterToMethod(val, paramType, convertToTypeName));
                }
                else if (this._xmlReader.NodeType == XmlNodeType.EndElement)
                {
                    // make sure that we see </Action>
                    if (this._xmlReader.Name == "Action")
                    {
                        break;
                    }
                }
            }

            int repeatCount = 1;

            if (!String.IsNullOrEmpty(repeatCountAttribute))
            {
                try
                {
                    repeatCount = Int32.Parse(repeatCountAttribute);
                }
                catch (FormatException)
                {
                    string message = String.Format("Cannot parse {0} to Int32. Please check testxml", repeatCountAttribute);

                    throw new FormatException(message);
                    
                }
            }

            for (int i = 1; i <= repeatCount; i++)
            {
                // create an Action item with the method name, class name and arguments.
                ActionItem actionItem = new ActionItem(nameAttribute, 
                           invokeType, 
                           classNameAttribute, 
                           methodNameAttribute, 
                           useWorkerThread, 
                           (object[])(args.ToArray()));
  
                string messageStr = String.Format("Creating action {0}: {1} {2} {3}", 
                       actionItem.ID.ToString(), 
                       nameAttribute, 
                       classNameAttribute, 
                       methodNameAttribute);

                if (actionItem.UseWorkerThread)
                {
                    messageStr += " [UseWorkerThread]";
                }

                if (repeatCount != 1)
                {
                    messageStr += " [Repeat item. Count = ";
                    messageStr += i.ToString();
                    messageStr += "]";
                }

                Logger.Current.Log(messageStr);

                ActionManager.Current.AddActionItemToList(actionItem);
            }
        }

        #endregion Private methods.


        #region Private fields.

        private XmlReader _xmlReader;

        private IEnumerator _enumerator = null;

        private Thread _thread = null;

        private ActionItem _actionItemForAnotherThread = null;

        #endregion Private fields.
    }
}
