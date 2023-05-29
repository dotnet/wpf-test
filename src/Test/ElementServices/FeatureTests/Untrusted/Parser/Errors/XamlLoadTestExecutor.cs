// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Please see comments on class declaration below.
 *          
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/

using Avalon.Test.CoreUI.Trusted;
using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Test.Serialization;
using System.IO.Packaging;

namespace Avalon.Test.CoreUI.Parser.Error
{
    /// <summary>
    /// XamlLoadTestExecutor runs testcases of type "XamlLoad", which means it uses
    /// XamlReader.Load() to load the given Xaml file, and if there is an error, returns 
    /// the error data.
    /// If LoadXml() doesn't throw an error, it tries to display the tree that's created 
    /// by LoadXml()
    /// </summary>
    public class XamlLoadTestExecutor : TestExecutor
    {
        /// <summary>
        /// 
        /// </summary>
        public XamlLoadTestExecutor()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expectedErrData"></param>
        /// <returns></returns>
        override public Hashtable Run(Hashtable expectedErrData)
        {
            // Retrieve the "XamlFileName" entry from expectedErrData
            _dataFilename = expectedErrData["XamlFileName"] as string;

            Hashtable actualErrData = null;

            // Open the Xaml file.
            Stream xamlFileStream = File.OpenRead(_dataFilename);
            object root = null;
            bool loadXmlFailed = false;
            try
            {
                // Attempt to load the Xaml file.
                ParserContext pc = new ParserContext();
                pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                root = System.Windows.Markup.XamlReader.Load(xamlFileStream, pc);
            }
            catch (Exception e)
            {
                loadXmlFailed = true;
                actualErrData = ExtractErrData(e);
            }
            finally
            {
                xamlFileStream.Close();
            }
            if (loadXmlFailed) return actualErrData;

            try
            {
                // Attempt to display the tree if we made it
                // this far.
                if (root is UIElement)
                {
                    SerializationHelper helper = new SerializationHelper();
                    helper.DisplayTree((UIElement)root, "CoreUI Error Verification Test");
                }

                // Exception expected, but not thrown. Fail the test.
                throw new Microsoft.Test.TestValidationException(_dataFilename + " didn't throw any exception.");
            }
            catch (Exception e)
            {
                return ExtractErrData(e);
            }

            // This is just to satisfy the compiler. Should not come here.
//            return null;
        }

        // Extract error data from an Exception object.
        private Hashtable ExtractErrData(Exception e)
        {
            Hashtable errData = new Hashtable();
            errData["ExceptionType"] = e.GetType().Name;
            errData["ErrorMessage"] = e.Message;
            if(null != e.InnerException)
            {
                errData["InnerException"] = e.InnerException.Message;
            }
            if (e is XamlParseException)
            {                    
                XamlParseException parseEx = (XamlParseException)e;               
                errData["Line"] = parseEx.LineNumber.ToString();
                errData["Position"] = parseEx.LinePosition.ToString();
            }
            return errData;
        }

        // Name of the Xaml file that's to be loaded.
        private string _dataFilename;
    }
}
