// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Threading;
using System.Globalization;
using System.Collections;
using System.Windows.Controls;
using Microsoft.Test;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// This test is to make sure nested PriorityBinding is not supported.
    /// </description>
    /// </summary>
    [Test(1, "Binding", "PBnesting")]
    public class PBnesting : WindowTest
    {
        System.Windows.Data.PriorityBinding _pb;
        System.Windows.Data.PriorityBinding _pb2;
        TextBox _tb1;
        DockPanel _dp;

        ObservableCollection<CLRBook> _verifyArrayList;
        Binding _bAuthor;
        Binding _bTitle;

        public PBnesting()
        {
            InitializeSteps += new TestStep(CreateTest);
            RunSteps += new TestStep(AddanotherPB);

        }
        private ObservableCollection<CLRBook> CreateData()
        {
            ObservableCollection<CLRBook> books = new ObservableCollection<CLRBook>();
            CLRBook item = new CLRBook("Homo Faber", "Max Frisch", 1957, 14.92, BookType.Novel);
            books.Add(item);
            item = new CLRBook("The Fourth Hand", "John Irving", 2001, 14.91, BookType.Novel);
            books.Add(item);
            item = new CLRBook("Inside C#", "Tom Archer e.a.", 2002, 49.99, BookType.Reference);
            books.Add(item);
            item = new CLRBook("A Man in Full", "Tom Wolfe", 1998, 8.95, BookType.Novel);
            books.Add(item);


            return books;
        }
        private TestResult CreateTest()
        {
            _verifyArrayList = CreateData();

            //creating a prioritybind
            _tb1 = new TextBox();
            _pb = new System.Windows.Data.PriorityBinding();
 
            _bAuthor = new Binding("Author");
            _pb.Bindings.Add(_bAuthor);

            MarkupExtensionServiceProvider serviceProvider = new MarkupExtensionServiceProvider( _tb1, TextBox.TextProperty );
            _tb1.SetValue(TextBox.TextProperty, ((MarkupExtension)_pb).ProvideValue(serviceProvider));

            _dp = new DockPanel();
            _dp.DataContext = CreateData();
            _dp.Children.Add(_tb1);
            Window.Content = _dp;
            return TestResult.Pass;
        }
        private TestResult AddanotherPB()
        {
            _pb2 = new System.Windows.Data.PriorityBinding();
            _bTitle = new Binding("Title");
            _pb2.Bindings.Add(_bTitle);
            Status("Catching the exception when you add PriorityBing to PriorityBinding");
            SetExpectedErrorTypeInStep(typeof(NotSupportedException));
            _pb.Bindings.Add(_pb2);
            return TestResult.Fail;
        }

    }

}




