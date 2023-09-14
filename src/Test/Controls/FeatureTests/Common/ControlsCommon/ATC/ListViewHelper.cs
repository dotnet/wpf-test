//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Xml;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System.Windows.Documents;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Markup;

using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Logging;
using Avalon.Test.ComponentModel.Utilities;
using Avalon.Test.ComponentModel.Validations;


namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// help class for testing listview control
    /// </summary>
    static class ListViewHelper
    {
        private const int c_Tolerance = 3;

        /// <!--summary>
        /// get (row, GridViewColumn) cell text
        /// </summary>
        /// <param name="lv">ListView</param>
        /// <param name="row">index of row</param>
        /// <param name="GridViewColumn">index of GridViewColumn</param>
        /// <returns></returns-->
        static public string GetColumnCellText(ListView lv, int row, int column)
        {
            ListViewItem lvi = (ListViewItem)VisualTreeUtils.FindPartByType(lv, typeof(ListViewItem), row);
            GlobalLog.LogStatus("ListViewItem :" + lvi);

            GridViewRowPresenter rp = (GridViewRowPresenter)VisualTreeUtils.FindPartByType(lvi, typeof(GridViewRowPresenter), 0);
            GlobalLog.LogStatus("GridViewRowPresenter :" + rp);

            //ContentPresenter cp = (ContentPresenter)VisualTreeUtils.FindPartByType(rp, typeof(ContentPresenter), column);
            //GlobalLog.LogStatus("ContentPresenter :" + cp.Content);

            TextBlock tb = (TextBlock)VisualTreeUtils.FindPartByType(rp, typeof(TextBlock), column);
            GlobalLog.LogStatus("TextBlock Text :" + tb.Text);

            //TextBlock tb = VisualTreeUtils.FindPartByType(cp, typeof(TextBlock), 0) as TextBlock;
            //GlobalLog.LogStatus("TextBlock.Text :" + tb);

            return tb.Text;
        }

        /// <summary>
        /// get dummy header from listview
        /// </summary>
        /// <param name="lv"></param>
        /// <returns></returns>
        static public GridViewColumnHeader GetDummyHeader(ListBox lv)
        {
            return VisualTreeUtils.FindPartByType(lv, typeof(GridViewColumnHeader), 0) as GridViewColumnHeader;
        }

        /// <summary>
        /// verify if the listviewitem is highlight, 
        /// add this method becouse of BC: move style trigger into controltemplate
        /// </summary>
        /// <param name="lvi"></param>
        /// <returns></returns>
        static public bool IsHighLightItem(ListViewItem lvi)
        {
            if (lvi == null)
                throw new ArgumentNullException("lvi");
            Border bd = VisualTreeUtils.FindPartByType(lvi, typeof(Border), 0) as Border;
            if (bd == null)
                throw new InvalidOperationException("can not find border in listview");

            string theme = DisplayConfiguration.GetTheme();

            if (theme.ToLower() == "aero")
            {
                LinearGradientBrush lgb = bd.Background as LinearGradientBrush;
                if (lgb == null || lgb.GradientStops.Count != 2)
                {
                    return false;
                }
                if (lvi.IsMouseOver == true)
                {
                    if (lgb.GradientStops[0].Color != (Color)ColorConverter.ConvertFromString("#FFEAF9FF")
                        || lgb.GradientStops[1].Color != (Color)ColorConverter.ConvertFromString("#FFC9EDFD"))
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    if (lgb.GradientStops[0].Color != (Color)ColorConverter.ConvertFromString("#FFD9F4FF")
                        || lgb.GradientStops[1].Color != (Color)ColorConverter.ConvertFromString("#FF9BDDFB"))
                    {
                        return false;
                    }
                    return true;
                }
            }
            else
            {
                return bd.Background == SystemColors.HighlightBrush;
            }
        }

        /// <summary>
        /// get normal header from listview
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        static public GridViewColumnHeader GetNormalColumnHeader(ListBox lv, int column)
        {
            GridViewHeaderRowPresenter chp = (GridViewHeaderRowPresenter)VisualTreeUtils.FindPartByType(lv, typeof(GridViewHeaderRowPresenter), 0);
            GridViewColumnHeader[] cols = (GridViewColumnHeader[])VisualTreeUtils.FindPartByType(chp, typeof(GridViewColumnHeader)).ToArray(typeof(GridViewColumnHeader));
            if (cols == null || column >= cols.Length)
            {
                return null;
            }

            return cols[cols.Length - 2 - column];
        }

        /// <summary>
        /// create binding according to Path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public Binding CreateBinding(string path)
        {
            Binding bind = new Binding();
            if (path == null)
            {
                return bind;
            }
            string strPath = path.Trim();

            if (strPath == string.Empty || strPath.Length < 1)
            {
                return bind;
            }
            if (strPath[0] == '@')
            {
                bind.XPath = path;
            }
            else
            {
                bind.Path = new PropertyPath(path, new object[0]);
            }
            return bind;
        }


        /// <summary>
        /// if two DateTime have the same year/month/day
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        static public bool IsEqualDate(DateTime first, DateTime second)
        {
            if (first.Year == second.Year && first.Month == second.Month && first.Day == second.Day)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// test two kinds of color(System.Drawing.Color and System.Windows.Media.Color)
        /// if they stand for the same color
        /// </summary>
        /// <param name="col1"></param>
        /// <param name="col2"></param>
        /// <returns></returns>
        static public bool IsEqual(System.Drawing.Color col1, System.Windows.Media.Color col2)
        {
            GlobalLog.LogStatus("Col1:" + col1.ToString());
            GlobalLog.LogStatus("Col2:" + col2.ToString());

            //test Red, Green, Blue component value, not include alpha
            if (Math.Abs(col1.R - col2.R) < c_Tolerance && Math.Abs(col1.G - col2.G) < c_Tolerance && Math.Abs(col1.B - col2.B) < c_Tolerance)
            {
                return true;
            }

            return false;
        }

        /// <!--summary>
        /// get GridViewColumn header text
        /// </summary>
        /// <param name="lv">ListView</param>
        /// <param name="GridViewColumn">index of GridViewColumn header</param>
        /// <returns></returns-->
        static public string GetColumnHeaderText(ListView lv, int column)
        {
            GridViewColumnHeader header = GetNormalColumnHeader(lv, column);
            if (header != null)
            {
               return "" + header.Content;
            }
            return null;
        }

        /// <summary>
        /// create custom ItemContainerStyle for GridView
        /// </summary>
        /// <returns></returns>
        static public Style CreateItemContainerStyle()
        {
            Style style = new Style();
            style.TargetType = typeof(ListViewItem);

            //set BorderBrush to Red
            Setter borderBrushSetter = new Setter();
            borderBrushSetter.Property = ListViewItem.BorderBrushProperty;
            borderBrushSetter.Value = Brushes.Red;
            style.Setters.Add(borderBrushSetter);

            //set BorderThickness to (2,2,2,2)
            Setter borderThicknessSetter = new Setter();
            borderThicknessSetter.Property = ListViewItem.BorderThicknessProperty;
            borderThicknessSetter.Value = new Thickness(2);
            style.Setters.Add(borderThicknessSetter);

            return style;
        }

        static public bool VerifyItemContainerStyle(ListViewItem lvi)
        {
            return (lvi.BorderBrush == Brushes.Red) && (lvi.BorderThickness == new Thickness(2));
        }
    }
}

