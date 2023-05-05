// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.DataServices
{
    using System.Windows.Controls;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Logging;

    [Test(0, "INotifyDataErrorInfo", "BindingGroupTest", SupportFiles = @"featuretests\dataservices\content\indei.bindinggroup.xaml", Keywords = "MicroSuite")]
    public class BindingGroupTest : INotifyDataErrorInfoTest 
    {
        [Variation(false, false)]
        [Variation(true, false)]
        public BindingGroupTest(bool async, bool complex)
            : base("indei.bindinggroup.xaml", async, complex)
        {
            this.RunSteps += AddRemoveError;
        }

        public override TestResult Initialize()
        {
            base.Initialize();

            _root = FindDependecyObject<DockPanel>(Window, "root");
            _textbox = FindDependecyObject<TextBox>(Window, "textbox");
            _listbox = FindDependecyObject<ListBox>(Window, "listbox");

            Assert.IsNotNull(_root, "Could not find root.");
            Assert.IsNotNull(_textbox, "Could not find textbox.");
            Assert.IsNotNull(_listbox, "Could not find listbox.");

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify errors are correctly updated in binding group
        /// </summary>
        private TestResult AddRemoveError()
        {
            object data = CreateDataObject(string.Empty, 0);

            _root.DataContext = data;
            _root.BindingGroup.BeginEdit();
            
            Wait();

            Assert.IsTrue(ValidateErrors(_root, 0), "Fail: Root initalial error count is wrong.");
            Assert.IsTrue(ValidateErrors(_textbox, 0, _listbox), "Fail: Target initial error count is wrong.");

            _textbox.Text = "##gotham##";
            _root.BindingGroup.CommitEdit(); 

            Wait();

            Assert.IsTrue(ValidateErrors(_root, 1), "Fail: Root error count is wrong after (1) update");
            Assert.IsTrue(ValidateErrors(_textbox, 1, _listbox), "Fail: Target error count is wrong after (1) update");

            _textbox.Text = "gotham";
            _root.BindingGroup.CommitEdit(); 

            Wait();

            Assert.IsTrue(ValidateErrors(_root, 1), "Fail: Root error count is wrong after (2) update");
            Assert.IsTrue(ValidateErrors(_textbox, 0, _listbox), "Fail: Target error count is wrong after (2) update");

            _textbox.Text = "superman";
            _root.BindingGroup.CommitEdit(); 

            Wait();

            Assert.IsTrue(ValidateErrors(_root, 0), "Fail: Root error count is wrong after (3) update");
            Assert.IsTrue(ValidateErrors(_textbox, 0, _listbox), "Fail: Target error count is wrong after (3) update");

            ResetBindings(_textbox);

            return TestResult.Pass;
        }

        private DockPanel _root = null;
        private TextBox _textbox = null;
        private ListBox _listbox = null;
    }
}
