// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using Avalon.Test.CoreUI.Common;
using System.Windows.Media;
using System.Windows.Documents;
using System.Collections;
using Avalon.Test.CoreUI.Parser;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using Microsoft.Test.Serialization.CustomElements;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// Verify xaml files for Animation
	/// Verification method for xaml files from ComponentModel team
 
    /// </summary>
	public class ConnectedDataVerifiers
	{
		/// <summary>
		/// 
		/// </summary>
		public static void XmlDataSourceVerify(ICustomElement c)
		{
			DockPanel myPanel = c as DockPanel;

			VerifyElement.VerifyBool(null == myPanel, false);
			
			CoreLogger.LogStatus("Verifying Resources ...");

			ResourceDictionary myResources = myPanel.Resources;

			VerifyElement.VerifyBool(null == myResources, false);
			VerifyElement.VerifyInt(myResources.Count, 2);
			String[] myKeys = new String[3];
			myResources.Keys.CopyTo(myKeys, 0);
			foreach (string key in myKeys)
				CoreLogger.LogStatus("key: " + key);

			CoreLogger.LogStatus("Verify DSO ...");
			if (false == myResources.Contains("DSO"))
				CoreLogger.LogStatus("NoDSO");
			else
			{
				Type myType = myResources["DSO"].GetType();

				if (null == myType)
					CoreLogger.LogStatus("null myResources[DSO]");
				else
					CoreLogger.LogStatus("Type1: " + myType.FullName);
			}

			System.Windows.Data.XmlDataProvider myDSO = myResources["DSO"] as System.Windows.Data.XmlDataProvider;
			VerifyElement.VerifyBool(null == myDSO, false);
			VerifyElement.VerifyString(myDSO.XPath, "XmlRoot");
			XmlDocument myDocument = myDSO.Document;
			String docStr= "<XmlRoot xmlns=\"\"><Magazine ISBN=\"1000\"><Title>Popular Science</Title></Magazine><Magazine ISBN=\"1000\"><Title>Car n Track</Title></Magazine><Magazine ISBN=\"1000\"><Title>Organic Gardening</Title></Magazine><Magazine ISBN=\"1000\"><Title>Hockey Digest</Title></Magazine></XmlRoot>";
			VerifyElement.VerifyString(myDocument.OuterXml, docStr);
			VerifyElement.VerifyBool(null == myDocument, false);

			CoreLogger.LogStatus("Verify Styles ...");

			if (false == myResources.Contains("DSO"))
				CoreLogger.LogStatus("NoDSO");
			else
			{
				Type myType = myResources["MyTemplate"].GetType();

				if (null == myType)
                    CoreLogger.LogStatus("null myResources[MyTemplate]");
				else
					CoreLogger.LogStatus("Type2: " + myType.FullName);
			}
            System.Windows.DataTemplate myStyle = myResources["MyTemplate"] as System.Windows.DataTemplate;
			VerifyElement.VerifyBool(null == myStyle, false);
			//FrameworkElementFactory myVisualTree = myStyle.VisualTree;
			//VerifyElement.VerifyBool(null == myVisualTree, false);

			//verify text
			CoreLogger.LogStatus("verify title ...");
			CoreLogger.LogStatus(myPanel.Children.Count.ToString() + " : " + myPanel.Children[0].GetType().FullName);
			System.Windows.Controls.ListBox myListBox = myPanel.Children[0] as System.Windows.Controls.ListBox;
			VerifyElement.VerifyBool(null == myListBox, false);
		}
	}

}
