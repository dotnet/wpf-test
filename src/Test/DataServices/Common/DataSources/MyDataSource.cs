// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Test;
using System.Globalization;

namespace Microsoft.Test.DataServices
{
   public class MyDataSource : DataSourceProvider
   {
        protected override void BeginQuery()
        {
             MyDataClass mdc = new MyDataClass("Hello", "Goodbye");
             OnQueryFinished(mdc);
        }
   }

   public class MyDataClass : INotifyPropertyChanged
   {
      private string _myData1;
      public string MyData1
      {
         get { return _myData1; }
         set
         {
            _myData1 = value;
            RaisePropertyChangedEvent("MyData1");
         }
      }

      private string _myData2;
      public string MyData2
      {
         get { return _myData2; }
         set
         {
            _myData2 = value;
            RaisePropertyChangedEvent("MyData2");
         }
      }

      public MyDataClass(string myDataStr1, string myDataStr2)
      {
         this._myData1 = myDataStr1;
         this._myData2 = myDataStr2;
      }

      public event PropertyChangedEventHandler PropertyChanged;

      private void RaisePropertyChangedEvent(string name)
      {
         if (PropertyChanged != null)
         {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
         }
      }
   }
}

