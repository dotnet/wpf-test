// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CustomControl // Namespace must be the same as what you set in project file
{

  using System;
  using System.Windows;
  using System.Windows.Controls;

  public class LibraryCSButton : Button
  {
        private static DependencyProperty s_integerProperty 
            = DependencyProperty.Register("Integer", typeof(int), typeof(LibraryCSButton));

        public int Integer
        {
            get
            {                  
                return (int)GetValue(s_integerProperty);
            }
	    set
	    {
		SetValue(s_integerProperty, value);
	    }

        }

        //protected virtual void OnDraggingChanged(DependencyPropertyChangedEventArgs e)
        private void OnIntegerChanged(DependencyPropertyChangedEventArgs e)
        {
        }

  }

}
