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
        private void StressMove()
        {
            //I don't want to have a changing seed
            //Since the seed is constant, the generated random numbers will be always the same sequence. 
            //The any exception or crash will be reproable if there is any.
            Random ran = new Random(0);

            for (int i = 0; i < 8; i++)
            {
                //Randomly generated size for resizing the window
                double[] location = new double[] { ran.NextDouble() * 400, ran.NextDouble() * 400 };
                XamlTestHelper.AddStep(Move, location);
            }

        }

        private object Move(object location)
        {
            if (location == null || !(location is IList))
            {
                throw new System.ApplicationException("location param should not be null, and it should be a double array");
            }

            XamlTestHelper.LogStatus(" ******  Run #" + ++_movecount + " ******");
            ////Setting the Left and Top
            this.Left = (double)((IList)location)[0];
            this.Top = (double)((IList)location)[1];

            XamlTestHelper.LogStatus("New Left = " + this.Left);
            XamlTestHelper.LogStatus("New Top = " + this.Top);

            return null;
        }

        private int _movecount = 0;
    }
}
