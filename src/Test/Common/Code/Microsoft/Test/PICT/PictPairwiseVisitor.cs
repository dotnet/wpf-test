// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    #region using;
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Security.Cryptography;
    using System.Collections;
    using System.Collections.Specialized;
    using System.IO;
    using System.Text;
    #endregion


    // cannot be called more than once!
    sealed class PictPairwiseVisitor : PairwiseVisitor
    {

        readonly PairwiseModelLookupContext context;
        readonly PairwiseModel model;

        readonly PairwiseSettings settings;

        public PictPairwiseVisitor(PairwiseModel model, PairwiseSettings settings)
        {
            this.model = model;
            this.settings = settings;
            this.context = model.CreateLookupContext(settings);
        }

        static string Bracketed(string s)
        {
            return "[" + s + "]";
        }

        public PairwiseTuple[] Generate()
        {
            PictExecutionInformation last;

            return Generate(out last);
        }

        public PairwiseTuple[] Generate(out PictExecutionInformation executionInformation)
        {
            string[][] results;
            string pictModel = (string)this.Visit(model);

            // PictConstants.Trace("Pict model: {0}", model.Comment);
            string fn = settings.CanCache ? PictConstants.CacheFileName : null;
            using (PictRunner exec = new PictRunner(fn))
            {
                results = exec.MaybeExecutePict(settings, pictModel);
                executionInformation = exec.LastExecutionInformation;
                if (settings.RandomizeGeneration)
                {
                    settings.RandomSeed = executionInformation.RandomSeed;
                    // NOTE: not setting settings.RandomSeedSpecified
                }
            }

            // PictConstants.Trace("Results of executing PICT: {0}", results.Substring(0, Math.Min(255, results.Length)));
            int tupleLen = results[0].Length;
            PairwiseTuple[] tuples = new PairwiseTuple[results.Length - 1]; // skip the header
            string[] fields = results[0];

            for (int tupleNumber = 0; tupleNumber < tuples.Length; ++tupleNumber)
            {
                // the line we access is actually '1-indexed': the first row is the fields
                string[] line = results[tupleNumber + 1];
                PairwiseTuple pt = new PairwiseTuple(fields);

                for (int currentField = 0; currentField < tupleLen; ++currentField)
                {
                    PairwiseValue value = context.FindValue(currentField, line[currentField]);

                    pt.Add(model.Parameters[currentField].Name, value);
                }

                tuples[tupleNumber] = pt;
            }

            return tuples;
        }

        static string GetComment(IPairwiseComment comment)
        {
            if (comment.Comment == null || comment.Comment.Length == 0)
            {
                return "";
            }
            else
            {
                return "# " + comment.Comment + Environment.NewLine;
            }
        }

        string GetParameterLine(PairwiseParameter p)
        {
            StringBuilder sb = new StringBuilder(128);

            foreach (PairwiseValue o in p.Values)
            {
                if (sb.Length != 0)
                {
                    sb.Append(context.Settings.ValueDelimiter);
                }

                sb.Append(context.LookupValuesAndAliases(o));
            }

            return string.Format("{0}{1}: {2}", GetComment(p), context.LookupName(p), sb);
        }

        // need a model for context/reducing to strings?
        static string LookupOperationSymbol(PairwiseComparison comparison)
        {
            switch (comparison)
            {
                case PairwiseComparison.Equal:
                    return "=";

                case PairwiseComparison.GreaterThan:
                    return ">";

                case PairwiseComparison.GreaterThanOrEqual:
                    return ">=";

                case PairwiseComparison.LessThan:
                    return "<";

                case PairwiseComparison.LessThanOrEqual:
                    return "<=";

                case PairwiseComparison.NotEqual:
                    return "<>";

                default:
                    throw new NotImplementedException("" + comparison);
            }
        }

        string Quoted(PairwiseValue value)
        {
            if (value.PairwiseValueType == PairwiseValueType.Number)
            {
                return context.LookupOutputValue(value);
            }
            else
            {
                return '"' + context.LookupOutputValue(value) + '"';
            }
        }

        public override object Visit(PairwiseInTerm term)
        {
            string s = "";

            foreach (PairwiseValue o in term.Values)
            {
                if (s.Length != 0)
                {
                    s += context.Settings.SetValueSeparator;
                }

                s += Quoted(o);
            }

            // can't have leading spaces after the first {{
            return String.Format("{0} IN {{{1}}}", Bracketed(context.LookupName(term.Parameter)), s);
        }

        public override object Visit(PairwiseNotInTerm term)
        {
            string s = "";

            foreach (PairwiseValue o in term.Values)
            {
                if (s.Length != 0)
                {
                    s += context.Settings.SetValueSeparator;
                }

                s += Quoted(o);
            }

            return String.Format("{0} NOT IN {{{1}}}", Bracketed(context.LookupName(term.Parameter)), s);
        }

        public override object Visit(PairwiseLikeTerm term)
        {
            if (term.Parameter.PairwiseValueType != PairwiseValueType.String)
            {
                throw new PairwiseException("Can't do a like term against a non-string: " + term.Parameter);
            }

            return Bracketed(context.LookupName(term.Parameter)) + " LIKE " + Quoted(new PairwiseValue(term.WildcardPattern));
        }

        public override object Visit(PairwiseLogicalClause clause)
        {
            string rel;

            if (clause.LogicalRelationship == PairwiseLogicalRelationship.And)
            {
                rel = "AND";
            }
            else if (clause.LogicalRelationship == PairwiseLogicalRelationship.Or)
            {
                rel = "OR";
            }
            else
            {
                throw new ArgumentException("" + clause.LogicalRelationship);
            }

            return string.Format("(({0}) {1} ({2}))", clause.First.Accept(this), rel, clause.Second.Accept(this));
        }

        public override object Visit(PairwiseComparisonTerm term)
        {
            string rhs;

            if (term.HasRightValue())
            {
                rhs = Quoted(term.GetRightValue());
            }
            else
            {
                rhs = Bracketed(context.LookupName(term.GetRightParameter()));
            }

            return string.Format("{0} {1} {2}", Bracketed(context.LookupName(term.Parameter)), LookupOperationSymbol(term.Comparison), rhs);
        }

        public override object Visit(PairwiseNotClause clause)
        {
            return "NOT (" + clause.Condition.Accept(this) + ")";
        }

        [Obsolete]
        public override object Visit(PairwiseParameterConstraint constraint)
        {
            // include the semicolon
            return GetComment(constraint) + string.Format("{0} {1} {2}", Bracketed(context.LookupName(constraint.First)), LookupOperationSymbol(constraint.Comparison), Bracketed(context.LookupName(constraint.Second)));
        }

        public override object Visit(PairwiseIfThenConstraint constraint)
        {
            if (constraint.ElsePart == null)
            {
                return GetComment(constraint) + string.Format("IF {0} THEN {1};", constraint.IfPart.Accept(this), constraint.ThenPart.Accept(this));
            }
            else
            {
                return GetComment(constraint) + string.Format("IF {0} THEN {1} ELSE {2};", constraint.IfPart.Accept(this), constraint.ThenPart.Accept(this), constraint.ElsePart.Accept(this));
            }
        }

        public override object Visit(PairwiseSubModel childModel)
        {
            if (childModel.SubModelParameters.Count <= 0)
            {
                throw new InvalidOperationException("Cluster doesn't have more than one parameter: " + childModel.SubModelParameters.Count);
            }

            string s = "{";

            foreach (PairwiseParameter pp in childModel.SubModelParameters)
            {
                s += pp.Name;
                s += context.Settings.SetValueSeparator;
            }

            s = s.Remove(s.Length - context.Settings.SetValueSeparator.Length, 
                context.Settings.SetValueSeparator.Length);
            s += "}";
            s += " @ " + childModel.SubModelOrder;
            return GetComment(childModel) + s;
        }

        public override object Visit(PairwiseModel model)
        {
            if (model != this.model)
            {
                throw new ArgumentException("Can't generate for a different model!");
            }

            using (StringWriter sw = new StringWriter(PictConstants.Culture))
            {
                sw.Write(GetComment(model));
                foreach (PairwiseParameter p in model.Parameters)
                {
                    sw.WriteLine(this.GetParameterLine(p));
                }

                sw.WriteLine();
                foreach (PairwiseSubModel sub in model.SubModels)
                {
                    sw.WriteLine(sub.Accept(this));
                }

                sw.WriteLine();
                foreach (PairwiseIfThenConstraint rule in model.IfThenConstraints)
                {
                    sw.WriteLine(rule.Accept(this));
                }

                foreach (PairwiseCondition cond in model.ParameterConstraints)
                {
                    sw.Write(GetComment(cond));
                    sw.WriteLine(cond.Accept(this) + ";");
                }

                return sw.ToString();
            }
        }
    }
}
