// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Threading;
using System.Windows.Threading;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage - CollectionViews leak, after CollectionViewSource is discarded, found by opening/closing files
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "RegressionTest16CollectionViewLeak", SecurityLevel=TestCaseSecurityLevel.FullTrust, Versions="4.0GDR+,4.0GDRClient+" )]
    public class RegressionTest16CollectionViewLeak : StepsTest
    {
        public RegressionTest16CollectionViewLeak()
        {
            this.RunSteps += new TestStep(CollectionViewSourceLeakTest);
            this.RunSteps += new TestStep(VerifyCollectionViewSourceLeakTest);
        }

        private TestResult CollectionViewSourceLeakTest()
        {
            // �  Create a CollectionViewSource in code
            LogComment("Creating CollectionViewSource...");
            CollectionViewSource collectionViewSource = new CollectionViewSource();

            // �  Set its Source, get its View, save a weak reference to the view
            string[] sourceStrings = new string[] { "foo", "bar", "baz" };
            _sampleSource = new ObservableCollection<string>(sourceStrings);
            collectionViewSource.Source = _sampleSource;
            ICollectionView myView = collectionViewSource.View;
            _weakRef = new WeakReference(myView);

            // �  Remove all references to the CVS
            sourceStrings = null;
            myView = null;
            collectionViewSource = null;
            return TestResult.Pass;
        }

        private TestResult VerifyCollectionViewSourceLeakTest()
        {

            // � Force a memory purge.
            ForceMemoryPurge();

            // �  Verify that the weak reference now is dead
            // Access SampleSource down here so that it is not aggressively GC'ed earlier.
            int unused = _sampleSource.Count;

            if (_weakRef.IsAlive)
            {
                LogComment("ERROR: Weak Reference to CollectionViewSource.View is still alive. Regression");
                return TestResult.Fail;
            }
            else
            {
                LogComment("PASS: Weak Reference to CollectionViewSource.View got cleaned up");
                return TestResult.Pass;
            }
        }

        private void ForceMemoryPurge()
        {
            LogComment("Forcing BindingOperations memory cleanup...");
            MethodInfo miCleanup = typeof(BindingOperations).GetMethod("Cleanup", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            do
            {
                GC.GetTotalMemory(true);
                GC.WaitForPendingFinalizers();
            } while ((bool)miCleanup.Invoke(null, null));
        }

        ObservableCollection<string> _sampleSource;
        WeakReference _weakRef;
    }
}
