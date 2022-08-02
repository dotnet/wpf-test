// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace HBRApp
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using System.ComponentModel;
    using System.Windows.Input;
    using System.Threading;
    using System.Windows.Threading;

    using System.IO;
    using System.Reflection;
    using System.Xml;

    public partial class HBRDemo
    {
        private readonly int _width = 800;
        private readonly int _height = 605;
        public ArticleList Articles;
        public System.Windows.Navigation.NavigationWindow window;

        // Setup the application window.
        protected override void OnStartup (System.Windows.StartupEventArgs e)
        {


            Articles = new ArticleList ();
            window = new System.Windows.Navigation.NavigationWindow ();

            window.ResizeMode = ResizeMode.CanResize;
            window.Title = "Harvard Business Review Online";
            window.Width = _width;
            window.Height = _height;
            window.Top = 0;
            window.Left = 0;

            // Show!
            window.Show ();

            // Add Navigated EventHandler
            Navigated += new System.Windows.Navigation.NavigatedEventHandler(OnNavigated);

            // Navigate to the startup page
            window.Navigate (new Uri("IssueChooser.xaml", UriKind.RelativeOrAbsolute));
        }

        private void OnNavigated(Object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!s_initDone)
            {
                s_dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
                s_dispatcher.BeginInvoke(
                        System.Windows.Threading.DispatcherPriority.Background,
                        new System.Windows.Threading.DispatcherOperationCallback(OnIdle),
                        null);
            }
        }


        private static void QuitWithDelay(int delayInMs)
        {
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer(
                            System.TimeSpan.FromMilliseconds(delayInMs),
                            System.Windows.Threading.DispatcherPriority.Background,
                            new EventHandler(OnQuitTimer),
                            s_dispatcher);
        }

        private static void OnQuitTimer(object sender, EventArgs e)
        {
            System.Windows.Threading.DispatcherTimer timer = (System.Windows.Threading.DispatcherTimer)sender;
            timer.IsEnabled = false;

            s_dispatcher.InvokeShutdown();
        }

        private object OnIdle(object arg)
        {
            if (window.CurrentSource.ToString().IndexOf("IssueChooser.xaml") > -1)
            {
                window.Navigate (new Uri("TOCandFrame.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                QuitWithDelay(200);
            }

            return null;
        }

        private static bool s_initDone = false;
        private static System.Windows.Threading.Dispatcher s_dispatcher =
                                    System.Windows.Threading.Dispatcher.CurrentDispatcher;

    }

    public delegate int BeforeDeleteHandler (object sender, int index);

    public class ArticleList : ObservableCollection<object>
    {
        private Dispatcher _dispatcher;
        internal static string _runDir;
        // never used   public event BeforeDeleteHandler BeforeDelete;

        public enum ArticleListActions
        {
            Add,
            Insert,
            Remove
        }

        private void AddArticle (string filename)
        {
            //_dispatcher.PostItem (new ArticleListQueueItem (this, ArticleListActions.Add, filename, -1));
            _dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new DispatcherOperationCallback (Dispatch),
                new ArticleListQueueItem (this, ArticleListActions.Add, filename, -1));
        }

        private void InsertArticle (string filename, int position)
        {
            //_dispatcher.PostItem (new ArticleListQueueItem (this, ArticleListActions.Insert, filename, position));
            _dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new DispatcherOperationCallback (Dispatch),
                new ArticleListQueueItem (this, ArticleListActions.Insert, filename, position));
        }

        private int RemoveArticle (string filename)
        {
//          for (int i = 0; i < this.Count; i++)
//          {
//              if (String.Compare (((Article)this[i]).Filename, filename, true) == 0)
//              {
//                  // fire BeforeDelete event
//                  if (BeforeDelete != null)
//                  {
//                      object[] args = { this, i };
//                      int DelayDelete = (int)_dispatcher.Invoke (BeforeDelete, args);
//
//                      Thread.Sleep (DelayDelete);
//                  }
//
//                  _dispatcher.PostItem (new ArticleListQueueItem (this, ArticleListActions.Remove, filename, i));
//                  return i;
//              }
//          }
//
            return -1;
        }

        public object Dispatch (object o)
        {
            ArticleListQueueItem ALQI = o as ArticleListQueueItem;

            switch (ALQI._action)
            {
                case ArticleListActions.Add:
                    ALQI._articleList.Add (new Article (ALQI._filename));
                    break;

                case ArticleListActions.Insert:
                    ALQI._articleList.Insert (ALQI._position, new Article (ALQI._filename));
                    break;

                case ArticleListActions.Remove:
                    ALQI._articleList.RemoveAt (ALQI._position);
                    break;
            }
            return null;
        }

        public class ArticleListQueueItem
        {
            public ArticleListQueueItem (ArticleList articleList, ArticleListActions action, string filename, int position)
            {
                _articleList = articleList;
                _action = action;
                _filename = filename;
                _position = position;
            }

            public ArticleList _articleList;
            public ArticleListActions _action;
            public string _filename;
            public int _position;
        }

        //handler for file change notifications
        private void OnChanged (object source, FileSystemEventArgs args)
        {
            try
            {
                switch (args.ChangeType)
                {
                    case WatcherChangeTypes.Created:
                        AddArticle (args.Name);
                        break;

                    case WatcherChangeTypes.Deleted:
                        RemoveArticle (args.Name);
                        break;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show (e.Message, e.GetType ().Name);
            }
        }

        //handler for file rename notifications
        private void OnRenamed (object source, RenamedEventArgs args)
        {
            try
            {
                InsertArticle (args.Name, RemoveArticle (args.OldName));
            }
            catch (Exception e)
            {
                MessageBox.Show (e.Message, e.GetType ().Name);
            }
        }

        public ArticleList ()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            //MessageBox.Show ("Starting ArticleList");
            string[] files = null;

            // enumerate through .xaml files
            try
            {
                //get current directory
                _runDir = System.IO.Path.GetDirectoryName (Assembly.GetCallingAssembly ().Location);

                //add existing xaml files to the article list
                files = Directory.GetFiles (_runDir, @"DrtFiles\HBROpt\HBRARTICLE*.xaml");
                for (int i = 0; i < files.Length; i++)
                    this.Add (new Article (System.IO.Path.GetFileName (files[i])));

                //create a FileSystemWatcher to watch for new xaml files
                FileSystemWatcher DirectoryWatcher = new FileSystemWatcher ();

                DirectoryWatcher.Path = _runDir;
                DirectoryWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
                DirectoryWatcher.Filter = "HBRARTICAL*.xaml";

                //add event handlers.
                DirectoryWatcher.Created += new FileSystemEventHandler (OnChanged);
                DirectoryWatcher.Deleted += new FileSystemEventHandler (OnChanged);
                DirectoryWatcher.Renamed += new RenamedEventHandler (OnRenamed);

                //begin watching.
                DirectoryWatcher.EnableRaisingEvents = true;
            }
            catch (Exception e)
            {
                MessageBox.Show (e.Message, e.GetType ().Name);
            }
        }

        //public class Article : INotifyPropertyChanged
        public class Article
        {
            static private System.Xml.XmlDocument s_articlesXml = null;
            protected string _filename = "";
            protected string _title = "";
            protected string _author = "(author unknown)";
            protected string _image = @"DrtFiles\HBROpt\Default.jpg";
            protected string _displayModel = "AdaptiveFlow";
            static protected System.Reflection.PropertyInfo Filename_PropertyInfo = typeof(Article).GetProperty ("Filename");
            static protected System.Reflection.PropertyInfo Title_PropertyInfo = typeof(Article).GetProperty ("Title");
            static protected System.Reflection.PropertyInfo Author_PropertyInfo = typeof(Article).GetProperty ("Author");
            static protected System.Reflection.PropertyInfo Image_PropertyInfo = typeof(Article).GetProperty ("Image");
            static protected System.Reflection.PropertyInfo DisplayModel_PropertyInfo = typeof(Article).GetProperty ("DisplayModel");
            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

            protected void INotifyPropertyChanged (System.Reflection.PropertyInfo info)
            {
                if (PropertyChanged != null)
                {
                    //PropertyChanged (this, new MSDotnetAvalon.Windows.Data.PropertyChangedEventArgs (info));
                }
            }

            private void LoadXmlDoc ()
            {
                s_articlesXml = new System.Xml.XmlDocument ();
                s_articlesXml.Load (_runDir + @"\DrtFiles\HBROpt\Articles.xml");
            }

            public Article (string filename)
            {
                _filename = filename;
                if (s_articlesXml == null)
                    LoadXmlDoc ();

                XmlElement element = s_articlesXml.GetElementById (Filename.ToLower ());

                if (element == null)
                {
                    _title = System.IO.Path.GetFileNameWithoutExtension (Filename);
                }
                else
                {
                    _title = element.GetElementsByTagName ("Title")[0].InnerText;
                    _author = element.GetElementsByTagName ("Author")[0].InnerText;
                    _image = element.GetAttribute ("Image");
                    _displayModel = element.GetAttribute ("DisplayModel");
                }
            }

            public Article (string filename, string title, string author, string image)
            {
                _filename = filename;
                _title = title;
                _author = author;
                _image = image;
            }

            public Article (string filename, string title, string author, string image, string displayModel)
            {
                _filename = filename;
                _title = title;
                _author = author;
                _image = image;
                _displayModel = displayModel;
            }

            //-------------------------------------------------------------------
            public string Filename
            {
                get { return _filename; }
                set
                {
                    if (_filename != value)
                    {
                        _filename = value;
                        INotifyPropertyChanged (Filename_PropertyInfo);
                    }
                }
            }

            public string Title
            {
                get { return _title; }
                set
                {
                    if (_title != value)
                    {
                        _title = value;
                        INotifyPropertyChanged (Title_PropertyInfo);
                    }
                }
            }

            public string Author
            {
                get { return _author; }
                set
                {
                    if (_author != value)
                    {
                        _author = value;
                        INotifyPropertyChanged (Author_PropertyInfo);
                    }
                }
            }

            public string Image
            {
                get { return _image; }
                set
                {
                    if (_image != value)
                    {
                        _image = value;
                        INotifyPropertyChanged (Image_PropertyInfo);
                    }
                }
            }

            public string DisplayModel
            {
                get { return _displayModel; }
                set
                {
                    if (_displayModel != value)
                    {
                        _displayModel = value;
                        INotifyPropertyChanged (DisplayModel_PropertyInfo);
                    }
                }
            }
        }
    }
}
