// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.TestObjects.Utilities.Validation
{
    using System;
    using System.Xml;

    public class DelayTrace : WorkflowTraceStep
    {
        private TimeSpan _timeSpan;

        public DelayTrace(TimeSpan timeSpan)
        {
            this._timeSpan = timeSpan;
        }

        internal TimeSpan TimeSpan
        {
            get
            {
                return _timeSpan;
            }
        }

        protected override void WriteInnerXml(XmlWriter writer)
        {
            writer.WriteAttributeString("timeSpan", this._timeSpan.ToString());

            base.WriteInnerXml(writer);
        }
    }
}
