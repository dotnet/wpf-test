// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Test.Modeling;
using System.Windows.Markup;
using System.Xml.Serialization;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Microsoft.Test.Integration
{
    /// <summary>
    /// 
    /// </summary>
    [ContentProperty("Content")]
    public class MDEVariationItem : VariationItem
    {
        /// <summary>
        /// 
        /// </summary>
        public MDEVariationItem()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<VariationItem> GetVIChildren()
        {
            return new List<VariationItem>() ;
        }

        /// <summary>
        /// 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public string Content
        {
            get 
            { 
                return _content; 
            }
            set
            {
                _content = value;
                _content = Regex.Unescape(_content); 
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string OriginalFile
        {
            set
            {
                _originalFile = value;
            }
            get
            {
                return _originalFile;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AsynchronousModel
        {
            set
            {
                _asynchronousModel = value;
            }
            get
            {
                return _asynchronousModel;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TypeDesc ModelType
        {
            get 
            { 
                return _modelClassName; 
            }
            set 
            { 
                _modelClassName = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Execute()
        {
            Type type = ModelType.GetCurrentType();
            

            if (type == null)
            {
                throw new InvalidOperationException("The type " + ModelType.ToString() + " cannot be found.");
            }

            Type modelType = type;
            
            object modelObject = Activator.CreateInstance(modelType);

            if (!(modelObject is Model))
            {
                throw new InvalidOperationException("ModelType (" + modelType.FullName +") is not a subclass of " + typeof(Model).FullName);
            }

            XtcTestCaseLoader xtcLoader = new XtcTestCaseLoader(_content);

            xtcLoader.AddModel((Model)modelObject);
            
            if (_asynchronousModel)
            {
                xtcLoader.RunAsync(true);
            }
            else
            {
                xtcLoader.Run();
            }            
        }

        private bool _asynchronousModel = false;        
        private TypeDesc _modelClassName;
        private string _content = "";
        private string _originalFile = "";


    }
}
