// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;
    using System.Xml.Serialization;
    using System.Collections;
    //

    // PairwiseSubModelCollection
    sealed class PairwiseSubModelCollection : CollectionBase
    {

        public PairwiseSubModel this[int index]
        {
            get
            {
                return (PairwiseSubModel)InnerList[index];
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                InnerList[index] = value;
            }
        }
        public PairwiseSubModelCollection() : base()
        {
        }

        public int Add(PairwiseSubModel childModel)
        {
            if (childModel == null)
            {
                throw new ArgumentNullException("childModel");
            }

            return InnerList.Add(childModel);
        }

        public bool Contains(PairwiseSubModel childModel)
        {
            if (childModel == null)
            {
                throw new ArgumentNullException("childModel");
            }

            return InnerList.Contains(childModel);
        }

        public void CopyTo(PairwiseSubModel[] array, int index)
        {
            InnerList.CopyTo(array, index);
        }

        public int IndexOf(PairwiseSubModel childModel)
        {
            if (childModel == null)
            {
                throw new ArgumentNullException("childModel");
            }

            return InnerList.IndexOf(childModel);
        }

        public void Insert(int index, PairwiseSubModel childModel)
        {
            if (childModel == null)
            {
                throw new ArgumentNullException("childModel");
            }

            InnerList.Insert(index, childModel);
        }

        protected override void OnValidate(object value)
        {
            base.OnValidate(value);

            PairwiseSubModel childModel = value as PairwiseSubModel;

            if (childModel == null)
            {
                throw new ArgumentException("PairwiseSubModel");
            }
        }

        public void Remove(PairwiseSubModel childModel)
        {
            if (childModel == null)
            {
                throw new ArgumentNullException("childModel");
            }

            InnerList.Remove(childModel);
        }
    }
    //

}
