using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Markup;
using System.Threading; 
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;

namespace Avalon.Test.ComponentModel
{
	/// <summary>
	/// Using UserInput to do Mouse input actions
	/// </summary>
    public class MouseInputAction : IAction
    {
        public MouseInputAction()
        {
        }

        public string ActionName
        {
            get
            {
                return "Keypress and Click on a FrameworkElement";
            }
        }

        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            if (actionParams != null && actionParams.Length >= 1 && frmElement != null)
            {
                if (actionParams[0] != null)
                {
                    switch (actionParams[0] as string)
                    {
                        case "click":
                            //Mouse left click on the on a framework element.
                            UserInput.MouseLeftClickCenter(frmElement);
                            break;

                        case "mouseleftdown":
                            //Mouse left click on the on a framework element.
                            UserInput.MouseLeftDown(frmElement);
                            break;

                        case "mouseleftup":
                            //Mouse left click on the on a framework element.
                            UserInput.MouseLeftUp(frmElement);
                            break;
                        case "mousemove":
                            //Mouse move to on a framework element.
                            int x = Convert.ToInt32(actionParams[1]);
                            int y = Convert.ToInt32(actionParams[2]);

                            UserInput.MouseMove(frmElement, x, y);
                            break;

                        default:
                            {
                                TestLog.Current.LogEvidence("Unknown test param.");
                                break;
                            }
                    }
                }
            }
        }
    }
}

namespace Avalon.Test.ComponentModel.Actions
{
    public class MouseClickCenterAction : IAction
    {
        /// <summary>
        /// Perform a mouse click action on the center of a control.
        /// The xtc file would look like the following:
        /// <Action Name="MouseClickCenterAction">
        /// 	<Parameter Value="tb1" />
        /// 	<Parameter Value="ToolBarThumb" />
        /// </Action>
        /// </summary>
        /// <param name="frmElement">Control to act upon.</param>
        /// <param name="actionParams">0=Name,1=PartID</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string elementId = actionParams[0] as String;
            string partId = actionParams[1] as String;
            if (elementId != "")
            {
                frmElement = LogicalTreeHelper.FindLogicalNode((System.Windows.DependencyObject)frmElement, elementId) as FrameworkElement;
            }
            if (partId != "")
            {
                frmElement = ControlPartHelper.FindPartById((Control)frmElement, partId);
            }
            UserInput.MouseLeftClickCenter(frmElement);
        }
    }
    public class MouseDoubleClickCenterAction : IAction
    {
        /// <summary>
        /// Perform a mouse click action on the center of a control.
        /// The xtc file would look like the following:
        /// <Action Name="MouseClickCenterAction">
        /// 	<Parameter Value="tb1" />
        /// 	<Parameter Value="ToolBarThumb" />
        /// </Action>
        /// </summary>
        /// <param name="frmElement">Control to act upon.</param>
        /// <param name="actionParams">0=Name,1=PartID</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string elementId = actionParams[0] as String;
            string partId = actionParams[1] as String;
            if (elementId != "")
            {
                frmElement = LogicalTreeHelper.FindLogicalNode((System.Windows.DependencyObject)frmElement, elementId) as FrameworkElement;
            }
            if (partId != "")
            {
                frmElement = ControlPartHelper.FindPartById((Control)frmElement, partId);
            }
            UserInput.MouseLeftClickCenter(frmElement);
            UserInput.MouseLeftClickCenter(frmElement);
        }
    }
    public class MouseLeftClickAction : IAction
    {
        /// <summary>
        /// Perform a mouse click action on the control according to giving coordinate.
        /// The xtc file would look like the following:
        /// <Action Name="MouseClickCenterAction">
        /// 	<Parameter Value="tb1" />
        /// 	<Parameter Value="ToolBarThumb" />
        /// </Action>
        /// </summary>
        /// <param name="frmElement">Control to act upon.</param>
        /// <param name="actionParams">0=Name,1=PartID,2=deltaX,3=deltaY</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string elementId = actionParams[0] as String;
            string partId = actionParams[1] as String;
            int deltaX = Convert.ToInt32(actionParams[2] as String, System.Globalization.CultureInfo.InvariantCulture);
            int deltaY = Convert.ToInt32(actionParams[3] as String, System.Globalization.CultureInfo.InvariantCulture);
            if (elementId != "")
            {
                frmElement = LogicalTreeHelper.FindLogicalNode((System.Windows.DependencyObject)frmElement, elementId) as FrameworkElement;
            }
            if (partId != "")
            {
                frmElement = ControlPartHelper.FindPartById((Control)frmElement, partId);
            }
            UserInput.MouseLeftDown(frmElement, deltaX, deltaY);
            UserInput.MouseLeftUp(frmElement, deltaX, deltaY);
        }
    }
    public class MouseLeftDownByIDAction : IAction
    {
        /// <summary>
        /// Perform a mouse down action on the control on a giving coordinates.
        /// The xtc file would look like the following:
        /// <Action Name="MouseLeftDownByIDAction">
        /// 	<Parameter Value="tb1" />
        /// 	<Parameter Value="partID" />
        ///     <Parameter Value="deltaX" />
        ///     <Parameter Value="deltaY" />
        /// </Action>
        /// </summary>
        /// <param name="frmElement">Control to act upon.</param>
        /// <param name="actionParams">0=Name,1=PartID</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string elementId = actionParams[0] as String;
            string partId = actionParams[1] as String;
            int deltaX = Convert.ToInt32(actionParams[2] as String, System.Globalization.CultureInfo.InvariantCulture);
            int deltaY = Convert.ToInt32(actionParams[3] as String, System.Globalization.CultureInfo.InvariantCulture);

            TestLog.Current.LogStatus("elementID=[" + elementId + "]");
            TestLog.Current.LogStatus("partId=[" + partId + "]");
            TestLog.Current.LogStatus("deltaX=[" + deltaX.ToString() + "]");
            TestLog.Current.LogStatus("deltaY=[" + deltaY.ToString() + "]");

            if (elementId != "")
            {
                frmElement = LogicalTreeHelper.FindLogicalNode((System.Windows.DependencyObject)frmElement, elementId) as FrameworkElement;
            }
            if (partId != "")
            {
                frmElement = ControlPartHelper.FindPartById((Control)frmElement, partId);
            }
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.MouseLeftDown(frmElement, deltaX, deltaY);
            QueueHelper.WaitTillQueueItemsProcessed();
        }
    }

    public class MouseLeftUpByIDAction : IAction
    {
        /// <summary>
        /// Perform a mouse Up action on the control on a giving Coordinates.
        /// The xtc file would look like the following:
        /// <Action Name="MouseLeftDownByIDAction">
        /// 	<Parameter Value="tb1" />
        /// 	<Parameter Value="partID" />
        ///     <Parameter Value="deltaX" />
        ///     <Parameter Value="deltaY" />
        /// </Action>
        /// </summary>
        /// <param name="frmElement">Control to act upon.</param>
        /// <param name="actionParams">0=Name,1=PartID</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string elementId = actionParams[0] as String;
            string partId = actionParams[1] as String;
            int deltaX = Convert.ToInt32(actionParams[2] as String, System.Globalization.CultureInfo.InvariantCulture);
            int deltaY = Convert.ToInt32(actionParams[3] as String, System.Globalization.CultureInfo.InvariantCulture);

            TestLog.Current.LogStatus("elementID=[" + elementId + "]");
            TestLog.Current.LogStatus("partId=[" + partId + "]");
            TestLog.Current.LogStatus("deltaX=[" + deltaX.ToString() + "]");
            TestLog.Current.LogStatus("deltaY=[" + deltaY.ToString() + "]");

            if (elementId != "")
            {
                frmElement = LogicalTreeHelper.FindLogicalNode((System.Windows.DependencyObject)frmElement, elementId) as FrameworkElement;
            }
            if (partId != "")
            {
                frmElement = ControlPartHelper.FindPartById((Control)frmElement, partId);
            }
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.MouseLeftUp(frmElement, deltaX, deltaY);
            QueueHelper.WaitTillQueueItemsProcessed();
        }
    }
    public class MouseMoveByIDAction : IAction
    {
        /// <summary>
        /// Perform a mouse drag action on a control.
        /// The xtc file would look like the following:
        /// <Action Name="MouseDragAction">
        /// 	<Parameter Value="tb1" />
        /// 	<Parameter Value="ToolBarThumb" />
        ///     <Parameter Value="deltaX" />
        ///     <Parameter Value="deltaY" />
        /// </Action>
        /// </summary>
        /// <param name="frmElement">Control to act upon.</param>
        /// <param name="actionParams">0=Name,1=PartID,2=DeltaX,3=DeltaY</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string elementId = actionParams[0] as String;
            string partId = actionParams[1] as String;
            int deltaX = Convert.ToInt32(actionParams[2] as String, System.Globalization.CultureInfo.InvariantCulture);
            int deltaY = Convert.ToInt32(actionParams[3] as String, System.Globalization.CultureInfo.InvariantCulture);

            TestLog.Current.LogStatus("elementID=[" + elementId + "]");
            TestLog.Current.LogStatus("partId=[" + partId + "]");
            TestLog.Current.LogStatus("deltaX=[" + deltaX.ToString() + "]");
            TestLog.Current.LogStatus("deltaY=[" + deltaY.ToString() + "]");

            if (elementId != "")
            {
                frmElement = LogicalTreeHelper.FindLogicalNode((System.Windows.DependencyObject)frmElement, elementId) as FrameworkElement;
            }
            if (partId != "")
            {
                frmElement = ControlPartHelper.FindPartById((Control)frmElement, partId);
            }
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.MouseMove(frmElement, deltaX, deltaY);
            QueueHelper.WaitTillQueueItemsProcessed();
        }
    }
    public class MouseDragAction : IAction
    {
        /// <summary>
        /// Perform a mouse drag action on a control.
        /// The xtc file would look like the following:
        /// <Action Name="MouseDragAction">
        /// 	<Parameter Value="tb1" />
        /// 	<Parameter Value="ToolBarThumb" />
        /// 	<Parameter Value="0" />
        /// 	<Parameter Value="-60" />
        /// </Action>
        /// </summary>
        /// <param name="frmElement">Control to act upon.</param>
        /// <param name="actionParams">0=Name,1=PartID,2=DeltaX,3=DeltaY</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            string elementId = actionParams[0] as String;
            string partId = actionParams[1] as String;
            int deltaX = Convert.ToInt32(actionParams[2] as String, System.Globalization.CultureInfo.InvariantCulture);
            int deltaY = Convert.ToInt32(actionParams[3] as String, System.Globalization.CultureInfo.InvariantCulture);

            TestLog.Current.LogStatus("elementID=[" + elementId + "]");
            TestLog.Current.LogStatus("partId=[" + partId + "]");
            TestLog.Current.LogStatus("deltaX=[" + deltaX.ToString() + "]");
            TestLog.Current.LogStatus("deltaY=[" + deltaY.ToString() + "]");

            if (elementId != "")
            {
                frmElement = LogicalTreeHelper.FindLogicalNode((System.Windows.DependencyObject)frmElement, elementId) as FrameworkElement;
            }
            if (partId != "")
            {
                frmElement = ControlPartHelper.FindPartById((Control)frmElement, partId);
            }
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.MouseLeftDown(frmElement, 4, 4);
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.MouseMove(frmElement, deltaX + 4, deltaY + 4);
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.MouseLeftUp(frmElement, 4, 4);
            QueueHelper.WaitTillQueueItemsProcessed();
        }
    }
}

