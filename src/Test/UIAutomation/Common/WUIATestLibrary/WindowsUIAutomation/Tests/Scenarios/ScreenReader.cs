// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//******************************************************************* 
//* Purpose: 
//* Owner: Microsoft
//* Contributors:
//******************************************************************* 
using System;
using System.CodeDom;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Automation;
using System.Windows;
using ATGTestInput;
using MS.Win32;
using Microsoft.Win32;
using System.Reflection;
using System.Text;
using System.Resources;

#pragma warning disable 1634, 1691

namespace Microsoft.Test.WindowsUIAutomation.Tests.Scenarios
{
    using InternalHelper.Enumerations;
    using InternalHelper.Tests;
    using Microsoft.Test.WindowsUIAutomation;
    using Microsoft.Test.WindowsUIAutomation.Core;
    using Microsoft.Test.WindowsUIAutomation.Interfaces;
    using Microsoft.Test.WindowsUIAutomation.TestManager;

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class ScreenReaderScenarioTests : ScenarioObject
    {
        #region constructor

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public ScreenReaderScenarioTests(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
            base(element, testSuite, priority, TypeOfControl.UnknownControl, TypeOfPattern.Unknown, null, testEvents, commands)
        {
        }

        #endregion constructor

        #region member variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        const string THIS = "ScreenReaderScenarioTests";

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public const string TestSuite = NAMESPACE + "." + THIS;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        string _NARRATOR_EXE = "Narrator.exe";
        string _NARRATOR_RESOURCE_DLL = "Narrator.resources.dll";

        /// -------------------------------------------------------------------
        /// <summary>Wait 10 seconds</summary>
        /// -------------------------------------------------------------------
        const int _WAIT_NORMAL_MILLISECONDS = 10000;

        /// -------------------------------------------------------------------
        /// <summary>Wait for 10 minutes</summary>
        /// -------------------------------------------------------------------
        const int _WAIT_STARTUP_MILLISECONDS = 600000;

        /// -------------------------------------------------------------------
        /// <summary>Defined here so I don't need to define it in every 
        /// method</summary>
        /// -------------------------------------------------------------------
        Exception _caughtException = null;

        /// -------------------------------------------------------------------
        /// <summary>Used to test in Wait* function if a value has been updated 
        /// </summary>
        /// -------------------------------------------------------------------
        object _changedToValue = null;

        /// -------------------------------------------------------------------
        /// <summary>Variable used in the Wait method</summary>
        /// -------------------------------------------------------------------
        static AutomationElement s_waitElement = null;

        /// -------------------------------------------------------------------
        /// <summary>Common delagates used</summary>
        /// -------------------------------------------------------------------
        delegate bool WaitTestState();
        delegate AutomationElement GetElement();


        /// -------------------------------------------------------------------
        /// <summary>Define the methods for invoking actions</summary>
        /// -------------------------------------------------------------------
        enum ActionMethod
        {
            UIAutomation,
            KeyPadOnly,
            Menu,
            CloseXButton,
            MouseClick,
            AltF4,
            F1,
            KillProcess,
            CheckBox
        }

        #endregion member variables

        /// -------------------------------------------------------------------
        /// <summary>
        /// Used from within the test cases to document any issues. Overrides the
        /// TestObject.HeaderComment method.
        /// </summary>
        /// -------------------------------------------------------------------
        internal void HeaderComment(TestCaseAttribute testCaseAttribute, bool initRegistry)
        {
            _caughtException = null;
            m_TestStep = 0;
            m_TestCase = testCaseAttribute;
            Debug.Assert(testCaseAttribute.WttJob != 0, "Need to set the WttJob");
            if (initRegistry)
            {
                HelperPersistKeyNameValuesToTable(_keyNameValuesOriginal);
                HelperSetKeyNameValuesFromTable(_keyNameValuesDefault);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Used from within the test cases to document any issues. Overrides the
        /// TestObject.HeaderComment method.
        /// </summary>
        /// -------------------------------------------------------------------
        new internal void HeaderComment(TestCaseAttribute testCaseAttribute)
        {
            HeaderComment(testCaseAttribute, true);
        }

        #region Screen Reader Scenarios

        #region 4.3 Registry Settings

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("312",
            TestSummary = "Verify initial registry settings",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ScreenReader,
           WttJob = 312,
            Description = new string[] {
                "Step: Verify initial registry settings",
        })]
        public void TestNarratorRegistrySettingsInitialSettingsAnnounceScrollNotifications(TestCaseAttribute testCaseAttribute)
        {
            // Dump information
            HeaderComment(testCaseAttribute, false);

            // "Step: Verify initial registry settings",
            TS_VerifyDefaultRegsitryEntries(CheckType.Verification);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("313",
            TestSummary = "Modify settings via Narrator UI",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ScreenReader,
            WttJob = 313,
            Description = new string[] {
                 "Step: Start Narrator",
                 "Step: Open the Background message dialog",
                 "Step: Change the Time period combo box to random value",
                 "Step: Close the Background message dialog",
                 //----------------------------------------------
                 "Step: Open \"Voice Settings\" dialog",
                 "Step: Toggle \"Announce Scroll Notifications\" checkbox",
                 "Step: Toggle \"Announce System Notifications\" checkbox",
                 "Step: Toggle \"Echo User's Keyboard\" checkbox",
                 "Step: Toggle \"Start Narrator Minimized\" checkbox",
                 "Step: Close \"Voice Settings\" Dialog",
                 "Step: Toggle \"Announce Scroll Notifications\" checkbox",
                 "Step: Toggle \"Announce System Notifications\" checkbox",
                 "Step: Toggle \"Echo User's Keyboard\" checkbox",
                 "Step: Toggle \"Start Narrator Minimized\" checkbox",
                 "Step: Close Narrator",
                 "Step: Restart Narrator",
                 //---------------------------------------------------------------
                 "Step: Start Narrator",
                 "Step: Open \"Background Message\" dialog",
                 "Step: Verify the registry settings for \"Background timeout\" combo box",
                 "Step: Verify the actual value for \"Background timeout\" combo box",
                 "Step: Close \"Background Message\" dialog",
                 //----------------------------------------------
                 "Step: Open \"Voice Settings\" dialog",
                 "Step: Verify current \"Pitch Settings\" registry setting to previous runs value set",
                 "Step: Verify current \"Pitch Settings\" combo box setting to previous runs value set",
                 "Step: Verify current \"Set Speed\" registry setting to previous runs value set",
                 "Step: Verify current \"Set Speed\" combo box setting to previous runs value set",
                 "Step: Verify current \"Set Volume\" registry setting to previous runs value set",
                 "Step: Verify current \"Set Volume\" combo box setting to previous runs value set",
                 "Step: Verify current \"Set Volume\" registry setting to previous runs value set",
                 "Step: Verify current \"Set Volume\" combo box setting to previous runs value set",
                 "Step: Close the \"Voice Settings\" dialog",
                 "Step: Verify current \"Echo Users Keystokes\" registry setting to previous runs value set",
                 "Step: Verify current \"Echo Users Keystokes\" check box setting to previous runs value set",
                 "Step: Verify current \"Announce System Messages\" registry setting to previous runs value set",
                 "Step: Verify current \"Announce System Messages\" check box setting to previous runs value set",
                 "Step: Verify current \"Announce Scroll Notifications\" registry setting to previous runs value set",
                 "Step: Verify current \"Announce Scroll Notifications\" check box setting to previous runs value set",
                 "Step: Verify current \"Start Narrator Minimized\" registry setting to previous runs value set",
                 "Step: Verify current \"Start Narrator Minimized\" check box setting to previous runs value set",
                 "Step: Close Narrator",        
            })]
        public void TestNarratorRegistrySettingsRegistryPersistency(TestCaseAttribute testCaseAttribute)
        {

            // Dump information
            HeaderComment(testCaseAttribute);

            try
            {
                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                // ----------------------------------------------
                // Background message settings
                // "Step: Open the Background message dialog",
                TS_DialogBackGroundMessageOpen(CheckType.Verification);

                // "Step: Change the Time period combo box to random value",
                TS_ChangeListValueRandom(MainForm.DialogBackgroundMessage.TimePeriodComboBox, WaitBackMessageComboBoxVolumeSettingChanged, CheckType.Verification);

                // "Step: Close the Background message dialog",
                TS_DialogBackGroundMessageClose(CheckType.Verification);

                // ----------------------------------------------
                // Background message settings
                // "Step: \"Open Voice Settings\" dialog",
                TS_DialogVoiceSettingsOpen(CheckType.Verification);

                // "Step: Toggle \"Announce Scroll Notifications\" checkbox",
                TS_ChangeListValueRandom(MainForm.DialogVoiceSettings.ComboBoxPitchSetting, WaitComboBoxPitchSettingChanged, CheckType.Verification);

                // "Step: Toggle \"Announce System Notifications\" checkbox",
                TS_ChangeListValueRandom(MainForm.DialogVoiceSettings.ComboBoxSpeedSetting, WaitComboBoxSpeedSettingChanged, CheckType.Verification);

                // "Step: Toggle \"Echo User's Keyboard\" checkbox",
                TS_ChangeListValueRandom(MainForm.DialogVoiceSettings.ComboBoxVolumeSetting, WaitComboBoxVolumeSettingChanged, CheckType.Verification);

                // "Step: Toggle \"Start Narrator Minimized\" checkbox",
                TS_ChangeListValueRandom(MainForm.DialogVoiceSettings.ListVoiceSettingList, WaitListVoiceSettingListChanged, CheckType.Verification);

                // "Step: Close Voice Settings Dialog",
                TS_DialogVoiceSettingsClose(CheckType.Verification);

                // "Step: Toggle \"Announce Scroll Notifications\" checkbox",
                TS_ToggleCheckState(MainForm.CheckBoxAnnounceScrollNotifications, WaitCheckBoxAnnounceScrollNotificationsIsChecked, WaitCheckBoxAnnounceScrollNotificationsIsUnChecked, CheckType.Verification);

                // "Step: Toggle \"Announce System Notifications\" checkbox",
                TS_ToggleCheckState(MainForm.CheckBoxAnnounceSystemMessages, WaitCheckBoxAnnounceSystemMessagesIsChecked, WaitCheckBoxAnnounceSystemMessagesIsUnChecked, CheckType.Verification);

                // "Step: Toggle \"Echo User's Keyboard\" checkbox",
                TS_ToggleCheckState(MainForm.CheckBoxKeyboardEcho, WaitCheckBoxKeyboardEchoIsChecked, WaitCheckBoxKeyboardEchoIsUnChecked, CheckType.Verification);

                // "Step: Toggle \"Start Narrator Minimized\" checkbox",
                TS_ToggleCheckState(MainForm.CheckBoxStartMinimized, WaitCheckBoxStartMinimizedIsChecked, WaitCheckBoxStartMinimizedIsUnChecked, CheckType.Verification);

                HelperPersistKeyNameValuesToTable(_keyNameValuesCurrent);

                // "Step: Close Narrator",
                TS_NarratorClose(ActionMethod.UIAutomation, true, CheckType.Verification);

                // "Step: Restart Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                // ---------------------------------------------------------------
                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                // Step: Open Background message dialog",
                TS_DialogBackGroundMessageOpen(CheckType.Verification);

                // Step: Verify the registry settings for Background timeout combo box",
                TS_VerifyRegistryKeyNameValue(KEYNAME_BACKGROUNDMESSAGETIMEOUT, _keyNameValuesCurrent[KEYNAME_BACKGROUNDMESSAGETIMEOUT], CheckType.Verification);

                // Step: Verify the actual value for Background timeout combo box",
                TS_VerifyListValue(MainForm.DialogBackgroundMessage.TimePeriodComboBox, KEYNAME_BACKGROUNDMESSAGETIMEOUT, CheckType.Verification);

                // "Step: Close Background message dialog",
                TS_DialogBackGroundMessageClose(CheckType.Verification);

                // ----------------------------------------------
                // Open Voice Settings dialog
                TS_DialogVoiceSettingsOpen(CheckType.Verification);

                // Step: Verify current \"Pitch Settings\" registry setting to previous runs value set",
                TS_VerifyRegistryKeyNameValue(KEYNAME_CURRENTPITCH, _keyNameValuesCurrent[KEYNAME_CURRENTPITCH], CheckType.Verification);

                // Step: Verify current \"Pitch Settings\" combo box setting to previous runs value set",
                TS_VerifyListValue(MainForm.DialogVoiceSettings.ComboBoxPitchSetting, KEYNAME_CURRENTPITCH, CheckType.Verification);

                // Step: Verify current \"Set Speed\" registry setting to previous runs value set",
                TS_VerifyRegistryKeyNameValue(KEYNAME_CURRENTSPEED, _keyNameValuesCurrent[KEYNAME_CURRENTSPEED], CheckType.Verification);

                // Step: Verify current \"Set Speed\" combo box setting to previous runs value set",
                TS_VerifyListValue(MainForm.DialogVoiceSettings.ComboBoxSpeedSetting, KEYNAME_CURRENTSPEED, CheckType.Verification);

                // Step: Verify current \"Set Volume\" registry setting to previous runs value set",
                TS_VerifyRegistryKeyNameValue(KEYNAME_CURRENTVOLUME, _keyNameValuesCurrent[KEYNAME_CURRENTVOLUME], CheckType.Verification);

                // Step: Verify current \"Set Volume\" combo box setting to previous runs value set",
                TS_VerifyListValue(MainForm.DialogVoiceSettings.ComboBoxVolumeSetting, KEYNAME_CURRENTVOLUME, CheckType.Verification);

                // Step: Verify current \"Set Volume\" registry setting to previous runs value set",
                TS_VerifyRegistryKeyNameValue(KEYNAME_CURRENTVOICE, _keyNameValuesCurrent[KEYNAME_CURRENTVOICE], CheckType.Verification);

                // Step: Verify current \"Set Volume\" combo box setting to previous runs value set",
                TS_VerifyListValue(MainForm.DialogVoiceSettings.ListVoiceSettingList, KEYNAME_CURRENTVOICE, CheckType.Verification);

                // "Step: Close the \"Voice Settings\" dialog",
                TS_DialogVoiceSettingsClose(CheckType.Verification);

                // Step: Verify current \"Echo Users Keystokes\" registry setting to previous runs value set",
                TS_VerifyRegistryKeyNameValue(KEYNAME_ECHOCHARS, _keyNameValuesCurrent[KEYNAME_ECHOCHARS], CheckType.Verification);

                // Step: Verify current \"Echo Users Keystokes\" check box setting to previous runs value set",
                TS_VerifyToggleState(MainForm.CheckBoxKeyboardEcho, KEYNAME_ECHOCHARS, CheckType.Verification);

                // Step: Verify current \"Announce System Messages\" registry setting to previous runs value set",
                TS_VerifyRegistryKeyNameValue(KEYNAME_ANNOUNCESYSTEMMESSAGES, _keyNameValuesCurrent[KEYNAME_ANNOUNCESYSTEMMESSAGES], CheckType.Verification);

                // Step: Verify current \"Announce System Messages\" check box setting to previous runs value set",
                TS_VerifyToggleState(MainForm.CheckBoxAnnounceSystemMessages, KEYNAME_ANNOUNCESYSTEMMESSAGES, CheckType.Verification);

                // Step: Verify current \"Announce Scroll Notifications\" registry setting to previous runs value set",
                TS_VerifyRegistryKeyNameValue(KEYNAME_ANNOUNCESCROLLNOTIFICATIONS, _keyNameValuesCurrent[KEYNAME_ANNOUNCESCROLLNOTIFICATIONS], CheckType.Verification);

                // Step: Verify current \"Announce Scroll Notifications\" check box setting to previous runs value set",
                TS_VerifyToggleState(MainForm.CheckBoxAnnounceScrollNotifications, KEYNAME_ANNOUNCESCROLLNOTIFICATIONS, CheckType.Verification);

                // Step: Verify current \"Start Narrator Minimized\" registry setting to previous runs value set",
                TS_VerifyRegistryKeyNameValue(KEYNAME_STARTTYPE, _keyNameValuesCurrent[KEYNAME_STARTTYPE], CheckType.Verification);

                // Step: Verify current \"Start Narrator Minimized\" check box setting to previous runs value set",
                TS_VerifyToggleState(MainForm.CheckBoxStartMinimized, KEYNAME_STARTTYPE, CheckType.Verification);

                // "Step: Close Narrator",
                TS_NarratorClose(ActionMethod.KeyPadOnly, true, CheckType.Verification);
            }
            catch (Exception exception)
            {
                _caughtException = exception;
            }
            finally
            {
                HelperFinallyCleanup(_caughtException);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("269",
            TestSummary = "Narrator should create registry keys if they do not exist",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ScreenReader,
            WttJob = 269,
            Description = new string[] {
                 "Step: Delete the HKEY_CURRENT_USER\\Software\\Microsoft\\Narrator subkeytree",
                 "Step: Start Narrator",
                 "Step: Verify that Narrator created the registry subkeytree and the default settings are set",
                 "Step: Close Narrator",
        })]
        public void Test269(TestCaseAttribute testCaseAttribute)
        {
            // Dump information
            HeaderComment(testCaseAttribute, false);

            try
            {
                // "Ste: Delete the HKEY_CURRENT_USER\\Software\\Microsoft\\Narrator subkeytree
                TS_DeleteRegistrySubKeyTree(CheckType.Verification);

                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                // "Step: Verify that Narrator created the registry subkeytree and the default settings are set",
                TS_VerifyDefaultRegsitryEntries(CheckType.Verification);

                // "Step: Close Narrator",
                TS_NarratorClose(ActionMethod.KeyPadOnly, true, CheckType.Verification);
            }
            catch (Exception exception)
            {
                _caughtException = exception;
            }
            finally
            {
                HelperFinallyCleanup(_caughtException);
            }
        }

        #endregion 4.3 Registry Settings

        #region 4.4 Exit Narrator

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("314",
            TestSummary = "Verify that Narrator can start up and close Narrator with WindowPattern.Close()",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.ReadyToAddToWtt,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ScreenReader,
            WttJob = 314,
            Description = new string[] {
                "Step: Start Narrator",
                "Step: Close Narrator using WindowPattern.Close()",
        })]
        public void TestNarratorStartStopByWindowPattern(TestCaseAttribute testCaseAttribute)
        {
            // Dump information
            HeaderComment(testCaseAttribute);

            try
            {
                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                // "Step: Close Narrator",
                TS_NarratorClose(ActionMethod.UIAutomation, true, CheckType.Verification);
            }
            catch (Exception exception)
            {
                _caughtException = exception;
            }
            finally
            {
                HelperFinallyCleanup(_caughtException);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("314",
            TestSummary = "Verify that Narrator can start up and then pressing Exit button closes Narrator",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ScreenReader,
            WttJob = 196531,
            Description = new string[] {
                 "Step: Start Narrator",
                 "Step: Close Narrator",
        })]
        public void TestNarratorStartCloseByPressingEnterOnExitButton(TestCaseAttribute testCaseAttribute)
        {
            // Dump information
            HeaderComment(testCaseAttribute);

            try
            {
                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                // "Step: Close Narrator",
                TS_NarratorClose(ActionMethod.KeyPadOnly, true, CheckType.Verification);
            }
            catch (Exception exception)
            {
                _caughtException = exception;
            }
            finally
            {
                HelperFinallyCleanup(_caughtException);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("270",
            TestSummary = "Verify that Narrator can start up and then pressing \"Alt+F4\" closes Narrator",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ScreenReader,
            WttJob = 270,
            Description = new string[] {
                 "Step: Start Narrator",
                 "Step: Close Narrator",
        })]
        public void Test270(TestCaseAttribute testCaseAttribute)
        {
            // Dump information
            HeaderComment(testCaseAttribute);

            try
            {
                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                // "Step: Close Narrator",
                TS_NarratorClose(ActionMethod.AltF4, true, CheckType.Verification);
            }
            catch (Exception exception)
            {
                _caughtException = exception;
            }
            finally
            {
                HelperFinallyCleanup(_caughtException);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("315",
            TestSummary = "Verify that Narrator can start up and then pressing Close Button \"X\" closes Narrator",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ScreenReader,
            WttJob = 196531,
            Description = new string[] {
                 "Step: Start Narrator",
                 "Step: Close Narrator",
        })]
        public void Test271(TestCaseAttribute testCaseAttribute)
        {
            // Dump information
            HeaderComment(testCaseAttribute);

            try
            {
                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                // "Step: Close Narrator",
                TS_NarratorClose(ActionMethod.CloseXButton, true, CheckType.Verification);
            }
            catch (Exception exception)
            {
                _caughtException = exception;
            }
            finally
            {
                HelperFinallyCleanup(_caughtException);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("272",
            TestSummary = "Verify that Narrator can start up and then Process.Kill() closes Narrator",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ScreenReader,
            WttJob = 272,
            Description = new string[] {
                 "Verification: Narrator.exe is located on the disk",
                 "Step: Start Narrator",
                 "Step: Close Narrator",
        })]
        public void Test272(TestCaseAttribute testCaseAttribute)
        {
            // Dump information
            HeaderComment(testCaseAttribute);

            try
            {
                TS_NarratorExistOnDrive(CheckType.Verification);

                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                // "Step: Close Narrator, by Process.Kill",
                TS_NarratorClose(ActionMethod.KillProcess, true, CheckType.Verification);
            }
            catch (Exception exception)
            {
                _caughtException = exception;
            }
            finally
            {
                HelperFinallyCleanup(_caughtException);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("271",
            TestSummary = "Open Narrator and then when closing Narrator, pressing Cancel button leaves Narrator open",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ScreenReader,
            WttJob = 196450,
            Description = new string[] {
                 "Step: Start Narrator",
                 "Step: Close Narrator, press Cancel button on Exit dialog",
                 "Step: Verify that Narrator is still visible",
                 "Step: Close Narrator",
        })]
        public void TestNarratorStartCancelExitDialog(TestCaseAttribute testCaseAttribute)
        {
            // Dump information
            HeaderComment(testCaseAttribute);

            try
            {
                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                // "Step: Close Narrator, press Cancel button on Exit dialog",
                TS_NarratorClose(ActionMethod.KeyPadOnly, false, CheckType.Verification);

                // Pause.  TS_CloseNarrator should have pressed the cancel button. Pause and then
                // check to make sure Narrator did actually stay up.
                Thread.Sleep(_WAIT_NORMAL_MILLISECONDS);

                // "Step: Verify that Narrator is still visible",
                TS_VerifyNarratorVisible(CheckType.Verification);

                // "Step: Close Narrator",
                TS_NarratorClose(ActionMethod.KeyPadOnly, true, CheckType.Verification);
            }
            catch (Exception exception)
            {
                _caughtException = exception;
            }
            finally
            {
                HelperFinallyCleanup(_caughtException);
            }
        }

        #endregion 4.4 Exit Narrator

        #region 4.5 Help

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("273",
            TestSummary = "Open the Help document with F1",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.ReadyToAddToWtt,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ScreenReader,
            WttJob = 196450,
            Description = new string[] {
                 "Step: Start Narrator",
                 "Step: Open the Help document with F1",
                 "Step: Close the Help document",
                 "Step: Close Narrator",
        })]
        public void Test273(TestCaseAttribute testCaseAttribute)
        {
            // Dump information
            HeaderComment(testCaseAttribute);

            try
            {
                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                // "Step: Open the Help document with F1",
                TS_HelpDocumentOpen(ActionMethod.F1, CheckType.Verification);

                // "Step: Close the Help document",
                TS_HelpDocumentClose(CheckType.Verification);

                // "Step: Close Narrator",
                TS_NarratorClose(ActionMethod.KeyPadOnly, true, CheckType.Verification);
            }
            catch (Exception exception)
            {
                _caughtException = exception;
            }
            finally
            {
                HelperFinallyCleanup(_caughtException);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("274",
            TestSummary = "Open the Help document with Menu",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.ReadyToAddToWtt,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ScreenReader,
            WttJob = 196450,
            Description = new string[] {
                 "Step: Start Narrator",
                 "Step: Open the Help document with Menu",
                 "Step: Close the Help document",
                 "Step: Close Narrator",
        })]
        public void Test274(TestCaseAttribute testCaseAttribute)
        {
            // Dump information
            HeaderComment(testCaseAttribute);

            try
            {
                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                // "Step: Open the Help document with Menu",
                TS_HelpDocumentOpen(ActionMethod.Menu, CheckType.Verification);

                // "Step: Close the Help document",
                TS_HelpDocumentClose(CheckType.Verification);

                // "Step: Close Narrator",
                TS_NarratorClose(ActionMethod.KeyPadOnly, true, CheckType.Verification);
            }
            catch (Exception exception)
            {
                _caughtException = exception;
            }
            finally
            {
                HelperFinallyCleanup(_caughtException);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("275",
            TestSummary = "Open the About dialog with Menu item",
            Priority = TestPriorities.Pri2,
            Status = TestStatus.ReadyToAddToWtt,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ScreenReader,
           WttJob = 275,
            Description = new string[] {
                 "Step: Start Narrator",
                 "Step: Open the About dialog with Menu",
                 "Step: Close the About dialog",
                 "Step: Close Narrator",
        })]
        public void Test275(TestCaseAttribute testCaseAttribute)
        {
            // Dump information
            HeaderComment(testCaseAttribute);

            try
            {
                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                // "Step: Open the About dialog",
                TS_DialogAboutBoxOpen(ActionMethod.Menu, CheckType.Verification);

                // "Step: Close the About dialog",
                TS_DialogAboutBoxClose(CheckType.Verification);

                // "Step: Close Narrator",
                TS_NarratorClose(ActionMethod.KeyPadOnly, true, CheckType.Verification);
            }
            catch (Exception exception)
            {
                _caughtException = exception;
            }
            finally
            {
                HelperFinallyCleanup(_caughtException);
            }
        }
        #endregion 4.5 Help

        #region 4.6 Echo User's Keystokes

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("276",
            TestSummary = "Toggle \"Echo User's Keystokes\" by selecting the menu itm from the \"Preferences\" menu",
            Priority = TestPriorities.Pri1,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ScreenReader,
            WttJob = 276,
            Description = new string[] {
                 "Step: Start Narrator",
                 "Step: Toggle the \"Echo User's Keystokes\" menu",
                 "Step: Close Narrator",
                 "Step: Restart Narrator",
                 "Step: Verify current \"Echo User's Keystokes\" registry setting to previous runs value set",
                 "Step: Verify current \"Echo User's Keystokes\" check box setting to previous runs value set",
                 "Step: Verify the ToggleState of the \"Echo User's Keystokes\" sub menu",
                 "Step: Close Narrator",
            })]
        public void Test276(TestCaseAttribute testCaseAttribute)
        {

            // Dump information
            HeaderComment(testCaseAttribute);

            try
            {

                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                TSM_MainDialogSettings(ActionMethod.Menu,
                    KEYNAME_ECHOCHARS,
                    GetMenuKeyBoardEcho,
                    GetCheckBoxKeyboardEcho,
                    WaitCheckBoxKeyboardEchoIsChecked, WaitCheckBoxKeyboardEchoIsUnChecked,
                    CheckType.Verification);

                // "Step: Close Narrator",
                TS_NarratorClose(ActionMethod.KeyPadOnly, true, CheckType.Verification);
            }
            catch (Exception exception)
            {
                _caughtException = exception;
            }
            finally
            {
                HelperFinallyCleanup(_caughtException);
            }
        }

        #endregion 4.6 Echo User's Keystokes

        #region 4.7 Announce System Notifications

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("277",
           TestSummary = "Toggle \"Announce System Notifications\" by selecting the menu itm from the \"Preferences\" menu",
            Priority = TestPriorities.Pri2,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ScreenReader,
           WttJob = 278,
            Description = new string[] {
                 "Step: Start Narrator",
                 "Step: Toggle the \"Announce System Notifications\" menu",
                 "Step: Close Narrator",
                 "Step: Restart Narrator",
                 "Step: Verify current \"Announce System Notifications\" registry setting to previous runs value set",
                 "Step: Verify current \"Announce System Notifications\" check box setting to previous runs value set",
                 "Step: Verify the ToggleState of the \"Announce System Notifications\" sub menu",
                 "Step: Close Narrator",
            })]
        public void Test277(TestCaseAttribute testCaseAttribute)
        {

            // Dump information
            HeaderComment(testCaseAttribute);

            try
            {
                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                TSM_MainDialogSettings(ActionMethod.Menu,
                    KEYNAME_ANNOUNCESYSTEMMESSAGES,
                    GetMenuAnnounceSystemMessages,
                    GetCheckBoxAnnounceSystemMessages,
                    WaitCheckBoxAnnounceSystemMessagesIsChecked,
                    WaitCheckBoxAnnounceSystemMessagesIsUnChecked,
                    CheckType.Verification);

                // "Step: Close Narrator",
                TS_NarratorClose(ActionMethod.KeyPadOnly, true, CheckType.Verification);
            }
            catch (Exception exception)
            {
                _caughtException = exception;
            }
            finally
            {
                HelperFinallyCleanup(_caughtException);
            }
        }

        #endregion 4.8 Announce System Notifications

        #region 4.8 Announce Scroll Notifications

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("278",
            TestSummary = "Toggle \"Announce Scroll Notifications\" by selecting the menu itm from the \"Preferences\" menu",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ScreenReader,
            WttJob = 278,
            Description = new string[] {
                 "Step: Start Narrator",
                 "Step: Toggle the \"Announce Scroll Notifications\" menu",
                 "Step: Close Narrator",
                 "Step: Restart Narrator",
                 "Step: Verify current \"Announce Scroll Notifications\" registry setting to previous runs value set",
                 "Step: Verify current \"Announce Scroll Notifications\" check box setting to previous runs value set",
                 "Step: Verify the ToggleState of the \"Announce Scroll Notifications\" sub menu",
                 "Step: Close Narrator",
            })]
        public void Test278(TestCaseAttribute testCaseAttribute)
        {

            // Dump information
            HeaderComment(testCaseAttribute);

            try
            {
                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                TSM_MainDialogSettings(ActionMethod.Menu,
                    KEYNAME_ANNOUNCESCROLLNOTIFICATIONS,
                    GetMenuAnnounceScrollMessages,
                    GetCheckBoxAnnounceScrollNotifications,
                    WaitCheckBoxAnnounceScrollNotificationsIsChecked,
                    WaitCheckBoxAnnounceScrollNotificationsIsUnChecked,
                    CheckType.Verification);


                // "Step: Close Narrator",
                TS_NarratorClose(ActionMethod.KeyPadOnly, true, CheckType.Verification);
            }
            catch (Exception exception)
            {
                _caughtException = exception;
            }
            finally
            {
                HelperFinallyCleanup(_caughtException);
            }
        }

        #endregion 4.8 Announce Scroll Notifications

        #region 4.9 Start Narator Minimized

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("279",
           TestSummary = "Toggle \"Start Narator Minimized\" by selecting the menu itm from the \"Preferences\" menu",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            TestCaseType = TestCaseType.Scenario,
            Client = Client.ScreenReader,
            WttJob = 279,
            Description = new string[] {
                 "Step: Start Narrator",
                 "Step: Toggle the \"Start Narator Minimized\" menu",
                 "Step: Close Narrator",
                 "Step: Restart Narrator",
                 "Step: Verify current \"Start Narator Minimized\" registry setting to previous runs value set",
                 "Step: Verify current \"Start Narator Minimized\" check box setting to previous runs value set",
                 "Step: Verify the ToggleState of the \"Start Narator Minimized\" sub menu",
                 "Step: Close Narrator",
            })]
        public void Test279(TestCaseAttribute testCaseAttribute)
        {

            // Dump information
            HeaderComment(testCaseAttribute);

            try
            {
                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                TSM_MainDialogSettings(ActionMethod.Menu,
                    KEYNAME_STARTTYPE,
                    GetMenuStartNarratorMinimized,
                    GetCheckBoxStartMinimized,
                    WaitCheckBoxStartMinimizedIsChecked, WaitCheckBoxStartMinimizedIsUnChecked,
                    CheckType.Verification);

                // "Step: Close Narrator",
                TS_NarratorClose(ActionMethod.KeyPadOnly, true, CheckType.Verification);
            }
            catch (Exception exception)
            {
                _caughtException = exception;
            }
            finally
            {
                HelperFinallyCleanup(_caughtException);
            }
        }

        #endregion 4.9 Start Narator Minimized

        #region 5.0 Narrator UI

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("280",
           TestSummary = "Make sure Narrator's dialogs are modal and stay in front of the Narrator main window",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Client = Client.ScreenReader,
            WttJob = 280,
            Description = new string[] {
                "Step: Start Narrator",
                //------------------------------------------------------------
                "Step: Open the \"About\" dialog",
                "Step: Attemp to set focus to the Narrator window and expect an InvalidOperationException",
                "Step: Close the \"About\" dialog",
                "Step: Open the \"Voice Settings\" dialog",
                "Step: Attemp to set focus to the Narrator window and expect an InvalidOperationException",
                "Step: Close the \"Voice Settings\" dialog",
                "Step: Open the \"Background Message\" dialog",
                "Step: Attemp to set focus to the Narrator window and expect an InvalidOperationException",
                "Step: Close the \"Background Message\" dialog",
                "Step: Open the \"Exit\" dialog",
                "Step: Attemp to set focus to the Narrator window and expect an InvalidOperationException",
                "Step: Close the \"Exit\" dialog by pressing \"Cancel\"",
                //------------------------------------------------------------
                "Step: Close Narrator",
            })]
        public void Test280(TestCaseAttribute testCaseAttribute)
        {

            // Dump information
            HeaderComment(testCaseAttribute);

            try
            {
                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                // "Step: Open the \"About\" dialog",
                TS_DialogAboutBoxOpen(ActionMethod.Menu, CheckType.Verification);

                // "Step: Attemp to set focus to the Narrator window and expect an InvalidOperationException",
                TS_AtteptSetFocus(MainForm.AutomationElement, typeof(InvalidOperationException), CheckType.Verification);

                // "Step: Close the \"About\" dialog",
                TS_DialogAboutBoxClose(CheckType.Verification);

                // "Step: Open the \"Voice Settings\" dialog",
                TS_DialogVoiceSettingsOpen(CheckType.Verification);

                // "Step: Attemp to set focus to the Narrator window and expect an InvalidOperationException",
                TS_AtteptSetFocus(MainForm.AutomationElement, typeof(InvalidOperationException), CheckType.Verification);

                // "Step: Close the \"Voice Settings\" dialog",
                TS_DialogVoiceSettingsClose(CheckType.Verification);

                // "Step: Open the \"Background Message\" dialog",
                TS_DialogBackGroundMessageOpen(CheckType.Verification);

                // "Step: Attemp to set focus to the Narrator window and expect an InvalidOperationException",
                TS_AtteptSetFocus(MainForm.AutomationElement, typeof(InvalidOperationException), CheckType.Verification);

                // "Step: Close the \"Background Message\" dialog",
                TS_DialogBackGroundMessageClose(CheckType.Verification);

                // "Step: Open the \"Exit\" dialog",
                TS_DialogExitOpen(CheckType.Verification);

                // "Step: Attemp to set focus to the Narrator window and expect an InvalidOperationException",
                TS_AtteptSetFocus(MainForm.AutomationElement, typeof(InvalidOperationException), CheckType.Verification);

                // "Step: Close the \"Exit\" dialog by pressing \"Cancel\"",
                TS_DialogExitClose(false, CheckType.Verification);

                // "Step: Close Narrator",
                TS_NarratorClose(ActionMethod.KeyPadOnly, true, CheckType.Verification);
            }
            catch (Exception exception)
            {
                _caughtException = exception;
            }
            finally
            {
                HelperFinallyCleanup(_caughtException);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [TestCaseAttribute("281",
            TestSummary = "Open each window and dialog and verify that no duplicate shurtcut keys exist",
            Priority = TestPriorities.Pri0,
            Status = TestStatus.Works,
            Author = "Microsoft",
            Client = Client.ScreenReader,
            WttJob = 281,
            Description = new string[] {
                 "Step: Start Narrator",
                 //------------------------------------------------------------
                 "Step: Verify no duplicate AccessKeys on the Narrator dialog",
                 //------------------------------------------------------------
                 "Step: Expand the \"File\" menu",
                 "Step: Verify no duplicate AccessKeys in any of the \"File\" sub menu items",
                 "Step: Collapse the \"File\" menu",
                 //------------------------------------------------------------
                 "Step: Expand the \"Preference\" menu",
                 "Step: Verify no duplicate AccessKeys in any of the \"Preference\" sub menu items",
                 "Step: Collapse the \"Preference\" menu",
                 //------------------------------------------------------------
                 "Step: Expand the \"Help\" menu",
                 "Step: Verify no duplicate AccessKeys in any of the \"Help\" sub menu items",
                 "Step: Collapse the \"Help\" menu",
                 //------------------------------------------------------------
                 "Step: Open the \"Voice Settings\" dialog",
                 "Step: Verify no duplicate AccessKeys in any of the \"Voice Settings\" sub menu items",
                 "Step: Close the \"Voice Settings\" dialog",
                 //------------------------------------------------------------
                 "Step: Open the \"Background Message\" dialog",
                 "BUG(TBD): Verify no duplicate AccessKeys in any of the \"Background Message\" sub menu items",
                 "Step: Close the \"Background Message\" dialog",
                 "Step: Close Narrator",
            })]
        public void Test281(TestCaseAttribute testCaseAttribute)
        {

            // Dump information
            HeaderComment(testCaseAttribute);

            try
            {
                // "Step: Start Narrator",
                TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

                // "Step: Verify no duplicate AccessKeys on the Narrator dialog",
                TS_VerifyNoDuplicateAccessKeys(
                   new AutomationElement[]{
                        MainForm.ButtonExit, 
                        MainForm.ButtonQuickHelp, 
                        MainForm.ButtonVoiceSettings, 
                        MainForm.CheckBoxAnnounceScrollNotifications, 
                        MainForm.CheckBoxAnnounceSystemMessages, 
                        MainForm.CheckBoxKeyboardEcho, 
                        MainForm.CheckBoxStartMinimized,
                        MainForm.ApplicationMenuBar.FileMenu.AutomationElement,
                        MainForm.ApplicationMenuBar.PreferencesMenu.AutomationElement,
                        MainForm.ApplicationMenuBar.HelpMenu.AutomationElement,
                       MainForm.TitleBar.SystemMenuBar.System.AutomationElement,
                    }, CheckType.Verification);

                // "Step: Expand the \"File\" menu",
                TS_ExpandMenuUsingAccessKey(MainForm.ApplicationMenuBar.FileMenu.AutomationElement, CheckType.Verification);

                // "Step: Verify no duplicate AccessKeys in any of the \"File\" sub menu items",
                TS_VerifyNoDuplicateAccessKeys(
                    new AutomationElement[]
                    {
                        MainForm.ApplicationMenuBar.FileMenu.Exit
                    }, CheckType.Verification);

                // "Step: Collapse the \"File\" menu",
                TS_CollapseMenu(MainForm.ApplicationMenuBar.FileMenu.AutomationElement, CheckType.Verification);

                // "Step: Expand the \"Preference\" menu",
                TS_ExpandMenuUsingAccessKey(MainForm.ApplicationMenuBar.PreferencesMenu.AutomationElement, CheckType.Verification);

                // "Step: Verify no duplicate AccessKeys in any of the \"Preference\" sub menu items",
                TS_VerifyNoDuplicateAccessKeys(
                    new AutomationElement[]
                    {
                        MainForm.ApplicationMenuBar.PreferencesMenu.AnnounceScrollMessages,
                        MainForm.ApplicationMenuBar.PreferencesMenu.AnnounceSystemMessages,
                        MainForm.ApplicationMenuBar.PreferencesMenu.BackgroundMessageSettings,
                        MainForm.ApplicationMenuBar.PreferencesMenu.EchoUsersKeystrokes,
                        MainForm.ApplicationMenuBar.PreferencesMenu.ElementAutoMonitor,
                        MainForm.ApplicationMenuBar.PreferencesMenu.StartNarratorMinimized,
                        MainForm.ApplicationMenuBar.PreferencesMenu.VoiceSettings
                    }, CheckType.Verification);

                // "Step: Collapse the \"Preference\" menu",
                TS_CollapseMenu(MainForm.ApplicationMenuBar.PreferencesMenu.AutomationElement, CheckType.Verification);

                // "Step: Expand the \"Help\" menu",
                TS_ExpandMenuUsingAccessKey(MainForm.ApplicationMenuBar.HelpMenu.AutomationElement, CheckType.Verification);

                // "Step: Verify no duplicate AccessKeys in any of the \"Help\" sub menu items",
                TS_VerifyNoDuplicateAccessKeys(
                    new AutomationElement[]
                    {
                        MainForm.ApplicationMenuBar.HelpMenu.Documentation,
                        MainForm.ApplicationMenuBar.HelpMenu.HelpAbout
                    }, CheckType.Verification);

                // "Step: Collapse the \"Help\" menu",
                TS_CollapseMenu(MainForm.ApplicationMenuBar.HelpMenu.AutomationElement, CheckType.Verification);

                // ------------------------------------------------------------
                // "Step: Open the \"Voice Settings\" dialog",
                TS_DialogVoiceSettingsOpen(CheckType.Verification);

                // "Step: Verify no duplicate AccessKeys in any of the \"Voice Settings\" sub menu items",
                TS_VerifyNoDuplicateAccessKeys(
                    new AutomationElement[]
                    {
                        //MainForm.VoiceSettings.ButtonCancel,
                        //MainForm.VoiceSettings.ButtonOK,
                        MainForm.DialogVoiceSettings.ListVoiceSettingList,
                        MainForm.DialogVoiceSettings.ComboBoxPitchSetting,
                        MainForm.DialogVoiceSettings.ComboBoxSpeedSetting,
                        MainForm.DialogVoiceSettings.ComboBoxVolumeSetting
                    }, CheckType.Verification);

                // "Step: Close the \"Voice Settings\" dialog",
                TS_DialogVoiceSettingsClose(CheckType.Verification);

                // ------------------------------------------------------------
                // "Step: Open the \"Background Message\" dialog",
                TS_DialogBackGroundMessageOpen(CheckType.Verification);

                // "Step: Verify no duplicate AccessKeys in any of the \"Background Message\" sub menu items",
                TS_VerifyNoDuplicateAccessKeys(
                    new AutomationElement[]
                    {
                        //MainForm.BackgroundMessageDialog.Cancel, 
                        //MainForm.BackgroundMessageDialog.OK, 
                        MainForm.DialogBackgroundMessage.TimePeriodComboBox
                    }, CheckType.Verification);

                // "Step: Close the \"Background Message\" dialog",
                TS_DialogBackGroundMessageClose(CheckType.Verification);

                // "Step: Close Narrator",
                TS_NarratorClose(ActionMethod.KeyPadOnly, true, CheckType.Verification);
            }
            catch (Exception exception)
            {
                _caughtException = exception;
            }
            finally
            {
                HelperFinallyCleanup(_caughtException);
            }
        }

        #endregion Narrator UI

        #endregion Screen Reader Scenarios

        #region TS* Methods


        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_ToggleMainFormSettingsByMenu(
            ActionMethod actionMethod,
            AutomationElement menu,
            GetElement subMenu,
            WaitTestState checkBoxOnCallBack,
            WaitTestState checkBoxOffCallBack,
            CheckType checkType)
        {
            switch (actionMethod)
            {
                case ActionMethod.Menu:
                    {
                        HelperExpandMenuUsingAccessKey(menu, checkType);
                        AutomationElement subMenuItem = subMenu();
                        Debug.Assert(subMenuItem != null);
                        ToggleState ts = GetMenuToggleState(subMenuItem);
                        HelperInvokeMenuItemUsingAccessKey(subMenuItem, CheckType.Verification);


                        if (ts == ToggleState.Off)
                            Wait(checkBoxOnCallBack, _WAIT_NORMAL_MILLISECONDS, checkType);
                        else
                            Wait(checkBoxOffCallBack, _WAIT_NORMAL_MILLISECONDS, checkType);


                    }
                    break;
                default:
                    throw new ArgumentException();
            }


            m_TestStep++;
        }

        ToggleState GetMenuToggleState(AutomationElement menu)
        {
            // Win32 menus only know if they are toggles if they are checked, else they are off
            return ((bool)menu.GetCurrentPropertyValue(AutomationElement.IsTogglePatternAvailableProperty) ? ToggleState.On : ToggleState.Off);
        }


        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_DeleteRegistrySubKeyTree(CheckType checkType)
        {
            string key = @"Software\Microsoft\Narrator";
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree(key);
            }
            catch (ArgumentException)
            {
                // Key does not exists
                m_TestStep++;
                return;
            }
            // Try to open it to see if it exists
            RegistryKey keyTree = Registry.CurrentUser.OpenSubKey(key);
            if (keyTree != null)
                Comment("Could not delete key({0})", key);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_VerifyRegistryForCheckBox(AutomationElement element, string keyName, CheckType checkType)
        {
            Library.ValidateArgumentNonNull(element, "AutomationElement");
            Library.ValidateArgumentNonNull(keyName, "KeyName");

            ToggleState ts = ((ToggleState)MainForm.CheckBoxAnnounceScrollNotifications.GetCurrentPropertyValue(TogglePattern.ToggleStateProperty));

            HelperVerifyRegistryKeyNameValue(keyName, ts == ToggleState.On ? 1 : 0, checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_VerifyRegistryForListBox(AutomationElement element, string keyName, CheckType checkType)
        {
            Library.ValidateArgumentNonNull(element, "AutomationElement");
            Library.ValidateArgumentNonNull(keyName, "KeyName");

            Debug.Assert((bool)element.GetCurrentPropertyValue(AutomationElement.IsSelectionPatternAvailableProperty));

            SelectionPattern sp = element.GetCurrentPattern(SelectionPattern.Pattern) as SelectionPattern;
            AutomationElement[] elements = sp.Current.GetSelection();

            // All the lists are single select within Narrator
            Debug.Assert(elements.Length == 1);

            HelperVerifyRegistryKeyNameValue(keyName, elements[0].Current.Name, checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_VerifyRegistryKeyNameValue(string keyName, int expectedValue, CheckType checkType)
        {
            Library.ValidateArgumentNonNull(keyName, "keyName");

            HelperVerifyRegistryKeyNameValue(keyName, expectedValue, checkType);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_VerifySettingsFormWithTable(Hashtable _keyNameValuesCurrent, CheckType checkType)
        {
            bool passed = true;
            passed |= HelperVerifyRegistrySettingToControlValue(KEYNAME_ANNOUNCESCROLLNOTIFICATIONS, MainForm.CheckBoxAnnounceScrollNotifications, CheckType.Warning);
            passed |= HelperVerifyRegistrySettingToControlValue(KEYNAME_ANNOUNCESYSTEMMESSAGES, MainForm.CheckBoxAnnounceSystemMessages, CheckType.Warning);
            passed |= HelperVerifyRegistrySettingToControlValue(KEYNAME_ECHOCHARS, MainForm.CheckBoxKeyboardEcho, CheckType.Warning);
            passed |= HelperVerifyRegistrySettingToControlValue(KEYNAME_STARTTYPE, MainForm.CheckBoxStartMinimized, CheckType.Warning);

            if (false == passed)
                ThrowMe(checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>Verify the default registy settings</summary>
        /// -------------------------------------------------------------------
        void TS_VerifyDefaultRegsitryEntries(CheckType checkType)
        {
            bool passed = true;

            passed |= HelperVerifyRegistryKeyNameValue(KEYNAME_ANNOUNCESCROLLNOTIFICATIONS, _defaultAnnounceScrollNotifications, CheckType.Warning);
            passed |= HelperVerifyRegistryKeyNameValue(KEYNAME_ANNOUNCESYSTEMMESSAGES, _defaultAnnounceSystemMessages, CheckType.Warning);
            passed |= HelperVerifyRegistryKeyNameValue(KEYNAME_BACKGROUNDMESSAGETIMEOUT, _defaultBackgroundMessageTimeout, CheckType.Warning);
            passed |= HelperVerifyRegistryKeyNameValue(KEYNAME_CURRENTPITCH, _defaultCurrentPitch, CheckType.Warning);
            passed |= HelperVerifyRegistryKeyNameValue(KEYNAME_CURRENTSPEED, _defaultCurrentSpeed, CheckType.Warning);
            passed |= HelperVerifyRegistryKeyNameValue(KEYNAME_CURRENTVOLUME, _defaultCurrentVolume, CheckType.Warning);
            passed |= HelperVerifyRegistryKeyNameValue(KEYNAME_ECHOCHARS, _defaultEchoChars, CheckType.Warning);
            passed |= HelperVerifyRegistryKeyNameValue(KEYNAME_ELEMENTAUTOMONITOR, _defaultElementAutoMonitor, CheckType.Warning);
            passed |= HelperVerifyRegistryKeyNameValue(KEYNAME_STARTTYPE, _defaultStartType, CheckType.Warning);

            if (false == passed)
                ThrowMe(checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TSM_NarratorRegistrySettingsRegistryPersistency(AutomationElement element, string keyName, WaitTestState callBackOn, WaitTestState callBackOff, CheckType checkType)
        {
            Library.ValidateArgumentNonNull(element, "AutomationElement");
            switch (element.Current.ControlType.ProgrammaticName)
            {
                case "ControlType.CheckBox":
                    {
                        TS_ToggleCheckState(element, callBackOn, callBackOff, checkType);
                        TS_VerifyRegistryForCheckBox(element, keyName, CheckType.Verification);
                    }
                    break;
                case "ControlType.ComboBox":
                case "ControlType.List":
                    {
                        TS_ChangeListValueRandom(element, callBackOn, checkType);
                    }
                    break;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS3_VerifyMainFormSetting(string keyName, GetElement checkBox, AutomationElement menu, GetElement subMenu, CheckType checkType)
        {
            Library.ValidateArgumentNonNull(checkBox, "checkBox");
            Library.ValidateArgumentNonNull(menu, "menu");
            Library.ValidateArgumentNonNull(subMenu, "subMenu");

            // "Step: Verify current \"\" registry setting to previous runs value set",
            TS_VerifyRegistryKeyNameValue(keyName, _keyNameValuesCurrent[keyName], checkType);

            // "Step: Verify current \"\" check box setting to previous runs value set",
            TS_VerifyToggleState(checkBox(), keyName, checkType);

            HelperExpandMenuUsingAccessKey(menu, checkType);

            Comment("Find menu : " + subMenu.Method.Name);

            // "Step: Verify the ToggleState of the sub menu",
            TS_VerifyToggleState(subMenu(), keyName, checkType);

        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -----------------------`--------------------------------------------
        private void TSM_MainDialogSettings(
            ActionMethod actionMethod,
            string keyName,
            GetElement subMenu,
            GetElement checkBox,
            WaitTestState callbackCheckBoxIsChecked,
            WaitTestState callbackCheckBoxIsUnChecked,
            CheckType checkType)
        {

            // "Step: Toggle the \"\" menu",
            TS_ToggleMainFormSettingsByMenu(
                ActionMethod.Menu,
                MainForm.ApplicationMenuBar.PreferencesMenu.AutomationElement,
                subMenu,
                callbackCheckBoxIsChecked, callbackCheckBoxIsUnChecked,
                CheckType.Verification);

            HelperPersistKeyNameValuesToTable(_keyNameValuesCurrent);

            // "Step: Close Narrator",
            TS_NarratorClose(ActionMethod.UIAutomation, true, CheckType.Verification);

            // "Step: Restart Narrator",
            TS_NarratorStart(WindowVisualState.Normal, CheckType.Verification);

            // "Step: Verify current \"\" registry setting to previous runs value set",
            // "Step: Verify current \"\" check box setting to previous runs value set",
            // "Step: Verify the ToggleState of the sub menu",
            TS3_VerifyMainFormSetting(keyName, checkBox, MainForm.ApplicationMenuBar.PreferencesMenu.AutomationElement, subMenu, CheckType.Verification);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_ChangeListValueRandom(AutomationElement element, WaitTestState callBackForChange, CheckType checkType)
        {
            Library.ValidateArgumentNonNull(element, "element");

            AutomationElementCollection elements = element.FindAll(TreeScope.Descendants,
                new AndCondition(
                new System.Windows.Automation.Condition[]
                {
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem),
                    new PropertyCondition(SelectionItemPattern.IsSelectedProperty, false)
                }));

            if (elements.Count != 0)
            {

                Random rnd = new Random();
                AutomationElement newSelect = elements[rnd.Next(elements.Count)];

                if (newSelect == null)
                    ThrowMe(checkType, "Could not get a non selected item");

                _changedToValue = newSelect.Current.Name;

                SelectionItemPattern sip = newSelect.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                sip.Select();

                Wait(callBackForChange, _WAIT_NORMAL_MILLISECONDS, checkType);
            }
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_ToggleCheckState(AutomationElement element, WaitTestState callBackOn, WaitTestState callBackOff, CheckType checkType)
        {
            Library.ValidateArgumentNonNull(element, "AutomationElement");

            ToggleState currentToggleState;
            ToggleState newToggleState;

            // Verify that control supports ToggelPattern
            if (false == (bool)element.GetCurrentPropertyValue(AutomationElement.IsTogglePatternAvailableProperty))
                ThrowMe(checkType, "Element does not support TogglePattern");

            // Get the current ToggleState
            currentToggleState = ((ToggleState)element.GetCurrentPropertyValue(TogglePattern.ToggleStateProperty));
            Comment("Current ToggleState of " + element.Current.Name + "\" is " + currentToggleState.ToString());

            // Identify the desited ToggleState after it has been Toggled
            newToggleState = currentToggleState == ToggleState.Off ? ToggleState.On : ToggleState.Off;

            // Toggle the checkbox
            ((TogglePattern)element.GetCurrentPattern(TogglePattern.Pattern)).Toggle();

            // Wait till it actually has been checked
            if (newToggleState == ToggleState.On)
                Wait(callBackOn, _WAIT_NORMAL_MILLISECONDS, CheckType.Verification);
            else
                Wait(callBackOff, _WAIT_NORMAL_MILLISECONDS, CheckType.Verification);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>Test step: Start Narrator</summary>
        /// -------------------------------------------------------------------
        void TS_NarratorStart(WindowVisualState windowState, CheckType checkType)
        {
            TS_NarratorStart(windowState, true, checkType);
            //m_TestStep++;  let the overloaded method increment the value
        }

        /// -------------------------------------------------------------------
        /// <summary>Test step: Start Narrator</summary>
        /// -------------------------------------------------------------------
        void TS_NarratorStart(WindowVisualState windowState, bool dismissLanguageDialog, CheckType checkType)
        {
            AutomationElement element;
            string pathNarrator;
            object caption;

            HelperGetNarratorIdentifiers(out pathNarrator, out caption, checkType);

            // Set up the thread to handle the language mismatch error dialog
            NarratorErrorDialogs narratorErrorDialogs = null;

            if (dismissLanguageDialog)
            {
                narratorErrorDialogs = new NarratorErrorDialogs();
                narratorErrorDialogs.HandleVoiceLanguageWarningForm();
            }

            // Start up Narrator
            Comment("Starting {0} at \"{1}:{2}:{3}:{4}\" ...", pathNarrator, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            Comment("Seach for window with \"{0}\" title", caption.ToString());
            Library.StartApplicationShellExecute(pathNarrator, caption.ToString(), null, out element, _WAIT_STARTUP_MILLISECONDS);

            Wait(WaitForNarratorToBeVisible, _WAIT_NORMAL_MILLISECONDS, checkType);

            Wait(WaitForNarratorUserInteractiveState, _WAIT_NORMAL_MILLISECONDS, checkType);

            HelperNarratorRestoreWindow(checkType);

            if (null == MainForm.AutomationElement)
                ThrowMe(checkType, "Could not find Narrator dialog");

            m_TestStep++;

        }

        /// -------------------------------------------------------------------
        /// <summary>Test step: Start Narrator</summary>
        /// -------------------------------------------------------------------
        void TS_VerifyNarratorVisible(CheckType checkType)
        {
            // Wait for Narrator to become visible
            Wait(WaitForNarratorToBeVisible, _WAIT_STARTUP_MILLISECONDS, CheckType.Verification);

            m_TestStep++;

        }

        /// -------------------------------------------------------------------
        /// <summary>Test step: Close Narrator</summary>
        /// -------------------------------------------------------------------
        void TS_NarratorClose(ActionMethod actionMethod, bool pressYes, CheckType checkType)
        {
            HelperCloseAllDialogs();

            // "Step: Start closing Narrator, which will open the \"Do you want to exit Narrator\" dialog",
            HelperCloseNarrator(actionMethod, pressYes, checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_VerifyToggleState(AutomationElement element, string keyname, CheckType checkType)
        {
            if (null == element)
            {
                Console.WriteLine("**********************");
                Thread.Sleep(10000);
                ThrowMe(checkType, "element == null in TS_VerifyToggleState...could not get menu?");
            }
            int regvalue = (int)HelperGetKeyNameValue(keyname);
            Comment("Verify that the toggle state of \"" + element.Current.Name + "\" is {0}", regvalue == 1 ? "toggled" : "not toggled");

            bool toggleSupported = (bool)element.GetCurrentPropertyValue(AutomationElement.IsTogglePatternAvailableProperty);

            if (false == toggleSupported)
            {
                if (element.Current.ControlType == ControlType.MenuItem)
                {
                    // win32 menus only know thre are togglable when they are checked
                    if (regvalue == 1 ? toggleSupported != true : toggleSupported != false)
                        ThrowMe(checkType, "ToggleState was incorrect");
                }
                else
                {
                    Debug.Assert(false, "Should support TogglePattern");
                }
            }
            else
            {
                ToggleState toggled = (ToggleState)element.GetCurrentPropertyValue(TogglePattern.ToggleStateProperty);
                if (regvalue == 1 ? toggled != ToggleState.On : toggled != ToggleState.Off)
                    ThrowMe(checkType, "ToggleState was incorrect");

            }
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_DialogVoiceSettingsClose(CheckType checkType)
        {
            HelperPressButton(MainForm.DialogVoiceSettings.ButtonOK, ActionMethod.KeyPadOnly, CheckType.Verification);
            Wait(WaitForVoiceSettingsToBeNoVisible, _WAIT_NORMAL_MILLISECONDS, CheckType.Verification);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_DialogVoiceSettingsOpen(CheckType checkType)
        {
            HelperPressButton(MainForm.ButtonVoiceSettings, ActionMethod.KeyPadOnly, CheckType.Verification);
            Wait(WaitForVoiceSettingsToBeVisible, _WAIT_NORMAL_MILLISECONDS, CheckType.Verification);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_VerifyListValue(AutomationElement list, string keyname, CheckType checkType)
        {
            string regvalue = HelperGetKeyNameValue(keyname).ToString();
            string tranformedValue = regvalue;
            Debug.Assert((bool)list.GetCurrentPropertyValue(AutomationElement.IsSelectionPatternAvailableProperty));
            SelectionPattern sp = list.GetCurrentPattern(SelectionPattern.Pattern) as SelectionPattern;
            AutomationElement[] elements = sp.Current.GetSelection();
            Debug.Assert(elements.Length == 1);

            if (keyname == KEYNAME_BACKGROUNDMESSAGETIMEOUT)
            {
                // localization here.  Just look for the actual seconds
                switch (regvalue)
                {
                    case "5000":
                        tranformedValue = "5 ";
                        break;
                    case "10000":
                        tranformedValue = "10 ";
                        break;
                    case "15000":
                        tranformedValue = "15 ";
                        break;
                    case "30000":
                        tranformedValue = "30 ";
                        break;
                    case "60000":
                        tranformedValue = "1 ";
                        break;
                    case "300000":
                        
                        // duplicate entry, assume if they get
                        // everything right, they will get this one 
                        // correct also.  We randomly set this so should
                        // get the other checks on the other values.
                        tranformedValue = "5 ";
                        break;
                    case "600000":
                       
                        // duplicate entry, assume if they get
                        // everything right, they will get this one 
                        // correct.  We randomly set this so should
                        // get the other checks on the other values.
                        tranformedValue = "10 ";
                        break;
                }
            }

            if (0 != tranformedValue.CompareTo(elements[0].Current.Name.Substring(0, tranformedValue.Length)))
                ThrowMe(checkType);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_DialogBackGroundMessageClose(CheckType checkType)
        {
            AutomationElement closeButton = MainForm.DialogBackgroundMessage.OK;
            Debug.Assert(closeButton != null, "MainForm.BackgroundMessageDialog.OK.AutomationElement");
            Debug.Assert((bool)closeButton.GetCurrentPropertyValue(AutomationElement.IsInvokePatternAvailableProperty));
            ((InvokePattern)closeButton.GetCurrentPattern(InvokePattern.Pattern)).Invoke();

            Wait(WaitForBackgroundSettingsDialogToBeNotVisible, _WAIT_NORMAL_MILLISECONDS, checkType);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_DialogBackGroundMessageOpen(CheckType checkType)
        {
            Comment("Try to open the \"Background Message\" dialog");
            HelperExpandMenuUsingAccessKey(MainForm.ApplicationMenuBar.PreferencesMenu.AutomationElement, checkType);
            HelperInvokeMenuItemUsingAccessKey(MainForm.ApplicationMenuBar.PreferencesMenu.BackgroundMessageSettings, CheckType.Verification);
            Wait(WaitForBackgroundSettingsDialogToBeVisible, _WAIT_NORMAL_MILLISECONDS, checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_DialogAboutBoxOpen(ActionMethod actionMethod, CheckType checkType)
        {
            switch (actionMethod)
            {
                case ActionMethod.Menu:
                    {
                        Comment("Opening \"About dialog\" by clicking on the menu item");
                        HelperExpandMenuUsingAccessKey(MainForm.ApplicationMenuBar.HelpMenu.AutomationElement, checkType);
                        HelperInvokeMenuItemUsingAccessKey(MainForm.ApplicationMenuBar.HelpMenu.HelpAbout, CheckType.Verification);
                    }
                    break;
                default:
                    ThrowMe(checkType, "Invalid argument");
                    break;
            }

            Wait(WaitForAboutBoxDialogToBeVisible, _WAIT_NORMAL_MILLISECONDS, checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_DialogAboutBoxClose(CheckType checkType)
        {
            HelperPressButton(MainForm.AboutDialog.OK, ActionMethod.KeyPadOnly, checkType);
            Wait(WaitForAboutBoxDialogToBeNotVisible, _WAIT_NORMAL_MILLISECONDS, checkType);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_HelpDocumentOpen(ActionMethod actionMethod, CheckType checkType)
        {
            switch (actionMethod)
            {
                case ActionMethod.Menu:
                    {
                        Comment("Opening \"Help Document\" by clicking on the menu item");
                        HelperExpandMenuUsingAccessKey(MainForm.ApplicationMenuBar.HelpMenu.AutomationElement, checkType);
                        HelperInvokeMenuItemUsingAccessKey(MainForm.ApplicationMenuBar.HelpMenu.Documentation, CheckType.Verification);
                    }
                    break;
                case ActionMethod.F1:
                    {
                        Comment("Opening \"Help Document\" by pressing F1");
                        Input.SendKeyboardInput(System.Windows.Input.Key.F1, true);
                        Input.SendKeyboardInput(System.Windows.Input.Key.F1, false);
                    }
                    break;
                default:
                    ThrowMe(checkType, "Invalid argument");
                    break;
            }

            Wait(WaitForHelpDocumentDialogToBeVisible, _WAIT_NORMAL_MILLISECONDS, checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_HelpDocumentClose(CheckType checkType)
        {
            MainForm.HelpDocument.AutomationElement.SetFocus();
            Input.SendKeyboardInput(System.Windows.Input.Key.LeftAlt, true);
            Input.SendKeyboardInput(System.Windows.Input.Key.F4, true);
            Input.SendKeyboardInput(System.Windows.Input.Key.F4, false);
            Input.SendKeyboardInput(System.Windows.Input.Key.LeftAlt, false);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_VerifyNoDuplicateAccessKeys(AutomationElement[] elements, CheckType checkType)
        {
            string accessKey;

            // Use a hash table so we can get to the elements by key if we find a duplicate
            Hashtable table = new Hashtable();
            foreach (AutomationElement element in elements)
            {
                accessKey = element.Current.AccessKey;
                if (string.IsNullOrEmpty(accessKey))
                    ThrowMe(checkType, "AccessKey is \"\" for AutomationId({0})/ControlType({1})", element.Current.AutomationId, element.Current.ControlType.ProgrammaticName);

                accessKey = accessKey.ToLower().Replace(" ", "");
                if (true == table.ContainsKey(accessKey))
                {
                    AutomationElement other = (AutomationElement)table[accessKey];

                    ThrowMe(checkType, "Found duplicate AccessKeys for \"{0}\" with AccessKey({1}) and \"{2}\" with AccessKey({3})",
                        element.Current.Name, accessKey, other.Current.Name, other.Current.AccessKey);
                }
                Comment("Found \"{0}\" with AccessKey = \"{1}\"", element.Current.Name, accessKey);
                table.Add(accessKey, element);
            }

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_ExpandMenuUsingAccessKey(AutomationElement menu, CheckType checkType)
        {
            HelperExpandMenuUsingAccessKey(menu, checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_CollapseMenu(AutomationElement menu, CheckType checkType)
        {
            Comment("Collapsing menu(\"{0}\")", menu.Current.Name);
            if ((ExpandCollapseState.Collapsed != (ExpandCollapseState)menu.GetCurrentPropertyValue(ExpandCollapsePattern.ExpandCollapseStateProperty)))
            {
                Input.SendKeyboardInput(System.Windows.Input.Key.Escape, true);
                Input.SendKeyboardInput(System.Windows.Input.Key.Escape, false);
            }
            Wait(WaitForMenuCollapsed, _WAIT_NORMAL_MILLISECONDS, menu, checkType);

            m_TestStep++;
        }


        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_DialogExitOpen(CheckType checkType)
        {
            Comment("Closing \"" + MainForm.AutomationElement.Current.Name + "\" by pressing \"Close\" button");
            HelperPressButton(MainForm.ButtonExit, ActionMethod.KeyPadOnly, checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_DialogExitClose(bool pressYes, CheckType checkType)
        {
            HelperCloseExitDialog(ActionMethod.KeyPadOnly, pressYes, checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_AtteptSetFocus(AutomationElement element, Type type, CheckType checkType)
        {
            Comment("Attempt to set focus to the \"{0}\" window and expect the exception of type({1})", element.Current.Name, type.ToString());
            Exception caughtException = null;
            try
            {
                element.SetFocus();
            }
            catch (Exception exception)
            {
                caughtException = exception;
            }

            if (type == null && caughtException != null)
                ThrowMe(checkType, "Should not have gottent the exception({0})", caughtException.GetType().ToString());

            if (type != null && caughtException.GetType() != type)
                ThrowMe(checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void TS_NarratorExistOnDrive(CheckType checkType)
        {
            string path = HelperGetSystemWindow() + "\\System32\\" + _NARRATOR_EXE;
            if (!File.Exists(path))
                ThrowMe(checkType, "Could not find {0}", path);
            m_TestStep++;
        }



        #endregion TS* Methods

        #region Delegates

        /// -------------------------------------------------------------------
        /// <summary>We need to pass these into a common method, but they
        /// do not exists yet as the menu has not been expanded, so pass
        /// a delegate to get them in the common function</summary>
        /// -------------------------------------------------------------------
        AutomationElement GetMenuKeyBoardEcho()
        {
            return MainForm.ApplicationMenuBar.PreferencesMenu.EchoUsersKeystrokes;
        }

        /// -------------------------------------------------------------------
        /// <summary>We need to pass these into a common method, but they
        /// do not exists yet as the menu has not been expanded, so pass
        /// a delegate to get them in the common function</summary>
        /// -------------------------------------------------------------------
        AutomationElement GetMenuStartNarratorMinimized()
        {
            return MainForm.ApplicationMenuBar.PreferencesMenu.StartNarratorMinimized;
        }

        /// -------------------------------------------------------------------
        /// <summary>We need to pass these into a common method, but they
        /// do not exists yet as the menu has not been expanded, so pass
        /// a delegate to get them in the common function</summary>
        /// -------------------------------------------------------------------
        AutomationElement GetMenuAnnounceScrollMessages()
        {
            return MainForm.ApplicationMenuBar.PreferencesMenu.AnnounceScrollMessages;
        }

        /// -------------------------------------------------------------------
        /// <summary>We need to pass these into a common method, but they
        /// do not exists yet as the menu has not been expanded, so pass
        /// a delegate to get them in the common function</summary>
        /// -------------------------------------------------------------------
        AutomationElement GetMenuAnnounceSystemMessages()
        {
            return MainForm.ApplicationMenuBar.PreferencesMenu.AnnounceSystemMessages;
        }

        /// -------------------------------------------------------------------
        /// <summary>We need to pass these into a common method, but they
        /// do not exists yet as the menu has not been expanded, so pass
        /// a delegate to get them in the common function</summary>
        /// -------------------------------------------------------------------
        AutomationElement GetCheckBoxAnnounceScrollNotifications()
        {
            return MainForm.CheckBoxAnnounceScrollNotifications;
        }

        /// -------------------------------------------------------------------
        /// <summary>We need to pass these into a common method, but they
        /// do not exists yet as the menu has not been expanded, so pass
        /// a delegate to get them in the common function</summary>
        /// -------------------------------------------------------------------
        AutomationElement GetCheckBoxAnnounceSystemMessages()
        {
            return MainForm.CheckBoxAnnounceSystemMessages;
        }

        /// -------------------------------------------------------------------
        /// <summary>We need to pass these into a common method, but they
        /// do not exists yet as the menu has not been expanded, so pass
        /// a delegate to get them in the common function</summary>
        /// -------------------------------------------------------------------
        AutomationElement GetCheckBoxKeyboardEcho()
        {
            return MainForm.CheckBoxKeyboardEcho;
        }

        /// -------------------------------------------------------------------
        /// <summary>We need to pass these into a common method, but they
        /// do not exists yet as the menu has not been expanded, so pass
        /// a delegate to get them in the common function</summary>
        /// -------------------------------------------------------------------
        AutomationElement GetCheckBoxStartMinimized()
        {
            return MainForm.CheckBoxStartMinimized;
        }

        #endregion Delegates

        #region Wait Methods

        /// -------------------------------------------------------------------
        /// <summary>Common wait method for callback</summary>
        /// -------------------------------------------------------------------
        void Wait(WaitTestState callback, int milliSeconds, AutomationElement element, CheckType checkType)
        {
            s_waitElement = element;
            Wait(callback, milliSeconds, checkType);
        }

        /// -------------------------------------------------------------------
        /// <summary>Common wait method for callback</summary>
        /// -------------------------------------------------------------------
        void Wait(WaitTestState callback, int milliSeconds, CheckType checkType)
        {
            object[] attribs = callback.Method.GetCustomAttributes(false);

            Debug.Assert(attribs != null, "callback.Method.GetCustomAttributes returned null");
            Debug.Assert(attribs.Length > 0, "Incorrect attribute type WaitAttribute associated with it");
            Debug.Assert(attribs[0] is WaitAttribute, "Incorrect attribute type");

            string eventMethod = ((WaitAttribute)attribs[0]).EventToWaitFor;

            DateTime stopTime = DateTime.Now + TimeSpan.FromMilliseconds(milliSeconds);
            for (; ; )
            {
                Comment("Waiting for {0}", eventMethod);
                if (true == callback())
                    return;

                // Allow other processes to proceed
                Thread.Sleep(500);

                if (DateTime.Now > stopTime)
                    ThrowMe(checkType, "Timeout waiting (Waited '{0}' seconds)", milliSeconds / 1000);

            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Narrator main window\" to become visible")]
        bool WaitForNarratorToBeVisible()
        {
            try
            {
                return HelperIsNarratorIsVisible(MainForm.AutomationElement, true);
            }
            catch (ElementNotAvailableException)
            {
                return false;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Narrator main window\" to become user interactive")]
        bool WaitForNarratorUserInteractiveState()
        {
            if (null == MainForm.ApplicationMenuBar.AutomationElement)
                return false;

            return ((WindowInteractionState)MainForm.AutomationElement.GetCurrentPropertyValue(WindowPattern.WindowInteractionStateProperty) == WindowInteractionState.ReadyForUserInteraction);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Narrator main window's\" WindowVisualState to be Normal")]
        bool WaitForNarratorToBeNormalWindowState()
        {
            return ((WindowVisualState)MainForm.AutomationElement.GetCurrentPropertyValue(WindowPattern.WindowVisualStateProperty) == WindowVisualState.Normal);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Narrator main window \" to be closed")]
        bool WaitForNarratorToBeNotVisible()
        {
            try
            {
                return HelperIsNarratorIsVisible(MainForm.AutomationElement, false);
            }
            catch (ElementNotAvailableException)
            {
                return true;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Narrator.exe's\" process to removed from OS")]
        bool WaitForNarratorProcessToCeaseToExists()
        {
            Process[] process = Process.GetProcessesByName("narrator");
            if (null == process)
                return true;
            else
                return process.Length == 0;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Voice Settings\" window to become visible")]
        bool WaitForVoiceSettingsToBeVisible()
        {
            try
            {
                return (null != MainForm.DialogVoiceSettings.AutomationElement);
            }
            catch (ElementNotAvailableException)
            {
                return false;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Voice Settings\" window to be closed")]
        bool WaitForVoiceSettingsToBeNoVisible()
        {
            try
            {
                return (null == MainForm.DialogVoiceSettings.AutomationElement);
            }
            catch (ElementNotAvailableException)
            {
                return true;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Voice Settings\" pitch combobox has changed to specified value")]
        bool WaitComboBoxPitchSettingChanged()
        {
            return HelperWaitVerifySelection(MainForm.DialogVoiceSettings.ComboBoxPitchSetting, (string)_changedToValue);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Voice Settings\" speed combobox has changed to specified value")]
        bool WaitComboBoxSpeedSettingChanged()
        {
            return HelperWaitVerifySelection(MainForm.DialogVoiceSettings.ComboBoxSpeedSetting, (string)_changedToValue);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Voice Settings\" volume combobox has changed to specified value")]
        bool WaitComboBoxVolumeSettingChanged()
        {
            return HelperWaitVerifySelection(MainForm.DialogVoiceSettings.ComboBoxVolumeSetting, (string)_changedToValue);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Voice Settings\" voice listbox has changed to specified value")]
        bool WaitListVoiceSettingListChanged()
        {
            return HelperWaitVerifySelection(MainForm.DialogVoiceSettings.ListVoiceSettingList, (string)_changedToValue);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Background Message\" settings combobox has changed to specified value")]
        bool WaitBackMessageComboBoxVolumeSettingChanged()
        {
            return HelperWaitVerifySelection(MainForm.DialogBackgroundMessage.TimePeriodComboBox, (string)_changedToValue);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Exit dialog\" to become visible")]
        bool WaitForExitDialogToBeVisible()
        {
            try
            {
                return HelperIsNarratorIsVisible(MainForm.DialogExit.AutomationElement, true);
            }
            catch (ElementNotAvailableException)
            {
                return false;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Exit dialog\" to be closed")]
        bool WaitForExitDialogToBeNotVisible()
        {
            try
            {
                // When exit dialog is dismissed, the main for is gone also.
                return HelperIsNarratorIsVisible(MainForm.DialogExit.AutomationElement, false);
            }
            catch (ElementNotAvailableException)
            {
                return true;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Announce Scroll Notifications\" checkbox ToggleState is \"On\"")]
        bool WaitCheckBoxAnnounceScrollNotificationsIsChecked()
        {
            return HelperCheckBoxCurrentToggleState(MainForm.CheckBoxAnnounceScrollNotifications, ToggleState.On);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Announce Scroll Notifications\" checkbox ToggleState is \"Off\"")]
        bool WaitCheckBoxAnnounceScrollNotificationsIsUnChecked()
        {
            return HelperCheckBoxCurrentToggleState(MainForm.CheckBoxAnnounceScrollNotifications, ToggleState.Off);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Echo Usser's Keystrokes\" checkbox ToggleState is \"On\"")]
        bool WaitCheckBoxKeyboardEchoIsChecked()
        {
            return HelperCheckBoxCurrentToggleState(MainForm.CheckBoxKeyboardEcho, ToggleState.On);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Echo Usser's Keystrokes\" checkbox ToggleState is \"Off\"")]
        bool WaitCheckBoxKeyboardEchoIsUnChecked()
        {
            return HelperCheckBoxCurrentToggleState(MainForm.CheckBoxKeyboardEcho, ToggleState.Off);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Announce System Messages\" checkbox ToggleState is \"On\"")]
        bool WaitCheckBoxAnnounceSystemMessagesIsChecked()
        {
            return HelperCheckBoxCurrentToggleState(MainForm.CheckBoxAnnounceSystemMessages, ToggleState.On);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Announce System Messages\" checkbox ToggleState is \"Off\"")]
        bool WaitCheckBoxAnnounceSystemMessagesIsUnChecked()
        {
            return HelperCheckBoxCurrentToggleState(MainForm.CheckBoxAnnounceSystemMessages, ToggleState.Off);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Start Narrator Minimized\" checkbox ToggleState is \"On\"")]
        bool WaitCheckBoxStartMinimizedIsChecked()
        {
            return HelperCheckBoxCurrentToggleState(MainForm.CheckBoxStartMinimized, ToggleState.On);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Start Narrator Minimized\" checkbox ToggleState is \"Off\"")]
        bool WaitCheckBoxStartMinimizedIsUnChecked()
        {
            return HelperCheckBoxCurrentToggleState(MainForm.CheckBoxStartMinimized, ToggleState.Off);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Background Settings dialog\" to become visible")]
        bool WaitForBackgroundSettingsDialogToBeVisible()
        {
            try
            {
                return (MainForm.DialogBackgroundMessage.AutomationElement != null);
            }
            catch (ElementNotAvailableException)
            {
                return false;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Background Settings dialog\" to become not visible")]
        bool WaitForBackgroundSettingsDialogToBeNotVisible()
        {
            try
            {
                return (MainForm.DialogBackgroundMessage.AutomationElement == null);
            }
            catch (ElementNotAvailableException)
            {
                return true;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"About dialog\" to become visible")]
        bool WaitForAboutBoxDialogToBeVisible()
        {
            try
            {
                return (MainForm.AboutDialog.AutomationElement != null);
            }
            catch (ElementNotAvailableException)
            {
                return false;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Help Document\" to become visible")]
        bool WaitForHelpDocumentDialogToBeVisible()
        {
            try
            {
                return (MainForm.HelpDocument.AutomationElement != null);
            }
            catch (ElementNotAvailableException)
            {
                return false;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"Help Document\" to become not visible")]
        bool WaitForHelpDocumentDialogToBeNotVisible()
        {
            try
            {
                return (MainForm.HelpDocument.AutomationElement == null);
            }
            catch (ElementNotAvailableException)
            {
                return true;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("\"About dialog\" to become not visible")]
        bool WaitForAboutBoxDialogToBeNotVisible()
        {
            try
            {
                return (MainForm.AboutDialog.AutomationElement == null);
            }
            catch (ElementNotAvailableException)
            {
                return true;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("Wait till menu is closed")]
        bool WaitForMenuClosed()
        {
            try
            {
                return (s_waitElement.Current.BoundingRectangle == Rect.Empty);
            }
            catch (ElementNotAvailableException)
            {
                return true;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("Wait till menu is closed")]
        bool WaitForMenuCollapsed()
        {
            try
            {
                ExpandCollapsePattern ecp = s_waitElement.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                Debug.Assert(null != ecp);
                return ecp.Current.ExpandCollapseState == ExpandCollapseState.Collapsed;
            }
            catch (ElementNotAvailableException)
            {
                return true;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        [WaitAttribute("Wait till menu is opened")]
        bool WaitForMenuOpened()
        {
            return (ExpandCollapseState.Expanded == (ExpandCollapseState)s_waitElement.GetCurrentPropertyValue(ExpandCollapsePattern.ExpandCollapseStateProperty));
        }

        #endregion Wait Methods

        #region Test Steps

        /// -------------------------------------------------------------------
        /// <summary>Helper: Press button</summary>
        /// -------------------------------------------------------------------
        void HelperPressButton(AutomationElement element, ActionMethod actionBY, CheckType checkType)
        {
            Library.ValidateArgumentNonNull(element, "Button AutomationElement");

            string name = element.Current.Name;

            switch (actionBY)
            {
                case ActionMethod.KeyPadOnly:
                    {
                        string buffer = element.Current.AccessKey;
                        if (!string.IsNullOrEmpty(buffer))
                        {
                            // There is an access key so use it
                            Comment("Pressing keyboard access key \"" + element.Current.AccessKey + "\"");
                            Input.SendKeyboardInput(element.Current.AccessKey);
                        }
                        else
                        {
                            // No access key so set focus and press enter
                            Comment("Setting focus to \"{0}\" and pressing ENTER", element.Current.Name);
                            element.SetFocus();
                            Input.SendKeyboardInput(System.Windows.Input.Key.Enter);
                        }
                    }
                    break;
                case ActionMethod.MouseClick:
                    {
                        Comment("Moving mouse to \"" + element.Current.Name + "\" and left clicking");
                        Input.MoveToAndClick(element);
                    }
                    break;
                default:
                    {
                        if (!(bool)element.GetCurrentPropertyValue(AutomationElement.IsInvokePatternAvailableProperty))
                            ThrowMe(checkType, "Button \"" + name + "\" does not support Invoke pattern");

                        Comment("Calling InvokePattern.Invoke() on \"" + name + "\"");
                        ((InvokePattern)element.GetCurrentPattern(InvokePattern.Pattern)).Invoke();
                    }
                    break;
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>Test step: Close Exit dialog</summary>
        /// -------------------------------------------------------------------
        private void HelperCloseExitDialog(ActionMethod actionMethod, bool pressYes, CheckType checkType)
        {
            if (pressYes)
            {
                // Step: Press the Yes button
                HelperPressButton(MainForm.DialogExit.ButtonYes, actionMethod, CheckType.Verification);
            }
            else
            {
                // Step: Press the No button
                HelperPressButton(MainForm.DialogExit.ButtonNo, actionMethod, CheckType.Verification);
            }

            // Wait for the dialog to go away
            Wait(WaitForExitDialogToBeNotVisible, _WAIT_NORMAL_MILLISECONDS, checkType);

        }

        /// -------------------------------------------------------------------
        /// <summary>Test step: Close the application by calling WindowPattern.Close()</summary>
        /// -------------------------------------------------------------------
        void HelperCloseNarrator(ActionMethod actionMethod, bool pressYes, CheckType checkType)
        {
            AutomationElement narrator = MainForm.AutomationElement;

            if (null == narrator)
                return;

            switch (actionMethod)
            {
                case ActionMethod.UIAutomation:
                    {
                        Comment("Closing \"" + narrator.Current.Name + "\" window using WindowPattern.Close()");
                        if (!(bool)narrator.GetCurrentPropertyValue(AutomationElement.IsWindowPatternAvailableProperty))
                            ThrowMe(checkType, "\"" + narrator.Current.Name + "\" window does not support WindowPatternAvailableProperty and should");

                        ((WindowPattern)narrator.GetCurrentPattern(WindowPattern.Pattern)).Close();
                    }
                    break;
                case ActionMethod.KeyPadOnly:
                    {
                        Comment("Closing \"" + narrator.Current.Name + "\" by pressing \"Close\" button");
                        HelperPressButton(MainForm.ButtonExit, ActionMethod.KeyPadOnly, checkType);
                    }
                    break;
                case ActionMethod.CloseXButton:
                    {
                        Comment("Closing \"" + narrator.Current.Name + "\" by pressing \"Alt+F4\" button");
                        HelperPressButton(MainForm.TitleBar.ButtonClose, ActionMethod.MouseClick, checkType);
                    }
                    break;
                case ActionMethod.KillProcess:
                    {
                        Comment("Closing \"" + narrator.Current.Name + "\" by call Process.Kill()");
                        try
                        {
                            Process.GetProcessesByName("Narrator")[0].Kill();
                        }
                        catch (IndexOutOfRangeException)
                        {
                            // There is no process with that name so referencing [0] is invalid
                        }
                    }
                    break;
                case ActionMethod.AltF4:
                    {
                        Comment("Closing \"" + narrator.Current.Name + "\" by pressing \"Alt+F4\" button");
                        Input.SendKeyboardInput(System.Windows.Input.Key.LeftAlt, true);
                        Input.SendKeyboardInput(System.Windows.Input.Key.F4, true);
                        Input.SendKeyboardInput(System.Windows.Input.Key.F4, false);
                        Input.SendKeyboardInput(System.Windows.Input.Key.LeftAlt, false);
                    }
                    break;

                default:
                    ThrowMe(CheckType.Verification, "Incorrect argument");
                    break;
            }


            if (actionMethod != ActionMethod.KillProcess)
            {
                Wait(WaitForExitDialogToBeVisible, _WAIT_NORMAL_MILLISECONDS, checkType);
                HelperCloseExitDialog(ActionMethod.KeyPadOnly, pressYes, checkType);
            }

            // If we closed it, then wait till everything is closed down
            if (pressYes || actionMethod == ActionMethod.KillProcess)
            {
                Wait(WaitForNarratorToBeNotVisible, _WAIT_NORMAL_MILLISECONDS, checkType);
                Wait(WaitForNarratorProcessToCeaseToExists, _WAIT_STARTUP_MILLISECONDS, checkType);
            }
            else
            {
                // nop, we cancelled the dialog
            }

        }


        #endregion Test Steps

        #region Helper

        #region Member variables

        const string KEY_NARRATOR = @"HKEY_CURRENT_USER\Software\Microsoft\Narrator";

        const string KEYNAME_ANNOUNCESCROLLNOTIFICATIONS = "AnnounceScrollNotifications";
        const string KEYNAME_ANNOUNCESYSTEMMESSAGES = "AnnounceSystemMessages";
        const string KEYNAME_BACKGROUNDMESSAGETIMEOUT = "BackgroundMessageTimeout";
        const string KEYNAME_CURRENTPITCH = "CurrentPitch";
        const string KEYNAME_CURRENTSPEED = "CurrentSpeed";
        const string KEYNAME_CURRENTVOICE = "CurrentVoice";
        const string KEYNAME_CURRENTVOLUME = "CurrentVolume";
        const string KEYNAME_ECHOCHARS = "EchoChars";
        const string KEYNAME_ELEMENTAUTOMONITOR = "ElementAutoMonitor";
        const string KEYNAME_STARTTYPE = "StartType";

        Hashtable _keyNameValuesOriginal = new Hashtable();
        Hashtable _keyNameValuesCurrent = new Hashtable();
        Hashtable _keyNameValuesDefault = new Hashtable();

        int _defaultAnnounceScrollNotifications = 0;
        int _defaultAnnounceSystemMessages = 1;
        int _defaultBackgroundMessageTimeout = 30000;
        int _defaultCurrentPitch = 5;
        int _defaultCurrentSpeed = 5;
        int _defaultCurrentVolume = 9;
        int _defaultEchoChars = 1;
        int _defaultElementAutoMonitor = 1;
        int _defaultStartType = 0;

        #endregion Member variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void HelperPersistKeyNameValuesToTable(Hashtable table)
        {
            table.Clear();
            table.Add(KEYNAME_ANNOUNCESCROLLNOTIFICATIONS, HelperGetKeyNameValue(KEYNAME_ANNOUNCESCROLLNOTIFICATIONS));
            table.Add(KEYNAME_ANNOUNCESYSTEMMESSAGES, HelperGetKeyNameValue(KEYNAME_ANNOUNCESYSTEMMESSAGES));
            table.Add(KEYNAME_BACKGROUNDMESSAGETIMEOUT, HelperGetKeyNameValue(KEYNAME_BACKGROUNDMESSAGETIMEOUT));
            table.Add(KEYNAME_CURRENTPITCH, HelperGetKeyNameValue(KEYNAME_CURRENTPITCH));
            table.Add(KEYNAME_CURRENTSPEED, HelperGetKeyNameValue(KEYNAME_CURRENTSPEED));
            table.Add(KEYNAME_CURRENTVOICE, HelperGetKeyNameValue(KEYNAME_CURRENTVOICE));
            table.Add(KEYNAME_CURRENTVOLUME, HelperGetKeyNameValue(KEYNAME_CURRENTVOLUME));
            table.Add(KEYNAME_ECHOCHARS, HelperGetKeyNameValue(KEYNAME_ECHOCHARS));
            table.Add(KEYNAME_ELEMENTAUTOMONITOR, HelperGetKeyNameValue(KEYNAME_ELEMENTAUTOMONITOR));
            table.Add(KEYNAME_STARTTYPE, HelperGetKeyNameValue(KEYNAME_STARTTYPE));
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void HelperSetKeyNameValuesFromTable(Hashtable table)
        {
            if (table.Count == 0 && table == _keyNameValuesDefault)
            {
                // This is the default table that has not been set yet
                Registry.SetValue(KEY_NARRATOR, KEYNAME_ANNOUNCESCROLLNOTIFICATIONS, _defaultAnnounceScrollNotifications);
                Registry.SetValue(KEY_NARRATOR, KEYNAME_ANNOUNCESYSTEMMESSAGES, _defaultAnnounceSystemMessages);
                Registry.SetValue(KEY_NARRATOR, KEYNAME_BACKGROUNDMESSAGETIMEOUT, _defaultBackgroundMessageTimeout);
                Registry.SetValue(KEY_NARRATOR, KEYNAME_CURRENTPITCH, _defaultCurrentPitch);
                Registry.SetValue(KEY_NARRATOR, KEYNAME_CURRENTSPEED, _defaultCurrentSpeed);
                Registry.SetValue(KEY_NARRATOR, KEYNAME_CURRENTVOLUME, _defaultCurrentVolume);
                Registry.SetValue(KEY_NARRATOR, KEYNAME_ECHOCHARS, _defaultEchoChars);
                Registry.SetValue(KEY_NARRATOR, KEYNAME_ELEMENTAUTOMONITOR, _defaultElementAutoMonitor);
                Registry.SetValue(KEY_NARRATOR, KEYNAME_STARTTYPE, _defaultStartType);
            }
            else
            {
                HelperRegistrySetValue(table, KEY_NARRATOR, KEYNAME_ANNOUNCESCROLLNOTIFICATIONS);
                HelperRegistrySetValue(table, KEY_NARRATOR, KEYNAME_ANNOUNCESYSTEMMESSAGES);
                HelperRegistrySetValue(table, KEY_NARRATOR, KEYNAME_BACKGROUNDMESSAGETIMEOUT);
                HelperRegistrySetValue(table, KEY_NARRATOR, KEYNAME_CURRENTPITCH);
                HelperRegistrySetValue(table, KEY_NARRATOR, KEYNAME_CURRENTSPEED);
                HelperRegistrySetValue(table, KEY_NARRATOR, KEYNAME_CURRENTVOLUME);
                HelperRegistrySetValue(table, KEY_NARRATOR, KEYNAME_ECHOCHARS);
                HelperRegistrySetValue(table, KEY_NARRATOR, KEYNAME_ELEMENTAUTOMONITOR);
                HelperRegistrySetValue(table, KEY_NARRATOR, KEYNAME_STARTTYPE);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void TS_VerifyRegistryKeyNameValue(string keyName, object expectedValue, CheckType checkType)
        {
            HelperVerifyRegistryKeyNameValue(keyName, expectedValue, checkType);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        bool HelperVerifyRegistryKeyNameValue(string keyName, object expectedValue, CheckType checkType)
        {
            bool retvalue = true;
            const string badValue = "-99";

            Comment("Verifying that {0}\\{1} is \"{2}\"", KEY_NARRATOR, keyName, expectedValue);

            object curRegObj = Registry.GetValue(KEY_NARRATOR, keyName, badValue);

            if (null == curRegObj)
            {
                Comment("Registry value for {0}\\{1} is null.", KEY_NARRATOR, keyName);
                retvalue = false;
            }
            else
            {
                string curRegValue = curRegObj.ToString();

                if (0 == badValue.CompareTo(curRegValue))
                {
                    Comment("Could not get the registry value from {0}\\{1}", KEY_NARRATOR, keyName);
                    retvalue = false;
                }

                if (0 != curRegValue.CompareTo(expectedValue.ToString()))
                {
                    Comment("Registry is not set correctly for {0}\\{1}.  Current registry value is {2} and expected {3}.", KEY_NARRATOR, keyName, curRegValue, expectedValue);
                    retvalue = false;
                }
            }
            return retvalue;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        bool HelperVerifyRegistrySettingToControlValue(string key, AutomationElement element, CheckType checkType)
        {
            int regValue = (int)HelperGetKeyNameValue(key);
            switch (element.Current.ControlType.ProgrammaticName)
            {
                case "ControlType.CheckBox":
                    {
                        Debug.Assert((bool)element.GetCurrentPropertyValue(AutomationElement.IsTogglePatternAvailableProperty));
                        ToggleState ts = (ToggleState)element.GetCurrentPropertyValue(TogglePattern.ToggleStateProperty);

                        Comment("Verify that \"{0}\".ToggleState is {1}", element.Current.Name, ts.ToString());
                        return regValue == 1 ? ts == ToggleState.On : ts == ToggleState.Off;
                    }
                default:
                    ThrowMe(CheckType.Verification, "Incorrect argument");
                    break;
            }
            return false;
        }

        /// -------------------------------------------------------------------
        /// <summary>Used by Wait*Changed methods</summary>
        /// -------------------------------------------------------------------
        bool HelperWaitVerifySelection(AutomationElement element, string expecetdValue)
        {
            Library.ValidateArgumentNonNull(element, "element");

            Debug.Assert((bool)element.GetCurrentPropertyValue(AutomationElement.IsSelectionPatternAvailableProperty));
            AutomationElement[] selection = (AutomationElement[])element.GetCurrentPropertyValue(SelectionPattern.SelectionProperty);
            if (selection.Length == 0)
                return false;
            return (selection[0].Current.Name == expecetdValue);
        }

        ///-------------------------------------------------------------------
        ///<summary></summary>
        ///-------------------------------------------------------------------
        void HelperNarratorRestoreWindow(CheckType checkType)
        {
            AutomationElement narrator = MainForm.AutomationElement;
            Debug.Assert((bool)narrator.GetCurrentPropertyValue(AutomationElement.IsWindowPatternAvailableProperty));
            ((WindowPattern)narrator.GetCurrentPattern(WindowPattern.Pattern)).SetWindowVisualState(WindowVisualState.Normal);
            Wait(WaitForNarratorToBeNormalWindowState, _WAIT_NORMAL_MILLISECONDS, checkType);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        object HelperGetKeyNameValue(string keyName)
        {
            object obj = null;
            object defaultValue = -1;

            if (KEYNAME_CURRENTVOICE == keyName)
                defaultValue = "XYZ";

            // For some reason I am getting a null reference here and can't seem to pinpoint 
            // what could be failing.  The defaultValue may not be returned as expected if the
            // key does not exists and may be returning null so when we do a ToString() on it
            // it is throwing an exception...do over kill on commenting so we can pinpoint it down.
            // Can't duplicate it on my machine.

            obj = Registry.GetValue(KEY_NARRATOR, keyName, defaultValue);
            Comment("Obtained value for key({0}) with value of ({1})", keyName, obj);

            if (null == obj)
            {
                Comment("WARNING: Registry.GetValue returned null instead of the default value supplied({0})", defaultValue);
                obj = string.Empty;
            }
            else
            {
                if (obj == defaultValue)
                    Comment("WARNING: Could not get the value of {0}\\{1}, but returning the default value({2})", KEY_NARRATOR, keyName, defaultValue);
            }
            return obj;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        string HelperGetSystemWindow()
        {
            return (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "SystemRoot", "");
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        bool HelperIsNarratorIsVisible(AutomationElement element, bool state)
        {
            if (element == null)
                return false == state;

            IntPtr ptr = new IntPtr(element.Current.NativeWindowHandle);
            if (ptr == IntPtr.Zero)
                return false == state;

            return (state == SafeNativeMethods.IsWindowVisible(NativeMethods.HWND.Cast(ptr)));
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        bool HelperCheckBoxCurrentToggleState(AutomationElement element, ToggleState state)
        {
            Library.ValidateArgumentNonNull(element, "AutomationElement");
            Debug.Assert((bool)element.GetCurrentPropertyValue(AutomationElement.IsTogglePatternAvailableProperty));
            return ((ToggleState)element.GetCurrentPropertyValue(TogglePattern.ToggleStateProperty) == state);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void HelperExpandMenuUsingAccessKey(AutomationElement menu, CheckType checkType)
        {
            Comment("Expanding menu item \"{0}\" by pressing keys \"{1}\"", menu.Current.Name, menu.Current.AccessKey);
            Input.SendKeyboardInput(menu.Current.AccessKey);
            Wait(WaitForMenuOpened, _WAIT_NORMAL_MILLISECONDS, menu, checkType);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void HelperInvokeMenuItemUsingAccessKey(AutomationElement menuItem, CheckType checkType)
        {
            Debug.Assert(menuItem.Current.ControlType == ControlType.MenuItem);

            string str = menuItem.Current.AccessKey.Replace(" ", "");
            str = str.Substring(str.IndexOf("+") + 1);

            char c = Convert.ToChar(str);

            Input.SendUnicodeKeyboardInput(c, true);
            Input.SendUnicodeKeyboardInput(c, false);

            Wait(WaitForMenuClosed, _WAIT_NORMAL_MILLISECONDS, menuItem, checkType);

        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        void HelperCloseAllDialogs()
        {
            if (null != MainForm.DialogExit.AutomationElement)
                ((InvokePattern)MainForm.DialogExit.ButtonNo.GetCurrentPattern(InvokePattern.Pattern)).Invoke();

            if (null != MainForm.DialogBackgroundMessage.AutomationElement)
                ((InvokePattern)MainForm.DialogBackgroundMessage.Cancel.GetCurrentPattern(InvokePattern.Pattern)).Invoke();

            if (null != MainForm.DialogVoiceSettings.AutomationElement)
                ((InvokePattern)MainForm.DialogVoiceSettings.ButtonCancel.GetCurrentPattern(InvokePattern.Pattern)).Invoke();
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void HelperFinallyCleanup(Exception exception)
        {
            HelperCloseNarrator(ActionMethod.KillProcess, true, CheckType.Warning);

            // Set Narrator back to original state
            HelperSetKeyNameValuesFromTable(_keyNameValuesOriginal);
            if (exception != null)
                throw new Exception("RETHROW", exception);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private static void HelperRegistrySetValue(Hashtable table, string key, string name)
        {
            if (table.ContainsKey(name))
                Registry.SetValue(key, name, table[name]);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        private void HelperGetNarratorIdentifiers(out string pathNarrator, out object caption, CheckType checkType)
        {
            string isoName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            const string narratorMainForm = "Microsoft.Narrator.NarratorMainForm";
            string system32 = HelperGetSystemWindow() + "\\System32\\";
            string baseName = string.Empty;
            string resourcePath = system32 + isoName + "\\" + _NARRATOR_RESOURCE_DLL;
            pathNarrator = system32 + _NARRATOR_EXE;
            string captionId = "$this.Text";

            // Try to get the localized title from the resource dll , if this doesn't work, then 
            // use the ddl that might be in the normal system32 directory, if this isn't 
            // there, then just get it from the exe
            if (File.Exists(system32 + isoName + "\\" + _NARRATOR_RESOURCE_DLL))    //default to the dll in localized directory
            {
                resourcePath = system32 + isoName + "\\" + _NARRATOR_RESOURCE_DLL;
                baseName = narratorMainForm + "." + isoName;
            }
            else if (File.Exists(system32 + _NARRATOR_RESOURCE_DLL))                // default to dll in normal directory
            {
                resourcePath = system32 + _NARRATOR_RESOURCE_DLL;
                baseName = narratorMainForm;
            }
            else if (File.Exists(system32 + _NARRATOR_EXE))                         //default to the exe
            {
                resourcePath = system32 + _NARRATOR_EXE;
                baseName = narratorMainForm;
            }

            Comment("Reading resource file ({0}) for basename ({1}) and caption ({2})", resourcePath, baseName, captionId);
            Assembly assembly = Assembly.LoadFrom(resourcePath);
            ResourceManager rm = new ResourceManager(baseName, assembly);
            caption = rm.GetObject(captionId);

            // This will only work if we are elevated
            if (null == caption)
                ThrowMe(checkType, "Could not get caption from Narrator resources");

        }

        #endregion Helpers

    }

    /// -----------------------------------------------------------------------
    /// <summary>Common custom attribute that allows the calling method to know 
    /// what the method is doing</summary>
    /// -----------------------------------------------------------------------
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Struct, AllowMultiple = true)]
    public sealed class WaitAttribute : Attribute
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        string _eventToWaitFor;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public string EventToWaitFor
        {
            get { return _eventToWaitFor; }
            set { _eventToWaitFor = value; }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public WaitAttribute(string eventToWaitFor)
        {
            _eventToWaitFor = eventToWaitFor;
        }
    }

    /// -----------------------------------------------------------------------
    /// <summary>Narrator object model</summary>
    /// -----------------------------------------------------------------------
    class MainForm
    {
        // --------------------------------------------------------------------
        /// <summary>Common helper that gets the AutomationElement based on the 
        /// AutomationId and the TreeScope</summary>
        // --------------------------------------------------------------------
        static AutomationElement GetElement(AutomationElement element, TreeScope scope, ControlType controlType, string automationId)
        {
            if (null == element)
                throw new ElementNotAvailableException("\"element\" is null and does not exist");

            return element.FindFirst(scope,
                new AndCondition(new System.Windows.Automation.Condition[]
                {
                    new PropertyCondition(AutomationElement.AutomationIdProperty, automationId),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, controlType)
                }));
        }
        // --------------------------------------------------------------------
        /// <summary>Gets an element based on the criteria</summary>
        // --------------------------------------------------------------------
        private static AutomationElement GetElement(AutomationElement element, TreeScope treeScope, ControlType controlType, string automationId, bool list)
        {
            if (list)
                List(MainForm.AutomationElement, "+");

            return GetElement(element, treeScope, controlType, automationId);
        }
        // --------------------------------------------------------------------
        /// <summary>Debug method</summary>
        // --------------------------------------------------------------------
        private static void List(AutomationElement element, string indent)
        {
            if (element == null)
                return;
            Console.WriteLine(indent + "ControlType({0}), AutomationId({1})", element.Current.ControlType.ProgrammaticName, element.Current.AutomationId);
            List(TreeWalker.ControlViewWalker.GetFirstChild(element), indent + "+");
            if (!Automation.Compare(element, MainForm.AutomationElement))
                List(TreeWalker.ControlViewWalker.GetNextSibling(element), indent);
        }
        // --------------------------------------------------------------------
        // Control         : Window
        // Name            : Microsoft Narrator
        // Parent          :
        // Parent Control  : Pane
        // --------------------------------------------------------------------
        public static AutomationElement AutomationElement
        {
            get
            {
                AutomationElement element = GetElement(AutomationElement.RootElement, TreeScope.Children, ControlType.Window, "NarratorMainForm");
                Trace.WriteLine("null == element " + (null == element ? "T" : "F"));
                return element;
            }
        }
        // --------------------------------------------------------------------
        // Control         : Pane
        // Name            :
        // Parent          : Microsoft Narrator
        // Parent Control  : Window
        // --------------------------------------------------------------------
        public static AutomationElement PanelSettings { get { return GetElement(MainForm.AutomationElement, TreeScope.Subtree, ControlType.Custom, "settingsPanel"); } }
        //// --------------------------------------------------------------------
        //// Control         : Text
        //// Name            : Narrator will read aloud what is on-screen as you navigate using the keyboard.
        //// Parent          :
        //// Parent Control  : Pane
        //public static AutomationElement LabelNarratorDescription { get { return GetElement(_mainForm, TreeScope.Subtree, ControlType.Text, "narratorDescriptionLabel"); } }
        //// --------------------------------------------------------------------
        //// Control         : Group
        //// Name            : Main Narrator Settings
        //// Parent          :
        //// Parent Control  : Pane
        //public static AutomationElement GroupBoxSettings { get { return GetElement(_mainForm, TreeScope.Subtree, "settingsGroupBox"); } }
        // --------------------------------------------------------------------
        // Control         : CheckBox
        // Name            : Echo User's Keystrokes
        // Parent          : Main Narrator Settings
        // Parent Control  : Group
        // --------------------------------------------------------------------
        public static AutomationElement CheckBoxKeyboardEcho { get { return GetElement(MainForm.AutomationElement, TreeScope.Subtree, ControlType.CheckBox, "KeyboardEchoCheckBox"); } }
        // --------------------------------------------------------------------
        // Control         : CheckBox
        // Name            : Announce System Messages
        // Parent          : Main Narrator Settings
        // Parent Control  : Group
        // --------------------------------------------------------------------
        public static AutomationElement CheckBoxAnnounceSystemMessages { get { return GetElement(MainForm.AutomationElement, TreeScope.Subtree, ControlType.CheckBox, "AnnounceSystemMessagesCheckBox"); } }
        // --------------------------------------------------------------------
        // Control         : CheckBox
        // Name            : Announce Scroll Notifications
        // Parent          : Main Narrator Settings
        // Parent Control  : Group
        // --------------------------------------------------------------------
        public static AutomationElement CheckBoxAnnounceScrollNotifications { get { return GetElement(MainForm.AutomationElement, TreeScope.Subtree, ControlType.CheckBox, "AnnounceScrollNotificationsCheckBox"); } }
        // --------------------------------------------------------------------
        // Control         : CheckBox
        // Name            : Start Narrator Minimized
        // Parent          : Main Narrator Settings
        // Parent Control  : Group
        // --------------------------------------------------------------------
        public static AutomationElement CheckBoxStartMinimized { get { return GetElement(MainForm.AutomationElement, TreeScope.Subtree, ControlType.CheckBox, "startMinimizedCheckBox"); } }
        //// --------------------------------------------------------------------
        //// Control         : Pane
        //// Name            :
        //// Parent          : Microsoft Narrator
        //// Parent Control  : Window
        //public static AutomationElement ButtonsPanel { get { return GetElement(_mainForm, TreeScope.Subtree, "buttonsPanel"); } }
        // --------------------------------------------------------------------
        // Control         : Button
        // Name            : Quick Help
        // Parent          :
        // Parent Control  : Pane
        // --------------------------------------------------------------------
        public static AutomationElement ButtonQuickHelp { get { return GetElement(MainForm.AutomationElement, TreeScope.Subtree, ControlType.Button, "QuickHelpButton"); } }
        // --------------------------------------------------------------------
        // Control         : Button
        // Name            : Voice Settings
        // Parent          :
        // Parent Control  : Pane
        // --------------------------------------------------------------------
        public static AutomationElement ButtonVoiceSettings { get { return GetElement(MainForm.AutomationElement, TreeScope.Subtree, ControlType.Button, "voiceSettingsButton"); } }
        // --------------------------------------------------------------------
        // Control         : Button
        // Name            : Exit
        // Parent          :
        // Parent Control  : Pane
        // --------------------------------------------------------------------
        public static AutomationElement ButtonExit { get { return GetElement(MainForm.AutomationElement, TreeScope.Subtree, ControlType.Button, "ExitButton"); } }
        public class TitleBar
        {
            // ----------------------------------------------------------------
            // Control         : TitleBar
            // Name            : Microsoft Narrator
            // Parent          : Microsoft Narrator
            // Parent Control  : Window
            // ----------------------------------------------------------------
            public static AutomationElement AutomationElement { get { return GetElement(MainForm.AutomationElement, TreeScope.Children, ControlType.TitleBar, "TitleBar"); } }
            public class SystemMenuBar
            {
                // ------------------------------------------------------------
                // Control         : MenuBar
                // Name            : System Menu Bar
                // Parent          : Microsoft Narrator
                // Parent Control  : TitleBar
                // ------------------------------------------------------------
                public static AutomationElement AutomationElement { get { return GetElement(MainForm.TitleBar.AutomationElement, TreeScope.Subtree, ControlType.MenuBar, "NarratorMainForm"); } }
                // ------------------------------------------------------------
                // Control         : Menu Item
                // Name            : System 
                // Parent          : System Menu Bar
                // Parent Control  : MenuBar
                // ------------------------------------------------------------
                public class System { public static AutomationElement AutomationElement { get { return GetElement(MainForm.TitleBar.SystemMenuBar.AutomationElement, TreeScope.Children, ControlType.MenuItem, "Item 1"); } } }
            }
            // ----------------------------------------------------------------
            // Control         : Button
            // Name            : Minimize
            // Parent          : Microsoft Narrator
            // Parent Control  : TitleBar
            // ----------------------------------------------------------------
            public static AutomationElement ButtonMinimize { get { return GetElement(AutomationElement, TreeScope.Subtree, ControlType.Button, "Minimize"); } }
            // ----------------------------------------------------------------
            // Control         : Button
            // Name            : Maximize
            // Parent          : Microsoft Narrator
            // Parent Control  : TitleBar
            // ----------------------------------------------------------------
            public static AutomationElement ButtonMaximize { get { return GetElement(AutomationElement, TreeScope.Subtree, ControlType.Button, "Maximize"); } }
            // ----------------------------------------------------------------
            // Control         : Button
            // Name            : Close
            // Parent          : Microsoft Narrator
            // Parent Control  : TitleBar
            // ----------------------------------------------------------------
            public static AutomationElement ButtonClose { get { return GetElement(AutomationElement, TreeScope.Subtree, ControlType.Button, "Close"); } }
        }

        // --------------------------------------------------------------------
        // Control         : MenuBar
        // Name            : Application
        // Parent          : Microsoft Narrator
        // Parent Control  : Window
        // --------------------------------------------------------------------
        public static class ApplicationMenuBar
        {
            public static AutomationElement AutomationElement { get { return GetElement(MainForm.AutomationElement, TreeScope.Children, ControlType.MenuBar, "NarratorMainForm"); } }
            // ----------------------------------------------------------------
            // Control         : MenuItem
            // Name            : File
            // Parent          : Application
            // Parent Control  : MenuBar
            // ----------------------------------------------------------------
            public static class FileMenu
            {
                public static AutomationElement AutomationElement { get { return GetElement(ApplicationMenuBar.AutomationElement, TreeScope.Subtree, ControlType.MenuItem, "Item 1"); } }
                // ------------------------------------------------------------
                // Control         : MenuItem
                // Name            : Exit
                // Parent          : FileMenu
                // Parent Control  : MenuBar
                // ------------------------------------------------------------
                public static AutomationElement Exit { get { return GetElement(AutomationElement, TreeScope.Subtree, ControlType.MenuItem, "Item 257"); } }
            }
            // ----------------------------------------------------------------
            // Control         : MenuItem
            // Name            : Preferences
            // Parent          : Application
            // Parent Control  : MenuBar
            // ----------------------------------------------------------------
            public static class PreferencesMenu
            {
                public static AutomationElement AutomationElement { get { return GetElement(ApplicationMenuBar.AutomationElement, TreeScope.Subtree, ControlType.MenuItem, "Item 2"); } }
                // ------------------------------------------------------------
                // Control         : MenuItem
                // Name            : VoiceSettings
                // Parent          : PreferencesMenu
                // Parent Control  : MenuBar
                // ------------------------------------------------------------
                public static AutomationElement VoiceSettings { get { return GetElement(AutomationElement, TreeScope.Subtree, ControlType.MenuItem, "Item 259"); } }
                // ------------------------------------------------------------
                // Control         : MenuItem
                // Name            : Echo Users Keystrokes
                // Parent          : PreferencesMenu
                // Parent Control  : MenuBar
                // ------------------------------------------------------------
                public static AutomationElement EchoUsersKeystrokes { get { return GetElement(AutomationElement, TreeScope.Subtree, ControlType.MenuItem, "Item 260"); } }
                // ------------------------------------------------------------
                // Control         : MenuItem
                // Name            : Background Message Settings
                // Parent          : PreferencesMenu
                // Parent Control  : MenuBar
                // ------------------------------------------------------------
                public static AutomationElement BackgroundMessageSettings { get { return GetElement(AutomationElement, TreeScope.Subtree, ControlType.MenuItem, "Item 261"); } }
                // ------------------------------------------------------------
                // Control         : MenuItem
                // Name            : Element Auto-monitor
                // Parent          : PreferencesMenu
                // Parent Control  : MenuBar
                // ------------------------------------------------------------
                public static AutomationElement ElementAutoMonitor { get { return GetElement(AutomationElement, TreeScope.Subtree, ControlType.MenuItem, "Item 262"); } }
                // ------------------------------------------------------------
                // Control         : MenuItem
                // Name            : Announce System Message
                // Parent          : PreferencesMenu
                // Parent Control  : MenuBar
                // ------------------------------------------------------------
                public static AutomationElement AnnounceSystemMessages { get { return GetElement(AutomationElement, TreeScope.Subtree, ControlType.MenuItem, "Item 263"); } }
                // ------------------------------------------------------------
                // Control         : MenuItem
                // Name            : Announce Scroll Messsages
                // Parent          : PreferencesMenu
                // Parent Control  : MenuBar
                // ------------------------------------------------------------
                public static AutomationElement AnnounceScrollMessages { get { return GetElement(AutomationElement, TreeScope.Subtree, ControlType.MenuItem, "Item 264"); } }
                // ------------------------------------------------------------
                // Control         : MenuItem
                // Name            : Start Narrator Minimized
                // Parent          : PreferencesMenu
                // Parent Control  : MenuBar
                // ------------------------------------------------------------
                public static AutomationElement StartNarratorMinimized { get { return GetElement(AutomationElement, TreeScope.Subtree, ControlType.MenuItem, "Item 265"); } }
            }
            // ----------------------------------------------------------------
            // Control         : MenuItem
            // Name            : Help
            // Parent          : Application
            // Parent Control  : MenuBar
            // ----------------------------------------------------------------
            public static class HelpMenu
            {
                public static AutomationElement AutomationElement { get { return GetElement(ApplicationMenuBar.AutomationElement, TreeScope.Subtree, ControlType.MenuItem, "Item 3"); } }
                // ------------------------------------------------------------
                // Control         : MenuItem
                // Name            : Documentation
                // Parent          : HelpMenu
                // Parent Control  : MenuBar
                // ------------------------------------------------------------
                public static AutomationElement Documentation { get { return GetElement(HelpMenu.AutomationElement, TreeScope.Subtree, ControlType.MenuItem, "Item 267"); } }
                // ------------------------------------------------------------
                // Control         : MenuItem
                // Name            : About
                // Parent          : HelpMenu
                // Parent Control  : MenuBar
                // ------------------------------------------------------------
                public static AutomationElement HelpAbout { get { return GetElement(HelpMenu.AutomationElement, TreeScope.Subtree, ControlType.MenuItem, "Item 269"); } }
            }
        }
        public class DialogExit
        {
            // ----------------------------------------------------------------
            // Control         : Window
            // Name            : Exit Narrator
            // Parent          : Microsoft Narrator
            // Parent Control  : Window
            // ----------------------------------------------------------------
            public static AutomationElement AutomationElement { get { return GetElement(MainForm.AutomationElement, TreeScope.Subtree, ControlType.Window, "NarratorExitForm"); } }
            // ----------------------------------------------------------------
            // Control         : Pane
            // Name            :
            // Parent          : Exit Narrator
            // Parent Control  : Window
            // ----------------------------------------------------------------
            public static AutomationElement TableLayoutPanel { get { return GetElement(DialogExit.AutomationElement, TreeScope.Subtree, ControlType.Pane, "tableLayoutPanel"); } }
            // ----------------------------------------------------------------
            // Control         : Pane
            // Name            :
            // Parent          :
            // Parent Control  : Pane
            // ----------------------------------------------------------------
            public static AutomationElement PanelButtonLayout { get { return GetElement(DialogExit.AutomationElement, TreeScope.Subtree, ControlType.Pane, "buttonLayoutPanel"); } }
            // ----------------------------------------------------------------
            // Control         : Button
            // Name            : Yes
            // Parent          :
            // Parent Control  : Pane
            // ----------------------------------------------------------------
            public static AutomationElement ButtonYes { get { return GetElement(DialogExit.AutomationElement, TreeScope.Subtree, ControlType.Button, "btnYes"); } }
            // ----------------------------------------------------------------
            // Control         : Button
            // Name            : No
            // Parent          :
            // Parent Control  : Pane
            // ----------------------------------------------------------------
            public static AutomationElement ButtonNo { get { return GetElement(DialogExit.AutomationElement, TreeScope.Subtree, ControlType.Button, "btnNo"); } }
            // ----------------------------------------------------------------
            // Control         : Text
            // Name            : Are you sure you want to exit Narrator?
            // Parent          :
            // Parent Control  : Pane
            // ----------------------------------------------------------------
            public static AutomationElement TextMessage { get { return GetElement(DialogExit.AutomationElement, TreeScope.Subtree, ControlType.Text, "lblMessage"); } }
            // ----------------------------------------------------------------
            // Control         : TitleBar
            // Name            : Exit Narrator
            // Parent          : Exit Narrator
            // Parent Control  : Window
            // ----------------------------------------------------------------
            public static AutomationElement TitleBar { get { return GetElement(DialogExit.AutomationElement, TreeScope.Subtree, ControlType.TitleBar, "TitleBar"); } }
            // ----------------------------------------------------------------
            // Control         : MenuBar
            // Name            : System Menu Bar
            // Parent          : Exit Narrator
            // Parent Control  : TitleBar
            // ----------------------------------------------------------------
            public static AutomationElement SystemMenuBar { get { return GetElement(DialogExit.AutomationElement, TreeScope.Subtree, ControlType.MenuBar, "NarratorExitForm"); } }
            // ----------------------------------------------------------------
            // Control         : MenuItem
            // Name            : System
            // Parent          : System Menu Bar
            // Parent Control  : MenuBar
            // ----------------------------------------------------------------
            public static AutomationElement SystemMenuItem { get { return GetElement(DialogExit.AutomationElement, TreeScope.Subtree, ControlType.MenuItem, "Item 1"); } }
            // ----------------------------------------------------------------
            // Control         : Button
            // Name            : Close
            // Parent          : Exit Narrator
            // Parent Control  : TitleBar
            // ----------------------------------------------------------------
            public static AutomationElement ButtonClose { get { return GetElement(DialogExit.AutomationElement, TreeScope.Subtree, ControlType.Button, "Close"); } }

        }
        public class DialogVoiceSettings
        {
            // ----------------------------------------------------------------
            // Control         : Window
            // Name            : Voice Settings - Narrator
            // Parent          : Microsoft Narrator
            // Parent Control  : Window
            // ----------------------------------------------------------------
            public static AutomationElement AutomationElement { get { return GetElement(MainForm.AutomationElement, TreeScope.Children, ControlType.Window, "VoiceSettingsForm"); } }
            // ----------------------------------------------------------------
            // Control         : Text
            // Name            : Select Voice
            // Parent          :
            // Parent Control  : Pane
            // ----------------------------------------------------------------
            public static AutomationElement LabelSelectVoice { get { return GetElement(DialogVoiceSettings.AutomationElement, TreeScope.Subtree, ControlType.Text, "selectVoiceLabel"); } }
            // ----------------------------------------------------------------
            // Control         : List
            // Name            : Select Voice
            // Parent          :
            // Parent Control  : Pane
            // ----------------------------------------------------------------
            public static AutomationElement ListVoiceSettingList { get { return GetElement(DialogVoiceSettings.AutomationElement, TreeScope.Subtree, ControlType.List, "voiceSettingList"); } }
            // ----------------------------------------------------------------
            // Control         : Text
            // Name            : Set Speed
            // Parent          :
            // Parent Control  : Pane
            // ----------------------------------------------------------------
            public static AutomationElement LabelSetSpeed { get { return GetElement(DialogVoiceSettings.AutomationElement, TreeScope.Subtree, ControlType.Text, "setSpeedLabel"); } }
            // ----------------------------------------------------------------
            // Control         : ComboBox
            // Name            : Set Speed
            // Parent          :
            // Parent Control  : Pane
            // ----------------------------------------------------------------
            public static AutomationElement ComboBoxSpeedSetting { get { return GetElement(DialogVoiceSettings.AutomationElement, TreeScope.Subtree, ControlType.ComboBox, "speedSetting"); } }
            // ----------------------------------------------------------------
            // Control         : Text
            // Name            : Set Volume
            // Parent          :
            // Parent Control  : Pane
            // ----------------------------------------------------------------
            public static AutomationElement LabelSetVolume { get { return GetElement(DialogVoiceSettings.AutomationElement, TreeScope.Subtree, ControlType.Text, "setVolumeLabel"); } }
            // ----------------------------------------------------------------
            // Control         : ComboBox
            // Name            : Set Volume
            // Parent          :
            // Parent Control  : Pane
            // ----------------------------------------------------------------
            public static AutomationElement ComboBoxVolumeSetting { get { return GetElement(DialogVoiceSettings.AutomationElement, TreeScope.Subtree, ControlType.ComboBox, "volumeSetting"); } }
            // ----------------------------------------------------------------
            // Control         : Text
            // Name            : Set Pitch
            // Parent          :
            // Parent Control  : Pane
            // ----------------------------------------------------------------
            public static AutomationElement LabelSetPitch { get { return GetElement(DialogVoiceSettings.AutomationElement, TreeScope.Subtree, ControlType.Text, "setPitchLabel"); } }
            // ----------------------------------------------------------------
            // Control         : ComboBox
            // Name            : Set Pitch
            // Parent          :
            // Parent Control  : Pane
            // ----------------------------------------------------------------
            public static AutomationElement ComboBoxPitchSetting { get { return GetElement(DialogVoiceSettings.AutomationElement, TreeScope.Subtree, ControlType.ComboBox, "pitchSetting"); } }
            // ----------------------------------------------------------------
            // Control         : Button
            // Name            : OK
            // Parent          :
            // Parent Control  : Pane
            // ----------------------------------------------------------------
            public static AutomationElement ButtonOK { get { return GetElement(DialogVoiceSettings.AutomationElement, TreeScope.Subtree, ControlType.Button, "applySettingsButton"); } }
            // ----------------------------------------------------------------
            // Control         : Button
            // Name            : Cancel
            // Parent          :
            // Parent Control  : Pane
            // ----------------------------------------------------------------
            public static AutomationElement ButtonCancel { get { return GetElement(DialogVoiceSettings.AutomationElement, TreeScope.Subtree, ControlType.Button, "cancelButton"); } }
            // ----------------------------------------------------------------
            // Control         : TitleBar
            // Name            : Voice Settings - Narrator
            // Parent          : Voice Settings - Narrator
            // Parent Control  : Window
            // ----------------------------------------------------------------
            public class TitleBar
            {
                public static AutomationElement AutomationElement { get { return GetElement(MainForm.DialogVoiceSettings.AutomationElement, TreeScope.Subtree, ControlType.TitleBar, "TitleBar"); } }
                // ------------------------------------------------------------
                // Control         : MenuBar
                // Name            : System Menu Bar
                // Parent          : Voice Settings - Narrator
                // Parent Control  : TitleBar
                // ------------------------------------------------------------
                public static class SystemMenuBar
                {
                    public static AutomationElement AutomationElement { get { return GetElement(MainForm.DialogVoiceSettings.TitleBar.AutomationElement, TreeScope.Subtree, ControlType.MenuBar, "VoiceSettingsForm"); } }
                    // --------------------------------------------------------
                    // Control         : MenuItem
                    // Name            : System
                    // Parent          : System Menu Bar
                    // Parent Control  : MenuBar
                    // --------------------------------------------------------
                    public static AutomationElement SystemnMenuItem1 { get { return GetElement(AutomationElement, TreeScope.Subtree, ControlType.MenuItem, "Item 1"); } }
                }
                // ------------------------------------------------------------
                // Control         : Button
                // Name            : Close
                // Parent          : Voice Settings - Narrator
                // Parent Control  : TitleBar
                // ------------------------------------------------------------
                public static AutomationElement ButtonClose { get { return GetElement(AutomationElement, TreeScope.Subtree, ControlType.Button, "Close"); } }
            }

        }
        public class DialogBackgroundMessage
        {
            // ----------------------------------------------------------------
            // Control         : Window
            // Name            : Exit Narrator
            // Parent          : Microsoft Narrator
            // Parent Control  : Window
            // ----------------------------------------------------------------
            public static AutomationElement AutomationElement { get { return GetElement(MainForm.AutomationElement, TreeScope.Children, ControlType.Window, "MessageSettingsForm"); } }
            // ----------------------------------------------------------------
            // Control         : Pane
            // Name            :
            // Parent          : Exit Narrator
            // Parent Control  : Window
            // ----------------------------------------------------------------
            public static AutomationElement Cancel { get { return GetElement(DialogBackgroundMessage.AutomationElement, TreeScope.Subtree, ControlType.Button, "cancelButton"); } }
            // ----------------------------------------------------------------
            // Control         : Pane
            // Name            :
            // Parent          :
            // Parent Control  : Pane
            // ----------------------------------------------------------------
            public static AutomationElement OK { get { return GetElement(DialogBackgroundMessage.AutomationElement, TreeScope.Subtree, ControlType.Button, "applySettingsButton"); } }
            // ----------------------------------------------------------------
            // Control         : Button
            // Name            : Yes
            // Parent          :
            // Parent Control  : Pane
            // ----------------------------------------------------------------
            public static AutomationElement TimePeriodComboBox { get { return GetElement(DialogBackgroundMessage.AutomationElement, TreeScope.Subtree, ControlType.ComboBox, "timePeriodSetting"); } }
        }

        public class AboutDialog
        {
            // ----------------------------------------------------------------
            // Control         : Window
            // Name            : Exit Narrator
            // Parent          : Microsoft Narrator
            // Parent Control  : Window
            // ----------------------------------------------------------------
            public static AutomationElement AutomationElement { get { return GetElement(MainForm.AutomationElement, TreeScope.Children, ControlType.Window, ""); } }
            // ----------------------------------------------------------------
            // Control         : Button
            // Name            :
            // Parent          :
            // Parent Control  : 
            // ----------------------------------------------------------------
            public static AutomationElement OK { get { return GetElement(AboutDialog.AutomationElement, TreeScope.Subtree, ControlType.Button, "1"); } }
        }

        public class HelpDocument
        {
            // ----------------------------------------------------------------
            // Control         : Window
            // Name            : Exit Narrator
            // Parent          : Microsoft Narrator
            // Parent Control  : Window
            // ----------------------------------------------------------------
            public static AutomationElement AutomationElement { get { return AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "HelpPane")); } }
        }
    }
}
