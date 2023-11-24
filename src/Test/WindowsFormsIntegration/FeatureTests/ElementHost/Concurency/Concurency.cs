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
// Author:      bogdanbr
//
public class Concurency : ReflectBase
{
    private ListBox wflbProducts = null;
    private TextBox wftbProductName = null;
    private TextBox wftbUnitPrice = null;
    private System.Windows.Controls.Grid avGrid = null;
    private System.Windows.Controls.ComboBox avcbProducts = null;
    private System.Windows.Controls.ListView avlvProducts = null;
    private System.Windows.Controls.TextBox avtbUnitPrice = null;

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
        wflbProducts.DataSource = Products.Instance;
        wflbProducts.DisplayMember = "ProductName";

        wftbProductName.DataBindings.Add(new Binding("Text", Products.Instance, "ProductName"));
        wftbUnitPrice.DataBindings.Add(new Binding("Text", Products.Instance, "UnitPrice"));

        avGrid.DataContext = Products.Instance;
        avlvProducts.ItemsSource = Products.Instance;
        ((System.Windows.Controls.GridView)avlvProducts.View).Columns[0].DisplayMemberBinding = new System.Windows.Data.Binding("ProductName");
        ((System.Windows.Controls.GridView)avlvProducts.View).Columns[1].DisplayMemberBinding = new System.Windows.Data.Binding("UnitPrice");

        avcbProducts.DataContext = avGrid.DataContext;
        avcbProducts.ItemsSource = Products.Instance;
        avcbProducts.DisplayMemberPath = "ProductName";

        System.Windows.Data.BindingOperations.SetBinding( avtbUnitPrice,
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
        avlvProducts.Items.MoveCurrentToPosition(0);
        wflbProducts.SelectedIndex = 0;
        Application.DoEvents();

        //change the price of the current product to a new value - use the WPF TextBox to do this change
        int newPrice = Convert.ToInt32(avtbUnitPrice.Text) + 10;
        avtbUnitPrice.Focus();
        avtbUnitPrice.Text = newPrice.ToString();
        avlvProducts.Focus();
        
        //verify that the new value appears in the WF text-box 
        sr.IncCounters( newPrice == Convert.ToInt32(wftbUnitPrice.Text),
                        "The Price-change in WPF Text-Box wasn't reflected in the WF TextBox",
                        p.log);

        //double-ckeck: change the selected row in the WF ListBox
        wflbProducts.SelectedIndex = 1;
        Application.DoEvents();
        
        //move back to first item and verify that the new price was preserved in the text-box
        wflbProducts.SelectedIndex = 0;
        Application.DoEvents();

        sr.IncCounters(newPrice == Convert.ToInt32(wftbUnitPrice.Text),
                "The Price-change wasn't preserved",
                p.log);
        
        return sr;
    }

    [Scenario("Verify that changing a bound value in a simple WF control is represented in WPF control. e.g. TextBox ")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //Select second row in the WPF listview and WF ListBox;
        avlvProducts.Items.MoveCurrentToPosition(1);
        wflbProducts.SelectedIndex = 1;
        Application.DoEvents();

        //change the price of the current product to a new value - use the WF TextBox to do this change
        int newPrice = Convert.ToInt32(wftbUnitPrice.Text) + 11;
        wftbUnitPrice.Focus();
        wftbUnitPrice.Text = newPrice.ToString();
        wflbProducts.Focus();

        //verify that the new value appears in the WPF text-box 
        sr.IncCounters( newPrice == Convert.ToInt32(avtbUnitPrice.Text),
                        "The Price-change in WF Text-Box wasn't reflected in the WPF TextBox",
                        p.log );

        //double check: change the selected row in the WPF ListView
        avlvProducts.Items.MoveCurrentToPosition(2);
        Application.DoEvents();

        //move back to second item and verify that the new price was preserved in the text-box
        avlvProducts.Items.MoveCurrentToPosition(1);
        Application.DoEvents();

        sr.IncCounters( newPrice == Convert.ToInt32(avtbUnitPrice.Text),
                        "The Price-change wasn't preserved",
                        p.log);

        return sr;
    }

    [Scenario("Verify that changing a bound value from a list is represented in the Avalon control and the WF control.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);
        
        //Select last row in the WPF ListView and WF ListBox;
        avlvProducts.Items.MoveCurrentToPosition( avlvProducts.Items.Count - 1 );
        wflbProducts.SelectedIndex = wflbProducts.Items.Count - 1;
        Application.DoEvents();
        
        //change the ProductName in the WPF ListView (will just add some '_' chars)
        string newProdName = (avlvProducts.Items.CurrentItem as Product).ProductName + "___";
        (avlvProducts.Items.CurrentItem as Product).ProductName = newProdName;
        Application.DoEvents();

        //verify that the ProductName changed in WF ListBox
        sr.IncCounters( newProdName == (wflbProducts.SelectedItem as Product).ProductName,
                        "The ProductName wasn't changed in WF ListBox",
                        p.log);

        //verify that the ProductName changed in WF TextBox
        sr.IncCounters( newProdName == wftbProductName.Text,
                        "The ProductName wasn't changed in WF TextBox",
                        p.log);

        //verify that the ProductName changed in WPF ComboBox
        sr.IncCounters( newProdName == (avcbProducts.SelectedItem as Product).ProductName ,
                        "The ProductName wasn't changed in WPF ComboBox",
                        p.log);

        //change the UnitPrice in the WF Listbox
        int newPrice = (wflbProducts.SelectedItem as Product).UnitPrice + 5;
        (wflbProducts.SelectedItem as Product).UnitPrice = newPrice;
        Application.DoEvents();

        //verify that the UnitPrice changed in WPF ListView
        sr.IncCounters(newPrice == (avlvProducts.SelectedItem as Product).UnitPrice,
                        "The UnitPrice wasn't changed in WPF ListView",
                        p.log);

        //verify that the UnitPrice changed in WF TextBox
        sr.IncCounters( newPrice == Convert.ToInt32(wftbUnitPrice.Text),
                        "The UnitPrice wasn't changed in WF TextBox",
                        p.log);

        //verify that the UnitPrice changed in WPF TextBox
        sr.IncCounters( newPrice == Convert.ToInt32(avtbUnitPrice.Text),
                        "The UnitPrice wasn't changed in WPF TextBox",
                        p.log);

        return sr;
    }

    [Scenario("Vefify that deleting a bound value from a list is represented in the Avalon control and the WF control.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //remember the second Product 
        Product secondProduct = avlvProducts.Items[1] as Product;

        //delete the first row
        Products.Instance.Remove(avlvProducts.Items[0] as Product);

        //Select first row in the WPF ListView and WF ListBox;
        avlvProducts.Items.MoveCurrentToPosition(0);
        wflbProducts.SelectedIndex = 0;
        Application.DoEvents();

        //verify that the all controls are updated to reflect the recent delete.
        sr.IncCounters( secondProduct.ProductName == (wflbProducts.SelectedItem as Product).ProductName,
                        "WF ListBox wasn't updated after the delete-operation",
                        p.log);

        sr.IncCounters( secondProduct.ProductName == wftbProductName.Text,
                        "WF TextBox wasn't updated after the delete-operation",
                        p.log);

        sr.IncCounters( secondProduct.UnitPrice == Convert.ToInt32(wftbUnitPrice.Text),
                        "WF TextBox wasn't updated after the delete-operation",
                        p.log);

        sr.IncCounters( secondProduct.ProductName == (avlvProducts.SelectedItem as Product).ProductName,
                        "WPF ListView wasn't updated after the delete-operation",
                        p.log);

        sr.IncCounters( secondProduct.UnitPrice == Convert.ToInt32(avtbUnitPrice.Text),
                        "WPF TextBox wasn't updated after the delete-operation",
                        p.log);

        sr.IncCounters( secondProduct.ProductName == (avcbProducts.SelectedItem as Product).ProductName,
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
        avlvProducts.Items.MoveCurrentToPosition(avlvProducts.Items.Count - 1);
        wflbProducts.SelectedIndex = wflbProducts.Items.Count - 1;
        Application.DoEvents();

        //verify that the all controls are updated to reflect the recent add.
        sr.IncCounters( newProduct.ProductName == (wflbProducts.SelectedItem as Product).ProductName,
                        "WF ListBox wasn't updated after the add-operation",
                        p.log);

        sr.IncCounters( newProduct.ProductName == wftbProductName.Text,
                        "WF TextBox wasn't updated after the add-operation",
                        p.log);

        sr.IncCounters( newProduct.UnitPrice == Convert.ToInt32(wftbUnitPrice.Text),
                        "WF TextBox wasn't updated after the add-operation",
                        p.log);

        sr.IncCounters( newProduct.ProductName == (avlvProducts.SelectedItem as Product).ProductName,
                        "WPF ListView wasn't updated after the add-operation",
                        p.log);

        sr.IncCounters( newProduct.UnitPrice == Convert.ToInt32(avtbUnitPrice.Text),
                        "WPF TextBox wasn't updated after the add-operation",
                        p.log);

        sr.IncCounters( newProduct.ProductName == (avcbProducts.SelectedItem as Product).ProductName,
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
        wflbProducts = new ListBox();
        wflbProducts.Dock = DockStyle.Fill;
        wflbProducts.FormattingEnabled = true;
        wfControlsPanel.Controls.Add(wflbProducts, 0, 0);

        //create a TextBox that will display the Name for the current product (the one selected in the wflbProductNames listBox )
        wftbProductName = new TextBox();
        wftbProductName.Dock = DockStyle.Top;
        wfControlsPanel.Controls.Add(wftbProductName, 0, 1);

        //create a TextBox that will display the UnitPrice for the current product (the one selected in the wflbProductNames listBox )
        wftbUnitPrice = new TextBox();
        wftbUnitPrice.Dock = DockStyle.Bottom;
        wfControlsPanel.Controls.Add(wftbUnitPrice, 0, 2);

        //create the first elementHost - it will host an WPF grid with 2 controls; a list box and a textbox
        ElementHost ehFirst = new ElementHost();
        ehFirst.Dock = DockStyle.Fill;
        elementHostPanel.Controls.Add(ehFirst, 0, 0);

        //create the second elementHost - it will host an WPF combobox
        ElementHost ehSecond = new ElementHost();
        ehSecond.Dock = DockStyle.Fill;
        elementHostPanel.Controls.Add(ehSecond, 0, 1);

        //create the WPF Grid that will be hosted by the ehSecond and that will contain a ListBox and a TextBox
        avGrid = new System.Windows.Controls.Grid();
        avGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition());
        avGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition());
        avGrid.RowDefinitions[1].Height = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel);
        ehFirst.Child = avGrid;

        //create the WPF ComboBox that will be hosted by the ehSecond - this combo will contain all the ProductNames 
        avcbProducts = new System.Windows.Controls.ComboBox();
        avcbProducts.IsSynchronizedWithCurrentItem = true;
        ehSecond.Child = avcbProducts;

        //create the WPF ListView that will display the ProductName - UnitPrice info
        System.Windows.Controls.GridViewColumn col1 = new System.Windows.Controls.GridViewColumn();
        col1.Header = "Product Name";
        System.Windows.Controls.GridViewColumn col2 = new System.Windows.Controls.GridViewColumn();
        col2.Header = "Unit Price";
        System.Windows.Controls.GridView view = new System.Windows.Controls.GridView();
        view.Columns.Add(col1);
        view.Columns.Add(col2);
        avlvProducts = new System.Windows.Controls.ListView();
        avlvProducts.IsSynchronizedWithCurrentItem = true;
        avlvProducts.View = view;
        avlvProducts.Margin = new System.Windows.Thickness(5, 5, 5, 5);
        System.Windows.Controls.Grid.SetColumn(avlvProducts, 0);
        System.Windows.Controls.Grid.SetRow(avlvProducts, 0);
        avGrid.Children.Add(avlvProducts);

        //create the WPF TextBox that will display the UnitPrice for the current product
        avtbUnitPrice = new System.Windows.Controls.TextBox();
        System.Windows.Controls.Grid.SetRow(avtbUnitPrice, 1);
        System.Windows.Controls.Grid.SetColumn(avtbUnitPrice, 0);
        avGrid.Children.Add(avtbUnitPrice);

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
    private string m_productName = "";
    private int m_unitPrice = 0;

    public event PropertyChangedEventHandler PropertyChanged;

    public Product(string productName, int unitPrice)
    {
        this.m_productName = productName;
        this.m_unitPrice = unitPrice;
    }

    public string ProductName
    {
        get
        {
            return m_productName;
        }
        set
        {
            m_productName = value;
            OnPropertyChanged("ProductName");
        }
    }
    public int UnitPrice
    {
        get
        {
            return m_unitPrice;
        }
        set
        {
            m_unitPrice = value;
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
    private static Products m_instance = new Products();
    private Products()
    {
    }
    public static Products Instance
    {
        get
        {
            return m_instance;
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
