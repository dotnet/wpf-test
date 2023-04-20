// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Schema
{
    using System;
    using System.Xaml;
    using Microsoft.Test.Logging;
    using Microsoft.Test.Xaml.Utilities;

    /// <summary>
    /// Test GetPreferredPrefix method.
    /// </summary>
    public abstract class GetPreferredPrefixTest : SchemaTestBase
    {
        #region Private data

        /// <summary>
        /// Xmlns uri to pass as input to GetPreferredPrefix method.
        /// </summary>
        private readonly string _xmlns = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the GetPreferredPrefixTest class.
        /// </summary>
        /// <param name="xmlns">Xmlns uri to pass as input to GetPreferredPrefix method.</param>
        protected GetPreferredPrefixTest(string xmlns)
        {
            this._xmlns = xmlns;
        }

        #endregion

        #region Protected members

        /// <summary>
        /// Gets or sets the value expected from GetPreferredPrefix.
        /// </summary>
        protected string ExpectedPrefix { get; set; }

        #endregion

        #region Public members

        /// <summary>
        /// Call GetPreferredPrefix method and verify return value.
        /// </summary>
        public override void Run()
        {
            if (StringResourceIdName == null && (ExpectedPrefix == null || _xmlns == null))
            {
                GlobalLog.LogDebug("Must set ExpectedPrefix and Xmlns!");
                throw new SchemaTestFailedException();
            }

            string returnedPrefix = null;
            try
            {
                returnedPrefix = SchemaContext.GetPreferredPrefix(_xmlns);
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

            if (StringResourceIdName != null)
            {
                GlobalLog.LogDebug("No exception is thrown!");
                throw new SchemaTestFailedException();
            }

            if (!returnedPrefix.Equals(ExpectedPrefix))
            {
                GlobalLog.LogDebug("GetPreferredPrefix returned [" + returnedPrefix + "] while expecting [" + ExpectedPrefix + "]");
                throw new SchemaTestFailedException();
            }
        }

        #endregion
    }
}
