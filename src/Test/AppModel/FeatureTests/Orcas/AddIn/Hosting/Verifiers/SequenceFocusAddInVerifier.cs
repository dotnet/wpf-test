// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.Diagnostics;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Threading;
using Microsoft.Test.Utilities;

namespace Microsoft.Test.AddIn
{
    public class SequenceFocusAddInVerifier : IVerifyAddIn
    {

        #region Private Members

        private TestLog _log;
        private HostSequenceFocusContractToViewAdapter _hostView;
        private Panel _parent;
        private AddInHost _addInHost;
        private List<FocusItem> _list;
        private TextBox _tb1;
        private TextBox _tb2;

        #endregion

        #region Constructor

        public SequenceFocusAddInVerifier()
        {
            _log = TestLog.Current;
            _hostView = null;
            _tb1 = null;
            _tb2 = null;
            _list = new List<FocusItem>();
        }

        #endregion

        #region IVerfiyAddIn Members

        /// <summary>
        /// Prepares Verifier to verify the AddIn
        /// </summary>
        /// <param name="hostParameters">Copy of the AddIn parameters that were passed to the AddIn</param>
        /// <param name="parent">AddIn Host parent panel</param>
        public void Initialize(string addInParameters, Panel parent)
        {
            //In future look into the string to determine if AddIns are nested
            this._parent = parent;

            _log.LogStatus("Adding two TextBoxes to the root panel");
            
            _tb1 = new TextBox();
            _tb1.Name = "Host_TextBox1";
            _tb1.TabIndex = 1;
            _tb1.Height = 30;
            _tb1.Width = 100;
            _tb1.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(Item_GotFocus);
            this._parent.Children.Add(_tb1);

            _tb2 = new TextBox();
            _tb2.Name = "Host_TextBox2";
            _tb2.TabIndex = 2;
            _tb2.Height = 30;
            _tb2.Width = 100;
            _tb2.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(Item_GotFocus);
            this._parent.Children.Add(_tb2);

            WaitForPriority(DispatcherPriority.Background);
        }

        /// <summary>
        /// Event handler for focus event for the Host Textboxes. Adds an entry in the list of items when called.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void Item_GotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _log.LogStatus("Item Got Focus");
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                FocusItem item = new FocusItem();
                item.Name = element.Name;
                item.DateTime = DateTime.Now;
                lock (_list)
                {
                    _list.Add(item);
                }
            }
        }

        /// <summary>
        /// Verifies the AddIn
        /// </summary>
        /// <param name="testAddIn">Reference to the HostView instance</param>
        /// <returns>Pass if the AddIn worked as expected
        /// Fail if it did not respond correctly
        /// Unknown if the Verifier can not verify the AddIn</returns>
        public TestResult VerifyTestAddIn(object hostView)
        {
            if (!CanVerify(hostView.GetType()))
            {
                _log.LogEvidence("Can not verify " + hostView.ToString() + " with " + this.ToString() + " as the verifier");
                return TestResult.Unknown;
            }
            else
            {
                this._hostView = (HostSequenceFocusContractToViewAdapter)hostView;
                if (this._hostView == null)
                {
                    _log.LogEvidence("Can not cast " + hostView.ToString() + " to HostSequenceFocusContractToViewAdapter");
                    return TestResult.Fail;
                }
                else
                {
                    return VerifyAddIn();
                }
            }
        }

        /// <summary>
        /// Indicates if the Verifier can verify a given AddIn
        /// </summary>
        /// <param name="addInType">Type of the Host View of the AddIn</param>
        /// <returns>true if the Verifier can verify the AddIn, false if not</returns>
        public bool CanVerify(Type hostViewType)
        {
            _log.LogStatus("Determining if " + this.ToString() + " can verify " + hostViewType.ToString());
            return hostViewType == typeof(HostSequenceFocusContractToViewAdapter);
        }

        /// <summary>
        /// Property to access the Panel that will host the AddIn UI
        /// </summary>
        public Panel AddInHostParent 
        {
            get{ return _parent; }
            set{ _parent = value; }
        }

        /// <summary>
        /// Reference to the AddInHost that this verifier is contained in
        /// </summary>
        public AddInHost AddInHost
        {
            get { return _addInHost; }
            set { _addInHost = value; }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Verifies the AddIn
        /// </summary>
        /// <returns>Pass if the AddIn passes, Fail if it does not perform as expected</returns>
        private TestResult VerifyAddIn()
        {
            //Verify initial values
            List<string> emptyList = new List<string>();
            WaitForPriority(DispatcherPriority.Background);

            _log.LogStatus("Verifying initial AddIn values");
            if (VerifySequence( emptyList ))
            {
                _log.LogEvidence("Initial sequence was correct");
            }
            else
            {
                _log.LogEvidence("Initial sequence was not correct");
                return TestResult.Fail;
            }

            //Click on TextBox1 (will cause focus event to fire)
            MoveMouseAndClick(_tb1);

            //Tab forward 10 times
            if (TabForwardAndVerify(10))
            {
                _log.LogEvidence("Tabbing forward had the correct sequence");
            }
            else
            {
                _log.LogEvidence("Tabbing forward did not have the correct sequence");
                return TestResult.Fail;
            }

            //Click on TextBox2 (will cause focus event to fire)
            MoveMouseAndClick(_tb2);

            //Clear the lists, should leave focus in TextBox2
            if (ClearListsAndVerify())
            {
                _log.LogEvidence("Clear was correct");
            }
            else
            {
                _log.LogEvidence("Clear was not correct");
                return TestResult.Fail;
            }
            
            //Click on TextBox1 (will cause focus event to fire)
            MoveMouseAndClick(_tb1);
            
            //Tab backwards 10 times
            if (!TabBackwardAndVerify(10))
            {
                return TestResult.Fail;
            }
            else
            {
                _log.LogEvidence("Tabbing backward had the correct sequence");
            }

            WaitForPriority(DispatcherPriority.Background);

            return TestResult.Pass;
        }

        /// <summary>
        /// Tabs forward a number of times
        /// </summary>
        /// <param name="tabTotal">number of tab forward to perform</param>
        /// <returns>true if verification is correct, false if there is an error</returns>
        private bool TabForwardAndVerify(int tabTotal)
        {
            List<string> expected = new List<string>();
            _log.LogStatus("Tabbing forward " + tabTotal + " times");
            for (int i = 0; i < tabTotal; i++)
            {
                Tab();
            }
            
            WaitForPriority(DispatcherPriority.Background);
            _log.LogStatus("Verifying tab forward");
            
            // 
            expected.Add("Host_TextBox1");
            expected.Add("Host_TextBox2");
            expected.Add("AddIn_TextBox1");
            expected.Add("AddIn_TextBox2");
            expected.Add("AddIn_TextBox3");
            expected.Add("Host_TextBox1");
            expected.Add("Host_TextBox2");
            if ((SystemInformation.WpfVersion == WpfVersions.Wpf30) || (SystemInformation.WpfVersion == WpfVersions.Wpf35) || (SystemInformation.WpfVersion == WpfVersions.Wpf40))
            {
                expected.Add("AddIn_TextBox3"); 
            }
            expected.Add("AddIn_TextBox1");
            expected.Add("AddIn_TextBox2");
            expected.Add("AddIn_TextBox3");
            expected.Add("Host_TextBox1");

            return VerifySequence(expected);
        }

        /// <summary>
        /// Tabs backward a number of times
        /// </summary>
        /// <param name="tabTotal">number of tab back to perform</param>
        /// <returns>true if verification is correct, false if there is an error</returns>
        private bool TabBackwardAndVerify(int tabTotal)
        {
            List<string> expected = new List<string>();
            _log.LogStatus("Tabbing backward " + tabTotal + " times");
            for (int i = 0; i < tabTotal; i++)
            {
                ShiftTab();
            }

            WaitForPriority(DispatcherPriority.Background);
            _log.LogStatus("Verifying tab backward");

            // 
            expected.Add("Host_TextBox1");
            expected.Add("AddIn_TextBox3");
            expected.Add("AddIn_TextBox2");
            expected.Add("AddIn_TextBox1");
            expected.Add("Host_TextBox2");
            expected.Add("Host_TextBox1");
            if ((SystemInformation.WpfVersion == WpfVersions.Wpf30) || (SystemInformation.WpfVersion == WpfVersions.Wpf35) || (SystemInformation.WpfVersion == WpfVersions.Wpf40))
            {
                expected.Add("AddIn_TextBox1"); 
            }
            expected.Add("AddIn_TextBox3");
            expected.Add("AddIn_TextBox2");
            expected.Add("AddIn_TextBox1");
            expected.Add("Host_TextBox2");
            expected.Add("Host_TextBox1");
            return VerifySequence(expected);
        }

        /// <summary>
        /// Clears the lists and verifies the result
        /// </summary>
        /// <returns>true if the lists are empty</returns>
        private bool ClearListsAndVerify()
        {
            List<string> emptyList = new List<string>();

            _log.LogStatus("Clearing lists");
            ClearLists();
            _log.LogStatus("Verifying clear worked");

            return VerifySequence(emptyList);
        }

        /// <summary>
        /// Merges the host list and the AddIn list
        /// </summary>
        /// <returns>A Merged list</returns>
        private List<FocusItem> MergeHostAndAddInList()
        {
            List<FocusItem>[] lists = new List<FocusItem>[2];
            List<FocusItem> items = new List<FocusItem>(_hostView.GetFocusSequence());
            lists[0] = this._list;
            lists[1] = items;

            List<FocusItem> fullList = ListUtilities.Merge<FocusItem>(lists, MergeAffinity.UsePreviousList);
            return fullList;
        }

        /// <summary>
        /// Simulates a press of Tab Key
        /// </summary>
        private void Tab()
        {
            UserInput.KeyPress(Key.Tab, true);
            UserInput.KeyPress(Key.Tab, false);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            System.Threading.Thread.Sleep(1000);
        }

        /// <summary>
        /// Simulates a press of Shift + Tab Key (Back Tab)
        /// </summary>
        private void ShiftTab()
        {
            UserInput.KeyPress(Key.LeftShift, true);
            Tab();
            UserInput.KeyPress(Key.LeftShift, false);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
        }

        /// <summary>
        /// Clears the lists
        /// </summary>
        private void ClearLists()
        {
            _list.Clear();
            _hostView.ClearSequence();
        }

        /// <summary>
        /// Verifies the sequence using the passed in expected values and the Merged lists of actual values
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        private bool VerifySequence(List<string> names)
        {
            WaitForPriority(DispatcherPriority.Background);
            int i = 0;

            List<FocusItem> fullList = MergeHostAndAddInList();

            if (fullList == null && names == null)
            {
                _log.LogStatus("Null sequence was correct");
                return true;
            }

            if (fullList == null && names != null)
            {
                _log.LogEvidence("Sequence was null, was expected to be: ");
                for (i = 0; i < names.Count; i++)
                {
                    _log.LogEvidence(names[i]);
                }
                return false;
            }

            if (fullList != null && names == null)
            {
                _log.LogEvidence("Expected sequence to be null, actual: ");
                for (i = 0; i < fullList.Count; i++)
                {
                    _log.LogEvidence(fullList[i].Name);
                }
                return false;
            }

            if (fullList.Count != names.Count)
            {

                long initialOffSet = fullList[0].DateTime.Ticks;

                _log.LogEvidence("Expected sequence length was " + names.Count + " actual length was " + fullList.Count);

                _log.LogEvidence("Actual Values");
                for (i = 0; i < fullList.Count; i++)
                {
                    _log.LogEvidence(fullList[i].Name + "\t" + (fullList[i].DateTime.Ticks - initialOffSet).ToString());
                }

                return false;
            }
            else
            {
                bool isCorrect = true;
                for (i = 0; i < fullList.Count; i++)
                {
                    if (fullList[i].Name != names[i])
                    {
                        isCorrect = false;
                        _log.LogEvidence("Expected " + names[i] + " actual was " + fullList[i].Name);
                    }
                    else
                    {
                        _log.LogStatus(i.ToString() + " was correct " + names[i]);
                    }
                }
                if (isCorrect)
                {
                    _log.LogStatus("Names were in the correct sequence");
                }
                return isCorrect;
            }
        }

        /// <summary>
        /// Move the mouse cursor over the specified FrameworkElement and pressed the left click
        /// </summary>
        /// <param name="element"></param>
        private void MoveMouseAndClick(FrameworkElement element)
        {
            _log.LogStatus("Determining point to move to based on " + element.Name);
            Rectangle rect = ImageUtility.GetScreenBoundingRectangle(element);
            _log.LogStatus(rect.ToString());
            System.Windows.Point point = new System.Windows.Point(rect.X + 10, rect.Y + 10);
            _log.LogStatus("Moving mouse and clicking on point x=" + point.X.ToString() + " y=" + point.Y.ToString());
            Input.Input.MoveToAndClick(point);
            WaitForPriority(DispatcherPriority.Background);
            _log.LogStatus("Move and click complete");
        }


        /// This is from Avalon Test. Should move to a base class or use helper function
        /// <summary>
        /// Waits for a specific Dispatcher Priority to occur
        /// </summary>
        /// <param name="priority">Dispatcher Priority to wait for</param>
        /// <returns>true if sucessfull otherwise false when a timeout occurs</returns>
        private void WaitForPriority(DispatcherPriority priority)
        {
            DispatcherHelper.DoEvents(0, priority);
        }

        #endregion

    }
}
