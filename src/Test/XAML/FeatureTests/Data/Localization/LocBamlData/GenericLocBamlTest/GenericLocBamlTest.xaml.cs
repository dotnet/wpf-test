// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Localization
{
    public partial class GenericLocBamlTest : Window
    {
        public GenericLocBamlTest()
        {
            this.Loaded  += new RoutedEventHandler(OnLoaded);
        }

        public void OnLoaded(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }

    // These classes are used by specific .xaml files in the test suite.

    /// <summary>
    /// MyObject class
    /// </summary>
    [Localizability(LocalizationCategory.Text)]
    [UidProperty("Uid")]
    [ContentProperty("Text")]
    public class MyObject
    {
        /// <summary>
        /// string property
        /// </summary>
        private string _myText;

        /// <summary>
        /// Uid property is used for localization.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text value.</value>
        [Localizability(LocalizationCategory.Text)]
        public string MyText
        {
            set
            {
                _myText = value;
            }

            get
            {
                return _myText;
            }
        }
    }
}
