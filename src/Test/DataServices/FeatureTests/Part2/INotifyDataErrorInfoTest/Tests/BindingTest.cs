// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.DataServices
{
    using System.Windows.Controls;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Logging;

    [Test(0, "INotifyDataErrorInfo", "BindingTest", SupportFiles = @"featuretests\dataservices\content\indei.binding.xaml")]
    public partial class BindingTest : INotifyDataErrorInfoTest
    {
        [Variation(false, false)]
        [Variation(false, true)]
        [Variation(true, false)]
        [Variation(true, true)]
        public BindingTest(bool async, bool complex) 
            : base("indei.binding.xaml", async, complex)
        {
            this.RunSteps += AddError;
            this.RunSteps += RemoveError;
            this.RunSteps += AddMultipleErrors;
            this.RunSteps += RemoveMultipleErrors;
            this.RunSteps += CrossFieldNotify;
        }

        public override TestResult Initialize()
        {
            base.Initialize();

            _textboxA = FindDependecyObject<TextBox>(Window, "textboxA");
            _textboxB = FindDependecyObject<TextBox>(Window, "textboxB");
            _listboxA = FindDependecyObject<ListBox>(Window, "listboxA");
            _listboxB = FindDependecyObject<ListBox>(Window, "listboxB");

            Assert.IsNotNull(_textboxA, "Could not find textbox (A) target.");
            Assert.IsNotNull(_textboxB, "Could not find textbox (B) target.");
            Assert.IsNotNull(_listboxA, "Could not find listbox (A) target.");
            Assert.IsNotNull(_listboxB, "Could not find textbox (B) target.");

            return TestResult.Pass;
        }
        /// <summary>
        /// Add error to a property and verify Validation.Errors returns
        /// correct errors.
        /// </summary>
        private TestResult AddError()
        {
            object data = CreateDataObject("superman?", 0);
            SetBinding(_textboxA, TextBox.TextProperty, CreateBinding(data, "StringProperty", true));

            Assert.IsTrue(ValidateErrors(_textboxA, 0, _listboxA), "Fail: Initial error count.");

            _textboxA.Text = "up, up and away!!!";

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxA, 1, _listboxA), "Fail: Adding error.");

            ResetBindings(_textboxA);

            return TestResult.Pass;
        }
        /// <summary>
        /// Bind to an object that contains an error then update the property 
        /// and verify error is removed.  Verify Validation.Errors returns
        /// correct errors.
        /// </summary>
        private TestResult RemoveError()
        {
            object data = CreateDataObject("batman#", 0);
            SetBinding(_textboxA, TextBox.TextProperty, CreateBinding(data, "StringProperty", true));

            Assert.IsTrue(ValidateErrors(_textboxA, 1, _listboxA), "Fail: Initial error count.");

            _textboxA.Text = "batman!";

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxA, 0, _listboxA), "Fail: Removing error.");

            ResetBindings(_textboxA);

            return TestResult.Pass;
        }
        /// <summary>
        /// Add multiple errors to a single property and verify Validation.Errors returns
        /// correct errors.
        /// </summary>
        private TestResult AddMultipleErrors()
        {
            object data = CreateDataObject(string.Empty, 500);
            SetBinding(_textboxB, TextBox.TextProperty, CreateBinding(data, "IntProperty", true));

            Assert.IsTrue(ValidateErrors(_textboxB, 0, _listboxB), "Fail: Initial error count.");

            _textboxB.Text = "2333";

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxB, 2, _listboxB), "Fail: Adding multiple errors.");

            ResetBindings(_textboxB);

            return TestResult.Pass;
        }
        /// <summary>
        /// Bind to an object that contains an error then update the property 
        /// and verify error is removed.  Verify Validation.Errors returns
        /// correct errors.
        /// </summary>
        private TestResult RemoveMultipleErrors()
        {
            object data = CreateDataObject(string.Empty, 1331);
            SetBinding(_textboxB, TextBox.TextProperty, CreateBinding(data, "IntProperty", true));

            Assert.IsTrue(ValidateErrors(_textboxB, 2, _listboxB), "Fail: Adding multiple errors.");

            _textboxB.Text = "154";

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxB, 0, _listboxB), "Fail: Removing error.");

            ResetBindings(_textboxB);

            return TestResult.Pass;
        }
        /// <summary>
        /// Verify erros for prop A can notify error for prop B
        /// </summary>
        private TestResult CrossFieldNotify()
        {
            object data = CreateDataObject("the flash!", 800);

            SetBinding(_textboxA, TextBox.TextProperty, CreateBinding(data, "StringProperty", true));
            SetBinding(_textboxB, TextBox.TextProperty, CreateBinding(data, "IntProperty", true));

            Assert.IsTrue(ValidateErrors(_textboxA, 0, _listboxA), "Fail: Initial error count (A).");
            Assert.IsTrue(ValidateErrors(_textboxB, 0, _listboxB), "Fail: Initial error count (B).");

            _textboxB.Text = "-90";

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxA, 1, _listboxA), "Fail: Error count (A) is wrong after update.");
            Assert.IsTrue(ValidateErrors(_textboxB, 0, _listboxB), "Fail: Error count (B) is wrong after update");

            ResetBindings(_textboxA);
            ResetBindings(_textboxB);

            return TestResult.Pass;
        }

        private TextBox _textboxA = null;
        private TextBox _textboxB = null;
        private ListBox _listboxA = null;
        private ListBox _listboxB = null;
    }
}
