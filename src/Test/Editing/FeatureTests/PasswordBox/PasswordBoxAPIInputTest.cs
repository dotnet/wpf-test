// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;

    using System.Windows;
    using System.Windows.Controls;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    
    #endregion Namespaces.

    /// <summary>
    /// This class intend to test the PasswordBox OM.
    /// </summary>
    [Test(0, "PasswordBox", "PasswordBoxAPIInputTest", MethodParameters = "/TestCaseType=PasswordBoxAPIInputTest /Priority=0", Keywords = "Localization_Suite")]
    [TestOwner("Microsoft"), TestBugs("460, 461, 462, 107, 463"),
     TestTactics("424, 425, 426, 427"), TestWorkItem("67")]
    public class PasswordBoxAPIInputTest : PasswordBoxBase
    {
        #region BVT cases
        char _defaultPasswordCharNotInVisualTree = '*';
        /// <summary>
        /// BVT case to Test Clear() Method of PasswordBox.
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.BVT, "BVT case to Test Clear() Method of PasswordBox")]
        public void Test_Clear_Method_BVT()
        {
            EnterFuction("Test_Clear_Method_BVT");

            //call clear when there is nothing in.
            PasswordBox pBox1 = new PasswordBox();
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, "", 0, 0, 0);
            pBox1.Clear();
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, "", 0, 0, 0);

            //Clean when there is something in
            string password = "abcdefghi123423148&83^$^%$$&$%^%#$^&";
            pBox1.Password = password;
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, password, 0, 0, 0);
            pBox1.Clear();
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, "", 0, 0, 0);
            QueueDelegate(EndTest);
            EndFunction();
        }
        /// <summary>
        /// BVT case to Test paste() Method of PasswordBox.
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.BVT, "BVT case to Test paste() Method of PasswordBox")]
        public void Test_Paste_Method_BVT()
        {
            EnterFunction("Test_Paste_Method_BVT");
            PasswordBox pBox1 = new PasswordBox();
            //paste into empty passwordbox.
            MyLogger.Log(CurrentFunction + " - paste into empty PasswordBox");
            pBox1.Clear();
            string password = "abcdefghi123423148&83^$^%$$&$%^%#$^&";
            System.Windows.Clipboard.SetDataObject(password);
            pBox1.Paste();
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, password, password.Length, 0, 0);

            //paste over selection
            MyLogger.Log(CurrentFunction + " - paste over selection");
            pBox1.SelectAll();
            pBox1.Paste();
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, password, password.Length, 0, 0);

            //Paste when Clipboard is empty.
            System.Windows.Clipboard.SetDataObject("");
            pBox1.Password = password;
            pBox1.Paste();
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, password, 0, 0, 0);

            QueueDelegate(EndTest);
            EndFunction();
        }

        /// <summary>
        /// TEst SelectAll method when in different length of the password.
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.BVT, "BVT case to test SelectAll method of PasswordBox")]
        public void Test_SelectAll_Method_BVT()
        {
            EnterFunction("Test_SelectAll_BVT");
            PasswordBox pBox1 = new PasswordBox();

            //SelectAll have no effect when there is a empty PasswordBox
            MyLogger.Log(CurrentFunction + " - SelectAll have no effect when there is a empty PasswordBox");
            pBox1.Clear();
            pBox1.SelectAll();
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, "", 0, 0, 0);

            //SelectAll to select password with lenght=1"
            MyLogger.Log(CurrentFunction + " - SelectAll to select password with lenght=1");
            pBox1.Password = "a";
            pBox1.SelectAll();
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, "a", 0, 1, 0);

            //SelectAll to select password with lenght=100
            MyLogger.Log(CurrentFunction + " - SelectAll to select password with lenght=100");
            pBox1.Password = RepeatString("a", 100);
            pBox1.SelectAll();
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, RepeatString("a", 100), 0, 100, 0);
            QueueDelegate(EndTest);
            EndFunction();
        }
        /// <summary>
        /// Test the MaxLength property of the PasswordBox. it can be 0 - win32.max
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.BVT, "BVT case to test the MaxLength property of the PasswordBox")]
        public void Test_MaxLength_Property_BVT()
        {
            EnterFunction("Test_MaxLength_BVT");
            PasswordBox pBox1 = new PasswordBox();

            //Check the default value = 0;
            MyLogger.Log(CurrentFunction + " - Check the default value = 0");
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, "", 0, 0, 0);
            pBox1.MaxLength = 0;
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, "", 0, 0, 0);
            try
            {
                pass = false;
                pBox1.MaxLength = -5;
            }
            catch (System.ArgumentException e)
            {
                pass = true;
                MyLogger.Log("Expected expcetion: " + e.Message);
            }
            
            MyLogger.Log(CurrentFunction + " - Check the default MaxLength = 1, Regression_Bug107 is by design");
            pBox1.MaxLength = 1;
            pBox1.Password = "abzasdfadsfasd";
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, "abzasdfadsfasd", 0, 0, 1);

            //set to int32.MaxValue
            MyLogger.Log(CurrentFunction + " - Check the default MaxLength = win32.MaxValue");
            pBox1.MaxLength = Int32.MaxValue;
            pBox1.Password = RepeatString("a", 1000);
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, RepeatString("a", 1000), 0, 0, Int32.MaxValue);
            QueueDelegate(EndTest);
            EndFunction();
        }

        /// <summary>
        /// Test the Password property of the PasswordBox.
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.BVT, "BVT case to test the Password property of the PasswordBox")]
        public void Test_Password_Property_BVT()
        {
            EnterFunction("Test_Password_Property_BVT");
            PasswordBox pBox1 = new PasswordBox();
            //Check null value;
            MyLogger.Log(CurrentFunction + " - Set null value for Password Property.");
            pBox1.Password = "junch";
            pBox1.Password = null;
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, "", 0, 0, 0);

            FailedIf(!pass, CurrentFunction + " - Failed: we did not get the expected exception!!!");

            //check empty string
            MyLogger.Log(CurrentFunction + " - Set null value for Password Property.");
            pBox1.Password = "";
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, "", 0, 0, 0);

            //String contains some characters.
            MyLogger.Log(CurrentFunction + " - set SecureString contains some characters for Password Property.");
            pBox1.Password = "123456789";
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, "123456789", 0, 0, 0);
            QueueDelegate(EndTest);
            EndFunction();
        }

        /// <summary>
        /// BVT case to test the PasswordChar property of the PasswordBox.
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.BVT, "BVT case to test the PasswordChar property of the PasswordBox")]
        public void Test_PasswordChar_Property_BVT()
        {
            EnterFunction("Test_PasswordChar_Property_BVT");
            PasswordBox pBox1 = new PasswordBox();
            //defult value='*'
            MyLogger.Log(CurrentFunction + " - Default value='*' for Password Property.");
            PasswordVerifier(pBox1, _defaultPasswordCharNotInVisualTree, "", 0, 0, 0);

            //PasswordChar='?'
            MyLogger.Log(CurrentFunction + " - Set PasswordChar='?' for Password Property.");
            pBox1.PasswordChar = '?';
            pBox1.Password = "abc";
            PasswordVerifier(pBox1, '?', "abc", 0, 0, 0);

            //passwordChar='\uD8D2'
            MyLogger.Log(CurrentFunction + " - Set PasswordChar='\uD8D2' for Password Property.");
            pBox1.PasswordChar = '\uD8D2';  //Suragate: "\uD8D2\uFDE3";
            PasswordVerifier(pBox1, '\uD8D2', "abc", 0, 0, 0);

            QueueDelegate(EndTest);

            EndFunction();
        }
        
        /// <summary>
        /// Test PasswordChanged event. This event should fire when password is changed either by set Password property or user input.
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.BVT, "BVT case to test the Test PasswordChanged event. of the PasswordBox")]
        public void Test_PasswordChanged_Event_BVT()
        {
            EnterFunction("Test_PasswordChanged_Event_BVT");

            PasswordBox pBox1 = _wraper1.Element as PasswordBox;

            pBox1.PasswordChanged += new RoutedEventHandler(PasswordChangedEventHandler);
            //Set empty string on empty PasswordBox
            MyLogger.Log(CurrentFunction + " - Set empty string on empty PasswordBox.");
            pBox1.Password = "";
            FailedIf(PasswordChangedCounter != 0, CurrentFunction + " - Failed: PasswordChanged event should not fire when set empty PasswordBox with[]!!!");

            //set a password triggle the event
            pBox1.Password = "a";
            MyLogger.Log(CurrentFunction + " - Set [a] on empty PasswordBox.");
            FailedIf(PasswordChangedCounter != 1, CurrentFunction + " - Failed: PasswordChanged event did not fire when set empty PasswordBox with[abc]!!!");

            MyLogger.Log(CurrentFunction + " - Set the same password should not triggle the event.");
            pBox1.Password = "abc";
            FailedIf(PasswordChangedCounter != 2, CurrentFunction + " - Failed: PasswordChanged event should fire when set the same password that is already in the PasswordBox. Regression_Bug524 is by design!!!");

            //Set empty string when PasswordBox is not empty.
            MyLogger.Log(CurrentFunction + " - Set empty string when PasswordBox is not empty.");
            PasswordChangedCounter = 0;
            pBox1.Password = "";
            FailedIf(PasswordChangedCounter != 1, CurrentFunction + " - Failed: PasswordChanged event did not fire when set non empty PasswordBox with empty string!!!");

            PasswordChangedCounter = 0;
            //intentionally remove twice.
            pBox1.PasswordChanged -= new RoutedEventHandler(PasswordChangedEventHandler);
            pBox1.PasswordChanged -= new RoutedEventHandler(PasswordChangedEventHandler);
            pBox1.Password = "junck";
            FailedIf(PasswordChangedCounter != 0, CurrentFunction + " - Failed: PasswordChanged event should not fire after the handler is removed!!!");

            QueueDelegate(EndTest);
            EndFunction();
        }

        #endregion      
    }
}
