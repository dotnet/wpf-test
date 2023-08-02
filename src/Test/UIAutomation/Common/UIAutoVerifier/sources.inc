!IF 0
------------------------------------------------------------------------------
  Accessible Technology Group 
  Copyright (c) Microsoft Corporation, 2001
------------------------------------------------------------------------------
!ENDIF

TARGETNAME=UIAutoVerifier

!include $(WUI_ACCESSIBLETECH_TST_PATH)\AccessibleTechBuildConsole.inc

CSHARP_SOURCES= \
	UIAutoVerifier.cs \
	$(AT_UIV_GENERICDRIVER_SOURCES)

CLR_REFERENCES=$(AT_UIV_GENERICDRIVER_REFERENCES)