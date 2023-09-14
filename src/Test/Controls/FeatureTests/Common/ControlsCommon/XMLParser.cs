using System;
using System.Xml;
using System.IO;

using System.Windows.Markup;

using Avalon.Test.ComponentModel;
using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Parse string to various data types
    /// </summary>
    public static class XMLParser
    {
        
        public static double ParseDouble(XmlNode node, string attr)
        {
            return Convert.ToDouble(ParseString(node, attr), System.Globalization.CultureInfo.InvariantCulture);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public static int ParseInt(XmlNode node, string attr)
        {
            return Convert.ToInt32(ParseString(node, attr), System.Globalization.CultureInfo.InvariantCulture);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public static bool ParseBool(XmlNode node, string attr)
        {
            return Convert.ToBoolean(ParseString(node, attr), System.Globalization.CultureInfo.InvariantCulture);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public static object ParseEnum(XmlNode node, string attr, Type t)
        {
            return Enum.Parse(t, ParseString(node, attr));
        }

        public static double ParseDouble(string val)
        {
            return Convert.ToDouble(val, System.Globalization.CultureInfo.InvariantCulture);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public static int ParseInt(string val)
        {
            return Convert.ToInt32(val, System.Globalization.CultureInfo.InvariantCulture);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public static bool ParseBool(string val)
        {
            return Convert.ToBoolean(val, System.Globalization.CultureInfo.InvariantCulture);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public static object ParseEnum(string val, Type t)
        {
            return Enum.Parse(t, val);
        }

        //----------------------------------------------------------
        public static string ParseString(XmlNode node, string attr)
        {
            XmlAttribute a = node.Attributes[attr];

            if (a == null)
            {
                string idx = "";

                if (node.Attributes["Index"] != null)
                {
                    idx = "#" + node.Attributes["Index"].Value;
                }

                throw new ApplicationException(node.Name + idx + " misses " + attr + "= attribute");
            }

            return a.Value;
        }

        //----------------------------------------------------------
        public static object ParseObject(XmlNode xmlNode)
        {
            GlobalLog.LogDebug("Loading Item Control using Parser: " + xmlNode.OuterXml);
            return ParseObject(xmlNode.OuterXml);
        }

        //----------------------------------------------------------
        public static object ParseObjectWithDefaultNS(XmlNode xmlNode)
        {
            string xamlString = xmlNode.OuterXml;

            xamlString = xamlString.Insert(xamlString.IndexOf(">"), defaultNameSpace);

            return ParseObject(xamlString);
        }

        public static object ParseObject(string xamlString)
        {

            object obj = null;

            if (xamlString != null)
            {
                //Create Avalon tree using Parser
                obj = XamlReader.Load(new XmlTextReader(new StringReader(xamlString)));
            }

            if (obj == null)
            {
                throw new ApplicationException("unable to create object using System.Windows.Markup.XamlReader");
            }

            return obj;
        }

        /// <summary>
        /// Default XML Namespace
        /// </summary>
        private static string defaultNameSpace = " xmlns = \"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"";

    }
}
