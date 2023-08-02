!IF 0
------------------------------------------------------------------------------
  Accessible Technology Group 
  Copyright (c) Microsoft Corporation, 2001
------------------------------------------------------------------------------
!ENDIF

#need this somewhere
CLS_COMPLIANT=0

################################################################################
# Paths : Defined for buckets of tests/tools.  Usually in these directores is 
#         located an inc file that defines the specifics of the projects
################################################################################

AT_PATH_LOGGING=$(WUI_ACCESSIBLETECH_TST_PATH)\Logging
AT_PATH_COMMON=$(WUI_ACCESSIBLETECH_TST_PATH)
AT_PATH_BINPLACE=FeatureTests\UIAutomation
AT_PATH_UIV_COMMON=$(WUI_ACCESSIBLETECH_TST_PATH)\ATGCommon
AT_PATH_UIV_WUIATESTLIBRARY=$(WUI_ACCESSIBLETECH_TST_PATH)\WUIATESTLIBRARY

################################################################################
# ATG Metadata's
################################################################################

# TODO: Note AT_OS_FLAVOR was "winfx" vs. "vista".  We're using winfx only.
#AT_WUIATESTLIBRARY_METADATA=$(WUI_ACCESSIBLETECH_TST_OBJ_PATH)\uiverify\wuiatestlibrary\$(AT_OS_FLAVOR)\$O\wuiatestlibrary$(AT_INTERNAL_FLAVOR).metadata_dll
AT_WUIATESTLIBRARY_METADATA=$(WUI_ACCESSIBLETECH_TST_PATH)\wuiatestlibrary\WinFx\$(O)\WUIATestLibrary.dll

################################################################################
# Logging
################################################################################

AT_LOGGER_REL_PATH=\Logging
AT_LOGGER_PATH=$(WUI_ACCESSIBLETECH_TST_PATH)\Logging
AT_LOGGER_METADATA=$(WUI_ACCESSIBLETECH_TST_PATH)\logging\wuialogging\$(O)\WUIALogging.dll

################################################################################
# Misc
################################################################################

# todo: I didn't copy some of this code over.  Doesn't appear we need it.
#AT_INPUT_NO_AUTOMATION=$(AT_PATH_COMMON)\Common\Input\InputNoAutomation.cs
#AT_PATTERNBASE_REQUIREMENTS=$(AT_PATH_COMMON)\AE_PatternCommon\PatternBase.cs \

AT_INPUT=$(AT_PATH_COMMON)\Input\Input.cs
AT_WIN32_IMPORTDLL=$(AT_PATH_COMMON)\Input\NativeMethods.cs $(AT_PATH_COMMON)\Input\SafeNativeMethods.cs $(AT_PATH_COMMON)\Input\UnSafeNativeMethods.cs
AT_KEY=$(AT_PATH_COMMON)\Input\Key.cs $(AT_PATH_COMMON)\Input\KeyInterop.cs 

################################################################################
# UIVerify
################################################################################

AT_UIV_SOURCES=$(AT_INPUT) $(AT_WIN32_IMPORTDLL) 
AT_UIV_GENERICDRIVER_SOURCES=$(AT_PATH_UIV_COMMON)\Attributes.cs $(AT_PATH_UIV_COMMON)\framework.cs $(AT_PATH_UIV_COMMON)\WrapperControls.cs $(AT_PATH_UIV_COMMON)\WrapperApplication.cs $(AT_WIN32_IMPORTDLL) 

################################################################################
# Metadata's
################################################################################

!if "$(URTTARGET)" == ""
URTTARGET=$(SDK_REF_PATH)
!endif
AT_ACCESSIBILITY_METADATA=$(URTTARGET)\Accessibility.dll
AT_SYSTEM_METADATA=$(URTTARGET)\System.Data.dll;$(URTTARGET)\System.dll;$(URTTARGET)\System.xml.dll;$(URTTARGET)\System.Windows.Forms.dll;$(URTTARGET)\System.Drawing.dll;$(AT_ACCESSIBILITY_METADATA)
MITA_REFERENCES=$(SDKTOOLS_REF_PATH)\Mita.Foundation.dll;$(SDKTOOLS_REF_PATH)\Mita.Modeling.dll;$(SDKTOOLS_REF_PATH)\Mita.localization.dll;$(SDKTOOLS_REF_PATH)\Mita.resourceextractionprovider.dll
MAUI_REFERENCES=$(SDKTOOLS_REF_PATH)\Maui\v1.1\Maui.Core.dll;$(SDKTOOLS_REF_PATH)\Maui\v1.1\resourcemanagerbasic.dll
NARRATOR_REFERENCES=$(WINDOWS_REF_PATH)\ScreenReaderHelper.dll

################################################################################
# ATG References
################################################################################

AT_BASIC_REFERENCE=$(AT_UIA_METADATA);$(AT_LOGGER_METADATA);$(AT_SYSTEM_METADATA)
AT_BVT_REFERENCES=$(AT_UIA_METADATA);$(AT_LOGGER_METADATA);$(AT_AE_PATTERNCOMMON_METADATA);$(AT_ACCESSIBILITY_METADATA)
AT_UIV_REFERENCES=$(AT_BASIC_REFERENCE);
AT_UIV_GENERICDRIVER_REFERENCES=$(AT_BASIC_REFERENCE);$(AT_WUIATESTLIBRARY_METADATA)

