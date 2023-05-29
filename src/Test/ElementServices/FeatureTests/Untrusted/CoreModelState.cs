// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Automation for the stateless MDE model.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using Microsoft.Test;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;
using System.IO;
using System.Collections;
using System.Text;
using System.Reflection;

using Microsoft.Test.Discovery;

namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CoreState
    {
         /// <summary>
        /// 
        /// </summary>
        static private string s_fileName = "__ModelState.xml";

        /// <summary>
        /// 
        /// </summary>
        public CoreState()
        {
            _dictionary = new PropertyBag();
        }

        /// <summary>
        /// Converts a Microsoft.Test.Modeling.State object to a ModelState object.
        /// </summary>
        /// <param name="state"></param>
        public CoreState(State state)
            : this()
        {
            // Copy all key-value pairs from state to _dictionary.
            foreach (object key in state.Keys)
            {
                //_dictionary.Add(key, state[key]);
                _dictionary[(string)key] = (string)state[key];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PropertyBag Dictionary
        {
            get
            {
                return _dictionary;
            }
            set
            {
                _dictionary = value;
            }
        }


        /// <summary>
        /// </summary>
        public bool CompiledVersion
        {
            get
            {
                if(Dictionary == null || Dictionary["CompiledVersion"] == null)
                {
                    return false;
                }
                
                return (bool)Dictionary.ContainsProperty("CompiledVersion");
            }
        }
        
        /// <summary>
        /// </summary>
        public HostType Source
        {
            get
            {
                if(Dictionary == null || Dictionary["Source"] == null)
                {
                    return HostType.HwndSource;
                }
                return (HostType)Enum.Parse(typeof(HostType), Dictionary["Source"]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void LogState()
        {
            
        }


        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return WriteState();            
        }


        private string WriteState()
        {
            return _dictionary.ToString();     
        }

        /// <summary>
        /// 
        /// </summary>
        static public string GetFullFilePath()
        {
            return Utility.TempDir + "\\" + s_fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        static public void Persist(CoreState modelState)
        {
            // IOHelper.SaveObjectToFile(modelState, GetFullFilePath());
            
            string fileName = GetFullFilePath();
            Microsoft.Test.Logging.GlobalLog.LogDebug("Saving state: " + fileName);

            object obj = modelState;

            Stream stream = File.Open(fileName, FileMode.Create);
            StreamWriter writer = new StreamWriter(stream);

            try
            {
                //SoapFormatter formatter = new SoapFormatter();
                //formatter.Serialize(stream, obj);
                string s = Microsoft.Test.Serialization.ObjectSerializer.Serialize(obj);
                writer.Write(s);
            }
            finally
            {
                writer.Close();
                stream.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        static public CoreState Load()
        {
            string useFileName = GetFullFilePath();

            if (!File.Exists(useFileName))
            {
                string assemblyPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(CoreModelState)).Location);
                useFileName = assemblyPath + "\\" + s_fileName;
            }

            //return (CoreModelState)IOHelper.LoadObjectFromFile(useFileName);

            string fileName = useFileName;

            Stream stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
            System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(stream);
            object obj = null;

            try
            {
                //SoapFormatter formatter = new SoapFormatter();
                //obj = formatter.Deserialize(stream);
                obj = Microsoft.Test.Serialization.ObjectSerializer.Deserialize(reader, typeof(CoreState), null);
            }
            finally
            {
                reader.Close();
                stream.Close();
            }

            return (CoreState)obj;
        }

        private PropertyBag _dictionary = null;
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class CoreModelState
    {
        /// <summary>
        /// 
        /// </summary>
        static private string s_fileName = "__ModelState.xml";

        /// <summary>
        /// 
        /// </summary>
        public CoreModelState()
        {
            _dictionary = new Hashtable();
        }

        /// <summary>
        /// Converts a Microsoft.Test.Modeling.State object to a CoreModelState object.
        /// </summary>
        /// <param name="state"></param>
        public CoreModelState(State state)
            : this()
        {
            // Copy all key-value pairs from state to _dictionary.
            foreach (object key in state.Keys)
            {
                _dictionary.Add(key, state[key]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Hashtable Dictionary
        {
            get
            {
                return _dictionary;
            }
            set
            {
                _dictionary = value;
            }
        }


        /// <summary>
        /// </summary>
        public bool CompiledVersion
        {
            get
            {
                if(Dictionary == null || Dictionary["CompiledVersion"] == null)
                {
                    return false;
                }
                
                return (bool)Dictionary["CompiledVersion"];
            }
        }
        
        /// <summary>
        /// </summary>
        public HostType Source
        {
            get
            {
                if(Dictionary == null || Dictionary["Source"] == null)
                {
                    return HostType.HwndSource;
                }
                return (HostType)Dictionary["Source"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void LogState()
        {
            
        }


        /// <summary>
        /// </summary>
        public override string ToString()
        {
            return WriteState();            
        }


        private string WriteState()
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            if (_dictionary == null)
                return stringBuilder.ToString();

            IDictionaryEnumerator enumarator = _dictionary.GetEnumerator();

            while(enumarator.MoveNext())
            {
                stringBuilder.Append(enumarator.Key);
                stringBuilder.Append(": ");
                stringBuilder.Append(enumarator.Value);            
                stringBuilder.Append(" ");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        static public string GetFullFilePath()
        {
            return Utility.TempDir + "\\" + s_fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        static public void Persist(CoreModelState modelState)
        {
            IOHelper.SaveObjectToFile(modelState, GetFullFilePath());
        }

        /// <summary>
        /// 
        /// </summary>
        static public CoreModelState Load()
        {
            string useFileName = GetFullFilePath();

            if (!File.Exists(useFileName))
            {
                string assemblyPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(CoreModelState)).Location);
                useFileName = assemblyPath + "\\" + s_fileName;                
            }

            return (CoreModelState)IOHelper.LoadObjectFromFile(useFileName);
        }


        private Hashtable _dictionary = null;

    }
}
