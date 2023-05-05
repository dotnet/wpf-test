// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.DataServices
{
    using System.Windows.Controls;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Logging;

    [Test(0, "INotifyDataErrorInfo", "MultiBinding", SupportFiles = @"featuretests\dataservices\content\indei.multibinding.xaml")]
    public class MultiBindingTest : INotifyDataErrorInfoTest
    {
        [Variation(false, false)]
        [Variation(false, true)]
        [Variation(true, false)]
        [Variation(true, true)]
        public MultiBindingTest(bool async, bool complex)
            : base("indei.multibinding.xaml", async, complex)
        {
            this.RunSteps += BindingErrors;
        }

        public override TestResult Initialize()
        {
            base.Initialize();

            _textboxA = FindDependecyObject<TextBox>(Window, "textboxA");
            _textboxB = FindDependecyObject<TextBox>(Window, "textboxB");

            Assert.IsNotNull(_textboxA, "Could not find textbox (A) target.");
            Assert.IsNotNull(_textboxB, "Could not find textbox (B) target.");

            return TestResult.Pass;
        }

        /// <summary>
        /// Add error to a property and verify Validation.Errors returns
        /// correct errors.
        /// </summary>
        private TestResult BindingErrors()
        {
            object data1 = CreateDataObject("superman#", 0);
            object data2 = CreateDataObject("batman#", 0);

            _textboxA.DataContext = data1;
            _textboxB.DataContext = data2;

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxA, 1), "Error count is wrong for MultiBinding (ValidatesOnNotifyDataErrors == true)");
            Assert.IsTrue(ValidateErrors(_textboxB, 0), "Error count is wrong for MultiBinding (ValidatesOnNotifyDataErrors == false)");

            return TestResult.Pass;
        }

        private TextBox _textboxA = null;
        private TextBox _textboxB = null;
    }
}
