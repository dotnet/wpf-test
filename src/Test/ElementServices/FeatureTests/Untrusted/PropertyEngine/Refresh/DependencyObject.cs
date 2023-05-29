// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
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


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshDependencyObjectTest
{
    /******************************************************************************
    * CLASS:          TestDependencyObject
    ******************************************************************************/
    [Test(0, "PropertyEngine.DependencyObject", TestCaseSecurityLevel.FullTrust, "TestDependencyObject")]
    public class TestDependencyObject : TestCase
    {
        #region Constructor
        /******************************************************************************
        * Function:          TestDependencyObject Constructor
        ******************************************************************************/
        public TestDependencyObject()
        {
            RunSteps += new TestStep(Bug51);
            RunSteps += new TestStep(TestConstructor);
            RunSteps += new TestStep(GetSetClear);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          Bug51
        ******************************************************************************/
        /// <summary>
        /// SetValue should allow DependencyProperty.UnsetValue. It is treated the same as ClearValue
        /// </summary>
        TestResult Bug51()
        {
            Utilities.PrintTitle("Bug51: SetValue should allow DependencyProperty.UnsetValue. It is treated the same as ClearValue");

            TestMercuryPlainSimpleWithDefaultValue demo = new TestMercuryPlainSimpleWithDefaultValue();
            decimal v1 = (decimal)demo.GetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);
            string v2 = (string)demo.GetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);
            Utilities.Assert(v1 == 9m, "Defalut value verified for value type");
            Utilities.Assert(v2 == "9m", "Default value verified for reference type");
            Utilities.PrintStatus("Change value using SetValue");
            demo.SetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty, 200.20m);
            demo.SetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty, "200.20m");
            v1 = (decimal)demo.GetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);
            v2 = (string)demo.GetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);
            Utilities.Assert(v1 == 200.20m, "New value verified for value type");
            Utilities.Assert(v2 == "200.20m", "New value verified for reference type");
            Utilities.PrintStatus("Clear Value using SetValue(DependencyProperty.UnsetValue)");
            demo.SetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty, DependencyProperty.UnsetValue);
            demo.SetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty, DependencyProperty.UnsetValue);
            v1 = (decimal)demo.GetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);
            v2 = (string)demo.GetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);
            Utilities.Assert(v1 == 9m, "Defalut value verified for value type");
            Utilities.Assert(v2 == "9m", "Default value verified for reference type");

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          TestConstructor
        ******************************************************************************/
        /// <summary>
        /// Test Constructors, DependecyObjectType property
        /// </summary>
        TestResult TestConstructor()
        {
            Utilities.PrintTitle("Test Constructors, DependecyObjectType property");

            Utilities.PrintStatus("Constructor 1");
            TestMercuryPlainSimple test = new TestMercuryPlainSimple();
            Utilities.Assert((decimal)test.GetValue(TestMercuryPlainSimple.ValueTypeProperty) == 0m, "Value Type Property Verified");
            Utilities.Assert(test.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty) == null, "Reference Type Property Verified");
            Utilities.Assert(test.DependencyObjectType == DependencyObjectType.FromSystemType(typeof(TestMercuryPlainSimple)), "DependencyPropertyType property verified");
            Utilities.Assert(test.Dispatcher != null, "Dispather is not null");

            Utilities.PrintStatus("Constructor 2");
            TestMercury test1 = new TestMercury();
            Utilities.Assert(test1.Delta == 3.14, "Default Value Validated");

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          GetSetClear
        ******************************************************************************/
        /// <summary>
        /// Mainly for testing GetValue, SetValue and ClearValue
        /// </summary>
        TestResult GetSetClear()
        {
            Utilities.PrintTitle("GetValue, SetValue & ClearValue");
            Utilities.PrintStatus("GetValue, SetValue, ClearValue and GetLocalValue on DependencyObject directly with no CLR accessors or native cache support");

            TestMercuryPlainSimple mps1 = new TestMercuryPlainSimple();
            TestMercuryPlainSimple mps2 = new TestMercuryPlainSimple();

            Utilities.PrintStatus("(1) ValueType Dependency Property");
            {
                //GetValue --- mps1
                object obj1 = mps1.GetValue(TestMercuryPlainSimple.ValueTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj1.ToString());
                Utilities.Assert((decimal)obj1 == 0m,
                    "GetValue is 0");

                //GetLoclValue --- mps1
                object obj2 = mps1.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj2.ToString());
                Utilities.Assert(obj2 == DependencyProperty.UnsetValue,
                    "GetLoclValue is DependencyProperty.UnsetValue");

                //SetValue --- mps1
                mps1.SetValue(TestMercuryPlainSimple.ValueTypeProperty,
                    99m);

                //GetValue after SetValue --- mps1
                object obj3 = mps1.GetValue(TestMercuryPlainSimple.ValueTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj3.ToString());
                Utilities.Assert((decimal)obj3 == 99m,
                    "GetValue is 99");

                //GetLoclValue after SetValue --- mps1
                object obj4 = mps1.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj4.ToString());
                Utilities.Assert((Decimal)obj4 == 99m,
                    "GetLoclValue is 99");

                //It should not affect mps2
                //GetValue --- mps2
                object obj5 = mps2.GetValue(TestMercuryPlainSimple.ValueTypeProperty);

                Utilities.PrintStatus("mps2.GetValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj5.ToString());
                Utilities.Assert((decimal)obj5 == 0m,
                    "GetValue is 0");

                //GetLoclValue --- mps2
                object obj6 = mps2.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty);

                Utilities.PrintStatus("mps2.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj6.ToString());
                Utilities.Assert(obj6 == DependencyProperty.UnsetValue,
                    "GetLoclValue is DependencyProperty.UnsetValue");

                //ClearValue --- mps1
                mps1.ClearValue(TestMercuryPlainSimple.ValueTypeProperty);

                //GetValue after ClearValue --- mps1
                object obj7 = mps1.GetValue(TestMercuryPlainSimple.ValueTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj7.ToString());
                Utilities.Assert((decimal)obj7 == 0m,
                    "GetValue is 0");

                //GetLoclValue --- mps1
                object obj8 = mps1.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj8.ToString());
                Utilities.Assert(obj8 == DependencyProperty.UnsetValue,
                    "GetLoclValue is DependencyProperty.UnsetValue");
            }
            Utilities.PrintStatus("(2) ReferenceType Dependency Property");
            {
                //GetValue --- mps1
                object obj1 = mps1.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + (obj1 == null ? "(null)" : obj1.ToString()));
                Utilities.Assert(obj1 == null,
                    "GetValue is null");

                //GetLoclValue --- mps1
                object obj2 = mps1.ReadLocalValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + obj2.ToString());
                Utilities.Assert(obj2 == DependencyProperty.UnsetValue,
                    "GetLoclValue is DependencyProperty.UnsetValue");

                //SetValue --- mps1
                mps1.SetValue(TestMercuryPlainSimple.ReferenceTypeProperty,
                    "WPP");

                //GetValue after SetValue --- mps1
                object obj3 = mps1.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + obj3.ToString());
                Utilities.Assert((string)obj3 == "WPP",
                    "GetValue is WPP");

                //GetLoclValue after SetValue --- mps1
                object obj4 = mps1.ReadLocalValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + obj4.ToString());
                Utilities.Assert((string)obj4 == "WPP",
                    "GetLoclValue is WPP");

                //It should not affect mps2
                //GetValue --- mps2
                object obj5 = mps2.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                Utilities.PrintStatus("mps2.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + (obj5 == null ? "(null)" : obj5.ToString()));
                Utilities.Assert(obj5 == null,
                    "GetValue is null");

                //GetLoclValue --- mps2
                object obj6 = mps2.ReadLocalValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                Utilities.PrintStatus("mps2.ReadLocalValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + obj6.ToString());
                Utilities.Assert(obj6 == DependencyProperty.UnsetValue,
                    "GetLoclValue is DependencyProperty.UnsetValue");

                //ClearValue --- mps1
                mps1.ClearValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                //GetValue after ClearValue --- mps1
                object obj7 = mps1.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + (obj7 == null ? "(null)" : obj5.ToString()));
                Utilities.Assert(obj7 == null,
                    "GetValue is null");

                //GetLoclValue --- mps1
                object obj8 = mps1.ReadLocalValue(TestMercuryPlainSimple.ReferenceTypeProperty);

                Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + obj8.ToString());
                Utilities.Assert(obj8 == DependencyProperty.UnsetValue,
                    "GetLoclValue is DependencyProperty.UnsetValue");
            }

            Utilities.PrintTitle("GetValue, SetValue, ClearValue and GetLocalValue on DependencyObject directly with no CLR accessors or native cache support. However, defaultValue is specifically provided via Metadata.");
            {
                TestMercuryPlainSimpleWithDefaultValue mps1a = new TestMercuryPlainSimpleWithDefaultValue();
                TestMercuryPlainSimpleWithDefaultValue mps2a = new TestMercuryPlainSimpleWithDefaultValue();
                Utilities.PrintStatus("(1) ValueType Dependency Property");
                {
                    //GetValue --- mps1
                    object obj1 = mps1a.GetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                    Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj1.ToString());
                    Utilities.Assert((decimal)obj1 == 9m,
                        "GetValue is 9");

                    //GetLoclValue --- mps1
                    object obj2 = mps1a.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                    Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj2.ToString());
                    Utilities.Assert(obj2 == DependencyProperty.UnsetValue,
                        "GetLoclValue is DependencyProperty.UnsetValue");

                    //SetValue --- mps1
                    mps1a.SetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty,
                        99m);

                    //GetValue after SetValue --- mps1
                    object obj3 = mps1a.GetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                    Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ValueTypeProperty) is " + obj3.ToString());
                    Utilities.Assert((decimal)obj3 == 99m,
                        "GetValue is 99");

                    //GetLoclValue after SetValue --- mps1
                    object obj4 = mps1a.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                    Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty) is " + obj4.ToString());
                    Utilities.Assert((Decimal)obj4 == 99m,
                        "GetLoclValue is 99");

                    //It should not affect mps2
                    //GetValue --- mps2
                    object obj5 = mps2a.GetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                    Utilities.PrintStatus("mps2.GetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty) is " + obj5.ToString());
                    Utilities.Assert((decimal)obj5 == 9m,
                        "GetValue is 9");

                    //GetLoclValue --- mps2
                    object obj6 = mps2a.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                    Utilities.PrintStatus("mps2.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty) is " + obj6.ToString());
                    Utilities.Assert(obj6 == DependencyProperty.UnsetValue,
                        "GetLoclValue is DependencyProperty.UnsetValue");

                    //ClearValue --- mps1
                    mps1a.ClearValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                    //GetValue after ClearValue --- mps1
                    object obj7 = mps1a.GetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                    Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty) is " + obj7.ToString());
                    Utilities.Assert((decimal)obj7 == 9m,
                        "GetValue is 9");

                    //GetLoclValue --- mps1
                    object obj8 = mps1a.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty);

                    Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ValueTypeProperty) is " + obj8.ToString());
                    Utilities.Assert(obj8 == DependencyProperty.UnsetValue,
                        "GetLoclValue is DependencyProperty.UnsetValue");
                }

                Utilities.PrintStatus("(2) ReferenceType Dependency Property");
                {
                    //GetValue --- mps1
                    object obj1 = mps1a.GetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                    Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty) is " + (obj1 == null ? "(null)" : obj1.ToString()));
                    Utilities.Assert((string)obj1 == "9m",
                        "GetValue is 9m");

                    //GetLoclValue --- mps1
                    object obj2 = mps1a.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                    Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty) is " + obj2.ToString());
                    Utilities.Assert(obj2 == DependencyProperty.UnsetValue,
                        "GetLoclValue is DependencyProperty.UnsetValue");

                    //SetValue --- mps1
                    mps1a.SetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty,
                        "WPP");

                    //GetValue after SetValue --- mps1
                    object obj3 = mps1a.GetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                    Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimple.ReferenceTypeProperty) is " + obj3.ToString());
                    Utilities.Assert((string)obj3 == "WPP",
                        "GetValue is WPP");

                    //GetLoclValue after SetValue --- mps1
                    object obj4 = mps1a.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                    Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty) is " + obj4.ToString());
                    Utilities.Assert((string)obj4 == "WPP",
                        "GetLoclValue is WPP");

                    //It should not affect mps2
                    //GetValue --- mps2
                    object obj5 = mps2a.GetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                    Utilities.PrintStatus("mps2.GetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty) is " + (obj5 == null ? "(null)" : obj5.ToString()));
                    Utilities.Assert((string)obj5 == "9m",
                        "GetValue is 9m");

                    //GetLoclValue --- mps2
                    object obj6 = mps2a.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                    Utilities.PrintStatus("mps2.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty) is " + obj6.ToString());
                    Utilities.Assert(obj6 == DependencyProperty.UnsetValue,
                        "GetLoclValue is DependencyProperty.UnsetValue");

                    //ClearValue --- mps1
                    mps1a.ClearValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                    //GetValue after ClearValue --- mps1
                    object obj7 = mps1a.GetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                    Utilities.PrintStatus("mps1.GetValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty) is " + (obj7 == null ? "(null)" : obj5.ToString()));
                    Utilities.Assert((string)obj7 == "9m",   //Actually [obj == "9m"] also works. But with compiler warning;
                      "GetValue is 9m");

                    //GetLoclValue --- mps1
                    object obj8 = mps1a.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty);

                    Utilities.PrintStatus("mps1.ReadLocalValue(TestMercuryPlainSimpleWithDefaultValue.ReferenceTypeProperty) is " + obj8.ToString());
                    Utilities.Assert(obj8 == DependencyProperty.UnsetValue,
                        "GetLoclValue is DependencyProperty.UnsetValue");
                }
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          TestMercuryPlainSimple
    ******************************************************************************/
    /// <summary>
    /// Class TestMercuryPlainSimple derives from DependencyObject
    /// It contains two Dependency Properties. 
    /// There is no CLR Property, Cache support or associated metadataa
    /// </summary>
    public class TestMercuryPlainSimple : System.Windows.DependencyObject
    {
        #region TestMercuryPlainSimple Members
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

        /// <summary>
        /// Constructor that does nothing
        /// </summary>
        public TestMercuryPlainSimple()
        {
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          TestMercury
    ******************************************************************************/
    /// <summary>
    /// Based on the sample class from spec, TestMercury
    /// class uses native cache to optimize property value access
    /// </summary>
    public class TestMercury : System.Windows.DependencyObject
    {
        #region TestMercury Members
        /// <summary>
        ///     The dependency property for the DeltaProperty.
        /// </summary>
        public static readonly DependencyProperty DeltaProperty = DependencyProperty.Register("Delta",
            typeof(double),
            typeof(TestMercury),
            new PropertyMetadata(3.14));

        /// <summary>
        /// Just delta
        /// </summary>
        public double Delta
        {
            get { return (double) GetValue(DeltaProperty); }
            set { SetValue(DeltaProperty, value); }
        }

        /// <summary>
        /// Default ctor
        /// </summary>
        public TestMercury()
        {
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          TestMercuryPropertyMetadataSealed
    ******************************************************************************/
    /// <summary>
    /// Seal the associated PropertyMetadate
    /// </summary>
    public class TestMercuryPropertyMetadataSealed : System.Windows.DependencyObject
    {
        #region TestMercuryPropertyMetadataSealed Members
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

        /// <summary>
        /// The constructor that does nothing
        /// </summary>
        public TestMercuryPropertyMetadataSealed()
        {
        }

        static TestMercuryPropertyMetadataSealed()
        {
            meta1 = new PropertyMetadata(1m);
            meta1.PropertyChangedCallback = new PropertyChangedCallback(HandlerMercuryPropertyChanged);
            ValueTypeProperty.OverrideMetadata(typeof(TestMercuryPropertyMetadataSealed),
                meta1);
            ReferenceTypeProperty.OverrideMetadata(typeof(TestMercuryPropertyMetadataSealed),
                meta2);
        }

        private static void HandlerMercuryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion
    }

    /******************************************************************************
    * CLASS:          TestMercuryPlainSimpleWithDefaultValue
    ******************************************************************************/
    /// <summary>
    /// This is a test class that is derived from DependencyObject
    /// and it has Provide DefaultValue metadata
    /// </summary>
    public class TestMercuryPlainSimpleWithDefaultValue : DependencyObject
    {
        #region TestMercuryPlainSimpleWithDefaultValue Members
        /// <summary>
        /// A dependencyProperty whose type is decimal
        /// </summary>
        public static readonly DependencyProperty ValueTypeProperty = DependencyProperty.Register("ValueType",
            typeof(decimal),
            typeof(TestMercuryPlainSimpleWithDefaultValue));

        /// <summary>
        /// A DependencyProperty whose type is string
        /// </summary>
        public static readonly DependencyProperty ReferenceTypeProperty = DependencyProperty.Register("Reference",
            typeof(string),
            typeof(TestMercuryPlainSimpleWithDefaultValue));

        private static PropertyMetadata s_meta1 = new PropertyMetadata(9m);
        private static PropertyMetadata s_meta2 = new PropertyMetadata("9m");

        /// <summary>
        /// The constructor that does nothing
        /// </summary>
        public TestMercuryPlainSimpleWithDefaultValue()
        {
        }

        static TestMercuryPlainSimpleWithDefaultValue()
        {
            ValueTypeProperty.OverrideMetadata(typeof(TestMercuryPlainSimpleWithDefaultValue),
                s_meta1);
            ReferenceTypeProperty.OverrideMetadata(typeof(TestMercuryPlainSimpleWithDefaultValue),
                s_meta2);
        }
        #endregion
    }
}

