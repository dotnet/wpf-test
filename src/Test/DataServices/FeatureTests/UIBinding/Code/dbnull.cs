// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Xml;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// 


    [Test(3, "Binding", "DbNull")]
    public class DbNull : WindowTest
    {
        ListBox _lb;
        ArrayList _list;
        public DbNull () 
        {
         RunSteps += new TestStep(Step1);
         RunSteps += new TestStep(Verify);
        }

        TestResult Step1()
        {
           
            _list = new ArrayList();
            _list.Add(new MyPropeties("Boolean is Null", 32, 32.32, new DateTime(2000, 2, 26), null));
            _list.Add(new MyPropeties("Date is Null", 32, 32.32, null, true));
            _list.Add(new MyPropeties("Double is Null", 2, null, new DateTime(1993, 2, 2), false));
            _list.Add(new MyPropeties("Integer is Null", null, 1.2, new DateTime(1959, 4, 12), true));
            _list.Add(new MyPropeties(null, 24, 124.2, new DateTime(1961, 6, 11), false));
            _list.Add(new MyPropeties("The Rest are null", null, null, null, null));


            DataTable table = new DataTableSource(_list);
            DockPanel dp = new DockPanel();
            dp.Height = 500d;
            dp.Width = 1000d;
            _lb = new ListBox();
            _lb.ItemsSource = table.DefaultView;

            DataTemplate dt = new DataTemplate();
            FrameworkElementFactory doctemplate = new FrameworkElementFactory(typeof(DockPanel));
            FrameworkElementFactory bordertemplate;
            FrameworkElementFactory txttemplate;
            Binding b;

            for (int i = 0; i < table.Columns.Count; i++)
            {
                txttemplate = new FrameworkElementFactory(typeof(TextBlock));
                b = new Binding(table.Columns[i].ColumnName);
                txttemplate.SetBinding(TextBlock.TextProperty, b);
                txttemplate.SetValue(TextBlock.NameProperty, "column_" + table.Columns[i].ColumnName);
                bordertemplate = new FrameworkElementFactory(typeof(Border));
                bordertemplate.SetValue(Border.WidthProperty, 135.0);
                bordertemplate.AppendChild(txttemplate);
                doctemplate.AppendChild(bordertemplate);

           }

            dt.VisualTree = doctemplate;
            _lb.ItemTemplate = dt;
            dp.Children.Add(_lb);
            Window.Content = dp;

            WaitForPriority(System.Windows.Threading.DispatcherPriority.Render);
            return TestResult.Pass;
        }


        TestResult Verify()
        {
            int i = 0;
            FrameworkElement[] column_Name = Util.FindElements(_lb, "column_Name");
            foreach (UIElement visual in column_Name)
            {
                if (((MyPropeties)_list[i]).MyString != ((TextBlock)visual).Text && ((TextBlock)visual).Text != "")
                {
                    LogComment("Expected Value: " + ((MyPropeties)_list[i]).MyString.ToString() + " - Actual: " + ((TextBlock)visual).Text);
                    return TestResult.Fail;
                }
                i++;
            }

            i = 0;
            FrameworkElement[] column_Value = Util.FindElements(_lb, "column_Value");
            foreach (UIElement visual in column_Value)
            {
                if (((MyPropeties)_list[i]).MyInteger.ToString() != ((TextBlock)visual).Text && ((TextBlock)visual).Text != "")
                {
                    LogComment("Expected Value: " + ((MyPropeties)_list[i]).MyInteger.ToString() + " - Actual: " + ((TextBlock)visual).Text);
                    return TestResult.Fail;
                }
                i++;
            }
            i = 0;
            FrameworkElement[] column_Price = Util.FindElements(_lb, "column_Price");
            foreach (UIElement visual in column_Price)
            {
                Nullable<Double> expected = (Nullable<double>)((MyPropeties)_list[i]).MyDouble;
                string actualString = ((TextBlock)visual).Text;

                if ((expected != null) && (actualString != ""))
                {
                    Nullable<Double> actual = new Nullable<Double>(Double.Parse(actualString, new CultureInfo("en-US")));

                    if (!(expected.Equals(actual)))
                    {
                        LogComment("Expected Value: " + expected + " - Actual: " + actual);
                        return TestResult.Fail;
                    }
                    i++;
                }
            }
            i = 0;
            FrameworkElement[] column_Date = Util.FindElements(_lb, "column_Date");
            foreach (UIElement visual in column_Date)
            {
                Nullable<DateTime> expectedDate = (Nullable<DateTime>)((MyPropeties)_list[i]).MyDate;
                string actual = ((TextBlock)visual).Text;
                if (expectedDate != null)
                {
                    string expected = ((DateTime)expectedDate).ToString(new CultureInfo("en-US"));

                    if (!(expected.Equals(actual)))
                    {
                        LogComment("Expected Value: " + expectedDate + " - Actual: " + actual);
                        return TestResult.Fail;
                    }
                }
                else
                {
                    if (actual != "")
                    {
                        LogComment("Expected Value: " + expectedDate + " - Actual: " + actual);
                        return TestResult.Fail;
                    }
                }
                i++;
            }
            i = 0;
            FrameworkElement[] column_Boolean = Util.FindElements(_lb, "column_Flag");
            foreach (UIElement visual in column_Boolean)
            {
                if (((MyPropeties)_list[i]).MyBoolean.ToString() != ((TextBlock)visual).Text && ((TextBlock)visual).Text != "")
                {
                    LogComment("Expected Value: " + ((MyPropeties)_list[i]).MyBoolean.ToString() + " - Actual: " + ((TextBlock)visual).Text);
                    return TestResult.Fail;
                }
                i++;
            } 

            
            return   TestResult.Pass;
        }

    }
}
