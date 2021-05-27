// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;
    using System.Xml.Serialization;
    using System.Collections;

    // PairwiseAliasCollection
    sealed class PairwiseAliasCollection : CollectionBase
    {
        readonly PairwiseValue parent;

        public PairwiseValue this[int index]
        {
            get
            {
                return (PairwiseValue)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        // only createable w/i assembly
        internal PairwiseAliasCollection(PairwiseValue parent)
        {
            this.parent = parent;
        }

        public int Add(PairwiseValue value)
        {
            return List.Add(value);
        }

        public bool Contains(PairwiseValue value)
        {
            return List.Contains(value);
        }

        public void CopyTo(PairwiseValue[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public int IndexOf(PairwiseValue value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, PairwiseValue value)
        {
            List.Insert(index, value);
        }

        protected override void OnValidate(object val)
        {
            base.OnValidate(val);
            if (val == null)
            {
                throw new ArgumentNullException("value");
            }

            PairwiseValue value = val as PairwiseValue;

            if (value == null)
            {
                throw new ArgumentException("Not a PairwiseValue, was a " + value.GetType().FullName);
            }

            if (value.ValueType != parent.ValueType)
            {
                throw new ArgumentException("Was a " + value.ValueType + "; expected " + parent.ValueType);
            }

            if (value.IsNegative != parent.IsNegative)
            {
                throw new ArgumentException(value + " IsNegative = " + value.IsNegative + " but expected " + parent.IsNegative);
            }

            // TBD: is this allowable?
            if (value.Aliases.Count != 0)
            {
                throw new ArgumentException("Attempted to do nested aliases: " + value.Aliases.Count + " present");
            }

            if (this.Contains(value))
            {
                throw new ArgumentException("Already contains " + value);
            }

            if (value.PairwiseValueType == PairwiseValueType.ComplexObject)
            {
                throw new NotSupportedException("Complex type comparison not yet supported");
            }
        }

        public void Remove(PairwiseValue value)
        {
            List.Remove(value);
        }
    }
    //

}
