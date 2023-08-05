!IF 0
------------------------------------------------------------------------------
  Accessible Technology Group 
  Copyright (c) Microsoft Corporation, 2001
------------------------------------------------------------------------------
!ENDIF

TARGETNAME=WUIATestLibrary

!include $(WUI_ACCESSIBLETECH_TST_PATH)\AccessibleTechBuildDll.inc

# I think the first line doesn't work, but the second one does
USER_CS_FLAGS=$(USER_CS_FLAGS) /res:..\resources\UIVerify.resources
RESOURCE_FILES=..\resources\UIVerify.resources


CSHARP_SOURCES=$(AT_PATH_UIV_WUIATESTLIBRARY)\AssemblyInfo.cs\
#########################################################################
# namespace Microsoft.Test.WindowsUIAutomation.Core
#########################################################################
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Core\Library.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Core\MenuWrapper.cs \
#########################################################################
# namespace WindowsUIAutomation
#########################################################################
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\TestRuns.cs \
#########################################################################
# namespace Microsoft.Test.WindowsUIAutomation.Interfaces
#########################################################################
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Interfaces\WUIATestLibraryInterfaces.cs \
#########################################################################
# namespace InternalHelper.Enumerations
#########################################################################
	$(AT_PATH_UIV_WUIATESTLIBRARY)\InternalHelper\Enumerations\Enumerations.cs \
#########################################################################
# namespace InternalHelper
#########################################################################
	$(AT_PATH_UIV_WUIATESTLIBRARY)\InternalHelper\Helpers.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\InternalHelper\TestManager.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\InternalHelper\TextLibrary.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\InternalHelper\TextLibraryCount.cs \
#########################################################################
# namespace InternalHelper.Logging
#########################################################################
	$(AT_PATH_UIV_WUIATESTLIBRARY)\InternalHelper\Logging\UIVerifyLogging.cs \
#########################################################################
# namespace InternalHelper.Tests
#########################################################################
	$(AT_PATH_UIV_WUIATESTLIBRARY)\InternalHelper\Tests\ControlObject.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\InternalHelper\Tests\EventHandlers.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\InternalHelper\Tests\ExceptionTypes.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\InternalHelper\Tests\PatternObject.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\InternalHelper\Tests\ScenarioObject.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\InternalHelper\Tests\TestObject.cs \
#########################################################################
#namespace Microsoft.Test.WindowsUIAutomation.Tests.Controls
#########################################################################
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\Button.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\Calendar.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\CheckBox.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\ComboBox.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\Custom.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\datagrid.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\dataitem.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\document.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\Edit.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\Group.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\header.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\headeritem.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\HyperLink.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\HyperLinkList.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\Image.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\IpAddress.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\List.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\ListItem.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\Menu.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\MenuBar.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\MenuItem.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\pane.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\ProgressBar.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\RadioButton.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\ScrollBar.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\seperator.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\Slider.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\Spinner.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\splitbutton.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\StatusBar.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\Tab.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\TabItem.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\table.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\Text.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\Thumb.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\titlebar.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\Toolbar.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\Tooltip.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\Tree.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\TreeItem.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Controls\window.cs \
#########################################################################
#namespace Microsoft.Test.WindowsUIAutomation.Tests.Patterns
#########################################################################
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\AutomationElementTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\DockTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\ExpandCollapseTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\GridItemTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\GridTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\InvokeTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\MultipleViewTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\RangeValueTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\ScrollItemTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\ScrollTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\SelectionItemTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\SelectionTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\TableItemTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\TableTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\TextTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\TextTestsHelper.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\ToggleTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\TransformsTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\ValueTests.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Patterns\WindowTests.cs \
#########################################################################
#namespace Microsoft.Test.WindowsUIAutomation.Tests.Scenarios
#########################################################################
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Scenarios\Shell.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Scenarios\Menu.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Scenarios\Msaa.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Scenarios\Narrator.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Scenarios\ScreenReader.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Scenarios\Stress.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Scenarios\StressDriver.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Scenarios\Text.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Scenarios\TextPatternAvalon.cs \
	$(AT_PATH_UIV_WUIATESTLIBRARY)\WindowsUIAutomation\Tests\Scenarios\TopLevelEvents.cs \
#########################################################################
# Shared code
#########################################################################
	$(WUI_ACCESSIBLETECH_TST_PATH)\Narrator\NarratorErrorDialogs.cs  \
#
# Win32 & Input
#
	$(WUI_ACCESSIBLE_TECH_INPUT) \
    	$(WUI_ACCESSIBLE_TECH_WIN32_IMPORTDLL) \
	$(AT_UIV_SOURCES)

CLR_REFERENCES=\
	$(AT_UIV_REFERENCES)