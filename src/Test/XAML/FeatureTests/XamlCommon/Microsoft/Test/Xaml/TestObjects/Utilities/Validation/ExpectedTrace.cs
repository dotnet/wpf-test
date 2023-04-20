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

    /// <summary>
    /// Root class of the expected trace data structure
    /// </summary>
    /// 
#if !SILVERLIGHT
    [Serializable]
#endif
    public class ExpectedTrace
    {
        /// <summary>
        /// Verify than no other steps occured, except for listed ones
        /// </summary>
        /// 
        internal bool verifyCompleteness = true;

        /// <summary>
        /// Depending on the value of userVerifyTypes, either verify only types on the verify list
        /// or verify only types not in the ignore list
        /// </summary>
        private readonly HashSet<Type> _verifyTypes = new HashSet<Type>();

        private readonly HashSet<Type> _ignoreTypes = new HashSet<Type>();

        private bool _useVerifyTypes = false;

        public void AddIgnoreTypes(params Type[] types)
        {
            _useVerifyTypes = false;

            if (types == null)
            {
                return;
            }

            foreach (Type ignoreType in types)
            {
                _ignoreTypes.Add(ignoreType);
            }
        }

        public void AddVerifyTypes(params Type[] types)
        {
            _useVerifyTypes = true;

            if (types == null)
            {
                return;
            }

            foreach (Type verifyType in types)
            {
                _verifyTypes.Add(verifyType);
            }
        }

        // <summary>
        // Maintain a list of ignore activities: ".*: Cancelling" or "CAG1: .*"
        // </summary>
        //public List<string> IgnoreList = new List<string>();
        public TraceGroup Trace { get; set; }
        
        public ExpectedTrace()
        {
        }

        public ExpectedTrace(TraceGroup trace)
        {
            this.Trace = trace;
        }

        public ExpectedTrace(ExpectedTrace expectedTrace)
        {
            this.Trace = TraceGroup.GetNewTraceGroup(expectedTrace.Trace);
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
                    Trace.WriteXml(xmlWriter);
                }
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// Depending on the value of userVerifyTypes, either verify only types on the verify list
        /// or verify only types not in the ignore list
        /// </summary>
        /// <param name="step">Step to check</param>
        internal bool CanBeIgnored(IActualTraceStep step)
        {
            if (_useVerifyTypes)
            {
                return !_verifyTypes.Contains(step.GetType());
            }
            else
            {
                return _ignoreTypes.Contains(step.GetType());
            }
        }
    }
}
