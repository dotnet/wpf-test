// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Test Hardware Tier API
    /// </summary>
    public class TierAPITest : CoreGraphicsTest
    {
        /// <summary/>
        public override void Init(Variation v)
        {
            base.Init(v);
            v.AssertExistenceOf("ExpectedHardwareTier");
            // NOTE: this value is read as DECIMAL, but is interpreted as HEXADECIMAL:
            //   StringConverter.ToInt() and related don't handle hex strings very well.
            _expectedHardwareTier = StringConverter.ToInt(v["ExpectedHardwareTier"]);

            #region TierChanged Test
            _tierChangedCount = 0;
            _testTierChangedEvent = false;
            if (v["TestTierChangedEvent"] != null)
            {
                _testTierChangedEvent = StringConverter.ToBool(v["TestTierChangedEvent"]);
            }
            if (_testTierChangedEvent)
            {
                _modeSwitchToolPath = "%PIPER_DIRECTORY%";
                if (v["ModeSwitchToolPath"] != null)
                {
                    _modeSwitchToolPath = v["ModeSwitchToolPath"];
                }
                _modeSwitchToolPath = EnvironmentWrapper.ExpandEnvironmentVariables(_modeSwitchToolPath);
                if (v["ModeSwitchWaitTime"] != null)
                {
                    _modeSwitchWaitTime = StringConverter.ToInt(v["ModeSwitchWaitTime"]);
                }
            }
            #endregion
        }

        /// <summary/>
        public override void RunTheTest()
        {
            TestExpectedTier();
        }

        private void TestExpectedTier()
        {
            Log("Getting the RenderCapability.Tier property ...");
            int currentRunningTier = RenderCapability.Tier;
            if (_expectedHardwareTier != currentRunningTier)
            {
                // Note Hardware Tiering is reported in HEXADECIMAL,
                //   where the hi word has major and the low word has minor version.
                AddFailure("Incorrect Hardware Tier found: Expected=0x{0} Actual=0x{1}.",
                    _expectedHardwareTier.ToString("X8"), currentRunningTier.ToString("X8"));
            }

            // If we have a hardware tier supported, use it to run event test when specified.
            if (currentRunningTier > 0 && _testTierChangedEvent)
            {
                Log("Hardware support detected, running TierChanged Event test.");
                TestTierChangedEvent();
            }
        }

        #region TierChanged Test
        private void TestTierChangedEvent()
        {
            // Create event handler
            EventHandler tierChanged = new EventHandler(OnTierChanged);

            // test adding event handler
            Log("Adding TierChanged Event ...");
            RenderCapability.TierChanged += tierChanged;

            _tierChangedCount = 0;
            ChangeVideoModeToSoftware();
            if (_tierChangedCount != 1)
            {
                AddFailure("TierChanged Event mismatched invocations on software switch. " +
                            "Expected={0} Actual={1}.", 1, _tierChangedCount);
            }

            _tierChangedCount = 0;
            ChangeVideoModeToHardware();
            if (_tierChangedCount != 1)
            {
                AddFailure("TierChanged Event mismatched invocations on hardware switch. " +
                            "Expected={0} Actual={1}.", 1, _tierChangedCount);
            }

            // test removing the event handler
            Log("Removing TierChanged Event ...");
            RenderCapability.TierChanged -= tierChanged;

            _tierChangedCount = 0;
            ChangeVideoModeToSoftware();
            if (_tierChangedCount != 0)
            {
                AddFailure("TierChanged Event fired on software switch after handler was removed. " +
                            "Expected={0} Actual={1}.", 1, _tierChangedCount);
            }

            _tierChangedCount = 0;
            ChangeVideoModeToHardware();
            if (_tierChangedCount != 0)
            {
                AddFailure("TierChanged Event fired on hardware switch after handler was removed. " +
                            "Expected={0} Actual={1}.", 1, _tierChangedCount);
            }
        }

        private void OnTierChanged(object sender, EventArgs e)
        {
            Log("TierChanged event handler fired...");
            _tierChangedCount++;
        }

        private void ChangeVideoModeToSoftware()
        {
            Log("Switching to Sofware video mode...");
            Log("DisplayConfig path is " + _modeSwitchToolPath);
            // NOTE: DisplayConfig requires 2 parameters after the /hardware switch.
            //   The second one is ignored ...
            Process p = Process.Start(_modeSwitchToolPath + @"\DisplayConfig.exe", "/hardware off 0");
            p.WaitForExit();
            Thread.Sleep(_modeSwitchWaitTime);

            int currentRunningTier = RenderCapability.Tier;
            if (currentRunningTier != 0)
            {
                // Note Hardware Tiering is reported in HEXADECIMAL,
                //   where the hi word has major and the low word has minor version.
                AddFailure("Incorrect Hardware Tier found: 0x{0}. Expected Software only tier. ",
                    currentRunningTier.ToString("X8"));
            }
        }

        private void ChangeVideoModeToHardware()
        {
            Log("Switching to Hardware video mode...");
            Log("DisplayConfig path is " + _modeSwitchToolPath);
            // NOTE: DisplayConfig requires 2 parameters after the /hardware switch.
            //   The second one is ignored ...
            Process p = Process.Start(_modeSwitchToolPath + @"\DisplayConfig.exe", "/hardware on 0");
            p.WaitForExit();
            Thread.Sleep(_modeSwitchWaitTime);

            int currentRunningTier = RenderCapability.Tier;
            if (currentRunningTier == 0)
            {
                // Note Hardware Tiering is reported in HEXADECIMAL,
                //   where the hi word has major and the low word has minor version.
                AddFailure("Incorrect Hardware Tier found: 0x{0}. Expected Hardware tier. ",
                    currentRunningTier.ToString("X8"));
            }
        }

        // TierChanged event testing
        private string _modeSwitchToolPath;
        private bool _testTierChangedEvent;
        private int _tierChangedCount;
        private int _modeSwitchWaitTime = 5000;
        #endregion

        private int _expectedHardwareTier;
    }
}