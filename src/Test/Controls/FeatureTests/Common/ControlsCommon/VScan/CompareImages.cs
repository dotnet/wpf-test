using System;
using System.Drawing;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Logging;
using System.Xml;
using System.IO;

namespace Avalon.Test.ComponentModel.Utilities.VScanTools
{
    public class CompareImages
    {
        /// <summary>
        /// Constructor for class which is used to compare images using VScan.ImageComparator 
        /// </summary>
        public CompareImages()
        {
            firstimage = null;
            secondimage = null;
            errorimage = null;
        }

        /// <summary>
        /// Takes a screenshot of the first image of a UIElement.
        /// </summary>
        /// <param name="element">The UIElement which you want to take a screenshot of.</param>
        public void SetFirstImage(System.Windows.UIElement element)
        {
            if (element != null)
            {
                GlobalLog.LogEvidence("Getting BoundingRectangle for " + element.GetType().ToString());

                System.Drawing.Rectangle rect = ImageUtility.GetScreenBoundingRectangle(element);
                
                SetFirstImage(rect);
            }
            else
            {
                throw new NullReferenceException("The element must not be null.");
            }
        }

        /// <summary>
        /// Takes a screenshot of the first image in the area specified by the params.
        /// </summary>
        /// <param name="x">The Left value for the bounding box.</param>
        /// <param name="y">The Top value for the bounding box.</param>
        /// <param name="width">The Width of the bounding box.</param>
        /// <param name="height">The Height of the bounding box.</param>
        public void SetFirstImage(int x, int y, int width, int height)
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(x, y, width, height);

            SetFirstImage(rect);
        }

        /// <summary>
        /// Takes a screenshot of the first image in the area specified in the rect.
        /// </summary>
        /// <param name="rect">Specifies the region in which you want to take the screenshot for the first image.</param>
        public void SetFirstImage(System.Drawing.Rectangle rect)
        {
            if (rect != System.Drawing.Rectangle.Empty)
            {
                GlobalLog.LogEvidence("Using BoundingRectangle " + rect.ToString() + " for Snapshot");
                /*
                                ImageUtility iu = new ImageUtility();
                                iu.ScreenSnapshot(rect);
                                firstimage = new ImageAdapter(iu.Bitmap32Bits);
                */
                Bitmap bmp = ImageUtility.CaptureScreen(rect);
                firstimage = new ImageAdapter(bmp);

            }
            else
            {
                throw new NullReferenceException("The BoundingRect is Empty.");
            }
        }

        /// <summary>
        /// Uses the file specified as the first image.
        /// </summary>
        /// <param name="filename">Filename of the bitmap</param>
        public void SetFirstImage(string filename)
        {
            GlobalLog.LogEvidence("Using file " + filename + " for Snapshot image.");
            firstimage = new ImageAdapter(filename);
        }

        /// <summary>
        /// Uses the file specified as the second image.
        /// </summary>
        /// <param name="filename">Filename of the bitmap</param>
        public void SetSecondImage(string filename)
        {
            GlobalLog.LogEvidence("Using file " + filename + " for Snapshot image.");
            secondimage = new ImageAdapter(filename);
        }

        /// <summary>
        /// Takes a screenshot of the second image of a UIElement.
        /// </summary>
        /// <param name="element">The UIElement which you want to take a screenshot of.</param>
        public void SetSecondImage(System.Windows.UIElement element)
        {
            if (element != null)
            {
                GlobalLog.LogEvidence("Getting BoundingRectangle for " + element.GetType().ToString());

                System.Drawing.Rectangle rect = ImageUtility.GetScreenBoundingRectangle(element);

                SetSecondImage(rect);               
            }
            else
            {
                throw new NullReferenceException("The element must not be null.");
            }           
        }

        /// <summary>
        /// Takes a screenshot of the second image in the area specified by the params.
        /// </summary>
        /// <param name="x">The Left value for the bounding box.</param>
        /// <param name="y">The Top value for the bounding box.</param>
        /// <param name="width">The Width of the bounding box.</param>
        /// <param name="height">The Height of the bounding box.</param>
        public void SetSecondImage(int x, int y, int width, int height)
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(x, y, width, height);

            SetSecondImage(rect);
        }

        /// <summary>
        /// Takes a screenshot of the second image in the area specified in the rect.
        /// </summary>
        /// <param name="rect">Specifies the region in which you want to take the screenshot for the second image.</param>
        public void SetSecondImage(System.Drawing.Rectangle rect)
        {
            if (rect != System.Drawing.Rectangle.Empty)
            {
/*              Test.ComponentModel.AutomationFrwk.Log("Using BoundingRectangle " + rect.ToString() + " for Snapshot");

                ImageUtility iu = new ImageUtility();
                iu.ScreenSnapshot(rect);
                secondimage = new ImageAdapter(iu.Bitmap32Bits);
*/
                Bitmap bmp = ImageUtility.CaptureScreen(rect);
                secondimage = new ImageAdapter(bmp);

            }
            else
            {
                throw new NullReferenceException("The BoundingRect is Empty.");
            }
        }

        /// <summary>
        /// Used to specify the tolerence used for comparison of the images.
        /// </summary>
        public enum ImageTolerances
        {
            /// <summary>
            /// Not yet implemented.
            /// </summary>
            None,
            /// <summary>
            /// This Low Tolerence can be used on images with small antialising and color differences.
            /// </summary>
            Low,
            /// <summary>
            /// Not yet implemented.
            /// </summary>
            Medium,
            /// <summary>
            /// Not yet implemented.
            /// </summary>
            High,
            /// <summary>
            /// Not yet implemented.
            /// </summary>
            Custom
        }

        /// <summary>
        /// Compares the two images within the given tolerence to determine if they are equal or not.
        /// ImageTolerences.Low is the default used for this comparison.
        /// </summary>
        /// <returns>True if the images are equal, False if the images are not equal.</returns>
        public bool ImagesEqual()
        {
            return ImagesEqual(ImageTolerances.Low);
        }

        /// <summary>
        /// Compares the two images within the given tolerence to determine if they are equal or not.
        /// </summary>
        /// <param name="imgTol">Specifies the Tolerence to use for comparision.</param>
        /// <returns>True if the images are equal, False if the images are not equal within the tolerance specified.</returns>
        public bool ImagesEqual(ImageTolerances imgTol)
        {
            bool isImageEqual = false;
            if ((firstimage != null) && (secondimage != null))
            {   
                isImageEqual = ImagesEqualImpl(imgTol, string.Empty);
            }
            return isImageEqual;

        }


        /// <summary>
        /// compate two image based on the tolerance, if a tolerance file name is specified, use it. otherwise use ImageTolerances imgTol.
        /// </summary>
        /// <param name="imgTol"></param>
        /// <param name="tolearanceFileName"></param>
        /// <returns></returns>
        private bool ImagesEqualImpl(ImageTolerances imgTol, string tolearanceFileName)
        {
            ImageComparator comparator = new ImageComparator();
            comparator.FilterLevel = 0;
            comparator.ChannelsInUse = ChannelCompareMode.ARGB;
            if (!string.IsNullOrEmpty(tolearanceFileName))
            {
                SetImageTolerance(tolearanceFileName, comparator.Curve.CurveTolerance);
            }
            else
            {
                SetImageTolerance(imgTol, comparator.Curve.CurveTolerance);
            }

            bool imagesEqual = comparator.Compare(firstimage, secondimage, true);
            //bool imagesEqual = comparator.Compare(firstimage, secondimage);
            GlobalLog.LogEvidence("ErrorDiffSum = " + comparator.ErrorDiffSum.ToString());
            errorimage = comparator.GetErrorDifference(ErrorDifferenceType.IgnoreAlpha);

            return imagesEqual;
        }


        /// <summary>
        /// Set tolerance by a xml formatted file.
        /// </summary>
        /// <param name="tolearanceFileName"></param>
        /// <param name="curveTolerance"></param>
        private void SetImageTolerance(string tolearanceFileName, CurveTolerance curveTolerance)
        {
            string fileName = tolearanceFileName;
            string themeFileName = DisplayConfiguration.GetTheme() + "_" + tolearanceFileName;
            if (File.Exists(themeFileName))
            {
                fileName = themeFileName;
            }
            GlobalLog.LogStatus("Theme specific tolerance file is " + fileName);

            XmlDocument toleranceDocument = new XmlDocument();
            toleranceDocument.Load(fileName);
            curveTolerance.LoadTolerance(toleranceDocument);
            toleranceDocument.RemoveAll();
        }



        /// <!--summary>
        /// Compares the two images within the given tolerence file name..
        /// </summary>
        /// <param name="imgTol">Specifies the Tolerence to use for comparision.</param>
        /// <returns>True if the images are equal, False if the images are not equal within the tolerance specified.</returns-->
        public bool ImagesEqual(string tolearanceFileName)
        {
            bool isImageEqual = false;
            if ((firstimage != null) && (secondimage != null))
            {   
                isImageEqual =  ImagesEqualImpl(ImageTolerances.None, tolearanceFileName);
            }
            return isImageEqual;
        }


        /// <summary>
        /// Saves the images used in the comparison.
        /// Used for debugging purposes, failure analysis or master creation.
        /// </summary>
        /// <param name="filenamePrefix">Prefix for the image filename.</param>
        public void SaveBitmaps(string filenamePrefix)
        {
            if ((firstimage != null) && (secondimage != null))
            {
                if (filenamePrefix != "")
                {
                    filenamePrefix += "_";
                }

                ImageUtility.ToImageFile(firstimage, filenamePrefix + image1, System.Drawing.Imaging.ImageFormat.Bmp);
                ImageUtility.ToImageFile(secondimage, filenamePrefix + image2, System.Drawing.Imaging.ImageFormat.Bmp);

                GlobalLog.LogFile(filenamePrefix + image1);
                GlobalLog.LogFile(filenamePrefix + image2);
            }
        }

        /// <summary>
        /// Saves the images used in the comparison.
        /// Used for debugging purposes, failure analysis or master creation.
        /// </summary>
        public void SaveBitmaps()
        {
            SaveBitmaps("");            
        }

        /// <summary>
        /// Saves the error image of the comparison.
        /// </summary>
        /// <param name="filenamePrefix">Prefix for the image filename.</param>
        public void SaveErrorImage(string filenamePrefix)
        {
            if (errorimage != null)
            {
                if (filenamePrefix != "")
                {
                    filenamePrefix += "_";
                }

                ImageUtility.ToImageFile(errorimage, filenamePrefix + "errimg.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                GlobalLog.LogFile(filenamePrefix + "errimg.bmp");
            }
        }

        /// <summary>
        /// Saves the error image of the comparison.
        /// </summary>
        public void SaveErrorImage()
        {
            SaveErrorImage("");
        }       

        /// <summary>
        /// Saves all of the files associated with this Comparison.
        /// Including the first and second images as well as the error image.
        /// </summary>
        public void SaveAllData()
        {
            SaveBitmaps();
            SaveErrorImage();
        }

        /// <summary>
        /// Saves all of the files associated with this Comparison.
        /// Including the first and second images as well as the error image.
        /// </summary>
        /// <param name="filenamePrefix">Prefix for the image filename.</param>
        public void SaveAllData(string filenamePrefix)
        {
            SaveBitmaps(filenamePrefix);
            SaveErrorImage(filenamePrefix);
        }

        //
        private void SetImageTolerance(ImageTolerances imgTol, CurveTolerance curveTolerance)
        {
            if (imgTol == ImageTolerances.None)
            {
                //
            }

            if (imgTol == ImageTolerances.Low)
            {
                curveTolerance.Entries.Clear();
                curveTolerance.Entries.Add(0, 1.0);
                curveTolerance.Entries.Add(2, 0.0005);
                curveTolerance.Entries.Add(10, 0.0004);
                curveTolerance.Entries.Add(15, 0.0003);
                curveTolerance.Entries.Add(25, 0.0002);
                curveTolerance.Entries.Add(35, 0.0001);
                curveTolerance.Entries.Add(45, 0.00001);
            }

            if (imgTol == ImageTolerances.Medium)
            {
                //
            }

            if (imgTol == ImageTolerances.High)
            {
                //
            }
        }

        public IImageAdapter FirstImage
        {
            get
            {
                return firstimage;
            }
        }

        public IImageAdapter SecondImage
        {
            get
            {
                return secondimage;
            }
        }

        private IImageAdapter errorimage;

        private IImageAdapter firstimage;

        private IImageAdapter secondimage;

        protected string image1 = "rendimg1.bmp";

        protected string image2 = "rendimg2.bmp";

        private void Globallog(string p)
        {
            throw new NotImplementedException();
        }
    }
}
