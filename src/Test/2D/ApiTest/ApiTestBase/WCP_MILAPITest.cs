// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Microsoft.Test.Graphics - Common base class for a Test object.
//
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class ApiTest : DrawingVisual
    {
        public ApiTest ( double left, double top, double width, double height) : base()
        {
            m_left = left;
            m_top = top;
            m_height = width;
            m_width = height;
        }

        protected void Update()
        {
            using(DrawingContext dc = RenderOpen())
            {
                OnRender( dc );
            }
        }

        protected virtual void OnRender(DrawingContext ctx)
        {
        }

        /// <summary>
        ///     Added new keyword to make hide of original implementation
        /// explicit
        /// </summary>
        public new VisualCollection Children
        {
            get { return this.Children; }
        }

        protected double m_left;

        protected double m_top;
        protected double m_height;
        protected double m_width;
    }

    internal class WCP_MILShapeAPITest : Canvas
    {
        public WCP_MILShapeAPITest (double left, double top, double width, double height) : base()
        {
            m_left = left;
            m_top = top;
            m_height = width;
            m_width = height;
        }

        protected override void OnRender(DrawingContext ctx)
        {
        }

        protected double m_left;
        protected double m_top;
        protected double m_height;
        protected double m_width;
    }
}
