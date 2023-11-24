using System;
using System.Windows.Forms;
using ReflectTools;
using WPFReflectTools;
using ReflectTools.AutoPME;
using WFCTestLib.Util;
using WFCTestLib.Log;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using System.Windows.Media;

//
// Testcase:    XWindowsFormsHost
// Description: System.Windows.Forms.Integration.WindowsFormsHost AutoPME Test
// Author:      a-rickyt
//
namespace WindowsFormsHostTests
{

public class XWindowsFormsHost : XHwndHost
{
    #region Testcase setup
    
    WindowsFormsHost windowsFormsHost;

    public XWindowsFormsHost(string[] args) : base(args) { }

    protected override Type Class
    {
        get { return typeof(WindowsFormsHost); }
    }

    protected override Object CreateObject(TParams p)
    {
        return new WindowsFormsHost();
    }

    protected override void InitTest(TParams p) 
    {
        ExcludedProperties.Add("Visibility");
        ExcludedProperties.Add("Name");

        NoAutoEvent = true;

        //ExcludedEvents.Add("ToolTipOpening");
        //ExcludedEvents.Add("ToolTipClosing");
        //ExcludedEvents.Add("ContextMenuOpening");
        //ExcludedEvents.Add("ContextMenuClosing");
        //ExcludedEvents.Add("PreviewQueryContinueDrag");
        //ExcludedEvents.Add("QueryContinueDrag");
        //ExcludedEvents.Add("PreviewGiveFeedback");
        //ExcludedEvents.Add("GiveFeedback");
        //ExcludedEvents.Add("PreviewDragEnter");
        //ExcludedEvents.Add("DragEnter");
        //ExcludedEvents.Add("PreviewDragOver");
        //ExcludedEvents.Add("DragOver");
        //ExcludedEvents.Add("PreviewDragLeave");
        //ExcludedEvents.Add("DragLeave");
        //ExcludedEvents.Add("PreviewDrop");
        //ExcludedEvents.Add("Drop");
        //ExcludedEvents.Add("FocusableChanged");
        //ExcludedEvents.Add("IsVisibleChanged");
        //ExcludedEvents.Add("IsHitTestVisibleChanged");
        //ExcludedEvents.Add("IsEnabledChanged");

        base.InitTest(p);
    }

    #endregion

    #region Scenarios
    
    protected ScenarioResult get_Child(TParams p)
    {
        SWF.Button wfButton = new SWF.Button();
        return set_Child(p, wfButton);
    }

    protected ScenarioResult set_Child(TParams p, SWF.Control value)
    {
        windowsFormsHost = GetWindowsFormsHost(p);
        windowsFormsHost.Child = value;
        return new ScenarioResult(value, windowsFormsHost.Child, "Child " +
            "should be " + value + ", but is " + windowsFormsHost.Child + " instead.", p.log);
    }

    protected ScenarioResult OnPropertyChanged(TParams p, String propertyName, Object value)
    {
        ScenarioResult sr = new ScenarioResult();

        windowsFormsHost = GetWindowsFormsHost(p);
        //windowsFormsHost.OnPropertyChanged(propertyName, value);
        
        return new ScenarioResult(true, "OnPropertyChanged", p.log);
    }

    protected ScenarioResult TabInto(TParams p, System.Windows.Input.TraversalRequest value)
    {
        windowsFormsHost = GetWindowsFormsHost(p);
        value = new System.Windows.Input.TraversalRequest(new System.Windows.Input.FocusNavigationDirection());
        return new ScenarioResult(!windowsFormsHost.TabInto(value), "TabInto " +
            "should be " + false + ", but is " + windowsFormsHost.TabInto(value) + 
            " instead.", p.log);
    }

    protected ScenarioResult get_Padding(TParams p)
    {
        return set_Padding(p, new Thickness(p.ru.GetDouble(), p.ru.GetDouble(), p.ru.GetDouble(), p.ru.GetDouble()));
    }

    protected ScenarioResult set_Padding(TParams p, Thickness value)
    {
        windowsFormsHost = GetWindowsFormsHost(p);
        windowsFormsHost.Padding = value;
        return new ScenarioResult(value, windowsFormsHost.Padding, "Padding " +
            "should be " + value + ", but is " + windowsFormsHost.Padding + " instead.", p.log);
    }

    protected ScenarioResult get_TabIndex(TParams p)
    {
        return set_TabIndex(p, p.ru.GetInt());
    }

    protected ScenarioResult set_TabIndex(TParams p, int value)
    {
        windowsFormsHost = GetWindowsFormsHost(p);
        windowsFormsHost.TabIndex = value;
        return new ScenarioResult(value, windowsFormsHost.TabIndex, "TabIndex " +
            "should be " + value + ", but is " + windowsFormsHost.TabIndex + " instead.", p.log);
    }

    protected ScenarioResult get_FontFamily(TParams p)
    {
        return set_FontFamily(p, new FontFamily("Arial"));
    }

    protected ScenarioResult set_FontFamily(TParams p, FontFamily value)
    {
        value = new FontFamily("Arial"); //need to give value a FontFamily object
        windowsFormsHost = GetWindowsFormsHost(p);
        windowsFormsHost.FontFamily = value;
        return new ScenarioResult(value, windowsFormsHost.FontFamily, "FontFamily " +
            "should be " + value + ", but is " + windowsFormsHost.FontFamily + " instead.", p.log);
    }

    protected ScenarioResult get_FontSize(TParams p)
    {
        return set_FontSize(p, p.ru.GetDouble());
    }

    protected ScenarioResult set_FontSize(TParams p, double value)
    {
        value = 10; //FontSize cannot be 0
        windowsFormsHost = GetWindowsFormsHost(p);
        windowsFormsHost.FontSize = value;
        return new ScenarioResult(value, windowsFormsHost.FontSize, "FontSize " +
            "should be " + value + ", but is " + windowsFormsHost.FontSize + " instead.", p.log);
    }

    protected ScenarioResult get_FontStyle(TParams p)
    {
        return set_FontStyle(p, new FontStyle());
    }

    protected ScenarioResult set_FontStyle(TParams p, FontStyle value)
    {
        windowsFormsHost = GetWindowsFormsHost(p);
        windowsFormsHost.FontStyle = value;
        return new ScenarioResult(value, windowsFormsHost.FontStyle, "FontStyle " +
            "should be " + value + ", but is " + windowsFormsHost.FontStyle + " instead.", p.log);
    }

    protected ScenarioResult get_FontWeight(TParams p)
    {
        return set_FontWeight(p, new FontWeight());
    }

    protected ScenarioResult set_FontWeight(TParams p, FontWeight value)
    {
        windowsFormsHost = GetWindowsFormsHost(p);
        windowsFormsHost.FontWeight = value;
        return new ScenarioResult(value, windowsFormsHost.FontWeight, "FontWeight " +
            "should be " + value + ", but is " + windowsFormsHost.FontWeight + " instead.", p.log);
    }

    protected ScenarioResult get_Foreground(TParams p)
    {
        return set_Foreground(p, Brushes.Red);
    }

    protected ScenarioResult set_Foreground(TParams p, Brush value)
    {
        windowsFormsHost = GetWindowsFormsHost(p);
        windowsFormsHost.Foreground = value;
        return new ScenarioResult(value, windowsFormsHost.Foreground, "Foreground " +
            "should be " + value + ", but is " + windowsFormsHost.Foreground + " instead.", p.log);
    }

    protected ScenarioResult get_Background(TParams p)
    {
        return set_Background(p, Brushes.Red);
    }

    protected ScenarioResult set_Background(TParams p, Brush value)
    {
        windowsFormsHost = GetWindowsFormsHost(p);
        windowsFormsHost.Background = value;
        return new ScenarioResult(value, windowsFormsHost.Background, "Background " +
            "should be " + value + ", but is " + windowsFormsHost.Background + " instead.", p.log);
    }

    protected ScenarioResult get_PropertyMap(TParams p)
    {
        string value = "System.Windows.Forms.Integration.WindowsFormsHostPropertyMap";
        return new ScenarioResult(value, windowsFormsHost.PropertyMap.ToString(), "PropertyMap " +
            "should be " + value + ", but is " + windowsFormsHost.PropertyMap.ToString() + " instead.", p.log);
    }

    #endregion

    #region Helper Functions 

    WindowsFormsHost GetWindowsFormsHost(TParams p)
    {
        if (p.target is WindowsFormsHost)
        {
            return (WindowsFormsHost)p.target;
        }
        else
        {
            p.log.WriteLine("object !instanceof WindowsFormsHost");
            return null;
        }
    }

    #endregion
}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ get_Child()
//@ set_Child(Control value)
//@ get_Padding()
//@ set_Padding(Thickness value)
//@ get_TabIndex()
//@ set_TabIndex(Int32 value)
//@ get_FontFamily()
//@ set_FontFamily(FontFamily value)
//@ get_FontSize()
//@ set_FontSize(Double value)
//@ get_FontStyle()
//@ set_FontStyle(FontStyle value)
//@ get_FontWeight()
//@ set_FontWeight(FontWeight value)
//@ get_Foreground()
//@ set_Foreground(Brush value)
//@ get_Background()
//@ set_Background(Brush value)
//@ get_PropertyMap()
//@ get_Handle()
//@ get_Parent()
//@ get_Style()
//@ set_Style(Style value)
//@ get_OverridesDefaultStyle()
//@ set_OverridesDefaultStyle(Boolean value)
//@ get_Triggers()
//@ get_TemplatedParent()
//@ get_Resources()
//@ set_Resources(ResourceDictionary value)
//@ get_DataContext()
//@ set_DataContext(Object value)
//@ get_Language()
//@ set_Language(XmlLanguage value)
//@ get_Name()
//@ set_Name(String value)
//@ get_Tag()
//@ set_Tag(Object value)
//@ get_InputScope()
//@ set_InputScope(InputScope value)
//@ get_ActualWidth()
//@ get_ActualHeight()
//@ get_LayoutTransform()
//@ set_LayoutTransform(Transform value)
//@ get_Width()
//@ set_Width(Double value)
//@ get_MinWidth()
//@ set_MinWidth(Double value)
//@ get_MaxWidth()
//@ set_MaxWidth(Double value)
//@ get_Height()
//@ set_Height(Double value)
//@ get_MinHeight()
//@ set_MinHeight(Double value)
//@ get_MaxHeight()
//@ set_MaxHeight(Double value)
//@ get_FlowDirection()
//@ set_FlowDirection(FlowDirection value)
//@ get_Margin()
//@ set_Margin(Thickness value)
//@ get_HorizontalAlignment()
//@ set_HorizontalAlignment(HorizontalAlignment value)
//@ get_VerticalAlignment()
//@ set_VerticalAlignment(VerticalAlignment value)
//@ get_FocusVisualStyle()
//@ set_FocusVisualStyle(Style value)
//@ get_Cursor()
//@ set_Cursor(Cursor value)
//@ get_ForceCursor()
//@ set_ForceCursor(Boolean value)
//@ get_IsInitialized()
//@ get_IsLoaded()
//@ get_ToolTip()
//@ set_ToolTip(Object value)
//@ get_PersistId()
//@ get_AllowDrop()
//@ set_AllowDrop(Boolean value)
//@ get_DesiredSize()
//@ get_IsMeasureValid()
//@ get_IsArrangeValid()
//@ get_RenderSize()
//@ set_RenderSize(Size value)
//@ get_RenderTransform()
//@ set_RenderTransform(Transform value)
//@ get_RenderTransformOrigin()
//@ set_RenderTransformOrigin(Point value)
//@ get_IsMouseDirectlyOver()
//@ get_IsMouseOver()
//@ get_IsStylusOver()
//@ get_IsStylusCaptureWithin()
//@ get_IsKeyboardFocusWithin()
//@ get_IsMouseCaptured()
//@ get_IsMouseCaptureWithin()
//@ get_IsStylusDirectlyOver()
//@ get_IsStylusCaptured()
//@ get_IsKeyboardFocused()
//@ get_IsInputMethodEnabled()
//@ get_Opacity()
//@ set_Opacity(Double value)
//@ get_OpacityMask()
//@ set_OpacityMask(Brush value)
//@ get_BitmapEffect()
//@ set_BitmapEffect(BitmapEffect value)
//@ get_BitmapEffectInput()
//@ set_BitmapEffectInput(BitmapEffectInput value)
//@ get_Visibility()
//@ set_Visibility(Visibility value)
//@ get_ClipToBounds()
//@ set_ClipToBounds(Boolean value)
//@ get_Clip()
//@ set_Clip(Geometry value)
//@ get_SnapsToDevicePixels()
//@ set_SnapsToDevicePixels(Boolean value)
//@ get_IsFocused()
//@ get_IsEnabled()
//@ set_IsEnabled(Boolean value)
//@ get_IsHitTestVisible()
//@ set_IsHitTestVisible(Boolean value)
//@ get_IsVisible()
//@ get_Focusable()
//@ set_Focusable(Boolean value)
//@ get_HasAnimatedProperties()
//@ get_InputBindings()
//@ get_CommandBindings()
//@ get_DependencyObjectType()
//@ get_IsSealed()
//@ get_Dispatcher()
//@ AutoTestBoolProperties()
//@ AutoTestEnumProperties()
//@ AutoTestStringProperties()
//@ AutoTestEvents()
