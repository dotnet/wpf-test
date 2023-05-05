// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.DataServices
{
    using System.Windows.Controls;
    using System.Windows.Threading;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Logging;
    using Microsoft.Test.Threading;

    // [DISABLED_WHILE_PORTING]
    // [Test(1, "INotifyDataErrorInfo", "CommitChangesTest", SupportFiles = @"featuretests\dataservices\content\indei.commit.xaml")]
    public class CommitChangesTest : INotifyDataErrorInfoTest
    {
        public CommitChangesTest()
            : base("indei.commit.xaml", false, false)
        {
            this.RunSteps += CommitMultiple;
            this.RunSteps += QueueErrorsAsync;
        }

        public override Microsoft.Test.Logging.TestResult Initialize()
        {
            base.Initialize();

            _root = FindDependecyObject<Grid>(Window, "root");
            Assert.IsNotNull(_root, "Could not find root grid.");

            _textBox1A = FindDependecyObject<TextBox>(Window, "textbox1a");
            _textBox1B = FindDependecyObject<TextBox>(Window, "textbox1b");
            _textBox2A = FindDependecyObject<TextBox>(Window, "textbox2a");
            _textBox2B = FindDependecyObject<TextBox>(Window, "textbox2b");

            Assert.IsNotNull(_textBox1A, "Could not find textbox (1A) target.");
            Assert.IsNotNull(_textBox1B, "Could not find textbox (1B) target.");
            Assert.IsNotNull(_textBox2A, "Could not find textbox (2A) target.");
            Assert.IsNotNull(_textBox2B, "Could not find textbox (2B) target.");

            return TestResult.Pass;
        }

        /// <summary>
        /// Commit multiple errors at once
        /// </summary>
        private TestResult CommitMultiple()
        {
            object data1 = CreateDataObject("enemies", 10);
            object data2 = CreateDataObject("friends", 4);

            Grid root = FindDependecyObject<Grid>(Window, "root");
            Assert.IsNotNull(root, "Could not find root grid.");

            TextBox textBox1a = FindDependecyObject<TextBox>(Window, "textbox1a");
            TextBox textBox1b = FindDependecyObject<TextBox>(Window, "textbox1b");
            TextBox textBox2a = FindDependecyObject<TextBox>(Window, "textbox2a");
            TextBox textBox2b = FindDependecyObject<TextBox>(Window, "textbox2b");

            Assert.IsNotNull(textBox1a, "Could not find textbox target (1a).");
            Assert.IsNotNull(textBox1b, "Could not find textbox target (1b).");
            Assert.IsNotNull(textBox2a, "Could not find textbox target (2a).");
            Assert.IsNotNull(textBox2b, "Could not find textbox target (2b).");

            textBox1a.DataContext = data1;
            textBox1b.DataContext = data1;
            textBox2a.DataContext = data2;
            textBox2b.DataContext = data2;

            root.BindingGroup.BeginEdit();
            
            Wait();

            Assert.IsTrue(ValidateErrors(root, 0), "Initial error count wrong.");

            textBox1a.Text = "joker#";
            textBox1b.Text = "9990";
            textBox2a.Text = "batman#";
            textBox2b.Text = "1000";

            Wait();

            Assert.IsTrue(ValidateErrors(root, 0), "Error count wrong before commit.");

            root.BindingGroup.CommitEdit();

            Wait();

            Assert.IsTrue(ValidateErrors(root, 4), "Error count wrong after commit.");

            Wait();

            textBox1a.Text += "#batman";
            textBox1b.Text += "99";
            textBox2a.Text += "#robin";
            textBox2b.Text += "101";

            root.BindingGroup.CommitEdit();

            Wait();

            Assert.IsTrue(ValidateErrors(root, 8), "Error count wrong after commit.");

            // values to clear errors
            textBox1a.Text = "enemies";
            textBox1b.Text = "54";
            textBox2a.Text = "friends";
            textBox2b.Text = "360";

            root.BindingGroup.CommitEdit();

            Wait();

            Assert.IsTrue(ValidateErrors(root, 0), "Error count wrong after commit.");

            return TestResult.Pass;
        }

        /// <summary>
        /// Commit multiple errors at once using background worker
        /// </summary>
        /// <returns></returns>
        private TestResult QueueErrorsAsync()
        {
            DataStringErrors data1 = new DataStringErrors(true) { StringProperty = "enemies", IntProperty = 10 };
            data1.ThreadWait = 2;

            DataStringErrors data2 = new DataStringErrors(true) { StringProperty = "friends", IntProperty = 4 };
            data2.ThreadWait = 1;

            _textBox1A.DataContext = data1;
            _textBox1B.DataContext = data1;
            _textBox2A.DataContext = data2;
            _textBox2B.DataContext = data2;

            _root.BindingGroup.BeginEdit();

            Wait();

            Assert.IsTrue(ValidateErrors(_root, 0), "Fail: Initial error.");

            _textBox1A.Text = "joker#batman";
            _textBox1B.Text = "9999";
            _textBox2A.Text = "batman#robin";
            _textBox2B.Text = "1001";

            Assert.IsTrue(ValidateErrors(_root, 0), "Fail: Error count before commit.");

            _root.BindingGroup.CommitEdit();

            DispatcherHelper.DoEvents(13000, DispatcherPriority.SystemIdle);

            Assert.IsTrue(ValidateErrors(_root, 8), "Fail: Error count after commit.");

            _textBox1A.Text = "enemies";
            _textBox1B.Text = "10";
            _textBox2A.Text = "friends";
            _textBox2B.Text = "100";

            Assert.IsTrue(ValidateErrors(_root, 8), "Fail: Error count before commit.");

            _root.BindingGroup.CommitEdit();

            DispatcherHelper.DoEvents(13000, DispatcherPriority.SystemIdle);

            Assert.IsTrue(ValidateErrors(_root, 0), "Fail: Error count after commit.");

            return TestResult.Pass;
        }

        private Grid _root = null;
        private TextBox _textBox1A = null;
        private TextBox _textBox1B = null;
        private TextBox _textBox2A = null;
        private TextBox _textBox2B = null;
    }
}
