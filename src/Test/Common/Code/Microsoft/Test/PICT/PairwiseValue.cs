// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;
    using System.Collections;
    using System.Xml.Serialization;

    // immutable?
    sealed class PairwiseValue
    {
        // this is readonly, but is modifable
        readonly PairwiseAliasCollection aliases;

        readonly bool isNegative;

        readonly PairwiseValueType optType;

        string str;

        readonly object value;

        readonly Type valueType;

        readonly int weight;

        #region FromString and implicit operators

#endregion

        public PairwiseAliasCollection Aliases
        {
            get { return aliases; }
        }

        public bool IsNegative
        {
            get { return this.isNegative; }
        }

        internal PairwiseValueType PairwiseValueType
        {
            get
            {
                return optType;
            }
        }

        public object Value
        {
            get
            {
                return this.value;
            }
        }

        public Type ValueType
        {
            get
            {
                return valueType;
            }
        }

        public int Weight
        {
            get { return this.weight; }
        }
#region FromString and implicit operators

        public static implicit operator PairwiseValue(string s)
        {
            return FromString(s);
        }

        public static implicit operator PairwiseValue(Enum e)
        {
            return new PairwiseValue(e, false, PairwiseValueType.Enum);
        }

        public static implicit operator PairwiseValue(Guid g)
        {
            return new PairwiseValue(g, false, PairwiseValueType.Enum);
        }

        public static implicit operator PairwiseValue(bool b)
        {
            return new PairwiseValue(b, false, PairwiseValueType.Enum);
        }

        public static implicit operator PairwiseValue(int i)
        {
            return new PairwiseValue(i, false, PairwiseValueType.Number);
        }

        public static implicit operator PairwiseValue(char c)
        {
            return new PairwiseValue(c, false);
        }

        public static implicit operator PairwiseValue(long l)
        {
            return new PairwiseValue(l, false, PairwiseValueType.Number);
        }

        #endregion

        public PairwiseValue(object value): this(value, false)
        {
        }

        public PairwiseValue(object value, int weight):
            this(value, weight, false)
        {
        }

        public PairwiseValue(object value, bool isNegative):
            this(value, isNegative, GetOptionType(value))
        {
        }

        public PairwiseValue(object value, int weight, bool isNegative):
            this(value, weight, isNegative, GetOptionType(value))
        {
        }

        PairwiseValue(object value, bool isNegative, PairwiseValueType valueType):
            this(value, PictConstants.DefaultWeight, isNegative, valueType)
        {
        }

        PairwiseValue(object value, int weight, bool isNegative, PairwiseValueType valueType)
        {
            this.optType = valueType;
            this.weight = weight;
            if (weight < 1)
            {
                throw new ArgumentOutOfRangeException("weight", "Weight cannot be less than 1");
            }

            if (value is PairwiseValue)
            {
                throw new ArgumentException("Can't have a nested PairwiseValue");
            }

            this.value = value;
            this.isNegative = isNegative;
            this.aliases = new PairwiseAliasCollection(this);
            if (this.value == null)
            {
                this.valueType = typeof(object);
            }
            else
            {
                this.valueType = this.value.GetType();
            }
        }

        public static PairwiseValue CreateNegativeOption(object value)
        {
            return new PairwiseValue(value, true);
        }

        public static PairwiseValue CreateNegativeOption(PairwiseValue value)
        {
            if (value.IsNegative)
            {
                throw new ArgumentException(value + " already negative");
            }

            return CreateNegativeOption(value.value);
        }

        public override bool Equals(object obj)
        {
            PairwiseValue opt = obj as PairwiseValue;

            if (opt == null)
            {
                return false; // throw?
            }

            if (opt == this)
            {
                return true;
            }

            // note: considering negative but not weights or aliases
            return (opt.isNegative == this.isNegative) && //
                (opt.ValueType == this.ValueType) && // 
                object.Equals(opt.value, this.value);
        }

        string ExpandOptionType()
        {
            if (this.PairwiseValueType == PairwiseValueType.ComplexObject || this.PairwiseValueType == PairwiseValueType.Enum)
            {
                return this.value.GetType().FullName;
            }
            else
            {
                return this.ValueType.ToString();
            }
        }
#region FromString and implicit operators

        public static PairwiseValue FromBoolean(bool b)
        {
            return new PairwiseValue(b, false, PairwiseValueType.Enum);
        }

        public static PairwiseValue FromBoolean(bool b, bool isNegative)
        {
            return new PairwiseValue(b, isNegative, PairwiseValueType.Enum);
        }

        public static PairwiseValue FromDouble(double d)
        {
            return new PairwiseValue(d, false, PairwiseValueType.Number);
        }

        public static PairwiseValue FromDouble(double d, bool isNegative)
        {
            return new PairwiseValue(d, isNegative, PairwiseValueType.Number);
        }

        public static PairwiseValue FromEnum(Enum e)
        {
            return new PairwiseValue(e, false, PairwiseValueType.Enum);
        }

        public static PairwiseValue FromEnum(Enum e, bool isNegative)
        {
            return new PairwiseValue(e, isNegative, PairwiseValueType.Enum);
        }

        public static PairwiseValue FromGuid(Guid g)
        {
            return new PairwiseValue(g, false, PairwiseValueType.Enum);
        }

        public static PairwiseValue FromGuid(Guid g, bool isNegative)
        {
            return new PairwiseValue(g, isNegative, PairwiseValueType.Enum);
        }

        public static PairwiseValue FromInt32(int i)
        {
            return new PairwiseValue(i, false, PairwiseValueType.Number);
        }

        public static PairwiseValue FromInt32(int i, bool isNegative)
        {
            return new PairwiseValue(i, isNegative, PairwiseValueType.Number);
        }

        public static PairwiseValue FromInt64(long l)
        {
            return new PairwiseValue(l, false, PairwiseValueType.Number);
        }

        public static PairwiseValue FromInt64(long l, bool isNegative)
        {
            return new PairwiseValue(l, isNegative, PairwiseValueType.Number);
        }

        public static PairwiseValue FromString(string s)
        {
            return new PairwiseValue(s, false, PairwiseValueType.String);
        }

        public static PairwiseValue FromString(string s, bool isNegative)
        {
            return new PairwiseValue(s, isNegative, PairwiseValueType.String);
        }

#endregion

        public override int GetHashCode()
        {
            int hash = this.isNegative.GetHashCode();

            hash = hash ^ this.ValueType.AssemblyQualifiedName.GetHashCode();
            hash = hash ^ (this.value == null ? 1 : this.value.GetHashCode());

            // note: not considering aliases or weights [just like Equals()]
            return hash;
        }

        static PairwiseValueType GetOptionType(object value)
        {
            if (value == null)
            {
                return PairwiseValueType.ComplexObject;
            }

            if (value is string)
            {
                return PairwiseValueType.String;
            }

            Type t = value.GetType();

            if (t.IsEnum)
            {
                return PairwiseValueType.Enum;
            }

            if (Array.IndexOf(PictConstants.NumericTypes, t) != -1)
            {
                return PairwiseValueType.Number;
            }

            if (Array.IndexOf(PictConstants.TreatLikeEnum, t) != -1)
            {
                return PairwiseValueType.Enum;
            }

            return PairwiseValueType.ComplexObject;
        }

        public bool IsSameValue(PairwiseValue value)
        {
            return value.optType == this.optType && object.Equals(this.value, value.value);
        }

        public override string ToString()
        {
            if (str == null)
            {
                if (isNegative)
                {
                    str = string.Format("PairwiseValue({0}, Value={1}, IsNegative={2})", ExpandOptionType(), this.value == null ? "null" : value, this.IsNegative);
                }
                else
                {
                    str = string.Format("PairwiseValue({0}, Value={1})", ExpandOptionType(), this.value == null ? "null" : value);
                }
            }

            return str;
        }
    }
}
