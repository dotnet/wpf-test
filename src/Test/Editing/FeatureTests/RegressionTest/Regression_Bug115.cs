// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Regression test Regression_Bug115

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Xml;
    using System.Collections;
    using System.Threading; using System.Windows.Threading;
    
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;

    using System.Windows.Documents;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    [TestOwner("Microsoft"), TestTitle("Regression_Bug116 Regression Test")]
    public class Regression_Bug116 : CustomTestCase
    {
        #region Main flow.

        public override void RunTestCase()
        {
            this._elementWrapper = new UIElementWrapper(new TextBox());
            MainWindow.Content = (FrameworkElement)this._elementWrapper.Element;
            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnAdded));
        }

        private void OnAdded()
        {
            MouseInput.MouseClick(this._elementWrapper.Element);
            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnElementClicked));
        }

        private void OnElementClicked()
        {
            TextRangeMovable trm = this._elementWrapper.SelectionInstance.MovableCopy();

            trm.SetStart(this._elementWrapper.TextPointerStart);
            trm.SetEnd(this._elementWrapper.TextPointerEnd);

            Logger.Current.Log("Ready to call TextRangeMovable.GetContextEnumerator");

            IEnumerator e = trm.GetContentEnumerator();

            Logger.Current.Log("Ready to call IEnumerator.MoveNext");

            while (e.MoveNext())
            {
                if (e.Current is Control)
                {
                    Control c = (Control)e.Current;
                }
            }

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Private fields.

        private UIElementWrapper _elementWrapper = null;

        #endregion Private fields.
    }
}