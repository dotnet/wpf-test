// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//////////////////////////////////////////////////////////
// This file contains core tracing/validation types. No Oslo product
// dependencies or non-SilverLight compatible code should be
// introduced.

namespace Microsoft.Test.Xaml.Common.TestObjects.Utilities.Trace
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.Test.Xaml.Common.TestObjects.Utilities.Validation;

    public sealed class TestTraceManager : MarshalByRefObject
    {
        private static readonly TestTraceManager s_instance = new TestTraceManager();

        private readonly Dictionary<Guid, ActualTrace> _allTraces = new Dictionary<Guid, ActualTrace>();
        private readonly Dictionary<Guid, List<Subscription>> _allSubscriptions = new Dictionary<Guid, List<Subscription>>();
        private readonly HashSet<Guid> _allKnownTraces = new HashSet<Guid>();

        private readonly List<string> _traceFilter = new List<string>();

        private readonly object _thisLock = new object();

        private TestTraceManager()
        {
            AddCompensationFilterTraces();
        }

        public static TestTraceManager Instance
        {
            get
            {
                return s_instance;
            }
        }

        public Dictionary<Guid, ActualTrace> AllTraces
        {
            get
            {
                return _allTraces;
            }
        }

        public List<string> TraceFilter
        {
            get
            {
                return this._traceFilter;
            }
        }

        public void AddTrace(Guid instanceId, IActualTraceStep trace)
        {
            if (trace == null)
            {
                throw new ArgumentNullException("trace");
            }

            lock (_thisLock)
            {
                ActualTrace instanceTraces = GetInstanceActualTrace(instanceId);
                instanceTraces.Add(trace);

                CheckSubscriptions(instanceId, instanceTraces);
            }
        }

        private void CheckSubscriptions(Guid instanceId, ActualTrace instanceTraces)
        {
            lock (_thisLock)
            {
                List<Subscription> workflowInstanceSubscriptions = null;
                if (_allSubscriptions.TryGetValue(instanceId, out workflowInstanceSubscriptions))
                {
                    List<Subscription> subscriptionsPendingRemoval = new List<Subscription>();

                    foreach (Subscription subscription in workflowInstanceSubscriptions)
                    {
                        if (subscription.NotifyTraces(instanceTraces))
                        {
                            subscriptionsPendingRemoval.Add(subscription);
                        }
                    }

                    foreach (Subscription subscription in subscriptionsPendingRemoval)
                    {
                        workflowInstanceSubscriptions.Remove(subscription);
                    }
                    if (workflowInstanceSubscriptions.Count == 0)
                    {
                        _allSubscriptions.Remove(instanceId);
                    }
                }
            }
        }

        private void AddSubscription(Guid instanceId, Subscription subscription)
        {
            lock (_thisLock)
            {
                List<Subscription> workflowInstanceSubscriptions = null;
                if (_allSubscriptions.TryGetValue(instanceId, out workflowInstanceSubscriptions))
                {
                    workflowInstanceSubscriptions.Add(subscription);
                }

                else
                {
                    workflowInstanceSubscriptions = new List<Subscription>();
                    workflowInstanceSubscriptions.Add(subscription);
                    _allSubscriptions.Add(instanceId, workflowInstanceSubscriptions);
                }

                // Make sure that the condition is not already met //
                CheckSubscriptions(instanceId, GetInstanceActualTrace(instanceId));
            }
        }

        public ActualTrace GetInstanceActualTrace(Guid instanceId)
        {
            lock (_thisLock)
            {
                ActualTrace instanceTraces = null;

                if (!_allTraces.TryGetValue(instanceId, out instanceTraces))
                {
                    instanceTraces = new ActualTrace();
                    _allTraces.Add(instanceId, instanceTraces);
                }

                return instanceTraces;
            }
        }

        public void MarkInstanceAsKnown(Guid id)
        {
            _allKnownTraces.Add(id);
        }

        private bool IsInstanceKnown(Guid id)
        {
            return _allKnownTraces.Contains(id);
        }

        public Guid GetLastInstanceId()
        {
            lock (_thisLock)
            {
                Guid lastTraceGuid = Guid.Empty;
                DateTime lastTimestamp = DateTime.MinValue;
                foreach (Guid key in this.AllTraces.Keys)
                {
                    DateTime timestamp = this.AllTraces[key].Steps[0].Timestamp;
                    if (!IsInstanceKnown(key) && timestamp > lastTimestamp)
                    {
                        lastTimestamp = timestamp;
                        lastTraceGuid = key;
                    }
                }

                if (lastTimestamp == DateTime.MinValue)
                {
                    throw new Exception("Couldn't find any more unknown instances");
                }

                return lastTraceGuid;
            }
        }

        public void AddFilterTrace(string displayNameToFilter)
        {
            lock (this._thisLock)
            {
                this._traceFilter.Add(displayNameToFilter);
            }
        }

        public void WaitForTrace(Guid workflowInstanceId, IActualTraceStep trace, int count)
        {
            ManualResetEvent mre = new ManualResetEvent(false);

            Subscription subscription = new Subscription(trace, count, mre);
            AddSubscription(workflowInstanceId, subscription);

            mre.WaitOne();
        }

        public void WaitForEitherOfTraces(Guid workflowInstanceId, IActualTraceStep trace, IActualTraceStep otherTrace, out IActualTraceStep successfulTrace)
        {
            ManualResetEvent mre = new ManualResetEvent(false);

            ORSubscription subscription = new ORSubscription(trace, otherTrace, mre);
            AddSubscription(workflowInstanceId, subscription);

            mre.WaitOne();

            successfulTrace = subscription.SuccessfulTraceStep;
        }

        private void AddCompensationFilterTraces()
        {
            AddFilterTrace("WorkflowCompensationBehavior");
            AddFilterTrace("CompensableActivity");
            AddFilterTrace("CompensationParticipant");
            AddFilterTrace("InternalConfirm");
            AddFilterTrace("InternalCompensate");
            AddFilterTrace("Confirm");
            AddFilterTrace("Compensate");
            AddFilterTrace("DefaultConfirmation");
            AddFilterTrace("DefaultCompensation");
        }

        internal class Subscription
        {
            protected IActualTraceStep traceStep;
            protected int count;
            protected ManualResetEvent manualResetEvent;

            public Subscription(IActualTraceStep traceStep, int numOccurance, ManualResetEvent mre)
            {
                this.traceStep = traceStep;
                this.count = numOccurance;
                this.manualResetEvent = mre;
            }

            internal virtual bool NotifyTraces(ActualTrace instanceTraces)
            {
                bool removeSubscription = false;

                int currentCount = count;
                foreach (IActualTraceStep step in instanceTraces.Steps)
                {
                    if (step.Equals(this.traceStep))
                    {
                        currentCount--;
                    }
                    if (currentCount == 0)
                    {
                        removeSubscription = true;
                        this.manualResetEvent.Set();
                        break;
                    }
                }

                return removeSubscription;
            }
        }

        internal class ORSubscription : Subscription
        {
            private readonly IActualTraceStep _otherTraceStep;
            private IActualTraceStep _successfulTraceStep;

            public ORSubscription(IActualTraceStep traceStep, IActualTraceStep otherTraceStep, ManualResetEvent mre)
                : base(traceStep, 1, mre)
            {
                this._otherTraceStep = otherTraceStep;
            }

            internal override bool NotifyTraces(ActualTrace instanceTraces)
            {
                bool foundTrace = false;
                foreach (IActualTraceStep step in instanceTraces.Steps)
                {
                    if (step.Equals(this.traceStep))
                    {
                        foundTrace = true;
                        this._successfulTraceStep = this.traceStep;
                    }
                    else if (step.Equals(this._otherTraceStep))
                    {
                        foundTrace = true;
                        this._successfulTraceStep = this._otherTraceStep;
                    }

                    if (foundTrace)
                    {
                        this.manualResetEvent.Set();
                        break;
                    }
                }

                return foundTrace;
            }

            internal IActualTraceStep SuccessfulTraceStep
            {
                get
                {
                    return this._successfulTraceStep;
                }
            }
        }
    }
}
