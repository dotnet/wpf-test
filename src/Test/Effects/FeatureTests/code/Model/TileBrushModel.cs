// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test Effects with TileBrush with different TileMode.
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.Windows;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Discovery;
using System.Xml;
using System.Collections.Generic;
using Microsoft.Test.Markup;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Pict.MatrixOutput;
using System.IO;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Model base test for TileMode integration test. 
    /// </summary>
    public class TileBrushModel : EffectsModel
    {
        #region Private Data

        private const string xamlTemplateName = "XamlTemplateForTileMode.xaml";
        
        #endregion


        #region Methods

        /// <summary>
        /// Build xaml file based on parameters. 
        /// </summary>
        /// <param name="parameters">parameters</param>
        /// <returns>file name of the xaml generated.</returns>
        protected override string CreateXaml(PropertyBag parameters)
        {
            string xamlFileName = ModelFileName + (VariationIndex) + ".xaml";
            XmlDocument document = new XmlDocument();

            document.Load(xamlTemplateName);

            XmlElement rectangleElement = document.GetElementsByTagName("Rectangle")[0] as XmlElement;

            //set effect
            rectangleElement.SetAttribute("Effect", "{StaticResource " + parameters["Effect"] +"}");

            //set Fill 
            XmlElement brushNode = AddProperty(document, rectangleElement, "Fill", parameters);
            brushNode.SetAttribute("TileMode", parameters["TileMode"]);
            string viewportSize = parameters["ViewportSize"];
            brushNode.SetAttribute("Viewport", string.Format("0,0,{0},{0}", viewportSize));

            //clean helper segments
            XmlNode helpers = document.GetElementsByTagName("HelperXamlSegments")[0];
            helpers.ParentNode.RemoveChild(helpers);

            document.Save(xamlFileName);
            return xamlFileName;
        }

        #endregion
    }
}