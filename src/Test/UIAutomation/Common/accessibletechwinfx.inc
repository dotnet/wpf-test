##------------------------------------------------------------------------------
##  Microsoft UI Automation
##  Copyright (c) Microsoft Corporation, 2004
##
##  File:       AccessibleTechWinFx.inc
##------------------------------------------------------------------------------

AT_INTERNAL_FLAVOR=
AT_OS_FLAVOR=WINFX

################################################################################
# UIA metadata
################################################################################

AT_UIA_METADATA=$(WINFX_30_REF_PATH)\WindowsBase.dll;$(WINFX_30_REF_PATH)\UIAutomationTypes.dll;$(WINFX_30_REF_PATH)\UIAutomationClient.dll;$(WINFX_30_REF_PATH)\UIAutomationProvider.dll    

!include $(WUI_ACCESSIBLETECH_TST_PATH)\AccessibleTechCommon.inc
