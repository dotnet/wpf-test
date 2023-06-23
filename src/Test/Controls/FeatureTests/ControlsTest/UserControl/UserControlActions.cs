using System;
using System.Drawing;
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
using Microsoft.Test.RenderingVerification;

namespace Avalon.Test.ComponentModel.Actions
{

    /// <summary>
    /// MouseClick on a Control that's inside UserControl.FixedTemplate
    /// This action will search for the control inside UserControl visual tree 
    /// <Action Name="UCControlMouseLeftClickAction">
    /// 	<Parameter Value="CONTROL_NAME_INSIDE_USERCONTROL_FIXEDTEMPLATE" />
    /// </Action>
    /// </summary>
    /// <param name="actionParams[0]">Name of the control inside UserControl.FixedTemplate</param>
    public class UCControlMouseLeftClickAction : IAction
    {
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            //Name of the control inside UserControl.FixedTemplate
            string controlName = actionParams[0] as string;

            Control control = VisualTreeUtils.FindPartByName(frmElement, controlName) as Control;

            if (control == null)
                throw new NullReferenceException("Unable to find " + controlName + " inside visual tree of " + frmElement.ToString());

            ControlMouseLeftClickAction controlMouseLeftClickAction  = new ControlMouseLeftClickAction();
            controlMouseLeftClickAction.Do(control);
        }
    }

    /// <summary>
    /// Find an Element inside visual the tree of given FrameworkElement
    /// <Action Name="FindElementByNameInVisualAction">
    /// 	<Parameter Value="NAME" />
    /// 	<Parameter Value="KeyToStoreReferenceInHashTable" />
    /// </Action>
    /// </summary>
    /// <param name="actionParams[0]">Name of the element</param>
    /// <param name="actionParams[1]">Key to store reference in HashTable</param>
    public class FindElementByNameInVisualAction : IAction
    {
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            if (actionParams.Length < 2)
            {
                throw new ArgumentException("InSufficient number of arguments");
            }
            
            //Name of the control
            string controlName = actionParams[0] as string;

            string keyToStoreElm = actionParams[1] as string;

            FrameworkElement elem = VisualTreeUtils.FindPartByName(frmElement, controlName) as FrameworkElement;

            if (elem == null)
            {
                throw new NullReferenceException("Unable to find " + controlName + " inside visual tree of " + frmElement.ToString());
            }

            //Store in StateTable
            StateTable.Add(keyToStoreElm, elem);
        }
    }

    /// <summary>
    /// Find an Element inside Logical the tree of given FrameworkElement
    /// <Action Name="FindElementByNameInVisualAction">
    /// 	<Parameter Value="NAME" />
    /// 	<Parameter Value="KeyToStoreReferenceInHashTable" />
    /// </Action>
    /// </summary>
    /// <param name="actionParams[0]">Name of the element</param>
    /// <param name="actionParams[1]">Key to store reference in HashTable</param>
    public class FindElementByNameInLogicalTreeAction : IAction
    {
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            if (actionParams.Length < 2)
            {
                throw new ArgumentException("InSufficient number of arguments");
            }

            string elementName = actionParams[0] as string;

            string keyToStoreElm = actionParams[1] as string;

            FrameworkElement element = LogicalTreeHelper.FindLogicalNode(frmElement, elementName) as FrameworkElement;

            if (element == null)
            {
                throw new NullReferenceException("Unable to find " + elementName + " inside Logical tree of " + frmElement.ToString());
            }

            //Store in StateTable
            StateTable.Add(keyToStoreElm, element);
        }
    }

    /// <summary>
    /// Capture the control rendering and store it inside StateTable
    /// Second action param is optional, if not given this will capture rendering
    /// of the framework element passed to action
    /// <Action Name="FindElementByNameInVisualAction">
    /// 	<Parameter Value="KeyToStoreBitmapInHashTable" />
    /// 	<Parameter Value="Reference to Element store in HashTable" />
    /// </Action>
    /// </summary>
    /// <param name="actionParams[0]">Name of the element</param>
    /// <param name="actionParams[1]">Reference to Element stored in HashTable</param>
    public class CaptureImageAction : IAction
    {
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string keyToStoreBitmap = null;

            if (actionParams.Length < 1)
            {
                throw new ArgumentException("InSufficient number of arguments");
            }

            keyToStoreBitmap = actionParams[0] as string;

            if (actionParams.Length >1)
            {
                string captureElementKey = actionParams[1] as string;
                frmElement = StateTable.Get(captureElementKey) as FrameworkElement;

                if (frmElement == null)
                {
                    throw new NullReferenceException("Unable to retrieve FrameworkElement with key " + captureElementKey + " from StateTable");
                }
            }

            System.Drawing.Rectangle rect = ImageUtility.GetScreenBoundingRectangle(frmElement);
            Bitmap bmp = ImageUtility.CaptureScreen(rect);

            StateTable.Add(keyToStoreBitmap, bmp);
        }
    }

    /// <summary>
    /// Capture the control rendering and store it inside StateTable
    /// Second action param is optional, if not given this will capture rendering
    /// of the framework element passed to action
    /// <Action Name="FindElementByNameInVisualAction">
    /// 	<Parameter Value="KeyToStoreBitmapInHashTable" />
    /// 	<Parameter Value="Reference to Element store in HashTable" />
    /// </Action>
    /// </summary>
    /// <param name="actionParams[0]">Name of the element</param>
    /// <param name="actionParams[1]">Reference to Element stored in HashTable</param>
    public class CreateUserControl : IAction
    {
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            TestUserControl testUserControl = new TestUserControl();
            Panel panel = frmElement as Panel;

            if (panel == null)
                throw new ArgumentException("Passed in FrameworkElement is not a Panel");

            panel.Children.Add(testUserControl);
        }
    }
}
