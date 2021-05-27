// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

﻿
using System;
using System.Linq.Expressions;

namespace Microsoft.Test.VariationGeneration.Constraints
{
    /// <summary>
    /// Optionally represents a condition if the expression in <see cref="IfPredicate{T}"/> is false.
    /// </summary>
    /// <typeparam name="T">The type of variation being operated on.</typeparam>
    public class IfThenElseConstraint<T> : ConditionalConstraint<T> where T : new()
    {
        internal IfThenElseConstraint(IfThenConstraint<T> ifThen, Expression<Func<T, bool>> predicate) 
        { 
            Condition = predicate;
            IfThen = ifThen;

            InnerConstraint = new ConditionalConstraint<T>(IfThenConstraint<T>.BuildInternalConstraint(IfThen.IfPredicate.Predicate.Body, IfThen.Condition.Body, Condition.Body));
        }

        internal IfThenConstraint<T> IfThen { get; private set; }

        internal ConditionalConstraint<T> InnerConstraint { get; private set; }

        internal override ParameterInteraction GetExcludedCombinations(Model<T> model)
        {
            return InnerConstraint.GetExcludedCombinations(model);
        }

        internal override ConstraintSatisfaction SatisfiesContraint(Model<T> model, ValueCombination combination)
        {
            return InnerConstraint.SatisfiesContraint(model, combination);
        }
    }
}
