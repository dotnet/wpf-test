// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Windows;


namespace Microsoft.Test.DataServices
{
    public class DataTableSource : DataTable
    {
        public DataTableSource()
        {
            CreateDataTable(10, null);
        }

        public DataTableSource(string maxrows)
        {
            CreateDataTable(int.Parse(maxrows), null);
        }

        public DataTableSource(int maxrows)
        {
            CreateDataTable(maxrows, null);
        }

        //ArrayList needs to be comprised of ADOProperties object
        public DataTableSource(System.Collections.ArrayList list)
        {
            //if empty list add  default list.
            if (list == null)
            {
                list = new System.Collections.ArrayList();
                list.Add(new MyPropeties("Boolean is Null", 32, 32.32, new DateTime(2000, 2, 26), null));
                list.Add(new MyPropeties("Date is Null", 32, 32.32, null, true));
                list.Add(new MyPropeties("Double is Null", 2, null, new DateTime(1993, 2, 2), false));
                list.Add(new MyPropeties("Integer is Null", null, 1.2, new DateTime(1959, 4, 12), true));
                list.Add(new MyPropeties(null, 24, 124.2, new DateTime(1961, 6, 11), false));
                list.Add(new MyPropeties("The Rest are null", null, null, null, null));
            }
            CreateDataTable(null, list);

        }

        private void CreateDataTable(Nullable<int> maxrows, System.Collections.ArrayList list)
        {
            this.TableName = "Test";
            this.Columns.Add("Name", typeof(System.String));
            this.Columns.Add("Value", typeof(System.Int32));
            this.Columns.Add("Price", typeof(System.Double));
            this.Columns.Add("Date", typeof(System.DateTime));
            this.Columns.Add("Flag", typeof(System.Boolean));


            if (list != null)
            {
                DataRow row;
                foreach (MyPropeties item in list)
                {
                    row = this.NewRow();
                    if (item.MyString != null)
                        row["Name"] = (string)item.MyString;
                    else
                        row["Name"] = System.DBNull.Value;
                    if (item.MyInteger != null)
                        row["Value"] = (int)item.MyInteger;
                    else
                        row["Value"] = System.DBNull.Value;
                    if (item.MyDouble != null)
                        row["Price"] = (double)item.MyDouble;
                    else
                        row["Price"] = System.DBNull.Value;
                    if (item.MyDate != null)
                        row["Date"] = (DateTime)item.MyDate; 
                    else
                        row["Date"] = System.DBNull.Value;
                    if (item.MyBoolean != null)
                        row["Flag"] = (bool)item.MyBoolean;
                    else
                        row["Flag"] = System.DBNull.Value;
                    this.Rows.Add(row);
                }

            }
            else
            {
                for (int i = 0; i < maxrows; ++i)
                {
                    DataRow row = this.NewRow();

                    row["Name"] = "TableData " + i.ToString();
                    row["Value"] = i;
                    row["Price"] = (.37 * i + 5);
                    DateTime dt = new DateTime(2000, 2, 26);
                    row["Date"] = dt.AddDays(double.Parse(i.ToString()));
                    int choice;
                    Math.DivRem(i, 2, out choice);
                    if (choice == 0)
                        row["Flag"] = true;
                    else if (choice == 1)
                        row["Flag"] = false;
                    this.Rows.Add(row);
                }
            }
        }
    }
    public class MyPropeties
    {
        private String _string;
        private Nullable<int> _int;
        private Nullable<double> _double;
        private Nullable<DateTime> _date;
        private Nullable<bool> _bool;

        public MyPropeties(object str, object i, object dble, object date, object b)
        {
            if (str != null) _string = (string)str;
            if (i != null) _int = (int)i;
            if (dble != null) _double = (double)dble;
            if (date != null) _date = (DateTime)date;
            if (b != null) _bool = (bool)b;


        }
        public string MyString
        {
            get { return _string; }
            set { _string = value; }
        }

        public Nullable<int> MyInteger
        {
            get { return _int; }
            set { _int = value; }
        }
        public Nullable<double> MyDouble
        {
            get { return _double; }
            set { _double = value; }
        }

        public Nullable<DateTime> MyDate
        {
            get { return _date; }
            set { _date = value; }
        }

        public Nullable<Boolean> MyBoolean
        {
            get { return _bool; }
            set { _bool = value; }
        }

    }
}
