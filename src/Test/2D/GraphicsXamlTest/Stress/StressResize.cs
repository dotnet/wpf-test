// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections;

namespace Microsoft.Test.Graphics
{
    public partial class GraphicsStress : Window
    {
        private void StressResize()
        {
            //I don't want to have a changing seed
            //Since the seed is constant, the generated random numbers will be always the same sequence. 
            //The any exception or crash will be reproable if there is any.
            Random ran = new Random(0);

            for (int i = 0; i < 8; i++)
            {
                //Randomly generated size for resizing the window
                double[] size = new double[] { ran.NextDouble() * 600, ran.NextDouble() * 500 };
                XamlTestHelper.AddStep(Resize, size);
            }

        }

        private object Resize(object size)
        {
            if (size == null || !(size is IList))
            {
                throw new System.ApplicationException("size param should not be null, and it should be a double array");
            }

            XamlTestHelper.LogStatus(" ******  Run #" + ++_resizecount + " ******");
            ////Setting the width and height
            this.Width = (double)((IList)size)[0];
            this.Height = (double)((IList)size)[1];

            XamlTestHelper.LogStatus("New Width = " + this.Width);
            XamlTestHelper.LogStatus("New Height = " + this.Height);
            
            return null;
        }

        private int _resizecount = 0;
    }
}
