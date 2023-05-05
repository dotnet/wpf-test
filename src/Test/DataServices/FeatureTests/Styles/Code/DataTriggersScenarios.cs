// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Xml;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests using a DataTrigger and a MultiDataTrigger inside a Style, a ControlTemplate 
    /// and a DataTemplate.
    /// </description>
    /// <relatedBugs>




    /// </relatedBugs>
    /// </summary>
    [Test(0, "Styles", "DataTriggersScenarios")]
    public class DataTriggersScenarios : XamlTest
	{
        private ListBox _lb;
        private Style _listBoxItemStyle;
        private Style _listBoxItemStyleMulti;
        private Style _listBoxItemStyle2;
        private Style _listBoxItemStyleMulti2;
        private DataTemplate _writerDataTemplate;
        private DataTemplate _writerDataTemplateMulti;

        public DataTriggersScenarios()
            : base(@"DataTriggersScenarios.xaml")
        {
			InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(DataTriggerInStyle);
            RunSteps += new TestStep(MultiDataTriggerInStyle);
            RunSteps += new TestStep(DataTriggerInControlTemplate);
            RunSteps += new TestStep(MultiDataTriggerInControlTemplate);
            RunSteps += new TestStep(DataTriggerInDataTemplate);
            RunSteps += new TestStep(MultiDataTriggerInDataTemplate);
        }

        private TestResult Setup()
        {
            Status("Setup");

            _lb = LogicalTreeHelper.FindLogicalNode(RootElement, "lb") as ListBox;
            if (_lb == null)
            {
                LogComment("Fail - Not able to reference ListBox lb");
                return TestResult.Fail;
            }

            _listBoxItemStyle = RootElement.Resources["listBoxItemStyle"] as Style;
            if (_listBoxItemStyle == null)
            {
                LogComment("Fail - Not able to reference Style listBoxItemStyle");
                return TestResult.Fail;
            }
            _listBoxItemStyleMulti = RootElement.Resources["listBoxItemStyleMulti"] as Style;
            if (_listBoxItemStyleMulti == null)
            {
                LogComment("Fail - Not able to reference Style listBoxItemStyleMulti");
                return TestResult.Fail;
            }
            _listBoxItemStyle2 = RootElement.Resources["listBoxItemStyle2"] as Style;
            if (_listBoxItemStyle2 == null)
            {
                LogComment("Fail - Not able to reference Style listBoxItemStyle2");
                return TestResult.Fail;
            }
            _listBoxItemStyleMulti2 = RootElement.Resources["listBoxItemStyleMulti2"] as Style;
            if (_listBoxItemStyleMulti2 == null)
            {
                LogComment("Fail - Not able to reference Style listBoxItemStyleMulti2");
                return TestResult.Fail;
            }
            _writerDataTemplate = RootElement.Resources["writerDataTemplate"] as DataTemplate;
            if (_writerDataTemplate == null)
            {
                LogComment("Fail - Not able to reference DataTemplate writerDataTemplate");
                return TestResult.Fail;
            }
            _writerDataTemplateMulti = RootElement.Resources["writerDataTemplateMulti"] as DataTemplate;
            if (_writerDataTemplateMulti == null)
            {
                LogComment("Fail - Not able to reference DataTemplate writerDataTemplateMulti");
                return TestResult.Fail;
            }

            Status("Setup was successful");
            return TestResult.Pass;
        }

        private TestResult DataTriggerInStyle()
        {
            Status("DataTriggerInStyle");

            if (!CheckTriggerInStyle(_listBoxItemStyle)) { return TestResult.Fail; }

            Status("DataTriggerInStyle was successful");
            return TestResult.Pass;
        }

        private TestResult MultiDataTriggerInStyle()
        {
            Status("MultiDataTriggerInStyle");

            if (!CheckTriggerInStyle(_listBoxItemStyleMulti)) { return TestResult.Fail; }

            Status("MultiDataTriggerInStyle was successful");
            return TestResult.Pass;
        }


        private TestResult DataTriggerInControlTemplate()
        {
            Status("DataTriggerInControlTemplate");

            if (!CheckTriggerInControlTemplate(_listBoxItemStyle2)) { return TestResult.Fail; }

            Status("DataTriggerInControlTemplate was successful");
            return TestResult.Pass;
        }

        private TestResult MultiDataTriggerInControlTemplate()
        {
            Status("MultiDataTriggerInControlTemplate");

            if (!CheckTriggerInControlTemplate(_listBoxItemStyleMulti2)) { return TestResult.Fail; }

            Status("MultiDataTriggerInControlTemplate was successful");
            return TestResult.Pass;
        }

        private TestResult DataTriggerInDataTemplate()
        {
            Status("DataTriggerInDataTemplate");

            if (!CheckTriggerInDataTemplate(_writerDataTemplate)) { return TestResult.Fail; }

            Status("DataTriggerInDataTemplate was successful");
            return TestResult.Pass;
        }

        private TestResult MultiDataTriggerInDataTemplate()
        {
            Status("MultiDataTriggerInDataTemplate");

            if (!CheckTriggerInDataTemplate(_writerDataTemplateMulti)) { return TestResult.Fail; }

            Status("MultiDataTriggerInDataTemplate was successful");
            return TestResult.Pass;
        }

        #region AuxMethods
        private bool CheckTriggerInStyle(Style style)
        {
            int triggeredIndex = 0;

            _lb.ItemContainerStyle = style;
            _lb.ItemTemplate = null;
            WaitForPriority(DispatcherPriority.Render);

            for (int i = 0; i < _lb.Items.Count; i++)
            {
                ListBoxItem item = _lb.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                if (!CheckBackground((Control)item, triggeredIndex, i)) { return false; }
            }
            return true;
        }

        private bool CheckTriggerInControlTemplate(Style style)
        {
            int triggeredIndex = 3;
            _lb.ItemContainerStyle = style;
            _lb.ItemTemplate = null;
            WaitForPriority(DispatcherPriority.Render);

            for (int i = 0; i < _lb.Items.Count; i++)
            {
                ListBoxItem item = _lb.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                TextBox rootVisual = VisualTreeHelper.GetChild(item, 0) as TextBox;
                if (!CheckBackground((Control)rootVisual, triggeredIndex, i)) { return false; }
            }
            return true;
        }

        private bool CheckTriggerInDataTemplate(DataTemplate dataTemplate)
        {
            int triggeredIndex = 2;
            _lb.ItemContainerStyle = null;
            _lb.ItemTemplate = dataTemplate;
            WaitForPriority(DispatcherPriority.Render);

            for (int i = 0; i < _lb.Items.Count; i++)
            {
                // gets the first visual of the ContentPresenter with the Content passed in 2nd param
                // starts searching from 1st param down
                TextBox tb = Util.FindDataVisual(_lb, _lb.Items[i]) as TextBox;
                if (!CheckBackground((Control)tb, triggeredIndex, i)) { return false; }
            }
            return true;
        }

        private bool CheckBackground(Control control, int indexColoredItem, int i)
        {
            if (i == indexColoredItem)
            {
                if (control.Background != Brushes.Brown)
                {
                    LogComment("Fail - DataTrigger should have triggered for this item");
                    return false;
                }
            }
            else
            {
                if (control.Background != Brushes.Tan)
                {
                    LogComment("Fail - DataTrigger should not have triggered for item in index " + i);
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}

