// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CSDocFormatter
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.IO;
    using System.Text;

    #endregion Namespaces.

    /// <summary>
    /// This class enhances the documentation available in a regular
    /// C# documentation file with Assembly metadata.
    /// </summary>
    public class TypeNameFilters
    {
        #region Constructors.

        /// <summary>
        /// Creates a new CSDocFormatter.TypeNameFilters instance.
        /// </summary>
        /// <param name='filters'>Filters to use on type names..</param>
        public TypeNameFilters(string[] filters)
        {
            if (filters == null)
            {
                throw new ArgumentNullException("filters");
            }

            this._typeNameFilters = filters;
        }

        /// <summary>
        /// Creates a new CSDocFormatter.TypeNameFilters instance.
        /// </summary>
        /// <param name='path'>The complete file path to be read.</param>
        public TypeNameFilters(string path)
        {
            ArrayList filters;  // Filters to be added.

            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            filters = new ArrayList();
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length > 0 && line[0] != '#')
                    {
                        filters.Add(line);
                    }
                }
            }
            _typeNameFilters = (string[]) filters.ToArray(typeof(string));
        }

        #endregion Constructors.

        #region Private data.

        /// <summary>
        /// Type names to be filtered, null to filter all files. Filters ignore
        /// assemblies, and may optionally end in a * to indicate that
        /// all other entries match.
        /// </summary>
        private string[] _typeNameFilters;

        #endregion Private data.

        #region Public properties.

        /// <summary>Whether the object has any type name filters at all.</summary>
        public bool HasFilters
        {
            get { return this._typeNameFilters.Length > 0; }
        }

        /// <summary>
        /// Type names to be printed out, null to filter all files. Filters ignore
        /// assemblies, and may optionally end in a * to indicate that
        /// all other entries match.
        /// </summary>
        public string[] Filters
        {
            get { return this._typeNameFilters; }
        }

        #endregion Public properties.

        #region Public methods.

        /// <summary>
        /// Checks whether the full type name matches the filter.
        /// </summary>
        /// <param name='typeFullName'>Full name of type, eg: typeof(string).FullName.</param>
        /// <returns>
        /// true if the type matches one of the filters, false if it doesn't
        /// match any. If there are no filters, then, the match is always false.
        /// </returns>
        public bool MatchesFilters(string typeFullName)
        {
            if (typeFullName == null)
            {
                throw new ArgumentNullException("typeFullName");
            }

            for (int i = 0; i < _typeNameFilters.Length; i++)
            {
                bool isMask;
                string typeForCompare;
                string maskForCompare;

                isMask = _typeNameFilters[i].EndsWith("*");
                if (isMask)
                {
                    maskForCompare = _typeNameFilters[i].Substring(0, _typeNameFilters[i].Length - 1);
                    if (typeFullName.Length < maskForCompare.Length)
                        continue;
                    typeForCompare = typeFullName.Substring(0, maskForCompare.Length);
                }
                else
                {
                    typeForCompare = typeFullName;
                    maskForCompare = _typeNameFilters[i];
                }
                if (typeForCompare == maskForCompare)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion Public methods.
    }
}