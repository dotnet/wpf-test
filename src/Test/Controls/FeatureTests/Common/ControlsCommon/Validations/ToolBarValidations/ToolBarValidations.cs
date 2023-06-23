using System;
using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.Validations
{			
	/// <summary>
	/// Validate the value of a control's property by Id.
	/// </summary>
	public class ToolBarTray_GetIsLocked : IValidation
	{
		/// <summary>
		/// Perform the validation.
		/// The XTC would look something like the following:
		/// <Validation Name="ToolBarTray_GetIsLocked">
		/// 	<Parameter Value="true" />
		/// 	<Parameter Value="tbt1" />
		/// </Validation>
		/// </summary>
		/// <param name="validateParams">First is the control, second is an array within which is the name of the property and the expected value.</param>
		/// <returns>True if validation passed, false otherwise.</returns>
		/// assumption is that the second element of validateParams is array with first element name and second value.
		public bool Validate(params object[] validateParams)
		{
			object control = validateParams[0];

			string isLocked = (string)(validateParams[1] as Array).GetValue(0);
			string elementId = "";

			if ((validateParams[1] as Array).Length > 1)
			{
				 elementId = (string)(validateParams[1] as Array).GetValue(1);
			}

			if (elementId != "")
			{
				control = System.Windows.LogicalTreeHelper.FindLogicalNode((System.Windows.DependencyObject)control, elementId) as System.Windows.FrameworkElement;
			}

			bool actualValue = System.Windows.Controls.ToolBarTray.GetIsLocked((System.Windows.DependencyObject)control);
			bool expectedValue = Convert.ToBoolean(isLocked, System.Globalization.CultureInfo.InvariantCulture);

			TestLog.Current.LogStatus("ToolBarTray.GetIsLocked(ToolBarTray)");
			TestLog.Current.LogStatus("Actual Value: [" + actualValue.ToString() + "]");
			TestLog.Current.LogStatus("Expected Value: [" + expectedValue.ToString() + "]");

			return actualValue == expectedValue;
		}
	}

	/// <summary>
	/// Validate the value of a control's property by Id.
	/// </summary>
	public class ToolBarTray_InvalidOperationException : IValidation
	{
		/// <summary>
		/// Perform the validation.
		/// The XTC would look something like the following:
		/// <Validation Name="ToolBarTray_InvalidOperationException">
		/// 	<Parameter Value="Add" />
		/// 	<Parameter Value="tbt1" />
		/// </Validation>
		/// </summary>
		/// <param name="validateParams">First is the control, second is an array within which is the name of the property and the expected value.</param>
		/// <returns>True if validation passed, false otherwise.</returns>
		public bool Validate(params object[] validateParams)
		{
			object control = validateParams[0];

			string addOrInsert = (string)(validateParams[1] as Array).GetValue(0);
			string elementId = "";

			if ((validateParams[1] as Array).Length > 1)
			{
				 elementId = (string)(validateParams[1] as Array).GetValue(1);
			}

			if (elementId != "")
			{
				control = System.Windows.LogicalTreeHelper.FindLogicalNode((System.Windows.DependencyObject)control, elementId) as System.Windows.FrameworkElement;
			}

			string actualValue = "";
			string expectedValue = "";


			try
            		{
				if (addOrInsert == "Insert")
				{
					TestLog.Current.LogStatus("ToolBarTray.ToolBars.Insert(0, new System.Windows.Controls.Button())");
					((System.Windows.Controls.ToolBarTray)control).ToolBars.Insert(0,new System.Windows.Controls.ToolBar());
				}
				else if (addOrInsert == "Add")
				{
					TestLog.Current.LogStatus("ToolBarTray.ToolBars.Add(new System.Windows.Controls.Button())");
					((System.Windows.Controls.ToolBarTray)control).ToolBars.Add(new System.Windows.Controls.ToolBar());
				}
				else
				{
					TestLog.Current.LogStatus("Unknown Param");
				}
			}
            		catch (Exception exception)
           		{
                		actualValue = exception.GetType().Name;
            		}

			TestLog.Current.LogStatus("Actual Value: [" + actualValue + "]");
			TestLog.Current.LogStatus("Expected Value: [" + expectedValue + "]");


			return actualValue == expectedValue;
		}
	}
}
