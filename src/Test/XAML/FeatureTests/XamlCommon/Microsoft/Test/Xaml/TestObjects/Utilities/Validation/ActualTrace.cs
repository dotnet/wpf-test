// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

////////////////////////////////////////////////////////////
// This file contains core tracing/validation types. No Oslo product
// dependencies or non-SilverLight compatible code should be
// introduced.

namespace Microsoft.Test.Xaml.Common.TestObjects.Utilities.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xml;

#if !SILVERLIGHT
    [Serializable]
#endif
    public class ActualTrace
    {
        public ActualTrace()
        {
        }

        public ActualTrace(ActualTrace actualTrace)
        {
            foreach (IActualTraceStep step in actualTrace.Steps)
            {
                UserTrace userTrace = step as UserTrace;
                if (userTrace != null)
                {
                    this.Steps.Add(new UserTrace(userTrace.InstanceId, userTrace.Message));
                    continue;
                }
            }
        }

        private readonly List<IActualTraceStep> _steps = new List<IActualTraceStep>();

        public void Add(IActualTraceStep step)
        {
            lock (this._steps)
            {
                if (step.Timestamp == default(DateTime))
                {
                    step.Timestamp = DateTime.Now;
                }

                this._steps.Add(step);
            }
        }

        public void Validate(ExpectedTrace expectedTrace)
        {
            lock (this._steps)
            {
                TraceValidator.Validate(this, expectedTrace);
            }
        }

        internal List<IActualTraceStep> Steps
        {
            get
            {
                return this._steps;
            }
        }

        public override string ToString()
        {
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                XmlWriterSettings xmlSettings = new XmlWriterSettings();
                xmlSettings.Indent = true;
                xmlSettings.OmitXmlDeclaration = true;
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlSettings))
                {
                    OrderedTraces orderedTraces = new OrderedTraces();

                    foreach (IActualTraceStep step in _steps)
                    {
                        if (step is WorkflowTraceStep)
                        {
                            orderedTraces.Steps.Add(step as WorkflowTraceStep);
                        }
                    }

                    orderedTraces.WriteXml(xmlWriter);
                }
                return stringWriter.ToString();
            }
        }
    }
}
