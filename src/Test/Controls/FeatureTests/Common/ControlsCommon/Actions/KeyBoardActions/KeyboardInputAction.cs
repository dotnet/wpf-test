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
	/// Using UserInput to do keyboard input actions
	/// </summary>
	public class KeyboardInputAction : IAction
	{
		public string ActionName
		{
			get
			{
				return "Keyboard input on a FrameworkElement";
			}
		}

		public void Do(FrameworkElement frmElement, params object[] actionParams)
		{
			if (actionParams != null && actionParams.Length >= 1 && frmElement != null)
			{
				if (actionParams[0] != null)
				{
					frmElement.Focus();
					switch (actionParams[0] as string)
					{
						case "KeyPress":
							UserInput.KeyPress(actionParams[1] as string);
							break;
						case "KeyDown":
							UserInput.KeyDown(actionParams[1] as string);
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

	public class KeyboardInputActionNoFocus : IAction
	{
		public string ActionName
		{
			get
			{
				return "Keyboard input";
			}
		}

		public void Do(FrameworkElement frmElement, params object[] actionParams)
		{
			if (actionParams != null && actionParams.Length >= 1 && frmElement != null)
			{
				if (actionParams[0] != null)
				{
					TestLog.Current.LogStatus("PARAM[0]" + actionParams[0]);
					if (actionParams[1] != null)
					{
                        TestLog.Current.LogStatus("PARAM[1]" + actionParams[1]);
					}
					switch (actionParams[0] as string)
					{
						case "KeyPress":
							UserInput.KeyPress(actionParams[1] as string);
							break;
						case "KeyDown":
							UserInput.KeyDown(actionParams[1] as string);
							break;
						case "KeyUp":
							UserInput.KeyUp(actionParams[1] as string);
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
