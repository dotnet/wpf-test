// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.ComponentModel;
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
	/// This test binds 2 properties of a control to 2 different view of the same data source
	/// (the default view and a named view).
	/// It verifies that changing currency in the view changes the bound value.
	/// </description>
	/// </summary>
    [Test(0, "Views", "CurrencyBvt")]
    public class CurrencyBvt : WindowTest
    {
        HappyMan _happy;
        Dwarf _dwarf;

        public CurrencyBvt()
        {
            InitializeSteps += new TestStep(CreateTree);

            RunSteps += new TestStep(DefaultView);
            RunSteps += new TestStep(NamedView);
        }

        private TestResult CreateTree()
        {
			Status("CreateTree");
			_happy = new HappyMan();
            _happy.Name = "Mr_X";
            _happy.Position = new Point(500, 500);
            _happy.Width = 200;
            _happy.Height = 200;
            _happy.EyeColor = Colors.LimeGreen;
            Window.Content = _happy;

            _dwarf = new Dwarf("Sleepy", "Brown", 2, 500, Colors.Purple, new Point(200, 500), true);
            _dwarf.Buddies.Add(new Dwarf("Dopey", "Green", 40, 300, Colors.Salmon, new Point(300, 700), false));
            _dwarf.Buddies.Add(new Dwarf("Happy", "Red", 30, 400, Colors.Purple, new Point(500, 100), true));
            _dwarf.Buddies.Add(new Dwarf("Grumpy", "Orange", 40, 275, Colors.Brown, new Point(700, 300), false));
            _dwarf.Buddies.Add(new Dwarf("Bashful", "Purple", 30, 600, Colors.Gray, new Point(100, 500), true));
            _dwarf.Buddies.Add(new Dwarf("Sneezy", "Black", 40, 800, Colors.DeepPink, new Point(500, 200), false));
            _dwarf.Buddies.Add(new Dwarf("Doc", "Pink", 40, 800, Colors.DarkMagenta, new Point(200, 100), false));

            _happy.DataContext = _dwarf;

			LogComment("CreateTree was successful");
			return TestResult.Pass;
        }

        private TestResult DefaultView()
        {
			Status("DefaultView");
			_happy.SetBinding(HappyMan.EyeColorProperty, "Buddies/EyeColor");
            ICollectionView view = CollectionViewSource.GetDefaultView(_dwarf.Buddies);
            if (_happy.EyeColor != Colors.Green)
            {
                LogComment("Bound value is incorrect.  Expected:  eye color Green.  Actual: '" + _happy.EyeColor + "'.");
                return TestResult.Fail;
            }

            Status("Navigating to last dwarf.");
            view.MoveCurrentToLast(); //sync
            if (_happy.EyeColor != Colors.Pink)
            {
                LogComment("Bound value is incorrect.  Expected:  eye color Pink.  Actual: '" + _happy.EyeColor + "'.");
                return TestResult.Fail;
            }

			LogComment("DefaultView was successful");
			return TestResult.Pass;
        }

        private TestResult NamedView()
        {
			Status("NamedView");
            CollectionViewSource cvs = new CollectionViewSource();
            cvs.Source = _dwarf.Buddies;
            Binding binding = new Binding("Name");
            binding.Source = cvs;
			_happy.SetBinding(HappyMan.NameProperty, binding);
            ICollectionView view = cvs.View;
            if (_happy.Name != "Dopey")
            {
                LogComment("Bound value is incorrect.  Expected:  'Dopey'.  Actual: '" + _happy.Name + "'.");
                return TestResult.Fail;
            }

            Status("Navigating to next dwarf.");
            view.MoveCurrentToNext(); //sync
            if (_happy.Name != "Happy")
            {
                LogComment("Bound value is incorrect.  Expected:  'Happy'.  Actual: '" + _happy.Name + "'.");
                return TestResult.Fail;
            }

            Status("Creating another binding the existing named view");
            binding = new Binding("SkinColor");
            binding.Source = cvs;
            _happy.SetBinding(HappyMan.SkinColorProperty, binding);
            if (_happy.SkinColor != Colors.Purple)
            {
                LogComment("Bound value is incorrect.  Expected:  'Purple'.  Actual: '" + _happy.SkinColor + "'.");
                return TestResult.Fail;
            }

			LogComment("NamedView was successful");
			return TestResult.Pass;
        }
    }
}
