// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Collections;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Documents;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// Tests ValidationAdorners are applied when element is in error.
    /// Provides coverage for AdornedElementPlaceholder.
	/// </description>
    /// <relatedBugs>

    /// </relatedBugs>
	/// </summary>
    [Test(1, "Validation", "ValidationAdorners")]
	public class ValidationAdorners : XamlTest
	{
        private TextBox _tbName;
        private TextBox _tbAge;
        private TextBox _tbNameDefaultAdorner;
        private ControlTemplate _validationTemplate;
        private bool _foundAdorner;

        public ValidationAdorners()
            : base(@"ValidationAdorners.xaml")
        {
			InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(SetNullAdornerTemplate);
            RunSteps += new TestStep(GetNullAdornerTemplate);
            RunSteps += new TestStep(SetCorrectAdornerTemplate);
            // uncomment when this is possible to do. 
            //There is no way to get the visual tree of the ControlTemplate and change 
            //the Cotroltemplate due to fef change, It'll be possible to do it in V2.0
            //RunSteps += new TestStep(AddChildAdornedElementPlaceholder);

            RunSteps += new TestStep(NotifyOfError);
            RunSteps += new TestStep(CheckForAdornerFound);
            RunSteps += new TestStep(TestDefaultTemplate);

            // uncomment when this is possible to do. 
            //There is no way to get the visual tree of the ControlTemplate and change 
            //the Cotroltemplate due to fef change, It'll be possible to do it in V2.0
            //RunSteps += new TestStep(CoveragePlaceholder);
            RunSteps += new TestStep(AddBadChildPlaceholder);

            // uncomment when this is possible to do. 
            //There is no way to get the visual tree of the ControlTemplate and change 
            //the Cotroltemplate due to fef change, It'll be possible to do it in V2.0
            //RunSteps += new TestStep(AddTwoChildrenPlaceholder);

            RunSteps += new TestStep(AddNullChildPlaceholder);

            // uncomment when this is possible to do. 
            //There is no way to get the visual tree of the ControlTemplate and change 
            //the Cotroltemplate due to fef change, It'll be possible to do it in V2.0
            //RunSteps += new TestStep(AddAdornedPlaceholderToDockPanel);
        }

        private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.SystemIdle);

            _tbName = Util.FindElement(RootElement, "tbName") as TextBox;
            _tbAge = Util.FindElement(RootElement, "tbAge") as TextBox;
            _tbNameDefaultAdorner = Util.FindElement(RootElement, "tbNameDefaultAdorner") as TextBox;
            _validationTemplate = RootElement.Resources["validationTemplate"] as ControlTemplate;

            if (_tbName == null)
            {
                LogComment("Fail - Unable to reference TextBox tbName");
                return TestResult.Fail;
            }
            if (_tbAge == null)
            {
                LogComment("Fail - Unable to reference TextBox tbAge");
                return TestResult.Fail;
            }
            if (_tbNameDefaultAdorner == null)
            {
                LogComment("Fail - Unable to reference TextBox tbNameDefaultAdorner");
                return TestResult.Fail;
            }
            if (_validationTemplate == null)
            {
                LogComment("Fail - Unable to reference ControlTemplate validationTemplate");
                return TestResult.Fail;
            }

            _foundAdorner = false;

            Status("Setup was successful");
            return TestResult.Pass;
		}

        private TestResult SetNullAdornerTemplate()
        {
            Status("SetAdornerTemplate");

            SetExpectedErrorTypeInStep(typeof(System.ArgumentNullException));
            Validation.SetErrorTemplate(null, _validationTemplate);

            Status("SetAdornerTemplate was successful");
            return TestResult.Pass;
        }

        private TestResult GetNullAdornerTemplate()
        {
            Status("GetNullAdornerTemplate");

            SetExpectedErrorTypeInStep(typeof(System.ArgumentNullException));
            Validation.GetErrorTemplate(null);

            Status("GetNullAdornerTemplate was successful");
            return TestResult.Pass;
        }

        private TestResult SetCorrectAdornerTemplate()
        {
            Status("SetCorrectAdornerTemplate");
            
            Validation.SetErrorTemplate(_tbAge, _validationTemplate);
            ControlTemplate ct = Validation.GetErrorTemplate(_tbAge);

            if (ct != _validationTemplate)
            {
                LogComment("Fail - control template not set as expected");
                return TestResult.Fail;
            }

            Status("SetCorrectAdornerTemplate was successful");
            return TestResult.Pass;
        }

        private TestResult AddChildAdornedElementPlaceholder()
        {
            Status("AddChildAdornedElementPlaceholder");

            ControlTemplate ct = RootElement.Resources["validationTemplate"] as ControlTemplate;
            FrameworkElementFactory placeholder = ct.VisualTree.FirstChild as FrameworkElementFactory;
            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BorderBrushProperty, Brushes.Red);
            border.SetValue(Border.BorderThicknessProperty, new Thickness(2));
            placeholder.AppendChild(border);

            ControlTemplate ct2 = _tbName.GetValue(Validation.ErrorTemplateProperty) as ControlTemplate;
            FrameworkElementFactory border2 = ct2.VisualTree.FirstChild.FirstChild as FrameworkElementFactory;
            if (border2 == null || border2.Type != typeof(Border))
            {
                LogComment("Fail - Border not correctly added as a child of AdornedElementPlaceholder");
                return TestResult.Fail;
            }

            Status("AddChildAdornedElementPlaceholder was successful");
            return TestResult.Pass;
        }

        private TestResult NotifyOfError()
        {
            Status("NotifyOfError");

            Validation.AddErrorHandler(_tbAge, new EventHandler<ValidationErrorEventArgs>(ErrorHandler));
            _tbAge.Text = "error";
            TestResult resultErrorHandler = WaitForSignal("errorHandler");
            if (resultErrorHandler != TestResult.Pass) { return resultErrorHandler; }

            Status("NotifyOfError was successful");
            return TestResult.Pass;
        }

        private void ErrorHandler(object sender, ValidationErrorEventArgs args)
        {
            if (!Validation.GetHasError(_tbAge))
            {
                Signal("errorHandler", TestResult.Fail);
            }

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback(CheckTreeForAdorner), _tbAge);
            Signal("errorHandler", TestResult.Pass);
        }

        private object CheckTreeForAdorner(object elementInError)
        {
            AdornerLayer al = AdornerLayer.GetAdornerLayer((UIElement)elementInError);
            Adorner[] adorners = al.GetAdorners((UIElement)elementInError);
            if ((adorners != null) && (adorners.Length == 1))
            {
                _foundAdorner = true;
            }
            return null;
        }

        private TestResult CheckForAdornerFound()
        {
            Status("CheckForAdornerFound");
            WaitForPriority(DispatcherPriority.Normal);
            if (!_foundAdorner)
            {
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult TestDefaultTemplate()
        {
            Status("TestDefaultTemplate");

            ControlTemplate ct = _tbNameDefaultAdorner.GetValue(Validation.ErrorTemplateProperty) as ControlTemplate;
            if (ct == null)
            {
                LogComment("Fail - No default template exists for TextBox tbNameDefaultAdorner");
                return TestResult.Fail;
            }

            Status("TestDefaultTemplate was successful");
            return TestResult.Pass;
        }

        private TestResult CoveragePlaceholder()
        {
            Status("CoveragePlaceholder");

            AdornedElementPlaceholder aep = new AdornedElementPlaceholder();
            if (aep.Child != null)
            {
                LogComment("Fail - AdornedElementPlaceholder's Child should be null");
                return TestResult.Fail;
            }
            Border border = new Border();
            border.SetValue(Border.BackgroundProperty, Brushes.AliceBlue);
            aep.Child = border;
            if (aep.Child != border)
            {
                LogComment("Fail - AdornedElementPlaceholder's Child should be border (first time)");
                return TestResult.Fail;
            }
            aep.Child = border;
            if (aep.Child != border)
            {
                LogComment("Fail - AdornedElementPlaceholder's Child should be border (second time)");
                return TestResult.Fail;
            }
            aep.Child = null;
            if (aep.Child != null)
            {
                LogComment("Fail - AdornedElementPlaceholder's Child should be null");
                return TestResult.Fail;
            }

            object adornedElement = aep.AdornedElement;
            if (adornedElement != null)
            {
                LogComment("Fail - AdornedElement should be null");
                return TestResult.Fail;
            }

            // this does nothing - does not throw
            ((IAddChild)aep).AddText("");

            Status("CoveragePlaceholder was successful");
            return TestResult.Pass;
        }

        private TestResult AddBadChildPlaceholder()
        {
            Status("AddBadChildPlaceholder");

            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            AdornedElementPlaceholder aep = new AdornedElementPlaceholder();
            // ContentElement is not UIElement
            ((IAddChild)aep).AddChild(new ContentElement());

            Status("AddBadChildPlaceholder was successful");
            return TestResult.Pass;
        }

        private TestResult AddTwoChildrenPlaceholder()
        {
            Status("AddTwoChildrenPlaceholder");

            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            AdornedElementPlaceholder aep = new AdornedElementPlaceholder();
            ((IAddChild)aep).AddChild(new Border());
            ((IAddChild)aep).AddChild(new Border());

            Status("AddTwoChildrenPlaceholder was successful");
            return TestResult.Pass;
        }

        private TestResult AddNullChildPlaceholder()
        {
            Status("AddNullChildPlaceholder");

            // does not throw
            AdornedElementPlaceholder aep = new AdornedElementPlaceholder();
            ((IAddChild)aep).AddChild(null);

            Status("AddNullChildPlaceholder was successful");
            return TestResult.Pass;
        }

        private TestResult AddAdornedPlaceholderToDockPanel()
        {
            Status("AddAdornedPlaceholderToDockPanel");

            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
            DockPanel dp = RootElement as DockPanel;
            AdornedElementPlaceholder aep = new AdornedElementPlaceholder();
            dp.Children.Add(aep);

            Status("AddAdornedPlaceholderToDockPanel was successful");
            return TestResult.Pass;  
        }
    }   
}

