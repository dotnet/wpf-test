// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;

namespace Microsoft.Test.Graphics.CachedComposition
{
    /// <summary>
    /// Change content through a databinding.
    /// </summary>
    class DataBindingChanger : Changer, INotifyPropertyChanged
    {
        public override TestResult Change()
        {
            Binding bind = new Binding("MyColor");
            bind.Source = this;
            content.BindColor(bind); // bind to -same- color as initial. re-rendering on binding is uninteresting.

            DispatcherHelper.DoEvents(DispatcherPriority.ApplicationIdle); // wait for databinding to complete
            //now that it's bound, change it.
            MyColor = Brushes.Green;
            DispatcherHelper.DoEvents(DispatcherPriority.ApplicationIdle); // wait for it to render so that we return in a testable state
            return TestResult.Pass;
        }
        SolidColorBrush _b = Brushes.Red; // inital color is the same as the content initial color
        public SolidColorBrush MyColor
        {
            set
            {
                _b = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("MyColor"));
                }
            }
            get { return _b; }
        }

        //INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
