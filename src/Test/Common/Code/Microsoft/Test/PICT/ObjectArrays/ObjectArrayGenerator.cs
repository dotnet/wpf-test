// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict.ObjectArrays
{
    using System;
    using System.Collections;

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    sealed class ObjectArrayGenerator
    {

        static readonly AllPossibleObjectArrayGenerationStrategy allStrategy = new AllPossibleObjectArrayGenerationStrategy();
        ObjectArrayGenerator()
        {
        }

        public static IObjectArrayCollection GenerateAllPossibleObjectArrays(params IEnumerable[] enumerables)
        {
            return allStrategy.Generate(enumerables);
        }
        
        public static 
            IObjectArrayCollection
            GeneratePairwiseObjectArrays(params IEnumerable[] enumerables)
        {
            return new PairwiseObjectArrayGenerationStrategy(2).Generate(enumerables);
        }

        public static IObjectArrayCollection GeneratePairwiseObjectArrays(int order, params IEnumerable[] enumerables)
        {
            return new PairwiseObjectArrayGenerationStrategy(order).Generate(enumerables);
        }
    }
}
