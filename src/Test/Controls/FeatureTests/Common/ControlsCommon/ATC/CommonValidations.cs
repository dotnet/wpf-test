//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------


#region Using Statements

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

using Microsoft.Test.Controls;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;

using Avalon.Test.ComponentModel.Utilities;

#endregion

namespace Avalon.Test.ComponentModel.Validations
{
    /// <summary>
    /// It has the following responsibilities:
    /// 1. Adapter for IValidation and ITestStep
    ///     Any validation deriving from this class can operate both as an IValidation and an ITestStep
    /// 2. It separates the parsing and the validation logic.
    ///     Any class deriving this class should have a IXmlParser attribute to parse its properties.
    ///     So that the parsing work will be done the attached IXmlParser attributes.
    ///     The validation logic will be done by the class self.
    /// </summary>
    [IsExtension]
    public abstract class ValidationAdapter : TestObject, IValidation
    {
        #region IValidation Members

        bool IValidation.Validate(params object[] validationParams)
        {
            this.validationParams = validationParams;
            return Assert.RunTest(Run);
        }

        private void Run()
        {
            Assert.AssertFalse("Invalid Parameters",
                validationParams == null || validationParams.Length == 0);

            object testObject = validationParams[0];
            object[] parameters = new object[validationParams.Length - 1];
            if (validationParams.Length > 1)
            {
                for (int i = 0; i < parameters.Length; ++i)
                {
                    parameters[i] = validationParams[i + 1];
                }
            }

            Validate(testObject, parameters);
        }

        private object[] validationParams;

        private void Validate(object testObject, params object[] validationParams)
        {
            XmlElement validationXml = validationParams[0] as XmlElement;
            Assert.AssertTrue("validation xml is null", validationXml != null);
            XmlParserHelper.Parse(this, validationXml);
            ((ITestStep)this).Do(testObject);
        }

        #endregion
    }

    /// <summary>
    /// dispatcher validation
    /// it will use finder to find object from the source testObject and then execute steps in Steps against it.
    /// for example, if a control is like the belowing
    /// <code>
    ///     <StackPanel>
    ///         <TextBlock Name="testObject" Text="Hello" FontSize="24"/>
    ///     </StackPanel>
    /// </code>
    /// then you can verify the TextBlock using the steps.
    /// set Finder to a LogicalNameFinider with NameToFind=testObject
    /// and then add test steps into Steps property.
    /// </summary>
    [InnerPropertyParser("Validations", ExtraProperty = "Finder")]
    [ContentProperty("Validations")]
    public class DispatcherValidation : ValidationAdapter
    {
        protected override void DoCore()
        {
            object target = Finder.Find(Target);
            foreach (ITestStep step in Validations)
            {
                step.Do(target);
            }
        }

        #region Finder

        public IFinder Finder
        {
            get { return _finder; }
            set { _finder = value; }
        }

        private IFinder _finder;

        #endregion

        #region Validations

        public List<ITestStep> Validations
        {
            get { return _validations; }
        }

        private List<ITestStep> _validations = new List<ITestStep>();

        #endregion
    }

    /// <summary>
    /// validation for object properties
    /// It will test that:
    /// 1. the test object is a of type of TyeName
    /// 2. every property will have the correct value
    /// </summary>
    [ObjectPropertyParser("TypeName", "Properties")]
    [ContentProperty("Properties")]
    public sealed class ObjectPropertyValidation : ValidationAdapter
    {
        protected override void DoCore()
        {
            // verify testObject is correct
            Assert.AssertTrue("testObject is null", Target != null);

            // find the validat type with which it verifies every property
            Type validateType = null;
            for (Type cur = Target.GetType(); cur != null; cur = cur.BaseType)
            {
                if (cur.FullName.EndsWith(TypeName))
                {
                    validateType = cur;
                    break;
                }
            }
            Assert.AssertTrue("testObject is not of type of " + TypeName, validateType != null);

            // verify each property has the correct value
            foreach (Checker checker in Properties)
            {
                checker.Check(Target);
            }
        }

        #region TypeName

        public string TypeName
        {
            get { return _typeName; }
            set { _typeName = value; }
        }

        private string _typeName;

        #endregion

        #region Properties

        public List<Checker> Properties
        {
            get { return _properties; }
        }

        private List<Checker> _properties = new List<Checker>();

        #endregion
    }

    /// <summary>
    /// property checker
    /// check the property has the expected value
    /// </summary>
    public class Checker : TestObject
    {
        #region Constructor

        public Checker(string property, object value)
        {
            Property = property;
            Value = value;
        }

        public Checker()
        {
        }

        #endregion

        #region Property

        public string Property
        {
            get { return _property; }
            set { _property = value; }
        }

        private string _property;

        #endregion

        #region Value

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(Checker));

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion

        public void Check(object testObject)
        {
            Assert.AssertTrue("testObject is null", testObject != null);
            Assert.AssertTrue(Property + "Exp.[" + Value + "]",
                ControlTestHelper.VerifyObjectProperty(testObject, Property, Value));
        }

        protected override void DoCore()
        {
            Check(Target);
        }
    }

    /// <summary>
    /// validation for object is null or not
    /// it will fail if testObject is not null.
    /// </summary>
    public sealed class NullValidation : ValidationAdapter
    {
        protected override void DoCore()
        {
            GlobalLog.LogStatus("testObject Exp.[" + null + "] Act.[" + Target + "]");
            Assert.AssertTrue("testObject is not null", Target == null);
        }
    }

    /// <summary>
    /// Verify that a child of the testObject is visilbe or not.
    /// </summary>
    [XmlContentParser("Child")]
    public sealed class VisibleValidation : ValidationAdapter
    {
        protected override void DoCore()
        {
            FrameworkElement parent = Parent;
            Assert.AssertTrue("Parent is null", parent != null);
            FrameworkElement child = Child;
            Assert.AssertTrue("Child is null", child != null);
            Assert.AssertEqual("Visible", Visible, ControlTestHelper.ContainFramworkElement(parent, child));
        }

        #region Child

        public FrameworkElement Child
        {
            get { return (FrameworkElement)GetProperty(ChildProperty); }
            set { SetProperty(ChildProperty, value); }
        }

        public static readonly DependencyProperty ChildProperty = Register("Child", typeof(FrameworkElement), typeof(VisibleValidation));

        #endregion

        #region Parent

        public FrameworkElement Parent
        {
            get { return (FrameworkElement)GetProperty(ParentProperty); }
            set { SetProperty(ParentProperty, value); }
        }

        public static readonly DependencyProperty ParentProperty = Register(
            "Parent",
            typeof(FrameworkElement),
            typeof(VisibleValidation),
            new PropertyMetadata(new Finding()));

        #endregion

        #region Visible

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        private bool _visible = true;

        #endregion
    }

    public enum Position
    {
        Top,
        Bottom,
        Left,
        Right
    }

    /// <summary>
    /// Verify two UIElements have the expected distance.
    /// </summary>
    public sealed class DistanceValidation : ValidationAdapter
    {
        #region First

        public static readonly DependencyProperty FirstProperty = Register("First", typeof(UIElement), typeof(DistanceValidation));

        public UIElement First
        {
            get { return (UIElement)GetProperty(FirstProperty); }
            set { SetProperty(FirstProperty, value); }
        }

        #endregion

        #region Second

        public static readonly DependencyProperty SecondProperty = Register("Second", typeof(UIElement), typeof(DistanceValidation));

        public UIElement Second
        {
            get { return (UIElement)GetProperty(SecondProperty); }
            set { SetProperty(SecondProperty, value); }
        }

        #endregion

        #region Distance

        public int Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }

        private int _distance = 0;

        #endregion

        #region Position

        private Position _position = Position.Left;

        public Position Position
        {
            get { return _position; }
            set { _position = value; }
        }

        #endregion

        #region Tolerance

        private int _tolerance = 1;

        public int Tolerance
        {
            get { return _tolerance; }
            set { _tolerance = value; }
        }

        #endregion

        protected override void DoCore()
        {
            UIElement firstElement = First;
            UIElement secondElement = Second;

            Assert.AssertTrue("First element is null", firstElement!= null);
            Assert.AssertTrue("Second element is null", secondElement!= null);

            Rectangle firstRectangle = ImageUtility.GetScreenBoundingRectangle(firstElement);
            Rectangle secondRectangle = ImageUtility.GetScreenBoundingRectangle(secondElement);

            int expectedDistance = DpiHelper.GetScreenLength(Distance);

            switch (Position)
            {
                case Position.Left:
                    {
                        int actualDistance = secondRectangle.Left - firstRectangle.Left;
                        Assert.AssertTrue("First.Left:[" + firstRectangle.Left + "] Second.Left:[" + secondRectangle.Left + "] Distance Exp.[" + expectedDistance + "] Act.[" + actualDistance + "]",
                            Math.Abs(expectedDistance - actualDistance) <= Tolerance);
                        break;
                    }
                case Position.Right:
                    {
                        int actualDistance = secondRectangle.Right - firstRectangle.Right;
                        Assert.AssertTrue("First.Right:[" + firstRectangle.Right + "] Second.Right:[" + secondRectangle.Right + "] Distance Exp.[" + expectedDistance + "] Act.[" + actualDistance + "]",
                            Math.Abs(expectedDistance - actualDistance) <= Tolerance);
                        break;
                    }
                case Position.Top:
                    {
                        int actualDistance = secondRectangle.Top - firstRectangle.Top;
                        Assert.AssertTrue("First.Top:[" + firstRectangle.Top + "] Second.Top:[" + secondRectangle.Top + "] Distance Exp.[" + expectedDistance + "] Act.[" + actualDistance + "]",
                            Math.Abs(expectedDistance - actualDistance) <= Tolerance);
                        break;
                    }
                case Position.Bottom:
                    {
                        int actualDistance = secondRectangle.Bottom - firstRectangle.Bottom;
                        Assert.AssertTrue("First.Bottom:[" + firstRectangle.Bottom + "] Second.Bottom:[" + secondRectangle.Bottom + "] Distance Exp.[" + expectedDistance + "] Act.[" + actualDistance + "]",
                            Math.Abs(expectedDistance - actualDistance) <= Tolerance);
                        break;
                    }
                default:
                    throw new ArgumentException("Invalid Position value:" + Position);
            }
        }
    }
}
