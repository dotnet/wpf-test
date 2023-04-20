// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Avalon.Test.ComponentModel
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Collections;
  using Microsoft.Test;
  using Microsoft.Test.Input;

  public partial class CSBVT
  {
    bool _casePassed;

    public CSBVT()
    {
        _casePassed = false;
    }

    void Done(bool testResult)
    {
        if (testResult == true)
        {
            Application.Current.Shutdown(0);
        }
        else
        {
            Application.Current.Shutdown(1);
        }
    }

    void OnLoaded (object sender, RoutedEventArgs e)
    {
        if (!CheckNonLocalizedText())
        {
            Done(false);
        }
        else if (!CheckWireupContents())
        {
            Done(false);
        }
        else if (!CheckWireupEventHandlers())
        {
            Done(false);
        }
        else if (!CheckCodeBesideBehind())
        {
            Done(false);
        }
        // Add checking id, property, event, etc. on locally and externally defined controls
        else
        {
            Done(true);
        }
    }

    void HandleClickCodeBehind(object sender, RoutedEventArgs e)
    {
	Console.WriteLine("We verified click handler defined in Code-Behind works.");
	_casePassed = true;
    }

    void MethodInCodeBehind()
    {
	Console.WriteLine("We verified a method defined in Code-Behind can be called from Code-Beside.");	
	_casePassed = true;
    }

    void HandleButton3WireupClick(object sender, RoutedEventArgs e)
    {
	Console.WriteLine("We verified click handler on Button 3 works.");

	if (sender == Wireup.Children[2])
	{
	    Console.WriteLine("Click event was sent by the correct button.");
		_casePassed = true;
	}
	else
	{
	    Console.WriteLine("Click event was sent by some source other than the clicked button.");
	    _casePassed = false;
	}

    }

    void HandleButton4WireupClick(object sender, RoutedEventArgs e)
    {
	Console.WriteLine("We verified click handler on Button 4 works.");
	if (sender == Button4Wireup)
	{
	    Console.WriteLine("Click event was sent by the correct button.");
		_casePassed = true;
	}
	else
	{
	    Console.WriteLine("Click event was sent by some source other than the clicked button.");
		_casePassed = false;
	}

    }

    // This checks for the contents of each button, so any changes made in either markup or code need to be synced.
    // Code behind also references content and elemnt type explicitly.
    bool CheckWireupContents()
    {
	// Index will be used to track which button we are looking at.
	int i = 0;
	// Content of each button, corresponding to the XAML declarations above. Need to keep them synchronized.
	String[] contents = {"No Name, No Event Handler", "Name, No Event Handler", "No Name, Event Handler", "Name, Event Handler"};

	// Walk through each UIElement child and verify it is both a button and that it's content matches the expectation.
	foreach (UIElement myElement in Wireup.Children)
	{
	    if (myElement.GetType().FullName == "System.Windows.Controls.Button")
	    {
	    	Button myButton = myElement as Button;
	        if (myButton.Content as String == contents[i])
	        {
		    Console.WriteLine("Button " + (i + 1) + "'s content of '" + (myButton.Content as String) + "' matched expectation.");
	        }
		else
		{
		    Console.WriteLine("Button " + (i + 1) + "'s content of '" + (myButton.Content as String) + "' did not match expectation of '" + contents[i] + "'.");
			return false;
		}
	    }
	    else
	    {
		Console.WriteLine("Element " + (i + 1) + " was of type '" + myElement.GetType().FullName + "' when it should have been of type System.Windows.Controls.Button.");
		return false;
	}
	    i++;
	}

	// Refer to content explicity for those with Name
	if (Button2Wireup.Content as String == "Name, No Event Handler")
	{
		Console.WriteLine("Content of Button 2 as expected, able to reference by Name.");
	}
	else
	{
		Console.WriteLine("Content of Button 2 was '" + (Button2Wireup.Content as String) + "' did not match expectation of 'Name, No Event Handler'.");
		return false;
	}

	// Refer to content explicity for those with Name
	if (Button4Wireup.Content as String == "Name, Event Handler")
	{
		Console.WriteLine("Content of Button 4 as expected, able to reference by Name.");
	}
	else
	{
		Console.WriteLine("Content of Button 4 was '" + (Button4Wireup.Content as String) + "' did not match expectation of 'Name, Event Handler'.");
		return false;
	}

	return true;
}

	  bool CheckWireupEventHandlers()
	  {
		  QueueHelper.WaitTillQueueItemsProcessed();
		  UserInput.MouseLeftClickCenter(Wireup.Children[2] as System.Windows.Controls.Button);
		  QueueHelper.WaitTillQueueItemsProcessed();
		  if (!_casePassed)
		  {
			  Console.WriteLine("Clicking on third button did not trigger event.");
			  return false;
		  }
		  _casePassed = false;
		  QueueHelper.WaitTillQueueItemsProcessed();
		  UserInput.MouseLeftClickCenter(Wireup.Children[3] as System.Windows.Controls.Button);
		  QueueHelper.WaitTillQueueItemsProcessed();
		  if (!_casePassed)
		  {
			  Console.WriteLine("Clicking on fourth button did not trigger event.");
			  return false;
		  }
		  _casePassed = false;

		  // also do for element with explicit id
		  QueueHelper.WaitTillQueueItemsProcessed();
		  UserInput.MouseLeftClickCenter(Button4Wireup);
		  QueueHelper.WaitTillQueueItemsProcessed();
		  if (!_casePassed)
		  {
			  Console.WriteLine("Clicking on fourth button did not trigger event.");
			  return false;
		  }
		  _casePassed = false;

		  return true;
	  }

	  bool CheckNonLocalizedText()
	  {
		  Console.WriteLine("Able to compile xaml and code with non-localized characters");

		  if (GlobalText.Content as string == "Text using masculin, 按钮, ボタン, 단추, кнопка, κουμπί")
		  {
			  Console.WriteLine("Able to verify foreign character contents of a control");
		  }
		  else
		  {
			  Console.WriteLine("Unable to verify foreign character contents of a control");
			  return false;
		  }

		  if (masculin.Content as string == "Name using Language1")
		  {
			  Console.WriteLine("Able to verify foreign character Name of a control");
		  }
		  else
		  {
			  Console.WriteLine("Unable to verify foreign character Name of a control");
			  return false;
		  }

		  if (按钮.Content as string == "Name using Language2")
		  {
			  Console.WriteLine("Able to verify foreign character Name of a control");
		  }
		  else
		  {
			  Console.WriteLine("Unable to verify foreign character Name of a control");
			  return false;
		  }

		  if (ボタン.Content as string == "Name using Language3")
		  {
			  Console.WriteLine("Able to verify foreign character Name of a control");
		  }
		  else
		  {
			  Console.WriteLine("Unable to verify foreign character Name of a control");
			  return false;
		  }

		  if (단추.Content as string == "Name using Language4")
		  {
			  Console.WriteLine("Able to verify foreign character Name of a control");
		  }
		  else
		  {
			  Console.WriteLine("Unable to verify foreign character Name of a control");
			  return false;
		  }

		  if (кнопка.Content as string == "Name using Language5")
		  {
			  Console.WriteLine("Able to verify foreign character Name of a control");
		  }
		  else
		  {
			  Console.WriteLine("Unable to verify foreign character Name of a control");
			  return false;
		  }

		  if (κουμπί.Content as string == "Name using Language6")
		  {
			  Console.WriteLine("Able to verify foreign character Name of a control");
		  }
		  else
		  {
			  Console.WriteLine("Unable to verify foreign character Name of a control");
			  return false;
		  }

		  Console.WriteLine("Verified non-localized characters accepted in both IDs and content.");
		  return true;
	  }

	  bool CheckCodeBesideBehind()
	  {
		  QueueHelper.WaitTillQueueItemsProcessed();
		  UserInput.MouseLeftClickCenter(CodeBeside);
		  QueueHelper.WaitTillQueueItemsProcessed();
		  if (!_casePassed)
		  {
			  Console.WriteLine("Clicking on button did not trigger event defined in Code Beside.");
			  return false;
		  }
		  _casePassed = false;
		  QueueHelper.WaitTillQueueItemsProcessed();
		  UserInput.MouseLeftClickCenter(CodeBehind);
		  QueueHelper.WaitTillQueueItemsProcessed();
		  if (!_casePassed)
		  {
			  Console.WriteLine("Clicking on button did not trigger event defined in Code Behind.");
			  return false;
		  }
		  _casePassed = false;

		  MethodInCodeBeside();
		  if (!_casePassed)
		  {
			Console.WriteLine("Was unable to call methods in code behind from code beside and vice versa.");
			return false;
		  }
		  _casePassed = false;

		  return true;
	  }

	  void HandleClick(object sender, RoutedEventArgs e)
	  {
	  }

  }

}
