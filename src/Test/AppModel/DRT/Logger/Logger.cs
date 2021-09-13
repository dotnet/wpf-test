// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

using System.Windows;


namespace DRT
{
    public class Logger
    {
        protected string DrtName
        {
            get{ return _drtName;}
        }

        protected string Contact
        {
            get { return _contact; }
        }

        protected string Title
        {
            get { return _title; }
        }

        public Logger(string drtName, string contact, string title)
        {
            _drtName = drtName;
            _contact = contact;
            _title = title;

            _logFile = System.IO.File.CreateText(DrtName + ".log");

            Console.WriteLine("DrtName: {0} -- Owner: [AppModel/{1}]", _drtName, _contact);
            _logFile.WriteLine("DrtName: {0} -- Owner: [AppModel/{1}]", _drtName, _contact);

            Console.WriteLine("Title: {0}", _title);
            _logFile.WriteLine("Title: {0}", _title);
        }
           
        public void Log(string message)
        {
            Console.WriteLine(message);
            _logFile.WriteLine(message);
            _logFile.Flush();
        }

        public void Log(string message, params object[] args)
        {
            Console.WriteLine(message, args);
            _logFile.WriteLine(message, args);
            _logFile.Flush();
        }

        public void Log(object obj)
        {
            Log(obj.ToString());
        }


        private Logger()
        {
        }
        
        private string          _drtName;
        private string          _contact;
        private string          _title;
        private StreamWriter    _logFile;   
    }
}
