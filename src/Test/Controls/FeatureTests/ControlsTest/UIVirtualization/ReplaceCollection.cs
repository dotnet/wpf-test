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

    public class ReplaceCollection : IAction
    {
        /// <summary>
        /// Change the Orientation of the ListBox's VirtualizingStackPanel
        /// <param name="frmElement">Control to act upon.</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {

            //Search the Resources section of FrameworkElement's parent to find the CollectionViewSource

            CollectionViewSource csv2 = (frmElement.Parent as FrameworkElement).FindResource("cvs2") as CollectionViewSource;

            if (csv2 == null)
            {
                TestLog.Current.LogStatus("Unable to find Resource of type CollectionViewSource with key cvs2 on " + frmElement.ToString() + " parent");
                throw new NullReferenceException("Unable to find Resource of type CollectionViewSource with key cvs2 on " + frmElement.ToString() + " parent");
            }

            Places places = new Places();
            Place place = new Place("AAAAAA", "OR");
            places.Add(place);

            TestLog.Current.LogStatus("Replacing Source of CollectionViewSource key cvs2");
            csv2.Source = places;
        }

    }
}
