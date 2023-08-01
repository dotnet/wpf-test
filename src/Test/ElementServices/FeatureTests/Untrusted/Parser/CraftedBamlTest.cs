// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Markup;
using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Security;
using System.Security.Permissions;

using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Serialization;
using Avalon.Test.CoreUI.Common;

using Microsoft.Test;
using Microsoft.Test.Serialization;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Parser
{
    /// <summary>
    /// 
    /// </summary>
    public class CraftedBamlTest
    {
        #region RunTest
        /// <summary>
        /// Test case Entry point
        /// </summary>
        public void RunTest()
        {
            string strParams = DriverState.DriverParameters["Params"];

            switch (strParams)
            {
                case "Text_under_ComplexProp":
                    Text_under_ComplexProp();
                    break;
                case "Invalid_Text_under_ComplexProp":
                    Invalid_Text_under_ComplexProp();
                    break;
                case "No_IDictionary_Magic_Tag":
                    No_IDictionary_Magic_Tag();
                    break;
                case "NonProperty_in_Prop_record":
                    NonProperty_in_Prop_record();
                    break;
                default:
                    CoreLogger.LogStatus("CraftedBamlTest.RunTest was called with an unsupported parameter: " + strParams);
                    throw new Microsoft.Test.TestSetupException("Parameter " + strParams + " is not supported");
            }
        }
        #endregion RunTest

        #region Text_under_ComplexProp/Invalid_Text_under_ComplexProp
        /// <summary>
        /// Craft a BAML with text under a complex property,
        /// and verify that it parses correctly
        /// </summary>
        public void Text_under_ComplexProp()
        {
            string xamlName = "Text_under_ComplexProp.xaml";

            // Compile the XAML into BAML
            string bamlPath = ParserUtil.CompileXamlToBaml(xamlName);
            string newBamlPath = Path.GetDirectoryName(bamlPath) + "\\new_" + Path.GetFileName(bamlPath);

            // Edit the BAML, using a callback. 
            BamlHelper.EditBaml(bamlPath, newBamlPath, new BamlHelper.BamlNodeCallback(EditBaml1));

            Page rootPage = ParserUtil.LoadBamlFile(@"pack://siteoforigin:,,,/" + newBamlPath) as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast failed");
            DockPanel dockpanel = rootPage.Content as DockPanel;

            Button Button0 = null;

            Button0 = LogicalTreeHelper.FindLogicalNode(dockpanel, "Button0") as Button;

            if (!Color.Equals((Button0.Background as SolidColorBrush).Color, Colors.Green))
                throw new Microsoft.Test.TestValidationException("Button's background expected to be Green, but is actually " + (Button0.Background as SolidColorBrush).Color);
        }

        /// <summary>
        /// Craft a BAML with invalid text under a complex property,
        /// and verify that it it causes a XamlParseException.
        /// Invalid text is a string which cannot be converted using a 
        /// typeconverter.
        /// </summary>
        public void Invalid_Text_under_ComplexProp()
        {
            string xamlName = "Invalid_Text_under_ComplexProp.xaml";

            // Compile the XAML into BAML
            string bamlPath = ParserUtil.CompileXamlToBaml(xamlName);
            string newBamlPath = Path.GetDirectoryName(bamlPath) + "\\new_" + Path.GetFileName(bamlPath);

            // Edit the BAML, using a callback. 
            BamlHelper.EditBaml(bamlPath, newBamlPath, new BamlHelper.BamlNodeCallback(EditBaml1));

            try
            {
                ParserUtil.LoadBamlFile(@"pack://siteoforigin:,,,/" + newBamlPath);
                throw new Microsoft.Test.TestValidationException("LoadBaml() was expected to throw, but didn't");
            }
            catch (XamlParseException) { }
        }

        /// <summary>
        /// Callback that changes a simple property to a complex property with text inside
        /// </summary>
        /// <param name="actualData"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        public BamlNodeAction EditBaml1(BamlNodeData actualData, BamlWriterWrapper writer)
        {
            // We are changing a simple Button.Background property to a complex property with 
            // text under it.
            if (actualData.Name == "System.Windows.Controls.Control.Background")
            {
                writer.WriteStartComplexProperty(actualData.AssemblyName, actualData.Name.Substring(0, actualData.Name.LastIndexOf('.')), actualData.LocalName);
                writer.WriteText(actualData.Value, actualData.TypeConverterAssemblyName, actualData.TypeConverterName);
                writer.WriteEndComplexProperty();
                return BamlNodeAction.Skip;
            }
            else
            {
                return BamlNodeAction.Continue;
            }
        }
        #endregion

        #region No_IDictionary_Magic_Tag
        /// <summary>
        /// Craft a BAML so that the "ResourceDictionary" magic tag usually inserted under 
        /// DockPanel.Resources property is removed. This changes a codepath in BamlRecordReader.
        /// </summary>
        public void No_IDictionary_Magic_Tag()
        {
            string xamlName = "No_IDictionary_Magic_Tag.xaml";

            // Compile the XAML into BAML
            string bamlPath = ParserUtil.CompileXamlToBaml(xamlName);
            string newBamlPath = Path.GetDirectoryName(bamlPath) + "\\new_" + Path.GetFileName(bamlPath);

            // Edit the BAML, using a callback. 
            BamlHelper.EditBaml(bamlPath, newBamlPath, new BamlHelper.BamlNodeCallback(EditBaml2));

            // Load the new BAML and verify
            Page rootPage = ParserUtil.LoadBamlFile(@"pack://siteoforigin:,,,/" + newBamlPath) as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast failed");
            DockPanel dockpanel = rootPage.Content as DockPanel;

            ResourceDictionary resources = dockpanel.Resources;

            SolidColorBrush greenBrush = resources["GreenBrush"] as SolidColorBrush;
            if (null == greenBrush)
                throw new Microsoft.Test.TestValidationException("Addition of resource {GreenBrush} to DockPanel's resources didn't happen");

            if (!Color.Equals(greenBrush.Color, Colors.Green))
                throw new Microsoft.Test.TestValidationException("Color of {GreenBrush} expected to be green, but it's " + greenBrush.Color);

        }

        /// <summary>
        /// Callback that removes the "ResourceDictionary" magic tags (start and end) from BAML
        /// </summary>
        /// <param name="actualData"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        public BamlNodeAction EditBaml2(BamlNodeData actualData, BamlWriterWrapper writer)
        {
            if (actualData.Name == "System.Windows.ResourceDictionary")
            {
                return BamlNodeAction.Skip;
            }
            else
            {
                return BamlNodeAction.Continue;
            }
        }

        #endregion No_IDictionary_Magic_Tag

        #region NonProperty_in_Prop_record
        /// <summary>
        /// We put an event in a property record.
        /// </summary>
        public void NonProperty_in_Prop_record()
        {            
            string xamlName = "NonProperty_in_Prop_record.xaml";

            // Compile the XAML into BAML
            string bamlPath = ParserUtil.CompileXamlToBaml(xamlName);

            // Edit the BAML, using a callback. 
            BamlHelper.EditBaml(bamlPath, bamlPath + ".new", new BamlHelper.BamlNodeCallback(EditBaml3));

            try
            {
                ParserUtil.LoadBamlFile(@"pack://siteoforigin:,,,/" + bamlPath + ".new");
                throw new Microsoft.Test.TestValidationException("LoadBaml() was expected to throw, but didn't");
            }
            catch (XamlParseException) { }
        }

        /// <summary>
        /// Callback that removes the "Background" property and puts a 
        /// "Click" property (which is really an event) in that record.
        /// </summary>
        /// <param name="actualData"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        public BamlNodeAction EditBaml3(BamlNodeData actualData, BamlWriterWrapper writer)
        {
            if (actualData.Name == "System.Windows.Controls.Control.Background")
            {
                actualData.Name = "System.Windows.Controls.Primitives.ButtonBase.Click";
                actualData.LocalName = "Click";
            }
            return BamlNodeAction.Continue;
        }
        
        #endregion NonProperty_in_Prop_record
    }
}
