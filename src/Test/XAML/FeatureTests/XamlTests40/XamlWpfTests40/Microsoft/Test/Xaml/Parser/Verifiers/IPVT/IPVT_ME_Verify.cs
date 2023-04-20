// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.Verifiers.IPVT
{
    /// <summary>
    /// Verification class for Xaml File
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\IPVT\IPVT_ME.xaml
    /// </summary>
    public static class IPVT_ME_Verify
    {
        /// <summary>
        /// This method verifies that IPVT returns the right instance and type when used in a MarkupExtension
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns>bool value</returns>
        public static bool Verify(object rootElement)
        {
            bool result = true;
            CustomRootWithCollection root = (CustomRootWithCollection) rootElement;
            int hitCheck = 0;
            foreach (object obj in root.Content)
            {
                Custom_IPVT_DO item = (Custom_IPVT_DO) obj;
                switch (item.Name)
                {
                    case "Test1":
                        result &= IPVT_Verify.VerifyContent(item);
                        hitCheck += 1;
                        break;
                    case "Test2":
                        result &= IPVT_Verify.VerifyDP(item);
                        hitCheck += 3;
                        break;
                    case "Test3":
                        result &= IPVT_Verify.VerifyContent(item);
                        result &= IPVT_Verify.VerifyDP(item);
                        hitCheck += 5;
                        break;
                    default:
                        GlobalLog.LogEvidence("Unexpected item name: " + item.Name);
                        result = false;
                        break;
                }
            }

            if (hitCheck != 9)
            {
                GlobalLog.LogEvidence("Not all cases were hit.  HitCheck = " + hitCheck);
                result = false;
            }

            return result;
        }
    }
}
