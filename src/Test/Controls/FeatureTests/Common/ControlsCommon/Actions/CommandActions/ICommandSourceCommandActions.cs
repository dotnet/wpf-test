using System;
using System.Windows;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Controls;
using Microsoft.Test.TestTypes;
using Microsoft.Test;
using Microsoft.Test.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Actions;

namespace Avalon.Test.ComponentModel.Actions
{
    /// <summary>
    /// Tests to verify ICommandSource behavior with respect to:
    /// 1. source fires "PreviewCanExecute" and "CanExecute" events when
    ///    its Command property is assigned a RoutedCommand
    /// 2. source is 'enabled' or 'disabled' in agreement with how
    ///    CanExecuteRoutedEventArgs.CanExecute = true/false was set
    ///    in its CanExecute event handler.
    /// 3. source fires "PreviewExecuted" and "Executed" events
    ///    when ENABLED but not DISABLED source is left-clicked.
    /// <parameter_constraints>
    /// Required: the commandSource is a direct or indirect logical child of a panel.
    /// Required: the commandSource has a public ICommand Command {set;} accessor.
    /// </parameterconstraints>
    /// </summary>
    public static class ICommandSourceCommandActions
    {
        #region private members

        private static MethodInfo GetCommandPropertySetMethod(FrameworkElement sourceElement)
        {
            // This statement won't fail because ICommandSource exposes a public 'Command' {get;} accessor.
            //
            PropertyInfo commandProperty = sourceElement.GetType().GetProperty(
                "Command",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                typeof(ICommand),
                new Type[0],
                null);

            // .........................................................
            // This statement can fail if source class does not also implement a 'Command' {set;} accessor,
            //
            MethodInfo setMethod = commandProperty.GetSetMethod();
            if (null == setMethod)
            {
                throw new TestValidationException("Command-source class must implement a 'Command' { set;} accessor.");
            }
            return setMethod;
        }

        private static DependencyObject FindParentPanel(DependencyObject element)
        {
            if (null == element || element is Panel)
            {
                return element;
            }
            return FindParentPanel(LogicalTreeHelper.GetParent(element));
        }

        #endregion

        #region public members

        /// <summary>
        /// Verify that commandSource framework element fires PreviewCanExecute and CanExecute
        /// routed events when its public ICommand Command property is assigned a RoutedCommand.
        /// </summary>
        /// <param name="commandSource">
        /// See class summary
        /// </param>
        public static void Run_CanExecute_Tests(ICommandSource commandSource)
        {
            FrameworkElement sourceElement = commandSource as FrameworkElement;
            if (null == sourceElement)
            {
                throw new TestValidationException("Command-source class must be a FrameworkElement");
            }

            MethodInfo commandSetMethod = GetCommandPropertySetMethod(sourceElement);

            Panel parentPanel = (Panel)FindParentPanel(sourceElement);
            if (null == parentPanel)
            {
                throw new TestValidationException("Command-source element does not have a direct or indirect Panel parent");
            }

            RoutedCommand routedCommand = new RoutedCommand("Test_CanExecute", sourceElement.GetType(), null);

            CommandBinding commandBinding = new CommandBinding(routedCommand);

            parentPanel.CommandBindings.Add(commandBinding);

            String[] routedEventNames = new String[] { "PreviewCanExecute", "CanExecute" };
            foreach (String eventName in routedEventNames)
            {
                CanExecuteRoutedEventArgs savedCanExecuteRoutedEventArgs = null;

                DispatcherHelper.DoEvents(0, DispatcherPriority.Normal);

                EventHelper.ExpectEvent<CanExecuteRoutedEventArgs>(
                    delegate()
                    {
                        QueueHelper.WaitTillQueueItemsProcessed();
                        commandSetMethod.Invoke(sourceElement, new Object[] { routedCommand });
                        // QueueHelper.WaitTillQueueItemsProcessed() call is necessary
                        // here. Otherwise the event will not have bubbled or tunneled
                        // in time for ExpectEvent's surrogate handler to receive it.
                        QueueHelper.WaitTillQueueItemsProcessed();
                    },
                    commandBinding,
                    eventName,
                    delegate(Object sender, CanExecuteRoutedEventArgs args) { savedCanExecuteRoutedEventArgs = args; });

                if (null == savedCanExecuteRoutedEventArgs)
                {
                    throw new TestValidationException(
                        "Failed!: Assignment of a RoutedCommand to property: "
                        + sourceElement.GetType().FullName + ".Command did not raise a "
                        + eventName + " event");
                }
            }
            // clean-up in case this method is called again with
            // the same panel or sourceElement
            routedCommand = null;
            commandSetMethod.Invoke(sourceElement, new Object[] { null });
            parentPanel.CommandBindings.Remove(commandBinding);
            commandBinding = null;
        }// --- end of: Run_CanExecute_Tests ---

        /// <summary>
        /// Verify that commandSource framework element bool IsEnabled property corresponds
        /// to the value assigned in the CanExecuteRoutedCommandHandler
        /// </summary>
        /// <param name="commandSource">
        /// See class summary
        /// </param>
        public static void Run_IsEnabled_Tests(ICommandSource commandSource)
        {
            FrameworkElement sourceElement = commandSource as FrameworkElement;
            if (null == sourceElement)
            {
                throw new TestValidationException("Command-source class must be a FrameworkElement");
            }

            MethodInfo commandSetMethod = GetCommandPropertySetMethod(sourceElement);

            Panel parentPanel = (Panel)FindParentPanel(sourceElement);
            if (null == parentPanel)
            {
                throw new TestValidationException("Command-source element does not have a direct or indirect Panel parent");
            }

            RoutedCommand routedCommand = new RoutedCommand("Test_IsEnabled", sourceElement.GetType(), null);

            CommandBinding commandBinding = new CommandBinding(routedCommand);

            parentPanel.CommandBindings.Add(commandBinding);

            CanExecuteRoutedEventArgs savedCanExecuteRoutedEventArgs = null;
            Boolean expected = false;

            CanExecuteRoutedEventHandler canExecuteRoutedEventHandler =
                delegate(Object sender, CanExecuteRoutedEventArgs args)
                {
                    savedCanExecuteRoutedEventArgs = args; args.CanExecute = expected;
                };

            do // execute twice -- first time: expected == false, second time: expected == true
            {
                commandBinding.CanExecute += canExecuteRoutedEventHandler;

                // This statement should trigger the canExecuteEventHandler
                commandSetMethod.Invoke(sourceElement, new Object[] { routedCommand });

                QueueHelper.WaitTillQueueItemsProcessed();

                if (null == savedCanExecuteRoutedEventArgs)
                {
                    throw new TestValidationException("commandBinding.CanExecute handler not called");
                }

                // Verify that IsEnabled state has been affected by CanExecuteRoutedEventHandler true/false
                if (sourceElement.IsEnabled != expected)
                {
                    throw new TestValidationException(
                        "Failed!: "
                        + sourceElement.GetType().FullName + ".IsEnabled value {actual = "
                        + (!expected).ToString() + ", expected = " + expected.ToString() + "}");
                }

                // reset for second do-loop iteration
                commandSetMethod.Invoke(sourceElement, new Object[] { null });
                savedCanExecuteRoutedEventArgs = null;
                expected = !expected;
            } while (true == expected);
            // clean-up in case this method is called again with
            // the same panel or sourceElement
            routedCommand = null;
            commandSetMethod.Invoke(sourceElement, new Object[] { null });
            parentPanel.CommandBindings.Remove(commandBinding);
            commandBinding = null;
        }// --- end of: Run_IsEnabled_Tests ---


        /// <summary>
        /// Verify that if commandSource IsEnabled-state is true then commandSource
        /// DOES fire PreviewExecuted and Executed routed events when clicked.
        /// </summary>
        /// <param name="commandSource">
        /// See class summary
        /// </param>
        public static void Run_Executed_IsEnabledTrue_Tests(ICommandSource commandSource)
        {
            FrameworkElement sourceElement = commandSource as FrameworkElement;
            if (null == sourceElement)
            {
                throw new TestValidationException("Command-source class must be a FrameworkElement");
            }

            MethodInfo commandSetMethod = GetCommandPropertySetMethod(sourceElement);

            Panel parentPanel = (Panel)FindParentPanel(sourceElement);
            if (null == parentPanel)
            {
                throw new TestValidationException("Command-source element does not have a direct or indirect Panel parent");
            }

            RoutedCommand routedCommand = new RoutedCommand("Test_Executed", sourceElement.GetType(), null);

            CommandBinding commandBinding = new CommandBinding(routedCommand);
            parentPanel.CommandBindings.Add(commandBinding);

            String[] routedEventNames = new String[] { "PreviewExecuted", "Executed" };

            //
            CanExecuteRoutedEventArgs savedCanExecuteRoutedEventArgs;

            // Force canExecuteRoutedEventHandler to always respond true
            Boolean expectEnabled = true;

            CanExecuteRoutedEventHandler canExecuteRoutedEventHandler =
                delegate(Object sender, CanExecuteRoutedEventArgs args)
                {
                    savedCanExecuteRoutedEventArgs = args; args.CanExecute = expectEnabled;
                };

            commandBinding.CanExecute += canExecuteRoutedEventHandler;

            savedCanExecuteRoutedEventArgs = null;

            // cause CanExecute routed event to be raised and handler invoked.
            commandSetMethod.Invoke(sourceElement, new Object[] { routedCommand });
            Debug.Assert(sourceElement.IsEnabled == expectEnabled);

            // Verify canExecuteRoutedEventHandler executed
            QueueHelper.WaitTillQueueItemsProcessed();
            if (null == savedCanExecuteRoutedEventArgs)
            {
                throw new TestValidationException("commandBinding.CanExecute event never processed.");
            }

            // source ENABLED click-verification
            foreach (String eventName in routedEventNames)
            {
                ExecutedRoutedEventArgs savedExecutedRoutedEventArgs = null;

                if (sourceElement.IsEnabled != expectEnabled)
                {
                    throw new TestValidationException(
                        "Unexpected!: " + sourceElement.GetType().FullName
                        + " expected: IsEnabled == " + expectEnabled.ToString() + ", actual: IsEnabled == " + sourceElement.IsEnabled.ToString());
                }

                EventHelper.ExpectEvent<ExecutedRoutedEventArgs>(
                    delegate()
                    {
                        QueueHelper.WaitTillQueueItemsProcessed();
                        Input.MoveToAndClick(sourceElement);
                        // QueueHelper.WaitTillQueueItemsProcessed() call is necessary
                        // here. Otherwise the event will not have bubbled or tunneled
                        // in time for ExpectEvent's surrogate handler to receive it.
                        QueueHelper.WaitTillQueueItemsProcessed();
                    },
                    commandBinding,
                    eventName,
                    delegate (Object sender, ExecutedRoutedEventArgs args)
                    {
                        savedExecutedRoutedEventArgs = args;
                    });

                if (null == savedExecutedRoutedEventArgs)
                {
                    throw new TestValidationException(
                        "Failed!: Click on IsEnabled == " + sourceElement.IsEnabled.ToString() + ", "
                        + sourceElement.GetType().FullName + " DID NOT RAISE a "
                        + eventName + " event");
                }
            }

            // clean-up redundantly in case this method is called again with
            // the same panel or sourceElement

            commandSetMethod.Invoke(sourceElement, new Object[] { null });
            QueueHelper.WaitTillQueueItemsProcessed();

            expectEnabled = false; // <-- Workaround for DemoBug
            commandSetMethod.Invoke(sourceElement, new Object[] { routedCommand });
            QueueHelper.WaitTillQueueItemsProcessed();

            commandSetMethod.Invoke(sourceElement, new Object[] { null });
            QueueHelper.WaitTillQueueItemsProcessed();

            parentPanel.CommandBindings.Remove(commandBinding);

            routedCommand = null;
            commandBinding = null;
        } // --- end of: Run_Executed_IsEnabledTrue_Tests ---

        /// <summary>
        /// Verify that if commandSource IsEnabled-state is false then commandSource
        /// DOES NOT fire PreviewExecuted or Executed routed events when clicked.
        /// </summary>
        /// <param name="commandSource">
        /// See class summary
        /// </param>
        public static void Run_Executed_IsEnabledFalse_Tests(ICommandSource commandSource)
        {
            FrameworkElement sourceElement = commandSource as FrameworkElement;
            if (null == sourceElement)
            {
                throw new TestValidationException("Command-source class must be a FrameworkElement");
            }

            MethodInfo commandSetMethod = GetCommandPropertySetMethod(sourceElement);

            Panel parentPanel = (Panel)FindParentPanel(sourceElement);
            if (null == parentPanel)
            {
                throw new TestValidationException("Command-source element does not have a direct or indirect Panel parent");
            }

            RoutedCommand routedCommand = new RoutedCommand("Test_Executed", sourceElement.GetType(), null);

            CommandBinding commandBinding = new CommandBinding(routedCommand);
            parentPanel.CommandBindings.Add(commandBinding);

            String[] routedEventNames = new String[] { "PreviewExecuted", "Executed" };

            // -----------------------
            const Boolean expectEnabled = false;
            Int32 executedEventCount = 0;

            CanExecuteRoutedEventArgs savedCanExecuteRoutedEventArgs;

            //always answer false
            CanExecuteRoutedEventHandler canExecuteRoutedEventHandler =
                delegate(Object sender, CanExecuteRoutedEventArgs args)
                {
                    savedCanExecuteRoutedEventArgs = args; args.CanExecute = expectEnabled;
                };

            ExecutedRoutedEventHandler executedRoutedEventHandler =
                delegate(Object sender, ExecutedRoutedEventArgs args)
                {
                    args.Handled = true; executedEventCount++;
                };

            commandBinding.CanExecute += canExecuteRoutedEventHandler;
            commandBinding.Executed += executedRoutedEventHandler;

            savedCanExecuteRoutedEventArgs = null;
            // cause CanExecute routed event to be raised and handler invoked.
            commandSetMethod.Invoke(sourceElement, new Object[] { routedCommand });
            Debug.Assert(sourceElement.IsEnabled == expectEnabled);

            // Verify canExecuteRoutedEventHandler executed
            QueueHelper.WaitTillQueueItemsProcessed();
            if (null == savedCanExecuteRoutedEventArgs)
            {
                throw new TestValidationException("commandBinding.CanExecute event never processed.");
            }

            // Note that unlike the IsEnabled == true test method
            // We do not expect an event, and so EventHelper.ExpectEvent is not as useful.
            // Our executedRoutedEventHandler should never be invoked but if it does it will
            // change the value of executedEventCount.
            foreach (String eventName in routedEventNames)
            {
                if (sourceElement.IsEnabled != expectEnabled)
                {
                    throw new TestValidationException(
                        "Unexpected!: " + sourceElement.GetType().FullName
                        + " expected: IsEnabled == " + expectEnabled.ToString() + ", actual: IsEnabled == " + sourceElement.IsEnabled.ToString());
                }

                executedEventCount = 0;

                Input.MoveToAndClick(sourceElement);
                QueueHelper.WaitTillQueueItemsProcessed();

                if (executedEventCount > 0)
                {
                    throw new TestValidationException(
                        "Failed!: Click on IsEnabled == " + sourceElement.IsEnabled.ToString() + ", "
                        + sourceElement.GetType().FullName + " RAISED an "
                        + eventName + " event");
                }
            }

            // clean-up redundantly in case this method is called again with
            // the same panel or sourceElement

            commandSetMethod.Invoke(sourceElement, new Object[] { null });
            QueueHelper.WaitTillQueueItemsProcessed();

            commandSetMethod.Invoke(sourceElement, new Object[] { routedCommand });
            QueueHelper.WaitTillQueueItemsProcessed();

            commandSetMethod.Invoke(sourceElement, new Object[] { null });
            QueueHelper.WaitTillQueueItemsProcessed();

            parentPanel.CommandBindings.Remove(commandBinding);

            routedCommand = null;
            commandBinding = null;
        } // --- end of: Run_Executed_IsEnabledFalse_Tests ---

        #endregion
   }
}
