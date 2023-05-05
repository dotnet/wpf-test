// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xaml;
using System.IO;

namespace Microsoft.Test.Xaml.Parser
{
    public class TestFileWriter : XamlWriter
    {
        private int _typeIndex;
        private int _propIndex;
        private int _nsIndex;

        private Dictionary<string, int> _typeDictionary;
        private Dictionary<string, int> _propertyDictionary;
        private Dictionary<string, int> _namespaceDictionary;

        private StringBuilder _infosetBuilder;


        public TestFileWriter()
        {
            _typeIndex = 0;
            _propIndex = 0;
            _nsIndex = 0;

            _typeDictionary = new Dictionary<string, int>();
            _propertyDictionary = new Dictionary<string, int>();
            _namespaceDictionary = new Dictionary<string, int>();

            _infosetBuilder = new StringBuilder();
        }

        public void WriteFile(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            sw.Write(AssembleOutput());
            sw.Flush();
            sw.Close();
        }

        public string AssembleOutput()
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine("NamespaceTable:");
            output.Append(WriteReverseDictionary(_namespaceDictionary));
            output.AppendLine(":EndTable");
            output.AppendLine();
            output.AppendLine("TypeTable:");
            output.Append(WriteReverseDictionary(_typeDictionary));
            output.AppendLine(":EndTable");
            output.AppendLine();
            output.AppendLine("PropertyTable:");
            output.Append(WriteReverseDictionary(_propertyDictionary));
            output.AppendLine(":EndTable");
            output.AppendLine();
            output.AppendLine("Infoset:");
            output.Append(_infosetBuilder.ToString());
            output.AppendLine(":EndTable");

            return output.ToString();
        }

        private string WriteReverseDictionary(Dictionary<string, int> dic)
        {
            StringBuilder sb = new StringBuilder();
            foreach(KeyValuePair<string, int> kvp in dic)
            {
                sb.AppendLine(kvp.Value.ToString() + "," + kvp.Key);
            }
            return sb.ToString();
        }

        private int GetTypeNumber(string typeDescription)
        {
            if (_typeDictionary.ContainsKey(typeDescription))
            {
                return _typeDictionary[typeDescription];
            }
            else
            {
                _typeDictionary.Add(typeDescription, _typeIndex);
                return _typeIndex++;
            }
        }

        private int GetPropertyNumber(string propertyDescription)
        {
            if (_propertyDictionary.ContainsKey(propertyDescription))
            {
                return _propertyDictionary[propertyDescription];
            }
            else
            {
                _propertyDictionary.Add(propertyDescription, _propIndex);
                return _propIndex++;
            }
        }

        private int GetNamespaceNumber(string namespaceDescription)
        {
            if (_namespaceDictionary.ContainsKey(namespaceDescription))
            {
                return _namespaceDictionary[namespaceDescription];
            }
            else
            {
                _namespaceDictionary.Add(namespaceDescription, _nsIndex);
                return _nsIndex++;
            }
        }

        private string GetTypeDescription(XamlType xType)
        {
            return xType.Name + "," + XamlContext.GetClrType(xType);
        }

        private string GetPropertyDescription(XamlProperty xProp)
        {
            if (xProp.OwnerType != null)
            {
                return xProp.Name + "," + GetTypeNumber(GetTypeDescription(xProp.OwnerType));
            }
            else
            {
                return xProp.Name;
            }
        }

        private string GetNamespaceDescription(XamlNamespace xNS, string prefix)
        {
            return prefix + "," + xNS.BoundName + "," + xNS.TargetNamespace;
        }

        private void AddInfosetInstruction(string instruction)
        {
            _infosetBuilder.AppendLine(instruction);
        }


        

        #region XamlWriter Members
        public override void Close()
        {
            AddInfosetInstruction("Close");
        }

        public override object Result
        {
            get { return null; }
        }

        public override void WriteEndObject()
        {
            AddInfosetInstruction("WEO");
        }

        public override void WriteEndProperty()
        {
            AddInfosetInstruction("WEP");
        }

        public override void WriteNamespace(XamlNamespace xamlNamespace, string prefix)
        {
            int num = GetNamespaceNumber(GetNamespaceDescription(xamlNamespace, prefix));
            AddInfosetInstruction("WNS," + num.ToString() + "," + prefix);
        }

        public override void WriteObject(XamlType type, string prefix)
        {
            int num = GetTypeNumber(GetTypeDescription(type));
            AddInfosetInstruction("WO," + num.ToString() + "," + prefix);
        }

        public override void WriteProperty(XamlProperty property, string prefix)
        {
            int num = GetPropertyNumber(GetPropertyDescription(property));
            AddInfosetInstruction("WP," + num.ToString() + "," + prefix);
        }

        public override void WriteText(string text)
        {
            AddInfosetInstruction("WT," + text);
        }
        #endregion
    }
}
