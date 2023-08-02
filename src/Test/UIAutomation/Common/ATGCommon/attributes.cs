// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Description: Wrapper objects for driving the UI easier
// Author : Microsoft

using System;
using System.Windows;

namespace WUITest
{

    /// <summary>
    /// This is the custom attribues on each test cases
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Struct, AllowMultiple = true)]
    public sealed class TestCaseAttribute : Attribute
    {
        string _testName;
        TestStatus _teststatus;
        string _description;
        string _author;
        TestType _testType;
        int _wttJobId = 0;
        Priority _priority = Priority.Pri1;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="TestName"></param>
        /// <param name="priority"></param>
        /// <param name="TestStatus"></param>
        /// <param name="Author"></param>
        /// <param name="Description"></param>
        public TestCaseAttribute(string testName, TestStatus testStatus, string author, string description)
        {
            _testName = testName;
            _teststatus = testStatus;
            _description = description;
            _author = author;
            _testType = TestType.Generic;
        }
        public Priority Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }
        public int WttJobId
        {
            get { return _wttJobId; }
            set { _wttJobId = value; }
        }
        public TestStatus TestStatus
        {
            get { return _teststatus; }
        }
        public string TestName
        {
            get { return _testName; }
            set { _testName = value; }
        }
        public string Description
        {
            get { return _description; }
        }
        public string Author
        {
            get { return _author; }
        }
        public TestType TestType
        {
            get { return _testType; }
            set { _testType = value; }
        }
    }

    public enum Priority
    {
        Pri0 = 1,
        Pri1 = 2,
        Pri2 = 4,
        Pri3 = 8,
        FIT = 16,
        PriAll = Pri0 | Pri1 | Pri2 | Pri3 | FIT
    }
    /// ---------------------------------------
    /// <summary>
    /// States the status of a test
    /// </summary>
    /// ---------------------------------------
    public enum TestStatus
    {
        /// <summary>Test works</summary>
        Works = 1,
        /// <summary>Under development</summary>
        WorkingOn = 2,
        /// <summary>Problem with the test</summary>
        Problem = 3,
        /// <summary>Bug entered on the test</summary>
        BugEntered = 4,
        /// <summary>Test is blocked</summary>
        Blocked = 5,
    }

    /// ---------------------------------------
    /// <summary>
    /// Determines what type of test this is
    /// </summary>
    /// ---------------------------------------
    public enum TestType
    {
        Scenario,
        Generic
    }
}
