using System;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;
using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.Actions
{
    /// <summary>
    /// Add ScrollViewer.ScrollChanged event handler 
    /// </summary>
    public class AddScrollChangedEventAction : IAction
    {

        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            if (actionParams == null) 
                throw new NullReferenceException("actionParams null");

            if (actionParams.Length < 1)
                throw new ArgumentException("Specify key to store ScrollChanged event args in StateTable");

            stateTable_KeyName = actionParams[0] as string;  //Key used to store ScrollChanged event args in StateTable
            frmElement.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(OnScrollChanged), true);
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs args)
        {
            TestLog.Current.LogStatus("OnScrollChanged");
            TestLog.Current.LogStatus("sender : " + sender.ToString());
            TestLog.Current.LogStatus("args.VerticalOffset " + args.VerticalOffset.ToString());
            TestLog.Current.LogStatus("args.ViewportHeight " + args.ViewportHeight.ToString());
            TestLog.Current.LogStatus("===============================================");

            if (StateTable.Contains(stateTable_KeyName))
            {
                StateTable.Remove(stateTable_KeyName);
            }

            StateTable.Add(stateTable_KeyName, args);
        }

        private string stateTable_KeyName = null;
    }

}
