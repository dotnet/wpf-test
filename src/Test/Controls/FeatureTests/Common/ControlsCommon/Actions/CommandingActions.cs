using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using Avalon.Test.ComponentModel.Utilities;
using System.Reflection;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Input;
using System.Windows.Input;
using System.Collections.Generic;
using Microsoft.Test;
using Microsoft.Test.Threading;
using System.Windows.Threading;

namespace Avalon.Test.ComponentModel.Actions
{
    public static class CommandingActions
    {
        // The order of tests is not important, but they are arranged from less to more
        // demanding, the latter tests make use of behavior proven by earlier tests.
        public static void RunCommandingTests(ICommandSource commandSource)
        {
            //TestLog.Current.LogEvidence("Calling <<Run_IsEnabled_Tests>> for class: \"" + commandSource.GetType().ToString() + "\"");
            ICommandSourceCommandActions.Run_IsEnabled_Tests(commandSource);
            
            //TestLog.Current.LogEvidence("Calling <<Run_CanExecute_Tests>> for class: \"" + commandSource.GetType().ToString() + "\"");
            ICommandSourceCommandActions.Run_CanExecute_Tests(commandSource);

            //TestLog.Current.LogEvidence("Calling <<Run_Executed_IsEnabledFalse_Tests>> for class: \"" + commandSource.GetType().ToString() + "\"");
            ICommandSourceCommandActions.Run_Executed_IsEnabledFalse_Tests(commandSource);

            //TestLog.Current.LogEvidence("Calling <<Run_Executed_IsEnabledTrue_Tests>> for class: \"" + commandSource.GetType().ToString() + "\"");
            ICommandSourceCommandActions.Run_Executed_IsEnabledTrue_Tests(commandSource);
        }
    }
}


