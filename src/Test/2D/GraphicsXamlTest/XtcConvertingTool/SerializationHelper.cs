// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;


namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public partial class XamlPadPage : Window
    {
        // ------------------------------------------------------------------------------------

        public object CreateWCPObjFromXTC()
        {
            object wcpRetVal = null;
            object gtoObject = null;

            if (String.IsNullOrEmpty(DllName.Text) ||
                string.IsNullOrEmpty(FactoryName.Text) ||
                string.IsNullOrEmpty(xtcTB.Text)
                )
            {
                SetStatusText("Need to have Dll, Factory and XTC in order to run the Serialization!!", Brushes.Red);
                return null;
            }

            try
            {
                Assembly a = Assembly.LoadFrom(DllName.Text);
                if (a == null)
                {
                    throw new System.ApplicationException("Incorrect Assembly name or Assembly path " + DllName.Text + " !");
                }

                Type[] types = a.GetTypes();
                if (types == null)
                {
                    throw new System.ApplicationException("Fail to query for any type in Assembly " + DllName.Text + " !");
                }
                foreach (Type t in types)
                {

                    // need to special case for Pen since there is NO PenFactory
                    if (String.Compare(t.Name + "Factory", "PenFactory", false, System.Globalization.CultureInfo.InvariantCulture) == 0 &&
                        String.Compare(FactoryName.Text, "PenFactory", false, System.Globalization.CultureInfo.InvariantCulture) == 0
                        )
                    {
                        gtoObject = Activator.CreateInstance(t, new object[] { LoadingXTC().DocumentElement });
                        if (gtoObject == null)
                        {
                            throw new System.ApplicationException("Fail to create GTO Pen object !");
                        }

                        wcpRetVal = CreatingWCPOjbect(gtoObject);

                        return wcpRetVal;

                    }

                    int makeNum = -1;
                    if (String.Compare(t.Name, FactoryName.Text, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                    {
                        // Try #1 for Make(XmlElement)  
                        MethodInfo mi = t.GetMethod("Make", new Type[] { typeof(System.Xml.XmlElement) });
                        if (mi == null)
                        {
                            // Try #2 for Make(XmlElement, String)
                            mi = t.GetMethod("Make", new Type[] { typeof(System.Xml.XmlElement), typeof(String) });
                            if (mi == null)
                            {
                                // Try #3 for Make( string )
                                mi = t.GetMethod("Make", new Type[] { typeof(String) });
                                if (mi == null)
                                {
                                    throw new System.ApplicationException("We have tried all possible Make(...) calls, still no match!");
                                }
                                else
                                {
                                    makeNum = 2;
                                }
                            }
                            else
                            {
                                makeNum = 1;
                            }
                        }
                        else
                        {
                            makeNum = 0;
                        }

                        XmlDocument xd = LoadingXTC();

                        object o = Activator.CreateInstance(t);

                        switch (makeNum)
                        {
                            case 0:
                                gtoObject = mi.Invoke(o, new object[] { xd.DocumentElement });
                                break;
                            case 1:
                                gtoObject = mi.Invoke(o, new object[] { xd.DocumentElement, xd.DocumentElement.Attributes["Type"].Value });
                                break;
                            case 2:
                                gtoObject = mi.Invoke(o, new object[] { xd.DocumentElement.Attributes["Type"].Value });
                                break;
                            default:
                                throw new System.ApplicationException("Unknown Make call");
                        }

                        wcpRetVal = CreatingWCPOjbect(gtoObject);

                    }
                }
            }
            catch (System.Exception e)
            {
                SetStatusText(e.Message, Brushes.Red); ;
            }

            return wcpRetVal;
        }

        private XmlDocument LoadingXTC()
        {
            XmlDocument xd = new XmlDocument();
            if (!String.IsNullOrEmpty(xtcTB.Text))
            {
                xd.LoadXml(xtcTB.Text);
            }
            else
            {
                throw new System.ApplicationException("Please insert XTC into the XTC textbox before clicking the Serialization button!!!!");
            }

            return xd;
        }
        private object CreateWCPObject(Object gtoObject, string method)
        {
            if (gtoObject == null)
            {
                SetStatusText("gtoObject is NULL in CreateWCPObject", Brushes.Red);
                return null;
            }

            if (String.IsNullOrEmpty(method))
            {
                SetStatusText("method passed into CreateWCPObject is empty or null!", Brushes.Red);
                return null;
            }

            Type t = gtoObject.GetType();
            MethodInfo mo = t.GetMethod(method);
            return mo.Invoke(gtoObject, null);
        }

        private object CreatingWCPOjbect(object gtoObject)
        {
            object wcpRetVal = null;

            //  This nested If/Else statement is needed because most of the factory will only return
            // a GTO version of the object.  We will need to call a CreateXXXX function in the object to 
            // generate the Avalon version of the object.
            // However, there is always special case needs to be taken care. There are actually  2 Create apis in 
            // ITransform interface.  The right is CreateWCPTransform in order to get the Avalon Transform           
            if (String.Compare(FactoryName.Text, "TransformFactory", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                wcpRetVal = CreateWCPObject(gtoObject, "CreateWCPTransform");
                return wcpRetVal;
            }
            else
                if (String.Compare(FactoryName.Text, "PixelFormatFactory", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    // The value returned from PixelFormatFactory is already the Avalon version of PixelFormat.
                    wcpRetVal = gtoObject;
                }
                else
                {
                    // It is a hack.  I am trying to construct the function name from the Factory name.
                    // E.g. BrushFactory.  The function to create an Avalon Brush from a GTO Brush is by calling 
                    //      CreateBrush.
                    wcpRetVal = CreateWCPObject(gtoObject, "Create" + FactoryName.Text.Substring(0, FactoryName.Text.IndexOf("Factory")));
                }

            return wcpRetVal;
        }

        private string SerializeWCPObject(object o)
        {
            string s = null;
            s = XamlWriter.Save(o);
            s = IndentXaml(s);

            return s;
        }

        private string IndentXaml(string xaml)
        {
            if (!String.IsNullOrEmpty(xaml))
            {
                xaml = xaml.Replace(" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"", "");
            }
            else
            {
                return string.Empty;
            }

            //open the string as an XML node
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xaml);
            XmlNodeReader nodeReader = new XmlNodeReader(xmlDoc);

            //write it back onto a stringWriter
            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter(stringWriter);
            xmlWriter.Formatting = System.Xml.Formatting.Indented;
            xmlWriter.WriteNode(nodeReader, false);

            string result = stringWriter.ToString();
            xmlWriter.Close();

            return result;
        }

    }
}
