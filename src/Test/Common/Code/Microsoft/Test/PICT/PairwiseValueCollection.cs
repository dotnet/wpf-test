// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;
    using System.Xml.Serialization;
    using System.Collections;
    //

    // PairwiseValueCollection
    sealed class PairwiseValueCollection : CollectionBase
    {

        public PairwiseValue this[int index]
        {
            get
            {
                return (PairwiseValue)InnerList[index];
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
        public PairwiseValueCollection() : base()
        {
        }

        public PairwiseValueCollection(params PairwiseValue[] values)
        {
            foreach (PairwiseValue o in values)
            {
                this.Add(o);
            }
        }

        public PairwiseValueCollection(PairwiseValueCollection c)
        {
            this.InnerList.AddRange(c.InnerList);
        }

        public int Add(PairwiseValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return InnerList.Add(value);
        }

        public bool Contains(PairwiseValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return InnerList.Contains(value);
        }

        public void CopyTo(PairwiseValue[] array, int index)
        {
            InnerList.CopyTo(array, index);
        }

        public static PairwiseValueCollection CreateFromEnumType(Type t)
        {
            if (!t.IsEnum)
            {
                throw new ArgumentException(t.FullName + " isn't an enum!");
            }

            PairwiseValueCollection coll = new PairwiseValueCollection();

            foreach (Enum e in Enum.GetValues(t))
            {
                coll.Add(new PairwiseValue(e));
            }

            return coll;
        }

        public static PairwiseValueCollection CreateFromObjects(params object[] values)
        {
            PairwiseValueCollection coll = new PairwiseValueCollection();

            foreach (object o in values)
            {
                coll.Add(new PairwiseValue(o));
            }

            return coll;
        }

        [Obsolete("Does not work with aliases")]
        public int IndexOf(PairwiseValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return InnerList.IndexOf(value);
        }

        public void Insert(int index, PairwiseValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            InnerList.Insert(index, value);
        }

        protected override void OnValidate(object value)
        {
            base.OnValidate(value);

            PairwiseValue pvalue = value as PairwiseValue;

            if (pvalue == null)
            {
                throw new ArgumentException("PairwiseValue");
            }
        }

        public void Remove(PairwiseValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            InnerList.Remove(value);
        }
    }
    //

}
