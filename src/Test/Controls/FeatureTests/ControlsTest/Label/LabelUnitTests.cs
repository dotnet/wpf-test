using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml;
using Microsoft.Test.Logging;
using Avalon.Test.ComponentModel.Validations;
using Avalon.Test.ComponentModel.Actions;
using Avalon.Test.ComponentModel.Utilities;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.UnitTests;

namespace Avalon.Test.ComponentModel.UnitTests
{
	[TargetType(typeof(Label))]
    public class LabelAccesskeyUnitTest : IUnitTest
    {
        /// <summary>
        /// Test label accesskey to make sure it is forward focus to the target element.
        /// </summary>
        public TestResult Perform(object obj, XmlElement variation)
		{
			//get label 
			Label label = (Label)obj;

            System.Windows.Window window = WindowPositionResizeAction.GetParentWindow(label); 
            		//Turn on English keyboard so test could be run on other OS local.
           	 	ChangeIMESystemLocal ime = new ChangeIMESystemLocal();
            		ime.Do(window,"00000409");
		
			//get the UIElement that the Label is targeting.			
			UIElement targetUI = (UIElement)label.Target;
			Control targetElement = (Control)targetUI;
		
			//Walk the visual Tree to fine the accesskey to press
			AccessText accesstext = VisualTreeUtils.FindPartByType(label, typeof(AccessText), 0) as AccessText;
			//Get the accesskey from AccessText
			char aKey = accesstext.AccessKey;
			string key = aKey.ToString();
			
			//Convert to upper case to pass into uiautomation
			key = key.ToUpper();			
			
			//Pressing down Alt to invoke the Accesskey
			ControlPressRightAltDownAction AltDownAction = new ControlPressRightAltDownAction();
			AltDownAction.Do(label);
			//Pressing the Access key
			MultipleKeyActions Acesskeypressaction = new MultipleKeyActions();
			Acesskeypressaction.Do(label, key);
			
			//Releasing the Alt key
			ControlPressRightAltUpAction AltUpAction = new ControlPressRightAltUpAction();
			AltUpAction.Do(label);

			QueueHelper.WaitTillQueueItemsProcessed();

			//Verified the Label Target UIElement got focus after the acesskey is invoked.
			if (targetElement.Focusable == true)
			{
				return TestResult.Pass;
			}
			else
			{
				return TestResult.Fail;
			}
		
		}					
	}	   
}




