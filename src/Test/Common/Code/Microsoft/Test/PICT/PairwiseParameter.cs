// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Pict
{
    using System;

    sealed class PairwiseParameter : IPairwiseComment
    {
        string comment;

        PairwiseValueType firstType;

        readonly string name;

        bool setFirstType = false;

        readonly PairwiseValueCollection valueCollection = new PairwiseValueCollection();

        public string Comment
        {
            get
            {
                return this.comment;
            }
            set
            {
                this.comment = value;
            }
        }

        public string Name
        {
            get { return this.name; }
        }

        internal PairwiseValueType PairwiseValueType
        {
            get
            {
                if (!setFirstType)
                {
                    throw new InvalidOperationException("No elements have been added to the parameter yet");
                }

                return firstType;
            }
        }

        internal PairwiseValueCollection Values
        {
            get
            {
                return this.valueCollection;
            }
        }

        public PairwiseParameter(string name, params PairwiseValue[] values):
            this(name, new PairwiseValueCollection(values))
        {
        }

        public PairwiseParameter(string name, Type enumType): this(name)
        {
            foreach (Enum e in Enum.GetValues(enumType))
            {
                this.AddValue(e);
            }
        }

        public PairwiseParameter(string name, PairwiseValueCollection values)
        {
            this.name = name;
            foreach (PairwiseValue o in values)
            {
                this.AddValue(o);
            }
        }

        public PairwiseParameter(string name, string[] values)
        {
            this.name = name;
            foreach (string s in values)
            {
                this.AddValue(PairwiseValue.FromString(s));
            }
        }

        // Rule support
        public PairwiseValue AddValue(PairwiseValue value)
        {
            if (!setFirstType)
            {
                firstType = value.PairwiseValueType;
                setFirstType = true;
            }

            if (value.PairwiseValueType != firstType)
            {
                throw new PairwiseException(string.Format("Type mismatch: {0} != {1}", this.firstType, value.ValueType, firstType));
            }

            valueCollection.Add(value);
            return value;
        }

        public PairwiseEqualTerm Equal(PairwiseParameter parameter)
        {
            // this constructor throws if not correct
            return new PairwiseEqualTerm(this, parameter);
        }

        public PairwiseEqualTerm Equal(PairwiseValue value)
        {
            return new PairwiseEqualTerm(this, value);
        }

        /// <summary>
        /// returns a copy of the values
        /// </summary>
        public PairwiseValueCollection GetValues()
        {
            return new PairwiseValueCollection(this.valueCollection);
        }

        public PairwiseGreaterThanTerm GreaterThan(PairwiseParameter parameter)
        {
            return new PairwiseGreaterThanTerm(this, parameter);
        }

        public PairwiseGreaterThanTerm GreaterThan(PairwiseValue value)
        {
            return new PairwiseGreaterThanTerm(this, value);
        }

        public PairwiseGreaterThanOrEqualTerm GreaterThanOrEqual(PairwiseParameter parameter)
        {
            return new PairwiseGreaterThanOrEqualTerm(this, parameter);
        }

        public PairwiseGreaterThanOrEqualTerm GreaterThanOrEqual(PairwiseValue value)
        {
            return new PairwiseGreaterThanOrEqualTerm(this, value);
        }

        public PairwiseInTerm In(params PairwiseValue[] values)
        {
            return In(new PairwiseValueCollection(values));
        }

        public PairwiseInTerm In(PairwiseValueCollection values)
        {
            if (values.Count == 0)
            {
                throw new ArgumentException("empty values");
            }

            return new PairwiseInTerm(this, values);
        }

        internal bool IsCompatibleWith(PairwiseComparison operation)
        {
            bool isrelative = !(operation == PairwiseComparison.Equal || operation == PairwiseComparison.NotEqual);
            PairwiseValueType t = this.PairwiseValueType;

            if (isrelative)
            {
                if (t == PairwiseValueType.Enum || t == PairwiseValueType.ComplexObject)
                {
                    // throw new NotSupportedException("Not supported: " + operation + " on " + t + "; might be possible, though!");
                    return false;
                }
            }

            return true;
        }

        internal bool IsCompatibleWith(PairwiseParameter parameter)
        {
            return this.PairwiseValueType == parameter.PairwiseValueType;
        }

        internal bool IsCompatibleWith(PairwiseValue value)
        {
            if (value == null)
            {
                return false;
            }

            if (!setFirstType)
            {
                return true;
            }

            return this.PairwiseValueType == value.PairwiseValueType;
        }

        public PairwiseLessThanTerm LessThan(PairwiseParameter parameter)
        {
            return new PairwiseLessThanTerm(this, parameter);
        }

        public PairwiseLessThanTerm LessThan(PairwiseValue value)
        {
            return new PairwiseLessThanTerm(this, value);
        }

        public PairwiseLessThanOrEqualTerm LessThanOrEqual(PairwiseParameter parameter)
        {
            return new PairwiseLessThanOrEqualTerm(this, parameter);
        }

        public PairwiseLessThanOrEqualTerm LessThanOrEqual(PairwiseValue value)
        {
            return new PairwiseLessThanOrEqualTerm(this, value);
        }

        public PairwiseLikeTerm Like(string wildcardPattern)
        {
            return new PairwiseLikeTerm(this, wildcardPattern);
        }

        public PairwiseNotEqualTerm NotEqual(PairwiseParameter parameter)
        {
            return new PairwiseNotEqualTerm(this, parameter);
        }

        public PairwiseNotEqualTerm NotEqual(PairwiseValue value)
        {
            return new PairwiseNotEqualTerm(this, value);
        }

        public PairwiseNotInTerm NotIn(params PairwiseValue[] values)
        {
            return new PairwiseNotInTerm(this, new PairwiseValueCollection(values));
        }

        public void RemoveValue(PairwiseValue value)
        {
            valueCollection.Remove(value);
        }

        public static PairwiseParameter CreateFromEnum(string name, Type enumType)
        {
            return new PairwiseParameter(name, PairwiseValueCollection.CreateFromEnumType(enumType));

        }

        // no operators (ie, <, >, <= ,etc.) defined: too confusing
    }
}
