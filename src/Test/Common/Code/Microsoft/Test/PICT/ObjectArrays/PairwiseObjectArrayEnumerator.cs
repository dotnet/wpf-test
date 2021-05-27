// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Pict.ObjectArrays
{
    using System;
    using System.Collections;

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    struct PairwiseObjectArrayEnumerator : IObjectArrayEnumerator
    {
        PairwiseTuple[] allTuples;

        object[] cur;

        int current;

        readonly IList[] lists;

        readonly PairwiseModel model;

        bool rebuildRequired;

        readonly PairwiseSettings settings;

        readonly int tupleLen;

        public PairwiseObjectArrayEnumerator(PairwiseSettings settings, PairwiseModel model, IList[] lists)
        {
            this.settings = settings;
            this.model = model;
            this.tupleLen = model.Parameters.Count;
            this.lists = (IList[])lists.Clone();
            current = -1;
            rebuildRequired = true;
            allTuples = null;

            // readonly cur 
            this.cur = null;
            this.Reset();
        }

        public object[] Current
        {
            get
            {
                return GetCurrent();
            }
        }

        object[] GetCurrent()
        {
            if (rebuildRequired)
            {
                PairwiseTuple tup = allTuples[current];

                cur = new object[tupleLen];
                for (int p = 0; p < cur.Length; ++p)
                {
                    cur[p] = lists[p][(int)tup[p].Value];
                }

                rebuildRequired = false;
            }

            return cur;
        }

        public bool MoveNext()
        {
            rebuildRequired = true;
            return ++current < allTuples.Length;
        }

        public void Reset()
        {
            this.allTuples = model.GenerateTuples(settings);
            this.current = -1;
            this.rebuildRequired = true;
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return GetCurrent();
            }
        }
    }
}
