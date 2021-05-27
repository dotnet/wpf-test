// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;
    using System.Xml.Serialization;
    using System.Collections;
    //
    
    // PairwiseIfThenConstraintCollection
    sealed class PairwiseIfThenConstraintCollection : CollectionBase
    {

        public PairwiseIfThenConstraint this[int index]
        {
            get
            {
                return (PairwiseIfThenConstraint)InnerList[index];
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
        public PairwiseIfThenConstraintCollection() : base()
        {
        }

        public void Add(PairwiseIfThenConstraintCollection collection)
        {
            this.InnerList.AddRange(collection.InnerList);
        }

        public int Add(PairwiseIfThenConstraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }

            return InnerList.Add(constraint);
        }

        public int Add(PairwiseCondition ifPart, PairwiseCondition thenPart)
        {
            return Add(new PairwiseIfThenConstraint(ifPart, thenPart));
        }

        public int Add(PairwiseCondition ifPart, PairwiseCondition thenPart, PairwiseCondition elsePart)
        {
            return Add(new PairwiseIfThenConstraint(ifPart, thenPart, elsePart));
        }

        public bool Contains(PairwiseIfThenConstraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }

            return InnerList.Contains(constraint);
        }

        public void CopyTo(PairwiseIfThenConstraint[] array, int index)
        {
            InnerList.CopyTo(array, index);
        }

        public int IndexOf(PairwiseIfThenConstraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }

            return InnerList.IndexOf(constraint);
        }

        public void Insert(int index, PairwiseIfThenConstraint constraint)
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

            PairwiseIfThenConstraint constraint = value as PairwiseIfThenConstraint;

            if (constraint == null)
            {
                throw new ArgumentException("PairwiseIfThenConstraint");
            }
        }

        public void Remove(PairwiseIfThenConstraint constraint)
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
