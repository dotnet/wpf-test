// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *
 *   Program:   AnimationOnElement
 *
 ************************************************************/
// $Id:$ $Change:$
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Markup;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Threading;


namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.PropertyMethodEvent</area>
    /// <priority>3</priority>
    /// <description>
    /// Verify animation on each DP on each specified DO, using Reflection.
    /// </description>
    /// </summary>
    [Test(3, "Animation.PropertyMethodEvent", "AnimationOnElementTest", Timeout=365, SupportFiles=@"FeatureTests\Animation\MarkupElements.xaml,FeatureTests\Animation\borg.jpg", Versions="3.0sp2,3.5sp1,4.0+")]

    class AnimationOnElementTest : StepsTest
    {
        #region Test case members

        private Canvas                          _body                        = null;
        private static string                   s_testElement                 = "";
        private static string                   s_elementFile                 = "";
        private static string                   s_inputString                 = "";
        public  static bool                     testPassed                  = false;
        public  static NameValueCollection      applicationProps            = null;
        public static DispatcherSignalHelper    dispatcherSignalHelper;

        #endregion


        #region Constructor

        [Variation("VIEWPORT3D",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("BUTTON", Disabled = true)]
        [Variation("BOLD", Disabled = true)]
        [Variation("LINE")]
        [Variation("TABLEROWGROUP", Disabled = true)]
        [Variation("BORDER")]
        [Variation("CANVAS", Priority=1)]
        [Variation("CHECKBOX")]
        [Variation("COLUMNDEFINITION", Disabled = true)]
        [Variation("COMBOBOX", Priority=2, Disabled = true)]
        [Variation("COMBOBOXITEM", Disabled = true)]
        [Variation("CONTENTCONTROL", Disabled = true)]
        [Variation("CONTENTPRESENTER", Disabled = true)]
        [Variation("CONTEXTMENU", Disabled = true)]
        [Variation("CONTROL", Disabled = true)]
        [Variation("DOCKPANEL", Disabled = true)]
        [Variation("ELLIPSE", Priority=2, Disabled = true)]
        [Variation("FOOTER", Disabled = true)]
        [Variation("FRAME", Disabled = true)]
        [Variation("GLYPHS", Disabled = true)]
        [Variation("GRID", Priority=2, Disabled = true)]
        [Variation("HEADER", Disabled = true)]
        [Variation("HEADING", Disabled = true)]
        [Variation("HORIZONTALSLIDER", Disabled = true)]
        [Variation("HYPERLINK", Disabled = true)]
        [Variation("IMAGE", Priority=1)]
        [Variation("RUN", Disabled = true)]
        [Variation("ITALIC", Disabled = true)]
        [Variation("ITEMSCONTROL", Disabled = true)]
        [Variation("LABEL", Disabled = true)]
        [Variation("LINEBREAK", Disabled = true)]
        [Variation("LIST", Disabled = true)]
        [Variation("LISTBOX", Priority=2)]
        [Variation("LISTBOXITEM", Priority=2, Disabled = true)]
        [Variation("MENU", Disabled = true)]
        [Variation("PARAGRAPH", Disabled = true)]
        [Variation("PATH")]
        [Variation("POLYGON", Disabled = true)]
        [Variation("POLYLINE", Disabled = true)]
        [Variation("RADIOBUTTONLIST", Disabled = true)]
        [Variation("RADIONBUTTON", Disabled = true)]
        [Variation("RECTANGLE")]
        [Variation("REPEATBUTTON", Disabled = true)]
        [Variation("TABLEROW", Disabled = true)]
        [Variation("SCROLLVIEWER", Disabled = true)]
        [Variation("SECTION", Disabled = true)]
        [Variation("SUBSCRIPT", Disabled = true)]
        [Variation("SUPERSCRIPT", Disabled = true)]
        [Variation("TABLE", Priority=2)]
        [Variation("TEXT", Priority=1, Disabled = true)]
        [Variation("TEXTBOX", Priority=1, Disabled = true)]
        [Variation("FLOWDOCUMENTSCROLLVIEWER", Disabled = true)]
        [Variation("THUMB", Disabled = true)]
        [Variation("TOOLTIP", Disabled = true)]
        [Variation("UNDERLINE", Disabled = true)]
        [Variation("VERTICALSLIDER", Disabled = true)]
        [Variation("TEXTBLOCK", Priority=1, Disabled = true)]
        [Variation("EXPANDER", Disabled = true)]
        [Variation("PROGRESSBAR", Disabled = true)]
        [Variation("WRAPPANEL", Disabled = true)]
        [Variation("ACCESSTEXT", Disabled = true)]
        [Variation("DECORATOR", Disabled = true)]
        [Variation("TOGGLEBUTTON", Disabled = true)]
        [Variation("GRIDSPLITTER", Disabled = true)]
        [Variation("GROUPBOX", Disabled = true)]
        [Variation("GROUPITEM", Disabled = true)]
        [Variation("HEADEREDCONTENTCONTROL", Disabled = true)]
        [Variation("INKCANVAS", Disabled = true)]
        [Variation("INKPRESENTER", Disabled = true)]
        [Variation("ITEMSPRESENTER", Disabled = true)]
        [Variation("LISTVIEW", Disabled = true)]
        [Variation("LISTVIEWITEM", Disabled = true)]
        [Variation("MENUITEM", Disabled = true)]
        [Variation("PASSWORDBOX", Disabled = true)]
        [Variation("BULLETDECORATOR", Disabled = true)]
        [Variation("DOCUMENTPAGEVIEW", Disabled = true)]
        [Variation("POPUP", Disabled = true)]
        [Variation("RESIZEGRIP", Disabled = true)]
        [Variation("SCROLLBAR", Priority=1, Disabled = true)]
        [Variation("STATUSBAR", Disabled = true)]
        [Variation("STATUSBARITEM", Disabled = true)]
        [Variation("TABPANEL", Disabled = true)]
        [Variation("TICKBAR", Disabled = true)]
        [Variation("TOOLBAROVERFLOWPANEL", Disabled = true)]
        [Variation("STACKPANEL", Disabled = true)]
        [Variation("TOOLBARPANEL", Disabled = true)]
        [Variation("TRACK", Disabled = true)]
        [Variation("UNIFORMGRID", Disabled = true)]
        [Variation("SEPARATOR", Disabled = true)]
        [Variation("TABCONTROL", Disabled = true)]
        [Variation("TABITEM", Disabled = true)]
        [Variation("TOOLBARTRAY", Disabled = true)]
        [Variation("TOOLBAR", Disabled = true)]
        [Variation("TREEVIEW", Disabled = true)]
        [Variation("TREEVIEWITEM", Disabled = true)]
        [Variation("USERCONTROL", Disabled = true)]
        [Variation("VIEWBOX", Disabled = true)]
        [Variation("VIEWPORT3D", Disabled = true)]
        [Variation("VIRTUALIZINGSTACKPANEL", Disabled = true)]
        [Variation("ADORNERDECORATOR", Disabled = true)]
        [Variation("BLOCKUICONTAINER", Disabled = true)]
        [Variation("DOCUMENTVIEWER", Disabled = true)]
        [Variation("FIGURE", Disabled = true)]
        [Variation("FIXEDDOCUMENT", Disabled = true)]
        [Variation("FIXEDPAGE", Disabled = true)]
        [Variation("FLOATER", Disabled = true)]
        [Variation("FLOWDOCUMENT", Disabled = true)]
        [Variation("FLOWDOCUMENTPAGEVIEWER", Disabled = true)]
        [Variation("FLOWDOCUMENTREADER", Disabled = true)]
        [Variation("GRIDVIEWCOLUMNHEADER", Disabled = true)]
        [Variation("GRIDVIEWHEADERROWPRESENTER", Disabled = true)]
        [Variation("GRIDVIEWROWPRESENTER", Disabled = true)]
        [Variation("HEADEREDITEMSCONTROL", Disabled = true)]
        [Variation("INLINEUICONTAINER", Disabled = true)]
        [Variation("MEDIAELEMENT")]
        [Variation("PAGECONTENT", Disabled = true)]
        [Variation("RICHTEXTBOX", Disabled = true)]
        [Variation("ROWDEFINITION", Disabled = true)]
        [Variation("SPAN", Disabled = true)]
        [Variation("TABLECELL", Disabled = true)]
        [Variation("TABLECOLUMN", Disabled = true)]

        /******************************************************************************
        * Function:          AnimationOnElementTest Constructor
        ******************************************************************************/
        public AnimationOnElementTest(string testValue)
        {
            s_inputString = testValue;
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize: create a new Window and add content to it.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        TestResult Initialize()
        {
            GlobalLog.LogStatus("Initialize");

            dispatcherSignalHelper = new DispatcherSignalHelper();

            s_elementFile = "MarkupElements.xaml";
            s_testElement = s_inputString;

            Window appWin = new Window();
            appWin.Width            = 400;
            appWin.Height           = 400;
            appWin.Title            = "Animation Test";
            appWin.Left             = 0;
            appWin.Top              = 0;
            appWin.WindowStyle      = WindowStyle.None;
            appWin.ContentRendered += new EventHandler(OnContentRendered);

            _body = new Canvas();
            _body.Width              = 400;
            _body.Height             = 400;
            _body.Background         = Brushes.Teal;
            appWin.Content = _body;
            appWin.Show();

            return TestResult.Pass;
        }

        private void OnContentRendered(object sender, EventArgs e)
        {

            applicationProps = new NameValueCollection();
            IntegrationUtilities.AddApplicationProperty( CommonConstants.flagTestElement, s_testElement);
            DependencyObject[] testItems = IntegrationUtilities.GetStockElementAndParent(s_elementFile, s_testElement);


            if (testItems[0] != null)
            {
                // build test (100 millisecond intervals)
                FilteredObjectAnimationDriver ad = new FilteredObjectAnimationDriver(s_elementFile,100);

                // set exception handling
//                ad.HandleDispatcherExceptions = true;

                // grab the filter from the property bag if set ...
                string filter = IntegrationUtilities.GetApplicationProperty( CommonConstants.flagFilterString );
                if (filter != null && filter != String.Empty)
                {
                    ad.Filter = filter;
                }
                // start test
                ad.Test((DependencyObject)testItems[0], _body );
             }
            else
            {
                GlobalLog.LogEvidence("ERROR!!! OnContentRendered: Could not create element " + s_testElement + " from stock markup.");
                testPassed = false;
                dispatcherSignalHelper.Signal("AnimationDone", TestResult.Fail);
            }
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns>A TestResult, indicating pass or fail</returns>
        TestResult Verify()
        {
            dispatcherSignalHelper.WaitForSignal("AnimationDone", 360000);

            if (testPassed)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
        #endregion
    }
}
