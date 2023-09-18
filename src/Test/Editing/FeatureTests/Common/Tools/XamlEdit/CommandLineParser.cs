// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Parse the command line string to an object 

namespace XamlPadEdit
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
    using System.Windows.Data;
    using System.Reflection;

    #endregion Namespaces.


    public class CommandItems 
    {
        public string type ;
        public string name ;
        public object[] methodArguments;
        public object objectInstance;
        public Type objectType;

        public CommandItems()
        {
            methodArguments = new object[0];
            type = name = "";
            objectInstance = null;
            objectType = null;
        }

    }

    public class CommandParser
    {
        private Hashtable _objectArray = new Hashtable(); 
        private UIElement _mainRoot = null;
        private TextBox _interpreterResultsBox = null;
        private static CommandItems[] s_commandItemsArray = new CommandItems[12];
        private int _commandIndex = 0;
        private int _objectIndex = 1;
        private string _objString = "object";
        private bool _describeFlag = false;
        private bool _functionPresent = false;
        private string _errorMessage = "";
        private bool _success = true;
        private bool _assignStat = false;
        private string _leftSideOfAssignStat = "";
        private bool _collOperation = false;
        private int _collVal = -1;
        /// <summary>
        /// Get the parsed object
        /// </summary>
        public UIElement MainRoot
        {
            get
            {
                return _mainRoot;
            }
            set
            {
                _mainRoot = value;
            }
        }

        public CommandParser(UIElement MainRootPassed, TextBox InterpreterResultsBoxPassed)
        {
            MainRoot = MainRootPassed;
            _interpreterResultsBox = InterpreterResultsBoxPassed;
        }

        public   void ClearObjectArrHashTable()
        {
            _objectArray.Clear();
            _objectIndex = 1;
        }
        public object GetProperty(object objectInstance, string propertyName)
        {
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
                                throw new Exception("Cannot retreive property value");
                            }
                            break;
                    }
                    return returnValue;
                    // return new ObjectItem(returnValue, "");
                }
                catch (Exception e)
                {
                    if (!_functionPresent)
                    {
                        _errorMessage = e.Message;
                    }
                    //Ignore the exception.
                }
                if (returnValue != null)
                {
                    _success = true;
                    break;
                }
            }
            return null;
            //return new ObjectItem(null, "Parser error: the command line can't be evaluated!");
        }

        public void ParseCommand(string cmd)
        {
            _assignStat = false;
            _collOperation = false;
            _collVal = -1;
            _commandIndex = 0;
            if (cmd.Trim().ToLower() == "cls")
            {
                _interpreterResultsBox.Clear();
                return;
            }
            try
            {
                string newCmd = "";
                cmd = CommandCleaningHelper(cmd);

                #region ProcessingRightSide

                    if (cmd.Contains("new "))
                    {
                        newCmd = NewFunctionHelper(cmd, newCmd);
                        cmd = newCmd;
                    }

                    if (cmd.Contains("="))
                    {
                        _assignStat = true;
                        int indexOfEqualSign = cmd.IndexOf('=');
                        string temp = cmd.Substring(0, indexOfEqualSign );
                        char[] separators = new char[1];
                        separators[0] = ' ';
                        string[] tempArr = temp.Trim().Split(separators);
                        _leftSideOfAssignStat = tempArr[tempArr.Length - 1];
                        cmd = cmd.Substring(indexOfEqualSign+1);
                        double doubleRes;
                        if (double.TryParse(cmd, out doubleRes))
                        {
                            cmd = ExecuteCommand(cmd, false);
                        }
                    }

                    cmd = cmd.Replace(" ", "");
                    _functionPresent = (cmd.Contains("{")) ? true : false;

                    newCmd = "";
                    //Process function parameters
                    FunctionCommandHelper(cmd, ref newCmd);
                    cmd = (newCmd == "") ? cmd : newCmd;

                    //Create Command items
                    while (cmd.IndexOf('.') > 0)
                    {
                        int indexOfDot = cmd.IndexOf('.');
                        CommandConstructionHelper(ref cmd, ref indexOfDot);
                    }

                    //Process the last part of the cmd
                    int indexOfLastChar = cmd.Length;
                    CommandConstructionHelper(ref cmd, ref indexOfLastChar);

                    object rhsValue = ExecuteCompleteCommand();

                #endregion ProcessingRightSide.


                try
                {
                    if (rhsValue != null)
                    {
                        if (_assignStat)
                        {
                            if (_leftSideOfAssignStat.Contains("."))
                            {
                                string lhsCmd = _leftSideOfAssignStat;
                                while (lhsCmd.IndexOf('.') > 0)
                                {
                                    int indexOfDot = lhsCmd.IndexOf('.');
                                    CommandConstructionHelper(ref lhsCmd, ref indexOfDot);
                                }
                                object lhsValue = ExecuteCompleteCommand();

                                ReflectionUtils.SetProperty(lhsValue, lhsCmd.Trim(), rhsValue);
                            }
                            else
                            {
                                if (_objectArray.ContainsKey(_leftSideOfAssignStat))
                                {
                                    _objectArray.Remove(_leftSideOfAssignStat);
                                    _objectArray.Add(_leftSideOfAssignStat, rhsValue);
                                }
                                else
                                {
                                    _objectArray.Add(_leftSideOfAssignStat, rhsValue);
                                }
                            }
                        }
                    }
                    if (rhsValue != null)
                    {
                        _interpreterResultsBox.Text += ((_describeFlag) ? ReflectionUtils.DescribeProperties(rhsValue) : rhsValue);
                    }
                    _interpreterResultsBox.Text += "[DONE]\r\n";

                }
                catch (Exception e2)
                {
                    _interpreterResultsBox.Text += e2.Message.ToString()+"\r\n";
                }
            }
            catch (IndexOutOfRangeException)
            {
                _interpreterResultsBox.Text += "Error - Possible problem with syntax\r\n";
            }
            finally
            {
                _interpreterResultsBox.Text += "---------------------------------------------\r\n";
                _interpreterResultsBox.ScrollToEnd();
            }
        }

        private string CommandCleaningHelper(string cmd)
        {
            _describeFlag = (cmd[0] == '~') ? true : false;
            cmd = cmd.Replace("~", "");
          
            while(cmd.Contains("( "))
                cmd = cmd.Replace("( ", "(");
            while (cmd.Contains(" )"))
                cmd = cmd.Replace(" )", ")");
            cmd = cmd.Replace("()", "{}");

            while (cmd.Contains("\""))
            {
                int begin = cmd.IndexOf("\"");
                int end = cmd.IndexOf("\"", begin + 1);
                string substr = cmd.Substring(begin + 1, end - begin + 1 - 2).Trim();
                string temp = ExecuteCommand(substr, false);
                substr = cmd.Substring(0, begin);
                substr += temp;
                substr += cmd.Substring(end + 1);
                cmd = substr;
            }

            int i = 0;
            while (cmd.IndexOf("(", i) > 0)
            {
                i = cmd.IndexOf("(", i);
                if (char.IsLetterOrDigit(cmd[i - 1]) && char.IsLetterOrDigit(cmd[i + 1]))
                {
                    cmd = cmd.Remove(i, 1);
                    cmd = cmd.Insert(i, "{");
                    i++;
                    int countOpeningBraces = 1;
                    while ((countOpeningBraces > 0) && (i < cmd.Length))
                    {
                        if (cmd[i] == ')')
                        {
                            countOpeningBraces--;
                            if (countOpeningBraces == 0)
                            {
                                cmd = cmd.Remove(i, 1);
                                cmd = cmd.Insert(i, "}");

                                break;
                            }
                        }
                        else
                            if (cmd[i] == '(')
                            {
                                countOpeningBraces++;
                            }
                        i++;
                    }
                }
                i++;
            }

            return cmd;
        }

        private string NewFunctionHelper(string cmd, string newCmd)
        {
            if (cmd.Contains("new "))
            {
                int startIndex = cmd.IndexOf("new ");
                int closeIndex = cmd.IndexOf("}", startIndex);
                string replaceStr = "";
                string subCmd = cmd.Substring(startIndex + 3, (closeIndex -1 - (startIndex + 1)));
                FunctionCommandHelper(subCmd, ref replaceStr);

                EvaluateFunctionParametersHelper(replaceStr);
                _commandIndex++;
                replaceStr = "";

                try
                {
                    object p = null;
                    p = ReflectionUtils.CreateInstanceOfType(s_commandItemsArray[0].name, s_commandItemsArray[0].methodArguments);

                    ObjectWrapped o = new ObjectWrapped(p);
                    o.Name = _objString + _objectIndex.ToString();
                    if (_objectArray.ContainsKey(o.Name))
                    {
                        _objectArray.Remove(o.Name);
                    }
                    _objectArray.Add(o.Name, o);
                    replaceStr += o.Name;
                    _objectIndex++;
                    _commandIndex = 0;
                }
                catch (Exception e)
                {
                    _interpreterResultsBox.Text += e.Message.ToString() +"\r\n";
                    _commandIndex = 0;
                }

                newCmd = cmd.Substring(0, startIndex);
                newCmd += replaceStr;
                newCmd += cmd.Substring(closeIndex + 1);
            }
            return newCmd;
        }

        private void FunctionCommandHelper(string cmd,ref string newCmd)
        {
            int begin = 0;
            try
            {
                while (cmd.IndexOf('{', begin) > 0)
                {

                    int functionCloseIndex = cmd.IndexOf('}', cmd.IndexOf('}', begin));
                    if (functionCloseIndex > (cmd.IndexOf('{', begin)))
                    {
                        int functionStartIndex = cmd.IndexOf('{', begin);
                        string insertString = "";
                        string temp = cmd.Substring(functionStartIndex + 1, functionCloseIndex - 1 - functionStartIndex);
                        if (temp != "")
                        {
                            while (temp.IndexOf(',') > 0)
                            {
                                string tempCmd = temp.Substring(0, temp.IndexOf(','));
                                insertString += ExecuteCommand(tempCmd,false) + ",";
                                temp = temp.Substring(temp.IndexOf(',') + 1, temp.Length - temp.IndexOf(',') - 1);
                            }
                            temp = temp.Substring(0, temp.Length);
                            insertString += ExecuteCommand(temp,false);

                        }
                        int start = (begin == 0) ? begin : (begin - 1);
                        newCmd += cmd.Substring(start, functionStartIndex - start + 1);
                        newCmd += insertString;
                        begin = functionCloseIndex + 1;
                    }

                }
                begin = (begin == 0) ? 1 : begin;
                newCmd += cmd.Substring(begin - 1, cmd.Length - begin + 1);

            }
            catch (ArgumentOutOfRangeException)
            {
                _interpreterResultsBox.Text += "Parsing Error - Possible error in syntax\r\n";
            }
        }

        public string CreateObjectForTreeItem(object treeItemTag)
        {
            ObjectWrapped o = new ObjectWrapped(treeItemTag);
            o.Name = _objString + _objectIndex.ToString();
            if (_objectArray.ContainsKey(o.Name))
            {
                _objectArray.Remove(o.Name);
            }
            _objectArray.Add(o.Name, o);
            _objectIndex++;
            return o.Name;
        }

        public string CreateObjectForTreeItem(object treeItemTag, string name)
        {
            ObjectWrapped o = new ObjectWrapped(treeItemTag);
            o.Name = name;
            if (_objectArray.ContainsKey(o.Name))
            {
                _objectArray.Remove(o.Name);
            }
            _objectArray.Add(o.Name, o);
            _objectIndex++;
            return o.Name;
        }

        private void CommandConstructionHelper(ref string cmd, ref int index)
        {
            if (cmd[index - 1] == ')')
            {
                int countOfClosingParantheses = 1;
                int end = index - 2;
                int endOfString = end - 1;
                int traversal = 0;
                s_commandItemsArray[_commandIndex] = new CommandItems();

                while ((end >= 0) && (countOfClosingParantheses > 0))
                {
                    if (cmd[end] == ')')
                    {
                        countOfClosingParantheses++;
                    }
                    else
                        if (cmd[end] == '(')
                        {
                            countOfClosingParantheses--;
                            if (traversal == 0)
                            {
                                string name = cmd.Substring(end+1, endOfString - end);
                                s_commandItemsArray[_commandIndex].name = name;
                                traversal++; endOfString = -1;
                            }
                            else
                                if ((traversal == 1) && (endOfString != -1))
                                {
                                    string type = cmd.Substring(end+1, endOfString - end);
                                    s_commandItemsArray[_commandIndex].objectType = ReflectionUtils.FindType(type);
                                    traversal++;
                                }
                        }
                        else
                            if (endOfString == -1)
                            {
                                endOfString = end;
                            }
                    end--;
                }
                end++;
                _commandIndex++;
                cmd = cmd.Remove(end, index - 1 - end);
            }
            else
                if (cmd[index - 1] == '}')
                {
                    cmd = EvaluateFunctionParametersHelper(cmd);
                  
                    _commandIndex++;
                }
                else
                {
                    index--;
                    int tail = index;
                    while (tail >= 0)
                    {
                        if ((cmd[tail] != ')') || (cmd[tail] != '('))
                        {
                            tail--;
                        }
                    }
                    tail++;
                    string name = cmd.Substring(tail, index + 1 - tail);
                    s_commandItemsArray[_commandIndex] = new CommandItems();

                    if (name.Contains("["))
                    {
                        int begin = name.IndexOf("[");
                        int end = name.IndexOf("]");
                        string num = name.Substring(begin + 1, end - begin - 1);
                        int intRes;
                        if (int.TryParse(num.Trim(), out intRes) == true)
                        {
                            _collOperation = true;
                            name = name.Substring(0, begin);
                            _collVal = intRes;
                        }
                    }

                    string checkIfVarExistsInObjArr = ExecuteCommand(name,true);
                    if (checkIfVarExistsInObjArr == name)
                    {
                        s_commandItemsArray[_commandIndex].name = name;
                    }
                    else
                    {
                        s_commandItemsArray[_commandIndex].name = checkIfVarExistsInObjArr;
                        name = checkIfVarExistsInObjArr;
                    }

                    if(_objectArray.ContainsKey(name))
                    {
                        IDictionaryEnumerator enumerator = _objectArray.GetEnumerator();
                        while(enumerator.MoveNext())
                        {
                            if (enumerator.Key.ToString() == name)
                            {
                                s_commandItemsArray[_commandIndex].objectInstance = (enumerator.Value is ObjectWrapped) ? ((ObjectWrapped)enumerator.Value).ObjectValue : enumerator.Value;
                            }
                        }
                    }

                    _commandIndex++;
                    int endOfDeletedString = index + 2 - tail;
                    endOfDeletedString = (endOfDeletedString > cmd.Length) ? cmd.Length : endOfDeletedString;
                    cmd = cmd.Remove(tail, endOfDeletedString);
                }
        }

        private string EvaluateFunctionParametersHelper(string cmd)
        {
            try
            {
                s_commandItemsArray[_commandIndex] = new CommandItems();
                string functionName = "";
                int indexOfFunctionStart = cmd.IndexOf("{");
                functionName = cmd.Substring(0, indexOfFunctionStart);
                s_commandItemsArray[_commandIndex] = new CommandItems();
                s_commandItemsArray[_commandIndex].name = functionName.Replace(" ", "");

                string tempCmd = "";
                tempCmd = cmd.Substring(indexOfFunctionStart + 1, (cmd.IndexOf('}') - (indexOfFunctionStart + 1)));
                int startIndexForCutting = cmd.IndexOf('}') + 1;
                startIndexForCutting = (cmd.Length == startIndexForCutting) ? (startIndexForCutting - 1) : (startIndexForCutting + 1);
                cmd = cmd.Substring(startIndexForCutting );

                char[] separatorArr = new char[1];
                separatorArr[0] = ',';
                if (tempCmd != "")
                {
                    string[] parameters = tempCmd.Split(separatorArr);
                    object[] objArr = new object[parameters.Length];
                    int count = 0;
                    IDictionaryEnumerator enumerator = _objectArray.GetEnumerator();
                    foreach (string str in parameters)
                    {
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Key.ToString() == str)
                            {
                                ObjectWrapped obj = enumerator.Value as ObjectWrapped;
                                if (obj != null)
                                {
                                    objArr[count] = obj.ObjectValue;
                                }
                                else
                                {
                                    objArr[count] = enumerator.Value;
                                }
                                count++;
                                enumerator.Reset();
                                break;
                            }
                        }
                    }
                    s_commandItemsArray[_commandIndex].methodArguments = objArr;
                }
                return cmd;
            }
            catch(Exception)
            {
                return "";
            }
        }

        object  ExecuteCompleteCommand()
        {
            _success = true;
            object root1 = null;
            try
            {
                for (int i = 0; i < _commandIndex; i++)
                {
                    if (i == 0)
                    {
                        root1 = LogicalTreeHelper.FindLogicalNode(MainRoot, s_commandItemsArray[i].name);
                        if (root1 == null)
                        {
                            try
                            {
                                root1 = ReflectionUtils.FindType(s_commandItemsArray[i].name);
                            }
                            catch (Exception) { }
                            if (root1 == null)
                            {
                                if (_objectArray.ContainsKey(s_commandItemsArray[i].name))
                                {
                                    IDictionaryEnumerator enumerator = _objectArray.GetEnumerator();
                                    while (enumerator.MoveNext())
                                    {
                                        if (enumerator.Key.ToString() == s_commandItemsArray[i].name)
                                        {
                                            root1 = (enumerator.Value is ObjectWrapped) ? ((ObjectWrapped)enumerator.Value).ObjectValue : enumerator.Value;
                                            break;
                                        }
                                    }
                                }
                                if (root1 == null)
                                {
                                    string error = s_commandItemsArray[i].name + " cannot be found";
                                    throw new Exception(error);
                                }
                            }
                        }
                        if (root1 != null)
                        {
                            s_commandItemsArray[i].objectInstance = root1;
                            s_commandItemsArray[i].objectType = root1.GetType();
                        }
                    }
                    else
                    {
                        object p = GetProperty(root1, s_commandItemsArray[i].name);
                        if (p == null)
                        {

                            object[] o = new object[2];

                            p = ReflectionUtils.InvokeInstanceMethod(root1, s_commandItemsArray[i].name, s_commandItemsArray[i].methodArguments);
                        }
                        root1 = p;
                        if (root1 == null)
                        {
                            _interpreterResultsBox.Text += "Null\r\n";
                            break;
                        }
                    }

                }
                if (_success == false)
                {
                    _interpreterResultsBox.Text += _errorMessage +"\r\n";
                }
            }
            catch (Exception e)
            {
                _success = false;
                root1 = null;
                try
                {
                    if (_functionPresent)
                    {

                        _interpreterResultsBox.Text += e.Message.ToString() + "\r\n";
                    }
                    else
                    {
                        _interpreterResultsBox.Text += _errorMessage + "\r\n";
                    }
                }
                catch (Exception)
                { 
                }
            }
            finally
            {
                _interpreterResultsBox.ScrollToEnd();
                s_commandItemsArray = new CommandItems[10];
                _commandIndex = 0;
            }
            try
            {
                if (_collOperation)
                {
                    IEnumerator enumerator = ((ICollection)root1).GetEnumerator();
                    int count = 0;
                    while (enumerator.MoveNext())
                    {
                        if (count == _collVal)
                        {
                            root1 = enumerator.Current;
                            enumerator.Reset();
                            break;
                        }
                        else
                        {
                            count++;
                        }
                    }
                }
            }
            catch (InvalidCastException)
            {
                _success = false;
                _interpreterResultsBox.Text += "InvalidCastException thrown in enumerator \r\n";
                root1 = null;
            }
            return root1;
        }

        string ExecuteCommand(string subCmd, bool insertSubCmdName)
        {
            string finalResultStr = "";
            double doubleRes=0;
            if((subCmd.Contains("."))&&(double.TryParse(subCmd,out doubleRes) == false))
            {
                int count = 0;
                char [] separatorArr = new char[1];
                separatorArr[0] = '.';
                object root1 = null;
                object p = null;

                string[] cmds = subCmd.Split(separatorArr);
                foreach (string str in cmds)
                {
                    if (count++ == 0)
                    {
                        root1 = LogicalTreeHelper.FindLogicalNode(MainRoot, str);
                        if (root1 == null)
                        {
                            root1=GetProperty(null, str);
                        }
                        if (root1 == null)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        p =GetProperty(root1, str);
                        //InterpreterResultsBox.Text += ReflectionUtils.DescribeProperties(p) + "---------------\r\n";
                        root1 = p;
                    }
                }
                ObjectWrapped o = new ObjectWrapped(p);
                o.Name = _objString + _objectIndex.ToString();
                if (_objectArray.ContainsKey(o.Name))
                {
                    _objectArray.Remove(o.Name);
                }
                _objectArray.Add(o.Name, o);
                finalResultStr += o.Name;
                _objectIndex++;
                return finalResultStr;
            }
            else
            {
                if (_objectArray.ContainsKey(subCmd))
                {
                    return subCmd;
                }
                else
                {
                    int result;
                    ObjectWrapped o ;
                    bool outBool;
                    if (int.TryParse(subCmd, out result))
                    {
                        o = new ObjectWrapped(result);
                    }
                    else
                    if(double.TryParse(subCmd,out doubleRes))
                    {
                        o = new ObjectWrapped(doubleRes);
                    }
                    else if (bool.TryParse(subCmd, out outBool))
                    {
                        o = new ObjectWrapped(outBool);
                    }
                    else
                    {
                        o = new ObjectWrapped(subCmd);
                    }

                    o.Name = (insertSubCmdName)? subCmd:( _objString + _objectIndex.ToString());
                    if (_objectArray.ContainsKey(o.Name))
                    {
                        _objectArray.Remove(o.Name);
                    }
                    _objectArray.Add(o.Name, o);
                    finalResultStr += o.Name;
                    _objectIndex = (insertSubCmdName)?_objectIndex:(_objectIndex + 1);
                    return finalResultStr;
                }
            }
        }

        public object ParsePropertyCommand(string cmd)
        {
            object root1 = null;
            char[] separators = new char[1];
            separators[0] = '.';
            string[] cmds = cmd.Split(separators);
            int count = 0;
            bool findType = false;

            foreach (string subCmd in cmds)
            {
                if (count++ == 0)
                {
                    root1 = LogicalTreeHelper.FindLogicalNode(MainRoot, subCmd);
                    if (root1 == null)
                    {
                        try
                        {
                            root1 = ReflectionUtils.FindType(subCmd);
                            findType = true;
                        }
                        catch (Exception) { findType = false; }
                        if (root1 == null)
                        {
                            if (_objectArray.ContainsKey(subCmd))
                            {
                                IDictionaryEnumerator enumerator = _objectArray.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    if (enumerator.Key.ToString() == subCmd)
                                    {
                                        root1 = (enumerator.Value is ObjectWrapped) ? ((ObjectWrapped)enumerator.Value).ObjectValue : enumerator.Value;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (root1 == null)
                    {
                        return null;
                    }
                    root1 = GetProperty(root1, subCmd);
                }
            }
            if (root1 != null)
            {
                Type type;
                if (findType == false)
                {
                    type = root1.GetType();
                }
                else
                {
                    type = root1 as Type;
                }

                ArrayList strList = new ArrayList();
                BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static |  BindingFlags.FlattenHierarchy;
                PropertyInfo[] pInfo = type.GetProperties(bindingAttr);

                foreach (PropertyInfo info in pInfo)
                {
                    if (!strList.Contains(info.Name))
                    {
                        strList.Add(info.Name);
                    }
                }

                if (findType == false)
                {
                    MethodInfo[] mInfos = type.GetMethods( bindingAttr);
                    foreach (MethodInfo minfo in mInfos)
                    {
                        if((minfo.ToString().Contains("EventHandler") == false) && !(minfo.Name.Contains("get_")) && !(minfo.Name.Contains("set_")))
                        {

                            if (!strList.Contains(minfo.Name + "()"))
                            {
                                strList.Add(minfo.Name + "()");
                            }
                        }
                    }
                }
                if (findType)
                {
                    FieldInfo[] fInfos = type.GetFields(bindingAttr);
                    foreach (FieldInfo fInfo in fInfos)
                    {
                        if (fInfo.Name.Trim().Contains("Property"))
                        {
                            if (strList.Contains(fInfo.Name.Replace("Property", "")))
                            {
                                strList.Remove(fInfo.Name.Replace("Property", ""));
                            }
                        }
                        if ((!strList.Contains(fInfo.Name)&& !(fInfo.Name.Contains("__"))))
                        {
                            strList.Add(fInfo.Name);
                        }
                    }
                }
                strList.Sort();
                return (strList.Count==0)?null:strList;
            }
            return root1;
        }
    }


    public class ObjectWrapped 
    {
        private object _obj;
        private string _name;

        public ObjectWrapped(object o)
        {
            _obj = o;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public object ObjectValue
        {
            get
            {
                return _obj;
            }
            set
            {
                _obj = value;
            }
        }
    }
}