using System;
using System.Drawing;
using System.IO;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;

namespace Avalon.Test.ComponentModel.Utilities.VScanTools
{
    public class MasterRenderImagesCompare : CompareImages
    {
        /// <summary>
        /// Constructor for class which is used to compare images using VScan.ImageComparator 
        /// </summary>
        public MasterRenderImagesCompare()
        {
            image1 = "master.bmp";
            image2 = "render.bmp";
        }

        /// <summary>
        /// Takes a screenshot of the master image of a UIElement.
        /// </summary>
        /// <param name="element">The UIElement which you want to take a screenshot of.</param>
        public void SetMasterImage(System.Windows.UIElement element)
        {
            SetFirstImage(element);
        }

        /// <summary>
        /// Takes a screenshot of the master image in the area specified by the params.
        /// </summary>
        /// <param name="x">The Left value for the bounding box.</param>
        /// <param name="y">The Top value for the bounding box.</param>
        /// <param name="width">The Width of the bounding box.</param>
        /// <param name="height">The Height of the bounding box.</param>
        public void SetMasterImage(int x, int y, int width, int height)
        {
            SetFirstImage(x, y, width, height);
        }

        /// <summary>
        /// Takes a screenshot of the master image in the area specified in the rect.
        /// </summary>
        /// <param name="rect">Specifies the region in which you want to take the screenshot for the master image.</param>
        public void SetMasterImage(System.Drawing.Rectangle rect)
        {
            SetFirstImage(rect);
        }

        /// <summary>
        /// Look for theme specific file first, by appending current theme
        /// to the given file name, if not exists then use the one given
        /// </summary>
        /// <param name="filename">Filename of the bitmap</param>
        public void SetMasterImage(string filename)
        {
            GlobalLog.LogStatus("Current theme testing " + DisplayConfiguration.GetTheme());
            string currentTheme = DisplayConfiguration.GetTheme();
            string themeSpecificMaster;
            if (currentTheme == "Luna")
            {                
                string LunaThemeStyle = DisplayConfiguration.GetThemeStyle();
                GlobalLog.LogStatus("Current Luna theme Style:  " + LunaThemeStyle);
                if (LunaThemeStyle == "NormalColor")
                {
                    themeSpecificMaster = "Luna" + "_" + filename;
                }  
                else
                {
                    themeSpecificMaster = LunaThemeStyle + "_" + filename;
                }
            }
            else
            {
                themeSpecificMaster = currentTheme + "_" + filename;
            }
            if (File.Exists(themeSpecificMaster))
            {              
                masterFilename = themeSpecificMaster;
            }
            else
            {
                masterFilename = filename;
                GlobalLog.LogStatus("Theme specific master image file " + themeSpecificMaster + " not found - using the given filename");
            }

            SetFirstImage(masterFilename);
        }

        /// <summary>
        /// Uses the file specified as the render image.
        /// </summary>
        /// <param name="filename">Filename of the bitmap</param>
        public void SetRenderImage(string filename)
        {
            SetSecondImage(filename);
        }

        /// <summary>
        /// Takes a screenshot of the render image of a UIElement.
        /// </summary>
        /// <param name="element">The UIElement which you want to take a screenshot of.</param>
        public void SetRenderImage(System.Windows.UIElement element)
        {
            SetSecondImage(element);
        }

        /// <summary>
        /// Takes a screenshot of the render image in the area specified by the params.
        /// </summary>
        /// <param name="x">The Left value for the bounding box.</param>
        /// <param name="y">The Top value for the bounding box.</param>
        /// <param name="width">The Width of the bounding box.</param>
        /// <param name="height">The Height of the bounding box.</param>
        public void SetRenderImage(int x, int y, int width, int height)
        {
            SetSecondImage(x, y, width, height);
        }

        /// <summary>
        /// Takes a screenshot of the render image in the area specified in the rect.
        /// </summary>
        /// <param name="rect">Specifies the region in which you want to take the screenshot for the render image.</param>
        public void SetRenderImage(System.Drawing.Rectangle rect)
        {
            SetSecondImage(rect);
        }

        /// <summary>
        /// Path to the Master, something like this
        /// "*\REDIST\Client\Wcptests\ComponentModel\ListBox\BVT\VScan"
        /// </summary>
        public string MasterPath
        {
            get
            {
                return masterPath;
            }
            set
            {
                masterPath = value;
            }
        }

        /// <summary>
        /// Save VscanPackage, master can be updated from Failure analysis client using
        /// this package
        /// </summary>
        public void SaveVScanPackage()
        {
            Package vscanPackage = null;

            vscanPackage = Package.Create(true);

            masterPath = masterPath + "\\" + masterFilename;

            vscanPackage.MasterBitmap = ImageUtility.ToBitmap(FirstImage);
            vscanPackage.CapturedBitmap = ImageUtility.ToBitmap(SecondImage);
            vscanPackage.MasterSDLocation = masterPath;
            vscanPackage.PackageName = System.Environment.CurrentDirectory + @"\MasterRender.VScan";
            vscanPackage.PackageCompare = PackageCompareTypes.ImageCompare;
            vscanPackage.Save();
            TestLog.Current.LogFile(System.Environment.CurrentDirectory + @"\MasterRender.VScan");
        }

        private string masterFilename;
        private string masterPath;
    }
}
