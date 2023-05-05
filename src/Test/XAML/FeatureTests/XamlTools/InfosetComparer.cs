// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xaml;

namespace UserObjects
{
    public static class InfosetComparer
    {
        public static bool Compare(InfosetProvider infoset1, InfosetProvider infoset2)
        {
            InfosetConsumer table1 = new InfosetTable();
            InfosetConsumer table2 = new InfosetTable();

            while (infoset1.Read())
            {
                table1.WriteNode(infoset1);
            }
            while (infoset2.Read())
            {
                table2.WriteNode(infoset2);
            }
            return table1.Equals(table2);
        }
    }

    public class InfosetTable : InfosetConsumer
    {
        private List<InfosetEntry> _infoList;

        public InfosetTable()
        {
            _infoList = new List<InfosetEntry>();
        }

        public List<InfosetEntry> InfosetEntries
        {
            get { return _infoList; }
        }


        public override bool Equals(object obj)
        {
            InfosetTable it = obj as InfosetTable;

            if (this.InfosetEntries.Count != it.InfosetEntries.Count)
            {
                return false;
            }
            for (int i = 0; i < this.InfosetEntries.Count; i++)
            {
                if (!this.InfosetEntries[i].Equals(it.InfosetEntries[i]))
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return _infoList.GetHashCode();
        }

        #region InfosetConsumer Members
        public override void StartObject(string prefix, XamlType xamlType, XamlNamespace xamlNS)
        {
            _infoList.Add(new InfosetEntry(XamlNodeType.StartObject, xamlType.GetName()));
        }

        public override void EndObject()
        {
            _infoList.Add(new InfosetEntry(XamlNodeType.EndObject, ""));
        }

        public override void StartProperty(string prefix, XamlProperty property)
        {
            _infoList.Add(new InfosetEntry(XamlNodeType.StartProperty, property.GetName()));
        }

        public override void EndProperty()
        {
            _infoList.Add(new InfosetEntry(XamlNodeType.EndProperty, ""));
        }

        public override void TextRepresentation(string text)
        {
            _infoList.Add(new InfosetEntry(XamlNodeType.Text, text));
        }

        public override void AddNamespace(string prefix, XamlNamespace xamlNs)
        {
            _infoList.Add(new InfosetEntry(XamlNodeType.AddNamespace, xamlNs.ToString()));
        }

        public override void SetPositionalArgumentTypes(IEnumerable<XamlType> args)
        {
            _infoList.Add(new InfosetEntry(XamlNodeType.PositionalArgumentTypes, ""));
        }

        public override void SetXmlData(System.Xml.Serialization.IXmlSerializable xmlData)
        {
            _infoList.Add(new InfosetEntry(XamlNodeType.XmlData, xmlData.ToString()));
        }

        public override void Eof()
        {
            _infoList.Add(new InfosetEntry(XamlNodeType.Eof, ""));
        }

        public override object Root
        {
            get { return null; }
        }
        #endregion
    }

    public class InfosetEntry
    {
        private XamlNodeType _xamlNodeType;
        private string _info;

        public InfosetEntry(XamlNodeType xamlNodeType, string info)
        {
            _xamlNodeType = xamlNodeType;
            _info = info;
        }

        public XamlNodeType NodeType
        {
            get { return _xamlNodeType; }
        }

        public string Info
        {
            get { return _info; }
        }

        public override bool Equals(object obj)
        {
            InfosetEntry ie = obj as InfosetEntry;
            return (this.NodeType == ie.NodeType) && (this.Info == ie.Info);
        }

        public override int GetHashCode()
        {
            return _info.GetHashCode() ^ _xamlNodeType.GetHashCode();
        }
    }
}
