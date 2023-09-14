using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Avalon.Test.ComponentModel;
using Microsoft.Test;

namespace Microsoft.Test.Controls.Helpers
{
    /// <summary>
    /// These methods are in lieu of "extension" methods featured in .Net 3.5,
    /// but not available in .Net 3.0. Without the "extension" feature these 
    /// these can still be scoped together.
    /// </summary>
    public static class GridSplitterGridLengthExtend
    {
        /// <summary>
        /// A customized, ToString() for GridLength objects
        /// </summary>
        public static String GetString(GridLength g)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Value:{0:F3}, ", g.Value.ToString());
            sb.AppendFormat("UnitType:{0} ", g.GridUnitType.ToString());
            return sb.ToString();
        }
    }
}
