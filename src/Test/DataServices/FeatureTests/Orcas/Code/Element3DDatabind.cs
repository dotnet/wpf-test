// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Modeling;
using System.Xml;
using System.IO;
using System.Windows.Markup;
using System.Windows.Resources;
using System.Collections.Generic;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Load Skeleton xaml to a XmlDocument, and Apply Databind Transform to 
    /// the Skeleton xaml and assign it to another XmlDocument.  
    /// Verifying property on after Databind.
    /// </description>
    /// </summary>
    [Test(0, "Controls", "Element3DDatabind", Keywords = "MicroSuite")]
    public class Element3DDatabind : WindowTest
    {
        #region Private Data

        private XmlDocument _originalTree;
        private XmlDocument _transformedTree;
        private List<string> _bindingElementsID;
        private string _transformName;

        #endregion

        #region Public Member

        [Variation("BindingUnderA3DViewport")]
        [Variation("Viewport3DBindToAnotherViewport3D")]
        [Variation("BindToAnotherViewport3DModelUIElement3D")]
        [Variation("BindToA2DObjectIsSiblingOfParent")]
        [Variation("SiblingsOfParentBindToViewport3D")]
        public Element3DDatabind(string transformName)
        {
            this._transformName = transformName;

            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(VerifyWithTransform);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            XmlDocument transformDoc = LoadXmlDocument("DataServicesTestOrcas", @"resource/databindingtransform.xml");
            XamlTransformer xamlTransformer = new XamlTransformer(transformDoc);

            _originalTree = LoadXmlDocument("DataServicesTestOrcas", @"resource/testskeleton.xaml");
            _transformedTree = xamlTransformer.ApplyTransform(_originalTree, _transformName);

            _bindingElementsID = new List<string>();
            XmlNodeList targetNodes = null;

            if (String.Equals(_transformName, "BindToAnotherViewport3DModelUIElement3D"))
            {
                // Get the Name of all elements that have a data bound IsEnabled property because ModelUIElement3D does not have Background property
                targetNodes = _transformedTree.SelectNodes("//*[starts-with(@IsEnabled,'{Binding')]/@Name");
            }
            else
            {
                // Get the Name of all elements that have a data bound Background property
                targetNodes = _transformedTree.SelectNodes("//*[starts-with(@Background,'{Binding')]/@Name");
            }
            
            foreach (XmlAttribute nameAttribute in targetNodes)
            {
                _bindingElementsID.Add(nameAttribute.Value);
            }

            return TestResult.Pass;
        }

        private TestResult VerifyWithTransform()
        {
            Status("Verify After Databind");

            XmlNodeReader xmlNodeReader = new XmlNodeReader(_transformedTree);
            FrameworkElement frameworkElement = XamlReader.Load(xmlNodeReader) as FrameworkElement;
            Window.Content = frameworkElement;

            // databind is ready after DataBind priority.
            WaitForPriority(DispatcherPriority.DataBind);

            foreach (string bindingElementID in _bindingElementsID)
            {
                Control control = frameworkElement.FindName(bindingElementID) as Control;
                if (control == null)
                {
                    TestLog.Current.LogEvidence("Fail to find Control '" + bindingElementID + "' in logical tree.");
                    return TestResult.Fail;
                }

                if (String.Equals(_transformName, "BindToAnotherViewport3DModelUIElement3D"))
                {
                    // Validating IsEnabled property is false to test binding works across 3D elements.
                    // Binding source is ModelUIElement3D
                    // Does not matter what property we are using. 
                    if (!Object.Equals(control.IsEnabled, false))
                    {
                        TestLog.Current.LogEvidence("Fail: " + bindingElementID + " IsEnabled property should be 'false' after databinding.  Actaul value '" + control.IsEnabled.ToString() + "'.");
                        return TestResult.Fail;
                    }
                }
                else
                {
                    // Validating background property color is red to test binding Brushes works across 3D elements Viewport2DVisual3D.
                    // Does not matter what color we are using. 
                    if (!Object.Equals(control.Background, Brushes.Red))
                    {
                        TestLog.Current.LogEvidence("Fail: " + bindingElementID + " Background property should be 'Red' after databinding.  Actaul value '" + control.Background.ToString() + "'.");
                        return TestResult.Fail;
                    }
                }

                //This has no real effect on the test, but we wait .1 seconds for the test to render for easier debugging
                WaitFor(100);
            }

            return TestResult.Pass;
        }

        private XmlDocument LoadXmlDocument(string callingAssembly, string fileName)
        {
            string strUri = "pack://application:,,,/" + callingAssembly + ";component/" + fileName;
            Uri uri = new Uri(strUri, UriKind.RelativeOrAbsolute);
            StreamResourceInfo info = Application.GetResourceStream(uri);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(info.Stream);
            return xmlDocument;
        }

        #endregion
    }
}

