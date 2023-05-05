// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where the WeakEventManager doesn't work with static events
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "WeakEventManagerStatic")]
    public class WeakEventManagerStatic : AvalonTest
    {
        #region Constructors

        public WeakEventManagerStatic()            
        {
            InitializeSteps += new TestStep(Validate);                        
        }

        #endregion

        #region Private Members        
                
        private TestResult Validate()
        {
            Client normalClient = new Client();
            normalClient.Run();

            if (normalClient.EventRaisedFlag == false)
            {
                LogComment("Static Event was not listened by the WeakEventManager.");
                return TestResult.Fail;
            }
            else{

                LogComment("Static Event was not listened by the WeakEventManagerasdadd.");
            }
            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes

    
    public static class PersonService
    {
        public static event EventHandler ActivePersonChanged;

        public static void RaiseActivePersonChanged()
        {
            if (ActivePersonChanged != null) { ActivePersonChanged(null, EventArgs.Empty); }
        }
    }


    public class Client : IWeakEventListener
    {
        public bool EventRaisedFlag;

        public Client()
        {
            EventRaisedFlag = false;
            PersonChangedEventManager.AddListener(this);
        }

        public void Run()
        {
            PersonService.RaiseActivePersonChanged();
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            EventRaisedFlag = true;            
            return true;
        }
    }


    public class PersonChangedEventManager : WeakEventManager
    {
        private static PersonChangedEventManager CurrentManager
        {
            get
            {
                Type managerType = typeof(PersonChangedEventManager);
                PersonChangedEventManager currentManager = (PersonChangedEventManager)GetCurrentManager(managerType);
                if (currentManager == null)
                {
                    currentManager = new PersonChangedEventManager();
                    SetCurrentManager(managerType, currentManager);
                }
                return currentManager;
            }
        }

        public static void AddListener(IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(null, listener);
        }

        public static void RemoveListener(IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(null, listener);
        }

        protected override void StartListening(object source)
        {
            PersonService.ActivePersonChanged += PersonServiceActivePersonChanged;
        }

        protected override void StopListening(object source)
        {
            PersonService.ActivePersonChanged -= PersonServiceActivePersonChanged;
        }

        private void PersonServiceActivePersonChanged(object sender, EventArgs e)
        {            
            base.DeliverEvent(sender, e);
        }
    }
    
    #endregion
}
