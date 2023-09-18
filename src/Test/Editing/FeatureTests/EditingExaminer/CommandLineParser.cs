// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Parse the command line string to an object

namespace EditingExaminer
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Windows;
    using System.IO;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Input;
    using System.Xml;

    #endregion Namespaces.
      /// <summary>
    /// This class parse the command line commands.
    /// </summary>
    public class CommandLine
    {
        Hashtable _table;
        ObjectItem _result;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="commandLine"></param>
        /// <param name="table"></param>
        public CommandLine(string commandLine, Hashtable table)
        {
            _table = table;
            _result = new ObjectItem(null, null);
            DoParse(commandLine);
        }

        /// <summary>
        /// recursively parse the commandline.
        /// </summary>
        /// <param name="commandLine"></param>
        void DoParse(string commandLine)
        {
            string strLeft;
            string strRight;
            object left;
            string str = commandLine.Replace("  ", " ");
            str = str.Trim();
            //Asignment
            if (str.Contains("="))
            {
                string[] strs = str.Split(new char[] { '=' });
                if (strs.Length != 2 || strs[0].Length == 0 || str[1] == 0)
                {
                    _result.Error = "syntax error: Don't know how to do Assignment!";
                    return;
                }
                strLeft = strRight = string.Empty;
                strLeft = strs[0];
                strRight = strs[1];
                strLeft = strLeft.Trim();
                strRight = strRight.Trim();
                _result = new CommandLine(strRight, _table).Result;

                //Set a value to a variable in the Table.
                if (_table.Contains(strLeft))
                {
                    _table.Remove(strLeft);
                    _table.Add(strLeft, Result.Value);
                }
                //Assign a value to a property.
                else if (strLeft.Contains("."))
                {
                    left = new CommandLine(strLeft.Substring(0, strLeft.LastIndexOf(".")), _table).Result.Value;
                    if (left != null)
                    {
                        ReflectionUtils.SetProperty(left, strLeft.Substring(strLeft.LastIndexOf(".") + 1), Result.Value);
                    }
                    else
                    {
                        Result.Error = "Failed: can't set value to " + strLeft + "!";
                    }
                }
                //make a declaration
                else
                {
                    _table.Add(strLeft, Result.Value);
                }
            }
            //invoke a method, Creating a object.
            else if (str.EndsWith(")"))
            {
                if (str.StartsWith("new "))
                {
                    str = str.Substring(4).Trim();
                }
                left = null;
                strRight = strLeft = str.Substring(0, str.IndexOf("("));

                //if there is dot, we are invoke a method from a object
                if (strLeft.LastIndexOf(".") >= 0)
                {
                    strLeft = strLeft.Substring(0, strLeft.LastIndexOf("."));
                    left = new CommandLine(strLeft, _table).Result.Value;
                }

                //when left is null, we are invoke an constructor, otherwise we are invoke a method.
                if (left != null)
                {
                    //Get the command line start from the method name
                    //this will get the argument list.
                    str = str.Substring(strRight.LastIndexOf(".") + 1);
                }

                Result.Value = InvokMethod(str, left);
            }
            //Get - Retrive data
            else
            {
                if (_table.Contains(str))
                {
                    _result.Value = _table[str];
                    return;
                }
                left = null;
                strRight = str;

                if (str.Contains("."))
                {
                    strLeft = str.Substring(0, str.IndexOf("."));
                    left = _table[strLeft];

                    //If we found the object from the Table, we need the property name to retrieve the property value.
                    if (left != null && str.Length - (strLeft.Length + 1) > 0)
                    {
                        strRight = str.Substring(str.IndexOf(".") + 1, str.Length - (strLeft.Length + 1));
                    }
                }
                //strRight could be the following:
                //  1.  strRight=\"abc\"        - string in double quotes
                //  2.  strRight = 12345        - integer  
                //  3.  strRight = 123.456      - double
                //Thus, we should always call get value event if the left is null.
                _result = Get_Value(left, strRight);

                if (Result.Error != null && Result.Error.Length > 0)
                {
                    //if the expression is just for a static member of a class
                    _result = GetProperty(null, strRight);
                }
            }
        }

        /// <summary>
        /// Create an object. 
        ///     1. new a object: new abc(p1, p2, p3...)
        ///     2. Create a instance for a primative values. for example
        ///     3. Create instance for Enum.
        ///     4. create a instance for struct.
        /// </summary>
        /// <param name="commandLine"></param>
        /// <param name="objectInstance"></param>
        /// <returns></returns>
        object InvokMethod(string commandLine, object objectInstance)
        {
            string name;
            string arg;
            string[] args;
            object[] objs;
            object returnValue;
            commandLine = commandLine.Trim();
            objs = null;
            returnValue = false;
            name = commandLine.Substring(0, commandLine.IndexOf("("));
            arg = commandLine.Substring(commandLine.IndexOf("(") + 1, commandLine.IndexOf(")") - (commandLine.IndexOf("(") + 1));
            arg = commandLine.Substring(commandLine.IndexOf("(") + 1);
            arg = arg.Substring(0, arg.Length - 1);
            if (arg.Length > 0)
            {
                args = arg.Trim().Split(new char[] { ',' });
                objs = new object[args.Length];

                for (int i = 0; i < args.Length; i++)
                {
                    objs[i] = new CommandLine(args[i], _table).Result.Value;
                }
            }
            else
            {
                objs = new object[0];
            }
            //If there is an instance, we should invoke a instance methods. 
            if (objectInstance != null)
            {
                try
                {
                    //try the instance methods first.
                    returnValue = ReflectionUtils.InvokeInstanceMethod(objectInstance, name, objs);
                }
                catch (Exception e)
                {
                    //try the static methods.
                    //if there the invoking failed here, we got the Exception message in the Error box.
                    returnValue = ReflectionUtils.InvokeStaticMethod(objectInstance as Type, name, objs);
                }
            }
            //create a object. 
            else
            {
                returnValue = ReflectionUtils.CreateInstanceOfType(name, objs);
            }
            return returnValue;
        }

        /// <summary>
        /// This method helps recursivly retrive propeties.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        ObjectItem Get_Value(object o, string name)
        {
            ObjectItem result;
            string left;
            int index;
            string parseError = "Parser error: the command line can't be evaluated!";

            result = new ObjectItem(null, null);
            if (/*o == null ||*/ name == null || name == string.Empty)
            {
                return result;
            }

            index = name.IndexOf('.');
            index = (index > 0) ? index : name.Length;
            left = name.Substring(0, index);

            if (left.Contains("Children"))
            {
                result = GetProperty(o, "Children");
                if (left == "Children") { }
                else
                {
                    string str = left;
                    int _StartIndex = str.IndexOf('[');
                    int _endIndex = str.IndexOf(']', _StartIndex + 1);
                    if ((_StartIndex < _endIndex) && (_StartIndex >= 0) && (_endIndex < str.Length))
                    {
                        string num = str.Substring(_StartIndex + 1, (_endIndex - _StartIndex - 1));
                        int val = Int32.Parse(num);
                        UIElementCollection uiColl = result.Value as UIElementCollection;
                        result.Value = uiColl[val];
                    }
                }
            }
            else
	        {
          	      result = GetProperty(o, left);
	        }	

            if (result.Error != null && result.Error.Length > 0)
            {
                result.Error = parseError;
            }

            if (left != name)
            {
                if (result.Value == null)
                {
                    result.Error = parseError;
                }
                else
                {
                    result = Get_Value(result.Value, name.Substring(index + 1));
                }
            }

            return result;
        }

        ObjectItem GetProperty(object objectInstance, string propertyName)
        {
            string typeName;
            Type type = null;
            object returnValue = null;
            for (int i = 0; i <= 8; i++)
            {
                try
                {
                    switch (i)
                    {
                        case 0:
                            returnValue = ReflectionUtils.GetField(objectInstance, propertyName);
                            break;
                        case 1:

                            returnValue = ReflectionUtils.GetProperty(objectInstance, propertyName);
                            break;
                        case 2:
                            returnValue = ReflectionUtils.GetStaticField(objectInstance as Type, propertyName);
                            break;
                        case 3:
                            returnValue = ReflectionUtils.GetInterfaceProperty(objectInstance, propertyName, propertyName);
                            break;
                        case 4:
                            returnValue = Convert.ToInt32(propertyName);
                            break;
                        case 5:
                            returnValue = Convert.ToDouble(propertyName);
                            break;
                        case 6:
                            if (propertyName.StartsWith("\"") && propertyName.EndsWith("\""))
                            {
                                returnValue = propertyName.Substring(1, propertyName.Length - 2);
                            }
                            else
                            {
                                throw new Exception("error");
                            }
                            break;
                        case 7:

                            returnValue = ReflectionUtils.GetStaticProperty(objectInstance as Type, propertyName);
                            break;
                        case 8:
                            if (objectInstance == null)
                            {
                                returnValue = ReflectionUtils.FindType(propertyName);
                            }
                            else
                            {
                                throw new Exception("");
                            }
                            break; 
                    }
                    return new ObjectItem(returnValue, "");
                }
                catch (Exception e)
                {
                    //Ignore the exception.
                }
                if (returnValue != null)
                {
                    break;
                }
            }
            return new ObjectItem(null, "Parser error: the command line can't be evaluated!");
        }

        /// <summary>
        /// retrieve the parsed object.
        /// </summary>
        public ObjectItem Result
        {
            get
            {
                return _result;
            }
        }
    }

    /// <summary>
    /// class for hold a parsed object
    /// </summary>
    public class ObjectItem
    {
        private object _value;
        private string _error;

        /// <summary>
        /// Constructor for creating a object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="error"></param>
        public ObjectItem(object value, string error)
        {
            _value = value;
            _error = error;
        }

        /// <summary>
        /// override the ToSring method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return (Error == null || Error == string.Empty) ? (Value == null) ? "null" : Value.ToString() : Error;
        }

        /// <summary>
        /// Get the parsed object
        /// </summary>
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Get the error message.
        /// </summary>
        public string Error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
            }
        }
    }
}