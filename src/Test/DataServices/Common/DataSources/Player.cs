// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Data;
using System.ComponentModel;

namespace Microsoft.Test.DataServices
{
	public class Player : INotifyPropertyChanged
	{
		private string _playerName;
		private int _votes;
		public string PlayerName
		{
			get { return _playerName; }
			set
			{
				_playerName = value;
				RaisePropertyChangedEvent("PlayerName");
			}
		}
		public int Votes
		{
			get { return _votes; }
			set
			{
				_votes = value;
				RaisePropertyChangedEvent("Votes");
			}
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
