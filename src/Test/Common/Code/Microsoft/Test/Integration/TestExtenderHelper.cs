// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Integration
{
    /// <summary>
    /// 
    /// </summary>
    public enum TestExtenderFileTypes
    {
        /// <summary>
        /// 
        /// </summary>
        TestExtenderOutput,
        
        /// <summary>
        /// 
        /// </summary>
        TestExtenderGraph,

        /// <summary>
        /// 
        /// </summary>
        None
    }






    /// <summary>
    /// Provide helper functions for TestExtender.
    /// </summary>
	public static class TestExtenderHelper
	{
        /// <summary>
        /// 
        /// </summary>
        static public string SearchDirectoryForTestExtenderAdaptor
        {
            get
            {
                if (_searchDirectoryForTestExtenderAdaptor.Length == 0)
                {
                    throw new InvalidOperationException("SearchDirectoryForTestExtenderAdaptor is used without being initialized.");
                }
                return _searchDirectoryForTestExtenderAdaptor;
            }
            set
            {
                _searchDirectoryForTestExtenderAdaptor = value;
            }
        }

        [ThreadStatic]
        static string _searchDirectoryForTestExtenderAdaptor = "";

        /// <summary>
        /// Takes a relative or absolute path and returns an absolute path.
        /// </summary>
        /// <param name="inputPath">The path to qualify.</param>
        /// <returns>Absolute path equivalent of inputPath.</returns>
        static public string QualifyPath(string inputPath)
        {
            if (Path.IsPathRooted(inputPath))
            {
                return inputPath;
            }
            else
            {
                return Path.Combine(SearchDirectoryForTestExtenderAdaptor, inputPath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void CleanCache(string directoryPath)
        {
            string[] allTXRFiles;
            string[] allTXROFiles;

            directoryPath = Path.GetDirectoryName(directoryPath);

            GetFilesinDirectory(directoryPath, out allTXRFiles, out allTXROFiles);

            List<string> txrFiles = new List<string>(allTXRFiles.Length);
            txrFiles.AddRange(allTXRFiles);

            foreach (string file in allTXROFiles)
            {
                string fileModified = Path.ChangeExtension(file, ".txr");

                if (txrFiles.Contains(fileModified))
                {
                    File.Delete(file);
                }
            }
        }
        
        /// <summary>
        /// Return all the the files that are TestExtenderGraph or TestExterderOutput
        /// for the directory passed as parameter.
        /// </summary>
        /// <param name="directoryPath">Directory path to search.</param>
        /// <returns>All files that are TestExtenderGraph or TestExterderOutput.</returns>
        public static string[] GetTestExtenderFiles(string directoryPath)
        {
            string[] allTXRFiles;
            string[] allTXROFiles;

            GetFilesinDirectory(directoryPath, out allTXRFiles, out allTXROFiles);

            List<string> allFiles = new List<string>(allTXRFiles.Length + allTXROFiles.Length);

            allFiles.AddRange(allTXROFiles);

            foreach (string file in allTXRFiles)
            {
                string fileModified = Path.ChangeExtension(file, ".txro");

                if (!allFiles.Contains(fileModified))
                {
                    allFiles.Add(file);
                }
            }



            // 


            return allFiles.ToArray();

        }

        private static void GetFilesinDirectory(string directoryPath, out string[] allTXRFiles, out string[] allTXROFiles)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new ArgumentException("Argument cannot be null or empty string.", "directoryPath");
            }

            if (!Directory.Exists(directoryPath))
            {
                throw new ArgumentException("The diretory path does not exist.", "directoryPath");
            }

            allTXRFiles = Directory.GetFiles(directoryPath, "*.txr", SearchOption.AllDirectories);
            allTXROFiles = Directory.GetFiles(directoryPath, "*.txro", SearchOption.AllDirectories);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public static string GetAssemblyForGenericLoader
        {
            get
            {
                return "TestRuntimeUntrusted.dll";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string GetClassForGenericLoader
        {
            get
            {
                return "Microsoft.Test.Integration.TXRHelper";
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public static string GetMethodForGenericLoader
        {
            get
            {
                return "TestDriverEntryMethod";
            }
        }


        /// <summary>
        /// Indicates if the file pass as argument is a TestExtender friendly file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static TestExtenderFileTypes GetTypeOfFile(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentException("Argument cannot be null or empty string.", "file");
            }

            if (!File.Exists(file))
            {
                throw new ArgumentException("The file (" + file + ") does not exist.", "file");
            }

            string fileExtension = Path.GetExtension(file);

            if (string.Compare(fileExtension, ".txro", true) == 0)
            {
                return TestExtenderFileTypes.TestExtenderOutput;
            }
            else if (string.Compare(fileExtension, ".txr", true) == 0)
            {
                return TestExtenderFileTypes.TestExtenderGraph;
            }

            return TestExtenderFileTypes.None;
        }

        /// <summary>
        /// Loads a TestExterderGraph or TestExtenderOutput file string.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="createCache"></param>
        /// <returns>
        /// Microsoft.Test.TestExterderOutput object. Null if the file is not a TXRO or TXR.
        /// </returns>
        public static TestExtenderOutput LoadFile(ref string file, bool createCache)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentException("Argument cannot be null or empty string.", "file");
            }

            if (!File.Exists(file))
            {
                throw new ArgumentException("The file does not exist.", "file");
            }

            TestExtenderOutput txro = null;

            try
            {
                TestExtenderFileTypes fileType = GetTypeOfFile(file);

                if (fileType == TestExtenderFileTypes.TestExtenderOutput)
                {
                    txro =  TestExtenderOutput.Load(file);
                }
                else if (fileType == TestExtenderFileTypes.TestExtenderGraph)
                {
                    TestExtenderGraph txr = TestExtenderGraph.Load(file);
                    txro = txr.Generate();
                    // file = WriteCache(file, txro);
                }
            }
            catch (Exception e) 
            {
                GlobalLog.LogStatus(e.ToString());
                txro = null;
            }

            if (txro == null)                            
            {
                GlobalLog.LogStatus("A TestExtender cannot be loaded. There could be missing test cases. File: " + file);                
            }

            return txro;
        }

        private static string WriteCache(string file, TestExtenderOutput txro)
        {
            string path = Path.GetDirectoryName(file);
            string fileName = Path.GetFileNameWithoutExtension(file);
            fileName = fileName + ".txro";
            fileName = Path.Combine(path, fileName);
            txro.Save(fileName);
            file = fileName;
            return file;
        }
	}
}
