// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

namespace Microsoft.Test.ElementServices.PropertyEngine.RegressionTests
{
    /// <summary>
    /// Regression test for 


    [Test(1, @"PropertyEngine.RegressionTests", TestCaseSecurityLevel.FullTrust, "RegressionReadOnlyAttachedProp", SupportFiles=@"FeatureTests\ElementServices\RegressionTests\*.xaml")]
    public class RegressionReadOnlyAttachedProp : XamlTest
    {
        #region Private Data

        private Grid            _grid;
        private CustomListBox   _customListBox;

        #endregion


        #region Constructors

        /******************************************************************************
        * Function:          RegressionReadOnlyAttachedProp Constructor
        ******************************************************************************/
        public RegressionReadOnlyAttachedProp(): base(@"RegressionReadOnlyAttachedProp.xaml")
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(StartTest);
        }

        #endregion

        #region Test Steps

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize: Create a custom ListBox and add it to the page.
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        TestResult Initialize()
        {
            _grid = (Grid)LogicalTreeHelper.FindLogicalNode((Page)Window.Content,"Grid1");

            _customListBox = new CustomListBox();
            
            _grid.Children.Add(_customListBox);

            return TestResult.Pass;
        }

        
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// StartTest:  Verify the Attached property.
        /// </summary>
        /// <returns>TestResult</returns>
        TestResult StartTest()
        {
            int expectedValue = CustomListBox.assignedValue;
            int actualValue = CustomListBox.GetAttachedRO(_customListBox);

            GlobalLog.LogEvidence("Expected Value : " + expectedValue);
            GlobalLog.LogEvidence("Actual Value   : " + actualValue);

            if (actualValue == expectedValue)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        #endregion
    }


    /******************************************************************************
    /******************************************************************************
    * Class:          CustomListBox
    /******************************************************************************
    ******************************************************************************/
    public class CustomListBox : ListBox
    {
        #region Public and Protected Members

        public static int assignedValue = 42;

        static CustomListBox()
        {
            CustomListBox.AttachedROProperty = CustomListBox.AttachedROPropertyKey.DependencyProperty;
        }

        public CustomListBox()
        : base()
        {
            this.SetValue( CustomListBox.AttachedROPropertyKey, assignedValue );
            this.SetValue( CustomListBox.AttachedProperty, 21 );
        }

        protected static readonly DependencyPropertyKey AttachedROPropertyKey =
        DependencyProperty.RegisterAttachedReadOnly( "AttachedROProperty", typeof( int ), typeof( CustomListBox ), new FrameworkPropertyMetadata( -1, FrameworkPropertyMetadataOptions.Inherits ) );

        public static readonly DependencyProperty AttachedROProperty;

        public static int GetAttachedRO( DependencyObject item )
        {
            return ( int )item.GetValue( CustomListBox.AttachedROProperty );
        }

        public static readonly DependencyProperty AttachedProperty =
        DependencyProperty.RegisterAttached( "Attached", typeof( int ), typeof( CustomListBox ), new FrameworkPropertyMetadata( -1, FrameworkPropertyMetadataOptions.Inherits ) );

        public static int GetAttached( DependencyObject item )
        {
            return ( int )item.GetValue( CustomListBox.AttachedProperty );
        }

        #endregion
    }
}

