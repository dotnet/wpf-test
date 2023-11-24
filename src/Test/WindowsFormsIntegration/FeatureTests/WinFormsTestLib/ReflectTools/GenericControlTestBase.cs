using System;
using System.Collections;
using System.Windows.Forms;

using ReflectTools;
using WFCTestLib.Log;
using WFCTestLib.Util;

namespace ReflectTools {

    //
    // This class provides an easy framework to implement a generic Control test using
    // ReflectBase.  It provides a property, TestedControl which is an instance of the
    // control to be tested.
    //
    // To specify the control to be tested, use the "/control:<control>" command-line
    // argument.  This makes it easy to set up a bunch of MadDog contexts for a
    // generic testcase that runs through every control.
    //
    public abstract class GenericControlTestBase : ReflectBase {
        private Control testedControl;

        protected GenericControlTestBase(string[] args) : base(args) { }

        //
        // An instance of the control to be tested. This is guaranteed to be set to
        // a control at any time after command-line parameters have been processed.
        //
        protected Control TestedControl {
            get { return testedControl; }
            set { testedControl = value; }
        }
		
		protected Type TestedType {
			get { return testedControl.GetType(); }
		}

        protected override void ProcessCommandLineParameters() {
            ArrayList paramsToRemove = new ArrayList();
            bool argFound = false;

            foreach ( string arg in CommandLineParameters ) {
                string argUpper = arg.ToUpper(System.Globalization.CultureInfo.InvariantCulture);

                if ( argUpper.StartsWith("/CONTROL:") ) {
                    string controlName = argUpper.Substring(9);
                    argFound = true;
                    paramsToRemove.Add(arg);

                    switch ( controlName ) {
                        case "BUTTON" :             TestedControl = new Button(); break;
                        case "CHECKBOX" :           TestedControl = new CheckBox(); break;
                        case "CHECKEDLISTBOX" :     TestedControl = new CheckedListBox(); break;
                        case "COMBOBOX" :           TestedControl = new ComboBox(); break;
                        case "CONTAINERCONTROL" :   TestedControl = new ContainerControl(); break;
                        case "DATETIMEPICKER" :     TestedControl = new DateTimePicker(); break;
                        case "DOMAINUPDOWN" :       TestedControl = new DomainUpDown(); break;
                        case "TEXTBOX" :            TestedControl = new TextBox(); break;
                        case "FORM" :               TestedControl = new Form(); break;
                        case "GROUPBOX" :           TestedControl = new GroupBox(); break;
                        case "HSCROLLBAR" :         TestedControl = new HScrollBar(); break;
                        case "LABEL" :              TestedControl = new Label(); break;
                        case "LINKLABEL" :          TestedControl = new LinkLabel(); break;
                        case "LISTBOX" :            TestedControl = new ListBox(); break;
                        case "LISTVIEW" :           TestedControl = new ListView(); break;
                        case "MONTHCALENDAR" :      TestedControl = new MonthCalendar(); break;
                        case "NUMERICUPDOWN" :      TestedControl = new NumericUpDown(); break;
                        case "PANEL" :              TestedControl = new Panel(); break;
                        case "PICTUREBOX" :         TestedControl = new PictureBox(); break;
                        case "PROGRESSBAR" :        TestedControl = new ProgressBar(); break;
                        case "RADIOBUTTON" :        TestedControl = new RadioButton(); break;
                        case "RICHTEXTBOX" :        TestedControl = new RichTextBox(); break;
                        case "SCROLLABLECONTROL" :  TestedControl = new ScrollableControl(); break;
                        case "SPLITTER" :           TestedControl = new Splitter(); break;
                        case "TABCONTROL" :         TestedControl = new TabControl(); break;
                        case "TRACKBAR" :           TestedControl = new TrackBar(); break;
                        case "TREEVIEW" :           TestedControl = new TreeView(); break;
                        case "VSCROLLBAR" :         TestedControl = new VScrollBar(); break;

                        /* KevinTao: Added 3/25/04

                           May eventually need:
                                DataGridView
                                ToolStrip (and subclasses)
                                Others?
                        */
                        case "SPLITCONTAINER" :     TestedControl = new SplitContainer(); break;
                        case "USERCONTROL" :        TestedControl = new UserControl(); break;
                        case "MASKEDTEXTBOX":       TestedControl = new MaskedTextBox(); break;

                        default :
                            ErrorExit("Control \"" + controlName + "\" is not valid for this test");
                            break;
                    }
                }
            }

            foreach ( string arg in paramsToRemove )
                CommandLineParameters.Remove(arg);

            base.ProcessCommandLineParameters();

            // Do this after we call the base method so it will handle other
            // parameters such as /?
            if ( !argFound )
                ErrorExit("Please specify a control to use using the /control:<control> argument");
        }

        protected override void PrintHelp() {
            base.PrintHelp();
            Console.WriteLine();
            Console.WriteLine("GenericControlTestBase command-line parameters:");
            Console.WriteLine("  /control:<control> Run this test using the specified control");
        }

        protected override void LogCommandLineParameters() {
            base.LogCommandLineParameters();
            LogCommandLineParameter("control", TestedControl.GetType().Name);
        }
    }
}
