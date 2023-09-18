// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Reflection;

namespace Microsoft.Test.KoKoMo
{
    /// <summary>
    /// ModelFunction delegate
    /// </summary>
    /// <returns></returns>
    public delegate object ModelFunction();

    /// <summary>
    /// ModelExpresion
    /// </summary>
    public class ModelExpression : ModelRequirement
    {
        //Data
        ModelFunction _func = null;

        
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="value"></param>
        public ModelExpression(ModelFunction func, ModelValue value)
            : base(null, value)
        {
            _func = func;
        }

        /// <summary>
        /// Evaluate method
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        public override bool Evaluate(Object expected)
        {
            return base.Evaluate(_func());
        }
    }
}
