// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Pict.ObjectArrays
{
    using System;
    using System.Collections;

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    sealed class AllPossibleObjectArrayGenerationStrategy : IObjectArrayGenerationStrategy
    {
        public AllPossibleObjectArrayCollection Generate(params IEnumerable[] enumerables)
        {
            return new AllPossibleObjectArrayCollection(enumerables);
        }

        IObjectArrayCollection IObjectArrayGenerationStrategy.Generate(params IEnumerable[] enumerables)
        {
            return new AllPossibleObjectArrayCollection(enumerables);
        }
    }
}
