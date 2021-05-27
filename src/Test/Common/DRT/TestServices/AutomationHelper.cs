// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <summary>

//  Utility class simplifying UI automation for TestServices. </summary>



#pragma warning disable 1634, 1691 // Stops compiler from warning about unknown warnings

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using System.Windows.Input;

namespace DRT
{
    /// <summary>
    /// Utility class simplifying UI automation for TestServices.
    /// </summary>
    public static class AutomationHelper
    {
        #region Public Methods
        //----------------------------------------------------------------------
        //
        // Public Methods
        //
        //----------------------------------------------------------------------

        /// <summary>
        /// Will close the specified window.
        /// </summary>
        /// <param name="window">Window to close.</param>
        public static void CloseWindow(AutomationElement window)
        {
            bool supportedWindowPattern = false;

            foreach (AutomationPattern pattern in window.GetSupportedPatterns())
            {
                if (pattern == WindowPattern.Pattern)
                {
                    WindowPattern wndPattern = (WindowPattern)window.GetCurrentPattern(WindowPattern.Pattern);
                    wndPattern.WaitForInputIdle(Timeout);
                    wndPattern.Close();
                    supportedWindowPattern = true;
                    break;
                }
            }

            if (!supportedWindowPattern)
            {
                UnsafeNativeMethods.SendMessage(
                    (IntPtr)window.Current.NativeWindowHandle,
                    Win32Messages.WM_DESTROY,
                    IntPtr.Zero,
                    IntPtr.Zero);
            }
        }

        /// <summary>
        /// Will wait for the first descendant with that id to exist.
        /// </summary>
        /// <remarks>
        /// Will TestServices.Assert on timeout.
        /// 
        /// UISpy is immensely valuable for discovering property names and ids.
        /// Locaiton: ($SDXROOT)windows\accessibletech\wap\tools\uispy
        /// </remarks>
        /// <param name="container">The AutomationElement to search within.
        /// </param>
        /// <param name="id">AutomationId of the AutomationElement to look for.
        /// </param>
        /// <returns>The AutomationElement for the descendant or null if it 
        /// times out.</returns>
        public static AutomationElement FindDescendantById(
            AutomationElement container, string id)
        {
            return FindDescendantById(container, id, true);
        }

        /// <summary>
        /// Will wait for the first descendant with that id to exist.
        /// </summary>
        /// <remarks>
        /// Will TestServices.Assert on timeout.
        /// 
        /// UISpy is immensely valuable for discovering property names and ids.
        /// Locaiton: ($SDXROOT)windows\accessibletech\wap\tools\uispy
        /// </remarks>
        /// <param name="container">The AutomationElement to search within.
        /// </param>
        /// <param name="id">AutomationId of the AutomationElement to look for.
        /// </param>
        /// <param name="assertOnFailure">Whether to TestServices.Assert if the element is not found.
        /// </param>
        /// <returns>The AutomationElement for the descendant or null if it 
        /// times out.</returns>
        public static AutomationElement FindDescendantById(
            AutomationElement container, string id, bool assertOnFailure)
        {
            AutomationElement result = null;

            DateTime end = DateTime.Now.AddMilliseconds(Timeout);
            while ((result == null) && (DateTime.Now < end))
            {
                Condition cond = new PropertyCondition(
                    AutomationElement.AutomationIdProperty, id);
                result = container.FindFirst(TreeScope.Descendants, cond);
                System.Threading.Thread.Sleep(Interval);
            }

            TestServices.Assert(!assertOnFailure || (result != null),
                "Descendant {0} was not found.", id);

            if (result == null)
            {
                TestServices.Log("Descendant {0} was not found.", id);
                return null;
            }

            TraceAutomationElement("FoundById", result);            
            return result;
        }

        /// <summary>
        /// Will wait for the first descendant with that name to exist.
        /// </summary>
        /// <remarks>
        /// Will TestServices.Assert on timeout.
        /// 
        /// UISpy is immensely valuable for discovering property names and ids.
        /// Locaiton: ($SDXROOT)windows\accessibletech\wap\tools\uispy
        /// </remarks>
        /// <param name="container">The AutomationElement to search within.
        /// </param>
        /// <param name="name">Exact name of the AutomationElement to look for.
        /// </param>
        /// <returns>The AutomationElement for the descendant or null if it 
        /// times out.</returns>
        public static AutomationElement FindDescendantByName(
            AutomationElement container, string name)
        {
            return FindDescendantByName(container, name, true /* assertOnFailure */);
        }
        /// <summary>
        /// Will wait for the first descendant with that name to exist.
        /// </summary>
        /// <remarks>
        /// Will TestServices.Assert on timeout if assertOnFailure is set.
        /// 
        /// UISpy is immensely valuable for discovering property names and ids.
        /// Locaiton: ($SDXROOT)windows\accessibletech\wap\tools\uispy
        /// </remarks>
        /// <param name="container">The AutomationElement to search within.
        /// </param>
        /// <param name="name">Exact name of the AutomationElement to look for.
        /// </param>
        /// <param name="assertOnFailure">Whether to TestServices.Assert if the element is not found.
        /// </param>
        /// <returns>The AutomationElement for the descendant or null if it 
        /// times out and assertOnFailure is unset</returns>
        public static AutomationElement FindDescendantByName(
            AutomationElement container, string name, bool assertOnFailure)
        {
            AutomationElement result = null;

            DateTime end = DateTime.Now.AddMilliseconds(Timeout);
            while ((result == null) && (DateTime.Now < end))
            {
                Condition cond = new PropertyCondition(
                    AutomationElement.NameProperty, name);
                result = container.FindFirst(TreeScope.Descendants, cond);
                System.Threading.Thread.Sleep(Interval);
            }

            TestServices.Assert(!assertOnFailure || (result != null),
                "Descendant {0} was not found.", name);

            if (result != null)
            {
                TraceAutomationElement("FoundByName", result);
            }

            return result;
        }

        /// <summary>
        /// Will wait for a window with that name to exist and return it.
        /// </summary>
        /// <remarks>
        /// Will TestServices.Assert on timeout.
        /// </remarks>
        /// <param name="windowName">Exact name of window.</param>
        /// <returns>The AutomationElement for the window or null if it times 
        /// out.</returns>
        public static AutomationElement FindWindow(string windowName)
        {
            return FindWindow(windowName, true /* assertOnFailure */ );
        }

        /// <summary>
        /// Will wait for a window with that name to exist and return it.
        /// </summary>
        /// <remarks>
        /// Will TestServices.Assert on timeout if assertOnFailure is set.
        /// </remarks>
        /// <param name="windowName">Exact name of window.</param>
        /// <param name="assertOnFailure">Whether to assert if the window is not found.</param>
        /// <returns>The AutomationElement for the window or null if it times out and
        /// assertOnFailure is set.</returns>
        public static AutomationElement FindWindow(string windowName, bool assertOnFailure)
        {
            return FindWindow(windowName, assertOnFailure, Timeout);
        }

        /// <summary>
        /// Will wait for a window with that name to exist and return it.
        /// </summary>
        /// <remarks>
        /// Will TestServices.Assert on timeout if assertOnFailure is set.
        /// </remarks>
        /// <param name="windowName">Exact name of window.</param>
        /// <param name="assertOnFailure">Whether to assert if the window is not found.</param>
        /// <param name="timeout">Maximum time to wait, in milliseconds.</param>
        /// <returns>The AutomationElement for the window or null if it times out and
        /// assertOnFailure is set.</returns>
        public static AutomationElement FindWindow(string windowName, bool assertOnFailure, int timeout)
        {
            AutomationElement result = null;

            DateTime end = DateTime.Now.AddMilliseconds(timeout);
            while ((result == null) && (DateTime.Now < end))
            {
                IntPtr hWnd = UnsafeNativeMethods.FindWindow(null, windowName);

                if (hWnd != IntPtr.Zero)
                {
                    result = AutomationElement.FromHandle(hWnd);
                }

                System.Threading.Thread.Sleep(Interval);
            }

            if (assertOnFailure)
            {
                TestServices.Assert(
                    result != null, "Window {0} was not found.", windowName);
            }

            return result;
        }

        /// <summary>
        /// Walks the Automation Tree from the specified root, searching for the element with the given property.
        /// (Stolen from HostingHelper used by DrtLaunchContainer)
        /// </summary>
        /// <param name="root">The root to start searching from</param>
        /// <param name="property">The property to search for</param>
        /// <param name="value">The value of above property</param>
        /// <param name="maxDepth">The maximum depth to search</param>
        /// <param name="allowPartialMatch">Allow partial matches (i.e. for window titles)</param>
        /// <returns></returns>
        public static AutomationElement FindElementWithPropertyValue(AutomationElement root,
                                                                      AutomationProperty property,
                                                                      object value,
                                                                      int maxDepth,
                                                                      bool allowPartialMatch)
        {
            object[] objectArray = new object[1];
            objectArray[0] = value;

            return FindElementWithPropertyValue(root, property, objectArray, maxDepth, allowPartialMatch);
        }
        
        /// <summary>
        /// Walks the Automation Tree from the specified root, searching for the element with the given property.
        /// (Stolen from HostingHelper used by DrtLaunchContainer)
        /// </summary>
        /// <param name="root">The root to start searching from</param>
        /// <param name="property">The property to search for</param>
        /// <param name="value">The possible values of above property</param>
        /// <param name="maxDepth">The maximum depth to search</param>
        /// <param name="allowPartialMatch">Allow partial matches (i.e. for window titles)</param>
        /// <returns></returns>
        public static AutomationElement FindElementWithPropertyValue(AutomationElement root,
                                                                      AutomationProperty property,
                                                                      object[] values,
                                                                      int maxDepth,
                                                                      bool allowPartialMatch)
        {
            AutomationElement outElement = null;

            DateTime end = DateTime.Now.AddMilliseconds(Timeout);
            while ((outElement == null) && (DateTime.Now < end))
            {
                foreach (object value in values)
                {
                    if (FindElementWithPropertyValueImpl(root, property, value, maxDepth,
                                                          allowPartialMatch, 0 /* currentDepth */,
                                                          ref outElement))
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(Interval);
                }
            }

            TestServices.Assert(
                outElement != null, "The specified element was not found. Looking for values:", values);
           

            return outElement;
        }
        

        /// <summary>
        /// Will input the given KeyGesture to the system.
        /// </summary>
        /// <param name="keyGesture"></param>
        public static void InputKeyGesture(KeyGesture keyGesture)
        {
            if (keyGesture == null)
            {
                return;
            }
            // Check Modifiers, and press if needed.
            ModifierKeys modifiers = keyGesture.Modifiers;
            if ((modifiers & ModifierKeys.Alt) > 0)
            {
                KeyDown(Key.LeftAlt);
            }
            if ((modifiers & ModifierKeys.Control) > 0)
            {
                KeyDown(Key.LeftCtrl);
            }
            if ((modifiers & ModifierKeys.Shift) > 0)
            {
                KeyDown(Key.LeftShift);
            }
            if ((modifiers & ModifierKeys.Windows) > 0)
            {
                KeyDown(Key.LWin);
            }

            // Press the desired Key
            InputKey(keyGesture.Key);

            // Release any Modifier keys.
            if ((modifiers & ModifierKeys.Alt) > 0)
            {
                KeyUp(Key.LeftAlt);
            }
            if ((modifiers & ModifierKeys.Control) > 0)
            {
                KeyUp(Key.LeftCtrl);
            }
            if ((modifiers & ModifierKeys.Shift) > 0)
            {
                KeyUp(Key.LeftShift);
            }
            if ((modifiers & ModifierKeys.Windows) > 0)
            {
                KeyUp(Key.LWin);
            }
        }

        public static void InputKey(Key key)
        {
            InputKey(key, _uidelay);
        }

        /// <summary>
        /// Will input the given Key to the system.
        /// </summary>
        /// <param name="key"></param>
        public static void InputKey(Key key, int delay)
        {
            // Press desired key
            KeyDown(key, delay);

            // Release desired key
            KeyUp(key, delay);
        }

        public static void InputTextString(string inputText)
        {
            InputTextString(inputText, _uidelay);
        }

        /// <summary>
        /// Will input the given string to the system (a Key at a time).
        /// </summary>
        /// <param name="inputText"></param>
        public static void InputTextString(string inputText, int delay)
        {
            if (String.IsNullOrEmpty(inputText))
            {
                return;
            }

            // Create the KeyConverter if one hasn't already been created.
            if (keyConverter == null)
            {
                keyConverter = new KeyConverter();
            }

            // Foreach char in the string, input the appropriate key
            for (int i = 0; i < inputText.Length; i++)
            {
                string keyString = inputText.Substring(i, 1);
                // If there are more keys that KeyConverter has problems with then
                // change this condition to a switch.
                switch (keyString)
                {
                    case "." :
                        // This is not properly found by KeyConveter.
                        InputKey(Key.OemPeriod, delay);
                        break;
                    case "%" :
                        // KeyGesture doesn't currently support D5 or Shift, so this will
                        // perform the action manually.
                        KeyDown(Key.LeftShift, delay);
                        InputKey(Key.D5, delay);
                        KeyUp(Key.LeftShift, delay);
                        break;
                    case ":" :
                        // This is not properly found by KeyConverter.
                        KeyDown(Key.LeftShift, delay);
                        InputKey(Key.OemSemicolon, delay);
                        KeyUp(Key.LeftShift, delay);
                        break;
                    case "-":
                        // This is not properly found by KeyConverter.
                        InputKey(Key.OemMinus, delay);
                        break;
                    case "/" :
                        InputKey(Key.OemQuestion, delay);
                        break;
                    case "?":
                        KeyDown(Key.LeftShift, delay);
                        InputKey(Key.OemQuestion, delay);
                        KeyUp(Key.LeftShift, delay);
                        break;
                    case "_":
                        KeyDown(Key.LeftShift, delay);
                        InputKey(Key.OemMinus, delay);
                        KeyUp(Key.LeftShift, delay);
                        break;
                    case "#" :
                        KeyDown(Key.LeftShift, delay);
                        InputKey(Key.D3, delay);
                        KeyUp(Key.LeftShift, delay);
                        break;
                    default :
                        InputKey((Key)keyConverter.ConvertFrom(keyString), delay);
                        break;
                }
            }
        }

        public static void KeyDown(Key key)
        {
            KeyDown(key, _uidelay);
        }

        /// <summary>
        /// Will depress the requested key.
        /// </summary>
        /// <param name="key"></param>
        public static void KeyDown(Key key, int delay)
        {
            Input.SendKeyboardInput(key, true);
            WaitForUIRefresh(delay);
        }

        public static void KeyUp(Key key)
        {
            KeyUp(key, _uidelay);
        }

        /// <summary>
        /// Will release the requested key.
        /// </summary>
        /// <param name="key"></param>
        public static void KeyUp(Key key, int delay)
        {
            Input.SendKeyboardInput(key, false);
            WaitForUIRefresh(delay);
        }

        /// <summary>
        /// Waits for the window with the given name to close.
        /// </summary>
        /// <remarks>
        /// Asserts if the window does not close within the given timeout.
        /// </remarks>
        /// <param name="windowName">Exact name of window.</param>        
        public static void WaitForWindowClose(string windowName)
        {              
            DateTime end = DateTime.Now.AddMilliseconds(Timeout);
            while (DateTime.Now < end)
            {
                IntPtr hWnd = UnsafeNativeMethods.FindWindow(null, windowName);

                //The window no longer exists.
                if (hWnd == IntPtr.Zero)
                {
                    return;
                }

                System.Threading.Thread.Sleep(Interval);
            }

            TestServices.Assert(false, "Window {0} did not close.", windowName);
            
        }

        /// <summary>
        /// Will invoke the default pattern on an automation element or if the
        /// element is a WinForm element it will click it.
        /// </summary>
        /// <param name="element">The AutomationElement to target.</param>
        public static void Invoke(AutomationElement element)
        {
            if (IsWindowsFormControl(element))
            {
                NativeButtonClick(element);
            }
            else
            {
                //Get the list of supported patterns for this element.
                AutomationPattern[] supportedPatterns = element.GetSupportedPatterns();

                //Walk the list and invoke the first pattern we support.
                for (int i = 0; i < supportedPatterns.Length; i++)
                {
                    object pattern = element.GetCurrentPattern(supportedPatterns[i]);                    

                    //Check for patterns we know about

                    //TogglePattern: For ToggleButtons
                    if (pattern is TogglePattern)
                    {
                        ((TogglePattern)pattern).Toggle();
                        break;
                    }
                    //InvokePattern: For Buttons and other clickable things
                    else if (pattern is InvokePattern)
                    {
                        ((InvokePattern)pattern).Invoke();
                        break;
                    }
                    //ExpandCollapsePattern: For Menus and expandable things
                    else if( pattern is ExpandCollapsePattern )
                    {
                        ((ExpandCollapsePattern)pattern).Expand();
                        break;
                    }

                }
            }
        }

        /// <summary>
        /// Will set a text value on an automation element.
        /// </summary>
        /// <param name="element">The AutomationElement to target.</param>
        /// <param name="newValue">The new text value to set.</param>
        public static void SetText(AutomationElement element, string newValue)
        {
            ValuePattern valPattern = (ValuePattern)element.GetCurrentPattern(
                ValuePattern.Pattern);

            valPattern.SetValue(newValue);
        }


        /// <summary>
        /// Will get a text value on an automation element.
        /// </summary>
        /// <param name="element">The AutomationElement to target.</param>
        /// <returns>The current text value</returns>
        public static string GetText(AutomationElement element)
        {
            TextPattern textPattern = (TextPattern)element.GetCurrentPattern(
                TextPattern.Pattern);

            return textPattern.DocumentRange.GetText(_maxStringLength);
        }

        /// <summary>
        /// This will select the automation element, for example in a list box.
        /// </summary>
        /// <param name="element">The AutomationElement to target.</param>
        public static void Select(AutomationElement element)
        {
            if (IsWindowsFormControl(element))
            {
                NativeButtonClick(element);
            }
            else
            {
                SelectionItemPattern ctrlPattern = 
                    (SelectionItemPattern)element.GetCurrentPattern(
                        SelectionItemPattern.Pattern);

                ctrlPattern.Select();
            }
        }

        /// <summary>
        /// Will wait for the element to have one of the specified names.
        /// </summary>
        /// <remarks>
        /// Will TestServices.Assert on timeout.
        /// </remarks>
        /// <param name="element">The AutomationElement to watch.</param>
        /// <param name="strings">The text to wait for.</param>
        public static void WaitForText(AutomationElement element, string[] strings)
        {
            DateTime end = DateTime.Now.AddMilliseconds(Timeout);
            bool foundText = false;
            while ((!foundText) && (DateTime.Now < end))
            {
                foreach (string text in strings)
                {
                    if (element.Current.Name.Equals(text, StringComparison.CurrentCultureIgnoreCase))
                    {
                        foundText = true;
                        break;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(Interval);
                    }
                }
            }
            TestServices.Assert(
                foundText, 
                "None of the desired strings were found: {0}", 
                strings);
        }

        /// <summary>
        /// Will wait for the element to have the specified name.
        /// </summary>
        /// <remarks>
        /// Will TestServices.Assert on timeout.
        /// </remarks>
        /// <param name="element">The AutomationElement to watch.</param>
        /// <param name="text">The text to wait for.</param>
        public static void WaitForText(AutomationElement element, string text)
        {
            string[] stringArray = new string[1];
            stringArray[0] = text;

            WaitForText(element, stringArray);
        }

        /// <summary>
        /// Waits for the address bar in a given IE window to display the
        /// specified address.
        /// </summary>
        /// <remarks>
        /// Will TestServices.Assert on timeout.
        /// </remarks>
        /// <param name="ieWindow">The IE window</param>
        /// <param name="address">The address to wait for</param>
        public static void WaitForAddressBarText(AutomationElement ieWindow, string address)
        {
            // wait for the address bar to show the new document
            AutomationElement addressBar = AutomationHelper.FindDescendantById(ieWindow, "41477");

            Condition editClassCondition = new PropertyCondition(AutomationElement.ClassNameProperty, "Edit");
            addressBar = addressBar.FindFirst(TreeScope.Descendants, editClassCondition);

            TestServices.Assert(
                addressBar != null,
                "Address bar edit control was not found.");

            TraceAutomationElement("AddressBar", addressBar);    

            DateTime end = DateTime.Now.AddMilliseconds(Timeout);
            bool foundAddress = false;
            while ((!foundAddress) && (DateTime.Now < end))
            {
                TextPattern textPattern = (TextPattern)(addressBar.GetCurrentPattern(TextPattern.Pattern));
                string currentAddress = textPattern.DocumentRange.GetText(-1);

                if (currentAddress.Equals(address, StringComparison.CurrentCultureIgnoreCase))
                {
                    foundAddress = true;
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(Interval);
                }
            }

            TestServices.Assert(
                foundAddress,
                "The address was not found: {0}",
                address);
        }

        /// <summary>
        /// Finds the requested element from its Automation ID and invokes it,
        /// throwing an assert if the element returned is null.
        /// </summary>
        /// <param name="parent">The parent under which the element we're searching for lives.</param>
        /// <param name="automationID">The automation ID to search for under the parent</param>
        public static void FindAndInvoke(AutomationElement parent, string automationID)
        {
            //Find the element
            AutomationElement element = AutomationHelper.FindDescendantById(parent, automationID);
            TestServices.Assert(element != null, "Could not find the element with Automation ID '"
                + automationID + "'");

            //Invoke the element
            AutomationHelper.Invoke(element);
        }

        /// <summary>
        /// Invokes an element that should open a window.  This method will search for the
        /// element, invoke it, and then return the opened window.  To work around 





        public static AutomationElement FindAndInvokeUntilWindowOpened(
            AutomationElement parent, string automationID, string windowName)
        {
            while (true)
            {
                //Invoke the element.  Here we should fail silently since it is possible that the previous
                //iteration has opened a modal dialog or closed the original window by this point.
                AutomationHelper.FindAndManualInvoke(parent, automationID, true);

                //Attempt to locate the window
                AutomationElement window = FindWindow(windowName, /*assertOnFailure*/ false, 
                    /*timeout*/ 15000);
                if (window == null)
                {
                    TestServices.Log("Could not find the " + windowName + " window -- invoking the " +
                        automationID + " button failed.  Retrying...");
                }
                else
                {
                    return window;
                }
            }
        }

        /// <summary>
        /// Similar to FindAndInvoke, with the difference that this method
        /// performs a "manual" (non-input pattern) invocation of the element to work around
        /// automation issues with modal dialogs.
        /// </summary>
        /// <param name="parent">The parent under which the element we're searching for lives.</param>
        /// <param name="automationID">The automation ID to search for under the parent</param>
        public static void FindAndManualInvoke(AutomationElement parent, string automationID)
        {
            FindAndManualInvoke(parent, automationID, false);
        }

        /// <summary>
        /// Similar to FindAndInvoke, with the difference that this method
        /// performs a "manual" (non-input pattern) invocation of the element to work around
        /// automation issues with modal dialogs.
        /// </summary>
        /// <param name="parent">The parent under which the element we're searching for lives.</param>
        /// <param name="automationID">The automation ID to search for under the parent</param>
        /// <param name="failSilently">If true, we will simply return if the element can't be
        /// found or can't be focused.  If false, we throw an exception.</param>
        public static void FindAndManualInvoke(
            AutomationElement parent, string automationID, bool failSilently)
        {
            //We do the invoke differently here due to an unfortunate issue where
            //Invoke()ing a Winforms Button that brings up a Modal Dialog
            //will not return until the Modal Dialog is closed.  
            //This obviously would prevent this test from continuing on past that point.
            //What we're doing to work around the issue is to put focus on the button and invoke
            //an Enter keypress to bring up the dialog.
            AutomationElement element = AutomationHelper.FindDescendantById(
                parent, automationID, !failSilently);
            if (failSilently && (element == null))
            {
                TestServices.Log("warning - FindAndManualInvoke could not locate " + automationID);
                return;
            }
            TestServices.Assert(element != null, "Could not find the element with Automation ID '"
                + automationID + "'");            

            //Focus the element
            try
            {
                element.SetFocus();
            }
            catch (InvalidOperationException)
            {
                if (failSilently)
                {
                    TestServices.Log("warning - FindAndManualInvoke could not focus " + automationID);
                    return;
                }
                else
                {
                    throw;
                }
            }

            //Ensure the element has focus before continuing.
            WaitForFocus(element);

            //Invoke the "Enter" key to raise the dialog
            Input.SendKeyboardInput(Key.Enter, true);
            WaitForUIRefresh();
            Input.SendKeyboardInput(Key.Enter, false);
        }

        /// <summary>
        /// Finds the requested element from its Automation ID and sets text in it,
        /// throwing an assert if the element returned is null.
        /// </summary>
        /// <param name="parent">The parent under which the element we're searching for lives.</param>
        /// <param name="automationID">The automation ID to search for under the parent</param>
        public static void FindAndSetText(AutomationElement parent, string automationID, string text)
        {
            AutomationElement element = AutomationHelper.FindDescendantById(parent, automationID);
            TestServices.Assert(element != null, "Could not find the element with Automation ID '"
                + automationID + "'");
            AutomationHelper.SetText(element, text);
        }

        /// <summary>
        /// Waits until the given element has received focus.
        /// </summary>
        /// <param name="element"></param>
        public static void WaitForFocus(AutomationElement element)
        {
            DateTime end = DateTime.Now.AddMilliseconds(Timeout);
            while (DateTime.Now < end)
            {
                //Does the element have focus? Then we're done.
                if (element.Current.HasKeyboardFocus)
                {
                    return;
                }

                System.Threading.Thread.Sleep(Interval);

            }

            TestServices.Assert(false, "The requested element did not receive focus.");
        }

        /// <summary>
        /// Waits until the given element has the IsEnabled state set to the same state is the 
        /// IsEnabled param.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="element"></param>
        public static void WaitForIsEnableChange(AutomationElement element, bool targetEnabledState)
        {
            DateTime end = DateTime.Now.AddMilliseconds(Timeout);
            while (DateTime.Now < end)
            {
                //Is the element enabled to match the target state? Then we're done.
                if (element.Current.IsEnabled == targetEnabledState)
                {
                    return;
                }

                System.Threading.Thread.Sleep(Interval);

            }

            TestServices.Assert(false, "The requested element IsEnabled state was never marked: " +
                targetEnabledState.ToString());
        }

        /// <summary>
        /// Will wait the default amount of time, so that the UI has time to react to input keys.
        /// </summary>
        public static void WaitForUIRefresh()
        {
            WaitForUIRefresh(_uidelay);
        }

        /// <summary>
        /// Will wait the given amount of time, so that the UI has time to react to input keys.
        /// </summary>
        /// <param name="milliseconds">Count of milliseconds to wait.</param>
        public static void WaitForUIRefresh(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        /// <summary>
        /// Focus a control, send it a keygesture, and check the proper result via GetText.
        /// </summary>
        /// <param name="target">The AutomationElement to focus on, or null to keep current focus.</param>
        /// <param name="keyGesture">The key gesture to send the target.</param>
        /// <param name="textElement">The AutomationElement to check the text of (via TextPattern).</param>
        /// <param name="desiredValue">The expected string.</param>
        public static void CheckKeyResult(
            AutomationElement target,
            KeyGesture keyGesture,
            AutomationElement textElement,
            string desiredValue
            )
        {
            if (target != null)
            {
                target.SetFocus();
                AutomationHelper.WaitForFocus(target);
            }

            InputKeyGesture(keyGesture);

            string actualValue = AutomationHelper.GetText(textElement);
            TestServices.Assert(actualValue.Equals(desiredValue),
                "Key gesture ({0}) failed.  DesiredResult: {1} ActualResult: {2}",
                keyGesture.DisplayString,
                desiredValue,
                actualValue);
        }

        /// <summary>
        /// Will set the requested string as the text of the textBox, and then change
        /// focus to the requested element.
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="newValue"></param>
        /// <param name="newFocusElement"></param>
        public static void SetNewTextBoxValue(
            AutomationElement textBox,
            string newValue,
            AutomationElement newFocusElement
            )
        {
            ClearAndSetTextBoxValue(textBox, newValue);
            newFocusElement.SetFocus();
        }

        /// <summary>
        /// Will set the requested string as the text of the textBox, and then simulate
        /// the requested keypress.
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="newValue"></param>
        /// <param name="inputKey"></param>
        public static void SetNewTextBoxValue(
            AutomationElement textBox,
            string newValue,
            Key inputKey
            )
        {
            ClearAndSetTextBoxValue(textBox, newValue);
            AutomationHelper.InputKey(inputKey);
        }

        /// <summary>
        /// Will select all text of the textBox and replace it with the requested string.
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="newValue"></param>
        public static void ClearAndSetTextBoxValue(
            AutomationElement textBox,
            string newValue
            )
        {
            textBox.SetFocus();
            AutomationHelper.InputKeyGesture(new KeyGesture(Key.A, ModifierKeys.Control));
            AutomationHelper.InputTextString(newValue);
        }

        /// <summary>
        /// Will press the given key gesture, and verify that the requested element is focused.
        /// </summary>
        /// <param name="gesture"></param>
        /// <param name="element"></param>
        public static void PressKeyCheckFocus(
            KeyGesture gesture,
            AutomationElement newElement)
        {
            AutomationHelper.InputKeyGesture(gesture);

            // Wait for new element to take focus.
            WaitForFocus(newElement);

            // Check if the new element was correctly assigned focus.
            TestServices.Assert(newElement.Current.HasKeyboardFocus,
                "Gesture [{0}+{1}] failed; {2} should have focus, but {3} does.",
                gesture.Modifiers.ToString(),
                gesture.Key.ToString(),
                newElement.Current.AutomationId,
                AutomationElement.FocusedElement.Current.AutomationId);
        }

        /// <summary>
        /// Will press the key, and verify that the requested element is focused.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="element"></param>
        public static void PressKeyCheckFocus(
            Key key,
            AutomationElement newElement)
        {
            AutomationHelper.InputKey(key);

            // Wait for new element to take focus.
            WaitForFocus(newElement);

            // Check if the new element was correctly assigned focus.
            TestServices.Assert(newElement.Current.HasKeyboardFocus,
                "Key {0} failed; {1} should have focus, but {2} does.",
                key.ToString(),
                newElement.Current.AutomationId,
                AutomationElement.FocusedElement.Current.AutomationId);
        }
        #endregion

        #region Public Properties
        //----------------------------------------------------------------------
        //
        // Public Properties
        //
        //----------------------------------------------------------------------

        /// <summary>
        /// The amount of time AutomationHelper will wait for elements to 
        /// appear in milliseconds.
        /// </summary>
        public static int Timeout
        {
            get { return _timeout; }
            set 
            { 
                _timeout = value;

                if (_timeout == System.Threading.Timeout.Infinite)
                {
                    // switch to maxValue as we use date compare for automation
                    // timeout
                    _timeout = int.MaxValue;

                    TestServices.Warning(
                        "You have specified an infinite timeout waiting for automation elements; application may hang.");
                }
                else
                {
                    TestServices.Trace(
                        "AutomationHelper.Timeout={0}", _timeout); 
                }
            }
        }
        #endregion

        #region Private Methods
        //----------------------------------------------------------------------
        //
        // Private Methods
        //
        //----------------------------------------------------------------------

        /// <summary>
        /// Recursively walks the Automation Tree from the specified root, searching for the element with the given property.
        /// (Stolen from HostingHelper used by DrtLaunchContainer)
        /// </summary>
        /// <param name="root">The root to start searching from</param>
        /// <param name="property">The property to search for</param>
        /// <param name="value">The value of above property</param>
        /// <param name="maxDepth">The maximum depth to search</param>
        /// <param name="allowPartialMatch">Allow partial matches (i.e. for window titles)</param>
        /// <param name="currentLevel">The current depth of the search</param>
        /// <param name="outelement">The element found</param>
        /// <returns></returns>
        private static bool FindElementWithPropertyValueImpl(AutomationElement root,
                                                             AutomationProperty property,
                                                             object value,
                                                             int maxDepth,
                                                             bool allowPartialMatch,
                                                             int currentLevel,
                                                             ref AutomationElement outelement)
        {
            // Bail if element is null or you're too far deep.
            if (root != null && currentLevel <= maxDepth)
            {
                // See if we found the element we're looking for.
                if (IsElementWithPropertyValue(root, property, value, allowPartialMatch))
                {
                    outelement = root;
                    return true;
                }

                TreeWalker tw = TreeWalker.ControlViewWalker;

                // Don't go to siblings if you're at level zero.
                if (currentLevel != 0)
                {
                    AutomationElement nextSibling = null;

                    try
                    {
                        nextSibling = tw.GetNextSibling(root);
                    }
                    catch (ElementNotAvailableException)
                    {
                        // This exception can be ignored.                       
                    }

                    if (FindElementWithPropertyValueImpl(nextSibling,
                                                         property, value, maxDepth,
                                                         allowPartialMatch,
                                                         currentLevel, ref outelement))
                    {
                        return true;
                    }
                }

                AutomationElement nextChild = null;
                try
                {
                    nextChild = tw.GetFirstChild(root);
                }
                catch (ElementNotAvailableException)
                {
                    // This exception can be ignored.                    
                }

                if (FindElementWithPropertyValueImpl(nextChild,
                                                     property, value, maxDepth,
                                                     allowPartialMatch,
                                                     currentLevel + 1, ref outelement))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// See if this element has a specified property with specified value.
        /// </summary>
        /// <param name="root">The root of this element</param>
        /// <param name="property">The property to check</param>
        /// <param name="value">The value to check for</param>
        /// <param name="allowPartialMatch">Whether to allow partial matches</param>
        /// <returns></returns>
        private static bool IsElementWithPropertyValue(AutomationElement root,
                                                       AutomationProperty property,
                                                       object value,
                                                       bool allowPartialMatch)
        {
            // Get the value and see if you found it.
            object actualValue = root.GetCurrentPropertyValue(property);
            if (value.Equals(actualValue))
            {
                return true;
            }

            // Assume that the value is a string if allowPartialMatch is true.
            if (allowPartialMatch)
            {
                string expectedString = value as string;
                string actualString = actualValue as string;

                if (actualString == null)
                {
                    return false;
                }

                TestServices.Assert(expectedString != null, "value must be string when allowPartialMatch is true.");
                if (actualString.ToLower().IndexOf(expectedString.ToLower()) != -1)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsWindowsFormControl(AutomationElement element)
        {
            return (element.Current.ClassName.ToLower(
                System.Globalization.CultureInfo.InvariantCulture)
                    .IndexOf("windowsforms") > -1);
        }
        private static void NativeButtonClick(AutomationElement element)
        {
            UnsafeNativeMethods.SendMessage(
                new IntPtr(element.Current.NativeWindowHandle),
                Win32Messages.WM_LBUTTONDOWN,
                IntPtr.Zero,
                IntPtr.Zero);
            UnsafeNativeMethods.SendMessage(
                new IntPtr(element.Current.NativeWindowHandle),
                Win32Messages.WM_LBUTTONUP,
                IntPtr.Zero,
                IntPtr.Zero);
        }
        private static void TraceAutomationElement(
            string message, AutomationElement element)
        {
            TestServices.Trace(
                "{0}: Name:{1}, Id:{2}, Class:{3}",
                message,
                element.GetCurrentPropertyValue(
                    AutomationElement.NameProperty),
                element.GetCurrentPropertyValue(
                    AutomationElement.AutomationIdProperty),
                element.GetCurrentPropertyValue(
                    AutomationElement.ClassNameProperty));
        }
        #endregion

        #region Private Fields
        //----------------------------------------------------------------------
        //
        // Private Fields
        //
        //----------------------------------------------------------------------

        private static int Interval = 15;
        // by default we will time out after one minutes
        private static int _timeout = 60000;
        // the amount to delay while waiting for a refresh pass.
        private static int _uidelay = 50;
        // KeyConverter used to parse strings for keys.
        private static KeyConverter keyConverter;
        // Maximum length of text strings to get
        private static int _maxStringLength = 1000;
        #endregion

        private static class UnsafeNativeMethods
        {
            #region Public Methods
            //------------------------------------------------------------------
            //
            // Public Methods
            //
            //------------------------------------------------------------------

            [DllImport("user32.dll")]
            public static extern IntPtr FindWindow(
                //[MarshalAs(UnmanagedType.)]
                string className,
                //[MarshalAs(UnmanagedType.LPTStr)]
                string windowName);

            [DllImport("user32.dll")]
            public static extern int SendMessage(
                IntPtr hwnd, 
                int msg, 
                IntPtr wParam, 
                IntPtr lParam);
            #endregion
        }

        private static class Win32Messages
        {
            #region Public Fields
            //------------------------------------------------------------------
            //
            // Public Fields
            //
            //------------------------------------------------------------------

            public const int WM_DESTROY = 0x0002;
            public const int WM_LBUTTONDOWN = 0x0201;
            public const int WM_LBUTTONUP = 0x0202;
            #endregion
        }
    }
}
