// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Markup;
using Microsoft.Test.Windows;

namespace Microsoft.Test.Xaml.Utilities
{
    /// <summary>
    /// Helpers for comparing.
    /// </summary>
    public static class ComparisonServices
    {
        /// <summary>
        /// Compares two xaml files
        /// </summary>
        /// <param name="firstFileName">First name of the file.</param>
        /// <param name="secondFileName">Name of the second file.</param>
        /// <returns>bool value</returns>
        public static bool CompareXamlFiles(string firstFileName, string secondFileName)
        {
            XmlCompareResult xmlCompareResult = XamlComparer.CompareFiles(firstFileName, secondFileName);
            if (xmlCompareResult.Result != CompareResult.Equivalent)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Compares two object trees
        /// </summary>
        /// <param name="firstRoot">The first root.</param>
        /// <param name="secondRoot">The second root.</param>
        /// <returns>bool value</returns>
        public static bool CompareObjectTrees(object firstRoot, object secondRoot)
        {
            TreeCompareResult treeCompareResult = TreeComparer.CompareLogical(firstRoot, secondRoot);
            if (treeCompareResult.Result != CompareResult.Equivalent)
            {
                return false;
            }

            return true;
        }
    }
}
