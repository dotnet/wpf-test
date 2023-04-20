// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Creates a listbox with several items including another listbox and verifies items were added
    /// correctly. Makes changes to the items collection and verifies those changes.
    /// </description>
    /// </summary>
    [Test(2, "Controls", "VisualElementsBvt")]
    public class VisualElementsBvt : WindowTest
    {
        ListBox _lb1,_lb2;
        Button _btn1,_btn2;
        CheckBox _chk1,_chk2,_chk3;
        FrameworkElement[] _visualelements;

        public VisualElementsBvt()
        {
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(AddStrings);
            RunSteps += new TestStep(AddVisualElements);
            RunSteps += new TestStep(VerifyOjects);
            RunSteps += new TestStep(ChangeItemsCollection);
        }

        #region CreateTree
        // Prepares the test.
        TestResult CreateTree()
        {
            Status("CreateTree");

            _lb1 = new ListBox();
            _lb2 = new ListBox();
            _btn1 = new Button();
            _btn2 = new Button();
            _chk1 = new CheckBox();
            _chk2 = new CheckBox();
            _chk3 = new CheckBox();

            Status("Add ListBox to Window.");
            Window.Content = _lb1;

            LogComment("CreateTree was successful");
            return TestResult.Pass;
        }
        #endregion

        #region AddStrings
        // Inserts strings to the ItemsCollection.
        TestResult AddStrings()
        {
            Status("AddStrings");
            WaitForPriority(DispatcherPriority.Render);
            for (int x = 0; x < 3; x++)
            {
                _lb1.Items.Add("Hello " + x);
            }


            WaitForPriority(DispatcherPriority.Render);
            LogComment("AddStrings was successful");
            return TestResult.Pass;
        }
        #endregion

        #region AddVisualElements
        // Inserts VisualElements to the ItemsCollection.
        private TestResult AddVisualElements()
        {
            Status("AddVisualElements");

            _lb1.Items.Add(_btn1);
            _btn1.Content = "Button1";

            _lb1.Items.Add(_chk1);
            _chk1.Content = "CheckBox1";

            _lb1.Items.Add(_lb2);
            _lb2.Items.Add(_btn2);
            _btn2.Content = "Button2";

            _lb2.Items.Add(_chk2);
            _chk2.Content = "Checkbox2";

            WaitForPriority(DispatcherPriority.Render);
            LogComment("AddVisualElements was successful");
            return TestResult.Pass;
        }
        #endregion


        #region VerifyOjects
        private TestResult VerifyOjects()
        {
            Status("VerifyOjects");
            visualElements();

            Status("Verify ItemsCollection is NOT empty.");
            if (_visualelements.Length != _lb1.Items.Count)
            {
                LogComment("ListBox did not generate the correct numer of items.");
                LogComment("Expected: " + _lb1.Items.Count);
                LogComment("Actual:   " + _visualelements.Length);
                return TestResult.Fail;
            }

            Status("Verify strings were added to the VisualTree correctly.");
            for (int y = 0; y < 3; y++)
            {
                if (((TextBlock)_visualelements[y]).Text != "Hello " + y)
                {
                    LogComment("Strings were not added correctly to the VisualTree.");
                    LogComment("Expected:  " + "Hello " + y);
                    LogComment("Actual:  " + (((TextBlock)_visualelements[y]).Text));
                    return TestResult.Fail;
                }
            }

            object obj1 = _visualelements[3];
            object obj2 = _visualelements[4];
            object obj3 = _visualelements[5];

            Status("Verify expected object type in visual tree.");
            if (!(VerifyObjectType(obj1.GetType(), typeof(Button))))
                return TestResult.Fail;

            if (!(VerifyObjectType(obj2.GetType(), typeof(CheckBox))))
                return TestResult.Fail;

            if (!(VerifyObjectType(obj3.GetType(), typeof(ListBox))))
                return TestResult.Fail;

            LogComment("VerifyOjects was successful");
            return TestResult.Pass;
        }
        #endregion

        #region ChangeItemsCollection
        // Adds, Removes items from the ItemsCollection.
        private TestResult ChangeItemsCollection()
        {
            Status("ChangeItemsCollection");
            Status("Remove 1st item from ItemsCollection.");
            _lb1.Items.RemoveAt(0);
            WaitForPriority(DispatcherPriority.Render);


            Status("Find visual elements.");
            visualElements();

            Status("Verify visual element count.");
            if(!(verifyCount(5)))
                return TestResult.Fail;

            Status("Remove item from middle of collection.");
            _lb1.Items.Remove(_btn1); 
            WaitForPriority(DispatcherPriority.Render);


            Status("Find visual elements.");
            visualElements();

            Status("Verify visual element count.");
            if (!(verifyCount(4)))
                return TestResult.Fail;

            Status("Remove item from end of collection.");      
            _lb1.Items.MoveCurrentToLast();
            _lb1.Items.Remove(_lb2);
            WaitForPriority(DispatcherPriority.Render);


            Status("Find visual elements.");
            visualElements();

            Status("Verify visual element count.");
            if (!(verifyCount(3)))
                return TestResult.Fail;

            LogComment("ChangeItemsCollection was successful");
            return TestResult.Pass;  
        }
        #endregion

        #region AuxMethods
        // Finds visual elements in tree.
        void visualElements()
        {
            _visualelements = Util.FindDataVisuals(_lb1, _lb1.Items);
        }
        
        // Verifies visual element count.
        bool verifyCount(int count)
        {
            if (_visualelements.Length != count)
            {
                LogComment("ItemsCollection count is incorrect.  Expected: '" + count + "'" + " Actual: '" + _visualelements.Length + "'");
                return false;
            }

            return true;
        }

        // Verifies object types
        public static bool VerifyObjectType(Type expectedObject, Type actualObject)
        {
            if (actualObject != expectedObject)
            {
                GlobalLog.LogEvidence("Object type is incorrect.  Expected:  " + expectedObject + "  Actual:  " + actualObject);
                return false;
            }

            return true;
        }
        #endregion
    }
}
