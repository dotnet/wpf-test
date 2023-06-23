using System;
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

    public class StorePropertyValues : IAction
    {
        /// <summary>
        /// StorePropertyValues
        /// <param name="frmElement">Control to act upon.</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            foreach (object obj in actionParams)
            {
                object value = ObjectFactory.GetObjectProperty(frmElement, obj.ToString());
                StateTable.SetValue(obj.ToString(), value);

                TestLog.Current.LogStatus("Key: " + obj.ToString() + " Value: " + value.ToString());
            }

        }

    }
}
