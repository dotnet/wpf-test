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

    public class ChangeItemsInCollection : IAction
    {
        /// <summary>
        /// Change the Orientation of the ListBox's VirtualizingStackPanel
        /// <param name="frmElement">Control to act upon.</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            //Search the Resources section of FrameworkElement's parent to find the data source

            ObservableCollection<Place> col = (frmElement.Parent as FrameworkElement).FindResource("places") as ObservableCollection<Place>;

            if (col == null)
            {
                TestLog.Current.LogStatus("Unable to find Resource of type ObservableCollection with key places on " + frmElement.ToString() + " parent");
                throw new NullReferenceException("Unable to find Resource of type ObservableCollection with key places on " + frmElement.ToString() + " parent");
            }

            if (col.Count < 6)
            {
                TestLog.Current.LogStatus("Expected at least 6 items in ObservableCollection");
                throw new NullReferenceException("Expected at least 6 items in ObservableCollection");
            }

            Place placeToModify1 = col[3];
            Place placeToModify2 = col[5];

            TestLog.Current.LogStatus("Modify Item 3 in Collection From : " + placeToModify1.Name + " " + placeToModify1.State);
            TestLog.Current.LogStatus("To : " + placeToModify1.Name + "C1" + " " + placeToModify1.State + "S1");
            TestLog.Current.LogStatus("Modify Item 5 in Collection From :" + placeToModify2.Name + " " + placeToModify2.State);
            TestLog.Current.LogStatus("To : " + placeToModify2.Name + "C2" + " " + placeToModify2.State + "S2");

            placeToModify1.Name += "C1";
            placeToModify1.State += "S1";

            placeToModify2.Name += "C2";
            placeToModify2.State += "S2";
        }

    }
}
