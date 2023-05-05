// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// This tests bindings using indexers in Markup. 3 situations are tested: when the data source takes an
    /// integer as the indexer, a string and a multi indexer. 
    /// </description>
    /// </summary>
    [Test(0, "Binding", "IndexersInMarkup")]
    public class IndexersInMarkup : XamlTest 
	{
        HappyMan _happy1;
        HappyMan _happy2;
        HappyMan _happy3;

		public IndexersInMarkup()
			    : base(@"indexers.xaml")
		{
            InitializeSteps += new TestStep(Init);
            RunSteps += new TestStep(VerifyIndexes);
        }

    
        TestResult Init()
        {
             _happy1 = LogicalTreeHelper.FindLogicalNode(RootElement, "HappyPersonOne") as HappyMan;
             _happy2 = LogicalTreeHelper.FindLogicalNode(RootElement, "HappyPersonTwo") as HappyMan;
             _happy3 = LogicalTreeHelper.FindLogicalNode(RootElement, "HappyPersonThree") as HappyMan;

            if (_happy1 == null)
            {
                LogComment("HappyMan1 was null!");
                return TestResult.Fail;
            }

            if (_happy2 == null)
            {
                LogComment("HappyMan2 was null!");
                return TestResult.Fail;
            }
            if (_happy3 == null)
            {
                LogComment("HappyMan3 was null!");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        TestResult VerifyIndexes()
        {
            if (_happy1.HappyName != "Grumpy")
            {
                LogComment("Indexing into collection failed!  'Buddies[(sys:Int32)2].Name' ");
                return TestResult.Fail;
            }
            if (_happy2.HappyName != "Bashful")
            {
                LogComment("Multiple Indexer into collection failed!  'Buddies[Bashful, (sys:String)Purple].Name' ");
                return TestResult.Fail;
            }

            if (_happy2.SkinColor != Colors.DeepPink)
            {
                LogComment("Multiple Indexer into collection failed!  'Buddies[Dopey, (media:Color)DeepPink].SkinColor' ");
                return TestResult.Fail;
            }

            // This will find the wrong indexer, expect an error
            if (_happy3.HappyName != "ExpectedErr")
            {
                LogComment("Multiple Indexer into collection succeeded when failure was expected!  'Buddies[Bashful, Purple].Name' ");
                return TestResult.Fail;
            }

            // This will find the first indexer so need to cast
            if (_happy3.SkinColor != Colors.Salmon)
            {
                LogComment("Multiple Indexer into collection failed!  'Buddies[Dopey, Salmon].SkinColor' ");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }
    }
}
