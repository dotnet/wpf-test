//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------


#region Using Statements

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Windows.Markup;
using System.Windows.Controls.Primitives;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;
using Avalon.Test.ComponentModel.Actions.Internals;
using Microsoft.Test.Input;

#endregion


namespace Avalon.Test.ComponentModel.Actions
{
    /// <summary>
    /// 1. Adapter for IAction and ITestStep
    ///     Any classes deriving from this one can both be an IAction and an ITestStep.
    ///     So it can be used traditionaly and also can be used in a new framework.
    /// 2. Split the parsing work from the action logic
    ///     the parsing will be done by the attached IXmlParser attribute
    ///     the action logic will be done at the Do() function
    /// </summary>
    [IsExtension]
    public abstract class ActionAdapter : TestObject, IAction
    {
        #region IAction Members

        void IAction.Do(FrameworkElement frmElement, params object[] actionParams)
        {
            if (actionParams == null || actionParams.Length != 1 || !(actionParams[0] is XmlElement))
                throw new ArgumentException("actionParams not valid");

            Do(frmElement, (XmlElement)actionParams[0]);
        }

        private void Do(FrameworkElement testObject, XmlElement actionXml)
        {
            XmlParserHelper.Parse(this, actionXml);
            ((ITestStep)this).Do(testObject);
        }

        #endregion
    }

    public class MouseClickAction : IAction
    {
        #region IAction Members

        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            UserInput.MouseLeftClickCenter(frmElement);
        }

        #endregion
    }

    /// <summary>
    /// dispatcher action
    /// it will use finder to find uielement and then execute steps against the object.
    /// </summary>
    [InnerPropertyParser("Actions", ExtraProperty = "Finder")]
    [ContentProperty("Actions")]
    public class DispatcherAction : ActionAdapter
    {
        #region ActionAdapter Members

        protected override void DoCore()
        {
            object target = Finder.Find(Target);
            foreach (ITestStep step in Actions)
            {
                step.Do(target);
                QueueHelper.WaitTillQueueItemsProcessed();
            }
        }

        #endregion

        #region Finder

        public IFinder Finder
        {
            get { return _finder; }
            set { _finder = value; }
        }

        private IFinder _finder;

        #endregion

        #region Actions

        public List<ITestStep> Actions
        {
            get { return _actions; }
        }

        private List<ITestStep> _actions = new List<ITestStep>();

        #endregion
    }


    /// <summary>
    /// let framework element gain focus
    /// </summary>
    public class FocusAction : ActionAdapter
    {
        #region ActionAdapter Members

        protected override void DoCore()
        {
            FrameworkElement frmElement = Target as FrameworkElement;
            if (frmElement == null)
                throw new ArgumentNullException("frmElement");
            frmElement.Focus();
        }

        #endregion
    }

    /// <summary>
    /// Set value a property of the test object
    /// Value can be set in two different ways:
    /// either by
    /// <code>
    ///     <PropertySetAction PropertyName="Background" Value="Red"/>
    /// </code>
    /// or
    /// <code>
    ///     <PropertySetAction PropertyName="View">
    ///         <GridView xmlns="http://schemas.microsoft.com/winfx/avalon/2005">
    ///             <GridViewColumn Header='Day'/>
    ///         </GridView>
    ///     </PropertySetAction>
    /// </code>
    /// the format will be parsed by the XamlContentParser.
    /// </summary>
    [XamlContentParser("Value")]
    [ContentProperty("Value")]
    public class PropertySetAction : ActionAdapter
    {
        #region PropertyName

        public string PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

        private string _propertyName;

        #endregion

        #region Value

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private object _value;

        #endregion

        #region ActionAdapter Members

        protected override void DoCore()
        {
            Assert.AssertTrue("Target should not be null", Target != null);

            String propertyName = PropertyName;
            Assert.AssertTrue("PropertyName not specified", !string.IsNullOrEmpty(propertyName));

            string[] path = ControlTestHelper.FromPathToDotPath(propertyName);
            object obj = ControlTestHelper.GetObjectByDotPath(Target, path);
            int length = path.Length;
            GlobalLog.LogStatus("Object : " + obj);

            PropertyInfo pi = obj.GetType().GetProperty(path[length - 1], BindingFlags.Instance | BindingFlags.Public);

            Assert.AssertTrue("Property " + propertyName + " not found in " + Target.GetType(), pi != null);

            object setValue = null;
            if (Value != null && !pi.PropertyType.IsAssignableFrom(Value.GetType()))
            {
                setValue = XmlHelper.ConvertToType(pi.PropertyType, Value);
            }
            else
            {
                setValue = Value;
            }

            pi.SetValue(obj, setValue, new object[0]);
        }

        #endregion
    }

    /// <summary>
    /// Add object to FrameworkElement.Resources with a key
    /// </summary>
    [XamlContentParser("Value")]
    [ContentProperty("Value")]
    public class AddResourcesAction : ActionAdapter
    {
        #region Key

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        private string _key;

        #endregion

        #region Value

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private object _value;

        #endregion

        #region ActionAdapter Members

        protected override void DoCore()
        {
            FrameworkElement frmElement = Target as FrameworkElement;
            Assert.AssertTrue("testObject is not a FrameworkElement", frmElement != null);
            frmElement.Resources.Add(Key, Value);
        }

        #endregion
    }

    /// <summary>
    /// remove an object with the key from FrameworkElement.Resources
    /// </summary>
    [PropertyParser]
    public class RemoveResourcesAction : ActionAdapter
    {
        #region ActionAdapter Members

        protected override void DoCore()
        {
            FrameworkElement frmElement = Target as FrameworkElement;
            Assert.AssertTrue("testObject is not a FrameworkElement", frmElement != null);
            frmElement.Resources.Remove(Key);
        }

        #endregion

        #region Key

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        private string _key;

        #endregion
    }

    /// <summary>
    /// the action will move property's value between different objects.
    /// </summary>
    [DoubleXmlContentParser("First", "Second")]
    public class PropertySwapAction : ActionAdapter
    {
        #region ActionAdapter Members

        protected override void DoCore()
        {
            string propertyName = PropertyName;
            Assert.AssertTrue("PropertyName not specified", !string.IsNullOrEmpty(propertyName));

            object[] objs = new object[] { First, Second };

            PropertyInfo the1stInfo = objs[0].GetType().GetProperty(propertyName);
            object the1stValue = the1stInfo.GetValue(objs[0], new object[0]);

            PropertyInfo the2ndInfo = objs[1].GetType().GetProperty(propertyName);
            object the2ndValue = the2ndInfo.GetValue(objs[1], new object[0]);

            //clear the value of each property
            the1stInfo.SetValue(objs[0], null, new object[0]);
            the2ndInfo.SetValue(objs[1], null, new object[0]);

            the1stInfo.SetValue(objs[0], the2ndValue, new object[0]);
            the2ndInfo.SetValue(objs[1], the1stValue, new object[0]);
        }

        #endregion

        #region First

        public object First
        {
            get { return GetProperty(FirstProperty); }
            set { SetValue(FirstProperty, value); }
        }

        public static readonly DependencyProperty FirstProperty = Register(
            "First",
            typeof(object),
            typeof(PropertySwapAction));

        #endregion

        #region Second

        public object Second
        {
            get { return GetProperty(SecondProperty); }
            set { SetValue(SecondProperty, value); }
        }

        public static readonly DependencyProperty SecondProperty = Register(
            "Second",
            typeof(object),
            typeof(PropertySwapAction));

        #endregion

        #region PropertyName

        public string PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

        private string _propertyName;

        #endregion
    }

    /// <summary>
    /// enum for Widows Theme (classic, luna)
    /// </summary>
    public enum WindowTheme
    {
        Classic = 0,
        Luna = 1,
    }

    /// <summary>
    /// use this action to change windows theme setting
    /// example:
    /// <ChangeThemeAction Theme="Luna"/>
    /// </summary>
    [PropertyParser]
    public sealed class ChangeThemeAction : ActionAdapter
    {
        #region Theme Property

        private WindowTheme _theme;

        public WindowTheme Theme
        {
            set
            {
                _theme = value;
            }
            get
            {
                return _theme;
            }
        }

        #endregion

        #region ActionAdapter Members

        protected override void DoCore()
        {
            ChangeThemesHelper.ChangeTheme(Theme);
        }

        #endregion
    }

    /// <summary>
    /// change grouping action
    /// it will remove all group descriptions and then put the new group descption into it.
    /// </summary>
    [ContentProperty("Group")]
    [XamlContentParser("Group")]
    public class ChangeGroupingAction : ActionAdapter
    {
        #region ActionAdapter Members

        protected override void DoCore()
        {
            Assert.AssertTrue("testObject is not an ICollection<GroupDescription> instance", Target is ICollection<GroupDescription>);
            ICollection<GroupDescription> groups = (ICollection<GroupDescription>)Target;
            groups.Clear();
            if (Group != null)
                groups.Add(Group);
        }

        #endregion

        #region Group

        public GroupDescription Group
        {
            get { return _group; }
            set { _group = value; }
        }

        private GroupDescription _group;

        #endregion
    }

    /// <summary>
    /// action for moving item between collections
    /// Steps:
    /// 1. remove the item with index=FromIndex from From collections
    /// 2. put the item into To collections with index=ToIndex
    /// </summary>
    [DoubleXmlContentParser("From", "To")]
    public sealed class ItemMoveAction : ActionAdapter
    {
        #region From

        public object From
        {
            get { return GetProperty(FromProperty); }
            set { SetProperty(FromProperty, value); }
        }

        public static readonly DependencyProperty FromProperty = Register(
            "From",
            typeof(IList),
            typeof(ItemMoveAction));

        #endregion

        #region To

        public object To
        {
            get { return GetProperty(ToProperty); }
            set { SetProperty(ToProperty, value); }
        }

        public static readonly DependencyProperty ToProperty = Register(
            "To",
            typeof(IList),
            typeof(ItemMoveAction));

        #endregion

        #region FromIndex

        public int FromIndex
        {
            get { return _itemSourceIndex; }
            set { _itemSourceIndex = value; }
        }

        private int _itemSourceIndex;

        #endregion

        #region ToIndex

        /// <summary>
        /// the index of target collections
        /// -1 means that the item will be appened to the tail of the collection
        /// </summary>
        public int ToIndex
        {
            get { return _itemTargetIndex; }
            set { _itemTargetIndex = value; }
        }

        private int _itemTargetIndex;

        #endregion

        #region ActionAdapter Members

        protected override void DoCore()
        {
            Assert.AssertTrue("From is null", From != null);
            Assert.AssertTrue("To is null", To != null);

            IList source = From as IList;
            Assert.AssertTrue("From object is not an IList", source != null);
            IList target = To as IList;
            Assert.AssertTrue("To object is not an IList", target != null);

            Assert.AssertTrue("From index was out of range",
                FromIndex < source.Count && FromIndex >= 0);
            Assert.AssertTrue("To index was out of range",
                ToIndex <= target.Count && ToIndex >= -1);

            object item = source[FromIndex];
            source.RemoveAt(FromIndex);
            if (ToIndex == -1)
                target.Add(item);
            else
                target.Insert(ToIndex, item);
        }

        #endregion
    }

    /// <summary>
    /// Invoke a method against testObject
    /// It can invoke any method with any parameters in test object.
    /// It also can do some return value validations.
    /// Sample:
    /// <!--code>
    ///     <MethodInvoker Method="Move">
    ///         <s:Int32>3</s:Int32>
    ///         <s:Int32>1</s:Int32>
    ///     </MethodInvoker>
    /// </code-->
    /// it will call Add(3, 1).
    /// <code>
    ///     <MethodInvoker Method='ToString' Return='System.Windows.Controls.ListView'/>
    /// </code>
    /// it will call ToString() and verify the return value is 'System.Windows.Controls.ListView'.
    /// </summary>
    [ContentProperty("Parameters")]
    public sealed class MethodInvoker : TestObject
    {
        #region TestObject Members

        protected override void DoCore()
        {
            Assert.AssertTrue("testObject is null", Target != null);
            Assert.AssertTrue("Method is null or empty", !string.IsNullOrEmpty(Method));
            MethodInfo mi = Target.GetType().GetMethod(Method);
            Assert.AssertTrue("Method not exist in " + Target, mi != null);
            object ret = mi.Invoke(Target, Parameters.ToArray());
            if (_isReturnSet)
            {
                Assert.AssertEqual("Return", Return, ret);
            }
        }

        #endregion

        #region Method

        public string Method
        {
            get { return _method; }
            set { _method = value; }
        }

        private string _method;

        #endregion

        #region Parameters

        public List<object> Parameters
        {
            get { return _parameters; }
        }

        private List<object> _parameters = new List<object>();

        #endregion

        #region Return

        public object Return
        {
            get { return _return; }
            set
            {
                _isReturnSet = true;
                _return = value;
            }
        }

        private object _return;

        private bool _isReturnSet = false;

        #endregion
    }

    /// <summary>
    /// Usage: <SetColumnBindingAction Target='{Finding View.Columns[0],TypePath=p:ListView}' Path="Day"/>
    /// </summary>
    public class SetColumnBindingAction : TestObject
    {
        #region TestObject Members

        protected override void DoCore()
        {
            GridViewColumn column = Target as GridViewColumn;
            if (column == null)
            {
                return;
            }
            if (Path != string.Empty)
            {
                Binding bind = new Binding(Path);
                column.DisplayMemberBinding = bind;
            }
            else
            {
                column.DisplayMemberBinding = null;
            }
            return;
        }

        #endregion

        #region Path property
        public string Path
        {
            set
            {
                _strPath = value;
            }
            get
            {
                return _strPath;
            }
        }

        private string _strPath = string.Empty;

        #endregion

    }
}
