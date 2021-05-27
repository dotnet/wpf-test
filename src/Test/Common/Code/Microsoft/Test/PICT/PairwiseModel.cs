// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Pict
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.IO;
    using System.Collections;
    using System.Globalization;
    //

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    sealed class PairwiseModel : IPairwiseVisitable, IPairwiseComment
    {
        string comment;

        readonly PairwiseIfThenConstraintCollection ifThenConstraints = new PairwiseIfThenConstraintCollection();

        readonly PairwiseConditionCollection parameterConstraints = new PairwiseConditionCollection();

        readonly PairwiseParameterCollection parameters = new PairwiseParameterCollection();

        readonly PairwiseSubModelCollection submodels = new PairwiseSubModelCollection();

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

        public PairwiseIfThenConstraintCollection IfThenConstraints
        {
            get { return this.ifThenConstraints; }
        }

        // Constraint Terms (field-field)
        public PairwiseConditionCollection ParameterConstraints
        {
            get { return this.parameterConstraints; }
        }

        // list of parameters
        public PairwiseParameterCollection Parameters
        {
            get { return this.parameters; }
        }

        // list of childModels
        public PairwiseSubModelCollection SubModels
        {
            get { return submodels; }
        }

        public PairwiseModel()
        {
        }

        public object Accept(PairwiseVisitor visitor)
        {
            return visitor.Visit(this);
        }

        public void Add(PairwiseSubModel model)
        {
            this.submodels.Add(model);
        }

        public void Add(PairwiseParameter parameter)
        {
            this.parameters.Add(parameter);
        }

        public void Add(PairwiseParameter first, params PairwiseParameter[] others)
        {
            this.parameters.Add(first, others);
        }

        public void Add(PairwiseCondition parameterConstraint)
        {
            this.parameterConstraints.Add(parameterConstraint);
        }

        public void Add(PairwiseIfThenConstraint constraint)
        {
            this.ifThenConstraints.Add(constraint);
        }

        internal PairwiseModelLookupContext CreateLookupContext(PairwiseSettings settings)
        {
            return new PairwiseModelLookupContext(this, settings);
        }

        public PairwiseTuple[] GenerateTuples(int order)
        {
            return GenerateTuples(new PairwiseSettings(order, true));
        }

        public PairwiseTuple[] GenerateTuples(PairwiseSettings settings)
        {
            return new PictPairwiseVisitor(this, settings).Generate();
        }

        public string ToPictModelString()
        {
            return (string)new PictPairwiseVisitor(this, PairwiseSettings.DefaultSettings).Visit(this);
        }

        public void WritePictFileTo(string pictFileName)
        {
            using (TextWriter tw = new StreamWriter(pictFileName))
            {
                WritePictFileTo(tw);
            }
        }

        public void WritePictFileTo(TextWriter output)
        {
            string results = ToPictModelString();

            output.WriteLine(results);
        }
    }

    // used to look up variables
    sealed class PairwiseModelLookupContext
    {
        Hashtable[] lookupTables = null;

        readonly Hashtable objectLookup = new Hashtable();

        readonly PairwiseParameterCollection parameters;

        readonly PairwiseSettings settings;

        public PairwiseSettings Settings
        {
            get { return this.settings; }
        }

        public PairwiseModelLookupContext(PairwiseModel model, PairwiseSettings settings)
        {
            this.parameters = new PairwiseParameterCollection(model.Parameters);
            this.settings = settings;
        }

        void BuildLookupTable()
        {
            // initialize
            lookupTables = new Hashtable[this.parameters.Count];
            for (int i = 0; i < this.parameters.Count; ++i)
            {
                PairwiseParameter pp = this.parameters[i];
                PairwiseValueCollection values = pp.Values;
                Hashtable h = new Hashtable(values.Count);

                foreach (PairwiseValue pv in values)
                {
                    string key = LookupInputValue(pv);

                    h[key] = pv;
                    if (pv.Aliases.Count != 0)
                    {
                        foreach (PairwiseValue a in pv.Aliases)
                        {
                            h[LookupInputValue(a)] = a;
                        }
                    }
                }

                lookupTables[i] = h;
            }
        }

        public PairwiseValue FindValue(int parameterPosition, string value)
        {
            if (lookupTables == null)
            {
                // just build once
                BuildLookupTable();
            }

            object val = lookupTables[parameterPosition][value];

            if (val != null)
            {
                return (PairwiseValue)val;
            }
            else
            {
                throw new PairwiseException("Couldn't find value " + value + " in parameter " + parameters[parameterPosition].Name);
            }
        }

        public string LookupInputValue(PairwiseValue valuex)
        {
            return LookupWork(valuex, false, valuex.Weight);
        }

        public string LookupName(PairwiseParameter parameter)
        {
            return parameter.Name;
        }

        public string LookupOutputValue(PairwiseValue valuex)
        {
            return LookupWork(valuex, true, valuex.Weight);
        }

        string LookupOutputValueWithoutWeight(PairwiseValue valuex)
        {
            return LookupWork(valuex, false, valuex.Weight);
        }

        string LookupOutputValueWithWeight(PairwiseValue valuex, int weight)
        {
            return LookupWork(valuex, true, weight);
        }

        // NOTE: for PICT 3.0.34.0, the negative aliases are double-~'d, so we don't put them when we build the alias list
        public string LookupValuesAndAliases(PairwiseValue value)
        {
            if (value.Aliases.Count == 0)
            {
                return LookupOutputValue(value);
            }

            StringBuilder sb = new StringBuilder(100 + (20 * value.Aliases.Count));

            sb.Append(LookupOutputValueWithoutWeight(value));
            for (int i = 0, nminus1 = value.Aliases.Count - 1; i < value.Aliases.Count; ++i)
            {
                PairwiseValue alias = value.Aliases[i];

                sb.Append(settings.AliasDelimiter);

                string q;

                if (i == nminus1)
                {
                    // get the weight from the first value, not the second!
                    q = LookupOutputValueWithWeight(alias, value.Weight);
                }
                else
                {
                    q = LookupOutputValueWithoutWeight(alias);
                }

                if (q.StartsWith(settings.NegativePrefixString))
                {
                    // q = q.Substring(PictConstants.NegativeValuePrefixString.Length);
                    sb.Append(q, settings.NegativePrefixString.Length, q.Length - settings.NegativePrefixString.Length);
                }
                else
                {
                    sb.Append(q);
                }
            }

            return sb.ToString();
        }

        string LookupWork(PairwiseValue valuex, bool includeWeight, int weight)
        {
            string v;
            object vv = valuex.Value;

            switch (valuex.PairwiseValueType)
            {
                case PairwiseValueType.Enum:
                    v = PictConstants.AggressivelyEscape(vv.ToString().Replace(',', ';'));
                    break;

                case PairwiseValueType.String:
                    // 
                    v = PictConstants.AggressivelyEscape(vv.ToString());
                    break;

                case PairwiseValueType.Number:
                    // NOTE: here we fixed the german issue
                    if (valuex.ValueType == typeof(Char))
                    {
                        v = PictConstants.AggressivelyEscape(string.Format(PictConstants.Culture.NumberFormat, "{0}", vv));
                    }
                    else
                    {
                        v = string.Format(PictConstants.Culture.NumberFormat, "{0}", vv);
                    }
                    break;

                case PairwiseValueType.ComplexObject:
                    if (vv == null)
                    {
                        v = PictConstants.NullObjectString;
                    }
                    else
                    {
                        if (!objectLookup.ContainsKey(vv))
                        {
                            // use the hex representation of the count
                            v = string.Format(PictConstants.Culture, "o_{0:X}", objectLookup.Count);
                            objectLookup[vv] = v;
                        }
                        else
                        {
                            v = ((string)objectLookup[vv]);
                        }
                    }

                    break;

                default:
                    throw new NotSupportedException("PairwiseValueType = " + valuex.PairwiseValueType);
            }
            if (settings.ContainsSpecialCharacters(v))
            {
                throw new PairwiseException("Invalid characters in name: " + v);
            }

            if (valuex.IsNegative)
            {
                v = settings.NegativePrefix + v;
            }

            if (includeWeight && weight != PictConstants.DefaultWeight)
            {
                v = v + "(" + weight + ")";
            }

            return v;
        }
    }
}
