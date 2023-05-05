// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
	/// <description>
    /// Tests binding to a subproperty and to a subproperty's subproperty.
    /// </description>
	/// </summary>
    [Test(0, "Binding", "SubPropertyBvt")]
    public class SubPropertyBvt : WindowTest
    {
        HappyMan _happy;
        Dwarf _dwarf;

        public SubPropertyBvt()
        {
            InitializeSteps += new TestStep(CreateTree);

            RunSteps += new TestStep(BindToSubProperty);
            RunSteps += new TestStep(BindToSubSubProperty);
		}

		private TestResult CreateTree()
        {
			Status("CreateTree");

			_happy = new HappyMan();

            _happy.Name = "George";
            _happy.Position = new Point(200,200);
            _happy.Width = 200;
            _happy.Height = 200;

            Window.Content = _happy;

            _dwarf = new Dwarf("Sleepy", "Brown", 2, 500, Colors.Purple, new Point(2, 5), true);

            _happy.DataContext = _dwarf;

			LogComment("CreateTree was successful");
			return TestResult.Pass;
		}

		// Verify Binding to subproperty functionality
		private TestResult BindToSubProperty()
        {
			Status("BindToSubProperty");

			_dwarf.Friend = new Dwarf("Dopey", "Red", 40, 400, Colors.Salmon, new Point(3, 7), false);
			//dwarf.Friend.Friend = new Dwarf("Sneezy", "Orange", 40, 400, Colors.RosyBrown, new Point(3, 7), false);

			Status("Binding to Friend.Name.");
            Binding bind = new Binding("Friend.Name");
            //bind.Mode = BindingMode.TwoWay;
            _happy.SetBinding(HappyMan.NameProperty, bind);
            if (_happy.Name != "Dopey")
            {
                LogComment("Bound value is incorrect.  Expected:  'Dopey',   Actual: '" + _happy.Name + "'");
                return TestResult.Fail;
            }

            Status("Changing Friend.Name");
            _dwarf.Friend.Name = "Foo_1";

            if (_happy.Name != "Foo_1")
            {
                LogComment("Bound value is incorrect.  Expected:  'Foo_1',   Actual: '" + _happy.Name + "'");
                return TestResult.Fail;
            }

            Status("Changing Friend");
            _dwarf.Friend = new Dwarf("CoolDaddy", "Brown", 2, 500, Colors.Purple, new Point(2, 5), true);
            if (_happy.Name != "CoolDaddy")
            {
                LogComment("Bound value is not correct.  Expected 'CoolDaddy',   Actual: '" + _happy.Name + "'");
                return TestResult.Fail;
            }

			LogComment("BindToSubProperty was successful");
			return TestResult.Pass;
		}

		// Verify bind to subproperty subproperty functionality
		private TestResult BindToSubSubProperty()
        {
			Status("BindToSubSubProperty");

			_dwarf.Friend.Friend = new Dwarf("Sneezy", "Orange", 40, 400, Colors.RosyBrown, new Point(3, 7), false);

            Status("Binding to Friend.Friend.Name.");
            Binding bind = new Binding("Friend.Friend.Name");
            //bind.Mode = BindingMode.TwoWay;
            _happy.SetBinding(HappyMan.NameProperty, bind);
            if (_happy.Name != "Sneezy")
            {
                LogComment("Bound value is not correct.  Expected 'Sneezy',   Actual: '" + _happy.Name + "'");
                return TestResult.Fail;
            }

            Status("Changing Friend.Friend.Name");
            _dwarf.Friend.Friend.Name = "Foo_2";
            if (_happy.Name != "Foo_2")
            {
                LogComment("Bound value is not correct.  Expected 'Foo_2',   Actual: '" + _happy.Name + "'");
                return TestResult.Fail;
            }

            Status("Changing Friend.Friend");
            _dwarf.Friend.Friend = new Dwarf("Smelly", "Brown", 2, 500, Colors.Purple, new Point(2, 5), true);
            if (_happy.Name != "Smelly")
            {
                LogComment("Bound value is not correct.  Expected 'Smelly',   Actual: '" + _happy.Name + "'");
                return TestResult.Fail;
            }

            Status("Changing Friend");
            Dwarf GDawg = new Dwarf("GDawg", "Brown", 2, 500, Colors.Purple, new Point(2, 5), true);
            GDawg.Friend = new Dwarf("Bumquisha", "Brown", 2, 500, Colors.Purple, new Point(2, 5), true);
            _dwarf.Friend = GDawg;
            if (_happy.Name != "Bumquisha")
            {
                LogComment("Bound value is not correct.  Expected 'Bumquisha',   Actual: '" + _happy.Name + "'");
                return TestResult.Fail;
            }

			LogComment("BindToSubSubProperty was successful");
			return TestResult.Pass;
		}
	}
}
