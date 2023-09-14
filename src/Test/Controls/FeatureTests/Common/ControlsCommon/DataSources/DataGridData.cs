using System;

namespace Microsoft.Test.Controls
{
#if TESTBUILD_CLR40
    /// <summary>
    /// DataGrid Test Data
    /// </summary>
    public class DataGridData
    {
        public DataGridData(String stringType, Nullable<int> intType, Nullable<double> doubleType, Nullable<bool> boolType, Nullable<DateTime> structType)
        {
            StringType = stringType;
            IntType = intType;
            DoubleType = doubleType;
            BoolType = boolType;
            UriType = new Uri("http://www." + stringType.ToLower().Replace(' ', '_') + ".com/");
            StructType = structType;
        }
        public String StringType { set; get; }
        public Nullable<int> IntType { set; get; }
        public Nullable<double> DoubleType { set; get; }
        public Nullable<bool> BoolType { set; get; }
        public Uri UriType { set; get; }
        public Nullable<DateTime> StructType { set; get; }
    }
#endif
}
