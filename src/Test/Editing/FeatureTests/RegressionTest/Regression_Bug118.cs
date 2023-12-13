// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Regression test Regression_Bug118

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    #endregion Namespaces.

    [TestOwner("Microsoft"), TestTitle("Regression_Bug118 Regression Test")]
    public class Regression_Bug118 : CustomTestCase
    {
        public override void RunTestCase()
        {
            string stringInTextBox = ConfigurationSettings.Current.GetArgument("StringInTextBox");
            
            this._stringToFind = ConfigurationSettings.Current.GetArgument("StringToFind");

            Window window = base.MainWindow;
            this._textbox1 = new TextBox();

            FlowPanel panel = new FlowPanel();

            this._textbox1.Height = 200;
            this._textbox1.Width = 200;
            
            this._textbox1.Text = stringInTextBox;
            
            panel .Children.Add(this._textbox1);

            window.Content = panel;
            window.Visibility = Visibility.Visible;

            QueueHelper.Current.QueueDelegate (new SimpleHandler (OnUICreated));
        }

        private void OnUICreated()
        {
            for (int i = 0; i < 2; i++)
            {
                this._textbox1.Find(this._stringToFind , FindOptions.MatchWholeWords);
                new Bold(this._textbox1.Selection.Start, this._textbox1.Selection.End);
            }
                                
            QueueHelper.Current.QueueDelegate (new SimpleHandler (OnTestComplete));
        }
        
        private void OnTestComplete()
        {            
            Logger.Current.ReportSuccess();        
        }

        private TextBox _textbox1;
        private string  _stringToFind;               
    }   
}