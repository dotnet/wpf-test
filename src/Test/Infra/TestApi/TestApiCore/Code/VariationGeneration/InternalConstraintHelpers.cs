// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

﻿
using System.Diagnostics;

namespace Microsoft.Test.VariationGeneration
{
    /// <summary>
    /// Helper function for determining relations between internal constraint tables.
    /// </summary>
    internal static class InternalConstraintHelpers
    {
        // helper to implement Constraint.SatisfiesConstraint
        internal static ConstraintSatisfaction SatisfiesContraint<T>(Model<T> model, ValueCombination combination, ParameterInteraction interaction) where T : new()
        {
            Debug.Assert(model != null && combination != null && interaction != null);

            var parameterMap = combination.ParameterToValueMap;
            for(int i = 0; i < interaction.Parameters.Count; i++)
            {
                if (!parameterMap.ContainsKey(interaction.Parameters[i]))
                {
                    return ConstraintSatisfaction.InsufficientData;
                }
            }

            for(int i = 0; i < interaction.Combinations.Count; i++)
            {
                if (ParameterInteractionTable<T>.MatchCombination(interaction.Combinations[i], combination))
                {
                    if (interaction.Combinations[i].State == ValueCombinationState.Excluded)
                    {
                        return ConstraintSatisfaction.Unsatisfied;
                    }
                }
            }

            return ConstraintSatisfaction.Satisfied;
        }        
    }
}
