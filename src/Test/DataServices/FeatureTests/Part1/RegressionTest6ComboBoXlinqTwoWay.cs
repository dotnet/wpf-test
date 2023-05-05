// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.DataServices.RegressionTest6;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using System;
using System.IO;
using System.Xml.Linq;
// DataGrid Helpers
using LocalClasses = Microsoft.Test.DataServices.RegressionTest3;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage - ComboBox and DataGridComboBoxColumn two-way data binding broken in .NET 4 with XLinq for the SelectedItem/SelectedItemBindings property
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "RegressionTest6ComboBoXlinqTwoWay", SecurityLevel = TestCaseSecurityLevel.FullTrust, Versions="4.0GDR+,4.0GDRClient+" )]
    public class RegressionTest6ComboBoXlinqTwoWay : XamlTest
    {
        #region Private Data

        DataGridComboBoxColumn _myDataGridComboColumn;
        private DataGrid _myDataGrid;
        private XElement Doc { get; set; }

        private string _priorXml;
        private string _element2Name;
        private string _element2ComboValue;

        #endregion

        #region Constructors

        public RegressionTest6ComboBoXlinqTwoWay() : base(@"RegressionTest6ComboBoXlinqTwoWay.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myDataGrid = (DataGrid)RootElement.FindName("myDataGrid");
            _myDataGridComboColumn = (DataGridComboBoxColumn)RootElement.FindName("myDataGridComboColumn");

            if ((_myDataGrid == null) || (_myDataGridComboColumn == null))
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }
            StringReader inputXml = new StringReader("<nexus_file><charsets><charset name=\"name1\" SampleEnumValue=\"DefaultValue\" /><charset name=\"name2\" SampleEnumValue=\"DefaultValue\" /></charsets></nexus_file>");
            Doc = XElement.Load(inputXml);
            _myDataGrid.DataContext = Doc;

            return TestResult.Pass;
        }

        private TestResult Validate()
        {
            LogComment("Beginning validation: Ensuring that DataGrid ComboBox 2-way binding propagates back to XLinq source.");

            WaitForPriority(DispatcherPriority.SystemIdle);
            List<XNode> nodes = new List<XNode>(((XElement)Doc.FirstNode).Nodes());
            _priorXml = Doc.ToString();
            _element2ComboValue = ((XElement)nodes[1]).Attribute("SampleEnumValue").Value;
            _element2Name = ((XElement)nodes[1]).Attribute("name").Value;

            if ((_element2ComboValue == "DefaultValue") && (_element2Name == "name2"))
            {
                LogComment("Default values for document as expected (" + Doc.ToString() + "), proceeding with test");
            }
            else
            {
                LogComment("Default values for document not as expected (" + Doc.ToString() + "), aborting test");
                return TestResult.Fail;
            }

            _myDataGrid.SelectionMode = DataGridSelectionMode.Single;
            _myDataGrid.SelectionUnit = DataGridSelectionUnit.CellOrRowHeader;
            _myDataGrid.CurrentCell = new DataGridCellInfo(nodes[1], _myDataGrid.Columns[1]);
            LocalClasses.DataGridHelpers.WaitTillQueueItemsProcessed();

            // Change selected item on the combobox.  If regression happens, this will not propagate back to Doc.
            _myDataGrid.BeginEdit();
            LocalClasses.DataGridHelpers.WaitTillQueueItemsProcessed();
            var row = _myDataGrid.ItemContainerGenerator.ContainerFromIndex(0);
            var cell = LocalClasses.DataGridHelpers.GetCell(_myDataGrid, 1, 1);
            var comboBox = LocalClasses.DataGridHelpers.GetVisualChild<ComboBox>(cell);
            LocalClasses.DataGridHelpers.WaitTillQueueItemsProcessed();
            if (comboBox != null)
            {
                comboBox.SelectedIndex = 4;
            }
            LocalClasses.DataGridHelpers.WaitTillQueueItemsProcessed();
            _myDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            // Change name string item from the other Column.  This should always work.
            _myDataGrid.CurrentCell = new DataGridCellInfo(nodes[1], _myDataGrid.Columns[0]);
            _myDataGrid.BeginEdit();
            LocalClasses.DataGridHelpers.WaitTillQueueItemsProcessed();
            row = _myDataGrid.ItemContainerGenerator.ContainerFromIndex(0);
            cell = LocalClasses.DataGridHelpers.GetCell(_myDataGrid, 1, 0);
            var textBox = LocalClasses.DataGridHelpers.GetVisualChild<TextBox>(cell);
            LocalClasses.DataGridHelpers.WaitTillQueueItemsProcessed();
            if (textBox != null)
            {
                textBox.Text = "new name";
            }
            LocalClasses.DataGridHelpers.WaitTillQueueItemsProcessed();
            _myDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            WaitForPriority(DispatcherPriority.SystemIdle);

            if ((_element2ComboValue == ((XElement)nodes[1]).Attribute("SampleEnumValue").Value) ||
                 (_element2Name == ((XElement)nodes[1]).Attribute("name").Value))
            {
                LogComment("Error: Expected all values to be changed: Prior = \n\n" + _priorXml + "\n\n and Current = \n\n" + Doc.ToString());
                return TestResult.Fail;
            }
            else
            {
                LogComment("Success: Changing values in ComboBox propagated back to XLinq ELement \n Prior = \n\n" + _priorXml + "\n\n and Current = \n\n" + Doc.ToString());
                return TestResult.Pass;
            }
        }
        #endregion
    }
}

namespace Microsoft.Test.DataServices.RegressionTest6
{

    public class SampleEnumValue
    {
        public string Name { get; set; }
    }

    public class SampleEnumValues
    {
        private static List<SampleEnumValue> s_values = new List<SampleEnumValue>();
        static SampleEnumValues()
        {
            var names = new[] {
                "DefaultValue",
                "Value1",
                "Value2",
                "Value3",
                "ChangedTestValue"
               };

            foreach (var name in names)
            {
                s_values.Add(new SampleEnumValue { Name = name });
            }
        }
        public static List<SampleEnumValue> StaticList { get { return s_values; } }
        public List<SampleEnumValue> ModelList { get { return s_values; } }
    }

    [ValueConversion(typeof(string), typeof(SampleEnumValue))]
    public class ComboConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return SampleEnumValues.StaticList.Find(m => m.Name == (string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((SampleEnumValue)value).Name;
        }
    }

}
