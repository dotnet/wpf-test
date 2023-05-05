// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.Verifiers.IPVT
{
    /// <summary>
    /// IPVT Test.  
    /// </summary>
    internal sealed class IPVT_Verify
    {
        /// <summary>
        /// Verifies the content.
        /// </summary>
        /// <param name="item">The Custom_IPVT_DO item.</param>
        /// <returns>bool value</returns>
        internal static bool VerifyContent(Custom_IPVT_DO item)
        {
            bool result = true;
            Custom_IPVTObject returnedItem = item.Content;

            if (returnedItem.TargetPropertyType != "PropertyInfo")
            {
                GlobalLog.LogEvidence("TargetPropertyType was incorrect.  Excepected: PropertyInfo Found: " + returnedItem.TargetPropertyType);
                GlobalLog.LogEvidence("Failing Item: " + item.Name);
                result = false;
            }

            if (!ReferenceEquals(item, returnedItem.TargetObject))
            {
                GlobalLog.LogEvidence("Element and TargetObject were not refernce equals");
                GlobalLog.LogEvidence("Failing Item: " + item.Name);
                result = false;
            }

            return result;
        }

        // This code will be re-enabled once custom attached properties have been implemented

        /*
        internal static bool VerifyLocalAttached(Custom_IPVT_DO item)
        {
            bool result = true;
            Custom_IPVTObject returnedItem = (Custom_IPVTObject)item.GetValue(Custom_IPVT_DO.IPVTAttachedPropertyProperty);

            if (returnedItem.TargetPropertyType != "MethodInfo")
            {
                GlobalLog.LogEvidence("TargetPropertyType was incorrect.  Excepected: MethodInfo Found: " + returnedItem.TargetPropertyType);
                GlobalLog.LogEvidence("Failing Item: " + item.Name);
                result = false;
            }

            if (!Object.ReferenceEquals(item, returnedItem.TargetObject))
            {
                GlobalLog.LogEvidence("Element and TargetObject were not refernce equals");
                GlobalLog.LogEvidence("Failing Item: " + item.Name);
                result = false;
            }
            return result;
        }

        internal static bool VerifyParentAttached(Custom_IPVT_DO item)
        {
            bool result = true;
            Custom_IPVTObject returnedItem = (Custom_IPVTObject)item.GetValue(CustomRootWithCollection.ObjectAttachedPropertyProperty);

            if (returnedItem.TargetPropertyType != "MethodInfo")
            {
                GlobalLog.LogEvidence("TargetPropertyType was incorrect.  Excepected: MethodInfo Found: " + returnedItem.TargetPropertyType);
                GlobalLog.LogEvidence("Failing Item: " + item.Name);
                result = false;
            }

            if (!Object.ReferenceEquals(item, returnedItem.TargetObject))
            {
                GlobalLog.LogEvidence("Element and TargetObject were not refernce equals");
                GlobalLog.LogEvidence("Failing Item: " + item.Name);
                result = false;
            }
            return result;
        }
        */

        /// <summary>
        /// Verifies the DP.
        /// </summary>
        /// <param name="item">The Custom_IPVT_DO item.</param>
        /// <returns>bool value</returns>
        internal static bool VerifyDP(Custom_IPVT_DO item)
        {
            bool result = true;
            Custom_IPVTObject returnedItem = (Custom_IPVTObject) item.GetValue(Custom_IPVT_DO.IPVTDependencyPropertyProperty);

            if (returnedItem.TargetPropertyType != "DependencyProperty")
            {
                GlobalLog.LogEvidence("TargetPropertyType was incorrect.  Excepected: DependencyProperty Found: " + returnedItem.TargetPropertyType);
                GlobalLog.LogEvidence("Failing Item: " + item.Name);
                result = false;
            }

            if (!ReferenceEquals(item, returnedItem.TargetObject))
            {
                GlobalLog.LogEvidence("Element and TargetObject were not refernce equals");
                GlobalLog.LogEvidence("Failing Item: " + item.Name);
                result = false;
            }

            return result;
        }
    }
}
