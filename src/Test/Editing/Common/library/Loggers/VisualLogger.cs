// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides logging services to describe a visual tree.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Loggers/VisualLogger.cs $")]

namespace Test.Uis.Loggers
{
    #region Namespaces.

    using System;
    using System.Collections;    
    using System.Text;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>
    /// Provides methods to log information about text tree objects.
    /// </summary>
    public static class VisualLogger
    {

        #region Public methods.

        /// <summary>
        /// Describes the visual tree rooted at the specified IVisual.
        /// </summary>
        /// <param name='root'>visual root of tree to describe.</param>
        /// <returns>A multiline description of the instance.</returns>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void LogWindowTree(Window w) {
        ///   Logger.Current.Log(VisualLogger.DescribeVisualTree(Visual w);
        /// }</code></example>
        public static string DescribeVisualTree(Visual root)
        {
            if (root == null)
                throw new ArgumentNullException("root");

            StringBuilder sb = new StringBuilder();
            DescribeVisualTree(root, "  ", sb);
            return sb.ToString();
        }

        #endregion Public methods.

        #region Private methods.

        private static void DescribeVisualTree(DependencyObject root, string indent,
            StringBuilder sb)
        {
            sb.Append(indent);
            sb.Append(root.ToString());
            sb.Append(" [VisualContentBounds=");
            if (root is Visual)
            {
                sb.Append(VisualTreeHelper.GetContentBounds((Visual)root).ToString());
            }
            else if (root is Visual3D)
            {
                sb.Append(VisualTreeHelper.GetContentBounds((Visual3D)root).ToString());
            }
            sb.Append("]");
            sb.Append(System.Environment.NewLine);

            string childIndent = indent + "  ";
            int count = VisualTreeHelper.GetChildrenCount(root);
            for(int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(root, i);
                if (child == null)
                {
                    sb.Append(childIndent);
                    sb.Append("[null]");
                    sb.Append(System.Environment.NewLine);
                }
                else
                {
                    DescribeVisualTree(child, childIndent, sb);
                }
            }
        }

        #endregion Private methods.
    }
}