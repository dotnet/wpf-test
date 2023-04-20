// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where No ErrorTemplate displayed on initial validation in ListView item
    /// </description>
    /// <relatedBugs>
   

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "TargetValidates")]
    public class TargetValidates : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;       
        
        #endregion

        #region Constructors

        public TargetValidates()
            : base(@"TargetValidates.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {         
            _myStackPanel = (StackPanel)RootElement.FindName("myStackPanel");

            if (_myStackPanel == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }            
            return TestResult.Pass;
        }
                        
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Background);

            // Grab the Adorner Layer
            UniformGrid grid = (UniformGrid)VisualTreeHelper.GetParent(_myStackPanel);
            ContentPresenter contentPresenter = (ContentPresenter)VisualTreeHelper.GetParent(grid);
            AdornerDecorator adornerDecorator = (AdornerDecorator)VisualTreeHelper.GetParent(contentPresenter);
            AdornerLayer adornerLayer = (AdornerLayer)VisualTreeHelper.GetChild(adornerDecorator, 1);
            
            // Verify if it has children (this will be the error template)
            if (VisualTreeHelper.GetChildrenCount(adornerLayer) != 1)
            {
                LogComment("Did not display Error Template on initialize." + VisualTreeHelper.GetChildrenCount(adornerLayer));
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes

    public class Type2
    {
        public string PropertyB { get; set; }

        public Type2()
        {
        }
    }

    public class ValueIsNotNull : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value as string;

            if (!string.IsNullOrEmpty(str))
            {
                return ValidationResult.ValidResult;
            }
            else
            {
                return new ValidationResult(false, "Value must not be null");
            }
        }
    }

    #endregion
}
