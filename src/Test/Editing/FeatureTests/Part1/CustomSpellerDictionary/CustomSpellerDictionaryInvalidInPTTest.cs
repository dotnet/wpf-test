// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Verifies that invalid uri types do not succeed in partial trust and that
//  failure information does not reveal sensitive information

using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Security;

using Microsoft.Test.Discovery;
using Test.Uis.Data;
using Test.Uis.TestTypes;
using Test.Uis.Loggers;

namespace Microsoft.Test.Editing
{
    /// <summary>
    /// This test verifies that certain custom speller functionality fails as expected when run in partial trust.
    /// </summary>
    [Test(2, "Speller", "CustomSpellerDictionaryInvalidInPTTest", MethodParameters = "/TestCaseType:CustomSpellerDictionaryInvalidInPTTest /XbapName=EditingTestDeployPart1", SupportFiles = @"FeatureTests\Editing\*.lex, FeatureTests\Editing\EditingTestDeployPart1*", Timeout = 120)]   
    public class CustomSpellerDictionaryInvalidInPTTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            SetTestValues();

            _textBoxBase = (TextBoxBase)_testControlType.CreateInstance();
            _textBoxBase.SpellCheck.IsEnabled = true;
           
            QueueDelegate(DoDictionaryLoad);
        }

        private void DoDictionaryLoad()
        {
            string uriFileName = string.Empty;
            string exceptionMessage = string.Empty;
            string exceptionStackTrace = string.Empty;            

            foreach (Uri uri in _customDictionaryUris)
            {
               Log(string.Format("Testing Uri: {0}", uri.ToString()));
               if (uri.IsAbsoluteUri)
               {
                   Log(string.Format("File name: {0}", uri.Segments[uri.Segments.Length - 1]));
                   uriFileName = uri.Segments[uri.Segments.Length - 1];
               }
               else
               {
                   Log(string.Format("File name: {0}", uri.ToString()));
                   uriFileName = uri.ToString();
               }
            }

            Type actualExceptionType = null;
         
            try
            {
                foreach (Uri uri in _customDictionaryUris)
                {
                    _textBoxBase.SpellCheck.CustomDictionaries.Add(uri);
                }                
                throw new ApplicationException("Expected an exception at this point.  Test fails!");
            }
            catch (SecurityException e)
            {
                Log("Got a Security Exception");               
                actualExceptionType = e.GetType();
                exceptionMessage = e.Message;
                exceptionStackTrace = e.StackTrace;
            }
            catch (System.NotSupportedException e)
            {
                Log("Got a NotSupported Exception");
                actualExceptionType = e.GetType();
                exceptionMessage = e.Message;
                exceptionStackTrace = e.StackTrace;
            }

            Verifier.Verify(actualExceptionType.Equals(_expectedExceptionType), "Verifying that we got the expected Exception Type", true);
            Verifier.Verify(!exceptionMessage.Contains(uriFileName), "Verifying exception message is not providing information about the failed uri load.", true);
            Verifier.Verify(!exceptionStackTrace.Contains(uriFileName), "Verifying exception stack trace is not providing information about the failed uri load.", true);

            NextCombination();
        }

        private void SetTestValues()
        {
            switch (_testCustomDictionaryUriType.ToLower())
            {                
                case "localfile":
                    _expectedExceptionType = typeof(SecurityException);               
                    _customDictionaryUris = new Uri[] { new Uri("CustomSpellerDictionary1.lex", UriKind.Relative) };
                    break;
                case "localfile_invalid":
                    _expectedExceptionType = typeof(SecurityException);
                    _customDictionaryUris = new Uri[] { new Uri(@"c:\somedir\doesnotexist.lex", UriKind.RelativeOrAbsolute) };
                    break;
                case "uncfile":
                    _expectedExceptionType = typeof(SecurityException);
                    _customDictionaryUris = new Uri[] { new Uri(@"\\wpf\testscratch\Editing\TestCaseData\CustomSpellerDictionaryUNC.lex", UriKind.Absolute) };
                    break;
                case "uncfile_invalid":
                    _expectedExceptionType = typeof(SecurityException);
                    _customDictionaryUris = new Uri[] { new Uri(@"\\wpf\testscratch\Editing\TestCaseData\doesnotexist.lex", UriKind.Absolute) };
                    break;
                case "siteoforgin":
                    _expectedExceptionType = typeof(NotSupportedException);
                    _customDictionaryUris = new Uri[] { new Uri("pack://siteoforgin:,,,/CustomSpellerDictionary1.lex") };
                    break;
                case "siteoforgin_invalid":
                    _expectedExceptionType = typeof(NotSupportedException);
                    _customDictionaryUris = new Uri[] { new Uri("pack://siteoforgin:,,,/doesnotexist.lex") };
                    break;
                default:
                    throw new ApplicationException("Invalid value for testCustomDictionaryUriType [" + _testCustomDictionaryUriType + "]");
            }
        }                                

        #endregion

        #region Private fields

        // Combinatorial engine variables; set to default values                
        private TextEditableType _testControlType = null;       
        private string _testCustomDictionaryUriType = string.Empty;       
      
        private TextBoxBase _textBoxBase;       
        private Uri[] _customDictionaryUris;
        private Type _expectedExceptionType = null;
        
        #endregion
    }
}
