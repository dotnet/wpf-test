// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  CmdParserUtil class is util class for parsing cmd line

//                                

//  File:       CommandLineParser.cs


//              07/08 Modified

using System;
using System.IO;
using System.Collections;

namespace Microsoft.Test.Text.Common 
{
    public class CommandLineParser 
    {
        private Hashtable table = null;
        
        #region Public Methods
        public CommandLineParser(String[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Trim();
                if (args[i].Equals(String.Empty))
                {
                    // one of the args contains only white space
                    throw new ArgumentException("CommandLineParser, no arg found, please remove additional space between switches!");
                }
                else
                {
                    if ('/' == args[i][0] || '-' == args[i][0])
                    {
                        args[i] = args[i].Remove(0, 1);
                    }
                }
            }
            Initialize(args);
        }

        public CommandLineParser(String commandLine)
        {
            String[] args = null;

            //path in quotation
            if (commandLine.StartsWith("\""))
            {
                args = commandLine.Split('\"');
                if (args.Length > 2)
                {
                    commandLine = args[2];
                }
                else
                {
                    commandLine = String.Empty;
                }
            }
            else
            {
                // cannot have additional space
                args = commandLine.Split(' ');
                if (args.Length < 2)
                {
                    commandLine = args[0];
                }
                else
                {
                    commandLine = String.Empty;
                    for (int i = 1; i < args.Length; i++)
                    {
                        commandLine += args[i] + " ";
                    }
                }
            }

            commandLine = commandLine.Trim(' ');
            if (commandLine.StartsWith("/"))
            {
                args = commandLine.Split('/');
            }
            else if (commandLine.StartsWith("-"))
            {
                args = commandLine.Split('-');
            }
            else
            {
                args = commandLine.Split(' ');
            }
            Initialize(args);
        }

        public CommandLineParser() : this(Environment.CommandLine)
        {
        }

        //check if switch exist
        public bool HasSwitch(String switchName)
        {
            return table.ContainsKey(switchName) ? true : false;
        }

        //get String param
        public String GetStringParam(String switchName)
        {
            if (HasSwitch(switchName))
            {
                return Convert.ToString(table[switchName]);
            }
            else
            {
                throw new ArgumentException("CommandLineParser, switchName \"" + switchName + "\" cannot be found");
            }
        }

        //get int param 32 bit int
        public int GetIntParam(String switchName)
        {
            if (HasSwitch(switchName))
            {
                return Convert.ToInt32(table[switchName]);
            }
            else
            {
                throw new ArgumentException("CommandLineParser, switchName \"" + switchName + "\" cannot be found");
            }
        }

        //get int param 64 bit int
        public long GetLongParam(String switchName)
        {
            if (HasSwitch(switchName))
            {
                return Convert.ToInt64(table[switchName]);
            }
            else
            {
                throw new ArgumentException("CommandLineParser, switchName \"" + switchName + "\" cannot be found");
            }
        }

        public double GetDoubleParam(String switchName)
        {
            if (HasSwitch(switchName))
            {
                return Convert.ToDouble(table[switchName]);
            }
            else
            {
                throw new ArgumentException("CommandLineParser, switchName \"" + switchName + "\" cannot be found");
            }
        }

        //get boolean 
        public bool GetBooleanParam(String switchName)
        {
            if (HasSwitch(switchName))
            {
                return ((table[switchName]).Equals("0") || ((Convert.ToString(table[switchName])).ToLower()).Equals("false")) ? false : true;
            }
            else
            {
                throw new ArgumentException("CommandLineParser, switchName \"" + switchName + "\" cannot be found");
            }
        }
        #endregion

        #region Private Methods
        private void Initialize(String[] args)
        {
            table = new Hashtable();
            
            for (int i=0; i<args.Length; ++i)
            {
                if (String.Empty != args[i])
                {
                    String [] strArray = null;
                    if (args[i].Contains("="))
                    {
                        strArray = args[i].Split('=');
                    }
                    else if ((args[i].Contains(":")))
                    {
                        strArray = args[i].Split(':');
                    }
                    else
                    {
                        strArray = new String[1];
                        strArray[0] = args[i];
                    }
                    
                    strArray[0] = strArray[0].Trim();
                    if (1 == strArray.Length) 
                    {
                        // default switch - interpret as true
                        table.Add(strArray[0], "1");
                    }
                    else
                    {
                        table.Add(strArray[0], strArray[1].Trim());
                    }
                }
            }
        }
        #endregion
    }    
}

