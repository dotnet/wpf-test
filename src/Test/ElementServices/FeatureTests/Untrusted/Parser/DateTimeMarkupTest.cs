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

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;
using System.Reflection;
using System.Collections;
using System.Xml;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;
using System.Globalization;
using System.Runtime.InteropServices;

using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.Source;

using Microsoft.Test.Win32;
using Microsoft.Test.Serialization;
using Microsoft.Test;
using Microsoft.Test.Discovery;

namespace Avalon.Test.CoreUI.Parser
{
    /// <summary>
    /// DateTime markup tests.
    /// </summary>
    /// <remarks>
    /// Tests for parsing/serialization of DateTime strings/objects.
    /// </remarks>
    [TestDefaults]
    public class DateTimeMarkupTest
    {
        /// <summary>
        /// Initializes test objects for both functional tests and error tests.
        /// </summary>
        public DateTimeMarkupTest()
        {
            // Calculate the suffix used for local times *without* Daylight Savings (Jan 1st). 
            // This "localSuffixNoDS" is used below to determine expected values.
            string formatString = "yyyy-MM-dd'T'HH':'mmzzz";
            string now = new DateTime(2006, 1, 1).ToString(formatString, CultureInfo.GetCultureInfo("en-US"));
            string localSuffixNoDS = now.Substring(now.Length - 6 /*e.g. -08:00 or +01:00*/);

            // Now calculate the suffix for local times *with* Daylight Savings (July 1st).
            now = new DateTime(2006, 7, 1).ToString(formatString, CultureInfo.GetCultureInfo("en-US"));
            string localSuffixWithDS = now.Substring(now.Length - 6 /*e.g. -08:00 or +01:00*/);

            // Create the functional test objects.            
            _functionalTestObjects = new FunctionalTestObject[] { 
                // DateTime.MaxValue in UTC
                new FunctionalTestObject(DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc),
                "   12/31/9999 23:59:59.9999999Z    ",
                "9999-12-31T23:59:59.9999999Z"),

                // DateTime.MaxValue in Local
                new FunctionalTestObject(DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Local),
                "   12-31-9999 23:59:59.9999999     " + localSuffixNoDS,
                "9999-12-31T23:59:59.9999999" + localSuffixNoDS),

                // DateTime.MinValue in Local
                /* 









*/ 

                // DateTime object that has just a date in UTC
                new FunctionalTestObject(new DateTime(2006, 2, 18, 0, 0, 0, DateTimeKind.Utc),
                "   2/18/2006Z  ",
                "2006-02-18T00:00Z"),

                // DateTime object that has just a date in Unspecified
                new FunctionalTestObject(new DateTime(2006, 5, 29, 0, 0, 0, DateTimeKind.Unspecified ),
                "   2006-05-29  ",
                "2006-05-29"),

                // DateTime objects that have just a date in Local
                new FunctionalTestObject(new DateTime(2006, 2, 18, 0, 0, 0, DateTimeKind.Local),
                null,
                "2006-02-18T00:00" + localSuffixNoDS),

                new FunctionalTestObject(new DateTime(2006, 5, 29, 0, 0, 0, DateTimeKind.Local),
                null,
                "2006-05-29T00:00" + localSuffixWithDS),

                // DateTime object that has just a time in UTC
                new FunctionalTestObject(new DateTime(1, 1, 1, 17, 30, 0, DateTimeKind.Utc),
                "   17:30Z  ",
                "0001-01-01T17:30Z"),

                // DateTime object that has just a time in Unspecified
                new FunctionalTestObject(new DateTime(1, 1, 1, 17, 30, 0, DateTimeKind.Unspecified),
                "   5:30pm  ",
                "0001-01-01T17:30"),

                // DateTime object that has just a time in Local
                /* 



*/

                // DateTime object that has both date and time in UTC
                new FunctionalTestObject(new DateTime(2006, 5, 29, 17, 30, 0, DateTimeKind.Utc),
                "   5 29 2006 17:30Z    ",
                "2006-05-29T17:30Z"),

                // DateTime object that has both date and time in Unspecified
                new FunctionalTestObject(new DateTime(2006, 2, 18, 17, 30, 18, 265, DateTimeKind.Unspecified),
                "   2006 2 18 5:30:18.265 pm    ",
                "2006-02-18T17:30:18.265"),

                // DateTime objects that have both date and time in Local
                new FunctionalTestObject(new DateTime(2006, 2, 18, 17, 30, 18, DateTimeKind.Local),
                "   02/18/2006 17:30:18  " + localSuffixNoDS,
                "2006-02-18T17:30:18" + localSuffixNoDS),

                new FunctionalTestObject(new DateTime(2006, 5, 29, 18, 30, 18, DateTimeKind.Local),
                "   05-29-2006 17:30:18  " + localSuffixNoDS,
                "2006-05-29T18:30:18" + localSuffixWithDS)
            };

            // Create the error test objects. 
            _errorTestObjects = new ErrorTestObject[] {
                new ErrorTestObject("12006-05-29T13:06:18.2656250Z"),
                new ErrorTestObject("2006 13 29 13:06:18.2656250Z"),
                new ErrorTestObject("2006 5 32 13:06:18.2656250Z"),
                new ErrorTestObject("2006-05-029T13:06:18.2656250Z"),
                new ErrorTestObject("5/29/2006 5.30pmZ"),
                new ErrorTestObject("5/29/2006 5.30pm" + localSuffixNoDS),
            };
        }

        /// <summary>
        /// Goes through the functional test objects and runs a test
        /// on each of them.
        /// </summary>
        public void RunFunctionalTests()
        {
            foreach( FunctionalTestObject testObject in _functionalTestObjects )
            {
                RunSingleFunctionalTest( testObject );
            }
        }

        /// <summary>
        /// Goes through the error test objects and runs a test
        /// on each of them.
        /// </summary>
        public void RunErrorTests()
        {
            foreach (ErrorTestObject testObject in _errorTestObjects)
            {
                RunSingleErrorTest(testObject);
            }
        }
        
        /// <summary>
        /// This method runs a functional test scenario.  
        /// The test scenario is described in the FunctionalTestObject.
        /// That object has the DateTime we want to test, as well 
        /// as the expected SerializedOuput. It may also have StringInput, 
        /// in which case we'll try to parse it first.        
        /// </summary>
        [Test(0, @"Parser\DateTime", "RunSingleFunctionalTest", Area = "XAML")]
        private void RunSingleFunctionalTest(FunctionalTestObject testObject)
        {
            CoreLogger.BeginVariation();
            Custom_DO_With_DateTimeProperty xamlRoot;

            // If this scenario has StringInput, we'll parse it first.
            if( testObject.StringInput != null )
            {
                // Create markup from the date/time string.
                string inputString = _xamlHeader + testObject.StringInput + _xamlMiddle + testObject.StringInput + _xamlFooter;

                // Load the markup.
                xamlRoot = XamlReader.Load(new XmlTextReader(new StringReader(inputString))) as Custom_DO_With_DateTimeProperty;

                // Validate the DateTime value.
                if(testObject.DateTime != xamlRoot.DateTimeProperty)
                {
                    throw new Microsoft.Test.TestValidationException("Non-attached DateTime property value doesn't match for " + testObject.StringInput );
                }
                if(testObject.DateTime != Custom_DP_Attacher.GetAttachedDateTimeProperty(xamlRoot)) 
                {
                    throw new Microsoft.Test.TestValidationException("Attached DateTime property value doesn't match for " + testObject.StringInput);
                }
            }
            else
            {            
                // Create a DateTimeControl with its DateTime set to the scenario input.
                xamlRoot = new Custom_DO_With_DateTimeProperty();
                xamlRoot.DateTimeProperty = testObject.DateTime;
                Custom_DP_Attacher.SetAttachedDateTimeProperty(xamlRoot, testObject.DateTime);
            }

            // Serialize this DateTimeControl
            string outputString = SerializationHelper.SerializeObjectTree(xamlRoot);

            // Calculate what we expect the serialized output to be
            string expectedString = _xamlHeader + testObject.SerializedOutput 
                                   + _xamlMiddle + testObject.SerializedOutput
                                   + _xamlFooter;

            // Validate the ouput
            if(outputString != expectedString)
            {
                throw new Microsoft.Test.TestValidationException("Serialization of DateTime Failed, \n" +
                        "expected '" + expectedString + "'" +
                        "got '" + outputString + "'" );
            }
            CoreLogger.EndVariation();
        }

        /// <summary>
        /// This method runs a error test scenario.  
        /// The test scenario is described in the ErrorTestObject.
        /// That object has a DateTime string (StringInput), which we  
        /// try to parse, and verify that the expected error is thrown.
        /// </summary>
        private void RunSingleErrorTest(ErrorTestObject testObject)
        {
            DateTimeValueSerializer s = new DateTimeValueSerializer();
            
            if (!s.CanConvertFromString(String.Empty,null))
            {
                throw new Microsoft.Test.TestValidationException("CanConvertFromString should always return null.");
            }

            
            // Create markup from the date/time string.
            string inputString = _xamlHeader + testObject.StringInput + _xamlMiddle + testObject.StringInput + _xamlFooter;

            // Load the markup.
            try
            {
                Custom_DO_With_DateTimeProperty xamlRoot = XamlReader.Load(new XmlTextReader(new StringReader(inputString))) as Custom_DO_With_DateTimeProperty;
            }
            catch (XamlParseException xpe)
            {
                if (!(xpe.InnerException is FormatException))
                {
                    throw new Microsoft.Test.TestValidationException("XamlParseException was caught, but its InnerException is not a FormatException, as expected");
                }

                if (!(xpe.Message.Contains("System.DateTime")))
                {
                    throw new Microsoft.Test.TestValidationException("XamlParseException's message was expected to contain the string 'System.DateTime', but it doesn't");
                }
            }
        }

        // Raw markup            
        string _xamlHeader = "<Custom_DO_With_DateTimeProperty DateTimeProperty=\""; 
        string _xamlMiddle = "\" Custom_DP_Attacher.AttachedDateTimeProperty=\"";
        string _xamlFooter = "\" xmlns=\"clr-namespace:Avalon.Test.CoreUI.Parser;assembly=CoreTestsUntrusted\" />";

        private FunctionalTestObject[] _functionalTestObjects;
        private ErrorTestObject[] _errorTestObjects;


        #region class FunctionalTestObject
        /// <summary>
        /// This class is used to describe a functional case for the DateTime markup tests.
        /// </summary>
        public class FunctionalTestObject
        {
            /// <summary>
            /// C'tor
            /// </summary>
            public FunctionalTestObject(DateTime dateTime, string stringInput, string serializedOutput)
            {
                DateTime = dateTime;
                StringInput = stringInput;
                SerializedOutput = serializedOutput;
            }

            /// <summary>
            /// 
            /// </summary>
            public DateTime DateTime;
            /// <summary>
            /// 
            /// </summary>
            public string StringInput;
            /// <summary>
            /// 
            /// </summary>
            public string SerializedOutput;
        }
        #endregion class FunctionalTestObject

        #region class ErrorTestObject
        /// <summary>
        /// This class is used to describe an error scenario 
        /// for the DateTime markup tests. The StringInput property represents
        /// a string that cannot be parsed by the DateTime converter.
        /// </summary>
        public class ErrorTestObject
        {
            /// <summary>
            /// C'tor
            /// </summary>
            public ErrorTestObject(string stringInput)
            {
                StringInput = stringInput;
            }

            /// <summary>
            /// 
            /// </summary>
            public string StringInput;
        }
        #endregion class ErrorTestObject
    }
}
