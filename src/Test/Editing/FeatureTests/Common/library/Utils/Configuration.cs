// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides configuration services for test cases.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Xml;
using Microsoft.Test.Logging;
using System.Collections.Generic;
using System.Windows.Input;

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Utils/Configuration.cs $")]

namespace Test.Uis.Utils
{
    /// <summary>Configuration services for test cases.</summary>
    /// <remarks>
    /// <p>
    /// This class implements the providing configuration of
    /// information to test cases. There are two sources of argument
    /// values: command-line arguments and an XML file.
    /// </p><p>
    /// Command line arguments are a list of name-value pairs,
    /// separated by a color and optionally starting with a slash,
    /// one dash, or two dashes.
    /// </p><p>
    /// The XML file is loaded from a name passed in the
    /// command line with argument name "xml". This argument
    /// is optional. The XML file should have element-text pairs
    /// for name-value pairs in the top-level element. The name
    /// of the top-level element is currently ignored. If not xml
    /// name exists and a file named testxml.xml exists in the
    /// current assembly directory, that file is used instead.
    /// </p><p>
    /// When an argument is searched, first the command
    /// line arguments are searched, then the XML elements,
    /// and finally everything is re-searched with the test name and a dash
    /// prepended. The first match found wins. If no match is found
    /// null is returned, unless the caller requests that an exception
    /// be raised.
    /// </p><p>
    /// AutoData integration is provided by prefixing a setting value
    /// with the four characters '!AD:'. The rest of the string is
    /// used to configure how data is retrieved from AutoData and has the
    /// following format: index=index [;length=length] [;max=maxlength]
    /// [;min=minlength] <br />
    /// index is the index in ProblemString.xml; use -1 for random <br />
    /// length (optional) is the length of string to return <br />
    /// maxlength (optional) is the maximum length for a returned string <br />
    /// minlength (optional) is the minimum length for a returned string <br />
    /// </p></remarks>
    /// <example>The following sample shows how to create a fully
    /// setup Configuration object and use it.<code>...
    /// public ConfigurationSettings GetConfig(string[] args, AutomationFramework fw) {
    ///   ConfigurationSettings config = new ConfigurationSettings(args);
    ///   config.AutomationFramework = fw;
    ///   return config;
    /// }
    /// ...
    /// public void EntryPoint(string[] args, AutomationFramework fw) {
    ///   ConfigurationSettings config = GetConfig(args, fw);
    ///   System.Console.WriteLine(config.GetArgument("TestCaseName");
    /// }</code></example>
    public class ConfigurationSettings
    {
        #region Constructors.

        /// <summary>Creates a new Configuration instance.</summary>
        /// <param name="arguments">
        /// Command-line arguments to use, possibly null.
        /// </param>
        /// <remarks>
        /// The configuration services provides logging information
        /// through the Logger singleton.
        /// </remarks>
        public ConfigurationSettings(string[] arguments)
        {
            //
            // All arguments end up in the _arguments variable. The only
            // extra copy is that of _commandLineArguments that holds
            // commandline-only settings.
            //
            _arguments = new Hashtable();

            _commandLineArguments = (arguments == null) ? new string[0] : arguments;
            BuildArguments(_commandLineArguments);

            //
            // Test name may be null when initializing from command line
            // and arguments will come from a different process.
            //
            string testName = GetTestName();
            if (testName != String.Empty)
            {
                _testNamePrefix = testName + "-";
                BuildXmlArguments(testName);
            }
            else
            {
                _testNamePrefix = String.Empty;
            }

            Current = this;
        }

        /// <summary>Creates a deep copy of the configuration settings.</summary>
        /// <param name="settings">Existing settings.</param>
        /// <remarks>
        /// The new instance does not replace the existing instance in the
        /// Current static variable.
        /// </remarks>
        public ConfigurationSettings(ConfigurationSettings settings)
        {
            _arguments = (Hashtable)settings._arguments.Clone();
            _commandLineArguments = (string[])settings._commandLineArguments.Clone();
            _testLog = settings.TestLog;
            _testNamePrefix = settings._testNamePrefix;
        }

        /// <summary>
        /// Creates a configuration settings from a hash table of
        /// argument name/argument value pairs.
        /// </summary>
        /// <param name="settings">Existing settings.</param>
        /// <remarks>
        /// The new instance does not replace the existing instance in the
        /// Current static variable.
        /// </remarks>
        public ConfigurationSettings(Hashtable settings)
        {
            _commandLineArguments = new string[0];
            _arguments = (Hashtable)settings.Clone();
            _testNamePrefix = GetTestName() + "-";
        }

        #endregion Constructors.

        
        #region Public methods.

        /// <summary>
        /// Clones all values in the configuration settings into
        /// a Hashtable.
        /// </summary>
        /// <returns>Hashtable with all argument name/value pairs.</returns>
        /// <remarks>AutomationFramework values are not returned.</remarks>
        public Hashtable CloneValues()
        {
            return (Hashtable)_arguments.Clone();
        }

        /// <summary>
        /// Attempts to retrieve arguments to the test case from an
        /// XML file.
        /// </summary>
        public void BuildXmlArguments()
        {
            BuildXmlArguments(GetTestName());
        }

        /// <summary>
        /// Attempts to retrieve arguments to the test case from an
        /// XML file.
        /// </summary>
        public void BuildXmlArguments(string testName)
        {
            PermissionSet perms = new PermissionSet(PermissionState.Unrestricted);
            perms.AddPermission(
                new SecurityPermission(PermissionState.Unrestricted));
            perms.AddPermission(
                new FileIOPermission(PermissionState.Unrestricted));
            perms.Assert();

            if (testName == null)
            {
                throw new ArgumentNullException("testName");
            }
            if (testName == "")
            {
                Log("Test name is blank - XML configuration will not be used.");
                return;
            }

            string xmlFileName = GetArgument("xml");
            if (xmlFileName == String.Empty)
            {
                string msg =
                    "Configuration service found no explicit XML file." +
                    Environment.NewLine;

                string defaultName = AppDomain.CurrentDomain.BaseDirectory;
                defaultName = System.IO.Path.Combine(defaultName, "testxml.xml");
                if (System.IO.File.Exists(defaultName))
                {
                    xmlFileName = defaultName;
                    msg += "Configuration service defaulting to XML in " +
                        defaultName;
                }
                else
                {
                    msg += "No XML file will be used.";
                }
            }

            if (xmlFileName != String.Empty)
            {
                //File.Exists() works for either absolute or relative path
                if (!File.Exists(xmlFileName))
                {
                    //Construct a absolute file path in the work folder used by Avalon Test. 
                    //the work folder is under the root of the system Directory.
                    xmlFileName = Path.Combine("work", xmlFileName);
                    xmlFileName = Path.Combine(Path.GetPathRoot(System.Environment.SystemDirectory), xmlFileName);
                }
                BuildXmlArgumentsFromFile(xmlFileName, testName);
            }
        }

        /// <summary>Processes XML arguments from a file.</summary>
        /// <param name="fileName">XML file name.</param>
        /// <param name="testName">Name of test case.</param>
        public void BuildXmlArgumentsFromFile(string fileName, string testName)
        {
            XmlTestConfiguration xmlTestConfiguration =
                new XmlTestConfiguration(fileName, testName);
            xmlTestConfiguration.AddSettingsTo(_arguments);
        }

        /// <summary>Retrieves a named configuration value.</summary>
        /// <param name="argumentName">The name of the configuration item.</param>
        /// <returns>The value of the configuration item, an empty string if not found.</returns>
        /// <example>The following example shows how to use this method.
        /// <code>
        /// string name = configuration.GetArgument("MyName");
        /// if (name != String.Empty) {
        ///     System.Console.WriteLine("My name is " + name);
        /// }
        /// </code></example>
        public string GetArgument(string argumentName)
        {
            if (argumentName == null)
                throw new ArgumentNullException("argumentName");
            return GetArgument(argumentName, false);
        }

        /// <summary>Retrieves a named configuration value.</summary>
        /// <param name="argumentName">The name of the configuration item.</param>
        /// <param name="failIfMissing">Whether to throw an exception if the value is missing.</param>
        /// <returns>The value of the configuration item, an empty string if not found.</returns>
        /// <example>The following example shows how to use this method.
        /// <code>
        /// string name = configuration.GetArgument("MyName", true);
        /// // If the case argument is not found, then the next line will
        /// // never be executed.
        /// System.Console.WriteLine("My name is " + name);
        /// </code></example>
        public string GetArgument(string argumentName, bool failIfMissing)
        {
            string result;
            bool hasArgument = HasArgument(argumentName, out result);
            if (!hasArgument)
            {
                if (failIfMissing)
                    throw new MissingValueException(argumentName);
                result = String.Empty;
            }
            return result;
        }

        /// <summary>Retrieves a named configuration value as a boolean value.</summary>
        /// <param name="argumentName">The name of the configuration item.</param>
        /// <returns>The value of the configuration item, false if not found.</returns>
        public bool GetArgumentAsBool(string argumentName)
        {
            string str = GetArgument(argumentName, false);
            return str == null || !(str.ToLower() == "true") ? false : true;
        }

        /// <summary>Retrieves a named configuration value as a boolean value.</summary>
        /// <param name="argumentName">The name of the configuration item.</param>
        /// <param name="failIfMissing">Whether to throw an exception if the value is missing.</param>
        /// <returns>The value of the configuration item. failIfMissing determines
        /// whether false is returned if not found or if an exception
        /// is raised.</returns>
        public bool GetArgumentAsBool(string argumentName, bool failIfMissing)
        {
            string str = GetArgument(argumentName, failIfMissing);
            return str == null || !(str.ToLower() == "true") ? false : true;
        }

        /// <summary>Retrieves a named configuration value as an integer.</summary>
        /// <param name="argumentName">The name of the configuration item.</param>
        /// <returns>The value of the configuration item, a zero if not found.</returns>
        public int GetArgumentAsInt(string argumentName)
        {
            return GetArgumentAsInt(argumentName, false);
        }

        /// <summary>Retrieves a named configuration value as an integer.</summary>
        /// <param name="argumentName">The name of the configuration item.</param>
        /// <param name="failIfMissing">Whether to throw an exception if the value is missing.</param>
        /// <returns>
        /// The value of the configuration item. failIfMissing determines
        /// whether a zero is returned if not found or if an exception
        /// is raised. The invariant culture is used for the number format.
        /// for details.
        /// </returns>
        public int GetArgumentAsInt(string argumentName, bool failIfMissing)
        {
            string str;
            int result;

            result = 0;

            // .GetArgument takes care of handling failIfMissing cases.
            str = GetArgument(argumentName, failIfMissing);
            if (str != null && str.Length > 0)
            {
                switch (str)
                {
                    case "Int32.MinValue":
                        result = Int32.MinValue;
                        break;
                    case "Int32.MaxValue":
                        result = Int32.MaxValue;
                        break;
                    default:
                        try
                        {
                            IFormatProvider provider = System.Globalization.NumberFormatInfo.InvariantInfo;
                            result = Int32.Parse(str, provider);
                        }
                        catch
                        {
                            Log("Warning: value {0} ({1}) can't be converted to int.", argumentName, str);
                        }
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a string array containing the command line arguments for the 
        /// current process.
        /// </summary>
        /// <returns>
        /// An array of string where each element contains a command line 
        /// argument. The first element is the executable file name, and the 
        /// following zero or more elements contain the remaining command line 
        /// arguments.
        /// </returns>
        /// <remarks>
        /// The first element in the array contains the file name of the 
        /// executing program. If the file name is not available, the first 
        /// element is equal to String.Empty. The remaining elements contain 
        /// any additional tokens entered on the command line. The program 
        /// file name can, but is not required to, include path information.
        /// </remarks>
        public static string[] GetEnvironmentCommandLineArgs()
        {
            new System.Security.Permissions.EnvironmentPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return System.Environment.GetCommandLineArgs();
        }

        /// <summary>
        /// Retrieves an XmlReader that moves over the XML configuration block.
        /// </summary>
        /// <returns>A new XmlReader instance.</returns>
        /// <remarks>
        /// The top-level node should be ignored.
        /// </remarks>
        public XmlReader GetXmlBlockReader()
        {
            string block = GetArgument(XmlTestConfiguration.XmlBlockArgumentName);
            if (block == String.Empty)
            {
                block = "<empty />";
            }
            return new XmlTextReader(new StringReader(block));
        }

        /// <summary>Checks whether a configuration item is defined.</summary>
        /// <param name='argumentName'>Configuration item to check for.</param>
        /// <returns>true if the argument is defined, false otherwise.</returns>
        public bool HasArgument(string argumentName)
        {
            string result;
            return HasArgument(argumentName, out result);
        }

        /// <summary>
        /// Checks whether a configuration item is defined and returns
        /// the value, if any.
        /// </summary>
        /// <param name='argumentName'>Configuration item to check for.</param>
        /// <param name='argumentValue'>
        /// On return, the value of the item, null if missing.
        /// </param>
        /// <returns>true if the argument is defined, false otherwise.</returns>
        public bool HasArgument(string argumentName, out string argumentValue)
        {
            if (argumentName == null)
                throw new ArgumentNullException("argumentName");
            object val = InternalGetArgument(argumentName);
            if (val == null)
            {
                string altArgumentName;
                altArgumentName = _testNamePrefix + argumentName;
                val = InternalGetArgument(altArgumentName);
            }
            if (val == null)
            {
                argumentValue = null;
                return false;
            }
            else
            {
                // Process any magic prefixes.
                //argumentValue = CheckAutoData(argumentName, val.ToString());
                argumentValue = CheckEscape(argumentName, val.ToString());
                return true;
            }
        }

        /// <summary>
        /// Sets an argument value that can later on be retrieved through GetArgument.
        /// </summary>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="value">Value for the argument.</param>
        /// <remarks>
        /// Note that you cannot unset arguments, although you can
        /// set one to a blank string.
        /// </remarks>
        public void SetArgument(string argumentName, string value)
        {
            _arguments[argumentName] = value;
        }



        /// <summary>
        /// Gets the pre installed keyboard layouts
        /// </summary>
        /// <returns></returns>
        public List<KeyboardLayout> GetPreInstalledKeyboardLayouts()
        {
            return _installedKeyboardLayouts;
        }

        /// <summary>
        /// Sets object properties from configuration values
        /// based on a prefix for argument names.
        /// </summary>
        /// <param name="obj">Object with values to set.</param>
        /// <param name="prefixName">Name to prefix property names to
        /// generate argument names.</param>
        public void SetObjectProperties(object obj, string prefixName)
        {
            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();

            IFormatProvider provider = System.Globalization.CultureInfo.InvariantCulture;
            Type t = obj.GetType();
            PropertyInfo[] props = t.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                string name = prefixName + prop.Name;
                string val = GetArgument(name, false);
                if (val != String.Empty)
                {
                    object propValue = System.Convert.ChangeType(
                        val, prop.PropertyType, provider);
                    prop.SetValue(obj, propValue, null);
                }
            }
        }

        #endregion Public methods.

        #region Internal Methods.

        /// <summary>
        /// Stores the Initial Keyboard layouts installed on the machine
        /// </summary>
        /// <param name="layoutList">Installed Keyboard layouts</param>
        public void SetInstalledKeyboardLayouts(List<KeyboardLayout> layoutList)
        {
            foreach (KeyboardLayout layout in layoutList)
            {
                _installedKeyboardLayouts.Add(layout);
            }
        }

        /// <summary>
        /// Reinstates keyboard state
        /// </summary>
        /// <returns></returns>
        public bool ReinstateOriginalKeyboardState()
        {
            bool result = true;
            List<KeyboardLayout> currentLayoutList = KeyboardLayoutHelper.GetCurrentKeyboardLayouts();

            foreach (KeyboardLayout layout in currentLayoutList)
            {
                if (_installedKeyboardLayouts.Contains(layout) == false)
                {
                    result = result && KeyboardLayoutHelper.TryUninstallLayout(layout);
                }
            }
            foreach (KeyboardLayout layout in _installedKeyboardLayouts)
            {
                if (KeyboardLayoutHelper.IsLayoutInstalled(layout) == false)
                {
                    result = result && KeyboardLayoutHelper.TryInstallLayout(layout);
                }
            }
            if (KeyboardInput.IsImeFunctionCalled)
            {
                InputMethod.Current.ImeState = ImeState;
            }
            return result;
        }

        #endregion


        #region Public properties.

        /// <summary>An AutomationFramework instance that can be used
        /// to retrieve configuration information.</summary>
        /// <remarks>To enable access to information retrieved from the
        /// automation framework, set this property to a valid object.</remarks>
        public TestLog TestLog
        {
            get { return _testLog; }
            set { _testLog = value; }
        }

        /// <summary>Command-line arguments for the test case.</summary>
        public string[] CommandLineArguments
        {
            get { return _commandLineArguments; }
        }

        /// <summary>Provides static access to the last created
        ///   object.</summary>
        /// <remarks>This static property can be set to override setting the
        ///   default. This would only happen when a test case is using more
        ///   than one configuration service expilcitly; the test library
        ///   never does this.</remarks>
        public static ConfigurationSettings Current
        {
            get
            {
                lock (s_typeLock)
                {
                    if (s_lastConfiguration == null)
                        s_lastConfiguration = new ConfigurationSettings((string[])null);
                }
                return s_lastConfiguration;
            }
            set
            {
                s_lastConfiguration = value;
            }
        }

        public InputMethodState ImeState
        {
            get { return _imeState; }
            set { _imeState = value; }
        }
        #endregion Public properties.

    
        #region Private methods.

        /// <summary>
        /// Populates the Arguments properties with command-line arguments. The
        /// format is [propname]:[propvalue].
        /// </summary>
        /// <remarks>
        /// The property name can also be prefixed by two minus characters,
        /// one minus character, or one slash character. The separator
        /// between the property name and the property value
        /// can be either a colon or an equal sign.
        /// </remarks>
        /// <param name="args">Arguments to process.</param>
        private void BuildArguments(string[] args)
        {
            System.Diagnostics.Debug.Assert(args != null);

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                System.Diagnostics.Debug.Assert(arg != null);
                int prefixCharCount = 0;
                if (arg.StartsWith("--"))
                    prefixCharCount = 2;
                else if (arg.StartsWith("/"))
                    prefixCharCount = 1;
                else if (arg.StartsWith("-"))
                    prefixCharCount = 1;
                else
                    prefixCharCount = 0;
                if (prefixCharCount > 0)
                {
                    int separator = arg.IndexOf(':');

                    //
                    // Verify whether a "=" is used in place of ":", or if
                    // both are present, that the leftmost separator is used.
                    //
                    int altSeparator = arg.IndexOf('=');
                    bool separatorValid = (separator != -1);
                    bool altIsBefore =
                        (altSeparator != -1) && (altSeparator < separator);
                    if (!separatorValid || altIsBefore)
                    {
                        separator = altSeparator;
                    }
                    if (separator != -1)
                    {
                        string argName = arg.Substring(prefixCharCount,
                            separator - prefixCharCount);
                        string argValue = arg.Substring(separator + 1);
                        _arguments[argName] = argValue;
                    }
                }
            }
        }

        #region Magic prefix processing.

        ///// <summary>
        ///// Processes an AutoData invocation command if required.
        ///// </summary>
        //private string CheckAutoData(string argumentName, string command)
        //{
        //    const string AutoDataPrefix = "!AD:";

        //    if (command == null) return null;
        //    if (!command.StartsWith(AutoDataPrefix)) return command;

        //    // Remove the prefix.
        //    command = command.Substring(AutoDataPrefix.Length);

        //    //
        //    // The rest of the command is as follows.
        //    // index=<index> [;length=<length>] [;max=<maxlength>] [;min=<minlength>]
        //    // index is the index in ProblemString.xml; use -1 for random
        //    // length (optional) is the length of string to return
        //    // maxlength (optional) is the maximum length for a returned string
        //    // minlength (optional) is the minimum length for a returned string
        //    //
        //    int index = -1;
        //    int length = -1;
        //    int maxLength = -1;
        //    int minLength = -1;
        //    string[] args = command.Trim().Split(';');
        //    foreach(string arg in args)
        //    {
        //        string[] parts = arg.Split('=');
        //        if (parts.Length != 2)
        //        {
        //            throw new ArgumentException(String.Format(
        //                "AutoData part {0} in [{1}] does not have name/value.",
        //                arg, command), "command");
        //        }
        //        string name = parts[0].Trim();
        //        if (name == "index")
        //        {
        //            index = Int32.Parse(parts[1]);
        //        }
        //        else if (name == "length")
        //        {
        //            length = Int32.Parse(parts[1]);
        //        }
        //        else if (name == "max")
        //        {
        //            maxLength = Int32.Parse(parts[1]);
        //        }
        //        else if (name == "min")
        //        {
        //            minLength = Int32.Parse(parts[1]);
        //        }
        //    }

        //    //
        //    // The call is made in another method to avoid loading
        //    // the AutoData assembly unless it's required by the argument
        //    // specification (assemblies are loaded on-demand when the
        //    // method is JITted).
        //    //
        //    string result = GetAutoDataString(index, length, maxLength, minLength);

        //    //
        //    // We store the real value used, to get the same result for
        //    // repeated invocations (required to keep compatibility).
        //    //
        //    SetArgument(argumentName, result);

        //    return result;
        //}

        /// <summary>
        /// Processes a C-style escape invocation command if required.
        /// </summary>
        private string CheckEscape(string argumentName, string command)
        {
            const string EscapePrefix = "!EC:";

            if (command == null) return null;
            if (!command.StartsWith(EscapePrefix)) return command;

            // Remove the prefix.
            command = command.Substring(EscapePrefix.Length);

            command = TextUtils.ProcessCStyleEscapedChars(command);

            // We store the real value used, for performance reasons.
            SetArgument(argumentName, command);

            return command;
        }

        ///// <summary>Gets a string from AutoData.</summary>
        ///// <param name="index">Index of problem string, -1 to randomize.</param>
        ///// <param name="length">Requested length, -1 for unspecified.</param>
        ///// <param name="maxLength">Length max constraint, -1 for unspecified.</param>
        ///// <param name="minLength">Length min constraint, -1 for unspecified.</param>
        //private string GetAutoDataString(int index, int length, int maxLength,
        //    int minLength)
        //{
        //    // Verify that the options specified do not contradict each other.
        //    const string LengthAndMaxLength =
        //        "Do not specify a maximum length if specific length is requested.";
        //    const string LengthAndMinLength =
        //        "Do not specify a maximum length if specific length is requested.";
        //    const string IndexInvalid =
        //        "AutoData should be -1, 0, or positive.";
        //    const string MaxLengthInvalid =
        //        "Maximum length should be -1, 0, or positive.";
        //    const string MinLengthInvalid =
        //        "Minimum length should be -1, 0, or positive.";
        //    const string MinLengthGreaterMaxLength =
        //        "Minimum length should not be greater than maximum length.";
        //    if (length != -1 && maxLength != -1)
        //        throw new ArgumentException(LengthAndMaxLength, "maxLength");
        //    if (length != -1 && minLength != -1)
        //        throw new ArgumentException(LengthAndMinLength, "minLength");
        //    if (index < -1)
        //        throw new ArgumentOutOfRangeException("index", index, IndexInvalid);
        //    if (maxLength < -1)
        //        throw new ArgumentOutOfRangeException(
        //            "maxLength", maxLength, MaxLengthInvalid);
        //    if (minLength < -1)
        //        throw new ArgumentOutOfRangeException(
        //            "minLength", minLength, MinLengthInvalid);
        //    if (minLength > maxLength)
        //        throw new ArgumentOutOfRangeException(
        //            "minLength", minLength, MinLengthGreaterMaxLength);

        //    string result;
        //    if (index == -1)
        //    {
        //        if (maxLength != -1)
        //            AutoData.Generate.MaxStringSize = maxLength;
        //        if (length == -1)
        //            result = AutoData.Generate.GetRandomString();
        //        else
        //            result = AutoData.Generate.GetRandomString(length);
        //    }
        //    else
        //    {
        //        if (length == -1)
        //            result = AutoData.Extract.GetTestString(index);
        //        else
        //            result = AutoData.Extract.GetTestString(index, length);
        //        if (maxLength != -1 && result.Length > maxLength)
        //            result = result.Substring(0, maxLength);
        //    }

        //    // If the result is too short, append a fixed-length string
        //    // to make it long enough.
        //    if ((minLength >= -1) && (result.Length < minLength))
        //    {
        //        result += GetAutoDataString(index,
        //            result.Length - minLength, -1, -1);
        //    }
        //    return result;
        //}

        #endregion Magic prefix processing.

        /// <summary>Logs a message if the logging service is available.</summary>
        private void Log(string format, params object[] args)
        {
            System.Diagnostics.Trace.WriteLine(String.Format(format, args));
        }

        /// <summary>Retrieves the test name of the executing test.</summary>
        /// <returns>The test name, an empty string if it is not found.</returns>
        private string GetTestName()
        {
            string result = GetArgument("TestName");
            if (result == String.Empty)
            {
                result = GetArgument("TestCaseType");
                if (result == String.Empty)
                {
                    result = GetTestNameFromCommandLine();
                }
            }
            return (result == null) ? String.Empty : result;
        }

        /// <summary>
        /// Attempts to retrieve the test name from command-line arguments.
        /// </summary>
        private string GetTestNameFromCommandLine()
        {
            string result = String.Empty;
            for (int i = 0; i < _commandLineArguments.Length; i++)
            {
                string arg = _commandLineArguments[i];
                if (arg.StartsWith("Run"))
                {
                    result = arg.Substring(3);
                    Log("Argument {0} [{1}] used for test name {2}",
                        arg, i, result);
                    return result;
                }
            }
            return result;
        }

        /// <summary>Retrieves a named configuration value.</summary>
        /// <param name="argumentName">The name of the configuration item.</param>
        /// <returns>The value of the configuration item, null if not found.</returns>
        /// <remarks>
        /// Unlike the exposed GetArgument version, this method does not
        /// attempt to look using the test name prefix. Also, nulls may be returned
        /// instead of empty strings.
        /// </remarks>
        private string InternalGetArgument(string argumentName)
        {
            object val = _arguments[argumentName];

            return (val == null) ? null : val.ToString();
        }


        #endregion Private methods.

        
        #region Private fields.

        /// <summary>Available arguments.</summary>
        private Hashtable _arguments;

        /// <summary>Arguments available through the command line.</summary>
        private string[] _commandLineArguments;

        private List<KeyboardLayout> _installedKeyboardLayouts = new List<KeyboardLayout>();

        /// <summary>Automation framework from which configuration data is obtained.</summary>
        private TestLog _testLog;

        /// <summary>String to prefix for test case-specific configuration.</summary>
        private string _testNamePrefix;

        private InputMethodState _imeState;

        #region Static fields.

        /// <summary>Last configuration created or set for the type.</summary>
        private static ConfigurationSettings s_lastConfiguration;

        /// <summary>Type-wide lock.</summary>
        private static object s_typeLock = new object();

        #endregion Static fields.

        #endregion Private fields.

        
        #region Inner classes.

        /// <summary>Represents a missing value exception.</summary>
        /// <example>This sample shows how this class is typically used.<code>...
        /// public string GetValue(string argumentName)
        /// {
        ///     string value = GetValueNullIfMissing(argumentName);
        ///     if (value == null)
        ///         throw new ConfigurationSettings.MissingValueException(argumentName);
        ///     return value;
        /// }</code></example>
        public class MissingValueException : Exception
        {
            /// <summary>Creates a new MissingValueException instance</summary>
            /// <param name="valueName">Name of value sought and not found.</param>
            public MissingValueException(string valueName)
                : base(String.Format("Configuration value {0} is required but missing", valueName))
            {
            }
        }

        #endregion Inner classes.

        /// <summary>
        /// This is to dump values in hashtable specified by argument ht.
        /// </summary>
        /// <param name="ht"></param>
        public void DumpingHashtableValues(Hashtable ht)
        {
            IEnumerator enumerator;
            int index;
            string message;

            message = "\r\nHashtable dump:\r\n";
            enumerator = ht.GetEnumerator();
            index = 0;
            while (enumerator.MoveNext())
            {
                DictionaryEntry entry = (DictionaryEntry)enumerator.Current;
                object key = entry.Key;
                object val = entry.Value;

                message += "    Item " + index + "\r\n" +
                    "    Key:   Type=" + key.GetType().Name + "; Value=" + key + "\r\n" +
                    "    Value: Type=" + val.GetType().Name + "; Value=" + val + "\r\n";
                index++;
            }
            System.Diagnostics.Trace.WriteLine(message);
        }

        /// <summary>
        /// Overload to DumpingHashTableValues to dump _arguments
        /// </summary>
        public void DumpingHashtableValues()
        {
            DumpingHashtableValues(_arguments);
        }
    }
}
