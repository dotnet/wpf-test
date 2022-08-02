// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace HBRApp
{
    using System;
    using System.Windows;
    using System.Windows.Navigation;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Input; 

    public partial class TOCandFrame
    {
        private double _scaleSize = 1;  //Current zoom magnification
        private HBRApp.HBRDemo _thisApplication;

        private void Init (object sender, System.EventArgs args)
        {
            _thisApplication = (HBRApp.HBRDemo)System.Windows.Application.Current;
            foreach (HBRApp.ArticleList.Article article in _thisApplication.Articles)
            {
                TOC.Items.Add (article);
            }

            HBRPageViewer.Document = LoadXaml(@"DrtFiles\HbrOpt\HBRARTICLEplaceholder2.xaml");            

            if (HBRPageViewer.Document.DocumentPaginator.IsPageCountValid)
            {
                OnPaginationCompleted(null, null);
            }
            else
            {
                ((DynamicDocumentPaginator)HBRPageViewer.Document.DocumentPaginator).PaginationCompleted += new EventHandler(OnPaginationCompleted);
            }

        }
        
        public void OnPaginationCompleted(
                                 object sender,
                                 EventArgs args         
                )
        {            

            // <!-- Performance Instrumentation : ProcessEnv -->                      
            
        }

        // <!-- Performance Instrumentation : PerfVars -->
        // <!-- Performance Instrumentation : ResizeVars -->
        // <!-- Performance Instrumentation : ScrollVars -->
        // <!-- Performance Instrumentation : HoverVars -->

        // <!-- Performance Instrumentation : BeginHandleIdle -->
        // <!-- Performance Instrumentation : HandleIdleResize -->
        // <!-- Performance Instrumentation : HandleIdleScroll -->
        // <!-- Performance Instrumentation : HandleIdleHover -->
        // <!-- Performance Instrumentation : EndHandleIdle -->

        // <!-- Performance Instrumentation : WorkerResize -->
        // <!-- Performance Instrumentation : WorkerScroll -->
        // <!-- Performance Instrumentation : WorkerHover -->
        // <!-- Performance Instrumentation : UiUtils -->

        private void TestArticleList (Object sender, RoutedEventArgs args)
        {
//          ArticleList al = new ArticleList ();
//          MessageBox.Show(al.Count.ToString ());
//          foreach (HBRApp.ArticleList.Article article in al)
//          {
//              TOC.Items.Add (article);
//          }

        }

        void LoadArticle (Object sender, SelectionChangedEventArgs args)
        {
            HBRApp.ArticleList.Article selectedItem = ((ListBox)sender).SelectedItem as HBRApp.ArticleList.Article;
                if (selectedItem != null)
                {
                    HBRPageViewer.Document = LoadXaml(selectedItem.Filename);
                    //FadeOutHolder.Timeline.BeginIn (0);
                    //FadeInHolder.Timeline.EndIn (0);

                    if (HBRPageViewer.Document.DocumentPaginator.IsPageCountValid)
                    {
                        OnPaginationCompleted(null, null);
                    }
                    else
                    {
                        ((DynamicDocumentPaginator)HBRPageViewer.Document.DocumentPaginator).PaginationCompleted += new EventHandler(OnPaginationCompleted);
                    }
                }
        }


        public void ShowHideTOC (Object element, RoutedEventArgs args)
        {
            if (TOCPanel.Visibility == Visibility.Visible)
            {
                TOCPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                TOCPanel.Visibility = Visibility.Visible;
            }
        }

        private void Zoom (object sender, MouseButtonEventArgs args)
        {
            Console.WriteLine ("Zoom");
            Console.WriteLine ("Left control=" + Keyboard.GetKeyStates (Key.LeftCtrl).ToString ());
            Console.WriteLine ("Right control=" + Keyboard.GetKeyStates (Key.RightCtrl).ToString ());

            if (Keyboard.IsKeyDown (Key.LeftCtrl) || Keyboard.IsKeyDown (Key.RightCtrl) )
            {
                //Console.WriteLine ("Left control=" + Keyboard.GetKeyState (Key.LeftCtrl).ToString ());
                //Console.WriteLine ("Right control=" + Keyboard.GetKeyState (Key.RightCtrl).ToString ());

                Point mousePosition = args.GetPosition (null);
                Console.WriteLine ("X=" + mousePosition.X.ToString () + " Y=" + mousePosition.Y);
                double oldScale = _scaleSize;
                if (args.LeftButton == MouseButtonState.Pressed)
                {
                    Console.WriteLine ("LeftButton is Down");
                    _scaleSize = _scaleSize * 2;
                }

                if (args.RightButton == MouseButtonState.Pressed)
                {
                    Console.WriteLine ("Right Button is Down");
                    _scaleSize = _scaleSize * .5;
                    mousePosition = new Point (0, 0);
                }
                if (_scaleSize < 1)
                    _scaleSize = 1;
                Console.WriteLine ("Scale is " +_scaleSize.ToString());
                Console.WriteLine ("");
                DoubleAnimation danim = new DoubleAnimation (_scaleSize, new Duration(TimeSpan.FromMilliseconds(500)));
                danim.FillBehavior = FillBehavior.HoldEnd;
                ScaleTransform scaleTrans = new ScaleTransform(oldScale,oldScale,mousePosition.X,mousePosition.Y);
                scaleTrans.BeginAnimation(ScaleTransform.ScaleXProperty,danim);
                scaleTrans.BeginAnimation(ScaleTransform.ScaleYProperty,danim);
                RootDecorator.LayoutTransform = scaleTrans;
            }
        }

        /// <summary>
        /// Load Xaml content from file.
        /// </summary>
        /// <param name="filename">Relative filename to load.</param>
        private System.Windows.Documents.IDocumentPaginatorSource LoadXaml(string filename)
        {
            System.IO.FileStream fileStream = new System.IO.FileStream(filename, System.IO.FileMode.Open);

            object newXaml = System.Windows.Markup.XamlReader.Load(fileStream);
            // Leave fileStream open due to async loading.
            //  fileStream.Close();

            if (newXaml is System.Windows.Documents.IDocumentPaginatorSource)
            {
                return (System.Windows.Documents.IDocumentPaginatorSource)newXaml;
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "PageViewer can only accept children of type IDocumentFormatter.");
                return null;
            }
        }


    }
}

