// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Markup;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.MarkupExtensionTests
{
    /******************************************************************************
    * CLASS:          ArrayExtensionTests
    ******************************************************************************/

    /// <summary>
    ///  ArrayExtension Tests.
    /// </summary>
    public class ArrayExtensionTests
    {
       /******************************************************************************
        * Function:          VerifyArrayExtensionMethods
        ******************************************************************************/

        /// <summary>
        /// Verifies ArrayExtension scenarios.
        /// </summary>
        public static void VerifyArrayExtensionMethods()
        {
            bool testPassed = true;

            /************************************************************************
            / Test #1
            /************************************************************************/
            GlobalLog.LogEvidence("Test #1: Verify Defaults");

            ArrayExtension extension1 = new ArrayExtension();

            VerifyType(null, extension1.Type, ref testPassed);
            VerifyCount(0, extension1.Items.Count, ref testPassed);

            /************************************************************************
            / Test #2
            /************************************************************************/
            GlobalLog.LogEvidence("Test #2: Verify calling AddChild and AddText - ctor()");

            double expectedItem2a = 100d;
            string expectedItem2b = "abc";
            int expectedCount2 = 2;
            
            ArrayExtension extension2 = new ArrayExtension();

            extension2.AddChild(expectedItem2a);
            extension2.AddText(expectedItem2b);

            if (VerifyCount(expectedCount2, extension2.Items.Count, ref testPassed))
            {
                 VerifyItems(0, expectedItem2a, (double)extension2.Items[0], ref testPassed);
                 VerifyItems(1, expectedItem2b, (string)extension2.Items[1], ref testPassed);
            }

            VerifyType(null, extension2.Type, ref testPassed);

            /************************************************************************
            / Test #3
            /************************************************************************/
            GlobalLog.LogEvidence("Test #3: Verify calling AddChild and AddText - ctor(Array)");

            int[] array3 = new int[1];
            array3[0] = 333;
            int expectedItem3a = array3[0];
            string expectedItem3b = "string3b";
            bool expectedItem3c = true;
            Type expectedType3 = typeof(int);
            int expectedCount3 = 3;

            ArrayExtension extension3 = new ArrayExtension(array3);
            
            extension3.AddText(expectedItem3b);
            extension3.AddChild(expectedItem3c);

            if (VerifyCount(expectedCount3, extension3.Items.Count, ref testPassed))
            {
                VerifyItems(0, expectedItem3a, (int)extension3.Items[0], ref testPassed);
                VerifyItems(1, expectedItem3b, (string)extension3.Items[1], ref testPassed);
                VerifyItems(2, expectedItem3c, (bool)extension3.Items[2], ref testPassed);
            }

            VerifyType(expectedType3, extension3.Type, ref testPassed);

            /************************************************************************
            / Test #4
            /************************************************************************/
            GlobalLog.LogEvidence("Test #4: Verify calling AddChild and AddText - ctor(Type)");

            string expectedItem4a = "string4a";
            string expectedItem4b = "string4b";
            int expectedCount4 = 200;
            int num = 444;
            Type expectedType4 = num.GetType();

            ArrayExtension extension4 = new ArrayExtension(expectedType4);
            
            for (int i = 0; i < expectedCount4 - 1; i++)
            {
                extension4.AddChild(expectedItem4a);
            }

            extension4.AddText(expectedItem4b);

            if (VerifyCount(expectedCount4, extension4.Items.Count, ref testPassed))
            {
                int j = 0;
                while (j < extension4.Items.Count - 1)
                {
                    VerifyItems(j, expectedItem4a, (string)extension4.Items[j], ref testPassed);
                    j++;
                }

                VerifyItems(j, expectedItem4b, (string)extension4.Items[extension4.Items.Count - 1], ref testPassed);
            }

            VerifyType(expectedType4, extension4.Type, ref testPassed);

            /************************************************************************
            / Test #5
            /************************************************************************/
            GlobalLog.LogEvidence("Test #5: Verify passing nulls");

            int expectedCount5 = 2;
            
            ArrayExtension extension5 = new ArrayExtension();

            extension5.AddChild(null);
            extension5.AddText(null);

            if (VerifyCount(expectedCount5, extension5.Items.Count, ref testPassed))
            {
                VerifyItemsNull(0, extension5.Items[0], ref testPassed);
                VerifyItemsNull(1, extension5.Items[1], ref testPassed);
            }

            VerifyType(null, extension5.Type, ref testPassed);

            /************************************************************************
            / Final Pass/Fail
            /************************************************************************/

            if (testPassed)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                TestLog.Current.Result = TestResult.Fail;
            }
        }

       /******************************************************************************
        * Function:          VerifyCount
        ******************************************************************************/

        /// <summary>
        /// Verifies the Items Count.
        /// </summary>
        /// <param name="expected">The expected Count.</param>
        /// <param name="actual">The actual Count.</param>
        /// <param name="passed">A bool indicating whether or not the expected matched the actual.</param>
        /// <returns>A boolean indicating whether or not the count was correct.</returns>
        public static bool VerifyCount(int expected, int actual, ref bool passed)
        {
            bool countCorrect = true;

            if (!actual.Equals(expected))
            {
                GlobalLog.LogEvidence("---------FAIL: Count is incorrect. Expected: " + expected.ToString() + " / Actual: " + actual.ToString());
                passed = false;
                countCorrect = false;
            }

            return countCorrect;
        }

       /******************************************************************************
        * Function:          VerifyItems
        ******************************************************************************/

        /// <summary>
        /// Verifies the content of Items returned by ArrayExtension.
        /// </summary>
        /// <param name="itemNumber">The number of the item in the Items collection being verified.</param>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="expected">The expected value of the Item.</param>
        /// <param name="actual">The actual value of the Item.</param>
        /// <param name="passed">A bool indicating whether or not the expected matched the actual.</param>
        public static void VerifyItems<T>(int itemNumber, T expected, T actual, ref bool passed)
        {
            if (!actual.Equals(expected))
            {
                GlobalLog.LogEvidence("---------FAIL: Item #" + itemNumber.ToString() + " is incorrect.  Expected: " + expected.ToString() + " / Actual: " + actual.ToString());
                passed = false;
            }
        }

       /******************************************************************************
        * Function:          VerifyItemsNull
        ******************************************************************************/

        /// <summary>
        /// Verifies the content of Items returned by ArrayExtension when the expected value is null.
        /// </summary>
        /// <param name="itemNumber">The number of the item in the Items collection being verified.</param>
        /// <param name="actual">The actual value of the Item.</param>
        /// <param name="passed">A bool indicating whether or not the expected matched the actual.</param>
        public static void VerifyItemsNull(int itemNumber, object actual, ref bool passed)
        {
            if (actual != null)
            {
                GlobalLog.LogEvidence("---------FAIL: Item #" + itemNumber.ToString() + " is incorrect.  Expected: null" + " / Actual: " + actual.ToString());
                passed = false;
            }
        }

       /******************************************************************************
        * Function:          VerifyType
        ******************************************************************************/

        /// <summary>
        /// Verifies the Type returned by ArrayExtension.
        /// </summary>
        /// <param name="expected">The expected value of the Type.</param>
        /// <param name="actual">The actual value of the Type.</param>
        /// <param name="passed">A bool indicating whether or not the expected matched the actual.</param>
        public static void VerifyType(Type expected, Type actual, ref bool passed)
        {
            if (expected != actual)
            {
                if (expected == null)
                {
                    GlobalLog.LogEvidence("---------FAIL: the Type returned is incorrect.  Expected: null / Actual: " + actual.ToString());
                }
                else if (actual == null)
                {
                    GlobalLog.LogEvidence("---------FAIL: the Type returned is incorrect.  Expected: " + expected.ToString() + " / Actual: null");
                }
                else
                {
                    GlobalLog.LogEvidence("---------FAIL: the Type returned is incorrect.  Expected: " + expected.ToString() + " / Actual: " + actual.ToString());
                }

                passed = false;
            }
        }
    }
}
