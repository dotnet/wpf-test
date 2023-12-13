// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Management attributes to add metadata to test cases.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Management/Attributes.cs $")]

namespace Test.Uis.Management
{
    #region Namespaces.

    using System;
    using System.Reflection;

    #endregion Namespaces.

    /// <summary>
    /// Specifies associated bug numbers for this test case.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public class TestBugsAttribute: System.Attribute
    {
        #region Public members.

        /// <summary>Creates a new TestBugsAttribute instance.</summary>
        /// <param name='bugs'>Comma-separated string of bug numbers.</param>
        public TestBugsAttribute(string bugs)
        {
            this._bugs = bugs;
        }

        /// <summary>Comma-delimited string of bug numbers.</summary>
        public string Bugs
        {
            get { return this._bugs; }
        }

        /// <summary>Bug numbers.</summary>
        public int[] BugIDs
        {
            get
            {
                string[] strings = this._bugs.Split(',');
                int[] ids = new int[strings.Length];

                for (int i=0; i < strings.Length; i++)
                {
                    ids[i] = int.Parse(strings[i]);
                }
                return ids;
            }
        }

        /// <summary>Retrieves the attribute for a given type.</summary>
        /// <param name='type'>Type to get attribute from.</param>
        /// <returns>The attribute instance, null if none was set.</returns>
        public static TestBugsAttribute FromType(Type type)
        {
            const bool inherit = true;
            Type safeType = type;
            object[] attributes = safeType.GetCustomAttributes(
                typeof(TestBugsAttribute), inherit);

            if (attributes.Length == 0)
                return null;
            else
                return (TestBugsAttribute) attributes[0];
        }

        #endregion Public members.

        #region Private members.

        /// <summary>Bug numbers.</summary>
        private readonly string _bugs;

        #endregion Private members.
    }

    /// <summary>Specifies that this type holds a TestCaseData table.</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public class TestCaseDataTableClassAttribute: System.Attribute
    {
        #region Constructors.

        /// <summary>Creates a new TestCaseDataTableClassAttribute instance.</summary>
        public TestCaseDataTableClassAttribute()
        {
        }

        #endregion Constructors.
    }

    /// <summary>Specifies that this field holds a TestCaseData table.</summary>
    [AttributeUsage(AttributeTargets.Field, Inherited=false, AllowMultiple=false)]
    public class TestCaseDataTableAttribute: System.Attribute
    {
        #region Constructors.

        /// <summary>Creates a new TestCaseDataTableAttribute instance.</summary>
        public TestCaseDataTableAttribute()
        {
        }

        #endregion Constructors.
    }

    /// <summary>
    /// Specifies that a method is an entry point for a test case.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TestEntryPointAttribute: System.Attribute
    {
        #region Public members.

        /// <summary>Creates a new TestEntryPointAttribute instance.</summary>
        public TestEntryPointAttribute()
        {
        }

        /// <summary>Retrieves the attribute for a given method.</summary>
        /// <param name='method'>Method  to get attribute from.</param>
        /// <returns>The attribute instance, null if none was set.</returns>
        public static TestEntryPointAttribute FromMethod(System.Reflection.MethodInfo method)
        {
            const bool inherit = true;
            MethodInfo safeMethod;
            safeMethod = method;
            object[] attributes = safeMethod.GetCustomAttributes(
                typeof(TestEntryPointAttribute), inherit);

            if (attributes.Length == 0)
                return null;
            else
                return (TestEntryPointAttribute) attributes[0];
        }

        #endregion Public members.
    }

    /// <summary>
    /// Specifies the alias of the test developer that owns a test case type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public class TestOwnerAttribute: System.Attribute
    {
        #region Public members.

        /// <summary>Creates a new TestOwnerAttribute instance.</summary>
        /// <param name='ownerAlias'>Email alias of test owner.</param>
        public TestOwnerAttribute(string ownerAlias)
        {
            this._ownerAlias = ownerAlias;
        }

        /// <summary>Email alias of test owner.</summary>
        public string OwnerAlias
        {
            get { return this._ownerAlias; }
        }

        /// <summary>Retrieves the attribute for a given type.</summary>
        /// <param name='type'>Type to get attribute from.</param>
        /// <returns>The attribute instance, null if none was set.</returns>
        public static TestOwnerAttribute FromType(Type type)
        {
            const bool inherit = true;
            Type safeType = type;
            object[] attributes = safeType.GetCustomAttributes(
                typeof(TestOwnerAttribute), inherit);

            if (attributes.Length == 0)
                return null;
            else
                return (TestOwnerAttribute) attributes[0];
        }

        #endregion Public members.

        #region Private members.

        /// <summary>Email alias of test owner.</summary>
        private readonly string _ownerAlias;

        #endregion Private members.
    }

    /// <summary>
    /// Specifies the date on which the case was last updated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class TestLastUpdatedOnAttribute : System.Attribute
    {
        #region Public members.

        /// <summary>Creates a new TestLastUpdatedOnAttribute instance.</summary>
        /// <param name='updateDate'>updateDate of the test case.</param>
        public TestLastUpdatedOnAttribute(string updateDate)
        {
            this._updateDate = updateDate;
        }

        /// <summary>updateDate of the test case.</summary>
        public string UpdateDate
        {
            get { return this._updateDate; }
        }

        /// <summary>Retrieves the attribute for a given type.</summary>
        /// <param name='type'>Type to get attribute from.</param>
        /// <returns>The attribute instance, null if none was set.</returns>
        public static TestLastUpdatedOnAttribute FromType(Type type)
        {
            const bool inherit = true;
            Type safeType = type;
            object[] attributes = safeType.GetCustomAttributes(
                typeof(TestLastUpdatedOnAttribute), inherit);

            if (attributes.Length == 0)
                return null;
            else
                return (TestLastUpdatedOnAttribute)attributes[0];
        }

        #endregion Public members.

        #region Private members.

        /// <summary>Email alias of test owner.</summary>
        private readonly string _updateDate;

        #endregion Private members.
    }

    /// <summary>
    /// Specifies information about a test case in the Tactics database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public class TestTacticsAttribute: System.Attribute
    {
        /// <summary>Tactics IDs.</summary>
        private readonly string _ids;

        #region Public members.

        /// <summary>Creates a new TestTacticsAttribute instance.</summary>
        /// <param name='ids'>Tactics ID numbers.</param>
        public TestTacticsAttribute(string ids)
        {
            this._ids = ids;
        }

        /// <summary>Tactics IDs.</summary>
        public string[] IDs
        {
            get { return this._ids.Split(','); }
        }

        /// <summary>Provides a string representation of the attribute.</summary>
        /// <returns>The Tactics ID numbers for this case.</returns>
        public override string ToString()
        {
            return this._ids;
        }

        /// <summary>Retrieves the attribute for a given type.</summary>
        /// <param name='type'>Type to get attribute from.</param>
        /// <returns>The attribute instance, null if none was set.</returns>
        public static TestTacticsAttribute FromType(Type type)
        {
            const bool inherit = true;
            Type safeType = type;
            object[] attributes = safeType.GetCustomAttributes(
                typeof(TestTacticsAttribute), inherit);

            if (attributes.Length == 0)
                return null;
            else
                return (TestTacticsAttribute) attributes[0];
        }

        #endregion Public members.
    }

    /// <summary>
    /// Specifies an argument that a test case uses.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=true, AllowMultiple=true)]
    public class TestArgumentAttribute: System.Attribute
    {
        /// <summary>Argument name.</summary>
        private readonly string _name;
        /// <summary>Argument description.</summary>
        private readonly string _description;

        #region Public members.

        /// <summary>Creates a new TestBugsAttribute instance.</summary>
        /// <param name='name'>Argument name.</param>
        public TestArgumentAttribute(string name)
        {
            this._name = name;
            this._description = String.Empty;
        }

        /// <summary>Creates a new TestBugsAttribute instance.</summary>
        /// <param name='name'>Argument name.</param>
        /// <param name='description'>Argument description.</param>
        public TestArgumentAttribute(string name, string description)
        {
            this._name = name;
            this._description = description;
        }

        /// <summary>Argument name.</summary>
        public string Name
        {
            get { return this._name; }
        }

        /// <summary>Argument description.</summary>
        public string Description
        {
            get { return this._description; }
        }

        /// <summary>Retrieves the attributes for a given type.</summary>
        /// <param name='type'>Type to get attributes from.</param>
        /// <returns>The attribute instances, an empty array if none was set.</returns>
        public static TestArgumentAttribute[] FromType(Type type)
        {
            const bool inherit = true;
            Type safeType = type;
            object[] attributes = safeType.GetCustomAttributes(
                typeof(TestArgumentAttribute), inherit);
            return (TestArgumentAttribute[]) attributes;
        }

        #endregion Public members.
    }

    /// <summary>
    /// Specifies the title for a test case.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public class TestTitleAttribute: System.Attribute
    {
        /// <summary>Test title.</summary>
        private readonly string _title;

        #region Public members.

        /// <summary>Creates a new TestTitleAttribute instance.</summary>
        /// <param name='title'>Test case title.</param>
        public TestTitleAttribute(string title)
        {
            this._title = title;
        }

        /// <summary>Test case title.</summary>
        public string Title
        {
            get { return this._title; }
        }

        /// <summary>Retrieves the attribute for a given type.</summary>
        /// <param name='type'>Type to get attribute from.</param>
        /// <returns>The attribute instance, null if none was set.</returns>
        public static TestTitleAttribute FromType(Type type)
        {
            const bool inherit = false;
            Type safeType = type;
            object[] attributes =
                safeType.GetCustomAttributes(typeof(TestTitleAttribute),
                inherit);
            if (attributes.Length > 0)
                return (TestTitleAttribute)attributes[0];
            else
                return null;
        }

        #endregion Public members.
    }

    /// <summary>
    /// Specifies the API sample that was based on the test case.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=true)]
    public class TestSampleAttribute: System.Attribute
    {
        /// <summary>Sample API.</summary>
        private readonly string _sampleAPI;

        #region Public members.

        /// <summary>Creates a new TestSampleAttribute instance.</summary>
        /// <param name='sampleAPI'>
        /// API (class or class.member) that has a sample based on the
        /// test case.
        /// </param>
        public TestSampleAttribute(string sampleAPI)
        {
            this._sampleAPI = sampleAPI;
        }

        /// <summary>Sample API.</summary>
        public string SampleAPI
        {
            get { return this._sampleAPI; }
        }

        /// <summary>Retrieves the attributes for a given type.</summary>
        /// <param name='type'>Type to get attributes from.</param>
        /// <returns>The attribute instances, an empty array if none was set.</returns>
        public static TestSampleAttribute[] FromType(Type type)
        {
            const bool inherit = true;
            Type safeType = type;
            object[] attributes = safeType.GetCustomAttributes(
                typeof(TestSampleAttribute), inherit);
            return (TestSampleAttribute[]) attributes;
        }

        #endregion Public members.
    }

    /// <summary>
    /// Specifies the test spec that defines the work item.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=true)]
    public class TestSpecAttribute: System.Attribute
    {
        #region Public members.

        /// <summary>Initializes a new TestSpecAttribute instance.</summary>
        /// <param name='specCode'>Specification code name.</param>
        public TestSpecAttribute(string specCode)
        {
            this._specCode = specCode;
        }

        /// <summary>Specification code name.</summary>
        public string SpecCode
        {
            get { return this._specCode; }
        }

        /// <summary>Retrieves the attribute for a given type.</summary>
        /// <param name='type'>Type to get attribute from.</param>
        /// <returns>The attribute instance, null if none was set.</returns>
        public static TestSpecAttribute FromType(Type type)
        {
            const bool inherit = false;
            object[] attributes = type
                .GetCustomAttributes(typeof(TestSpecAttribute), inherit);
            if (attributes.Length > 0)
                return (TestSpecAttribute)attributes[0];
            else
                return null;
        }

        #endregion Public members.

        #region Private members.

        /// <summary>Specification code name.</summary>
        private readonly string _specCode;

        #endregion Private members.
    }

    /// <summary>
    /// Specifies the work item that the test case implements.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=true)]
    public class TestWorkItemAttribute: System.Attribute
    {
        #region Public members.

        /// <summary>Initializes a new TestWorkItemAttribute instance.</summary>
        /// <param name='workItemIdentifier'>Work item identifier.</param>
        public TestWorkItemAttribute(string workItemIdentifier)
        {
            this._workItemIdentifier = workItemIdentifier;
        }

        /// <summary>Work item identifier.</summary>
        public string WorkItemIdentifier
        {
            get { return this._workItemIdentifier; }
        }

        /// <summary>Retrieves the attribute for a given type.</summary>
        /// <param name='type'>Type to get attribute from.</param>
        /// <returns>The attribute instance, null if none was set.</returns>
        public static TestWorkItemAttribute FromType(Type type)
        {
            const bool inherit = false;
            object[] attributes = type
                .GetCustomAttributes(typeof(TestWorkItemAttribute), inherit);
            if (attributes.Length > 0)
                return (TestWorkItemAttribute)attributes[0];
            else
                return null;
        }

        #endregion Public members.

        #region Private members.

        /// <summary>Work item identifier.</summary>
        private readonly string _workItemIdentifier;

        #endregion Private members.
    }

    /// <summary>
    /// This class is to support attribute on classes in subclasses of CustomTestCase.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class WindowlessTestAttribute: System.Attribute
    {
        #region Public members.

        /// <summary>
        /// Initializes a new WindowlessTestAttribute instance.
        /// </summary>
        /// <param name="isWindowless">
        /// Whether the test case can run without a window.
        /// </param>
        public WindowlessTestAttribute(bool isWindowless)
        {
            this._isWindowless = isWindowless;
        }

        /// <summary>Retrieves the attributes for a given type.</summary>
        /// <param name='type'>Type to get attributes from.</param>
        /// <returns>The attribute instances, an empty array if none was set.</returns>
        /// <example><code>...
        /// Type testCaseType = typeof(MyTestCase);
        /// WindowlessTestAttribute attribute =
        ///     WindowlessTestAttribute.FromType(testCaseType);
        /// bool isTestWindowless = attribute == null &amp;&amp; attribute.IsWindowless;
        /// System.Console.WriteLine("Test {0} can run without a window: {1}",
        ///     testCaseType, isTestWindowless);
        /// </code></example>
        public static WindowlessTestAttribute FromType(Type type)
        {
            const bool inherit = true;

            Type safeType = type;
            object[] attributes = safeType.GetCustomAttributes(typeof(WindowlessTestAttribute), inherit);

            if (attributes.Length > 0)
            {
                return (WindowlessTestAttribute)attributes[0];
            }

            return null;
        }

        /// <summary>Whether the test case can run without a window.</summary>
        public bool IsWindowless
        {
            get { return this._isWindowless; }
        }

        #endregion Public members.

        #region Internal methods.

        internal static bool IsTestWindowless(Type testCaseType)
        {
            WindowlessTestAttribute attribute;

            if (testCaseType == null)
            {
                throw new ArgumentNullException("testCaseType");
            }

            attribute = FromType(testCaseType);
            if (attribute == null)
            {
                return false;
            }
            else
            {
                return attribute.IsWindowless;
            }
        }

        #endregion Internal methods.

        #region Private fields.

        /// <summary>Whether the test case can run without a window.</summary>
        private readonly bool _isWindowless;

        #endregion Private fields.
    }
}
