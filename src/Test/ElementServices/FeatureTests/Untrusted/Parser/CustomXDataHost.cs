// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: This is a Custom XdataHost, and the Verifiers to run the test case. 
 
  
 * Revision:         $Revision:$
 
 * Filename:         $Source:$
 *********************************************************************/
using System;
using System.Windows;
using System.Xml.Serialization;
using System.Xml;
using System.ComponentModel;
using System.Windows.Markup;
using System.Xml.Schema;
using Avalon.Test.CoreUI.Trusted;
using System.Windows.Controls;
using Avalon.Test.CoreUI.Serialization;
using System.Windows.Media;

namespace Avalon.Test.CoreUI.Parser.XData
{
    /// <summary>
    /// A c
    /// </summary>
    [ContentProperty("XmlSerializer")]
    public class CustomXDataHost
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CustomXDataHost()
        {
            _xmlSerializer = new MySerializer(this);
        }

        /// <summary>
        /// IXmlSerializable property that hold the xml content.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IXmlSerializable XmlSerializer
        {
            get { return _xmlSerializer; }
        }

        /// <summary>
        /// Should serialize it only if loaded. 
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeXmlSerializer()
        {
            return _inlineXmlLoaded;
        }

        /// <summary>
        /// Inner document to hold xml content.
        /// </summary>
        public XmlDocument Document
        {
            get { return _doc; }
        }

        private MySerializer _xmlSerializer;
        /// <summary>
        /// 
        /// </summary>
        public XmlDocument _doc = new XmlDocument();
        /// <summary>
        /// 
        /// </summary>
        public bool _inlineXmlLoaded;

        /// <summary>
        /// Verifies routine for XDataTest.xaml
        /// </summary>
        public static void XDataTestVerify(UIElement root)
        {
            CoreLogger.LogStatus("Verifying XDataTest.xaml.");
            FrameworkElement fe = root as FrameworkElement;

            CoreLogger.LogStatus("Verifying textblock1 ...");
            TextBlock textblock1 = fe.FindName("textblock1") as TextBlock;
            VerifyElement.VerifyBool(null == textblock1, false);
            if (0 != String.Compare(textblock1.Text.Trim(), "Test1", true))
            {
                throw new Microsoft.Test.TestValidationException("Text.Text is >>" + textblock1.Text.Trim() + "<<, should be Test");
            }
            VerifyBindings(textblock1);

            CoreLogger.LogStatus("Verifying textblock2 ...");
            TextBlock textblock2 = fe.FindName("textblock2") as TextBlock;
            VerifyElement.VerifyBool(null == textblock2, false);
            VerifyBindings(textblock2);

            CoreLogger.LogStatus("Verifying textblock3 ...");
            TextBlock textblock3 = fe.FindName("textblock3") as TextBlock;
            VerifyElement.VerifyBool(null == textblock3, false);
            VerifyBrushBinding(textblock3);

            CoreLogger.LogStatus("Verifying textblock4 ...");
            TextBlock textblock4 = fe.FindName("textblock4") as TextBlock;
            VerifyElement.VerifyBool(null == textblock4, false);
            VerifyBrushBinding(textblock4);

            //Verify Custom Xdata host
            CoreLogger.LogStatus("Verifying CustomXDataHost DS03 ...");
            CustomXDataHost host = (CustomXDataHost)fe.FindResource("DSO3");
            VerifyCustomXDataHost(host);

            //Verify Custom Xdata host
            CoreLogger.LogStatus("Verifying CustomXDataHost DS04 ...");
            host = (CustomXDataHost)fe.FindResource("DSO4");
            VerifyCustomXDataHost(host);

            CoreLogger.LogStatus("Verifying CustomXDataHost DS05 ...");
            host = (CustomXDataHost)fe.FindResource("DSO5");
            VerifyCustomXDataHost(host);

            CoreLogger.LogStatus("Verifying CustomXDataHost DS06 ...");
            host = (CustomXDataHost)fe.FindResource("DSO6");
            VerifyCustomXDataHost(host);

            CoreLogger.LogStatus("Verifying button1 ..."); 
            Button button = fe.FindName("button1") as Button;
            VerifyElement.VerifyBool(button != null, true);
            host = (CustomXDataHost)button.Content;
            VerifyElement.VerifyBool(host != null, true);
            VerifyCustomXDataHost(host);

            CoreLogger.LogStatus("Verifying button2 ..."); 
            button = fe.FindName("button2") as Button;
            VerifyElement.VerifyBool(button != null, true);
            host = (CustomXDataHost)button.Content;
            VerifyElement.VerifyBool(host != null, true);
            VerifyCustomXDataHost(host);

            CoreLogger.LogStatus("Verifying button3 ..."); 
            button = fe.FindName("button3") as Button;
            VerifyElement.VerifyBool(button != null, true);
            host = (CustomXDataHost)button.Content;
            VerifyElement.VerifyBool(host != null, true);
            VerifyCustomXDataHost(host);

            CoreLogger.LogStatus("Verifying button4 ..."); 
            button = fe.FindName("button4") as Button;
            VerifyElement.VerifyBool(button != null, true);
            host = (CustomXDataHost)button.Content;
            VerifyElement.VerifyBool(host != null, true);
            VerifyCustomXDataHost(host);
        }
        static void VerifyCustomXDataHost(CustomXDataHost host)
        {
            VerifyElement.VerifyBool(host != null, true);
            XmlElement rootElement = host.Document.DocumentElement;
            VerifyElement.VerifyBool(rootElement != null, true);
            XmlNodeList children = rootElement.ChildNodes;
            VerifyElement.VerifyInt(children.Count, 3);
        }
        static void VerifyBindings(TextBlock textblock)
        {
            if (textblock.FontSize != 200)
            {
                throw new Microsoft.Test.TestValidationException("Text.FontSize should be 200");
            }
            if (textblock.FontStyle != System.Windows.FontStyles.Italic)
            {
                throw new Microsoft.Test.TestValidationException("Text.FontStyle is" + textblock.FontStyle + ", should be 2: Italic");
            }
            VerifyBrushBinding(textblock);
        }

        static void VerifyBrushBinding(TextBlock textblock)
        {
            SolidColorBrush brush = textblock.Foreground as SolidColorBrush;
            VerifyElement.VerifyColor(brush.Color, Colors.Red);
        }
    }

    /// <summary>
    /// Another Custom XData Host using MySerializer2 as the IXmlSerializer property type.
    /// </summary>
    [ContentProperty("XmlSerializer")]
    public class CustomXDataHost2 : CustomXDataHost
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        public CustomXDataHost2()
        {
            _xmlSerializer = new MySerializer2(this);
        }
        /// <summary>
        /// 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        new public IXmlSerializable XmlSerializer
        {
            get { return _xmlSerializer; }
        }

        private MySerializer2 _xmlSerializer;
    }

    /// <summary>
    /// IXmlSerializable that inplement the methods in current type.
    /// </summary>
    public class MySerializer : IXmlSerializable
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host"></param>
        public MySerializer(CustomXDataHost host)
        {
            _host = host;
        }

        /// <summary>
        /// GetSchema
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// WriteXml
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            _host.Document.Save(writer);
        }

        /// <summary>
        /// ReadXml
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            _host.Document.Load(reader);
            _host._inlineXmlLoaded = true;
        }

        private CustomXDataHost _host;
    }
    /// <summary>
    /// IXmlSerializable that inplement the methods in IXmlSerializable interface.
    /// </summary>
    public class MySerializer2 : IXmlSerializable
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host"></param>
        public MySerializer2(CustomXDataHost2 host)
        {
            _host = host;
        }

        /// <summary>
        /// IXmlSerializable.GetSchema
        /// </summary>
        /// <returns></returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// IXmlSerializable.WriteXml
        /// </summary>
        /// <param name="writer"></param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            _host.Document.Save(writer);
        }

        /// <summary>
        /// IXmlSerializable.ReadXml
        /// </summary>
        /// <param name="reader"></param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            _host.Document.Load(reader);
            _host._inlineXmlLoaded = true;
        }

        private CustomXDataHost2 _host;
    }
}
