// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.CrossProcess;

namespace Avalon.Test.CoreUI.Commanding
{
    /******************************************************************************
    * CLASS:          CommandingApp
    ******************************************************************************/
    [Test(1, "Commanding", TestCaseSecurityLevel.FullTrust, "Var", SupportFiles=@"FeatureTests\ElementServices\CoreCommanding_*.xaml,FeatureTests\ElementServices\Controller*.*")]
    public class CommandingApp : AvalonTest
    {
        #region Private Data
        private string              _testName        = "";
        private string              _applicationType = "";
        private string              _testHost        = "";
        private bool _multiVariations = false;
        #endregion


        #region Constructor

        //NOTE: there are four mutually exclusive groups of Commanding tests: Pri0, Pri1, WindowsOnly, and BrowserOnly,
        //that are placed with different combinations of ApplicationType and HostType.

        //**********************************************************************************************************
        //SET 1:  ApplicationType(WpfApplication/ClrExe/WinFormsApplication) x HostType(HwndSource/NavigationWindow/WindowsFormSource)
        //        (Pri 0 group)
        //**********************************************************************************************************
        //Pri 0 group:
        [Variation("CommandManagerRegisterClassCommandBindingsApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("CommandManagerRegisterClassCommandBindingsApp_ClrExe_HwndSource",Priority=0)]
        [Variation("CommandManagerRegisterClassCommandBindingsApp_ClrExe_Window",Priority=0)]
        [Variation("CommandManagerRegisterClassCommandBindingsApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("CommandManagerRegisterClassCommandBindingsApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("CommandManagerRegisterClassCommandBindingsApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("CommandManagerRegisterClassCommandBindingsApp_WinFormsApplication_Window",Priority=0)]
        [Variation("CommandManagerRegisterClassCommandBindingsApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("CommandManagerRegisterClassCommandBindingsApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("CommandManagerRegisterClassCommandBindingsApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("CommandManagerRegisterClassCommandBindingsApp_WpfApplication_Window",Priority=0)]
        [Variation("CommandManagerRegisterClassCommandBindingsApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("CommandManagerRegisterClassInputBindingsApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("CommandManagerRegisterClassInputBindingsApp_ClrExe_HwndSource",Priority=0)]
        [Variation("CommandManagerRegisterClassInputBindingsApp_ClrExe_Window",Priority=0)]
        [Variation("CommandManagerRegisterClassInputBindingsApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("CommandManagerRegisterClassInputBindingsApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("CommandManagerRegisterClassInputBindingsApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("CommandManagerRegisterClassInputBindingsApp_WinFormsApplication_Window",Priority=0)]
        [Variation("CommandManagerRegisterClassInputBindingsApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("CommandManagerRegisterClassInputBindingsApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("CommandManagerRegisterClassInputBindingsApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("CommandManagerRegisterClassInputBindingsApp_WpfApplication_Window",Priority=0)]
        [Variation("CommandManagerRegisterClassInputBindingsApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("UIElementRaiseCanExecuteApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("UIElementRaiseCanExecuteApp_ClrExe_HwndSource",Priority=0)]
        [Variation("UIElementRaiseCanExecuteApp_ClrExe_Window",Priority=0)]
        [Variation("UIElementRaiseCanExecuteApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("UIElementRaiseCanExecuteApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("UIElementRaiseCanExecuteApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("UIElementRaiseCanExecuteApp_WinFormsApplication_Window",Priority=0)]
        [Variation("UIElementRaiseCanExecuteApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("UIElementRaiseCanExecuteApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("UIElementRaiseCanExecuteApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("UIElementRaiseCanExecuteApp_WpfApplication_Window",Priority=0)]
        [Variation("UIElementRaiseCanExecuteApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("UIElementRaiseCommandApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("UIElementRaiseCommandApp_ClrExe_HwndSource",Priority=0)]
        [Variation("UIElementRaiseCommandApp_ClrExe_Window",Priority=0)]
        [Variation("UIElementRaiseCommandApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("UIElementRaiseCommandApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("UIElementRaiseCommandApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("UIElementRaiseCommandApp_WinFormsApplication_Window",Priority=0)]
        [Variation("UIElementRaiseCommandApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("UIElementRaiseCommandApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("UIElementRaiseCommandApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("UIElementRaiseCommandApp_WpfApplication_Window",Priority=0)]
        [Variation("UIElementRaiseCommandApp_WpfApplication_WindowsFormSource",Priority=0)]

        //**********************************************************************************************************
        //SET 2:  ApplicationType=WpfApplication / HostType=Window (Pri 1 group and WindowsOnly group)
        //**********************************************************************************************************
        //Pri 1 group:
        //These commented out tests were not successfully ported from v1.
        //ContentElementCommandBindingKeyBindingApp
        //CommandBindingInvokeKeyDownApp
        //CommandLibraryExecuteApp
        //CommandLibraryNavigationCommandsExecuteApp
        //ExecutedRoutedEventArgsNullApp
        //ExecuteInvalidInputElementApp
        //CommandManagerRegisterClassCommandBindingsGlobalApp
        //QueryEnabledInvalidInputElementApp
        //MouseBindingRaiseEventContentElementApp
        //MouseBindingRaiseEventApp
        //CanExecuteRoutedEventArgsNullApp
        [Variation("UIElementPreviewCanExecuteApp_WpfApplication_Window")]
        [Variation("ContentElementRaiseCommandApp_WpfApplication_Window")]
        [Variation("CommandConverterConvertFromApp_WpfApplication_Window")]
        [Variation("CommandConverterConvertToApp_WpfApplication_Window")]
        [Variation("CommandBindingEnabledApp_WpfApplication_Window")]
        [Variation("CommandBindingKeyBindingApp_WpfApplication_Window")]
        [Variation("CommandSerializeApp_WpfApplication_Window")]
        [Variation("CommandBindingCollectionClearApp_WpfApplication_Window")]
        [Variation("CommandBindingCollectionContainsApp_WpfApplication_Window")]
        [Variation("CommandBindingCollectionCopyToApp_WpfApplication_Window")]
        [Variation("CommandBindingCollectionCountApp_WpfApplication_Window")]
        [Variation("InputGesturesCollectionCountFromDefaultsApp_WpfApplication_Window")]
        [Variation("CommandBindingCollectionIListApp_WpfApplication_Window")]
        [Variation("CommandBindingCollectionRemoveApp_WpfApplication_Window")]
        [Variation("CommandBindingMouseBindingApp_WpfApplication_Window")]
        [Variation("CommandBindingSamplePenBindingApp_WpfApplication_Window")]
        [Variation("CommandBindingSealApp_WpfApplication_Window")]
        [Variation("CommandBindingSerializeApp_WpfApplication_Window")]
        [Variation("CommandLibraryCommandConverterConvertToApp_WpfApplication_Window")]
        [Variation("CommandLibraryCommandConverterConvertFromApp_WpfApplication_Window")]
        [Variation("CommandManagerRegisterClassCommandBindingsContentElementApp_WpfApplication_Window")]
        [Variation("CommandManagerRegisterClassInputBindingsContentElementApp_WpfApplication_Window")]
        [Variation("CommandManagerSuggestRequeryApp_WpfApplication_Window")]
        [Variation("CommandEmptyStringApp_WpfApplication_Window")]
        [Variation("CommandNullStringApp_WpfApplication_Window")]
        [Variation("CommandNullDeclaringTypeApp_WpfApplication_Window")]
        [Variation("CommandSourceKeyBindingApp_WpfApplication_Window")]
        [Variation("CommandSourceMouseBindingApp_WpfApplication_Window")]
        [Variation("InputBindingCollectionApp_WpfApplication_Window")]
        [Variation("InputBindingSerializeApp_WpfApplication_Window")]
        [Variation("InputGestureCollectionApp_WpfApplication_Window")]
        [Variation("KeyGestureConverterCanConvertFromApp_WpfApplication_Window")]
        [Variation("KeyGestureConverterCanConvertToApp_WpfApplication_Window")]
        [Variation("KeyGestureConverterConvertFromApp_WpfApplication_Window")]
        [Variation("KeyGestureConverterConvertToApp_WpfApplication_Window")]
        [Variation("KeyGestureOverrideChildApp_WpfApplication_Window")]
        [Variation("KeyGestureOverrideChildDefaultGesturesApp_WpfApplication_Window")]
        [Variation("KeyGesturePropertiesApp_WpfApplication_Window")]
        [Variation("MouseActionConverterCanConvertFromApp_WpfApplication_Window")]
        [Variation("MouseActionConverterCanConvertToApp_WpfApplication_Window")]
        [Variation("MouseGestureConverterCanConvertFromApp_WpfApplication_Window")]
        [Variation("MouseGestureConverterCanConvertToApp_WpfApplication_Window")]
        [Variation("MouseGestureOverrideChildDefaultGesturesApp_WpfApplication_Window")]
        [Variation("MouseGesturePropertiesApp_WpfApplication_Window")]
        [Variation("UICommandNullCommandTextApp_WpfApplication_Window")]
        [Variation("InputBindingPartialTrustApp_WpfApplication_Window")]
        [Variation("SecureCommandExecutePartialTrustApp_WpfApplication_Window")]
        [Variation("SecureCommandPartialTrustApp_WpfApplication_Window")]
        [Variation("SecureCommandBindingPartialTrustApp_WpfApplication_Window")]
        [Variation("SecureCommandBindingExecutePartialTrustApp_WpfApplication_Window")]
        [Variation("UICommandExecutePartialTrustApp_WpfApplication_Window")]
        [Variation("UICommandPartialTrustApp_WpfApplication_Window")]
        [Variation("UICommandBindingPartialTrustApp_WpfApplication_Window")]
        [Variation("UICommandBindingExecutePartialTrustApp_WpfApplication_Window")]
        [Variation("CommandConverterConvertFromPrefixApp_WpfApplication_Window")]
        [Variation("CommandBindingCollectionAddRangeApp_WpfApplication_Window")]
        [Variation("CommandBindingCollectionAddNonBindingApp_WpfApplication_Window")]
        [Variation("CommandBindingCollectionFromIListApp_WpfApplication_Window")]
        [Variation("CommandBindingCollectionRemoveFromArrayApp_WpfApplication_Window")]
        [Variation("CommandBindingCollectionRemoveNonBindingApp_WpfApplication_Window")]
        [Variation("CommandBindingCollectionSyncApp_WpfApplication_Window")]
        [Variation("CommandBindingDynamicCommandSerializeApp_WpfApplication_Window")]
        [Variation("CommandBindingFromDefaultsApp_WpfApplication_Window")]
        [Variation("CommandSerializeInstanceCommandApp_WpfApplication_Window")]
        [Variation("CommandSourceButtonBaseApp_WpfApplication_Window")]
        [Variation("CommandSourceHyperlinkApp_WpfApplication_Window")]
        [Variation("CommandSourceMenuItemApp_WpfApplication_Window")]
        [Variation("FrameworkContentElementCommandBindingOnDocumentViewerApp_WpfApplication_Window")]
        [Variation("FrameworkElementCommandBindingOnFrameApp_WpfApplication_Window")]
        [Variation("KeyGestureConverterConvertFromCombination2KeyApp_WpfApplication_Window")]
        [Variation("KeyGestureConverterConvertFromCombination3KeyApp_WpfApplication_Window")]
        [Variation("KeyGestureConverterConvertFromInvalidKeyGestureApp_WpfApplication_Window")]
        [Variation("KeyGestureConverterConvertToInvalidDestinationTypeApp_WpfApplication_Window")]
        [Variation("KeyGestureAlphanumericKeyApp_WpfApplication_Window")]
        //Windows only group:
        [Variation("CommandLibraryPropertiesNoExecuteHandlerApp_WpfApplication_Window")]
        [Variation("CommandLibraryPropertiesApp_WpfApplication_Window")]
        [Variation("CommandLibraryNavigationWindowPropertiesApp_WpfApplication_Window")]
        [Variation("CommandLibraryNavigationWindowPropertiesNoExecuteHandlerApp_WpfApplication_Window")]
        [Variation("MouseGestureOverrideChildApp_WpfApplication_Window")]
        [Variation("CommandLibraryCommandValueSerializerConvertFromApp_WpfApplication_Window")]
        [Variation("CommandLibraryCommandValueSerializerConvertToApp_WpfApplication_Window")]
        [Variation("CommandMultipleKeyBindingSpecialKeyApp_WpfApplication_Window")]
        [Variation("CommandMultipleKeyBindingAppsKeyApp_WpfApplication_Window")]
        [Variation("CommandBindingEnabledAfterRemoveApp_WpfApplication_Window")]
        [Variation("CommandBindingKeyBindingAppsKeyApp_WpfApplication_Window")]
        [Variation("CommandBindingSameKeyBindingApp_WpfApplication_Window")]
        [Variation("CommandDefaultsKeyBindingSpecialKeyApp_WpfApplication_Window")]
        [Variation("CommandBindingMouseBindingMiddleButtonApp_WpfApplication_Window")]
        [Variation("CommandManagerRegisterClassInputBindingTypeParentApp_WpfApplication_Window")]
        [Variation("MouseBindingFromDefaultsApp_WpfApplication_Window")]
        [Variation("MouseBindingFromDefaultsContentElementApp_WpfApplication_Window")]
        [Variation("CommandManagerRegisterClassInputBindingsClassCommandBindingApp_WpfApplication_Window")]
        [Variation("UIElementRaiseQueryEnabledClassBindingApp_WpfApplication_Window")]

        //**********************************************************************************************************
        //SET 3:  ApplicationType=WpfApplication / HostType=WindowsFormSource  (Pri1 Group)
        //**********************************************************************************************************
        //Pri 1 group:
        [Variation("UIElementPreviewCanExecuteApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementRaiseCommandApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandConverterConvertFromApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandConverterConvertToApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingEnabledApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingKeyBindingApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandSerializeApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingCollectionClearApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingCollectionContainsApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingCollectionCopyToApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingCollectionCountApp_WpfApplication_WindowsFormSource")]
        [Variation("InputGesturesCollectionCountFromDefaultsApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingCollectionIListApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingCollectionRemoveApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingMouseBindingApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingSamplePenBindingApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingSealApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingSerializeApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandLibraryCommandConverterConvertToApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandLibraryCommandConverterConvertFromApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandManagerRegisterClassCommandBindingsContentElementApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandManagerRegisterClassInputBindingsContentElementApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandManagerSuggestRequeryApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandEmptyStringApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandNullStringApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandNullDeclaringTypeApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandSourceKeyBindingApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandSourceMouseBindingApp_WpfApplication_WindowsFormSource")]
        [Variation("InputBindingCollectionApp_WpfApplication_WindowsFormSource")]
        [Variation("InputBindingSerializeApp_WpfApplication_WindowsFormSource")]
        [Variation("InputGestureCollectionApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyGestureConverterCanConvertFromApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyGestureConverterCanConvertToApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyGestureConverterConvertFromApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyGestureConverterConvertToApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyGestureOverrideChildApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyGestureOverrideChildDefaultGesturesApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyGesturePropertiesApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseActionConverterCanConvertFromApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseActionConverterCanConvertToApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseGestureConverterCanConvertFromApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseGestureConverterCanConvertToApp_WpfApplication_WindowsFormSource")]
        // [Variation("MouseGestureOverrideChildDefaultGesturesApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseGesturePropertiesApp_WpfApplication_WindowsFormSource")]
        [Variation("UICommandNullCommandTextApp_WpfApplication_WindowsFormSource")]
        [Variation("InputBindingPartialTrustApp_WpfApplication_WindowsFormSource")]
        [Variation("SecureCommandExecutePartialTrustApp_WpfApplication_WindowsFormSource")]
        [Variation("SecureCommandPartialTrustApp_WpfApplication_WindowsFormSource")]
        [Variation("SecureCommandBindingPartialTrustApp_WpfApplication_WindowsFormSource")]
        [Variation("SecureCommandBindingExecutePartialTrustApp_WpfApplication_WindowsFormSource")]
        [Variation("UICommandExecutePartialTrustApp_WpfApplication_WindowsFormSource")]
        [Variation("UICommandPartialTrustApp_WpfApplication_WindowsFormSource")]
        [Variation("UICommandBindingPartialTrustApp_WpfApplication_WindowsFormSource")]
        [Variation("UICommandBindingExecutePartialTrustApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandConverterConvertFromPrefixApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingCollectionAddRangeApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingCollectionAddNonBindingApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingCollectionFromIListApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingCollectionRemoveFromArrayApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingCollectionRemoveNonBindingApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingCollectionSyncApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingDynamicCommandSerializeApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandBindingFromDefaultsApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandSerializeInstanceCommandApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandSourceButtonBaseApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandSourceHyperlinkApp_WpfApplication_WindowsFormSource")]
        [Variation("CommandSourceMenuItemApp_WpfApplication_WindowsFormSource")]
        [Variation("FrameworkContentElementCommandBindingOnDocumentViewerApp_WpfApplication_WindowsFormSource")]
        [Variation("FrameworkElementCommandBindingOnFrameApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyGestureConverterConvertFromCombination2KeyApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyGestureConverterConvertFromCombination3KeyApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyGestureConverterConvertFromInvalidKeyGestureApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyGestureConverterConvertToInvalidDestinationTypeApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyGestureAlphanumericKeyApp_WpfApplication_WindowsFormSource")]

        /******************************************************************************
        * Function:          CommandingApp Constructor
        ******************************************************************************/
        public CommandingApp(string arg)
        {
            char[] delimiters = new char[] { '_' };
            String[] argArray = arg.Split(delimiters);
            if (argArray.Length != 3)
            {
                throw new Microsoft.Test.TestSetupException("Three parameters delimited by underscores must be specified.");
            }

            _testName = argArray[0];
            _applicationType = argArray[1];
            _testHost = argArray[2];

            //This is a workaround to match the variations between old and new infra for these tests.
            if (_testName == "CommandLibraryCommandConverterConvertToApp" || _testName == "CommandLibraryCommandValueSerializerConvertToApp")
            {
                _multiVariations = true;
            }                            
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            DictionaryStore.StartServer();
            GlobalLog.LogStatus("In CommandingApp.RunTest...");

            GlobalLog.LogStatus("*******************************************");
            GlobalLog.LogStatus("ApplicationType: " + _applicationType);
            GlobalLog.LogStatus("Host:            " + _testHost);
            GlobalLog.LogStatus("Running:         " + _testName);
            GlobalLog.LogStatus("*******************************************");

            ApplicationType appType = (ApplicationType)Enum.Parse(typeof(ApplicationType), _applicationType);

            CommonStorage.CleanAll();
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), _testHost);

            if (_multiVariations)
            {
                Variation.Current.LogResult(Result.Pass);
                Microsoft.Test.Logging.Log.Current.CurrentVariation.Close();
            }
            ContainerVariationItem cvi = new ContainerVariationItem();
            cvi.Execute(appType, hostType, "Commanding", _testName);
            //A test failure will be handled by an Exception thrown during Verification.
            if (_multiVariations)
            {
                Microsoft.Test.Logging.Log.Current.CreateVariation("EndTest");
            }
            return TestResult.Pass;
        }
        #endregion
    }
}
