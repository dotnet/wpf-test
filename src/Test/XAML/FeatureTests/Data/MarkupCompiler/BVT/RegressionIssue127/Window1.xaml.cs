// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace RegressionIssue127
{
    /// <summary>
    /// NullRefException when placing event handler on button within the data template of a nested ItemsControl
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            DataContext = new MyData();
        }

        protected void WindowActivated(object sender, EventArgs e)
        {
            Application.Current.Shutdown(0);
        }
        
        protected void ButtonClick(object sender, EventArgs e)
        {
        }
    }

    class MyData
    {
        private List<Class1> _children = new List<Class1>();
        public List<Class1> Children { get { return _children; } }

        public MyData() { _children.Add(new Class1()); }

    }



    class Class1
    {
        private List<string> _children = new List<string>();
        public List<string> Children { get { return _children; } }

        public Class1() { _children.Add("1"); }

    }
}
