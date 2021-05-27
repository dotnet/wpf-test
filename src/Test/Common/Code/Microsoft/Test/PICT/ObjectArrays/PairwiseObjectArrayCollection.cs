// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Pict.ObjectArrays
{
    using System;
    using System.Collections;

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    sealed class PairwiseObjectArrayCollection : IObjectArrayCollection
    {
        readonly IList[] lists;

        readonly PairwiseModel model;

        readonly PairwiseSettings settings;

        public PairwiseObjectArrayCollection(params IEnumerable[] enumerables): this(2, enumerables)
        {
        }

        public PairwiseObjectArrayCollection(int order, params IEnumerable[] enumerables)
        {
            this.settings = new PairwiseSettings(order, true);
            this.lists = new IList[enumerables.Length];

            // build the big lookup table
            this.model = new PairwiseModel();
            for (int i = 0; i < enumerables.Length; ++i)
            {
                // the enumerator from here is disposed (if necessary) in the below foreach loop
                IEnumerable origList = enumerables[i];
                bool copy;
                IList temp = origList as IList;

                if (temp != null)
                {
                    copy = false;
                }
                else
                {
                    temp = new ArrayList();
                    copy = true;
                }

                int index = 0;
                PairwiseParameter param = new PairwiseParameter("P" + i.ToString(PictConstants.Culture));

                foreach (object item in origList)
                {
                    if (copy)
                    {
                        temp.Add(item);
                    }

                    param.AddValue(index);
                    ++index;
                }

                this.lists[i] = temp;
                model.Parameters.Add(param);
            }
        }

        PairwiseObjectArrayEnumerator GetEnum()
        {
            return new PairwiseObjectArrayEnumerator(this.settings, this.model, this.lists);
        }

        public PairwiseObjectArrayEnumerator GetEnumerator()
        {
            return GetEnum();
        }

        IObjectArrayEnumerator IObjectArrayCollection.GetEnumerator()
        {
            return GetEnum();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnum();
        }
    }
}

