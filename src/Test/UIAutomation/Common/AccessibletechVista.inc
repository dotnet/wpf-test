##------------------------------------------------------------------------------
##  Microsoft UI Automation
##  Copyright (c) Microsoft Corporation, 2004
##
##  File:       AccessibleTechVista.inc
##------------------------------------------------------------------------------

AT_INTERNAL_FLAVOR=INTERNAL
AT_OS_FLAVOR=VISTA

################################################################################
# Metadata's
################################################################################

AT_SYSTEM_METADATA=$(URTTARGET)\System.Data.metadata_dll;$(URTTARGET)\System.metadata_dll;$(URTTARGET)\System.xml.metadata_dll;$(URTTARGET)\System.Windows.Forms.metadata_dll;$(URTTARGET)\System.Drawing.metadata_dll;$(URTTARGET)\Accessibility.metadata_dll

################################################################################
# UIA metadata
################################################################################

# AT_UIA_METADATA=$(WINDOWS_REF_PATH)\InternalTestUIAutomation.metadata_dll
# todo not there
AT_UIA_METADATA=$(WINDOWS_REF_PATH)\InternalTestUIAutomation.dll
!include $(WUI_ACCESSIBLETECH_TST_PATH)\AccessibleTechCommon.inc


