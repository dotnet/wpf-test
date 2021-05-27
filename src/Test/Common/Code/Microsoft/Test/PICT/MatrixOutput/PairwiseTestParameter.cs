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

    [XmlType("Parameter")]
    public sealed class PairwiseTestParameter
    {
        string name;

        string val;

        [XmlAttribute]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [XmlAttribute]
        public string Value
        {
            get { return val; }
            set { val = value; }
        }

        public PairwiseTestParameter()
        {
        }

        public PairwiseTestParameter(string name, string value)
        {
            this.name = name;
            this.val = value;
        }

        public override bool Equals(object obj)
        {
            PairwiseTestParameter that = obj as PairwiseTestParameter;

            return that != null && this.name == that.name && this.val == that.val;
        }

        public override int GetHashCode()
        {
            return (name == null ? 0 : name.GetHashCode()) ^ (val == null ? 0 : val.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format(PictConstants.Culture, "TestParameter(Name={0}, Value={1})", this.name, this.val);
        }
    }
}
