// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test ShaderEffect with shader from internet.
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.Xml;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Loaders;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Test shader from internet. Upload a shader onto 
    /// internet, update the xaml with the URISource, 
    /// and then run EffectsXamlBasedTest. 
    /// </summary>`
   //commenting ignored cases
    // [Test(2, "Arrowhead\\NegativeTest", "ShaderFromInternet", SupportFiles = @"FeatureTests\Effects\Xamls\BasicShaderEffect_ShaderFromInternet.xaml,FeatureTests\Effects\Masters\BasicShaderEffect.png,Common\Effects\Shaders\Simple.ps, common\InvariantTheme.xaml")]
    public class ShaderFromInternetTest : AvalonTest
    {
        #region Methods

        /// <summary>
        /// Constructor, pass in xaml file name, master file name, and shader file name. 
        /// and set up test step. 
        /// </summary>
        /// <param name="xamlFileName"></param>
        /// <param name="masterImageName"></param>
        /// <param name="shaderfileName"></param>
        [Variation("BasicShaderEffect_ShaderFromInternet.xaml", "BasicShaderEffect.png", "simple.ps")]
        public ShaderFromInternetTest(string xamlFileName, string masterImageName, string shaderfileName)
        {
            this._xamlFileName = xamlFileName;
            this._masterImageName = masterImageName;
            this._shaderfileName = shaderfileName;         
            RunSteps += new TestStep(RunTest);
        }

        /// <summary>
        /// Runtest 
        /// </summary>
        /// <returns></returns>
        TestResult RunTest()
        {
            _shaderUri = UploadShader(_shaderfileName);
            UpdateXamlWithShader(_xamlFileName);
            PropertyBag parameters = new PropertyBag();
            parameters["Xaml"] = _xamlFileName;
            parameters["Master"] = _masterImageName;
            ExceptionHelper.ExpectException<SystemException>(
                delegate()
                {
                    EffectsXamlBasedTest test = new EffectsXamlBasedTest();
                    test.RunTest(parameters);
                },
                new SystemException());

            return TestResult.Pass;
        }

        /// <summary>
        /// Upload the shader onto FileHost
        /// </summary>
        /// <param name="shaderfileName"></param>
        /// <returns></returns>
        private Uri UploadShader(string shaderfileName)
        {
            FileHost host = new FileHost();
            host.UploadFile(shaderfileName);
            return host.GetUri(shaderfileName, FileHostUriScheme.HttpInternet);
        }

        /// <summary>
        /// Add Uri property for the ShaderEffect in the xaml file
        /// </summary>
        /// <param name="xamlFileName"></param>
        private void UpdateXamlWithShader(string xamlFileName)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xamlFileName);
            XmlNodeList pixelShaders = document.GetElementsByTagName("PixelShader");

            if(pixelShaders.Count != 1)
            {
                throw new TestSetupException(string.Format("Should has only 1 PixelShader node in {0}.", xamlFileName));
            }

            XmlElement pixelShaderElement = pixelShaders[0] as XmlElement;

            pixelShaderElement.SetAttribute("UriSource", _shaderUri.ToString());
            document.Save(xamlFileName);
        }
        #endregion

        #region private fields

        private string _xamlFileName;
        private string _masterImageName;
        private string _shaderfileName;
        private Uri _shaderUri;

        #endregion
    }
}