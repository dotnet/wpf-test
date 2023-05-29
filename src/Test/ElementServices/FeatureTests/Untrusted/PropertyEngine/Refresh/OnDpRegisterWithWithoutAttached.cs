// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshOnDpRegisterWithWithoutAttachedTest
{
    /******************************************************************************
    * CLASS:          OnDpRegisterWithWithoutAttached
    ******************************************************************************/
    /// <summary>
    /// Only new APIs are covered here. They include
    /// (1) DependencyProperty.Register methods:
    ///     Register(name, propertyType, ownerType)
    ///     Register(name, propertyType, ownerType, typeMetadata)
    ///     Register(name, propertyType, ownerType, typeMetadata, validateValueCallback)
    /// (2) new overload of AddOwner. It is a convenience method that will automatically OverrideMetadata on your behalf:
    ///     AddOwner(ownerType, typeMetadata)
    /// </summary>
    [Test(0, "PropertyEngine.Style", TestCaseSecurityLevel.FullTrust, "OnDpRegisterWithWithoutAttached")]
    public class OnDpRegisterWithWithoutAttached : TestCase
    {
        #region Constructor
        /******************************************************************************
        * Function:          OnDpRegisterWithWithoutAttached Constructor
        ******************************************************************************/
        public OnDpRegisterWithWithoutAttached()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            TestOnDpRegisterWithWithoutAttachedPass2 test2 = new TestOnDpRegisterWithWithoutAttachedPass2();
            TestOnDpRegisterWithWithoutAttached test = new TestOnDpRegisterWithWithoutAttached();

            Utilities.StartRunAllTests("OnDpRegisterWithWithoutAttached");
            test2.TestRegister();
            test.TestAddOwner();
            Utilities.StopRunAllTests();

            return TestResult.Pass;
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          TestOnDpRegisterWithWithoutAttached
    ******************************************************************************/
    /// <summary></summary>
    public class TestOnDpRegisterWithWithoutAttached
    {
        /// <summary>
        /// One Dp
        /// </summary>
        public static readonly DependencyProperty MyOwnProperty = DependencyProperty.RegisterAttached("MyOwn", typeof(int), typeof(TestOnDpRegisterWithWithoutAttached), new PropertyMetadata(12345));

        /******************************************************************************
        * Function:          TestAddOwner
        ******************************************************************************/
        /// <summary>
        /// Test AddOwner() method
        /// </summary>
        public void TestAddOwner()
        {
            Utilities.PrintTitle("Test AddOwner(ownerType, typeMetadata). It essentially covers AddOwner(ownerType)");
            Utilities.PrintStatus("(1) ownerType is null");

            PropertyMetadata metadata1 = new PropertyMetadata("Default Value");

            try
            {
                DockPanel.DockProperty.AddOwner(null, metadata1);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            Utilities.PrintStatus("(2) PropertyAlreadyRegistered");
            try
            {
                MyOwnProperty.AddOwner(typeof(TestOnDpRegisterWithWithoutAttached), metadata1);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            Utilities.PrintStatus("(3.1) OverrideMetadata failed as type is not DO derived");
            try
            {
                DockPanel.DockProperty.AddOwner(typeof(TestOnDpRegisterWithWithoutAttached), metadata1);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            Utilities.PrintStatus("(3.2) OverrideMetadata failed due to OverridingMedatadataDoesNotMatchBaseMetadataType");
            try
            {
                AvalonObject.AddOwnerFailure();
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            Utilities.PrintStatus("(4) Success case when an owner is added");
            AvalonObject.AddOwnerSuccess();
        }
    }


    /******************************************************************************
    * CLASS:          AvalonObject
    ******************************************************************************/
    /// <summary>
    /// Helper class for TestOnDpRegisterWithWithoutAttached
    /// </summary>
    public class AvalonObject : DependencyObject
    {
        /// <summary>
        /// Used to test DP.Register
        /// </summary>
        public static readonly DependencyProperty AvalonProperty = DependencyProperty.Register("Avalon", typeof(int), typeof(AvalonObject), new PropertyMetadata(1978));

       /// <summary>
        /// This method is called by TestAddOwner to provide a failure example
        /// </summary>
        public static void AddOwnerFailure()
        {
            DockPanel.DockProperty.AddOwner(typeof(AvalonObject), new PropertyMetadata(Dock.Right));
        }

        /// <summary>
        /// This method is called by TestAddOwner to provide a success example
        /// </summary>
        public static void AddOwnerSuccess()
        {
            DockPanel.DockProperty.AddOwner(typeof(AvalonObject), new FrameworkPropertyMetadata(Dock.Right));
            ListBox.SelectionModeProperty.AddOwner(typeof(AvalonObject), new FrameworkPropertyMetadata(SelectionMode.Multiple));
        }
    }

    /******************************************************************************
    * CLASS:          TestOnDpRegisterWithWithoutAttachedPass2
    ******************************************************************************/
    /// <summary>
    /// Add a new class that is derived from DependencyObject
    /// </summary>
    public class TestOnDpRegisterWithWithoutAttachedPass2 : DependencyObject
    {
        /// <summary>
        /// Call base with (null, true)
        /// </summary>
        public TestOnDpRegisterWithWithoutAttachedPass2()
        {
        }

        /******************************************************************************
        * Function:          TestRegister
        ******************************************************************************/
        /// <summary>
        /// Test Register() Methods
        /// </summary>
        public void TestRegister()
        {
            Utilities.PrintTitle("Test DependencyProperty.Register() methods");
            Utilities.PrintStatus("(1) Success for Register(name, propertyType, ownerType)");
            ValidateTestRegister();
        }

        /******************************************************************************
        * Function:          ValidateTestRegister
        ******************************************************************************/
        private object ValidateTestRegister()
        {
            DependencyProperty Case1Property = DependencyProperty.Register("Case1", typeof(int), typeof(TestOnDpRegisterWithWithoutAttachedPass2));

            Utilities.Assert((int)Case1Property.DefaultMetadata.DefaultValue == 0, "(int)Case1Property.DefaultMetadata.DefaultValue == 0");
            Utilities.PrintStatus("(2) Success for Register(name, propertyType, ownerType, typeMetadata)");

            DependencyProperty Case2Property = DependencyProperty.Register("Case2", typeof(int), typeof(TestOnDpRegisterWithWithoutAttachedPass2), new PropertyMetadata(12321));
            Utilities.Assert((int)Case2Property.DefaultMetadata.DefaultValue == 12321, "(int)Case2Property.DefaultMetadata.DefaultValue == 0");

            DependencyObject d = new DependencyObject();
            int myValue1 = (int)(d.GetValue(Case2Property));
            TestOnDpRegisterWithWithoutAttachedPass2 dd = new TestOnDpRegisterWithWithoutAttachedPass2();
            int myValue2 = (int)(dd.GetValue(Case2Property));

            Utilities.Assert(myValue1 == 12321, "myValue1 == 12321");
            Utilities.Assert(myValue2 == 12321, "myValue2 == 12321");
            Utilities.PrintStatus("(3) Success for Register(name, propertyType, ownerType, typeMetadata, validateValueCallback)");

            DependencyProperty Case3Property = DependencyProperty.Register("Case3", typeof(int), typeof(TestOnDpRegisterWithWithoutAttachedPass2), new PropertyMetadata(12), new ValidateValueCallback(validateCase3));

            Utilities.Assert((int)Case3Property.DefaultMetadata.DefaultValue == 12, "(int)Case3Property.DefaultMetadata.DefaultValue == 12"); 
            myValue1 = (int)(d.GetValue(Case3Property));
            myValue2 = (int)(dd.GetValue(Case3Property));
            Utilities.Assert(myValue1 == 12, "myValue1 == 12"); //Issue: 915915 is fixed
            Utilities.Assert(myValue2 == 12, "myValue2 == 12");
            Utilities.PrintStatus("(4) Failure for Register due to invalid default value");
            try
            {
                DependencyProperty Case4Property = DependencyProperty.Register("Case4", typeof(int), typeof(TestOnDpRegisterWithWithoutAttachedPass2), new PropertyMetadata(0), new ValidateValueCallback(validateCase3));

                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            return null;
        }

        /******************************************************************************
        * Function:          validateCase3
        ******************************************************************************/
        /// <summary>
        /// value can only be between 10 and 20
        /// </summary>
        /// <param name="obj">integer to validate</param>
        /// <returns></returns>
        private bool validateCase3(object obj)
        {
            int i = (int)obj;

            if (i < 10 || i > 20)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

#region Reference 
/*
(1) DP.Register must has containing class that is derived from DependencyObject\,
while DP.RegisterAttached has no cuch requirement;
(2) DP.Register first called RegisterAttached (defaultMetadata always null) and then 
call RegisterAttached;

-----------------------------------EMail Discussion -----------------------------
Thanks for your explanation, Mark. 

As for 4), I do not see urgent need to know the registration differences. But I could use it if such information is provided. 

For example, if I write a SDK tool that lists AttachedProperty (such as DockPanel.DockProperty) for special documentation, I only need to worry about those DPs that are registered with RegisterAttached API. (It should eventually be the guideline that one class should not use RegisterAttached for its property it that property is supposed to be used in the class itself.)

Zhanbo
_____________________________________________
From: Mark Finocchio 
Sent: Wednesday, November 19, 2003 1:50 PM
To: Namita Gupta; Zhanbo Sun; Mike Hillberg; Nick Kramer; Jeff Bogdan; S. Ramani
Subject: RE: Property registration API changes

1) It means the class that registered it is expecting to use it in more places than on itself. 
2) a. You get 0 (good question J), b. You get 12321
3) Correct
4) There is no way to tell. Do people need to know?
5) These changes are just about convenience. All properties are still attachable. However, some classes register properties to be used by themselves for themselves. In this case, they register and they immediately override metadata. Register simply combines these 2 calls. That same property can be set anywhere;

- Mark

_____________________________________________
From: Namita Gupta 
Sent: Wednesday, November 19, 2003 1:39 PM
To: Zhanbo Sun; Mark Finocchio; Mike Hillberg; Nick Kramer; Jeff Bogdan; S. Ramani
Subject: RE: Property registration API changes

Adding Ramani;
They look good to me

_____________________________________________
From: Zhanbo Sun 
Sent: Wednesday, November 19, 2003 1:40 PM
To: Mark Finocchio; Namita Gupta; Mike Hillberg; Nick Kramer; Jeff Bogdan
Subject: RE: Property registration API changes

Questions
(1) Attached in RegisterAttached: does this name indicate that this Dependency Property is supposed to be used as Attached Property?

(2) After this definition,
Public MyButton : DependencyObject{
public static readonly DependencyProperty TestProperty = DependencyProperty.Register(Test, typeof(int), typeof(MyButton), 12321);
}

What will an object of a class other than MyButton get for: GetValue(MyButton.TestProperty)? Is it 0? Or get exception?
What will an object of a class that derives from MyButton get for GetValue(MyButton.TestProperty)? It should be 12321, right?

(3) Register and RegisterAttached cannot register two DPs with the same name and owning type, is that correct? 

(4) Given a DP, what is the easiest way to know whether or not it is registered with RegisterAttached? 

(5) When you say In the cases where a client code now has both a call to RegisterAttached as well as a call to OverrideMetadata, these areas will be converted over to a single call to Register at a later time. Why RegisterAttached + OverrideMetadata == Register? If I do Register, it has different meaning from RegisterAttached right? Or are we saying that it is very rare to RegisterAttached and OverrideMetadata at the same time?

Thanks,

Zhanbo
_____________________________________________
From: Mark Finocchio 
Sent: Wednesday, November 19, 2003 1:24 PM
To: Namita Gupta; Mike Hillberg; Nick Kramer; Jeff Bogdan; Zhanbo Sun; Mark Finocchio
Subject: Property registration API changes

Here are the changes Ive made. If everyone is happy with them, I can continue to update all the client code, update the PS item and go to WAR;

Let me know

Thanks,

- Mark


Breaking change description:

The Register APIs on DependencyProperty have been changed based on customer feedback. 

Previously, the metadata passed to Register was default metadata (meaning, this metadata was used when the property was queried on any type;

Now, the metadata passed in is to be used as per-type metadata (meaning, the metadata is to be used only when the property is queried on the same type that registered the property). For this reason, the defaultMetadata parameter was renamed to typeMetadata (this, of course, has no implication on the function signature itself):

    public static DependencyProperty Register(string name, Type propertyType, Type ownerType)

    public static DependencyProperty Register(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata)

    public static DependencyProperty Register(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata, ValidateValueCallback validateValueCallback)


The previous behavior of Register is still available through new RegisterAttached methods:

    public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType)

    public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType, PropertyMetadata defaultMetadata)

    public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType, PropertyMetadata defaultMetadata, ValidateValueCallback validateValueCallback)


Lastly, a new override of AddOwner has been added (as a convenience) that will automatically OverrideMetadata on your behalf:

    public DependencyProperty AddOwner(Type ownerType, PropertyMetadata typeMetadata)


As to not break existing client code behavior, all calls to Register have been replaced with RegisterAttached. In the cases where a client code now has both a call to RegisterAttached as well as a call to OverrideMetadata, these areas will be converted over to a single call to Register at a later time;




*/
#endregion
