// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.CoreInput.Common;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.PropertyEngine
{
    /// <summary>
    /// All property engine verifiers in this class
    /// </summary>
    public static partial class Verifiers
    {
        /// <summary>
        /// Used to verify a series of PropertyTrigger-related test cases
        /// </summary>
        /// <param name="root">Root element of the VisualTree</param>
        /// <returns>true when verification completes, false otherwise.</returns>
        public static bool PropertyTriggerVerifier(FrameworkElement root)
        {
            int totalStep = 4;

            //Get hold of buttons
            Button btn1 = root.FindName("FirstBtn") as Button;
            Button btn2 = root.FindName("SecondBtn") as Button;
            Button btn3 = root.FindName("ThirdBtn") as Button;
            string tag = root.Tag as string;

            Debug.Assert(btn1 != null && btn2 != null && btn3 != null && tag != null, "Button & Tag should always be found");
            int step = GetVerificationStep(totalStep);

            switch (tag)
            {
                case "DP0001":
                case "DP0002":
                    PropertyTriggerValidationScenario1(tag, step, btn1, btn2, btn3);
                    break;
                case "DP0003":
                    PropertyTriggerValidationScenario2(tag, step, btn1, btn2, btn3);
                    break;
                case "DP0101":
                case "DP0102":
                    PropertyTriggerValidationScenario3(tag, step, btn1, btn2, btn3);
                    break;
                case "DP0103":
                    PropertyTriggerValidationScenario4(tag, step, btn1, btn2, btn3);
                    break;
                case "DP0106":
                case "DP0107":
                    PropertyTriggerValidationScenario5(tag, step, btn1, btn2, btn3);
                    break;
                case "DP0108":
                    PropertyTriggerValidationScenario6(tag, step, btn1, btn2, btn3);
                    break;
                case "DP0111":
                case "DP0112":
                    PropertyTriggerValidationScenario3a(tag, step, btn1, btn2, btn3);
                    break;
                case "DP0113":
                    PropertyTriggerValidationScenario4a(tag, step, btn1, btn2, btn3);
                    break;
                case "DP0116":
                case "DP0117":
                    PropertyTriggerValidationScenario5a(tag, step, btn1, btn2, btn3);
                    break;
                case "DP0118":
                    PropertyTriggerValidationScenario6a(tag, step, btn1, btn2, btn3);
                    break;
                case "DP0121": case "DP0141": case "DP0151":
                case "DP0122": case "DP0142": case "DP0152":
                    PropertyTriggerValidationScenario3b(tag, step, btn1, btn2, btn3);
                    break;
                case "DP0123": case "DP0143": case "DP0153":
                    PropertyTriggerValidationScenario4b(tag, step, btn1, btn2, btn3);
                    break;
                case "DP0126":
                case "DP0127":
                    PropertyTriggerValidationScenario5b(tag, step, btn1, btn2, btn3);
                    break;
                case "DP0128":
                    PropertyTriggerValidationScenario6b(tag, step, btn1, btn2, btn3);
                    break;
                case "DP0131":
                case "DP0132":
                    PropertyTriggerValidationScenario3c(tag, step, btn1, btn2, btn3);
                    break;
                case "DP0133":
                    PropertyTriggerValidationScenario4c(tag, step, btn1, btn2, btn3);
                    break;
                default:
                    Debug.Fail("Invaid Tag encountered in PropertyTriggerVerifier");
                    break;

            }

            if (step == totalStep)
            {
                MouseHelper.MoveOutside(root, MouseLocation.Top); 
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void PropertyTriggerValidationScenario1(string tagForTestCase, int step, Button btn1, Button btn2, Button btn3)
        {
            switch (step)
            {
                case 1:
                    PrintTitle(tagForTestCase);
                    //Without triggering anything, all three buttons should be LightBlue
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 2:
                    //Move mouse to btn3, this action alone could not meet both conditions of MultiTrigger 
                    MouseHelper.Move(btn3);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 3:
                    //Also give focus to btn3, now btn3 should be tiggered into LightGreen
                    btn3.Focus();
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightGreen);
                    break;
                case 4:
                    //Move the focus to btn2, now all buttons get back to LightBlue
                    btn2.Focus();
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
            }
        }

        private static void PropertyTriggerValidationScenario2(string tagForTestCase, int step, Button btn1, Button btn2, Button btn3)
        {
            switch (step)
            {
                case 1:
                    PrintTitle(tagForTestCase);
                    MouseHelper.MoveOutside(btn3, MouseLocation.Bottom);

                    //Without triggering anything, all three buttons should be LightBlue
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 2:
                    //Move mouse to btn3, trigger Background to be Indigo
                    MouseHelper.Move(btn3);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushIndigo);
                    break;
                case 3:
                    //Also give focus to btn3, btn3 remains to be Indigo because sequence counts
                    btn3.Focus();
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushIndigo);
                    break;
                case 4:
                    //Move the focus to btn2, now btn2 is LightGreen and btn3 is Indigo
                    btn2.Focus();
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightGreen, btn3, s_brushIndigo);
                    break;
            }
        }

        //This scenario: Move Mouse to Btn1 and then to Btn2
        //When IsMouseOver is true, Color is Green; When IsMouseOver is false, Still Green
        private static void PropertyTriggerValidationScenario3(string tagForTestCase, int step, Button btn1, Button btn2, Button btn3)
        {
            switch (step)
            {
                case 1:
                    PrintTitle(tagForTestCase);
                    //Without triggering anything, all three buttons should be LightBlue
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 2:
                    //When IsMouseOver is true, Color is Green for Btn1
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 3:
                    //Also give focus to btn1, Color is still Green from Btn1
                    btn1.Focus();
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 4:
                    //Move Mouse Cursor to btn2, Color is still Green for Btn1. Also Green for Btn2
                    MouseHelper.Move(btn2);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushGreen, btn3, s_brushLightBlue);
                    break;
            }
        }

        //ExitActions
        private static void PropertyTriggerValidationScenario3a(string tagForTestCase, int step, Button btn1, Button btn2, Button btn3)
        {
            switch (step)
            {
                case 1:
                    PrintTitle(tagForTestCase);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 2:
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 3:
                    btn1.Focus();
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 4:
                    MouseHelper.Move(btn2);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
            }
        }

        //Both EnterActions and ExitActions
        private static void PropertyTriggerValidationScenario3b(string tagForTestCase, int step, Button btn1, Button btn2, Button btn3)
        {
            switch (step)
            {
                case 1:
                    PrintTitle(tagForTestCase);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 2:
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 3:
                    btn1.Focus();
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 4:
                    MouseHelper.Move(btn2);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushGreen, btn3, s_brushLightBlue);
                    break;
            }
        }

        //EnterActions and ExitActions target different property
        private static void PropertyTriggerValidationScenario3c(string tagForTestCase, int step, Button btn1, Button btn2, Button btn3)
        {
            switch (step)
            {
                case 1:
                    PrintTitle(tagForTestCase);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonWidth(btn1, 500, btn2, 500, btn3, 500);
                    break;
                case 2:
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonWidth(btn1, 500, btn2, 500, btn3, 500);
                    break;
                case 3:
                    btn1.Focus();
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonWidth(btn1, 500, btn2, 500, btn3, 500);
                    break;
                case 4:
                    MouseHelper.Move(btn2);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushGreen, btn3, s_brushLightBlue);
                    ValidateThreeButtonWidth(btn1, 600, btn2, 500, btn3, 500);
                    break;
            }
        }

        //This scenario: Move Mouse to Btn1 and then to Btn2
        //When IsMouseOver is true, Color is Green, Height is 50, Width is 600; When IsMouseOver is false, Still the same
        private static void PropertyTriggerValidationScenario4(string tagForTestCase, int step, Button btn1, Button btn2, Button btn3)
        {
            switch (step)
            {
                case 1:
                    PrintTitle(tagForTestCase);
                    //Without triggering anything, all three buttons should be LightBlue
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 30, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 500, btn2, 500, btn3, 500);
                    break;
                case 2:
                    //When IsMouseOver is true, Color is Green for Btn1
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 50, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 600, btn2, 500, btn3, 500);
                    break;
                case 3:
                    //Also give focus to btn1, Color is still Green from Btn1
                    btn1.Focus();
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 50, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 600, btn2, 500, btn3, 500);
                    break;
                case 4:
                    //Move Mouse Cursor to btn2, Color is still Green for Btn1. Also Green for Btn2
                    MouseHelper.Move(btn2);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushGreen, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 50, btn2, 50, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 600, btn2, 600, btn3, 500);
                    break;
            }
        }

        //ExitAction
        private static void PropertyTriggerValidationScenario4a(string tagForTestCase, int step, Button btn1, Button btn2, Button btn3)
        {
            switch (step)
            {
                case 1:
                    PrintTitle(tagForTestCase);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 30, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 500, btn2, 500, btn3, 500);
                    break;
                case 2:
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 30, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 500, btn2, 500, btn3, 500);
                    break;
                case 3:
                    btn1.Focus();
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 30, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 500, btn2, 500, btn3, 500);
                    break;
                case 4:
                    MouseHelper.Move(btn2);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 50, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 600, btn2, 500, btn3, 500);
                    break;
            }
        }

        //EnterAction & ExitAction
        private static void PropertyTriggerValidationScenario4b(string tagForTestCase, int step, Button btn1, Button btn2, Button btn3)
        {
            switch (step)
            {
                case 1:
                    PrintTitle(tagForTestCase);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 30, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 500, btn2, 500, btn3, 500);
                    break;
                case 2:
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 50, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 600, btn2, 500, btn3, 500);
                    break;
                case 3:
                    btn1.Focus();
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 50, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 600, btn2, 500, btn3, 500);
                    break;
                case 4:
                    MouseHelper.Move(btn2);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushGreen, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 30, btn2, 50, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 500, btn2, 600, btn3, 500);
                    break;
            }
        }

        //EnterActions and ExitActions target different property
        private static void PropertyTriggerValidationScenario4c(string tagForTestCase, int step, Button btn1, Button btn2, Button btn3)
        {
            switch (step)
            {
                case 1:
                    PrintTitle(tagForTestCase);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 30, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 500, btn2, 500, btn3, 500);
                    break;
                case 2:
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 30, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 600, btn2, 500, btn3, 500);
                    break;
                case 3:
                    btn1.Focus();
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 30, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 600, btn2, 500, btn3, 500);
                    break;
                case 4:
                    // MouseHelper.Move(btn2);  //This does not work as it does not update coordinate when btn1.height is changed
                    MouseHelper.MoveOutside(btn1, MouseLocation.Top);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 50, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 600, btn2, 500, btn3, 500);
                    break;
            }
        }

        //MutiTrigger Scenario
        //This scenario: Move Mouse to Btn1, then give focus, and then to Btn2
        private static void PropertyTriggerValidationScenario5(string tagForTestCase, int step, Button btn1, Button btn2, Button btn3)
        {
            switch (step)
            {
                case 1:
                    PrintTitle(tagForTestCase);
                    //Without triggering anything, all three buttons should be LightBlue
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 2:
                    //When IsMouseOver is true, MultiTrigger conditions are still false
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 3:
                    //Now give focus to btn1, MultiTrigger conditions are true
                    //btn1.Focus();
                    MouseHelper.Click();
                    MouseHelper.MoveOutside(btn1, MouseLocation.Top);
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 4:
                    //Move Mouse Cursor to btn2, Color is still Green for Btn1. Btn2 is not changed
                    MouseHelper.Move(btn2);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
            }
        }

        //ExitAction
        private static void PropertyTriggerValidationScenario5a(string tagForTestCase, int step, Button btn1, Button btn2, Button btn3)
        {
            switch (step)
            {
                case 1:
                    PrintTitle(tagForTestCase);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 2:
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 3:
                    MouseHelper.Click();
                    MouseHelper.MoveOutside(btn1, MouseLocation.Top);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 4:
                    MouseHelper.Move(btn2);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
            }
        }

        //EnterAction and ExitAction
        private static void PropertyTriggerValidationScenario5b(string tagForTestCase, int step, Button btn1, Button btn2, Button btn3)
        {
            switch (step)
            {
                case 1:
                    PrintTitle(tagForTestCase);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 2:
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 3:
                    MouseHelper.Click();
                    MouseHelper.MoveOutside(btn1, MouseLocation.Top);
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
                case 4:
                    MouseHelper.Move(btn2);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    break;
            }
        }

        //MutiTrigger Scenario
        //This scenario: Move Mouse to Btn1, then give focus, and then to Btn2
        private static void PropertyTriggerValidationScenario6(string tagForTestCase, int step, Button btn1, Button btn2, Button btn3)
        {
            switch (step)
            {
                case 1:
                    PrintTitle(tagForTestCase);
                    //Without triggering anything, all three buttons should be LightBlue
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 30, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 500, btn2, 500, btn3, 500);
                    break;
                case 2:
                    //When IsMouseOver is true, MultiTrigger conditions are still false
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 30, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 500, btn2, 500, btn3, 500);
                    break;
                case 3:
                    //Now give focus to btn1, MultiTrigger conditions are true
                    //btn1.Focus();
                    MouseHelper.Click();
                    MouseHelper.MoveOutside(btn1, MouseLocation.Top);
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 50, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 600, btn2, 500, btn3, 500);
                    break;
                case 4:
                    //Move Mouse Cursor to btn2, Color is still Green for Btn1. Btn2 is not changed
                    MouseHelper.Move(btn2);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 50, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 600, btn2, 500, btn3, 500);
                    break;
            }
        }

        //ExitAction
        private static void PropertyTriggerValidationScenario6a(string tagForTestCase, int step, Button btn1, Button btn2, Button btn3)
        {
            switch (step)
            {
                case 1:
                    PrintTitle(tagForTestCase);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 30, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 500, btn2, 500, btn3, 500);
                    break;
                case 2:
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 30, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 500, btn2, 500, btn3, 500);
                    break;
                case 3:
                    MouseHelper.Click();
                    MouseHelper.MoveOutside(btn1, MouseLocation.Top);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 50, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 600, btn2, 500, btn3, 500);
                    break;
                case 4:
                    MouseHelper.Move(btn2);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 50, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 600, btn2, 500, btn3, 500);
                    break;
            }
        }

        private static void PropertyTriggerValidationScenario6b(string tagForTestCase, int step, Button btn1, Button btn2, Button btn3)
        {
            switch (step)
            {
                case 1:
                    PrintTitle(tagForTestCase);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 30, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 500, btn2, 500, btn3, 500);
                    break;
                case 2:
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 30, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 500, btn2, 500, btn3, 500);
                    break;
                case 3:
                    MouseHelper.Click();
                    MouseHelper.MoveOutside(btn1, MouseLocation.Top);
                    MouseHelper.Move(btn1);
                    ValidateThreeButtonBackground(btn1, s_brushGreen, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 50, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 600, btn2, 500, btn3, 500);
                    break;
                case 4:
                    MouseHelper.Move(btn2);
                    ValidateThreeButtonBackground(btn1, s_brushLightBlue, btn2, s_brushLightBlue, btn3, s_brushLightBlue);
                    ValidateThreeButtonHeight(btn1, 30, btn2, 30, btn3, 30);
                    ValidateThreeButtonWidth(btn1, 500, btn2, 500, btn3, 500);
                    break;
            }
        }

    }
}

