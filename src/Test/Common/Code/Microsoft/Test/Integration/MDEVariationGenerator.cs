// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Microsoft.Test.Integration
{
    /// <summary>
    /// 
    /// </summary>
    public enum MDELoadTypes
    {
        /// <summary>
        /// 
        /// </summary>
        XTC
    }

    /// <summary>
    /// 
    /// </summary>
    public class MDEVariationGenerator : BaseVariationGenerator, IVariationGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        public MDEVariationGenerator() : this (MDELoadTypes.XTC)
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        public MDEVariationGenerator(MDELoadTypes loadType) 
        {
            LoadType = loadType;
        }


        /// <summary>
        /// 
        /// </summary>
        public MDELoadTypes LoadType
        {
            get
            {
                return _loadType;
            }
            set
            {
                _loadType = value;
                InvalidateCache();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FilePath
        {
            set
            {
                string val = value; 
                if (!File.Exists(val))
                {
                    throw new ArgumentException(val + " cannot be found.");
                }

                _filePath = value;
                InvalidateCache();
            }
            get
            {
                return _filePath;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public TypeDesc ModelType
        {
            get
            {
                return _modelTypeDesc;
            }
            set
            {
                _modelTypeDesc = value;
                InvalidateCache();
            }
        }




        #region IVariationGenerator Members

        ///<summary>
        ///</summary>
        public override List<VariationItem> Generate()
        {            
            if (_generatedCache == null)
            {
               _generatedCache = GenerateFromXTC();
            }

            return _generatedCache;
        }

        
        #endregion


        List<VariationItem> _generatedCache = null;

        ///<summary>
        ///</summary>
        protected override void InvalidateCache()
        {
            base.InvalidateCache();
            _generatedCache = null;

        }

        private List<VariationItem> GenerateFromXTC()
        {
            XmlDocument xmlDoc = new XmlDocument();
            List<VariationItem> variationList = new List<VariationItem>();


            XmlTextReader xmlTR = new XmlTextReader(_filePath);
            
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;

            XmlReader reader = XmlReader.Create(xmlTR, settings);
            
            xmlDoc.Load(reader);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("curr", xmlDoc.DocumentElement.NamespaceURI);

            XmlElement xtcRoot = (XmlElement)xmlDoc.DocumentElement;

            foreach (XmlNode node in xtcRoot.ChildNodes)
            {
                XmlElement xtcTest = (XmlElement)node;

                if (string.Compare(xtcTest.Name, "Test", true) != 0)
                {
                    throw new InvalidOperationException("Expecting a Test XmlElement Node");
                }

                XmlNode xtcRootCopy = xtcRoot.CloneNode(false);
                XmlNode xtcTestCopy = xtcTest.CloneNode(true);
                xtcRootCopy.AppendChild(xtcTestCopy);

                //XmlCDataSection data = xmlDoc.CreateCDataSection(xtcRootCopy.OuterXml);
                

                MDEVariationItem v = new MDEVariationItem();
                v.Creator = "MDEVariationGenerator";

                TypeDesc typeDesc = new TypeDesc();
                typeDesc.Copy(_modelTypeDesc);
                v.ModelType = typeDesc;
                v.OriginalFile = Path.GetFileName(_filePath);
                v.Content = xtcRootCopy.OuterXml;
                              
                variationList.Add(v);
            }
            
            return variationList;
        }

        private TypeDesc _modelTypeDesc;       
        private string _filePath = "";
        MDELoadTypes _loadType;             
    }
}
