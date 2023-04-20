// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Input;


namespace Microsoft.Test.DataServices
{
	public class SelectedItemSource : INotifyPropertyChanged
	{
        private Place _placeSource;

        public Place PlaceSource
        {
            get { return _placeSource; }
            set
            {
                _placeSource = value;
                RaisePropertyChangedEvent("PlaceSource");
            }
        }

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChangedEvent(string name)
		{
			if (PropertyChanged != null)
			{
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("PlaceSource"));
            }
		}

    }
}
