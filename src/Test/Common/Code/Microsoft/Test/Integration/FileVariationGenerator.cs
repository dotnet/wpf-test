// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Microsoft.Test.Integration
{
    /// <summary>
    /// Finds all the xamls in specific directory and return them as VariationItems
    /// </summary>
	public class FileVariationGenerator : BaseVariationGenerator, IVariationGenerator
	{
        /// <summary>
        /// 
        /// </summary>
        public FileVariationGenerator()
        {
            // Setting the default Test Contract
            
            DefaultTestContract.Description = "Generate VariationItems for each file found in a directory specified in the DirectoryPath property.";

            StorageItem si = new StorageItem();
            si.Name = "FileName";
            TypeDesc typeDesc = new TypeDesc(typeof(string));
            si.Type = typeDesc;
            DefaultTestContract.Output.Add(si);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override List<VariationItem> Generate()
        {
            if (!Directory.Exists(TestExtenderHelper.QualifyPath(DirectoryPath)))
            {
                throw new InvalidOperationException("The DirectoryPath doesn't exist. " + DirectoryPath);
            }

            List<VariationItem> viList = new List<VariationItem>();

            string fileExtSearch = "*." + FileExtension;

            string[] files = Directory.GetFiles(TestExtenderHelper.QualifyPath(DirectoryPath),
                                                  fileExtSearch,
                                                  SearchOption.AllDirectories);

            foreach (string file in files)
            {
                FileVariationItem vi = new FileVariationItem();
                vi.FileName = Path.GetFileName(file);
                vi.Title = file;
                viList.Add(vi);
                
                vi.SupportFiles.Add(Path.Combine(DirectoryPath, Path.GetFileName(file)));                
            }

            return viList;
        }


        /// <summary>
        /// File extension to search.  Don't include "."
        /// </summary>
        public string FileExtension
        {
            get { return _fileExtension; }
            set { _fileExtension = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DirectoryPath
        {
            get { return _directoryPath; }
            set { _directoryPath = value; }
        }


        private string _directoryPath = "";
        private string _fileExtension = "*";
    }
}
