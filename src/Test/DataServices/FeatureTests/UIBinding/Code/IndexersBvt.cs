// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// This tests binding using indexers. 3 situations are tested: when the data source takes an
    /// integer as the indexer, a string and a multi indexer. This also tests changing the source
    /// and making sure that it is reflected in the target.
    /// </description>
    /// <relatedBugs>


    /// </relatedBugs>
    /// </summary>
    [Test(2, "Binding", "IndexersBvt")]
    public class IndexersBvt : WindowTest
    {
        HappyMan _happy;
        Dwarf _dwarf;

        public IndexersBvt()
        {
            InitializeSteps += new TestStep(CreateTree);

            RunSteps += new TestStep(BindToIndexerInt);
            RunSteps += new TestStep(BindToIndexerString);
            RunSteps += new TestStep(BindToIndexerMulti);
        }

        private TestResult CreateTree()
        {
            Status("CreateTree");
            _happy = new HappyMan();
            _happy.Name = "George";
            _happy.Position = new Point(200, 200);
            _happy.Width = 200;
            _happy.Height = 200;
            Window.Content = _happy;

            DwarfBuddies dwarfbuddies = new DwarfBuddies();
            _dwarf = (Dwarf)dwarfbuddies[0];
            _happy.DataContext = dwarfbuddies;
            LogComment("CreateTree was successful");
            return TestResult.Pass;
        }


        private TestResult BindToIndexerInt()
        {
            Status("BindToIndexerInt");

            Binding bind = new Binding();
            bind.Path = new PropertyPath("Buddies[(0)].Name", (Int32)2);
            bind.Mode = BindingMode.OneWay;
            _happy.SetBinding(HappyMan.NameProperty, bind);
            if (_happy.Name != "Grumpy")
            {
                LogComment("Binding to indexer failed.  Expected 'Grumpy',   Actual: '" + _happy.Name + "'");
                return TestResult.Fail;
            }


            _dwarf.Buddies.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Buddies_CollectionChanged);
            _dwarf.Buddies.RemoveAt(2);
            _dwarf.Buddies[2] = new Dwarf("Bashful", "Pink", 40, 800, Colors.DarkMagenta, new Point(2, 1), false);

            TestResult res = WaitForSignal("CollectionChanged");
            if (res != TestResult.Pass) { return TestResult.Fail; }

            if (_happy.Name != "Bashful")
            {
                LogComment("Binding to indexer failed.  Expected 'Bashful',   Actual: '" + _happy.Name + "'");
                return TestResult.Fail;
            }

            LogComment("BindToIndexerInt was successful");
            return TestResult.Pass;
        }

        private void Buddies_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Status("Buddies_CollectionChanged event handler");
            Signal("CollectionChanged", TestResult.Pass);
        }

        // Verifies BindingExpression to IndexerStrings functionality.
        private TestResult BindToIndexerString()
        {
            Status("BindToIndexerString");

            Binding bind = new Binding("Buddies[Bashful].EyeColor");
            bind.Mode = BindingMode.OneWay;
            _happy.SetBinding(HappyMan.NameProperty, bind);
            if (_happy.Name != "Pink")
            {
                LogComment("Binding to string Indexer failed.  Expected 'Purple',   Actual: '" + _happy.Name + "'");
                return TestResult.Fail;
            }

            Status("Changing EyeColor");
            _dwarf.Buddies["Bashful"].EyeColor = "Pink";
            if (_happy.Name != "Pink")
            {
                LogComment("Binding to string indexer functionality failed.  Expected:  'Pink',   Actual: '" + _happy.Name + "'");
                return TestResult.Fail;
            }

            LogComment("BindToIndexerString was successful");
            return TestResult.Pass;
        }

        // Verifies binding to multi-indexers functionality.
        private TestResult BindToIndexerMulti()
        {
            Status("BindToIndexerMulti");

            Binding bind = new Binding("Buddies[Happy, Purple].EyeColor");
            bind.Mode = BindingMode.OneWay;
            _happy.SetBinding(HappyMan.NameProperty, bind);
            if (_happy.Name != "Red")
            {
                LogComment("Multi-indexer binding failed.  Expected:  'Red',   Actual: '" + _happy.Name + "'");
                return TestResult.Fail;
            }

            Status("Changing EyeColor");
            _dwarf.Buddies["Happy", Colors.Purple].EyeColor = "Yellow";
            if (_happy.Name != "Yellow")
            {
                LogComment("Binding to string indexer functionality failed.  Expected:  'Yellow',   Actual: '" + _happy.Name + "'");
                return TestResult.Fail;
            }

            LogComment("BindToIndexerMulti was successful");
            return TestResult.Pass;
        }
    }
}
