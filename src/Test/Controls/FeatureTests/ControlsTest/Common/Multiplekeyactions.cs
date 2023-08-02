using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Avalon.Test.ComponentModel.Utilities;
using System.Collections;
using Microsoft.Test.Input;

namespace Avalon.Test.ComponentModel
{
	public class MultipleKeyActions : IAction
	{
		public string ActionName
		{
			get
			{
				return "MultipleKeyActions";
			}
		}

		public void Do(FrameworkElement frmElement, params object[] actionParams)
		{
			if (actionParams != null && actionParams.Length >= 1 && frmElement != null)
			{
				foreach (string action in actionParams)
				{
					UserInput.KeyPress(action);
                    QueueHelper.WaitTillQueueItemsProcessed();
				}
			}
			else
			{
				throw new Exception("must provide atleast one param.");
			}
		}
	}
}


