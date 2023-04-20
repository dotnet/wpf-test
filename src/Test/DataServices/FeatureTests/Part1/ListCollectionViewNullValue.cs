// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows.Data;
using System.Collections.Specialized;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System;
using System.Globalization;
using System.Data;
using System.ComponentModel;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where ListCollectionView sorting with dotted property paths fails on null values�������������
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "ListCollectionViewNullValue")]
    public class ListCollectionViewNullValue : AvalonTest
    {
        #region Constructors

        public ListCollectionViewNullValue()
            : base()
        {
            InitializeSteps += new TestStep(Validate);

        }

        #endregion

        #region Private Members


        private TestResult Validate()
        {
            // Incase of a regression adding a sort description will cause a InvalidOperationException.
            List<TestClass> list = new List<TestClass>();
            list.Add(new TestClass() { Field = new FieldClass() { Value = "string" } });
            list.Add(new TestClass());
            var view = CollectionViewSource.GetDefaultView(list);
            view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Field.Value", System.ComponentModel.ListSortDirection.Ascending));
            

            return TestResult.Pass;
        }

        #endregion

    }

    #region Helper Classes

    public class TestClass
    {
        public FieldClass Field { get; set; }
    }

    public class FieldClass
    {
        public string Value { get; set; }
    }

    #endregion
}