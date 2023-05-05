// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.TestObjects.Utilities.Validation
{
    using System;
    using System.Globalization;
    using System.Xml;
    using Microsoft.Test.Xaml.Common.TestObjects.Utilities.Trace;

    [Serializable]
    public class UserTrace : WorkflowTraceStep, IActualTraceStep
    {
        private readonly string _message;
        private DateTime _timeStamp;
        private int _validated;

        public UserTrace(string message)
        {
            this._message = message;
        }

        internal string Message
        {
            get
            {
                return _message;
            }
        }

        protected override void WriteInnerXml(XmlWriter writer)
        {
            writer.WriteAttributeString("message", this._message);

            base.WriteInnerXml(writer);
        }

        public override string ToString()
        {
            return ((IActualTraceStep) this).GetStringId();
        }

        public override bool Equals(object obj)
        {
            UserTrace trace = obj as UserTrace;
            if (trace != null)
            {
                if (this.ToString() == trace.ToString())
                {
                    return true;
                }
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IActualTraceStep implementation

        DateTime IActualTraceStep.Timestamp
        {
            get
            {
                return _timeStamp;
            }
            set
            {
                _timeStamp = value;
            }
        }

        int IActualTraceStep.Validated
        {
            get
            {
                return _validated;
            }
            set
            {
                _validated = value;
            }
        }

        bool IActualTraceStep.Equals(IActualTraceStep trace)
        {
            UserTrace userTrace = trace as UserTrace;

            if (userTrace != null &&
                userTrace._message == this._message)
            {
                return true;
            }

            return false;
        }

        string IActualTraceStep.GetStringId()
        {
            string stepId = String.Format(
                CultureInfo.InvariantCulture,
                "UserTrace: {0}",
                this._message);

            return stepId;
        }

        #endregion

        private readonly Guid _instanceId;

        internal UserTrace(Guid instanceId, string message) : this(message)
        {
            this._instanceId = instanceId;
        }

        internal Guid InstanceId
        {
            get
            {
                return this._instanceId;
            }
        }

        #region UserTrace helpers

        public static void Trace(Guid instanceId, string format, params object[] args)
        {
            Trace(instanceId, String.Format(CultureInfo.InvariantCulture, format, args));
        }

        public static void Trace(Guid instanceId, string message)
        {
            TestTraceManager.Instance.AddTrace(instanceId, new UserTrace(message));
        }

        #endregion
    }
}
