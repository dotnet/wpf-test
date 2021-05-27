// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//---------------------------------------------------------------------
//
//  Description: Module that does VScan actions.
//  Creator: Derek Mehlhorn (derekme)
//  Date Created: 3/24/04
//---------------------------------------------------------------------

using System;
using System.Windows;
using Microsoft.Test.RenderingVerification.Model.Analytical;	
using Microsoft.Test.RenderingVerification;
using System.Drawing;											
using System.Reflection;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Interop;

namespace Annotations.Test.Framework
{
	/// <summary>
	/// Rendering mode that test is running under.
	/// </summary>
	public enum RenderMode
	{
        /// <summary>
        /// Software rendering.
        /// </summary>
		Software,
        /// <summary>
        /// Hardware rendering.
        /// </summary>
		Hardware
	}

	/// <summary>
	/// Module for doing VScan related actions.
	/// </summary>
	public class VScanModule
	{
		#region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="suite"></param>
		public VScanModule(TestSuite suite)
		{
			_suite = suite;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Compare a screen shot of the given element to this master using these tolerances.
		/// </summary>
		/// <remarks>
		/// - Test will be logged as FAIL if element capture and master comparison fails.
		/// - If RenderMode == Software then tolerance file will be ignored and an exact image comparison
		/// will be performed.
		/// </remarks>
		/// <param name="elementToCapture">Element to screen capture and compare to master.</param>
		/// <param name="masterFile">Master to compare element capture against.</param>
		/// <param name="toleranceFile">File that contains VScan tolerances that are used in the comparison.</param>
		public void CompareToMaster(UIElement elementToCapture, string masterFile, string toleranceFile)
		{
			_suite.printStatus("VisualScan: Starting.");
			_suite.printStatus("VisualScan: Master = '" + masterFile + ".");
			_suite.printStatus("VisualScan: Tolerance = '" + toleranceFile + ".");

			if (!File.Exists(masterFile))
				throw new FileNotFoundException("Master file '" + masterFile + "' does not exist.");
	
			EnsureRenderComplete();
			Bitmap capturedImage = CreateBitmap(elementToCapture);
            Bitmap masterImage = new Bitmap(masterFile);

            // If ComparisionMode is Software do an exact comparision between the element and the master.
            if (ComparisionMode == RenderMode.Software)
                toleranceFile = null;
            else
            {
                if (!File.Exists(toleranceFile))
                    throw new FileNotFoundException("Tolerance file '" + toleranceFile + "' does not exist.");
            }

            CompareImages(masterImage, capturedImage, toleranceFile);
        }        

		/// <summary>
		/// Use Vscan to verify the current visuals (e.g. screenshot) against the given model.
		/// </summary>
		/// <param name="elementToCapture">UIElement to take a screen shot of.</param>
		/// <param name="modelFilename">Full path to model file to compare current visuals against.</param>
		/// <exception cref="ArgumentException">If modelFilename is not a valid path.</exception>
		public void CompareVisualsToModel(UIElement elementToCapture, object modelFilename) 
		{
			string modelName = modelFilename as string;
			Console.WriteLine("[VisualScan]: Master is '" + modelFilename + "'.");

			EnsureRenderComplete();
			
			// Create vscan object.
			VScan scan = new VScan(CreateBitmap(elementToCapture));			
			// Analyze Data.
			Console.WriteLine("[VisualScan]: analyzing screen capture.");
			scan.OriginalData.Analyze();
			// Compare model with image.
			Console.WriteLine("[VisualScan]: comparing against master.");
			bool result = scan.OriginalData.CompareModels(modelName);

			Console.WriteLine("[VisualScan]: " + ((result) ? "SUCCEEDED" : "FAILED"));
			if (!result)
			{
				// Dump a report of the differences.
				//
				XmlNode xmlDiff = scan.OriginalData.ModelDifferences;
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(xmlDiff.OuterXml);
				string errorFilename = "vscan_error_" + DateTime.Now.Ticks + ".xml";
				xmlDoc.Save(errorFilename);
				throw new Exception("Visuals differ from expected, error log generated '" + errorFilename + "'.");
			}
		}

		/// <summary>
		/// Take screenshot of given element and save it to a randomly generated filename.
		/// </summary>
		public void TakeScreenShot(UIElement element)
		{
			TakeScreenShot(element, "screenshot" + DateTime.Now.Ticks);
		}

		/// <summary>
		/// Take screenshot of given element and save it to file with the given name.
		/// </summary>
        /// <param name="element">Element to capture.</param>
		/// <param name="filename">File name WITHOUT extension.</param>
		public void TakeScreenShot(UIElement element, string filename)
		{
			EnsureRenderComplete();
			string fullname = filename + ".bmp";
			ImageUtility.ToImageFile(new ImageAdapter(CreateBitmap(element)), fullname, System.Drawing.Imaging.ImageFormat.Bmp);
			_suite.printStatus("Screenshot saved to: '" + fullname + "'");
		}

		/// <summary>
		/// Set the render mode of the given Window to 'Software'.
		/// </summary>
		public static void SetRenderModeToSoftware(Window window)
		{
			// Convert window to an HwndTarget.
			HwndTarget hwndTarget = (HwndTarget)PresentationSource.FromVisual(window).CompositionTarget;
                        hwndTarget.RenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
		}

		/// <summary>
		/// Directly get/set RenderMode that module should operate under.
		/// </summary>
		public RenderMode ComparisionMode
		{
			get
			{
				return _comparisionMode;
			}
			set
			{
				_comparisionMode = value;
			}
		}

		#endregion

		#region Private Methods

        private void CompareImages(Bitmap masterImage, Bitmap capturedImage, string toleranceFile)
        {
            // Create Comparator
            ImageComparator comparator = new ImageComparator();
            comparator.ChannelsInUse = ChannelCompareMode.ARGB;

            // Set tolerance if one is provided.
            if (!string.IsNullOrEmpty(toleranceFile))
            {
                _suite.printStatus("VisualScan: Loading tolerance.");
                comparator.Curve.CurveTolerance.LoadTolerance(toleranceFile);
            }

            // Perform Comparison.
            _suite.printStatus("VisualScan: Comparing images.");
            if (comparator.Compare(new ImageAdapter(masterImage), new ImageAdapter(capturedImage), false))
                _suite.printStatus("VisualScan: Succeeded.");
            else
            {
                _suite.printStatus("VisualScan: Failed.");
                GenerateAndWriteDebuggingOutput(comparator, masterImage, capturedImage);
                _suite.failTest("Captured visual is different from Master.");
            }
        }

        private void GenerateAndWriteDebuggingOutput(ImageComparator comparator, Bitmap masterImage, Bitmap capturedImage)
        {
            // Unique identifier.
            long timestamp = DateTime.Now.Ticks;

            // Create a Package for analysis.              
            string packageName = Path.GetTempPath() + _suite.CaseNumber + "_error_" + timestamp + ".vscan";
            Package package = Package.Create(packageName, masterImage, capturedImage);
            if (comparator.Curve.CurveTolerance != null)
                package.Tolerance = comparator.Curve.CurveTolerance.WriteToleranceToNode();
            package.Save();
            _suite.LogToFile(packageName); // Copy package to Log location.

            // Output the captured image for updating the masters.
            string captureDump = _suite.CaseNumber + "_capture_" + timestamp + ".bmp";
            _suite.printStatus("Captured image: " + captureDump);
            Stream captureStream = new MemoryStream();
            ImageUtility.ToImageStream(new ImageAdapter(capturedImage), captureStream, System.Drawing.Imaging.ImageFormat.Bmp);
            _suite.LogToFile(captureDump, captureStream);
        }

		private void EnsureRenderComplete()
		{
			//
			// Magical code that is supposed to block the current operation until all Render
			// operations are complete (e.g. ensure that window updates are done before we
			// take a screenshot).
			//
			Assembly mcasm = Assembly.GetAssembly(typeof(System.Windows.Interop.HwndTarget));
			Type mcType = mcasm.GetType("System.Windows.Media.MediaContext");
			object mc = mcType.InvokeMember("From", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { System.Windows.Threading.Dispatcher.CurrentDispatcher });
			mcType.InvokeMember("CompleteRender", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, mc, new object[] { });
		}

		/// <summary>
		/// Create a bmp screenshot of the given element.
		/// </summary>
		private Bitmap CreateBitmap(UIElement element)
		{		
			return ImageUtility.CaptureElement(element);
		}

		#endregion

		#region Private Variables

		TestSuite _suite;
		RenderMode _comparisionMode = RenderMode.Hardware; // Assume hardware acceleration unless explicitly set.

		#endregion
	}
}
