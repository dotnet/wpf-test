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
	/// Verifyfies that binding to an attached property located in separate binary.
    /// e.g. Path=(Pie:PiePanel.Value)}
	/// </description>
	/// <relatedTasks>

    /// </relatedTasks>
	/// </summary>

    [Test(1, "Binding", "AttachedPropertyInAssembly")]

    public class AttachedPropertyInAssembly : XamlTest 
    {
        public AttachedPropertyInAssembly() : base("Pie.xaml")
        {
                
         RunSteps += new TestStep(Step1);
        }

        TestResult Step1()
        {
            WaitForPriority(DispatcherPriority.Render);
            Canvas _canvas = LogicalTreeHelper.FindLogicalNode(RootElement, "canvas") as Canvas;
            ObjectDataProvider ods = _canvas.FindResource("DSO") as ObjectDataProvider;
            

            FrameworkElement[] visuals = null;
            PieChart _piechart = LogicalTreeHelper.FindLogicalNode(RootElement, "piechart") as PieChart;
            if (_piechart != null)
                visuals = Util.FindElements(_piechart, "contentpresenter");

            if (visuals == null)
            {
                LogComment("Visuals == null!");
                return TestResult.Fail;
            }


            if (visuals.Length != ((SortDataItems)ods.Data).Count)
            {
                LogComment("Expetect 9 visual elements, there were only : " + visuals.Length.ToString());
                return TestResult.Fail;
            }

            for (int i = 0; i < visuals.Length; i++)
            {
                if (((SortDataItems)ods.Data)[i].Top != (double)((ContentPresenter)visuals[i]).Content)
                {
                    LogComment("Unexepected Content Value: " + ((ContentPresenter)visuals[i]).Content.ToString() + "Expected: " + ((SortDataItems1)ods.Data)[i].Top.ToString());
                    return TestResult.Fail;

                }

            }
            
            
            return TestResult.Pass;
        }
    }    
}
