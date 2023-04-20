// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;

namespace Com.ControlStore
{
    public class TransButton : Button
    {
        public TransButton()
            : base()
        {
            Opacity = 0.5;
        }
    }

    public class TransListBox : ListBox
    {
        public TransListBox()
            : base()
        {
            Opacity = 0.5;
        }
    }

    public class TransListBoxItem : ListBoxItem
    {
        public TransListBoxItem()
            : base()
        {
            Opacity = 0.5;
        }
    }    
}
