// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using Microsoft.Test;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Verifies binding to Image Source of different resources.
    /// </description>
	/// </summary>

    [Test(1, "Binding", SupportFiles = @"FeatureTests\DataServices\ContentImage.jpg")]

    public class ImageSourceBug : XamlTest 
    {
        private Image _unboundContent;
        private Image _unboundResource;
        private Image _boundContent;
        private Image _boundResource; 

        public ImageSourceBug()
            : base("ImageSourceBug.xaml")
        {
                
         InitializeSteps += new TestStep(SetUp);
         RunSteps += new TestStep(Verify);
        }

        TestResult SetUp()
        {
            WaitForPriority(DispatcherPriority.Render);
            
            _unboundContent = LogicalTreeHelper.FindLogicalNode(RootElement, "UnboundContent") as Image;
            _unboundResource = LogicalTreeHelper.FindLogicalNode(RootElement, "UnboundResource") as Image;

            _boundContent = LogicalTreeHelper.FindLogicalNode(RootElement, "BoundContent") as Image;
            _boundResource = LogicalTreeHelper.FindLogicalNode(RootElement, "BoundResource") as Image;

            if (_unboundContent == null)
            {
                LogComment("Could not reference UnboundContent Image");
                return TestResult.Fail;
            }

            if (_unboundResource == null)
            {
                LogComment("Could not reference UnboundResource Image");
                return TestResult.Fail;
            }

            if (_boundContent == null)
            {
                LogComment("Could not reference BoundContent Image");
                return TestResult.Fail;
            }

            if (_boundResource == null)
            {
                LogComment("Could not reference BoundResource Image");
                return TestResult.Fail;
            }
            
            return TestResult.Pass;
        }

        private TestResult Verify()
        {
            Status("Verifying heights of Images");
            Status("Height of unbound content " + _unboundContent.ActualHeight.ToString());
            Status("Height of unbound resource " + _unboundResource.ActualHeight.ToString());

            if (_boundContent.ActualHeight != 100)
            {
                LogComment("Expected boundContent actual height to be 100, actual " + _boundContent.ActualHeight.ToString());
                return TestResult.Fail;
            }

            if (_boundResource.ActualHeight != 100)
            {
                LogComment("Expected boundResource actual height to be 100, actual " + _boundResource.ActualHeight.ToString());
                return TestResult.Fail;
            }
            
            LogComment("Actual height was expected");

            return TestResult.Pass;
        }

    }    
}
