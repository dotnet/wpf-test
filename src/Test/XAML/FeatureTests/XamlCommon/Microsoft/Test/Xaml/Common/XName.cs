// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Xname class to remove the dependency on the 
    /// linq binary which does not ship with the client sku
    /// </summary>
    public class XName : IEquatable<XName>
    {
        /// <summary>
        /// local name to use
        /// </summary>
        private string _localName;

        /// <summary>
        /// Namespace to use
        /// </summary>
        private XNamespace _ns;

        /// <summary>
        ///  hash code value
        /// </summary>
        private int _hashCode;

        /// <summary>
        /// Initializes a new instance of the XName class
        /// </summary>
        /// <param name="ns">namespace to use</param>
        /// <param name="localName">local name to use</param>
        internal XName(XNamespace ns, string localName)
        {
            this._ns = ns;
            this._localName = localName;
            this._hashCode = ns.NamespaceName.GetHashCode() ^ localName.GetHashCode();
        }

        /// <summary>
        /// Prevents a default instance of the XName class from being created.
        /// </summary>
        private XName()
        {
        }

        /// <summary>
        /// Gets the local name property
        /// </summary>
        public string LocalName
        {
            get
            {
                return this._localName;
            }
        }

        /// <summary>
        /// Gets the Namespace property
        /// </summary>
        public XNamespace Namespace
        {
            get
            {
                return this._ns;
            }
        }

        /// <summary>
        /// Gets the NamespaceName property
        /// </summary>
        public string NamespaceName
        {
            get
            {
                return this._ns.NamespaceName;
            }
        }

        /// <summary>
        /// Create an XName
        /// </summary>
        /// <param name="localName">local name to use</param>
        /// <param name="namespaceName">namespace to use</param>
        /// <returns>instance of XName</returns>
        public static XName Get(string localName, string namespaceName)
        {
            XName name = new XName(XNamespace.Get(namespaceName), localName);
            return name;
        }

        /// <summary>
        /// implicit operator
        /// </summary>
        /// <param name="expandedName">expanded name</param>
        /// <returns>Xname instance</returns>
        [CLSCompliant(false)]
        public static implicit operator XName(string expandedName)
        {
            if (expandedName == null)
            {
                return null;
            }

            return Get(expandedName);
        }

        /// <summary>
        /// Create an XName
        /// </summary>
        /// <param name="expandedName">expanded name to use</param>
        /// <returns>instance of XName</returns>
        public static XName Get(string expandedName)
        {
            if (expandedName == null)
            {
                throw new ArgumentNullException("expandedName");
            }

            if (expandedName.Length == 0)
            {
                throw new ArgumentException("argument is empty", "expandedName");
            }

            if (expandedName[0] != '{')
            {
                return XName.Get(expandedName, String.Empty);
            }

            int num = expandedName.LastIndexOf('}');
            if ((num <= 1) || (num == (expandedName.Length - 1)))
            {
                throw new ArgumentException("expanded name format is incorrect", "expandedName");
            }

            return XName.Get(expandedName.Substring(1, num - 1), expandedName.Substring(num + 1, expandedName.Length - 1 - num));
        }

        /// <summary>
        /// overriding the == operator
        /// </summary>
        /// <param name="left">left xname to compare</param>
        /// <param name="right">right xname to compare</param>
        /// <returns>true if both are equal</returns>
        public static bool operator ==(XName left, XName right)
        {
            object leftobj = left as object;
            object rightobj = right as object;

            if (leftobj == null && rightobj == null)
            {
                return true;
            }

            if (leftobj == null || rightobj == null)
            {
                return false;
            }

            return left._hashCode == right._hashCode;
        }

        /// <summary>
        /// overriding the not equal operator
        /// </summary>
        /// <param name="left">left xname to compare</param>
        /// <param name="right">right xname to compare</param>
        /// <returns>true if not equal</returns>
        public static bool operator !=(XName left, XName right)
        {
            return !(left == right);
        }

        /// <summary>
        /// overriding to string
        /// </summary>
        /// <returns>string rep of xname</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{{{0}}}{1}", this.NamespaceName, this._localName);
        }

        /// <summary>
        /// overriding the equals
        /// </summary>
        /// <param name="obj">object to compare to</param>
        /// <returns>true if equal</returns>
        public override bool Equals(object obj)
        {
            return this == (XName)obj;
        }

        /// <summary>
        /// Get the hashcode
        /// </summary>
        /// <returns>hash code of xname</returns>
        public override int GetHashCode()
        {
            return this._hashCode;
        }

        /// <summary>
        /// override equals
        /// </summary>
        /// <param name="other">other xname</param>
        /// <returns>true if equal</returns>
        bool IEquatable<XName>.Equals(XName other)
        {
            return this == other;
        }
    }
}
