// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshWithImplicitBasedOnStyleLookupTest
{
    /******************************************************************************
    * CLASS:          WithImplicitBasedOnStyleLookup
    ******************************************************************************/
    [Test(0, "PropertyEngine.Style", TestCaseSecurityLevel.FullTrust, "WithImplicitBasedOnStyleLookup")]
    public class WithImplicitBasedOnStyleLookup : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("LookUpThemeFirst")]
        [Variation("ThemeNotFoundUsePropertyMetadata")]
        [Variation("NegativeMustBeFrameworkDerived")]
        [Variation("NegativeStyleCannotBeBasedOnSelf")]
        [Variation("NegativeStyleBasedOnHasLoop")]

        /******************************************************************************
        * Function:          WithImplicitBasedOnStyleLookup Constructor
        ******************************************************************************/
        public WithImplicitBasedOnStyleLookup(string arg)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            TestWithImplicitBasedOnStyleLookup test = new TestWithImplicitBasedOnStyleLookup();

            Utilities.StartRunAllTests("WithImplicitBasedOnStyleLookup");

            switch (_testName)
            {
                case "LookUpThemeFirst":
                    test.LookUpThemeFirst();
                    break;
                case "ThemeNotFoundUsePropertyMetadata":
                    test.ThemeNotFoundUsePropertyMetadata();
                    break;
                case "NegativeMustBeFrameworkDerived":
                    test.NegativeMustBeFrameworkDerived();
                    break;
                case "NegativeStyleCannotBeBasedOnSelf":
                    test.NegativeStyleCannotBeBasedOnSelf();
                    break;
                case "NegativeStyleBasedOnHasLoop":
                    test.NegativeStyleBasedOnHasLoop();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            Utilities.StopRunAllTests();

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          TestWithImplicitBasedOnStyleLookup
    ******************************************************************************/
    /// <summary>
    /// If a style isn't explicitly defined in the BasedOn, 
    /// we go look in the theme First
    /// and if we dont find a resource there 
    /// we go and check the default value.
    /// 
    /// </summary>
    public class TestWithImplicitBasedOnStyleLookup
    {
        /// <summary>
        /// Look in the theme First and Find it
        /// </summary>
        public void LookUpThemeFirst()
        {
            Utilities.PrintTitle("Look in the theme First and Find it");

            Style myCheckBoxStyle = new Style(typeof(CheckBox));
            Utilities.Assert(myCheckBoxStyle.BasedOn == null, "myCheckBoxStyle.BasedOn == null");
            CheckBox myCheckBox = new CheckBox();
            myCheckBox.Style = myCheckBoxStyle;
            Utilities.Assert(myCheckBoxStyle.BasedOn == null, "myCheckBoxStyle.BasedOn == null");

        }

        /// <summary>
        /// Theme file lookup failed so DepenendencyProperty Metadata is used
        /// </summary>
        public void ThemeNotFoundUsePropertyMetadata()
        {
            Utilities.PrintTitle("Theme file lookup failed so DepenendencyProperty Metadata is used");

            Utilities.PrintStatus("Test with FrameworkElement-Derived Class");
            //Play with TestCheckBox
            Style myTestCheckBoxStyle = new Style(typeof(TestCheckBox));
            Utilities.Assert(myTestCheckBoxStyle.BasedOn == null, "myTestCheckBoxStyle.BasedOn == null");
            TestCheckBox myTestCheckBox = new TestCheckBox();
            myTestCheckBox.Style = myTestCheckBoxStyle;
            Utilities.Assert(myTestCheckBoxStyle.BasedOn == null, "myTestCheckBoxStyle.BasedOn == null");

            Utilities.PrintStatus("Test with FrameworkContentElement-Derived Class");

            Style myTestColumnStyle = new Style(typeof(TestColumn));
            Utilities.Assert(myTestColumnStyle.BasedOn == null, "myTestColumnStyle.BasedOn == null");
            TestColumn myTestColumn = new TestColumn();
            myTestColumn.Style = myTestColumnStyle;
            Utilities.Assert(FrameworkContentElement.StyleProperty.GetMetadata(myTestColumn).DefaultValue == null,
              "FrameworkContentElement.StyleProperty.GetMetadata(myTestColumn).DefaultValue == null");
            //Because getMetadata gets null for Styleroperty, BasedOn should still be null
            Utilities.Assert(myTestColumnStyle.BasedOn == null, "myTestColumnStyle.BasedOn == null");
        }

        /// <summary>
        /// Negative Test: TargetType must be FrameworkDerived
        /// </summary>
        public void NegativeMustBeFrameworkDerived()
        {
            Utilities.PrintTitle("Negative Test: TargetType must be FrameworkDerived");

            Style myStyle = new Style();
            try
            {
                myStyle.TargetType = typeof(System.Windows.Media.SkewTransform);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /// <summary>
        /// Negative Test: A Style cannot be based on itself.
        /// </summary>
        public void NegativeStyleCannotBeBasedOnSelf()
        {
            Utilities.PrintTitle("Negative Test: A Style cannot be based on itself.");
            Style myStyle = new Style();
            try
            {
                myStyle.BasedOn = myStyle;
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /// <summary>
        /// A loop has been detected in this Style's hierarchy of BasedOn references.
        /// </summary>
        public void NegativeStyleBasedOnHasLoop()
        {
            Utilities.PrintTitle("Negative Test: A loop has been detected in this Style's hierarchy of BasedOn references.");
            Style[] myStyles = new Style[11];
            myStyles[0] = new Style();
            for (int i = 1; i < myStyles.Length; i++)
            {
                myStyles[i] = new Style();
                myStyles[i].BasedOn = myStyles[i - 1];
            }
            myStyles[0].BasedOn = myStyles[myStyles.Length - 1];


            Button myButton = new Button();
            try
            {
                myButton.Style = myStyles[0];
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }


        }

        /// <summary>
        /// For 

        public void BugRepro()
        {
            Utilities.PrintTitle("Bug : InvalidCastException");

            //Play With Column 


            ////        Style myCheckBoxStyle = new Style(typeof(CheckBox)).
            ////        System.Windows.Documents.Column myColumn = new System.Windows.Documents.Column().
            ////        myColumn.Style = myCheckBoxStyle.
            ////
            // Why there is no exception here?
            //        Style myStyle = new Style(typeof(System.Windows.Documents.Column)).
            //        CheckBox box = new CheckBox().
            //        box.Style = myStyle.

            //        Style myColumnStyle = new Style(typeof(System.Windows.Documents.Column)).
            //        //        ZBHelper.Assert(myColumnStyle.BasedOn == null, "").
            //        System.Windows.Documents.Column myColumn = new System.Windows.Documents.Column().
            //        myColumn.Style = myColumnStyle.
            //        ZBHelper.Assert(myColumnStyle.BasedOn != null, "").

            //                Style myTestColumnStyle = new Style(typeof(TestColumn)).
            //                ZBHelper.Assert(myTestColumnStyle.BasedOn == null, "").
            //                TestColumn myTestColumn = new TestColumn().
            //                myTestColumn.Style = myTestCheckBoxStyle.
            //                ZBHelper.Assert(myTestColumnStyle.BasedOn != null, "").

        }
    }



    /******************************************************************************
    * CLASS:          TestCheckBox
    ******************************************************************************/
    /// <summary>
    /// Just a class that derives from CheckBox. Theme know nothing about it.
    /// This class is ultimately deriving from FrameowrkElement
    /// </summary>
    public class TestCheckBox : CheckBox
    {
    }

    /******************************************************************************
    * CLASS:          TestColumn
    ******************************************************************************/
    /// <summary>
    /// Just a class that is ultimately deriving from FrameworkContentElement
    /// </summary>
    public class TestColumn : System.Windows.Documents.TableColumn
    {
    }
}
