// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

namespace Microsoft.Test.Xaml.Types.SDX
{
    /// <summary>
    /// XamlName base class
    /// </summary>
    public abstract class XamlName
    {
        /// <summary> Base Prefix </summary>
        private string _basePrefix;

        /// <summary> Base Name string </summary>
        private string _baseName;

        /// <summary>
        /// Gets or sets the base prefix.
        /// </summary>
        /// <value>The base prefix.</value>
        public string BasePrefix
        {
            get
            {
                return _basePrefix;
            }

            set
            {
                _basePrefix = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the base.
        /// </summary>
        /// <value>The name of the base.</value>
        public string BaseName
        {
            get
            {
                return _baseName;
            }

            set
            {
                _baseName = value;
            }
        }

        /// <summary>
        /// Gets the name of the scoped.
        /// </summary>
        /// <value>The name of the scoped.</value>
        public abstract string ScopedName { get; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return ScopedName;
        }

        /// <summary>
        /// Splits the name.
        /// </summary>
        /// <param name="longName">The long name.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="preDot">The pre dot.</param>
        /// <param name="name">The name value.</param>
        protected void SplitName(string longName, out string prefix, out string preDot, out string name)
        {
            prefix = String.Empty;
            preDot = String.Empty;
            name = String.Empty;

            int colonIdx = longName.IndexOf(':');
            if (colonIdx != -1)
            {
                prefix = longName.Substring(0, colonIdx);
            }

            int dotIdx = longName.IndexOf('.');
            if (dotIdx != -1)
            {
                int length = dotIdx - (colonIdx + 1);
                preDot = longName.Substring(colonIdx + 1, length);
                name = longName.Substring(dotIdx + 1);
            }
            else
            {
                name = longName.Substring(colonIdx + 1);
            }
        }
    }

    /// <summary>
    /// XamlTypeName Test class
    /// </summary>
    internal class XamlTypeName : XamlName
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XamlTypeName"/> class.
        /// </summary>
        /// <param name="longName">The long name.</param>
        public XamlTypeName(string longName)
        {
            string prefix;
            string preDot;
            string name;
            
            SplitName(longName, out prefix, out preDot, out name);
            if (!String.IsNullOrEmpty(preDot))
            {
                string errorMessage = String.Format("Type Name {0} cannot have a '.'", longName);
                throw new ArgumentException(errorMessage);
            }

            BasePrefix = prefix;
            BaseName = name;
        }

        /// <summary>
        /// Gets the name of the scoped.
        /// </summary>
        /// <value>The name of the scoped.</value>
        public override string ScopedName
        {
            get
            {
                if (!String.IsNullOrEmpty(BasePrefix))
                {
                    return BasePrefix + ":" + BaseName;
                }
                else
                {
                    return BaseName;
                }
            }
        }

        /*
        /// FxCop says this is not called
        public XamlTypeName(string prefix, string longName)
        {
            string noPrefix;
            string preDot;
            string name;

            base.SplitName(longName, out noPrefix, out preDot, out name);

            if (!String.IsNullOrEmpty(prefix))
            {
                string error = KS.Fmt("Two prefixes given: '{0}' and '{1}'", prefix, longName);
                throw new ArgumentException(error);
            }

            if (!String.IsNullOrEmpty(preDot))
            {
                string error = KS.Fmt("Type Name {0} cannot have a '.'", longName);
                throw new ArgumentException(error);
            }
            _prefix = prefix;
            _name = name;
        }
        */
    }

    /// <summary>
    /// XamlPropertyName Test class
    /// </summary>
    internal class XamlPropertyName : XamlName
    {
        /// <summary>
        /// owner Name.
        /// </summary>
        private readonly string _ownerName;

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlPropertyName"/> class.
        /// </summary>
        /// <param name="longName">The long name.</param>
        public XamlPropertyName(string longName)
        {
            string prefix;
            string preDot;
            string name;

            SplitName(longName, out prefix, out preDot, out name);
            BasePrefix = prefix;
            BaseName = name;
            _ownerName = preDot;
        }

        /// <summary>
        /// Gets the name of the owner.
        /// </summary>
        /// <value>The name of the owner.</value>
        public string OwnerName
        {
            get
            {
                return _ownerName;
            }
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>The full name.</value>
        /// FxCop says this is never called
        public string FullName
        {
            get
            {
                if (!String.IsNullOrEmpty(OwnerName))
                {
                    return OwnerName + "." + BaseName;
                }
                else
                {
                    return BaseName;
                }
            }
        }

        /// <summary>
        /// Gets the name of the scoped.
        /// </summary>
        /// <value>The name of the scoped.</value>
        public override string ScopedName
        {
            get
            {
                if (!String.IsNullOrEmpty(BasePrefix))
                {
                    return BasePrefix + ":" + FullName;
                }
                else
                {
                    return FullName;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is dotted name.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is dotted name; otherwise, <c>false</c>.
        /// </value>
        public bool IsDottedName
        {
            get
            {
                return !String.IsNullOrEmpty(OwnerName);
            }
        }

        /*
        /// FxCop says this is not called
        public string ScopedOwnerName
        {
            get
            {
                if (!String.IsNullOrEmpty(_prefix))
                {
                    return Prefix + ":" + OwnerName;
                }
                else
                {
                    return OwnerName;
                }
            }
        }

        public XamlPropertyName(string prefix, string longName)
        {
            string noPrefix;
            string preDot;
            string name;

            base.SplitName(longName, out noPrefix, out preDot, out name);
            if (!String.IsNullOrEmpty(noPrefix))
            {
                string errorMessage = KS.Fmt("Two prefixes given '{0}' and '{1}'", prefix, noPrefix);
                throw new ArgumentException(errorMessage);
            }
            base._prefix = prefix;
            base._name = name;
            _ownerName = preDot;
        }
         */
    }
}
