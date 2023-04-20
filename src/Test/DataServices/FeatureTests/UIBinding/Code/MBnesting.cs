// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
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
    /// This test is to make sure nested MultiBinding
    /// is not supported.
    /// </description>
    /// </summary>
    [Test(1, "Binding", "MBnesting")]
    public class MBnesting : WindowTest
    {
        MultiBinding _mb;
        MultiBinding _mb2;
        ObservableCollection<CLRBook> _verifyArrayList;
        Binding _bTitle; 
        Binding _bTitle2; 
        TextBox _tb;

        public MBnesting()
        {
            InitializeSteps += new TestStep(CreateTest);
            RunSteps += new TestStep(AddanotherMultiBinding);

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
            //_convertVerifier = new ConverterVerifier();
            _tb = new TextBox();
            _mb = new MultiBinding();
            _mb.Converter = new MBConverter();
            _bTitle = new Binding("Title");
            _mb.Bindings.Add(_bTitle);

            MarkupExtensionServiceProvider serviceProvider = new MarkupExtensionServiceProvider( _tb, TextBox.TextProperty );
            _tb.SetValue(TextBox.TextProperty, ((MarkupExtension)_mb).ProvideValue(serviceProvider));

            DockPanel dp = new DockPanel();
            dp.DataContext = CreateData();
            dp.Children.Add(_tb);
            Window.Content = dp;
            //values = new object[] { ((CLRBook)verifyArrayList[0]).Title, ((CLRBook)verifyArrayList[0]).Year, ((CLRBook)verifyArrayList[0]).Price };
            //step = "Setting up BindList";
            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }
        private TestResult AddanotherMultiBinding()
        {
            _mb2 = new MultiBinding();
            _mb2.Converter = new MBConverter();
            _bTitle2 = new Binding("Title");
            _mb2.Bindings.Add(_bTitle2);

            Status("Catching the exception when you add MultiBing to MultiBinding");
            SetExpectedErrorTypeInStep(typeof(NotSupportedException));

            _mb.Bindings.Add(_mb2);
            
            return TestResult.Fail;

        }   

    }


    // This IServiceProvider implementation is used to call MarkupExtension.ProvideValue,
    // and pass in the target object/property.
    
    internal class MarkupExtensionServiceProvider: IServiceProvider, IProvideValueTarget
    {
        internal MarkupExtensionServiceProvider(object targetObject, object targetProperty)
        {
            _targetObject = targetObject;
            _targetProperty = targetProperty;
        }


        object IProvideValueTarget.TargetObject
        {
            get { return _targetObject; }
        }
        
        object IProvideValueTarget.TargetProperty
        {
            get { return _targetProperty; }
        }

        private object _targetObject = null;
        private object _targetProperty = null;

        public object GetService(Type service)
        {
            if( service == typeof(IProvideValueTarget))
            {
                return this as IProvideValueTarget;
            }

            else
            {
                return null;
            }
                
        }

    }

}



