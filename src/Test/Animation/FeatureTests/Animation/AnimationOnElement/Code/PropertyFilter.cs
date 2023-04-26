// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// $Id:$ $Change:$
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text.RegularExpressions;
using System.Globalization;


namespace Microsoft.Test.Animation
{

    internal struct PropertyFilter
    {
        private string _typeName;
        private string _dataFilter;
        private int[] _bugIds;

        public PropertyFilter(string typeName, string dataFilter, string bugIds)
        {
            _typeName = typeName;
            _dataFilter = dataFilter;
            _bugIds = null;
        }

        public string typeName
        {
            get
            {
                return _typeName;
            }
        }
        public string dataFilter
        {
            get
            {
                return _dataFilter;
            }
        }

        public int[] BugIds
        {
            get
            {
                return _bugIds;
            }

        }

    }



    // singleton
    public class PropertyFilterTable
    {
        private static ArrayList s_filters = null;
        private static PropertyFilterTable s_filterTable = null;
        private static string s_filterString = String.Empty;
        private Type _currentObjectType = null;

        public override string ToString()
        {
            string output = String.Empty;
            foreach (PropertyFilter filter in s_filters)
            {
                output += filter.typeName + ": " + filter.dataFilter + "\n";
            }
            return output;
        }


        protected PropertyFilterTable()
        {
            s_filters = new ArrayList();

            PropertyFilter currentFilter;

            currentFilter = new PropertyFilter("System.Object", "Timeline", "Bug - Windows OS Bugs");
            s_filters.Add(currentFilter);

            currentFilter = new PropertyFilter("System.Object", "InputLanguage RestoreInputLanguage IsInputMethodEnabled PreferredImeState PreferredImeConversionMode PreferredImeSentenceMode", "Bug  - Windows OS Bugs");
            s_filters.Add(currentFilter);

            currentFilter = new PropertyFilter("System.Object", "ComboBox.SelectedItems", "Bug  - Windows OS Bugs");
            s_filters.Add(currentFilter);

            currentFilter = new PropertyFilter("System.Object", "ComboBox.SelectedIndex", "Bug  - Windows OS Bugs");
            s_filters.Add(currentFilter);

            currentFilter = new PropertyFilter("System.Windows.FrameworkContentElement,PresentationFramework", "Foreground Background BorderBrush TextDecorationPen", "Bug  - Windows OS Bugs");
            s_filters.Add(currentFilter);

            currentFilter = new PropertyFilter("System.Windows.Document.TextPanel", "Foreground", "Bug  - Windows OS Bugs");
            s_filters.Add(currentFilter);

            currentFilter = new PropertyFilter("System.Windows.Controls.TextBlock", "Foreground TextDecorationPen", "Bug  - Windows OS Bugs");
            s_filters.Add(currentFilter);

            currentFilter = new PropertyFilter("System.Windows.Controls.CheckBox,PresentationFramework", "Checked", "Bug  - Windows OS Bugs");
            s_filters.Add(currentFilter);

            currentFilter = new PropertyFilter("System.Windows.Controls.Primitives.Slider,PresentationFramework", "Style", "Bug  - Windows OS Bugs");
            s_filters.Add(currentFilter);

            currentFilter = new PropertyFilter("System.Windows.Controls.Primitives.Slider,PresentationFramework", "Value", "Bug  - Windows OS Bugs");
            s_filters.Add(currentFilter);
        }

        public static PropertyFilterTable Instance()
        {
            if (s_filterTable == null)
            {
                s_filterTable = new PropertyFilterTable();
            }
            return s_filterTable;
        }

        public string GetFilterString(object originalElement)
        {
            // check to see if we have processed a type yet, or if the type being
            // passed in is different than the one we previously processed
            if (_currentObjectType == null || !_currentObjectType.Equals(originalElement.GetType()))
            {
                _currentObjectType = originalElement.GetType();
                s_filterString = GenerateFilterString(originalElement);
            }

            return s_filterString;
        }


        private string GenerateFilterString(object originalElement)
        {
            string filterString = String.Empty;

            foreach (PropertyFilter filter in s_filters)
            {
                Type currentType = Type.GetType(filter.typeName);
//                    try
//                    {
//                        Type currentType = Type.GetType(filter.typeName, true);
//                    }
//                    catch (System.TypeLoadException e)
//                    {
//                        Log2("handled exception:" + e.Message);
//                    }

                if (currentType != null)
                {
                    if (currentType.IsInstanceOfType(originalElement))
                    {
                        filterString += filter.dataFilter + " ";
                    }
                }
            }
            
            
            // now clean up the filter string, which just has the list of things to filter
            // so that it's regex-friendly.
            filterString = filterString.Trim();
            filterString = filterString.Replace(" ", "|");
            filterString = filterString.Replace(".", "\\.");

            return filterString;
        }
    }
}
