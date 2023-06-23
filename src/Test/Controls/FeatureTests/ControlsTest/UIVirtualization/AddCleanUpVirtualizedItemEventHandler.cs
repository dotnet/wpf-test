using System;
using System.Collections;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Markup;
using System.Threading;
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;
using System.Windows.Data;
using Avalon.Test.ComponentModel.DataSources;
using System.Collections.ObjectModel;

namespace Avalon.Test.ComponentModel.Actions
{

    public class AddCleanUpVirtualizedItemCancelEventHandler : IAction
    {

        public AddCleanUpVirtualizedItemCancelEventHandler()
        {
            eventArgs = new ArrayList();
        }

        /// <summary>
        /// Add AddCleanUpVirtualizedItemEventHandler to ListBox
        /// <param name="frmElement">Control to act upon.</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            ListBox listBox = frmElement as ListBox;

            TestLog.Current.LogStatus(listBox.ToString());

            listBox.AddHandler(VirtualizingStackPanel.CleanUpVirtualizedItemEvent, new CleanUpVirtualizedItemEventHandler(OnCleanupVirtualizedItemCancel), true);

            if (actionParams.Length != 1)
            {
                TestLog.Current.LogStatus("Key to store EventArgs is not given");
            }
            else
            {
                eventArgsKey = actionParams[0] as string;
            }
           
        }

        public void OnCleanupVirtualizedItemCancel(object sender, CleanUpVirtualizedItemEventArgs args)
        {
            TestLog.Current.LogStatus("OnCleanupVirtualizedItem");
            TestLog.Current.LogStatus("Value: " + args.Value.GetType().ToString());
            TestLog.Current.LogStatus("UIElement: " + args.UIElement.GetType().ToString());
            TestLog.Current.LogStatus("Cancel: " + args.Cancel.GetType().ToString());

            TestLog.Current.LogStatus("Setting args.Cancel: true");

            TestLog.Current.LogStatus("Index :" + (sender as ListBox).ItemContainerGenerator.IndexFromContainer(args.UIElement));

            args.Cancel = true;

            if ( (sender as ListBox).ItemContainerGenerator.IndexFromContainer(args.UIElement) == 0)
            {
                eventArgs.Clear();
            }               

            if (eventArgsKey != null)
            {
                if (!StateTable.Contains(eventArgsKey))
                {
                    StateTable.Add(eventArgsKey, eventArgs);
                }

                eventArgs.Add(args);
            }
        }

        private string eventArgsKey = null;
        private ArrayList eventArgs;

    }
}
