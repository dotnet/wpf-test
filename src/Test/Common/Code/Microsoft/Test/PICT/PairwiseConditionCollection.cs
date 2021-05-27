// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;
    using System.Xml.Serialization;
    using System.Collections;
    //


    // PairwiseConditionCollection
     sealed class PairwiseConditionCollection : CollectionBase
    {

        public PairwiseCondition this[int index]
        {
            get
            {
                return (PairwiseCondition)InnerList[index];
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
        public PairwiseConditionCollection() : base()
        {
        }

        public int Add(PairwiseCondition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }

            return InnerList.Add(condition);
        }

        public bool Contains(PairwiseCondition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }

            return InnerList.Contains(condition);
        }

        public void CopyTo(PairwiseCondition[] array, int index)
        {
            InnerList.CopyTo(array, index);
        }

        public int IndexOf(PairwiseCondition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }

            return InnerList.IndexOf(condition);
        }

        public void Insert(int index, PairwiseCondition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }

            InnerList.Insert(index, condition);
        }

        protected override void OnValidate(object value)
        {
            base.OnValidate(value);

            PairwiseCondition condition = value as PairwiseCondition;

            if (condition == null)
            {
                throw new ArgumentException("PairwiseCondition");
            }
        }

        public void Remove(PairwiseCondition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }

            InnerList.Remove(condition);
        }
    }
   //

}
