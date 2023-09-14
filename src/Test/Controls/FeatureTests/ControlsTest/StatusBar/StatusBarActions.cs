//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using Avalon.Test.ComponentModel.Utilities;

namespace Avalon.Test.ComponentModel.Actions
{
    class SelectTabItemAction : IAction
    {
        /// <summary>
        /// Index of currently selected TabItem
        /// </summary>
        static int s_Index = 0;

        /// <summary>
        /// Action: change current selected TabItem index
        /// </summary>
        /// <param name="frmElement">Test Object</param>
        /// <param name="actionParams">null</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            TabControl tabCtrl = frmElement as TabControl;
            tabCtrl.SelectedIndex = (++s_Index) % tabCtrl.Items.Count; 
            QueueHelper.WaitTillQueueItemsProcessed();
        }
    }
}
