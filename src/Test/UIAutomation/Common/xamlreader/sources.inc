!IF 0
------------------------------------------------------------------------------
  Accessible Technology Group 
  Copyright (c) Microsoft Corporation, 2001
------------------------------------------------------------------------------
!ENDIF

TARGETNAME=XamlReader

!include $(WUI_ACCESSIBLETECH_TST_PATH)\AccessibleTechBuildProgram.inc

CSHARP_SOURCES= \
	XamlReader.cs \

CLR_REFERENCES= \
    $(SDK_REF_PATH)\System.dll; \
    $(SDK_REF_PATH)\System.Xml.dll; \
    $(SDK_REF_PATH)\System.Drawing.dll; \
    $(WINFX_30_REF_PATH)\WindowsBase.dll; \
    $(WINFX_30_REF_PATH)\PresentationCore.dll; \
    $(WINFX_30_REF_PATH)\PresentationFramework.dll; \

