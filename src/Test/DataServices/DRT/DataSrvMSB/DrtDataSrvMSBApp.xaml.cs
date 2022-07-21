// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace DrtDataSrv
{
    public partial class DrtDataSrvMSBApp : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted);
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

        private void OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            Dispatcher.BeginInvoke(
                DispatcherPriority.ApplicationIdle,
                new DispatcherOperationCallback(DoVerify),
                null
                );
        }

        object DoVerify(object arg)
        {
            DataTriggerPage page = (DataTriggerPage) this.MainWindow.Content;

            Assert(page._looseFileImage.Source != null, "Binding for loose file image failed");
            Assert(page._resourceImage.Source != null, "Binding for embedded resource image failed");
            
            Dispatcher.BeginInvoke(
                DispatcherPriority.ApplicationIdle,
                new DispatcherOperationCallback(Quit),
                null
                );
            return null;
        }

        object Quit(object arg)
        {
            Shutdown(0);
            return null;
        }

        public void Assert(bool cond, string message, params object[] arg)
        {
            if (!cond)
            {
                string s;
                // Don't call String.Format if we didn't get any args.  Otherwise,
                // curly brackets in the string will cause String.Format to throw.
                if (arg.Length == 0)
                {
                    s = message;
                }
                else
                {
                    s = String.Format(message, arg);
                }
                throw new Exception("Assert failed: " + s);
            }
        }
        
    }

    public class Places : ObservableCollection<Place>
    {
        public Places()
        {
            Add(new Place("Seattle", "WA"));
            Add(new Place("Redmond", "WA"));
            Add(new Place("Bellevue", "WA"));
            Add(new Place("Kirkland", "WA"));
            Add(new Place("Portland", "OR"));
            Add(new Place("San Francisco", "CA"));
            Add(new Place("Los Angeles", "CA"));
            Add(new Place("San Diego", "CA"));
            Add(new Place("San Jose", "CA"));
            Add(new Place("Santa Ana", "CA"));
            Add(new Place("Bellingham", "WA"));
        }
    }

    public class Place
    {
        private string _name;

        private string _state;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string State
        {
            get { return _state; }
            set { _state = value; }
        }

        public Place()
        {
            _name = "";
            _state = "";
        }

        public Place(string name, string state)
        {
            _name = name;
            _state = state;
        }
    }


    public class Writers : ObservableCollection<Writer>
    {
        public Writers()
        {
            Add(new Writer("Carl", "Sagan"));
            Add(new Writer("Stephen", "King"));
            Add(new Writer("Jules", "Verne"));
            Add(new Writer("J.R.R.", "Tolkien"));
        }
    }

    public class Writer
    {
        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public Writer()
        {
        }

        public Writer(string firstName, string lastName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
        }

    }
}
