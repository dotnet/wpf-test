// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Test that second thread is blocked from 
	/// adding into underlining collection. 
	/// </description>
	/// </summary>
    [Test(3, "Collections", "MultiThreadALDC")]
    public class MultiThreadALDC : XamlTest
    {

        #region Test Case

        #region Test case members

        private ObjectDataProvider _dso;
        private Library _library;
        private ListBox _testListBox;
        private Thread _testThread;
        private TestResult _testresult = TestResult.Fail;
		private int _eventcalled = 0;

        #endregion

        protected Library MyLibrary
        {
            get { return _library; }
            set { _library = value; }
        }

        public MultiThreadALDC()
            : base(@"CollectionTest.xaml")
        {
            InitializeSteps += new TestStep(SetUp);
			 RunSteps += new TestStep(Check);
        }

        // Reference the objects needed, start the second thread
        private TestResult SetUp()
        {
            
            _dso = RootElement.Resources["DSO"] as ObjectDataProvider;
            if (_dso == null)
            {
                LogComment("DataSource is null");
                return TestResult.Fail;
            }

            if (_dso.Data == null)
            {
                LogComment("DataSource.Data is null");
                return TestResult.Fail;
            }


            _testListBox = (ListBox)Util.FindElement(RootElement, "testListBox");
            if(_testListBox == null)
            {
                LogComment("DataSource.Data is null");
                return TestResult.Fail;
            }

            MyLibrary = (Library)_dso.Data;

            ICollectionView cv = _testListBox.Items;
            if (cv == null)
            {
                LogComment("ICollectionView is NULL");
                return TestResult.Fail;
            }

            cv.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionView_CollectionChanged);

            _testThread = new Thread(new ThreadStart(SecondThread));
            _testThread.Start();
            return TestResult.Pass;
        }

        // Collection Changed event handler
        void CollectionView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

			//  This Event should never get called, because modification from another thread should throw.
			//Increment counter incase it gets called. 
			_eventcalled++;
			
        }

        // Handles swapping to the other thread and calls the validate function
        private TestResult Check()
        {
			if (_testThread.Join(new TimeSpan(0, 1, 0)))
			{
				if (_eventcalled == 0)
				{
					Status("NotifyCollectionChangedEvent fired '" + _eventcalled.ToString() + "' times");
					return _testresult;
				}
				else
				{
					LogComment("NotifyCollectionChangedEvent was fired '" + _eventcalled.ToString() + "' times.  It shouldn't have fired");
					return TestResult.Fail;
				}
			}
			return TestResult.Fail;

        }

        /// <summary>
        /// The other thread to spin up and modify the collection with
        /// </summary>
        public void SecondThread()
        {

            Console.WriteLine("SecondThread");

            Status("Passed to my thread, adding a book");
			try
			{
				Add(new Book(100.1));
			}

			catch (Exception e)
			{
				if (e.GetType() != typeof(NotSupportedException))
					_testresult = TestResult.Fail;
				else
					_testresult = TestResult.Pass;
			}
			finally
			{

			}


        }

        #endregion

        #region Helper Functions

  
        #region Remove, Add, and Insert Methods, with locks

        protected void RemoveAt(int index)
        {
            lock (MyLibrary.SyncRoot)
            {
                MyLibrary.RemoveAt(index);
            }
        }

		protected void Add(Book book)
		{
            lock (MyLibrary.SyncRoot)
            {
                MyLibrary.Add(book);
            }
        }


		protected void Insert(int index, Book book)
		{
            lock (MyLibrary.SyncRoot)
            {
                MyLibrary.Insert(index, book);
            }
        }


        #endregion

        #endregion

    }
}

