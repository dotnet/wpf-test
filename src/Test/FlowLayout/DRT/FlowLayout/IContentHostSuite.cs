// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Basic support for Text DRTs. Created for testing IContentHost methods using the TextView suite model.
//

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Text test suite base class.
    // ----------------------------------------------------------------------
    internal abstract class IContentHostSuite : DrtTestSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        protected IContentHostSuite(string suiteName)
            : base(suiteName)
        {
        }

        // ------------------------------------------------------------------
        // Initialize tests.
        // ------------------------------------------------------------------
        public override DrtTest[] PrepareTests()
        {
            // Initialize the suite here. This includes loading the tree.
            DRT.Show(CreateTree());
            InitializeTests();
            DRT.Assert(_tests != null && _tests.Length > 0, this.Name + ": Tests are not initialized.");

            // Return the lists of tests to run against the tree
            DrtTest[] tests = new DrtTest[_tests.Length];
            for (int i = 0; i < tests.Length; i++)
            {
                tests[i] = new DrtTest(NextTest);
            }
            return tests;
        }

        // ------------------------------------------------------------------
        // Create initial element tree.
        // ------------------------------------------------------------------
        protected abstract UIElement CreateTree();

        // ------------------------------------------------------------------
        // Initialize tests.
        // ------------------------------------------------------------------
        protected abstract void InitializeTests();

        // ------------------------------------------------------------------
        // Run next test.
        // ------------------------------------------------------------------
        private void NextTest()
        {
            DrtTestDetails test = _tests[_currentTest];

            // Create Tree and wait for render
            if (test.Load != null)
            {
                test.Load();
                ((UIElement)DRT.RootElement).UpdateLayout();
                DRT.WaitForCompleteRender();
            }

            // Run custom tests
            if (test.Run != null)
            {
                test.Run();
            }

            ++_currentTest;
        }

        protected string TestName
        {
            get { return this.Name + (_currentTest < _tests.Length ? _tests[_currentTest].Name : "#None#"); }
        }

        protected string DrtFilesDirectory
        {
            get { return DRT.BaseDirectory + "DrtFiles\\FlowLayout\\"; }
        }

        protected DrtTestDetails[] _tests;
        protected int _currentTest;
    }

    // ----------------------------------------------------------------------
    // Single DRT test details.
    // ----------------------------------------------------------------------
    internal class DrtTestDetails
    {
        // ------------------------------------------------------------------
        // Constructor.
        //
        // Params:
        //      name - name of the test
        //      cbkLoad - load test callback
        //      cbkRun - run test callback
        // ------------------------------------------------------------------
        internal DrtTestDetails(string name, LoadDelegate cbkLoad, RunDelegate cbkRun)
        {
            _name = name;
            _cbkRun = cbkRun;
            _cbkLoad = cbkLoad;
        }

        // ------------------------------------------------------------------
        // Delegate responsible for loading test case.
        // ------------------------------------------------------------------
        internal delegate void LoadDelegate();

        // ------------------------------------------------------------------
        // Delegate responsible for verification.
        // ------------------------------------------------------------------
        internal delegate void RunDelegate();

        // ------------------------------------------------------------------
        // Name of the test.
        // ------------------------------------------------------------------
        internal string Name { get { return _name; } }
        private string _name;

        // ------------------------------------------------------------------
        // Execute test case delegate.
        // ------------------------------------------------------------------
        internal RunDelegate Run { get { return _cbkRun; } }
        private RunDelegate _cbkRun;

        // ------------------------------------------------------------------
        // Load test case delegate.
        // ------------------------------------------------------------------
        internal LoadDelegate Load { get { return _cbkLoad; } }
        private LoadDelegate _cbkLoad;
    }
}
