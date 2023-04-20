// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Collections.Generic;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression Test : This test passes if there are no invariant asserts, otherwise it fails
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "MasterDetailInvariantAssert")]
    public class MasterDetailInvariantAssert : XamlTest
    {
        #region Private Data

        DataSet _sampleData;
        int _departmentID = 1;
        int _employeeID = 1;

        #endregion

        #region Constructors

        public MasterDetailInvariantAssert()
            : base(@"MasterDetailInvariantAssert.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _sampleData = new DataSet();
            DataTable departmentTable = _sampleData.Tables.Add("Departments");
            departmentTable.Columns.Add("DepartmentID", typeof(int));
            departmentTable.Columns.Add("DepartmentName", typeof(string));

            DataTable employeeTable = _sampleData.Tables.Add("Employees");
            employeeTable.Columns.Add("EmployeeID", typeof(int));
            employeeTable.Columns.Add("EmployeeName", typeof(string));
            employeeTable.Columns.Add("DepartmentID", typeof(int));

            _sampleData.Relations.Add("EmployeeDepartmentRelation", departmentTable.Columns["DepartmentID"], employeeTable.Columns["DepartmentID"], true);

            departmentTable.Rows.Add(_departmentID, "Department " + _departmentID++);
            departmentTable.Rows.Add(_departmentID, "Department " + _departmentID++);

            employeeTable.Rows.Add(_employeeID, "Employee " + _employeeID++, 1);
            employeeTable.Rows.Add(_employeeID, "Employee " + _employeeID++, 1);
            employeeTable.Rows.Add(_employeeID, "Employee " + _employeeID++, 2);

            Window.DataContext = _sampleData.Tables["Departments"];

            return TestResult.Pass;
        }

        private TestResult Validate()
        {
            foreach (DataRow row in _sampleData.Tables["Departments"].Rows)
            {
                row.BeginEdit();
                row[0] = ((int)row[0]) + 100;
                row[1] = row[1];
                row.EndEdit();  // ExecutionEngineException happens when PropertyChanged event with ProperyName=null or "" is raised.
            }
            _departmentID = 1;
            _sampleData.AcceptChanges();
            _sampleData.AcceptChanges();

            return TestResult.Pass;
        }

        #endregion
    }
}

