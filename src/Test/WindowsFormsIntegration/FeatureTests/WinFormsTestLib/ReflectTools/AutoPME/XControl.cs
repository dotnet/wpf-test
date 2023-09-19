// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using SystemPens = System.Drawing.SystemPens;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Runtime.InteropServices;

namespace ReflectTools.AutoPME
{
    public abstract class XControl : XComponent
    {
        private Control _c;

        private Control _newC;   // a newly-created object for use with testing Reset properties

        public XControl(String[] args) : base(args) { }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
            ExcludedEvents.Add("UserPreferenceChanged");
            ExcludedEvents.Add("UserPreferenceChanging");
            // Exclude the following properties from AutoTest testing
            ExcludedProperties.Add("TopLevel");     // Exclude TopLevel since it can't be set to true if control is parented
            ExcludedProperties.Add("Anchor");       // Do we want to exclude this?
            ExcludedProperties.Add("RightToLeft");  // Can be set to Inherit but never returns it.
            ExcludedProperties.Add("ImeMode");      // Can be set to Inherit but never returns it.
            ExcludedProperties.Add("AllowDrop");    // Fails without UIPermission (expected)
			ExcludedProperties.Add("AutoRelocate"); // Obsolete
            if (PreHandleMode)
                ExcludedProperties.Add("Visible");  // Always returns false before handle is created.

            if (!Utilities.HavePermission(LibSecurity.AllWindows))
                ExcludedProperties.Add("Capture");  // This fails if we don't have AllWindows

            // Initialize the new object
            _newC = (Control)CreateObject(p);

            // Exclude Text on Win9x (Regression_Bug51).  There are chars on Win9x that cannot be
            // round-tripped, even if they are valid Unicode characters.
            //
            if (Utilities.IsWin9x)
                ExcludedProperties.Add("Text");
        }

        private Control GetControl(TParams p)
        {
            if (p.target is Control)
                return (Control)p.target;
            else
            {
                p.log.WriteLine("object !instanceof Control");
                return null;
            }
        }

        //
        // AutoTest custom methods for raising events.
        //
        protected virtual void RaisePaint(TParams p)
        {
            Control c = (Control)p.target;
            Graphics g = c.CreateGraphics();
            PaintEventArgs e = new PaintEventArgs(g, p.ru.GetContainedRectangle(c.Bounds));

            InvokeOnEventMethod(p.target, "Paint", new object[] { e });
            g.Dispose();
        }

        protected virtual void RaiseControlRemoved(TParams p)
        {
            ControlEventArgs e = new ControlEventArgs(new Control());

            InvokeOnEventMethod(p.target, "ControlRemoved", new object[] { e });
        }

        protected void RaiseQueryAccessibilityHelp(TParams p)
        {
            // Getting this will raise the event
            p.log.WriteLine("AccessibilityObject.Help returned \"{0}\"", ((Control)p.target).AccessibilityObject.Help);
        }

        protected AccessibleRole GetDefaultAccessibleRoleForType(Type t)
        {
            AccessibleRole ar;
            if (t == typeof(Button))
                ar = AccessibleRole.PushButton;
            else if (t == typeof(ContainerControl))
                ar = AccessibleRole.Client;
            else if (t == typeof(CheckBox))
                ar = AccessibleRole.CheckButton;
            else if (t == typeof(CheckedListBox))
                ar = AccessibleRole.List;
            else if (t == typeof(ComboBox))
                ar = AccessibleRole.ComboBox;
            else if (t == typeof(ContextMenuStrip))
                ar = AccessibleRole.MenuPopup;
            else if (t == typeof(DataGridView))
                ar = AccessibleRole.Table;
            else if (t == typeof(DataGridViewComboBoxEditingControl))
                ar = AccessibleRole.ComboBox;
            else if (t == typeof(DataGridViewTextBoxEditingControl))
                ar = AccessibleRole.Text;
            else if (t == typeof(BindingNavigator))
                ar = AccessibleRole.ToolBar;
            else if (t == typeof(DateTimePicker))
                ar = AccessibleRole.DropList;
            else if (t == typeof(DomainUpDown))
                ar = AccessibleRole.ComboBox;
            else if (t == typeof(FlowLayoutPanel))
                ar = AccessibleRole.Client;
            else if (t == typeof(Form))
                ar = AccessibleRole.Client;
            else if (t == typeof(GroupBox))
                ar = AccessibleRole.Grouping;
            else if (t == typeof(HScrollBar))
                ar = AccessibleRole.ScrollBar;
            else if (t == typeof(Label))
                ar = AccessibleRole.StaticText;
            else if (t == typeof(LinkLabel))
                ar = AccessibleRole.StaticText;
            else if (t == typeof(ListBox))
                ar = AccessibleRole.List;
            else if (t == typeof(ListView))
                ar = AccessibleRole.List;
            else if (t == typeof(MaskedTextBox))
                ar = AccessibleRole.Text;
            else if (t == typeof(MenuStrip))
                ar = AccessibleRole.MenuBar;
            else if (t == typeof(MonthCalendar))
                ar = AccessibleRole.Client;
            else if (t == typeof(NumericUpDown))
                ar = AccessibleRole.ComboBox;
            else if (t == typeof(Panel))
                ar = AccessibleRole.Client;
            else if (t == typeof(PictureBox))
                ar = AccessibleRole.Client;
            else if (t == typeof(PrintPreviewControl))
                ar = AccessibleRole.Client;
            else if (t == typeof(PrintPreviewDialog))
                ar = AccessibleRole.Client;
            else if (t == typeof(ProgressBar))
                ar = AccessibleRole.ProgressBar;
            else if (t == typeof(PropertyGrid))
                ar = AccessibleRole.Client;
            else if (t == typeof(RadioButton))
                ar = AccessibleRole.RadioButton;
            else if (t == typeof(ToolStripContainer))
                ar = AccessibleRole.Client;
            else if (t == typeof(RichTextBox))
                ar = AccessibleRole.Text;
            else if (t == typeof(ScrollableControl))
                ar = AccessibleRole.Client;
            else if (t == typeof(SplitContainer))
                ar = AccessibleRole.Client;
            else if (t == typeof(Splitter))
                ar = AccessibleRole.Client;
            else if (t == typeof(StatusStrip))
                ar = AccessibleRole.StatusBar;
            else if (t == typeof(TabControl))
                ar = AccessibleRole.PageTabList;
            else if (t == typeof(TableLayoutPanel))
                ar = AccessibleRole.Client;
            else if (t == typeof(TabPage))
                ar = AccessibleRole.Client;
            else if (t == typeof(TextBox))
                ar = AccessibleRole.Text;
            else if (t == typeof(ThreadExceptionDialog))
                ar = AccessibleRole.Window;
            else if (t == typeof(ToolStrip))
                ar = AccessibleRole.ToolBar;
            else if (t == typeof(ToolStripDropDown))
                ar = AccessibleRole.MenuPopup;
            else if (t == typeof(ToolStripDropDownMenu))
                ar = AccessibleRole.MenuPopup;
            else if (t == typeof(ToolStripOverflow))
                ar = AccessibleRole.MenuPopup;
            else if (t == typeof(TrackBar))
                ar = AccessibleRole.Slider;
            else if (t == typeof(TreeView))
                ar = AccessibleRole.Outline;
            else if (t == typeof(UserControl))
                ar = AccessibleRole.Client;
            else if (t == typeof(VScrollBar))
                ar = AccessibleRole.ScrollBar;
            else if (t == typeof(WebBrowser))
                ar = AccessibleRole.Client;
            //Note: these types are requirements in certain tests, and aren't in the libs.
            else if (t.Name == "InheritComboBox")
                ar = GetDefaultAccessibleRoleForType(typeof(ComboBox));
            else if (t.Name == "InheritRichTextBox")
                ar = GetDefaultAccessibleRoleForType(typeof(RichTextBox));
            else if (t.Name == "InheritTabControl")
                ar = GetDefaultAccessibleRoleForType(typeof(TabControl));
            else
                throw new ArgumentException("Type " + t.ToString() + " default role not defined.  Please add expected default to the XControl class.  Contact SamuRai for details on what the correct default accessible role should be or see the control reference at http://www.msdn.microsoft.com/library/default.asp?url=/library/en-us/msaa/msaapndx_4erc.asp");
            return ar;
        }
        /**
         * ==================================================
         * TESTS
         * ==================================================
         */
        // Dummy Scenario to be called if the method being tested is marked <InternalOnly/>
        [Scenario(false)]
        protected virtual ScenarioResult InternalOnlyMethod(TParams p)
        {
            p.log.WriteTag("InternalOnly", true);
            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult get_Bottom(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            p.log.WriteLine("Botom is " + _c.Bottom.ToString());
            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult get_CanFocus(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            p.log.WriteLine("CanFocus is " + _c.CanFocus.ToString());
            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult get_CanSelect(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            p.log.WriteLine("CanSelect is " + _c.CanSelect.ToString());
            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult set_Capture(TParams p, bool value)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;
            ScenarioResult sr = new ScenarioResult();
            bool expected = !_c.Capture;
            bool bSet = SecurityCheck(sr, delegate
            {
                _c.Capture = expected;
            }, typeof(Control).GetMethod("set_Capture"), LibSecurity.AllWindows);
            expected = bSet ? expected : !expected;
            sr.IncCounters(expected, _c.Capture, "FAIL: Didn't get expected capture value", p.log);
            return get_Capture(p);
        }

        protected virtual ScenarioResult get_Capture(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;
            bool b = p.ru.GetBoolean();
            SafeMethods.SetCapture(_c, b);
            return new ScenarioResult(b, _c.Capture, "FAIL: couldn't get capture", p.log);
        }

        protected virtual ScenarioResult get_ClientRectangle(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            Rectangle r = _c.ClientRectangle;

            p.log.WriteLine("ClientRectangle is " + r.ToString());
            return new ScenarioResult(r.X == 0 && r.Y == 0 && r.Width == _c.ClientSize.Width && r.Height == _c.ClientSize.Height);
        }

        protected virtual ScenarioResult set_ClientSize(TParams p, Size value)
        {
            return get_ClientSize(p);
        }

        protected virtual ScenarioResult get_ClientSize(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            Size pt = p.ru.GetSize(this.Size);

            p.log.WriteLine("ClientSize was " + _c.ClientSize.ToString());
            p.log.WriteLine("ClientSize to be " + pt.ToString());
            _c.ClientSize = pt;

            Size ppt = _c.ClientSize;

            p.log.WriteLine("ClientSize is " + ppt.ToString());

            // Form can't get narrower than 88 pixels.
            if ((_c is Form) && (pt.Width < 88))
                pt.Width = 88;

            return new ScenarioResult(pt.Equals(ppt));
        }

        protected virtual ScenarioResult get_Controls(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            System.Windows.Forms.Control.ControlCollection cc = _c.Controls;

            return new ScenarioResult(cc != null);
        }

        protected virtual ScenarioResult get_Created(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            _c = (Control)CreateObject(p);
            result.IncCounters(!_c.Created, "FAIL: returned true, expected false", p.log);

            bool expected = true;

            Controls.Add(_c);
            Application.DoEvents();
            if (PreHandleMode)
            {
                p.log.WriteLine("Created returns false in PreHandleMode");
                expected = false;
            }

            result.IncCounters(_c.Created == expected, "FAIL: after adding control to the Form returned " + _c.Created + ", expected " + expected, p.log);
            return result;
        }

        protected virtual ScenarioResult get_DisplayRectangle(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            Rectangle r = _c.DisplayRectangle;
            Size s = _c.ClientSize;

            p.log.WriteLine("DisplayRect is " + r.ToString());
            p.log.WriteLine("ClientSize is " + s.ToString());
            return new ScenarioResult(r.Width == s.Width && r.Height == s.Height);
        }

        protected virtual ScenarioResult get_IsDisposed(TParams p)
        { return Dispose(p); }

        protected virtual ScenarioResult get_Disposing(TParams p)
        { return Dispose(p); }

        protected virtual ScenarioResult set_Enabled(TParams p, bool value)
        { return get_Enabled(p); }

        protected virtual ScenarioResult get_Enabled(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            bool b = _c.Enabled;

            p.log.WriteLine("Enabled was " + b.ToString());
            _c.Enabled = !b;

            bool bb = _c.Enabled;

            p.log.WriteLine("Enabled is " + bb.ToString());
            return new ScenarioResult(b != bb);
        }

        protected virtual ScenarioResult get_Focused(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ScenarioResult result = new ScenarioResult();
            Button b = new Button();
            bool expected = true;

            Controls.Add(b);
            SafeMethods.Focus(b);
            Application.DoEvents();
            result.IncCounters(!_c.Focused, "FAIL: returned true expected false", p.log);
            SafeMethods.Focus(_c);
            if (PreHandleMode)
            {
                p.log.WriteLine("cannot focus in PreHandleMode");
                expected = false;
            }

            result.IncCounters(_c.Focused == expected, "FAIL: returned " + _c.Focused + " expected " + expected, p.log);
            return result;
        }

        protected virtual ScenarioResult get_HasChildren(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            // HasChildren will be false for almost every newly created control
            _c = (Control)CreateObject(p);
            result.IncCounters(_c.HasChildren == (_c.Controls.Count != 0), "FAILED initial state", p.log);

            // Add child and verify HasChildren is now true
            _c.Controls.Add(new Button());
            result.IncCounters(_c.HasChildren, "FAILED: returned false after child control added", p.log);
            return result;
        }

        protected virtual ScenarioResult set_ImeMode(TParams p, ImeMode value)
        {
            return get_ImeMode(p);
        }

        protected virtual ScenarioResult get_ImeMode(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            System.Windows.Forms.ImeMode imemd = _c.ImeMode;

            p.log.WriteLine("initially Got: " + EnumTools.GetEnumStringFromValue(typeof(System.Windows.Forms.ImeMode), (int)imemd));
            if (System.Text.Encoding.Default.CodePage == 932)   // Japanese
                imemd = (ImeMode)p.ru.GetValidJapaneseImeValue();
            else if (System.Text.Encoding.Default.CodePage == 949)  // Korean
                imemd = (ImeMode)p.ru.GetValidKoreanImeValue();
            else
                imemd = (ImeMode)p.ru.GetDifferentEnumValue(typeof(System.Windows.Forms.ImeMode), (int)imemd);

            p.log.WriteLine("Setting to: " + EnumTools.GetEnumStringFromValue(typeof(System.Windows.Forms.ImeMode), (int)imemd));
            _c.ImeMode = imemd;

            ImeMode imemd2 = _c.ImeMode;

            p.log.WriteLine("retrieved: " + EnumTools.GetEnumStringFromValue(typeof(System.Windows.Forms.ImeMode), (int)imemd2));
            Application.DoEvents();

            //Added to reflect changes in QFE 4448, in which ImeMode.OnHalf got added but returns ImeMode.On
            //First check if ImeMode.OnHalf exists
            if (Enum.IsDefined(typeof(System.Windows.Forms.ImeMode), "OnHalf"))
            {
                if(imemd == ImeMode.OnHalf)
		            return new ScenarioResult(imemd2 == ImeMode.On);
            }

            return new ScenarioResult(imemd2 == GetExpectedImeMode(_c, imemd));
        }

        protected virtual ScenarioResult get_IsHandleCreated(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            _c = (Control)CreateObject(p);
            result.IncCounters(!_c.IsHandleCreated, "FAIL: returned true, expected false", p.log);

            Controls.Add(_c);
            Application.DoEvents();

            bool expected = true;

            if (PreHandleMode)
            {
                p.log.WriteLine("In PreHandleMode IsHandleCreated returns false");
                expected = false;
            }

            result.IncCounters(_c.IsHandleCreated == expected, "FAIL: returned " + _c.IsHandleCreated + ", expected " + expected, p.log);
            return result;
        }

        protected virtual ScenarioResult set_Left(TParams p, int value)
        {
            return get_Left(p);
        }

        protected virtual ScenarioResult get_Left(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ((Control)_c).Dock = DockStyle.None;

            int l = _c.Left;

            p.log.WriteLine("Left was " + l.ToString());

            int x;

            if (_c is Form && ((Form)_c).TopLevel)
                x = p.ru.GetScreenPoint().X;
            else
                x = p.ru.GetRange(0, _c.Width);

            int ll = p.ru.GetRange(-_c.Width / 2, x - (_c.Width / 2));

            p.log.WriteLine("Setting Left to " + ll.ToString());
            _c.Left = ll;
            l = _c.Left;
            p.log.WriteLine("Left is " + l.ToString());
            return new ScenarioResult(l == ll);
        }

        protected virtual ScenarioResult set_Location(TParams p, Point value)
        {
            return get_Location(p);
        }

        // when Visible = falase, Dock has no effect on location
        protected virtual ScenarioResult get_Location(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            if (_c is Form)
            {
                _c = new Form();
            }
            else
            {
                // if Control is parented to GroupBox - from previous scenarios
                // it's location will not be (0, 0)
                // So we'll parent out control to the Form
                if ((SafeMethods.GetParent(_c) != null) && (!SafeMethods.GetParent(_c).Equals(this)))
                {
                    SafeMethods.GetParent(_c).Controls.Remove(_c);
                    this.Controls.Clear();
                    this.Controls.Add(_c);
                    Application.DoEvents();
                }
            }

            p.log.WriteLine("current Visible: " + _c.Visible);
            p.log.WriteLine("current Dock: " + _c.Dock.ToString());

            Point pt = _c.Location;

            p.log.WriteLine("Location was " + pt.ToString());

            Point ppt = p.ru.GetScreenPoint();

            p.log.WriteLine("Setting to Location " + ppt.ToString());
            _c.Location = ppt;
            p.log.WriteLine("Location is " + _c.Location.ToString());

            // If control is docked and Visible, its location will not change
            // Splitter respects docking even when it's not visible
            if (_c.Dock != DockStyle.None && (_c.Visible || _c is Splitter))
            {
                return new ScenarioResult(_c.Location.Equals(pt));
            }

            return new ScenarioResult(_c.Location.Equals(ppt));
        }

        protected virtual ScenarioResult set_Parent(TParams p, Control value)
        {
            //don't call get_Parent, will cause a SecurityException with the wrong stack
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            Control pc = SafeMethods.GetParent(_c);
            if (pc != null) p.log.WriteLine("Parent was " + pc.ToString());

            Control expected = null;

            if (_c is Form) ((Form)_c).TopLevel = false;

            // pick a new parent
            int i = p.ru.GetRange(0, 5);

            p.log.WriteLine(i.ToString());
            switch (i)
            {
                case 0: // null
                    p.log.WriteLine("Setting Parent to null");
                    expected = null;
                    _c.Parent = expected;
                    break;

                case 1: // itself
                    p.log.WriteLine("Setting Parent to self");
                    try
                    {
                        expected = _c;
                        _c.Parent = expected;
                        return new ScenarioResult(false, "Control can't be parented to itself");
                    }
                    catch (Exception)
                    {
                        expected = pc;
                    }
                    break;

                case 2: // it's current parent
                    p.log.WriteLine("Setting Parent to Parent");
                    expected = SafeMethods.GetParent(_c);
                    _c.Parent = expected;
                    break;

                case 3: // another control already on the form
                    p.log.WriteLine("Setting Parent to control already on form");

                    GroupBox gb = new GroupBox();

                    this.Controls.Add(gb);
                    expected = gb;
                    _c.Parent = expected;
                    break;

                case 4: // another control not yet on the form
                default:
                    p.log.WriteLine("Setting Parent to control not yet on form");

                    GroupBox gb2 = new GroupBox();
                    expected = gb2;
                    _c.Parent = expected;
                    this.Controls.Add(gb2);
                    break;
            }

            Control pc2 = SafeMethods.GetParent(_c);

            AddObjectToForm(p);
            return new ScenarioResult(pc2 == expected, "EXPECTED: " + (expected == null ? "null" : expected.ToString()) + "; GOT: " + (pc2 == null ? "null" : pc2.ToString()));
        }

        protected virtual ScenarioResult get_Parent(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;
            ScenarioResult sr = new ScenarioResult();
            Control pc = null;

            bool gotParent = SecurityCheck(sr, delegate
            {
                pc = _c.Parent;
            }, typeof(Control).GetMethod("get_Parent"), LibSecurity.AllWindows);
            if (!gotParent)
            { sr.IncCounters(null == pc, "FAIL: get_Parent returned a value in partial trust", p.log); }


            if (pc != null) p.log.WriteLine("Parent was " + pc.ToString());

            Control expected = null;

            if (_c is Form) ((Form)_c).TopLevel = false;

            switch (p.ru.GetRange(0, 5))
            {
                case 0: // null
                    p.log.WriteLine("Setting Parent to null");
                    expected = null;
                    _c.Parent = null;
                    break;

                case 1: // itself
                    p.log.WriteLine("Setting Parent to self");
                    expected = SafeMethods.GetParent(_c);
					try
					{
						_c.Parent = _c;
						sr.IncCounters(new ScenarioResult(false, "Control can't be parented to itself (expected exception)", p.log));
					}
					catch (NotSupportedException) { }//For a control with a readonly controls collection, it will throw NSE instead of ArgE
					catch (ArgumentException) { }
                    break;

                case 2: // it's current parent
                    p.log.WriteLine("Setting Parent to Parent");
                    expected = SafeMethods.GetParent(_c);
                    _c.Parent = expected;
                    break;

                case 3: // another control already on the form
                    p.log.WriteLine("Setting Parent to control already on form");

                    GroupBox gb = new GroupBox();

                    this.Controls.Add(gb);
                    expected = gb;
                    _c.Parent = expected;
                    break;

                case 4: // another control not yet on the form
                default:
                    p.log.WriteLine("Setting Parent to control not yet on form");

                    GroupBox gb2 = new GroupBox();
                    _c.Parent = gb2;

                    expected = gb2;
                    this.Controls.Add(gb2);
                    break;
            }

            Control finalParent = null;
            gotParent = SecurityCheck(sr, delegate
            {
                finalParent = _c.Parent;
            }, typeof(Control).GetMethod("get_Parent"), LibSecurity.AllWindows);
            if (!gotParent)
            { finalParent = SafeMethods.GetParent(_c); }

            AddObjectToForm(p);
            sr.IncCounters(expected, finalParent, "FAIL: Final parent was incorrect", p.log);

            return sr;
        }

        protected virtual ScenarioResult get_RecreatingHandle(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            bool b = _c.RecreatingHandle;

            p.log.WriteLine("RecreatingHandle is " + b.ToString());
            return ScenarioResult.Pass;
        }

        // when Visible = false, Docking has no effect on returned Right value
        protected virtual ScenarioResult get_Right(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            p.log.WriteLine("current IsHandleCreated: " + _c.IsHandleCreated);
            p.log.WriteLine("current Visible: " + _c.Visible);
            p.log.WriteLine("current Dock: " + _c.Dock.ToString());

            int r = _c.Right;

            p.log.WriteLine("Right was " + r.ToString());
            p.log.WriteLine("Bounds were " + _c.Bounds.ToString());
            _c.Width++;

            int rr = _c.Right;

            p.log.WriteLine("Right is " + rr.ToString());
            p.log.WriteLine("Bounds are " + _c.Bounds.ToString());

            // Right changes when Dock = None, Left or Visible = false 
            if (_c.Dock != DockStyle.None && _c.Dock != DockStyle.Left && _c.Visible && _c.IsHandleCreated)
            {
                return new ScenarioResult(_c.Right == r);
            }

            return new ScenarioResult(rr == r + 1);
        }

        protected virtual ScenarioResult set_TabIndex(TParams p, int value)
        {
            return get_TabIndex(p);
        }

        protected virtual ScenarioResult get_TabIndex(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            int initTabIndex = _c.TabIndex;

            p.log.WriteLine("initial TabIndex: " + initTabIndex.ToString());

            int newTabIndex = p.ru.GetInt();

            try
            {
                p.log.WriteLine("setting TabIndex to: " + newTabIndex.ToString());
                _c.TabIndex = newTabIndex;
                if (newTabIndex < 0)
                    return new ScenarioResult(false, "FAILED: didn't throw exception for negative TabIndex", p.log);
            }
            catch (ArgumentException)
            {
                ScenarioResult sr = new ScenarioResult();

                sr.IncCounters(newTabIndex < 0, "FAILED: exception was thrown for non-negative value", p.log);
                sr.IncCounters(_c.TabIndex == initTabIndex, "FAILED: didn't preserve initial TabIndex", p.log);
                return sr;
            }
            catch (Exception e)
            {
                return new ScenarioResult(false, "FAILED: unexpected exception was thrown: " + e.Message, p.log);
            }
            p.log.WriteLine("new TabIndex: " + _c.TabIndex.ToString());
            return new ScenarioResult(_c.TabIndex == newTabIndex, "FAILED: set/get TabIndex", p.log);
        }

        protected virtual ScenarioResult set_TabStop(TParams p, bool value)
        {
            return get_TabStop(p);
        }

        protected virtual ScenarioResult get_TabStop(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            bool b = _c.TabStop;

            p.log.WriteLine("TabStop was " + b.ToString());
            _c.TabStop = !b;

            bool bb = _c.TabStop;

            p.log.WriteLine("TabStop is " + bb.ToString());
            return new ScenarioResult(b != bb);
        }

        protected virtual ScenarioResult set_Text(TParams p, String value)
        {
            return get_Text(p);
        }

        protected virtual ScenarioResult get_Text(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            String sold = _c.Text;
            String ss = p.ru.GetString(32, true);

            p.log.WriteLine("Text was " + sold);
            p.log.WriteLine("Setting Text to " + ss);
            textInASCII(ss);
            _c.Text = ss;

            String s = _c.Text;

            p.log.WriteLine("Text is " + s);
            textInASCII(s);

            // On Win9x, there are some chars that are not round-trippable even though
            // they are valid Unicode chracters.  According to TadaoM, it is sufficient to
            // just test the string length rather than muck with filtering all those characters.
            if (Utilities.IsWin9x)
                return new ScenarioResult(ss.Length == s.Length, "FAIL: Set string len = " + ss.Length + "; returned string len = " + s.Length, p.log);
            else
                return new ScenarioResult(s.Equals(ss));
        }

        protected void textInASCII(string toPrint)
        {
            string temp = "   ";

            for (int i = 0; i < toPrint.Length; i++)
            {
                temp += " " + (int)toPrint[i] + "  ";
            }

            scenarioParams.log.WriteLine(temp);
        }

        protected virtual ScenarioResult set_Top(TParams p, int value)
        {
            return get_Top(p);
        }

        protected virtual ScenarioResult get_Top(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            int l = _c.Top;

            p.log.WriteLine("current IsHandleCreated: " + _c.IsHandleCreated);
            p.log.WriteLine("current Visible: " + _c.Visible);
            p.log.WriteLine("Current dock: " + _c.Dock.ToString());
            p.log.WriteLine("Top was " + l.ToString());

            int ll = p.ru.GetRange(-_c.Width / 2, p.ru.GetScreenPoint().Y - (_c.Width / 2));

            p.log.WriteLine("Setting top to " + ll.ToString());
            _c.Top = ll;
            p.log.WriteLine("Top is " + _c.Top.ToString());
            if (PreHandleMode && _c is Splitter && (_c.Parent != null))
            {
                p.log.WriteLine("testing Splitter in PreHandle mode");
                p.log.WriteLine("curent IsHandleCreated: " + _c.IsHandleCreated);
                return new ScenarioResult(_c.Top == l, "FAIL: Expected " + l, p.log);
            }
            else
                // Top will change only when Dock is None and control is visible and the handle is created
                if (_c.Dock != DockStyle.None && _c.Visible && _c.IsHandleCreated)
                    return new ScenarioResult(_c.Top == l, "FAIL: Expected " + l, p.log);
                else
                    return new ScenarioResult(_c.Top == ll, "FAIL: Expected " + l, p.log);
        }

        protected virtual ScenarioResult get_TopLevelControl(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;


            Control actual = null;
            ScenarioResult sr = new ScenarioResult();
            bool bSet = SecurityCheck(sr, delegate
            {
                actual = _c.TopLevelControl;
            }, typeof(Control).GetMethod("get_TopLevelControl"), LibSecurity.AllWindows);

            Control expected = null;
            if (bSet)
            {
                expected = _c;
                while (expected != null && !(expected is Form && ((Form)expected).TopLevel))
                { expected = SafeMethods.GetParent(expected); }
            }

            return new ScenarioResult(expected, actual, "FAIL: Unexpected TopLevelControl", p.log);
        }

        protected virtual ScenarioResult set_Visible(TParams p, bool value)
        {
            return get_Visible(p);
        }

        protected virtual ScenarioResult get_Visible(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            bool b = _c.Visible;
            bool bb;

            p.log.WriteLine("Visible was " + b.ToString());
            _c.Visible = !b;
            bb = _c.Visible;
            p.log.WriteLine("Visible is " + bb.ToString());
            _c.Visible = b;

            bool expected = !b;

            if (PreHandleMode)
            {
                p.log.WriteLine("In PreHandleMode Visible returns false");
                expected = false;
            }

            return new ScenarioResult(bb == expected);
        }

        protected virtual ScenarioResult set_WindowTarget(TParams p, IWindowTarget value)
        {
            return InternalOnlyMethod(p);
        }

        protected virtual ScenarioResult get_WindowTarget(TParams p)
        {
            return InternalOnlyMethod(p);
        }

        protected virtual ScenarioResult BringToFront(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            // send control to Front, its ChildIndex should become 0
            _c.BringToFront();

            /*       int ind = this.Controls.GetChildIndex(c);
            p.log.WriteLine("after bringing control to Front its ChildIndex: " + ind.ToString());

            // check if TopLevelControl's name is the name of current control
            string current = (typeof(c).ToString()).Trim();
            string topLevel = ((c.TopLevelControl).ToString()).Trim();
            // retrieve name of control from both strings
            current = current.Substring(current.LastIndexOf(".") + 1, current.Length - current.LastIndexOf(".") - 1);
            topLevel = topLevel.Substring(1, topLevel.IndexOf(",") - 1); //get rid of 'x' at the beginning
            int iSame = System.String.Compare(current, topLevel);
            this.Controls.SetChildIndex(this, initInd);
            return new ScenarioResult((ind == 0) && (iSame == 0), "failed to bring control to the Front", p.log);
      */
                        return ScenarioResult.Pass;
        }

        /* No longer used
        private bool clicked = false;
                public void Clicked(Object source, MouseEventArgs e)           //for MouseEvent
                {
                    clicked = true;
                }    
        */
                        private bool IsSpecialCaseCallWndProc(Control c)
        {
            // These used to use "if ( c.GetType == tyepof(Foo) )", but these wouldn't cover
            // subclasses for inheritance tests, so we changed them to use the "is" operator
            if (c is DateTimePicker) return true;

            if (c is GroupBox) return true;

            if (c is Label) return true;

            if (c is LinkLabel) return true;

            if (c is Panel) return true;

            if (c is PictureBox) return true;

            if (c is ProgressBar) return true;

            if (c is ScrollableControl) return true;

            return false;
        }
#if false        
        protected virtual ScenarioResult CallWndProc(TParams p, int msg, IntPtr wParam, IntPtr lParam)
        {
// This method is gone for RTM
return ScenarioResult.Pass;
/*
            AddRequiredPermission(LibSecurity.AllWindows);

            if ((c = GetControl(p)) == null) return ScenarioResult.Fail;
            MouseEventHandler eh = new MouseEventHandler(this.Clicked);
            clicked = false;
            c.MouseUp += eh;

            c.CallWndProc(win.WM_LBUTTONUP, (IntPtr)0, (IntPtr)0);
            Application.DoEvents();

            c.MouseUp -= eh;

            // force success if special-cased control
            if (IsSpecialCaseCallWndProc(c)) clicked = true;
            return new ScenarioResult(clicked);
*/

        }
#endif

        protected virtual ScenarioResult Contains(TParams p, Control ctl)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ctl = _c;

            System.Windows.Forms.Control.ControlCollection cc = _c.Controls;

            if (cc.Count != 0) ctl = (Control)(cc[0]);

            bool b = _c.Contains(ctl);

            if (ctl == _c)
                return new ScenarioResult(!b, "Contains should return false for self");
            else
                return new ScenarioResult(b, "Contains failed for contained control");
        }

        protected virtual ScenarioResult get_ContainsFocus(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ScenarioResult sr = new ScenarioResult();

            // We'll use ContainsFocus instead of Focused since a control's child can have focus
            // and Focused will return false
            bool b = _c.ContainsFocus;

            p.log.WriteLine(" initial ContainsFocus: " + b.ToString());
            p.log.WriteLine("1. setting Focus to control...");
            SafeMethods.Focus(_c);

            p.log.WriteLine(" new Focused: " + _c.Focused.ToString());
            p.log.WriteLine(" ContainsFocus returns: " + _c.ContainsFocus.ToString());
            if (PreHandleMode && _c is Form)
            {
                _c = (Control)CreateObject(p);
            }

            // Forms with child controls or forms return false for Focused and true for ContainsFocus.
            if (!PreHandleMode && (_c.Focused || _c is UpDownBase || (_c is Form && !_c.Focused && (((Form)_c).IsMdiContainer) || _c.Controls.Count > 0)))
                sr.IncCounters(_c.ContainsFocus, "FAILED: returned False for focused control", p.log);
            else
                sr.IncCounters(!_c.ContainsFocus, "FAILED: returned True for non-focused control", p.log);

            System.Windows.Forms.Control.ControlCollection cc = _c.Controls;

            // It doesn't quite work this way on the grid so we'll skip this test.
            if (cc.Count != 0)
            {
                p.log.WriteLine("2. setting Focus to contained control...");
                SafeMethods.Focus((cc[0]));
                p.log.WriteLine(" contained control Focused: " + cc[0].Focused.ToString());
                b = _c.ContainsFocus;
                if (cc[0].Focused)
                    sr.IncCounters(b, "FAILED: returned False when contained control is focused", p.log);
                else
                {
                    p.log.WriteLine(" control itself is Focused: " + _c.Focused.ToString());
                    if (_c.Focused)
                        sr.IncCounters(_c.ContainsFocus, "FAILED: returned False for focused control", p.log);
                    else
                        sr.IncCounters(!_c.ContainsFocus, "FAILED: returned True for non-focused control", p.log);
                }

                p.log.WriteLine(" ContainsFocus returns: " + b.ToString());
            }

            return sr;
        }

        protected virtual ScenarioResult CreateControl(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ScenarioResult sr = new ScenarioResult();

            // 
            // our tested control is already CREATED
            //  calling CreateControl() on already created control should not affect 
            //  Created-property of this control, no matter the control is visible or not
            //
            bool b;

            p.log.WriteLine("   Initial Created: " + _c.Created.ToString());
            p.log.WriteLine("1. calling CreateControl() on already created control with Visible=true");
            b = _c.Visible;               // saving initial Visible
            _c.Visible = true;
            _c.CreateControl();
            p.log.WriteLine("   new Created: " + _c.Created.ToString());
            sr.IncCounters(_c.Created, "FAILED: to maintain control created when it's visible", p.log);
            p.log.WriteLine("2. calling CreateControl() on already created control with Visible=false");
            _c.Visible = false;
            _c.CreateControl();
            p.log.WriteLine("   new Created: " + _c.Created.ToString());
            sr.IncCounters(_c.Created, "FAILED: to maintain control created when it's not visible", p.log);
            _c.Visible = b;                  // restoring initial Visible

            //
            // declaring new control, that is not initially created
            //   when its Visible is set to false, calling CreateControl() 
            //  control should not be created
            //
            p.log.WriteLine("4. declaring new non-Visible control...");

            Control cc = new Control();
            Form f = new Form();

            f.Visible = true;
            f.CreateControl();
            cc.Visible = false;
            p.log.WriteLine("   Created: " + cc.Created.ToString());
            p.log.WriteLine("  a. calling CreateControl()");
            cc.CreateControl();
            p.log.WriteLine("   new Created: " + cc.Created.ToString());
            sr.IncCounters(!cc.Created, "FAILED: non-visible control should not be created", p.log);
            p.log.WriteLine("  b. adding non-visible not-created control to the form");
            f.Controls.Add(cc);
            p.log.WriteLine("    new Created: " + cc.Created.ToString());
            sr.IncCounters(!cc.Created, "FAILED: Non-visible control should not be created, even if added to form ", p.log);
            p.log.WriteLine("  c. calling CreateControl for non-visible not-created control that was added to the form");
            cc.CreateControl();
            p.log.WriteLine("   new Created: " + cc.Created.ToString());
            sr.IncCounters(!cc.Created, "FAILED: non-visible control on the form should not be created", p.log);

            // when Visible = true CreateControl() should create 
            // **changing Visible to True calls CreateControl for control with created parent
            //
            p.log.WriteLine("5. changing Visible for control on the form(control is not-created & non-visible) - it should call CreateControl()");
            cc.Visible = true;
            p.log.WriteLine("   new Created: " + cc.Created.ToString());
            sr.IncCounters(cc.Created, "FAILED: setting Visible=true for control on the form didn't call CreateControl()", p.log);
            p.log.WriteLine("6. calling CreateControl() for visible created control on the form");
            cc.CreateControl();
            p.log.WriteLine("   new Created: " + cc.Created.ToString());
            sr.IncCounters(cc.Created, "FAILED: CreateControl() for created control on the form didn't preserved it's 'Created'", p.log);
            cc.Dispose();
            f.Dispose();
            b = cc.Created;
            cc = new Control();
            cc.Visible = true;
            p.log.WriteLine("7. calling CreateControl for visible control without parent");
            p.log.WriteLine("   initial Created: " + cc.Created.ToString());
            cc.CreateControl();
            p.log.WriteLine("   new Created: " + cc.Created.ToString());
            sr.IncCounters(cc.Created, "FAILED: visible control was not created", p.log);
            sr.IncCounters(!b, "FAILED: created is true after Dispose()", p.log);
            return sr;
        }

        protected virtual ScenarioResult Focus(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            // We'll use ContainsFocus instead of Focused since a control's child can have focus
            // and Focused will return false
            bool initialFocus = _c.ContainsFocus;
            bool can = _c.CanFocus;
            ScenarioResult sr = new ScenarioResult();

            p.log.WriteLine("ContainsFocus was: " + initialFocus.ToString());
            p.log.WriteLine("CanFocus: " + can.ToString());

            bool bSetFocus = SecurityCheck(sr, delegate
            { _c.Focus(); }, typeof(Control).GetMethod("Focus"), LibSecurity.AllWindows);

            if (can && bSetFocus)
            {
                sr.IncCounters(_c.CanFocus, "FAIL: did not Focus when CanFocus", p.log);
            }
            else
            {
                sr.IncCounters(initialFocus, _c.ContainsFocus, "FAIL: performed focusing when Cannot focus(!?)", p.log);
            }

            p.log.WriteLine("new ContainsFocus is: " + (_c.ContainsFocus).ToString());
            return sr;
        }

        // when child-control has Width or Height = 0, GetChildAtPoint cannot retrieve this child
        protected virtual ScenarioResult GetChildAtPoint(TParams p, Point value)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            Point pt = p.ru.GetPoint(new Point(_c.ClientSize));

            p.log.WriteLine("ClientSize of the control: " + _c.ClientSize.ToString());
            p.log.WriteLine("initial point to get a child at: " + pt.ToString());
            p.log.WriteLine("number of child-controls on the control: " + _c.Controls.Count.ToString());

            // SECURITY: GetChildAtPoint only demands AllWindows if the control returned from
            //           UnsafeNativeMethods.ChildWindowFromPoint is not a descendant.  This
            //			 should never happen, so we can basically assume this method does not
            //			 require any permissions.  Not much we can do otherwise.
            //AddRequiredPermission(LibSecurity.AllWindows);
            Control ctl = _c.GetChildAtPoint(pt);

            if ((_c.Controls.Count == 0) && (ctl != null))
                return new ScenarioResult(false, "FAILED: returned non-null for control with no children", p.log);
            else if ((_c.Controls.Count != 0) && (ctl == null))
            {
                // point is outside of child-controls - need to adjust it
                Size clsz = _c.Controls[0].ClientSize;
                Point loc = _c.Controls[0].Location;
                Size parentSz = _c.ClientSize;

                p.log.WriteLine("Bounds of 1st child: " + (_c.Controls[0].Bounds).ToString());

                // picking point within child's ClientArea--constrain it with a 1 pixel border
                pt = p.ru.GetPoint(1, clsz.Width - 1, 1, clsz.Height - 1);

                // adjust point according to location of child
                pt = new Point(loc.X + pt.X, loc.Y + pt.Y);
                p.log.WriteLine("new point to get a child: " + pt.ToString());
                ctl = _c.GetChildAtPoint(pt);

                //Utilities.MarkPoint(c, pt);     // For debugging--paint a marker to show the point.
                // in cases when Width or Height of child = 0, this child cannot be retrieved
		// ToolStripDropDowns may have scrollbuttons that may or may not be visible
                // Likewise with controls outside of the client area (and maybe invisible controls?)
                if ((clsz.Width != 0 && clsz.Height != 0) && (pt.X >= 0 && pt.X < parentSz.Width) && (pt.Y >= 0 && pt.Y < parentSz.Height) && _c.Controls[0].Visible)
                    return new ScenarioResult(ctl == _c.Controls[0], "FAILED: Expected " + _c.Controls[0] + ", but got " + ctl, p.log);
	        else if ((_c is ToolStripDropDownMenu) && (!_c.Controls[0].Visible))
 		    return new ScenarioResult(true);
                else
                    return new ScenarioResult(ctl == null, "FAILED: Expected null but got " + ctl, p.log);
            }

            return ScenarioResult.Pass;
        }

        // when child-control has Width or Height = 0, GetChildAtPoint cannot retrieve this child
        protected virtual ScenarioResult GetChildAtPoint(TParams p, Point value, GetChildAtPointSkip skip)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            skip = p.ru.GetEnumValue<GetChildAtPointSkip>();
            Point pt = p.ru.GetPoint(new Point(_c.ClientSize));

            p.log.WriteLine("ClientSize of the control: " + _c.ClientSize.ToString());
            p.log.WriteLine("initial point to get a child at: " + pt.ToString());
            p.log.WriteLine("number of child-controls on the control: " + _c.Controls.Count.ToString());

            // SECURITY: GetChildAtPoint only demands AllWindows if the control returned from
            //           UnsafeNativeMethods.ChildWindowFromPoint is not a descendant.  This
            //			 should never happen, so we can basically assume this method does not
            //			 require any permissions.  Not much we can do otherwise.
            //AddRequiredPermission(LibSecurity.AllWindows);
            Control ctl = _c.GetChildAtPoint(pt, skip);

            if ((_c.Controls.Count == 0) && (ctl != null))
                return new ScenarioResult(false, "FAILED: returned non-null for control with no children", p.log);
            else if ((_c.Controls.Count != 0) && (ctl == null))
            {
                // point is outside of child-controls - need to adjust it
                Control targetControl = _c.Controls[0];
                Size clsz = targetControl.ClientSize;
                Point loc = targetControl.Location;
                Size parentSz = _c.ClientSize;

                p.log.WriteLine("Bounds of 1st child: " + (_c.Controls[0].Bounds).ToString());

                // picking point within child's ClientArea--constrain it with a 1 pixel border
                pt = p.ru.GetPoint(1, clsz.Width - 1, 1, clsz.Height - 1);

                // adjust point according to location of child
                pt = new Point(loc.X + pt.X, loc.Y + pt.Y);
                p.log.WriteLine("new point to get a child: " + pt.ToString());
                switch (skip)
                {
                    case GetChildAtPointSkip.Disabled:
                        bool prevEnabled = targetControl.Enabled;
                        try
                        {
                            targetControl.Enabled = false;
                            ctl = _c.GetChildAtPoint(pt, GetChildAtPointSkip.Disabled);
                            return new ScenarioResult(null, ctl, "FAIL: should have skipped disabled child", p.log);
                        }
                        finally { targetControl.Enabled = prevEnabled; }
                    case GetChildAtPointSkip.Invisible:
                        bool prevVisible = targetControl.Visible;
                        try
                        {
                            targetControl.Visible = false;
                            ctl = _c.GetChildAtPoint(pt, GetChildAtPointSkip.Invisible);
                            return new ScenarioResult(null, ctl, "FAIL: should have skipped disabled child", p.log);
                        }
                        finally { targetControl.Visible = prevVisible; }
                    case GetChildAtPointSkip.None:
                        //normal testing follows
                        break;
                    case GetChildAtPointSkip.Transparent:
                        //normal testing follows
                        break;
                }
                ctl = _c.GetChildAtPoint(pt, GetChildAtPointSkip.None);


                //Utilities.MarkPoint(c, pt);     // For debugging--paint a marker to show the point.
                // in cases when Width or Height of child = 0, this child cannot be retrieved
		// ToolStripDropDowns may have scrollbuttons that may or may not be visible
                // Likewise with controls outside of the client area (and maybe invisible controls?)
                if ((clsz.Width != 0 && clsz.Height != 0) && (pt.X >= 0 && pt.X < parentSz.Width) && (pt.Y >= 0 && pt.Y < parentSz.Height) && _c.Controls[0].Visible)
                    return new ScenarioResult(ctl == _c.Controls[0], "FAILED: Expected " + _c.Controls[0] + ", but got " + ctl, p.log);
		else if ((_c is ToolStripDropDownMenu) && (!_c.Controls[0].Visible))
		    return new ScenarioResult(true);
                else
                    return new ScenarioResult(ctl == null, "FAILED: Expected null but got " + ctl, p.log);
            }

            return ScenarioResult.Pass;
        }


        protected virtual ScenarioResult GetContainerControl(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ScenarioResult sr = new ScenarioResult();
            IContainerControl container = null;
            bool bSuccess = SecurityCheck(sr, delegate
            { container = _c.GetContainerControl(); }, typeof(Control).GetMethod("GetContainerControl"), LibSecurity.AllWindows);

            if (bSuccess && container != null)
            {
                Control current = _c;
                bool bFoundContainer = false;
                while (null != current)
                {
                    if (current == container)
                    { bFoundContainer = true; }
                    current = SafeMethods.GetParent(current);
                }
                sr.IncCounters(bFoundContainer, "FAIL: returned object was not a parent of the control", p.log);
            }
            return sr;
        }

        protected virtual ScenarioResult GetNextControl(TParams p, Control ctl, Boolean forward)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ScenarioResult sr = ScenarioResult.Pass;

            if (_c.Controls.Count != 0)
            {
                ctl = (Control)(_c.Controls[0]);

                Control cc = _c.GetNextControl(ctl, true);

                cc = _c.GetNextControl(cc, false);
                sr = new ScenarioResult(cc == ctl, "Forward and back didn't return same control");
            }

            return sr;
        }

        protected virtual ScenarioResult Hide(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            bool b;

            _c.Hide();
            b = _c.Visible;
            _c.Show();
            return new ScenarioResult(!b, "Hide didn't set Visible to false");
        }

        private bool _painted = false;

        private void Painted(Object source, PaintEventArgs e)
        {
            _painted = true;
            scenarioParams.log.WriteLine("*******************PAINT EVENT FIRED***********************");
        }

        // You can tell if a control should receive Paint events by seeing if it or its
        // base class calls SetStyle(ControlStyles.UserPaint, true).  UserPaint is false by
        // default so it must explicitly be set to true for the control to receive paint
        // events.
        //
        // <seealso member="UpdateScenario"/>
        // <seealso member="RefreshScenario"/>
        private bool IsSpecialCasePaintControl(Control c)
        {
            // These used to use "if ( c.GetType == tyepof(Foo) )", but these wouldn't cover
            // subclasses for inheritance tests, so we changed them to use the "is" operator
            if (c is DateTimePicker) return true;

            if (c is MonthCalendar) return true;

            if (c is GroupBox) return true;

            // KevinTao: These are UserPaint controls.  They should receive paint events.
            //if (c is LinkLabel) return true;
            //if (c is Label) return true;
            if (c is ListBox) return true;

            if (c is ListView) return true;

            if (c is CheckedListBox) return true;

            if (c is ProgressBar) return true;

            if (c is TabControl) return true;

            if (c is TextBox) return true;

            if (c is RichTextBox) return true;

            if (c is ComboBox) return true;

            if (c is TrackBar) return true;

            if (c is NumericUpDown) return true;

            if (c is DomainUpDown) return true;

            // FlatStyle.System controls don't fire paint events
            if (c is ButtonBase && ((ButtonBase)c).FlatStyle == FlatStyle.System)
                return true;

            // LinkLabel doesn't do FlatStyle.System so we only special case Label
            if (c.GetType() == typeof(Label) && ((Label)c).FlatStyle == FlatStyle.System)
                return true;

            if (c is GroupBox && ((GroupBox)c).FlatStyle == FlatStyle.System)
                return true;

            return false;
        }

        //
        // Move control out from under security bubble as it causes failures in the
        // scenarios which raise paint events.
        //
        private void MoveControlOutOfSecurityBubble(TParams p, Control c)
        {
            if (!Utilities.HavePermission(LibSecurity.AllWindows) && c.Location.Y < 200)
            {
                c.Dock = (c is Splitter) ? DockStyle.Bottom : DockStyle.None;
                c.Top = 200;
                p.log.WriteLine("Moving control out from under the security bubble: " + c.Location);
            }
        }

		protected virtual void PrepareForInvalidate(TParams p)
		{
			this.Size = new Size(800, 800);
			_c = GetControl(p);
			_c.Size = new Size(200, 200);
			_c.Location = new Point(200, 200);

			SafeMethods.Activate(this);
			SafeMethods.Focus(this);
			Application.DoEvents();
			System.Threading.Thread.Sleep(100);
			Application.DoEvents();
		}

        protected virtual ScenarioResult Invalidate(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            // make sure the control is visible and created
            if (_c is Form) ((Form)_c).TopLevel = false;

            AddObjectToForm(p);
            Application.DoEvents();
            _c.Visible = true;
            Application.DoEvents();
            p.log.WriteLine("Bounds are " + _c.Bounds.ToString());
            p.log.WriteLine("ClientRectangle is " + _c.ClientRectangle.ToString());
            p.log.WriteLine("Form.ClentSize: " + this.ClientSize.ToString());
            MoveControlOutOfSecurityBubble(p, _c);
            if (controlWillNotInvalidate(_c, p))
            {
                return ScenarioResult.Pass;
            }

            ScenarioResult sr = new ScenarioResult();
	    PrepareForInvalidate(p);

            _painted = false;



            PaintEventHandler peh = new PaintEventHandler(this.Painted);

            ((Control)_c).Paint += peh;

            // need to make sure that there is visible rectangle to  Invalidate
            if (GetVisibleRectangle(p, _c) == Rectangle.Empty)
            {
                return ScenarioResult.Pass;
            }

            _c.Invalidate();
            p.log.WriteLine("before DoEvents onPaint was called: " + _painted.ToString());
            if (_painted) sr.Comments = "Invalidate not asynchronous";

            sr.IncCounters(!_painted);
            Application.DoEvents();
            p.log.WriteLine("after DoEvents onPaint was called: " + _painted.ToString());
            if (!IsSpecialCasePaintControl(_c))
            {
                if (!_painted) sr.Comments = "DoEvents did not process Paint message";

                sr.IncCounters(_painted);
            }

            p.log.WriteLine("current Visible: " + _c.Visible);
            _painted = false;
            _c.Invalidate();
            _c.Update();
            p.log.WriteLine("after Invalidate&Update onPaint was called: " + _painted.ToString());
            if (!IsSpecialCasePaintControl(_c))
            {
                if (!_painted) sr.Comments = "Update didn't process Paint message";

                sr.IncCounters(_painted);
            }

            ((Control)_c).Paint -= peh;
            return sr;
        }

        protected virtual ScenarioResult Invalidate(TParams p, Boolean invalidateChildren)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            // make sure the control is visible and created
            if (_c is Form) ((Form)_c).TopLevel = false;

            AddObjectToForm(p);
            _c.Visible = true;
            MoveControlOutOfSecurityBubble(p, _c);
            if (controlWillNotInvalidate(_c, p))
            {
                return ScenarioResult.Pass;
            }

            ScenarioResult sr = new ScenarioResult();

            _painted = false;

			PrepareForInvalidate(p);

            PaintEventHandler peh = new PaintEventHandler(this.Painted);

            ((Control)_c).Paint += peh;

            // need to make sure that there is visible rectangle to  Invalidate
            if (GetVisibleRectangle(p, _c) == Rectangle.Empty)
            {
                return ScenarioResult.Pass;
            }

            _c.Invalidate(true);
            p.log.WriteLine("before DoEvents onPaint was called: " + _painted.ToString());
            if (_painted) sr.Comments = "Invalidate not asynchronous";

            sr.IncCounters(!_painted);
            Application.DoEvents();
            p.log.WriteLine("after DoEvents onPaint was called: " + _painted.ToString());
            if (!IsSpecialCasePaintControl(_c))
            {
                if (!_painted) sr.Comments = "DoEvents did not process Paint message";

                sr.IncCounters(_painted);
            }

            p.log.WriteLine("current Visible: " + _c.Visible);
            _painted = false;
            _c.Invalidate(false);
            _c.Update();
            p.log.WriteLine("after Invalidate&Update onPaint was called: " + _painted.ToString());
            if (!IsSpecialCasePaintControl(_c))
            {
                if (!_painted) sr.Comments = "Update didn't process Paint message";

                sr.IncCounters(_painted);
            }

            ((Control)_c).Paint -= peh;
            return sr;
        }

        //
        // Returns a rectangle within the control that is within the control's
        // parent's bounds.
        //
        private Rectangle GetVisibleRectangle(TParams p, Control cc)
        {
            Rectangle cb = cc.RectangleToScreen(cc.ClientRectangle);
            //top level control
            if (SafeMethods.GetParent(cc) == null)
                return cc.ClientRectangle;
            Rectangle pb = SafeMethods.GetParent(cc).RectangleToScreen(SafeMethods.GetParent(cc).ClientRectangle);
            Rectangle ir = Rectangle.Intersect(cb, pb);

            // The control must be outside of its parent's bounds
            if (ir.IsEmpty)
            {
                p.log.WriteLine("control and its container have no visible intersection-area");
                return Rectangle.Empty;
            }

            // Form can be paritally outside of the screen
            Form f = SafeMethods.FindForm(cc);
            Rectangle screen = SystemInformation.WorkingArea;

            p.log.WriteLine("Screen: " + screen);

            Rectangle fBounds = f.DesktopBounds;

            p.log.WriteLine("Form in screen coord: " + fBounds);

            Rectangle fVisible = Rectangle.Intersect(screen, fBounds);

            p.log.WriteLine("Visible part of the Form in screen coord: " + fVisible);
            ir = Rectangle.Intersect(ir, fVisible);

            // rectangle must be within visible area of the Form
            if (ir.IsEmpty)
            {
                p.log.WriteLine("control and Form have no visible intersection-area");
                return Rectangle.Empty;
            }

            Rectangle rr;

            do { rr = p.ru.GetIntersectingRectangle(ir); } while (rr.Width < 1 || rr.Height < 1);

            //  -- special case for TabPage ---
            // for TabPage it's neccessary to remember about its 'tab'-part(with text)
            // we need to generate intersecting rectangle within TabPage without this part with text
            if (cc is TabPage)
            {
                int dif = 34;  // need to figure out how to calculate 

                // Height of the strip with caption of TabPage
                if (ir.Height < dif)
                {
                    return Rectangle.Empty;
                }

                while (Rectangle.Intersect(cb, rr).Height < dif || rr.Width == 0)
                {
                    rr = p.ru.GetIntersectingRectangle(ir);
                }
            }

            return cc.RectangleToClient(rr);
        }

        protected virtual ScenarioResult Invalidate(TParams p, Rectangle rc)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            p.log.WriteLine("control ClientRectangle is " + _c.ClientRectangle.ToString());
            p.log.WriteLine("Form ClientRectangle is " + this.ClientRectangle.ToString());

            // make sure the control is visible and created
            if (_c is Form) ((Form)_c).TopLevel = false;
            if (_c is ToolStripDropDown && !Utilities.HavePermission(LibSecurity.AllWindows))
            {
                ((ToolStripDropDown)_c).TopLevel = false;
                ((ToolStripDropDown)_c).Parent = this;
            }

            AddObjectToForm(p);
            _c.Visible = true;
            Application.DoEvents();

            // on WinXP and Win98 have to give some time to receive previous events
            System.Threading.Thread.Sleep(500);

            // more debug info for Panel
            if (_c is Panel)
            {
                p.log.WriteLine("location of Panel: " + ((Panel)_c).Location.ToString());
                p.log.WriteLine("Form contains Panel: " + this.Controls.Contains(_c));
                ((Panel)_c).AutoScroll = false;   // if scrollbar is only visible then no invalidation
            }

            MoveControlOutOfSecurityBubble(p, _c);
            if (controlWillNotInvalidate(_c, p))
            {
                return ScenarioResult.Pass;
            }

            ScenarioResult sr = new ScenarioResult();

			PrepareForInvalidate(p); 
			
			_painted = false;

            PaintEventHandler peh = new PaintEventHandler(this.Painted);

            ((Control)_c).Paint += peh;
            rc = GetVisibleRectangle(p, _c);

            // doesn't make sense to invalidate empty rectangle
            if (rc == Rectangle.Empty)
            {
                return ScenarioResult.Pass;
            }

            p.log.WriteLine("intersection to be Invalidated: " + Rectangle.Intersect(_c.ClientRectangle, rc).ToString());

            // for controls with border when rectangle to Invalidate contains only border
            // onPaint will not be triggered
            if ((Rectangle.Intersect(_c.ClientRectangle, rc).Width < 3 || Rectangle.Intersect(_c.ClientRectangle, rc).Height < 3))
            {
                p.log.WriteLine("  Invalidate will not be triggered");
                return ScenarioResult.Pass;
            }

            // rectangle to update is in Form coordinates already
            p.log.WriteLine("rectangle to update is " + rc.ToString());

            // in case of 'bizarre' region rectangle-to-invalidate may not be within visible
            // part of the control - to ensure visibility change region to standard 'rectangle'-region
            ((Control)_c).Region = null;
            _c.Invalidate(rc);
            p.log.WriteLine("before DoEvents onPaint was called: " + _painted.ToString());
            if (_painted) sr.Comments = "Invalidate not asynchronous";

            sr.IncCounters(!_painted);
            Application.DoEvents();
            p.log.WriteLine("after DoEvents onPaint was called: " + _painted.ToString());
            if (!IsSpecialCasePaintControl(_c))
                sr.IncCounters(_painted, "DoEvents did not process Paint message: " + rc.ToString(), p.log);

            _painted = false;
            rc = GetVisibleRectangle(p, _c);

            // rectangle to update is in Form coordinates already
            p.log.WriteLine("rectangle to update is " + rc.ToString());
            p.log.WriteLine("intersection to be Invalidated: " + Rectangle.Intersect(_c.ClientRectangle, rc).ToString());

            // for controls with border when rectangle to Invalidate contains only border
            // onPaint will not be triggered
            if ((Rectangle.Intersect(_c.ClientRectangle, rc).Width < 3 || Rectangle.Intersect(_c.ClientRectangle, rc).Height < 3))
            {
                p.log.WriteLine(" Invalidate will not be triggered");
                return ScenarioResult.Pass;
            }

            p.log.WriteLine("current Visible: " + _c.Visible);
            _c.Invalidate(rc);
            _c.Update();
            p.log.WriteLine("after Invalidate&Update onPaint was called: " + _painted.ToString());
            if (!IsSpecialCasePaintControl(_c))
                sr.IncCounters(_painted, "Update didn't process Paint message: " + rc.ToString(), p.log);

            _c.Paint -= peh;
            if (_c is ToolStripDropDown && !Utilities.HavePermission(LibSecurity.AllWindows))
            {
                ((ToolStripDropDown)_c).Parent = null;
                ((ToolStripDropDown)_c).TopLevel = true;
            }
            return sr;
        }

        protected virtual ScenarioResult Invalidate(TParams p, Rectangle rc, Boolean invalidateChildren)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            p.log.WriteLine("ClientRectangle is " + _c.ClientRectangle.ToString());

            // make sure the control is visible and created
            if (_c is Form) ((Form)_c).TopLevel = false;
            if (_c is ToolStripDropDown && !Utilities.HavePermission(LibSecurity.AllWindows))
            {
                ((ToolStripDropDown)_c).TopLevel = false;
                ((ToolStripDropDown)_c).Parent = this;
            }

            AddObjectToForm(p);
            _c.Visible = true;
            Application.DoEvents();

            // on WinXP     and Win98 have to give some time for previous events to be received
            System.Threading.Thread.Sleep(500);
            if (_c is Panel)
            {
                p.log.WriteLine("location of Panel: " + ((Panel)_c).Location.ToString());
                p.log.WriteLine("Form contains Panel: " + this.Controls.Contains(_c));
                ((Panel)_c).AutoScroll = false;  // if scrollbar only visible - no invalidating
            }

            MoveControlOutOfSecurityBubble(p, _c);
            if (controlWillNotInvalidate(_c, p))
            {
                return ScenarioResult.Pass;
            }

            ScenarioResult sr = new ScenarioResult();

			PrepareForInvalidate(p);

            _painted = false;

            PaintEventHandler peh = new PaintEventHandler(this.Painted);

            ((Control)_c).Paint += peh;
            rc = GetVisibleRectangle(p, _c);

            // doesn't make sense to invalidate empty rectangle
            if (rc == Rectangle.Empty)
            {
                return ScenarioResult.Pass;
            }

            //    p.log.WriteLine("intersection to be Invalidated: " + Rectangle.Intersect(c.ClientRectangle, rc).ToString());
            // for controls with border when rectangle to Invalidate contains only border
            // onPaint will not be triggered
            if ((Rectangle.Intersect(_c.ClientRectangle, rc).Width < 3 || Rectangle.Intersect(_c.ClientRectangle, rc).Height < 3))
            {
                p.log.WriteLine("  Invalidate will not be triggered");
                return ScenarioResult.Pass;
            }

            // rectangle to update is in Form's coordinates already
            p.log.WriteLine("rectangle to update is " + rc.ToString());

            // in case of 'bizarre' region rectangle-to-invalidate may not be within visible
            // part of the control - to ensure visibility change region to standard 'rectangle'-region
            ((Control)_c).Region = null;
            _c.Invalidate(rc, true);
            p.log.WriteLine("before DoEvents onPaint was called: " + _painted.ToString());
            if (_painted) sr.Comments = "Invalidate not asynchronous";

            sr.IncCounters(!_painted);
            Application.DoEvents();
            p.log.WriteLine("after DoEvents onPaint was called: " + _painted.ToString());
            if (!IsSpecialCasePaintControl(_c))
            {
                if (!_painted) sr.Comments = "DoEvents did not process Paint message: " + rc.ToString(); ;
                sr.IncCounters(_painted);
            }

            _painted = false;
            rc = GetVisibleRectangle(p, _c);

            // rectangle to update in in Form's coordinates already
            p.log.WriteLine("rectangle to update is " + rc.ToString());

            //    p.log.WriteLine("intersection to be Invalidated: " + Rectangle.Intersect(c.ClientRectangle, rc).ToString());
            // for controls with border when rectangle to Invalidate contains only border
            // onPaint will not be triggered
            if ((Rectangle.Intersect(_c.ClientRectangle, rc).Width < 3 || Rectangle.Intersect(_c.ClientRectangle, rc).Height < 3))
            {
                p.log.WriteLine(" Invalidate will not be triggered");
                return ScenarioResult.Pass;
            }

            p.log.WriteLine("current Visible: " + _c.Visible);
            _c.Invalidate(rc, false);
            _c.Update();
            p.log.WriteLine("after Invalidate&Update onPaint was called: " + _painted.ToString());
            if (!IsSpecialCasePaintControl(_c))
            {
                if (!_painted) sr.Comments = "Update didn't process Paint message: " + rc.ToString();

                sr.IncCounters(_painted);
            }

            ((Control)_c).Paint -= peh;
            if (_c is ToolStripDropDown && !Utilities.HavePermission(LibSecurity.AllWindows))
            {
                ((ToolStripDropDown)_c).Parent = null;
                ((ToolStripDropDown)_c).TopLevel = true;
            }
            return sr;
        }

        //
        //  if only border of Panel is visible, onPaint will not be triggered when
        // Invalidating or Refreshing
        // will use this helper in all Invalidate/Refresh related scenarios to return Pass 
        // for Panel/MultiplexPanel if only their border is visible
        //
        bool controlWillNotInvalidate(Control cc, TParams p)
        {
            Rectangle r1 = cc.RectangleToScreen(cc.ClientRectangle);
            //toplevel control - most likely will Invalidate
            if (SafeMethods.GetParent(cc) == null)
                return false;

            Rectangle r2 = SafeMethods.GetParent(cc).RectangleToScreen(SafeMethods.GetParent(cc).ClientRectangle);


            Rectangle ir = Rectangle.Intersect(r1, r2);

            p.log.WriteLine("in ControlWillNotInvalidate: intersection: " + ir);

            // Control might be obscured by security bubble.
            if (!Utilities.HavePermission(LibSecurity.AllWindows) && cc.Location.Y < 200)
            {
                p.log.WriteLine("Security bubble obscures control.  Won't invalidate.");
                return true;
            }

            if (ir.Width == r1.Height && ir.Height == r1.Height)
            {
                return false;
            }
            else
            {
                p.log.WriteLine("Visible area of control is less than 2 x 2.  Won't invalidate.");
                p.log.WriteLine("intersection: " + ir.ToString());
                return true;
            }
        }

        protected virtual ScenarioResult PerformLayout(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            _c.PerformLayout();
            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult PerformLayout(TParams p, Control affectedControl, String affectedProperty)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            affectedControl = null;
            if (_c.Controls.Count != 0)
                affectedControl = (Control)(_c.Controls[0]);

            affectedProperty = "Visible";
            _c.PerformLayout(affectedControl, affectedProperty);
            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult PointToClient(TParams p, Point pt)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            pt = p.ru.GetPoint();
            return new ScenarioResult(_c.PointToClient(_c.PointToScreen(pt)).Equals(pt));
        }

        protected virtual ScenarioResult PointToScreen(TParams p, Point pt)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            pt = p.ru.GetPoint();
            return new ScenarioResult(_c.PointToScreen(_c.PointToClient(pt)).Equals(pt));
        }

        protected virtual ScenarioResult PreProcessMessage(TParams p, ref Message msg)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            Message m = new Message();

            _c.PreProcessMessage(ref m);
            return InternalOnlyMethod(p);
        }

		protected virtual ScenarioResult PreProcessControlMessage(TParams p, ref Message msg)
		{
			if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

			Message m = new Message();
			ScenarioResult sr = new ScenarioResult();
			new NamedPermissionSet("FullTrust").Assert();
//			new SecurityPermission(PermissionState.Unrestricted).Assert();
			PreProcessControlState pcs = _c.PreProcessControlMessage(ref m);
			Boolean b = _c.PreProcessMessage(ref m);

			switch (pcs)
			{
				case PreProcessControlState.MessageNeeded:
					sr.IncCounters(!b);
					break;

				case PreProcessControlState.MessageNotNeeded:
					sr.IncCounters(!b);
					break;
				case PreProcessControlState.MessageProcessed:
					sr.IncCounters(b);
					break;
			}

			sr.IncCounters(InternalOnlyMethod(p));
			return sr;
		}

        protected virtual ScenarioResult RectangleToClient(TParams p, Rectangle r)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            r = p.ru.GetRectangle();
            return new ScenarioResult(_c.RectangleToClient(_c.RectangleToScreen(r)).Equals(r));
        }

        protected virtual ScenarioResult RectangleToScreen(TParams p, Rectangle r)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            r = p.ru.GetRectangle();
            return new ScenarioResult(_c.RectangleToScreen(_c.RectangleToClient(r)).Equals(r));
        }

        protected virtual ScenarioResult Refresh(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            // make sure the control is visible and created
            if (_c is Form) ((Form)_c).TopLevel = false;
            if (_c is ToolStripDropDown && !Utilities.HavePermission(LibSecurity.AllWindows))
            {
                ((ToolStripDropDown)_c).TopLevel = false;
                ((ToolStripDropDown)_c).Parent = this;
            }

            AddObjectToForm(p);
            _c.Visible = true;
            p.log.WriteLine("current Visible: " + _c.Visible);
            MoveControlOutOfSecurityBubble(p, _c);
            if (controlWillNotInvalidate(_c, p))
            {
                return ScenarioResult.Pass;
            }

            _painted = false;

            PaintEventHandler peh = new PaintEventHandler(this.Painted);

            _c.Paint += peh;

            //************************************************
            Rectangle rc = GetVisibleRectangle(p, _c);

            // doesn't make sense to invalidate empty rectangle
            if (rc == Rectangle.Empty)
            {
                return ScenarioResult.Pass;
            }

            // in case of 'bizarre' region rectangle-to-invalidate may not be within visible
            // part of the control - to ensure visibility change region to standard 'rectangle'-region
            ((Control)_c).Region = null;
            _c.Invalidate(rc, true);

            //**********************************************************************************
            _c.Refresh();

            // force success if special-cased paint control
            // !!! for new controls IsSpecialCase.. may return False as they are
            // not named among special controls --> add line for new control in IsSpecial..
            if (IsSpecialCasePaintControl(_c)) _painted = true;

            _c.Paint -= peh;
            if (_c is ToolStripDropDown && !Utilities.HavePermission(LibSecurity.AllWindows))
            {
                ((ToolStripDropDown)_c).Parent = null;
                ((ToolStripDropDown)_c).TopLevel = true;
            }
            return new ScenarioResult(_painted);
        }

        protected virtual ScenarioResult ResetText(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            p.log.WriteLine("initial Text: " + _c.Text);
            _c.ResetText();
            p.log.WriteLine("Text after reset: " + _c.Text);

            bool bResult = _c.Text == "";

            return new ScenarioResult(bResult, "FAILED: didn't reset Text to empty string", p.log);
        }

        protected virtual ScenarioResult ResumeLayout(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            _c.SuspendLayout();
            _c.PerformLayout();
            _c.ResumeLayout();
            _c.PerformLayout();
            return ScenarioResult.Pass;
        }

        protected ScenarioResult ResumeLayout(TParams p, bool b)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            _c.ResumeLayout(false);
            _c.ResumeLayout(true);
            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult Select(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            if (_c is Form) return ScenarioResult.Pass;

            bool canFocus = _c.CanFocus;

            p.log.WriteLine("current CanFocus: " + canFocus);

            //need to set Active Control to none
            SafeMethods.SetActiveControl(this, null);
            SafeMethods.Focus(this);
            Application.DoEvents();
            BeginSecurityCheck(LibSecurity.AllWindows);
            _c.Select();
            EndSecurityCheck();

            // We'll use ContainsFocus instead of Focused since a control's child can have focus
            // and Focused will return false
            bool result = (canFocus && _c.ContainsFocus) || (!canFocus && !_c.ContainsFocus);

            p.log.WriteLine("after Select Focused = " + _c.Focused.ToString());
            return new ScenarioResult(result);
        }

        protected virtual ScenarioResult SelectNextControl(TParams p, Control ctl, Boolean forward, Boolean tabStopOnly, Boolean nested, Boolean wrap)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ctl = _c;

            int rng = -1;

            p.log.WriteLine("start ctrl: " + ctl.ToString());

            //check to see if composite control
            if (_c.Controls.Count != 0)
            {
                rng = p.ru.GetRange(0, _c.Controls.Count - 1);
                ctl = (Control)(_c.Controls[rng]);
            }

            foreach (Control ct in _c.Controls)
            {
                p.log.WriteLine(ct.ToString());
            }

            p.log.WriteLine("Rng: " + rng.ToString());
            p.log.WriteLine("End ctrl: " + ctl.ToString());
            forward = p.ru.GetBoolean();
            tabStopOnly = p.ru.GetBoolean();
            nested = p.ru.GetBoolean();
            wrap = p.ru.GetBoolean();
            p.log.WriteLine("Forward: " + forward.ToString() + " tabStopOnly: " + tabStopOnly.ToString());
            p.log.WriteLine("Nested: " + nested.ToString() + " Wrap: " + wrap.ToString());
            try
            {
                Application.DoEvents();
                bool b = _c.SelectNextControl(ctl, forward, tabStopOnly, nested, wrap);

                p.log.WriteLine("SelectNextControl returned " + b.ToString());

                // Perform check for security purpose.  We want to trigger a security exception.
                while (Controls.Count < 2)
                {
                    Controls.Add(new Button());
                }

                // We'll do it on the form since it's a container control
                SelectNextControl(Controls[0], true, false, false, true);
            }
            catch (SecurityException)
            {
                BeginSecurityCheck(LibSecurity.AllWindows);
                throw;
            }
            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult SendToBack(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            _c.SendToBack();
            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult Show(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            _c.Show();
            return new ScenarioResult(_c.Visible, "Show did not set Visible to true");
        }

        protected virtual ScenarioResult SuspendLayout(TParams p)
        {
            return ResumeLayout(p);
        }

        protected virtual ScenarioResult Update(TParams p)
        {
            return Invalidate(p);
        }

        private bool IsMnemonic(TParams p, bool expected, char charCode, String text)
        {
            p.log.Write("Is '" + charCode.ToString() + "' the mnemonic for \"" + text + "\"?  ");

            bool b = Control.IsMnemonic(charCode, text);

            p.log.WriteLine(b ? "YES" : "NO");
            return b == expected;
        }

        protected virtual ScenarioResult set_CausesValidation(TParams p, bool value)
        {
            return get_CausesValidation(p);
        }

        protected virtual ScenarioResult get_CausesValidation(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            bool b = _c.CausesValidation;

            p.log.WriteLine("CausesValidation was " + b.ToString());
            _c.CausesValidation = !b;

            bool bb = _c.CausesValidation;

            p.log.WriteLine("CausesValidation is " + bb.ToString());
            return new ScenarioResult(b != bb);
        }

        protected virtual ScenarioResult get_Handle(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            int h = (int)_c.Handle;

            p.log.WriteLine("Handle is " + h.ToString());
            if (PreHandleMode)
            {
                p.target = CreateObject(p);
            }

            return new ScenarioResult(h != 0, "handle not created");
        }

        protected virtual ScenarioResult ResetRightToLeft(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            // Get the default value from an unaltered instance of this control
            RightToLeft defaultValue = _newC.RightToLeft;

            p.log.WriteLine("default RightToLeft = " + defaultValue.ToString());

            RightToLeft newValue;

            newValue = (RightToLeft)p.ru.GetEnumValue(typeof(RightToLeft));
            p.log.WriteLine("setting RightToLeft to " + newValue.ToString());
            _c.RightToLeft = newValue;

            // ResetRightToLeft resets RightToLeft to Inherit
            // so we will need to compare it to Parent.RightToLeft
            p.log.WriteLine("calling ResetRightToLeft()");
            _c.ResetRightToLeft();
            newValue = _c.RightToLeft;
            if (SafeMethods.GetParent(_c) != null)
            {
                RightToLeft expected = SafeMethods.GetParent(_c).RightToLeft;

                // cannot set RightToLeft on ListBox - on Win9X, NT4 OSes
                if (_c is ListBox && (SafeMethods.GetOSVersion().Platform != System.PlatformID.Win32NT || SafeMethods.GetOSVersion().Version.Major < 5))
                    expected = RightToLeft.No;

                p.log.WriteLine("Default value = {0}, Expected_value = {1}, New_value_after_Reset = {2}", defaultValue.ToString(), expected.ToString(), newValue.ToString());
                return new ScenarioResult(expected.Equals(newValue));
            }
            else
            {
                return new ScenarioResult(true, "The Form/Control has no parent to inherit RTL settings from!");
            }
        }

        protected virtual ScenarioResult ResetForeColor(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            // Get the default value from an unaltered instance of this control
            Color defaultValue = _newC.ForeColor;
            Color newValue;

            _c.ForeColor = p.ru.GetColor();
            _c.ResetForeColor();
            newValue = _c.ForeColor;
            p.log.WriteLine("Default = {0}, New = {1}", defaultValue, newValue);
            return new ScenarioResult(defaultValue.Equals(newValue));
        }

        protected virtual ScenarioResult ResetBackColor(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            // Get the default value from an unaltered instance of this control
            Color defaultValue = _newC.BackColor;
            Color newValue;

            _c.BackColor = p.ru.GetColor();
            _c.ResetBackColor();
            newValue = _c.BackColor;
            p.log.WriteLine("Default = {0}, New = {1}", defaultValue, newValue);
            return new ScenarioResult(defaultValue.Equals(newValue));
        }

        protected virtual ScenarioResult ResetFont(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            // Get the default value from an unaltered instance of this control
            Font defaultValue = _newC.Font;
            Font newValue;

            Font newFont = p.ru.GetFont();
            // work around VSWhidbey #107035 for Label controls
            if (_c is Label && (newFont.Unit == GraphicsUnit.Inch || newFont.SizeInPoints > 150))
                newFont = new Font(newFont.FontFamily, Math.Min(newFont.Size, 150f), newFont.Style, GraphicsUnit.Pixel);
            _c.Font = newFont;
            _c.ResetFont();
            newValue = _c.Font;
            p.log.WriteLine("Default = {0}, New = {1}", defaultValue, newValue);
            return new ScenarioResult(defaultValue.Equals(newValue));
        }

        private bool _invokeSuccess;

        private delegate void delVoidParamsVoid();

        private delegate void delVoidParamsIntStringObject(int n, String s, Object o);

        private void VoidMethodVoid()
        {
            _invokeSuccess = true;
        }

        private void VoidMethodIntStringObject(int n, String s, Object o)
        {
            _invokeSuccess = true;
        }

        //Looks like these two methods are removed from Control.
        protected virtual ScenarioResult BeginInvoke(TParams p, Delegate method, Object[] args)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            _invokeSuccess = false;
            args = new Object[] { 42, "Howdy!", this };
            _c.BeginInvoke(new delVoidParamsIntStringObject(this.VoidMethodIntStringObject), args);
            if (_invokeSuccess)
                return new ScenarioResult(false, "InvokeAsync not asynchronous");

            Application.DoEvents();
            return new ScenarioResult(_invokeSuccess, "InvokeAsync failed");
        }

        protected virtual ScenarioResult BeginInvoke(TParams p, Delegate method)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            _invokeSuccess = false;
            _c.BeginInvoke(new delVoidParamsVoid(this.VoidMethodVoid));
            if (_invokeSuccess)
                return new ScenarioResult(false, "InvokeAsync not asynchronous");

            Application.DoEvents();
            return new ScenarioResult(_invokeSuccess, "InvokeAsync failed");
        }

        protected virtual ScenarioResult EndInvoke(TParams p, IAsyncResult asyncResult)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            _invokeSuccess = false;
            _c.EndInvoke(_c.BeginInvoke(new delVoidParamsVoid(this.VoidMethodVoid)));
            return new ScenarioResult(_invokeSuccess, "InvokeAsync not asynchronous");
        }

        protected virtual ScenarioResult Invoke(TParams p, Delegate method, Object[] args)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            _invokeSuccess = false;
            args = new Object[] { 42, "Howdy!", this };
            _c.Invoke(new delVoidParamsIntStringObject(this.VoidMethodIntStringObject), args);
            return new ScenarioResult(_invokeSuccess, "Invoke failed");
        }

        protected virtual ScenarioResult Invoke(TParams p, Delegate method)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            _invokeSuccess = false;
            _c.Invoke(new delVoidParamsVoid(this.VoidMethodVoid));
            return new ScenarioResult(_invokeSuccess, "Invoke failed");
        }

        protected virtual ScenarioResult FindForm(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ScenarioResult sr = new ScenarioResult();
            Form found = null;
            bool bFound = SecurityCheck(sr, delegate
            {
                found = _c.FindForm();
            }, typeof(Control).GetMethod("FindForm"), LibSecurity.AllWindows);


            Control expected = null;
            if (bFound)
            {
                expected = _c;

                while (SafeMethods.GetParent(expected) != null)
                { expected = SafeMethods.GetParent(expected); }
            }

            return new ScenarioResult(expected, found, "FAIL: FindForm found the wrong form", p.log);
        }

        protected virtual ScenarioResult DoDragDrop(TParams p, Object data, DragDropEffects allowedEffects)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            data = "Howdy!";
            allowedEffects = (DragDropEffects)p.ru.GetEnumValue(typeof(DragDropEffects));
            SafeMethods.FindForm(_c).BringToFront();
            Application.DoEvents();

            // This test would hang if the control was outside of the bounds of the form,
            // so we'll resize the form and relocate the control for this test.
            Rectangle origFormBounds = this.DesktopBounds;
            Point origLoc = _c.Location;

            _c.Location = new Point(10, 10);
            this.DesktopBounds = new Rectangle(10, 10, 300, 300);
            SafeMethods.SetCursorPosition(_c.PointToScreen(new Point(Math.Max(0, -_c.Left), Math.Max(0, -_c.Top))));
            p.log.WriteLine("Effect tested: " + allowedEffects.ToString());
            p.log.WriteLine("Control bounds: " + _c.Bounds.ToString());
            p.log.WriteLine("Form.ClientSize : " + this.ClientSize.ToString());

            DragDropEffects ddeDone = _c.DoDragDrop(data, allowedEffects);

            Application.DoEvents();
            p.log.WriteLine("DoDragDrop return value: " + ddeDone.ToString());
            _c.Location = origLoc;
            this.DesktopBounds = origFormBounds;
            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult CreateGraphics(TParams p, IntPtr dc)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ScenarioResult sr = ScenarioResult.Pass;
            Graphics gr = _c.CreateGraphics();

            gr.DrawLine(SystemPens.WindowText, 0, 0, _c.Width, _c.Height);
            gr.DrawLine(SystemPens.WindowText, _c.Width, 0, 0, _c.Height);
            Application.DoEvents();
            try
            {
                gr.DrawLine(SystemPens.WindowText, 0, 0, _c.Width, _c.Height);
                gr.DrawLine(SystemPens.WindowText, _c.Width, 0, 0, _c.Height);
                gr.Dispose();
            }
            catch (Exception)
            {
                sr = new ScenarioResult(false, "Graphics disposed by forcing message loop");
            }
            _c.Invalidate();
            return sr;
        }

        protected virtual ScenarioResult CreateGraphics(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ScenarioResult sr = ScenarioResult.Pass;
            Graphics gr = _c.CreateGraphics();

            gr.DrawLine(SystemPens.WindowText, 0, 0, _c.Width, _c.Height);
            gr.DrawLine(SystemPens.WindowText, _c.Width, 0, 0, _c.Height);
            Application.DoEvents();
            try
            {
                gr.DrawLine(SystemPens.WindowText, 0, 0, _c.Width, _c.Height);
                gr.DrawLine(SystemPens.WindowText, _c.Width, 0, 0, _c.Height);
                gr.Dispose();
            }
            catch (Exception)
            {
                sr = new ScenarioResult(false, "FAILED: Graphics disposed by forcing message loop");
            }
            _c.Invalidate();
            return sr;
        }

        protected virtual ScenarioResult set_RightToLeft(TParams p, RightToLeft value)
        {
            return get_RightToLeft(p);
        }

        protected virtual ScenarioResult get_RightToLeft(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            RightToLeft rtle = _c.RightToLeft;

            p.log.WriteLine("initially Got: " + EnumTools.GetEnumStringFromValue(typeof(System.Windows.Forms.RightToLeft), (int)rtle));
            rtle = (RightToLeft)p.ru.GetDifferentEnumValue(typeof(System.Windows.Forms.RightToLeft), (int)rtle);
            p.log.WriteLine("Setting to: " + EnumTools.GetEnumStringFromValue(typeof(System.Windows.Forms.RightToLeft), (int)rtle));
            _c.RightToLeft = rtle;

            RightToLeft rtle2 = _c.RightToLeft;

            p.log.WriteLine("retrieved: " + EnumTools.GetEnumStringFromValue(typeof(System.Windows.Forms.RightToLeft), (int)rtle2));
            Application.DoEvents();
            return new ScenarioResult(rtle2 == GetExpectedRightToLeft(_c, rtle), "Expected " + GetExpectedRightToLeft(_c, rtle));
        }

        //
        // Returns the expected RightToLeft value given the value set to the control's property.
        //
        private RightToLeft GetExpectedRightToLeft(Control c, RightToLeft setValue)
        {
            if (setValue != RightToLeft.Inherit)
                return setValue;
            else
            {
                // No need to travel up the parent chain since this control will inherit directly
                // from its parent.
                if (SafeMethods.GetParent(c) != null)
                    return SafeMethods.GetParent(c).RightToLeft;
                else
                {
                    // This code copied from Control.DefaultRightToLeft (Private)
                    // We should use RTL by default if the lower 10 bits of LCID are 0x01 or 0x0d.
                    //int lcid = System.Globalization.CultureInfo.CurrentCulture.LCID & 0x3FF; // 0x3FF == lower 10 bits
                    //if (lcid == 0x01 || lcid == 0x0d) 
                    //        return RightToLeft.Yes;
                    //else
                    return RightToLeft.No;
                }
            }
        }

        protected virtual ScenarioResult set_Region(TParams p, Region value)
        {
            return get_Region(p);
        }

        protected virtual ScenarioResult get_Region(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            Region orig = _c.Region;
            Region region = p.ru.GetRegion(_c.Size);

            //willsad 3/2/00
            // Region Type enum cannot be null when setting the random region
            // need to get different region to avoid null ref exception
            while (region == null)
            {
                region = p.ru.GetRegion(_c.Size);
            }

            if (_c is Form)
            {                      // This is only protected for top-level Forms
                ((Form)_c).MdiParent = null;
                ((Form)_c).TopLevel = true;
                //				BeginSecurityCheck(LibSecurity.AllWindows);
            }
            
            ScenarioResult sr = new ScenarioResult();
            bool bSet = SecurityCheck(sr, delegate
            {
                _c.Region = region;
            }, typeof(Control).GetMethod("set_Region"), (_c is Form) ? LibSecurity.AllWindows : LibSecurity.SafeTopLevelWindows);

            bool passed = (_c.Region == region);

            // Return region to original value so painting tests aren't ----ed up
            SafeMethods.SetRegion(_c, orig);
            return new ScenarioResult(bSet, passed, "FAIL: incorrect region", p.log);
        }

        protected virtual ScenarioResult get_InvokeRequired(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            bool b = _c.InvokeRequired;

            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult set_ForeColor(TParams p, Color value)
        {
            return get_ForeColor(p);
        }

        protected virtual ScenarioResult get_ForeColor(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            Color co = _c.ForeColor;
            Color cc = p.ru.GetColor();

            _c.ForeColor = cc;
            return new ScenarioResult(cc.Equals(_c.ForeColor));
        }

        protected virtual ScenarioResult set_BackColor(TParams p, Color value)
        {
            return get_BackColor(p);
        }

        protected virtual ScenarioResult get_BackColor(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            Color co = _c.BackColor;
            Color cc = p.ru.GetColor();

            _c.BackColor = cc;
            if (PreHandleMode)
                p.target = CreateObject(p);

            return new ScenarioResult(cc.Equals(_c.BackColor));
        }

        protected virtual ScenarioResult set_Font(TParams p, Font value)
        {
            return get_Font(p);
        }

        protected virtual ScenarioResult get_Font(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            Font f = _c.Font;

            p.log.WriteLine("initial Font: " + f.ToString());

            Font ff = p.ru.GetFont();
            // work around VSWhidbey #107035 - Win32Exception for large Fonts on Label
            if (_c is Label && (ff.Unit == GraphicsUnit.Inch || ff.SizeInPoints > 150))
                ff = new Font(ff.FontFamily, Math.Min(ff.Size, 150f), ff.Style, GraphicsUnit.Pixel);

            p.log.WriteLine("setting Font to: " + ff.ToString());
            _c.Font = ff;
            return new ScenarioResult(ff.Equals(_c.Font));
        }

        protected virtual ScenarioResult get_DefaultFont(TParams p)
        {
            Font defaultFont = Control.DefaultFont;
            OperatingSystem osv = SafeMethods.GetOSVersion();

            // JPN WinNT4 has special logic to default to MS UI Gothic
            if (osv.Platform == System.PlatformID.Win32NT && osv.Version.Major <= 4)
            {
                if ((System.Globalization.CultureInfo.CurrentUICulture.LCID & 0x3ff) == 0x0011)
                {
                    p.log.WriteLine("System is JPN NT4, expect MS UI Gothic 8 pt.");
                    p.log.WriteLine("System is JPN NT4, real DefaultFont is " + defaultFont.ToString());
                    if ((System.Globalization.CultureInfo.CurrentCulture.EnglishName.ToString()).IndexOf("Japan") != -1)
                        return new ScenarioResult(defaultFont != null, "Failed: font is null", p.log);

                    return new ScenarioResult(defaultFont.Name == "MS UI Gothic" && defaultFont.Size == 8, "DefaultFont was " + defaultFont);
                }
            }

            // We're really only concerned about the JPN NT4 special case.  The rest of the time,
            // it uses the DEFAULT_GUI_FONT, or a few other choices if that doesn't work.
            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult set_ContextMenuStrip(TParams p, ContextMenuStrip value)
        {
            return get_ContextMenuStrip(p);
        }

        protected virtual ScenarioResult get_ContextMenuStrip(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            _c = GetControl(p);

            ContextMenuStrip cm = _c.ContextMenuStrip;
            ContextMenuStrip cm2 = null;

            if (p.ru.GetBoolean())
            {
                cm2 = new ContextMenuStrip();
                cm2.Items.Add(new ToolStripButton());
                cm2.Items.Add(new ToolStripLabel());
                cm2.Items.Add(new ToolStripMenuItem());
                cm2.Items.Add(new ToolStripSeparator());
            }

            _c.ContextMenuStrip = cm2;

            sr.IncCounters(cm2 == _c.ContextMenuStrip, "Wasn't the right ContextMenuStrip.", p.log);

            if (cm2 != null)
                sr.IncCounters(cm2.Items.Count == 4, "Item count was: " + cm2.Items.Count, p.log);

            return sr;
        }

        protected virtual ScenarioResult set_Dock(TParams p, DockStyle value)
        {
            return get_Dock(p);
        }

        protected virtual ScenarioResult get_Dock(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            DockStyle cde = _c.Dock;

            p.log.WriteLine("Dock was " + cde.ToString());
            cde = (DockStyle)p.ru.GetDifferentEnumValue(typeof(DockStyle), (int)cde);
            p.log.WriteLine("Setting Dock to " + cde.ToString());
            _c.Dock = cde;
            p.log.WriteLine("Dock is " + _c.Dock.ToString());
            return new ScenarioResult(cde == _c.Dock);
        }

        protected virtual ScenarioResult set_Cursor(TParams p, Cursor value)
        {
            return get_Cursor(p);
        }

        protected virtual ScenarioResult get_Cursor(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            Cursor cu = _c.Cursor;

            cu = p.ru.GetCursor();
            BeginSecurityCheck(LibSecurity.SafeSubWindows);
            _c.Cursor = cu;
            EndSecurityCheck();
            p.log.Write("UseWaitCursor: " + _c.UseWaitCursor);
            return new ScenarioResult(_c.UseWaitCursor ? _c.Cursor == Cursors.WaitCursor : _c.Cursor == cu);
        }

        protected virtual ScenarioResult set_BackgroundImageLayout(TParams p, ImageLayout value)
        {
            return get_BackgroundImageLayout(p);
        }

        protected virtual ScenarioResult get_BackgroundImageLayout(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ImageLayout il = _c.BackgroundImageLayout;

            il = (ImageLayout)p.ru.GetEnumValue(typeof(ImageLayout));
            p.log.WriteLine("Setting to " + il.ToString());
            _c.BackgroundImageLayout = il;
            return new ScenarioResult(il == _c.BackgroundImageLayout, "Set " + il.ToString() + " but Got " + _c.BackgroundImageLayout.ToString());
        }

        protected virtual ScenarioResult set_BackgroundImage(TParams p, Image value)
        {
            return get_BackgroundImage(p);
        }

        // Used to fail because of ASURT #19067--now closed.
        protected virtual ScenarioResult get_BackgroundImage(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            Image i = _c.BackgroundImage;

            i = p.ru.GetImage(ImageStyle.Random);
            _c.BackgroundImage = i;
            return new ScenarioResult(i == _c.BackgroundImage);
        }

        protected virtual ScenarioResult set_Anchor(TParams p, AnchorStyles value)
        {
            return get_Anchor(p);
        }

        protected virtual ScenarioResult get_IsMirrored(TParams p)
        {
            p.log.WriteLine("Tested by exclusive test using control consumption");
            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult get_Anchor(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            AnchorStyles cae = _c.Anchor;

            cae = (AnchorStyles)p.ru.GetDifferentEnumValue(typeof(AnchorStyles), (int)cae);
            _c.Anchor = cae;
            return new ScenarioResult(cae == _c.Anchor);
        }

        protected virtual ScenarioResult set_AllowDrop(TParams p, bool value)
        {
            p.log.WriteLine("In Control.AllowDrop");
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;
            // SECURITY: Simple security verification.  We only demand when setting it to true.
            // Updated, AllowDrop throws an InvalidOperationException when Clipboard access is denied, not a SecurityException

			p.log.WriteLine(_c.AllowDrop.ToString());
            //Check whether we have read access to the clipboard (the same as AllClipboard)
            bool hasClip = Utilities.HavePermission(LibSecurity.AllClipboard);

            p.log.WriteLine("Security: checking ClipboardPermission: " + (hasClip ? "Granted" : "DENIED"));
            try
            {
                //Attempt to set AllowDrop if we don't have ClipboardRead, this should throw IOE
                _c.AllowDrop = true;
                //Should have thrown by now if we had perms
                if (!hasClip)
                {
                    //Mimic the EndSecurityCheck exception
                    throw new ReflectBaseException("FAIL (SECURITY): Setting Control.AllowDrop=true without AllClipboard succeeded (expected InvalidOperationException)");
                }
            }
            catch (InvalidOperationException e)
            {
                //If we can't explain this InvalidOperationException based on the security level
                if (hasClip)
                { throw; }
                else
                {
					p.log.WriteLine(_c.AllowDrop.ToString());
					p.log.WriteLine(e.InnerException.ToString());
					p.log.WriteLine("Control.AllowDrop threw InvalidOperationException (as expected) because ClipboardPermission was denied");
                }
            }
            return get_AllowDrop(p);
        }

        protected virtual ScenarioResult get_AllowDrop(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;
			try
			{
				ScenarioResult result = new ScenarioResult();

				try
				{
					_c.AllowDrop = false;
					result.IncCounters(!_c.AllowDrop, "FAIL: set to false, returned true", p.log);

					// We only demand when setting it to true
					SafeMethods.SetAllowDrop(_c, true);
					result.IncCounters(_c.AllowDrop, "FAIL: set to true, returned false", p.log);
				}
				catch (Exception e)
				{
					// Make sure SecurityExceptions propagate up (Regression_Bug52 by design)
					if (e.InnerException is SecurityException)
						throw e.InnerException;
					else if (e.InnerException is System.Threading.ThreadStateException)
					{
						p.log.LogException(e);
						return new ScenarioResult(false, p.log, BugDb.ASURT, 53, "ThreadStateException when run as HREF exe.  Postponed for Whidbey.");
					}
					else
						throw;
				}
				return result;
			}
			finally
			{
				SafeMethods.SetAllowDrop(_c, false);
			}
        }

        protected virtual ScenarioResult set_IsAccessible(TParams p, bool value)
        {
            return get_IsAccessible(p);
        }

        protected virtual ScenarioResult get_IsAccessible(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            bool b = _c.IsAccessible;

            _c.IsAccessible = !b;
            return new ScenarioResult(_c.IsAccessible == !b);
        }

        protected virtual ScenarioResult set_AccessibleRole(TParams p, AccessibleRole value)
        {
            return get_AccessibleRole(p);
        }

        protected virtual ScenarioResult get_AccessibleRole(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            AccessibleRole are = _c.AccessibleRole;

            p.log.WriteLine("AccessibleRole was " + are.ToString());
            are = (AccessibleRole)p.ru.GetDifferentEnumValue(typeof(AccessibleRole), (int)are);
            p.log.WriteLine("Setting AccessibleRole to " + are.ToString());
            _c.AccessibleRole = are;
            p.log.WriteLine("AccessibleRole is " + _c.AccessibleRole.ToString());
            sr.IncCounters(are, _c.AccessibleRole, "Control.AccessibleRole not the value set.", p.log);
            sr.IncCounters(are, _c.AccessibilityObject.Role, "Control.AccessibilityObject.Role not the value set for AccessibleRole.", p.log);
            p.log.WriteLine("Setting AccessibleRole to the Default.");
            _c.AccessibleRole = AccessibleRole.Default;
            AccessibleRole expectedDefaultRole = GetDefaultAccessibleRoleForType(_c.GetType());
            sr.IncCounters(AccessibleRole.Default, _c.AccessibleRole, "Control.AccessibleRole not AccessibleRole.Default.", p.log);
            sr.IncCounters(expectedDefaultRole, SafeMethods.GetAccessibleRole(_c.AccessibilityObject), "Control.AccessibilityObject.Role not the value expected when setting AccessibleRole to the default.", p.log);
            return sr;
        }

        protected virtual ScenarioResult set_AccessibleName(TParams p, String value)
        {
            return get_AccessibleName(p);
        }

        protected virtual ScenarioResult get_AccessibleName(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            String s = _c.AccessibleName;
            p.log.WriteLine("AccessibleName was " + s);
            s = p.ru.GetString(255, true);
            _c.AccessibleName = s;
            p.log.WriteLine("AccessibleName is " + _c.AccessibleName);
            p.log.WriteLine("Control.AccessibleObject.Name is " + _c.AccessibilityObject.Name);
            sr.IncCounters(s, _c.AccessibleName, "Control.AccessibleName not the value it was set to.", p.log);
            sr.IncCounters(s, _c.AccessibilityObject.Name, "Control.AccessiblityObject.Name not the value Control.AccessibleName was set to.", p.log);
            return sr;
        }

        protected virtual ScenarioResult set_AccessibleDescription(TParams p, String s)
        {
            return get_AccessibleDescription(p);
        }

        protected virtual ScenarioResult get_AccessibleDescription(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            String s = _c.AccessibleDescription;
            p.log.WriteLine("AccessibleDescription was " + s);
            s = p.ru.GetString(255, true);
            _c.AccessibleDescription = s;
            p.log.WriteLine("AccessibleDescription is " + _c.AccessibleDescription);
            p.log.WriteLine("Control.AccessibleObject.Description is " + _c.AccessibilityObject.Description);
            sr.IncCounters(s, _c.AccessibleDescription, "Control.AccessibleDescription not the value it was set to.", p.log);
            sr.IncCounters(s, _c.AccessibilityObject.Description, "Control.AccessiblityObject.Description not the value Control.AccessibleDescription was set to.", p.log);
            return sr;

        }

        protected virtual ScenarioResult set_AccessibleDefaultActionDescription(TParams p, String s)
        {
            return get_AccessibleDefaultActionDescription(p);
        }

        protected virtual ScenarioResult get_AccessibleDefaultActionDescription(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            String s = _c.AccessibleDefaultActionDescription;
            p.log.WriteLine("AccessibleDefaultActionDescription was " + s);
            s = p.ru.GetString(255, true);
            _c.AccessibleDefaultActionDescription = s;
            p.log.WriteLine("AccessibleDefaultActionDescription is " + _c.AccessibleDefaultActionDescription);
            p.log.WriteLine("Control.AccessibleObject.DefaultAction is " + _c.AccessibilityObject.DefaultAction);
            sr.IncCounters(s, _c.AccessibleDefaultActionDescription, "Control.AccessibleDefaultActionDescription not the value it was set to.", p.log);
            sr.IncCounters(s, _c.AccessibilityObject.DefaultAction, "Control.AccessiblityObject.DefaultAction not the value Control.AccessibleDefaultActionDescription was set to.", p.log);
            return sr;
        }

        // Calling AccessibilityObject results in handle creation. 
        // AccObj.Bounds return unpredictable resuls when Form with the control 
        // has no handle created and was not yet shown.
        // AccessibleObject.Bounds will depend on a location of the Form.
        // This scenario can be removed from PreHandle scenarios
        [PreHandleScenario(false)]
        protected virtual ScenarioResult get_AccessibilityObject(TParams p)
        {
            // Need to override per control to test the following properties:
            // DefaultAction
            // KeyboardShortcut
            // Parent
            // State
            // Value
            // The following properties are tested elsewhere in XControl
            // Description
            // Name
            // Role

            ScenarioResult sr = new ScenarioResult();
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;
            AccessibleObject cao = _c.AccessibilityObject;
            p.log.WriteLine("Test Accessible Object Bounds");
            sr.IncCounters(_c.RectangleToScreen(_c.ClientRectangle), SafeMethods.GetAccessibleBounds(cao), "Bounds for control and the accessible object don't match.", p.log);
            return sr;
        }

        protected virtual ScenarioResult get_CompanyName(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            String cn = _c.CompanyName;
            String en = "Microsoft Corporation";

            return new ScenarioResult(cn.Equals(en));
        }

        protected virtual ScenarioResult get_ProductName(TParams p)
        {

            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

			//The unicode value of the registered trademark symbol
			int characterCode = 0x00AE;
            String actualValue = _c.ProductName;
            String oldExpectedValue = "Microsoft(R) .NET Framework";
			String newExpectedValue = "Microsoft" + Convert.ToChar(characterCode) + " .NET Framework"; 

            return new ScenarioResult(actualValue.Equals(oldExpectedValue) || actualValue.Equals(newExpectedValue), "Unexpected value: " + actualValue);
        }

        protected virtual ScenarioResult get_ProductVersion(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            String pv = _c.ProductVersion;

            return new ScenarioResult(!pv.Equals(null));
        }

        protected virtual ScenarioResult ResetCursor(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            // Get the default value from an unaltered instance of this control
            Cursor defaultValue = _newC.Cursor;
            Cursor newValue;

            _c.Cursor = p.ru.GetCursor();
            _c.ResetCursor();
            newValue = _c.Cursor;
            p.log.WriteLine("Default = {0}, New = {1}", defaultValue, newValue);
            p.log.WriteLine("UseWaitCursor = " + _c.UseWaitCursor);
            return new ScenarioResult(_c.UseWaitCursor ? newValue == Cursors.WaitCursor : defaultValue.Equals(newValue));
        }

        // get_Control.Site calls base.get_Site
        protected override ScenarioResult get_Site(TParams p)
        {
            return base.get_Site(p);
        }

        // basically set_Site also calls bast.set_Site()
        // with only difference that it raises On[Font/ForeColor/BackColor/Cursor]Changed
        // events in these properties change as a result base.set_Site() call
        // As base(Component) implementation of Site doesn't really do anything,
        // it doesn't make sense to check if listed events are raised - it will never be the case
        // So we'll just call base.set_Site()
        protected override ScenarioResult set_Site(TParams p, ISite value)
        {
            return base.set_Site(p, value);
        }

        protected virtual ScenarioResult get_DataBindings(TParams p)
        {
            p.log.WriteLine("Tested by DataBinding automation");
            return ScenarioResult.Pass;
        }

        protected ScenarioResult ResetBindings(TParams p)
        {
            p.log.WriteLine("Tested by DataBinding automation");
            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult get_BindingContext(TParams p)
        {
            p.log.WriteLine("Tested by DataBinding automation");
            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult set_BindingContext(TParams p, BindingContext value)
        {
            p.log.WriteLine("Tested by DataBinding automation");
            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult Invalidate(TParams p, Region region, bool invalidateChildren)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            if (_c is Form)
                ((Form)_c).TopLevel = false;

            region = p.ru.GetRegion(_c.Size);
            invalidateChildren = p.ru.GetBoolean();
            _c.Invalidate(region, invalidateChildren);
            Application.DoEvents();
            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult Invalidate(TParams p, Region region)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            if (_c is Form)
                ((Form)_c).TopLevel = false;

            region = p.ru.GetRegion(_c.Size);
            _c.Invalidate(region);
            Application.DoEvents();
            return ScenarioResult.Pass;
        }

        [OverrideScenario]
        protected override ScenarioResult Dispose(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            p.log.WriteLine("Control.IsDisposed before calling Control.Dispose is " + _c.IsDisposed.ToString());
            p.log.WriteLine("Control.Disposing before calling Control.Dispose is " + _c.Disposing.ToString());

            bool result;

            try
            {
                _c.Dispose();
                p.log.WriteLine("Control.Disposing after calling Control.Dispose is " + _c.Disposing.ToString());
                p.log.WriteLine("Control.IsDisposed after calling Control.Dispose is " + _c.IsDisposed.ToString());
                result = _c.IsDisposed;
            }
            catch (Exception e)
            {
                p.log.WriteLine("Exception thrown: " + e.ToString());
                result = false;
            }

            RecreateControl(p);
            return new ScenarioResult(result);
        }

        protected virtual void RecreateControl(TParams p)
        {
            // since we ----d the Control by calling Dispose, we should recreate it.
            // we can't guarantee that this method is the last one called.
            p.target = (Control)this.CreateObject(p);
            if (_c is Form)
                ((Form)_c).TopLevel = false;

            this.AddObjectToForm(p);
            Application.DoEvents();
        }

        // Special case methods to handle Docked, Anchored, and AutoSize controls
        protected bool PreservesHeight(Control c, TParams p)
        {
            DockStyle dock = c.Dock;

            if ((c is Label) && ((Label)c).AutoSize)
            {
                p.log.WriteLine("AutoSize = true");
                return true;
            }

			if ((c is TextBox) && (!((TextBox)c).Multiline) && ((TextBox)c).AutoSize)
			{
				p.log.WriteLine("AutoSize = true");
				return true;
			}

            if ((c is RichTextBox) && (!((RichTextBox)c).Multiline) && ((RichTextBox)c).AutoSize)
            {
                p.log.WriteLine("AutoSize = true");
                return true;
            }

            if ((c is PictureBox) && ((PictureBox)c).SizeMode == PictureBoxSizeMode.AutoSize)
            {
                p.log.WriteLine("AutoSize = true");
                return true;
            }

            if ((c is TrackBar) && ((TrackBar)c).Orientation == Orientation.Horizontal && ((TrackBar)c).AutoSize)
            {
                p.log.WriteLine("Orientation = Horizontal");
                p.log.WriteLine("AutoSize = true");
                return true;
            }

            if (c is UpDownBase)
                return true;

            if (c is DateTimePicker)
                return true;

            if (c is ComboBox && ((ComboBox)c).DropDownStyle != ComboBoxStyle.Simple)
                return true;

            if (PreHandleMode && c is Splitter && (dock == DockStyle.Left || dock == DockStyle.Right || dock == DockStyle.Fill))
                return true;

            // no AutoSize property or AutoSize == false 
            if ((dock == DockStyle.Left || dock == DockStyle.Right || dock == DockStyle.Fill) && c.Visible)
                return true;

            if (c is ToolStrip && (((ToolStrip)c).AutoSize || (((ToolStrip)c).Dock == DockStyle.Left || ((ToolStrip)c).Dock == DockStyle.Fill || ((ToolStrip)c).Dock == DockStyle.Right)))
            { return true; }
            // DockStyle Top and Bottom
            return false;
        }

        protected bool PreservesWidth(Control c, TParams p)
        {
            DockStyle dock = c.Dock;

            if ((c is Label) && ((Label)c).AutoSize)
            {
                p.log.WriteLine("AutoSize = true");
                return true;
            }

            if ((c is PictureBox) && ((PictureBox)c).SizeMode == PictureBoxSizeMode.AutoSize)
            {
                p.log.WriteLine("AutoSize = true");
                return true;
            }

            if ((c is TrackBar) && ((TrackBar)c).Orientation == Orientation.Vertical && ((TrackBar)c).AutoSize)
            {
                p.log.WriteLine("Orientation = Vertical");
                p.log.WriteLine("AutoSize = true");
                return true;
            }

            if (PreHandleMode && ((c is Splitter) || (c is ToolStrip)) && (dock == DockStyle.Top || dock == DockStyle.Bottom || dock == DockStyle.Fill))
                return true;

            // no AutoSize property or AutoSize == false  
            if ((dock == DockStyle.Top || dock == DockStyle.Bottom || dock == DockStyle.Fill) && c.Visible)
                return true;

            if (c is ToolStrip && (((ToolStrip)c).AutoSize || (((ToolStrip)c).Dock == DockStyle.Top || ((ToolStrip)c).Dock == DockStyle.Fill || ((ToolStrip)c).Dock == DockStyle.Bottom)))
            { return true; }

            // DockStyle Left and Right
            return false;
        }

        protected int GetExpectedX(Control c, Rectangle orig, Rectangle expected)
        {
            DockStyle ds = c.Dock;

            if (ds == DockStyle.Fill || ds == DockStyle.Left || ds == DockStyle.Top || ds == DockStyle.Bottom)
                return orig.X;

            if (ds == DockStyle.Right)
                return orig.X + (orig.Width - expected.Width);

            // Not Docked
            return expected.X;
        }

        protected int GetExpectedY(Control c, Rectangle orig, Rectangle expected)
        {
            DockStyle ds = c.Dock;

            if (ds == DockStyle.Fill || ds == DockStyle.Top || ds == DockStyle.Left || ds == DockStyle.Right)
                return orig.Y;

            if (ds == DockStyle.Bottom)
                return orig.Y + (orig.Height - expected.Height);

            // Not Docked
            return expected.Y;
        }

        // NOTE:  SetSizeHelper() IS NOW OBSOLETE!!  Need to move this code to get_Size() and fix
        //        any tests that break.
        // Use this enum to determine which method/property SetSizeHelper tests.
        protected enum SizeMethod { SetSizeMethod, SizeProperty }

        // SetSizeHelper contains code to test both the SetSize methods.  Specify which
        // method to test using the SizeMethod enum.  The ScenarioResult is returned in the
        // "result" out parameter.
        protected virtual void SetSizeHelper(TParams p, SizeMethod which, out ScenarioResult result)
        {
            if ((_c = GetControl(p)) == null)
            {
                result = ScenarioResult.Fail;
                return;
            }

            Control parent = SafeMethods.GetParent(_c);

            if (parent == null)
                p.log.WriteLine("Parent is null.");
            else
                p.log.WriteLine("Parent's Size: " + parent.Size.ToString());

            DockStyle initDS = DockStyle.None;

            if (_c is CheckedListBox)
            {
                p.log.WriteLine("-- testing CheckedListBox --");
                initDS = ((CheckedListBox)_c).Dock;
                ((CheckedListBox)_c).Dock = DockStyle.None;
                p.log.WriteLine("number of Items: " + ((CheckedListBox)_c).Items.Count);
                p.log.WriteLine("IntegralHeight: " + ((CheckedListBox)_c).IntegralHeight);
                p.log.WriteLine("ItemHeight: " + ((CheckedListBox)_c).ItemHeight);
            }

            p.log.WriteLine("current DockStyle: " + ((Enum)_c.Dock).ToString());

            Size origSize = _c.Size;

            Size maxSize = this.Size;
            if (_c is ToolStripDropDown | _c is Form)
                maxSize = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size;

            Size newSize = p.ru.GetSize(maxSize);

            if (_c is ComboBox && newSize.Width == 0)
                newSize.Width = 1;
            if (_c is Splitter)
            {
                if (newSize.Width < 3)
                    newSize.Width = 3;
                if (newSize.Height < 3)
                    newSize.Height = 3;
            }

            p.log.WriteLine("Size was " + origSize.ToString());
            p.log.WriteLine("Setting Size to " + newSize.ToString());
            if (which == SizeMethod.SetSizeMethod)
                _c.Size = new Size(newSize.Width, newSize.Height);
            else
                _c.Size = newSize;

            Size expectedSize = newSize;

            if (PreservesWidth(_c, p))
            {
                expectedSize.Width = origSize.Width;
                p.log.WriteLine("Preserves width at " + expectedSize.Width);
            }

            if (PreservesHeight(_c, p))
            {
                expectedSize.Height = origSize.Height;
                p.log.WriteLine("Preserves height at " + expectedSize.Height);
            }
            else if (_c is ListBox)
                expectedSize.Height = listBoxExpectedHeight((ListBox)_c, origSize.Height, newSize.Height, p);

            if (_c is SplitContainer)
            {
                SplitContainer sc = (SplitContainer)_c;
                int wd = 0;
                if (sc.BorderStyle == BorderStyle.FixedSingle)
                    wd = SystemInformation.BorderSize.Width;
                else if (sc.BorderStyle == BorderStyle.Fixed3D)
                    wd = SystemInformation.Border3DSize.Width;

                wd = sc.Panel1MinSize + sc.Panel2MinSize + sc.SplitterWidth + wd;
                if (expectedSize.Width < wd && (sc.Orientation == Orientation.Vertical))
                    expectedSize.Width = wd;
                if (expectedSize.Height < wd && (sc.Orientation == Orientation.Horizontal))
                    expectedSize.Height = wd;
            }

            Size sz = _c.Size;

            p.log.WriteLine("Size is " + sz.ToString());
            p.log.WriteLine("expected Size is " + expectedSize.ToString());
            if (_c is CheckedListBox)
            {
                ((CheckedListBox)_c).Dock = initDS;
            }

            result = new ScenarioResult(sz.Equals(expectedSize));
        }

        //
        //  when ListBox has 1)DrawMode.Normal or DrawMode.OwnerDrawFixed, 2)IntegralHeight = true,
        //  3)Items.Count > 0
        //  Height of ListBox is adjusted to show only fully displayed items
        //
        protected int listBoxExpectedHeight(ListBox lb, int origHeight, int newHeight, TParams p)
        {
            int expectedHeight = newHeight;

            p.log.WriteLine("current BorderStyle = " + lb.BorderStyle.ToString());
            if (PreHandleMode)
                return expectedHeight;

            // when IntegralHeight = true Height of ListBox is adjusted to show only full items
            if (lb.DrawMode != DrawMode.OwnerDrawVariable && lb.IntegralHeight && lb.Items.Count > 0)
            {
                p.log.WriteLine("DrawMode: " + lb.DrawMode.ToString());
                p.log.WriteLine("IntegralHeight: " + lb.IntegralHeight);
                p.log.WriteLine("Height was " + origHeight);

                // calculate expected Height
                // GetItemHeight(index) returns real Height of items
                // all items have the same Height in iven DrawModes
                int itmHght = lb.GetItemHeight(0);    // real Item Height
                int delta = origHeight - itmHght * (int)(origHeight / itmHght);

                p.log.WriteLine("GetItemHeight(): " + itmHght);
                p.log.WriteLine("delta = " + delta);

                // when new Height < ItemHeight * n + delta, resulting Height will be set to delta
                if (newHeight < (itmHght * (int)(newHeight / itmHght) + delta))
                    expectedHeight = newHeight - itmHght;

                expectedHeight = itmHght * (int)(expectedHeight / itmHght) + delta;
                if (expectedHeight < 0 && newHeight < delta)
                {
                    if (newHeight == 0)
                        expectedHeight = delta;
                    else
                        expectedHeight = newHeight;
                }
                // not true any more               
                //    if ((expectedHeight == 0) && (delta == 0))
                //          expectedHeight = itmHght;
            }

            p.log.WriteLine("expected Height: " + expectedHeight);
            return expectedHeight;
        }

        protected virtual ScenarioResult get_Size(TParams p)
        {
            ScenarioResult result;

            SetSizeHelper(p, SizeMethod.SizeProperty, out result);
            return result;
        }

        protected virtual ScenarioResult set_Size(TParams p, Size value)
        {
            return get_Size(p);
        }

        protected virtual ScenarioResult set_Width(TParams p, int value)
        {
            return get_Width(p);
        }

        protected virtual ScenarioResult get_Width(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            p.log.WriteLine("current DockStyle: " + ((Enum)_c.Dock).ToString());

            int oldWidth = _c.Width;
            int newWidth = p.ru.GetRange(0, _c.Size.Width * 3); // set upper bound of 3 x current width
            if (_c is Splitter)
            {
                if (newWidth < 3)
                    newWidth = 3;
            }

            p.log.WriteLine("initial Width is " + oldWidth.ToString());
            p.log.WriteLine("setting width to " + newWidth.ToString());
            _c.Width = newWidth;

            int w = _c.Width;
            int wExpected = newWidth;

            p.log.WriteLine("new Width is " + w.ToString());
            if (PreservesWidth(_c, p))
                wExpected = oldWidth;

            // Width of the Vertical SplitContainer is limited by the value below
            if (_c is SplitContainer && ((SplitContainer)_c).Orientation == Orientation.Vertical)
            {
                SplitContainer sc = (SplitContainer)_c;
                int wd = 0;
                if (sc.BorderStyle == BorderStyle.FixedSingle)
                    wd = SystemInformation.BorderSize.Width;
                else if (sc.BorderStyle == BorderStyle.Fixed3D)
                    wd = SystemInformation.Border3DSize.Width;

                wd = sc.Panel1MinSize + sc.Panel2MinSize + sc.SplitterWidth + wd;
                if (wExpected < wd)
                    wExpected = wd;
            }

            p.log.WriteLine("expected Width: " + wExpected.ToString());
            return new ScenarioResult(w == wExpected, "failed to set/get Width as expected", p.log);
        }

        //
        // Used with BoundsHelper to determine which bounds method or property to use in
        // the test.
        //
        protected enum BoundsMethod
        {
            Property,                   // Bounds property
            MethodNoSpecified,          // SetBounds(int, int, int, int)
            MethodSpecified             // SetBounds(int, int, int, int, BoundsSpecified)
        }

        //
        // Helper method to test all three Bounds methods/property.
        //
        protected virtual void BoundsHelper(TParams p, BoundsMethod which, out ScenarioResult result)
        {
            if ((_c = GetControl(p)) == null)
            {
                result = ScenarioResult.Fail;
                return;
            }

            p.log.WriteLine("current DockStyle: " + ((Enum)_c.Dock).ToString());

            Rectangle origRect = _c.Bounds;
            Rectangle newRect = p.ru.GetIntersectingRectangle(this.ClientRectangle);

            //BoundsSpecified bs = 0;
            BoundsSpecified bs = BoundsSpecified.All;

            p.log.WriteLine("initial Bounds: " + origRect.ToString());
            p.log.WriteLine("Setting Bounds to " + newRect.ToString());
            switch (which)
            {
                case BoundsMethod.Property:
                    _c.Bounds = newRect;
                    break;

                case BoundsMethod.MethodNoSpecified:
                    _c.SetBounds(newRect.X, newRect.Y, newRect.Width, newRect.Height);
                    break;

                case BoundsMethod.MethodSpecified:
                    bs = (BoundsSpecified)p.ru.GetEnumValue(typeof(BoundsSpecified));
                    p.log.WriteLine("SetBounds parameters : " + newRect.ToString() + ", " + bs.ToString());
                    _c.SetBounds(newRect.X, newRect.Y, newRect.Width, newRect.Height, bs);
                    break;

                default:
                    throw new ArgumentException("Invalid BoundsMethod: " + which.ToString());
            }

            // Adjust expected Rect to account for dock, autosize, bounds specified, etc.
            Rectangle bounds = _c.Bounds;
            Rectangle rExpected;

            p.log.WriteLine("new Bounds: " + bounds.ToString());
            if (which != BoundsMethod.MethodSpecified)
                rExpected = newRect;    // start with the new rect
            else
            {
                rExpected = origRect;   // start with orig rect, and add specified bounds
                if ((bs & BoundsSpecified.X) == BoundsSpecified.X)
                    rExpected.X = newRect.X;

                if ((bs & BoundsSpecified.Y) == BoundsSpecified.Y)
                    rExpected.Y = newRect.Y;

                if ((bs & BoundsSpecified.Width) == BoundsSpecified.Width)
                    rExpected.Width = newRect.Width;

                if ((bs & BoundsSpecified.Height) == BoundsSpecified.Height)
                    rExpected.Height = newRect.Height;
            }

            if (_c is Splitter)
            {
                if (rExpected.Width == 0) rExpected.Width = 3;

                if (rExpected.Height == 0) rExpected.Height = 3;
            }

            if (_c is SplitContainer)
            {
                SplitContainer sc = ((SplitContainer)_c);
                int borderSize = 0;
                switch (sc.BorderStyle)
                {
                    case BorderStyle.FixedSingle:
                        borderSize = SystemInformation.BorderSize.Width;
                        break;
                    case BorderStyle.Fixed3D:
                        borderSize = SystemInformation.Border3DSize.Width;
                        break;
                }
                int minAllowed = sc.Panel1MinSize + sc.Panel2MinSize +
                    sc.SplitterWidth + borderSize;
                if ((sc.Orientation == Orientation.Vertical) && (rExpected.Width < minAllowed))
                    rExpected.Width = minAllowed;
                else if ((sc.Orientation == Orientation.Horizontal) && (rExpected.Height < minAllowed))
                    rExpected.Height = minAllowed;
            }

            if (PreservesHeight(_c, p))
                rExpected.Height = origRect.Height;
            else if (_c is ListBox && (bs & BoundsSpecified.Height) == BoundsSpecified.Height)
                rExpected.Height = listBoxExpectedHeight((ListBox)_c, origRect.Height, newRect.Height, p);

            if (PreservesWidth(_c, p))
                rExpected.Width = origRect.Width;

            // Height of a ComboBox with DropDownStyle != ComboBoxStyle.Simple
            // will be set depending of Font.Height before handle is created 
            if (PreHandleMode && _c is ComboBox && (((ComboBox)_c).DropDownStyle != ComboBoxStyle.Simple))
            {
                //  rExpected.Height = (int)c.Font.Height + SystemInformation.BorderSize.Height * 4 + 3;
                p.log.WriteLine("... testing ComboBox in prehandle mode...");
                p.log.WriteLine(" ComboBox.PreferredHeight = " + ((ComboBox)_c).PreferredHeight);
                if (_c.IsHandleCreated)		 // preserves orig Height for DropDownStyle other than Simple
                    rExpected.Height = origRect.Height;
                else
                    rExpected.Height = ((ComboBox)_c).PreferredHeight;

                p.log.WriteLine("ComboBox.IsHandleCreated: " + _c.IsHandleCreated);
            }

            // correct Location
            rExpected.X = GetExpectedX(_c, origRect, rExpected);
            rExpected.Y = GetExpectedY(_c, origRect, rExpected);
            p.log.WriteLine("expected Bounds: " + rExpected.ToString());
            result = new ScenarioResult(bounds.Equals(rExpected));
        }

        protected virtual ScenarioResult set_Bounds(TParams p, Rectangle value)
        {
            return get_Bounds(p);
        }

        protected virtual ScenarioResult get_Bounds(TParams p)
        {
            ScenarioResult result;

            BoundsHelper(p, BoundsMethod.Property, out result);
            return result;
        }

        protected virtual ScenarioResult SetBounds(TParams p, Int32 x, Int32 y, Int32 width, Int32 height)
        {
            ScenarioResult result;

            BoundsHelper(p, BoundsMethod.MethodNoSpecified, out result);
            return result;
        }

        protected virtual ScenarioResult SetBounds(TParams p, Int32 x, Int32 y, Int32 width, Int32 height, BoundsSpecified specified)
        {
            ScenarioResult result;

            BoundsHelper(p, BoundsMethod.MethodSpecified, out result);
            return result;
        }

        protected virtual ScenarioResult set_Height(TParams p, int value)
        {
            return get_Height(p);
        }

        protected virtual ScenarioResult get_Height(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            int origHeight = _c.Height;
            //int newHeight = p.ru.GetRange(1, 1024);
            int newHeight = p.ru.GetRange(1, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);

            p.log.WriteLine("initial Height is " + origHeight.ToString());
            p.log.WriteLine("Setting Height to " + newHeight.ToString());
            _c.Height = newHeight;

            int height = _c.Height;
            int hExpected = newHeight;

            p.log.WriteLine(" new Height is " + height.ToString());
            if (PreservesHeight(_c, p))
                hExpected = origHeight;
            else if (_c is ListBox)
                hExpected = listBoxExpectedHeight((ListBox)_c, origHeight, newHeight, p);

            // Height of the Horizontal SplitContainer is limited by the value below
            if (_c is SplitContainer && ((SplitContainer)_c).Orientation == Orientation.Horizontal)
            {
                SplitContainer sc = (SplitContainer)_c;
                int ht = 0;
                if (sc.BorderStyle == BorderStyle.FixedSingle)
                    ht = SystemInformation.BorderSize.Height;
                else if (sc.BorderStyle == BorderStyle.Fixed3D)
                    ht = SystemInformation.Border3DSize.Height;

                ht = sc.Panel1MinSize + sc.Panel2MinSize + sc.SplitterWidth + ht;
                if (hExpected < ht)
                    hExpected = ht;
            }
            p.log.WriteLine("expected Height: " + hExpected.ToString());
            return new ScenarioResult(height == hExpected, "failed set/get Height as expected", p.log);
        }

        // Use this enum to determine which method/property ScaleHelper tests.
        protected enum ScaleMethod { OneParam, TwoParam, Struct }

        // ScaleHelper contains code to test both the Scale methods.  Specify which
        // method to test using the ScaleMethod enum.  The ScenarioResult is returned in the
        // "result" out parameter.

#pragma warning disable 618 // 618 = Obsolete
        protected virtual void ScaleHelper(TParams p, ScaleMethod which, out ScenarioResult result)
        {
            if ((_c = GetControl(p)) == null)
            {
                result = ScenarioResult.Fail;
                return;
            }

            result = new ScenarioResult();
            p.log.WriteLine("current DockStyle: " + ((Enum)_c.Dock).ToString());

            _c.Scale(0.5f);
            _c.Scale(2.0f);

            // Set up scale values based on which method we're testing
            float xScale;
            float yScale;

            if (which == ScaleMethod.OneParam)
            {
                xScale = 0.5f;
                yScale = xScale;
            }
            else
            {
                xScale = 0.5f;
                yScale = 4.0f;

                // Max height appears to be 29632, so we only allow a height of 1/4 this so that we can test it
                if (_c.Height > 7408) _c.Height = 7408;
            }

            // Set up variables and call the appropriate Scale() method
            Size origSize = _c.Size;
            Size expectedSize = new Size((int)(origSize.Width * xScale), (int)(origSize.Height * yScale));
            Size newSize;
            Point origLocation = _c.Location;
            Point expectedLocation = new Point((int)(origLocation.X * xScale), (int)(origLocation.Y * yScale));
            Point newLocation;

            p.log.WriteLine("initial Size was " + origSize.ToString());
            p.log.WriteLine("initial Location was " + origLocation.ToString());
            if (which == ScaleMethod.OneParam)
                _c.Scale(xScale);
            else if (which == ScaleMethod.TwoParam)
                _c.Scale(xScale, yScale);
            else
            {
                SizeF newScale = new SizeF(xScale, yScale);
                _c.Scale(newScale);
            }

            newSize = _c.Size;
            newLocation = _c.Location;

            // Calculate expected size and location
            if (PreservesHeight(_c, p))
                expectedSize.Height = origSize.Height;
            else if (_c is ListBox)
                expectedSize.Height = listBoxExpectedHeight((ListBox)_c, origSize.Height, newSize.Height, p);

            if (PreservesWidth(_c, p))
                expectedSize.Width = origSize.Width;

            expectedLocation.X = GetExpectedX(_c, new Rectangle(origLocation, origSize), new Rectangle(newLocation, newSize));
            expectedLocation.Y = GetExpectedY(_c, new Rectangle(origLocation, origSize), new Rectangle(newLocation, newSize));

            // Check results
            if (which == ScaleMethod.OneParam)
            {
                p.log.WriteLine("after Scale({0}) Size is {1}", xScale, newSize.ToString());
                p.log.WriteLine("after Scale({0}) Location is {1}", xScale, newLocation.ToString());
            }
            else
            {
                p.log.WriteLine("after Scale({0}, {1}) Size is {2}", xScale, yScale, newSize.ToString());
                p.log.WriteLine("after Scale({0}, {1}) Location is {2}", xScale, yScale, newLocation.ToString());
            }

            result.IncCounters(newSize.Equals(expectedSize), "1. Size: " + newSize + " != " + expectedSize, p.log);
            result.IncCounters(newLocation.Equals(expectedLocation), "2. Loc: " + newLocation + " != " + expectedLocation, p.log);

            // Rescale to original size and check results
            if (which == ScaleMethod.OneParam)
                _c.Scale(1.0f / xScale);
            else if (which == ScaleMethod.TwoParam)
                _c.Scale(1.0f / xScale, 1.0f / yScale);
            else
            {
                SizeF oldScale = new SizeF(1 / xScale, 1 / yScale);
                _c.Scale(oldScale);
            }

            newSize = _c.Size;
            newLocation = _c.Location;
            p.log.WriteLine("recreated Size is " + newSize.ToString());
            p.log.WriteLine("recreated Location is " + newLocation.ToString());

            bool res = origSize.Equals(newSize) || (Math.Abs(origSize.Width - newSize.Width) <= 1 && Math.Abs(origSize.Height - newSize.Height) <= 1);

            if (!res && (origSize.Height + origLocation.Y < 0 || origSize.Width + origLocation.X < 0))
                result.IncCounters(false, p.log, BugDb.ASURT, 55, "3. Size: " + newSize + " != " + origSize);
            else
                result.IncCounters(res, "3. Size: " + newSize + " != " + origSize, p.log);

            res = origLocation.Equals(newLocation) || (Math.Abs(origLocation.X - newLocation.X) <= 1 && Math.Abs(origLocation.Y - newLocation.Y) <= 1);
            result.IncCounters(res, "4. Loc: " + newLocation + " != " + origLocation, p.log);

            // in ListBox this small impercision results in greater difference in Height
            // when IntegralHeight = true
            if (origSize != newSize)
            {
                if (_c is ListBox && ((ListBox)_c).IntegralHeight == true)
                {
                    int temp = Math.Abs(origSize.Height - newSize.Height);
                    int temp1 = ((ListBox)_c).GetItemHeight(0);

                    if (Math.IEEERemainder(temp, temp1) == 0)
                    {
                        p.log.WriteLine("Height differs because of round-off imprecision");
                        result = ScenarioResult.Pass;
                    }
                }
            }

            // in ComboBox this small imprecision results in greater difference in Height
            // when IntegralHeight = true
            if (origSize != newSize)
            {
                if (_c is ComboBox && ((ComboBox)_c).IntegralHeight == true)
                {
                    int temp = Math.Abs(origSize.Height - newSize.Height);
                    int temp1 = ((ComboBox)_c).GetItemHeight(0);

                    if (Math.IEEERemainder(temp, temp1) == 0)
                    {
                        p.log.WriteLine("Height differs because of round-off imprecision");
                        result = ScenarioResult.Pass;
                    }
                }
            }
        }

        protected virtual ScenarioResult Scale(TParams p, SizeF factor)
        {
            ScenarioResult result;

            ScaleHelper(p, ScaleMethod.TwoParam, out result);
            return result;
        }

#pragma warning restore 618 // 618 = Obsolete
        protected virtual ScenarioResult Scale(TParams p, float ratio)
        {
            ScenarioResult result;

            ScaleHelper(p, ScaleMethod.OneParam, out result);
            return result;
        }
#pragma warning restore 618 // 618 = Obsolete
        protected virtual ScenarioResult Scale(TParams p, float dx, float dy)
        {
            ScenarioResult result;

            ScaleHelper(p, ScaleMethod.TwoParam, out result);
            return result;
        }

        //This method will show up as an extra scenario.  It used to be a test for Control.SetNewControls(),
        //which was replaced with the AddRange() method on the Controls collection.  No harm in keeping this
        //test though, since we could use additional collection coverage.
        protected virtual ScenarioResult ControlsAddRange(TParams p, Control[] value)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ScenarioResult sr = new ScenarioResult();
            // Temporarily removing RadioButton - it throws an exception in internet zone: Regression_Bug66
            //Control[] ca = new Control[] { new Button(), new CheckBox(), new RadioButton() };
            Control[] ca = new Control[] { new Button(), new Button(), new Button() };
            int initNumber = _c.Controls.Count;

            p.log.WriteLine("initial number of controls on tested control: " + initNumber);
            p.log.WriteLine("adding " + ca.Length.ToString() + " controls by Controls.AddRange...");
            _c.Controls.AddRange(ca);
            if (_c.Controls.Count != ca.Length + initNumber)
            {
                sr.IncCounters(false, "FAILED: added " + (_c.Controls.Count - initNumber).ToString() + " controls instead of " + initNumber, p.log);
                return sr;
            }

            for (int i = 0; i < ca.Length; i++)
            {
                Application.DoEvents();
                try
                {
                    int index = _c.Controls.GetChildIndex(ca[i]);

                    Application.DoEvents();
                    _c.Controls.Remove(ca[i]);
                    Application.DoEvents();
                    p.log.WriteLine(i + "-control was found among newly added controls");
                    sr.IncCounters(true);
                }
                catch (ArgumentException)
                {
                    sr.IncCounters(false, "FAILED: " + ca[i].ToString() + " - (" + i + ")-control was not found among newly added", p.log);
                }
                catch (Exception e)
                {
                    sr.IncCounters(false, "FAILED: when tried to find " + ca[i].ToString() + " - (" + i + ")-control among just added wrong exception was thrown: " + e.Message, p.log);
                }
            }

            sr.IncCounters(_c.Controls.Count == initNumber, "FAILED: to Remove just added controls", p.log);
            return sr;
        }

        protected virtual ScenarioResult ResetImeMode(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            _c.ImeMode = (ImeMode)p.ru.GetDifferentEnumValue(typeof(ImeMode), (int)ImeMode.Inherit);
            _c.ResetImeMode();

            ImeMode expectedResult = ExpectedReturnedImeDefault(_c);

            return new ScenarioResult(_c.ImeMode == expectedResult, "ImeMode: " + _c.ImeMode + "  Expected result: " + expectedResult, p.log);
        }

        protected virtual ScenarioResult set_Name(TParams p, string s)
        {
            return get_Name(p);
        }

        protected virtual ScenarioResult get_Name(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            string newName = p.ru.GetString(p.ru.GetRange(0, 1000));

            _c.Name = newName;
            return new ScenarioResult(_c.Name.Equals(newName), "Name set to:\n" + newName + "\n\nName is:\n" + _c.Name);
        }

        protected virtual ScenarioResult set_Tag(TParams p, object o)
        {
            return get_Tag(p);
        }

        protected virtual ScenarioResult get_Tag(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            object newTag = new Object();

            _c.Tag = newTag;
            return new ScenarioResult(_c.Tag == newTag);
        }

        // Returns the actual default ImeMode of the given control.  Contrast with
        // ExpectedRetrunedImeDefault(), which gives you what value the control would actually
        // return if it was set to its default.
        //
        private ImeMode ExpectedActualImeDefault(Control c)
        {
            if (c is ButtonBase || c is Label || c is MonthCalendar || c is PictureBox || c is ProgressBar || c is ScrollBar || c is Splitter || c is TrackBar)
                return ImeMode.Disable;
            else
                return ImeMode.Inherit;
        }

        // Returns what the given control should return if its ImeMode property were in its
        // default state.  This may differ from the actual default, e.g. in the case of ImeMode.Inherit.
        //
        private ImeMode ExpectedReturnedImeDefault(Control c)
        {
            return GetExpectedImeMode(c, ExpectedActualImeDefault(c));
        }

        //
        // Returns the expected ImeMode value given the value set to the control's property,
        //
        protected ImeMode GetExpectedImeMode(Control c, ImeMode setValue)
        {
            if (setValue != ImeMode.Inherit)
                return setValue;
            else
            {
                // No need to travel up the parent chain since this control will inherit directly
                // from its parent.
                if (SafeMethods.GetParent(c) != null)
                    return SafeMethods.GetParent(c).ImeMode;
                else
                    return ImeMode.NoControl;
            }
        }

        // Whidbey features
        protected virtual ScenarioResult set_AutoSize(TParams p, bool value)
        {
            return get_AutoSize(p);
        }

        protected virtual ScenarioResult get_AutoSize(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            // we may need to remember initial AutoSize and then restore it after the test
            // in order to avoid impact on other scenarios
            bool orig = _c.AutoSize;
            bool newAutoSize = p.ru.GetBoolean();

            p.log.WriteLine("change AutoSize from {0} to {1}", _c.AutoSize, newAutoSize);
            _c.AutoSize = newAutoSize;
            p.log.WriteLine("new AutoSize = " + _c.AutoSize);
            bool result = _c.AutoSize == newAutoSize;

            _c.AutoSize = orig;
            return new ScenarioResult(result, "Failed: to set/get AutoSize", p.log);
        }
        protected virtual ScenarioResult set_AutoRelocate(TParams p, bool value)
        {
		return ScenarioResult.Pass;
        }

		protected virtual ScenarioResult get_AutoRelocate(TParams p)
		{
			return ScenarioResult.Pass;
		}
		protected virtual ScenarioResult set_Margin(TParams p, Padding value)
        {
            return get_Margin(p);
        }

        protected virtual ScenarioResult get_Margin(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            // Win32 Exception thrown from ReflectTools.ReflectBase.TestEngine
            // for very big values - will restrict with max
            int max = 512;
            Padding newValue = p.ru.GetPadding(max, max, max, max);

            p.log.WriteLine("change Margin from {0} to {1}", _c.Margin, newValue);
            _c.Margin = newValue;
            p.log.WriteLine("new Margin = " + _c.Margin);
            return new ScenarioResult(_c.Margin == newValue, "Failed: to set/get Margin", p.log);
        }

        protected virtual ScenarioResult set_Padding(TParams p, Padding value)
        {
            return get_Padding(p);
        }

        protected virtual ScenarioResult get_Padding(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ScenarioResult result = new ScenarioResult();

            // Win32 Exception thrown from ReflectTools.ReflectBase.TestEngine
            // for very big values - will restrict with max
            int max = 512;
            Padding newValue = p.ru.GetPadding(max, max, max, max);

            p.log.WriteLine("change Padding from {0} to {1}", _c.Padding, newValue);
            try
            {
                _c.Padding = newValue;
                p.log.WriteLine("new Padding = " + _c.Padding);
                result.IncCounters(_c.Padding == newValue, "Failed: to set/get Padding", p.log);
            }
            catch (OverflowException e)
            {
                p.log.WriteLine("Exception: " + e.ToString());
                result.IncCounters(false, p.log, BugDb.VSWhidbey, 56, "OverflowException for large Padding");
            }
            return result;
        }

        protected virtual ScenarioResult get_PreferredSize(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ScenarioResult result = new ScenarioResult();
            Size initPrefSize = _c.PreferredSize;
            string initText = _c.Text;
            Font initFont = _c.Font;
            Size initSize = _c.Size;

            // increase Font to change PreferredSize
            //int increase = p.ru.GetRange(5, 1024);
            int increase = p.ru.GetRange(5, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 10);

            // If Size of the control changes as a result of the Font.Increase
            // PreferredSize should be also updated
            if (!(_c is DateTimePicker))
                _c.Text = p.ru.GetString(Int32.MaxValue, true);
            // have to limit length of text for Button with UseCompatibleTextRendering = false
            // due to Postponed VSWhidbey #535880
            if ((_c is Button) && (!((Button)_c).UseCompatibleTextRendering) && (_c.Text.Length > 500))
                _c.Text = _c.Text.Substring(0, 500);

            p.log.WriteLine("Get PreferredSize on control with Text.Length = " + _c.Text.Length);
            result.IncCounters(IncreaseControlPreferredSize(_c, increase, p), "Failed: to change Font", p.log);
            p.log.WriteLine("PreferredSize: init/new: {0}/{1} ", initPrefSize, _c.PreferredSize);
            p.log.WriteLine("Size: init/new: {0}/{1} ", initSize, _c.Size);

            // verify PreferredSize increase
            bool pass = (_c.PreferredSize.Width >= initPrefSize.Width) && (_c.PreferredSize.Height > initPrefSize.Height);

            //If you have a ComboBox, it may be bound by PreferredHeight if in one of the DropDown styles.
            ComboBox cb = _c as ComboBox;
            if (cb != null)
            {
                if (cb.DropDownStyle == ComboBoxStyle.DropDown || cb.DropDownStyle == ComboBoxStyle.DropDownList)
                    pass = (_c.PreferredSize.Width >= initPrefSize.Width) && (_c.PreferredSize.Height >= initPrefSize.Height);
            }
            // In some cases Height may remain unchanged if Text is "" (ButtonBase)
            // For controls like Form, GroupBox, Panel, ListBox, PictureBox, etc.
            // Font.Size doesn't affect PreferredSize
            if (((_c is ButtonBase) && (_c.Text == "")) || (_c is Form) || (_c is GroupBox) || (_c is Panel) || (_c is ListView) || (_c is PictureBox) || (_c is ProgressBar) || (_c is PropertyGrid) || (_c is Splitter) || (_c is TabControl) || (_c is TrackBar) || (_c is TreeView) || (_c is PrintPreviewControl) || (_c is ScrollBar) || (_c is PropertyGrid))
                    pass = (_c.PreferredSize.Width >= initPrefSize.Width) && (_c.PreferredSize.Height == initPrefSize.Height);
            
            // For Button control with AutoSizeMode = GrowOnly Height may remain unchanged even for non-empty text
            // - In case if initial Height already exceeded Height required to accomodate Text string.
            if ((_c is Button) && (_c.Text.Length > 0) && (((Button)_c).AutoSizeMode == AutoSizeMode.GrowOnly)) 
                pass = (_c.PreferredSize.Width >= initPrefSize.Width) && (_c.PreferredSize.Height >= initPrefSize.Height);

            // For ContainerControls like Form, PropertyBrowser.
            // Font.Size affects PreferredSize if AutoScaleMode = Font
            if ((_c is ContainerControl) && (((ContainerControl)_c).AutoScaleMode == AutoScaleMode.Font))
                pass = (_c.PreferredSize.Width >= initPrefSize.Width) && (_c.PreferredSize.Height >= initPrefSize.Height);

            // For TextBoxBase-derived controls only onde dimension of the PreferredSize can increase with Font increase
            // - especially for large fonts
            if (_c is TextBoxBase && (Marshal.SizeOf(typeof(IntPtr)) * 8 == 64))
                pass = (_c.PreferredSize.Width >= initPrefSize.Width) || (_c.PreferredSize.Height >= initPrefSize.Height);

            //Note much you can guarantee for DataGridView -- it would even be possible to have Width and Height
            //both _decrease_ if you have funky autosizing going on
            if (_c is DataGridView)
                pass = true;

            // For ListBox PreferredSize remains unchanged for OwnerDraw* modes and PreHandleMode
            if ((_c is ListBox) && ((((ListBox)_c).DrawMode != DrawMode.Normal) || PreHandleMode))
                pass = (_c.PreferredSize.Width == initPrefSize.Width) && (_c.PreferredSize.Height == initPrefSize.Height);

            // log several known bugs
            if (_c is ListBox && PreHandleMode)
                result.IncCounters(pass, p.log, BugDb.VSWhidbey, 57, "Unexpected PreferredSize prior handle creation");
            else if (_c is RichTextBox && PreHandleMode)
                result.IncCounters(pass, p.log, BugDb.VSWhidbey, 58, "RTB returns default PreferredSize prior handle creation");
            else if (_c is CheckedListBox && !pass && _c.PreferredSize.Height == initPrefSize.Height)
                result.IncCounters(true, p.log, BugDb.VSWhidbey, 59, "CLB: Won't Fix bug: PreferredSize always returns default Height");
            else if (_c is SplitContainer && !pass && ((SplitContainer)_c).AutoSize)
                result.IncCounters(pass, p.log, BugDb.VSWhidbey, 60, "SC: PreferredSize changes with Font increase when AutoSize = true");
            else
                result.IncCounters(pass, "Failed: to adjust PreferredSize for control with Text.Length = " + _c.Text.Length, p.log);

            if ((_c is ListBox) && (_c.Text == "") && (initSize.Width != _c.Size.Width || initSize.Height != _c.Size.Height) && (initPrefSize.Height == _c.PreferredSize.Height))
                result.IncCounters(false, p.log, BugDb.VSWhidbey, 148270, "PreferredSize didn't change when actual Size has changed");

            if ((_c is ListBox) && !pass)
            {
                p.log.WriteLine("Items.Count = " + ((ListBox)_c).Items.Count);
                p.log.WriteLine("SelectedIndex: " + ((ListBox)_c).SelectedIndex);
                p.log.WriteLine("DrawMode: " + ((ListBox)_c).DrawMode);
            }

            // restore initial conditions
            if (!(_c is DateTimePicker))
                _c.Text = initText;
            _c.Font = initFont;
            _c.Size = initSize;

            //p.log.WriteLine("PrefSize after restore: " + c.PreferredSize);
            return result;
        }

        protected virtual bool IncreaseControlPreferredSize(Control cc, int delta, TParams pp)
        {
            float initFontSize = cc.Font.Size;
            Font newFont = new Font(cc.Font.FontFamily, cc.Font.Size + delta, cc.Font.Style, cc.Font.Unit);
            if (cc is Label && (newFont.Unit == GraphicsUnit.Inch || newFont.SizeInPoints > 100))
                pp.log.WriteLine("test may hang due to VSWhidbey #107035");

            pp.log.WriteLine("new Font: Style: {0}- Unit: {1}- SizeInPoints {2}", newFont.Style, newFont.Unit, newFont.SizeInPoints);
            pp.log.WriteLine("init Font = " + cc.Font);
            pp.log.WriteLine("new Font = " + newFont);
            pp.log.WriteLine("change Font.Size from {0} to {1}", cc.Font.Size, newFont.Size);
            pp.log.WriteLine("change Font.Height from {0} to {1}", cc.Font.Height, newFont.Height);
            cc.Font = newFont;
            Application.DoEvents();

            bool success = (cc.Font.Size == newFont.Size);

            // in some cases Font is not set exactly - we are not testing it here
            // we will be safisfied that the Font.Size was increased
            if (!success)
            {
                pp.log.WriteLine("new Font.Size: " + cc.Font.Size);
                if (cc is RichTextBox)
                    success = (cc.Font.Size > initFontSize);
            }

            return (success);
        }

        //	If proposedSize is (0, 0) it's converted into (Int32.Max, Int32.Max)
        //	So when proposedZise == (0, 0) || (Int32.Max, Int32.Max), default PreferredSize is returned
        //  The logic to calculate GetPreferredSize() is very complicated
        //  So we'll just make sure that GetPreferredSize (proposedSize) returns within
        //  expected boundaries defined by current Control.MaximumSize and MinimumSize.
        //
        protected virtual ScenarioResult GetPreferredSize(TParams p, Size proposedSize)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ScenarioResult result = new ScenarioResult();
            Size initSize = _c.PreferredSize;
            Size initControlSize = _c.Size;

            p.log.WriteLine("default PreferredSize = " + initSize);
            p.log.WriteLine("1. call when (0, 0) is proposed");

            Size retSize = _c.GetPreferredSize(Size.Empty);

            result.IncCounters(retSize.Equals(initSize), "Failed: returned " + retSize + " when proposed (0, 0)", p.log);
            p.log.WriteLine("2. call when (Int32.Max, Int32.Max) is proposed");
            retSize = _c.GetPreferredSize(new Size(Int32.MaxValue, Int32.MaxValue));
            result.IncCounters(retSize.Equals(initSize), "Failed: returned " + retSize + " when proposed (Int32.Max, Int32.Max)", p.log);
            p.log.WriteLine("3. call when exact default PreferredSize is proposed");
            retSize = _c.GetPreferredSize(initSize);
            result.IncCounters(retSize.Equals(initSize), "Failed: returned " + retSize + " when proposed default PreferredSize", p.log);

            // remember MinimumSize and MaximumSize to restore at the end of the scenario
            Size saveMaxSize = _c.MaximumSize;
            Size saveMinSize = _c.MinimumSize;

            p.log.WriteLine("4. call when MaximimSize is smaller than default PreferredSize and (0, 0) is proposed");

            // make sure new MaxSize doesn't contain 0 - 0 means unlimited
            Size newSize = p.ru.GetSize(1, initSize.Width - 1, 1, initSize.Height - 1);

            _c.MaximumSize = newSize;
            if (_c is ComboBox)
                newSize.Height = 0;
            result.IncCounters(_c.MaximumSize.Equals(newSize), "Failed to set MaximumSize", p.log);
            retSize = _c.GetPreferredSize(Size.Empty);

            // if MinimumSize.Width or Height is greater than MaximumSize.Width/Height
            // the MinimumSize will take precedence
            Size expSize = newSize;

            if (_c.MinimumSize.Width > expSize.Width)
                expSize.Width = _c.MinimumSize.Width;

            if (_c.MinimumSize.Height > expSize.Height)
                expSize.Height = _c.MinimumSize.Height;

            if (_c is ComboBox)
                result.IncCounters(retSize.Width <= expSize.Width, "Failed: returned " + retSize + " when MaxSize = " + newSize, p.log);
            else
                result.IncCounters(retSize.Width <= expSize.Width && retSize.Height <= expSize.Height, "Failed: returned " + retSize + " when MaxSize = " + newSize, p.log);
            p.log.WriteLine("5. call when MinimumSize is greater than default PreferredSize and (0, 0) is proposed");
            newSize = p.ru.GetSize(initSize.Width + 1, initSize.Height + 1);
            _c.MaximumSize = Size.Empty; // clear Maximum restriction 
            _c.MinimumSize = newSize;
            if (_c is ComboBox)
                newSize.Height = 0;
            result.IncCounters(_c.MinimumSize.Equals(newSize), "Failed to set MinimumSize", p.log);
            retSize = _c.GetPreferredSize(Size.Empty);
            result.IncCounters(retSize.Width >= newSize.Width && retSize.Height >= newSize.Height, "Failed: returned " + retSize + " when MinSize = " + newSize, p.log);

            // restore MaximumSize and MinimumSize
            _c.MinimumSize = saveMinSize;
            _c.MaximumSize = saveMaxSize;
            _c.Size = initControlSize;
            return result;
        }

        //
        //	This property is interesting for people writing designers
        //  Will require additional coverage
        //  For now will verify that the LeyoutEngine is not null
        //
        protected virtual ScenarioResult get_LayoutEngine(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            object retValue = _c.LayoutEngine;

            // verify that it's not null
            return new ScenarioResult(retValue != null, "Failed: returned value is null", p.log);
        }

        //
        //	Scrolls the contents of the specified windows client area
        // Visual verification is needed
        // For AutoPME wi'll just make sure that we can call the function
        // with random values of its arguments
        //	dx - amount of horizontal scrolling
        //	dy - amount of vertical scrolling
        //		protected virtual ScenarioResult ScrollWindow(TParams p, Int32 dx, Int32 dy)
        //		{
        //			if ((c = GetControl(p)) == null) return ScenarioResult.Fail;
        //
        //			// We're just making sure ScrollWindow doesn't throw an exception.
        //			scrollWindowHelper(p, 0);
        //			return ScenarioResult.Pass;
        //		}

        //	Scrolls the contents of the specified windows client area
        // Visual verification is needed
        // For AutoPME wi'll just make sure that we can call the function
        // with random values of its arguments
        //	dx - amount of horizontal scrolling
        //	dy - amount of vertical scrolling
        //	scrollRect - portion of the client area to be scrolled
        //	clipRect - coordinates of the clipping rectangle. Only device bits within

        // the clipping rectangle are affected. Bits scrolled from the outside of the 
        // rectangle to the inside are painted; but scrolled from inside to the outside are not painted.
        //	rgnUpdate - region that is invalidated by scrolling
        //	enumVal - flags for scrolling (eraze, invalidate, scrollchildren, smoothscroll)
        //		protected virtual ScenarioResult ScrollWindow(TParams p, Int32 dx, Int32 dy, Rectangle scrollRect, Rectangle clipRect, Region rgnUpdate, ScrollType enumVal)
        //		{
        //			if ((c = GetControl(p)) == null) return ScenarioResult.Fail;
        //
        //			// We're just making sure ScrollWindow doesn't throw an exception.
        //			scrollWindowHelper(p, 1);
        //			return ScenarioResult.Pass;
        //		}

        //
        // Helper to test ScrollWindow 
        // 'region' defines which version of the ScrollWindow is called
        //
        //		protected void scrollWindowHelper(TParams p, int nOverLoadNum) {
        //			int dxVal = p.ru.GetRange(int.MinValue, int.MaxValue);
        //			int dyVal = p.ru.GetRange(int.MinValue, int.MaxValue);
        //			Rectangle scrollRectVal = p.ru.GetRectangle();
        //			Rectangle clipRectVal = p.ru.GetRectangle();
        //			Region regionVal = null;
        //
        //			p.log.WriteLine("horizontal scroll amount: " + dxVal);
        //			p.log.WriteLine("vertical scroll amount: " + dyVal);
        //			p.log.WriteLine("area to be scrolled: " + scrollRectVal);
        //			p.log.WriteLine("clip area : " + clipRectVal);
        //
        //			if (p.ru.GetBoolean())
        //			{
        //				regionVal = p.ru.GetRegion(c.Size);
        //				p.log.WriteLine("region : " + regionVal);
        //			}
        //			else
        //				p.log.WriteLine("region is null");
        //
        //			ScrollType flags = (ScrollType)p.ru.GetEnumValue(typeof(System.Windows.Forms.ScrollType), true);
        //
        //			p.log.WriteLine("ScrollType: " + flags);
        //			p.log.WriteLine("call ScrollWindow()");
        //
        //			switch (nOverLoadNum)
        //			{
        //				case 0:
        //					c.ScrollWindow(dxVal, dyVal);
        //					break;
        //				case 1:
        //					c.ScrollWindow(dxVal, dyVal, scrollRectVal, clipRectVal, regionVal, flags);
        //					break;
        //				default:
        //					throw new ArgumentException("nOverLoadNum must be 0 or 1");
        //			}
        //
        //			p.log.WriteLine("just completed the call");
        //		}
        //
        protected virtual ScenarioResult set_MaximumSize(TParams p, Size value)
        {
            return get_MaximumSize(p);
        }

        //	Property name can change - "MaximumSize" name doesn't reflect 
        // actual meaning of the property and is misleading
        protected virtual ScenarioResult get_MaximumSize(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ScenarioResult result = new ScenarioResult();
            Size origMaxSize = _c.MaximumSize;
            Size origControlSize = _c.Size;
            //Size newSize = p.ru.GetSize(Int16.MaxValue / 10, Int16.MaxValue / 10);
            int newWidth = p.ru.GetRange(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 2, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width);
            int newHeight = p.ru.GetRange(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 2, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            Size newSize = new Size(newWidth, newHeight);

            // 1. change MaximumSize
            p.log.WriteLine("1. change MaximumSize from {0} to {1}", _c.MaximumSize, newSize);
            _c.MaximumSize = newSize;
            p.log.WriteLine("new MaximumSize = " + _c.MaximumSize);
            result.IncCounters(_c.MaximumSize == newSize, "FAIL: set/get MaximumSize", p.log);

            // 2. Try modifying size to value larger than max.
            p.log.WriteLine("2. Try modifying size to value larger than max.  Size should return max.");
            Size randSize = p.ru.GetSize(newSize.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, newSize.Height, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            Size origSize = _c.Size;

            // in partial trust, window can not be set off the screen
            Size expected = newSize;
            if (_c is Form)
            {
                if (((Form)_c).IsRestrictedWindow)
                {
                    p.log.WriteLine("IsRestrictedWindow, adjusting expected size...");
                    Rectangle working = Screen.GetWorkingArea(_c);
                    expected.Width = working.Width - _c.Location.X;
                    expected.Height = working.Height - _c.Location.Y;
                    p.log.WriteLine("	new expected (" + expected.Width + ", " + expected.Height + ")");
                }
            }

            p.log.WriteLine("setting Size to " + randSize);
            _c.Size = randSize;

            if (PreservesHeight(_c, p))
                expected.Height = origSize.Height;

            if (PreservesWidth(_c, p))
                expected.Width = origSize.Width;

            //MonthCalendar size depends on how many calendars are completely visible
			if (_c is MonthCalendar)
			{
				p.log.WriteLine(_c.Size.ToString());
				result.IncCounters(((_c.Size.Width <= expected.Width) && (_c.Size.Height <= expected.Height)), "FAIL: Size is greater than MaximumSize", expected.ToString(), _c.Size.ToString(), p.log);
			}
			else if (_c is ListBox)
				expected.Height = listBoxExpectedHeight((ListBox)_c, origSize.Height, newSize.Height, p);
			else
                result.IncCounters(expected, _c.Size, "FAIL: set Size", p.log);

            // restore minsize so as not to break subsequent sizing scenarios.
            _c.MaximumSize = origMaxSize;
            _c.Size = origControlSize;
            return result;
        }

        protected virtual ScenarioResult set_MinimumSize(TParams p, Size value)
        {
            return get_MinimumSize(p);
        }

        //	Property name can change - "MinimumSize" name doesn't reflect 
        // actual meaning of the property and is misleading
        // VSWhidbey 135632: Min and MaximumSize do affect control size now.
        protected virtual ScenarioResult get_MinimumSize(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            // should accept any Size
            ScenarioResult result = new ScenarioResult();
            Application.DoEvents();
            Size origMinSize = _c.MinimumSize;
            Size origControlSize = _c.Size;
            //Size newSize = p.ru.GetSize(Int16.MaxValue / 10, Int16.MaxValue / 10);
            int newWidth = p.ru.GetRange(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 2, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width);
            int newHeight = p.ru.GetRange(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 2, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            Size newSize = new Size(newWidth, newHeight);

            // 1. change MinimumSize
            p.log.WriteLine("1. change MinimumSize from {0} to {1}", _c.MinimumSize, newSize);
            _c.MinimumSize = newSize;
            p.log.WriteLine("new MinimumSize = " + _c.MinimumSize);
            if (_c is ComboBox)
                newSize.Height = 0;
            result.IncCounters(_c.MinimumSize == newSize, "FAIL: set/get MinimumSize", p.log);

            // 2. Try modifying size to value smaller than min.
            p.log.WriteLine("2. Try modifying size to value smaller than min.  Size should return min.");
            Size randSize = p.ru.GetSize(0, newSize.Width, 0, newSize.Height);
            Size origSize = _c.Size;
            Size expected = newSize;
            p.log.WriteLine("setting Size to " + randSize);
            _c.Size = randSize;
            Application.DoEvents();
            if (PreservesHeight(_c, p))
                expected.Height = origSize.Height;

            if (PreservesWidth(_c, p))
                expected.Width = origSize.Width;

            //MonthCalendar size depends on how many calendars are completely visible
            if (_c is MonthCalendar)
            {
                p.log.WriteLine(_c.Size.ToString());
                result.IncCounters(((_c.Size.Width >= expected.Width) && (_c.Size.Height >= expected.Height)), "FAIL: Size is less than MinimumSize", expected.ToString(), _c.Size.ToString(), p.log);
            }
            else if (_c is ListBox)
                expected.Height = listBoxExpectedHeight((ListBox)_c, origSize.Height, newSize.Height, p);
			else if (_c is TextBox)
				//TextBox nolonger preserves height when AutoSize = true
				expected.Height = _c.PreferredSize.Height;
            else if (_c is TrackBar) {
                //trackbar no longer preserves height when Autosize = true
                expected.Height = newSize.Height;
                result.IncCounters(expected, _c.Size, "FAIL: set Size", p.log);
            }
            else
                result.IncCounters(expected, _c.Size, "FAIL: set Size", p.log);

            // restore minsize so as not to break subsequent sizing scenarios.
            _c.MinimumSize = origMinSize;
            _c.Size = origControlSize;
            return result;
        }

        protected virtual ScenarioResult set_UseWaitCursor(TParams p, bool value)
        {
            return get_UseWaitCursor(p);
        }

        protected virtual ScenarioResult get_UseWaitCursor(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;

            ScenarioResult result = new ScenarioResult();
            bool initUseWait = _c.UseWaitCursor;

            _c.UseWaitCursor = false;

            Cursor origCursor = _c.Cursor;

            _c.UseWaitCursor = initUseWait;

            // set to true, verify that Cursor is Wait 
            p.log.WriteLine("original Cursor: " + origCursor);
            p.log.WriteLine("1. change UseWaitCursor from {0} to true", _c.UseWaitCursor);
            _c.UseWaitCursor = true;
            result.IncCounters(_c.UseWaitCursor, "Failed: to set UseWaitCursor = true", p.log);
            result.IncCounters(_c.Cursor.Equals(Cursors.WaitCursor), "Failed: to update Cursor to Wait, Cursor is " + _c.Cursor, p.log);

            // set to false, verify Cursor went back to original
            p.log.WriteLine("2. change UseWaitCursor from {0} to false", _c.UseWaitCursor);
            _c.UseWaitCursor = false;
            result.IncCounters(!_c.UseWaitCursor, "Failed: to set UseWaitCursor = false", p.log);
            result.IncCounters(_c.Cursor.Equals(origCursor), p.log, BugDb.VSWhidbey, 61, "Didn't preserve Cursor in process of changing UseWaitCursor ");
            p.log.WriteLine("3. change UseWaitCursor to true, change Cursor property");
            _c.UseWaitCursor = true;
            result.IncCounters(_c.UseWaitCursor, "Failed: to set UseWaitCursor = true", p.log);
            result.IncCounters(_c.Cursor.Equals(Cursors.WaitCursor), "Failed: to update Cursor to Wait, Cursor is " + _c.Cursor, p.log);

            Cursor newCursor = p.ru.GetCursor();

            p.log.WriteLine("set Cursor to " + newCursor);

            // make sure our random cursor in non-Wait
            while (newCursor.Equals(Cursors.WaitCursor))
                newCursor = p.ru.GetCursor();

            // Cursor should return WaitCursor while UseWaitCursor = true
            _c.Cursor = newCursor;
            p.log.WriteLine("Cursor returns " + _c.Cursor + " after setting to " + newCursor + ", UseWaitCursor = " + _c.UseWaitCursor);
            result.IncCounters(_c.Cursor.Equals(Cursors.WaitCursor), p.log, BugDb.VSWhidbey, 61, "Cursor didn't return WaitCursor after setting to random cursor when UseWaitCursor = true");

            // Cursor should be restored to the new cursor after UseWait = false
            p.log.WriteLine("... set UseWaitCursor = false, verify new cursor is restored");
            _c.UseWaitCursor = false;
            p.log.WriteLine("Cursor returns " + _c.Cursor + " after setting UseWaitCursor = false ");
            result.IncCounters(_c.Cursor.Equals(newCursor), "Failed to restore new Cursor: " + newCursor + " after setting UseWait = false", p.log);
            _c.UseWaitCursor = p.ru.GetBoolean();	// for future scenarios
            return result;
        }

        protected virtual ScenarioResult get_AutoScrollOffset(TParams p)
        {
            _c = GetControl(p);

            Point pt = p.ru.GetPoint(Int32.MinValue, Int32.MaxValue, Int32.MinValue, Int32.MaxValue);
            _c.AutoScrollOffset = pt;

            return new ScenarioResult(pt.Equals(_c.AutoScrollOffset), "Point mismatch: " + pt.ToString() + " vs. " + _c.AutoScrollOffset.ToString());
        }

        protected virtual ScenarioResult set_AutoScrollOffset(TParams p, Point value)
        {
            return get_AutoScrollOffset(p);
        }

        protected virtual ScenarioResult Control_CheckForIllegalCrossThreadCalls(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            bool expected = false;
            bool initValue = Control.CheckForIllegalCrossThreadCalls;	// store initial value

            // check default value
            if (System.Diagnostics.Debugger.IsAttached)
                expected = true;

            result.IncCounters(initValue == expected, "default CheckForIllegalCrossThreadCalls returned unexpected value", p.log);
            if (initValue != expected)
                p.log.WriteLine("CheckForIllegalCrossThreadCalls = {0} when Debugger.IsAttached = {1}", initValue, expected);

            // verify Browsable = false & EditorBrowsable = Advanced
            Utilities.VerifyMemberBrowsable(new Control(), "CheckForIllegalCrossThreadCalls", false, EditorBrowsableState.Advanced, result, p.log);
            Control.CheckForIllegalCrossThreadCalls = true;
            result.IncCounters(Control.CheckForIllegalCrossThreadCalls, "Failed: returned false after explicitely setting to true", p.log);
            Control.CheckForIllegalCrossThreadCalls = false;
            result.IncCounters(!Control.CheckForIllegalCrossThreadCalls, "Failed: returned true after explicitely setting to false", p.log);

            // restore initial value
            Control.CheckForIllegalCrossThreadCalls = initValue;
            return result;
        }

        protected virtual ScenarioResult DrawToBitmap(TParams p, Bitmap bitmap, Rectangle targetBounds)
        {
            ScenarioResult result = new ScenarioResult();

            Bitmap mapA = new Bitmap(50, 50);
            Bitmap mapB = (Bitmap)mapA.Clone();
            _c = GetControl(p);

            bool bSet = SecurityCheck(result,
                delegate
                {
                    _c.DrawToBitmap(mapA, _c.ClientRectangle);
                }, typeof(Control).GetMethod("DrawToBitmap"),
                LibSecurity.AllWindows);

            if(bSet)
                result.IncCounters(!Utilities.BitmapsIdentical(mapA, mapB), "nothing was drawn", p.log);

            return result;
        }

        // KEVINTAO: ShouldSerialize methods are now internal.  I'll keep this code around for
        //           now, in case we ever need a test for these again.
        /* ShouldSerializeMethods are now internal
        protected virtual ScenarioResult ShouldSerializeText(TParams p)
                {
                    ScenarioResult result = new ScenarioResult();

                    c = (Control)CreateObject(p);
                    result.IncCounters(!c.ShouldSerializeText(), "FAILED initial state", p.log);

                    c.Text = p.ru.GetString(100);
                    result.IncCounters(c.ShouldSerializeText(), "FAILED after set value", p.log);

                    c.ResetText();
                    result.IncCounters(!c.ShouldSerializeText(), "FAILED after reset", p.log);

                    return result;
                }

               protected virtual ScenarioResult ShouldSerializeSize(TParams p)
               {
                    ScenarioResult result = new ScenarioResult();

                    c = (Control)CreateObject(p);
                    Controls.Add(c);
                    Application.DoEvents();
                    result.IncCounters(!c.ShouldSerializeSize(), "FAILED initial state", p.log);

                    c.Size = p.ru.GetSize(1000, 1000);  // set a reasonable upper bound
                    result.IncCounters(c.ShouldSerializeSize(), "FAILED after set value", p.log);

                    return result;
               }

               protected virtual ScenarioResult ShouldSerializeLocation(TParams p)
               {
                    ScenarioResult result = new ScenarioResult();

                    c = (Control)CreateObject(p);
                    Controls.Add(c);
                    Application.DoEvents();
                    result.IncCounters(!c.ShouldSerializeLocation(), "FAILED initial state", p.log);

                    c.Location = p.ru.GetPoint();
                    result.IncCounters(c.ShouldSerializeLocation(), "FAILED after set value", p.log);

                    return result;
               }

                protected virtual ScenarioResult ShouldSerializeRightToLeft(TParams p)
                {
                    ScenarioResult result = new ScenarioResult();

                    c = (Control)CreateObject(p);
                    result.IncCounters(!c.ShouldSerializeRightToLeft(), "FAILED initial state", p.log);

                    // NOTE: Inherit is the default for RightToLeft, but c.RightToLeft will return RightToLeft.No, so
                    //       we can't pass c.RightToLeft to p.ru.GetDifferentEnumValue().
                    c.RightToLeft = (RightToLeft)p.ru.GetDifferentEnumValue(typeof(RightToLeft), (int)RightToLeft.Inherit);
                    result.IncCounters(c.ShouldSerializeRightToLeft(), "FAILED after set value", p.log);

                    c.ResetRightToLeft();
                    result.IncCounters(!c.ShouldSerializeRightToLeft(), "FAILED after reset", p.log);

                    return result;
                }

                protected virtual ScenarioResult ShouldSerializeFont(TParams p)
                {
                    ScenarioResult result = new ScenarioResult();

                    c = (Control)CreateObject(p);
                    result.IncCounters(!c.ShouldSerializeFont(), "FAILED initial state", p.log);

                    c.Font = p.ru.GetFont();
                    result.IncCounters(c.ShouldSerializeFont(), "FAILED after set value", p.log);

                    c.ResetFont();
                    result.IncCounters(!c.ShouldSerializeFont(), "FAILED after reset", p.log);

                    return result;
                }

                protected virtual ScenarioResult ShouldSerializeForeColor(TParams p)
                {
                    ScenarioResult result = new ScenarioResult();

                    c = (Control)CreateObject(p);
                    result.IncCounters(!c.ShouldSerializeForeColor(), "FAILED initial state", p.log);

                    c.ForeColor = p.ru.GetColor();
                    result.IncCounters(c.ShouldSerializeForeColor(), "FAILED after set value", p.log);

                    return result;
                }

                protected virtual ScenarioResult ShouldSerializeBackColor(TParams p)
                {
                    ScenarioResult result = new ScenarioResult();

                    c = (Control)CreateObject(p);
                    result.IncCounters(!c.ShouldSerializeBackColor(), "FAILED initial state", p.log);

                    c.BackColor = p.ru.GetColor();
                    result.IncCounters(c.ShouldSerializeBackColor(), "FAILED after set value", p.log);

                    c.ResetBackColor();
                    result.IncCounters(!c.ShouldSerializeBackColor(), "FAILED after reset", p.log);

                    return result;
                }

                protected virtual ScenarioResult ShouldSerializeImeMode(TParams p) {
                    ScenarioResult sr = new ScenarioResult();

                    c = (Control)CreateObject(p);
                    p.log.WriteLine("Initial state: " + c.ShouldSerializeImeMode().ToString());
                    sr.IncCounters(!c.ShouldSerializeImeMode(), "FAILED initial state", p.log);

                    ImeMode setValue = (ImeMode)p.ru.GetEnumValue(typeof(ImeMode));
                    c.ImeMode = setValue;
                    bool expectedResult = (setValue != ExpectedActualImeDefault(c));

                    sr.IncCounters(c.ShouldSerializeImeMode()==expectedResult, "FAILED after set value: set to " + setValue + " value returned: " + c.ShouldSerializeImeMode().ToString(), p.log);

                    return sr;
                }

                protected virtual ScenarioResult ShouldSerializeCursor(TParams p)
                {
                    ScenarioResult result = new ScenarioResult();

                    c = (Control)CreateObject(p);
                    result.IncCounters(!c.ShouldSerializeCursor(), "FAILED initial state", p.log);

                    c.Cursor = p.ru.GetCursor();
                    result.IncCounters(c.ShouldSerializeCursor() == (!c.Cursor.Equals(newC.Cursor)), "FAILED after set value", p.log);

                    c.ResetCursor();
                    result.IncCounters(!c.ShouldSerializeCursor(), "FAILED after reset", p.log);

                    return result;
                }

                protected ScenarioResult ShouldSerializeBindings(TParams p) {
                    p.log.WriteLine("Tested by DataBinding automation");
                    return ScenarioResult.Pass; 
                }
        */
        /* BETA2: SendMessage is now Protected
        protected virtual ScenarioResult SendMessage(TParams p, Int32 msg, Int32 wParam, Int32 lParam)
        {
            if ((c = GetControl(p)) == null) return ScenarioResult.Fail;
            clicked = false;
            MouseEventHandler eh = new MouseEventHandler(this.Clicked);
            c.MouseUp += eh;

            c.SendMessage(win.WM_LBUTTONUP, 0, 0);
            Application.DoEvents();

            c.MouseUp -= eh;
            
            return new ScenarioResult(clicked);
        }

        // SendMessage with new parameters - calling previously existed SendMessage
        // (?) do we need to write special test for these 'ref'-parameters (?)
      protected virtual ScenarioResult SendMessage(TParams p, Int32 msg, ref short wParam, ref short lParam)
        {
            if ((c = GetControl(p)) == null) return ScenarioResult.Fail;
            return SendMessage(p, msg, wParam, lParam);
        }*/
                /* BETA2: TopLevel is now a property of Forms, not Controls
        protected virtual ScenarioResult set_TopLevel(TParams p, bool value)
        {
            return get_TopLevel(p);
        }

        protected virtual ScenarioResult get_TopLevel(TParams p)
        {
            if ((c = GetControl(p)) == null) return ScenarioResult.Fail;
            bool b = c.TopLevel;
            p.log.WriteLine("TopLevel was " + b.ToString());
            try
            {
                c.TopLevel = !b;
                if (c.Parent != null)
                    return new ScenarioResult(false, "Shouldn't be allowed to set TopLevel for parented control");
            }
            catch (Exception)
            {
                if (c.Parent == null)
                    return new ScenarioResult(false, "Failed to set TopLevel for non-parented control");
            }
            bool bb = c.TopLevel;
            p.log.WriteLine("TopLevel is " + bb.ToString());
            if (c.Parent != null)
                return new ScenarioResult(b == bb);
            else
                return new ScenarioResult(b != bb);
        }*/
                // new UI methods in Control
        // if Handle is not created, return true
        // IF control IS NOT top-level without parent, parent.property returned
        //
        /*BETA2: UI Show*Cues methods are protected
       protected virtual ScenarioResult get_ShowKeyboardCues(TParams p)
       {
           if ((c = GetControl(p)) == null) return ScenarioResult.Fail;

           ScenarioResult result; 
           UIShowHelper(p, ShowMethod.Keyboard, out result);
           return result;
       }

       protected virtual ScenarioResult get_ShowFocusCues(TParams p)
       {
           if ((c = GetControl(p)) == null) return ScenarioResult.Fail;
           ScenarioResult result; 
           UIShowHelper(p, ShowMethod.Focus, out result);
           return result;
       }

       // helper for UI Show*-methods
       protected enum ShowMethod {
           Keyboard,                   // ShowKeyboardCues
           Focus             // ShowFocusCues
       }

       //
       // Helper method to test both UI Show*Cues methods
       //
       protected virtual void UIShowHelper(TParams p, ShowMethod method, out ScenarioResult result)
       {
           if ((c = GetControl(p)) == null){
               result = ScenarioResult.Fail;
               return;
           }

           result = new ScenarioResult();
           
           bool bExpected = false;

           // - situation when Handle is not created yet
           // for newly created & not yet displayed control, Handle should not be created
           // for TabControl and TabPage Handle is created as soon as TabPage is added - so it's an exception
           // for ListBox handle is also created
           if (! (c is TabControl || c is TabPage || c is ListBox)) {
              p.log.WriteLine("1. Getting value for control without handle");
              Control temp = (Control)CreateObject(p);
              string descr = "FAILED: returned false for control without handle";
              if (temp.IsHandleCreated) {
                 descr = "FAILED: Handle was created for new, not yet dispayed control - investigation needed ";
              }

              switch ( method ) {
                  case ShowMethod.Keyboard :
                      bExpected = temp.ShowKeyboardCues;
                      break;
                  case ShowMethod.Focus :
                      bExpected = temp.ShowFocusCues;
                      break;
                  default :
                      throw new ArgumentException("Invalid ShowMethod: " + method.ToString());  
              }        

              result.IncCounters(!temp.IsHandleCreated && bExpected, descr, p.log);
              temp.Dispose();
           }

           // our tested control is already displayed - it should have Handle created
           if (!c.IsHandleCreated){ 
               result.IncCounters(false, "FAILED: Control's handle is not created - can't execute remainder of test", p.log);
               return;
           }

           p.log.WriteLine("2. Getting value for control with handle");
           if (!(c.TopLevel || c.Parent == null)) {         // our situation
               switch ( method ) {
                   case ShowMethod.Keyboard :
                       bExpected = (c.ShowKeyboardCues == c.Parent.ShowKeyboardCues);
                       break;
                   case ShowMethod.Focus :
                       bExpected = (c.ShowFocusCues == c.Parent.ShowFocusCues);
               }        
               result.IncCounters(bExpected, "FAILED: didn't return parent's value", p.log);
           }
           // modeling situation when control is TopLevel and without Parent
           if (c.Parent != null) {
               c.Parent = null;
           }
           if (!c.TopLevel) {
               c.TopLevel = true;
           }
           bool b = false; 
           switch ( method ) {
           case ShowMethod.Keyboard :
                   b = ((SendMessage(win.WM_QUERYUISTATE, 0, 0) & win.UISF_HIDEACCEL) == 0); 
                   bExpected = (c.ShowKeyboardCues == b);
                   break;
               case ShowMethod.Focus :
                   b = ((SendMessage(win.WM_QUERYUISTATE, 0, 0) & win.UISF_HIDEFOCUS) == 0); 
                   bExpected = (c.ShowFocusCues == b);
           }        
           result.IncCounters(bExpected, "FAILED: unexpected value for top-level control without parent", p.log);
           
           // discard control we manipulated with and recreate it for further scenarios
           c.Dispose();
           p.target = (Control)this.CreateObject(p);
           c = GetControl(p);
           this.AddObjectToForm(p);  
       }
*/

        // Accessible event tests

        //EVENT_OBJECT_CREATE: Instanciate the control and make it visable on a form.  Verify this event fires as the AccessibleObject is created.
        #region Accessibility Tests
        protected virtual ScenarioResult TestAccessibleEvents(TParams p)
        {
	    if (Utilities.IsWin9x)
                return ScenarioResult.Pass;
            ScenarioResult sr = new ScenarioResult();
            sr.IncCounters(OBJECT_CREATE(p));
            sr.IncCounters(OBJECT_DESTROY(p));
            sr.IncCounters(OBJECT_FOCUS(p));
            sr.IncCounters(OBJECT_LOCATIONCHANGE(p));
            //sr.IncCounters(OBJECT_NAMECHANGE(p));
            //sr.IncCounters(OBJECT_DEFACTIONCHANGE(p));
            p.log.WriteLine("VSWhidbey# Regression_Bug62 postponed.  OBJECT_NAMECHANGE and OBJECT_DEFACTIONCHANGE tests disabled");
            sr.IncCounters(OBJECT_PARENTCHANGE(p));
            return sr;
        }
        [Scenario(false)]
        protected virtual ScenarioResult OBJECT_CREATE(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;
            ScenarioResult sr = new ScenarioResult();
            bool prevVisible = this.Visible;
            this.Visible = true;
            try
            {
                using (AccessibleEventListener listener = new AccessibleEventListener(AccessibleEventListener.EVENT.OBJECT_CREATE))
                {
                    _c = new Control();
                    this.Controls.Add(_c);
                    Application.DoEvents();
                    sr.IncCounters(1 <= listener.FiredEvents.Count, "FAIL: expected OBJECT_CREATE accessible event to be fired once", p.log);
                    this.Controls.Remove(_c);
                }
                return sr;
            }
            finally { this.Visible = prevVisible; }
        }

        ////EVENT_OBJECT_DEFACTIONCHANGE:  Default Action Changed.  Perform the action on the control that will cause the default action to change and verify the event fired.
        //[Scenario(false)]
        //protected virtual ScenarioResult OBJECT_DEFACTIONCHANGE(TParams p)
        //{
        //    if ((c = GetControl(p)) == null) return ScenarioResult.Fail;
        //    ScenarioResult sr = new ScenarioResult();
        //    if (c is Form) ((Form)c).TopLevel = false;
        //    AddObjectToForm(p);
        //    using (AccessibleEventListener listener = new AccessibleEventListener(AccessibleEventListener.EVENT.OBJECT_DEFACTIONCHANGE))
        //    {
        //        p.log.WriteLine("DefaultActionDescription was: " + c.AccessibleDefaultActionDescription);
        //        c.AccessibleDefaultActionDescription = p.ru.GetString(20);
        //        p.log.WriteLine("DefaultActionDescription is: " + c.AccessibleDefaultActionDescription);
        //        Application.DoEvents();
        //        sr.IncCounters(1 <=listener.FiredEvents.Count, p.log, BugDb.VSWhidbey, Regression_Bug62 );//"FAIL: expected OBJECT_DEFACTIONCHANGE accessible event to be fired once", p.log);
        //    }
        //    return sr;
        //}
        //EVENT_OBJECT_DESTROY: Dispose of the control object and verify that the Event fires.
        [Scenario(false)]
        protected virtual ScenarioResult OBJECT_DESTROY(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;
            ScenarioResult sr = new ScenarioResult();
            if (_c is Form) ((Form)_c).TopLevel = false;
            AddObjectToForm(p);
            _c.Visible = true;
            using (AccessibleEventListener listener = new AccessibleEventListener(AccessibleEventListener.EVENT.OBJECT_DESTROY))
            {
                try
                {
                    _c.Dispose();
                    Application.DoEvents();
                    sr.IncCounters(1 <= listener.FiredEvents.Count, "FAIL: expected OBJECT_DESTROY accessible event to be fired once", p.log);
                }
                finally
                {
                    this.Controls.Remove(_c);
                    RecreateControl(p);
                }
            }
            return sr;
        }
        //EVENT_OBJECT_FOCUS: Should fire when the control gets focus.
        [Scenario(false)]
        protected virtual ScenarioResult OBJECT_FOCUS(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;
            ScenarioResult sr = new ScenarioResult();
            if (!_c.CanFocus)
            { return ScenarioResult.Pass; }

            AddObjectToForm(p);
            //if (c is Form) ((Form)c).TopLevel = false;
            //if (AddControlToForm)
            //    this.Controls.Add(c);
            _c.Visible = true;
            bool prevVisible = this.Visible;
            this.Visible = true;
            try
            {
                Button b = new Button();
                this.Controls.Add(b);
                SafeMethods.Focus(b);
                using (AccessibleEventListener listener = new AccessibleEventListener(AccessibleEventListener.EVENT.OBJECT_FOCUS))
                {
                    if (!AddControlToForm)
                        _c.Show();
                    SafeMethods.Focus(_c);

                    Application.DoEvents();
                    sr.IncCounters(1 <= listener.FiredEvents.Count, "FAIL: expected OBJECT_FOCUS accessible event to be fired once", p.log);
                }
                return sr;
            }
            finally { this.Visible = prevVisible; }
        }
        //EVENT_OBJECT_HIDE:  An object is hidden.  Verify by calling Hide on the object.
        [Scenario(false)]
        protected virtual ScenarioResult OBJECT_HIDE(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;
            ScenarioResult sr = new ScenarioResult();
            _c = CreateObject(p) as Control;
            if (_c is Form) ((Form)_c).TopLevel = false;
            if (!AddControlToForm)
                _c.Show();
            else
                this.Controls.Add(_c);
            using (AccessibleEventListener listener = new AccessibleEventListener(AccessibleEventListener.EVENT.OBJECT_HIDE))
            {
                _c.Hide();
                Application.DoEvents();
                sr.IncCounters(1 <= listener.FiredEvents.Count, "FAIL: expected OBJECT_HIDE accessible event to be fired once", p.log);
            }
            _c.Show();
            return sr;
        }
        //EVENT_OBJECT_LOCATIONCHANGE: Fired on both a resize and a move.
        [Scenario(false)]
        protected virtual ScenarioResult OBJECT_LOCATIONCHANGE(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;
            ScenarioResult sr = new ScenarioResult();
            p.target = _c = CreateObject(p) as Control;

            if (_c is Form) ((Form)_c).TopLevel = false;
            AddObjectToForm(p);

            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;
            _c.Visible = true;
            using (AccessibleEventListener listener = new AccessibleEventListener(AccessibleEventListener.EVENT.OBJECT_LOCATIONCHANGE))
            {
                _c.Location = new Point(_c.Location.X + 1, _c.Location.Y + 1);
                Application.DoEvents();
                sr.IncCounters(1 <= listener.FiredEvents.Count, "FAIL: expected OBJECT_LOCATIONCHANGE accessible event to be fired once when control moves", p.log);

                Size oldSize = _c.Size;
                _c.Size = new Size(_c.Size.Width + 1, _c.Size.Height + 1);
                Application.DoEvents();

                if (oldSize == _c.Size)
                    sr.IncCounters(1 <= listener.FiredEvents.Count, "FAIL: size did not change and expected no OBJECT_LOCATIONCHANGE accessible event to be fired", p.log);
                else
                    sr.IncCounters(2 <= listener.FiredEvents.Count, "FAIL: expected OBJECT_LOCATIONCHANGE accessible event to be fired again when control is resized", p.log);
            }

            return sr;
        }
        ////EVENT_OBJECT_NAMECHANGE: Fired if the Accessible Name property is changed.  Verify by changing the Control. AccessibleName property.
        //[Scenario(false)]
        //protected virtual ScenarioResult OBJECT_NAMECHANGE(TParams p)
        //{
        //    if ((c = GetControl(p)) == null) return ScenarioResult.Fail;
        //    ScenarioResult sr = new ScenarioResult();
        //    c = CreateObject(p) as Control;

        //    AddObjectToForm(p);

        //    using (AccessibleEventListener listener = new AccessibleEventListener(AccessibleEventListener.EVENT.OBJECT_NAMECHANGE))
        //    {
        //        c.AccessibleName = p.ru.GetString(20);
        //        Application.DoEvents();
        //        sr.IncCounters(1<= listener.FiredEvents.Count, p.log, BugDb.VSWhidbey, Regression_Bug62 );//"FAIL: expected OBJECT_NAMECHANGE accessible event to be fired once", p.log);
        //    }
        //    return sr;
        //}
        //EVENT_OBJECT_PARENTCHANGE: Fired when the parent object changes.  Verify by changing the elements parent.
        [Scenario(false)]
        protected virtual ScenarioResult OBJECT_PARENTCHANGE(TParams p)
        {
            if ((_c = GetControl(p)) == null) return ScenarioResult.Fail;
            ScenarioResult sr = new ScenarioResult();

            if (_c is Form) ((Form)_c).TopLevel = false;
            //top level control can't have parent
            if (!AddControlToForm)
                return new ScenarioResult(true);

            //			this.Controls.Add(c = CreateObject(p) as Control);
            _c = CreateObject(p) as Control;
            if (_c is Form) ((Form)_c).TopLevel = false;
            this.Controls.Add(_c);


            Panel pa = new Panel();
            this.Controls.Add(pa);
            Application.DoEvents();
            using (AccessibleEventListener listener = new AccessibleEventListener(AccessibleEventListener.EVENT.OBJECT_PARENTCHANGE))
            {
                pa.Controls.Add(_c);
                Application.DoEvents();
                //This event could fire multiple times because of the parking window, that's not a problem
                sr.IncCounters(0 < listener.FiredEvents.Count, "FAIL: expected OBJECT_PARENTCHANGE accessible event to be fired once", p.log);
            }
            return sr;
        }
        #endregion Accessibility Tests
    }
}
