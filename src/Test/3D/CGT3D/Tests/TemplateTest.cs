// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Test.Graphics.TestTypes;

#if !STANDALONE_BUILD
using TrustedAssembly = Microsoft.Test.Security.Wrappers.AssemblySW;
#else
using TrustedAssembly = System.Reflection.Assembly;
#endif

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Set a Viewport3D as the content of a control
    /// template.  Should render the same as if the
    /// control wasn't present.
    /// </summary>
    public class TemplateTest : Visual3DRenderingTest
    {
        /// <summary/>
        public override void Init(Variation v)
        {
            base.Init(v);

            v.AssertExistenceOf("TargetType");
            TrustedAssembly presentationFramework = TrustedAssembly.GetAssembly(typeof(Control));
            targetType = PT.Untrust(presentationFramework.GetType(v["TargetType"], true));
        }

        /// <summary/>
        public override Visual GetWindowContent()
        {
            if (!variation.UseViewport3D)
            {
                throw new ArgumentException("TemplateTest must use Viewport3D");
            }

            ControlTemplate template = VisualUtils.ToTemplate(parameters.Viewport);
            template.TargetType = targetType;

            FrameworkElement element = (FrameworkElement)targetType.GetConstructor(new Type[0]).Invoke(null);
            if (element is Control)
            {
                ((Control)element).Template = template;
            }
            else if (element is Page)
            {
                ((Page)element).Template = template;
            }
            else
            {
                throw new NotSupportedException("Templates cannot be set on: " + element.GetType().Name);
            }

            return element;
        }

        /// <summary/>
        protected Type targetType;
    }
}