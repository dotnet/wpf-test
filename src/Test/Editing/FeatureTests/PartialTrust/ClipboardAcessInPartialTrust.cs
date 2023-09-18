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
    using System.Windows.Controls.Primitives;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;    

    #endregion Namespaces.

    /// <summary>
    /// This class intend to test clipboard access in partial trust
    /// </summary>
    // DISABLEDUNSTABLETEST:
    // TestName:ClipboardAccessInPartialTrust
    // Area: Editing SubArea: PartialTrust
    // Disable this case due to high fail rate, will enable after fix it.
    // to find all disabled tests in test tree, use: findstr /snip DISABLEDUNSTABLETEST
    [Test(2, "PartialTrust", TestCaseSecurityLevel.FullTrust, "ClipboardAccessInPartialTrust", MethodParameters = "/TestCaseType=ClipboardAccessInPartialTrust /Pri=0 /InputMonitorEnabled:False /XbapName=EditingTestDeploy", Timeout = 200,Disabled = true)]
    [TestOwner("Microsoft"), TestBugs("465"), TestTactics("422"), TestWorkItem("66")]
    public class ClipboardAccessInPartialTrust : ManagedCombinatorialTestCase
    {
        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);
            if (_editableType.IsPassword)
            {
                result = false;
            }

            return result; 
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            //Create control
            KeyboardInput.ResetCapsLock();
            _wrapper = new UIElementWrapper(_editableType.CreateInstance());
            TestElement = (FrameworkElement)_wrapper.Element;
            QueueDelegate(SelectContent);
           
        }

        /// <summary>Select "AB" </summary>
        private void SelectContent()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("ab");
            QueueDelegate(DoMoreInput1);
            
        }

        /// <summary>DoInput</summary>
        private void DoMoreInput1()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("^{Home}");
            QueueDelegate(DoMoreInput2);
        }

        /// <summary>DoInput</summary>
        private void DoMoreInput2()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("+{RIGHT 2}");
            QueueDelegate(DoUserCopyPaste);
        }

        /// <summary>Perfrom User action of copy and paste.</summary>
        private void DoUserCopyPaste()
        {
            VerifyResult("ab", "ab");
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("^c");
            QueueDelegate(DoMoreInput3);
        }

        /// <summary>DoInput</summary>
        private void DoMoreInput3()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("{END}^v");
            QueueDelegate(DoMoreInput4);
        }  
        
        /// <summary>DoInput</summary>
        private void DoMoreInput4()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("+{LEFT 4}");
            QueueDelegate(DoUserCut);
        }  
        
        /// <summary>
        /// perform user action: cut
        /// </summary>
        private void DoUserCut()
        {
            VerifyResult("abab", "abab");
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(DoMoreInput5);
        }
        /// <summary>DoInput</summary>
        private void DoMoreInput5()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("+{RIGHT 3}");
            QueueDelegate(DoMoreInput6);
        }

        /// <summary>DoInput</summary>
        private void DoMoreInput6()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("^x");
            QueueDelegate(DoMoreInput7);
        }
        /// <summary>DoInput</summary>
        private void DoMoreInput7()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("^a");
            QueueDelegate(DoMoreInput8);
        }

        /// <summary>DoInput</summary>
        private void DoMoreInput8()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("{DELETE}");
            QueueDelegate(DoMoreInput9);
        }

        /// <summary>DoInput</summary>
        private void DoMoreInput9()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("^v");
            QueueDelegate(VerifyUserCut);
        }
      
        /// <summary>Verify the cut result</summary>
        void VerifyUserCut()
        {
            VerifyResult("", "aba");
            //Make selection of 'a'
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(DoMoreInput10);
        }

        /// <summary>DoInput</summary>
        private void DoMoreInput10()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("+{RIGHT}");
            QueueDelegate(DoAPICopy);
        }

       /// <summary>TestBoxBase.copy()</summary>
        private void DoAPICopy()
        {
            VerifyResult("a", "aba");
            //API copy
            QueueDelegate(((TextBoxBase)_wrapper.Element).Copy);
            QueueDelegate(VerifyAPICopy);

        }

        /// <summary>Verify TextBoxBase.Copy()</summary>
        private void VerifyAPICopy()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("^a");
            QueueDelegate(DoMoreInput11);
        }

        /// <summary>DoInput</summary>
        private void DoMoreInput11()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("^v^v");
            QueueDelegate(DoAPICut);
        }

        /// <summary>Test TextBoxBase.Cut()</summary>
        private void DoAPICut()
        {
            // Programmatic copy performed in DoAPICopy() is allowed in 3.X versions
#if TESTBUILD_CLR20
            VerifyResult("", "aa");
#endif
            // Programmatic copy performed in DoAPICopy() is not allowed in 4.0 versions (call fails silently). Part1 Regression_Bug75
            // Hence we verify against different expected content
#if TESTBUILD_CLR40
            VerifyResult("", "abaaba");
#endif
            //prepare selection for API Cut
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("^a");

            QueueDelegate(DoMoreInput12);
        }

        /// <summary>DoInput</summary>
        private void DoMoreInput12()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("cut");
            QueueDelegate(DoMoreInput13);
        }
        /// <summary>DoInput</summary>
        private void DoMoreInput13()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("+{LEFT 3}");
            QueueDelegate(DoMoreInput14);
        }

        /// <summary>DoInput</summary>
        private void DoMoreInput14()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("^x");
            QueueDelegate(DoMoreInput15);
        }
        /// <summary>DoInput</summary>
        private void DoMoreInput15()
        {
            _wrapper.Element.Focus();
            KeyboardInput.TypeString("^v^v");
            QueueDelegate(TestEnd);
        }

        private void TestEnd()
        {
            VerifyResult("", "cutcut");
            bool result = false;            
            
            //Test non accessable APIs.
            try
            {
                Clipboard.SetDataObject("data");
            }
            catch (Exception e)
            {
                result = true;
                Log(e.Message);
            }
            Verifier.Verify(result, "Should not be able to set data to the clipbaord in Partial Trust");

            try
            {
                Clipboard.GetDataObject();
            }
            catch(Exception e)
            {
                result = true;
                Log(e.Message);
            }
            Verifier.Verify(result, "Should not be able to get data  for the clipbaord in Partial Trust");
           
            try
            {
                //Clipboard should have the word "cut" in it. Now try doing a programmatic 
                //paste (by calling Paste() API)
                ((TextBoxBase)_wrapper.Element).Paste();
            }
            catch (Exception e)
            {
                result = true;
                Log(e.Message);
            }
            Verifier.Verify(result, "Should not be able to paste from API in Partial Trust");
            QueueDelegate(NextCombination);
        }


        /// <summary>
        /// Verify the content of a TextEditableType.
        /// </summary>
        /// <param name="selectedText"></param>
        /// <param name="AllText"></param>
       private void VerifyResult(string selectedText, string AllText)
        {
            if (_wrapper.Element is RichTextBox)
            {
                AllText += "\r\n";
            }
            Verifier.Verify(_wrapper.Text == AllText, "After an action, expected Text[" + AllText + "], Actual Text[" + _wrapper.Text + "]!");
            Verifier.Verify(_wrapper.SelectionInstance.Text == selectedText, "After an action, expected Text[" + selectedText + "], Actual Text[" + _wrapper.SelectionInstance.Text + "]!");
        }

        UIElementWrapper _wrapper;
        TextEditableType _editableType = null;
    }

    /// <summary>
    /// Test the constrainedDataObject class.
    /// </summary>
    [TestOwner("Microsoft"), TestBugs("446, 106, 466"), TestTactics("423"), TestWorkItem("")]
    public class ConstrainedDataObjectTest : CustomTestCase
    {
        private UIElementWrapper _wrapper;
        string _text = "Test";
        int _fontsize = 50;
        /// <summary>
        /// Start the ConstrainedDataObjectTest
        /// </summary>
        public override void RunTestCase()
        {
            _wrapper = new UIElementWrapper(new RichTextBox());
            MainWindow.Content = _wrapper.Element;
            _wrapper.Text = _text ;
            if (!TextRangeSaveLoadTest.IsFullTrust())
            {
                ((RichTextBox)_wrapper.Element).FontSize = _fontsize;
            }
            QueueDelegate(SetFocus);
        }

        void SetFocus()
        {
            _wrapper.Element.Focus();
            _wrapper.SelectAll();
            QueueDelegate(PerformTest);
        }

        void PerformTest()
        {
            if (TextRangeSaveLoadTest.IsFullTrust())
            {
                Log("Running in full trust ...");
                //check the pasted format
                DataObject.AddPastingHandler(_wrapper.Element, new DataObjectPastingEventHandler(CheckPasteFormat));
    
                //Perfrom Paste from PT
                ((RichTextBox)_wrapper.Element).Paste();
             
                //Verify the pasted content
                //when Regression_Bug446 is fixed. remove one "\r\n" from the next line
                Verifier.Verify(_wrapper.Text==_text + "\r\n\r\n", "Wrong Text[" + _wrapper.Text + "] is pasted! Expected[" + _text + "]"); 
                
                //Perfrom API Test for the ConstrainedDataObject.
                InputTestConstrainedDataObject();
            }
            else
            {
                Log("Running in partial trust...");
                ((TextBoxBase)_wrapper.Element).Copy();
            }
            Logger.Current.ReportSuccess();
        }

        void CheckPasteFormat(object obj, DataObjectPastingEventArgs args)
        {
            Log("Check the pasted format to be a UnicodeText...");
            Verifier.Verify(args.FormatToApply == DataFormats.UnicodeText, "Wrong dataformat[" + args.FormatToApply + "] is pasted! Expected[" +DataFormats.UnicodeText + "]" );
        }

        void InputTestConstrainedDataObject()
        {
            TestConstrainedDataObject_GetData();
            TestConstrainedDataObject_GetDataPresent();
            TestConstrainedDataObject_GetFormats();
            TestConstrainedDataObject_SetData();
        }

        void TestConstrainedDataObject_SetData()
        {
            IDataObject idobj;
            string message = "Thrown By Test code!";

            Log("Test the ConstrainedDataObject.SetData method...");
            idobj = Test.Uis.Wrappers.ClipboardWrapper.GetDataObject() as IDataObject;
            try
            {
                idobj.SetData("abc");
                throw new Exception(message);
            }
            catch (Exception e)
            {
                Verifier.Verify(e.Message != message, "Wrong message[" + e.Message + "]");
            }
            try
            {
                idobj.SetData(DataFormats.Xaml, "<xaml>", false);
                throw new Exception(message);
            }
            catch (Exception e)
            {
                Verifier.Verify(e.Message != message, "Wrong message[" + e.Message + "]"); 
            }
            try
            {
                idobj.SetData("Microsoft", "MicrosoftData", false);
                throw new Exception(message);
            }
            catch (Exception e)
            {
                Verifier.Verify(e.Message != message, "Wrong message[" + e.Message + "]"); 
            }
            try
            {
                idobj.SetData(typeof(System.Windows.Controls.Button), new Button());
                throw new Exception(message);
            }
            catch (Exception e)
            {
                Verifier.Verify(e.Message != message, "Wrong message[" + e.Message + "]");
            }
        }

        void TestConstrainedDataObject_GetFormats()
        {
            IDataObject idobj;
            string[] formats1;
            string[] formats2;
            
            Log("Test ConstrainedDataObjectTest.GetFormats()...");

            idobj = Test.Uis.Wrappers.ClipboardWrapper.GetDataObject() as IDataObject;
            formats1 = idobj.GetFormats();
            formats2 = idobj.GetFormats(true);
            {
                for (int i = 0; i < formats2.Length; i++)
                {
                    Verifier.Verify(formats1[i] == formats2[i], "Formats won't match for ConstrainedDataObject!");
                }
            }
        }

        void TestConstrainedDataObject_GetDataPresent()
        {
            IDataObject idobj;
            bool bvalue;

            Log("Test ConstrainedDataObject.GEtDataPresent()...");
            idobj = Test.Uis.Wrappers.ClipboardWrapper.GetDataObject() as IDataObject;
            try
            {
                idobj.GetDataPresent((string)null);
                new Exception("We should not accept null as argument of GetDataPresent(string)!");
            }
            catch (ArgumentException e)
            {
                Log(e.Message);
            }
            try
            {
                idobj.GetDataPresent((Type)null);
                new Exception("We should not accept null as argument of GetDataPresent(Type)!");
            }
            catch (ArgumentException e)
            {
                Log(e.Message);
            }
            bvalue = idobj.GetDataPresent(DataFormats.Text);
            Verifier.Verify(bvalue, "We should able to get the Text format from ConstrainedDataObject!");
            bvalue = idobj.GetDataPresent(DataFormats.Xaml);
            Verifier.Verify(!bvalue, "We should not able to get the Xaml format from ConstrainedDataObject!");
            bvalue = idobj.GetDataPresent(typeof(System.Windows.Controls.Button));
            Verifier.Verify(!bvalue, "We should not able to get the Type format from ConstrainedDataObject!");
        }

       void TestConstrainedDataObject_GetData()
        {
            IDataObject idobj;

           Log("Test ConstrainedDataObject_GetData()...");
            object obj;
            idobj = Test.Uis.Wrappers.ClipboardWrapper.GetDataObject() as IDataObject;

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    if (0 == i)
                    {
                        idobj.GetData((string)null);
                    }
                    else if (1 == i)
                    {
                        idobj.GetData(null, false);
                    }
                    else
                    {
                        idobj.GetData((Type)null);
                    }

                    throw new Exception("No exceptoin is thrown when calling ConstrainedDataObject.Getdata(null, false)!");
                }
                catch (Exception e)
                {
                    if (!(e is ArgumentNullException))
                    {
                        throw new Exception("Expect ArgumentNullException!");
                    }
                }
                if (0 == i)
                {
                    obj = idobj.GetData(DataFormats.Xaml);
                }
                else if (1 == i)
                {
                    obj = idobj.GetData(DataFormats.Xaml, true);
                }
                else
                {
                    obj = idobj.GetData(typeof(System.Windows.Controls.Button));
                }

                Verifier.Verify(obj == null, "Xaml should not be avaliable when it is set in PT!");

                if (0 == i)
                {
                    obj = idobj.GetData(DataFormats.Text);
                }
                else if (1 == i)
                {
                    obj = idobj.GetData(DataFormats.Text, true);
                }
                else
                {
                    obj = idobj.GetData(typeof(System.String));
                }

                Verifier.Verify(obj.ToString() == _text + "\r\n", "Falied: expected[" + _text + "\r\n], Actaul[" + obj.ToString() + "]");
            }
        }
    }
}

