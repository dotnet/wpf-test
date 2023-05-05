// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.ComponentModel;
using System.Data.SqlTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// Data Source for SqlTypes tests
    /// Add more meat later as needed
    /// </summary>
    public class SqlTypesDataSource : DataTable
    {
        #region Fields

        private SqlInt16 _myint16 = new SqlInt16(1);
        private SqlBoolean _myboolean = new SqlBoolean(true);
        private SqlByte _mybyte = new SqlByte(1); // 
        private SqlDateTime _mydatetime = new SqlDateTime(9999, 12, 31);
        private SqlDecimal _mydecimal = new SqlDecimal(10.23);
        private SqlDouble _mydouble = new SqlDouble(168.234);
        private SqlGuid _myguid = new Guid("3AAAAAAA-BBBB-CCCC-DDDD-2EEEEEEEEEEE");
        private SqlInt32 _myint32 = new SqlInt32(123456);
        private SqlMoney _mymoney = new SqlMoney(1000000); // awesome!
        private SqlSingle _mysingle = new SqlSingle(2.6);
        private SqlString _mystring = "this is some text";

        #endregion

        #region Constructor

        public SqlTypesDataSource()
        {
            this.TableName = "SqlTypesDSO";

            this.Columns.Add("ID", typeof(System.Data.SqlTypes.SqlInt16));
            this.Columns.Add("MyBool", typeof(System.Data.SqlTypes.SqlBoolean));
            this.Columns.Add("MyByte", typeof(System.Data.SqlTypes.SqlByte));
            this.Columns.Add("MyDateTime", typeof(System.Data.SqlTypes.SqlDateTime));
            this.Columns.Add("MyDecimal", typeof(System.Data.SqlTypes.SqlDecimal));
            this.Columns.Add("MyDouble", typeof(System.Data.SqlTypes.SqlDouble));
            this.Columns.Add("MyGuid", typeof(System.Data.SqlTypes.SqlGuid));
            this.Columns.Add("MyInt32", typeof(System.Data.SqlTypes.SqlInt32));
            this.Columns.Add("MyMoney", typeof(System.Data.SqlTypes.SqlMoney));
            this.Columns.Add("MySingle", typeof(System.Data.SqlTypes.SqlSingle));
            this.Columns.Add("MyString", typeof(System.Data.SqlTypes.SqlString));

            this.Rows.Add(_myint16, true, _mybyte, _mydatetime, _mydecimal, _mydouble, _myguid, _myint32, _mymoney, _mysingle, _mystring);
            this.Rows.Add(SqlInt16.Null, true, _mybyte, _mydatetime, _mydecimal, _mydouble, _myguid, _myint32, _mymoney, _mysingle, _mystring);
            this.Rows.Add(new SqlInt16(3), SqlBoolean.Null, _mybyte, _mydatetime, _mydecimal, _mydouble, _myguid, _myint32, _mymoney, _mysingle, _mystring);
            this.Rows.Add(new SqlInt16(4), false, SqlByte.Null, _mydatetime, _mydecimal, _mydouble, _myguid, _myint32, _mymoney, _mysingle, _mystring);
            this.Rows.Add(new SqlInt16(5), true, _mybyte, SqlDateTime.Null, _mydecimal, _mydouble, _myguid, _myint32, _mymoney, _mysingle, _mystring);
            this.Rows.Add(new SqlInt16(6), false, _mybyte, _mydatetime, SqlDecimal.Null, _mydouble, _myguid, _myint32, _mymoney, _mysingle, _mystring);
            this.Rows.Add(new SqlInt16(7), true, _mybyte, _mydatetime, _mydecimal, SqlDouble.Null, _myguid, _myint32, _mymoney, _mysingle, _mystring);
            this.Rows.Add(new SqlInt16(8), false, _mybyte, _mydatetime, _mydecimal, _mydouble, SqlGuid.Null, _myint32, _mymoney, _mysingle, _mystring);
            this.Rows.Add(new SqlInt16(9), true, _mybyte, _mydatetime, _mydecimal, _mydouble, _myguid, SqlInt32.Null, _mymoney, _mysingle, _mystring);
            this.Rows.Add(new SqlInt16(10), false, _mybyte, _mydatetime, _mydecimal, _mydouble, _myguid, _myint32, SqlMoney.Null, _mysingle, _mystring);
            this.Rows.Add(new SqlInt16(11), true, _mybyte, _mydatetime, _mydecimal, _mydouble, _myguid, _myint32, _mymoney, SqlSingle.Null, _mystring);
            this.Rows.Add(new SqlInt16(12), false, _mybyte, _mydatetime, _mydecimal, _mydouble, _myguid, _myint32, _mymoney, _mysingle, SqlString.Null);
            this.Rows.Add(new SqlInt16(14), SqlBoolean.Null, SqlByte.Null, SqlDateTime.Null, SqlDecimal.Null, SqlDouble.Null, SqlGuid.Null, SqlInt32.Null, SqlMoney.Null, SqlSingle.Null, SqlString.Null);
        }

        #endregion
    }
}
