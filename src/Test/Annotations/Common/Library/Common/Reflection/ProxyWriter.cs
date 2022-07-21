// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Converts a ProxyDefinition into compilable C# code.

using System;
using System.Windows;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Annotations.Test.Reflection
{

  /// <summary>
  /// Module for converting a ProxyDefinition to compilable C# code and writing it to a file.
  /// </summary>
  public class ProxyWriter
  {
    //------------------------------------------------------
    //
    //  Public Methods
    //
    //------------------------------------------------------

    #region Public Methods

    /// <summary>
    /// Create proxy for this ProxyDefinition in the given directory.
    /// </summary>
    /// <returns>Reference to Proxy file that was generated.</returns>
    public FileInfo WriteProxy(ProxyDefinition proxyDef, DirectoryInfo outputDirectory)
    {
      FileInfo file = new FileInfo(outputDirectory.FullName + proxyDef.Source.Name);
      if (file.Exists)
        file.Delete();

      StreamWriter output = new StreamWriter(file.OpenWrite());
      output.AutoFlush = true;

      WriteHeader(output);
      WriteImports(output, proxyDef);

      output.WriteLine("namespace " + ProxyConstants.PROXY_NAMESPACE_PREFIX + "." + proxyDef.Namespace);
      output.WriteLine("{");

      IList<Type> classes = proxyDef.ClassTypes;
      for (int i = 0; i < classes.Count; i++)
      {
        Console.WriteLine("\tWriting - " + classes[i].FullName);
        if (classes[i].IsEnum)
          WriteEnum(output, proxyDef, classes[i]);
        else if (classes[i].IsInterface)
        {
          WriteInterface(output, proxyDef, classes[i]);
        }
        else
          WriteClass(output, proxyDef, classes[i]);
      }
      WriteDelegates(output, proxyDef);

      output.WriteLine("}");
      output.Close();
      return file;
    }

    #endregion Public Methods

    //------------------------------------------------------
    //
    //  Private Methods
    //
    //------------------------------------------------------

    #region Private Methods

    /// <summary>
    /// Add a header to the proxy class indicating that it is automatically generated and
    /// when it was last built.
    /// </summary>
    private void WriteHeader(StreamWriter writer)
    {
      writer.WriteLine("//---------------------------------------------------------------------");
      writer.WriteLine("//  Copyright (c) Microsoft Corporation, 2004");
      writer.WriteLine("//");
      writer.WriteLine("//  *** FILE IS AUTOMATICALLY GENERATED, DO NOT EDIT BY HAND ***");
      writer.WriteLine("//");
      writer.WriteLine("//        Generated: " + DateTime.Now);
      writer.WriteLine("//---------------------------------------------------------------------");
      writer.WriteLine("");
    }

    /// <summary>
    /// Write all the namespaces that this Proxy needs.
    /// </summary>
    private void WriteImports(StreamWriter writer, ProxyDefinition def)
    {
      writer.WriteLine("// Required proxy imports.");
      for (int i = 0; i < proxyImports.Length; i++)
      {
        if (!def.Imports.Contains(proxyImports[i]))
          writer.WriteLine("using " + proxyImports[i] + ";");
      }

      if (ImportOverloads != null)
        writer.WriteLine(ImportOverloads);

      writer.WriteLine("// Delegate specific imports.");
      IEnumerator<string> imports = def.Imports.GetEnumerator();
      while (imports.MoveNext())
      {
        if (!imports.Current.StartsWith("MS."))
          writer.WriteLine("using " + imports.Current + ";");
      }

      writer.WriteLine("");
    }

    /// <summary>
    /// Write the body of a public Enum class.
    /// </summary>
    private void WriteEnum(StreamWriter writer, ProxyDefinition def, Type type)
    {
      writer.WriteLine("\tpublic enum " + type.Name);
      writer.WriteLine("\t{");

      FieldInfo[] fields = def.PublicFields(type);
      for (int i = 0; i < fields.Length; i++)
      {
        if (!fields[i].IsSpecialName)
        {
          writer.WriteLine("\t\t" + fields[i].Name + ((i == fields.Length - 1) ? "" : ","));
        }
      }

      writer.WriteLine("\t}");
    }

    /// <summary>
    /// Write the body of a public Interface.
    /// </summary>
    private void WriteInterface(StreamWriter writer, ProxyDefinition def, Type type)
    {
      string signature = "public interface " + type.Name;
      // Must implement IReflectiveProxy.
      signature += " : IReflectiveProxy";
      // Implement all inherited interfaces.
      Type[] interfaces = def.Interfaces(type);
      for (int i = 0; i < interfaces.Length; i++)
        signature += ", " + TypeAsString(interfaces[i]);

      writer.WriteLine("\t" + signature); 
      writer.WriteLine("\t{");
      
      WriteMethods(writer, def, type, true);
      WriteProperties(writer, def, type, true);

      writer.WriteLine("\t}");
    }

    /// <summary>
    /// A ProxyDefinition may contain multiple classes, write the proxy class for this
    /// Type.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="def"></param>
    /// <param name="type"></param>
    private void WriteClass(StreamWriter writer, ProxyDefinition def, Type type)
    {
      string classSignature = "\t";

      if (type.IsAbstract)
        classSignature += "abstract ";
      classSignature += "public class " + type.Name + " : " + def.BaseClass(type);

      Type[] interfaces = def.Interfaces(type);
      for (int i = 0; i < interfaces.Length; i++)
        classSignature += ", " + TypeAsString(interfaces[i]);

      writer.WriteLine(classSignature);
      writer.WriteLine("\t{");

      WriteConstructors(writer, type.Name, def.Constructors(type));
      WriteMethods(writer, def, type, false);
      WriteProperties(writer, def, type, false);
      WriteStaticFields(writer, def, type);
      WriteEvents(writer, def, type);
      WriteFields(writer, def, type);

      writer.WriteLine("\t}");
    }

    /// <summary>
    /// Write a section heading (e.g. Constuctors, Methods, etc).
    /// </summary>
    private void WriteSectionHeading(StreamWriter writer, string heading)
    {
      writer.WriteLine("");
      writer.WriteLine("\t\t//------------------------------------------------------");
      writer.WriteLine("\t\t//");
      writer.WriteLine("\t\t//  " + heading);
      writer.WriteLine("\t\t//");
      writer.WriteLine("\t\t//------------------------------------------------------");
      writer.WriteLine("");
    }

    #region Field Writers

    /// <summary>
    /// Write declarations for all public static fields.
    /// </summary>
    private void WriteStaticFields(StreamWriter writer, ProxyDefinition def, Type type)
    {
      WriteSectionHeading(writer, "Delegate Static Fields");

      FieldInfo[] fields = def.PublicFields(type);
      for (int i = 0; i < fields.Length; i++)
      {
        FieldInfo current = fields[i];
        if (current.IsStatic)
        {
          string typeName = TypeAsString(current.FieldType);
          if (current.FieldType.IsPrimitive || current.FieldType.Equals(typeof(String)))
          {
            string value = current.GetValue(null).ToString();
            writer.WriteLine("\t\tpublic static " + typeName + " " + current.Name + " = \"" + value + "\";");
          }
          else
          {
            writer.WriteLine("\t\tpublic static " + typeName + " " + current.Name + ";");
          }
        }
      }

      WriteSectionHeading(writer, "Proxy Static Fields");
      // If we inherit from another proxy that we already have this field.
      if (!def.NonDefaultBaseClass(type))
      {
        writer.WriteLine("\t\t//So that static methods can load the correct assembly.");
        writer.WriteLine("\t\tprotected static string static_DelegateAssembly = \"" + type.Assembly.GetName().Name + "\";");
      }
    }

    /// <summary>
    /// Write all non-static Fields for this class.  
    /// Since we cannot route accessing fields directly, write all fields properties with both setters
    /// and getters that reflectively reference the corresponding delegate field.
    /// </summary>
    private void WriteFields(StreamWriter writer, ProxyDefinition def, Type type)
    {
      WriteSectionHeading(writer, "Delegate Non-Static Fields (as properties)");

      FieldInfo[] fields = def.PublicFields(type);
      for (int i = 0; i < fields.Length; i++)
      {
        FieldInfo current = fields[i];
        if (!current.IsStatic)
        {
          string fieldType = TypeAsString(current.FieldType);

          string propertySignature = "\t\tpublic " + fieldType + " " + current.Name;
          writer.WriteLine(propertySignature);
          writer.WriteLine("\t\t{");
          writer.WriteLine("\t\t\tget { return (" + fieldType + ") GetField(\"" + current.Name + "\"); }");
          writer.WriteLine("\t\t\tset { SetField(\"" + current.Name + "\", value); }");
          writer.WriteLine("\t\t}");
        }
      }
    }

    #endregion Field Writers

    #region Property Writers

    /// <summary>
    /// Write all the Property methods for this proxy.
    /// If 'asInterface' is true then write properties as if this an interface, otherwise write it as
    /// if this is a class.
    /// </summary>
    private void WriteProperties(StreamWriter writer, ProxyDefinition def, Type type, bool asInterface)
    {
      WriteSectionHeading(writer, "Properties");

      PropertyInfo[] properties = def.Properties(type);
      for (int i = 0; i < properties.Length; i++)
      {
        PropertyInfo current = properties[i];

        string propertySignature = "\t\t";
        if (current.GetAccessors(true).Length > 0) 
          propertySignature += DetermineMethodAttributes(current.GetAccessors(true)[0], def, type, asInterface);

        //                if (!asInterface)
//                {
//                    // this makes all properties public.
////          if (current.GetAccessors().Length > 0)
//                        propertySignature += "public ";
////          else 
////            propertySignature += "protected ";
          
//                    if (PropertyIsStatic(current))
//                        propertySignature += "static ";
//                }
        propertySignature += TypeAsString(current.PropertyType) + " ";

        if (current.GetIndexParameters().Length > 1)
        {
          throw new NotImplementedException("Generating proxies for properties with more than 1 index parameter has not been implemented.");
        }
        else if (current.GetIndexParameters().Length == 1)
        {
          ParameterInfo [] parameters = current.GetIndexParameters();
          propertySignature += "this[" + TypeAsString(parameters[0].ParameterType) + " index" + "]";
        }
        else
          propertySignature += EnsureMethodName(current.Name);

        writer.WriteLine(propertySignature);
        writer.WriteLine("\t\t{");
        WritePropertyGet(writer, current, asInterface);
        WritePropertySet(writer, current, asInterface);
        writer.WriteLine("\t\t}");
      }
    }

    /// <summary>
    /// Return true if property is private.
    /// </summary>
    private bool PropertyIsPrivate(PropertyInfo property)
    {
      return property.GetAccessors(true).Length > 0 && property.GetAccessors(true)[0].IsPrivate;
    }
    private bool ReturnTypeIsPublic(PropertyInfo property)
    {
      return property.PropertyType.IsPublic;
    }
    private bool PropertyIsStatic(PropertyInfo property)
    {
      MethodInfo[] accessors = property.GetAccessors(true);
      return (accessors.Length > 0 && accessors[0].IsStatic);
    }

    /// <summary>
    /// Write 'get' method for given property.
    /// </summary>
    private void WritePropertyGet(StreamWriter writer, PropertyInfo property, bool asInterface)
    {
      if (!property.CanRead)
        return;

      MethodInfo getMethod = property.GetGetMethod(true);
      if (asInterface || getMethod.IsAbstract)
        writer.WriteLine("\t\t\tget;");
      else
      {
        writer.WriteLine("\t\t\tget");
        writer.WriteLine("\t\t\t{");

        if (getMethod.IsStatic)
        {
          writer.WriteLine("\t\t\t\treturn (" + TypeAsString(property.PropertyType) + ")AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), null, Assembly.Load(static_DelegateAssembly));");
        }
        else
        {
          if (property.GetIndexParameters().Length > 0)
            writer.WriteLine("\t\t\t\treturn (" + TypeAsString(property.PropertyType) + ")RouteInstance(MethodBase.GetCurrentMethod(), new object [] { index });");
          else
            writer.WriteLine("\t\t\t\treturn (" + TypeAsString(property.PropertyType) + ")RouteInstance(MethodBase.GetCurrentMethod(), null);");
        }
        writer.WriteLine("\t\t\t}");
      }
    }

    /// <summary>
    /// Write 'set' method for given property.
    /// </summary>
    private void WritePropertySet(StreamWriter writer, PropertyInfo property, bool asInterface)
    {
      if (!property.CanWrite)
        return;

      MethodInfo setMethod = property.GetSetMethod(true);
      if (asInterface || setMethod.IsAbstract)
        writer.WriteLine("\t\t\tset;");
      else
      {
        writer.WriteLine("\t\t\tset");
        writer.WriteLine("\t\t\t{");

        if (setMethod.IsStatic)
        {
          writer.WriteLine("\t\t\t\tAReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), new object[] { value }, Assembly.Load(static_DelegateAssembly));");
        }
        else
        {
          if (property.GetIndexParameters().Length > 0)
            writer.WriteLine("\t\t\t\tRouteInstance(MethodBase.GetCurrentMethod(), new object[] { index, value });");
          else
            writer.WriteLine("\t\t\t\tRouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });");
        }
        writer.WriteLine("\t\t\t}");
      }
    }

    #endregion Property Writers

    #region Method Writers

    /// <summary>
    /// Write all methods (delegate and proxy) for the given ProxyDefinition and type.  If asInterface is
    /// true, write methods as if this were an interface.
    /// </summary>
    /// <param name="asInterface">True, if methods should be written as if an interface, false otherwise.</param>
    private void WriteMethods(StreamWriter writer, ProxyDefinition def, Type type, bool asInterface)
    {
      WriteDelegateMethods(writer, def, type, asInterface);
      
      // Don't write abstract methods if interface.
      if (!asInterface)
        WriteProxyMethods(writer, def, type);
    }

    private void WriteProxyMethods(StreamWriter writer, ProxyDefinition def, Type type)
    {
      WriteSectionHeading(writer, "Proxy Methods");

      // Virtual method which returns the fully qualified type name of this proxy's delegate.
      writer.WriteLine("\t\tpublic override string DelegateClassName()");
      writer.WriteLine("\t\t{");
      writer.WriteLine("\t\t\treturn \"" + type.ToString() + "\";");
      writer.WriteLine("\t\t}");

      // Virtual method which returns the name of the assembly that this proxy's delegate lives in.
      // (e.g. "PresentationFramework, Version=6.0.*.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")
      writer.WriteLine("\t\tprotected override string DelegateAssemblyName()");
      writer.WriteLine("\t\t{");
      if (type.Assembly.Equals(Assembly.GetCallingAssembly()))
        writer.WriteLine("\t\t\treturn null;");
      else
        writer.WriteLine("\t\t\treturn \"" + type.Assembly.GetName().Name + "\";");
      writer.WriteLine("\t\t}");
    }

    /// <summary>
    /// Write all the Methods for this proxy.
    /// </summary>
    private void WriteDelegateMethods(StreamWriter writer, ProxyDefinition def, Type type, bool asInterface)
    {
      WriteSectionHeading(writer, "Delegate Methods");
      WriteMethods(writer, def, type, def.PublicMethods(type), asInterface);
      if (!asInterface)
        WriteMethods(writer, def, type, def.NonPublicMethods(type), false);
    }

    private void WriteMethods(StreamWriter writer, ProxyDefinition def, Type type, MethodInfo[] methods, bool asInterface)
    {
      for (int i = 0; i < methods.Length; i++)
      {
        MethodInfo currentMethod = methods[i];

        // Ignore 'Finalize' methods.
        if (!currentMethod.Name.Equals("Finalize"))
        {
          // Ignore property methods.
          if (!APropertyMethod(currentMethod) && !AnEventMethod(currentMethod))
          {
            string methodSignature = CreateMethodSignature(currentMethod, def, type, asInterface);

            // If class is an interface or the method is abstract, we just write the
            // signature with a trailing semicolon.
            if (asInterface || (!type.IsInterface && currentMethod.IsAbstract))
            {
              writer.WriteLine("\t\t" + methodSignature + ";");
            }
            else
            {
              writer.WriteLine("\t\t" + methodSignature);
              writer.WriteLine("\t\t{");
              writer.Write(CreateMethodBody(currentMethod));
              writer.WriteLine("\t\t}");
            }
          }
        }
      }
    }

    /// <summary>
    /// Return true if this method belongs to a property.
    /// </summary>
    private bool APropertyMethod(MethodInfo method)
    {
      Match match = new Regex("(get_|set_)(.*)").Match(method.Name);
      bool propertyExists = false;
      if (match.Success) 
      {
        try
        {
            propertyExists = ReflectionHelper.FindPropertyInHierarchy(method.DeclaringType, match.Groups[2].Value) != null;
        }
        catch (AmbiguousMatchException)
        {
          propertyExists = true;
        }
      }
      return propertyExists;
    }

    /// <summary>
    /// Return true if this method belongs to an event.
    /// </summary>
    private bool AnEventMethod(MethodInfo method)
    {
      Match match = new Regex("(add_|remove_)(.*)").Match(method.Name);
      if (match.Success)
      {
        return method.DeclaringType.GetEvent(match.Groups[2].Value, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static) != null;
      }
      return false;
    }

    private string DetermineMethodAttributes(MethodInfo info, ProxyDefinition def, Type type, bool asInterface)
    {
      string attributes = "";
      if (!asInterface)
      {

        // Handle special cases first.
        switch (info.Name)
        {
          case ("Equals"):
            return "public override ";
          case ("GetHashCode"):
            return "public override ";
          default:
            break;
        }

        // Add 'static' tag.
        if (info.IsStatic)
          attributes += "static ";

        // Interfaces don't have special signature tags.
        if (!type.IsInterface)
        {
          string proxyBaseTypename = def.BaseClass(type);
          Type declaringType = info.GetBaseDefinition().DeclaringType;

          // If method exists in proxy base class.
          if (proxyBaseTypename.Equals(declaringType.Name))
          {
            // Add 'abstract' tag.
            if (info.IsAbstract || info.IsVirtual)
            {
              attributes += "override ";
            }
            else
            {
              attributes += "new ";
            }
          }
          else if (type.IsAbstract && info.IsAbstract)
          {
            attributes += "abstract ";
          }
          else if (info.IsVirtual)
          {
            attributes += "virtual ";
          }
        }

        // make all methods public.
        //        if (info.IsPublic)
        attributes += "public ";
        //        else
        //          signature += "protected ";
      }

      return attributes;
    }

    /// <summary>
    /// Create a method signature (e.g. "public void FooBar(int value, string msg)");
    /// </summary>
    private string CreateMethodSignature(MethodInfo info, ProxyDefinition def, Type type, bool asInterface)
    {
      string methodName = EnsureMethodName(info.Name);
      string returnType = TypeAsString(info.ReturnType);
      string signature = DetermineMethodAttributes(info, def, type, asInterface);

      signature += returnType + " " + methodName + "(";
      ParameterInfo[] parameters = info.GetParameters();
      for (int i = 0; i < parameters.Length; i++)
      {
        // Handle special parameters: 'out' and 'ref'.
        if (parameters[i].IsOut)
          signature += "out ";
        else if (parameters[i].ParameterType.IsByRef)
          signature += "ref ";

        signature += TypeAsString(parameters[i].ParameterType) + " " + parameters[i].Name;
        if (i < parameters.Length - 1)
          signature += ", ";
      }
      signature += ")";
      return signature;
    }

    /// <summary>
    /// Create a method body that looks like this:
    /// 
    /// public int Foobar(int param1, out int param2, ref int para3) {
    ///   object [] parameters = new object[3];
    ///   object[0] = param1;
    ///   object[1] = param2;
    ///   object[2] = param3;
    ///   int result = (int) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
    ///   param2 = object[1];
    ///   param3 = object[2];
    ///   return result;
    /// }
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    private string CreateMethodBody(MethodInfo info)
    {
      string indent = "\t\t\t";
      ParameterInfo[] parameters = info.GetParameters();
      string inputArray = indent + "object [] parameters = new object[" + parameters.Length + "];\n";
      string outputArray = "";

      for (int i = 0; i < parameters.Length; i++)
      {
        // Compile error to read an outparameter before setting it.
        if (!parameters[i].IsOut)
          inputArray += indent + "parameters[" + i + "] = " + parameters[i].Name + ";\n";

        if (parameters[i].IsOut || parameters[i].ParameterType.IsByRef)
          outputArray += indent + parameters[i].Name + " = " + "(" + TypeAsString(parameters[i].ParameterType) + ") parameters[" + i + "];\n";
      }

      bool isVoid = info.ReturnType.Equals(typeof(void));

      string body = inputArray;

      string routecall;
      if (info.IsStatic)
        routecall = "AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);\n";
      else
        routecall = "RouteInstance(MethodBase.GetCurrentMethod(), parameters);\n";

      if (isVoid)
      {
        body += indent + routecall;
        body += outputArray;
      }
      else
      {
        string returnType = TypeAsString(info.ReturnType);
        body += indent + returnType + " routedResult = (" + returnType + ") " + routecall;
        body += outputArray;
        body += indent + "return routedResult;\n";
      }
      return body;
    }

    #endregion Method Writers

    #region Constructor Writers

    /// <summary>
    /// Write all the constructor methods for this proxy.
    /// </summary>
    private void WriteConstructors(StreamWriter writer, string classname, ConstructorInfo [] constructors)
    {
      WriteDelegateConstructors(writer, classname, constructors);
      WriteProxyConstructors(writer, classname);
    }

    /// <summary>
    /// Write all the constructors that are specific to the Proxy class.
    /// </summary>
    private void WriteProxyConstructors(StreamWriter writer, string classname)
    {
      WriteSectionHeading(writer, "Proxy Constructors");
      writer.WriteLine("\t\tstatic " + classname + "() { InitializeStaticFields(static_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }");
      writer.WriteLine("\t\tprotected " + classname + "(Type[] types, object[] values) : base (types, values) { }");
      writer.WriteLine("\t\tprotected " + classname + "(object delegateObject) : base (delegateObject) { }");
    }

    /// <summary>
    /// Write all the constructors that are proxies for a delegate (e.g. that exist in the
    /// delegate class).
    /// </summary>
    private void WriteDelegateConstructors(StreamWriter writer, string classname, ConstructorInfo[] constructors)
    {
      bool wroteDefaultCtor = false;
      WriteSectionHeading(writer, "Delegate Constructors");
      for (int n = 0; n < constructors.Length; n++)
      {
        ConstructorInfo current = constructors[n];

        // Write at most 1 default constructor.
        if (current.GetParameters().Length > 0 || !wroteDefaultCtor)
        {
          string signature = "";
          string parameterTypeList = "";
          string parameterList = "";
          ParameterInfo[] parameters = current.GetParameters();
          for (int i = 0; i < parameters.Length; i++)
          {
            string parameterType = TypeAsString(parameters[i].ParameterType);
            signature += parameterType + " " + parameters[i].Name;
            parameterTypeList += "typeof(" + parameterType + ")";
            parameterList += parameters[i].Name;
            if (i < parameters.Length - 1)
            {
              signature += ", ";
              parameterTypeList += ", ";
              parameterList += ", ";
            }
          }

          writer.WriteLine("\t\tpublic " + classname + "(" + signature + ")");
          if (parameters.Length > 0)
            writer.WriteLine("\t\t: base (new Type[] { " + parameterTypeList + " }, new object[] { " + parameterList + " })");
          else
            writer.WriteLine("\t\t: base (Type.EmptyTypes, new object[0])");
          writer.WriteLine("\t\t{");
          writer.WriteLine("\t\t\t//Empty.");
          writer.WriteLine("\t\t}");
        }

        if (current.GetParameters().Length == 0)
          wroteDefaultCtor = true;
      }
    }

    #endregion Constructor Writers

    #region Event Writers

    /// <summary>
    /// Write out an event proxy consisting of an "add" and "remove" method for each event.
    /// </summary>
    private void WriteEvents(StreamWriter writer, ProxyDefinition def, Type type)
    {
      WriteSectionHeading(writer, "Events");
      EventInfo [] events = def.Events(type);
      for (int i = 0; i < events.Length; i++)
      {
        Type handlerType = events[i].EventHandlerType;

        // Write a new event "Proxy_" event that we will use to register with the delegate.
        // When the delegate event fires this event will convert the args from the delegate type,
        // and then route them to the proxy event handler.
        MethodInfo invokeMethod = handlerType.GetMethod("Invoke");
        if (invokeMethod == null)
          throw new AnnotationProxyException("Did not find 'Invoke' method for event handler type '" + handlerType.FullName + "'.");
        ParameterInfo[] parameters = invokeMethod.GetParameters();
        string argType = parameters[1].ParameterType.Name;

        string proxyEventName = "Proxy_" + events[i].Name;
        writer.WriteLine("\t\tprivate event " + handlerType.Name + " " + proxyEventName + ";");
        string proxyEventHandlerName = "Proxy_On" + events[i].Name;
        writer.WriteLine("\t\tprivate void " + proxyEventHandlerName + "(object sender, " + argType + " args)");
        writer.WriteLine("\t\t{");
        writer.WriteLine("\t\t\tif (" + proxyEventName + " != null)");
        writer.WriteLine("\t\t\t\t" + proxyEventName + "(sender, (" + argType + ")ProxyTypeConverter.WrapObject(null, args));");
        writer.WriteLine("\t\t}");

        // Write the Proxy version of the delegate event.
        writer.WriteLine("\t\tpublic event " + handlerType.Name + " " + events[i].Name);
        writer.WriteLine("\t\t{");

        writer.WriteLine("\t\t\tadd");
        writer.WriteLine("\t\t\t{");
        writer.WriteLine("\t\t\t\t" + proxyEventName + " += value;");
        writer.WriteLine("\t\t\t\tRouteEventMethod(MethodBase.GetCurrentMethod(), new " + handlerType.Name + "(" + proxyEventHandlerName + "));");
        writer.WriteLine("\t\t\t}");

        writer.WriteLine("\t\t\tremove");
        writer.WriteLine("\t\t\t{");
        writer.WriteLine("\t\t\t\t" + proxyEventName + " -= value;");
        writer.WriteLine("\t\t\t\tRouteEventMethod(MethodBase.GetCurrentMethod(), new " + handlerType.Name + "(" + proxyEventHandlerName + "));");
        writer.WriteLine("\t\t\t}");

        writer.WriteLine("\t\t}");
      }
    }

    #endregion Event Writers

    /// <summary>
    /// If method contains interface prefix, remove the prefix.
    /// </summary>
    /// <param name="methodName"></param>
    /// <returns></returns>
    private string EnsureMethodName(string methodName)
    {
      if (methodName.Contains("."))
        return methodName.Substring(methodName.LastIndexOf('.') + 1);
      return methodName;
    }

    /// <summary>
    /// Objects of type 'delegate' are special because they are essentially compiled into
    /// full classes.  We can't use reflection to generate a full descriptor of one so we
    /// just memorize the exact definition and then put it into the correct proxy namespace.
    /// </summary>
    private void WriteDelegates(StreamWriter writer, ProxyDefinition def)
    {
      IEnumerator<string> delegates = def.Delegates.GetEnumerator();
      while (delegates.MoveNext())
      {
        writer.WriteLine(delegates.Current);
      }
    }

    /// <summary>
    /// Some Type names need to be modified slightly in order to by syntactically correct,
    /// this method makes sure that they are.
    /// </summary>
    /// <returns>Syntactically correct Type as a string.</returns>
    private string TypeAsString(Type type)
    {
      string typename = type.Name;
      // Convert 'Void' to 'void'.
      if (type.Equals(typeof(void)))
        typename = "void";
      // Remove '&' from end of ref and out parameters.
      if (typename.EndsWith("&"))
        typename = typename.Substring(0, typename.Length - 1);
      // Handle generic collections.
      else if (type.IsGenericType)
      {
        typename = new Regex("(\\w+).*").Match(type.Name).Groups[1] + "<";
        Type[] args = type.GetGenericArguments();
        for (int i = 0; i < args.Length; i++)
        {
          // Could be nested generics.
          typename += TypeAsString(args[i]);
          typename += (i == args.Length - 1) ? ">" : ", ";
        }
      }
      return typename;
    }


    #endregion Private Methods

    //------------------------------------------------------
    //
    //  Public Members
    //
    //------------------------------------------------------

    #region Public Members

    /// <summary>
    /// Namespaces that AReflectiveProxy's depend upon.
    /// </summary>
    public string[] proxyImports = new string[] { 
                              "Annotations.Test.Reflection",
                              "System.Reflection",
                              "System.Windows",
                              "System.Windows.Controls",
                              "System"
                             };

    /// <summary>
    /// Blob of text that will written immediately after the standard imports.  
    /// Intended purpose is to add import alias overloads for non-proxy classes.
    /// </summary>
    public string ImportOverloads = null;

    #endregion protected Members
  }
}


