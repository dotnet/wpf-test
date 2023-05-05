// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Data;
using System.Windows.Data;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression Test - Creating a CollectionView from an empty dataset causes a crash in .net 3.5 on vista
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>            
    [Test(1, "Views", "RegressionEmptyDataSetCollectionViewException")]
    public class RegressionEmptyDataSetCollectionViewException : WindowTest
    {
        #region Public Members

        public RegressionEmptyDataSetCollectionViewException()            
        {
            RunSteps += new TestStep(Verify);            
        }

        public TestResult Verify()
        {
            DataView emptyDataSet = new DataView();
            CollectionView myCollectionView = new CollectionView(emptyDataSet);

            // If we get here than it means creating a collection view from an
            // empty dataset didn't crash.
            return TestResult.Pass;
        }

        #endregion
    }
}
