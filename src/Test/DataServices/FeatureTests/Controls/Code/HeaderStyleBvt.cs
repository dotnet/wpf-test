// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.ComponentModel;

using System.Windows.Controls;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Tests Explicit and resource headerstyle functionality.
	/// </description>
	/// <relatedBugs>

	/// </relatedBugs>
	/// </summary>
    [Test(2, "Controls", "HeaderStyleBvt")]
	public class HeaderStyleBvt : WindowTest
	{
		Menu _menu1;
		MenuItem _mheader,_item1,_item2,_item3;
		FrameworkElementFactory _menuitem;
		DataTemplate _explicitTemplate,_resourceTemplate;

		public HeaderStyleBvt()
		{
			InitializeSteps += new TestStep(CreateTree);
			RunSteps += new TestStep(ExplicitStyle);
			RunSteps += new TestStep(ResourceStyle);
			RunSteps += new TestStep(RemoveStyle);

		}

        private TestResult CreateTree()
		{
			DockPanel dp = new DockPanel();

			_menu1 = new Menu();
			_menu1.SetValue(DockPanel.DockProperty, Dock.Top);
			Window.Content = dp;
			dp.Children.Add(_menu1);

			_mheader = new MenuItem();
			_menu1.Items.Add(_mheader);

			_mheader.Header = "File";
			_item1 = new MenuItem();
			_item2 = new MenuItem();
			_item3 = new MenuItem();

			_item1.Header = "Open";
			_item2.Header = "Save";
			_item3.Header = "Close";

			_mheader.Items.Add(_item1);
			_mheader.Items.Add(_item2);
			_mheader.Items.Add(_item3);

			_explicitTemplate = new DataTemplate();
			_menuitem = new FrameworkElementFactory(typeof(TextBlock));
            _menuitem.SetBinding(TextBlock.TextProperty, new Binding());
            _menuitem.SetValue(TextBlock.FontSizeProperty, 25.0);
            _menuitem.SetValue(TextBlock.NameProperty, "Explicit");
            _explicitTemplate.VisualTree = _menuitem;
			_item2.HeaderTemplate = _explicitTemplate;

			_resourceTemplate = new DataTemplate();

			FrameworkElementFactory simpletext = new FrameworkElementFactory(typeof(TextBlock));

			_resourceTemplate.VisualTree = simpletext;
            simpletext.SetBinding(TextBlock.TextProperty, new Binding());
            simpletext.SetValue(TextBlock.FontStyleProperty, FontStyles.Italic);
            simpletext.SetValue(TextBlock.NameProperty, "Resource");
            _menu1.Resources = new ResourceDictionary();
			_menu1.Resources[new DataTemplateKey(typeof(string))] = _resourceTemplate;

			_mheader.IsSubmenuOpen = true;
			WaitForPriority(DispatcherPriority.Render);

			return TestResult.Pass;
		}

		// Verifies Elements with explicit style contain the correct bound and property value.
        private TestResult ExplicitStyle()
		{
			//only doing verification due to timing issue (need to set style when creating tree)
			//item2.HeaderTemplate = explicitTemplate;	//throws exception if setting it here
			WaitForPriority(DispatcherPriority.Render);

			Status("Find menuitem that has a explicit style set.");
			TextBlock txt = Util.FindElement(_item2, "Explicit") as TextBlock;

			Status("Verify properties value.");
			if ((txt.Text != "Save") || (txt.FontSize != 25))
			{
				LogComment("Values are incorrect.  Expected:  bound='Save'.  Font=25.  ActualBound:  " + txt.Text + "ActualProperty:  " + txt.FontSize);
				return TestResult.Fail;
			}

			return TestResult.Pass;
		}

		// Verifies Elements with resource style contain the correct bound and property value.
        private TestResult ResourceStyle()
		{
			Status("Find elements that use the resource style.");
			TextBlock sText1 = Util.FindElement(_mheader, "Resource") as TextBlock;
			TextBlock sText2 = Util.FindElement(_item1, "Resource") as TextBlock;
			TextBlock sText3 = Util.FindElement(_item3, "Resource") as TextBlock;

			Status("Verify properties value.");
			if (
				(sText1.Text != "File") ||
				(sText2.Text != "Open") ||
				(sText3.Text != "Close")||
				(sText1.FontStyle != FontStyles.Italic) ||
				(sText2.FontStyle != FontStyles.Italic) ||
				(sText3.FontStyle != FontStyles.Italic)
				)
			{
				LogComment("Bound values are incorrect.  Expected:  File, Open, Close with Italic font.");
				return TestResult.Fail;
			}

			return TestResult.Pass;
		}


        private TestResult RemoveStyle()
		{
            _item2.HeaderTemplate = null;
            TextBlock sText1 = Util.FindElement(_item2, "Resource") as TextBlock;

            if ((sText1.Text != "Save") || (sText1.FontStyle != FontStyles.Italic))
            {
                LogComment("Bound values are incorrect.  Expected:  Save with Italic font.");
                return TestResult.Fail;
            }

			return TestResult.Pass;
		}
	}
}




