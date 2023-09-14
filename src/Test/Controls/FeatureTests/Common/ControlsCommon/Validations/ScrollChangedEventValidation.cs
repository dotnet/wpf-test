using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;

using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.Validations
{

    /// <summary>
    /// This validation verifies that ScrollChangedEvent fired
    /// </summary>
    public class ScrollChangedEventValidation : IValidation
    {
        public bool Validate(params object[] validationParams)
        {
            if (validationParams == null)
                throw new NullReferenceException("validationParams null");

            object[] param = validationParams[1] as object[];

            if (param.Length < 1)
                throw new ArgumentException("Specify key to retrieve ScrollChanged event args in StateTable");

            ScrollChangedEventArgs scrollChangedEventArgs = null;

            stateTable_KeyName = param[0] as string;  //Key used to store ScrollChanged event args in StateTable

            if (StateTable.Contains(stateTable_KeyName))
            {
                scrollChangedEventArgs = StateTable.Get(stateTable_KeyName) as ScrollChangedEventArgs;
            }

            if (scrollChangedEventArgs == null)
            {
                TestLog.Current.LogStatus("FAIL:ScrollChangedEvent not fired");
                return false;
            }
            else
            {
                TestLog.Current.LogStatus("PASS:ScrollChangedEvent fired");
                return true;
            }
        }

        private string stateTable_KeyName = null;
    }

}


