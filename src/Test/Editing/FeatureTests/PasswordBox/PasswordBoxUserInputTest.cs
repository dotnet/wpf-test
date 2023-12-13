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
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>
    /// This class intend to test the PasswordBox OM.
    /// </summary>
    [TestOwner("Microsoft"), TestBugs("459, Regression_Bug114, Regression_Bug113"), TestTactics("426"), TestWorkItem("68, 69")]
    public class PasswordBoxUserInputTest : PasswordBoxBase
    {
        #region BVT cases
    
        /// <summary>
        /// This case containing 
        ///                     1. Mouse Selection
        ///                     2. Keyboard typping
        ///                     3. Caret Navigation
        ///                     4. Double Clicking.
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.BVT, "Use Mouse to make selection in PasswordBox")]
        public void Basic_BVT_Cases()
        {
            EnterFunction("Basic_BVT_Cases");

            TestDataArrayList = new ArrayList();
            MouseSelection();
            CommandCases();
            KeyboardEditing();
           
            //clipboard was set before any other cases. 
            //So that no passowrdbox editing action should change the Clipboard value.
            ClipBaordAccessTest();
            
            SetInitValuesForNextLocalCase();
            QueueDelegate(PasswordInputExecution);
            
            EndFunction();
        }

        /// <summary>Add Clipboard cases</summary>
        void ClipBaordAccessTest()
        {
            System.Windows.Clipboard.SetDataObject("a1b2c3d4e5");
            TestDataArrayList.Add(new PasswordBoxKeyboardInputData("Paste to PasswordBox", "^a^c^x^v", new PasswordBoxStatus("12345", 0, 5, DefaultPasswordChar, 0), new PasswordBoxStatus("a1b2c3d4e5", 10, 0, DefaultPasswordChar, 0), true, 0));
            TestDataArrayList.Add(new PasswordBoxKeyboardInputData("Paste to PasswordBox", "{end}^c^x^v", new PasswordBoxStatus("x", 1, 0, DefaultPasswordChar, 0), new PasswordBoxStatus("xa1b2c3d4e5", 11, 0, DefaultPasswordChar, 0), true, 0));
        }

        /// <summary>add command cases</summary>
        void CommandCases()
        {
            //Cut selection should not be supported.
            TestDataArrayList.Add(new PasswordBoxKeyboardInputData("Cut a selection", "abcde+{Left 2}^x", null, new PasswordBoxStatus("abcde", 3, 2, DefaultPasswordChar, 0), true, 0));
        
            //Copy command is test in Clipboard cases.

            //UndoRedo should not be supported.
            TestDataArrayList.Add(new PasswordBoxKeyboardInputData("Cut a selection", "abcde+{Left 2}^z", null, new PasswordBoxStatus("abcde", 3, 2, DefaultPasswordChar, 0), true, 0));
        }
        
        /// <summary>add Keyboard editing cases</summary>
        void KeyboardEditing()
        {
            //keyboard cases
            TestDataArrayList.Add(new PasswordBoxKeyboardInputData("Basic typing", "abcde1234567890", null, new PasswordBoxStatus("abcde1234567890", 15, 0, DefaultPasswordChar, 0), true, 0));
            
            //Type over a selection
            TestDataArrayList.Add(new PasswordBoxKeyboardInputData("type over a selection", "abcde+{Left 2}f", null, new PasswordBoxStatus("abcf", 4, 0, DefaultPasswordChar, 0), true, 0));
            
            //Backspace a char
            TestDataArrayList.Add(new PasswordBoxKeyboardInputData("Backspace to delete char", "abcde{backspace 2}f", null, new PasswordBoxStatus("abcf", 4, 0, DefaultPasswordChar, 0), true, 0));
            
            //backspace a selection
            TestDataArrayList.Add(new PasswordBoxKeyboardInputData("type to delete a selection", "abcde+{Left 2}{backspace}f", null, new PasswordBoxStatus("abcf", 4, 0, DefaultPasswordChar, 0), true, 0));
            
            //delete a char
            TestDataArrayList.Add(new PasswordBoxKeyboardInputData("Delete a char", "abcde{Left 2}{delete 2}f", null, new PasswordBoxStatus("abcf", 4, 0, DefaultPasswordChar, 0), true, 0));
            
            //Delete a selection
            TestDataArrayList.Add(new PasswordBoxKeyboardInputData("Delete a Selection", "abcde+{Left 2}{delete}f", null, new PasswordBoxStatus("abcf", 4, 0, DefaultPasswordChar, 0), true, 0));
            
            //Home and the delete
            TestDataArrayList.Add(new PasswordBoxKeyboardInputData("Home key to Navigate and then delete", "abcdef{Home}{delete}", null, new PasswordBoxStatus("bcdef", 0, 0, DefaultPasswordChar, 0), true, 0));
            
            //End key
            TestDataArrayList.Add(new PasswordBoxKeyboardInputData("End key to navigate", "abcdef{Home}{delete}{End}", null, new PasswordBoxStatus("bcdef", 5, 0, DefaultPasswordChar, 0), true, 0));
            
            //Type Over Selection.
            TestDataArrayList.Add(new PasswordBoxKeyboardInputData("Type over a selection", "abcde+{Left 2}^xf", null, new PasswordBoxStatus("abcf", 4, 0, DefaultPasswordChar, 0), true, 0));
            
            //Select All 
            TestDataArrayList.Add(new PasswordBoxKeyboardInputData("Select All in a PasswordBox", "abcde^a", null, new PasswordBoxStatus("abcde", 0, 5, DefaultPasswordChar, 0), true, 0));
            
            //Move caret to right
            TestDataArrayList.Add(new PasswordBoxKeyboardInputData("Move caret to right", "abcde{HOME}+{RIGHT 2}", null, new PasswordBoxStatus("abcde", 0, 2, DefaultPasswordChar, 0), true, 0));

            //Cut selection should not be supported. //Regression_Bug113
            TestDataArrayList.Add(new PasswordBoxKeyboardInputData("Cut a selection", "abcde+{Left 2}^x", null, new PasswordBoxStatus("abcde", 3, 2, DefaultPasswordChar, 0), true, 0));
        }

        /// <summary>add mouse selection cases</summary>
        void MouseSelection()
        {
            string password = "abcdefgh";
            Dimension[] dimensions;
            int startIndex, endIndex;

            //Note: Regression_Bug114 prevent us to make index up to 8. When it is fixed, we will enable selection from end. 
            dimensions = new Dimension[] {
                new Dimension("StartIndex", new object[] {0, 1, 2, 3,4,5,6, 7}),
                new Dimension("EndIndex", new object [] {0,1, 2, 3, 4, 5, 6, 7}),
            };

            //caret combination for mouse selection.
            _engine = CombinatorialEngine.FromDimensions(dimensions);
            values = new Hashtable();
            PasswordBoxStatus initStatus = new PasswordBoxStatus(password, 0, 0, DefaultPasswordChar, 0);

            while (_engine.Next(values))
            {
                startIndex = (int)values["StartIndex"];
                endIndex = (int)values["EndIndex"];

                PasswordBoxStatus finalStatus = new PasswordBoxStatus(password, Math.Min(startIndex, endIndex), Math.Abs(endIndex - startIndex), DefaultPasswordChar, 0);
                TestDataArrayList.Add(new PasswordBoxMouseDragData("MouseSelection: Start=" + startIndex + " End = " + endIndex, startIndex, endIndex, initStatus, finalStatus, true, 1));
            }

            for (int i = 0; i < 9; i++)
            {
                PasswordBoxStatus finalStatus = new PasswordBoxStatus(password, 0, 8, DefaultPasswordChar, 0);
                TestDataArrayList.Add(new PasswordBoxMouseDoubleClickData("DoubleClick at: " + i, i, initStatus, finalStatus, true, 0));
            }
        }
 
        #endregion
    }
}
