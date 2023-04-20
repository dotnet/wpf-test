// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace XamlCommon.Microsoft.Test.Xaml.CustomTypes
{
    /// <summary>
    /// Class containing properties with different 
    /// acessors
    /// </summary>
    public class CustomAccessModifiers
    {
        /// <summary>
        /// Initializes a new instance of the CustomAccessModifiers class
        /// </summary>
        public CustomAccessModifiers()
        {
        }

        /// <summary>
        /// Gets or sets Internal property
        /// </summary>
        internal int InternalIntProperty { get; set; }

        /// <summary>
        /// Gets or sets Protected property
        /// </summary>
        protected int ProtectedIntProperty { get; set; }

        /// <summary>
        /// Gets or sets private property
        /// </summary>
        private int PrivateIntProperty { get; set; }
    }
}
