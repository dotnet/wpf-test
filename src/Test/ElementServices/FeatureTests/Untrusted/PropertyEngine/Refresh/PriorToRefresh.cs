// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshPriorToRefreshTest
{
    /******************************************************************************
    * CLASS:          PriorToRefresh
    ******************************************************************************/
    [Test(0, "PropertyEngine.PriorToRefresh", TestCaseSecurityLevel.FullTrust, "PriorToRefresh")]
    public class PriorToRefresh : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("BvtContainerBindings1")]
        [Variation("FrameworkElementFactoryBvtTestCases")]
        [Variation("MetadataCtors")]
        [Variation("PropertySetter")]
        [Variation("SealedPropertySetter")]
        [Variation("DpRegister")]
        [Variation("OverrideMetadataPositive")]
        [Variation("OverrideMetadataNegative")]
        [Variation("GetMetadata")]
        [Variation("TestAddOwner")]
        [Variation("Scenario1To6")]
        [Variation("NegativeTestCases")]
        [Variation("GetSetClearSimpleDirect")]
        [Variation("GetSetClearSimpleDirectButSetDefaultValueWithMetadata")]
        [Variation("DeriveOverride")]

        /******************************************************************************
        * Function:          PriorToRefresh Constructor
        ******************************************************************************/
        public PriorToRefresh(string arg)
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
            Utilities.StartRunAllTests("PriorToRefresh");
            TestPriorToRefresh test = new TestPriorToRefresh();

            switch (_testName)
            {
                case "BvtContainerBindings1":
                    test.BvtContainerBindings1();
                    break;
                case "FrameworkElementFactoryBvtTestCases":
                    test.FrameworkElementFactoryBvtTestCases();
                    break;
                case "MetadataCtors":
                    test.MetadataCtors();
                    break;
                case "PropertySetter":
                    test.PropertySetter();
                    break;
                case "SealedPropertySetter":
                    test.SealedPropertySetter();
                    break;
                case "DpRegister":
                    test.DpRegister();
                    break;
                case "OverrideMetadataPositive":
                    test.OverrideMetadataPositive();
                    break;
                case "OverrideMetadataNegative":
                    test.OverrideMetadataNegative();
                    break;
                case "GetMetadata":
                    test.GetMetadata();
                    break;
                case "TestAddOwner":
                    test.TestAddOwner();
                    break;
                case "Scenario1To6":
                    test.Scenario1To6();
                    break;
                case "NegativeTestCases":
                    test.NegativeTestCases();
                    break;
                case "GetSetClearSimpleDirect":
                    test.GetSetClearSimpleDirect();
                    break;
                case "GetSetClearSimpleDirectButSetDefaultValueWithMetadata":
                    test.GetSetClearSimpleDirectButSetDefaultValueWithMetadata();
                    break;
                case "DeriveOverride":
                    test.DeriveOverride();
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
    * CLASS:          TestPriorToRefresh
    ******************************************************************************/
    public class TestPriorToRefresh
    {
        #region TemplateBindingExtension (used to be AliasProperty)
        /// <summary>
        /// </summary>
        public void BvtContainerBindings1()
        {
            Utilities.PrintTitle("Test On Framework Element Directly");

            VerifyTestOnFrameworkElementFactoryDirectly(null);
        }

        private object VerifyTestOnFrameworkElementFactoryDirectly(object arg)
        {
            FrameworkElementFactory fef1 = new FrameworkElementFactory(typeof(System.Windows.Controls.Canvas));

            //Call AliasProperty overload 1 with null (1 test case)
            try
            {
                fef1.SetValue(null, new TemplateBindingExtension(null));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

            //Call AliasProperty overliad 2 with null (2 test cases)
            try
            {
                fef1.SetValue(null, new TemplateBindingExtension(System.Windows.Controls.Canvas.BottomProperty));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

            try
            {
                fef1.SetValue(System.Windows.Controls.Canvas.BottomProperty, new TemplateBindingExtension(null));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

            //source should be assignable from destination  (1 case)
            /* Comment out by developer 
            try
            {
                fef1.SetValue(System.Windows.Controls.Button.BackgroundProperty, new TemplateBindingExtension(System.Windows.Controls.CheckBox.CheckedProperty));
                ZBHelper.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                ZBHelper.ExpectedExceptionReceived(ex);
            }
                        */

            //Alias the same property should definitely work
            fef1.SetValue(System.Windows.Controls.Button.BackgroundProperty, new TemplateBindingExtension(System.Windows.Controls.Button.BackgroundProperty));
            fef1.SetValue(System.Windows.Controls.Canvas.ClipToBoundsProperty, new TemplateBindingExtension(System.Windows.Controls.Canvas.ClipToBoundsProperty));
            Utilities.PrintStatus("Alias the same property should definitely work");

            //Alias two compatible properties
            fef1.SetValue(System.Windows.Controls.Button.FocusableProperty, new TemplateBindingExtension(System.Windows.Controls.Canvas.IsEnabledProperty));
            Utilities.PrintStatus("Alias compatiple properties also work");

            //Seal it
            System.Windows.Style myStyle = new System.Windows.Style();
            ControlTemplate template = new ControlTemplate(typeof(Button));
            template.VisualTree = fef1;
            myStyle.Setters.Add(new Setter(Button.TemplateProperty, template));
            Button testBtn = new Button();
            testBtn.Style = myStyle;

            try
            {
                fef1.SetValue(System.Windows.Controls.Control.BorderBrushProperty, new TemplateBindingExtension(System.Windows.Controls.Control.BorderBrushProperty));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            return null;
        }

        ///<summary>
        /// FrameworkElementFactory BVT test cases so far. Needs LH machine to run
        ///</summary>
        public void FrameworkElementFactoryBvtTestCases()
        {
            Utilities.PrintTitle("FrameworkElementFactory BVT test cases so far. Needs LH machine to run");
        }
        #endregion

        #region Test System.Windows.PropertyMetadata (Keyword: PropertyMetadata)
        /// <summary>
        /// Called by MetadataCtors to reuse validation code
        /// </summary>
        private void PropertyMetadataValidation(PropertyMetadata metadata, object defaultValueExpected, PropertyChangedCallback propertyChangedCallbackExpected, CoerceValueCallback coerceValueCallbackExpected)
        {
            if (metadata.DefaultValue != null)
            {
                Utilities.Assert(metadata.DefaultValue.Equals(defaultValueExpected), "DefaultValue as expected.");
            }
            else
            {
                Utilities.Assert(defaultValueExpected == null, "DefaultValue is null.");
            }
            if(metadata.PropertyChangedCallback != null)
            {
            Utilities.Assert(metadata.PropertyChangedCallback.Equals(propertyChangedCallbackExpected), "PropertyChangedCallback as expected.");
            }
            else{
                Utilities.Assert(propertyChangedCallbackExpected == null, "PropertyChangedCallback is null.");
            }
            if (metadata.CoerceValueCallback != null)
            {
                Utilities.Assert(metadata.CoerceValueCallback.Equals(coerceValueCallbackExpected), "CoerceValueCallback as expected.");
            }
            else
            {
                Utilities.Assert(coerceValueCallbackExpected == null, "CoerceValueCallback is null.");
            }
        }

        ///<summary>
        /// Test all System.Windows.PropertyMetadata constructors 
        ///</summary>
        public void MetadataCtors()
        {
            PropertyChangedCallback pcCallback = new PropertyChangedCallback(HandlerPropertyChanged);
            CoerceValueCallback cvCallback = new CoerceValueCallback(HandlerCoerceValueCallback);

            Utilities.PrintTitle("Test Overloaded PropertyMetadata constructors as well as public property getter");
            Utilities.PrintStatus("Five (5) constructors. Four (4) public properties: DefaultValue, PropertyChangedCallback, CoerceValueCallback, ReadOnly");
            Utilities.PrintStatus("ReadOnly is RO property, while others are R/W property");

            Utilities.PrintStatus("[1] Constructor with No parameter");
            PropertyMetadata metaA = new PropertyMetadata();
            PropertyMetadataValidation(metaA, null, null, null);

            Utilities.PrintStatus("[2] Constructor with defaultValue only ");
            PropertyMetadata metaB = new PropertyMetadata(12345);
            PropertyMetadataValidation(metaB, 12345, null, null);

            Utilities.PrintStatus("[3] Constructor with PropertyChangedCallback only ");
            PropertyMetadata metaC = new PropertyMetadata(pcCallback);
            PropertyMetadataValidation(metaC, null, pcCallback, null);

            Utilities.PrintStatus("[4] Constructor with both defaultValue and propertyChangedCallback.");
            PropertyMetadata metaD = new PropertyMetadata("TestString", pcCallback);
            PropertyMetadataValidation(metaD, "TestString", pcCallback, null);

            Utilities.PrintStatus("[5] Constructor with defaultValue, propertyChangedCallback and coerceValueCallback.");
            PropertyMetadata metaE = new PropertyMetadata(99, pcCallback, cvCallback);
            PropertyMetadataValidation(metaE, 99, pcCallback, cvCallback);
        }

        /// <summary>
        ///Used as PropertyChanged callback handler
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void HandlerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Used as CoerceValueCallback handler
        /// </summary>
        /// <param name="d">DependencyObject</param>
        /// <param name="baseValue">Base Value</param>
        /// <returns>coerced value</returns>
        private static object HandlerCoerceValueCallback(DependencyObject d, object baseValue)
        {
            return baseValue;
        }

        ///<summary>
        ///Test PropertyMetadata public property setter of 
        ///(1) DefaultValue 
        ///(2) PropertyChangedCallback 
        ///(3) CoerceValueCallback 
        /// 
        /// For negative test cases that setting property values when PropertyMetadata is sealed
        /// See SealedPropertySetter() below.
        ///</summary>
        public void PropertySetter()
        {
            PropertyChangedCallback pcCallback = new PropertyChangedCallback(HandlerPropertyChanged);
            CoerceValueCallback cvCallback = new CoerceValueCallback(HandlerCoerceValueCallback);

            Utilities.PrintTitle("Test PropertyMetadata public property setter of (1) DefaultValue (2)PropertyChangedCallback (3)CoerceValueCallback");
            PropertyMetadata meta = new PropertyMetadata();
            Utilities.PrintStatus("(1) DefaultValue");
            meta.DefaultValue = "12345";
            meta.DefaultValue = 99;
            PropertyMetadataValidation(meta, 99, null, null);

            Utilities.PrintStatus("(2)PropertyChangedCallback ");
            meta.PropertyChangedCallback = pcCallback;
            PropertyMetadataValidation(meta, 99, pcCallback, null);

            Utilities.PrintStatus("(3)GetValueOverride ");
            meta.CoerceValueCallback = cvCallback;
            PropertyMetadataValidation(meta, 99, pcCallback, cvCallback);
        }

        ///<summary>
        ///When PropertyMetadata is sealed, property setter for 
        ///(1) DefaultValue 
        ///(2) PropertyChangedCallback 
        ///(3) CoerceValueCallback 
        ///should throw InvalidOperationException.
        ///</summary>
        public void SealedPropertySetter()
        {
            Utilities.PrintTitle("When PropertyMetadata is sealed, property setter for (1) DefaultValue (2)PropertyChangedCallback (3)CoerceValueCallback should throw InvalidOperationException. ");
            Utilities.PrintStatus("(1) DefaultValue");
            try
            {
                AccessButton03.AccessMeta.DefaultValue = "Try After Sealed";
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

            Utilities.PrintStatus("(2) PropertyChangedCallback ");
            try
            {
                AccessButton03.AccessMeta.PropertyChangedCallback = null;
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

            Utilities.PrintStatus("(3) CoerceValueCallback ");
            try
            {
                AccessButton03.AccessMeta.CoerceValueCallback = null;
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        ///<summary>
        /// Test DependencyProperty.RegisterAttached 
        ///</summary>
        public void DpRegister()
        {
            Utilities.PrintTitle("Straightforward DependencyProperty.RegisterAttached, both positive and negative");

            Utilities.PrintStatus("(1) Positive test case");
            Utilities.PrintStatus("(a) Register without providing defaultMetadata. System will provide one.");
            Utilities.PrintDependencyProperty(ExcelButton03.OneAProperty);
            Utilities.Assert(ExcelButton03.OneAProperty.Name == "1A", "");
            Utilities.Assert(ExcelButton03.OneAProperty.DefaultMetadata != null, "");
            Utilities.PrintStatus("(b) Register with defaultMetadata");
            Utilities.PrintDependencyProperty(ExcelButton03.OneBProperty);
            Utilities.Assert(ExcelButton03.OneBProperty.Name == "1B", "");
            Utilities.Assert((string)ExcelButton03.OneBProperty.DefaultMetadata.DefaultValue == "One Little Two Little", "");

            Utilities.PrintStatus("(2) Negative test case");
            ExcelButton03.ScenarioA();
            ExcelButton03.ScenarioB();
            ExcelButton03.ScenarioC();
            ExcelButton03.ScenarioD();
            ExcelButton03.ScenarioE();
        }

        ///<summary>
        /// Positive DependencyProperty.OverrideMetadata. In seven scenarios.
        ///</summary>
        public void OverrideMetadataPositive()
        {
            Utilities.PrintTitle("Positive DependencyProperty.OverrideMetadata. In seven scenarios.");
            CreateObjects(null);

            object obj, obj1, obj2, obj3;
            Utilities.PrintStatus("(1) Register and OverrideMetadata in the same class");
            obj = _btn.GetValue(FrontpageButton03.FirstProperty);
            obj1 = _fBtn.GetValue(FrontpageButton03.FirstProperty);
            obj2 = _fBtnDa.GetValue(FrontpageButton03.FirstProperty);
            obj3 = _fBtnDccc.GetValue(FrontpageButton03.FirstProperty);
            Utilities.PrintStatus("AccessButton03.GetValue is " + obj.ToString());
            Utilities.PrintStatus("FrontpageButton03.GetValue is " + obj1.ToString());
            Utilities.PrintStatus("FrontpageButton03DeriveA.GetValue is " + obj2.ToString());
            Utilities.PrintStatus("FrontpageButton03DeriveCCC.GetValue is " + obj3.ToString());
            Utilities.Assert(((string)obj).StartsWith("Default"), "");
            Utilities.Assert(((string)obj1).StartsWith("Override"), "");
            Utilities.Assert(((string)obj2).StartsWith("Override"), "");
            Utilities.Assert(((string)obj3).StartsWith("Override"), "");

            Utilities.PrintStatus("(2) Register only in base class, OverrideMetadata in immediately derived class ");
            obj = _btn.GetValue(FrontpageButton03.SecondProperty);
            obj1 = _fBtn.GetValue(FrontpageButton03.SecondProperty);
            obj2 = _fBtnDa.GetValue(FrontpageButton03.SecondProperty);
            obj3 = _fBtnDccc.GetValue(FrontpageButton03.SecondProperty);
            Utilities.PrintStatus("AccessButton03.GetValue is " + obj.ToString());
            Utilities.PrintStatus("FrontpageButton03.GetValue is " + obj1.ToString());
            Utilities.PrintStatus("FrontpageButton03DeriveA.GetValue is " + obj2.ToString());
            Utilities.PrintStatus("FrontpageButton03DeriveCCC.GetValue is " + obj3.ToString());
            Utilities.Assert(((string)obj).StartsWith("Default"), "");
            Utilities.Assert(((string)obj1).StartsWith("Default"), "");
            Utilities.Assert(((string)obj2).StartsWith("Override"), "");
            Utilities.Assert(((string)obj3).StartsWith("Override"), "");

            Utilities.PrintStatus("(3) Register only in base class, OverrideMetadata in derived class removed by more than 2");
            obj = _btn.GetValue(FrontpageButton03.thirdProperty);
            obj1 = _fBtn.GetValue(FrontpageButton03.thirdProperty);
            obj2 = _fBtnDa.GetValue(FrontpageButton03.thirdProperty);
            obj3 = _fBtnDccc.GetValue(FrontpageButton03.thirdProperty);
            Utilities.PrintStatus("AccessButton03.GetValue is " + obj.ToString());
            Utilities.PrintStatus("FrontpageButton03.GetValue is " + obj1.ToString());
            Utilities.PrintStatus("FrontpageButton03DeriveA.GetValue is " + obj2.ToString());
            Utilities.PrintStatus("FrontpageButton03DeriveCCC.GetValue is " + obj3.ToString());
            Utilities.Assert(((string)obj).StartsWith("Default"), "");
            Utilities.Assert(((string)obj1).StartsWith("Default"), "");
            Utilities.Assert(((string)obj2).StartsWith("Default"), "");
            Utilities.Assert(((string)obj3).StartsWith("Override"), "");

            Utilities.PrintStatus("(4) Register only in immediately derived class and OverrideMetadata in base class ");
            obj = _btn.GetValue(FrontpageButton03DeriveA.FourthProperty);
            obj1 = _fBtn.GetValue(FrontpageButton03DeriveA.FourthProperty);
            obj2 = _fBtnDa.GetValue(FrontpageButton03DeriveA.FourthProperty);
            obj3 = _fBtnDccc.GetValue(FrontpageButton03DeriveA.FourthProperty);
            Utilities.PrintStatus("AccessButton03.GetValue is " + obj.ToString());
            Utilities.PrintStatus("FrontpageButton03.GetValue is " + obj1.ToString());
            Utilities.PrintStatus("FrontpageButton03DeriveA.GetValue is " + obj2.ToString());
            Utilities.PrintStatus("FrontpageButton03DeriveCCC.GetValue is " + obj3.ToString());
            Utilities.Assert(((string)obj).StartsWith("Default"), "");
            Utilities.Assert(((string)obj1).StartsWith("Override"), "");
            Utilities.Assert(((string)obj2).StartsWith("Override"), "");
            Utilities.Assert(((string)obj3).StartsWith("Override"), "");

            Utilities.PrintStatus("(5) Register only in derived class removed by more than 2, Override metadata in base class ");
            obj = _btn.GetValue(FrontpageButton03DeriveCCC.FifthProperty);
            obj1 = _fBtn.GetValue(FrontpageButton03DeriveCCC.FifthProperty);
            obj2 = _fBtnDa.GetValue(FrontpageButton03DeriveCCC.FifthProperty);
            obj3 = _fBtnDccc.GetValue(FrontpageButton03DeriveCCC.FifthProperty);
            Utilities.PrintStatus("AccessButton03.GetValue is " + obj.ToString());
            Utilities.PrintStatus("FrontpageButton03.GetValue is " + obj1.ToString());
            Utilities.PrintStatus("FrontpageButton03DeriveA.GetValue is " + obj2.ToString());
            Utilities.PrintStatus("FrontpageButton03DeriveCCC.GetValue is " + obj3.ToString());
            Utilities.Assert(((string)obj).StartsWith("Default"), "");
            Utilities.Assert(((string)obj1).StartsWith("Override"), "");
            Utilities.Assert(((string)obj2).StartsWith("Override"), "");
            Utilities.Assert(((string)obj3).StartsWith("Override"), "");

            Utilities.PrintStatus("(6) Register in a class (no need to derive from DependencyObject), OverrideMetadata in another class (that has derived classes) ");
            obj = _btn.GetValue(InfoPathButton03.SixthProperty);
            obj1 = _fBtn.GetValue(InfoPathButton03.SixthProperty);
            obj2 = _fBtnDa.GetValue(InfoPathButton03.SixthProperty);
            obj3 = _fBtnDccc.GetValue(InfoPathButton03.SixthProperty);
            Utilities.PrintStatus("AccessButton03.GetValue is " + obj.ToString());
            Utilities.PrintStatus("FrontpageButton03.GetValue is " + obj1.ToString());
            Utilities.PrintStatus("FrontpageButton03DeriveA.GetValue is " + obj2.ToString());
            Utilities.PrintStatus("FrontpageButton03DeriveCCC.GetValue is " + obj3.ToString());
            Utilities.Assert(((string)obj).StartsWith("Default"), "");
            Utilities.Assert(((string)obj1).StartsWith("Override"), "");
            Utilities.Assert(((string)obj2).StartsWith("Override"), "");
            Utilities.Assert(((string)obj3).StartsWith("Override"), "");

            Utilities.PrintStatus("(7) Register in a class, OverrideMetadata is another class (that has base classes). ");
            obj = _btn.GetValue(InfoPathButton03.SeventhProperty);
            obj1 = _fBtn.GetValue(InfoPathButton03.SeventhProperty);
            obj2 = _fBtnDa.GetValue(InfoPathButton03.SeventhProperty);
            obj3 = _fBtnDccc.GetValue(InfoPathButton03.SeventhProperty);
            Utilities.PrintStatus("AccessButton03.GetValue is " + obj.ToString());
            Utilities.PrintStatus("FrontpageButton03.GetValue is " + obj1.ToString());
            Utilities.PrintStatus("FrontpageButton03DeriveA.GetValue is " + obj2.ToString());
            Utilities.PrintStatus("FrontpageButton03DeriveCCC.GetValue is " + obj3.ToString());
            Utilities.Assert(((string)obj).StartsWith("Default"), "");
            Utilities.Assert(((string)obj1).StartsWith("Default"), "");
            Utilities.Assert(((string)obj2).StartsWith("Default"), "");
            Utilities.Assert(((string)obj3).StartsWith("Override"), "");
        }
        private AccessButton03 _btn;
        private FrontpageButton03 _fBtn;
        private FrontpageButton03DeriveA _fBtnDa;
        private FrontpageButton03DeriveCCC _fBtnDccc;

        private object CreateObjects(object arg)
        {
            _btn = new AccessButton03();
            _fBtn = new FrontpageButton03();
            _fBtnDa = new FrontpageButton03DeriveA();
            _fBtnDccc = new FrontpageButton03DeriveCCC();
            return null;
        }

        ///<summary>
        /// Negative DependencyProperty.OverrideMetadata. In eight scenarios.
        ///</summary>
        public void OverrideMetadataNegative()
        {
            Utilities.PrintTitle("Negative DependencyProperty.OverrideMetadata. In eight scenarios.");

            WordButton03DeriveA test = (WordButton03DeriveA)CreateObject(null);
            test.ScenarioA();
            test.ScenarioB();
            test.ScenarioC();
            test.ScenarioD();
            Utilities.PrintStatus("Scenario E ");
            test.ScenarioE();
            test.ScenarioF();
            Utilities.PrintStatus("Scenario G ");
            //test.ScenarioG();
            test.ScenarioH();
        }

        private object CreateObject(object arg)
        {
            WordButton03DeriveA test = new WordButton03DeriveA();
            return test;
        }

        ///<summary>
        /// DependencyProperty.GetMetadata should always return non-null PropertyMetadata 
        /// Keyword: DependencyProperty.GetMetadata
        ///</summary>
        public void GetMetadata()
        {
            Utilities.PrintTitle("DependencyProperty.GetMetadata should always return non-null PropertyMetadata.");
            PropertyMetadata meta = null;
            PropertyMetadata meta1 = null;

            Utilities.PrintStatus("(1) Argument Validation");
            try
            {
                meta = FrontpageButton03.FirstProperty.GetMetadata((Type)null);
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            try
            {
                meta = FrontpageButton03.FirstProperty.GetMetadata((Type)null);
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            Utilities.PrintStatus("(2) GetMetadata used in OverrideMetadataPositive");
            Utilities.PrintStatus("(A) Register and OverrideMetadata in the same class ");
            meta = FrontpageButton03.FirstProperty.GetMetadata(typeof(FrontpageButton03));
            Utilities.Assert(meta.DefaultValue.ToString().StartsWith("Override"), "");
            Utilities.Assert(meta.PropertyChangedCallback.GetInvocationList().Length == 2, "");
            Utilities.Assert(FrontpageButton03.FirstProperty.ReadOnly == false, "");
            meta1 = FrontpageButton03.FirstProperty.GetMetadata(typeof(FrontpageButton03DeriveCCC));
            Utilities.Assert(meta == meta1, "");

            Utilities.PrintStatus("(B) Register only in base class, OverrideMetadata in immediately derived class ");
            meta = FrontpageButton03.SecondProperty.GetMetadata(typeof(FrontpageButton03));
            meta1 = FrontpageButton03.SecondProperty.GetMetadata(typeof(FrontpageButton03DeriveA));
            Utilities.Assert(meta != meta1, "");
            meta = FrontpageButton03.SecondProperty.GetMetadata(typeof(FrontpageButton03DeriveCCC));
            Utilities.Assert(meta == meta1, "");

            Utilities.PrintStatus("(C) Register only in base class, OverrideMetadata in derived class removed by more than 2");
            meta = FrontpageButton03.thirdProperty.GetMetadata(typeof(FrontpageButton03));
            meta1 = FrontpageButton03.thirdProperty.GetMetadata(typeof(FrontpageButton03DeriveBB));
            Utilities.Assert(meta == meta1, "");
            meta = FrontpageButton03.thirdProperty.GetMetadata(typeof(FrontpageButton03DeriveCCC));
            Utilities.Assert(meta != meta1, "");

            Utilities.PrintStatus("(D) Register only in immediately derived class and OverrideMetadata in base class ");
            meta = FrontpageButton03DeriveA.FourthProperty.GetMetadata(typeof(FrontpageButton03));
            meta1 = FrontpageButton03DeriveA.FourthProperty.GetMetadata(typeof(FrontpageButton03DeriveA));
            Utilities.Assert(meta == meta1, "");   //This is tricky! The overridden value has precedence over default value. Default value will be useful when ExcelButton03 is used
            meta = FrontpageButton03DeriveA.FourthProperty.GetMetadata(typeof(FrontpageButton03DeriveCCC));
            meta1 = FrontpageButton03DeriveA.FourthProperty.GetMetadata(typeof(ExcelButton03));
            Utilities.Assert(meta != meta1, "");

            Utilities.PrintStatus("(E) Register only in derived class removed by more than 2, Override metadata in base class");
            meta = FrontpageButton03DeriveCCC.FifthProperty.GetMetadata(typeof(FrontpageButton03));
            meta1 = FrontpageButton03DeriveCCC.FifthProperty.GetMetadata(typeof(FrontpageButton03DeriveA));
            Utilities.Assert(meta == meta1, "");
            meta1 = FrontpageButton03DeriveCCC.FifthProperty.GetMetadata(typeof(ExcelButton03));
            Utilities.Assert(meta != meta1, "");

            Utilities.PrintStatus("(F) Register in a class (no need to derive from DependencyObject), OverrideMetadata in another class (that has derived classes) ");
            meta = InfoPathButton03.SixthProperty.GetMetadata(typeof(FrontpageButton03));
            meta1 = InfoPathButton03.SixthProperty.GetMetadata(typeof(FrontpageButton03DeriveA));
            Utilities.Assert(meta == meta1, "");
            meta1 = InfoPathButton03.SixthProperty.GetMetadata(typeof(ExcelButton03));
            Utilities.Assert(meta != meta1, "");

            Utilities.PrintStatus("(G) Register in a class, OverrideMetadata is another class (that has base classes).");
            meta = InfoPathButton03.SeventhProperty.GetMetadata(typeof(FrontpageButton03));
            meta1 = InfoPathButton03.SeventhProperty.GetMetadata(typeof(FrontpageButton03DeriveA));
            Utilities.Assert(meta == meta1, "");
            meta1 = InfoPathButton03.SeventhProperty.GetMetadata(typeof(ExcelButton03));
            Utilities.Assert(meta == meta1, "");
            meta1 = InfoPathButton03.SeventhProperty.GetMetadata(typeof(FrontpageButton03DeriveCCC));
            Utilities.Assert(meta != meta1, "");

            Utilities.PrintStatus("(3) Test case itself define a DependencyProperty with default value specified, then GetMetadata from any of the class used in (2)");
            DependencyPropertyKey TestMePropertyKey = DependencyProperty.RegisterAttachedReadOnly("TestMe", typeof(bool), this.GetType(), new PropertyMetadata(true));
            DependencyProperty TestMeProperty = TestMePropertyKey.DependencyProperty;
            meta = TestMeProperty.GetMetadata(typeof(FrontpageButton03DeriveBB));
            Utilities.Assert((bool)meta.DefaultValue == true, "");
            //To be enabled!
            //Utilities.Assert(meta.ReadOnly == true, "").
        }

        ///<summary>
        ///DependencyProperty.AddOwner. 
        ///</summary>
        public void TestAddOwner()
        {
            Utilities.PrintTitle("Test both DependencyProperty.FromName and AddOwner ");

            Utilities.PrintStatus("(1) AddOwner positive case.");
            FrontpageButton03DeriveCCC.AddOwnerForFifthProperty();

            Utilities.PrintStatus("(2) AddOwner negative test cases");
            Utilities.PrintStatus("(A) when ownerType is null ");
            try
            {
                InfoPathButton03.SeventhProperty.AddOwner(null);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

            Utilities.PrintStatus("(B) When AddOwner is called again.");
            try
            {
                FrontpageButton03DeriveCCC.AddOwnerForFifthProperty();
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }
        #endregion

        #region Originally BvtVisualTree.cs
        ///<summary>
        /// Scenario 1 to 6 of VirtualTree BVT
        ///</summary>
        public void Scenario1To6()
        {
            Utilities.PrintTitle("Scenario 1 to Scenario 6: focused on Inherits in Visual Tree");

            VerifyScenario1To6(null);
        }

        private object VerifyScenario1To6(object arg)
        {
            //For Scenario 1 to 4
            AccessElement access1 = new AccessElement();
            ExcelElement excel1 = new ExcelElement();
            //For Scenario 5
            AccessElement access2 = new AccessElement();
            //For Scenario 6
            ExcelElement excel2 = new ExcelElement();
            AccessElement access3 = new AccessElement();
            AccessElement access4 = new AccessElement();
            AccessElement access5 = new AccessElement();
            ExcelElement excel3 = new ExcelElement();
            AccessElement access6 = new AccessElement();
            AccessElement access7 = new AccessElement();
            ExcelElement excel4 = new ExcelElement();

            //Construct Visual Tree
            Utilities.PrintStatus("Construct Visual Tree");

            //Scenario 1-4
            access1.Children.Add(excel1);
            //Scenario 5
            excel1.Children.Add(access2);
            //Scenario 6
            excel2.Children.Add(access3);
            access3.Children.Add(access4);
            access4.Children.Add(access5);
            access5.Children.Add(excel3);
            excel3.Children.Add(access6);
            access6.Children.Add(access7);
            access7.Children.Add(excel4);

            //Scenario 1
            Utilities.PrintStatus("Scenario 1: Access (defines with Inherits) -> Excel (does not override). [Should Propagate]");
            access1.SetValue(AccessElement.MyLengthProperty, 200);
            Utilities.Assert((int)access1.GetValue(AccessElement.MyLengthProperty) == 200, "");
            Utilities.Assert((int)excel1.GetValue(AccessElement.MyLengthProperty) == 200, "");
            //Scenario 2
            Utilities.PrintStatus("Scenario 2: Access (defines with Inherits) -> Excel (override with no Inherits). [Should Not propagate]");
            access1.SetValue(AccessElement.MyHeightProperty, 200);  //Also used for Scenario 5
            Utilities.Assert((int)access1.GetValue(AccessElement.MyHeightProperty) == 200, "");
            Utilities.Assert((int)excel1.GetValue(AccessElement.MyHeightProperty) == 88, "");
            //Scenario 3
            Utilities.PrintStatus("Scenario 3: Access (Override with Inherits) -> Excel (defines with no inherits). ");
            access1.SetValue(ExcelElement.MyTextProperty, "Access");
            Utilities.Assert((string)access1.GetValue(ExcelElement.MyTextProperty) == "Access", "");
            Utilities.Assert((string)excel1.GetValue(ExcelElement.MyTextProperty) == "Excel", "");
            //Scenario 4
            Utilities.PrintStatus("Scenario 4: Access (does not override) --> Excel(defines with Inherits) [Should propagate]");
            access1.SetValue(ExcelElement.MyAlternativeTextProperty, "AlternativeAccess");
            Utilities.Assert((string)access1.GetValue(ExcelElement.MyAlternativeTextProperty) == "AlternativeAccess", "");
            Utilities.Assert((string)excel1.GetValue(ExcelElement.MyAlternativeTextProperty) == "AlternativeAccess", "");
            //Scenario 5
            Utilities.PrintStatus("Scenario 5: Based on Scenario 2: access -> Excel -> access");
            Utilities.Assert((int)access2.GetValue(AccessElement.MyHeightProperty) == 200, "");
            //Scenario 6
            Utilities.PrintStatus("Now Scenario 6, which has three sub-scenarios");
            Utilities.PrintStatus("AccessElement.MyWidthProperty: Defined with no inherits; ExcelElement override it with Inherits. ");
            Utilities.PrintStatus("Excel(2) -> Access(3) -> Access(4) -> Access(5) -> Excel(3) -> Access(6) ->  Access(7) -> Excel(4)");
            Utilities.PrintStatus("Sub-Scenario (1): Set Value on Excel2");
            excel2.SetValue(AccessElement.MyWidthProperty, 666);
            Utilities.Assert((int)excel2.GetValue(AccessElement.MyWidthProperty) == 666, "");
            Utilities.Assert((int)excel3.GetValue(AccessElement.MyWidthProperty) == 88, "");
            Utilities.Assert((int)excel4.GetValue(AccessElement.MyWidthProperty) == 88, "");
            Utilities.Assert((int)access3.GetValue(AccessElement.MyWidthProperty) == 88, "");
            Utilities.Assert((int)access6.GetValue(AccessElement.MyWidthProperty) == 88, "");
            Utilities.PrintStatus("Sub-Scenario (2): Set Value on Access3");
            access3.SetValue(AccessElement.MyWidthProperty, 777);
            Utilities.Assert((int)excel2.GetValue(AccessElement.MyWidthProperty) == 666, "");
            Utilities.Assert((int)excel3.GetValue(AccessElement.MyWidthProperty) == 88, "");
            Utilities.Assert((int)excel4.GetValue(AccessElement.MyWidthProperty) == 88, "");
            Utilities.Assert((int)access3.GetValue(AccessElement.MyWidthProperty) == 777, "");
            Utilities.Assert((int)access6.GetValue(AccessElement.MyWidthProperty) == 88, "");
            Utilities.PrintStatus("Sub-Scenario (3): Set Value on excel3");
            excel3.SetValue(AccessElement.MyWidthProperty, 888);
            Utilities.Assert((int)excel2.GetValue(AccessElement.MyWidthProperty) == 666, "");
            Utilities.Assert((int)excel3.GetValue(AccessElement.MyWidthProperty) == 888, "");
            Utilities.Assert((int)excel4.GetValue(AccessElement.MyWidthProperty) == 88, "");
            Utilities.Assert((int)access3.GetValue(AccessElement.MyWidthProperty) == 777, "");
            Utilities.Assert((int)access6.GetValue(AccessElement.MyWidthProperty) == 88, "");

            return null;
        }

        /// <summary>
        /// To be completed
        /// </summary>
        private void Scenario7tox()
        {
            Utilities.PrintTitle("Scenario 7 to x: Focus on...");
            VerifyScenario7tox(null);
        }

        private object VerifyScenario7tox(object arg)
        {
            FrontpageElement fp1 = new FrontpageElement();
            InfoPathElement ip1 = new InfoPathElement();
            ip1.Children.Add(fp1);

            //((System.Windows.Media.IVisual)fp1).Children.Add(ip1).

            fp1.SetValue(FrontpageElement.MyPageHeightProperty, 99.999);

            System.Windows.Controls.DockPanel dp1 = new System.Windows.Controls.DockPanel();
            System.Windows.Controls.DockPanel dp2 = new System.Windows.Controls.DockPanel();

            ip1.Children.Add(dp1);
            dp1.Children.Add(dp2);

            dp2.SetValue(System.Windows.Controls.DockPanel.DockProperty, System.Windows.Controls.Dock.Right);
            return null;
        }

        private void VisualTreeInStyle()
        {
            Utilities.PrintTitle("Scenario 7 to x: Focus on...");
            Utilities.PrintStatus("(1)Cover +");
            OneNoteElement.Test1();
            Utilities.PrintStatus("(2)SetValue on Visual tree with invalid data");
            OneNoteElement.Test2();
            Utilities.PrintStatus("(3)SetValue on Visual when it is sealed");
            OneNoteElement.Test3();
            Utilities.PrintStatus("(4)Change style's visual tree after style is sealed");
            OneNoteElement.Test4();
        }

        /// <summary>
        /// All negative test cases
        /// </summary>
        public void NegativeTestCases()
        {
            Utilities.PrintTitle("All Negative Test Cases");
        }
        #endregion

        #region Originally GetSetClear.cs
        ///<summary>
        ///GetValue, SetValue, ClearValue and GetLocalValue on 
        ///DependencyObject directly with no CLR accessors or 
        ///native cache support
        ///</summary>
        public void GetSetClearSimpleDirect()
        {
            Utilities.PrintTitle("GetValue, SetValue, ClearValue and GetLocalValue on DependencyObject directly with no CLR accessors or native cache support");
            VerifyGetSetClearSimpleDirect(null);
        }

        private object VerifyGetSetClearSimpleDirect(object arg)
        {
            TestMercuryPlainSimple mps1 = new TestMercuryPlainSimple();
            TestMercuryPlainSimple mps2 = new TestMercuryPlainSimple();

            Utilities.PrintStatus("(1) ValueType Dependency Property");
            {
                //GetValue --- mps1
                object obj1 = mps1.GetValue(TestMercuryPlainSimple.ValueTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj1.ToString());
                Utilities.Assert((decimal)obj1 == 0m, "GetValue is 0");

                //GetLoclValue --- mps1
                object obj2 = mps1.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj2.ToString());
                Utilities.Assert(obj2 == DependencyProperty.UnsetValue, "GetLoclValue is DependencyProperty.UnsetValue");

                //SetValue --- mps1
                mps1.SetValue(TestMercuryPlainSimple.ValueTypeProperty, 99m);

                //GetValue after SetValue --- mps1
                object obj3 = mps1.GetValue(TestMercuryPlainSimple.ValueTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj3.ToString());
                Utilities.Assert((decimal)obj3 == 99m, "GetValue is 99");

                //GetLoclValue after SetValue --- mps1
                object obj4 = mps1.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj4.ToString());
                Utilities.Assert((Decimal)obj4 == 99m, "GetLoclValue is 99");

                //It should not affect mps2
                //GetValue --- mps2
                object obj5 = mps2.GetValue(TestMercuryPlainSimple.ValueTypeProperty);

                Utilities.PrintStatus("mps2.GetValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj5.ToString());
                Utilities.Assert((decimal)obj5 == 0m, "GetValue is 0");

                //GetLoclValue --- mps2
                object obj6 = mps2.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty);

                Utilities.PrintStatus("mps2.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj6.ToString());
                Utilities.Assert(obj6 == DependencyProperty.UnsetValue, "GetLoclValue is DependencyProperty.UnsetValue");

                //ClearValue --- mps1
                mps1.ClearValue(TestMercuryPlainSimple.ValueTypeProperty);

                //GetValue after ClearValue --- mps1
                object obj7 = mps1.GetValue(TestMercuryPlainSimple.ValueTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj7.ToString());
                Utilities.Assert((decimal)obj7 == 0m, "GetValue is 0");

                //GetLoclValue --- mps1
                object obj8 = mps1.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj8.ToString());
                Utilities.Assert(obj8 == DependencyProperty.UnsetValue, "GetLoclValue is DependencyProperty.UnsetValue");
            }
            Utilities.PrintStatus("(2) ReferenceType Dependency Property");
            {
                //GetValue --- mps1
                object obj1 = mps1.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + (obj1 == null ? "(null)" : obj1.ToString()));
                Utilities.Assert(obj1 == null, "GetValue is null");

                //GetLoclValue --- mps1
                object obj2 = mps1.ReadLocalValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + obj2.ToString());
                Utilities.Assert(obj2 == DependencyProperty.UnsetValue, "GetLoclValue is DependencyProperty.UnsetValue");

                //SetValue --- mps1
                mps1.SetValue(TestMercuryPlainSimple.ReferenceTypeProperty, "WPP");

                //GetValue after SetValue --- mps1
                object obj3 = mps1.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + obj3.ToString());
                Utilities.Assert((string)obj3 == "WPP", "GetValue is WPP");

                //GetLoclValue after SetValue --- mps1
                object obj4 = mps1.ReadLocalValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + obj4.ToString());
                Utilities.Assert((string)obj4 == "WPP", "GetLoclValue is WPP");

                //It should not affect mps2
                //GetValue --- mps2
                object obj5 = mps2.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                Utilities.PrintStatus("mps2.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + (obj5 == null ? "(null)" : obj5.ToString()));
                Utilities.Assert(obj5 == null, "GetValue is null");

                //GetLoclValue --- mps2
                object obj6 = mps2.ReadLocalValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                Utilities.PrintStatus("mps2.ReadLocalValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + obj6.ToString());
                Utilities.Assert(obj6 == DependencyProperty.UnsetValue, "GetLoclValue is DependencyProperty.UnsetValue");

                //ClearValue --- mps1
                mps1.ClearValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                //GetValue after ClearValue --- mps1
                object obj7 = mps1.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + (obj7 == null ? "(null)" : obj5.ToString()));
                Utilities.Assert(obj7 == null, "GetValue is null");

                //GetLoclValue --- mps1
                object obj8 = mps1.ReadLocalValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + obj8.ToString());
                Utilities.Assert(obj8 == DependencyProperty.UnsetValue, "GetLoclValue is DependencyProperty.UnsetValue");
            }
            return null;
        }
        ///<summary>
        ///GetValue, SetValue, ClearValue and GetLocalValue on 
        ///DependencyObject directly with no CLR accessors or 
        ///native cache support. However, defaultValue is specifically 
        ///provided via Metadata
        ///</summary>
        public void GetSetClearSimpleDirectButSetDefaultValueWithMetadata()
        {
            Utilities.PrintTitle("GetValue, SetValue, ClearValue and GetLocalValue on DependencyObject directly with no CLR accessors or native cache support. However, defaultValue is specifically provided via Metadata.");

            VerifyGetSetClearSimpleDirectButSetDefaultValueWithMetadata(null);
        }

        private object VerifyGetSetClearSimpleDirectButSetDefaultValueWithMetadata(object arg)
        {
            TestMercuryPlainSimpleWithDefaultValue mps1 = new TestMercuryPlainSimpleWithDefaultValue();
            TestMercuryPlainSimpleWithDefaultValue mps2 = new TestMercuryPlainSimpleWithDefaultValue();
            Utilities.PrintStatus("(1) ValueType Dependency Property");
            {
                //GetValue --- mps1
                object obj1 = mps1.GetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj1.ToString());
                Utilities.Assert((decimal)obj1 == 9m, "GetValue is 9");

                //GetLoclValue --- mps1
                object obj2 = mps1.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj2.ToString());
                Utilities.Assert(obj2 == DependencyProperty.UnsetValue, "GetLoclValue is DependencyProperty.UnsetValue");

                //SetValue --- mps1
                mps1.SetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty,
                    99m);

                //GetValue after SetValue --- mps1
                object obj3 = mps1.GetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj3.ToString());
                Utilities.Assert((decimal)obj3 == 99m, "GetValue is 99");

                //GetLoclValue after SetValue --- mps1
                object obj4 = mps1.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty) is " + obj4.ToString());
                Utilities.Assert((Decimal)obj4 == 99m, "GetLoclValue is 99");

                //It should not affect mps2
                //GetValue --- mps2
                object obj5 = mps2.GetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                Utilities.PrintStatus("mps2.GetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty) is " + obj5.ToString());
                Utilities.Assert((decimal)obj5 == 9m, "GetValue is 9");

                //GetLoclValue --- mps2
                object obj6 = mps2.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                Utilities.PrintStatus("mps2.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty) is " + obj6.ToString());
                Utilities.Assert(obj6 == DependencyProperty.UnsetValue, "GetLoclValue is DependencyProperty.UnsetValue");

                //ClearValue --- mps1
                mps1.ClearValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                //GetValue after ClearValue --- mps1
                object obj7 = mps1.GetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty) is " + obj7.ToString());
                Utilities.Assert((decimal)obj7 == 9m, "GetValue is 9");

                //GetLoclValue --- mps1
                object obj8 = mps1.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty) is " + obj8.ToString());
                Utilities.Assert(obj8 == DependencyProperty.UnsetValue, "GetLoclValue is DependencyProperty.UnsetValue");
            }
            Utilities.PrintStatus("(2) ReferenceType Dependency Property");
            {
                //GetValue --- mps1
                object obj1 = mps1.GetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty) is " + (obj1 == null ? "(null)" : obj1.ToString()));
                Utilities.Assert((string)obj1 == "9m", "GetValue is 9m");

                //GetLoclValue --- mps1
                object obj2 = mps1.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty) is " + obj2.ToString());
                Utilities.Assert(obj2 == DependencyProperty.UnsetValue, "GetLoclValue is DependencyProperty.UnsetValue");

                //SetValue --- mps1
                mps1.SetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty, "WPP");

                //GetValue after SetValue --- mps1
                object obj3 = mps1.GetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + obj3.ToString());
                Utilities.Assert((string)obj3 == "WPP", "GetValue is WPP");

                //GetLoclValue after SetValue --- mps1
                object obj4 = mps1.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty) is " + obj4.ToString());
                Utilities.Assert((string)obj4 == "WPP", "GetLoclValue is WPP");

                //It should not affect mps2
                //GetValue --- mps2
                object obj5 = mps2.GetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                Utilities.PrintStatus("mps2.GetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty) is " + (obj5 == null ? "(null)" : obj5.ToString()));
                Utilities.Assert((string)obj5 == "9m", "GetValue is 9m");

                //GetLoclValue --- mps2
                object obj6 = mps2.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                Utilities.PrintStatus("mps2.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty) is " + obj6.ToString());
                Utilities.Assert(obj6 == DependencyProperty.UnsetValue, "GetLoclValue is DependencyProperty.UnsetValue");

                //ClearValue --- mps1
                mps1.ClearValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                //GetValue after ClearValue --- mps1
                object obj7 = mps1.GetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty) is " + (obj7 == null ? "(null)" : obj5.ToString()));
                Utilities.Assert((string)obj7 == "9m",   //Actually [obj == "9m"] also works. But with compiler warning;
                  "GetValue is 9m");

                //GetLoclValue --- mps1
                object obj8 = mps1.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty) is " + obj8.ToString());
                Utilities.Assert(obj8 == DependencyProperty.UnsetValue, "GetLoclValue is DependencyProperty.UnsetValue");
            }
            return null;
        }
        #endregion

        #region OverrideMetadata
        ///<summary>
        ///(101) Test DependencyProperty.OverrideMetadata 
        ///functionality: Derived class override default metadata
        ///of base class
        ///</summary>
        public void DeriveOverride()
        {
            Utilities.PrintTitle("Derived class override default metadata of base class");

            object obj1, obj2;

            ButtonTypeABase btn1 = new ButtonTypeABase();
            ButtonTypeADerive1 rptBtn1 = new ButtonTypeADerive1();

            //GetValue --- btn1
            Utilities.PrintStatus("(1) Print base class (Button) Property");
            obj1 = btn1.GetValue(ButtonTypeABase.PressedProperty);
            Utilities.PrintStatus(obj1.ToString());

            //GetValue --- rptBtn1
            Utilities.PrintStatus("(2) Print derived class (RepeatButton) Property");
            obj2 = rptBtn1.GetValue(ButtonTypeABase.PressedProperty);
            Utilities.PrintStatus(obj2.ToString());

            ButtonTypeABase btnABase = new ButtonTypeABase();
            ButtonTypeADerive1 btnADerive1 = new ButtonTypeADerive1();
            ButtonTypeADerive2 btnADerive2 = new ButtonTypeADerive2();
            ButtonTypeZBase btnZBase = new ButtonTypeZBase();
            ButtonTypeZDerive1 btnZDerive1 = new ButtonTypeZDerive1();
            ButtonTypeZDerive2 btnZDerive2 = new ButtonTypeZDerive2();
            ButtonTypeZDerive3 btnZDerive3 = new ButtonTypeZDerive3();

            Utilities.PrintStatus("Z3Tested Property has ownerType of btnZDerive3");
            Utilities.PrintStatus("btnZDerive3.Z3Tested is " + btnZDerive3.GetValue(ButtonTypeZDerive3.Z3TestedProperty));
            Utilities.PrintStatus("btnZDerive2.Z3Tested is " + btnZDerive2.GetValue(ButtonTypeZDerive3.Z3TestedProperty));
            Utilities.PrintStatus("btnZDerive1.Z3Tested is " + btnZDerive1.GetValue(ButtonTypeZDerive3.Z3TestedProperty));
            Utilities.PrintStatus("btnZBase.Z3Tested is " + btnZBase.GetValue(ButtonTypeZDerive3.Z3TestedProperty));
            Utilities.PrintStatus("btnADerive2.Z3Tested is " + btnADerive2.GetValue(ButtonTypeZDerive3.Z3TestedProperty));
            Utilities.PrintStatus("btnADerive1.Z3Tested is " + btnADerive1.GetValue(ButtonTypeZDerive3.Z3TestedProperty));
            Utilities.PrintStatus("btnABase.Z3Tested is " + btnABase.GetValue(ButtonTypeZDerive3.Z3TestedProperty));

            Utilities.PrintStatus("ButtonTypeADerive2.ValidOverrideZ3TestedProperty");
            ButtonTypeADerive2.InvalidOverrideZ3TestedProperty();
            Utilities.PrintStatus("btnZDerive3.Z3Tested is " + btnZDerive3.GetValue(ButtonTypeZDerive3.Z3TestedProperty));
            Utilities.PrintStatus("btnZDerive2.Z3Tested is " + btnZDerive2.GetValue(ButtonTypeZDerive3.Z3TestedProperty));
            Utilities.PrintStatus("btnZDerive1.Z3Tested is " + btnZDerive1.GetValue(ButtonTypeZDerive3.Z3TestedProperty));
            Utilities.PrintStatus("btnZBase.Z3Tested is " + btnZBase.GetValue(ButtonTypeZDerive3.Z3TestedProperty));
            Utilities.PrintStatus("btnADerive2.Z3Tested is " + btnADerive2.GetValue(ButtonTypeZDerive3.Z3TestedProperty));
            Utilities.PrintStatus("btnADerive1.Z3Tested is " + btnADerive1.GetValue(ButtonTypeZDerive3.Z3TestedProperty));
            Utilities.PrintStatus("btnABase.Z3Tested is " + btnABase.GetValue(ButtonTypeZDerive3.Z3TestedProperty));
        }
        #endregion
     }

    #region Originally Used By Test FrameworkElementFactory
    /******************************************************************************
    * CLASS:          AccessCanvas01
    ******************************************************************************/
    /// <summary>
    /// Used in AccessButton01
    /// </summary>
    public class AccessCanvas01 : System.Windows.DependencyObject
    {
        /// <summary>
        /// Dependency Property for Left position. A related one is Top
        /// </summary>
        public static readonly DependencyProperty LeftProperty = DependencyProperty.RegisterAttached
        ("Left", typeof(int), typeof(AccessCanvas01),
            new FrameworkPropertyMetadata(33, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Dependency Property for top position. A related one is Left
        /// </summary>
        public static readonly DependencyProperty TopProperty = DependencyProperty.RegisterAttached
        ("Top", typeof(int), typeof(AccessCanvas01),
            new FrameworkPropertyMetadata(44, FrameworkPropertyMetadataOptions.AffectsArrange));

    }

    /******************************************************************************
    * CLASS:          AccessButton01
    ******************************************************************************/
    /// <summary>
    /// Access Button
    /// </summary>
    public class AccessButton01 : System.Windows.Controls.Button
    {
        /// <summary>
        /// Background property
        /// </summary>
        public static readonly DependencyProperty MyBackgroundProperty = DependencyProperty.RegisterAttached(
            "MyBackground", typeof(string), typeof(AccessButton01), new FrameworkPropertyMetadata("Red", FrameworkPropertyMetadataOptions.AffectsRender));
    }

    /******************************************************************************
    * CLASS:          ExcelCanvas01
    ******************************************************************************/
    /// <summary>
    /// Used in ExcelButton01
    /// </summary>
    public class ExcelCanvas01 : FrameworkElement
    {
        /// <summary>
        /// Dependency Property for Left position. A related one is Top
        /// </summary>
        public static readonly DependencyProperty LeftProperty = DependencyProperty.RegisterAttached(
            "Left", typeof(int), typeof(ExcelCanvas01), new FrameworkPropertyMetadata(33, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Dependency Property for top position. A related one is Left
        /// </summary>
        public static readonly DependencyProperty TopProperty = DependencyProperty.RegisterAttached(
            "Top", typeof(int), typeof(ExcelCanvas01), new FrameworkPropertyMetadata(44, FrameworkPropertyMetadataOptions.AffectsArrange));
    }

    /******************************************************************************
    * CLASS:          ExcelButton01
    ******************************************************************************/
    /// <summary>
    /// ExcelButton01 is derived from Button
    /// </summary>
    public class ExcelButton01 : System.Windows.Controls.Button
    {
        /// <summary>
        /// Background property
        /// </summary>
        public static readonly DependencyProperty MyBackgroundProperty = DependencyProperty.RegisterAttached(
            "MyBackground", typeof(string), typeof(ExcelButton01), new FrameworkPropertyMetadata("Red", FrameworkPropertyMetadataOptions.AffectsRender));
    }
    #endregion

    #region Used by SealedPropertySetter, DpRegister
    /******************************************************************************
    * CLASS:          AccessButton03
    ******************************************************************************/
    /// <summary>
    /// Used by SealedPropertySetter
    /// </summary>
    public class AccessButton03 : DependencyObject
    {
        /// <summary>
        ///  Used by SealedPropertySetter
        /// </summary>
        public static PropertyMetadata AccessMeta = new PropertyMetadata();
        /// <summary>
        /// SealedPropertySetter
        /// </summary>
        public static readonly DependencyProperty SealedByProperty = DependencyProperty.RegisterAttached(
        "SealedBy",
            typeof(string),
            typeof(AccessButton03),
            AccessMeta);
    }

    /******************************************************************************
    * CLASS:          ExcelButton03
    ******************************************************************************/
    /// <summary>
    /// Used by DpRegister (DependencyProtocol.Register), both positive and negative cases
    /// </summary>
    public class ExcelButton03 : DependencyObject
    {
        /// <summary>
        /// Register without providing defaultMetadata
        /// </summary>
        public static readonly DependencyProperty OneAProperty = DependencyProperty.RegisterAttached("1A",
            typeof(string),
            typeof(ExcelButton03));

        /// <summary>
        /// Register and provide defaultMetadata
        /// </summary>
        public static readonly DependencyProperty OneBProperty = DependencyProperty.RegisterAttached("1B",
            typeof(string),
            typeof(ExcelButton03),
            new PropertyMetadata("One Little Two Little"));

        /// <summary>
        /// when name is null 
        /// </summary>
        public static void ScenarioA()
        {
            Utilities.PrintStatus("[Scenario A] when name is null");
            try
            {
                DependencyProperty NegativeProperty = DependencyProperty.RegisterAttached(
                    null, typeof(string), typeof(ExcelButton03));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /// <summary>
        /// when propertyType is null 
        /// </summary>
        public static void ScenarioB()
        {
            Utilities.PrintStatus("[Scenario B] when propertyType is null ");
            try
            {
                DependencyProperty NegativeProperty = DependencyProperty.RegisterAttached(
                    "Negative", null, typeof(ExcelButton03));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /// <summary>
        /// when ownerType is null 
        /// </summary>
        public static void ScenarioC()
        {
            Utilities.PrintStatus("[Scenario C] when ownerType is null ");
            try
            {
                DependencyProperty NegativeProperty = DependencyProperty.RegisterAttached(
                    "Negative", typeof(string), null);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /// <summary>
        /// when same property name is already registered for same owner type
        /// </summary>
        public static void ScenarioD()
        {
            Utilities.PrintStatus("[Scenario D] when same property name is already registered for same owner type");

            try
            {
                DependencyProperty DuplicateOfOneAProperty = DependencyProperty.RegisterAttached(
                    "1A", typeof(int), typeof(ExcelButton03));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /// <summary>
        /// when defaultMetadata.DefaultValue type does not match DepedenyProperty.PropertyType 
        /// </summary>
        public static void ScenarioE()
        {
            Utilities.PrintStatus("[Scenario E] when defaultMetadata.DefaultValue type does not match DepedenyProperty.PropertyType");
            try
            {
                DependencyProperty NegativeProperty = DependencyProperty.RegisterAttached(
                    "Negative", typeof(string), typeof(ExcelButton03), new PropertyMetadata(12345));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }
    }

    /******************************************************************************
    * CLASS:          FrontpageButton03
    ******************************************************************************/
    /// <summary>
    /// Used for OverrideMetadataPositive
    /// </summary>
    public class FrontpageButton03 : DependencyObject
    {
        /// <summary>
        /// (1) Register and OverrideMetadata in the same class 
        /// </summary>
        public static readonly DependencyProperty FirstProperty = DependencyProperty.RegisterAttached("First",
            typeof(string),
            typeof(FrontpageButton03),
            new PropertyMetadata("Default First Property", new PropertyChangedCallback(HandlerPropertyChanged1), null));

        /// <summary>
        /// (2) Register only in base class, OverrideMetadata in immediately derived class 
        /// </summary>
        public static readonly DependencyProperty SecondProperty = DependencyProperty.RegisterAttached("Second",
            typeof(string),
            typeof(FrontpageButton03),
            new PropertyMetadata("Default Second Property"));

        /// <summary>
        /// (3) Register only in base class, OverrideMetadata in derived class removed by more than 2
        /// </summary>
        public static readonly DependencyProperty thirdProperty = DependencyProperty.RegisterAttached("Third",
            typeof(string),
            typeof(FrontpageButton03),
            new PropertyMetadata("Default Third Proeprty"));


        static FrontpageButton03()
        {
            FirstProperty.OverrideMetadata(
                typeof(FrontpageButton03), new PropertyMetadata("Override First Property", new PropertyChangedCallback(HandlerPropertyChanged2), null)
            );

            FrontpageButton03DeriveA.FourthProperty.OverrideMetadata(typeof(FrontpageButton03), new PropertyMetadata("Override Fourth Property", new PropertyChangedCallback(HandlerPropertyChanged2), null));
            FrontpageButton03DeriveCCC.FifthProperty.OverrideMetadata(typeof(FrontpageButton03), new PropertyMetadata("Override Fifth Property", new PropertyChangedCallback(HandlerPropertyChanged3), null));
            InfoPathButton03.SixthProperty.OverrideMetadata(typeof(FrontpageButton03), new PropertyMetadata("Override Sixth Property", new PropertyChangedCallback(HandlerPropertyChanged1), null));
        }

        private static void HandlerPropertyChanged1(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
        private static void HandlerPropertyChanged2(DependencyObject d, DependencyPropertyChangedEventArgs e){}
        private static void HandlerPropertyChanged3(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
    }

    /******************************************************************************
    * CLASS:          FrontpageButton03DeriveA
    ******************************************************************************/
    /// <summary>
    /// Used for OverrideMetadataPositive
    /// </summary>
    public class FrontpageButton03DeriveA : FrontpageButton03
    {
        /// <summary>
        /// (4) Register only in immediately derived class and OverrideMetadata in base class 
        /// </summary>
        public static readonly DependencyProperty FourthProperty = DependencyProperty.RegisterAttached("Fourth",
            typeof(string),
            typeof(FrontpageButton03DeriveA), new PropertyMetadata("Default Fourth Property"));

        static FrontpageButton03DeriveA()
        {
            FrontpageButton03.SecondProperty.OverrideMetadata(typeof(FrontpageButton03DeriveA),
                new PropertyMetadata("Override Second Property"));
        }
    }

    /******************************************************************************
    * CLASS:          FrontpageButton03DeriveBB
    ******************************************************************************/
    /// <summary>
    /// Used for OverrideMetadataPositive
    /// </summary>
    public class FrontpageButton03DeriveBB : FrontpageButton03DeriveA
    {
    }

    /******************************************************************************
    * CLASS:          FrontpageButton03DeriveCCC
    ******************************************************************************/
    /// <summary>
    /// Used for OverrideMetadataPositive
    /// </summary>
    public class FrontpageButton03DeriveCCC : FrontpageButton03DeriveBB
    {
        /// <summary>
        /// 5) Register only in derived class removed by more than 2, Override metadata in base class (
        /// </summary>
        public static readonly DependencyProperty FifthProperty = DependencyProperty.RegisterAttached("Fifth",
            typeof(string),
            typeof(FrontpageButton03DeriveCCC),
            new PropertyMetadata("Default Fifth Property"));

        static FrontpageButton03DeriveCCC()
        {
            FrontpageButton03.thirdProperty.OverrideMetadata(typeof(FrontpageButton03DeriveCCC), new PropertyMetadata("Override Third Property"));
            InfoPathButton03.SeventhProperty.OverrideMetadata(typeof(FrontpageButton03DeriveCCC), new PropertyMetadata("Override Seventh Property"));
        }

        /// <summary>
        /// First called by FromNameAddOwner for AddOwner positive case
        /// Second call should throw exception but we have not (

        public static void AddOwnerForFifthProperty()
        {
            InfoPathButton03.SeventhProperty.AddOwner(typeof(FrontpageButton03DeriveCCC));
        }
    }

    /******************************************************************************
    * CLASS:          InfoPathButton03
    ******************************************************************************/
    /// <summary>
    /// Used for OverrideMetadataPositive.
    /// This class does not derive from DependencyObject. It is
    /// used to define a DependencyPropery SixthProperty
    /// </summary>
    public class InfoPathButton03
    {
        /// <summary>
        /// (6) Register in a class (no need to derive from DependencyObject), OverrideMetadata in another class (that has derived classes) 
        /// </summary>
        public static readonly DependencyProperty SixthProperty = DependencyProperty.RegisterAttached("Sixth",
            typeof(string),
            typeof(InfoPathButton03),
            new PropertyMetadata("Default Sixth Property"));

        /// <summary>
        /// Register in a class, OverrideMetadata is another class (that has base classes). 
        /// </summary>
        public static readonly DependencyProperty SeventhProperty = DependencyProperty.RegisterAttached("Seventh",
            typeof(string),
            typeof(InfoPathButton03),
            new PropertyMetadata("Default seventh property"));
    }

    /******************************************************************************
    * CLASS:          VisioButton03
    ******************************************************************************/
    /// <summary>
    /// This class does not derive from DependencyObject, so it cannot 
    /// register metadata or override metadata
    /// </summary>
    public class VisioButton03
    {
        /// <summary>
        /// ClickModeProperty is used far more frequently than NameProperty
        /// and WidthProperty
        /// </summary>
        public static readonly DependencyProperty ClickModeProperty = DependencyProperty.RegisterAttached("ClickMode",
            typeof(string),
            typeof(VisioButton03),
            new PropertyMetadata("Default"));

        /// <summary>
        /// Note NameProperty has the same "string" type as ClickModeProperty,
        /// a PropertyMetadata may be used for either of them. But not both 
        /// of them because once it is used, it will be sealed. (Scenario C).
        /// It is also used in scenario F (After scenario C, it is already registered)
        /// </summary>
        public static readonly DependencyProperty NameProperty = DependencyProperty.RegisterAttached("Name",
            typeof(string),
            typeof(VisioButton03),
            new PropertyMetadata("Access"));

        /// <summary>
        /// WidthProperty does not have the same type as C----ModeProperty
        /// So PropertyMetadata for WidthProperty is not suitable for ClickModeProperty
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.RegisterAttached("Width",
            typeof(int),
            typeof(VisioButton03),
            new PropertyMetadata(99));

        /// <summary>
        /// AddressProperty is used to test one invalid scenario for
        /// DependencyProperty.OverrideMetadata. To be specific, it is for 
        /// scenario d:forType is not derived from DependencyObject class
        /// static method TryScenarioD is called for this purpose only
        /// </summary>
        public static readonly DependencyProperty AddressProperty = DependencyProperty.RegisterAttached("Address",
            typeof(string),
            typeof(VisioButton03),
            new PropertyMetadata("1 Microsoft Way"));

        /// <summary>
        /// DotnetFrameworkVersionProperty is used to test one invalid scenario for
        /// DependencyProperty.OverrideMetadata. To be specific, it is for scenario H:
        /// Incorrect metadata type (overriding metadata is the same type or 
        /// derived type from the base matadata). 
        /// </summary>
        public static readonly DependencyProperty DotnetFrameworkVersionProperty = DependencyProperty.RegisterAttached("DotnetFrameworkVersion",
            typeof(float),
            typeof(VisioButton03),
            new FrameworkPropertyMetadata(1.1f));

        /// <summary>
        /// Read the comment for AddressProperty for more detail
        /// </summary>
        public static void TryScenarioD()
        {
            AddressProperty.OverrideMetadata(typeof(VisioButton03), new PropertyMetadata("Building 10"));
        }
    }

    /******************************************************************************
    * CLASS:          WordButton03
    ******************************************************************************/
    /// <summary>
    /// Used for OverrideMetadata negative tests
    /// </summary>
    public class WordButton03 : DependencyObject
    {
        /// <summary>
        /// Used for scenario G
        /// </summary>
        public static readonly DependencyProperty CopyrightProperty = DependencyProperty.RegisterAttached(
        "Copyright",
            typeof(string),
            typeof(WordButton03),
            new PropertyMetadata("All rights reserved"));

        static WordButton03()
        {
            CopyrightProperty.OverrideMetadata(typeof(WordButton03), new PropertyMetadata("Copyright 2003"));

            VisioButton03.ClickModeProperty.AddOwner(typeof(WordButton03));
            VisioButton03.ClickModeProperty.OverrideMetadata(typeof(WordButton03),
                new PropertyMetadata("Excel"));
        }
    }

    /******************************************************************************
    * CLASS:          WordButton03DeriveA
    ******************************************************************************/
    /// <summary>
    /// Used for OverrideMetadata negative tests
    /// </summary>
    public class WordButton03DeriveA : WordButton03
    {
        //DependencyProperty.OverrideMetadata throws exception if (a) forType is null (b) metadata is null (c)metadata is sealed (d) forType is not derived from DependencyObject class (e)Type Metadata already registere( f) <



        public void ScenarioA()
        {
            Utilities.PrintStatus("[Scenario A]: forType is null");
            try
            {
                VisioButton03.ClickModeProperty.OverrideMetadata(null, new PropertyMetadata("WordButton03DeriveA"));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /// <summary>
        /// [Scenario B]: metadata is null
        /// </summary>
        public void ScenarioB()
        {
            Utilities.PrintStatus("[Scenario B]: metadata is null");
            try
            {
                VisioButton03.ClickModeProperty.OverrideMetadata(typeof(WordButton03DeriveA), null);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /// <summary>
        /// [Scenario C]: metadata is sealed
        /// </summary>
        public void ScenarioC()
        {
            Utilities.PrintStatus("[Scenario C]: metadata is sealed");
            //Used by NameProperty first (sealed) and then try with ClickModeProperty
            PropertyMetadata metadata = new PropertyMetadata("Excel");
            VisioButton03.NameProperty.OverrideMetadata(typeof(WordButton03DeriveA), metadata);
            try
            {
                VisioButton03.ClickModeProperty.OverrideMetadata(typeof(WordButton03DeriveA), metadata);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /// <summary>
        /// Scenario D: forType is not derived from DependencyObject class
        /// </summary>
        public void ScenarioD()
        {
            Utilities.PrintStatus("[Scenario D]: forType is not derived from DependencyObject class");
            try
            {
                VisioButton03.TryScenarioD();
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /// <summary>
        /// This is test against 

        public void ScenarioE()
        {
            Utilities.PrintStatus("[Scenario E]: provided metadata should be of type of the property");
            PropertyMetadata meta = new PropertyMetadata(1973);
            try
            {
                VisioButton03.AddressProperty.OverrideMetadata(typeof(WordButton03DeriveA),
                    meta);
                object obj = GetValue(VisioButton03.AddressProperty);
                Utilities.PrintStatus(obj.ToString());
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            //Prove that metadata is well formed.
            //Ironically if the previous override works, this 
            //OverrideMetadata will throw (due to sealed metadata)
            VisioButton03.WidthProperty.OverrideMetadata(typeof(WordButton03DeriveA), meta);
        }

        /// <summary>
        /// Scenario F: Type Metadata already registered. Scequence is important
        /// that ScenarioC is run first before F
        /// </summary>
        public void ScenarioF()
        {
            Utilities.PrintStatus("[Scenario F]: Type Metadata already registered");
            try
            {
                VisioButton03.NameProperty.OverrideMetadata(typeof(WordButton03DeriveA),
                    new PropertyMetadata("New Name"));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /// <summary>
        /// Scenario G: metadata.Readonly is false but its base already set it to be true
        /// Cannot run due to 


        public void ScenarioG()
        {
            Utilities.PrintStatus("[Scenario G]: metadata.Readonly is false but its base already set it to be true");
            try
            {
                WordButton03.CopyrightProperty.OverrideMetadata(typeof(WordButton03DeriveA), new PropertyMetadata("Copyright 2004"));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /// <summary>
        /// Scenario H: Incorrect metadata type (overriding metadata is the same type or derived type from the base matadata). 
        /// </summary>
        public void ScenarioH()
        {
            Utilities.PrintStatus("[Scenario H]: Incorrect metadata type (overriding metadata is the same type or derived type from the base matadata). ");
            try
            {
                VisioButton03.DotnetFrameworkVersionProperty.OverrideMetadata(typeof(WordButton03DeriveA),
                    new PropertyMetadata("I am base class of FrameworkPropertyMetadata"));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }
    }
    #endregion


    #region Used for VisualTree tests
    /*------------------------------------------------------------------------\
    |Scenario 1 through (6) are focused on Inherits in Visual Tree
    | 1 - 4: Access -> Excel
    |Scenario 1:
    |  AccessElement.MyLengthProperty: Defined with Inherits. ExcelElement does not override it;
    |Scenario 2:
    |  AccessElement.MyHeightProperty: Defined with Inherits. ExcelElement Override it with No inherits
    |Scenario 3:
    |  ExcelElement.MyTextProperty: defined without Inherits. AccessElement override it with Inherits;
    |Scenario 4:
    |  ExcelElement.MyAlternativeTextProperty: defined with Inherits. AccessElement does not override it at all;
    |Scenario 5:
    |  Based on Scenario 2. ExcelElement contains a child of AccessElement (AccessElement.MyHeightProperty defines Inherits
    |Scenario 6:
    |  (AccessElement.MyWidthProperty: Defined with no inherits; ExcelElement override it with Inherits
    |   Test with a long tree: Excel -> Access(3) -> Access(4) -> Access(5) -> Excel -> Access(6) ->  Access(7) -> Excel (Count: 5 Access, 3 Excel)
    |
    \-------------------------------------------------------------------------*/
    /******************************************************************************
    * CLASS:          AccessElement
    ******************************************************************************/
    /// <summary>
    /// AccessElement
    /// </summary>
    public class AccessElement : FrameworkElement
    {
        /// <summary>
        /// Ctor
        /// </summary>    
        public AccessElement()
        {
            _children = new VisualCollection(this);        
        }

        /// <summary>
        /// Get its Children
        /// </summary>    
        public VisualCollection Children
        {
            get
            {
                return _children;
            }
        }

        /// <summary>
        /// Returns the child at the specified index.
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            // if you have a template
            if(base.VisualChildrenCount != 0 && index == 0)
            {
                return base.GetVisualChild(0);
            }            
            // otherwise you can have your own children
            if(_children == null)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }
            if(index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }

            return _children[index];
        }


        /// <summary>
        /// Returns the Visual children count.
        /// </summary>        
        protected override int VisualChildrenCount
        {           
            get 
            {
                //you can either have a Template or your own children
                if(base.VisualChildrenCount > 0) return 1;
                else return  _children.Count; 
            }            
        }        

        private VisualCollection _children;
    
        /// <summary>
        /// //scenario 1
        /// </summary>
        public static readonly System.Windows.DependencyProperty MyLengthProperty = System.Windows.DependencyProperty.RegisterAttached("Length", typeof(int), typeof(AccessElement), new FrameworkPropertyMetadata(88, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// scenario 2
        /// </summary>
        public static readonly System.Windows.DependencyProperty MyHeightProperty = System.Windows.DependencyProperty.RegisterAttached("Height", typeof(int), typeof(AccessElement), new FrameworkPropertyMetadata(88, FrameworkPropertyMetadataOptions.Inherits));
        /// <summary>
        /// //Scenario 6
        /// </summary>
        public static readonly System.Windows.DependencyProperty MyWidthProperty = System.Windows.DependencyProperty.RegisterAttached("Width", typeof(int), typeof(AccessElement), new FrameworkPropertyMetadata(88));

        static AccessElement()
        {
            //Scenario 3
            FrameworkPropertyMetadata fmeta = new FrameworkPropertyMetadata();
            fmeta.Inherits = true;
            ExcelElement.MyTextProperty.OverrideMetadata(typeof(AccessElement), fmeta);
        }
    }

    /******************************************************************************
    * CLASS:          ExcelElement
    ******************************************************************************/
    /// <summary>
    /// ExcelElement
    /// </summary>
    public class ExcelElement : FrameworkElement
    {
        static ExcelElement()
        {
            //Scenario 2
            FrameworkPropertyMetadata fmeta = new FrameworkPropertyMetadata();
            fmeta.Inherits = false;
            AccessElement.MyHeightProperty.OverrideMetadata(typeof(ExcelElement), fmeta);

            //Scenario 6
            fmeta = new FrameworkPropertyMetadata();
            fmeta.Inherits = true;
            AccessElement.MyWidthProperty.OverrideMetadata(typeof(ExcelElement), fmeta);
        }

        /// <summary>
        /// Ctor
        /// </summary>    
        public ExcelElement()
        {
            _children = new VisualCollection(this);        
        }

        /// <summary>
        /// Get Its children
        /// </summary>    
        public VisualCollection Children
        {
            get
            {
                return _children;
            }
        }

        /// <summary>
        /// Returns the child at the specified index.
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            // if you have a template
            if(base.VisualChildrenCount != 0 && index == 0)
            {
                return base.GetVisualChild(0);
            }            
            // otherwise you can have your own children
            if(_children == null)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }
            if(index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }

            return _children[index];
        }


        /// <summary>
        /// Returns the Visual children count.
        /// </summary>        
        protected override int VisualChildrenCount
        {           
            get 
            {
                //you can either have a Template or your own children
                if(base.VisualChildrenCount > 0) return 1;
                else return  _children.Count; 
            }            
        }       

        private VisualCollection _children;
        
        /// <summary>
        /// //Scenario 3
        /// </summary>
        public static readonly System.Windows.DependencyProperty MyTextProperty = System.Windows.DependencyProperty.RegisterAttached("MyText", typeof(string), typeof(ExcelElement), new FrameworkPropertyMetadata("Excel"));
        /// <summary>
        /// //Scenario 4
        /// </summary>
        public static readonly System.Windows.DependencyProperty MyAlternativeTextProperty = System.Windows.DependencyProperty.RegisterAttached("MyAlternativeText", typeof(string), typeof(ExcelElement), new FrameworkPropertyMetadata("AlternativeExcel", FrameworkPropertyMetadataOptions.Inherits));
    }

    /******************************************************************************
    * CLASS:          FrontpageElement
    ******************************************************************************/
    /*------------------------------------------------------------------------\
    |Scenario (7) and (8) on testing AffectsParentMeasure and AffectsParentArrange
    /in visual tree
    |
     \--------------------------------------------------------------------------*/
    /// <summary>
    /// Frontpage Element
    /// </summary>
    public class FrontpageElement : FrameworkElement
    {
        /// <summary>
        /// MyPageHeight property is of type double with default value: 11.0 * 96.0d
        /// </summary>
        public static readonly System.Windows.DependencyProperty MyPageHeightProperty = System.Windows.DependencyProperty.RegisterAttached("MyPageHeight", typeof(double), typeof(FrontpageElement), new FrameworkPropertyMetadata(11.0 * 96.0d, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure));
    }

    /******************************************************************************
    * CLASS:          InfoPathElement
    ******************************************************************************/
    /// <summary>
    /// InfoPath Element
    /// </summary>
    public class InfoPathElement : FrameworkElement
    {
        /// <summary>
        /// Ctor
        /// </summary>        
        public InfoPathElement()
        {
            _children = new VisualCollection(this);
        }

        /// <summary>
        /// Get Its children
        /// </summary>            
        public VisualCollection Children
        {
            get
            {
                return _children;
            }
        }

        /// <summary>
        /// Returns the child at the specified index.
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            // if you have a template
            if(base.VisualChildrenCount != 0 && index == 0)
            {
                return base.GetVisualChild(0);
            }            
            // otherwise you can have your own children
            if(_children == null)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }
            if(index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }

            return _children[index];
        }


        /// <summary>
        /// Returns the Visual children count.
        /// </summary>        
        protected override int VisualChildrenCount
        {           
            get 
            {
                //you can either have a Template or your own children
                if(base.VisualChildrenCount > 0) return 1;
                else return  _children.Count; 
            }            
        }    
        
        private VisualCollection _children;
    
    }

    /******************************************************************************
    * CLASS:          OneNoteElement
    ******************************************************************************/
    /*
        Visual Tree In Style
    */
    /// <summary>
    /// One Note Element
    /// </summary>
    public class OneNoteElement : FrameworkElement
    {
        /// <summary>
        /// Cover 

        public static void Test1()
        {
            VerifyTest1(null);
        }

        private static object VerifyTest1(object arg)
        {
            System.Windows.Style s = new System.Windows.Style(typeof(OneNoteElement));
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(System.Windows.Controls.Button));
            fef.SetValue(System.Windows.Controls.Button.OpacityProperty, 88.888);
            ControlTemplate template = new ControlTemplate(typeof(System.Windows.Controls.Control));
            template.VisualTree = fef;
            s.Setters.Add(new Setter(System.Windows.Controls.Control.TemplateProperty, template));

            System.Windows.Controls.Control c = new System.Windows.Controls.Control();
            c.Style = s;
            s = c.Style;

            return null;
        }

        /// <summary>
        /// SetValue on Visual tree with invalid data
        /// </summary>
        public static void Test2()
        {
            System.Windows.Style s = new System.Windows.Style(typeof(OneNoteElement));
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(System.Windows.Controls.Button));

            fef.SetValue(System.Windows.Controls.Button.OpacityProperty, 88.888);
            ControlTemplate template = new ControlTemplate(typeof(System.Windows.Controls.Control));
            template.VisualTree = fef;
            s.Setters.Add(new Setter(System.Windows.Controls.Control.TemplateProperty, template));

            try
            {
                fef.SetValue(System.Windows.Controls.Button.HeightProperty, 99.9999);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /// <summary>
        /// SetValue on Visual when it is sealed
        /// </summary>
        public static void Test3()
        {
            System.Windows.Style s = new System.Windows.Style(typeof(OneNoteElement));
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(System.Windows.Controls.Button));

            fef.SetValue(System.Windows.Controls.Button.OpacityProperty, 88.888);
            ControlTemplate template = new ControlTemplate(typeof(System.Windows.Controls.Control));
            template.VisualTree = fef;
            s.Setters.Add(new Setter(System.Windows.Controls.Control.TemplateProperty, template));

            System.Windows.Controls.Control c = new System.Windows.Controls.Control();

            c.Style = s;
            c.ApplyTemplate();
            s = c.Style;

            try
            {
                fef.SetValue(System.Windows.Controls.Button.HeightProperty, 99.9999);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /// <summary>
        /// Change style's visual tree after style is sealed
        /// </summary>
        public static void Test4()
        {
            System.Windows.Style s = new System.Windows.Style(typeof(OneNoteElement));
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(System.Windows.Controls.Button));

            fef.SetValue(System.Windows.Controls.Button.OpacityProperty, 88.888);
            ControlTemplate template = new ControlTemplate(typeof(System.Windows.Controls.Control));
            template.VisualTree = fef;
            s.Setters.Add(new Setter(System.Windows.Controls.Control.TemplateProperty, template));

            System.Windows.Controls.Control c = new System.Windows.Controls.Control();

            c.Style = s;
            c.ApplyTemplate();
            s = c.Style;

            try
            {
                template.VisualTree = fef;
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }
    }
    #endregion

    #region used by GetValue, SetValue, ClearValue tests
    /******************************************************************************
    * CLASS:          TestMercuryPlainSimple
    ******************************************************************************/
    /// <summary>
    /// No Cache and No property
    /// </summary>
    public class TestMercuryPlainSimple : System.Windows.DependencyObject
    {
        /// <summary>
        /// A dependencyProperty whose type if decimal
        /// </summary>
        public static readonly DependencyProperty ValueTypeProperty = DependencyProperty.RegisterAttached("ValueType",
            typeof(decimal),
            typeof(TestMercuryPlainSimple));

        /// <summary>
        /// A DependencyProperty whose type is string
        /// </summary>
        public static readonly DependencyProperty ReferenceTypeProperty = DependencyProperty.RegisterAttached("Reference",
            typeof(string),
            typeof(TestMercuryPlainSimple));
    }

    /******************************************************************************
    * CLASS:          TestMercuryPlainSimpleWithDefaultValue
    ******************************************************************************/
    /// <summary>
    /// Provide DefaultValue metadata
    /// </summary>
    public class TestMercuryPlainSimpleWithDefaultValue : DependencyObject
    {
        /// <summary>
        /// A dependencyProperty whose type if decimal
        /// </summary>
        public static readonly DependencyProperty ValueTypeProperty = DependencyProperty.RegisterAttached("ValueType",
            typeof(decimal),
            typeof(TestMercuryPlainSimpleWithDefaultValue));

        /// <summary>
        /// A DependencyProperty whose type is string
        /// </summary>
        public static readonly DependencyProperty ReferenceTypeProperty = DependencyProperty.RegisterAttached("Reference",
            typeof(string),
            typeof(TestMercuryPlainSimpleWithDefaultValue));

        private static PropertyMetadata s_meta1 = new PropertyMetadata(9m);
        private static PropertyMetadata s_meta2 = new PropertyMetadata("9m");

        static TestMercuryPlainSimpleWithDefaultValue()
        {
            ValueTypeProperty.OverrideMetadata(typeof(TestMercuryPlainSimpleWithDefaultValue),
                s_meta1);
            ReferenceTypeProperty.OverrideMetadata(typeof(TestMercuryPlainSimpleWithDefaultValue),
                s_meta2);
        }
    }

    /******************************************************************************
    * CLASS:          TestMercuryPropertyMetadataSealed
    ******************************************************************************/
    /// <summary>
    /// Seal the associated PropertyMetadate
    /// </summary>
    public class TestMercuryPropertyMetadataSealed : DependencyObject
    {
        /// <summary>
        /// A dependencyProperty whose type if decimal
        /// </summary>
        public static readonly DependencyProperty ValueTypeProperty = DependencyProperty.RegisterAttached("ValueType",
            typeof(decimal),
            typeof(TestMercuryPropertyMetadataSealed));

        /// <summary>
        /// A dependencyProperty whose type is string
        /// </summary>
        public static readonly DependencyProperty ReferenceTypeProperty = DependencyProperty.RegisterAttached("Reference",
            typeof(string),
            typeof(TestMercuryPropertyMetadataSealed));

        /// <summary>
        /// MetaData meant for ValueTypeProperty
        /// </summary>
        public static PropertyMetadata meta1;

        /// <summary>
        /// MetaData meant for ReferenceTypeProperty
        /// </summary>
        public static PropertyMetadata meta2 = new PropertyMetadata("SOS", new PropertyChangedCallback(HandlerMercuryPropertyChanged));

        static TestMercuryPropertyMetadataSealed()
        {
            meta1 = new PropertyMetadata(1m);
            meta1.PropertyChangedCallback = new System.Windows.PropertyChangedCallback(HandlerMercuryPropertyChanged);
            ValueTypeProperty.OverrideMetadata(typeof(TestMercuryPropertyMetadataSealed), meta1);
            ReferenceTypeProperty.OverrideMetadata(typeof(TestMercuryPropertyMetadataSealed), meta2);
        }

        private static void HandlerMercuryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

    }
    #endregion

    #region data structors are used by GetValue, SetValue, ClearValue tests
    /******************************************************************************
    * CLASS:          ButtonTypeABase
    ******************************************************************************/
    /// <summary>
    /// 
    /// </summary>
    public class ButtonTypeABase : System.Windows.DependencyObject
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PressedProperty = DependencyProperty.RegisterAttached(
            "Pressed", typeof(decimal), typeof(ButtonTypeABase));

        private static PropertyMetadata s_meta1 = new PropertyMetadata(6m);

        static ButtonTypeABase()
        {
            PressedProperty.OverrideMetadata(typeof(ButtonTypeABase),
                s_meta1);
        }
    }

    /******************************************************************************
    * CLASS:          ButtonTypeADerive1
    ******************************************************************************/
    /// <summary>
    /// 
    /// </summary>
    public class ButtonTypeADerive1 : ButtonTypeABase
    {
        static ButtonTypeADerive1()
        {
            PressedProperty.OverrideMetadata(typeof(ButtonTypeADerive1),
                new PropertyMetadata(6m));
        }
    }

    /******************************************************************************
    * CLASS:          ButtonTypeADerive2
    ******************************************************************************/
    /// <summary>
    /// 
    /// </summary>
    public class ButtonTypeADerive2 : ButtonTypeADerive1
    {
        /// <summary>
        /// 
        /// </summary>
        static public void InvalidOverrideZ3TestedProperty()
        {
            ButtonTypeZDerive3.Z3TestedProperty.OverrideMetadata(typeof(ButtonTypeADerive2),
                new PropertyMetadata("ValidOverride"));
        }
    }

    /******************************************************************************
    * CLASS:          ButtonTypeZBase
    ******************************************************************************/
    /// <summary>
    /// </summary>
    public class ButtonTypeZBase : DependencyObject
    {
    }

    /******************************************************************************
    * CLASS:          ButtonTypeZDerive1
    ******************************************************************************/
    /// <summary>
    /// </summary>
    public class ButtonTypeZDerive1 : ButtonTypeZBase
    {
        /// <summary>
        /// This is invalid because "Property type metadata already registered for this type"
        /// </summary>
        static public void InvalidOverrideZ3TestedProperty()
        {
            ButtonTypeZDerive3.Z3TestedProperty.OverrideMetadata(typeof(ButtonTypeZDerive1), new PropertyMetadata("ValidOverride"));
        }
    }

    /******************************************************************************
    * CLASS:          ButtonTypeZDerive2
    ******************************************************************************/
    /// <summary>
    /// </summary>
    public class ButtonTypeZDerive2 : ButtonTypeZDerive1
    {
    }

    /******************************************************************************
    * CLASS:          ButtonTypeZDerive3
    ******************************************************************************/
    /// <summary>
    /// </summary>
    public class ButtonTypeZDerive3 : ButtonTypeZDerive2
    {
        /// <summary>
        /// One property defined in the derived class will be available to base class
        /// with the default value set at the registration.
        /// Also available to all other classes
        /// </summary>
        public static readonly DependencyProperty Z3TestedProperty = DependencyProperty.RegisterAttached(
            "Z3Tested", typeof(string), typeof(ButtonTypeZDerive3),new PropertyMetadata("DefaultValue"));

        static ButtonTypeZDerive3()
        {
            //            Z3TestedProperty.OverrideMetadata(typeof(ButtonTypeZDerive3),
            //                new PropertyMetadata("OverrideValue")).
        }
    }
    #endregion
}


