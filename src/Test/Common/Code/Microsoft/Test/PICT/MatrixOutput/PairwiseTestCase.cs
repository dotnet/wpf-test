// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Pict.MatrixOutput
{
    #region using;
    using System;
    using System.Globalization;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Serialization;
    using System.Collections;
    using System.IO;
    using System.Text;
    using Microsoft.Test.Pict;
    #endregion

    public sealed class PairwiseTestCase
    {
        PairwiseTestParameter[] pp;

        string sig = null;

        public PairwiseTestParameter[] Parameters
        {
            get { return pp; }
            set
            {
                sig = null;
                pp = value;
            }
        }

        [XmlAttribute]
        public string Signature
        {
            get
            {
                if (sig == null)
                {
                    sig = CalculateSignature();
                }

                return sig;
            }
            set
            {
                // ignore: XML serialization only
            }
        }

        public string this[string parameterName]
        {
            get
            {
                if (pp == null)
                {
                    throw new InvalidOperationException("Parameters = null");
                }

                if (pp.Length == 0)
                {
                    throw new InvalidOperationException("No parameters available");
                }

                foreach (PairwiseTestParameter p in this.pp)
                {
                    if (p.Name == parameterName)
                    {
                        return p.Value;
                    }
                }

                throw new IndexOutOfRangeException(parameterName + " not found");
            }
        }

        public PairwiseTestCase()
        {
        }

        public PairwiseTestCase(PairwiseTestParameter[] parameters)
        {
            this.pp = (PairwiseTestParameter[])parameters.Clone();
        }

        string CalculateSignature()
        {
            if (this.pp == null || this.pp.Length == 0)
            {
                return "";
            }

            // this is terribly inefficient; it could/should be improved...
            // PairwiseTestParameter[] copy = (PairwiseTestParameter[]) this.Parameters.Clone();
            // Array.Sort(copy, ParameterNameComparer.Instance);
            StringBuilder sb = new StringBuilder(pp.Length * 20);

            foreach (PairwiseTestParameter p in this.pp)
            {
                sb.Append(p.Value);
                sb.Append('_');
            }

            sb.Length = sb.Length - 1;

            // copy = null;
            return sb.ToString();
        }

        static readonly PairwiseTestCaseSignatureComparer comp = PairwiseTestCaseSignatureComparer.Instance;

        [XmlIgnore]
        public static IComparer Comparer
        {
            get { return comp; }
        }

        sealed class PairwiseTestCaseSignatureComparer : IComparer
        {
            internal static readonly PairwiseTestCaseSignatureComparer Instance = new PairwiseTestCaseSignatureComparer();

            PairwiseTestCaseSignatureComparer()
            {
            }

            public int Compare(object x, object y)
            {
                PairwiseTestCase a = (PairwiseTestCase)x;
                PairwiseTestCase b = (PairwiseTestCase)y;

                return string.CompareOrdinal(a.Signature, b.Signature);
            }
        }
    }
}
