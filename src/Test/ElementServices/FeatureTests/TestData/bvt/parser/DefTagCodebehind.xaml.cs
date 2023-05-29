// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Avalon.Test.CoreUI.Parser.MyName
{
using System;
using System.Windows;
using System.Windows.Controls;
    /// <summary>
    /// CodeBehind
    /// <para />
    /// </summary>
    public partial class _Page1
	{
        #region public functions    
		/// <summary>
		/// ButtonClick
		/// </summary>
		/// <param name="el"></param>
		/// <param name="cea"></param>
		void ButtonClick(object el, RoutedEventArgs cea)
		{
                   MessageBox.Show("The button has been clicked.");
		}
        #endregion public functions
    }
}
