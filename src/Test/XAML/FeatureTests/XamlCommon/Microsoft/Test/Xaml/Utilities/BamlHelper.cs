// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.Loader;
using System.Text;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Utilities
{
    /// <summary>
    ///  Utility to grab a baml resource from an assembly
    /// </summary>
    public class BamlHelper : MarshalByRefObject
    {
        /// <summary>
        ///  Grab a baml resource from an assembly, use a separate appdomain so 
        ///  that the loaded assembly can be unloaded
        /// </summary>
        /// <param name="assemblyPath">path to the assembly</param>
        /// <param name="bamlFilePath">path where the baml should be saved to</param>
        public static void ExtractBamlResource(string assemblyPath, string bamlFilePath)
        {
            // .NET Core 3.0 : AppDomain isolation no longer exists, calling the method directly, should verify if it still works as intended.
            // string appDomainBaseDir = AppDomain.CurrentDomain.BaseDirectory;
            // AppDomainSetup appSetup = AppDomain.CurrentDomain.SetupInformation;
            // appSetup.ApplicationBase = appDomainBaseDir;
            // appSetup.PrivateBinPath = appDomainBaseDir;
            // appSetup.PrivateBinPathProbe = appDomainBaseDir;

            // AppDomain appDomain = AppDomain.CreateDomain("CustomDomain", null, appSetup);

            // var bamlHelper = (BamlHelper)appDomain.CreateInstanceAndUnwrap(
                 // typeof(BamlHelper).Assembly.FullName,
                 // typeof(BamlHelper).FullName);
                 
            BamlHelper bamlHelper = new BamlHelper();

            bamlHelper.ExtractBamlResourceInternal(assemblyPath, bamlFilePath);

            //AppDomain.Unload(appDomain);
        }

        /// <summary>
        /// Get the list of assembly references in the baml file
        /// </summary>
        /// <param name="bamlFilePath">file path of baml file</param>
        /// <returns>list of referenced assemblies</returns>
        public static List<string> GetReferencesInBaml(string bamlFilePath)
        {
            // Quick and dirty way to get the references in the baml. The referenced 
            // assemblies are embedded in the baml as strings. We search for the 'Version' string
            // expand on it to get the full version and extract it. The other option is to take
            // a dependency on BamlDasm which has multiple dependencies that we do not want to 
            // take in test. 
            string text = String.Empty;
            using (BinaryReader reader = new BinaryReader(File.Open(bamlFilePath, FileMode.Open)))
            {
                byte[] bytes = reader.ReadBytes((int)reader.BaseStream.Length);
                text = Encoding.UTF8.GetString(bytes);
            }

            List<string> references = new List<string>();

            bool done = false;
            int startIndex = 0;
            while (!done)
            {
                int index = text.IndexOf("Version", startIndex);
                if (index > 0)
                {
                    int start = GetStartOfString(text, index);
                    int end = GetEndOfString(text, index);
                    references.Add(text.Substring(start + 1, end - start));
                    startIndex = end;
                    if (startIndex >= text.Length)
                    {
                        done = true;
                    }
                }
                else
                {
                    done = true;
                }
            }

            GlobalLog.LogDebug(String.Format(CultureInfo.InvariantCulture, "Referenced assemblies in {0} are ...", bamlFilePath));
            foreach (string reference in references)
            {
                GlobalLog.LogDebug(reference);
            }

            return references;
        }

        /// <summary>
        ///  Grab a baml resource from an assembly
        /// </summary>
        /// <param name="assemblyPath">path to the assembly</param>
        /// <param name="bamlFilePath">path where the baml should be saved to</param>
        internal void ExtractBamlResourceInternal(string assemblyPath, string bamlFilePath)
        {
            if (String.IsNullOrEmpty(assemblyPath))
            {
                throw new ArgumentException("assemblyPath");
            }

            Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);

            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                if (!StringEndsWith(resourceName, ".RESOURCES"))
                {
                    continue;
                }

                Stream resourceStream = assembly.GetManifestResourceStream(resourceName);
                using (ResourceReader reader = new ResourceReader(resourceStream))
                {
                    foreach (DictionaryEntry entry in reader)
                    {
                        string name = entry.Key as string;

                        if (StringEndsWith(name, ".BAML"))
                        {
                            Stream source = entry.Value as Stream;
                            using (Stream target = File.Open(bamlFilePath, FileMode.Create))
                            {
                                byte[] bamlBytes = new byte[source.Length];
#pragma warning disable CA2022 // Avoid inexact read
                                source.Read(bamlBytes, 0, bamlBytes.Length);
#pragma warning restore CA2022
                                target.Write(bamlBytes, 0, bamlBytes.Length);
                            }

                            return;
                        }
                    }
                }
            }

            throw new Exception("No baml resources in " + assemblyPath);
        }

        /// <summary>
        /// check if a string ends with a given suffix
        /// </summary>
        /// <param name="stringToCompare">string to compare</param>
        /// <param name="suffixString">suffix to look for</param>
        /// <returns>true if string ends with suffix</returns>
        private static bool StringEndsWith(string stringToCompare, string suffixString)
        {
            return stringToCompare.EndsWith(suffixString, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///  Get the start index of the string
        /// </summary>
        /// <param name="text">string to look in</param>
        /// <param name="index">index somwhere in the middle of the string</param>
        /// <returns>index of the begining of string</returns>
        private static int GetStartOfString(string text, int index)
        {
            int start = index;
            for (int i = index; i > 0; i--)
            {
                if (!IsAscii(text[i]))
                {
                    start = i + 1;
                    break;
                }
            }

            return start;
        }

        /// <summary>
        /// Get the end index of the string
        /// </summary>
        /// <param name="text">string to look in</param>
        /// <param name="index">index somewhere in the middle of the string</param>
        /// <returns>index of the end of the string</returns>
        private static int GetEndOfString(string text, int index)
        {
            int end = index;
            for (int i = index; i < text.Length; i++)
            {
                if (!IsAscii(text[i]))
                {
                    end = i - 1;
                    break;
                }
            }

            return end;
        }

        /// <summary>
        /// is the given character an ascii character 
        /// ascii characters between A and z
        /// </summary>
        /// <param name="chr">character to test</param>
        /// <returns>true if ascii</returns>
        private static bool IsAscii(char chr)
        {
            int code = (int)chr;
            if (code < 32 || code > 127)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
