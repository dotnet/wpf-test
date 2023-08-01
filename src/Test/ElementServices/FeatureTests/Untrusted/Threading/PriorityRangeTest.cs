// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using Microsoft.Test.Discovery;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI.Trusted;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Threading;
using System.Collections.Generic;

namespace Avalon.Test.CoreUI.Threading
{

    ///<summary>
    /// Main Class that holds the tests for PriorityRange Class
    ///</summary>
    [TestDefaults]
    public class PriorityRangeTest : TestCaseBase
    {
        /*        List<DispatcherPriority> _priorityList = new List<DispatcherPriority>();

                ///<summary>
                ///</summary>
                [Test(1, "Threading", TestCaseSecurityLevel.PartialTrust, "Testing Ilde, Foreground and Background priority range.")]
                public void TestingPriorityChangesAllOperations()
                {
                    DispatcherPriority[] priorities = Microsoft.Test.Threading.DispatcherPriorityHelper.GetAllDispatcherPriorities();


                    _priorityList.AddRange(priorities);
                    _priorityList.Sort();

                    List<CurrentPriorityChanges> cpcList = new List<CurrentPriorityChanges>();

                    cpcList.Add(new CurrentPriorityChanges("Idle", Dispatcher.IdlePriorityRange, DispatcherPriority.ContextIdle, DispatcherPriority.SystemIdle));
                    cpcList.Add(new CurrentPriorityChanges("Foreground", Dispatcher.ForegroundPriorityRange, DispatcherPriority.Send, DispatcherPriority.Loaded));
                    cpcList.Add(new CurrentPriorityChanges("Background", Dispatcher.BackgroundPriorityRange, DispatcherPriority.Input, DispatcherPriority.Background));


                    TestProperties(cpcList);
                    TestEquals(cpcList);
                    TestContains(cpcList);
            
                }

                void TestContains(List<CurrentPriorityChanges> cpcList)            
                {
                    for (int i = 0; i < cpcList.Count; i++)
                    {
                        for (int j = 0; j < _priorityList.Count; j++)
                        {
                            bool isEntered = false;
                            if (_priorityList[j] >= cpcList[i].PR.Min && cpcList[i].IncludeMin)
                            {
                                isEntered = true;
                            }
                            else if (_priorityList[j] > cpcList[i].PR.Min && !cpcList[i].IncludeMin)
                            {
                                isEntered = true;
                            }

                            if (isEntered)
                            {
                                isEntered = false;
                                if (_priorityList[j] <= cpcList[i].PR.Max && cpcList[i].IncludeMax)
                                {
                                    isEntered = true;
                                }
                                else if (_priorityList[j] < cpcList[i].PR.Min && !cpcList[i].IncludeMax)
                                {
                                    isEntered = true;
                                }
                            }

                            if (cpcList[i].PR.Contains(_priorityList[j]) != isEntered)
                            {
                                CoreLogger.LogTestResult(false, "PriorityRange.Contains for "+ cpcList[i].Name +" is not correct value.");
                            }                    
                        }               
                    }
                }

                void TestProperties(List<CurrentPriorityChanges> cpcList)
                {
                    for (int i = 0; i<cpcList.Count; i++)
                    {
                        if (cpcList[i].PR.Min != cpcList[i].Min)
                        {           
                            CoreLogger.LogTestResult(false, "The Min property on "+ cpcList[i].Name +" is not correct");
                        }

                        if (cpcList[i].PR.Max != cpcList[i].Max)
                        {           
                            CoreLogger.LogTestResult(false, "The Max property on "+ cpcList[i].Name +" is not correct");
                        }


                        if (cpcList[i].PR.IsMaxInclusive != cpcList[i].IncludeMax)
                        {           
                            CoreLogger.LogTestResult(false, "The IsMaxInclusive property on "+ cpcList[i].Name +" is not correct");
                        }
                
                        if (cpcList[i].PR.IsMinInclusive != cpcList[i].IncludeMin)
                        {           
                            CoreLogger.LogTestResult(false, "The IsMinInclusive property on "+ cpcList[i].Name +" is not correct");
                        }

                        if (cpcList[i].PR.IsValid != cpcList[i].IsValid)
                        {           
                            CoreLogger.LogTestResult(false, "The IsValid property on "+ cpcList[i].Name +" is not correct");
                        }
                    }
                }


                void TestEquals(List<CurrentPriorityChanges> cpcList)
                {
                    for (int i = 0; i<cpcList.Count; i++)
                    {
                        for (int j = 0; j < cpcList.Count; j++)
                        {
                            if (cpcList[i] == cpcList[j])
                            {
                                if (!cpcList[i].PR.Equals(cpcList[j].PR))
                                {
                                    CoreLogger.LogTestResult(false, "The PriorityRange.Equals should return true. Actual false.");
                                }

                                if (!(cpcList[i].PR == cpcList[j].PR))
                                {
                                    CoreLogger.LogTestResult(false, "The PriorityRange == should return true. Actual false.");
                                }

                                if (cpcList[i].PR != cpcList[j].PR)
                                {
                                    CoreLogger.LogTestResult(false, "The PriorityRange != should return true. Actual false.");
                                }                        
                            }
                            else
                            {
                                if (cpcList[i].PR.Equals(cpcList[j].PR))
                                {
                                    CoreLogger.LogTestResult(false, "The PriorityRange.Equals should return false. Actual true.");
                                }


                                if (cpcList[i].PR == cpcList[j].PR)
                                {
                                    CoreLogger.LogTestResult(false, "The PriorityRange == should return false. Actual true.");
                                }
                        
                                if (!(cpcList[i].PR != cpcList[j].PR))
                                {
                                    CoreLogger.LogTestResult(false, "The PriorityRange != should return false. Actual true.");
                                }                        
                            }
                        }                
                    }

                    if (cpcList[0].PR.Equals(null) || cpcList[0].PR.Equals(4))
                    {
                        CoreLogger.LogTestResult(false, "The PriorityRange.Equals should return false. Actual true.");    
                    }
                }


                class CurrentPriorityChanges
                {
                    public CurrentPriorityChanges(string name,PriorityRange range , DispatcherPriority max, DispatcherPriority min)
                    {
                        Name = name;
                        Max = max;
                        PR = range;
                        Min = min;
                    }

                    public PriorityRange PR;
            
                    public string Name;

                    public DispatcherPriority Max;

                    public DispatcherPriority Min;

                    public bool IncludeMax = true;

                    public bool IncludeMin = true;

                    public bool IsValid = true;

                }
        */
    }
}


