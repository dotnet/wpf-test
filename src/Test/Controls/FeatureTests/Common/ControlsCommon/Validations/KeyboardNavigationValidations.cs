using System;
using System.Windows;
using System.Windows.Input;

using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.Validations
{			
	/// <summary>
	/// Validate the value of a control's property by Id.
	/// </summary>
	public class KeyboardFocusedElementValidation : IValidation
	{
		/// <summary>
		/// Perform the validation.
		/// The XTC would look something like the following:
		/// <Validation Name="KeyboardFocusedElementValidation">
		/// 	<Parameter Value="tb1" />
		/// </Validation>
		/// </summary>
		/// <param name="validateParams">First is the control, second is an array within which is the name of the property and the expected value.</param>
		/// <returns>True if validation passed, false otherwise.</returns>
		/// assumption is that the second element of validateParams is array with first element name and second value.
		public bool Validate(params object[] validateParams)
		{
			object control = validateParams[0];

			string elementId = (string)(validateParams[1] as Array).GetValue(0);
			if (elementId != "")
			{
				TestLog.Current.LogStatus("Expected FocusedElement.Name: [" + elementId + "]");
				control = LogicalTreeHelper.FindLogicalNode((System.Windows.DependencyObject)control, elementId) as FrameworkElement;
			}
			else
			{
				//ElementId must be specified do something here.
			}

			System.Windows.IInputElement actualFocusedElement = Keyboard.FocusedElement;	

			FrameworkElement fe = actualFocusedElement as FrameworkElement;
			if (fe != null)
			{
				TestLog.Current.LogStatus("Actual FocusedElement.Name: [" + fe.Name + "]");
			}

			return actualFocusedElement == control;
		}
	}
}

