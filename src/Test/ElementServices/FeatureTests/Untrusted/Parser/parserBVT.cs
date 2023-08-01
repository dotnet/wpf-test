// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***************************************************************************\
*
*
\***************************************************************************/

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
using System.Windows.Media;
using System.Windows.Markup;
using System.Runtime.InteropServices;

using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.Source;

using Microsoft.Test.Win32;
using Microsoft.Test.Serialization;
using Microsoft.Test.Serialization.CustomElements;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Microsoft.Test;
using Microsoft.Test.Discovery;

namespace Avalon.Test.CoreUI.Parser
{

    /// <summary>
    /// </summary> 
    [TestDefaults]
    public class CultureInfoIetfLanguageTagConverterTest
    {

        /// <summary>
        /// </summary>
        public void SimpleTest()
        {
            CultureInfoIetfLanguageTagConverter c = new CultureInfoIetfLanguageTagConverter();

            if (!c.CanConvertFrom(null,typeof(string)))
            {
                CoreLogger.LogTestResult(false,"CultureInfoIetfLanguageTagConverter.CanConvertFrom must convert from string");
                return;
            }

            if (c.CanConvertFrom(null,typeof(object)))
            {
                CoreLogger.LogTestResult(false,"CultureInfoIetfLanguageTagConverter.CanConvertFrom must convert from object");
                return;
            }
        


            CoreLogger.LogTestResult(true,"");
            
        }


    }


    
    /// <summary>
    /// </summary>
    public class ResourceReferenceKeyNotFoundExceptionTest 
    {

        /// <summary>
        /// </summary>
        [Test(2, @"Resources", TestCaseSecurityLevel.FullTrust, "ResourceReferenceKeyNotFoundException ctor", Area = "XAML")]
        public void SimpleTest()
        {
            CoreLogger.BeginVariation();
            ResourceReferenceKeyNotFoundException e = new ResourceReferenceKeyNotFoundException("o", new object());

            // This FileStream is used for the serialization.
            using (MemoryStream stream = new MemoryStream())
            {

                // Serialize the derived exception.
                BinaryFormatter formatter =
                    new BinaryFormatter(null,
                        new StreamingContext(
                            StreamingContextStates.File));
                formatter.Serialize(stream, e);

                // Rewind the stream and deserialize the 
                // exception.
                stream.Position = 0;
                ResourceReferenceKeyNotFoundException deserExcept =
                    (ResourceReferenceKeyNotFoundException)
                        formatter.Deserialize(stream);

                if (String.Equals(e.Message, deserExcept.Message, StringComparison.InvariantCulture))
                {
                    CoreLogger.LogTestResult(true,"");
                    return;
                }
            }

            CoreLogger.LogTestResult(false,"");
            CoreLogger.EndVariation();
        }
    }


    /// <summary>
    /// </summary>
    public class ServiceProvidersTest 
    {


        /// <summary>
        /// </summary>
        [Test(2, @"Parser", TestCaseSecurityLevel.PartialTrust, "ServiceProvider Add and Get", Area = "XAML")]
        public void TestEquality()
        {
            CoreLogger.BeginVariation();
            ServiceProviders sp = new ServiceProviders();

            if (sp.GetService(typeof(object)) != null)
            {
                CoreLogger.LogTestResult(false,"GetService should return null.");
                return;
            }
            

            object o = new object();

            sp.AddService(typeof(object), o);

            if (sp.GetService(typeof(object)) != o)
            {
                CoreLogger.LogTestResult(false,"GetService should return a expected object.");
                return;
            }
            
            object o1 = new object();            
            bool expectionThrown = false;
            try
            {                
                sp.AddService(typeof(object), o1);
            }
            catch (ArgumentException)
            {
                expectionThrown = true;
            }

            if (!expectionThrown)
            {
                CoreLogger.LogTestResult(false,"AddService should throw an exception.");
                return;                
            }            


            sp.AddService(typeof(ServiceProvidersTest), this);

            if (sp.GetService(typeof(ServiceProvidersTest)) != this || sp.GetService(typeof(object)) != o)
            {
                CoreLogger.LogTestResult(false,"GetService should return the expected objects");
                return;
            }
            
            CoreLogger.LogTestResult(true,"");
            CoreLogger.EndVariation();
        }


    }    
}
