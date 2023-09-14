// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.CoreInput.Common;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.PropertyEngine
{
    /// <summary>
    /// All property engine verifiers in this class
    /// </summary>
    public static partial class Verifiers
    {
        /// <summary>
        /// Used to verify a series of PropertyTrigger-related test cases
        /// </summary>
        /// <param name="root">Root element of the VisualTree</param>
        /// <returns>true when verification completes, false otherwise.</returns>
        public static bool AutoAliasVerifier(StackPanel root)
        {

            VerifySimpleAutoAlias(root);
            VerifyNestedAutoAlias(root);
            VerifyNestedChainedAutoAlias(root);

            return true;
        }

        private static void VerifySimpleAutoAlias(StackPanel root)
        {
            CoreLogger.LogStatus("Verifying auto-aliased content presenter's content: ");

            Button b = (Button)root.FindName("SimpleAutoAlias");
            ControlTemplate ct = b.Template;
            ContentPresenter cp = (ContentPresenter)ct.FindName("CP", b);

            if ((string)cp.Content == "aliased_content")
            {
                CoreLogger.LogStatus("Ok", ConsoleColor.Green);
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Incorrect auto-aliased content, should be aliased_content: " + cp.Content);
            }
        }

        private static void VerifyNestedAutoAlias(StackPanel root)
        {
            CoreLogger.LogStatus("Verifying nested auto-aliased content presenter's content (should be none): ");

            Button b = (Button)root.FindName("NestedCP");
            ControlTemplate ct = b.Template;
            Button outerTemplateButton = (Button)ct.FindName("TemplateTreeControl", b);

            ControlTemplate innerTemplate = outerTemplateButton.Template;
            ContentPresenter innerTemplateCP = (ContentPresenter)innerTemplate.FindName("InnerTemplateCP", outerTemplateButton);


            if (innerTemplateCP.Content == null)
            {
                CoreLogger.LogStatus("Ok - Content is null as expected.", ConsoleColor.Green);
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Incorrect auto-aliased content, should be none: " + innerTemplateCP.Content);
            }
        }

        private static void VerifyNestedChainedAutoAlias(StackPanel root)
        {
            CoreLogger.LogStatus("Verifying chained nested auto-aliased content presenter's content (should be none): ");

            Button b = (Button)root.FindName("ChainedCP");
            ControlTemplate ct = b.Template;
            Button outerTemplateButton = (Button)ct.FindName("TemplateTreeControl", b);

            ControlTemplate innerTemplate = outerTemplateButton.Template;
            ContentPresenter innerTemplateCP = (ContentPresenter)innerTemplate.FindName("InnerTemplateCP", outerTemplateButton);

            ContentPresenter innerNestedCP = (ContentPresenter)innerTemplateCP.Content;

            if ((string)(innerNestedCP.Content) == "aliased_content")
            {
                CoreLogger.LogStatus("Ok", ConsoleColor.Green);
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Incorrect auto-aliased content, should be aliased_content: " + innerTemplateCP.Content);
            }
        }

    }
}
