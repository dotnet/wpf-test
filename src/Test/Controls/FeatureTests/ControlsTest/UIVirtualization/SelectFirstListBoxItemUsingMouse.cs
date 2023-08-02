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
using Avalon.Test.ComponentModel.Actions;

namespace Avalon.Test.ComponentModel.Actions
{

    public class SelectFirstListBoxItemUsingMouse : IAction
    {
        /// <summary>
        /// Select first ListBoxItem using Mouse
        /// I want make sure the first item selected and focus set to first Selectable item, so I cannot 
        /// set property to Select the item
        /// Also I cannot name the first element since items are generated
        /// <param name="frmElement">Control to act upon.</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {

            ListBox listBox = frmElement as ListBox;

            if (listBox == null)
            {
                throw new ArgumentException("This action is for ListBox, type of FrameworkElement passed: " + frmElement.GetType().ToString() );
            }

            ListBoxItem firstSelectableListBoxItem = null;

            for (int i = 0; i < listBox.Items.Count; i++)
            {
                if ( !(listBox.Items[i] is Separator) )
                {
                    firstSelectableListBoxItem = listBox.ItemContainerGenerator.ContainerFromItem(listBox.Items[i]) as ListBoxItem;
                    break;
                }
            }

            if (firstSelectableListBoxItem == null)
            {
                throw new ArgumentException("ListBox should contain at least selectable item");
            }

            ControlMouseLeftClickAction controlMouseLeftClickAction = new ControlMouseLeftClickAction();
            controlMouseLeftClickAction.Do(firstSelectableListBoxItem);
        }

    }
}
