using System;
using System.Windows;
using System.Drawing;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.RenderingVerification.Model.Analytical;
using Microsoft.Test.Logging;


namespace Avalon.Test.ComponentModel.Utilities.VScanTools
{
	public class CompareModels
	{
        VScan _vscanImage = null;
        private string _modelName = string.Empty;
		System.Drawing.Rectangle rect;

        /// <summary>
		/// Constructor for class which is used to compare images using VScan.ImageComparator 
		/// </summary>
		public CompareModels(object captureelement, string modelName)
		{
           	if (captureelement is UIElement)
			{
				UIElement element = captureelement as UIElement;
				rect = ImageUtility.GetScreenBoundingRectangle(element);
			}
			else if (captureelement is Rectangle)
			{
				rect = (Rectangle)captureelement;
			}
            using (Bitmap bmp = ImageUtility.CaptureScreen(rect))
            {
                _vscanImage = new VScan(new ImageAdapter(bmp));
		    }
            _modelName = modelName;
		}

        public bool Compare()
        {
            return _vscanImage.OriginalData.CompareModels(_modelName);
        }
        public void SavePackage(string packageName)
        {
            Package package = _vscanImage.OriginalData.GeneratedFailurePackage;
            package.PackageName = packageName;
            package.Save();
        }
    }
}
