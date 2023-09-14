using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System;
using Microsoft.Test.Input;
using Avalon.Test.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Controls.Primitives;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test   Verify that a shared context menu still
    /// works after it has failed to open because its owner was removed from the tree.
    /// </description>

    /// </summary>
    [Test(1, "ContextMenu", "RemoveContextMenuOwner", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class RemoveContextMenuOwner : RegressionXamlTest
    {
        private Model _model;
        private ListBox _listbox;
        private ContextMenu _contextMenu;

        #region Constructor

        public RemoveContextMenuOwner()
            : base(@"RemoveContextMenuOwner.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(RemoveOwnerWhileOpeningContextMenu);
            RunSteps += new TestStep(OpenContextMenu);
            RunSteps += new TestStep(CloseContextMenu);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial Setup
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            Status("Setup specific for RemoveContextMenuOwner");

            _model = Model.Create(100);
            RootElement.DataContext = _model;

            _listbox = (ListBox)RootElement.FindName("listbox");
            Assert.AssertTrue("Unable to find listBox", _listbox != null);

            _contextMenu = (ContextMenu)RootElement.FindResource("contextMenu");
            Assert.AssertTrue("Unable to find contextMenu from the resources", _contextMenu != null);

            QueueHelper.WaitTillQueueItemsProcessed();

            // Uncomment for ad-hoc debugging
            //base.Setup();

            LogComment("Setup for RemoveContextMenuOwner was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. right mouse-button down over the first item
        /// 2a. remove the first item from the collection
        /// 2b. right mouse-button up
        ///
        /// Verify context menu didn't open
        /// </summary>
        private TestResult RemoveOwnerWhileOpeningContextMenu()
        {
            Status("RemoveOwnerWhileOpeningContextMenu");

            // start the context menu process by pressing the right mouse button over the first item
            ListBoxItem lbi = _listbox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
            UserInput.MouseRightDownCenter(lbi);
            QueueHelper.WaitTillQueueItemsProcessed();

            // to get the effect we want, release the mouse button and remove the
            // item at the same time (without waiting for the queue to drain)
            Microsoft.Test.Input.Input.SendMouseInput(0,0, 0, SendMouseInputFlags.RightUp);
            _model.ItemList.RemoveAt(0);

            // when the dust settles, the lbi should not be in the tree, and
            // therefore the context menu should not be open
            QueueHelper.WaitTillQueueItemsProcessed();
            Assert.AssertTrue("ListBoxItem should be removed from tree", !lbi.IsVisible);
            Assert.AssertTrue("context menu for removed item should not be open", !_contextMenu.IsOpen);

            return TestResult.Pass;
        }

        /// <summary>
        /// 1. right mouse-button click over first item
        ///
        /// Verify context menu opened
        /// </summary>
        private TestResult OpenContextMenu()
        {
            Status("OpenContextMenu");

            ListBoxItem lbi = _listbox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
            UserInput.MouseRightClickCenter(lbi);

            QueueHelper.WaitTillQueueItemsProcessed();
            Assert.AssertTrue("context menu should be open at this point", _contextMenu.IsOpen);

            return TestResult.Pass;
        }

        /// <summary>
        /// 1. click away from the listbox
        ///
        /// Verify context menu closed
        /// </summary>
        private TestResult CloseContextMenu()
        {
            Status("CloseContextMenu");

            UserInput.MouseLeftDown(_listbox, -4, -4);
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.MouseLeftUp(_listbox, -4, -4);
            QueueHelper.WaitTillQueueItemsProcessed();

            Assert.AssertTrue("context menu should be closed at this point", !_contextMenu.IsOpen);

            return TestResult.Pass;
        }

        #endregion Test Steps

        #region Local types

        public class Model
        {
            ItemList _itemList;
            public ItemList ItemList
            {
                get { return _itemList; }
            }

            public static Model Create(int n)
            {
                ItemList list = new ItemList();
                for (int i=0; i<n; ++i)
                {
                    list.Add(new MyItem { Text = "Item " + i } );
                }

                Model model = new Model();
                model._itemList = list;
                return model;
            }
        }

        public class MyItem
        {
            public string Text { get; set; }
        }

        public class ItemList : ObservableCollection<MyItem>
        {
        }

        #endregion Local types
    }
}

