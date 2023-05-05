// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Windows.Markup;
using System.Windows.Documents;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;
using System.Globalization;
using Microsoft.Test.Modeling;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// This adds the possibility to start an animation when a trigger/datatrigger's condition 
    /// evaluates to true and another one it when it evaluates to false.
    /// </description>
    /// </summary>
    [Test(3, "Styles", SupportFiles = @"FeatureTests\DataServices\ActionsInTriggersTestCases.xtc")]
    public class ActionsInTriggersModel : XamlModel
    {
        // controls
        ContentControl _contentControl;
        ItemsControl _itemsControl;
        //TableRowGroup tableRowGroup;
        TreeView _treeView;

        // templates
        DataTemplate _dataTemplateSingle;
        DataTemplate _dataTemplateMulti;
        
        /* DCR TableTemplate Removed
        TableTemplate tableTemplateSingle;
        TableTemplate tableTemplateMulti;
        */
        
        HierarchicalDataTemplate _hdtSingle;
        HierarchicalDataTemplate _hdtMulti;

        Color _conditionTrueColor = Colors.Blue;
        Color _conditionFalseColor = Colors.Red;
        int _placesIndex = 6;
        int _leaguesIndex = 0;

        [Variation(4)] // DataTemplate + ContentControl + DataTriger + without setter
        [Variation(8)] // DataTemplate + ContentControl + DataTriger + with setter
        [Variation(1)] // DataTemplate + ContentControl + MultiDataTriger + without setter
        [Variation(12)] // DataTemplate + ContentControl + MultiDataTriger + with setter
        [Variation(6)] // DataTemplate + ItemsControl + DataTriger + without setter
        [Variation(2)] // DataTemplate + ItemsControl + DataTriger + with setter
        [Variation(16)] // DataTemplate + ItemsControl + MultiDataTriger + without setter
        [Variation(14)] // DataTemplate + ItemsControl + MultiDataTriger + with setter
        
        /* DCR TableTemplate Removed
        [Variation(7)] // TableTemplate + Table + DataTriger + without setter
        [Variation(5)] // TableTemplate + Table + DataTriger + with setter
        [Variation(10)] // TableTemplate + Table + MultiDataTriger + without setter
        [Variation(3)] // TableTemplate + Table + MultiDataTriger + with setter
        */
        
        [Variation(15)] // HierarchicalDataTemplate + TreeView + DataTriger + without setter
        [Variation(9)] // HierarchicalDataTemplate + TreeView + DataTriger + with setter
        [Variation(11)] // HierarchicalDataTemplate + TreeView + MultiDataTriger + without setter
        [Variation(13)] // HierarchicalDataTemplate + TreeView + MultiDataTriger + with setter
        public ActionsInTriggersModel(int testCaseNumber)
            : this(testCaseNumber, testCaseNumber)
        {
        }

        public ActionsInTriggersModel(int beginTestCaseNumber, int endTestCaseNumber)
            : this("ActionsInTriggersModel.xaml", "ActionsInTriggersTestCases.xtc", beginTestCaseNumber, endTestCaseNumber)
        {
        }

        public ActionsInTriggersModel(string xamlFileName, string xtcFileName, int beginTestCaseNumber, int endTestCaseNumber)
            : base(xamlFileName, xtcFileName, beginTestCaseNumber, endTestCaseNumber)
        {
            AddAction("RunAction", new ActionHandler(RunAction));
        }

        private bool RunAction(State endState, State inParams, State outParams)
        {
            Status("RunAction");
            WaitForPriority(DispatcherPriority.Render);

            string triggerType = inParams["TriggerType"];
            string control = inParams["Control"];
            string template = inParams["Template"];
            string setter = inParams["Setter_1"];

            // Test cases 4 and 8
            if ((triggerType == "DataTrigger") && (control == "ContentControl") && 
                (template == "DataTemplate"))
            {
                _contentControl = (ContentControl)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "contentControl"));
                _dataTemplateSingle = (DataTemplate)(this.RootElement.Resources["dataTemplateSingle"]);

                if (setter == "WithoutSetter")
                {
                    DataTrigger trigger = (DataTrigger)(_dataTemplateSingle.Triggers[0]);
                    trigger.Setters.RemoveAt(0);
                }

                _contentControl.ContentTemplate = _dataTemplateSingle;

                // Trigger condition is true
                if (!VerifyTriggerResults(setter, true, _contentControl)) { return false; }

                // Change data to change trigger condition
                Place place = (Place)(this.RootElement.Resources["place"]);
                place.State = "VT";

                // Trigger condition is false
                if (!VerifyTriggerResults(setter, false, _contentControl)) { return false; }
            }
            // Test cases 1 and 12
            else if ((triggerType == "MultiDataTrigger") && (control == "ContentControl") &&
                (template == "DataTemplate"))
            {
                _contentControl = (ContentControl)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "contentControl"));
                _dataTemplateMulti = (DataTemplate)(this.RootElement.Resources["dataTemplateMulti"]);

                if (setter == "WithoutSetter")
                {
                    MultiDataTrigger trigger = (MultiDataTrigger)(_dataTemplateMulti.Triggers[0]);
                    trigger.Setters.RemoveAt(0);
                }

                _contentControl.ContentTemplate = _dataTemplateMulti;

                // Trigger condition is true
                if (!VerifyTriggerResults(setter, true, _contentControl)) { return false; }

                // Change data to change trigger condition
                Place place = (Place)(this.RootElement.Resources["place"]);
                place.State = "VT";

                // Trigger condition is false
                if (!VerifyTriggerResults(setter, false, _contentControl)) { return false; }
            }
            // Test cases 6 and 2
            else if((triggerType == "DataTrigger") && (control == "ItemsControl") && (template == "DataTemplate"))
            {
                _itemsControl = (ItemsControl)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "itemsControl"));
                _dataTemplateSingle = (DataTemplate)(this.RootElement.Resources["dataTemplateSingle"]);

                if (setter == "WithoutSetter")
                {
                    DataTrigger trigger = (DataTrigger)(_dataTemplateSingle.Triggers[0]);
                    trigger.Setters.RemoveAt(0);
                }

                _itemsControl.ItemTemplate = _dataTemplateSingle;

                // Trigger condition is true
                if (!VerifyTriggerResults(setter, true, _itemsControl)) { return false; }

                // Change data to change trigger condition
                Places places = (Places)(this.RootElement.Resources["places"]);
                places[_placesIndex].State = "VT";

                // Trigger condition is false
                if (!VerifyTriggerResults(setter, false, _itemsControl)) { return false; }
            }
            // Test cases 16 and 14
            else if ((triggerType == "MultiDataTrigger") && (control == "ItemsControl") && (template == "DataTemplate"))
            {
                _itemsControl = (ItemsControl)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "itemsControl"));
                _dataTemplateMulti = (DataTemplate)(this.RootElement.Resources["dataTemplateMulti"]);

                if (setter == "WithoutSetter")
                {
                    MultiDataTrigger trigger = (MultiDataTrigger)(_dataTemplateMulti.Triggers[0]);
                    trigger.Setters.RemoveAt(0);
                }

                _itemsControl.ItemTemplate = _dataTemplateMulti;

                // Trigger condition is true
                if (!VerifyTriggerResults(setter, true, _itemsControl)) { return false; }

                // Change data to change trigger condition
                Places places = (Places)(this.RootElement.Resources["places"]);
                places[_placesIndex].State = "VT";

                // Trigger condition is false
                if (!VerifyTriggerResults(setter, false, _itemsControl)) { return false; }
            }
            // Test cases 7 and 5
            /* DCR TableTemplate Removed
            else if ((triggerType == "DataTrigger") && (control == "Table") && (template == "TableTemplate"))
            {
                tableRowGroup = (TableRowGroup)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "tableRowGroup"));
                tableTemplateSingle = (TableTemplate)(this.RootElement.Resources["tableTemplateSingle"]);

                if (setter == "WithoutSetter")
                {
                    DataTrigger trigger = (DataTrigger)(tableTemplateSingle.Triggers[0]);
                    trigger.Setters.RemoveAt(0);
                }

                tableRowGroup.ItemTemplate = tableTemplateSingle;

                // Trigger condition is true
                if (!VerifyTriggerResults(setter, true, tableRowGroup)) { return false; }

                // Change data to change trigger condition
                Places places = (Places)(this.RootElement.Resources["places"]);
                places[placesIndex].State = "VT";

                // Trigger condition is false
                if (!VerifyTriggerResults(setter, false, tableRowGroup)) { return false; }
            }
            // Test cases 10 and 3
            else if ((triggerType == "MultiDataTrigger") && (control == "Table") && (template == "TableTemplate"))
            {
                tableRowGroup = (TableRowGroup)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "tableRowGroup"));
                tableTemplateMulti = (TableTemplate)(this.RootElement.Resources["tableTemplateMulti"]);

                if (setter == "WithoutSetter")
                {
                    MultiDataTrigger trigger = (MultiDataTrigger)(tableTemplateMulti.Triggers[0]);
                    trigger.Setters.RemoveAt(0);
                }

                tableRowGroup.ItemTemplate = tableTemplateMulti;

                // Trigger condition is true
                if (!VerifyTriggerResults(setter, true, tableRowGroup)) { return false; }

                // Change data to change trigger condition
                Places places = (Places)(this.RootElement.Resources["places"]);
                places[placesIndex].State = "VT";

                // Trigger condition is false
                if (!VerifyTriggerResults(setter, false, tableRowGroup)) { return false; }
            }
            */
            // Test cases 15 and 9
            else if ((triggerType == "DataTrigger") && (control == "TreeView") && (template == "HierarchicalDataTemplate"))
            {
                _treeView = (TreeView)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "treeView"));
                _hdtSingle = (HierarchicalDataTemplate)(this.RootElement.Resources["hdtLeagueSingle"]);

                if (setter == "WithoutSetter")
                {
                    DataTrigger trigger = (DataTrigger)(_hdtSingle.Triggers[0]);
                    trigger.Setters.RemoveAt(0);
                }

                _treeView.ItemTemplate = _hdtSingle;

                // Trigger condition is true
                if (!VerifyTriggerResults(setter, true, _treeView)) { return false; }

                // Change data to change trigger condition
                ListLeagueList leagues = (ListLeagueList)(this.RootElement.Resources["leagues"]);
                leagues[_leaguesIndex].Name = "Wrong league name";

                // Trigger condition is false
                if (!VerifyTriggerResults(setter, false, _treeView)) { return false; }
            }
            // Test cases 11 and 13
            else if ((triggerType == "MultiDataTrigger") && (control == "TreeView") && (template == "HierarchicalDataTemplate"))
            {
                _treeView = (TreeView)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "treeView"));
                _hdtMulti = (HierarchicalDataTemplate)(this.RootElement.Resources["hdtLeagueMulti"]);

                if (setter == "WithoutSetter")
                {
                    MultiDataTrigger trigger = (MultiDataTrigger)(_hdtMulti.Triggers[0]);
                    trigger.Setters.RemoveAt(0);
                }

                _treeView.ItemTemplate = _hdtMulti;

                // Trigger condition is true
                if (!VerifyTriggerResults(setter, true, _treeView)) { return false; }

                // Change data to change trigger condition
                ListLeagueList leagues = (ListLeagueList)(this.RootElement.Resources["leagues"]);
                leagues[_leaguesIndex].Name = "Wrong league name";

                // Trigger condition is false
                if (!VerifyTriggerResults(setter, false, _treeView)) { return false; }
            }
            else
            {
                LogComment("Fail - Values passed to Setup are incorrect: TriggerType:" + inParams["TriggerType"] +
                    ": Control:" + inParams["Control"] + ": Template:" + inParams["Template"] +
                    ": Setter:" + inParams["Setter"] + ":");
                return false;
            }

            return true;
        }

        private bool VerifyTriggerResults(string setter, bool condition, DependencyObject dependencyObject)
        {
            Status("TestNonHierarchicalData");
            WaitForPriority(DispatcherPriority.SystemIdle);
            WaitFor(200); // this is terrible but it fails randomly otherwise in my machine 

            Color currentConditionColor;

            if (condition)
            {
                currentConditionColor = _conditionTrueColor;
            }
            else
            {
                currentConditionColor = _conditionFalseColor;
            }

            // Get the border around the data for the specific FrameworkElement
            Border dataBorder = null;
            if(dependencyObject is ContentControl)
            {
                dataBorder = (Border)(Util.FindDataVisual((ContentControl)dependencyObject, (Place)(this.RootElement.Resources["place"])));
            }
            // Notice that because TreeView is also an ItemsControl, this check has to come before ItemsControl's check
            else if (dependencyObject is TreeView)
            {
                TreeViewItem tvi = (TreeViewItem)(((TreeView)dependencyObject).ItemContainerGenerator.ContainerFromIndex(_leaguesIndex));
                dataBorder = (Border)(Util.FindDataVisual(tvi, ((ListLeagueList)(this.RootElement.Resources["leagues"]))[_leaguesIndex]));

            }
            else if (dependencyObject is ItemsControl)
            {
                ContentPresenter cpItem = (ContentPresenter)(((ItemsControl)dependencyObject).ItemContainerGenerator.ContainerFromIndex(_placesIndex));
                dataBorder = (Border)(Util.FindDataVisual(cpItem, ((Places)(this.RootElement.Resources["places"]))[_placesIndex]));
            }
            else if (dependencyObject is TableRowGroup)
            {
                IEnumerable tableRowsEnumerable = LogicalTreeHelper.GetChildren(dependencyObject);
                IEnumerator tableRowsEnumerator = tableRowsEnumerable.GetEnumerator();
                for (int i = 0; i <= _placesIndex; i++)
                {
                    tableRowsEnumerator.MoveNext();
                }
                IEnumerable bodyContainerProxyEnumerable = LogicalTreeHelper.GetChildren((DependencyObject)(tableRowsEnumerator.Current));
                IEnumerator bodyContainerProxyEnumerator = bodyContainerProxyEnumerable.GetEnumerator();
                bodyContainerProxyEnumerator.MoveNext();
                TableRow tableRow = (TableRow)(bodyContainerProxyEnumerator.Current);
                dataBorder = (Border)(LogicalTreeHelper.FindLogicalNode(tableRow, "myborder"));
            }

            // Verify Foreground of TextBlock
            TextBlock textBlock = (TextBlock)(dataBorder.Child);
            SolidColorBrush textBlockBrush = (SolidColorBrush)(textBlock.Foreground);

            if ((setter == "WithoutSetter") && (textBlockBrush.Color != _conditionFalseColor))
            {
                LogComment("Fail - Foreground color of TextBlock should be " + _conditionFalseColor.ToString() + ", instead it is " + textBlockBrush.ToString());
                return false;
            }
            else if ((setter == "WithSetter") && (textBlockBrush.Color != currentConditionColor))
            {
                LogComment("Fail - Foreground color of TextBlock should be " + currentConditionColor.ToString() + ", instead it is " + textBlockBrush.ToString());
                return false;
            }

            // Verify BorderBrush of Border
            SolidColorBrush dataBorderBrush = (SolidColorBrush)(dataBorder.BorderBrush);

            if (dataBorderBrush.Color != currentConditionColor)
            {
                LogComment("Fail - BorderBrush should be " + currentConditionColor.ToString() + ", instead it is " + dataBorderBrush.ToString());
                return false;
            }

            return true;
        }
    }
}
