using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows;
using SWF = System.Windows.Forms;
using SWC = System.Windows.Controls;
using SWM = System.Windows.Media;
using System.Windows.Forms.Integration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using ReflectTools.AutoPME;

//
// Testcase:    XElementHost
// Description: AutoPME Verification for Element Host.
// Author:      a-rickyt
//
public class XElementHost : XControl
{
    #region Test case setup

    ElementHost elementHost;
    String testPropertyValue;

    [STAThread]
    public static void Main(String[] args)
    {
        SWF.Application.Run(new XElementHost(args));
    }
    
    protected override void InitTest(TParams p)
    {
        base.InitTest(p);
        ExcludedEvents.Add("GotFocus");
    }
    
    public XElementHost(String[] args) : base(args) { }

    protected override Type Class
    {
        get
        {
            return typeof(ElementHost);
        }
    }

    protected override Object CreateObject(TParams p)
    {
        return new ElementHost();
    }

    #endregion

    #region Overrides
    
    protected override ScenarioResult get_Focused(TParams p)
    {
        return new ScenarioResult(true);
    }

    protected override ScenarioResult get_ContainsFocus(TParams p)
    {
        return new ScenarioResult(true);
    }

    protected override ScenarioResult get_PreferredSize(TParams p)
    {
        return new ScenarioResult(true);
    }

    protected override ScenarioResult get_AccessibleRole(TParams p)
    {
        return new ScenarioResult(true);
    }

    protected override ScenarioResult set_AccessibleRole(TParams p, SWF.AccessibleRole value)
    {
        return new ScenarioResult(true);
    }

    protected override ScenarioResult get_ProductName(TParams p)
    {
        return new ScenarioResult(true);
    }

    /// <summary>
    /// ResetImeMode on this XElementHost -- DISABLED
    /// </summary>
    /// <param name="p">Not applicable.</param>
    /// <returns>True</returns>
    /// <remarks>
    /// Documentation for System.Windows.Forms.Control.ResetImeMode has the following remark:
    /// "This method is not relevant for this class."
    /// </remarks>
    protected override ScenarioResult ResetImeMode(TParams p)
    {
        return new ScenarioResult(true);
    }

    /// <summary>
    /// get_ImeMode on this XElementHost -- DISABLED
    /// </summary>
    /// <param name="p">Not applicable.</param>
    /// <returns>True</returns>
    /// <remarks>
    /// ImeMode added to ExcludedProperties in XControl.
    /// </remarks>
    protected override ScenarioResult get_ImeMode(TParams p)
    {
        return new ScenarioResult(true);
    }

    /// <summary>
    /// set_ImeMode on this XElementHost -- DISABLED
    /// </summary>
    /// <param name="p">Not applicable.</param>
    /// <param name="value">Not applicable.</param>
    /// <returns>True</returns>
    /// <remarks>
    /// ImeMode added to ExcludedProperties in XControl.
    /// </remarks>
    protected override ScenarioResult set_ImeMode(TParams p, SWF.ImeMode value)
    {
        return new ScenarioResult(true);
    }

    #endregion

    #region Scenarios
    
    protected ScenarioResult get_HostContainer(TParams p)
    {
        elementHost = GetElementHost(p);
        return new ScenarioResult("System.Windows.Forms.Integration.AvalonAdapter", 
            elementHost.HostContainer.ToString(), "HostContainer should be " +
            "System.Windows.Forms.Integration.AvalonAdapter" + ", but is " + 
            elementHost.HostContainer.ToString() + " instead.", p.log);
    }

    protected ScenarioResult get_Child(TParams p)
    {
        SWC.Button avButton = new SWC.Button();
        return set_Child(p, avButton);
    }

    protected ScenarioResult set_Child(TParams p, UIElement value)
    {
        elementHost = GetElementHost(p);
        elementHost.Child = value;
        return new ScenarioResult(value, elementHost.Child, "Child " +
            "should be " + value + ", but is " + elementHost.Child + " instead.", p.log);
    }

    protected ScenarioResult get_BackColorTransparent(TParams p)
    {
        return set_BackColorTransparent(p, p.ru.GetBoolean());
    }

    protected ScenarioResult set_BackColorTransparent(TParams p, Boolean value)
    {
        elementHost = GetElementHost(p);
        elementHost.BackColorTransparent = value;
        return new ScenarioResult(value, elementHost.BackColorTransparent, "BackColorTransparent " +
            "should be " + value + ", but is " + elementHost.BackColorTransparent + " instead.", p.log);
    }

    protected ScenarioResult OnPropertyChanged(TParams p, String propertyName, Object value)
    {
        ScenarioResult sr = new ScenarioResult();
        elementHost = GetElementHost(p);
        String val = "Element Host 1";
        elementHost.PropertyMap.Add("Name", new PropertyTranslator(MyPropertyTranslator));
        elementHost.Name = val;
        sr.IncCounters("", testPropertyValue, "OnPropertyChanged: Name " +
            "should be " + "" + ", but is " + testPropertyValue + " instead.", p.log);

        elementHost.OnPropertyChanged("Name", val);
        sr.IncCounters(val, testPropertyValue, "OnPropertyChanged: Name " +
            "should be " + val + ", but is " + testPropertyValue + " instead.", p.log);

        return sr;
    }
    private void MyPropertyTranslator(object host, String propertyName, object value)
    {
        testPropertyValue = value.ToString();
    }

    protected ScenarioResult get_PropertyMap(TParams p)
    {
        elementHost = GetElementHost(p);
        return new ScenarioResult("System.Windows.Forms.Integration.ElementHostPropertyMap",
            elementHost.PropertyMap.ToString(), "PropertyMap should be " +
            "System.Windows.Forms.Integration.ElementHostPropertyMap" + ", but is " +
            elementHost.PropertyMap.ToString() + " instead.", p.log);
    }

    #endregion

    #region Utilities

    ElementHost GetElementHost(TParams p)
    {
        if (p.target is ElementHost)
        {
            return (ElementHost)p.target;
        }
        else
        {
            p.log.WriteLine("object !instanceof ElementHost");
            return null;
        }
    }

    #endregion
}
// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios. 
//
// [Scenarios]
//@ get_Focused()
//@ get_ContainsFocus()
//@ get_PreferredSize()
//@ get_AccessibleRole()
//@ set_AccessibleRole(AccessibleRole value)
//@ get_ProductName()
//@ get_HostContainer()
//@ get_Child()
//@ set_Child(UIElement value)
//@ get_BackColorTransparent()
//@ set_BackColorTransparent(Boolean value)
//@ OnPropertyChanged(String propertyName, Object value)
//@ get_PropertyMap()
//@ get_Bottom()
//@ get_CanFocus()
//@ get_CanSelect()
//@ set_Capture(Boolean value)
//@ get_Capture()
//@ get_ClientRectangle()
//@ set_ClientSize(Size value)
//@ get_ClientSize()
//@ get_Controls()
//@ get_Created()
//@ get_DisplayRectangle()
//@ get_IsDisposed()
//@ get_Disposing()
//@ set_Enabled(Boolean value)
//@ get_Enabled()
//@ get_HasChildren()
//@ set_ImeMode(ImeMode value)
//@ get_ImeMode()
//@ get_IsHandleCreated()
//@ set_Left(Int32 value)
//@ get_Left()
//@ set_Location(Point value)
//@ get_Location()
//@ set_Parent(Control value)
//@ get_Parent()
//@ get_RecreatingHandle()
//@ get_Right()
//@ set_TabIndex(Int32 value)
//@ get_TabIndex()
//@ set_TabStop(Boolean value)
//@ get_TabStop()
//@ set_Text(String value)
//@ get_Text()
//@ set_Top(Int32 value)
//@ get_Top()
//@ get_TopLevelControl()
//@ set_Visible(Boolean value)
//@ get_Visible()
//@ set_WindowTarget(IWindowTarget value)
//@ get_WindowTarget()
//@ BringToFront()
//@ Contains(Control ctl)
//@ CreateControl()
//@ Focus()
//@ GetChildAtPoint(Point value)
//@ GetChildAtPoint(Point value, GetChildAtPointSkip skip)
//@ GetContainerControl()
//@ GetNextControl(Control ctl, Boolean forward)
//@ Hide()
//@ Invalidate()
//@ Invalidate(Boolean invalidateChildren)
//@ Invalidate(Rectangle rc)
//@ Invalidate(Rectangle rc, Boolean invalidateChildren)
//@ PerformLayout()
//@ PerformLayout(Control affectedControl, String affectedProperty)
//@ PointToClient(Point pt)
//@ PointToScreen(Point pt)
//@ PreProcessMessage(Message&amp; msg)
//@ PreProcessControlMessage(Message&amp; msg)
//@ RectangleToClient(Rectangle r)
//@ RectangleToScreen(Rectangle r)
//@ Refresh()
//@ ResetText()
//@ ResumeLayout()
//@ ResumeLayout(Boolean b)
//@ Select()
//@ SelectNextControl(Control ctl, Boolean forward, Boolean tabStopOnly, Boolean nested, Boolean wrap)
//@ SendToBack()
//@ Show()
//@ SuspendLayout()
//@ Update()
//@ set_CausesValidation(Boolean value)
//@ get_CausesValidation()
//@ get_Handle()
//@ ResetRightToLeft()
//@ ResetForeColor()
//@ ResetBackColor()
//@ ResetFont()
//@ BeginInvoke(Delegate method, Object[] args)
//@ BeginInvoke(Delegate method)
//@ EndInvoke(IAsyncResult asyncResult)
//@ Invoke(Delegate method, Object[] args)
//@ Invoke(Delegate method)
//@ FindForm()
//@ DoDragDrop(Object data, DragDropEffects allowedEffects)
//@ CreateGraphics(IntPtr dc)
//@ CreateGraphics()
//@ set_RightToLeft(RightToLeft value)
//@ get_RightToLeft()
//@ set_Region(Region value)
//@ get_Region()
//@ get_InvokeRequired()
//@ set_ForeColor(Color value)
//@ get_ForeColor()
//@ set_BackColor(Color value)
//@ get_BackColor()
//@ set_Font(Font value)
//@ get_Font()
//@ get_DefaultFont()
//@ set_ContextMenuStrip(ContextMenuStrip value)
//@ get_ContextMenuStrip()
//@ set_Dock(DockStyle value)
//@ get_Dock()
//@ set_Cursor(Cursor value)
//@ get_Cursor()
//@ set_BackgroundImageLayout(ImageLayout value)
//@ get_BackgroundImageLayout()
//@ set_BackgroundImage(Image value)
//@ get_BackgroundImage()
//@ set_Anchor(AnchorStyles value)
//@ get_IsMirrored()
//@ get_Anchor()
//@ set_AllowDrop(Boolean value)
//@ get_AllowDrop()
//@ set_IsAccessible(Boolean value)
//@ get_IsAccessible()
//@ set_AccessibleName(String value)
//@ get_AccessibleName()
//@ set_AccessibleDescription(String s)
//@ get_AccessibleDescription()
//@ set_AccessibleDefaultActionDescription(String s)
//@ get_AccessibleDefaultActionDescription()
//@ get_AccessibilityObject()
//@ get_CompanyName()
//@ get_ProductVersion()
//@ ResetCursor()
//@ get_Site()
//@ set_Site(ISite value)
//@ get_DataBindings()
//@ ResetBindings()
//@ get_BindingContext()
//@ set_BindingContext(BindingContext value)
//@ Invalidate(Region region, Boolean invalidateChildren)
//@ Invalidate(Region region)
//@ Dispose()
//@ get_Size()
//@ set_Size(Size value)
//@ set_Width(Int32 value)
//@ get_Width()
//@ set_Bounds(Rectangle value)
//@ get_Bounds()
//@ SetBounds(Int32 x, Int32 y, Int32 width, Int32 height)
//@ SetBounds(Int32 x, Int32 y, Int32 width, Int32 height, BoundsSpecified specified)
//@ set_Height(Int32 value)
//@ get_Height()
//@ Scale(SizeF factor)
//@ Scale(Single ratio)
//@ Scale(Single dx, Single dy)
//@ ControlsAddRange(Control[] value)
//@ ResetImeMode()
//@ set_Name(String s)
//@ get_Name()
//@ set_Tag(Object o)
//@ get_Tag()
//@ set_AutoSize(Boolean value)
//@ get_AutoSize()
//@ set_AutoRelocate(Boolean value)
//@ get_AutoRelocate()
//@ set_Margin(Padding value)
//@ get_Margin()
//@ set_Padding(Padding value)
//@ get_Padding()
//@ GetPreferredSize(Size proposedSize)
//@ get_LayoutEngine()
//@ set_MaximumSize(Size value)
//@ get_MaximumSize()
//@ set_MinimumSize(Size value)
//@ get_MinimumSize()
//@ set_UseWaitCursor(Boolean value)
//@ get_UseWaitCursor()
//@ get_AutoScrollOffset()
//@ set_AutoScrollOffset(Point value)
//@ Control_CheckForIllegalCrossThreadCalls()
//@ DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
//@ TestAccessibleEvents()
//@ ToString()
//@ get_Container()
//@ GetLifetimeService()
//@ InitializeLifetimeService()
//@ CreateObjRef(Type requestedType)
//@ GetHashCode()
//@ GetType()
//@ Equals(Object obj)
//@ AutoTestBoolProperties()
//@ AutoTestEnumProperties()
//@ AutoTestStringProperties()
//@ AutoTestEvents()
