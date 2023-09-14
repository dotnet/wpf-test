using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Microsoft.Windows.Controls.Ribbon;
using System.ComponentModel;

namespace RibbonApplicationMenuTests
{
    /// <summary>
    /// Interaction logic for RAMPage1.xaml
    /// </summary>
    public partial class RAMPage1 : Page
    {
        public RAMPage1()
        {
            InitializeComponent();
            DataContext = new ViewModel(this);
            LastCalledFunction = "None";
            MRF = new MostRecentFiles(13);
            recentDocs.DataContext = MRF;
        }

        #region Public Properties

        public string LastCalledFunction
        {
            get;
            private set;
        }

        public RibbonApplicationMenu Menu
        {
            get {return RAM;}
        }

        public int RecentDocsLength
        {
            get { return MRF.Count; }
            set
            {
                MRF.Clear();
                MRF.populate(value);
            }
        }

        private int footerLength;
        public int FooterLength
        {
            get { return footerLength; }
            set
            {
                footerDockPanel.Children.Clear();
                for (int i = 0; i < value; i++)
                {
                    RibbonSplitButton but = new RibbonSplitButton();
                    DockPanel.SetDock(but, Dock.Right);
                    but.Margin = new Thickness(2);
                    but.ToolTipTitle = "Tool tip title #"+i;
//                    but.SmallImageSource = new BitmapImage(new Uri("Images\\ExitHS.png"));
                    footerDockPanel.Children.Add(but);
                }
                footerLength = value;
            }
        }

        

        #endregion

        #region Fields
        private static Type OwnerType = typeof(RAMPage1);
        public MostRecentFiles MRF;
        #endregion

        /// <summary>
        /// Perform API Tests against the ribbon application menu
        /// </summary>
        public bool DoAPITests()
        {            
            bool result=true;
            result = assert(RAM.KeyTip,"R");
            
            //footerpane
            DockPanel footerpane = RAM.FooterPaneContent as DockPanel;
            System.Collections.IEnumerator docklist = footerpane.Children.GetEnumerator();

            while (docklist.MoveNext()) 
            {
                if (docklist.Current.GetType() == typeof(RibbonSplitButton))
                {
                    result = assert(((RibbonSplitButton)docklist.Current).Label, "Exit");
                }
                if (docklist.Current.GetType() == typeof(RibbonButton))
                {
                    result = assert(((RibbonButton)docklist.Current).Label, "Options");
                }
            } 

            //auxiliarypane
            RibbonGallery auxpane = RAM.AuxiliaryPaneContent as RibbonGallery;
            result = assert(auxpane.Name, "AuxPane");

            return result;
        }



        private bool assert(string a, string b)
        {
        
             if(0 != String.Compare(a,b))
             {
                 return false;
             }
            return true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            LastCalledFunction = "Save";
        }

        private void RAM_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LastCalledFunction = "RAM";
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            Console.Write(New);
        }
    }


    public class MostRecentFiles : ObservableCollection<MostRecentFile> 
    {
        public MostRecentFiles()
        {
            Add(new MostRecentFile("First document.docx", true));
            Add(new MostRecentFile("Second document.docx", false));
            Add(new MostRecentFile("Ribbon Design Document.docx", false));
        }

        public MostRecentFiles(int l)
        {
            populate(l);
        }

        public void populate(int l)
        {
            for (int i = 0; i < l; i++)
            {
                Add(new MostRecentFile(i + "st document.docx", false));
            }
        }
    }

    public class MostRecentFile
    {
        public MostRecentFile(string name, bool isFixed)
        {
            Name = name;
            IsFixed = isFixed;
        }

        public string Name
        {
            get;
            set;
        }

        public bool IsFixed
        {
            get { return _isFixed; }
            set { _isFixed = value; }
        }
        bool _isFixed;
    }
}
