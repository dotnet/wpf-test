// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression Test- TemplateSelector: does not reuse template instance when selector returns the same template
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>            
    [Test(2, "Styles", "RegressionContentTemplateSelectorReuse")]
    public class RegressionContentTemplateSelectorReuse : WindowTest
    {
        #region Constructors

        public RegressionContentTemplateSelectorReuse()
        {
            RunSteps += new TestStep(Verify);
        }

        #endregion

        #region Private Members

        private TestResult Verify()
        {
            // Setup content control with a DataTemplateSelector and grab a handle to the
            // generated visual. The DataTemplateSelector is rigged to return the same
            // DataTemplate each time, so when I change the content to another item of the
            // same type we should reuse the existing template, and therefore the
            // generated visual instance shouldn't change.

            StackPanel sp = new StackPanel(); ;
            Window.Content = sp;
            ContentControl cc = new ContentControl();
            cc.Content = new Place("Brea", "CA");
            cc.ContentTemplateSelector = new FixedDataTemplateSelector();
            sp.Children.Add(cc);

            FrameworkElement fe = Util.FindDataVisual(cc, cc.Content);

            cc.Content = new Place("Seattle", "WA");

            FrameworkElement fe2 = Util.FindDataVisual(cc, cc.Content);

            if (fe == null || !Object.Equals(fe, fe2)) return TestResult.Fail;

            return TestResult.Pass;
        }

        private class FixedDataTemplateSelector : DataTemplateSelector
        {
            private DataTemplate _dt = null;

            public DataTemplate DT
            {
                get
                {
                    if (_dt == null)
                    {
                        StringReader stringReader = new StringReader("<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:src=\"clr-namespace:Microsoft.Test.DataServices;assembly=DataServicesTestCommon\" DataType=\"{x:Type src:Place}\"><TextBlock Text=\"{Binding Name}\" /></DataTemplate>");
                        XmlReader xmlReader = XmlReader.Create(stringReader);
                        _dt = (DataTemplate)XamlReader.Load(xmlReader);
                    }

                    return _dt;
                }
                set { _dt = value; }
            }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                return DT;
            }
        }

        #endregion
    }

}
