// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

﻿
using System.Linq.Expressions;

namespace Microsoft.Test.VariationGeneration.Constraints
{
    class LambdaParameterExpressionVisitor : ExpressionVisitor
    {
        
        public LambdaParameterExpressionVisitor(ParameterExpression parameter)
        {
            this.parameter = parameter;
        }

        public Expression ReplaceParameter(Expression exp)
        {
            return Visit(exp);
        }

        ParameterExpression parameter;

        protected override Expression VisitParameter(ParameterExpression p)
        {
            return this.parameter;
        }

        
    }
}
