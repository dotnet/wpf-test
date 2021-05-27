// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

﻿
using System;
using System.Linq.Expressions;

namespace Microsoft.Test.VariationGeneration.Constraints
{
    /// <summary>
    /// Represents a condition if the expression in the predicate is true.
    /// </summary>
    /// <typeparam name="T">The type of variation being operated on.</typeparam>
    public class IfThenConstraint<T> : ConditionalConstraint<T> where T : new()
    {
        internal IfThenConstraint(IfPredicate<T> ifTest, Expression<Func<T, bool>> predicate) 
        { 
            Condition = predicate;
            IfPredicate = ifTest;

            InnerConstraint = new ConditionalConstraint<T>(BuildInternalConstraint(IfPredicate.Predicate.Body, Condition.Body, Expression.Constant(true)));
        }

        internal ConditionalConstraint<T> InnerConstraint { get; private set; }

        internal IfPredicate<T> IfPredicate { get; private set; }

        internal override ParameterInteraction GetExcludedCombinations(Model<T> model)
        {
            return InnerConstraint.GetExcludedCombinations(model);
        }

        internal override ConstraintSatisfaction SatisfiesContraint(Model<T> model, ValueCombination combination)
        {
            return InnerConstraint.SatisfiesContraint(model, combination);
        }

        internal static Expression<Func<T, bool>> BuildInternalConstraint(Expression ifExpr, Expression thenExpr, Expression elseExpr)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "parameter");
            LambdaParameterExpressionVisitor visitor = new LambdaParameterExpressionVisitor(parameter);

            Expression conditional = Expression.Condition(visitor.ReplaceParameter(ifExpr), visitor.ReplaceParameter(thenExpr), visitor.ReplaceParameter(elseExpr));

            return (Expression<Func<T, bool>>)Expression.Lambda(conditional, parameter);
        }
    }
}
