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
    using System.Xml;
    using System.Runtime.Serialization;

    #region WorkflowTraceStep

#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class WorkflowTraceStep
    {
        private bool _optional = false;

        public bool Optional
        {
            get
            {
                return _optional;
            }
            set
            {
                _optional = value;
            }
        }

        internal bool Async
        {
            get
            {
                return false;
            } // keep it, in case it's needed later (validation engine can handle async steps)
            set
            {
            }
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(this.GetType().Name);

            WriteInnerXml(writer);

            writer.WriteEndElement();
        }

        protected virtual void WriteInnerXml(XmlWriter writer)
        {
            if (this.Async)
            {
                writer.WriteAttributeString("async", this.Async.ToString());
            }

            if (this._optional)
            {
                writer.WriteAttributeString("optional", this._optional.ToString());
            }
        }
    }

    #endregion

    #region IActualTraceStep

    public interface IActualTraceStep
    {
        DateTime Timestamp { get; set; }
        int Validated { get; set; }
        bool Equals(IActualTraceStep trace);
        string GetStringId();
    }

    #endregion

    #region Trace groups

    [Serializable]
    public abstract class TraceGroup : WorkflowTraceStep
    {
        private readonly List<WorkflowTraceStep> _steps;
        internal bool ordered;
        internal TraceGroup parent = null;
        internal int indexInParent = -1;

        internal int startIndex = -1;
        internal int[] endIndexes;

        protected TraceGroup(WorkflowTraceStep[] steps, bool ordered)
        {
            this._steps = new List<WorkflowTraceStep>(steps);
            this.ordered = ordered;
        }

        public List<WorkflowTraceStep> Steps
        {
            get
            {
                return _steps;
            }
        }

        protected override void WriteInnerXml(XmlWriter writer)
        {
            base.WriteInnerXml(writer);

            foreach (WorkflowTraceStep trace in this._steps)
            {
                trace.WriteXml(writer);
            }
        }

        //makes a copy of the existing trace group
        public static TraceGroup GetNewTraceGroup(TraceGroup traceGroup)
        {
            TraceGroup newTraceGroup = null;
            if (traceGroup.ordered)
            {
                newTraceGroup = new OrderedTraces();
            }
            else
            {
                newTraceGroup = new UnorderedTraces();
            }

            foreach (WorkflowTraceStep step in traceGroup.Steps)
            {
                UserTrace userTrace = step as UserTrace;
                if (userTrace != null)
                {
                    newTraceGroup.Steps.Add(new UserTrace(userTrace.InstanceId, userTrace.Message));
                    continue;
                }

                TraceGroup tempTraceGroup = step as TraceGroup;
                if (tempTraceGroup != null)
                {
                    newTraceGroup.Steps.Add(TraceGroup.GetNewTraceGroup(tempTraceGroup));
                    continue;
                }
            }
            return newTraceGroup;
        }

        public override bool Equals(object obj)
        {
            TraceGroup traceGroup = obj as TraceGroup;
            if (traceGroup != null)
            {
                return (this.ToString() == traceGroup.ToString());
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class OrderedTraces : TraceGroup
    {
        public OrderedTraces(params WorkflowTraceStep[] steps)
            : base(steps, true)
        {
        }
    }

    public class UnorderedTraces : TraceGroup
    {
        public UnorderedTraces(params WorkflowTraceStep[] steps)
            : base(steps, false)
        {
        }
    }

    #endregion

    #region ValidationFailedException

    [Serializable]
    public class ValidationFailedException : Exception
    {
        public ValidationFailedException()
            : base()
        {
        }

        public ValidationFailedException(string message)
            : base(message)
        {
        }

        protected ValidationFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public ValidationFailedException(String message, Exception innerEx) : base(message, innerEx) { }
    }

    #endregion
}
