// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;

using ReflectTools;

namespace WFCTestLib.Log
{
	/// <summary>
	/// Summary description for ObjectDumper.
	/// </summary>
	public class ObjectDumper
	{

		public const int DEFAULT_DEPTH = 2;

		public static string GetObjectDumpString(object o)
		{ return GetObjectDumpString(o, DEFAULT_DEPTH); }
		public static string GetObjectDumpString(object o, int depth)
		{ return GetObjectDumpString(o, (null == o) ? null : o.GetType()); }
		public static string GetObjectDumpString(object o, Type t)
		{ return GetObjectDumpString(o, t, DEFAULT_DEPTH); }

		public static string GetObjectDumpString(object o, Type t, int depth)
		{
			Exception error = null;
			if (null != o && null == t)
			{
				t = o.GetType();
			}
			using (MemoryStream mStream = new MemoryStream())
			using (XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.ASCII))
			{
				writer.Formatting = Formatting.Indented;
				writer.Indentation = 1;
				try
				{
					DumpObject(o, t, depth, writer);
				}
				catch (Exception ex)
				{ error = ex; }
				writer.Flush();
				mStream.Flush();
				byte[] buff = mStream.ToArray();
				string s = System.Text.Encoding.ASCII.GetString(buff);
				if (null == error)
				{
					return s;
				}
				else
				{
					return "<ERROR><DETAIL><![CDATA[" + error.ToString() + "]]></DETAIL><![CDATA[" + s + "]]></ERROR>";
				}
			}
		}

		public static void DumpObjectToLog(Log l, object o)
		{ l.WriteRaw(GetObjectDumpString(o)); }
		public static void DumpObjectToLog(Log l, object o, int depth)
		{ l.WriteRaw(GetObjectDumpString(o)); }
		public static void DumpObjectToLog(Log l, object o, Type t)
		{ l.WriteRaw(GetObjectDumpString(o,t));}
		public static void DumpObjectToLog(Log l, object o, Type t, int depth)
		{ l.WriteRaw(GetObjectDumpString(o, t)); }



		
		private static void DumpObject(object o, Type t, int depth, XmlTextWriter writer)
		{
			if (t.IsPrimitive || t == typeof(string))
			{
				writer.WriteString((o == null) ? "null" : o.ToString());
			}
			else
			{
				writer.WriteStartElement("object");
				{
					writer.WriteAttributeString("type", (null == t) ? "UNKNOWN" : t.Name);
					writer.WriteAttributeString("tostring", (o == null) ? "null" : o.ToString());
					writer.WriteStartElement("value");
					{
						if (null == o)
						{ writer.WriteString("NULL"); }
						else
						{

							FieldInfo[] fields = SafeMethods.GetFields(t, ~BindingFlags.Static);
							if (fields.Length > 0)
							{
								writer.WriteStartElement("fields");
								{
									foreach (FieldInfo f in fields)
									{
										object value = null;
										try
										{
											value = f.GetValue(o);
										}
										catch (Exception) { }
										writer.WriteStartElement(f.Name);
										{
											writer.WriteAttributeString("type", f.FieldType.Name);
											if (depth > 0)
											{
												DumpObject(value, f.FieldType, depth - 1, writer);
											}
											else
											{
												writer.WriteString((null == value) ? null : value.ToString());
											}
										}
										writer.WriteFullEndElement();//f.Name
									}
								}
								writer.WriteFullEndElement();//fields
							}
							PropertyInfo[] props = SafeMethods.GetProperties(t, ~BindingFlags.Static & ~BindingFlags.SetProperty);
							if (props.Length > 0)
							{
								writer.WriteStartElement("properties");
								{
									foreach (PropertyInfo p in props)
									{
										writer.WriteStartElement(p.Name);
										{
											writer.WriteAttributeString("type", p.PropertyType.Name);
											ParameterInfo[] inf = p.GetIndexParameters();
											if (inf.Length > 0)
											{
												writer.WriteString("Cannot enumerate indexed parameters");
											}
											else
											{
												object value = null;
												try { value = p.GetValue(o, null); }
												catch (Exception) { }
												if (depth > 0)
												{
													DumpObject(value, p.PropertyType, depth - 1, writer);
												}
												else
												{
													writer.WriteString((null == o) ? null : value.ToString());
												}
											}
										}
										writer.WriteFullEndElement();//f.Name
									}
								}
								writer.WriteFullEndElement();//properties
							}

						}
					}
					writer.WriteFullEndElement();//value
				}
				writer.WriteFullEndElement();//object	
			}
		}
	}
}
