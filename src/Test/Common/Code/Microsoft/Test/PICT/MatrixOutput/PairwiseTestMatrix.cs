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
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Security.Cryptography;
    using System.Security.Permissions;
    using Microsoft.Test.Pict;
    #endregion

    sealed class PairwiseTestMatrix
    {
        PairwiseTestCase[] cases;

        PictExecutionInformation info = new PictExecutionInformation();

        static readonly object mutex = new object();

        static XmlSerializer ser;

        static readonly XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();

        [XmlIgnore]
        public static XmlSerializer MatrixSerializer
        {
            get
            {
                if (ser == null)
                {
                    lock (mutex)
                    {
                        if (ser == null)
                        {
                            ser = new XmlSerializer(typeof(PairwiseTestMatrix), "");
                        }
                    }
                }

                return ser;
            }
        }

        public PairwiseTestCase[] PairwiseTestCases
        {
            get { return this.cases; }
            set { this.cases = value; }
        }

        public PictExecutionInformation PictExecutionInformation
        {
            get { return this.info; }
            set { this.info = value; }
        }

        public PairwiseTestMatrix()
        {
        }

        static PairwiseTestMatrix()
        {
            xsn.Add("", "");
        }

        public XmlDocument CreateXmlDocument()
        {
            XmlDocument xd = new XmlDocument();

            using (MemoryStream ms = new MemoryStream())
            {
                MatrixSerializer.Serialize(ms, this, xsn);
                ms.Seek(0, SeekOrigin.Begin);
                xd.Load(ms);
            }

            return xd;
        }

        public IXPathNavigable CreateXPathNavigable()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                MatrixSerializer.Serialize(ms, this, xsn);
                ms.Seek(0, SeekOrigin.Begin);
                return new XPathDocument(ms);
            }
        }

        public static PairwiseTestMatrix GenerateMatrixFromPictFile(string modelFile, PairwiseSettings settings)
        {
            if (modelFile == null)
            {
                throw new ArgumentNullException("modelFile");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            string[][] results = null;
            PairwiseTestMatrix matrix = new PairwiseTestMatrix();

            using (PictRunner pr = new PictRunner())
            {
                results = pr.AlwaysExecutePictOnFileName(modelFile, settings);
                matrix.PictExecutionInformation = pr.LastExecutionInformation;
            }

            string[] headerFields = results[0];
            PairwiseTestCase[] cases = new PairwiseTestCase[results.Length - 1];

            for (int tupleNo = 0; tupleNo < cases.Length; ++tupleNo)
            {
                string[] fields = results[tupleNo + 1];
                PairwiseTestParameter[] values = new PairwiseTestParameter[fields.Length];

                for (int field = 0; field < fields.Length; ++field)
                {
                    values[field] = new PairwiseTestParameter(headerFields[field], fields[field]);
                }

                cases[tupleNo] = new PairwiseTestCase(values);
            }

            // NOTE: sorting makes things easier to diff, kinda
            Array.Sort(cases, PairwiseTestCase.Comparer);
            matrix.PairwiseTestCases = cases;
            return matrix;
        }

        public static PairwiseTestMatrix GenerateMatrixFromPictFile(string modelFile, string pictArgs)
        {
            PairwiseSettings settings = PairwiseSettings.Parse(pictArgs);

            return GenerateMatrixFromPictFile(modelFile, settings);
        }

        public static PairwiseTestMatrix GenerateMatrixFromPictFile(string modelFile, string pictArgs, string pictDirectory)
        {
            string previous = PairwiseSettings.PictDirectory;

            try
            {
                PairwiseSettings.PictDirectory = pictDirectory;
                return GenerateMatrixFromPictFile(modelFile, pictArgs);
            }
            finally
            {
                PairwiseSettings.PictDirectory = previous;
            }
        }

        public static PairwiseTestMatrix LoadFromXmlFile(string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                return (PairwiseTestMatrix)MatrixSerializer.Deserialize(sr);
            }
        }

        public void WriteXmlTo(string filename)
        {
            using (TextWriter tw = new StreamWriter(filename))
            {
                WriteXmlTo(tw);
            }
        }

        public void WriteXmlTo(TextWriter textWriter)
        {
            XmlTextWriter xtw = new XmlTextWriter(textWriter);

            xtw.Formatting = Formatting.Indented;

            //
            WriteXmlTo(xtw);

            //
            textWriter.WriteLine();
            xtw.Close();
        }

        public void WriteXmlTo(XmlWriter writer)
        {
            MatrixSerializer.Serialize(writer, this, xsn);
        }

        public void WriteXmlTo(XmlTextWriter writer)
        {
            MatrixSerializer.Serialize(writer, this, xsn);
        }
    }
}

