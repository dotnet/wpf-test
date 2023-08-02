// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Helper to override cases in stateless MDE models
 *  
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
//using System.IO;
using System.Collections;
//using System.Runtime.Serialization.Formatters.Soap;
//using System.Windows.Threading;
//using System.Windows.Markup;
//using Microsoft.Test.Modeling;
//using Microsoft.Test.Serialization;
//using Microsoft.Test.Threading;

using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Microsoft.Test.Logging;

using System.Xml;

namespace Avalon.Test.CoreUI.Common
{
    internal class ModelFailureOverride
    {
        /// <summary>
        /// ModelFailureOverride parses an xml file to determine if test verification for
        /// a given model state should fail. Each override also contains the name of the 
        /// exception expected and a reason for the failure. 
        /// </summary>
        /// <remarks>
        /// This only determines if test case should fail given the input model, it doesn't
        /// do any special failure handling or error recovery.
        /// </remarks>
        /// <param name="state">Model state</param>
        /// <param name="failureOverrideXmlFile">Xml file defining failure overrides.</param>
        public ModelFailureOverride(CoreState state, string failureOverrideXmlFile)
        {
            _state = state;

            // Load model failure override xml document.
            _overrideDoc = new XmlDocument();
            _overrideDoc.Load(failureOverrideXmlFile);

            // Retrieve each failure override definition.
            XmlDocument doc = _overrideDoc;
            XmlElement parentElement = doc.DocumentElement;
            XmlNodeList overrideElements = parentElement.SelectNodes("Override");

            // Compare each override definition to the current state.
            bool matchedOverride = false;
            foreach (XmlNode oe in overrideElements)
            {                
                if (CompareOverride((XmlElement)oe))
                {
                    matchedOverride = true;

                    CoreLogger.LogStatus("Found matching override.");

                    // Retrieve reason for matched override.
                    if (oe["Reason"] != null)
                    {
                        _reason = oe["Reason"].InnerText;
                    }

                    break;
                }
            }

            // If an override matches the state verification should fail.
            if (matchedOverride)
            {
                _shouldFail = true;
                CoreLogger.LogStatus("Verification should fail", ConsoleColor.Yellow);
            }
            else
            {
                CoreLogger.LogStatus("Verification should pass", ConsoleColor.Yellow);
            }
        }

        /// <summary>
        /// Compare override combination to model state, return true if override includes state.
        /// </summary>
        private bool CompareOverride(XmlElement overrideElement)
        {
            // CoreLogger.LogStatus(" Comparing override");
            XmlElement combo = overrideElement["ModelState"];
            if (combo == null)
            {
                // todo: Change model configuration to state something?
                throw new Exception("Could not retrieve element ModelCombination from Override.");
            }

            // Make sure each parameter in the override matches the current state.
            XmlNodeList overrideParams = combo.SelectNodes("Param");
            if (overrideParams.Count == 0)
            {
                throw new Exception("Could not retrieve any params for override.");
            }


            bool paramsMatch = true;
            foreach (XmlNode n in overrideParams)
            {
                if (!CheckParam((XmlElement)n))
                    return false; // Parameter does not match state.
            }

            return paramsMatch;
        }

        /// <summary>
        /// Check if the state parameter value matches the override param's values
        /// </summary>
        private bool CheckParam(XmlElement overrideParam)
        {
            // Retreive value from state.
            string stateValue = (string)_state.Dictionary[overrideParam.Attributes["Name"].Value];
            
            // Param may contain a single text value or a number of <Value/> elements.
            XmlNodeList valueNodes = overrideParam.SelectNodes("Value");
            if (valueNodes.Count > 0)
            {
                // Compare each <Value/> node
                foreach(XmlNode overrideValue in valueNodes)
                {
                    CoreLogger.LogStatus("   Value " + overrideValue.InnerText);
                    if (stateValue == overrideValue.InnerText)
                    {
                        // This value matches.
                        return true;
                    }
                }
            }
            else if (stateValue == overrideParam.InnerText)
            {
                // todo: Test this exception.
                if (overrideParam.InnerText == String.Empty)
                {
                    throw new Microsoft.Test.TestValidationException("Override parameter does not contain Value nodes or a string value.");
                }

                // Only a text element under param, compare that with state value.
                return true;
            }

            //
            // No matching value found, this override does not apply to the current state.
            //

            return false;
        }

        
        public bool ShouldFail
        {
            get
            {
                return _shouldFail;
            }
        }

        public string Reason
        {
            get
            {
                return _reason;
            }
        }

        private bool _shouldFail = false;
        private string _reason = "Failure reason not defined.";
        private CoreState _state;
        XmlDocument _overrideDoc;
    }


    // todo: Make another class called ModelStateChooser?

  }

