using Microsoft.Test;
using Microsoft.Test.Diagnostics;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Theming;
using Microsoft.Test.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Diagnostics;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Resources;
using System.Windows.Threading;

namespace ResourceDictionaryDiagnosticsTests.FeatureTests
{
    public abstract class ResourceDictionaryTestBase : WindowTest
    {
        ResourceDictionaryTestEnabler _enabler = new ResourceDictionaryTestEnabler(DriverState.DriverParameters["Mode"]);

        protected ResourceDictionaryTestBase()
        {
            InitializeSteps += InitializeTest;
            RunSteps += RunTest;
            CleanUpSteps += CleanUpTest;
        }

        private TestResult InitializeTest()
        {
            Log.LogStatus("Loading content");

            if (LoadContent())
            {
                WaitForPriority(DispatcherPriority.ApplicationIdle);
                return TestResult.Pass;
            }
            else
            {
                Log.LogStatus("Failed to load content");
                _enabler.Dispose();
                return TestResult.Fail;
            }
        }

        private TestResult RunTest()
        {

            MyBorder = (Border)Window.Content;
            MyStackPanel = (StackPanel)((Grid)MyBorder.Child).Children[0];
            MySpan = (Span)((TextBlock)MyStackPanel.Children[4]).Inlines.FirstInline;

            using (_enabler)
            {
                if (Verify(_enabler.Mode))
                {
                    return TestResult.Pass;
                }
                else
                {
                    return TestResult.Fail;
                }
            }
        }

        private TestResult CleanUpTest()
        {
            CleanUp();
            return TestResult.Pass;
        }

        protected abstract bool Verify(string mode);

        protected virtual void CleanUp() { }

        protected Border MyBorder { get; set; }
        protected StackPanel MyStackPanel { get; set; }
        protected Span MySpan { get; set; }
        protected Application MyApplication { get; set; }

        private bool LoadContent()
        {
            bool result = true;

            try
            {
                // there's no Application object in this kind of test case,
                // but we can just make one, and give it some resources
                MyApplication = new Application();
                MyApplication.Resources.Add("AppString", "File");

                Window.Title = nameof(ResourceDictionaryDiagnosticsTests);
                Window.Content = (Border)XamlReader.Parse(xaml);
            }
            catch (Exception e)
            {
                Log.LogStatus(e.ToString());
                result = false;
            }

            return result;
        }

        private const string xaml = @"
<Border xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
        xmlns:sys='clr-namespace:System;assembly=mscorlib'
        xmlns:syssys='clr-namespace:System;assembly=System'
        xmlns:mc='http://schemas.openxmlformats.org/markup-compatibility/2006' >
    <Border.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source='pack://application:,,,/XamlSourceInfoTest.Debug;component/RD.xaml'/>
            </ResourceDictionary.MergedDictionaries>
            <Color x:Key='C1'>Red</Color>
            <Thickness x:Key='T1'>1,2,3,4</Thickness>
            <SolidColorBrush x:Key='B1' Color='Green'/>
            <sys:String x:Key='S1'>Hello</sys:String>
            <sys:Double x:Key='SmallSize'>10</sys:Double>
            <Thickness x:Key='SmallMargin' Left='{StaticResource SmallSize}'/>
            <Style x:Key='Style1' TargetType='{x:Type Button}'>
                <Setter Property='Width' Value='100'/>
            </Style>
            <ControlTemplate x:Key='Template1' TargetType='{x:Type Button}'>
                <Grid>
                    <Canvas x:Name='Root'>
                        <Rectangle
                          Width='100'
                          Height='100'/>
                          <TextBlock x:Name='PART_Text' Margin='{StaticResource T1}'>
                            <TextBlock.Text>
                                <Binding Path='Content'></Binding>
                            </TextBlock.Text>
                          </TextBlock>
                    </Canvas>
                </Grid>
            </ControlTemplate>
            <Style x:Key='btnStyle' TargetType='{x:Type Button}'>
                <Style.Setters>
                    <Setter Property='Template' Value='{StaticResource Template1}'/>
                </Style.Setters>
            </Style>
        </ResourceDictionary>
    </Border.Resources>
    <Grid>
        <StackPanel Width='400' Height='200'>
            <Menu>
                <MenuItem Template='{StaticResource {ComponentResourceKey TypeInTargetAssembly={x:Type MenuItem}, ResourceId=TopLevelItemTemplateKey}}'
                          Header='{StaticResource AppString}'/>
            </Menu>
            <Border
                x:Name='MyBorder'
                x:Uid='Border_1'
                BorderBrush='{StaticResource Red}'>
                <Button Template='{StaticResource Template1}'>Click Me</Button>
            </Border>
            <TextBox Text='{StaticResource S1}' Margin='{StaticResource SmallMargin}'/>
            <Button Style='{StaticResource btnStyle}'>Button 1</Button>
            <TextBlock>
                <Span>
                    <Span.Resources>
                      <ResourceDictionary>
                        <ResourceDictionary.MergedDictionaries>
                            <ResourceDictionary Source='pack://application:,,,/XamlSourceInfoTest.Debug;component/RD.xaml'/>
                        </ResourceDictionary.MergedDictionaries>
                        <syssys:Uri x:Key='DefaultUri'>http://default.com</syssys:Uri>
                      </ResourceDictionary>
                    </Span.Resources>
                    <Hyperlink NavigateUri='{StaticResource DefaultUri}'/>
                </Span>
            </TextBlock>
            <Button Style='{StaticResource btnStyle}'>Button 2</Button>
            <CheckBox>Option 1</CheckBox>
        </StackPanel>
    </Grid>
</Border>
";
    }


    [Test(1, "ResourceDictionaryDiagnostics", TestParameters="Mode=Enable")]
    [Test(1, "ResourceDictionaryDiagnostics", TestParameters="Mode=Null")]
    [Test(1, "ResourceDictionaryDiagnostics", TestParameters="Mode=Disable")]
    public class GetResourceDictionariesFromSource : ResourceDictionaryTestBase
    {
        protected override bool Verify(string mode)
        {
            bool result = true;
            List<ResourceDictionary> expected;
            IEnumerable<ResourceDictionary> observed;
            Uri uri;

            uri = new Uri("pack://application:,,,/XamlSourceInfoTest.Debug;component/RD.xaml");
            Log.LogStatus("Verify dictionaries for {0}", uri);
            expected = new List<ResourceDictionary>();
            expected.Add(MyBorder.Resources.MergedDictionaries[0]);
            expected.Add(MySpan.Resources.MergedDictionaries[0]);
            observed = ResourceDictionaryDiagnostics.GetResourceDictionariesForSource(uri);
            result = VerifyResult(expected, observed, mode) && result;

            uri = new Uri("http://foobar.net");
            Log.LogStatus("Verify dictionaries for {0}", uri);
            expected = new List<ResourceDictionary>();
            observed = ResourceDictionaryDiagnostics.GetResourceDictionariesForSource(uri);
            result = VerifyResult(expected, observed, mode) && result;

            return result;
        }

        private bool VerifyResult(List<ResourceDictionary> expected,
                                IEnumerable<ResourceDictionary> observed,
                                string mode)
        {
            bool result = true;

            int count = 0;
            foreach (ResourceDictionary rd in observed)
            {
                if (!expected.Contains(rd))
                {
                    Log.LogStatus("FAIL:  observed[{0}] does not appear in expected", count);
                    result = false;
                }
                ++count;
            }

            int expectedCount = (mode == "Enable") ? expected.Count : 0;
            if (expectedCount != count)
            {
                Log.LogStatus("FAIL:  Counts are different.  Expected: {0}  Observed: {1}", expectedCount, count);
                result = false;
            }

            return result;
        }
    }


    [Test(1, "ResourceDictionaryDiagnostics", TestParameters="Mode=Enable")]
    [Test(1, "ResourceDictionaryDiagnostics", TestParameters="Mode=Null")]
    [Test(1, "ResourceDictionaryDiagnostics", TestParameters="Mode=Disable")]
    public class GetResourceDictionaryOwners : ResourceDictionaryTestBase
    {
        protected override bool Verify(string mode)
        {
            bool result = true;
            List<object> expected;
            ResourceDictionary rd;
            string name;

            name = "Border";
            rd = MyBorder.Resources;
            Log.LogStatus("Verify owners for {0}", name);
            expected = new List<object>();
            expected.Add(MyBorder);
            result = VerifyResult(rd, expected, mode) && result;

            name = "Border.merged";
            rd = MyBorder.Resources.MergedDictionaries[0];
            Log.LogStatus("Verify owners for {0}", name);
            expected = new List<object>();
            expected.Add(MyBorder);
            result = VerifyResult(rd, expected, mode) && result;

            name = "Span.merged";
            rd = MySpan.Resources.MergedDictionaries[0];
            Log.LogStatus("Verify owners for {0}", name);
            expected = new List<object>();
            expected.Add(MySpan);
            result = VerifyResult(rd, expected, mode) && result;

            name = "Application";
            rd = MyApplication.Resources;
            Log.LogStatus("Verify owners for {0}", name);
            expected = new List<object>();
            expected.Add(MyApplication);
            result = VerifyResult(rd, expected, mode) && result;

            return result;
        }

        private bool VerifyResult(ResourceDictionary rd,
                                List<object> expected,
                                string mode)
        {
            IEnumerable<FrameworkElement> feObserved = ResourceDictionaryDiagnostics.GetFrameworkElementOwners(rd);
            IEnumerable<FrameworkContentElement> fceObserved = ResourceDictionaryDiagnostics.GetFrameworkContentElementOwners(rd);
            IEnumerable<Application> appObserved = ResourceDictionaryDiagnostics.GetApplicationOwners(rd);
            bool result = true;

            int feCount=0;
            foreach (FrameworkElement fe in feObserved)
            {
                if (!expected.Contains(fe))
                {
                    Log.LogStatus("FAIL:  feObserved[{0}] does not appear in expected", feCount);
                    result = false;
                }
                ++feCount;
            }

            int fceCount=0;
            foreach (FrameworkContentElement fce in fceObserved)
            {
                if (!expected.Contains(fce))
                {
                    Log.LogStatus("FAIL:  fceObserved[{0}] does not appear in expected", fceCount);
                    result = false;
                }
                ++fceCount;
            }

            int appCount=0;
            foreach (Application app in appObserved)
            {
                if (!expected.Contains(app))
                {
                    Log.LogStatus("FAIL:  appObserved[{0}] does not appear in expected", appCount);
                    result = false;
                }
                ++appCount;
            }

            int expectedCount = (mode == "Enable") ? expected.Count : 0;
            int count = (feCount + fceCount + appCount);
            if (expectedCount != count)
            {
                Log.LogStatus("FAIL:  Counts are different.  Expected: {0}  Observed: {1}", expectedCount, count);
                result = false;
            }

            return result;
        }
    }


    [Test(1, "ResourceDictionaryDiagnostics", TestParameters="Mode=Enable")]
    [Test(1, "ResourceDictionaryDiagnostics", TestParameters="Mode=Null")]
    [Test(1, "ResourceDictionaryDiagnostics", TestParameters="Mode=Disable")]
    public class StaticResourceResolvedEvent : ResourceDictionaryTestBase
    {
        List<StaticResourceResolvedEventArgs> _list = new List<StaticResourceResolvedEventArgs>();
        PropertyInfo _piIsThemeDictionary;

        public StaticResourceResolvedEvent()
        {
            ResourceDictionaryDiagnostics.StaticResourceResolved += OnStaticResourceResolved;
        }

        private void OnStaticResourceResolved(object sender, StaticResourceResolvedEventArgs e)
        {
            _list.Add(e);
        }

        protected override bool Verify(string mode)
        {
            _piIsThemeDictionary = typeof(ResourceDictionary).GetProperty("IsThemeDictionary",
                            BindingFlags.Instance | BindingFlags.NonPublic);

            bool result = true;

            if (mode != "Enable")
            {
                result = (_list.Count == 0);
                if (!result)
                {
                    Log.LogStatus("FAIL:  Event count is wrong.  Expected: 0  Observed: {0}", _list.Count);
                }

                return result;
            }

            // build expected results (hard-wired for the XAML declared above)
            List<ExpectedArgs> expected = new List<ExpectedArgs>();

            expected.Add(new ExpectedArgs(
                            ((Menu)MyStackPanel.Children[0]).Items[0],      // MenuItem
                            MenuItem.TemplateProperty,                      // .Template
                            new ComponentResourceKey(typeof(MenuItem), "TopLevelItemTemplateKey"),
                            null                                            // -> theme dictionary
                            ));
            expected.Add(new ExpectedArgs(
                            ((Menu)MyStackPanel.Children[0]).Items[0],      // MenuItem
                            MenuItem.HeaderProperty,                        // .Header
                            "AppString",                                    // "AppString"
                            MyApplication.Resources                         // -> app resources
                            ));
            expected.Add(new ExpectedArgs(
                            MyStackPanel.Children[1],                       // Border
                            Border.BorderBrushProperty,                     // .BorderBrush
                            "Red",                                          // "Red"
                            MyBorder.Resources.MergedDictionaries[0]        // -> merged dict
                            ));
            expected.Add(new ExpectedArgs(
                            ((Border)MyStackPanel.Children[1]).Child,       // Button
                            Button.TemplateProperty,                        // .Template
                            "Template1",                                    // "Template1"
                            MyBorder.Resources                              // -> resources
                            ));
            expected.Add(new ExpectedArgs(
                            MyStackPanel.Children[2],                       // TextBox
                            TextBox.TextProperty,                           // .Text
                            "S1",                                           // "S1"
                            MyBorder.Resources                              // -> resources
                            ));
            expected.Add(new ExpectedArgs(
                            MyStackPanel.Children[2],                       // TextBox
                            TextBox.MarginProperty,                         // .Margin
                            "SmallMargin",                                  // "SmallMargin"
                            MyBorder.Resources                              // -> resources
                            ));
            expected.Add(new ExpectedArgs(
                            MyStackPanel.Children[3],                       // Button
                            Button.StyleProperty,                           // .Style
                            "btnStyle",                                     // "btnStyle"
                            MyBorder.Resources                              // -> resources
                            ));
            expected.Add(new ExpectedArgs(
                            MySpan.Inlines.FirstInline,                     // Hyperlink
                            Hyperlink.NavigateUriProperty,                  // .NavigateUri
                            "DefaultUri",                                   // "DefaultUri"
                            MySpan.Resources                                // -> Span's resources
                            ));
            expected.Add(new ExpectedArgs(
                            MyStackPanel.Children[5],                       // Button
                            Button.StyleProperty,                           // .Style
                            "btnStyle",                                     // "btnStyle"
                            MyBorder.Resources                              // -> resources
                            ));

            // template-generated references
            Button button;
            button = (Button)((Border)MyStackPanel.Children[1]).Child;
            expected.Add(new ExpectedArgs(
                            button.Template.FindName("PART_Text", button),  // TextBlock (in Button template)
                            TextBlock.MarginProperty,                       // .Margin
                            "T1",                                           // "T1"
                            MyBorder.Resources                              // -> resources
                            ));
            button = (Button)MyStackPanel.Children[3];
            expected.Add(new ExpectedArgs(
                            button.Template.FindName("PART_Text", button),  // TextBlock (in Button template)
                            TextBlock.MarginProperty,                       // .Margin
                            "T1",                                           // "T1"
                            MyBorder.Resources                              // -> resources
                            ));
            button = (Button)MyStackPanel.Children[5];
            expected.Add(new ExpectedArgs(
                            button.Template.FindName("PART_Text", button),  // TextBlock (in Button template)
                            TextBlock.MarginProperty,                       // .Margin
                            "T1",                                           // "T1"
                            MyBorder.Resources                              // -> resources
                            ));

            // non-DO targets
            expected.Add(new ExpectedArgs(
                            MyBorder.Resources["SmallMargin"],              // Thickness
                            typeof(Thickness).GetProperty("Left"),          // .Left
                            "SmallSize",                                    // "SmallSize"
                            MyBorder.Resources                              // -> resources
                            ));
            expected.Add(new ExpectedArgs(
                            ((Style)MyBorder.Resources["btnStyle"]).Setters[0], // Setter
                            typeof(Setter).GetProperty("Value"),            // .Value
                            "Template1",                                    // "Template1"
                            MyBorder.Resources                              // -> resources
                            ));

            // match observed results to expected
            int count = 0;
            foreach (StaticResourceResolvedEventArgs ob in _list)
            {
                foreach (ExpectedArgs ex in expected)
                {
                    if (Object.Equals(ex.TargetObject, ob.TargetObject) &&
                        ex.TargetProperty == ob.TargetProperty &&
                        Object.Equals(ex.ResourceKey, ob.ResourceKey) &&
                        DictionariesMatch(ex.ResourceDictionary, ob.ResourceDictionary))
                    {
                        ex.Matched = true;
                        ++count;
                        break;
                    }
                }
            }

            result = (expected.Count == count);
            if (!result)
            {
                Log.LogStatus("FAIL:  Event count is wrong.  Expected: {0}  Matched: {1}", expected.Count, count);
                for (int i=0, n=expected.Count; i<n; ++i)
                {
                    ExpectedArgs ex = expected[i];
                    if (!ex.Matched)
                    {
                        Log.LogStatus("   {0} key:{1} obj:{2} prop:{3}",
                            i, ex.ResourceKey, ex.TargetObject, ex.TargetProperty);
                    }
                }
            }

            return result;
        }

        private bool DictionariesMatch(ResourceDictionary expected, ResourceDictionary observed)
        {
            if (expected != null)
                return (expected == observed);

            // null means "theme dictionary".  We don't check equality (too
            // painful to locate the theme dictionaries).  Use private flag.
            return (bool)_piIsThemeDictionary.GetValue(observed);
        }

        private class ExpectedArgs
        {
            internal ExpectedArgs(
                        Object targetObject,
                        Object targetProperty,
                        Object key,
                        ResourceDictionary rd)
            {
                TargetObject = targetObject;
                TargetProperty = targetProperty;
                ResourceDictionary = rd;
                ResourceKey = key;
            }

            public Object TargetObject { get; private set; }
            public Object TargetProperty { get; private set; }
            public ResourceDictionary ResourceDictionary { get; private set; }
            public object ResourceKey { get; private set; }
            public bool Matched { get; set; }
        }
    }
}

