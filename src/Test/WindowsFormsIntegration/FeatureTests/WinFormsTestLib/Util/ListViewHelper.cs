// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;

namespace WFCTestLib.Util
{
    // <doc>
    // <desc>
    //  Collection of methods which allow you to access a listview report row as
    //  if it was a grid (instead of in the Item & SubItem paradigm).  See
    //  VisualStudio Regression_Bug63
    // </desc>
    // </doc>
    public class ListViewHelper
    {
        // <doc>
        // <desc>
        //  Returns an array of Strings containing the text for
        //  nRowIndex of the specified ListView object.  Enables you to treat a
        //  listview item & subitems as if the text was in a contiguous array.
        // </desc>
        // <param term="lv">
        //  listview to get items from
        // </param>
        // <param term="nRowIndex">
        //  index of the row containing the items
        // </param>
        // <retvalue>
        //  array of strings where element 0 is the text of the main item
        // </retvalue>
        static public String[] GetListViewRowItems(ListView lv, int rowIndex)
        {
            ListViewItem row = lv.Items[rowIndex];
            int count   = row.SubItems.Count;

            String[] items = new String [count];
            items [0] = row.Text;

            for (int n = 0; n < count - 1; n++)
                items[n + 1] = row.SubItems[n].ToString();

            return items;
        }

        // <doc>
        // <desc>
        //  Populates the given listview with the array of strings.  If the supplied
        //  string[] length > columns, returns null.
        // </desc>
        // <param term="s">
        //  array of strings where element 0 is the text for the main itemm
        // </param>
        // <param term="columns">
        //  number of columns in the listview
        // </param>
        // <retvalue>
        //  null if string array length > columns, else returns an item with attached subitems
        // </retvalue>
        // </doc>
        static public ListViewItem MakeListViewItem(String[] s, int columns)
        {
            if (s.Length > columns)
                return null;

            ListViewItem item = new ListViewItem(s[0]);

            for (int n = 0; n < columns - 1; n++)
                item.SubItems[n].Text = s[n + 1];

            return item;
        }

        // <doc>
        // <desc>
        //  Returns the text a given ListViewItem subItem.  Treats the
        //  text of the ListViewItem object as a regular subitem.
        // </desc>
        // <param term="row">
        //  ListViewItem containing the subitem text you want
        // </param>
        // <param term="n">
        //  which subitem you want (0 based)
        // </param>
        // <retvalue>
        //  text of sub item if found, else returns ""
        // </retvalue>
        // </doc>
        static public String GetSubItemText(ListViewItem row, int n)
        {
            String s = "";

            if (n == 0)
                return row.Text;
            else
                n = n - 1;

            try
            {
                s = row.SubItems[n].ToString();
            }
            catch (Exception)
            {
                return "";
            }

            return s;
        }

        // <doc>
        // <desc>
        //  Sets the text of a the given ListViewItem.  Treats the text
        //  of the ListViewItem object as a regular subitem.
        // </desc>
        // <param term="row">
        //  ListViewItem you want to set
        // </param>
        // <param term="s">
        //  array of strings containing the text for the item+subitems
        // </param>
        // </doc>
        static public void SetListViewItems(ListViewItem row, String[] s)
        {
            row.Text = (s [0]);

            for (int n = 0; n < s.Length - 1; n++)
                row.SubItems[n].Text = s[n + 1];
        }
    }
}
