// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where IEditableCollectionView - CommitEdit() throws exception
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "IECVCommitEdit", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
    public class IECVCommitEdit : AvalonTest
    {
        #region Constructors

        public IECVCommitEdit()
        {
            InitializeSteps += new TestStep(Validate);                        
        }

        #endregion

        #region Private Members
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);         
            
            ObservableCollection<Person> people = new ObservableCollection<Person>();
            people.Add(new Person("Person #1", "Canadian"));
            ListCollectionView cvPeople = new ListCollectionView(people);
            cvPeople.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            cvPeople.Filter = (o) => { return false; };
            cvPeople.EditItem(people[0]);
            people[0].Name = "new name";
            bool isEditing = cvPeople.IsEditingItem;
            
            // Verify if item in view is being edited.
            if (!isEditing)
            {
                LogComment("Item is not in view, is Editing should return false.");
                return TestResult.Fail;
            }

            // Should not throw an exception.
            try
            {
                cvPeople.CommitEdit();
            }
            catch (Exception ex)
            {
                LogComment("Unexpected Exception thrown. " + ex.Message.ToString());
                return TestResult.Fail;
            }            

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes
    
    #endregion
}
