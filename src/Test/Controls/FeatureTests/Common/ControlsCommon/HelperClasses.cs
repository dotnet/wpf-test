using System;
using System.IO;
using System.Xml;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Collections;
using System.Drawing;
using Microsoft.Test.RenderingVerification;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Media.Animation;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Helper functions to block till all queue items are processed.
    /// </summary>
    public static class QueueHelper
    {
        /// <summary>
        /// Helper function to block till for a certain time.
        /// </summary>
        /// <param name="timeout">The time span to block for.</param>
        public static void WaitTillTimeout(TimeSpan timeout)
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            // Set a timer to terminate our loop in the frame after the
            // timeout has expired.
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
            timer.Interval = timeout;
            timer.Tick += delegate(object sender, EventArgs e)
            {
                ((DispatcherTimer)sender).IsEnabled = false;
                frame.Continue = false;
            };
            timer.Start();

            // Keep the thread busy processing events until the timeout has expired.
            Dispatcher.PushFrame(frame);
        }

        /// <summary>
        /// Helper function to block till all queue items are processed, also introduce time
        /// lag between subsequent test runs
        /// </summary>
        public static void WaitTillQueueItemsProcessed()
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(
                delegate(object arg)
                {
                    frame.Continue = false;
                    return null;
                }), null);

            // Keep the thread busy processing events until the timeout has expired.
            Dispatcher.PushFrame(frame);
        }

        public static DispatcherFrame BlockDispatcherFrameForAnimation(Timeline animation)
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            animation.Completed += delegate(object sender, EventArgs e)
            {
                frame.Continue = false;
            };

            return frame;
        }

        public static void WaitForAnimationCompleted(DispatcherFrame frame)
        {
            // Keep the thread busy processing events until the animation completed.
            Dispatcher.PushFrame(frame);
        }

        public static void WaitForToolTipOpened(ToolTip toolTip)
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            toolTip.Opened += delegate(object sender, RoutedEventArgs e)
            {
                frame.Continue = false;
            };

            Dispatcher.PushFrame(frame);
        }

        /// <summary>
        /// Change the priority from Inactive to ApplicationIdle
        /// </summary>
        private static void SchedulePriorityChange(DispatcherOperation op, TimeSpan timeSpan)
        {
            DispatcherTimer dTimer = new DispatcherTimer(DispatcherPriority.Background);
            dTimer.Tick += new System.EventHandler(ChangePriority);
            dTimer.Tag = op;
            dTimer.Interval = timeSpan;
            dTimer.Start();
        }

        /// <summary>
        /// Change priority of the DispatcherOperation
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private static void ChangePriority(object obj, EventArgs args)
        {
            DispatcherTimer dTimer = obj as DispatcherTimer;
            dTimer.Stop();
            DispatcherOperation op = dTimer.Tag as DispatcherOperation;
            op.Priority = DispatcherPriority.ApplicationIdle;
        }
    }

    ///// <summary>
    ///// Used for testing parts of Controls so you don't have to dig into the Visual Tree of a Control
    ///// to get at it's parts.
    ///// </summary>
    public static class ControlPartHelper
    {
        /// <summary>
        /// Used to get a part of a control by using the Name of a part.
        /// So to find the "ToolBarThumb" part of a ToolBar.
        /// FindPartByID(myToolBar,"ToolBarThumb");
        /// </summary>
        /// <param name="control">The Control who's part you want to get.</param>
        /// <param name="partId">The Name of part you want to find.</param>
        /// <returns>The part of the control as a FrameworkElement</returns>
        public static FrameworkElement FindPartById(Control curControl, string partId)
        {
            if (curControl != null)
            {
                ControlTemplate curCT = curControl.Template;
                FrameworkElement frmElement = curCT.FindName(partId, curControl) as FrameworkElement;

                return frmElement;
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Used for testing Controls that require you to dig into the Visual Tree of a Control
    /// to get at it's parts.
    /// </summary>
    public static class VisualTreeUtils
    {
        /// <summary>
        /// Find root visual element in the visual tree.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>popupRoot.</returns>
        public static UIElement FindRootVisual(UIElement element)
        {
            UIElement popupRoot = null;
            while ((element = (UIElement)VisualTreeHelper.GetParent(element)) != null)
            {
                popupRoot = element;
            }
            return popupRoot;
        }

        /// <summary>
        /// Get previous arrange rect for UIElement.
        /// </summary>
        /// <param name="popupRoot"></param>
        /// <returns>Final popupRoot rect.</returns>
        public static Rect GetPreviousArrangeRect(UIElement element)
        {
            PropertyInfo propertyInfo = element.GetType().GetProperty("PreviousArrangeRect", BindingFlags.Instance | BindingFlags.NonPublic);
            return (Rect)propertyInfo.GetValue(element, new object[0]);
        }

        /// <summary>
        /// Used to get an item of a specific type in the visual tree.
        /// So to find the 2nd item of type Button it would be something like.
        /// FindPartByType(visual,typeof(Button),1);
        /// </summary>
        /// <param name="vis">The visual who's tree you want to search.</param>
        /// <param name="visType">The type of object you want to find.</param>
        /// <param name="index">The count of the item as it is found in the tree.</param>
        /// <returns>The object of type visType found in vis that is the index item</returns>
        public static object FindPartByType(System.Windows.Media.Visual vis, System.Type visType, int index)
        {
            if (vis != null)
            {
                System.Collections.ArrayList parts = FindPartByType(vis, visType);

                if (index >= 0 && index < parts.Count)
                {
                    return parts[index];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns an ArrayList of all of the items of the specified type in the visual tree.
        /// </summary>
        /// <param name="vis">The visual who's tree you want to search.</param>
        /// <param name="visType">The type of object you want to find.</param>
        /// <returns>An ArrayList containing all of the objects of type visType found in the tree of vis.</returns>
        public static System.Collections.ArrayList FindPartByType(System.Windows.Media.Visual vis, System.Type visType)
        {
            System.Collections.ArrayList parts = new System.Collections.ArrayList();

            if (vis != null)
            {
                parts = FindPartByTypeRecurs(vis, visType, parts);
            }

            return parts;
        }

        private static System.Collections.ArrayList FindPartByTypeRecurs(DependencyObject vis, System.Type visType, System.Collections.ArrayList parts)
        {
            if (vis != null)
            {
                if (vis.GetType() == visType)
                {
                    parts.Add(vis);
                }

                int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(vis);

                for (int i = 0; i < count; i++)
                {
                    DependencyObject curVis = VisualTreeHelper.GetChild(vis, i);
                    parts = FindPartByTypeRecurs(curVis, visType, parts);
                }
            }

            return parts;
        }

        /// <summary>
        /// Find scrollviewer on a given treeview.
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <returns>Return a scrollviewer when it finds one.</returns>
        public static ScrollViewer FindScrollViewer(FrameworkElement frameworkElement)
        {
            ScrollViewer[] scrollViewers = (ScrollViewer[])VisualTreeUtils.FindPartByType(frameworkElement, typeof(ScrollViewer)).ToArray(typeof(ScrollViewer));

            for (int i = 0; i < scrollViewers.Length; i++)
            {
                if (scrollViewers[i].TemplatedParent == frameworkElement)
                {
                    ScrollViewer scrollViewer = scrollViewers[i] as ScrollViewer;

                    if (scrollViewer != null)
                    {
                        return scrollViewer;
                    }
                }
            }

            throw new ArgumentException("Could not find ScrollViewer inside of TreeView.");
        }


        /// <summary>
        /// tranverse the visual tree to get a visua by Name.
        /// </summary>
        /// <param name="ele"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DependencyObject FindPartByName(DependencyObject ele, string name)
        {
            DependencyObject result;
            if (ele == null)
            {
                return null;
            }
            if (name.Equals(ele.GetValue(FrameworkElement.NameProperty)))
            {
                return ele;
            }
            int count = VisualTreeHelper.GetChildrenCount(ele);
            for (int i = 0; i < count; i++)
            {
                DependencyObject vis = VisualTreeHelper.GetChild(ele, i);
                if ((result = FindPartByName(vis, name)) != null)
                {
                    return result;
                }
            }
            return null;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }

            return child;
        }
    }
    /// <summary>
    /// Sets the input locale identifier (formerly called the keyboard
    /// layout handle) for the calling thread.
    ///
    /// <example>
    /// <Action Name="ChangeIMESystemLocal">
    ///	<Parameter Value="00000409"/>	
    /// </Action>
    ///<code>
    /// Set input locale to: Arabic (Saudi Arabia) - Arabic (101) <Parameter Value = "00000401" />
    /// Set input locale to: English (United States) - US <Parameter Valu = "00000409" />
    /// Set input locale to: Hebrew - Hebrew <Parameter Value = "0000040d" />
    /// Set input locale to: Japanese - Japanese Input System (MS-IME2002) <Parameter Value = "e0010411" />
    /// Set input locale to: Spanish (Argentina) - Latin American <Parameter Value = "00002c0a" />
    /// </code></example> 
    /// </summary>       
    public static class SetKeyboardLayout
    {

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr ActivateKeyboardLayout(IntPtr hkl, int uFlags);
        /// <summary>
        /// Sets the input locale identifier (formerly called the keyboard
        /// layout handle) for the calling thread or the current process.
        /// </summary>
        /// <param name="hkl">Input locale identifier to be activated.</param>
        /// <param name="flags">Specifies how the input locale identifier is to be activated.</param>
        /// <returns>
        /// If the function succeeds, the return value is the previous input
        /// locale identifier. Otherwise, it is IntPtr.Zero.
        /// </returns>
        public static IntPtr SafeActivateKeyboardLayout(IntPtr hkl, int flags)
        {
            new System.Security.Permissions.SecurityPermission(
                 System.Security.Permissions.PermissionState.Unrestricted)
                 .Assert();
            return ActivateKeyboardLayout(hkl, flags);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr LoadKeyboardLayout(string pwszKLID, int flags);
        /// <summary>
        /// Loads a new input locale identifier (formerly called the
        /// keyboard layout) into the system.
        /// </summary>
        /// <param name="pwszKLID">A string composed of the hexadecimal value of the Language Identifier (low word) and a device identifier (high word).</param>
        /// <param name="flags">Specifies how the input locale identifier is to be loaded.</param>
        /// <returns>
        /// The input locale identifier to the locale matched with the
        /// requested name. If no matching locale is available, the return
        /// value is IntPtr.Zero.
        /// </returns>
        public static IntPtr SafeLoadKeyboardLayout(string pwszKLID, int flags)
        {

            new System.Security.Permissions.SecurityPermission(

                System.Security.Permissions.PermissionState.Unrestricted)

                .Assert();

            return LoadKeyboardLayout(pwszKLID, flags);

        }
    }
}


