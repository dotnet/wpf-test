// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

﻿
using System;
using System.Linq.Expressions;

namespace Microsoft.Test.VariationGeneration.Constraints
{
    /// <summary>
    /// Represents the predicate of a constraint with a logical implication.
    /// </summary>
    /// <typeparam name="T">The type of variation being operated on.</typeparam>
    public class IfPredicate<T> where T : new()
    {
        internal IfPredicate(Expression<Func<T, bool>> predicate) { Predicate = predicate; }

        internal Expression<Func<T, bool>> Predicate { get; set; }
    }
}
