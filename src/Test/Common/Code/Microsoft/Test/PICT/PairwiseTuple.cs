// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Diagnostics;

    // really a read-only class once constructed
    sealed class PairwiseTuple : IComparable
    {
        static readonly CaseInsensitiveComparer comp = new CaseInsensitiveComparer(PictConstants.Culture);

        bool isNegative = false;

        int last = -1;

        readonly string[] names;

        static readonly PairwiseTupleComparer tupleComp = new PairwiseTupleComparer();

        readonly PairwiseValue[] values;

        public bool IsNegative
        {
            get
            {
                return this.isNegative;
            }
        }

        internal int Length
        {
            get { return this.names.Length; }
        }

        public PairwiseValue this[string parameterName]
        {
            get
            {
                for (int i = 0; i < names.Length; ++i)
                {
                    if (comp.Compare(parameterName, names[i]) == 0)
                    {
                        return values[i];
                    }
                }

                throw new IndexOutOfRangeException("Couldn't find " + parameterName);
            }
        }

        public PairwiseValue this[PairwiseParameter parameter]
        {
            get
            {
                return this[parameter.Name];
            }
        }

        // warning: slow
        public PairwiseValue this[Type t]
        {
            get
            {
                PairwiseValue match = null;

                foreach (PairwiseValue v in this.values)
                {
                    if (v.ValueType == t)
                    {
                        if (match != null)
                        {
                            throw new ArgumentException("Type " + t + " occurred multiple times");
                        }

                        match = v;
                    }
                }

                if (match != null)
                {
                    return match;
                }
                else
                {
                    throw new IndexOutOfRangeException(t.FullName + " not found");
                }
            }
        }

        internal PairwiseValue this[int i]
        {
            get
            {
                return this.values[i];
            }
        }

        internal PairwiseTuple(string[] originalNames)
        {
            this.values = new PairwiseValue[originalNames.Length];
            this.names = originalNames;
        }

        internal int Add(string name, PairwiseValue value)
        {
            Debug.Assert(value != null);
            Debug.Assert(name != null);
            ++last;
            Debug.Assert(names[last] == name);
            values[last] = value;
            isNegative = isNegative || value.IsNegative;
            return last;
        }

        int System.IComparable.CompareTo(object obj)
        {
            return tupleComp.Compare(this, obj as PairwiseTuple);
        }

        public override bool Equals(object obj)
        {
            return tupleComp.Compare(this, obj as PairwiseTuple) == 0;
        }

        public override int GetHashCode()
        {
            return tupleComp.GetHashCode(this);
        }

        public string ToShortString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < this.values.Length; ++i)
            {
                if (i != 0)
                {
                    sb.Append(", ");
                }

                sb.Append(values[i].Value);
            }

            return (this.IsNegative ? PictDefaults.DefaultNegativeValuePrefixString : "") + "Tuple(" + this.last + ": " + sb + ")";
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < this.values.Length; ++i)
            {
                if (i != 0)
                {
                    sb.Append(", ");
                }

                sb.Append(values[i].Value);
            }

            return (this.IsNegative ? Pict.PictDefaults.DefaultNegativeValuePrefixString : "") + "Tuple(" + sb.ToString() + ")";
        }
#region PairwiseTupleComparer
        // not strictly needed (and obsoleted in Whidbey): IHashCodeProvider
        sealed class PairwiseTupleComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                PairwiseTuple a = x as PairwiseTuple;
                PairwiseTuple b = y as PairwiseTuple;

                return Compare(a, b);
            }

            public int Compare(PairwiseTuple a, PairwiseTuple b)
            {
                if (a == null)
                {
                    return -1;
                }

                if (b == null)
                {
                    return 1;
                }

                if (a.Length < b.Length)
                {
                    return -1;
                }

                if (b.Length > a.Length)
                {
                    return 1;
                }

                for (int i = 0; i < a.Length; ++i)
                {
                    int c;

                    c = a.names[i].CompareTo(b.names[i]);
                    if (c != 0)
                    {
                        return c;
                    }

                    IComparable m = a[i].Value as IComparable;
                    IComparable n = b[i].Value as IComparable;

                    if (m == null)
                    {
                        return -1;
                    }

                    if (n == null)
                    {
                        return 1;
                    }

                    c = m.CompareTo(n);

                    if (c != 0)
                    {
                        return c;
                    }
                }

                // all value comparisons went okay
                return 0;
            }

            public int GetHashCode(object obj)
            {
                return GetHashCode(obj as PairwiseTuple) ;
            }

            public int GetHashCode(PairwiseTuple tuple)
            {
                if (tuple == null)
                {
                    return 1;
                }

                int hash = 1;

                for (int i = 0; i < tuple.Length; ++i)
                {
                    hash = hash ^ tuple.names[i].GetHashCode();
                    hash = hash ^ tuple.values[i].GetHashCode();
                }

                return hash;
            }
        }

        #endregion
    }
}
