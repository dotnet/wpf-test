// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WFCTestLib.Util
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Drawing;
    using System.Windows.Forms;
    using WFCTestLib.Log;


    // <doc>
    // <desc>
    //  Contains methods to:
    //      show the differences between two strings
    //      set the font for a PropertyDude label
    //      create a name/value pair (for XML log entries)
    // </desc>
    // </doc>
    public class Handy
    {
        // <doc>
        // <desc>
        //  If the strings are different, will display side by side lists of chars
        //  so you can see which chars are different.
        // </desc>
        // <param term="log">
        //  The Log file to log the results to.
        // </param>
        // <param term="a">
        //  One of the strings to compare
        // </param>
        // <param term="b">
        //  The other string to compare
        // </param>
        // <retvalue>
        //  the value from the com+ String.CompareTo function.
        // </retvalue>
        // </doc>
        public static int CompareStrings (Log log, String a, String b)
        {

            int n = a.CompareTo(b);
            String s = "\nComparing: [" + a + "]\n        to [" + b + "]";

            if (log == null)
                Console.WriteLine(s);
            else
                log.WriteLine(s);

            if (n != 0 && (a.Length == b.Length))
            {
                s = ">>>CompareTo = " + n.ToString();
                if(log == null)
                    Console.WriteLine(s);
                else
                    log.WriteLine(s);

                char[] aa = a.ToCharArray();
                char[] bb = b.ToCharArray();
                for(int z = 0; z < aa.Length; z++)
                {
                    s = "char: " + z.ToString() + (aa[z] == bb[z] ? " == " : " != ") +
                        aa[z].ToString() + ", " + bb[z].ToString();

                    if (log == null)
                        Console.WriteLine(s);
                    else
                        log.WriteLine(s);
                }
            }

            return n;
        }

        // <doc>
        // <desc>
        //  If the strings are different, will display side by side lists of chars
        //  so you can see which chars are different.
        // </desc>
        // <param term="a">
        //  One of the strings to compare
        // </param>
        // <param term="b">
        //  The other string to compare
        // </param>
        // <retvalue>
        //  the value from the com+ String.CompareTo function.
        // </retvalue>
        // </doc>
        public static int CompareStrings (String a, String b)
        {
            return CompareStrings (null, a, b);
        }

        // <doc>
        // <desc>
        //  Used by PropertyDude tab pages to generate a font based on
        //  attributes.  Ie. if a property is browsable, then the font is
        //  bold.
        // </desc>
        // <param term="f">
        //  The template font
        // </param>
        // <param term="typ">
        //  The type of the class being tested
        // </param>
        // <param term="propertyName">
        //  The name of the property being tested.
        // </param>
        // <retvalue>
        //  A font that is <b>bold</b> if the property is browsable and/or
        //  <i>italic</i> if the property is actually implemented on the
        //  class being tested.
        // </retvalue>
        // </doc>
        public static Font GetPropertyFont(Font f, Type typ, String propertyName)
        {
            try
            {
                MethodInfo mi = typ.GetMethod(propertyName);
                if (mi != null)
                    if (mi.DeclaringType == typ)
                        f = new Font(f , FontStyle.Italic ) ;
                     

                PropertyInfo pi = typ.GetProperty(propertyName);
                if (pi != null)
                    if (pi.DeclaringType == typ)
                        f = new Font(f , FontStyle.Italic ) ;    

                if (pi != null)
                {
                    PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typ);
                    PropertyDescriptor pd = pdc[propertyName];
                    //PropertyDescriptor pd = ComponentDescriptor.GetProperty(typ, propertyName);
                    if (pd != null)
                        if (pd.IsBrowsable)
                            f = new Font(f , FontStyle.Bold ) ;                        
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }

            return f;
        }

        // <doc>
        // <desc>
        //  Generates a name="value" string used in xml log entries.
        // </desc>
        // <param term="name">
        //  The name in the name/value pair
        // </param>
        // <param term="value">
        //  The value in the name/value pair
        // </param>
        // <retvalue>
        //  A string formatted as: name="value"
        // </retvalue>
        // </doc>
        public static String NameValuePair(String name, String value)
        {
            return name + "=\"" + value + "\"";
        }

        // <doc>
        // <desc>
        //  Converts from a Point to a Size object
        // </desc>
        // <param term="pt">
        //  The X and Y parameters to be converted to Width and Height.
        // </param>
        // <retvalue>
        //  A size object whose Width and Height fields are the provided
        //  Point's X and Y fields (respectively)
        // </retvalue>
        // </doc>
        public static Size SizeFromPoint(Point pt)
        {
            return new Size(pt.X, pt.Y);
        }

        // <doc>
        // <desc>
        //  Dumps the values of all properties for the provided object
        //  to the provided Log
        // </desc>
        // <param term="o">
        //  The object the dump the properties for
        // </param>
        // <param term="log">
        //  The log file to dump the properties to
        // </param>
        // </doc>
        public static void DumpObjectProperties(Object o, Log log)
        {
            log.WriteTag("Properties", false, new LogAttribute("Object", o.GetType().Name));

            Type t = o.GetType();
            PropertyInfo[] pi = t.GetProperties();
            for (int i=0; i<pi.Length; i++)
            {
                log.Write(pi[i].Name + " = ");
                
                MethodInfo mi = pi[i].GetGetMethod();
                try
                {
                    if (mi.GetParameters().Length == 0)
                    {
                        Object v = mi.Invoke(o, new Object[] {});
                        log.WriteLine(v.ToString());
                    }
                    else
                        log.WriteLine("<getter takes parameters>");
                }
                catch (Exception e)
                {
                    String message = e.Message;
                    if (e.InnerException != null)
                        message = e.InnerException.Message;
                    log.WriteLine("<caught \"" + message + "\" invoking property getter>");
                }
            }

            log.CloseTag("Properties");
        }
    }
}

