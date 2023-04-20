// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Test Binding to a Single Item  from a WebService
	/// This doesn't use indigo, it faking the web service.
	/// </description>
	/// </summary>

    // [DISABLED_WHILE_PORTING]
    // [Test(0, "DataSources", TestCaseSecurityLevel.FullTrust, "SingleWebServiceBasicTest")]
    public class SingleWebServiceBasicTest : XamlTest
	{
        private TextBlock[] _txts;

        public SingleWebServiceBasicTest()
            : base("SingleWebService.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(VerifyContent);
        }

        TestResult Setup()
        {
            _txts = new TextBlock[4];
            _txts[0] = (TextBlock)Util.FindElement(RootElement, "txt0");
            _txts[1] = (TextBlock)Util.FindElement(RootElement, "txt1");
            _txts[2] = (TextBlock)Util.FindElement(RootElement, "txt2");
            _txts[3] = (TextBlock)Util.FindElement(RootElement, "txt3");


            for (int i = 0; i < _txts.Length; i++)
            {
                if (_txts[i] == null)
                {
                    LogComment("Could not reference txt" + i.ToString());
                    return TestResult.Fail;
                }
            }
            LogComment("Referenced TextBlocks correctly");
            return TestResult.Pass;
        }

        private TestResult VerifyContent()
        {
            Status("Verifying test content");
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { String.Format("Title of book {0}", 0.1), String.Format("Title of book {0}", 2.1), String.Format("Title of book {0}", 0.2), String.Format("Title of book {0}", 2.2)};
            

            for (int i = 0; i < _txts.Length; i++)
            {
                if (verify(_txts[i], expected[i]) == TestResult.Fail)
                {
                    return TestResult.Fail;
                }
            }

            LogComment("TextBlocks had the expected value");
            return TestResult.Pass;
        }

        private TestResult verify(TextBlock txt, string expectedText)
        {
		
            if (txt.Text != expectedText)
            {
                LogComment(txt.Name + " was " + txt.Text + " expected " + expectedText);
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }
    }

	/// <summary>
	/// <description>
	/// Test Binding to Multiple Items from a WebService
	/// This doesn't use indigo, it faking the web service.
	/// </description>
	/// </summary>
    // [DISABLED_WHILE_PORTING]
    // [Test(0, "DataSources", TestCaseSecurityLevel.FullTrust, "ListWebServiceBasicTest")]
    public class ListWebServiceBasicTest : XamlTest
    {
        private ListBox[] _lbs;

        public ListWebServiceBasicTest()
            : base("ListWebService.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(VerifyContent);
        }

        TestResult Setup()
        {
            _lbs = new ListBox[4];
            _lbs[0] = (ListBox)Util.FindElement(RootElement, "lst0");
            _lbs[1] = (ListBox)Util.FindElement(RootElement, "lst1");
            _lbs[2] = (ListBox)Util.FindElement(RootElement, "lst2");
            _lbs[3] = (ListBox)Util.FindElement(RootElement, "lst3");


            for (int i = 0; i < _lbs.Length; i++)
            {
                if (_lbs[i] == null)
                {
                    LogComment("Could not reference lst" + i.ToString());
                    return TestResult.Fail;
                }
            }
            LogComment("Referenced ListBoxes correctly");
            return TestResult.Pass;
        }
        private TestResult VerifyContent()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[][] expected  = new string[4][];
            expected[0] = new string[] { String.Format("Title of book {0}", 0.1), String.Format("Title of book {0}", 1.1), String.Format("Title of book {0}", 2.1), String.Format("Title of book {0}", 3.1), String.Format("Title of book {0}", 4.1), String.Format("Title of book {0}", 5.1), String.Format("Title of book {0}", 6.1), String.Format("Title of book {0}", 7.1), String.Format("Title of book {0}", 8.1), String.Format("Title of book {0}", 9.1) };
            expected[1] = new string[] { String.Format("Title of book {0}", 2.1), String.Format("Title of book {0}", 3.1), String.Format("Title of book {0}", 4.1), String.Format("Title of book {0}", 5.1), String.Format("Title of book {0}", 6.1), String.Format("Title of book {0}", 7.1), String.Format("Title of book {0}", 8.1), String.Format("Title of book {0}", 9.1) };
            expected[2] = new string[] { String.Format("Title of book {0}", 0.2), String.Format("Title of book {0}", 1.2), String.Format("Title of book {0}", 2.2), String.Format("Title of book {0}", 3.2), String.Format("Title of book {0}", 4.2), String.Format("Title of book {0}", 5.2), String.Format("Title of book {0}", 6.2), String.Format("Title of book {0}", 7.2), String.Format("Title of book {0}", 8.2), String.Format("Title of book {0}", 9.2) };
            expected[3] = new string[] { String.Format("Title of book {0}", 2.2), String.Format("Title of book {0}", 3.2), String.Format("Title of book {0}", 4.2), String.Format("Title of book {0}", 5.2), String.Format("Title of book {0}", 6.2), String.Format("Title of book {0}", 7.2), String.Format("Title of book {0}", 8.2), String.Format("Title of book {0}", 9.2) };

            string tag = "CLR";
            for (int i = 0; i < _lbs.Length; i++)
            {
                Status("Verifying ListBox " + _lbs[i].Name);
                if (i >1)
                {
                    tag = "XML";
                }
                if (verifyListBox(_lbs[i], expected[i], tag) == TestResult.Fail)
                {
                    return TestResult.Fail;
                }
            }
            LogComment("ListBoxes contained the correct data");
            return TestResult.Pass;
        }

        private TestResult verifyListBox(ListBox lb, string[] expectedText, string expectedTag)
        {
            FrameworkElement[] txts = Util.FindElements(lb, "txt");
            for (int i = 0; i < expectedText.Length; i++)
            {
                Status("Verifying item " + i.ToString());
                if (verifyTextBlock((TextBlock)txts[i], expectedText[i], expectedTag) == TestResult.Fail)
                {
                    LogComment("Incorrect data contained in " + lb.Name);
                    return TestResult.Fail;
                }
            }
            LogComment("ListBox " + lb.Name + " contained the correct information");
            return TestResult.Pass;
        }

        private TestResult verifyTextBlock(TextBlock txt, string expectedText, string expectedTag)
        {

            if (txt.Text != expectedText )
            {
                LogComment(txt.Name + " was " + txt.Text + " expected " + expectedText);
                return TestResult.Fail;
            }
            if (txt.Tag.ToString() != expectedTag)
            {
                LogComment(txt.Name + ".Tag was " + txt.Tag.ToString() + " expected " + expectedTag);
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

    }

}
