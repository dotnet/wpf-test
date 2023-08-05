##------------------------------------------------------------------------------
##  Microsoft UI Automation
##  Copyright (c) Microsoft Corporation, 2004
##
##  File:       common.inc
##------------------------------------------------------------------------------

################################################################################
# UIA Internal metadata
################################################################################
UIA_LONGHORN_METADATA=$(WINDOWS_REF_PATH)\InternalTestUIAutomation.metadata_dll

################################################################################
# UIVerify Base Location
################################################################################
WUI_ACCESSIBLETECH_TST_PATH=$(WPF_TEST_ROOT)\UIAutomation\Common

################################################################################
# LOGGER
################################################################################
WUI_LOGGER_ABSTRACT=$(WUI_ACCESSIBLETECH_TST_PATH)\Logging\WUIALogging\$(O)\WUIALogging.metadata_dll
WUI_LOGGER_PATH=$(WUI_ACCESSIBLETECH_TST_PATH)\Logging

################################################################################
# UI Automation 
################################################################################
WUI_METADATA=$(WINFX_30_REF_PATH)\WindowsBase.dll;$(WINFX_30_REF_PATH)\UIAutomationTypes.dll;$(WINFX_30_REF_PATH)\UIAutomationClient.dll;$(WINFX_30_REF_PATH)\UIAutomationProvider.dll;$(WINFX_30_REF_PATH)\PresentationCore.dll    
WUI_PROVIDER_METADATA=$(WINFX_30_REF_PATH)\WindowsBase.dll;$(WINFX_30_REF_PATH)\UIAutomationTypes.dll;$(WINFX_30_REF_PATH)\UIAutomationProvider.dll;$(WINFX_30_REF_PATH)\PresentationCore.dll
WUI_FULL_METADATA=$(WUI_METADATA)
WUI_CORE_METADATA=$(URTTARGET)\System.Data.dll;$(URTTARGET)\System.dll;$(URTTARGET)\System.xml.dll;$(URTTARGET)\System.Windows.Forms.dll;$(URTTARGET)\System.Drawing.dll;$(URTTARGET)\Accessibility.dll;$(URTTARGET)\PresentationCore.dll

################################################################################
# Misc
################################################################################

WUI_LOGGING_METADATA=$(TESTRUNTIME)
WUI_LOGGING_LH_METADATA=$(TESTRUNTIME)
