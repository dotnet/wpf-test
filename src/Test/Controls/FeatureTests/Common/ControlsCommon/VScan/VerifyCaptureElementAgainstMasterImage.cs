using System;
using System.Windows;
using System.Drawing;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.RenderingVerification.Model.Analytical;
using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.Utilities.VScanTools
{
    public static class VScanHelper
    {
        /// <summary>
        /// Verify the capture element against master image.
        /// </summary>
        /// <param name="captureElement"></param>
        /// <param name="masterFileName"></param>
        /// <param name="tolerenceFilePath"></param>
        /// <returns>true</returns>
        public static bool VerifyCaptureElementAgainstMasterImage(UIElement captureElement, string masterFileName, string tolerenceFilePath)
        {
            Bitmap capture = ImageUtility.CaptureElement(captureElement);
            ImageAdapter captureAdapter = new ImageAdapter(capture);

            bool passed = false;

            ImageAdapter masterImageAdapter = new ImageAdapter(masterFileName);
            if (masterImageAdapter != null)
            {
                ImageComparator comparator = CreateImageComparator(tolerenceFilePath);
                passed = comparator.Compare(masterImageAdapter, captureAdapter);
            }

            if (!passed || masterImageAdapter == null)
            {
                string renderedFileName = "rendered_" + masterFileName;
                ImageUtility.ToImageFile(captureAdapter, renderedFileName);

                GlobalLog.LogFile(renderedFileName);
                GlobalLog.LogFile(masterFileName);
            }

            return passed;
        }

        /// <summary>
        /// Create Image Comparator.
        /// </summary>
        /// <param name="toleranceFilePath"></param>
        /// <returns>ImageComparator</returns>
        private static ImageComparator CreateImageComparator(string toleranceFilePath)
        {
            ImageComparator comparator;
            if (!string.IsNullOrEmpty(toleranceFilePath))
            {
                CurveTolerance tolerance = new CurveTolerance();
                tolerance.LoadTolerance(toleranceFilePath);
                comparator = new ImageComparator(tolerance);
                GlobalLog.LogStatus("Using custom tolerance (" + toleranceFilePath + ")");
            }
            else
            {
                comparator = new ImageComparator();
                GlobalLog.LogStatus("Using default tolerance");
            }
            return comparator;
        }
    }

}
