// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Test.DataServices;
using Microsoft.Test;

namespace Microsoft.Test.DataServices
{
	public class MySlowDataSource : DataSourceProvider
	{
        protected override void BeginQuery()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(FillDataCallback), null);
        }

        private object FillDataCallback(object obj)
        {
            FillData();
            return null;
        }

		private void FillData()
		{
            ObservableCollection<FastDataItem> fastDataItemList = new ObservableCollection<FastDataItem>();

            fastDataItemList.Add(new FastDataItem("FastValue1"));
			fastDataItemList.Add(new FastDataItem("FastValue2"));
			fastDataItemList.Add(new FastDataItem("FastValue3"));
			fastDataItemList.Add(new FastDataItem("FastValue4"));
			fastDataItemList.Add(new FastDataItem("FastValue5"));

            OnQueryFinished(fastDataItemList);
		}
	}

	public class FastDataItem : System.ComponentModel.INotifyPropertyChanged
	{
		public FastDataItem()
		{
			_fastvalue = "This is fast!";
		}

		public FastDataItem(string fv)
		{
			_fastvalue = fv;
		}

		public string FastValue
		{
			get { return _fastvalue; }
			set
			{
				_fastvalue = value;
				RaisePropertyChangedEvent("FastValue");
			}
		}

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChangedEvent(string name)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
			}
		}

		private string _fastvalue;
	}
}
