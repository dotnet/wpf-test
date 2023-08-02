// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Navigation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary></summary>
    public class HyperlinkCommon
    {
        public static bool NavigationWindowSourceContainsString(NavigationWindow navWin, string stringValue)
        {
            string navWinSource = "";
            if (navWin.Source != null)
            {
                navWinSource = navWin.Source.ToString();
            }
            else
            {
                GlobalLog.LogEvidence("The source for the NavigationWindow is null.");
                return false;
            }

            if (!navWinSource.Contains(stringValue))
            {
                GlobalLog.LogEvidence(string.Format("The source for the NavigationWindow is not as expected.  Expected: {0} Actual: {1}", stringValue, navWin.Source));
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
