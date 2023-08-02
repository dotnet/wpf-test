using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class DataGridRowAutomationPeerRegressionTest6Test : Page
    {
        private ControlTemplate newRowControlTemplate;
        private ControlTemplate defaultRowControlTemplate;

        public DataGridRowAutomationPeerRegressionTest6Test()
        {
            InitializeComponent();

            Workers = new BindingList<Worker>();
            Workers.Add(new Worker() { Name = "worker1" });
            Workers.Add(new Worker() { Name = "worker2" });
            Workers.Add(new Worker() { Name = "worker3" });
            DataContext = this;

            newRowControlTemplate = (ControlTemplate)FindResource("NewRowControlTemplate");
            namesGrid.LoadingRow += new EventHandler<DataGridRowEventArgs>(NamesGrid_LoadingRow);
        }

        void NamesGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow loadingRow = e.Row;

            if (loadingRow != null)
            {
                if (defaultRowControlTemplate == null)
                {
                    defaultRowControlTemplate = loadingRow.Template;
                }

                ControlTemplate newTemplate = defaultRowControlTemplate;

                if (loadingRow.Item == CollectionView.NewItemPlaceholder)
                {
                    newTemplate = newRowControlTemplate;
                }

                if (loadingRow.Template != newTemplate)
                {
                    loadingRow.Template = newTemplate;

                    loadingRow.UpdateLayout();
                }
            }
        }

        public BindingList<Worker> Workers { get; set; }
    }

    public class Worker
    {
        public string Name { get; set; }
    }
}
