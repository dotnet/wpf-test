// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: DRT test suite for Inheritance context
//

using System;
using System.Xml;
using System.Collections;

using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace DRT
{
    public sealed class TestInheritanceContext : DrtTestSuite
    {
        public TestInheritanceContext () : base("InheritanceContext")
        {
            Contact = "Microsoft";
        }

        #region Setup

        public override DrtTest[] PrepareTests()
        {
            // parse, but do not load, the markup
            DRT.LoadXamlFile(@"DrtFiles\PropertyEngine\TestInheritanceContext.xaml", false);

            // some tests need preparation before we load the markup
            PrepareLoadedTests();

            // load the markup
            DRT.ShowRoot();

            return new DrtTest[]
            {
                new DrtTest(VerifyInitialLoaded),
                new DrtTest(Loaded_ChangeVisualBrushContent),
                new DrtTest(Loaded_DetachVisualBrush),
                new DrtTest(Loaded_AttachVisualBrush),

                new DrtTest(VerifyInitialInheritance),
                new DrtTest(Inheritance_ChangeValue),
                new DrtTest(Inheritance_ChangeVisualBrushContent),
                new DrtTest(Inheritance_DetachVisualBrush),
                new DrtTest(Inheritance_AttachVisualBrush),

                new DrtTest(VerifyInitialResources),
                new DrtTest(Resources_ChangeValue),
                new DrtTest(Resources_ChangeVisualBrushContent),
                new DrtTest(Resources_DetachVisualBrush),
                new DrtTest(Resources_AttachVisualBrush),

                new DrtTest(VerifyInitialNameScope),
                new DrtTest(NameScope_ChangeVisualBrushContent),
                new DrtTest(NameScope_DetachVisualBrush),
                new DrtTest(NameScope_AttachVisualBrush),

                new DrtTest(VerifyInitialDataBinding),
                new DrtTest(DataBinding_ChangeValue),
                new DrtTest(DataBinding_ChangeVisualBrushContent),
                new DrtTest(DataBinding_DetachVisualBrush),
                new DrtTest(DataBinding_AttachVisualBrush),

                new DrtTest(VerifyElementNameBinding),

                new DrtTest(VerifyResourcesInTemplate),

                new DrtTest(Verify3DListBox),
            };
        }

        void PrepareLoadedTests()
        {
            // attach Loaded handler to the root of the tree in a VisualBrush
            Page page = (Page)DRT.RootElement;
            FrameworkElement root = (FrameworkElement)page.Content;
            Control control = (Control)DRT.FindVisualByID("VBLoadedMentor", root);
            _vbLoaded = (VisualBrush)control.Background;
            _vbLoadedTB = (TextBlock)_vbLoaded.Visual;
            _vbLoadedTB.Loaded += new RoutedEventHandler(OnTBLoaded);
        }

        #endregion Setup

        #region Tests for Loaded/Unloaded

        // loaded event should fire on the tree in a VisualBrush
        void VerifyInitialLoaded()
        {
            DRT.AssertEqual("Loaded!", _vbLoadedTB.Text, "Initial loaded event did not fire within VisualBrush");
        }

        // replace the content of a VisualBrush.  The old content should get
        // Unloaded, and the new content should get Loaded.
        void Loaded_ChangeVisualBrushContent()
        {
            _oldTextBlock = _vbLoadedTB;
            _oldTextBlock.Unloaded += new RoutedEventHandler(OnTBUnloaded);

            _vbLoadedTB = new TextBlock();
            _vbLoadedTB.Foreground = Brushes.Cyan;
            _vbLoadedTB.Text = "unloaded";
            _vbLoadedTB.Loaded += new RoutedEventHandler(OnTBLoaded);

            _vbLoaded.Visual = _vbLoadedTB;
            DRT.ResumeAt(VerifyLoaded_ChangeVisualBrushContent);
        }

        void VerifyLoaded_ChangeVisualBrushContent()
        {
            DRT.AssertEqual("unloaded", _oldTextBlock.Text, "changing content did not fire Unloaded within VisualBrush");
            DRT.AssertEqual("Loaded!", _vbLoadedTB.Text, "changing content did not fire Loaded within VisualBrush");
        }

        // detach the VisualBrush.  Its content should get Unloaded.
        void Loaded_DetachVisualBrush()
        {
            Control control = (Control)DRT.FindVisualByID("VBLoadedMentor");
            _vbLoadedTB.Unloaded += new RoutedEventHandler(OnTBUnloaded);
            control.ClearValue(Control.BackgroundProperty);
            DRT.ResumeAt(VerifyLoaded_DetachVisualBrush);
        }

        void VerifyLoaded_DetachVisualBrush()
        {
            DRT.AssertEqual("unloaded", _vbLoadedTB.Text, "detaching VisualBrush did not fire Unloaded within VisualBrush");
        }

        // re=attach the VisualBrush.  Its content should get Loaded.
        void Loaded_AttachVisualBrush()
        {
            Control control = (Control)DRT.FindVisualByID("VBLoadedMentor");
            control.Background = _vbLoaded;
            DRT.ResumeAt(VerifyLoaded_AttachVisualBrush);
        }

        void VerifyLoaded_AttachVisualBrush()
        {
            DRT.AssertEqual("Loaded!", _vbLoadedTB.Text, "re-attaching VisualBrush did not fire Loaded within VisualBrush");
        }

        #endregion Tests for Loaded/Unloaded

        #region Tests for property inheritance

        // properties should inherit through VisualBrush into its tree
        void VerifyInitialInheritance()
        {
            Control control = (Control)DRT.FindVisualByID("VBInheritanceMentor");
            _vbInheritance = (VisualBrush)control.Background;
            _vbInheritanceSP = (StackPanel)_vbInheritance.Visual;
            _vbInheritanceTB = (TextBlock)_vbInheritanceSP.Children[0];
            DRT.AssertEqual("Foo", _vbInheritanceSP.DataContext, "Initial inheritance did not pass through VisualBrush");
            DRT.AssertEqual("Foo", _vbInheritanceTB.DataContext, "Initial inheritance did not pass through VisualBrush into subtree");
        }

        // change the value of an inheritable property.  The change should
        // appear in the VisualBrush tree.
        void Inheritance_ChangeValue()
        {
            Control control = (Control)DRT.FindVisualByID("VBInheritanceMentor");
            control.DataContext = "Bar";
            DRT.ResumeAt(VerifyInheritance_ChangeValue);
        }

        void VerifyInheritance_ChangeValue()
        {
            DRT.AssertEqual("Bar", _vbInheritanceSP.DataContext, "Change to inheritable property did not pass through VisualBrush");
            DRT.AssertEqual("Bar", _vbInheritanceTB.DataContext, "Change to inheritable property did not pass through VisualBrush into subtree");
        }

        // replace the content of a VisualBrush.  The old content should get
        // stop inheriting, and the new content should start.
        void Inheritance_ChangeVisualBrushContent()
        {
            _oldStackPanel = _vbInheritanceSP;
            _oldTextBlock = _vbInheritanceTB;

            _vbInheritanceTB = new TextBlock();
            _vbInheritanceTB.Foreground = Brushes.Cyan;
            _vbInheritanceTB.Text = "Inheritance";

            _vbInheritanceSP = new StackPanel();
            _vbInheritanceSP.Children.Add(_vbInheritanceTB);

            _vbInheritance.Visual = _vbInheritanceSP;
            DRT.ResumeAt(VerifyInheritance_ChangeVisualBrushContent);
        }

        void VerifyInheritance_ChangeVisualBrushContent()
        {
            DRT.AssertEqual(null, _oldStackPanel.DataContext, "changing content of VisualBrush did not change inherited property on old content");
            DRT.AssertEqual(null, _oldTextBlock.DataContext, "changing content of VisualBrush did not change inherited property on old content into subtree");
            DRT.AssertEqual("Bar", _vbInheritanceSP.DataContext, "changing content of VisualBrush did not change inherited property on new content");
            DRT.AssertEqual("Bar", _vbInheritanceTB.DataContext, "changing content of VisualBrush did not change inherited property on new content into subtree");
        }

        // detach the VisualBrush.  Its content should stop inheriting.
        void Inheritance_DetachVisualBrush()
        {
            Control control = (Control)DRT.FindVisualByID("VBInheritanceMentor");
            control.ClearValue(Control.BackgroundProperty);
            DRT.ResumeAt(VerifyInheritance_DetachVisualBrush);
        }

        void VerifyInheritance_DetachVisualBrush()
        {
            DRT.AssertEqual(null, _vbInheritanceSP.DataContext, "detaching VisualBrush did not change inherited property on its content");
            DRT.AssertEqual(null, _vbInheritanceTB.DataContext, "detaching VisualBrush did not change inherited property on its content into subtree");
        }

        // re-attach the VisualBrush.  Its content should start inheriting again.
        void Inheritance_AttachVisualBrush()
        {
            Control control = (Control)DRT.FindVisualByID("VBInheritanceMentor");
            control.Background = _vbInheritance;
            DRT.ResumeAt(VerifyInheritance_AttachVisualBrush);
        }

        void VerifyInheritance_AttachVisualBrush()
        {
            DRT.AssertEqual("Bar", _vbInheritanceSP.DataContext, "re-attaching VisualBrush did not change inherited property on its content");
            DRT.AssertEqual("Bar", _vbInheritanceTB.DataContext, "re-attaching VisualBrush did not change inherited property on its content into subtree");
        }

        #endregion  Tests for property inheritance

        #region Tests for resource references

        // resource references should work through a VisualBrush tree
        void VerifyInitialResources()
        {
            Control control = (Control)DRT.FindVisualByID("VBResourcesMentor");
            _vbResources = (VisualBrush)control.Background;
            _vbResourcesTB = (TextBlock)_vbResources.Visual;
            DRT.AssertEqual("Resource string", _vbResourcesTB.Text, "Initial resource reference did not pass through VisualBrush");
        }

        // change the value of a resource.  The change should
        // appear in the VisualBrush tree.
        void Resources_ChangeValue()
        {
            Control control = (Control)DRT.FindVisualByID("VBResourcesMentor");
            control.Resources["MyString"] = "new resource";
            DRT.ResumeAt(VerifyResources_ChangeValue);
        }

        void VerifyResources_ChangeValue()
        {
            DRT.AssertEqual("new resource", _vbResourcesTB.Text, "Change to resource did not pass through VisualBrush");
        }

        // replace the content of a VisualBrush.  The old content should get
        // stop resolving resource references, and the new content should start.
        void Resources_ChangeVisualBrushContent()
        {
            _oldTextBlock = _vbResourcesTB;

            Control control = (Control)DRT.FindVisualByID("VBResourcesMentor");
            _vbResourcesTB = new TextBlock();
            _vbResourcesTB.Foreground = Brushes.Cyan;
            _vbResourcesTB.SetResourceReference(TextBlock.TextProperty, "MyString");

            _vbResources.Visual = _vbResourcesTB;
            DRT.ResumeAt(VerifyResources_ChangeVisualBrushContent);
        }

        void VerifyResources_ChangeVisualBrushContent()
        {
            DRT.AssertEqual(String.Empty, _oldTextBlock.Text, "changing content of VisualBrush did not change resource reference on old content");
            DRT.AssertEqual("new resource", _vbResourcesTB.Text, "changing content of VisualBrush did not change resource reference on new content");
        }

        // detach the VisualBrush.  Its content should stop resolving resource references.
        void Resources_DetachVisualBrush()
        {
            Control control = (Control)DRT.FindVisualByID("VBResourcesMentor");
            control.ClearValue(Control.BackgroundProperty);
            DRT.ResumeAt(VerifyResources_DetachVisualBrush);
        }

        void VerifyResources_DetachVisualBrush()
        {
            DRT.AssertEqual(String.Empty, _vbResourcesTB.Text, "detaching VisualBrush did not change resource reference on its content");
        }

        // re-attach the VisualBrush.  Its content should start resolving resource references again.
        void Resources_AttachVisualBrush()
        {
            Control control = (Control)DRT.FindVisualByID("VBResourcesMentor");
            control.Background = _vbResources;
            DRT.ResumeAt(VerifyResources_AttachVisualBrush);
        }

        void VerifyResources_AttachVisualBrush()
        {
            DRT.AssertEqual("new resource", _vbResourcesTB.Text, "re-attaching VisualBrush did not change resource reference on its content");
        }

        #endregion  Tests for resource references

        #region Tests for name scope

        // NameScope should work through a VisualBrush tree
        void VerifyInitialNameScope()
        {
            Control control = (Control)DRT.FindVisualByID("VBNameScopeMentor");
            _vbNameScope = (VisualBrush)control.Background;
            _vbNameScopeTB = (TextBlock)_vbNameScope.Visual;

            object target = _vbNameScopeTB.FindName("VBNameScopeMentor");
            DRT.AssertEqual(control, target, "FindName did not pass up through VisualBrush");

            target = control.FindName("VBNameScopeTB");
            DRT.AssertEqual(_vbNameScopeTB, target, "FindName did not pass down through VisualBrush");
        }

        // replace the content of a VisualBrush.  The old content should get
        // stop resolving names, and the new content should start.
        void NameScope_ChangeVisualBrushContent()
        {
            _oldTextBlock = _vbNameScopeTB;

            Control control = (Control)DRT.FindVisualByID("VBNameScopeMentor");
            _vbNameScopeTB = new TextBlock();
            _vbNameScopeTB.Name = "NewVBNameScopeTB";
            _vbNameScopeTB.Foreground = Brushes.Cyan;
            _vbNameScopeTB.Text = "new NameScope";

            _vbNameScope.Visual = _vbNameScopeTB;
            DRT.ResumeAt(VerifyNameScope_ChangeVisualBrushContent);
        }

        void VerifyNameScope_ChangeVisualBrushContent()
        {
            Control control = (Control)DRT.FindVisualByID("VBNameScopeMentor");
            object target = _vbNameScopeTB.FindName("VBNameScopeMentor");
            DRT.AssertEqual(control, target, "After changing content, FindName did not pass up through VisualBrush");

            target = control.FindName("VBNameScopeTB");
            DRT.AssertEqual(_oldTextBlock, target, "After changing content, FindName should still find old TextBlock");
        }

        // detach the VisualBrush.  Its content should stop resolving names.
        void NameScope_DetachVisualBrush()
        {
            Control control = (Control)DRT.FindVisualByID("VBNameScopeMentor");
            control.ClearValue(Control.BackgroundProperty);
            DRT.ResumeAt(VerifyNameScope_DetachVisualBrush);
        }

        void VerifyNameScope_DetachVisualBrush()
        {
            Control control = (Control)DRT.FindVisualByID("VBNameScopeMentor");
            object target = _vbNameScopeTB.FindName("VBNameScopeMentor");
            DRT.AssertEqual(null, target, "after detaching VisualBrush, FindName should not resolve the name");
        }

        // re-attach the VisualBrush.  Its content should start resolving names again.
        void NameScope_AttachVisualBrush()
        {
            Control control = (Control)DRT.FindVisualByID("VBNameScopeMentor");
            control.Background = _vbNameScope;
            DRT.ResumeAt(VerifyNameScope_AttachVisualBrush);
        }

        void VerifyNameScope_AttachVisualBrush()
        {
            Control control = (Control)DRT.FindVisualByID("VBNameScopeMentor");
            object target = _vbNameScopeTB.FindName("VBNameScopeMentor");
            DRT.AssertEqual(control, target, "after re-attaching VisualBrush, FindName should resolve the name");
        }

        #endregion  Tests for name scope

        #region Tests for data binding

        // data binding via ambient DataContext should work through a VisualBrush tree
        void VerifyInitialDataBinding()
        {
            if (!DataProviderIsReady("XDP", VerifyInitialDataBinding))
                return;

            Control control = (Control)DRT.FindVisualByID("VBDataBindingMentor");
            _vbDataBinding = (VisualBrush)control.Background;
            _vbDataBindingTB = (TextBlock)_vbDataBinding.Visual;
            DRT.AssertEqual("data", _vbDataBindingTB.Text, "Initial data binding did not pass through VisualBrush");
        }

        // change the value of the data.  The change should
        // appear in the VisualBrush tree.
        void DataBinding_ChangeValue()
        {
            Control control = (Control)DRT.FindVisualByID("VBDataBindingMentor");
            IList list = (IList)control.DataContext;
            XmlNode node = (XmlNode)list[0];
            node.Attributes["Text"].Value = "new data";
            DRT.ResumeAt(VerifyDataBinding_ChangeValue);
        }

        void VerifyDataBinding_ChangeValue()
        {
            DRT.AssertEqual("new data", _vbDataBindingTB.Text, "Change to data did not pass through VisualBrush");
        }

        // replace the content of a VisualBrush.  The old content should
        // stop data binding, and the new content should start.
        void DataBinding_ChangeVisualBrushContent()
        {
            _oldTextBlock = _vbDataBindingTB;

            _vbDataBindingTB = new TextBlock();
            _vbDataBindingTB.Foreground = Brushes.Cyan;

            Binding binding = new Binding();
            binding.XPath = "@Text";
            _vbDataBindingTB.SetBinding(TextBlock.TextProperty, binding);

            Control control = (Control)DRT.FindVisualByID("VBDataBindingMentor");
            _vbDataBinding.Visual = _vbDataBindingTB;
            DRT.ResumeAt(VerifyDataBinding_ChangeVisualBrushContent);
        }

        void VerifyDataBinding_ChangeVisualBrushContent()
        {
            DRT.AssertEqual(String.Empty, _oldTextBlock.Text, "changing content of VisualBrush did not change data binding on old content");
            DRT.AssertEqual("new data", _vbDataBindingTB.Text, "changing content of VisualBrush did not change data binding on new content");
        }

        // detach the VisualBrush.  Its content should stop data binding.
        void DataBinding_DetachVisualBrush()
        {
            Control control = (Control)DRT.FindVisualByID("VBDataBindingMentor");
            control.ClearValue(Control.BackgroundProperty);
            DRT.ResumeAt(VerifyDataBinding_DetachVisualBrush);
        }

        void VerifyDataBinding_DetachVisualBrush()
        {
            DRT.AssertEqual(String.Empty, _vbDataBindingTB.Text, "detaching VisualBrush did not change data binding on its content");
        }

        // re-attach the VisualBrush.  Its content should start data binding again.
        void DataBinding_AttachVisualBrush()
        {
            Control control = (Control)DRT.FindVisualByID("VBDataBindingMentor");
            control.Background = _vbDataBinding;
            DRT.ResumeAt(VerifyDataBinding_AttachVisualBrush);
        }

        void VerifyDataBinding_AttachVisualBrush()
        {
            DRT.AssertEqual("new data", _vbDataBindingTB.Text, "re-attaching VisualBrush did not change data binding on its content");
        }

        #endregion  Tests for data binding

        #region Tests for ElementName binding

        // data binding via ElementName should work through a VisualBrush tree
        void VerifyElementNameBinding()
        {
            Control control = (Control)DRT.FindVisualByID("VBElementNameMentor");
            VisualBrush vbElementName = (VisualBrush)control.Background;
            TextBlock vbElementNameTB = (TextBlock)vbElementName.Visual;
            DRT.AssertEqual("My TextBlock", vbElementNameTB.Text, "Initial ElementName binding did not pass through VisualBrush");
        }

        #endregion Tests for ElementName binding

        #region Test for Resources defined in Style/Template

        // Properties set by a DynamicResources defined in Style/Template should
        // be invalidated when the VisualBrush hooks up (or whenever any ancestry
        // change happens).
        void VerifyResourcesInTemplate()
        {
            Button outer = (Button)DRT.FindVisualByID("VBWithDRInTemplateButton");
            FrameworkTemplate template = outer.Template;
            Button inner = (Button)template.FindName("InnerButton", outer);
            Brush cyanBrush = (Brush)outer.FindResource("CyanBrush");
            DRT.AssertEqual(cyanBrush, inner.Background, "Resource reference defined by template failed within VisualBrush");
        }

        #endregion

        #region Test for 3D ListBox

        // The ListBox with 3D template should pick up its data correctly
        void Verify3DListBox()
        {
            ListBox listbox = (ListBox)DRT.FindVisualByID("LB3D");
            IList list = (IList)listbox.ItemsSource;
            Panel panel = (Panel)DRT.FindVisualByPropertyValue(Panel.IsItemsHostProperty, true, listbox, false);
            UIElementCollection children = panel.Children;

            DRT.AssertEqual(list.Count, children.Count, "3D Listbox has wrong number of children");

            for (int i=0; i<children.Count; ++i)
            {
                XmlNode node = (XmlNode)list[i];
                string name = (string)node.Attributes["Name"].Value;

                Viewport3D viewport = (Viewport3D)DRT.FindVisualByID("MainViewport3D", children[i]);
                ModelVisual3D model = (ModelVisual3D)viewport.Children[0];
                Model3DGroup group = (Model3DGroup)model.Content;
                GeometryModel3D geometry = (GeometryModel3D)group.Children[1];
                DiffuseMaterial material = (DiffuseMaterial)geometry.Material;
                VisualBrush visualBrush = (VisualBrush)material.Brush;
                TextBlock tb = (TextBlock)visualBrush.Visual;

                DRT.AssertEqual(name, tb.Text, "Wrong content in 3D model for item {0}", i);
            }
        }

        #endregion Test for 3D ListBox

        #region Helper methods

        private XmlDataProvider GetDataProvider(string resourceName)
        {
            XmlDataProvider xdp = ((FrameworkElement)DRT.RootElement).FindResource(resourceName) as XmlDataProvider;
            DRT.Assert(xdp != null, String.Format(@"cannot find xml doc resource named '{0}'", resourceName));
            return xdp;
        }

        private bool DataProviderIsReady(string resourceName, DrtTest test)
        {
            XmlDataProvider dataSource = GetDataProvider(resourceName);
            object data = dataSource.Data;
            XmlDocument doc = dataSource.Document;

            if (data == null || doc == null || doc.DocumentElement == null)
            {
                // the document hasn't loaded yet.  Try again later.
                if (++_repeatCount < RepeatLimit)
                {
                    DRT.RepeatTest(2000);
                }
                else
                {
                    if (doc == null || doc.DocumentElement == null)
                    {
                        DRT.Assert(false, "dataSource.Document for test {0} not loaded after {1} attempts.\n  Error is {2}",
                            test.Method.Name, _repeatCount, dataSource.Error);
                    }
                    else if (data == null)
                    {
                        DRT.Assert(false, "dataSource.Data for test {0} not created after {1} attempts.\n  Error is {2}",
                            test.Method.Name, _repeatCount, dataSource.Error);
                    }
                    _repeatCount = 0;
                }
                return false;
            }

            _repeatCount = 0;

            return true;
        }

        #endregion Helper methods

        #region Event handlers

        void OnTBLoaded(object sender, RoutedEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            tb.Text = "Loaded!";
        }

        void OnTBUnloaded(object sender, RoutedEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            tb.Text = "unloaded";
        }

        #endregion Event handlers

        #region Fields

        VisualBrush _vbLoaded, _vbInheritance, _vbResources, _vbNameScope, _vbDataBinding;
        StackPanel _vbInheritanceSP, _oldStackPanel;
        TextBlock _oldTextBlock, _vbLoadedTB, _vbInheritanceTB, _vbResourcesTB, _vbNameScopeTB, _vbDataBindingTB;
        int _repeatCount;
        const int RepeatLimit = 5;

        #endregion Fields
    }
}
