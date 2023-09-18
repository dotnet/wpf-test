// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides coverage to verify 2 way binding in single bound run 

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Data;
using System.Resources;
using System.Windows;
using Microsoft.Test.Discovery;
using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;


namespace Microsoft.Test.DataBoundRun
{
    /// <summary>
    /// Test verifies 2 way binding in single bound run
    /// TextBlock structure <Run>sometext</Run>
    /// Source bound to single run
    /// </summary>
    [Test(2, "DataBoundRun", "BindingToStaticResource", MethodParameters = "/TestCaseType:BindingToStaticResource")]
    public class BindingToStaticResource : CustomTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        public override void RunTestCase()
        {
            _panel = new StackPanel();

            _myData = new Person();
            _myData.Name = "Editing";
            
            _rd = new ResourceDictionary();
            _rd.Add("myObject",_myData);

            _textblock = new TextBlock();

            _panel.Resources = _rd;
            _panel.Children.Add(_textblock);
            MainWindow.Content = _panel;
            QueueDelegate(TestVariations);
        }

        private void TestVariations()
        {
            // Set initial content 
            SetInitialContent();
            VerifyBinding("Editing", true);

            // Modify Text through property system
            _r.Text = "Hello";
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(delayTimeToBind);

            VerifyBinding("Hello", false);
            
            Logger.Current.ReportSuccess();
        }

        #endregion

        #region Helpers

        private void VerifyBinding(string expectedText, bool verifyTextBlockText)
        {
            // Verify Binding in Textblock
            if (verifyTextBlockText)
            {
                Verifier.Verify(expectedText.Equals(_textblock.Text), "Verifying that contents of TextBlock [" + _textblock.Text + "] are same as [" + expectedText + "]", true);
            }
            else
            {
                object temp = _panel.Resources["myObject"];
                Person person = (Person)temp;
                string name = person.Name;

                Verifier.Verify(expectedText.Equals(name), "Verifying that contents of object.name [" + name + "] are same as [" + expectedText + "]", true);
            }
        }

        private void SetInitialContent()
        {
            _r = new Run();
            _textblock.Inlines.Add(_r);            

            Binding binding1 = new Binding("Text");
            object personObject = _panel.Resources["myObject"];
            binding1.Source = (Person)personObject;
            binding1.Path = new PropertyPath("Name");
            binding1.Mode = BindingMode.TwoWay;
            _r.SetBinding(Run.TextProperty, binding1);
        }

        #endregion Helpers

        #region Private fields

        private TextBlock _textblock;
        private Person _myData;
        private ResourceDictionary _rd;
        private StackPanel _panel;
        private Run _r;
        private const int delayTimeToBind = 2000;

        #endregion
    }

    // Binding Source
    public class Person
    {
        string _name;
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                _name = value;
            }
        } 
    }
}