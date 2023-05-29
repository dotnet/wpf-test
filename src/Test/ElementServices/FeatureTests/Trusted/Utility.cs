// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Common routines useful across many feature areas or frameworks.
 *
 
  
 * Revision:         $Revision: 3 $
 
********************************************************************/
using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Test.Logging;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// Utility functions used by all of Core
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Returns the value of the %temp% environment variable.
        /// </summary>
        public static string TempDir
        {
            get
            {
                System.Security.Permissions.EnvironmentPermission envPermission = new System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.PermissionState.Unrestricted);
                envPermission.Assert();
                
                string tempDir = Environment.ExpandEnvironmentVariables("%temp%");

                System.Security.CodeAccessPermission.RevertAssert();

                return tempDir;
            }
        }
        /// <summary>
        /// Parse a string containing multiple pairs of command arguments.
        /// </summary>
        /// <param name="args">String of params delimited by semicolons.</param>
        /// <returns>A sorted list, where the name is the key to the corresponding value.</returns>
        /// <remarks>
        /// Each argument string should have a "[name]=[value]" format.  The names and values are converted to lower case.
        /// </remarks>
        public static SortedList ParseArgs(string args)
        {
            string arg = args.Insert(0, ";").Replace(";", " /").Replace(':','=').Trim();
            System.Console.WriteLine(arg);
            return Utility.ParseArgs(arg.Split(' '), false);
        }
        
        /// <summary>
        /// Parse a string containing multiple pairs of command arguments.
        /// </summary>
        /// <remarks>
        /// Each argument string should have a "/[name]=[value]" format.  The names and values are converted to lower case.
        /// </remarks>
        /// <param name="arg">Array of strings representing command arguments.</param>
        /// <param name="keepCase">Array of strings representing command arguments.</param>
        /// <returns>A sorted list, where the name is the key to the corresponding value.</returns>
        public static SortedList ParseFromStringToStringArray(string arg, bool keepCase)
        {
            Regex commaSep = new Regex("\\s+");

            string[] args = commaSep.Split(arg);
            return ParseArgs(args,keepCase);
        }
        
        
        /// <summary>
        /// Parse a string containing multiple pairs of command arguments.
        /// </summary>
                /// <param name="arg">Array of strings representing command arguments.</param>
                /// <returns>A sorted list, where the name is the key to the corresponding value.</returns>
                /// <exception cref="ApplicationException">Thrown when an input string has the incorrect format.</exception>
                /// <remarks>
                /// Each argument string should have a "/[name]=[value]" format.  The names and values are converted to lower case.
                /// </remarks>
        public static string[] ParseArgsSpaceString(string arg)
        {
            Regex commaSep = new Regex("\\s+");
        
            string[] args = commaSep.Split(arg);
            
            return args;
        }
        
        /// <summary>
        /// Parse a string containing multiple pairs of command arguments.
        /// </summary>
        /// <param name="args">Array of strings representing command arguments.</param>
        /// <returns>A sorted list, where the name is the key to the corresponding value.</returns>
        /// <exception cref="ApplicationException">Thrown when an input string has the incorrect format.</exception>
        /// <remarks>
        /// Each argument string should have a "/[name]=[value]" format.  The names and values are converted to lower case.
        /// </remarks>
        public static SortedList ParseArgs(string[] args)
        {
            return Utility.ParseArgs(args, false);
        }
        
        /// <summary>
        /// Parse a string containing multiple pairs of command arguments.
        /// </summary>
        /// <param name="args">Array of strings representing command arguments.</param>
        /// <param name="keepCase">Specifies whether or not to keep the existing character case.</param>
        /// <returns>A sorted list, where the name is the key to the corresponding value.</returns>
        /// <exception cref="ApplicationException">Thrown when an input string has the incorrect format.</exception>
        /// <remarks>
        /// Each argument string should have a "/[name]=[value]" format.  If keepCase is false, the names and values are converted to lower case.
        /// </remarks>
        public static SortedList ParseArgs(string[] args, bool keepCase)
        {
            return ParseArgs (args, keepCase, keepCase);
        } // end ParseArgs
        
        /// <summary>
        /// Parse a string containing multiple pairs of command arguments with format "/[name]=[value]".
        /// </summary>
        /// <param name="args">Array of strings representing command arguments.</param>
        /// <param name="keepSwitchCase">Specifies whether or not to keep the existing character case for the switch before the "=".</param>
        /// <param name="keepValueCase">Specifies whether or not to keep the existing character case for the value after the "=".</param>
        /// <returns>A sorted list, where the name is the key to the corresponding value.</returns>
        /// <exception cref="ApplicationException">Thrown when an input string has the incorrect format.</exception>
        /// <remarks>
        /// Each argument string should have a "/[name]=[value]" format.
        /// </remarks>
        public static SortedList ParseArgs(string[] args, bool keepSwitchCase, bool keepValueCase)
        {
            return ParseArgs(args, keepSwitchCase, keepValueCase, "/");
        }

        /// <summary>
        /// Parse a string containing multiple pairs of command arguments with format "[delimiter][name]=[value]".
        /// </summary>
        /// <param name="args">Array of strings representing command arguments.</param>
        /// <param name="keepSwitchCase">Specifies whether or not to keep the existing character case for the switch before the "=".</param>
        /// <param name="keepValueCase">Specifies whether or not to keep the existing character case for the value after the "=".</param>
        /// <param name="delimiter"></param>
        /// <returns>A sorted list, where the name is the key to the corresponding value. null if parsing failed for any reason.</returns>
        /// <remarks>
        /// Each argument string should have a "[delimiter][name]=[value]" format. The delimiter can be an empty string.
        /// </remarks>
        public static SortedList ParseArgs(string[] args, bool keepSwitchCase, bool keepValueCase, string delimiter) 
        {
            string[] arg = null;
            SortedList argList = new SortedList();

            for(int i = 0; i < args.Length; i++)
            {
                if (args[i].IndexOf(delimiter) != 0)
                {
                    WriteLineWithColor("Parsing Error: Command argument '" + args[i] + "' is not in the correct format: " + delimiter + "<name>=<value>.", ConsoleColor.Red);
                    return null;
                }

                arg = args[i].Remove(0, delimiter.Length).Split('=');
                //If One "=" is provided, It cannot have null value after =.
                if (arg.Length == 2 && arg[1].Length == 0)
                {
                    WriteLineWithColor("Parsing Error: Command argument '" + args[i] + "' is not in the correct format: " + delimiter + "<name>=<value>.", ConsoleColor.Red);
                    return null;
                }

                string val = "";
                for (int j = 1; j < arg.Length; j++)
                {
                    val += arg[j] + "=";
                }

                val = val.TrimEnd('=');

                if (!keepValueCase)
                    val = val.ToLowerInvariant();

                string key = "";
                if (keepSwitchCase)
                {
                    key = arg[0];
                }
                else
                {
                    key = arg[0].ToLowerInvariant();
                }
                //Check for duplicate key
                if (argList.Contains(key))
                {
                    Console.WriteLine("Parsing Error: Duplicate key detected - " + key);
                    return null;
                }
                else
                {
                    argList.Add(key, val);
                }
            }

            //Supports some commonly-used shortcut keys
            //Only do that when keepSwitchCase is false (which is what CoreTests chooses)
            if (!keepSwitchCase)
            {
                CheckAndAddShortcut(argList, "xaml", "xamlfile");
                CheckAndAddShortcut(argList, "action", "actionforxaml");
            }

            return argList;
        }

        private static void CheckAndAddShortcut(SortedList argList, string shortcutKey, string fullKey)
        {
            if (!argList.Contains(fullKey) && argList.Contains(shortcutKey))
            {
                argList.Add(fullKey, argList[shortcutKey]);
            }
        }

        /// <summary>
        /// Starts an executable with command line parameters.
        /// </summary>
        /// <param name="processRelPath"></param>
        /// <param name="cmdLine"></param>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        public static int RunWin32Process(string processRelPath, string cmdLine, string workingDirectory)
        {
            CoreLogger.LogStatus("Entered RunWin32Process()...");

            Process winProcess = null;
            int retCode = -1;
            
            try
            {
                winProcess = new Process();
                ProcessStartInfo winProcessStartInfo = new ProcessStartInfo();

                winProcessStartInfo.FileName = processRelPath;
                winProcessStartInfo.Arguments = cmdLine;
                    
                winProcessStartInfo.UseShellExecute = false;
                winProcessStartInfo.RedirectStandardOutput = true;

                winProcessStartInfo.WorkingDirectory = workingDirectory;

                winProcess.StartInfo = winProcessStartInfo;

                System.Security.Permissions.SecurityPermission permission = new System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode);
                permission.Assert();

                try
                {
                    winProcess.Start();

                    string standardOutput = winProcess.StandardOutput.ReadToEnd();

                    CoreLogger.LogStatus(standardOutput);

                    winProcess.WaitForExit();

                    retCode = winProcess.ExitCode;
                }
                finally
                {
                    System.Security.CodeAccessPermission.RevertAssert();
                }
            }
            finally
            {
                if (winProcess != null)
                    winProcess.Dispose();
            }

            return retCode;

        } // end RunWin32Process

        /// <summary>
        /// Output message in color
        /// </summary>
        /// <param name="message">The message to output</param>
        /// <param name="color">The color in which the message will be displayed in output</param>
        public static void WriteLineWithColor(string message, ConsoleColor color)
        {
            ConsoleColor savedForgroundColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = savedForgroundColor;
        }

        /// <summary>
        /// Returns all public types from the default CoreUI test assemblies.
        /// </summary>
        /// <returns>A dictionary of Type instances indexed by unqualified type names.</returns>
        public static Dictionary<string, Type> CoreTestTypes
        {
            get
            {
                return _LoadAllTypes();
            }
        }

        /// <summary>
        /// Searches an assembly for a type. 
        /// </summary>
        /// <param name="typeName">The search name.</param>
        /// <param name="assemblyName">The assembly to search.</param>
        /// <param name="searchFullName">Whether or not to match the full name of the type, including the namespace.</param>
        /// <returns>The specified type.</returns>
        public static Type FindType(string typeName, string assemblyName, bool searchFullName)
        {
            Type foundType = null;

            // Look for the type by searching all types in the given assembly.
            List<Type> types = GetAssemblyTypes(assemblyName);
            for (int i = 0; i < types.Count; i++)
            {
                Type type = types[i];

                if ((searchFullName && type.FullName.Equals(typeName)) ||
                    type.Name.Equals(typeName))
                {
                    foundType = type;
                    break;
                }
            }

            // Throw exception if we couldn't find the type.
            if (foundType == null)
            {
                throw new Microsoft.Test.TestSetupException("Count not find type '" + typeName + "' from '" + assemblyName + "'.");
            }

            return foundType;
        }

        /// <summary>
        /// Searches the default CoreUI test assemblies for a type.
        /// </summary>
        /// <param name="typeName">The search name.</param>
        /// <param name="searchFullName">Whether or not to match the full name of the type, including the namespace.</param>
        /// <returns>The specified type.</returns>
        public static Type FindType(string typeName, bool searchFullName)
        {
            List<string> typeNames = new List<string>();
            typeNames.Add(typeName);

            return (Utility.FindTypes(typeNames, searchFullName))[typeName];
        }

        /// <summary>
        /// Searches the default CoreUI test assemblies for a list of types.
        /// </summary>
        /// <param name="typeNames">The search names.</param>
        /// <param name="searchFullName">Whether or not to match the full name of the types, including the namespaces.</param>
        /// <returns>The specified types.</returns>
        public static Dictionary<string, Type> FindTypes(List<string> typeNames, bool searchFullName)
        {
            Dictionary<string, Type> types = Utility.CoreTestTypes;
            Dictionary<string, Type> foundTypes = new Dictionary<string, Type>();

            // For each type, check if it's in the search list.
            foreach (Type type in types.Values)
            {
                for (int i = 0; i < typeNames.Count; i++ )
                {
                    string typeName = typeNames[i];

                    if ((searchFullName && type.FullName.Equals(typeName)) ||
                        type.Name.Equals(typeName))
                    {
                        foundTypes.Add(typeName, type);
                        typeNames.RemoveAt(i);
                    }
                }

                if (typeNames.Count == 0)
                    break;
            }

            // Throw exception if we couldn't find some of the types.
            if (typeNames.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("Could not find the following types:");
                foreach (string typeName in typeNames)
                {
                    builder.AppendLine();
                    builder.Append("  Type: ");
                    builder.Append(typeName);
                }
                builder.AppendLine();
                throw new Microsoft.Test.TestSetupException(builder.ToString());
            }

            return foundTypes;
        }

        /// <summary>
        /// Searches the given list of Types for a single type by name.
        /// </summary>
        /// <param name="types">List of types to search.</param>
        /// <param name="typeName">The search name.</param>
        /// <param name="searchFullName">Whether or not to match the full name of the types, including the namespaces.</param>
        /// <returns>The specified type.</returns>
        public static Type FindType(List<Type> types, string typeName, bool searchFullName)
        {
            List<string> typeNames = new List<string>();
            typeNames.Add(typeName);

            Dictionary<string, Type> foundTypes = FindTypes(types, typeNames, searchFullName);

            return foundTypes[typeName];
        }

        /// <summary>
        /// Searches the given list of Types for a list of types by name.
        /// </summary>
        /// <param name="types">List of types to search.</param>
        /// <param name="typeNames">The search names.</param>
        /// <param name="searchFullName">Whether or not to match the full name of the types, including the namespaces.</param>
        /// <returns>The specified types.</returns>
        public static Dictionary<string, Type> FindTypes(List<Type> types, List<string> typeNames, bool searchFullName)
        {
            Dictionary<string, Type> foundTypes = new Dictionary<string, Type>();

            // For each type, check if it's in the search list.
            foreach (Type type in types)
            {
                for (int i = 0; i < typeNames.Count; i++)
                {
                    string typeName = typeNames[i];

                    if ((searchFullName && type.FullName.Equals(typeName)) ||
                        type.Name.Equals(typeName))
                    {
                        foundTypes.Add(typeName, type);
                        typeNames.RemoveAt(i);
                    }
                }

                if (typeNames.Count == 0)
                    break;
            }

            // Throw exception if we couldn't find some of the types.
            if (typeNames.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("Count not find some types.");
                foreach (string typeName in typeNames)
                {
                    builder.AppendLine();
                    builder.Append("  Type: ");
                    builder.Append(typeName);
                }

                throw new Microsoft.Test.TestSetupException(builder.ToString());
            }

            return foundTypes;
        }

        // Loads all types from the default CoreUI test assemblies.
        private static Dictionary<string, Type> _LoadAllTypes()
        {
            lock (s_lockObj)
            {
                if (s_typeCache != null)
                    return s_typeCache;

                s_typeCache = new Dictionary<string, Type>();

                string[] assemblyList = CoreTests.DefaultAssemblyList.Split(',');

                //
                // Loop through the default assembly list.
                // Add all types to the cache.
                //
                for (int n = 0; n < assemblyList.Length; n++)
                {
                    // Get the assembly types. 
                    List<Type> types = GetAssemblyTypes(assemblyList[n]);

                    // For each type, check if it's in the search list.
                    for (int j = 0; j < types.Count; j++)
                    {
                        Type type = types[j];

                        if (type == null)
                            continue;

                        string typeName = type.Name;

                        if (!s_typeCache.ContainsKey(type.FullName))
                        {
                            //Add the type only if it isn't already there (it may have been added from a previous assembly).
                            s_typeCache.Add(type.FullName, type);
                        }
                    }
                }
            }

            return s_typeCache;
        }

        
        /// <summary>
        /// Gets the given assembly's types. 
        /// Catches ReflectionTypeLoadException's, and returns any types that didn't cause an exception.
        /// That is necessary because an InheritanceDemand on some Avalon types which are overridden on 
        /// our test types causes a security exception for the types that are in partial trust assemblies.
        /// </summary>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
        public static List<Type> GetAssemblyTypes(Assembly assembly)
        {
            Type[] types = null;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }

            List<Type> typeList = new List<Type>();
            typeList.AddRange(types);

            return typeList;
        }

        /// <summary>
        /// Gets the given assembly's types. 
        /// Catches ReflectionTypeLoadException's, and returns any types that didn't cause an exception.
        /// That is necessary because an InheritanceDemand on some Avalon types which are overridden on 
        /// our test types causes a security exception for the types that are in partial trust assemblies.
        /// </summary>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
        public static List<Type> GetAssemblyTypes(string assemblyName)
        {
            string execDir = DriverState.ExecutionDirectory;
            Assembly assembly = Assembly.LoadFrom(Path.Combine(execDir, assemblyName));
            return GetAssemblyTypes(assembly);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attribute"></param>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
        public static void AddAttributeToTypeDescriptor(Type type, Attribute attribute)
        {
            System.ComponentModel.TypeDescriptor.AddAttributes(type, new Attribute[] { attribute });
        }

        private static object s_lockObj = new object();
        private static Dictionary<string, Type> s_typeCache = null;
    } // end Utility
}
