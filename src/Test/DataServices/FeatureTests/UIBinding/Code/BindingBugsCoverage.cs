// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// - Makes sure that when binding to a hierarchy of subproperties, changing a subproperty in the middle
    /// of that hierarchy and calling UpdateTarget refreshes the target as expected.
    /// - Tests that it is possible to data binding a property in an FE inside a VisualBrush to something
    /// outside of the VisualBrush.
    /// - Verifies that when having a binding in a tree of freezables, the TargetUpdated event is fired
    /// in the nearest FrameworkElement (the mentor).
    /// - Verifies that it is possible to bind a property of a custom control to a sibling control
    /// - Verified that a multibinding with an "Active" binding and a "PathError" binding has Status "Active"
    /// (and not "Unattached").
    /// </description>
    /// </summary>
    [Test(3, "Binding", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class BindingBugsCoverage : XamlTest
    {
        private Page _page;

        public BindingBugsCoverage()
            : base(@"BindingBugsCoverage.xaml")
        {
            InitializeSteps += new TestStep(Setup);

             
            RunSteps += new TestStep(OverrideGenericPropertyWithNonGeneric);
             
            RunSteps += new TestStep(SerializeBinding);
             
            RunSteps += new TestStep(UpdateTargetMiddleProperty);
             
            RunSteps += new TestStep(AncestorTypeInsideVisualBrush);
             
            RunSteps += new TestStep(TargetUpdatedFiredInMentor);
             
            RunSteps += new TestStep(BindToCustomControl);
             
            RunSteps += new TestStep(MultiBindingExpressionStatus);
        }

         
        TestResult OverrideGenericPropertyWithNonGeneric()
        {
            TextBlock tb = new TextBlock();
            Binding b = new Binding("Id");
            b.Source = new NonGenericSubClass();
            tb.SetBinding(TextBlock.TextProperty, b);

            if (tb.Text != "1") return TestResult.Fail;

            return TestResult.Pass;
        }

         
        TestResult SerializeBinding()
        {
            // Saving a new binding was sufficient to cause an exception
            XamlWriter.Save(new Binding());

            return TestResult.Pass;
        }

        TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.SystemIdle);
            _page = this.Window.Content as Page;
            if (_page == null)
            {
                LogComment("Fail - Page is null");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

         

        TestResult UpdateTargetMiddleProperty()
        {
            Status("UpdateTargetMiddleProperty");

            TextBlock tb = (TextBlock)(LogicalTreeHelper.FindLogicalNode(_page, "tb"));

            MySourceA src = (MySourceA)(_page.Resources["src"]);
            src.A.B = new MySourceC(5);
            BindingExpression be = tb.GetBindingExpression(TextBlock.TextProperty);
            be.UpdateTarget();

            WaitForPriority(DispatcherPriority.SystemIdle);

            if (!(tb.Text.Equals("5")))
            {
                LogComment("Fail - TextBlock does not have the expected value (5). Actual: " + tb.Text);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

         
        TestResult AncestorTypeInsideVisualBrush()
        {
            Status("AncestorTypeInsideVisualBrush");

            Button btn = (Button)(LogicalTreeHelper.FindLogicalNode(_page, "btn"));
            VisualBrush vb = (VisualBrush)(btn.Background);
            Button innerButton = (Button)(vb.Visual);
            if (!(innerButton.Content.Equals(btn.Content)))
            {
                LogComment("Fail - Inner Button's expected content: " + btn.Content + ". Actual: " + innerButton.Content);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

         

        TestResult TargetUpdatedFiredInMentor()
        {
            Status("TargetUpdatedFiredInMentor");

            // hook up event for Animation Completed
            Viewport3D vp3D = (Viewport3D)(LogicalTreeHelper.FindLogicalNode(_page, "vp"));
            EventTrigger trigger = (EventTrigger)(vp3D.Triggers[0]);
            BeginStoryboard bs = (BeginStoryboard)(trigger.Actions[0]);
            Storyboard storyboard = bs.Storyboard;
            DoubleAnimation animation = (DoubleAnimation)(storyboard.Children[0]);
            animation.Completed += new EventHandler(animation_Completed);

            // change source 
            Player player = (Player)(_page.Resources["player"]);
            player.Votes = 2;

            // update binding
            ModelVisual3D mv3D = (ModelVisual3D)(vp3D.Children[0]);
            Model3DGroup m3Dg = (Model3DGroup)(mv3D.Content);
            GeometryModel3D gm3D = (GeometryModel3D)(m3Dg.Children[3]);
            ScaleTransform3D st3D = (ScaleTransform3D)(gm3D.Transform);
            BindingExpression be = BindingOperations.GetBindingExpression(st3D, ScaleTransform3D.ScaleYProperty);
            be.UpdateTarget();

            // wait fo Animation to complete
            TestResult animationCompleted = WaitForSignal("AnimationCompleted");
            if (animationCompleted != TestResult.Pass)
            {
                LogComment("Fail - Problem when waiting for the 3D animation to complete");
                return TestResult.Fail;
            }

            // make sure binding target is as expected
            if (st3D.ScaleY != player.Votes)
            {
                LogComment("Fail - Actual ScaleTransform3D's ScaleY: " + st3D.ScaleY + ". Expected: " + player.Votes);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        void animation_Completed(object sender, EventArgs e)
        {
            Signal("AnimationCompleted", TestResult.Pass);
        }

         
        TestResult BindToCustomControl()
        {
            Status("BindToCustomControl");

            HappyMan pelle = (HappyMan)(LogicalTreeHelper.FindLogicalNode(_page, "pelle"));
            Util.AssertEquals(pelle.Width, 100d);

            return TestResult.Pass;
        }

         
        TestResult MultiBindingExpressionStatus()
        {
            Status("MultiBindingExpressionStatus");
            TextBlock tb1 = (TextBlock)(LogicalTreeHelper.FindLogicalNode(_page, "tb1"));
            TextBlock tb2 = (TextBlock)(LogicalTreeHelper.FindLogicalNode(_page, "tb2"));
            Util.AssertEquals(tb1.Text, tb2.Text);

            MultiBindingExpression mbe = BindingOperations.GetMultiBindingExpression(tb1, TextBlock.TextProperty);
            BindingExpression be1 = (BindingExpression)(mbe.BindingExpressions[0]);
            BindingExpression be2 = (BindingExpression)(mbe.BindingExpressions[1]);
            Util.AssertEquals(mbe.Status, BindingStatus.Active); // used to be Unattached
            Util.AssertEquals(be1.Status, BindingStatus.PathError);
            Util.AssertEquals(be2.Status, BindingStatus.Active);

            return TestResult.Pass;
        }

        public class GenericBaseClass<T> : INotifyPropertyChanged
        {
            public virtual T Id { get { return default(T); } }

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            // We don't actually need to use the PropertyChanged event for the regression test,
            // but if we don't use it somewhere we get a compiler warning as error.
            private void MakeCompilerWarningGoAway()
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Foo"));
            }
        }

        public class NonGenericSubClass : GenericBaseClass<int>
        {
            public override int Id { get { return 1; } }
        }

    }
}
