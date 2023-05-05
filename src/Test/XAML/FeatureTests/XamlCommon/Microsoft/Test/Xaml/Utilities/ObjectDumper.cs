// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Utilities
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Dump an objects contents out for debugging
    /// </summary>
    public class ObjectDumper
    {
        /// <summary>
        /// filtered out members
        /// </summary>
        private readonly string[] _filteredMembers =
            {
                typeof(DataSet).FullName, "rowDiffId",
                typeof(DataSet).FullName, "RowDiffId",
            };

        /// <summary>
        /// filtered out types
        /// </summary>
        private readonly string[] _filteredTypes =
            {
                typeof(Pointer).FullName,
                typeof(CultureInfo).FullName,
                "System.Data.Index",
                "System.Data.DataColumnCollection",
            };

        /// <summary>
        /// hashtable of objects to break infinite looping
        /// </summary>
        private readonly Hashtable _objects;

        /// <summary>
        /// Output goes here
        /// </summary>
        private TextWriter _writer = Console.Out;

        /// <summary>
        /// dump fields as well ?
        /// </summary>
        private bool _dumpFields = true;

        /// <summary>
        /// Should properties be dumped
        /// </summary>
        private bool _dumpProperties = true;

        /// <summary>
        /// index to use
        /// </summary>
        private int _indent = 1;

        /// <summary>
        /// Initializes a new instance of the ObjectDumper class
        /// </summary>
        public ObjectDumper()
        {
            ReferenceComparer refComparer = new ReferenceComparer();
            _objects = new Hashtable(refComparer);
        }

        /// <summary>
        /// Write out the location of the method
        /// </summary>
        /// <param name="method">method to dump</param>
        public static void DumpLocation(MethodBase method)
        {
            Console.WriteLine("Calling " + method.DeclaringType + "::" + method);
        }

        /// <summary>
        /// dump object to a string
        /// </summary>
        /// <param name="name">name to use</param>
        /// <param name="o">object to dump</param>
        /// <returns>dumped string rep of the object</returns>
        public string DumpToString(string name, object o)
        {
            StringBuilder stringBuilder = new StringBuilder();
            this._writer = new StringWriter(stringBuilder, CultureInfo.InvariantCulture);
            this.Dump(name, o);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Dump object out
        /// </summary>
        /// <param name="name">name for the dump</param>
        /// <param name="o">object to dump</param>
        public void Dump(string name, object o)
        {
            for (int i = 0; i < _indent; i++)
            {
                _writer.Write("--- ");
            }

            if (name == null)
            {
                name = string.Empty;
            }

            if (o == null)
            {
                _writer.WriteLine(name + " = null");
                return;
            }

            Type type = o.GetType();

            _writer.Write(type.Name + " " + name);

            if (_objects[o] != null)
            {
                _writer.WriteLine(" Existing " + type.Name + " object in graph");
                return;
            }

            if (((IList)_filteredTypes).Contains(type.FullName))
            {
                _writer.WriteLine(" <-- Type Filtered Out -->");
                return;
            }

            if (type != typeof(string))
            {
                _objects.Add(o, o);
            }

            if (type.IsArray)
            {
                Array a = (Array)o;
                _writer.WriteLine();
                _indent++;
                for (int j = 0; j < a.Length; j++)
                {
                    Dump("[" + j + "]", a.GetValue(j));
                }

                _indent--;
            }
            else if (o is Stream)
            {
                Stream stream = (Stream)o;
                int bytesRead = 0;
                byte[] buffer = new byte[256];
                do
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        DumpBytes(buffer, 0, bytesRead);
                    }
                }
                while (bytesRead > 0);
            }
            else if (o is byte[])
            {
                byte[] bytes = (byte[])o;
                DumpBytes(bytes, 0, bytes.Length);
            }
            else if (o is XmlQualifiedName)
            {
                Dump("Name", ((XmlQualifiedName)o).Name);
                Dump("Namespace", ((XmlQualifiedName)o).Namespace);
            }
            else if (o is XmlNode)
            {
                string xml = ((XmlNode)o).OuterXml;
                xml = xml.Replace('\n', ' ');
                xml = xml.Replace('\r', ' ');
                _writer.WriteLine(" = " + xml);
                return;
            }
            else if (type.IsEnum)
            {
                _writer.WriteLine(" = " + ((Enum)o).ToString());
            }
            else if (type == typeof(decimal))
            {
                _writer.WriteLine(" = " + ((decimal)o).ToString(CultureInfo.InvariantCulture));
            }
            else if (type == typeof(float))
            {
                _writer.WriteLine(" = " + ((float)o).ToString(CultureInfo.InvariantCulture));
            }
            else if (type == typeof(double))
            {
                _writer.WriteLine(" = " + ((double)o).ToString(CultureInfo.InvariantCulture));
            }
            else if (type.IsPrimitive)
            {
                _writer.WriteLine(" = " + o.ToString());
            }
            else if (typeof(Exception).IsAssignableFrom(type))
            {
                _writer.WriteLine(" = " + ((Exception)o).Message);
            }
            else if (o is string)
            {
                DumpString(o.ToString());
            }
            else if (o is Uri)
            {
                DumpString(o.ToString());
            }
            else if (o is DateTime)
            {
                DumpString(((DateTime)o).ToString(CultureInfo.InvariantCulture) + " Kind: " + ((DateTime)o).Kind.ToString());
            }
            else if (o is TimeSpan)
            {
                DumpString(o.ToString());
            }
            else if (o is Guid)
            {
                DumpString(o.ToString());
            }
            else if (o is Type)
            {
                DumpString(((Type)o).FullName);
            }
            else if (o is IEnumerable)
            {
                IEnumerator e = ((IEnumerable)o).GetEnumerator();
                if (e == null)
                {
                    _writer.WriteLine(" GetEnumerator() == null");
                    return;
                }

                _writer.WriteLine();
                int c = 0;
                _indent++;
                while (e.MoveNext())
                {
                    Dump("[" + c + "]", e.Current);
                    c++;
                }

                _indent--;
            }
            else
            {
                bool oldValue = _dumpProperties;
                if (o is System.Runtime.Serialization.ExtensionDataObject)
                {
                    _dumpProperties = false;
                }

                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
                _writer.WriteLine();
                _indent++;
                for (; type != null; type = type.BaseType)
                {
                    if (_dumpFields)
                    {
                        FieldInfo[] fields = type.GetFields(bindingFlags);
                        for (int i = 0; i < fields.Length; i++)
                        {
                            FieldInfo f = fields[i];
                            if (f.IsStatic)
                            {
                                continue;
                            }

                            if (IsMemberFiltered(type.FullName, f.Name))
                            {
                                continue;
                            }

                            Dump(f.Name, f.GetValue(o));
                        }
                    }

                    if (_dumpProperties)
                    {
                        PropertyInfo[] props = type.GetProperties(bindingFlags);
                        for (int i = 0; i < props.Length; i++)
                        {
                            PropertyInfo p = props[i];
                            if (p.GetIndexParameters().Length != 0)
                            {
                                continue;
                            }

                            if (!p.CanRead)
                            {
                                continue;
                            }

                            if (!typeof(IEnumerable).IsAssignableFrom(p.PropertyType) && !p.CanWrite)
                            {
                                continue;
                            }

                            if (p.PropertyType == type)
                            {
                                continue;
                            }

                            if (IsMemberFiltered(type.FullName, p.Name))
                            {
                                continue;
                            }

                            object v;
                            try
                            {
                                v = p.GetValue(o, null);
                            }
                            catch (Exception e)
                            {
                                v = e;
                            }

                            Dump(p.Name, v);
                        }
                    }
                }

                _indent--;

                if (o is System.Runtime.Serialization.ExtensionDataObject)
                {
                    _dumpProperties = oldValue;
                }
            }
        }

        /// <summary>
        /// check if member is filtered out
        /// </summary>
        /// <param name="typeName">type name to use</param>
        /// <param name="memberName">member name to use</param>
        /// <returns>true if filtered</returns>
        private bool IsMemberFiltered(string typeName, string memberName)
        {
            for (int j = 0; j < _filteredMembers.Length; j += 2)
            {
                if (_filteredMembers[j] == typeName && _filteredMembers[j + 1] == memberName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Dump string to output
        /// </summary>
        /// <param name="s">string to dump</param>
        private void DumpString(string s)
        {
            if (s.Length > 40)
            {
                _writer.WriteLine(" = ");
                _writer.WriteLine("\"" + s + "\"");
            }
            else
            {
                _writer.WriteLine(" = \"" + s + "\"");
            }
        }

        /// <summary>
        /// dump bytes to output
        /// </summary>
        /// <param name="buf">buffer of bytes</param>
        /// <param name="start">start index</param>
        /// <param name="len">length to dump</param>
        private void DumpBytes(byte[] buf, int start, int len)
        {
            bool more = false;
            if (len > 256)
            {
                len = 256;
                more = true;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = start; i < (start + len); i++)
            {
                sb.Append(" ").Append(buf[i].ToString(CultureInfo.InvariantCulture));
            }

            if (more)
            {
                sb.Append("...");
            }

            _writer.WriteLine(sb.ToString());
        }

        /// <summary>
        /// ReferenceComparer class
        /// </summary>
        public class ReferenceComparer : IEqualityComparer
        {
            /// <summary>
            /// Are two objects equal
            /// </summary>
            /// <param name="x">first object</param>
            /// <param name="y">second object</param>
            /// <returns>true if equal</returns>
            public new bool Equals(object x, object y)
            {
                return x == y;
            }

            /// <summary>
            /// Get the hashcode of the object
            /// </summary>
            /// <param name="obj">object to use</param>
            /// <returns>hashcode of object</returns>
            public int GetHashCode(object obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }
    }
}
