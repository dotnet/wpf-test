// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using LocalClasses = Microsoft.Test.DataServices.RegressionTest3;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage- Two-Way binding on indexed properties in DataGrid does not work
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "RegressionTest3DataGridTwoWayBinding", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class RegressionTest3DataGridTwoWayBinding : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private DataGrid _myDataGrid;

        #endregion

        #region Constructors

        public RegressionTest3DataGridTwoWayBinding() : base(@"RegressionTest3DataGridTwoWayBinding.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel)RootElement.FindName("myStackPanel");
            _myDataGrid = (DataGrid)RootElement.FindName("myDataGrid");

            if (_myDataGrid == null || _myStackPanel == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult Validate()
        {
            LogComment("Beginning validation: Ensuring that default validation does not show error in initial state.");

            LocalClasses.PersonRepository myRepository = (LocalClasses.PersonRepository)_myStackPanel.FindResource("Repository");

            _myDataGrid.SelectionMode = DataGridSelectionMode.Single;
            _myDataGrid.SelectionUnit = DataGridSelectionUnit.CellOrRowHeader;
            _myDataGrid.CurrentCell = new DataGridCellInfo(myRepository.Persons[1], _myDataGrid.Columns[0]);
            LocalClasses.DataGridHelpers.WaitTillQueueItemsProcessed();

            LogComment("Setters called before DataGrid editing:");
            foreach (LocalClasses.Person someGuy in myRepository.Persons)
            {
                LogComment(someGuy.Name + " was set " + someGuy.SetterCalls + "x");
            }

            _myDataGrid.BeginEdit();
            LocalClasses.DataGridHelpers.WaitTillQueueItemsProcessed();

            var row = _myDataGrid.ItemContainerGenerator.ContainerFromIndex(0);
            var cell = LocalClasses.DataGridHelpers.GetCell(_myDataGrid, 1, 0);


            var textBox = LocalClasses.DataGridHelpers.GetVisualChild<TextBox>(cell);
            LocalClasses.DataGridHelpers.WaitTillQueueItemsProcessed();

            textBox.Text = "New Value";
            LocalClasses.DataGridHelpers.WaitTillQueueItemsProcessed();

            _myDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            LocalClasses.DataGridHelpers.WaitTillQueueItemsProcessed();

            LogComment("Setters called after DataGrid editing: (2nd should now be named \"New Value\"");
            foreach (LocalClasses.Person someGuy in myRepository.Persons)
            {
                LogComment(someGuy.Name + " was set " + someGuy.SetterCalls + "x");
            }

            if (((LocalClasses.Person)myRepository.Persons[1]).SetterCalls > 0)
            {
                LogComment("Success, two-way binding with indexed data source and DataGrid resulted in property setter getting called");
                return TestResult.Pass;
            }
            else
            {
                LogComment("Error, two-way binding with indexed data source and DataGrid failed to cause property setter to get called");
                return TestResult.Fail;
            }
        }

        #endregion
    }
}

// Useful classes copied almost verbatim from the repro
namespace Microsoft.Test.DataServices.RegressionTest3
{
    public static class DataGridHelpers
    {
        public static DataGridCell GetCell(DataGrid dataGrid, int row, int column)
        {
            DataGridRow rowContainer = GetRow(dataGrid, row);
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                // try to get the cell but it may possibly be virtualized
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    // now try to bring into view and retreive the cell
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        public static DataGridRow GetRow(DataGrid dataGrid, int index)
        {
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // may be virtualized, bring into view and try again
                dataGrid.ScrollIntoView(dataGrid.Items[index]);
                dataGrid.UpdateLayout();
                row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public static void WaitTillQueueItemsProcessed()
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(
                delegate(object arg)
                {
                    frame.Continue = false;
                    return null;
                }), null);

            // Keep the thread busy processing events until the timeout has expired.
            Dispatcher.PushFrame(frame);
        }

        public static T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int index = 0; index < numVisuals; index++)
            {
                var visualChild = VisualTreeHelper.GetChild(parent, index);
                child = visualChild as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(visualChild);
                }
                if (child != null)
                {
                    break;
                }
            }

            return child;
        }

        public static T GetVisualChild<T>(Visual parent, int index) where T : Visual
        {
            T child = default(T);

            int encounter = 0;
            Queue<Visual> queue = new Queue<Visual>();
            queue.Enqueue(parent);
            while (queue.Count > 0)
            {
                Visual visualChild = queue.Dequeue();
                child = visualChild as T;
                if (child != null)
                {
                    if (encounter == index)
                        break;
                    encounter++;
                }
                else
                {
                    int numVisuals = VisualTreeHelper.GetChildrenCount(visualChild);
                    for (int count = 0; count < numVisuals; count++)
                    {
                        queue.Enqueue((Visual)VisualTreeHelper.GetChild(visualChild, count));
                    }
                }
            }

            return child;
        }

    }

    public class Person
    {
        public string Name { get; set; }

        public int SetterCalls = 0;

        public string this[long index]
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
                SetterCalls++;
            }
        }
    }

    public class PersonRepository
    {
        private List<Person> _persons;
        public int SetterCalls = 0;

        public List<Person> Persons
        {
            get
            {
                return _persons;
            }

            set
            {                
                SetterCalls++;
                this._persons = value;                
            }
        }

        public PersonRepository()
        {
            List<Person> returnValue = new List<Person>();
            returnValue.Add(new Person() { Name = "Filip" });
            returnValue.Add(new Person() { Name = "Davy" });
            _persons = returnValue;
        }
    }

    public class IndexProperty
    {
        private string _myString = "value";

        public string this[long index]
        {
            get
            {
                return _myString;
            }
            set
            {
                _myString = value;
            }
        }
    }
}
