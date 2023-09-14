using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Avalon.Test.ComponentModel.Utilities.VScanTools;
using Microsoft.Test.Discovery;
using Microsoft.Test.Graphics;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Serialization;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.Controls
{
    public class DataGridVisualValidation : XamlTest
    {
        #region Private and Protected Data

        private DataGrid myDataGrid;
        private MasterImageComparer masterImageComparer;
        protected static readonly int renderWaitTimeInMilliseconds = 500;
        protected string toleranceFilePath = null;
        protected UIElement elementToCompare = null;
        
        #endregion

        #region Constructors

        private DataGridVisualValidation()
            : base("")
        {
        }

        public DataGridVisualValidation(string fileName)
            : base(fileName)
        {
        }

        #endregion

        #region Public Properties

        public DataGrid DataGrid
        {
            get { return myDataGrid; }
            set { myDataGrid = value; }
        }

        #endregion

        #region Public Members

        public TestResult Initialize()
        {
            // Prepare the test case window for rendering
            Window.Width = 700;
            Window.Height = 700;
            Window.Left = 0;
            Window.Top = 0;
            Window.Topmost = true;
            Window.ShowsNavigationUI = false;

            // Init the MasterImageComparer
            MasterIndex masterIndex = new MasterIndex();
            masterIndex.Path = "FeatureTests\\Controls\\";
            masterIndex.AddCriteria(MasterMetadata.ThemeDimension, 1);
            masterIndex.AddCriteria(MasterMetadata.DwmDimension, 1);
            masterIndex.AddCriteria(MasterMetadata.DpiDimension, 1);
            masterIndex.AddCriteria(MasterMetadata.OsVersionDimension, 1);

            masterImageComparer = new MasterImageComparer(masterIndex);
            masterImageComparer.ResizeWindowForDpi = false;
            masterImageComparer.StabilizeWindowBeforeCapture = false;

            // load the default tolerance file which ignores text aliasing
            System.Xml.XmlDocument toleranceXml = new System.Xml.XmlDocument();
            toleranceXml.Load(toleranceFilePath);
            ImageComparisonSettings toleranceSettings = ImageComparisonSettings.CreateCustomTolerance(toleranceXml.DocumentElement);
            masterImageComparer.ToleranceSettings = toleranceSettings;


            // Get a handle to the DataGrid
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            myDataGrid = (DataGrid)RootElement.FindName("myDataGrid");

            if (myDataGrid == null)
            {
                throw new TestValidationException("myDataGrid not found.");
            }

            WaitFor(renderWaitTimeInMilliseconds);

            return TestResult.Pass;
        }

        public TestResult ValidateDataGridAgainstMasterImage(string masterImageName)
        {
            masterImageComparer.MasterIndex.FileName = masterImageName;
            WaitFor(renderWaitTimeInMilliseconds);

            if (null == elementToCompare)
            {
                elementToCompare = DataGrid;
            }

            if (masterImageComparer.Compare(elementToCompare))
            {
                LogComment("Rendered " + masterImageName + " matches expected.");
                return TestResult.Pass;
            }

            LogComment("Rendered " + masterImageName + " does NOT match expected.");

            return TestResult.Fail;
        }

        #endregion
    }
}
