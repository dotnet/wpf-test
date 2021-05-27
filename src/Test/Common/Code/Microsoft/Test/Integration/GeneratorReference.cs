// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace Microsoft.Test.Integration
{
    /// <summary>
    /// 
    /// </summary>
    [MarkupExtensionReturnType(typeof(IVariationGenerator))]
    public class GeneratorReference : MarkupExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (String.IsNullOrEmpty(_targetName))
            {
                throw new InvalidOperationException("TargetName is missing.");
            }

            return TestExtenderGraph.Current.FindName(_targetName) as IVariationGenerator;
        }

        /// <summary>
        /// 
        /// </summary>
        public string TargetName
        {
            get
            {
                return _targetName;
            }
            set
            {
                _targetName = value;
            }
        }

        string _targetName = "";
    }
}
