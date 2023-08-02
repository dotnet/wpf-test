using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Markup;
using System.Threading; using System.Windows.Threading;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;

namespace Avalon.Test.ComponentModel.Actions
{

    public class DragScrollBarThumbAction : IAction
    {
        /// <summary>
        /// Find the ScrollBar thumb and drag to specified location
        /// <param name="frmElement">Control to act upon.</param>
        /// <param name="actionParams">0=DeltaX,1=DeltaY</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            QueueHelper.WaitTillQueueItemsProcessed();
            int deltaX = Convert.ToInt32(actionParams[0] as String, System.Globalization.CultureInfo.InvariantCulture);
            int deltaY = Convert.ToInt32(actionParams[1] as String, System.Globalization.CultureInfo.InvariantCulture);

            FindScrollBarThumb(frmElement);

            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.MouseLeftDown(thumb);
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.MouseMove(thumb, deltaX, deltaY);
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.MouseLeftUp(thumb);
            QueueHelper.WaitTillQueueItemsProcessed();
        }


        /// <summary>
        /// Return the first occurrence of the specified type in the 
        /// Visual or Logical tree of the specfied FrameworkELement
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static void FindScrollBarThumb(DependencyObject vis)
        {
            if (vis != null)
            {

                int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(vis);

                for(int i = 0; i < count; i++)
                {
                    DependencyObject curVis = System.Windows.Media.VisualTreeHelper.GetChild(vis, i);
                    if (curVis is Thumb)
                    {
                        thumb = curVis as Thumb;
                        break;
                    }

                    if (curVis is FrameworkElement)
                    {

                        foreach (object child in LogicalTreeHelper.GetChildren(curVis as FrameworkElement))
                        {

                            if (child is Thumb)
                            {
                                thumb = child as Thumb;
                                break;
                            }
                        }
                    }

                    FindScrollBarThumb(curVis);
                }
            }
        }

        private static Thumb thumb;
    }
}
