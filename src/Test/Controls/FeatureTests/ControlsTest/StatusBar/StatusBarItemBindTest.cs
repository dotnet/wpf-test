//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System; //for EventHandler and TimeSpane
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Controls.Primitives;//for StatusBar and StatusBarItem
using System.Xml; //for XmlElement;
using Microsoft.Test.Logging; //for TestResult

namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// This class is used to test DataBinding for StatusBarItem 
    /// Bind a user-defined datasource to StatusBarItem's ContentProperty
    /// To run this test case, one parameter should be assigned: xtc file name
    /// for example : CMLoader.exe StatusBarItemBindTest.xtc
    /// </summary>
    [TargetType(typeof(StatusBar))]
    class StatusBarItemBindTest : IUnitTest
    {
        /// <summary>
        /// Set test counts: default is 5 
        /// </summary>
        private const int c_TestCount = 5; 
        

        /// <summary>
        /// Test databind for StatusBarItem using user-defined DataSource
        /// </summary>
        /// <param name="statusbar"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            StatusBar statusbar;
            StatusBarItem item;

            statusbar = (StatusBar)obj;
            item = new StatusBarItem();
            statusbar.Items.Add(item);

            //create data source and Binding
            MyDataForBind dataSource = new MyDataForBind();
            Binding bind = new Binding("MyData");
            bind.Source = dataSource;
            item.SetBinding(StatusBarItem.ContentProperty, bind);

            GlobalLog.LogStatus("Testing StatusBarItem DataBinding Start....");

            TestResult testResult = TestResult.Pass;

            for (int i = 0; i < c_TestCount; i++)
            {
                dataSource.ChangeDataRandly();
                GlobalLog.LogStatus("\n\rSource:" + dataSource.MyData +
                                    "\n\rDest  :" + item.Content.ToString());
                
                //Compare the Source Data and Destination Data
                if (dataSource.MyData.CompareTo(item.Content) != 0)
                {
                    testResult = TestResult.Fail;
                    GlobalLog.LogStatus("\n\rThe data of DataSource dismatch the item's content property");
                    break;
                }
                else
                {
                    GlobalLog.LogStatus("\n\r   test index " + i + " succeed");
                }

                QueueHelper.WaitTillQueueItemsProcessed();
            }

            //Restore
            statusbar.Items.Remove(item);

            return testResult;
        }
    }


    /// <summary>
    /// This class is used to create a user-defined DataSource
    /// </summary>
    public class MyDataForBind : INotifyPropertyChanged
    {
        private string _data;

        /// <summary>
        /// Constructor
        /// </summary>
        public MyDataForBind()
        {
        }


        /// <summary>
        /// ReadOnly Property
        /// <summary>
        public string MyData
        {
            get
            {
                return _data;
            }
        }


        /// <summary>
        /// public method for change data randly, just for test purpose
        /// <summary>
        public void ChangeDataRandly()
        {
            _data = CreateRandString();
            OnPropertyChanged("MyData");
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// randly create a string, just use a Guid for test purpose 
        /// <summary>
        private string CreateRandString()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }
       
    }
}
