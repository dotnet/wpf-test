// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for data-binding of TextBox controls.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/TextBoxOM/Data.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;    
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;        
    using System.Windows.Media;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;    

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
       
    #endregion Namespaces.

    /// <summary>
    /// Data model suitable for simple (databinding) test cases.
    /// </summary>
    public class TestDataModel: INotifyPropertyChanged
    {
        #region Public methods.
        /// <summary>
        /// Constructor which takes a string as argument
        /// </summary>
        /// <param name="inputValue"></param>
        public TestDataModel(string inputValue)
        {
            _value = inputValue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TestDataModel()
        {
            _value = "";
        }

        #endregion Public methods.

        #region Public properties.

        /// <summary>
        /// Provides the current value of the data model.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Value"));
                }
            }
        }

        #endregion Public properties.

        #region INotifyPropertyChanged Members

        /// <summary>Fires when a property is changed.</summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members

        #region Private fields.

        /// <summary>Current value for the model.</summary>
        private string _value = "";

        #endregion Private fields.
    }

    /// <summary>
    /// Verifies that the Text property can be bound and updated.
    /// </summary>
    [Test(0, "TextBox", "TextBoxBindTextSimple", MethodParameters = "/TestCaseType=TextBoxBindTextSimple", Timeout=120)]
    [TestOwner("Microsoft"), TestTactics("632"), TestBugs("782,783,565,784")]
    public class TextBoxBindTextSimple: TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            SetTextBoxProperties(TestTextBox);

            CheckEmptySelection();

            Log("Setting up data model...");
            _model = new TestDataModel("Initial Value");
            MainWindow.DataContext = _model;
            Binding bind = new Binding("Value");
            bind.Mode = BindingMode.TwoWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            _binding = TestTextBox.SetBinding(TextBox.TextProperty, bind);

            // Force an update.
            _binding.UpdateTarget();

            // The above may fail due to Regression_Bug565. Post an item to work
            // around the synchronous update failure.
            QueueDelegate(WorkaroundRegression_Bug565);
        }

        private void WorkaroundRegression_Bug565()
        {
            CheckDataModel();
            CheckEmptySelection();

            _beforeChange = BitmapCapture.CreateBitmapFromElement(TestTextBox);

            // Update asynchronously.
            _model.Value = "Modified Value";
            QueueDelegate(DoUpdate);
        }

        private void DoUpdate()
        {
            System.Drawing.Bitmap afterChange;
            System.Drawing.Bitmap differences;

            Log("Data model updated");
            CheckDataModel();
            CheckEmptySelection();

            Log("Verifying that rendering has changed...");
            afterChange = BitmapCapture.CreateBitmapFromElement(TestTextBox);
            if (ComparisonOperationUtils.AreBitmapsEqual(
                _beforeChange, afterChange, out differences))
            {
                Logger.Current.LogImage(_beforeChange, "BeforeChange");
                throw new Exception("No changes found after databinding update.");
            }

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        private void CheckDataModel()
        {
            Log("Current TextBox.Text value:        " + TestTextBox.Text);
            Log("Current TestDataModel.Value value: " + _model.Value);
            Verifier.Verify(TestTextBox.Text == _model.Value);
        }

        /// <summary>
        /// Verifies that there is no selection.
        /// </summary>
        private void CheckEmptySelection()
        {
            Log("Checking that selection is empty.");
            Log("TextBox.SelectedText value:        " + TestTextBox.SelectedText);
            Verifier.Verify(TestTextBox.SelectedText.Length == 0,
                "Selection is empty.", false);
        }

        #endregion Verifications.

        #region Private fields.

        private BindingExpressionBase _binding;
        private TestDataModel _model;
        private System.Drawing.Bitmap _beforeChange;

        #endregion Private fields.
    }

    /// <summary>
    /// Customer data class model used for Testing
    /// </summary>
    public class Customer : INotifyPropertyChanged
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the customer</param>
        /// <param name="age">Age of the customer</param>
        public Customer(string name, int age)
        {
            _name = name;
            _age = age;
        }

        /// <summary>
        /// Default constructor. Name is set to "xxx" and Age is set to 0
        /// </summary>
        public Customer()
        {
            _name = "xxx";
            _age = 0;
        }
        #endregion

        #region Public Properties
        /// <summary>Name of the Customer</summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        /// <summary>Age of the Customer</summary>
        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
                NotifyPropertyChanged("Age");
            }
        }
        #endregion

        /// <summary>PropertyChangedEventHandler</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #region Private Members
        private string _name;
        private int _age;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info)); ;
            }
        }
        #endregion
    }

    /// <summary>
    /// Class which has all the customer information.
    /// </summary>
    public class CustomerData : ObservableCollection<Customer>
    {
        /// <summary>Constructor</summary>
        public CustomerData()
            : base()
        {
            Add(c1);
            Add(c2);
        }

        /// <summary>Default customer entries for the class</summary>
        public Customer c1 = new Customer("Adam", 25);

        /// <summary>
        /// Default customer entries for the class</summary>
        public Customer c2 = new Customer("Eve", 25);
    }

    /// <summary>
    /// Verifies that TextBox can be used in repeater context like ListBox to edit
    /// instances in a collection and data bindabled.
    /// </summary>
    [Test(0, "TextBox", "TBDataBindingTest", MethodParameters = "/TestCaseType=TBDataBindingTest")]
    [TestOwner("Microsoft"), TestTactics("631"), TestWorkItem("123"), TestBugs("65")]
    public class TBDataBindingTest : CustomTestCase
    {
        #region Private Members
        private DockPanel _container;
        private ListBox _listBox;
        CustomerData _customerData;
        #endregion

        #region Setup
        private void SetUpTestCase()
        {
            _listBox = new ListBox();

            // Setting up the ItemsSource for binding
            _customerData = new CustomerData();
            _listBox.ItemsSource = _customerData;

            // Setting up style for Items
            DataTemplate listBoxItemTemplate = new DataTemplate();
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBox));
            fef.SetValue(TextBox.ForegroundProperty, Brushes.Blue);
            listBoxItemTemplate.VisualTree = fef;
            Binding bindTB = new Binding("Name");
            bindTB.Mode = BindingMode.TwoWay;
            bindTB.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            fef.SetBinding(TextBox.TextProperty, bindTB);

            _listBox.ItemTemplate = listBoxItemTemplate;
        }

        private void AddToTree()
        {
            //Adding the testElement to the tree
            //testElement.Parent = null;
            _container.Children.Clear();
            _container.Children.Add(_listBox);

            //_container.Parent = null;
            MainWindow.Content = null;
            MainWindow.Content = _container;
        }
        #endregion

        #region Main flow.
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _container = new DockPanel();
            DoTest();
        }

        private void DoTest()
        {
            SetUpTestCase();
            AddToTree();
            QueueDelegate(StartTesting);
        }

        private void StartTesting()
        {
            Log("Verifying binding after setup");
            VerifyBinding();

            Log("Updating data through CustomerData instance");
            UpdateTestData();

            Log("Verifying binding after updating the CustomerData instance");
            VerifyBinding();

            Log("Updating data through UI TextBox");
            QueueDelegate(UpdateTextBoxData);
        }

        private void UpdateTestData()
        {
            ((Customer)_customerData[0]).Name = "Adam++";
        }

        private void UpdateTextBoxData()
        {
            Visual listBoxVisual = (Visual)_listBox;
            Visual[] listBoxVisuals = XPathNavigatorUtils.ListVisuals(listBoxVisual, "//TextBox");

            TextBox tb = listBoxVisuals[0] as TextBox;
            MouseInput.MouseClick(tb);
            KeyboardInput.TypeString("^a{DELETE}Adam--");
            QueueDelegate(EndTesting);
        }

        private void EndTesting()
        {
            Log("Verifying binding after updating through UI TextBox");
            VerifyBinding();
            Logger.Current.ReportSuccess();
        }
        #endregion

        #region Verification
        private void VerifyBinding()
        {
            Visual listBoxVisual = (Visual)_listBox;
            Visual[] listBoxVisuals = XPathNavigatorUtils.ListVisuals(listBoxVisual, "//TextBox");

            for(int i =0; i < listBoxVisuals.Length; i++)
            {
                string logStatement;
                TextBox tb = listBoxVisuals[i] as TextBox;
                logStatement = "Verifying Binded Data: ";
                logStatement = "TextBox data [" + tb.Text + "] Customer data [" + ((Customer)_customerData[i]).Name + "]";
                Verifier.Verify(tb.Text == ((Customer)_customerData[i]).Name, logStatement, true);
            }
        }
        #endregion
    }

    /// <summary>
    /// Verifies that the Text property can be bound with
    /// interesting string values and updates both ways.
    /// </summary>
    [Test(2, "TextBox", "TextBoxTextBindTest", MethodParameters = "/TestCaseType=TextBoxTextBindTest", Timeout=120)]
    [TestOwner("Microsoft"), TestTactics("630"), TestWorkItem("123"), TestBugs("653")]
    public class TextBoxTextBindTest : CustomTestCase
    {
        #region PrivateMembers

        /// <summary>Combinatorial engine for values.</summary>
        private CombinatorialEngine _combinatorials;

        /// <summary>String to be tested.</summary>
        private string _testString;
        private TextBox _testTB;
        private StackPanel _container;

        private TestDataModel _model;
        #endregion

        #region MainFlow
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Dimension[] dimensions; // Dimensions for combinations.

            dimensions = new Dimension[] {
                new Dimension("BindContent", StringData.Values)
            };
            _combinatorials = CombinatorialEngine.FromDimensions(dimensions);

            _container = new StackPanel();
            MainWindow.Content = _container;

            QueueDelegate(RunNextTestCase);
        }

        private void RunNextTestCase()
        {
            Hashtable combinationValues;   // Values for combination.

            combinationValues = new Hashtable();
            if (_combinatorials.Next(combinationValues))
            {
                StringData stringData;
                stringData = combinationValues["BindContent"] as StringData;
                _testString = stringData.Value;
                if (!stringData.IsLong) //ignore null and long strings
                {
                    QueueDelegate(SetUpTestCase);
                }
                else
                {
                    QueueDelegate(RunNextTestCase);
                }
            }
            else
            {
                Logger.Current.ReportSuccess();
                return;
            }
        }

        private void SetUpTestCase()
        {
            //hook up the control
            _container.Children.Clear();
            _testTB = new TextBox();
            _container.Children.Add(_testTB);

            //Set up the binding
            Log("Setting up data model...");
            _model = new TestDataModel(_testString);
            _testTB.DataContext = _model;
            Binding bind = new Binding("Value");
            bind.Mode = BindingMode.TwoWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            _testTB.SetBinding(TextBox.TextProperty, bind);

            QueueDelegate(ChangeBindContents);
        }

        private void ChangeBindContents()
        {
            Log("Verify binding before any changes");
            CheckBinding();

            //Update the contents through TestDataModel
            _model.Value = _model.Value + _model.Value;
            _testTB.Focus();
            QueueDelegate(ChangeTextBoxContents);
        }

        private void ChangeTextBoxContents()
        {
            Log("Verify binding after TestDataModel instance is changed");
            CheckBinding();

            //Update the contents through the TextBox
            KeyboardInput.TypeString("^{END}Test");
            QueueDelegate(VerifyBinding);
        }

        private void VerifyBinding()
        {
            Log("Verifying binding after TextBox contents are edited");
            CheckBinding();

            QueueDelegate(RunNextTestCase);
        }

        private void CheckBinding()
        {
            Log("Current TextBox.Text value:        " + _testTB.Text);
            Log("Current TestDataModel.Value value: " + _model.Value);
            if (_model.Value != null)
            {
                Verifier.Verify(_testTB.Text == _model.Value);
            }
            else
            {
                Verifier.Verify(_testTB.Text == string.Empty);
            }
        }
        #endregion MainFlow
    }
}
