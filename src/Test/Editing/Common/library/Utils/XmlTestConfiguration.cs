// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides XML-based configuration for test cases.

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Drawing;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Net;
    using System.IO;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Xml;

    #endregion Namespaces.

    /// <summary>
    /// Handles XML configuration information.
    /// </summary>
    internal class XmlTestConfiguration
    {

        #region Constructors.

        /// <summary>Creates a new XmlTestConfiguration instance.</summary>
        internal XmlTestConfiguration(string xmlFileName, string testName) : this()
        {
            if (xmlFileName == null)
            {
                throw new ArgumentNullException("xmlFileName");
            }
            if (testName == null)
            {
                throw new ArgumentNullException("testName");
            }
            if (testName == String.Empty)
            {
                throw new ArgumentException("Test name cannot be blank.");
            }

            if (xmlFileName != String.Empty)
                InitializeValues(xmlFileName, testName);
        }

        /// <summary>Internal constructor.</summary>
        private XmlTestConfiguration()
        {
            _values = new System.Collections.Hashtable();
        }

        #endregion Constructors.


        #region Internal methods.

        /// <summary>Adds the settings to the specified hash table.</summary>
        internal void AddSettingsTo(Hashtable destination)
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            foreach (DictionaryEntry entry in _values)
            {
                destination[entry.Key] = entry.Value;
            }
        }

        /// <summary>Clones existing values.</summary>
        /// <returns>A clone of the value in the XML configuration.</returns>
        internal Hashtable CloneValues()
        {
            return (System.Collections.Hashtable) _values.Clone();
        }

        /// <summary>Retrieves a named value.</summary>
        /// <param name="valueName">Name of the value to retrieve.</param>
        /// <returns>The value as a string, null if not found.</returns>
        internal string GetValue(string valueName)
        {
            if (valueName == null || valueName == String.Empty)
                return null;
            object result = _values[valueName];
            string resultString = (result == null)? null : result.ToString();
            return resultString;
        }

        #endregion Internal methods.
      

        #region Internal properties.

        /// <summary>
        /// Name of argument name for XML block.
        /// </summary>
        internal static string XmlBlockArgumentName
        {
            get { return "XmlBlock"; }
        }

        #endregion Internal properties.


        #region Private methods.

        /// <summary>Initializes the instance from the configured properties.</summary>
        /// <remarks>
        /// The XML format is a top node whose name is ignored, containing a
        /// set of elements with text. The element name is used as the argument name,
        /// and the text node in the element is used as the argument value. If
        /// elements are embedded, then they the names are concatenated,
        /// separated with a minus character. Combinations tags and their
        /// subtrees are skipped.
        /// </remarks>
        private void InitializeValues(string xmlFileName, string testName)
        {
            System.Diagnostics.Debug.Assert(xmlFileName != null);
            System.Diagnostics.Debug.Assert(testName != null);

            bool testNameFoundInXml = false;

            PermissionSet perms = new PermissionSet(PermissionState.Unrestricted);
            perms.AddPermission(
                new SecurityPermission(PermissionState.Unrestricted));
            perms.AddPermission(
                new FileIOPermission(PermissionState.Unrestricted));
            perms.Assert();

            XmlTextReader reader = new XmlTextReader(xmlFileName);
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element &&
                        reader.Name == testName)
                    {
                        //
                        // Capture the whole block as-is, then parse that
                        // for name/value pairs. If the block is assigned
                        // from the data, do not ovewrite - this is required
                        // to allow a test case to completely set up
                        // the input file for another test case.
                        //
                        string xmlBlock = reader.ReadOuterXml();

                        XmlTextReader blockReader =
                            new XmlTextReader(new StringReader(xmlBlock));
                        ReadTestData(blockReader);
                        if (!_values.ContainsKey(XmlBlockArgumentName))
                        {
                            _values[XmlBlockArgumentName] = xmlBlock.ToString();
                        }
                        testNameFoundInXml = true;

                        break;
                    }
                }
            }
            finally
            {
                reader.Close();
            }

            if (!testNameFoundInXml && ConfigurationSettings.Current.HasArgument("TestName"))
            {
                string message = String.Format("Testxml named [{0}] doesn't contain the section [{1}]. Please check testxml.",
                      xmlFileName,
                      testName);

                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Reads all the name/value pairs of the reader.
        /// </summary>
        /// <remarks>
        /// XML comments are sent to the current logger.
        /// </remarks>
        private void ReadTestData(XmlTextReader reader)
        {
            ArrayList argumentNames = new ArrayList();
            int depth = 0;
            string argumentValue = null;
            bool inAction = false;
            bool inCombination = false;
            StringBuilder sb = new StringBuilder(128);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        // Chars added are tracked to remove quickly.
                        int charsAdded = 0;

                        // Skip elements named Combinations and their
                        // child elements.
                        if (reader.Name == "Combinations")
                        {
                            if (!reader.IsEmptyElement)
                            {
                                inCombination = true;
                            }
                            break;
                        }

                        if (inCombination) break;

                        // Skip elements named Action and their child
                        // elements.
                        if (reader.Name == "Action")
                        {
                            if (!reader.IsEmptyElement)
                            {
                                inAction = true;
                                break;
                            }
                        }

                        if (inAction) break;

                        // depth == 0 only for root element (ignored)
                        if (depth > 0)
                        {
                            if (depth > 1)
                            {
                                sb.Append("-");
                                charsAdded++;
                            }
                            sb.Append(reader.Name);
                            charsAdded += reader.Name.Length;
                            string val = reader.GetAttribute("Value");
                            if (val != null && val.Length > 0)
                                argumentValue = val;
                        }

                        //
                        // Empty elements will not give us a pass through
                        // EndElement, so we do some shortcut processing.
                        //
                        if (reader.IsEmptyElement)
                        {
                            if (argumentValue == null)
                            {
                                _values[sb.ToString()] = String.Empty;
                            }
                            else
                            {
                                _values[sb.ToString()] = argumentValue;
                            }

                            argumentValue = null;
                            sb.Remove(sb.Length - charsAdded, charsAdded);
                        }
                        else
                        {
                            if (depth > 0)
                                argumentNames.Add(reader.Name);

                            depth++;
                        }

                        break;
                    case XmlNodeType.EndElement:
                        // Stop skipping when combinations are over.
                        if (reader.Name == "Combinations")
                        {
                            inCombination = false;
                            break;
                        }
                        if (inCombination) break;

                        // Stop skipping when actions are over.
                        if (reader.Name == "Actions")
                        {
                            inAction = false;
                            break;
                        }
                        if (inAction) break;

                        if (argumentValue != null)
                        {
                            _values[sb.ToString()] = argumentValue;
                            argumentValue = null;
                        }
                        if (depth > 2)
                        {
                            sb.Remove(sb.Length - 1, 1);
                        }
                        if (depth > 1)
                        {
                            string s = (string)argumentNames[argumentNames.Count-1];
                            sb.Remove(sb.Length - s.Length, s.Length);
                            argumentNames.RemoveAt(argumentNames.Count-1);
                        }
                        depth--;
                        break;
                    case XmlNodeType.CDATA:
                        if (inCombination) break;
                        if (inAction) break;
                        argumentValue = reader.Value;
                        break;
                    case XmlNodeType.Text:
                        if (inCombination) break;
                        if (inAction) break;
                        argumentValue = reader.Value;
                        break;
                    case XmlNodeType.Comment:
                        System.Diagnostics.Trace.WriteLine("XML Comment: " + reader.Value);
                        break;
                }
            }
        }

        #endregion Private methods.


        #region Private fields.

        private System.Collections.Hashtable _values;

        #endregion Private fields.
    }
}