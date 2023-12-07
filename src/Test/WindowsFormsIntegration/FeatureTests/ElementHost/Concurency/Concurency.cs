// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Forms.Integration;
using System.Data;
using System.Data.OleDb;
using System.ComponentModel;

//
// Testcase:    Concurency
// Description: Verify that concurency is maintained with a shared Datasource
//
public class Concurency : ReflectBase
{
    private ListBox _wflbProducts = null;
    private TextBox _wftbProductName = null;
    private TextBox _wftbUnitPrice = null;
    private System.Windows.Controls.Grid _avGrid = null;
    private System.Windows.Controls.ComboBox _avcbProducts = null;
    private System.Windows.Controls.ListView _avlvProducts = null;
    private System.Windows.Controls.TextBox _avtbUnitPrice = null;

    #region Testcase setup
    public Concurency(string[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        //create the controls 
        InitializeControls();

        for (int i = 0; i < p.ru.GetRange(5, 10); i++)
		{
		    Products.Instance.Add( new Product(p.ru.GetString(20), p.ru.GetRange(1, 100)));
        }

        //bind the controls
        _wflbProducts.DataSource = Products.Instance;
        _wflbProducts.DisplayMember = "ProductName";

        _wftbProductName.DataBindings.Add(new Binding("Text", Products.Instance, "ProductName"));
        _wftbUnitPrice.DataBindings.Add(new Binding("Text", Products.Instance, "UnitPrice"));

        _avGrid.DataContext = Products.Instance;
        _avlvProducts.ItemsSource = Products.Instance;
        ((System.Windows.Controls.GridView)_avlvProducts.View).Columns[0].DisplayMemberBinding = new System.Windows.Data.Binding("ProductName");
        ((System.Windows.Controls.GridView)_avlvProducts.View).Columns[1].DisplayMemberBinding = new System.Windows.Data.Binding("UnitPrice");

        _avcbProducts.DataContext = _avGrid.DataContext;
        _avcbProducts.ItemsSource = Products.Instance;
        _avcbProducts.DisplayMemberPath = "ProductName";

        System.Windows.Data.BindingOperations.SetBinding( _avtbUnitPrice,
                                                          System.Windows.Controls.TextBox.TextProperty,
                                                          new System.Windows.Data.Binding("UnitPrice"));
        
        base.InitTest(p);
    }

    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("Verify that changing a bound value in a simple WPF control is represented in a simple WF control. e.g. TextBox ")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        Application.DoEvents();

        //Select first row in the WPF listview and WF ListBox;
        _avlvProducts.Items.MoveCurrentToPosition(0);
        _wflbProducts.SelectedIndex = 0;
        Application.DoEvents();

        //change the price of the current product to a new value - use the WPF TextBox to do this change
        int newPrice = Convert.ToInt32(_avtbUnitPrice.Text) + 10;
        _avtbUnitPrice.Focus();
        _avtbUnitPrice.Text = newPrice.ToString();
        _avlvProducts.Focus();
        
        //verify that the new value appears in the WF text-box 
        sr.IncCounters( newPrice == Convert.ToInt32(_wftbUnitPrice.Text),
                        "The Price-change in WPF Text-Box wasn't reflected in the WF TextBox",
                        p.log);

        //double-ckeck: change the selected row in the WF ListBox
        _wflbProducts.SelectedIndex = 1;
        Application.DoEvents();
        
        //move back to first item and verify that the new price was preserved in the text-box
        _wflbProducts.SelectedIndex = 0;
        Application.DoEvents();

        sr.IncCounters(newPrice == Convert.ToInt32(_wftbUnitPrice.Text),
                "The Price-change wasn't preserved",
                p.log);
        
        return sr;
    }

    [Scenario("Verify that changing a bound value in a simple WF control is represented in WPF control. e.g. TextBox ")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //Select second row in the WPF listview and WF ListBox;
        _avlvProducts.Items.MoveCurrentToPosition(1);
        _wflbProducts.SelectedIndex = 1;
        Application.DoEvents();

        //change the price of the current product to a new value - use the WF TextBox to do this change
        int newPrice = Convert.ToInt32(_wftbUnitPrice.Text) + 11;
        _wftbUnitPrice.Focus();
        _wftbUnitPrice.Text = newPrice.ToString();
        _wflbProducts.Focus();

        //verify that the new value appears in the WPF text-box 
        sr.IncCounters( newPrice == Convert.ToInt32(_avtbUnitPrice.Text),
                        "The Price-change in WF Text-Box wasn't reflected in the WPF TextBox",
                        p.log );

        //double check: change the selected row in the WPF ListView
        _avlvProducts.Items.MoveCurrentToPosition(2);
        Application.DoEvents();

        //move back to second item and verify that the new price was preserved in the text-box
        _avlvProducts.Items.MoveCurrentToPosition(1);
        Application.DoEvents();

        sr.IncCounters( newPrice == Convert.ToInt32(_avtbUnitPrice.Text),
                        "The Price-change wasn't preserved",
                        p.log);

        return sr;
    }

    [Scenario("Verify that changing a bound value from a list is represented in the Avalon control and the WF control.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);
        
        //Select last row in the WPF ListView and WF ListBox;
        _avlvProducts.Items.MoveCurrentToPosition( _avlvProducts.Items.Count - 1 );
        _wflbProducts.SelectedIndex = _wflbProducts.Items.Count - 1;
        Application.DoEvents();
        
        //change the ProductName in the WPF ListView (will just add some '_' chars)
        string newProdName = (_avlvProducts.Items.CurrentItem as Product).ProductName + "___";
        (_avlvProducts.Items.CurrentItem as Product).ProductName = newProdName;
        Application.DoEvents();

        //verify that the ProductName changed in WF ListBox
        sr.IncCounters( newProdName == (_wflbProducts.SelectedItem as Product).ProductName,
                        "The ProductName wasn't changed in WF ListBox",
                        p.log);

        //verify that the ProductName changed in WF TextBox
        sr.IncCounters( newProdName == _wftbProductName.Text,
                        "The ProductName wasn't changed in WF TextBox",
                        p.log);

        //verify that the ProductName changed in WPF ComboBox
        sr.IncCounters( newProdName == (_avcbProducts.SelectedItem as Product).ProductName ,
                        "The ProductName wasn't changed in WPF ComboBox",
                        p.log);

        //change the UnitPrice in the WF Listbox
        int newPrice = (_wflbProducts.SelectedItem as Product).UnitPrice + 5;
        (_wflbProducts.SelectedItem as Product).UnitPrice = newPrice;
        Application.DoEvents();

        //verify that the UnitPrice changed in WPF ListView
        sr.IncCounters(newPrice == (_avlvProducts.SelectedItem as Product).UnitPrice,
                        "The UnitPrice wasn't changed in WPF ListView",
                        p.log);

        //verify that the UnitPrice changed in WF TextBox
        sr.IncCounters( newPrice == Convert.ToInt32(_wftbUnitPrice.Text),
                        "The UnitPrice wasn't changed in WF TextBox",
                        p.log);

        //verify that the UnitPrice changed in WPF TextBox
        sr.IncCounters( newPrice == Convert.ToInt32(_avtbUnitPrice.Text),
                        "The UnitPrice wasn't changed in WPF TextBox",
                        p.log);

        return sr;
    }

    [Scenario("Vefify that deleting a bound value from a list is represented in the Avalon control and the WF control.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //remember the second Product 
        Product secondProduct = _avlvProducts.Items[1] as Product;

        //delete the first row
        Products.Instance.Remove(_avlvProducts.Items[0] as Product);

        //Select first row in the WPF ListView and WF ListBox;
        _avlvProducts.Items.MoveCurrentToPosition(0);
        _wflbProducts.SelectedIndex = 0;
        Application.DoEvents();

        //verify that the all controls are updated to reflect the recent delete.
        sr.IncCounters( secondProduct.ProductName == (_wflbProducts.SelectedItem as Product).ProductName,
                        "WF ListBox wasn't updated after the delete-operation",
                        p.log);

        sr.IncCounters( secondProduct.ProductName == _wftbProductName.Text,
                        "WF TextBox wasn't updated after the delete-operation",
                        p.log);

        sr.IncCounters( secondProduct.UnitPrice == Convert.ToInt32(_wftbUnitPrice.Text),
                        "WF TextBox wasn't updated after the delete-operation",
                        p.log);

        sr.IncCounters( secondProduct.ProductName == (_avlvProducts.SelectedItem as Product).ProductName,
                        "WPF ListView wasn't updated after the delete-operation",
                        p.log);

        sr.IncCounters( secondProduct.UnitPrice == Convert.ToInt32(_avtbUnitPrice.Text),
                        "WPF TextBox wasn't updated after the delete-operation",
                        p.log);

        sr.IncCounters( secondProduct.ProductName == (_avcbProducts.SelectedItem as Product).ProductName,
                        "WPF ComboBox wasn't updated after the delete-operation",
                        p.log);

        return sr;
    }

    [Scenario("Vefify that adding a bound value to a list is represented in the Avalon control and the WF control.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //create a new Product.
        Product newProduct = new Product(p.ru.GetString(30), p.ru.GetRange(1, 40));

        //add-it into the collection
        Products.Instance.Add(newProduct);

        //Select the last row in the WPF ListView and WF ListBox (should contain the new product) 
        _avlvProducts.Items.MoveCurrentToPosition(_avlvProducts.Items.Count - 1);
        _wflbProducts.SelectedIndex = _wflbProducts.Items.Count - 1;
        Application.DoEvents();

        //verify that the all controls are updated to reflect the recent add.
        sr.IncCounters( newProduct.ProductName == (_wflbProducts.SelectedItem as Product).ProductName,
                        "WF ListBox wasn't updated after the add-operation",
                        p.log);

        sr.IncCounters( newProduct.ProductName == _wftbProductName.Text,
                        "WF TextBox wasn't updated after the add-operation",
                        p.log);

        sr.IncCounters( newProduct.UnitPrice == Convert.ToInt32(_wftbUnitPrice.Text),
                        "WF TextBox wasn't updated after the add-operation",
                        p.log);

        sr.IncCounters( newProduct.ProductName == (_avlvProducts.SelectedItem as Product).ProductName,
                        "WPF ListView wasn't updated after the add-operation",
                        p.log);

        sr.IncCounters( newProduct.UnitPrice == Convert.ToInt32(_avtbUnitPrice.Text),
                        "WPF TextBox wasn't updated after the add-operation",
                        p.log);

        sr.IncCounters( newProduct.ProductName == (_avcbProducts.SelectedItem as Product).ProductName,
                        "WPF ComboBox wasn't updated after the add-operation",
                        p.log);

        return sr;
    }

    [Scenario("Verify that changing a bound value in an Avalon control in one EH is represented in the Avalon control hosted in another EH.")]
    public ScenarioResult Scenario6(TParams p)
    {
        //Done this part of the Scenario3
        return new ScenarioResult(true);
    }

    [Scenario("Verify that deleting a bound value in an Av control in one EH is represented in the Av control hosted in another EH.")]
    public ScenarioResult Scenario7(TParams p)
    {
        //Done this part of the Scenario4
        return new ScenarioResult(true);
    }

    [Scenario("Verify that adding a bound value in an Av control in one EH is represented in the Av control hosted in another EH.")]
    public ScenarioResult Scenario8(TParams p)
    {
        //Done this part of the Scenario5
        return new ScenarioResult(true);
    }

    #endregion

    #region Helper Methods

    //helper method that creates&sets all the needed controls
    private void InitializeControls()
    {
        //increase this Form a little.. 
        this.Width += 200;

        //Create a TableLayoutPanel with 2 colums (one for WinForms controls panel, one with a panel with element hosts)
        TableLayoutPanel formMainPanel = new TableLayoutPanel();
        formMainPanel.ColumnCount = 2;
        formMainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        formMainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        formMainPanel.Dock = DockStyle.Fill;
        formMainPanel.RowCount = 1;
        formMainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        this.Controls.Add(formMainPanel);

        //Create the TableLayoutPanel for WinForms controls ( 3 rows; one for a WinForm ListBox and two for WinForms TextBoxes )
        TableLayoutPanel wfControlsPanel = new TableLayoutPanel();
        wfControlsPanel.ColumnCount = 1;
        wfControlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        wfControlsPanel.Dock = DockStyle.Fill;
        wfControlsPanel.RowCount = 3;
        wfControlsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        wfControlsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        wfControlsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        formMainPanel.Controls.Add(wfControlsPanel, 0, 0);

        //Create the TableLayoutPanel for element-hosts ( with 2 rows; will put 2 element-hosts in these rows )
        TableLayoutPanel elementHostPanel = new TableLayoutPanel();
        elementHostPanel.ColumnCount = 1;
        elementHostPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        elementHostPanel.Dock = DockStyle.Fill;
        elementHostPanel.RowCount = 2;
        elementHostPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        elementHostPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
        formMainPanel.Controls.Add(elementHostPanel, 1, 0);

        //create the WinForms ListBox that will display product-names 
        _wflbProducts = new ListBox();
        _wflbProducts.Dock = DockStyle.Fill;
        _wflbProducts.FormattingEnabled = true;
        wfControlsPanel.Controls.Add(_wflbProducts, 0, 0);

        //create a TextBox that will display the Name for the current product (the one selected in the wflbProductNames listBox )
        _wftbProductName = new TextBox();
        _wftbProductName.Dock = DockStyle.Top;
        wfControlsPanel.Controls.Add(_wftbProductName, 0, 1);

        //create a TextBox that will display the UnitPrice for the current product (the one selected in the wflbProductNames listBox )
        _wftbUnitPrice = new TextBox();
        _wftbUnitPrice.Dock = DockStyle.Bottom;
        wfControlsPanel.Controls.Add(_wftbUnitPrice, 0, 2);

        //create the first elementHost - it will host an WPF grid with 2 controls; a list box and a textbox
        ElementHost ehFirst = new ElementHost();
        ehFirst.Dock = DockStyle.Fill;
        elementHostPanel.Controls.Add(ehFirst, 0, 0);

        //create the second elementHost - it will host an WPF combobox
        ElementHost ehSecond = new ElementHost();
        ehSecond.Dock = DockStyle.Fill;
        elementHostPanel.Controls.Add(ehSecond, 0, 1);

        //create the WPF Grid that will be hosted by the ehSecond and that will contain a ListBox and a TextBox
        _avGrid = new System.Windows.Controls.Grid();
        _avGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition());
        _avGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition());
        _avGrid.RowDefinitions[1].Height = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel);
        ehFirst.Child = _avGrid;

        //create the WPF ComboBox that will be hosted by the ehSecond - this combo will contain all the ProductNames 
        _avcbProducts = new System.Windows.Controls.ComboBox();
        _avcbProducts.IsSynchronizedWithCurrentItem = true;
        ehSecond.Child = _avcbProducts;

        //create the WPF ListView that will display the ProductName - UnitPrice info
        System.Windows.Controls.GridViewColumn col1 = new System.Windows.Controls.GridViewColumn();
        col1.Header = "Product Name";
        System.Windows.Controls.GridViewColumn col2 = new System.Windows.Controls.GridViewColumn();
        col2.Header = "Unit Price";
        System.Windows.Controls.GridView view = new System.Windows.Controls.GridView();
        view.Columns.Add(col1);
        view.Columns.Add(col2);
        _avlvProducts = new System.Windows.Controls.ListView();
        _avlvProducts.IsSynchronizedWithCurrentItem = true;
        _avlvProducts.View = view;
        _avlvProducts.Margin = new System.Windows.Thickness(5, 5, 5, 5);
        System.Windows.Controls.Grid.SetColumn(_avlvProducts, 0);
        System.Windows.Controls.Grid.SetRow(_avlvProducts, 0);
        _avGrid.Children.Add(_avlvProducts);

        //create the WPF TextBox that will display the UnitPrice for the current product
        _avtbUnitPrice = new System.Windows.Controls.TextBox();
        System.Windows.Controls.Grid.SetRow(_avtbUnitPrice, 1);
        System.Windows.Controls.Grid.SetColumn(_avtbUnitPrice, 0);
        _avGrid.Children.Add(_avtbUnitPrice);

    }

    //helper method that connects to nwind-access database and returns the products Table 
    private DataTable GetProducts()
    {
        string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=nwind.mdb";
        OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString);
        OleDbCommand cmd = new OleDbCommand("Select * from Products", conn);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

        DataSet ds = new DataSet();
        adapter.Fill(ds, "Products");
        return ds.Tables["Products"];
    }

    #endregion

class Product : INotifyPropertyChanged
{
    private string _productName = "";
    private int _unitPrice = 0;

    public event PropertyChangedEventHandler PropertyChanged;

    public Product(string productName, int unitPrice)
    {
        this._productName = productName;
        this._unitPrice = unitPrice;
    }

    public string ProductName
    {
        get
        {
            return _productName;
        }
        set
        {
            _productName = value;
            OnPropertyChanged("ProductName");
        }
    }
    public int UnitPrice
    {
        get
        {
            return _unitPrice;
        }
        set
        {
            _unitPrice = value;
            OnPropertyChanged("UnitPrice");
        }
    }

    protected void OnPropertyChanged(string propName)
    {
        if (this.PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
class Products : BindingList<Product>
{
    private static Products s_instance = new Products();
    private Products()
    {
    }
    public static Products Instance
    {
        get
        {
            return s_instance;
        }
    }
}

}


// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify that changing a bound value in a simple WPF control is represented in a simple WF control. e.g. TextBox 
//@ Verify that changing a bound value in a simple WF control is represented in WPF control. e.g. TextBox 
//@ Verify that changing a bound value from a list is represented in the Avalon control and the WF control.
//@ Vefify that deleting a bound value from a list is represented in the Avalon control and the WF control.
//@ Vefify that adding a bound value to a list is represented in the Avalon control and the WF control.
//@ Verify that changing a bound value in an Avalon control in one EH is represented in the Avalon control hosted in another EH.
//@ Verify that deleting a bound value in an Av control in one EH is represented in the Av control hosted in another EH.
//@ Verify that adding a bound value in an Av control in one EH is represented in the Av control hosted in another EH.
