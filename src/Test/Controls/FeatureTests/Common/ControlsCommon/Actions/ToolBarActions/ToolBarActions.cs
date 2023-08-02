using System;
using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.Actions
{
	/// <summary>
	/// Dump the entire Applications Visual Tree.
	/// </summary>
	public class ToolBar_SetOverflowMode : IAction
	{
		/// <summary>
		/// Calls the static method with the given param.
		/// The XTC will look something like this:
		/// <Action Name="ToolBar_SetOverflowMode">
		/// 	<Parameter Value="Always" />
		/// </Action>
		public void Do(System.Windows.FrameworkElement frmElement, params object[] actionParams)
		{
			string strMode = actionParams[0] as String;

			if (actionParams.Length > 1)
			{
				string elementId = actionParams[1] as String;
				frmElement = System.Windows.LogicalTreeHelper.FindLogicalNode((System.Windows.DependencyObject)frmElement, elementId) as System.Windows.FrameworkElement;
			}

			System.Windows.Controls.OverflowMode mode = System.Windows.Controls.OverflowMode.AsNeeded;

			if (strMode == "Always")
			{
				mode = System.Windows.Controls.OverflowMode.Always;
			}
			else if (strMode == "Never")
			{
				mode = System.Windows.Controls.OverflowMode.Never;
			}

			System.Windows.Controls.ToolBar.SetOverflowMode(frmElement, mode);			
		}        	
	}

	public class ToolBarTray_SetIsLocked : IAction
	{
		/// <summary>
		/// Calls the static method with the given param.
		/// The XTC will look something like this:
		/// <Action Name="ToolBarTray_SetIsLocked">
		/// 	<Parameter Value="True" />
		/// </Action>
		public void Do(System.Windows.FrameworkElement frmElement, params object[] actionParams)
		{
			string strBool = actionParams[0] as String;

			if (actionParams.Length > 1)
			{
				string elementId = actionParams[1] as String;
				frmElement = System.Windows.LogicalTreeHelper.FindLogicalNode((System.Windows.DependencyObject)frmElement, elementId) as System.Windows.FrameworkElement;
			}

			bool isLocked = Convert.ToBoolean(strBool, System.Globalization.CultureInfo.InvariantCulture);;

			System.Windows.Controls.ToolBarTray.SetIsLocked(frmElement, isLocked);			
		}        	
	}

	public class ToolBarTray_InsertChild : IAction
	{
		/// <summary>
		/// Calls the static method with the given param.
		/// The XTC will look something like this:
		/// <Action Name="ToolBarTray_InsertChild">
		/// 	<Parameter Value="True" />
		/// </Action>
		public void Do(System.Windows.FrameworkElement frmElement, params object[] actionParams)
		{
			if (actionParams.Length > 0)
			{
				string elementId = actionParams[0] as String;
				frmElement = System.Windows.LogicalTreeHelper.FindLogicalNode((System.Windows.DependencyObject)frmElement, elementId) as System.Windows.FrameworkElement;
			}
	
			System.Windows.Controls.ToolBar tb = new System.Windows.Controls.ToolBar();
			System.Windows.Controls.Button btn = new System.Windows.Controls.Button();
			btn.Content = "New Button";
			tb.Items.Add(btn);
			((System.Windows.Controls.ToolBarTray)frmElement).ToolBars.Insert(1,tb);			
		}        	
	}


}
