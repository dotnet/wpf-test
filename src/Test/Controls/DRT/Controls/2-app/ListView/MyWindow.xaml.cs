// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MyTestNamespace
{
  /// <summary>
  /// Interaction logic for MyWindow.xaml
  /// </summary>

    public partial class MyWindow : Window
    {
        ObservableCollection<DateTime> _items;
        CollectionViewSource _cvs;

        private void Init(object sender, EventArgs e)
        {
            _cvs = this.Resources["cvs"] as CollectionViewSource;
            CreateDataSource();
        }

        public void OnButtonClick(object sender, EventArgs e)
        {
            listView.View = null;
        }

        public void OnButtonClick1(object sender, EventArgs e)
        {
            listView.View = CreateDetailsView1();
        }

        public void OnButtonClick2(object sender, EventArgs e)
        {
            listView.View = CreateDetailsView2();
        }

        public void OnButtonClick3(object sender, EventArgs e)
        {
            ListViewItem lvi = listView.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
            ListView.SetIsSelected(lvi, true);
            lvi = listView.ItemContainerGenerator.ContainerFromIndex(1) as ListViewItem;
            ListView.SetIsSelected(lvi, true);
            lvi = listView.ItemContainerGenerator.ContainerFromIndex(3) as ListViewItem;
            ListView.SetIsSelected(lvi, true);
        }

        public void OnButtonClick4(object sender, EventArgs e)
        {
            listView.SelectionMode = SelectionMode.Single;
        }

        public void OnButtonClick5(object sender, EventArgs e)
        {
            listView.SelectedIndex = -1;
        }

        public void OnButtonClick6(object sender, EventArgs e)
        {
            _items.Add(new DateTime(2005, 10, 1));
        }

        public void OnButtonClick7(object sender, EventArgs e)
        {
            _items.RemoveAt(0);
            _items.RemoveAt(0);
        }

        public void OnButtonClick8(object sender, EventArgs e)
        {
            _items.Clear();
        }

        public void OnButtonClick9(object sender, EventArgs e)
        {
            CreateDataSource();
        }

        public void OnButtonClick10(object sender, EventArgs e)
        {
            GridViewColumn column = new GridViewColumn();
            column.DisplayMemberBinding = new Binding("Month");
            column.Header = "Month2";
            column.Width = 80.0;
            ((GridView)listView.View).Columns.Add(column);
        }

        public void OnButtonClick11(object sender, EventArgs e)
        {
            ((GridView)listView.View).Columns.RemoveAt(0);
            ((GridView)listView.View).Columns.RemoveAt(0);
        }

        public void OnButtonClick12(object sender, EventArgs e)
        {
            ((GridView)listView.View).Columns.Clear();
        }

        public void OnButtonClick13(object sender, EventArgs e)
        {
            CreateLargeDataSource();
        }

        public void OnButtonClick14(object sender, EventArgs e)
        {
            ToggleGrouping(1);
        }

        public void OnButtonClick15(object sender, EventArgs e)
        {
            ToggleGrouping(2);
        }

        private void CreateDataSource()
        {
            _items = new ObservableCollection<DateTime>();
            DateTime dt = new DateTime(2005, 1, 25);
            for (int i = 0; i < 15; ++i)
            {
                _items.Add(dt);
                dt = dt.AddDays(1);
            }
            _cvs.Source = _items;
        }

        private void CreateLargeDataSource()
        {
            _items = new ObservableCollection<DateTime>();
            DateTime dt = new DateTime(2005, 1, 25);
            for (int i = 0; i < 1000; ++i)
            {
                _items.Add(dt);
                dt = dt.AddDays(7);
            }
            _cvs.Source = _items;
        }

        private void ToggleGrouping(int depth)
        {
            var groupDescriptions = _cvs.GroupDescriptions;
            if (groupDescriptions.Count == depth)
            {
                depth = 0;
            }
            if (groupDescriptions.Count != depth)
            {
                using (_cvs.DeferRefresh())
                {
                    groupDescriptions.Clear();
                    if (depth > 0)
                    {
                        groupDescriptions.Add(new PropertyGroupDescription("Year"));
                    }
                    if (depth > 1)
                    {
                        groupDescriptions.Add(new PropertyGroupDescription("Month"));
                    }
                }
            }
        }

        // The simplest details view
        private GridView CreateDetailsView1()
        {
            GridView detailsView = new GridView();

            GridViewColumn cl1 = new GridViewColumn();
            cl1.DisplayMemberBinding = new Binding("Day");
            cl1.Header = "Day";
            cl1.Width = 80.0;
            detailsView.Columns.Add(cl1);

            GridViewColumn cl2 = new GridViewColumn();
            cl2.DisplayMemberBinding = new Binding("Month");
            cl2.Header = "Month";
            cl2.Width = 80.0;
            detailsView.Columns.Add(cl2);

            GridViewColumn cl3 = new GridViewColumn();
            cl3.DisplayMemberBinding = new Binding("Year");
            cl3.Header = "Year";
            cl3.Width = 80.0;
            detailsView.Columns.Add(cl3);

            return detailsView;
        }

        // A details view uses:
        // DisplayMemberBinding CellTemplate            CellTemplateSelector
        // Header       ColumnHeaderTemplate    ColumnHeaderTemplateSelector
        private GridView CreateDetailsView2()
        {
            GridView detailsView = new GridView();

            GridViewColumn cl1 = new GridViewColumn();
            cl1.DisplayMemberBinding = new Binding("Day");
            cl1.Header = "Day";
            cl1.Width = 80.0;
            detailsView.Columns.Add(cl1);

            GridViewColumn cl2 = new GridViewColumn();
            cl2.CellTemplate = this.Resources["CellTemplate2"] as DataTemplate;
            cl2.HeaderTemplate = this.Resources["ColumnHeaderTemplate2"] as DataTemplate;
            cl2.Width = 80.0;
            detailsView.Columns.Add(cl2);

            GridViewColumn cl3 = new GridViewColumn();
            cl3.CellTemplateSelector = new CellTemplateSelector3();
            cl3.Header = "Year";
            cl3.HeaderTemplateSelector = new ColumnHeaderTemplateSelector3();
            cl3.Width = 80.0;
            detailsView.Columns.Add(cl3);

            return detailsView;
        }

        // DataTemplateSelector for cell
        private class CellTemplateSelector3 : DataTemplateSelector
        {
            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                DataTemplate template = new DataTemplate();

                if (item is DateTime)
                {
                    FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
                    border.SetValue(Border.BorderBrushProperty, Brushes.Red);
                    border.SetValue(Border.BorderThicknessProperty, new Thickness(1.0));

                    FrameworkElementFactory textBlock = new FrameworkElementFactory(typeof(TextBlock));
                    Binding bind = new Binding("Year");
                    bind.Mode = BindingMode.OneWay;
                    textBlock.SetBinding(TextBlock.TextProperty, bind);

                    border.AppendChild(textBlock);

                    template.VisualTree = border;
                }

                else
                {
                    FrameworkElementFactory textBlock = new FrameworkElementFactory(typeof(TextBlock));
                    textBlock.SetValue(TextBlock.TextProperty, "Provided by CelllTemplateSelector3");

                    template.VisualTree = textBlock;
                }

                return template;
            }
        }

        // DataTemplateSelector for column header
        private class ColumnHeaderTemplateSelector3 : DataTemplateSelector
        {
            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                DataTemplate template = new DataTemplate();

                if (item is string)
                {
                    FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
                    border.SetValue(Border.BorderBrushProperty, Brushes.Red);
                    border.SetValue(Border.BorderThicknessProperty, new Thickness(1.0));

                    FrameworkElementFactory textBlock = new FrameworkElementFactory(typeof(TextBlock));
                    textBlock.SetValue(TextBlock.TextProperty, (string)item);

                    border.AppendChild(textBlock);

                    template.VisualTree = border;
                }

                else
                {
                    FrameworkElementFactory textBlock = new FrameworkElementFactory(typeof(TextBlock));
                    textBlock.SetValue(TextBlock.TextProperty, "Provided by ColumnHeaderTemplateSelector3");

                    template.VisualTree = textBlock;
                }

                return template;
            }
        }
    }
}
