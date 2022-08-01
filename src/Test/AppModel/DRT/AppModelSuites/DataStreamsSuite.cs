// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.Threading;

namespace DRT
{
    /// <summary>
    /// This test is designed to test the ability of the DataStreams object to save and restore
    /// the state of tagged DepenedencyProperties of tagged UIElements.
    ///
    /// To "tag" a DependencyProperty, set the metadata flag "Journal".
    /// To "tag" a UIElement, give it a PersistId that is not zer0.
    ///
    /// We test:
    ///
    /// 0. That the correct number of elements have been saved.
    /// 1. That a tagged property of a tagged element is saved and restored.
    /// 2. That a non-tagged property of a tagged element is NOT saved or restored.
    /// 3. That an element that is not tagged is NOT saved or restored.
    /// 4. That a tagged property that is data-bound or an expression is NOT saved or restored.
    ///
    /// Test team/owner: AppModel/Microsoft
    /// </summary>
    class DataStreamsSuite : DrtTestSuite
    {
		public DataStreamsSuite() : base("DataStreams")
		{
            TeamContact = "WPF";
            Contact     = "Microsoft";         // if different from DRT
        }

		public override DrtTest[] PrepareTests()
		{
			return new DrtTest[]{
                        new DrtTest( Run ),
			};
		}

        #region Test helpers (reflection)

        public void Save(object dataStreams, UIElement element)
        {
            MethodInfo method = dataStreams.GetType().GetMethod("Save", BindingFlags.Instance | BindingFlags.NonPublic);

            method.Invoke(dataStreams, new object[] { element });
        }

        public void Load(object dataStreams, UIElement element)
        {
            MethodInfo method = dataStreams.GetType().GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic);

            method.Invoke(dataStreams, new object[] { element });
        }

        public object CreateDataStreams()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(NavigationWindow)); // to get PresentationFramework.dll
            Type type = assembly.GetType("MS.Internal.AppModel.DataStreams");

            return Activator.CreateInstance(type, true);
        }

        public int DataStreamsCount(object dataStreams)
        {
            // Getting dataStreams._subStreams.Count via reflection
            FieldInfo field = dataStreams.GetType().GetField("_subStreams", BindingFlags.Instance | BindingFlags.NonPublic);
            object subStreams = field.GetValue(dataStreams);
            if (subStreams == null)
                return 0;
            MethodInfo method = subStreams.GetType().GetMethod("get_Count");
            return (int)method.Invoke(subStreams, null);
        }
        #endregion

		private void Run()
		{
			const string JournaledText = "JournalMe";

			// Create a tree with the specified text values
			TestTree tree = new TestTree(JournaledText, "CustomText", "NoPersistId");

			// Just check the text of the data-bound TextBox
			DRT.AssertEqual(tree.GetDataBoundText(), "1",
				"Test setup: did not get expected initial value of data-bound text");

			// Create a DataStreams object
			object dataStreams = this.CreateDataStreams();

			// Save the state of the tree in the DataStreams object
			Save(dataStreams, tree);

			// 0. Check to be sure that we have saved the correct number of SubStreams.
			// DataStreams.Count is the number of elements that have at least one DP
			// saved.
			DRT.AssertEqual(this.DataStreamsCount(dataStreams), 1,
				"Should only have saved values for one element");

			// Make a brand new tree with empty TextBoxen
			tree = new TestTree();

			// Restore the state of the tree
			Load(dataStreams, tree);

			// 1. Check the state of a regular TextBox that should have been journaled
			DRT.AssertEqual(tree.GetJournaledText(), JournaledText, "State of TextBox was not restored");

			// 2. Check the state the CustomTextBox that has turned journaling off
			DRT.AssertEqual(tree.GetCustomText(), string.Empty, "Overriding the metadata flag should make this NOT be journaled");

			// 3. Check the state of a TextBox that didn't have a PersistId
			DRT.AssertEqual(tree.GetNoPersistIdText(), string.Empty, "The PersistId must be != 0 to be journaled");

			// 4. Check the state of the data-bound TextBox
			DRT.AssertEqual(tree.GetDataBoundText(), "2", "Did not have correct value for data-bound TextBox");
		}
    }

    public class TestTree : StackPanel
    {
        static int s_instanceCount = 0; // for data binding

        TextBox _journaledTextBox;
        CustomTextBox _customTextBox;
        TextBox _noPersistIdTextBox;
        TextBox _dataBoundTextBox;

        public TestTree()
            : this(string.Empty, string.Empty, string.Empty)
        {
        }

        public TestTree(string journaledText, string customText, string noPersistIdText)
        {
            ++s_instanceCount;
            int persistId = 0;

            // This is a "normal" TextBox, with its PersistId set. Setting the PersistID
            // would normally be done by the parser.
            _journaledTextBox = new TextBox();
            SetPersistId(_journaledTextBox, ++persistId);
            _journaledTextBox.Text = journaledText;
            this.Children.Add(_journaledTextBox);

            // This is a "custom" TextBox, with the metadata of the Text property overridden
            // to turn journaling off.
            _customTextBox = new CustomTextBox();
            SetPersistId(_customTextBox, ++persistId);
            _customTextBox.Text = customText;
            this.Children.Add(_customTextBox);

            // This is a "normal" TextBox that has not been assigned a PersistId.
            _noPersistIdTextBox = new TextBox();
            _noPersistIdTextBox.Text = noPersistIdText;
            this.Children.Add(_noPersistIdTextBox);

            // This is a data-bound "normal" TextBox.
            _dataBoundTextBox = new TextBox();
            SetPersistId(_customTextBox, ++persistId);
            _dataBoundTextBox.DataContext = this;
            Binding binding = new Binding("InstanceCount");
            binding.Mode = BindingMode.OneWay;
            _dataBoundTextBox.SetBinding(TextBox.TextProperty, binding);
        }

        /// <summary>
        /// The property that the data-bound TextBox will bind to.
        /// </summary>
        public int InstanceCount
        {
            get { return s_instanceCount; }
        }

        public string GetJournaledText()
        {
            return _journaledTextBox.Text;
        }

        public string GetCustomText()
        {
            return _customTextBox.Text;
        }

        public string GetNoPersistIdText()
        {
            return _noPersistIdTextBox.Text;
        }

        public string GetDataBoundText()
        {
            return _dataBoundTextBox.Text;
        }

        private static void SetPersistId(UIElement target, int id)
        {
            if (s_setPersistId == null)
            {
                s_setPersistId = typeof(UIElement).GetMethod(
                    "SetPersistId",
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null, new Type[] {typeof(int)}, null);
            }

            s_setPersistId.Invoke(target, new object[] {id});
        }

        private static MethodInfo s_setPersistId;
    }

    public class CustomTextBox : TextBox
    {
        static CustomTextBox()
        {
            // Turn TextProperty's journaling off
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata();
            metadata.Journal = false;
            TextProperty.OverrideMetadata(typeof(CustomTextBox), metadata);
        }
    }
}
