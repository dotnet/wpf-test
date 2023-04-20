// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xaml;
    using Microsoft.Test.Logging;
    using Microsoft.Test.Xaml.Framework;
    using Microsoft.Test.Xaml.Utilities;

    /// <summary>
    /// Test GetAllXamlNamespaces method.
    /// </summary>
    public abstract class GetAllXamlNamespacesTest : GetXamlNamespacesTest
    {
        #region Public members

        /// <summary>
        /// Call GetAllXamlNamespaces method and verify return value.
        /// </summary>
        public override void Run()
        {
            SetExpectedNamespaces();

            // Load any supporting assemblies
            if (!String.IsNullOrEmpty(DriverState.DriverParameters["SupportingAssemblies"]))
            {
                string assembly = "XamlClrTypes";
                GlobalLog.LogStatus("Loading " + assembly);
                FrameworkHelper.LoadSupportingAssemblies(assembly);
            }

            IEnumerable<string> allXamlNamespaces = null;
            try
            {
                allXamlNamespaces = SchemaContext.GetAllXamlNamespaces();
            }
            catch (XamlSchemaException xse)
            {
                if (StringResourceIdName != null && ExceptionMessageHelper.Match(xse.Message, StringResourceIdName))
                {
                    return;
                }
                else
                {
                    throw;
                }
            }

            GlobalLog.LogDebug("Expected namespace list: ");
            foreach (string ns in ExpectedNamespaces)
            {
                GlobalLog.LogDebug(ns);
            }

            GlobalLog.LogDebug("Returned namespace list: ");
            foreach (string ns in allXamlNamespaces)
            {
                GlobalLog.LogDebug(ns);
            }

            if (!IsExpectedAndUnordered(new List<string>(allXamlNamespaces)))
            {
                throw new SchemaTestFailedException();
            }
        }

        #endregion

        #region Private members

        /// <summary>
        /// Set the ExpectedNamespaces collection.
        /// </summary>
        private void SetExpectedNamespaces()
        {
            SetExpectedNamespacesOverride();

            // Following namespaces are defined in XamlCommon
            ExpectedNamespaces.Add("http://test.schemas.microsoft.com/winfx/2006/xaml/presentation");
            ExpectedNamespaces.Add("http://test.schemas.microsoft.com/netfx/2007/xaml/presentation");
            ExpectedNamespaces.Add("http://Microsoft/Test/Xaml/CustomTypes/NamespaceWithXmlnsDefinition");

            // Following namespaces are defined in System.Xaml
            ExpectedNamespaces.Add("http://schemas.microsoft.com/winfx/2006/xaml");
        }

        /// <summary>
        /// Verify whether returnedNamespaces contains the same elements as ExpectedNamespaces
        /// Elements can be in different order.
        /// </summary>
        /// <param name="returnedNamespaces">Namespace collection returned by GetAllXamlNamespaces.</param>
        /// <returns>True if the collections match. False otherwise.</returns>
        private bool IsExpectedAndUnordered(IList<string> returnedNamespaces)
        {
            if (returnedNamespaces == null)
            {
                GlobalLog.LogDebug("returnedNamespaces is null");
                return false;
            }

            if (returnedNamespaces.Count != ExpectedNamespaces.Count)
            {
                GlobalLog.LogDebug("GetAllXamlNamespaces returned " + returnedNamespaces.Count.ToString(CultureInfo.CurrentCulture) + " namespaces while " + ExpectedNamespaces.Count.ToString(CultureInfo.CurrentCulture) + " namespaces expected");
                return false;
            }

            foreach (string ns1 in returnedNamespaces)
            {
                if (!ExpectedNamespaces.Contains(ns1))
                {
                    GlobalLog.LogDebug("Unexpected namespace encountered");
                    GlobalLog.LogDebug("Returned namespace list: ");
                    foreach (string ns2 in returnedNamespaces)
                    {
                        GlobalLog.LogDebug(ns2);
                    }

                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
