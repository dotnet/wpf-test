// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Xaml;
    using Microsoft.Test.Logging;
    using Microsoft.Test.Xaml.Utilities;

    /// <summary>
    /// Test GetXamlNamespaces method.
    /// </summary>
    public abstract class GetXamlNamespacesTest : SchemaTestBase
    {
        #region Private data

        /// <summary>
        /// Input to GetXamlNamespaces method.
        /// </summary>
        private readonly Type _typeToPassIn = null;

        /// <summary>
        /// Backing store for ExpectedNamespaces property.
        /// </summary>
        private List<string> _expectedNamespaces = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the GetXamlNamespacesTest class.
        /// </summary>
        protected GetXamlNamespacesTest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetXamlNamespacesTest class.
        /// </summary>
        /// <param name="typeToPassIn">Input to GetXamlNamespaces method.</param>
        protected GetXamlNamespacesTest(Type typeToPassIn)
        {
            this._typeToPassIn = typeToPassIn;
        }

        #endregion

        #region Protected properties

        /// <summary>
        /// Gets expected namespaces collection.
        /// </summary>
        protected List<string> ExpectedNamespaces
        {
            get
            {
                if (_expectedNamespaces == null)
                {
                    _expectedNamespaces = new List<string>();
                }

                return _expectedNamespaces;
            }
        }

        #endregion

        #region Public members

        /// <summary>
        /// Call GetXamlNamespaces method and verify return value.
        /// </summary>
        public override void Run()
        {
            SetExpectedNamespaces();

            XamlType xamlType = SchemaContext.GetXamlType(_typeToPassIn);
            IList<string> namespacesFromType = null;
            try
            {
                namespacesFromType = xamlType.GetXamlNamespaces();
            }
            catch (XamlSchemaException xse)
            {
                if (StringResourceIdName != null && ExceptionMessageHelper.Match(xse.Message, StringResourceIdName))
                {
                    return; // Pass
                }
                else
                {
                    throw;
                }
            }
            catch (InvalidOperationException ioe)
            {
                // error can be wrapped in an InvalidOperationException
                if (ioe.InnerException == null || 
                    ioe.InnerException.GetType() != typeof(XamlSchemaException) || 
                    StringResourceIdName == null)
                {
                    throw;
                }

                if (ExceptionMessageHelper.Match(ioe.InnerException.Message, StringResourceIdName))
                {
                    return;
                }

                throw;
            }

            if (StringResourceIdName != null)
            {
                GlobalLog.LogDebug("No exception is thrown!");
                throw new SchemaTestFailedException();
            }

            GlobalLog.LogDebug("Expected namespace list: ");
            foreach (string ns in ExpectedNamespaces)
            {
                GlobalLog.LogDebug(ns);
            }

            GlobalLog.LogDebug("Returned namespace list: ");
            foreach (string ns in namespacesFromType)
            {
                GlobalLog.LogDebug(ns);
            }

            if (!IsExpectedAndOrdered(namespacesFromType))
            {
                GlobalLog.LogDebug("XamlType.GetXamlNamespaces: Fail");
                throw new SchemaTestFailedException();
            }

            GlobalLog.LogDebug("XamlType.GetXamlNamespaces: Pass");

            PropertyInfo[] propertyInfos = _typeToPassIn.GetProperties(BindingFlags.DeclaredOnly);
            if (propertyInfos != null && propertyInfos.Length > 0)
            {
                XamlMember xamlProperty = xamlType.GetMember(propertyInfos[0].Name);

                IList<string> namespacesFromProperty = null;
                try
                {
                    namespacesFromProperty = xamlProperty.GetXamlNamespaces();
                }
                catch (XamlSchemaException xse)
                {
                    if (StringResourceIdName != null && ExceptionMessageHelper.Match(xse.Message, StringResourceIdName))
                    {
                        return; // Pass
                    }
                    else
                    {
                        throw;
                    }
                }

                if (StringResourceIdName != null)
                {
                    GlobalLog.LogDebug("No exception is thrown!");
                    throw new SchemaTestFailedException();
                }

                GlobalLog.LogDebug("Expected namespace list: ");
                foreach (string ns in ExpectedNamespaces)
                {
                    GlobalLog.LogDebug(ns);
                }

                GlobalLog.LogDebug("Returned namespace list: ");
                foreach (string ns in namespacesFromProperty)
                {
                    GlobalLog.LogDebug(ns);
                }

                if (!IsExpectedAndOrdered(namespacesFromProperty))
                {
                    GlobalLog.LogDebug("XamlProperty.GetXamlNamespaces: Fail");
                    throw new SchemaTestFailedException();
                }

                GlobalLog.LogDebug("XamlProperty.GetXamlNamespaces: Pass");
            }
        }

        #endregion

        #region Protected members

        /// <summary>
        /// Mandatory method to be overridden by derived classes to populate ExpectedNamespaces collection.
        /// </summary>
        protected abstract void SetExpectedNamespacesOverride();

        #endregion

        #region Private members

        /// <summary>
        /// Set the ExpectedNamespaces collection.
        /// </summary>
        private void SetExpectedNamespaces()
        {
            SetExpectedNamespacesOverride();
            AssemblyName assemblyName = new AssemblyName(_typeToPassIn.Assembly.FullName);
            ExpectedNamespaces.Add("clr-namespace:" + _typeToPassIn.Namespace + ";assembly=" + assemblyName.Name);
        }

        /// <summary>
        /// Verify whether returnedNamespaces contains the same elements as ExpectedNamespaces
        /// Elements should be in same order.
        /// </summary>
        /// <param name="returnedNamespaces">Namespace collection returned by GetXamlNamespaces.</param>
        /// <returns>True if the collections match. False otherwise.</returns>
        private bool IsExpectedAndOrdered(IList<string> returnedNamespaces)
        {
            if (returnedNamespaces == null)
            {
                GlobalLog.LogDebug("returnedNamespaces is null");
                return false;
            }

            if (returnedNamespaces.Count != ExpectedNamespaces.Count)
            {
                GlobalLog.LogDebug("GetXamlNamespaces returned " + returnedNamespaces.Count.ToString(CultureInfo.CurrentCulture) + " namespaces while " + ExpectedNamespaces.Count.ToString(CultureInfo.CurrentCulture) + " namespaces expected");
                return false;
            }

            for (int index = 0; index < returnedNamespaces.Count; index++)
            {
                if (!returnedNamespaces[index].Equals(ExpectedNamespaces[index]))
                {
                    GlobalLog.LogDebug("Unexpected namespace encountered");
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
