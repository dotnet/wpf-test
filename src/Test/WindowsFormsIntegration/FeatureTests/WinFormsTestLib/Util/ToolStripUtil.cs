// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;

namespace WFCTestLib.Util
{
	namespace ToolStripItems
	{
		/// <summary>
		/// A simple ToolStripItem which derives directly from ToolStripItem and is always selected.
		/// This class can be used to test ToolStripContainers ability to host ToolStripItem-derived members.
		/// </summary>
		public class CustomToolStripItem : ToolStripItem
		{
			/// <summary>
			/// override the base property selected to always return true.
			/// </summary>
			/// <value>true</value>
			public override string Text
			{
				get
				{
					return "Custom Item";
				}
			}
		}

		/// <summary>
		/// overrides ToolStripControl host to automatically contain a simple user control: <see cref="ToolStripSearchBox"/>.
		/// The Text property is forwarded to the contained control.
		/// </summary>
		public class ToolStripCustomUC : ToolStripControlHost
		{
			/// <summary>
			/// Forwards the text property to the contained ToolStripSearchBox
			/// </summary>
			/// <value></value>
			public override string Text
			{
				get
				{
					return Control.Text;
				}
				set
				{
					Control.Text = value;
				}
			}

			/// <summary>
			/// Creates a new ToolStripCustomUC and embeds a <see cref="ToolStripSearchBox"/> within it.
			/// </summary>
			public ToolStripCustomUC():base(new ToolStripSearchBox())
			{
			}
		}

		/// <summary>
		/// Simple user control containing a text box and a button (office-style search item).
		/// </summary>
		public class ToolStripSearchBox : System.Windows.Forms.UserControl
		{
			/// <summary>
			/// Forwards the text property on to the findText TextBox.
			/// </summary>
			/// <value>The value contained within the findText TextBox.</value>
			public override string Text
			{
				get
				{
					return _findText.Text;
				}
				set
				{
					_findText.Text = value;
				}
			}

			private System.Windows.Forms.TextBox _findText;

			private System.Windows.Forms.Button _findButton;

			/// <summary>
			/// Simple Constructor creates a default user control and initializes it.
			/// (simplified version of what the designer would create)
			/// </summary>
			public ToolStripSearchBox()
			{
				this._findText = new System.Windows.Forms.TextBox();
				this._findButton = new System.Windows.Forms.Button();
				this.SuspendLayout();
				this._findText.Location = new System.Drawing.Point(4, 4);
				this._findText.Size = new System.Drawing.Size(120, 20);
				this._findText.TabIndex = 0;
				this._findButton.Location = new System.Drawing.Point(128, 4);
				this._findButton.Size = new System.Drawing.Size(50, 20);
				this._findButton.TabIndex = 1;
				this._findButton.Text = "&Find";
				this.Controls.AddRange(new Control[] { this._findButton, this._findText });
				this.Name = "ToolStripSearchBox";
				this.Size = new System.Drawing.Size(182, 28);
				this.ResumeLayout(false);
				this.PerformLayout();
			}
		}
	}

	[Flags]
	public enum ToolStripContainerFlags
	{
		IsDropDown = 0x01
	}
	public struct ToolStripContainerDescription
	{
		public const int MAX_ITEMS = 50;
		private ToolStripContainerDescription(Type t, ToolStripContainerFlags flags, params ToolStripItemDescription[] availableItems)
		{ 
			T = t;
			_Flags = flags;
			_AvailableItems = availableItems;
		}
		private ToolStripContainerDescription(Type t, bool IsDropDown, params ToolStripItemDescription[] availableItems)
		{
			T = t;
			_Flags = 0;
			if (IsDropDown) { _Flags |= ToolStripContainerFlags.IsDropDown; }
			_AvailableItems = availableItems;
		}
		private ToolStripItemDescription[] _AvailableItems;
		public ToolStripItemDescription[] AvailableItems
		{
			get
			{
				ToolStripItemDescription[] ret = new ToolStripItemDescription[_AvailableItems.Length];
				_AvailableItems.CopyTo(ret, 0);
				return ret;
			}
		}
		private ToolStripContainerFlags _Flags;
		public bool IsDropDown { get { return (_Flags & ToolStripContainerFlags.IsDropDown) != 0; } }

		/// <summary>
		/// A Type object representing the Container
		/// </summary>
		public readonly Type T;

		public static Exception CompositeInvocationException(TargetInvocationException tio)
		{
			Exception ret = tio.InnerException;
			string innerMessage = ret.Message;
			string deepTrace = ret.StackTrace;

			string fullMessage = string.Join(Environment.NewLine, new string[] { innerMessage, "Stack Trace: ", deepTrace });
			Type t = typeof(Exception);
			LibSecurity.Reflection.Assert();
			try
			{
				t.GetField("_message", ~BindingFlags.DeclaredOnly).SetValue(ret, fullMessage);
			}
			finally { System.Security.CodeAccessPermission.RevertAssert(); }
			return ret;
		}

		public ToolStrip CreateEmpty(Form f)
		{
			ToolStrip ret;
			try
			{
				ret = Activator.CreateInstance(T) as ToolStrip;
			}
			catch(System.Reflection.TargetInvocationException ex)
			{ throw CompositeInvocationException(ex);}
			if (null != f && null != ret) { f.Controls.Add(ret); }
			return ret;
		}
		public ToolStrip CreateEmpty()
		{ return CreateEmpty(null); }

		public ToolStrip CreateRandom(RandomUtil ru, Form f, int numItems)
		{
			if (numItems <= 0) { throw new ArgumentOutOfRangeException("numItems must be positive"); }
			ToolStrip ret = CreateEmpty(f);
			ret.Items.AddRange(ToolStripItemDescription.CreateRandomItemArray(ru, numItems, AvailableItems));
			return ret;
		}

		public ToolStrip CreateRandom(RandomUtil ru, int numItems)
		{ return CreateRandom(ru, null, numItems); }
		
		public ToolStrip CreateRandom(RandomUtil ru)
		{ return CreateRandom(ru, null); }
		public ToolStrip CreateRandom(RandomUtil ru, Form f)
		{ return CreateRandom(ru, f, ru.GetRange(1, MAX_ITEMS)); }

		public static ToolStripContainerDescription ToolStripDescription { get { return FromType(typeof(ToolStrip)); } }
		public static ToolStripContainerDescription MenuStripDescription { get { return FromType(typeof(MenuStrip)); } }
		public static ToolStripContainerDescription ToolStripDropDownMenuDescription { get { return FromType(typeof(ToolStripDropDownMenu)); } }

		internal static ToolStripContainerDescription[] all = new ToolStripContainerDescription[]
		{
			new ToolStripContainerDescription(typeof(ToolStrip), false, 
				ToolStripItemDescription.FromType(typeof(ToolStripButton)),
				ToolStripItemDescription.FromType(typeof(ToolStripLabel)),
				ToolStripItemDescription.FromType(typeof(ToolStripSplitButton)),
				ToolStripItemDescription.FromType(typeof(ToolStripDropDownButton)),
//				ToolStripItemDescription.FromType(typeof(ToolStripProgressBar)),
//				ToolStripItemDescription.FromType(typeof(ToolStripTextBox)),
//				ToolStripItemDescription.FromType(typeof(ToolStripComboBox)),
				ToolStripItemDescription.FromType(typeof(ToolStripSeparator))
			),
			new ToolStripContainerDescription(typeof(MenuStrip), false,
				ToolStripItemDescription.FromType(typeof(ToolStripMenuItem)), 
				ToolStripItemDescription.FromType(typeof(ToolStripSeparator))
			),
			new ToolStripContainerDescription(typeof(ToolStripDropDownMenu), false,
				ToolStripItemDescription.FromType(typeof(ToolStripMenuItem)),
				ToolStripItemDescription.FromType(typeof(ToolStripSeparator))
			)
		};

		public ToolStripItem CreateRandomCompatibleItem(RandomUtil ru)
		{ return ToolStripItemDescription.CreateRandomItem(ru, AvailableItems); }

		public ToolStripItem[] CreateRandomCompatibleItemArray(RandomUtil ru, int numItems)
		{ return ToolStripItemDescription.CreateRandomItemArray(ru, numItems, AvailableItems); }
		public ToolStripItem[] CreateRandomCompatibleItemArray(RandomUtil ru)
		{ return ToolStripItemDescription.CreateRandomItemArray(ru, AvailableItems); }
		public static ToolStripContainerDescription FromType(Type t)
		{
			for (int i = 0; i < all.Length; i++)
			{
				if (all[i].T == t)
				{ return all[i]; }
			}

			throw new ArgumentException("Invalid Type specified, type must be present in all");
		}
	}

	[Flags]
	public enum ToolStripItemFlags
	{
		SupportsVertical = 0x01,
		SupportsChildren = 0x02,
		IsBuiltin = 0x04,
		IsSeperator = 0x08,
		ShouldRotate = 0x10
	}
	/// <summary>
	/// A description of an individual ToolStripItem.  This can be used to filter ToolStrips that support a particular functionality.
	/// </summary>
	public struct ToolStripItemDescription
	{
		private ToolStripItemFlags _flags;

		/// <summary>
		/// A Type object representing the Item
		/// </summary>
		public readonly Type T;

		public static ToolStripItemDescription ButtonDescription { get { return FromType(typeof(ToolStripButton)); } }		
//		public static ToolStripItemDescription ComboBoxDescription { get { return FromType(typeof(ToolStripComboBox)); } }
		public static ToolStripItemDescription LabelDescription { get { return FromType(typeof(ToolStripLabel)); } }
		public static ToolStripItemDescription SeparatorDescription { get { return FromType(typeof(ToolStripSeparator)); } }
		public static ToolStripItemDescription SplitButtonDescription { get { return FromType(typeof(ToolStripSplitButton)); } }
		public static ToolStripItemDescription DropDownButtonDescription { get { return FromType(typeof(ToolStripDropDownButton)); } }
//		public static ToolStripItemDescription TextBoxDescription { get { return FromType(typeof(ToolStripTextBox)); } }
		public static ToolStripItemDescription MenuItemDescription { get { return FromType(typeof(ToolStripMenuItem)); } }
//		public static ToolStripItemDescription ProgressBarHostDescription { get { return FromType(typeof(ToolStripProgressBar)); } }	
		public static ToolStripItemDescription CustomDescription { get { return FromType(typeof(ToolStripItems.CustomToolStripItem)); } }
//		public static ToolStripItemDescription UserControlDescription { get { return FromType(typeof(ToolStripItems.ToolStripCustomUC)); } }
//		public static ToolStripItemDescription SearchBoxDescription { get { return FromType(typeof(ToolStripItems.ToolStripSearchBox)); } }

		internal static ToolStripItemDescription[] all = new ToolStripItemDescription[] 
		{
			new ToolStripItemDescription(typeof(ToolStripButton), true, false, true, false, true),
//			new ToolStripItemDescription(typeof(ToolStripComboBox), true, false, true, false, false), 
			new ToolStripItemDescription(typeof(ToolStripLabel), true, false, true, false, true), 
			new ToolStripItemDescription(typeof(ToolStripSeparator), true, false, true, true, true), 
			new ToolStripItemDescription(typeof(ToolStripSplitButton), true, true, true, false, true), 
			new ToolStripItemDescription(typeof(ToolStripDropDownButton), true, true, true, false, true), 
//			new ToolStripItemDescription(typeof(ToolStripTextBox), true, false, true, false, false),
			new ToolStripItemDescription(typeof(ToolStripMenuItem), true, false, true, false, true), 
//			new ToolStripItemDescription(typeof(ToolStripProgressBar), true, false, true, false, true),
			new ToolStripItemDescription(typeof(ToolStripItems.CustomToolStripItem), true, false, false, false, true), 
//			new ToolStripItemDescription(typeof(ToolStripItems.ToolStripCustomUC), true, false, false, false, false)
		};

		public static ToolStripItemDescription FromType(Type t)
		{
			for (int i = 0; i < all.Length; i++)
			{
				if (all[i].T == t)
				{ return all[i]; }
			}
			throw new ArgumentException("Invalid Type specified, type must be present in all");
		}

		public static ToolStripItemDescription[] All
		{
			get
			{
				ToolStripItemDescription[] ret = new ToolStripItemDescription[all.Length];
				all.CopyTo(ret,0);

				return ret;
			}
		}

		/// <summary>
		/// Gets an array of all defined possible ToolStripTypes (defined in static array above)
		/// </summary>
		/// <returns>An array of Types derived from ToolStripItem</returns>
		public static Type[] GetAllToolStripItemTypes()
		{
			Type[] ret = new Type[all.Length];

			for (int i = 0; i < ret.Length; i++)
			{
				ret[i] = all[i].T;
			}

			return ret;
		}

		/// <summary>
		/// Filters an array of ToolStripItemDescriptions based on the specified mask and values.
		/// <see cref="IsCompatible"/>
		/// </summary>
		/// <param name="input">The array to consider.</param>
		/// <param name="mask">A mask identifying which fields to consider (set the bits for those flags that must match).</param>
		/// <param name="values">The values that must match.</param>
		/// <returns>An array of all ToolStripItemDescriptions which match the specified mask and values.</returns>
		public static ToolStripItemDescription[] Filter(ToolStripItemFlags mask, ToolStripItemFlags values, params ToolStripItemDescription[] input)
		{
			List<ToolStripItemDescription> lst = new List<ToolStripItemDescription>();

			foreach (ToolStripItemDescription itm in input)
			{
				if (itm.IsCompatible(mask, values))
				{ lst.Add(itm); }
			}

			return lst.ToArray();
		}

		/// <summary>
		/// Identifies whether a ToolStripItemDescription matches the specified parameters.
		/// </summary>
		/// <param name="mask">A mask identifying which fields to consider (set the bits for those flags that must match).</param>
		/// <param name="values">The values that must match.</param>
		/// <returns>true if the Description is compatible with the input values, else false.</returns>
		public bool IsCompatible(ToolStripItemFlags mask, ToolStripItemFlags values)
		{
			return (_flags & mask) == (values & mask);
		}

		public static ToolStripItem CreateRandomItem(RandomUtil ru, params ToolStripItemDescription[] possible)
		{ 
			if (ru == null) { throw new ArgumentNullException("ru"); }
			if (possible == null || possible.Length < 1) { throw new ArgumentOutOfRangeException("possible"); }
			return possible[ru.GetRange(0, possible.Length - 1)].Create();
		}

		public static ToolStripItem[] CreateRandomItemArray(RandomUtil ru, int numItems, params ToolStripItemDescription[] possible)
		{ 
			if (ru == null) { throw new ArgumentNullException("RandomUtil must be specified", "ru"); }
			if (possible == null || possible.Length < 1) { throw new ArgumentOutOfRangeException("Must be at least one possible Item", "possible"); }
			if (numItems <= 0) { return new ToolStripItem[0]; }

			ToolStripItem[] ret = new ToolStripItem[numItems];
			for (int i = 0; i < ret.Length; i++)
			{
				ret[i] = CreateRandomItem(ru, possible);
			}
			return ret;
		}
		public static ToolStripItem[] CreateRandomItemArray(RandomUtil ru, params ToolStripItemDescription[] possible)
		{ return CreateRandomItemArray(ru, ru.GetRange(1, ToolStripContainerDescription.MAX_ITEMS)); }

		public static ToolStripItem[] CreateAllItems(params ToolStripItemDescription[] itms)
		{
			if (itms == null || itms.Length < 1) { throw new ArgumentOutOfRangeException("at least one item must be specified"); }

			ToolStripItem[] ret = new ToolStripItem[itms.Length];

			for (int i = 0; i < ret.Length; i++)
			{ ret[i] = itms[i].Create(); }
			return ret;
		}

		/// <summary>
		/// Create an instance of the ToolStripItem identified by this description
		/// </summary>
		/// <returns>A new ToolStripItem</returns>
		public ToolStripItem Create()
		{
			try
			{
				ToolStripItem ret = (ToolStripItem)Activator.CreateInstance(T);

				ret.Text = ReflectBase.ScenarioParams.ru.GetString(ToolStripUtil.MAX_STRING_LENGTH);
//				ret.Text = new GenStrings.IntlStrings().GetString(ToolStripUtil.MAX_STRING_LENGTH, true, true);
				return ret;
			}
			catch (System.Reflection.TargetInvocationException ex)
			{//Flip the inner and outer exceptions
				if (ex.InnerException != null)
				{
					throw ex.InnerException;
				}
				else
				{
					throw new Exception(string.Format("Failed to create {0}", T.ToString()), ex);
				}
			}
		}

		public static ToolStripItemDescription[] FromTypeArray(params Type[] t)
		{
			ToolStripItemDescription[] ret = new ToolStripItemDescription[t.Length];

			for (int i = 0; i < ret.Length; i++)
			{
				ret[i] = FromType(t[i]);
			}

			return ret;
		}

		/// <summary>
		/// Creates a new ToolStripItemDescription, and initializes the value
		/// </summary>
		/// <param name="t">A Type object representing the Item</param>
		/// <param name="supportsVertical">
		/// Whether or not the item should be displayed in the main bar when aligned vertically
		/// (docked left/right/fill).  If false, the item should move to overflow when orientation is vertical.
		/// </param>
		/// <param name="supportsChildren">Identifies whether or not the object invokes ToolStripDropDownMenus.</param>
		/// <param name="isBuiltin">Identifies whether or not the Item is a builtin ToolStripMenuItem type.</param>
		/// <param name="isSeperator">Identifies whether or not the Item is a seperator or not.</param>
		internal ToolStripItemDescription(Type t, bool supportsVertical, bool supportsChildren, bool isBuiltin, bool isSeperator, bool shouldRotate)
		{
			T = t;
			_flags = 0;
			if (supportsVertical) { _flags |= ToolStripItemFlags.SupportsVertical; }

			if (supportsChildren) { _flags |= ToolStripItemFlags.SupportsChildren; }

			if (isBuiltin) { _flags |= ToolStripItemFlags.IsBuiltin; }

			if (isSeperator) { _flags |= ToolStripItemFlags.IsSeperator; }

			if (shouldRotate) { _flags |= ToolStripItemFlags.ShouldRotate; }
		}

		#region Flags properties
		/// <summary>
		/// Whether or not the item should be displayed in the main bar when aligned vertically
		/// (docked left/right/fill).  If false, the item should move to overflow when orientation is vertical.
		/// </summary>
		public bool SupportsVertical
		{ get { return 0 != (_flags & ToolStripItemFlags.SupportsVertical); } }

		/// <summary>
		/// Identifies whether or not the object invokes ToolStripDropDownMenus.
		/// </summary>
		public bool SupportsChildren
		{ get { return 0 != (_flags & ToolStripItemFlags.SupportsChildren); } }

		/// <summary>
		/// Identifies whether or not the Item is a builtin ToolStripMenuItem type.
		/// </summary>
		public bool IsBuiltin
		{ get { return 0 != (_flags & ToolStripItemFlags.IsBuiltin); } }

		/// <summary>
		/// Identifies whether or not the Item is a Seperator.
		/// </summary>
		public bool IsSeperator
		{ get { return 0 != (_flags & ToolStripItemFlags.IsSeperator); } }

		public bool ShouldRotate
		{ get { return 0 != (_flags & ToolStripItemFlags.ShouldRotate); } }
		#endregion Flags properties
	}

	/// <summary>
	/// Contains static methods to assist in testing ToolStrips
	/// </summary>
	public sealed class ToolStripUtil
	{
		private static Color s_VISIBLE_FLAG_COLOR = Color.HotPink;

		public static bool IsToolStripItemVisible(ToolStrip wb, ToolStripItem itm)
		{
			Color restoreBackColor = itm.BackColor;
			Color restoreForeColor = itm.ForeColor;

			try
			{
				itm.BackColor = s_VISIBLE_FLAG_COLOR;
				itm.ForeColor = s_VISIBLE_FLAG_COLOR;
				wb.Refresh();
				using (Bitmap bmp = Utilities.GetBitmapOfControl(wb))
				{
					bool bRet = Utilities.ContainsColor(bmp, s_VISIBLE_FLAG_COLOR);
					return bRet;
				}
			}
			finally
			{
				itm.BackColor = restoreBackColor;
				itm.ForeColor = restoreForeColor;
			}
		}

		public static bool IsDockValid(Form f, Control ctrl, DockStyle expectedDock, params Control[] higherZOrderControls)
		{
			if (ctrl.Dock != expectedDock)
			{ return false; }

			if (!f.Controls.Contains(ctrl))
			{ throw new InvalidOperationException("Control is not contained in the form"); }

			System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, f.ClientSize.Width, f.ClientSize.Height);

			if (null != higherZOrderControls)
			{
				for (int i = 0; i < higherZOrderControls.Length; i++)
				{
					int dX = higherZOrderControls[i].Width;
					int dY = higherZOrderControls[i].Height;

					switch (higherZOrderControls[i].Dock)
					{
						case DockStyle.Top:
							rect = new System.Drawing.Rectangle(rect.X, rect.Y + dY, rect.Width, rect.Height - dY);
							break;

						case DockStyle.Left:
							rect = new System.Drawing.Rectangle(rect.X + dX, rect.Y, rect.Width - dX, rect.Height);
							break;

						case DockStyle.Right:
							rect = new System.Drawing.Rectangle(rect.X, rect.Y, rect.Width - dX, rect.Height);
							break;

						case DockStyle.Bottom:
							rect = new System.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height - dY);
							break;

						default:
							throw new ArgumentException("Filled or non-docked controls should not be higher in the Z-order");
					}
				}
			}

			switch (expectedDock)
			{
				case DockStyle.Top:
					return (rect.Top == ctrl.Top && rect.Left == ctrl.Left && rect.Right == ctrl.Right);

				case DockStyle.Left:
					return (rect.Top == ctrl.Top && rect.Left == ctrl.Left && rect.Bottom == ctrl.Bottom);

				case DockStyle.Right:
					return (rect.Top == ctrl.Top && rect.Bottom == ctrl.Bottom && rect.Right == ctrl.Right);

				case DockStyle.Bottom:
					return (rect.Bottom == ctrl.Bottom && rect.Left == ctrl.Left && rect.Right == ctrl.Right);

				case DockStyle.Fill:
					return (rect.Bottom == ctrl.Bottom && rect.Left == ctrl.Left && rect.Right == ctrl.Right && rect.Top == ctrl.Top);

				default:
					throw new ArgumentException("Control must be docked to check validity");
			}
		}

		/// <summary>
		/// Private constructor prevents this object from being created (use static methods only)
		/// </summary>
		private ToolStripUtil() { }

		/// <summary>
		/// Defines the max String length which will be used to generate labels for ToolStripItems.
		/// </summary>
		public const int MAX_STRING_LENGTH = 10;

		#region Child enumerators
		/// <summary>
		/// Gets all of the immediate children of a form (top-level controls) which match the template provided
		/// </summary>
		/// <param name="f">The form to search.</param>
		/// <returns>An array of controls matching the template.</returns>
		///<example>
		/// Util.GetItemsOnForm&gt;ToolStrip&lt;(this); returns an array of all the ToolStrips on the form.
		///</example>
		public static T[] GetItemsOnForm<T>(Form f)
			where T : Control
		{
			List<T> ar = new List<T>();

			foreach (Control c in f.Controls)
			{
				if (c is T)
				{
					ar.Add((T)c);
				}
			}

			return ar.ToArray();
		}

		/// <summary>
		/// Gets an array of all the ToolStrips contained in a particular form.
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public static ToolStrip[] GetToolStripsOnForm(Form f)
		{
			return GetItemsOnForm<ToolStrip>(f);
		}
		#endregion Child Enumerators
		[Scenario(false)]
		public static int TestAddItem(TParams p, ToolStrip wb, ToolStripItem itm, ref ScenarioResult scr)
		{
			p.log.WriteLine("Add a {0} to the ToolStrip", itm.GetType().ToString());

			int prevCount = wb.Items.Count;

			scr.IncCounters(prevCount >= 0, "FAIL: previous item count is negative", p.log);

			int index = wb.Items.Add(itm);

			p.log.WriteLine("Item added at index {0}", index.ToString());
			scr.IncCounters(index >= 0 && index < wb.Items.Count, "FAIL: Adding an item to the ToolStrip failed to generate a valid index", p.log);
			scr.IncCounters(wb.Items.Contains(itm), "FAIL: Adding an item to the ToolStrip failed", p.log);
			scr.IncCounters(wb.Items.IndexOf(itm) == index, "FAIL: Index of added item incorrect", p.log);
			scr.IncCounters(wb.Items.Count == prevCount + 1, "FAIL: collection count did not increase by 1", p.log);
			return index;
		}

	}
}
