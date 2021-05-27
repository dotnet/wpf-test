// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;
    using System.Xml.Serialization;
    using System.Collections;
    //

    // PairwiseParameterConstraintCollection
    [Obsolete("No longer used: try PairwiseConditionCollection")]
    sealed class PairwiseParameterConstraintCollection : CollectionBase
    {

        public PairwiseParameterConstraint this[int index]
        {
            get
            {
                return (PairwiseParameterConstraint)InnerList[index];
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
        public PairwiseParameterConstraintCollection() : base()
        {
        }

        public int Add(PairwiseParameterConstraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }

            return InnerList.Add(constraint);
        }

        public bool Contains(PairwiseParameterConstraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }

            return InnerList.Contains(constraint);
        }

        public void CopyTo(PairwiseParameterConstraint[] array, int index)
        {
            InnerList.CopyTo(array, index);
        }

        public int IndexOf(PairwiseParameterConstraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }

            return InnerList.IndexOf(constraint);
        }

        public void Insert(int index, PairwiseParameterConstraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }

            InnerList.Insert(index, constraint);
        }

        protected override void OnValidate(object value)
        {
            base.OnValidate(value);

            PairwiseParameterConstraint constraint = value as PairwiseParameterConstraint;

            if (constraint == null)
            {
                throw new ArgumentException("PairwiseParameterConstraint");
            }
        }

        public void Remove(PairwiseParameterConstraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }

            InnerList.Remove(constraint);
        }
    }
    //

}
