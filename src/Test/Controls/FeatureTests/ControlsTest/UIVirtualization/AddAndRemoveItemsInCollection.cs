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

    public class AddAndRemoveItemsInCollection : IAction
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

            Place placeToRemove1 = col[3];
            Place placeToRemove2 = col[5];

            Place placeToAdd1 = new Place("Princeton", "NJ");
            Place placeToAdd2 = new Place("Cleveland", "OH");


            TestLog.Current.LogStatus("Add :" + placeToAdd1.Name + " " + placeToAdd1.State);
            TestLog.Current.LogStatus("Add :" + placeToAdd2.Name + " " + placeToAdd2.State);

            col.Add(placeToAdd1);
            col.Add(placeToAdd2);

            TestLog.Current.LogStatus("Remove :" + placeToRemove1.Name + " " + placeToRemove1.State);
            TestLog.Current.LogStatus("Remove :" + placeToRemove2.Name + " " + placeToRemove2.State);

            col.Remove(placeToRemove1);
            col.Remove(placeToRemove2);
        }

    }
}
