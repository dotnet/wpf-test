// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshDependencyProperty
{
    /******************************************************************************
    * CLASS:          RefreshDPBase
    ******************************************************************************/
    /// <summary>
    /// Abstract class providing basic functionality for DP testing.
    /// </summary>
    public abstract class RefreshDPBase : TestCase
    {
        #region Data
        private int _index = -1;
        #endregion


        #region Private and Protected Members
        /******************************************************************************
        * Function:          TestDP
        ******************************************************************************/
        /// <summary>
        /// This method tests correct DP registration
        /// </summary>
        protected void TestDP(string name, Type propertyType, Type ownerType, object validValue, object invalidValue)
        {
            DependencyProperty dp = DependencyProperty.RegisterAttached(name, propertyType, ownerType);
            Utilities.PrintDependencyProperty(dp);

            Utilities.Assert(dp.GlobalIndex > _index, "dp.GlobalIndex increasing");

            if (propertyType.IsValueType)
            {
                Utilities.Assert(!dp.IsValidType(null), "For ValueType, dp.IsValidType(null) is false");
            }
            else
            {
                Utilities.Assert(dp.IsValidType(null), "For ReferenceType, dp.IsValidType(null) is true");
            }
            Utilities.Assert(!dp.IsValidType(DependencyProperty.UnsetValue), "dp.IsValidType(DependencyProperty.UnsetValue) is false");

            Utilities.Assert(dp.IsValidType(validValue), "dp.IsValidType(validValue) is true");
            Utilities.Assert(!dp.IsValidType(invalidValue), "dp.IsValidType(invalidValue) is false");

            _index = dp.GlobalIndex;
        }

        /******************************************************************************
        * Function:          TestBadDP
        ******************************************************************************/
        /// <summary>
        /// This method tests incorrect DP registration
        /// </summary>
        protected void TestBadDP(string name, Type propertyType, Type ownerType, Type expectedExceptionType)
        {
            try
            {
                DependencyProperty.RegisterAttached(name, propertyType, ownerType);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (Exception ex)
            {
                if (ex.GetType() != expectedExceptionType)
                {
                    throw;
                }
                Utilities.ExpectedExceptionReceived(ex);
            }
        }
        #endregion
    }
}
