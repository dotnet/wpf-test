// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Pict.ObjectArrays
{
    using System;
    using System.Collections;

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    sealed class AllPossibleObjectArrayCollection : IObjectArrayCollection
    {
        readonly IEnumerable[] enumerables;

        public AllPossibleObjectArrayCollection(params IEnumerable[] enumerables)
        {
            this.enumerables = (IEnumerable[])enumerables.Clone();
        }

        public AllPossibleObjectArrayEnumerator GetEnumerator()
        {
            return new AllPossibleObjectArrayEnumerator(this.enumerables);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new AllPossibleObjectArrayEnumerator(this.enumerables);
        }

        IObjectArrayEnumerator IObjectArrayCollection.GetEnumerator()
        {
            return new AllPossibleObjectArrayEnumerator(this.enumerables);
        }
    }
}
