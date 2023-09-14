// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test EffectMapping
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using System.Windows.Markup;
using System.Collections.Generic;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Loaders;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;
using Microsoft.Test.VisualVerification;
using Microsoft.Test.Markup;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Test EffectMapping
    /// </summary>`
   //commenting ignored cases
    // [Test(1, "Arrowhead\\EffectMapping", "EffectMapping", 
    //     SupportFiles = @"FeatureTests\Effects\Xamls\EffectMapping*,
    //                      FeatureTests\Effects\Masters\*HittestMap*,
    //                      FeatureTests\Effects\Xmls\EffectMapping*.xml")]
    public class EffectMappingTest : WindowTest
    {
        #region Private Data
        private string _firstEffectIndex;
        private string _secondEffectIndex;
        private string _xaml;
        private FrameworkElement _root;
        private Rectangle _rect1;
        private Rectangle _rect2;
        private Rectangle _rect3;
        private Rectangle _rect4;
        private Canvas _mainCanvas;
        private Canvas _innerCanvas;
        private Viewport3D _viewport;
        private Viewport2DVisual3D _viewport2DVisual3D;
        private Snapshot _hittestMapMaster;
        private Snapshot _hittestResultMap;
        private string _templeateFile;
        private bool _isFirstHittesting = true;
        private const string effectsFile = "EffectMappingEffects.xml";
        private Dictionary<object, System.Drawing.Color> _colorTable;

        #endregion

        #region Methods
        /// <summary>
        /// Test load from templateFileName a template, which should contain canvas: mainCanvas, innerCanvas
        /// and rectangle: rect1, rect2, rect3, and rect4. 
        /// -------------------------------------------------------------
        /// | mainCanvas                                                |
        /// | ----------------------------------------------            |
        /// |  innerCanvas                                  | rect3     |
        /// |        rect1 rect2                            | rect4     |
        /// |                                               |           |
        /// -------------------------------------------------------------
        /// innerCanvas and rect3 has effects, defined EffectMappingEffects.xml. The effect on innerCanvas is the 
        /// one with Index attribute of firstIndex, and the effect on rect3 has the effect with Index as secondIndex. 
        /// In EffectMapping3DXamlTemplate.xaml, there are Viewport3D, Viewport2DVisual3D layers between mainCanvas and innerCanvas. 
        /// Test do a histtest on mainCanvas, compare and hittest result with a master image 
        /// {firstIndex}_{secondIndex}_HittestMapMaster.png, and then use
        /// TransformToVisual, TransformToDescendant, and TransformToAncestor to change the point mainCanvas -> 
        /// rect2 -> rect3 -> mainCanvas, and then do another hittesting verification against the master image. 
        /// </summary>
        /// <param name="templeteFileName"></param>
        /// <param name="firstIndex"></param>
        /// <param name="secondIndex"></param>
        [Variation("EffectMappingXamlTemplate.xaml", "effect1", "effect2")]
        [Variation("EffectMappingXamlTemplate.xaml", "effect1", "effect1")]
        [Variation("EffectMappingXamlTemplate.xaml", "effect2", "effect2")]
        [Variation("EffectMappingXamlTemplate.xaml", "effect3", "effect3")]
        public EffectMappingTest(string templeteFileName, string firstIndex, string secondIndex)
        {
            InitializeSteps += new TestStep(CreateMasterHittestMap);
            RunSteps += new TestStep(BuildXaml);
            RunSteps += new TestStep(RenderContent);
            RunSteps += new TestStep(FindElements);
            RunSteps += new TestStep(FirstHisttesting);
            RunSteps += new TestStep(VerifyHittestMapWithMaster);
            RunSteps += new TestStep(SecondHisttesting);
            RunSteps += new TestStep(VerifyHittestMapWithMaster);
            _firstEffectIndex = firstIndex;
            _secondEffectIndex = secondIndex;
            _templeateFile = templeteFileName;
        }

        /// <summary>
        /// Create an ImageAdapter from master image. 
        /// </summary>
        /// <returns></returns>
        TestResult CreateMasterHittestMap()
        {
            string masterImageFile = _firstEffectIndex + "_" + _secondEffectIndex + "_HittestMapMaster.png";
            if (File.Exists(masterImageFile))
            {
                _hittestMapMaster = Snapshot.FromFile(masterImageFile);
            }
            else
            {
                Status(string.Format("Cannot find file: {0}.", masterImageFile));
            }
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify the hittesting result is expected. For the second hittesting, since there some roundup some hittesting result on boundary
        /// are not exactly the same, we use a different tolerance file. 
        /// </summary>
        /// <returns></returns>
        TestResult VerifyHittestMapWithMaster()
        {
            string toleranceFilePath = _isFirstHittesting ? "EffectMappingTolerence.xml" : "EffectMappingTolerence2.xml";
            SnapshotHistogramVerifier verifier  = new SnapshotHistogramVerifier(Histogram.FromFile(toleranceFilePath));

            TestResult result = TestResult.Pass;
            bool same = false;

            Snapshot diff = _hittestMapMaster.CompareTo(_hittestResultMap);

            same = (verifier.Verify(diff) == VerificationResult.Pass);

            if (!same)
            {
                result = TestResult.Fail;

                LogComment("Visual validation failed ...");
            }
            else
            {
                Status("Visual validation passed ...");
            }

            EffectsTestHelper.LogPngSnapshot(_hittestMapMaster, "Master.png");
            
            EffectsTestHelper.LogPngSnapshot(_hittestResultMap, "HittestMap.png");
            
            return result;
        }

        /// <summary>
        /// Hittesting after the Point has been transforms with TransformToVisual, 
        /// TransformToDescendant, and TransformToAncestor. 
        /// </summary>
        /// <returns></returns>
        TestResult SecondHisttesting()
        {
            _isFirstHittesting = false;

            int height = (int)_mainCanvas.Height;
            int width = (int)_mainCanvas.Width;
            System.Drawing.Bitmap hittestMap = new System.Drawing.Bitmap(width, height);

            GeneralTransform transform1 = _mainCanvas.TransformToDescendant(_rect2);
            GeneralTransform transform2 = _rect2.TransformToVisual(_rect3);
            GeneralTransform transform3 = _rect3.TransformToVisual(_mainCanvas);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Point point = new Point(i, j);
                    Point pointInRect2 = transform1.Transform(point);
                    Point pointInRect3 = transform2.Transform(pointInRect2);
                    Point pointInMain = transform3.Transform(pointInRect3);

                    HitTestResult result = VisualTreeHelper.HitTest(_mainCanvas, pointInMain);
                    System.Drawing.Color color = GetColorBasedOnHitTestVisual(result);
                    hittestMap.SetPixel(i, j, color);
                }
            }
            
            _hittestResultMap = Snapshot.FromBitmap(hittestMap);

            return TestResult.Pass;
        }

        private System.Drawing.Color GetColorBasedOnHitTestVisual(HitTestResult result)
        {
            System.Drawing.Color color = System.Drawing.Color.FromArgb(255, 255, 0, 0);
            if (result != null && _colorTable.ContainsKey(result.VisualHit))
            {
                color = _colorTable[result.VisualHit];
            }
            return color;
        }
        /// <summary>
        /// Hittesting in coordinations of mainCanvas. 
        /// </summary>
        /// <returns></returns>
        TestResult FirstHisttesting()
        {
            _isFirstHittesting = true;

            int height = (int)_mainCanvas.Height;
            int width = (int)_mainCanvas.Width;
            System.Drawing.Bitmap hittestMap = new System.Drawing.Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Point point = new Point(i, j);

                    HitTestResult result = VisualTreeHelper.HitTest(_mainCanvas, point);

                    System.Drawing.Color color = GetColorBasedOnHitTestVisual(result);

                    hittestMap.SetPixel(i, j, color);
                }
            }

            _hittestResultMap = Snapshot.FromBitmap(hittestMap);

            return TestResult.Pass;
        }

        /// <summary>
        /// Find the elements and add them to ColorTable. 
        /// </summary>
        /// <returns></returns>
        TestResult FindElements()
        {
            Status("Finding elements ...");
            _rect1 = _root.FindName("rect1") as Rectangle;
            _rect2 = _root.FindName("rect2") as Rectangle;
            _rect3 = _root.FindName("rect3") as Rectangle;
            _rect4 = _root.FindName("rect4") as Rectangle;

            _mainCanvas = _root.FindName("mainCanvas") as Canvas;
            _innerCanvas = _root.FindName("innerCanvas") as Canvas;
            _viewport = _root.FindName("viewport") as Viewport3D;
            _viewport2DVisual3D = _root.FindName("viewport2DVisual3D") as Viewport2DVisual3D;

            if (_rect1 == null
                || _rect2 == null
                || _rect3 == null
                || _rect4 == null
                || _mainCanvas == null
                || _innerCanvas == null
            )
            {
                throw new TestSetupException("One of elements not found");
            }
            _colorTable = new Dictionary<object, System.Drawing.Color>();
            _colorTable.Add(_rect1, System.Drawing.Color.Cornsilk);
            _colorTable.Add(_rect2, System.Drawing.Color.DimGray);
            _colorTable.Add(_rect3, System.Drawing.Color.BlanchedAlmond);
            _colorTable.Add(_rect4, System.Drawing.Color.BlueViolet);
            _colorTable.Add(_innerCanvas, System.Drawing.Color.DarkOrchid);
            _colorTable.Add(_mainCanvas, System.Drawing.Color.LightPink);
            if (_viewport != null)
            {
                _colorTable.Add(_mainCanvas, System.Drawing.Color.Blue);
            }
            if (_viewport2DVisual3D != null)
            {
                _colorTable.Add(_viewport2DVisual3D, System.Drawing.Color.Yellow);
            }                    
             
            return TestResult.Pass;
        }
        /// <summary>
        /// Render the content and find element that help to mark the map with different color, and build the color table. 
        /// </summary>
        /// <returns></returns>
        TestResult RenderContent()
        {
            Status("Rendering ...");
            
            Stream stream = IOHelper.ConvertTextToStream(_xaml);
            _root = XamlReader.Load(stream) as FrameworkElement;
            Window.Content = _root;

            WaitFor(0);
            
            return TestResult.Pass;
        }

        /// <summary>
        /// Build xaml from templete file, by insert corresponding effects defined in effectsFile. 
        /// </summary>
        /// <returns></returns>
        TestResult BuildXaml()
        {
            XmlDocument effects = new XmlDocument();
            effects.Load(effectsFile);
            XmlElement firstEffectNode = FindEffectWithIndex(effects, _firstEffectIndex, "firstEffect");
            XmlElement secondEffectNode = FindEffectWithIndex(effects, _secondEffectIndex, "secondEffect");
            if(firstEffectNode == null || secondEffectNode == null)
            {
                return TestResult.Fail;
            }
            
            XmlDocument xamlTemplate = new XmlDocument();
            xamlTemplate.Load(_templeateFile);

            XmlElement rootElement = xamlTemplate.DocumentElement;
            XmlElement resourcesNode = xamlTemplate.CreateElement(rootElement.Name + ".Resources", XamlGenerator.AvalonXmlns);
            rootElement.InsertAfter(resourcesNode, null);
            resourcesNode.InsertAfter(xamlTemplate.ImportNode(firstEffectNode, true), null);
            resourcesNode.InsertAfter(xamlTemplate.ImportNode(secondEffectNode, true), null);

            _xaml = xamlTemplate.OuterXml;

            return TestResult.Pass;
        }

        /// <summary>
        /// Find effect with certain Index, and assign a X:Key value. 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        XmlElement FindEffectWithIndex(XmlDocument document, string index, string key)
        {
            XmlElement effects = document.DocumentElement;
            foreach (XmlElement effect in effects)
            {
                if (string.Compare(effect.GetAttribute("Index"), index) == 0)
                {
                    XmlElement clone = effect.CloneNode(true) as XmlElement;
                    clone.RemoveAttribute("Index");
                    clone.SetAttribute("Key", XamlGenerator.AvalonXmlnsX, key);
                    return clone;
                }
            }
            LogComment(string.Format("Cannot find effect with Index: {0}.", index));
            return null;
        }
        #endregion
    }
}