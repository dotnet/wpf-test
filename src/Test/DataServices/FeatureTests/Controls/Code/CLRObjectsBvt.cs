// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading; using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Test adding/removing clr objects from list box.
	/// </description>
	/// </summary>
    [Test(0, "Controls", "CLRObjectsBvt")]
	public class CLRObjectsBvt : WindowTest
	{
		ListBox _lb1;
		FrameworkElement[] _visualelements;

		public CLRObjectsBvt()
		{
			StepPriority = DispatcherPriority.Background;
			InitializeSteps += new TestStep(CreateTree);
			RunSteps += new TestStep(AddCLRObject);
			RunSteps += new TestStep(VerifyObjectsOnTree);
			RunSteps += new TestStep(ChangeItemsCollection);
		}

		#region CreateTree
		// Prepares the test.
        private TestResult CreateTree()
		{
			Status("CreateTree");

			_lb1 = new ListBox();

			Status("Add ListBox to Window.");
			Window.Content = _lb1;

			DataTemplate dwarfTemplate = new DataTemplate();
			FrameworkElementFactory template = new FrameworkElementFactory(typeof(TextBlock));
            template.SetValue(TextBlock.NameProperty, "dwarfTemplate");
            /* Setters are not supported in DataTemplate.  Review with NamitaG.
			dwarfTemplate.Setters.Add (new Setter());
            */

            template.SetBinding(TextBlock.TextProperty, new Binding("EyeColor"));
            dwarfTemplate.VisualTree = template;

			_lb1.ItemTemplate = dwarfTemplate;

			LogComment("CreateTree was successful");
			return TestResult.Pass;
		}
		#endregion

		#region AddCLRObject
		// Add CLRObjects to the ItemsCollection.
        private TestResult AddCLRObject()
		{
			_lb1.Items.Add(new Dwarf("Sleepy", "Brown", 2, 500, Colors.Purple, new Point(2, 5), true));
			_lb1.Items.Add(new Dwarf("Dopey", "Green", 40, 300, Colors.Salmon, new Point(3, 7), false));
			_lb1.Items.Add(new Dwarf("Happy", "Red", 30, 400, Colors.Purple, new Point(5, 1), true));
			_lb1.Items.Add(new Dwarf("Grumpy", "Orange", 40, 275, Colors.Brown, new Point(7, 3), false));
			_lb1.Items.Add(new Dwarf("Bashful", "Purple", 30, 600, Colors.Gray, new Point(1, 5), true));
			_lb1.Items.Add(new Dwarf("Dopey", "Black", 40, 800, Colors.DeepPink, new Point(5, 2), false));
			_lb1.Items.Add(new Dwarf("Doc", "Pink", 40, 800, Colors.DarkMagenta, new Point(2, 1), false));

			WaitForPriority(DispatcherPriority.Render);

			return TestResult.Pass;
		}
		#endregion

		#region VerifyObjectsOnTree
		// Verifies objects are added to VisualTree correctly.
        private TestResult VerifyObjectsOnTree()
		{
			Status("Find visual elements.");
			_visualelements = Util.FindElements(_lb1, "dwarfTemplate");

			Status("Verify visual element count.");
			if(!(verifyCount(7, _visualelements.Length)))
				return TestResult.Fail;

			return TestResult.Pass;
		}
		#endregion

		#region ChangeItemsCollection
		// Change Collection and verify VisualTree gets updated.
        private TestResult ChangeItemsCollection()
		{
			Status("Remove a dwarf from beginning of collection.");
			_lb1.Items.RemoveAt(0);
			WaitForPriority(DispatcherPriority.Render);

			Status("Find visual elements.");
			findVisualElements();

			Status("Verify visual element count.");
			if (!(verifyCount(6, _visualelements.Length)))
				return TestResult.Fail;


			Status("Remove a dwarf from middle of collection.");
			_lb1.Items.RemoveAt(4);
			WaitForPriority(DispatcherPriority.Render);

			Status("Find visual elements.");
			findVisualElements();

			Status("Verify visual element count.");
			if (!(verifyCount(5, _visualelements.Length)))
				return TestResult.Fail;


			Status("Remove dwarf from end of collection.");
			_lb1.Items.RemoveAt(4);
			WaitForPriority(DispatcherPriority.Render);

			Status("Find visual elements.");
			findVisualElements();

			Status("Verify visual element count.");
			if (!(verifyCount(4, _visualelements.Length)))
				return TestResult.Fail;

			Status("Add a dwarf to collection.");
			_lb1.Items.Add(new Dwarf("Sleepy", "Brown", 2, 500, Colors.Purple, new Point(2, 5), true));
			WaitForPriority(DispatcherPriority.Render);

			Status("Find visual elements.");
			findVisualElements();

			Status("Verify visual element count.");
			if (!(verifyCount(5, _visualelements.Length)))
				return TestResult.Fail;


			return TestResult.Pass;
		}
		#endregion

		#region AuxMethods
		private void findVisualElements()
		{
			_visualelements = Util.FindElements(_lb1, "dwarfTemplate");
		}

		private bool verifyCount(int expectedCount, int actualCount)
		{
			if (expectedCount != actualCount)
			{
				LogComment("ItemsCollection count is incorrect.  Expected: '" + expectedCount + "'" + " Actual: '" + actualCount + "'");
				return false;
			}

			return true;
		}
		#endregion
	}
}
