// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $")]

namespace Test.Uis.Management
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Threading; using System.Windows.Threading;
    using System.Reflection;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Documents;
    using System.Windows.Navigation;

    #endregion Namespaces.

    /// <summary>Provides an attribute to collection file version information.</summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class VersionInformationAttribute: Attribute
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors.

        /// <summary>
        /// Initializes a new VersionInformationAttribute instance with
        /// information from the specified description string.
        /// </summary>
        /// <param name='description'>Description string.</param>
        /// <remarks>
        /// The description string should be in the following
        /// format: $attrib1:value1$ $attrib1:value1$ ...
        /// </remarks>
        public VersionInformationAttribute(string description)
        {
            int processedLength;
            string fieldName;
            string fieldValue;

            System.Diagnostics.Debug.Assert(description != null);

            this._author = "";
            this._change = "";
            this._date = "";
            this._revision = "";
            this._source = "";

            processedLength = 0;
            while (ParseField(description, ref processedLength,
                out fieldName, out fieldValue))
            {
                switch(fieldName)
                {
                    case "author":
                        this._author = fieldValue;
                        break;
                    case "change":
                        this._change = fieldValue;
                        break;
                    case "date":
                        this._date = fieldValue;
                        break;
                    case "revision":
                        this._revision = fieldValue;
                        break;
                    case "source":
                        this._source = fieldValue;
                        break;
                }
            }
        }

        #endregion Constructors.

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public methods.

        /// <summary>Retrieves the attributes for a given assembly.</summary>
        /// <param name='assembly'>Assembly to get attributes from.</param>
        /// <returns>The attribute instances, an empty set if none were set.</returns>
        public static VersionInformationAttribute[] FromAssembly(Assembly assembly)
        {
            const bool inheritFalse = false;

            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            return (VersionInformationAttribute[])
                assembly.GetCustomAttributes(typeof(VersionInformationAttribute), inheritFalse);
        }

        /// <summary>
        /// Creates a report document in HTML format about the attributes
        /// declared in the assemblies loaded into the current
        /// AppDomain.
        /// </summary>
        /// <returns>Report document in HTML format.</returns>
        public static string CreateHtmlReport()
        {
            return CreateHtmlReport(AppDomain.CurrentDomain);
        }

        /// <summary>
        /// Creates a report document in HTML format about the attributes
        /// declared in the assemblies loaded into the specified
        /// AppDomain.
        /// </summary>
        /// <param name='domain'>Domain to report on.</param>
        /// <returns>Report document in HTML format.</returns>
        public static string CreateHtmlReport(AppDomain domain)
        {
            StringBuilder result;
            VersionInformationAttribute[] attributes;
            Assembly[] assemblies;

            if (domain == null)
            {
                throw new ArgumentNullException("domain");
            }

            result = new StringBuilder();
            result.Append("<html><head><title>File version report</title></head>" +
                "<body style='font-family:Verdana'><h1>File version report</h1>");
            assemblies = domain.GetAssemblies();
            foreach(Assembly assembly in assemblies)
            {
                result.Append("<p><b>Assembly: " + assembly + "</b></p>");
                attributes = FromAssembly(assembly);
                System.Diagnostics.Debug.Assert(attributes != null);
                if (attributes.Length == 0)
                {
                    result.Append("<p>No attributes in this assembly.</p><hr />");
                    continue;
                }

                System.Diagnostics.Debug.Assert(attributes.Length > 0);
                SortByChange(attributes);

                result.Append("<table><tr><th>Author</th><th>Change</th>" +
                    "<th>Date</th><th>Revision</th><th>Source</th></tr>");

                foreach(VersionInformationAttribute attribute in attributes)
                {
                    result.Append("<tr><td>" + attribute.Author +
                        "</td><td>" + attribute.Change + "</td><td>" +
                        attribute.Date + "</td><td>" + attribute.Revision +
                        "</td><td>" + attribute.Source + "</td></tr>");
                }
                result.Append("</table><hr />");
            }
            result.Append("</body></html>");
            return result.ToString();
        }

        #endregion Public methods.

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public properties.

        /// <summary>uthor of the current version.</summary>
        public string Author
        {
            get { return this._author; }
        }

        /// <summary>Change number of the current version.</summary>
        public string Change
        {
            get { return this._change; }
        }

        /// <summary>Date of checkin of the current version.</summary>
        public string Date
        {
            get { return this._date; }
        }

        /// <summary>Revision number of the current version.</summary>
        public string Revision
        {
            get { return this._revision; }
        }

        /// <summary>Source path of the current version.</summary>
        public string Source
        {
            get { return this._source; }
        }

        #endregion Public properties.

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private methods.

        /// <summary>Sorts the specified values by their change number.</summary>
        private static void SortByChange(VersionInformationAttribute[] values)
        {
            System.Diagnostics.Debug.Assert(values != null);
            Array.Sort(values, new ChangeComparer());
        }

        /// <summary>Compares two VersionInformationAttribute objects.</summary>
        class ChangeComparer: IComparer
        {
            public int Compare(object x, object y)
            {
                VersionInformationAttribute attributeX;
                VersionInformationAttribute attributeY;
                int changeX;
                int changeY;

                attributeX = (VersionInformationAttribute)x;
                attributeY = (VersionInformationAttribute)y;

                int.TryParse(attributeX.Change, out changeX);
                int.TryParse(attributeY.Change, out changeY);

                return changeY - changeX;
            }
        }

        /// <summary>Parses fields in standard format.</summary>
        private bool ParseField(string description, ref int processedLength,
            out string fieldName, out string fieldValue)
        {
            int startIndex;
            int endIndex;
            int separatorIndex;
            string field;

            System.Diagnostics.Debug.Assert(description != null);
            System.Diagnostics.Debug.Assert(processedLength <= description.Length);

            fieldName = null;
            fieldValue = null;

            startIndex = description.IndexOf("$", processedLength);
            if (startIndex == -1)
            {
                return false;
            }
            startIndex++;

            endIndex = description.IndexOf("$", startIndex);
            if (endIndex == -1)
            {
                return false;
            }

            field = description.Substring(startIndex, endIndex - startIndex);
            separatorIndex = field.IndexOf(":");
            if (separatorIndex == -1)
            {
                return false;
            }

            fieldName = field.Substring(0, separatorIndex).ToLowerInvariant().Trim();
            fieldValue = field.Substring(separatorIndex + 1).ToLowerInvariant().Trim();
            processedLength = endIndex + 1;
            return true;
        }

        #endregion Private methods.

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private fields.

        private readonly string _author;
        private readonly string _change;
        private readonly string _date;
        private readonly string _revision;
        private readonly string _source;

        #endregion Private fields.
    }
}

