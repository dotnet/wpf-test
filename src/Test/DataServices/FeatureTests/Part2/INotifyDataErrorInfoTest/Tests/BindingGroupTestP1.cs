// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.DataServices
{
    using System.Windows.Controls;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Logging;
    using Microsoft.Test.Threading;

    [Test(0, "INotifyDataErrorInfo", "BindingGroupTestP1", SupportFiles = @"featuretests\dataservices\content\indei.bindinggroup.xaml")]
    public class BindingGroupTestP1 : INotifyDataErrorInfoTest
    {
        [Variation(false)]
        [Variation(true)]
        public BindingGroupTestP1(bool throwCritical)
            : base("indei.bindinggroup.xaml", false, false)
        {
            this._throwCritical = throwCritical;
            this.RunSteps += GetErrorsException;
        }

        public override TestResult Initialize()
        {
            base.Initialize();

            _root = FindDependecyObject<DockPanel>(Window, "root");
            _textbox = FindDependecyObject<TextBox>(Window, "textbox");

            Assert.IsNotNull(_root, "Could not find root.");
            Assert.IsNotNull(_textbox, "Could not find textbox.");

            return TestResult.Pass;
        }

        /// <summary>
        /// Throw exception in Validation.GetErrors.  Verify error notification
        /// occurs, and exception is handled by data system.
        /// </summary>
        private TestResult GetErrorsException()
        {
            DataThrowGetErrors data = new DataThrowGetErrors(false, _throwCritical);

            _root.DataContext = data;
            _root.BindingGroup.BeginEdit();

            Wait();

            Assert.IsTrue(ValidateErrors(_root, 0), "Fail: Root initial error count is wrong.");
            Assert.IsTrue(ValidateErrors(_textbox, 0), "Fail: Target error count is wrong.");

            string teststring = "how fast is the flash?";

            _textbox.Text = teststring;

            if (_throwCritical)
            {
                try
                {
                    _root.BindingGroup.CommitEdit();
                }
                catch (System.Security.SecurityException)
                {
                    ResetBindings(_textbox);
                    ResetBindings(_root);

                    return TestResult.Pass;
                }

                Log.LogStatus("Fail: Critical SecurityException should have been thrown.");
                return TestResult.Fail;
            }
            
            _root.BindingGroup.CommitEdit();

            Wait();

            Assert.IsTrue(ValidateErrors(_textbox, 0), "Fail: Exceptions thrown in GetErrors, count should still be 0.");
            Assert.IsTrue(data.StringProperty.Equals(teststring), "Fail: Data object's string property is not correct value");

            ResetBindings(_textbox);
            ResetBindings(_root);

            return TestResult.Pass;
        }

        private DockPanel _root = null;
        private TextBox _textbox = null;
        private bool _throwCritical = false;
    }
}
