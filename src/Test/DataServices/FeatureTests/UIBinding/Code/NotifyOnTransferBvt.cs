// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests basic functionality of NotifyOnTransfer.
    /// </description>
    /// </summary>
    [Test(2, "Binding", "NotifyOnTransferBvt")]
    public class NotifyOnTransferBvt : WindowTest
    {
        HappyMan _happy;
        Dwarf _dwarf;
        int _counterTargetUpdated = 0;
        int _counterSourceUpdated = 0;
        object _senderTargetUpdated;
        object _senderSourceUpdated;
        DataTransferEventArgs _argsTargetUpdated;
        DataTransferEventArgs _argsSourceUpdated;

        public NotifyOnTransferBvt()
        {
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(SetupBinding);
            RunSteps += new TestStep(UpdateSourceAndTargetFires);
            RunSteps += new TestStep(RemoveEventHandlers);
            RunSteps += new TestStep(UpdateSourceAndTargetDoesNotFire);
            RunSteps += new TestStep(AddEventHandlers);
            RunSteps += new TestStep(UpdateSourceAndTargetFires);
            RunSteps += new TestStep(ClearDataContext);
            RunSteps += new TestStep(SetDataContextInParent);
            RunSteps += new TestStep(CreateNewBinding);
            RunSteps += new TestStep(ChangeBinding);
        }

        private TestResult CreateTree()
        {
            Status("CreateTree");
            _happy = new HappyMan();
            _happy.Name = "George";
            _happy.Position = new Point(100, 100);
            _happy.Width = 200;
            _happy.Height = 200;
            Window.Content = _happy;

            _dwarf = new Dwarf("Sleepy", "Yellow", 30, 500, Colors.Purple, new Point(2, 5), true);

            return TestResult.Pass;
        }

        // Validates NotifyOnTargetUpdated gets fired the correct number of times.
        private TestResult SetupBinding()
        {
            Status("SetupBinding");

            _happy.DataContext = _dwarf;
            _happy.AddHandler(Binding.TargetUpdatedEvent, new EventHandler<DataTransferEventArgs>(OnTargetUpdated));
            _happy.AddHandler(Binding.SourceUpdatedEvent, new EventHandler<DataTransferEventArgs>(OnSourceUpdated));
            Binding bind = new Binding("SkinColor");
            bind.Mode = BindingMode.TwoWay;
            bind.ConverterCulture = CultureInfo.InvariantCulture;
            bind.NotifyOnTargetUpdated = true;
            bind.NotifyOnSourceUpdated = true;
            _happy.SetBinding(HappyMan.SkinColorProperty, bind);

            // target is updated, source is not
            WaitForPriority(DispatcherPriority.Render);
            if (!ValidateTargetUpdated(_happy, 1, HappyMan.SkinColorProperty))
                return TestResult.Fail;
            if (!ValidateSourceUpdated(_happy, 0, HappyMan.SkinColorProperty))
                return TestResult.Fail;

            return TestResult.Pass;
        }

        // Updating the source causes TargetUpdated event to fire, but SourceUpdated does not fire
        // Updating the target's prop does not cause TargetUpdated to fire, but SourceUpdated fires
        private TestResult UpdateSourceAndTargetFires()
        {
            Status("UpdateSourceAndTargetFires");

            _dwarf.SkinColor = Colors.Purple;
            if (!ValidateTargetUpdated(_happy, 1, HappyMan.SkinColorProperty))
                return TestResult.Fail;
            if (!ValidateSourceUpdated(_happy, 0, HappyMan.SkinColorProperty))
                return TestResult.Fail;

            _happy.SkinColor = Colors.HotPink;
            if (!ValidateTargetUpdated(_happy, 0, HappyMan.SkinColorProperty))
                return TestResult.Fail;
            if (!ValidateSourceUpdated(_happy, 1, HappyMan.SkinColorProperty))
                return TestResult.Fail;

            return TestResult.Pass;
        }

        // Updating the source or target does not fire any handler
        private TestResult UpdateSourceAndTargetDoesNotFire()
        {
            Status("UpdateSourceAndTargetDoesNotFire");

            _dwarf.SkinColor = Colors.Purple;
            if (!ValidateTargetUpdated(_happy, 0, HappyMan.SkinColorProperty))
                return TestResult.Fail;
            if (!ValidateSourceUpdated(_happy, 0, HappyMan.SkinColorProperty))
                return TestResult.Fail;

            _happy.SkinColor = Colors.HotPink;
            if (!ValidateTargetUpdated(_happy, 0, HappyMan.SkinColorProperty))
                return TestResult.Fail;
            if (!ValidateSourceUpdated(_happy, 0, HappyMan.SkinColorProperty))
                return TestResult.Fail;

            return TestResult.Pass;
        }

        // remove event handlers - no events should be fired
        private TestResult RemoveEventHandlers()
        {
            Status("RemoveEventHandlers");

            Binding.RemoveSourceUpdatedHandler(_happy, new EventHandler<DataTransferEventArgs>(OnSourceUpdated));
            Binding.RemoveTargetUpdatedHandler(_happy, new EventHandler<DataTransferEventArgs>(OnTargetUpdated));

            return TestResult.Pass;
        }

        // add event handlers back - events should be fired as usual
        private TestResult AddEventHandlers()
        {
            Status("AddEventHandlers");

            Binding.AddSourceUpdatedHandler(_happy, new EventHandler<DataTransferEventArgs>(OnSourceUpdated));
            Binding.AddTargetUpdatedHandler(_happy, new EventHandler<DataTransferEventArgs>(OnTargetUpdated));

            return TestResult.Pass;
        }

        // DataContext is cleared - value in target should be empty now, no changes in source
        private TestResult ClearDataContext()
        {
            Status("ClearDataContext");

            _happy.ClearValue(FrameworkElement.DataContextProperty);
            if (!ValidateTargetUpdated(_happy, 1, HappyMan.SkinColorProperty))
                return TestResult.Fail;
            if (!ValidateSourceUpdated(_happy, 0, HappyMan.SkinColorProperty))
                return TestResult.Fail;

            return TestResult.Pass;
        }

        // DataContext is set in parent - value should be what was before in target, no changes in source
        private TestResult SetDataContextInParent()
        {
            Status("SetDataContextInParent");

            Window.DataContext = _dwarf;
            if (!ValidateTargetUpdated(_happy, 1, HappyMan.SkinColorProperty))
                return TestResult.Fail;
            if (!ValidateSourceUpdated(_happy, 0, HappyMan.SkinColorProperty))
                return TestResult.Fail;

            return TestResult.Pass;
        }

        // Changing the binding - TargetUpdated fires, SourceUpdated doesn't
        private TestResult CreateNewBinding()
        {
            Status("CreateNewBinding");

            Binding bind1 = new Binding("EyeColor");
            bind1.Mode = BindingMode.TwoWay;
            bind1.NotifyOnTargetUpdated = true;
            bind1.NotifyOnSourceUpdated = true;
            _happy.SetBinding(HappyMan.EyeColorProperty, bind1);

            WaitForPriority(DispatcherPriority.Render);
            if (!ValidateTargetUpdated(_happy, 1, HappyMan.EyeColorProperty))
                return TestResult.Fail;
            if (!ValidateSourceUpdated(_happy, 0, HappyMan.EyeColorProperty))
                return TestResult.Fail;

            return TestResult.Pass;
        }

        // Change the binding so that NotifyOn(Source|Target)Updated flag is false - no events are fired
        private TestResult ChangeBinding()
        {
            Status("ChangeBinding");

            _happy.SetBinding(HappyMan.EyeColorProperty, "EyeColor");

            WaitForPriority(DispatcherPriority.Render);
            if (!ValidateTargetUpdated(_happy, 0, HappyMan.EyeColorProperty))
                return TestResult.Fail;
            if (!ValidateSourceUpdated(_happy, 0, HappyMan.EyeColorProperty))
                return TestResult.Fail;

            return TestResult.Pass;
        }

        private void OnTargetUpdated(object sender, DataTransferEventArgs args)
        {
            Status("OnTargetUpdated");
            this._counterTargetUpdated++;
            this._senderTargetUpdated = sender;
            this._argsTargetUpdated = args;
        }

        private void OnSourceUpdated(object sender, DataTransferEventArgs args)
        {
            Status("OnSourceUpdated");
            this._counterSourceUpdated++;
            this._senderSourceUpdated = sender;
            this._argsSourceUpdated = args;
        }

        // Validates event count and sender value
        private bool ValidateTargetUpdated(object sender, int count, DependencyProperty dp)
        {
            if (this._counterTargetUpdated != count)
            {
                LogComment("NotifyOnTargetUpdated Event was called an unexpected amount of times!  Actual: " + _counterTargetUpdated + " - Expected: " + count);
                return false;
            }

            if (count == 0) //dont validate args if it never fired
                return true;

            if (this._senderTargetUpdated != sender)
            {
                LogComment("Sender was not the same. Expected: HappyMan");
                return false;
            }

            if (dp != _argsTargetUpdated.Property)
            {
                LogComment("Args.Property was not the same!  Actual: " + _argsTargetUpdated.Property.Name + " - Expected: " + dp.Name);
                return false;
            }

            _counterTargetUpdated = 0;
            return true;
        }

        private bool ValidateSourceUpdated(object sender, int count, DependencyProperty dp)
        {
            if (this._counterSourceUpdated != count)
            {
                LogComment("NotifyOnSourceUpdated Event was called an unexpected amount of times!  Actual: " + _counterSourceUpdated + " - Expected: " + count);
                return false;
            }
            if (count == 0) //dont validate args if it never fired
                return true;

            _counterSourceUpdated = 0;
            return true;
        }
    }
}
