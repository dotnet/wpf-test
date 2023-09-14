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
using Avalon.Test.CoreUI.Parser;
using Microsoft.Test.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Markup;

namespace Avalon.Test.CoreUI.Parser.Error
{
    /// <summary>
    /// XamlCompileTestExecutor runs testcases of type XamlCompile, i.e. it compiles
    /// the given Xaml, and then runs the resultant executable. If compilation errors
    /// occur, it returns the first error among them. If not, it goes ahead, runs the executable
    /// and catches runtime errors.
    /// </summary>
    public class XamlCompileTestExecutor : TestExecutor
    {
        /// <summary>
        /// 
        /// </summary>
        public XamlCompileTestExecutor()
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

            // Cleanup old compile directories and files if necessary.
            CompilerHelper compiler = new CompilerHelper();
            compiler.CleanUpCompilation();
            compiler.AddDefaults();

            // Compile xaml into Avalon app.
            List<ErrorWarningCode> buildErrorsAndWarnings = null;
            try
            {
                // Turn on the last param (debugBaml), which puts debugging info (line,position)
                // in Baml. We need this so that the runtime errors have line and position 
                // info.
                compiler.CompileApp(new string[]{_dataFilename}, "Application", null, null, null, Languages.CSharp, null, null, null, true /*debugBaml*/);
            
            }
                //Need to specify which Microsoft.Test.TestSetupException to use or test will fail
            catch (Microsoft.Test.TestSetupException e)
            {                
                // If there are compilation errors, CompileApp() throws an exception,
                // and sets the build errors and warnings list as custom exception data.
                // We retrieve that data here.
                buildErrorsAndWarnings = (List<ErrorWarningCode>)e.Data["buildErrorsAndWarnings"];
            }

            if ((null != buildErrorsAndWarnings) && (buildErrorsAndWarnings.Count > 0))
            {
                // The list contains both errors and warnings. 
                // Get the first error, extract the data from it and return it.
                foreach (ErrorWarningCode errAndWarn in buildErrorsAndWarnings)
                {
                    if (ErrorType.Error == errAndWarn.Type)
                    {
                        return CompilerHelper.ExtractErrData(errAndWarn);
                    }
                }
            }

            // Since we have reached here, it's clear that no compilation errors
            // have occurred. So now run the Avalon app.
            try
            {            
                compiler.RunCompiledApp();
            }
            catch(Microsoft.Test.TestValidationException tve)
            {
                // If a runtime exception occurs, RunCompiledApp() catches it, and sets
                // it as the InnerException of the Microsoft.Test.TestValidationException that it throws.
                // We retrieve it here.
                Exception runtimeException = tve.InnerException;
                return ExtractErrData(runtimeException);
            }

            // Runtime exception expected, but not thrown (evident from the 
            // fact that we haven't returned from the catch block above). Fail the test.
            return ExtractErrData(new Microsoft.Test.TestValidationException(_dataFilename + " didn't throw any exception when run."));

            //This is just to satisfy the compiler. Should not come here.
            
        }

        // Extract error data from an Exception object.
        private Hashtable ExtractErrData(Exception e)
        {
            Hashtable errData = new Hashtable();
            errData["ExceptionType"] = e.GetType().Name;
            errData["ErrorMessage"] = e.Message;
            if (null != e.InnerException)
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
        
        // Name of the Xaml file that is to be compiled and run.
        private string _dataFilename;
    }
}
