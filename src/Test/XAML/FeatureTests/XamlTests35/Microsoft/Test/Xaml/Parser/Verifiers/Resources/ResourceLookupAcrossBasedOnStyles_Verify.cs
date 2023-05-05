// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using System.Text;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Resources
{
    ///<summary>
    /// Verification class for Xaml File
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\Resources\ResourceLookupAcrossBasedOnStyles.xaml
    /// </summary>
    public class ResourceLookupAcrossBasedOnStyles_Verify
    {       

        /// <summary>
        /// Method verifies that the proper resource is found when looking up across styles based on other styles
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns> bool value </returns>
        public static bool Verify(object rootElement)
        {
            bool result = true;
            string GridRes = "GridRes";
            string StyleARes = "StyleARes";
            string StyleBRes = "StyleBRes";
            string StyleCRes = "StyleCRes";

            Grid grid = (Grid) rootElement;
            StackPanel sp = (StackPanel) grid.Children[0];            

            result &= VerifyContent((Button)sp.Children[0], StyleARes);
            result &= VerifyContent((Button)sp.Children[1], StyleBRes);
            result &= VerifyContent((Button)sp.Children[2], StyleCRes);
            result &= VerifyContent((Button)sp.Children[3], StyleBRes);
            result &= VerifyContent((Button)sp.Children[4], StyleARes);
            result &= VerifyContent((Button)sp.Children[5], StyleARes);
            result &= VerifyContent((Button)sp.Children[6], GridRes);
            result &= VerifyContent((Button)sp.Children[7], GridRes);
            result &= VerifyContent((Button)sp.Children[8], GridRes);

            return result;
        }

        private static bool VerifyContent(Button button, string expected)
        {
            if (!String.Equals(button.Content, expected))
            {
                TestLog.Current.LogEvidence(String.Format("{0}'s content was: {1}, should have been {2}", button.Name, button.Content, expected));
                TestLog.Current.Result = TestResult.Fail; 
                return false;
            }
            return true;
        }
    }
}
