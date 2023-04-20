// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Box Test class
    /// </summary>
    [XamlSetMarkupExtension("ReceiveMarkupExtension")]
    public class Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Box"/> class.
        /// </summary>
        public Box()
        {
        }

        /// <summary>
        /// Gets or sets the footprint.
        /// </summary>
        /// <value>The footprint.</value>
        public int Footprint { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name value.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        /// <value>The volume.</value>
        public int Volume { get; set; }

        /// <summary>
        /// Receives the markup extension.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="eventArgs">The <see cref="XamlSetMarkupExtensionEventArgs"/> instance containing the event data.</param>
        public static void ReceiveMarkupExtension(object targetObject, XamlSetMarkupExtensionEventArgs eventArgs)
        {
            Box box = targetObject as Box;
            DimensionExtension de = eventArgs.MarkupExtension as DimensionExtension;
            if (box != null && de != null)
            {
                switch (eventArgs.Member.Name)
                {
                    case "Footprint":
                        box.Footprint = de.ProvideExpression().Area;
                        break;
                    case "Volume":
                        box.Volume = de.ProvideExpression().Volume;
                        break;
                    default:
                        throw new InvalidOperationException("Unknown Property: " + eventArgs.Member.Name);
                }

                eventArgs.Handled = true;
            }
        }
    }

    /// <summary>
    /// Custom_NoIRME+AMEET(CustomExpression) class for IRME cases
    /// </summary>
    public class BoxNoIRME
    {
        /// <summary>
        /// Gets or sets the footprint.
        /// </summary>
        /// <value>The footprint.</value>
        public int Footprint { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name value .</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        /// <value>The volume.</value>
        public int Volume { get; set; }
    }
}
