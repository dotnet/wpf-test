// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression Test - Using CollectionViewSource.Filter leaks the entire window
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>            
    [Test(1, "Collections", "RegressionCollectionViewSourceFilterLeaksWindow", SecurityLevel=TestCaseSecurityLevel.FullTrust)]
    public class RegressionCollectionViewSourceFilterLeaksWindow : WindowTest
    {
        #region Private Data

        private MethodInfo _cleanupMethodInfo;
        private static bool s_filterCalled;

        #endregion

        #region Constructors

        public RegressionCollectionViewSourceFilterLeaksWindow()            
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(Verify);
        }

        #endregion

        #region Private Members

        private TestResult Initialize()
        {
            _cleanupMethodInfo = typeof(BindingOperations).GetMethod("Cleanup", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

            Window windowToClose = new Window();
            StackPanel stackPanel = new StackPanel();
            ComboBox filteredComboBox = new ComboBox();
            windowToClose.Content = stackPanel;

            CollectionViewSource cvs = new CollectionViewSource();
            cvs.Source = ContainerClass.PlacesCollection;
            cvs.Filter += new FilterEventHandler(ComboBoxFilter);
            filteredComboBox.ItemsSource = cvs.View;  
            
            windowToClose.Show();
            windowToClose.Close();

            return TestResult.Pass;
        }

        private TestResult Verify()
        {
            Cleanup();

            s_filterCalled = false;

            ContainerClass.AddPlace();

            if (!s_filterCalled)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        private void Cleanup()
        {
            GetMemory();
        }

        private void GetMemory()
        {
            long result = GC.GetTotalMemory(true);
            GC.WaitForPendingFinalizers();
            while ((bool)_cleanupMethodInfo.Invoke(null, null))
            {
                result = GC.GetTotalMemory(true);
                GC.WaitForPendingFinalizers();
            }
            return;
        }

        private void ComboBoxFilter(object sender, FilterEventArgs e)
        {
            s_filterCalled = true;
            Place place = (Place)e.Item;
            e.Accepted = place.State == "WA";
        }

        private class ContainerClass
        {
            public static Places PlacesCollection;

            static ContainerClass()
            {
                PlacesCollection = new Places();
            }

            public static void AddPlace()
            {
                PlacesCollection.Add(new Place("Mercer Island", "WA"));
            }
        }

        #endregion
    }
}
