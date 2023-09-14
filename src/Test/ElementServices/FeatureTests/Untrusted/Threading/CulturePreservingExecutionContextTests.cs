// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.Win32;
using Microsoft.Test.Utilities;

namespace Avalon.Test.CoreUI.Threading
{
    /// <summary>
    /// Tests for CulturePreservingExecutionContext
    /// 
    /// On .NET 4.6+, we introduce a helper type MS.Internal.CulturePreservingExecutionContext 
    /// which acts as a wrapper around ExecutionContext, and is used by Dispatcher and DispatcherOperation. 
    /// 
    /// CPEC is introduced to work around the fact that EC.Run reverts any changes to culture performed 
    /// by the callback as soon as the callback is completed. This EC behavior was introduced in 
    /// .NET 4.6. 
    /// 
    /// We also introduce an appcompat quirk, Switch.MS.Internal.DoNotUseCulturePreservingDispatcherOperations, 
    /// which can be set by applications wishing for WPF's behavior sans CPEC - i.e., just let EC do 
    /// what it does, and do not work around the fact that EC.Run would revert any culture changes
    /// made by the callback it executes during a DispatcherOperation. 
    /// 
    /// The following tests also make use of two other appcompat quirks:
    ///     i. TestSwitch.LocalAppContext.DisableCaching
    ///             This tells System.AppContext not to cache values of quirks. This allows us (i.e., the test)
    ///             to dynamically alter the value of other intersting quirks so that they can be tested. 
    ///     ii. Switch.System.Globalization.NoAsyncCurrentCulture
    ///             This quirk was introduced by ExecutionContext when its behavior was changed in .NET 4.6
    ///             to include culture related information to the set of things tracked by the EC (and thus 
    ///             reverted at the end of EC.Run)
    ///             We always set this switch to False in these tests. This is to ensure that we test
    ///             the new behavior of EC. 
    /// </summary>
    /// <remarks>
    ///     Area: ElementServices
    ///     SubArea: Threading
    ///     Name: CulturePreservingExecutionContextTests
    ///     Applicable versions: .NET 4.6 and later. 
    ///         If the /Versions switch is specified (in RunTests.cmd or QualityVaultFrontEnd.exe) during test execution, 
    ///         then this test will run *iff* a version of 4.6 or later is specified. The test will not run if a version 
    ///         of 4.5.2 or older is specified. 
    ///         
    ///         If the /Versions switch is not specified, the test will be scheduled to be run. 
    ///     Build notes: 
    ///         This test will only be built when WPF_TESTBUILD_TARGETVERSION is set to v4.6 or later 
    ///         either explicitly, or implicitly determined to be such by ..\Test\Microsoft.Wpf.Test.Versions.targets
    /// </remarks>
    [Test(1, "Threading", TestCaseSecurityLevel.FullTrust, "CulturePreservingExecutionContextTests", Versions = "4.6+")]
    public class CulturePreservingExecutionContextTests : AvalonTest
    {
        /// <summary>
        /// Constructor
        ///     Schedules test cases
        /// </summary>
        public CulturePreservingExecutionContextTests()
        {
            RunSteps += new TestStep(TestCulturePreservingDispatcherOperations);
            RunSteps += new TestStep(TestCultureRevertingDispatcherOperations);
        }

        /// <summary>
        /// Static constructor:
        ///     Initializes appcontext switches. 
        /// </summary>
        static CulturePreservingExecutionContextTests()
        {
            try
            {
                AppContextHelper.DisableCaching = true;
                AppContextHelper.NoAsyncCurrentCulture = false;
            }
            catch(TypeLoadException)
            {
                // Do nothing 
                //
                // If this fails, it is likely that the test
                // is being run on .NET 4.5.2 or an older framework
                // that does not support System.AppContext
                // 
                // The non-static constructor will catch this 
                // condition and ignore the test entirely.  
            }
        }


        #region Test1 

        /// <summary>
        /// When CulturePreservingExecutionContext is being used, we test to see that culture updates 
        /// are preserved across dispatcher operations. 
        /// </summary>
        /// <returns></returns>
        private TestResult TestCulturePreservingDispatcherOperations()
        { 
            _isCulturePreserved = false;

            GlobalLog.LogStatus("\tSetting {0}=false", AppContextHelper.DoNotUseCulturePreservingDispatcherOperationsName);
            AppContextHelper.DoNotUseCulturePreservingDispatcherOperations = false;

            var dispatcher = Dispatcher.CurrentDispatcher;

            dispatcher.BeginInvoke(new Action(ChangeCulture), DispatcherPriority.Background, null);
            dispatcher.BeginInvoke(new Action(VerifyCultureIsPreservedAndUpdateResult), DispatcherPriority.Background, null);

            // Wait for all operations to complete
            DispatcherHelper.DoEvents(DispatcherPriority.Background);

            return _isCulturePreserved ? TestResult.Pass : TestResult.Fail;
        }

        private void VerifyCultureIsPreservedAndUpdateResult()
        {
            GlobalLog.LogStatus("\tVerifying Culture...");
            ShowAllSwitchStatuses("\t\t");

            GlobalLog.LogStatus("\t\tCurrentUICulture = {0}", Thread.CurrentThread.CurrentUICulture.IetfLanguageTag);
            GlobalLog.LogStatus("\t\tExpected Culture = {0}", _newCulture.IetfLanguageTag);

            if (string.Equals(
                Thread.CurrentThread.CurrentUICulture.IetfLanguageTag,
                _newCulture.IetfLanguageTag,
                StringComparison.InvariantCultureIgnoreCase))
            {
                _isCulturePreserved = true;
            }

            GlobalLog.LogStatus("\t\tResult: {0}", _isCulturePreserved);
        }

        #endregion

        #region Test2 

        /// <summary>
        /// When CulturePreservingExecutionContext is turned off, we test to see that 
        /// culture updates are NOT preserved across dispatcher operations.
        /// </summary>
        /// <returns></returns>
        private TestResult TestCultureRevertingDispatcherOperations()
        {
            _wasCultureReverted = false;

            GlobalLog.LogStatus("\tSetting {0}=true", AppContextHelper.DoNotUseCulturePreservingDispatcherOperationsName);
            AppContextHelper.DoNotUseCulturePreservingDispatcherOperations = true;

            var dispatcher = Dispatcher.CurrentDispatcher;

            dispatcher.BeginInvoke(new Action(ChangeCulture), DispatcherPriority.Background, null);
            dispatcher.BeginInvoke(new Action(VerifyCultureIsNotPreservedAndUpdatedResult), DispatcherPriority.Background, null);

            // Wait for all operations to complete
            DispatcherHelper.DoEvents(DispatcherPriority.Background);

            return _wasCultureReverted ? TestResult.Pass : TestResult.Fail;
        }

        private void VerifyCultureIsNotPreservedAndUpdatedResult()
        {
            ShowAllSwitchStatuses("\t\t");

            GlobalLog.LogStatus("\t\tCurrentUICulture = {0}", Thread.CurrentThread.CurrentUICulture.IetfLanguageTag);
            GlobalLog.LogStatus("\t\tExpectation: CurrentUICulture should NOT be = {0}", _newCulture.IetfLanguageTag);

            if (!string.Equals(
                Thread.CurrentThread.CurrentUICulture.IetfLanguageTag,
                _newCulture.IetfLanguageTag,
                StringComparison.InvariantCultureIgnoreCase))
            {
                _wasCultureReverted = true;
            }


            GlobalLog.LogStatus("\t\tResult: {0}", _wasCultureReverted);
        }

        #endregion

        #region Helper Methods

        private void ChangeCulture()
        {
            var installedCultures =
                new List<CultureInfo>(CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures));

            _oldCulture = Thread.CurrentThread.CurrentUICulture;

            // Pick *any* culture that is not the same as the current UI culture
            _newCulture = installedCultures.Find((culture) => culture.IetfLanguageTag != _oldCulture.IetfLanguageTag);

            if (_newCulture != null)
            {
                Thread.CurrentThread.CurrentCulture = _newCulture;
                Thread.CurrentThread.CurrentUICulture = _newCulture;
                GlobalLog.LogStatus("\tUpdated culture. Old:{0}, New:{1}", _oldCulture.IetfLanguageTag, _newCulture.IetfLanguageTag);
            }
            else
            {
                GlobalLog.LogStatus("\t***********ERROR: Culture could not be updated***********");
            }
        }

        private bool ShowAllSwitchStatuses(string logPrefix)
        {
            GlobalLog.LogStatus("{0}AppContext Switches:", logPrefix);

            Action<bool?, string> ShowSwitchStatus = (bool? v, string name) =>
            {
                if (v.HasValue)
                {
                    GlobalLog.LogStatus("{0}\t{1} = {2}", logPrefix, name, v.Value);
                }
                else
                {
                    GlobalLog.LogStatus("{0}\t{1} = {2}", logPrefix, name, "Undefined");
                }
            };

            try
            {
                ShowSwitchStatus(AppContextHelper.DisableCaching, 
                    AppContextHelper.DisableCachingName);

                ShowSwitchStatus(AppContextHelper.NoAsyncCurrentCulture, 
                    AppContextHelper.NoAsyncCurrentCultureName);

                ShowSwitchStatus(AppContextHelper.DoNotUseCulturePreservingDispatcherOperations, 
                    AppContextHelper.DoNotUseCulturePreservingDispatcherOperationsName);
            }
            catch (Exception e)
            {
                GlobalLog.LogStatus(string.Empty);
                GlobalLog.LogStatus("{0}\tERROR: AppContext API's are not supported", logPrefix);
                GlobalLog.LogStatus(string.Empty);

                GlobalLog.LogStatus("{0}{1}", logPrefix, e.ToString());
                GlobalLog.LogStatus("{0}{1}", logPrefix, e.StackTrace.ToString());


                return false;
            }

            GlobalLog.LogStatus(string.Empty);

            return true;
        }

        #endregion

        #region Private Fields 

        private bool _isCulturePreserved;
        private bool _wasCultureReverted;

        private CultureInfo _oldCulture;
        private CultureInfo _newCulture;

        #endregion
    }

    /// <summary>
    /// Helper to set/read various app-compat quirks that are 
    /// backed by System.AppContext
    /// 
    /// For an explanation of what each of these quirks mean, 
    /// see the summary comment <see cref="CulturePreservingExecutionContextTests"/>
    /// </summary>
    /// <remarks>
    /// The properties are <see cref="Nullable{Boolean}"/> to help 
    /// represent them as a tri-state value. 
    ///     null: The quirk has no value set - it is neither true nor false. 
    ///     false: The default - indicates that the *new* behavior is in effect. 
    ///     true: Indicates that the quirk is set, and the *old* behavior has been rquested.
    /// </remarks>
    static class AppContextHelper
    {
        public static bool? DoNotUseCulturePreservingDispatcherOperations
        {
            set
            {
                Set(DoNotUseCulturePreservingDispatcherOperationsName, value);
            }

            get
            {
                return Get(DoNotUseCulturePreservingDispatcherOperationsName);
            }
        }

        public static bool? DisableCaching
        {
            set
            {
                Set(DisableCachingName, value);
            }

            get
            {
                return Get(DisableCachingName);
            }
        }

        public static bool? NoAsyncCurrentCulture
        {
            set
            {
                Set(NoAsyncCurrentCultureName, value);
            }

            get
            {
                return Get(NoAsyncCurrentCultureName);
            }
        }

        private static bool? Get(string name)
        {
            bool result;
            if (AppContext.TryGetSwitch(name, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        private static void Set(string name, bool? v)
        {
            if (v == null)
            {
                throw new ArgumentNullException();
            }

            AppContext.SetSwitch(name, v.Value);
        }

        public static readonly string DoNotUseCulturePreservingDispatcherOperationsName = 
            "Switch.MS.Internal.DoNotUseCulturePreservingDispatcherOperations";
        public static readonly string DisableCachingName = 
            "TestSwitch.LocalAppContext.DisableCaching";
        public static readonly string NoAsyncCurrentCultureName = 
            "Switch.System.Globalization.NoAsyncCurrentCulture";
    }

}


