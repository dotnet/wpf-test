Option Explicit

Function CreateProjectHeaderText(strAssembly, strGuid, strOutputType)
  Dim strResult

  strResult = "<?xml version='1.0' encoding='utf-8'?>" & vbNewLine
  strResult = strResult & "<Project DefaultTargets='Build' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>" & vbNewLine
  strResult = strResult & "  <PropertyGroup>" & vbNewLine
  strResult = strResult & "    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>" & vbNewLine
  strResult = strResult & "    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>" & vbNewLine
  strResult = strResult & "    <ProductVersion>8.0.40903</ProductVersion>" & vbNewLine
  strResult = strResult & "    <SchemaVersion>2.0</SchemaVersion>" & vbNewLine
  strResult = strResult & "    <OutputType>" & strOutputType & "</OutputType>" & vbNewLine
  strResult = strResult & "    <RootNamespace>" & strAssembly & "</RootNamespace>" & vbNewLine
  strResult = strResult & "    <AssemblyName>" & strAssembly & "</AssemblyName>" & vbNewLine
  strResult = strResult & "    <WarningLevel>4</WarningLevel>" & vbNewLine
  strResult = strResult & "    <StartupObject>" & vbNewLine
  strResult = strResult & "    </StartupObject>" & vbNewLine
  strResult = strResult & "  </PropertyGroup>" & vbNewLine
  strResult = strResult & "  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">" & vbNewLine
  strResult = strResult & "    <DebugSymbols>true</DebugSymbols>" & vbNewLine
  strResult = strResult & "    <DebugType>full</DebugType>" & vbNewLine
  strResult = strResult & "    <Optimize>false</Optimize>" & vbNewLine
  strResult = strResult & "    <OutputPath>.\</OutputPath>" & vbNewLine
  strResult = strResult & "    <DefineConstants>DEBUG;TRACE</DefineConstants>" & vbNewLine
  strResult = strResult & "    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>" & vbNewLine
  strResult = strResult & "  </PropertyGroup>" & vbNewLine
  strResult = strResult & "  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">" & vbNewLine
  strResult = strResult & "    <DebugSymbols>false</DebugSymbols>" & vbNewLine
  strResult = strResult & "    <Optimize>true</Optimize>" & vbNewLine
  strResult = strResult & "    <OutputPath>.\</OutputPath>" & vbNewLine
  strResult = strResult & "    <DefineConstants>TRACE</DefineConstants>" & vbNewLine
  strResult = strResult & "    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>" & vbNewLine
  strResult = strResult & "  </PropertyGroup>" & vbNewLine
  strResult = strResult & "  <ItemGroup>" & vbNewLine
  strResult = strResult & "    <AppDesigner Include='Properties\' />" & vbNewLine
  strResult = strResult & "  </ItemGroup>" & vbNewLine
  
  CreateProjectHeaderText = strResult
End Function

Function CreateProjectFooterText()
  Dim strResult
  
  strResult = "  <Import Project='$(MSBuildBinPath)\Microsoft.CSHARP.Targets' />" & vbNewLine
  strResult = strResult & "</Project>  " & vbNewLine
  
  CreateProjectFooterText = strResult
End Function

Function CreateExeProjectText()
  Dim strResult

  strResult = CreateProjectHeaderText("EditingTest", "{4E508082-7BB1-4040-B5F3-DACBF71593FF}", "Exe")

  ' Add extra library references.
  strResult = strResult & "  <ItemGroup>" & vbNewLine
  strResult = strResult & CreateStandardLibrariesText()
  strResult = strResult & "    <!-- Test support references. -->" & vbNewLine
  strResult = strResult & "    <Reference Include='TestRuntime'>" & vbNewLine
  strResult = strResult & "      <HintPath>.\TestRuntime.dll</HintPath>" & vbNewLine
  strResult = strResult & "      <SpecificVersion>False</SpecificVersion>" & vbNewLine
  strResult = strResult & "    </Reference>" & vbNewLine
  strResult = strResult & "    <Reference Include='EditingTestLib'>" & vbNewLine
  strResult = strResult & "      <HintPath>.\EditingTestLib.dll</HintPath>" & vbNewLine
  strResult = strResult & "      <SpecificVersion>False</SpecificVersion>" & vbNewLine
  strResult = strResult & "    </Reference>" & vbNewLine
  strResult = strResult & "  </ItemGroup>" & vbNewLine

  strResult = strResult & "  <!-- Entry point. -->" & vbNewLine
  strResult = strResult & "  <ItemGroup>" & vbNewLine
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\ExeTarget\EntryPoint.cs")
  strResult = strResult & "  </ItemGroup>" & vbNewLine
  
  strResult = strResult & "  <!-- Source code. -->" & vbNewLine
  strResult = strResult & "  <ItemGroup>" & vbNewLine
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\ActionDriven\ActionDrivenTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\AssemblyData\AssemblyTestCaseData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\AssemblyData\GroupedCustomTestcaseData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\ContextMenu\ContextMenuTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Editing\Tablet.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Editing\TextEditorTests.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Editing\UndoRedoTests.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\PartialTrust\ClipboardAcessInPartialTrust.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Input\KeyEvents.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Input\MouseEvents.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Interactive\TextExplorer.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Interactive\TBLineAPITestHelper.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\KeyNavigation\KeyNavigationTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\KeyNavigation\PageUpDownNavigation.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\KeyNavigation\TypographyNavigation.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\MouseEditing\MouseEditingTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RichText\AdvancedTypographyFeatures.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RichText\CharacterFormattingHelper.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RichText\CharacterFormattingTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RichText\FigureAndFloaterTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RichText\HyperlinkTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RichText\KeyboardRegressionTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RichText\RichEditingBase.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RichText\ParagraphCreateAndSplitingBVT.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RichText\ParagraphDeletingAndMergingBVT.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RichText\ParagraphEditingTestWithKeyboard.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RichText\ParagraphEditingWithMouse.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RichTextBoxOM\RichTextBoxProperties.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RichTextBoxOM\RTBFlowDirection.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Selection\CaretElement.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Selection\SelectionTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Speller\SpellerTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextBoxOM\Commands.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextBoxOM\ControlTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextBoxOM\DataBinding.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextBoxOM\Editing.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextBoxOM\Events.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextBoxOM\MaxMinLines.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextBoxOM\Navigation.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextBoxOM\Rendering.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextBoxOM\Scrolling.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextBoxOM\Serialization.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextBoxOM\Selection.cs") 
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\BackgroundLayout\BackgroundLayoutKeyboardInput.cs")  
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextBoxOM\Styling.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextBoxOM\Text.cs") 
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextBoxOM\Sequences.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextBoxOM\TextBoxTestCase.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextBoxOM\TextUIElementTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextOM\TextElementCollectionTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextOM\TextPositionTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextOM\TextRangeTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextRangeSerialization\DefaultPropertyValueSerialization.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextRangeSerialization\TextRangeSerializationTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\UndoRedo\CommonCase.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\UndoRedo\TextBoxBaseUndoApiTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\UndoRedo\UndoEmbededObjectTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\UndoRedo\UndoFormating.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\UndoRedo\UndoDelimiterTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\XamlRtf\XamlRtfTests.cs")

  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Clipboard\ClipboardTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Clipboard\CutCopyPastePlanText.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Clipboard\SetAvalonDataGetWin32DataInprocAPI.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Clipboard\ImageCopyPaste.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Clipboard\XamlPackageTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Clipboard\SetGetClipboardData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Clipboard\SetWin32DataGetAvalonDataInprocAPI.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Clipboard\ClipboardTestInCrossContainer.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Clipboard\ClipboardTestInSameContainer.cs")

  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\DataObject\DataFormatInvalidGetFormats.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\DataObject\DataFormatsGetFormatWithInt.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\DataObject\DataObjectInvalidAPIs.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\DataObject\DataObjectAPI.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\DataObject\DataObjectObjBitmapAPI.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\DataObject\DataObjectStrObjBitmapAPI.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\DataObject\DataObjectStrObjBoolBitmapAPI.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\DataObject\DataObjectTypeObjBitmapAPI.cs")

  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RegressionTest\BugRegressionTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RegressionTest\TextRegressionTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RegressionTest\XamlRegressionTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RegressionTest\LayoutRegressionTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RegressionTest\TextBoxRegressionTest.cs")

  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\DragDrop\DragDropAPITest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\DragDrop\DragDropUITest.cs")

  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\EditCommand\EditCommand.cs")

  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\Interactive\EditingExaminer.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\ListEditing\CaretNavigationInsideList.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\RichTextBoxOM\CreateTextPointerFromPoint.cs") 
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\ListEditing\ListCreatingTest.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TextOM\BlockUIContainerTest.cs")  
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\FeatureTests\TableEditing\TableEditingNegativeTest.cs")
  strResult = strResult & "  </ItemGroup>" & vbNewLine

  strResult = strResult & CreateProjectFooterText()
  
  CreateExeProjectText = strResult
End Function

Function CreateStandardLibrariesText()
  Dim strResult
  strResult =             "    <!-- Referenced libraries. -->" & vbNewLine
  strResult = strResult & "    <Reference Include='PresentationCore, Version=6.0.4030.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL'>" & vbNewLine
  strResult = strResult & "      <HintPath>.\PresentationCore.dll</HintPath>" & vbNewLine
  strResult = strResult & "      <SpecificVersion>False</SpecificVersion>" & vbNewLine
  strResult = strResult & "    </Reference>" & vbNewLine
  strResult = strResult & "    <Reference Include='PresentationFramework, Version=6.0.4030.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL'>" & vbNewLine
  strResult = strResult & "      <HintPath>.\PresentationFramework.dll</HintPath>" & vbNewLine
  strResult = strResult & "      <SpecificVersion>False</SpecificVersion>" & vbNewLine
  strResult = strResult & "    </Reference>" & vbNewLine
  strResult = strResult & "    <Reference Include='System' />" & vbNewLine
  strResult = strResult & "    <Reference Include='System.Drawing' />" & vbNewLine
  strResult = strResult & "    <Reference Include='System.Windows.Forms' />" & vbNewLine
  strResult = strResult & "    <Reference Include='System.XML' />" & vbNewLine
  strResult = strResult & "    <Reference Include='WindowsBase, Version=6.0.4030.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL'>" & vbNewLine
  strResult = strResult & "      <HintPath>.\WindowsBase.dll</HintPath>" & vbNewLine
  strResult = strResult & "      <SpecificVersion>False</SpecificVersion>" & vbNewLine
  strResult = strResult & "    </Reference>" & vbNewLine
  strResult = strResult & "    <Reference Include='UIAutomationClient'>" & vbNewLine
  strResult = strResult & "      <HintPath>.\UIAutomationClient.dll</HintPath>" & vbNewLine
  strResult = strResult & "      <SpecificVersion>False</SpecificVersion>" & vbNewLine
  strResult = strResult & "    </Reference>" & vbNewLine
  strResult = strResult & "    <Reference Include='UIAutomationTypes'>" & vbNewLine
  strResult = strResult & "      <HintPath>.\UIAutomationTypes.dll</HintPath>" & vbNewLine
  strResult = strResult & "      <SpecificVersion>False</SpecificVersion>" & vbNewLine
  strResult = strResult & "    </Reference>" & vbNewLine
  strResult = strResult & "    <Reference Include='UIAutomationProvider'>" & vbNewLine
  strResult = strResult & "      <HintPath>.\UIAutomationProvider.dll</HintPath>" & vbNewLine
  strResult = strResult & "      <SpecificVersion>False</SpecificVersion>" & vbNewLine
  strResult = strResult & "    </Reference>" & vbNewLine
  CreateStandardLibrariesText = strResult
End Function

Function CreateCompileText(strFileName)
  CreateCompileText = "    <Compile Include='" & strFileName & "'>" & vbNewLine & _
                      "      <Link>" & GetLinkName(strFileName) & "</Link>" & vbNewLine & _
                      "    </Compile>" & vbNewLine
End Function

Function CreateLibraryProjectText()
  Dim strResult

  strResult = CreateProjectHeaderText("EditingTestLib", "{4E508082-7BB1-4040-B5F3-DACBF71593FE}", "Library")

  strResult = strResult & "  <ItemGroup>" & vbNewLine
  strResult = strResult & CreateStandardLibrariesText()
  strResult = strResult & "    <!-- Test support references. -->" & vbNewLine

  strResult = strResult & "    <Reference Include='TestRuntime'>" & vbNewLine
  strResult = strResult & "      <HintPath>.\TestRuntime.dll</HintPath>" & vbNewLine
  strResult = strResult & "      <SpecificVersion>False</SpecificVersion>" & vbNewLine
  strResult = strResult & "    </Reference>" & vbNewLine
  strResult = strResult & "    <!-- MSBuild engine references. -->" & vbNewLine
  strResult = strResult & "    <Reference Include='Microsoft.Build.Engine'>" & vbNewLine
  strResult = strResult & "      <HintPath>%PUBLIC_ROOT%\sdk\ref\clr20\Microsoft.Build.Engine.metadata_dll</HintPath>" & vbNewLine
  strResult = strResult & "      <SpecificVersion>False</SpecificVersion>" & vbNewLine
  strResult = strResult & "    </Reference>" & vbNewLine
  strResult = strResult & "    <Reference Include='Microsoft.Build.Framework'>" & vbNewLine
  strResult = strResult & "      <HintPath>%PUBLIC_ROOT%\sdk\ref\clr20\Microsoft.Build.Framework.metadata_dll</HintPath>" & vbNewLine
  strResult = strResult & "      <SpecificVersion>False</SpecificVersion>" & vbNewLine
  strResult = strResult & "    </Reference>" & vbNewLine
  strResult = strResult & "    <Reference Include='Microsoft.Build.Utilities'>" & vbNewLine
  strResult = strResult & "      <HintPath>%PUBLIC_ROOT%\sdk\ref\clr20\Microsoft.Build.Utilities.metadata_dll</HintPath>" & vbNewLine
  strResult = strResult & "      <SpecificVersion>False</SpecificVersion>" & vbNewLine
  strResult = strResult & "    </Reference>" & vbNewLine
  strResult = strResult & "  </ItemGroup>" & vbNewLine

  strResult = strResult & "  <!-- Source code. -->" & vbNewLine
  strResult = strResult & "  <ItemGroup>" & vbNewLine
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Analysis\FailureAnalysisForm.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Data\AutomationData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Data\BrushData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Data\CustomControlData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Data\DataFormatsData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Data\TypographyFontData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Data\DataTransferExtensibilityData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Data\DocumentPositionData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Data\EditingCommandData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Data\InputLocaleData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Data\KeyboardEditingData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Data\RichTextContentData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Data\StringData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Data\TextEditableTypeData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Data\TextElementTypeData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Data\TextScriptData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Data\TextSelectionData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Data\DependencyPropertyData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\IO\LogStreamWorkItem.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\IO\StringStream.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\IO\TextFileUtils.cs")


  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\KoKoModel\Model.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\KoKoModel\ModelAction.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\KoKoModel\ModelEngine.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\KoKoModel\ModelEngineOptions.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\KoKoModel\ModelException.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\KoKoModel\ModelExpression.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\KoKoModel\ModelItem.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\KoKoModel\ModelItems.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\KoKoModel\ModelParameter.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\KoKoModel\ModelRange.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\KoKoModel\ModelRequirement.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\KoKoModel\ModelTrace.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\KoKoModel\ModelValue.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\KoKoModel\ModelVariable.cs")

  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Loggers\ClrProfilerLogger.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Loggers\CommandLogger.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Loggers\EventLogger.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Loggers\InputLogger.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Loggers\LoggerTraceListener.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Loggers\Loggers.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Loggers\TextTreeLogger.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Loggers\VisualLogger.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Management\Attributes.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Management\Coordinator.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Management\StringIdentifierSupport.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Management\TestFinder.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Management\TestRunner.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Management\VersionInformation.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Models\ArrayTextContainer.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Models\KeyboardEditingState.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Models\SelectedDocumentGenerator.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Models\TextUndoModel.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Models\TextLayoutModel.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\TestTypes\CombinatorialTestCase.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\TestTypes\ManagedCombinatorialTestCase.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\TestTypes\TestCaseData.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\TestTypes\TestTypes.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\TestTypes\CombinedCustomTestCase.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Threading\TestQueue.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\ActionManager.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\AutomationUtils.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\BuildUtils.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\CaretVerifier.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\Combinatorial.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\Configuration.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\CustomXPathNavigators.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\GlobalCachedObjects.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\Input.cs")  
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\InputGenerator.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\InputMonitor.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\InputMonitorManager.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\MathUtils.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\ProcessUtils.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\Queue.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\Reflection.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\Text.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\TextTreeTestHelper.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\Verifier.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\XamlUtils.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\TextOMUtils.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\XmlTestConfiguration.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Utils\Interop.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Wrappers\ActionItemWrapper.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Wrappers\ElementWrappers.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Wrappers\SecurityWrappers.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Wrappers\UniscribeWrappers.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Wrappers\WinFormsWrappers.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Wrappers\Wrappers.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\Stress\UisStress.cs")
  strResult = strResult & CreateCompileText("%sdxroot%\wpf\Test\Editing\Common\Library\TestCaseTemplates\ActionDrivenTest.cs")
  strResult = strResult & "  </ItemGroup>" & vbNewLine

  strResult = strResult & CreateProjectFooterText()
  
  CreateLibraryProjectText = strResult
End Function

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
' Gets the name used for the Link tag for a compilation file name.
Function GetLinkName(strFileName)
  Dim intPosition
  intPosition = InStrRev(strFileName, "\")
  
  If intPosition = -1 Then
    GetLinkName = strFileName
  Else
    ' Position is now on backslash before filename.
    intPosition = InStrRev(strFileName, "\", intPosition - 1)
    If intPosition = -1 Then
      GetLinkName = strFileName
    Else
      ' Position is now on backslash before file parent path.
      GetLinkName = Mid(strFileName, intPosition + 1)
    End If
  End If  
End Function

Function ReplaceEnvironmentVariables(strValue)
  strValue = Replace(strValue, "%sdxroot%", strSdxRoot)
  strValue = Replace(strValue, "%OBJECT_ROOT%", strObjectRoot)
  strValue = Replace(strValue, "%PUBLIC_ROOT%", strPublicRoot)
  strValue = Replace(strValue, "$BuildDir$", strBuildDir)
  
  ReplaceEnvironmentVariables = strValue
End Function

Sub ReadEnvironmentVariables()
  Dim objShell
  Dim arch

  Set objShell = WScript.CreateObject("WScript.Shell")
  strSdxRoot = objShell.ExpandEnvironmentStrings("%sdxroot%")
  strObjectRoot = objShell.ExpandEnvironmentStrings("%OBJECT_ROOT%")
  strPublicRoot = objShell.ExpandEnvironmentStrings("%PUBLIC_ROOT%")
  arch = objShell.ExpandEnvironmentStrings("%build.arch%")
  
  
  If Len(strSdxRoot) = 0 Then
    Err.Raise 100, "CreateProjectText", "sdxroot environment variable is not defined. " & vbNewLine _
      & vbNewLine & "Please run this script from a Razzle console."
  End If
  
  If Len(strObjectRoot) = 0 Then
    Err.Raise 101, "CreateProjectText", "OBJECT_ROOT environment variable is not defined. " & vbNewLine _
      & vbNewLine & "Please run this script from a Razzle console."
  End If
  
  If Len(strPublicRoot) = 0 Then
    Err.Raise 102, "CreateProjectText", "PUBLIC_ROOT environment variable is not defined. " & vbNewLine _
      & vbNewLine & "Please run this script from a Razzle console."
  End If
  
  If arch = "x86" Then
  	strBuildDir = objShell.ExpandEnvironmentStrings("obj%BUILD_ALT_DIR%\i386")
  Else
	strBuildDir = objShell.ExpandEnvironmentStrings("obj%BUILD_ALT_DIR%\%build.arch%")
  End If
 
End Sub

Sub WriteStringToFile(strText, strFileName)
  Dim fso         ' As FileSystemObject
  Dim txtFile     ' As TextStream

  Set fso = CreateObject("Scripting.FileSystemObject") ' New FileSystemObject
  Set txtFile = fso.CreateTextFile(strFileName)
  txtFile.Write strText
  txtFile.Close
End Sub

Dim strSdxRoot    ' %sdxroot%    
Dim strObjectRoot ' %OBJECT_ROOT%
Dim strPublicRoot ' %PUBLIC_ROOT%
Dim strBuildDir   ' objchk\i386 - like directory
Dim strText       ' As String
Dim strFileName   ' As String

ReadEnvironmentVariables

strFileName = ReplaceEnvironmentVariables("EditingTestLib.csproj")
strText = ReplaceEnvironmentVariables(CreateLibraryProjectText())
'WScript.Echo "Creating project file in " & strFileName
WriteStringToFile strText, strFileName

strFileName = ReplaceEnvironmentVariables("EditingTest.csproj")
strText = ReplaceEnvironmentVariables(CreateExeProjectText())
'WScript.Echo "Creating project file in " & strFileName
WriteStringToFile strText, strFileName
