// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;				
using System.IO;                //TextWriter
using System.Text;				//StringBuilder


namespace Microsoft.Test.KoKoMo
{
    /// <summary>
    /// ModelTraceLevel
    /// </summary>
	public enum ModelTraceLevel
	{
	};

    /// <summary>
    ///  ModelTrace class
    /// </summary>
	public class ModelTrace
	{
		//Data
        static bool         s_enabled    = true;
        static TextWriter   s_writer     = Console.Out;

        /// <summary>Get and set Enabled property</summary>
		public static bool		    Enabled
		{
			get { return s_enabled;	}
			set { s_enabled = value;	}
		}

        /// <summary>Get and set Out property</summary>
        public static TextWriter    Out
        {
            get { return s_writer;   }
            set { s_writer = value;  }
        }
        
        /// <summary>
        /// Write method
        /// </summary>
        /// <param name="value"></param>
		public static void		    Write(object value)
		{
            if(s_enabled && s_writer != null)
				s_writer.Write(value);
		}

        /// <summary>
        /// WriteLine Method.
        /// </summary>
		public static void		    WriteLine()
		{
			if(s_enabled && s_writer != null)
				s_writer.WriteLine();
		}

        /// <summary>
        /// WriteLine method
        /// </summary>
        /// <param name="value"></param>
		public static void		    WriteLine(object value)
		{
            if(s_enabled && s_writer != null)
                s_writer.WriteLine(value);
		}

        /// <summary>
        /// FormatMethod Method
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
		public static string	    FormatMethod(ModelActionInfo info)
		{
			//Format:
			//	Command cmd = conn.createcommand();
			StringBuilder buffer = new StringBuilder(100);

			//Return
			if(info.RetVal != null)
			{
				Model output = info.RetVal as Model;
				if(output != null)
				{
					//Type
					if(info.Created)
					{
						buffer.Append(output.GetType().Name);
						buffer.Append(" ");
					}
							
					//Variable
					buffer.Append(output.Name);
					buffer.Append(output.Id);
					buffer.Append(" = ");
				}
			}
			
			//Call
			buffer.Append(info.Action.Model.Name);
			if(!info.Action.Method.IsStatic)
				buffer.Append(info.Action.Model.Id);
			buffer.Append(".");
			buffer.Append(info.Action.Name);
			
			//Parameters
			buffer.Append("(");
			if(info.Parameters != null)
			{
				int ordinal = 0;
				foreach(ModelParameter parameter in info.Parameters)
				{
					if(ordinal++ > 0)
						buffer.Append(", ");
					FormatValue(buffer, parameter.Type, parameter.Value.Value);
				}
			}
			buffer.Append(")");
			buffer.Append(";");
		
			//Output
			return buffer.ToString();
		}

        /// <summary>
        /// FormatValue method
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
		public static void		    FormatValue(StringBuilder builder, Type type, object value)
		{
			String prefix = null;
			String suffix = null;
			String cast   = null;
			String format = null;

			//Special types
			if(type == typeof(String))
			{
				//Strings
				prefix = "\""; 
				suffix = "\""; 
			}
			else if(type.IsEnum)
			{
				//Enums
				prefix = type.Name + ".";
				format = value.ToString().Replace(", ", " | " + prefix);
			}
			else if(type == typeof(bool))
			{
				//Bool (lowercase)
				format = value.ToString().ToLower();
			}
			else if(type == typeof(byte))
			{
				cast = "(byte)";
			}
			else if(type == typeof(short))
			{
				cast = "(short)";
			}
			else if(type.IsArray && value != null)
			{
				//new object[]{...}
				builder.Append("new ");
				//Note: Don't box integers, primarily since Int32 isn't a portable type (ie: Java)
				builder.Append(type == typeof(Int32[]) ? "int[]" : type.Name);
				builder.Append("{");

				//Recurse
				int ordinal = 0;
				Array items = (Array)value;
				foreach(object item in items)
				{
					if(ordinal++ > 0)
						builder.Append(", ");
					FormatValue(builder, type.GetElementType(), item);
				}

				builder.Append("}");
				return;	//Were done
			}

			//Default formating
			if(format == null && value != null)
				format = value.ToString();
			if(format == null)
				format = "null";

			//Prefix
			if(prefix != null)
				builder.Append(prefix);
			
			//Value
			if(cast != null)
				builder.Append(cast);
			builder.Append(format);

			//Suffix
			if(suffix != null)
				builder.Append(suffix);
		}
	}
}
