using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections;


//
// Testcase:    MasterDetail
// Description: WF control represents the details of an Avalon Datasource
// Author:      bogdanbr
//
public class MasterDetail : ReflectBase
{
    //controls 
    private ListBox wflbMaster = null;
    private ListBox wflbDetail = null;
    private System.Windows.Controls.ListBox avlbMaster = null;
    private System.Windows.Controls.ListBox avlbDetail = null;

    //data-components
    private DataSet nwindDataSet = null;
    public BindingSource categoriesProductsBindingSource = null;
    public BindingSource categoriesBindingSource = null;

    #region Testcase setup
    public MasterDetail(string[] args) : base(args) { }


    protected override void InitTest(TParams p)
    {
        //create & set the needed controls 
        InitializeControls();

        //create & set the needed data-objects
        InitializeData();

        //bind controls 
        wflbMaster.DataSource = categoriesBindingSource;
        wflbMaster.DisplayMember = "CategoryName";

        wflbDetail.DataSource = categoriesProductsBindingSource;
        wflbDetail.DisplayMember = "ProductName";

        avlbMaster.IsSynchronizedWithCurrentItem = true;
        avlbMaster.ItemsSource = categoriesBindingSource;
        avlbMaster.DisplayMemberPath = "CategoryName";

        avlbDetail.IsSynchronizedWithCurrentItem = true;
        avlbDetail.ItemsSource = categoriesProductsBindingSource;
        avlbDetail.DisplayMemberPath = "ProductName";

        base.InitTest(p);
    }

    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("WPF control in an EH showing the Master-data and WF control showing the Detail-data")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //navigate in the WPF control showing the MasterData
        for (int i = 0; i < avlbMaster.Items.Count; i++)
        {
            avlbMaster.Items.MoveCurrentToPosition(i);
            Application.DoEvents();

            //get the selected CategoryID
            DataRowView selectedRow = avlbMaster.SelectedItem as DataRowView;
            int selectedCategoryID = Convert.ToInt32(selectedRow.Row["CategoryID"]);

            //Validate the products displayed by the WF list-box
            ValidateProducts(sr, selectedCategoryID, wflbDetail.Items);
        }

        return sr;
    }

    [Scenario("WPF control in an EH showing the Details-data and WF control showing the Master-data")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //navigate(backwards) in the WF control showing the MasterData
        for (int i = wflbMaster.Items.Count - 1; i >= 0; i--)
        {
            wflbMaster.SelectedIndex = i;
            Application.DoEvents();

            //get the selected CategoryID
            DataRowView selectedRow = wflbMaster.SelectedItem as DataRowView;
            int selectedCategoryID = Convert.ToInt32(selectedRow.Row["CategoryID"]);

            //Validate the products displayed by the WPF list-box
            ValidateProducts(sr, selectedCategoryID, avlbDetail.Items);
        }

        return sr;
    }

    [Scenario("WPF control in an EH showing the Master and another WPF control in another EH showing the Details")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //navigate in the WPF control showing the MasterData
        for (int i = 0; i < avlbMaster.Items.Count; i++)
        {
            avlbMaster.Items.MoveCurrentToPosition(i);
            Application.DoEvents();

            //get the selected CategoryID
            DataRowView selectedRow = avlbMaster.SelectedItem as DataRowView;
            int selectedCategoryID = Convert.ToInt32(selectedRow.Row["CategoryID"]);

            //Validate the products displayed by the WPF control showing the details 
            ValidateProducts(sr, selectedCategoryID, avlbDetail.Items);
        }

        return sr;
    }

    [Scenario("WPF control in WPF Window showing the Master and WPF control in WF Form EH showing the Details.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //display the WPF Window with the WPF control showing the Master data. 
        MasterWPFWindow masterWPFWnd = new MasterWPFWindow(this);
        masterWPFWnd.Show();

        //navigate in the WPF Window
        for (int i = 0; i < masterWPFWnd.avlbMaster.Items.Count; i++)
        {
            masterWPFWnd.avlbMaster.Items.MoveCurrentToPosition(i);
            Application.DoEvents();

            //get the selected CategoryID
            DataRowView selectedRow = masterWPFWnd.avlbMaster.SelectedItem as DataRowView;
            int selectedCategoryID = Convert.ToInt32(selectedRow.Row["CategoryID"]);

            //Validate the products displayed by the controls in main form
            ValidateProducts(sr, selectedCategoryID, avlbDetail.Items);
            ValidateProducts(sr, selectedCategoryID, wflbDetail.Items);
        }


        //close the WPF form
        masterWPFWnd.Close();

        return sr;
    }

    [Scenario("WPF control in WPF Window showing the Details and WPF control in WF Form EH showing the Master.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //display the WPF Window with the WPF control showing the Details data. 
        DetailWPFWindow detailWPFWnd = new DetailWPFWindow(this);
        detailWPFWnd.Show();

        //navigate in the WF control showing the MasterData
        for (int i = 0; i < wflbMaster.Items.Count; i++)
        {
            wflbMaster.SelectedIndex = i;
            Application.DoEvents();

            //get the selected CategoryID
            DataRowView selectedRow = wflbMaster.SelectedItem as DataRowView;
            int selectedCategoryID = Convert.ToInt32(selectedRow.Row["CategoryID"]);

            //Validate the products displayed by the control in WPF Window
            ValidateProducts(sr, selectedCategoryID, detailWPFWnd.avlbDetails.Items);
        }

        //navigate in the WPF control showing the MasterData
        for (int i = 0; i < avlbMaster.Items.Count; i++)
        {
            avlbMaster.Items.MoveCurrentToPosition(i);
            Application.DoEvents();

            //get the selected CategoryID
            DataRowView selectedRow = avlbMaster.SelectedItem as DataRowView;
            int selectedCategoryID = Convert.ToInt32(selectedRow.Row["CategoryID"]);

            //Validate the products displayed by the control in WPF Window
            ValidateProducts(sr, selectedCategoryID, detailWPFWnd.avlbDetails.Items);
        }

        //close the WPF form
        detailWPFWnd.Close();

        return sr;
    }

    [Scenario("WPF controls in a WFForm showing the Details and another WPF control on another WFForm showing the Master")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //display the WFForm with the WPF control showing the Details data. 
        DetailWFForm detailWFForm = new DetailWFForm(this);
        detailWFForm.Show();

        //navigate in the WPF control showing the MasterData
        for (int i = 0; i < avlbMaster.Items.Count; i++)
        {
            avlbMaster.Items.MoveCurrentToPosition(i);
            Application.DoEvents();

            //get the selected CategoryID
            DataRowView selectedRow = avlbMaster.SelectedItem as DataRowView;
            int selectedCategoryID = Convert.ToInt32(selectedRow.Row["CategoryID"]);

            //Validate the products displayed by the control in WPF Window
            ValidateProducts(sr, selectedCategoryID, detailWFForm.avlbDetails.Items);
        }

        //close the WPF form
        detailWFForm.Close();

        return sr;
    }

    #endregion

    #region Helper Functions 

    private DataSet GetData()
    {
        DataSet dsNorthwind = new DataSet();

        //create the connection-object
        // NOTE: This connection string used to have credentials, removed due to it triggering the credential scanning tool.
        string connectionString = "Data Source=wfsrv2;Initial Catalog=Northwind;";
        SqlConnection sqlConn = new SqlConnection(connectionString);

        //create the dataAdapter that will use to fill the dataSet
        SqlDataAdapter adapter = new SqlDataAdapter();

        //Get the Categories
        adapter.SelectCommand = new SqlCommand("SELECT CategoryID, CategoryName, Description FROM Categories", sqlConn);
        adapter.Fill(dsNorthwind, "Categories");

        //Get the products
        adapter.SelectCommand = new SqlCommand("SELECT ProductID, CategoryID, ProductName, UnitPrice FROM Products", sqlConn);
        adapter.Fill(dsNorthwind, "Products");

        dsNorthwind.Relations.Add("FK_Categories_Products", dsNorthwind.Tables["Categories"].Columns["CategoryID"], dsNorthwind.Tables["Products"].Columns["CategoryID"]);

        return dsNorthwind;
    }

    //helper method that creates&sets all the needed controls
    private void InitializeControls()
    {
        //increase a little the size of this form 
        this.Size = new Size(this.Width + 200, this.Height + 200);

        //Create a TableLayoutPanel with 4 cells (will contain WF control with detail-data, WF control with master-data, WPF control with detail-data, WPF control with master-data)
        TableLayoutPanel formMainPanel = new TableLayoutPanel();
        formMainPanel.ColumnCount = 2;
        formMainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        formMainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        formMainPanel.RowCount = 2;
        formMainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        formMainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        formMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.Controls.Add(formMainPanel);
        
        //Create a WF ListBox that will display the master-data
        wflbMaster = new ListBox();
        wflbMaster.Dock = DockStyle.Fill;
        formMainPanel.Controls.Add(wflbMaster, 0, 0);

        //create a WF ListBox that will display the detail-data
        wflbDetail = new ListBox();
        wflbDetail.Dock = DockStyle.Fill;
        formMainPanel.Controls.Add(wflbDetail, 0, 1);

        //create the ElementHost that will host the WPF with master data
        ElementHost ehMaster = new ElementHost();
        ehMaster.Dock = DockStyle.Fill;
        formMainPanel.Controls.Add(ehMaster, 1, 0);

        //create the WPF ListBox that will display the master-data 
        avlbMaster = new System.Windows.Controls.ListBox();
        ehMaster.Child = avlbMaster;

        //create the ElementHost that will host the WPF with detail-data
        ElementHost ehDetail = new ElementHost();
        ehDetail.Dock = DockStyle.Fill;
        formMainPanel.Controls.Add(ehDetail, 1, 1);

        //create the WPF ListBox that will display the detail-data 
        avlbDetail = new System.Windows.Controls.ListBox();
        ehDetail.Child = avlbDetail;
    }

    //helper method that creates & sets all the needed data-objects;
    private void InitializeData()
    {
        nwindDataSet = GetData();

        categoriesBindingSource = new BindingSource();
        this.categoriesBindingSource.DataMember = "Categories";
        this.categoriesBindingSource.DataSource = nwindDataSet;

        categoriesProductsBindingSource = new BindingSource();
        categoriesProductsBindingSource.DataMember = "FK_Categories_Products";
        categoriesProductsBindingSource.DataSource = categoriesBindingSource;

        // Due to current limitations, you have to give a little help
        // to the currency management aspect of the data models.
        // We need to create an event handler that will fire whenever the current item is changed
        // via the WPF ListBox and then force it to be in sync with the BindingSource.
        BindingListCollectionView cv = (BindingListCollectionView)CollectionViewSource.GetDefaultView(categoriesBindingSource);
        cv.CurrentChanged += new EventHandler(cv_CurrentChanged);
    }

    // Due to current limitations, you have to give a little help
    // to the currency management aspect of the data models.
    // We need to create an event handler that will fire whenever the current item is changed
    // via the WPF ListBox and then force it to be in sync with the BindingSource.
    void cv_CurrentChanged(object sender, EventArgs e)
    {
        BindingListCollectionView cv = sender as BindingListCollectionView;
        categoriesBindingSource.Position = cv.CurrentPosition;
    }

    //helper metod used to validate the Products displayed by a control after the Category was changed in another control 
    private void ValidateProducts(ScenarioResult sr, int categoryID, IList displayedProducts)
    {
        //get the products for the categoryID 
        DataRow[] expectedProducts = nwindDataSet.Tables["Products"].Select("CategoryID=" + categoryID.ToString());

        //validate the number of products displayed
        sr.IncCounters( expectedProducts.Length == displayedProducts.Count,
                        "Invalid navigation: categoryID=" + categoryID.ToString(),
                        scenarioParams.log);

        if (expectedProducts.Length == displayedProducts.Count)
        {
            for (int i = 0; i < expectedProducts.Length - 1; i++)
            {
                string expectedProdID = expectedProducts[i]["ProductID"].ToString();
                string actualProdID = (displayedProducts[i] as DataRowView).Row["ProductID"].ToString();
                sr.IncCounters( expectedProdID == actualProdID,
                                "Invalid navigation categoryID=" + categoryID.ToString(),
                                scenarioParams.log);
            }
        }
    }
         
    #endregion
}

class MasterWPFWindow : System.Windows.Window
{
    private MasterDetail m_parent = null;
    public System.Windows.Controls.ListBox avlbMaster = null;

    public MasterWPFWindow(MasterDetail parent)
    {
        m_parent = parent;

        this.Height = 200;
        this.Width = 300;

        avlbMaster = new System.Windows.Controls.ListBox();
        this.Content = avlbMaster;
        
        avlbMaster.IsSynchronizedWithCurrentItem = true;
        avlbMaster.ItemsSource = m_parent.categoriesBindingSource;
        avlbMaster.DisplayMemberPath = "CategoryName";
    }
}
class DetailWPFWindow : System.Windows.Window
{
    private MasterDetail m_parent = null;
    public System.Windows.Controls.ListBox avlbDetails = null;

    public DetailWPFWindow(MasterDetail parent)
    {
        m_parent = parent;

        this.Height = 200;
        this.Width = 300;

        avlbDetails = new System.Windows.Controls.ListBox();
        this.Content = avlbDetails;

        avlbDetails.IsSynchronizedWithCurrentItem = true;
        avlbDetails.ItemsSource = m_parent.categoriesProductsBindingSource;
        avlbDetails.DisplayMemberPath = "ProductName";
    }
}
class DetailWFForm : Form
{
    private MasterDetail m_parent = null;
    public System.Windows.Controls.ListBox avlbDetails = null;

    public DetailWFForm(MasterDetail parent)
    {
        m_parent = parent;

        this.Height = 200;
        this.Width = 300;

        ElementHost ehDetail = new ElementHost();
        ehDetail.Dock = DockStyle.Fill;
        this.Controls.Add(ehDetail);

        avlbDetails = new System.Windows.Controls.ListBox();
        ehDetail.Child = avlbDetails;

        avlbDetails.IsSynchronizedWithCurrentItem = true;
        avlbDetails.ItemsSource = m_parent.categoriesProductsBindingSource;
        avlbDetails.DisplayMemberPath = "ProductName";
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ WPF control in an EH showing the Master-data and WF control showing the Detail-data
//@ WPF control in an EH showing the Details-data and WF control showing the Master-data
//@ WPF control in an EH showing the Master and another WPF control in another EH showing the Details
//@ WPF control in WPF Window showing the Master and WPF control in WF Form EH showing the Details
//@ WPF control in WPF Window showing the Details and WPF control in WF Form EH showing the Master
//@ WPF controls in a WFForm showing the Details and another WPF control on another WFForm showing the Master