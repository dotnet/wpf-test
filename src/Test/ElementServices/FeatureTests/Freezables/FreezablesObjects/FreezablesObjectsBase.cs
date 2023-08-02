// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test.Logging;
using Microsoft.Test.ElementServices.Freezables.Objects;

namespace Microsoft.Test.ElementServices.Freezables
{
    /**********************************************************************************
    * CLASS:          FreezablesObjectsBase
    **********************************************************************************/
    public abstract class FreezablesObjectsBase
    {
        #region Private Data

        protected string                            testName;
        protected string                            objName;
        protected StringCollection                  failures;
        protected bool                              passed;
        protected List<Freezable>                   freezables;
        protected ArrayList                         dataBindingObjs;

        #endregion


        #region Constructors
        /******************************************************************************
        * Function:          FreezablesObjectsBase
        ******************************************************************************/
        public FreezablesObjectsBase(string test, string obj)
        {
            testName    = test;
            objName     = obj;
            failures    = new StringCollection();
            passed      = true;

            GlobalLog.LogStatus("---Testing: " + testName + " -- Object: " + objName);
        }
        #endregion


        #region Public and Protected Members

        /******************************************************************************
        * Function:          Perform
        ******************************************************************************/
        public bool Perform()
        {
            freezables = new List<Freezable>();
            dataBindingObjs = new ArrayList();

            Assembly dll = typeof(Animatable).Assembly;
            Type t = dll.GetType("System.Windows.Media." + objName);

            if (t == null)
            {
                throw new ApplicationException("ERROR -- Perform: Type is null.");
            }
            else
            {
                freezables.Add(FreezableFactory.Make(t));

                if (testName == "DataBindingTest")
                {
                    dataBindingObjs.Add(DataBindingFactory.Make(t));
                }

                RunTest();

                LogResult();
            }
            return true;
        }

        /******************************************************************************
        * Function:          GetMatchingDP
        ******************************************************************************/
        protected DependencyProperty GetMatchingDP(PropertyInfo pi, Freezable freezable)
        {
            FieldInfo[] fi = pi.DeclaringType.GetFields();
            for (int i = 0; i < fi.Length; i++)
            {
                if (fi[i].FieldType.IsPublic 
                    && fi[i].FieldType.ToString() == "System.Windows.DependencyProperty" 
                    && fi[i].Name == pi.Name + "Property")
                {
                    return (DependencyProperty)fi[i].GetValue(freezable);
                }
            } 
            return null;
        }

        /******************************************************************************
        * Function:          DoesCauseStackOverflow
        ******************************************************************************/
        protected bool DoesCauseStackOverflow(Freezable freezable, PropertyInfo property)
        {
            if (freezable.GetType().ToString() == "System.Windows.Media.MatrixTransform" && property.Name == "Inverse")
            {
                return true;
            }
            return false;
        }

        /******************************************************************************
        * Function:          LogResult
        ******************************************************************************/
        protected void LogResult()
        {
            if (!passed)
            {
                GlobalLog.LogEvidence("---------------------------------------------------------------------");
                GlobalLog.LogEvidence("TestName = " + testName + " (" + objName + ") - FAILURE REPORT");
                GlobalLog.LogEvidence("---------------------------------------------------------------------");
                for (int i = 0; i < failures.Count; i++)
                {
                    GlobalLog.LogEvidence(failures[i]);
                }
            }

            FreezablesObjects.testPassed = passed;
        }


        /******************************************************************************
        * Function:          RunTest
        ******************************************************************************/
        public abstract  void RunTest();
     
        /******************************************************************************
        * Function:          Dispose
        ******************************************************************************/
        public void Dispose() { }

        #endregion
    }
}

