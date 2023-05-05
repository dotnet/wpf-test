// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests PropertyPath bugs
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(2, "Binding", "RegressionPropertyPath")]
    public class RegressionPropertyPath : WindowTest
    {
        public RegressionPropertyPath()
        {
            RunSteps += new TestStep(PathWithNumericName);
        }

        // Regression Test - Binding cannot resolve path with numeric name
        private TestResult PathWithNumericName()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("123", typeof(double));
            DataRow dataRow = dataTable.NewRow();
            dataRow[0] = 11.111;
            dataTable.Rows.Add(dataRow);

            TextBlock tb = new TextBlock();
            tb.DataContext = dataTable;
            Binding b = new Binding("123");
            tb.SetBinding(TextBlock.TextProperty, b);

            if (!Object.Equals(tb.Text, "11.111")) return TestResult.Fail;

            return TestResult.Pass;
        }
    }
}
