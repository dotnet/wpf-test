// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides test case metadata to provide toolability.

namespace Test.Uis.TestTypes
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Documents;
    using System.Windows.Navigation;
    using System.Windows.Threading;
    using System.Runtime.InteropServices;

    using Test.Uis.Management;
    using Test.Uis.IO;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>Provides rich metadata for test cases.</summary>
    public class TestCaseData
    {
        #region Constructors.

        /// <summary>Initializes a new TestCaseData instance.</summary>
        /// <param name='testCaseType'>Type of test case to run.</param>
        /// <param name='filter'>Filter for test case data, in name=value format (possibly null).</param>
        /// <param name='dimensions'>Dimensions to generate combinations (possibly null).</param>
        public TestCaseData(Type testCaseType, string filter, params Dimension[] dimensions)
        {
            if (testCaseType == null)
            {
                throw new ArgumentNullException("testCaseType");
            }

            if (filter == null)
            {
                filter = String.Empty;
            }
            if (dimensions == null)
            {
                dimensions = /*Dimension.EmptyArray;*/ new Dimension[0];
            }
            VerifyFilter(filter);

            this._testCaseType = testCaseType;
            this._filter = filter;
            this._dimensions = dimensions;
        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>Provides a string representation for this object.</summary>
        /// <returns>A string representation for this object.</returns>
        public override string ToString()
        {
            if (Filter.Length == 0)
            {
                return TestCaseType.Name;
            }
            else
            {
                return TestCaseType.Name + " (" + Filter + ")";
            }
        }

        #endregion Public methods.

        #region Public properties.

        /// <summary>Dimensions used to generate combinations.</summary>
        public Dimension[] Dimensions
        {
            get { return this._dimensions; }
        }

        /// <summary>Filter for dimensions, in Name=Value format.</summary>
        public string Filter
        {
            get { return this._filter; }
        }

        /// <summary>Test case type.</summary>
        public Type TestCaseType
        {
            get { return this._testCaseType; }
        }

        #endregion Public properties.

        #region Internal methods.

        /// <summary>
        /// Overwrites the dimension values in the data, according
        /// to the ConfigurationSettings specified.
        /// </summary>
        /// <param name="data">Data to modify.</param>
        /// <param name="settings">Settings to use when modifying dimension values.</param>
        private static void FilterDimensionValues(TestCaseData data,
            ConfigurationSettings settings)
        {
            foreach(Dimension dimension in data.Dimensions)
            {
                string values;

                // Only process dimensions that are overriden in the settings.
                if (settings.HasArgument(dimension.Name, out values))
                {
                    List<object> list;      // List of filtered objects.
                    string[] settingValues; // Values requested in settings.

                    //
                    // Create and populate a list of object that are also
                    // in the comman-delimited configuration value.
                    // Use the ValueToString method to convert the
                    // objects into nice, command-line-friendly strings.
                    //
                    list = new List<object>(dimension.Values.Length);
                    settingValues = values.Split(',');
                    foreach(string settingValue in settingValues)
                    {
                        foreach(object objectValue in dimension.Values)
                        {
                            if (Dimension.ValueToString(objectValue) == settingValue)
                            {
                                list.Add(objectValue);
                            }
                        }
                    }
                    dimension.SetValues(list.ToArray());
                }
            }
        }

        /// <summary>
        /// Gets the test case data for the specified type, checking
        /// filters with the given settings.
        /// </summary>
        internal static TestCaseData GetTestCaseData(Type testCaseType, ConfigurationSettings settings)
        {
            TestCaseData[] table;

            if (testCaseType == null)
            {
                throw new ArgumentNullException("testCaseType");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            table = GetTestCaseDataTable();
            foreach(TestCaseData data in table)
            {
                // Should never actually find this on a regular run, but
                // during development an entry might be commented out.
                if (data == null)
                {
                    continue;
                }

                // We look for exact matches (at least for the time being).
                if (data.TestCaseType != testCaseType)
                {
                    continue;
                }

                // The data must pass configuration filters.
                if (EvaluateFilter(data._filter, settings))
                {
                    // Provide an opportunity to override dimension
                    // values from the configuration settings.
                    FilterDimensionValues(data, settings);
                    return data;
                }
            }
            throw new InvalidOperationException(
                "There are no TestCaseData entries that match the specified type [" +
                testCaseType + "] and settings.");
        }

        /// <summary>
        /// Gets the test case data table from the main assembly.
        /// </summary>
        internal static TestCaseData[] GetTestCaseDataTable()
        {
            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();

            Assembly[] assemblies;      // Assemblies that might have the test case data table.
            List<TestCaseData> cases;   // TestCaseData instances.

            assemblies = AppDomain.CurrentDomain.GetAssemblies();
            cases = new List<TestCaseData>();
            foreach(Assembly assembly in assemblies)
            {
                Type[] types;

                // Skip some well-known assemblies that do not have test data tables.
                if (TestFinder.IsAssemblyKnownNotTest(assembly))
                {
                    continue;
                }

                types = assembly.GetTypes();
                foreach(Type type in types)
                {
                    object[] attributes;    // Custom attributes.

                    attributes = type.GetCustomAttributes(typeof(TestCaseDataTableClassAttribute), false);
                    if (attributes != null && attributes.Length > 0)
                    {
                        FieldInfo[] fields; // Field information for type.

                        fields = type.GetFields();
                        foreach(FieldInfo field in fields)
                        {
                            attributes = field.GetCustomAttributes(typeof(TestCaseDataTableAttribute), false);
                            if (attributes != null && attributes.Length > 0)
                            {
                                TestCaseData[] casesInAclass = (TestCaseData[]) field.GetValue(null);
                                for (int i = 0; i < casesInAclass.Length; i++)
                                {
                                    cases.Add(casesInAclass[i]);
                                }
                            }
                        }
                    }
                }
            }

            // If we found any TestCaseData tables, we return their entries.
            if (cases.Count > 0)
            {
                return cases.ToArray();
            }

            string message;
            message = "There are no test case data tables in the loaded assemblies: ";
            foreach(Assembly assembly in assemblies)
            {
                message += "\r\n" + assembly.FullName;
                if (TestFinder.IsAssemblyKnownNotTest(assembly))
                {
                    message += " - known not to have tests";
                }
                else
                {
                    message += " - found no classes with TestCaseDataTableClassAttribute";
                }
            }
            throw new InvalidOperationException(message);
        }

        #endregion Internal methods.

        #region Private methods.

        /// <summary>Evaluates whether the specified filter matches the settings.</summary>
        private static bool EvaluateFilter(string filter, ConfigurationSettings settings)
        {
            int equalIndex;
            string name;
            string value;

            if (filter.Length == 0)
            {
                return true;
            }

            equalIndex = filter.IndexOf("=");
            name = filter.Substring(0, equalIndex);
            value = filter.Substring(equalIndex + 1);
            return settings.GetArgument(name) == value;
        }

        /// <summary>Verifies that the specified filter is valid.</summary>
        private static void VerifyFilter(string filter)
        {
            int equalIndex;

            if (filter.Length == 0)
            {
                return;
            }

            equalIndex = filter.IndexOf("=");
            if (equalIndex == -1)
            {
                throw new ArgumentException("filter", "Filter does not have a '=' character.");
            }
            if (equalIndex == 0)
            {
                throw new ArgumentException("filter", "Filter does not have a name part.");
            }
            if (equalIndex == filter.Length - 1)
            {
                throw new ArgumentException("filter", "Filter does not have a value part.");
            }
        }

        #endregion Private methods.

        #region Private fields.

        /// <summary>Dimensions used to generate combinations.</summary>
        private Dimension[] _dimensions;

        /// <summary>Filter for dimensions, in Name=Value format.</summary>
        private string _filter;

        /// <summary>Test case type.</summary>
        private Type _testCaseType;

        #endregion Private fields.
    }
}