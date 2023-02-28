using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace VisualDiagnostics.FeatureTests.XamlSourceInfo
{
    // [Test(1, "XamlSourceInfo")]
    public class XamlSourceInfoTest_ReleaseBaml_Enabled : XamlSourceInfoTestBase
    {
        protected override void RunTest()
        {
            // Verify that loading BAML from Release assembly will NOT include source info.
            this.Test_Baml("XamlSourceInfoTest.Release.TestControl4", false, true);
        }
    }

    // [Test(1, "XamlSourceInfo")]
    public class XamlSourceInfoTest_DebugSystemResources : XamlSourceInfoTestBase
    {
        protected override void RunTest()
        {
            // Verify that loading themed content will include source info. Note that URI has *.baml extension rather than *.xaml.
            Test_Baml("XamlSourceInfoTest.TestControl5", true, true,
                "Border, 13, 22, component/themes/generic.baml");
        }
    }

    [Test(1, "XamlSourceInfo")]
    public class XamlSourceInfoTest_ReleaseSystemResources : XamlSourceInfoTestBase
    {
        protected override void RunTest()
        {
            // Verify that loading themed content has no source info in Release binaries.
            Test_Baml("XamlSourceInfoTest.TestControl5", false, true);
        }
    }

    [Test(1, "XamlSourceInfo")]
    public class XamlSourceInfoTest_ReleaseBaml_Disabled : XamlSourceInfoTestBase
    {
        protected override void RunTest()
        {
            // Verify that loading BAML from Release assembly will NOT include source info.
            this.Test_Baml("XamlSourceInfoTest.Release.TestControl4", false, false);
        }
    }

    // [Test(1, "XamlSourceInfo")]
    public class XamlSourceInfoTest_DebugBaml_Enabled : XamlSourceInfoTestBase
    {
        protected override void RunTest()
        {
            // Verify that loading BAML from Debug assembly will include source info.
            this.Test_Baml("XamlSourceInfoTest.Debug.TestControl", true, true,
                 "TestControl, 5, 14, component/testcontrol.xaml",
                "ResourceDictionary, 9, 10, component/testcontrol.xaml",
                "ControlTemplate, 13, 14, component/testcontrol.xaml",
                "Grid, 29, 6, component/testcontrol.xaml",
                "StackPanel, 30, 10, component/testcontrol.xaml",
                "Border, 31, 14, component/testcontrol.xaml",
                "Button, 34, 18, component/testcontrol.xaml",
                "Grid, 14, 18, component/testcontrol.xaml",
                "Canvas, 15, 22, component/testcontrol.xaml",
                "Rectangle, 16, 26, component/testcontrol.xaml",
                "TextBlock, 19, 28, component/testcontrol.xaml",
                "TextBox, 36, 14, component/testcontrol.xaml",
                "CheckBox, 37, 14, component/testcontrol.xaml",
                "WrapPanel, 6, 10, component/RD.xaml",
                "TextBox, 7, 14, component/RD.xaml",
                "Ellipse, 8, 14, component/RD.xaml");
        }
    }

    [Test(1, "XamlSourceInfo")]
    public class XamlSourceInfoTest_DebugBaml_Disabled : XamlSourceInfoTestBase
    {
        protected override void RunTest()
        {
            // Verify that loading BAML from Debug assembly will NOT include source info.
            this.Test_Baml("XamlSourceInfoTest.Debug.TestControl", true, false);
        }
    }

    [Test(1, "XamlSourceInfo")]
    public class XamlSourceInfoTest_Sources_Enabled : XamlSourceInfoTestBase
    {
        protected override void RunTest()
        {
            this.Test_Sources(true,
                "DummyDependencyObject, 16, 10, component/testcontrol3.xaml",
                "Style, 11, 4, component/testcontrol3.xaml",
                "Setter, 12, 5, component/testcontrol3.xaml",
                "SolidColorBrush, 9, 4, component/testcontrol3.xaml");
        }
    }

    [Test(1, "XamlSourceInfo")]
    public class XamlSourceInfoTest_Sources_Disabled : XamlSourceInfoTestBase
    {
        protected override void RunTest()
        {
            this.Test_Sources(false);
        }
    }

    // [Test(1, "XamlSourceInfo")]
    public class XamlSourceInfoTest_LocalXaml_Enabled : XamlSourceInfoTestBase
    {
        protected override void RunTest()
        {
            this.Test_LocalXaml(true,
                "Border, 2, 2, pack://application:",
                "ResourceDictionary, 7, 10, pack://application:",
                "Style, 15, 14, pack://application:",
                "Setter, 16, 18, pack://application:",
                "ControlTemplate, 18, 14, pack://application:",
                "SolidColorBrush, 13, 14, pack://application:",
#if TESTBUILD_NET_ATLEAST_471
        // 4.7.1 now provides SourceInfo for template elements in XAML
                "Grid, 19, 18, pack://application:",
                "Canvas, 20, 22, pack://application:",
                "Rectangle, 21, 26, pack://application:",
                "TextBlock, 24, 28, pack://application:",
#endif
                "Grid, 34, 6, pack://application:",
                "StackPanel, 35, 10, pack://application:",
                "Border, 36, 14, pack://application:",
                "Button, 39, 18, pack://application:",
                "TextBox, 41, 14, pack://application:",
                "CheckBox, 42, 14, pack://application:");
        }
    }

    [Test(1, "XamlSourceInfo")]
    public class XamlSourceInfoTest_LocalXaml_Disabled : XamlSourceInfoTestBase
    {
        protected override void RunTest()
        {
            this.Test_LocalXaml(false);
        }
    }


    // [Test(1, "XamlSourceInfo")]
    public class XamlSourceInfoTest_XamlWithUri_Enabled : XamlSourceInfoTestBase
    {
        protected override void RunTest()
        {
            this.Test_XamlWithUri(true,
                @"Border, 2, 2, x:\foo\bar.xaml",
                @"ResourceDictionary, 7, 10, x:\foo\bar.xaml",
                @"Style, 15, 14, x:\foo\bar.xaml",
                @"Setter, 16, 18, x:\foo\bar.xaml",
                @"ControlTemplate, 18, 14, x:\foo\bar.xaml",
                @"SolidColorBrush, 13, 14, x:\foo\bar.xaml",
                "Grid, 19, 18, file:///x:/foo/bar.xaml",
                "Canvas, 20, 22, file:///x:/foo/bar.xaml",
                "Rectangle, 21, 26, file:///x:/foo/bar.xaml",
                "TextBlock, 24, 28, file:///x:/foo/bar.xaml",
                @"Grid, 34, 6, x:\foo\bar.xaml",
                @"StackPanel, 35, 10, x:\foo\bar.xaml",
                @"Border, 36, 14, x:\foo\bar.xaml",
                @"Button, 39, 18, x:\foo\bar.xaml",
                @"TextBox, 41, 14, x:\foo\bar.xaml",
                @"CheckBox, 42, 14, x:\foo\bar.xaml");
        }
    }

    [Test(1, "XamlSourceInfo")]
    public class XamlSourceInfoTest_XamlWithUri_Disabled : XamlSourceInfoTestBase
    {
        protected override void RunTest()
        {
            this.Test_XamlWithUri(false);
        }
    }

    [Test(1, "XamlSourceInfo")]
    public class XamlSourceInfoTest_ResourceDictionary: XamlSourceInfoTestBase
    {
        protected override void RunTest()
        {
            // Verify that there is source info for a ResourceDictionary
            // loaded from a file, e.g. app.xaml, generic.xaml, etc.
            using (SourceInfoRecorder recorder = new SourceInfoRecorder(this.Window, enableSourceInfo: true))
            {
                ResourceDictionary resourceDictionary = (ResourceDictionary)this.LoadObjectFromAssembly("RD.xaml", isDebugAssembly: true);
                recorder.RecordResources(resourceDictionary);
                VerifyResult(recorder,
                    "ResourceDictionary, 3, 5, component/RD.xaml",
                    "SolidColorBrush, 4, 6, component/RD.xaml",
                    "ControlTemplate, 5, 6, component/RD.xaml");
            }
        }
    }

    [Test(1, "XamlSourceInfo")]
    public class XamlSourceInfoTest_LatestSourceInfo : XamlSourceInfoTestBase
    {
        protected override void RunTest()
        {
            // Verify that if source info is set multiple times for the same object
            // then latest info is kept. This is a scenario of loading MyUserControl
            // in context of MainWindow.
            using (SourceInfoRecorder recorder = new SourceInfoRecorder(this.Window, enableSourceInfo: true))
            {
                object value = new object();
                recorder.SetXamlSourceInfo(value, new Uri("c:\\one.xaml"), 1, 1);
                recorder.Record(value);
                recorder.SetXamlSourceInfo(value, new Uri("c:\\two.xaml"), 2, 2);
                recorder.Record(value);
                recorder.SetXamlSourceInfo(value, new Uri("c:\\three.xaml"), 3, 3);
                recorder.Record(value);

                VerifyResult(recorder,
                    "Object, 1, 1, c:\\one.xaml",
                    "Object, 2, 2, c:\\two.xaml",
                    "Object, 3, 3, c:\\three.xaml");
            }
        }
    }



    public class XamlSourceInfoTestBase : WindowTest
    {
        private const string xaml = @"
<Border xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
        xmlns:sys='clr-namespace:System;assembly=mscorlib'
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
            <Style x:Key='Style1' TargetType='{x:Type Button}'>
                <Setter Property='Width' Value='100'/>
            </Style>
            <ControlTemplate x:Key='Template1' TargetType='{x:Type Button}'>
                <Grid>
                    <Canvas x:Name='Root'>
                        <Rectangle
                          Width='100'
                          Height='100'/>
                          <TextBlock>
                            <TextBlock.Text>
                                <Binding Path='Content'></Binding>
                            </TextBlock.Text>
                          </TextBlock>
                    </Canvas>
                </Grid>
            </ControlTemplate>
        </ResourceDictionary>
    </Border.Resources>
    <Grid>
        <StackPanel Width='400' Height='200'>
            <Border
                x:Name='MyBorder'
                x:Uid='Border_1'>
                <Button Template='{DynamicResource Template1}'>Click Me</Button>
            </Border>
            <TextBox>Hello World</TextBox>
            <CheckBox>Option 1</CheckBox>
        </StackPanel>
    </Grid>
</Border>
";

        public XamlSourceInfoTestBase()
        {
            RunSteps += new TestStep(this.ExecuteTest);
        }

        private TestResult ExecuteTest()
        {
            try
            {
                this.RunTest();
            }
            catch
            {
                return TestResult.Fail;
            }

            return this.Log.Result;
        }

        protected virtual void RunTest()
        {
        }

        protected void Test_Sources(bool enableSourceInfo, params string[] expectedRecords)
        {
            // Verify that Debug assembly BAML contains XAML source info for non-Visual objects
            // such as Style and SolidColorBrush. Note that there'll be no info for strings
            // or value types like Color or Thickness. Here is TestControl3 XAML:
            //   <UserControl x:Class="XamlSourceInfoTest.Debug.TestControl3"
            //       xmlns:local="clr-namespace:XamlSourceInfoTest.Debug"
            //       xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            //       xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
            //       <UserControl.Resources>
            //           <Color x:Key="C1">Red</Color>
            //           <Thickness x:Key="T1">1,2,3,4</Thickness>
            //           <SolidColorBrush x:Key="B1" Color="Green"/>
            //           <sys:String x:Key="S1">Hello</sys:String>
            //           <Style x:Key="S1" TargetType="{x:Type Button}">
            //               <Setter Property="Width" Value="100"/>
            //           </Style>
            //       </UserControl.Resources>
            //       <local:TestControl2>
            //           <local:DummyDependencyObject/>
            //       </local:TestControl2>
            //   </UserControl>
            using (SourceInfoRecorder recorder = new SourceInfoRecorder(this.Window, enableSourceInfo))
            {
                UserControl control = (UserControl)CreateTestControl("XamlSourceInfoTest.Debug.TestControl3", true);
                object testControlWithDO = control.Content;
                PropertyInfo propertyInfo = testControlWithDO.GetType().GetProperty("TestContent");
                DependencyObject obj = (DependencyObject)propertyInfo.GetValue(testControlWithDO);
                recorder.Record(obj);
                recorder.RecordResources(control.Resources);

                VerifyResult(recorder, expectedRecords);
            }
        }

        protected void Test_Baml(string controlTypeName, bool isDebug, bool enableSourceInfo, params string[] expectedRecords)
        {
            // Verify that loading BAML from Debug assembly will include source info.
            using (SourceInfoRecorder recorder = new SourceInfoRecorder(this.Window, enableSourceInfo))
            {
                recorder.Window.Content = CreateTestControl(controlTypeName, isDebug);
                recorder.Window.UpdateLayout();

                this.CollectSourceInfoRecursively(recorder.Window, recorder);
                VerifyResult(recorder, expectedRecords);
            }
        }

        protected void Test_LocalXaml(bool enableSourceInfo, params string[] expectedRecords)
        {
            using (SourceInfoRecorder recorder = new SourceInfoRecorder(this.Window, enableSourceInfo))
            {
                Border control = (Border)XamlReader.Parse(xaml);
                recorder.Window.Content = control;
                recorder.Window.UpdateLayout();

                this.CollectSourceInfoRecursively(recorder.Window, recorder);
                VerifyResult(recorder, expectedRecords);
            }
        }

        protected void Test_XamlWithUri(bool enableSourceInfo, params string[] expectedRecords)
        {
            using (SourceInfoRecorder recorder = new SourceInfoRecorder(this.Window, enableSourceInfo))
            {
                ParserContext context = new ParserContext();
                context.BaseUri = new Uri(@"x:\foo\bar.xaml");

                Border control = (Border)XamlReader.Parse(xaml, context);
                recorder.Window.Content = control;
                recorder.Window.UpdateLayout();

                CollectSourceInfoRecursively(recorder.Window, recorder);
                VerifyResult(recorder, expectedRecords);
            }
        }


        private Assembly LoadAssembly(bool isDebugAssembly)
        {
            // This code assumes that helper test assemblies XamlSourceInfoTest.Debug.dll and
            // XamlSourceInfoTest.Release.dll located in the same folder as "this" assembly.
            // The dlls are part of this project and is copied to output directory. Source C#
            // project for XamlSourceInfoTest.dll is in zip file next to the dll.
            string assemblyName = isDebugAssembly ? @"XamlSourceInfoTest.debug.dll" : "XamlSourceInfoTest.release.dll";
            string path = this.GetType().Assembly.Location;
            string dir = System.IO.Path.GetDirectoryName(path);
            path = System.IO.Path.Combine(dir, assemblyName);

            Assembly testAssembly = Assembly.LoadFile(path);
            return testAssembly;
        }

        private object CreateTestControl(string typeName, bool isDebugAssembly)
        {
            Assembly testAssembly = this.LoadAssembly(isDebugAssembly);
            Type type = testAssembly.GetType(typeName);
            object control = Activator.CreateInstance(type);
            return control;
        }

        protected object LoadObjectFromAssembly(string resource, bool isDebugAssembly)
        {
            Assembly testAssembly = this.LoadAssembly(isDebugAssembly);
            string assemblyName = testAssembly.GetName().Name;
            string resourceLocator = string.Format("/{0};component/{1}", assemblyName, resource);
            Uri uri = new Uri(resourceLocator, UriKind.Relative);
            object result = Application.LoadComponent(uri);
            return result;
        }

        protected void VerifyResult(SourceInfoRecorder recorder, params string[] expectedRecords)
        {
            this.IsTrue(recorder.Result.Count == expectedRecords.Length, $"Expected (= {expectedRecords.Length}) /actual (= {recorder.Result.Count}) record count mismatch");
            for (int i = 0; i < expectedRecords.Length; i++)
            {
                // Get next expected record
                string record = expectedRecords[i];
                SourceInfo expectedInfo = SourceInfo.Parse(record);

                // Verify it exists in the result
                int index = recorder.Result.FindIndex(info => SourceInfo.Compare(info, expectedInfo));
                this.IsTrue(index >= 0, "Expected record not found: " + record);
            }
        }

        private void IsTrue(bool result, string message)
        {
            this.Log.LogStatus(message);
            if (!result)
            {
                this.Log.Result = TestResult.Fail;
                throw new ApplicationException(message);
            }
        }


        private void CollectSourceInfoRecursively(DependencyObject obj, SourceInfoRecorder recorder)
        {
            if (obj == null)
            {
                return;
            }

            // Record objects with non-null XAML source info
            SourceInfo info = SourceInfo.FromObject(obj);
            if (info != null)
            {
                recorder.Result.Add(info);
            }

            FrameworkElement element = obj as FrameworkElement;
            if (element != null)
            {
                recorder.RecordResources(element.Resources);
            }

            int count = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                this.CollectSourceInfoRecursively(child, recorder);
            }
        }
    }


    [DebuggerDisplay("{Name}, line={Line}, column={Column}, uri={Uri}")]
    public class SourceInfo
    {
        public string Name;
        public int Line;
        public int Column;
        public string Uri;

        private SourceInfo()
        {
        }

        public static SourceInfo FromObject(object obj)
        {
            System.Windows.Diagnostics.XamlSourceInfo info = System.Windows.Diagnostics.VisualDiagnostics.GetXamlSourceInfo(obj);
            if (info != null)
            {
                return new SourceInfo()
                {
                    Name = obj.GetType().Name,
                    Line = info.LineNumber,
                    Column = info.LinePosition,
                    Uri = info.SourceUri != null ? info.SourceUri.OriginalString : null
                };
            }

            return null;
        }

        public static SourceInfo Parse(string data)
        {
            // Incoming string contains comma separated values
            //   [Name], [line], [column], uri
            // Example:
            //  Grid, 10, 2, test.xaml
            string[] parts = data.Split(',');
            if (parts.Length != 4)
            {
                throw new ArgumentException("data");
            }

            SourceInfo info = new SourceInfo();
            info.Name = parts[0].Trim();
            info.Line = int.Parse(parts[1].Trim());
            info.Column = int.Parse(parts[2].Trim());
            info.Uri = parts[3].Trim();

            return info;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}",
                Name, Line, Column, Uri);
        }

        public static bool Compare(SourceInfo left, SourceInfo right)
        {
            if (left == right)
            {
                // Same or both are null
                return true;
            }

            if (left == null || right == null)
            {
                return false;
            }

            // For test puproses "expected" info will have short Uri.
            // We say two info object are the same if one's uri is a
            // substring of another uri.
            if (left.Name != right.Name ||
                left.Line != right.Line ||
                left.Column != right.Column)
            {
                return false;
            }

            if (left.Uri == right.Uri)
            {
                // Same URI's. Including null case
                return true;
            }

            if (left.Uri == null || right.Uri == null)
            {
                // One is null while another is not
                return false;
            }

            if (left.Uri.IndexOf(right.Uri) >= 0 || right.Uri.IndexOf(left.Uri) >= 0)
            {
                return true;
            }

            return false;
        }
    }

    // Helper class to record changes
    [DebuggerDisplay("Count={Result.Count}")]
    public class SourceInfoRecorder : IDisposable
    {
        public List<SourceInfo> Result = new List<SourceInfo>();
        public Window Window;

        public SourceInfoRecorder(Window window, bool enableSourceInfo)
        {
            this.Window = window;
            this.Window.Left = 10000;
            this.Window.Width = 400;
            this.Window.Height = 200;
            this.EnableSourceInfo(enableSourceInfo);
        }

        public void Record(object obj)
        {
            SourceInfo info = SourceInfo.FromObject(obj);
            if (info != null)
            {
                Result.Add(info);
            }
        }

        public void RecordResources(ResourceDictionary resourceDictionary)
        {
            if (resourceDictionary != null)
            {
                Record(resourceDictionary);
                foreach (object value in resourceDictionary.Values)
                {
                    Record(value);

                    // Record style setters
                    Style style = value as Style;
                    if (style != null)
                    {
                        foreach (Setter setter in style.Setters)
                        {
                            Record(setter);
                        }
                    }
                }
            }
        }

        public void EnableSourceInfo(bool enable)
        {
            Assembly assembly = typeof(System.Windows.Diagnostics.VisualDiagnostics).Assembly;
            Type type = assembly.GetType("System.Windows.Diagnostics.XamlSourceInfoHelper");
            MethodInfo method = type.GetMethod("InitializeEnableXamlSourceInfo", BindingFlags.NonPublic | BindingFlags.Static);
            object[] parameters = new object[] { enable ? "1" : "0" };
            method.Invoke(null, parameters);
        }

        public void SetXamlSourceInfo(object obj, Uri sourceUri, int elementLineNumber, int elementLinePosition)
        {
            Assembly assembly = typeof(System.Windows.Diagnostics.VisualDiagnostics).Assembly;
            Type type = assembly.GetType("System.Windows.Diagnostics.XamlSourceInfoHelper");
            MethodInfo method = type.GetMethod("SetXamlSourceInfo", BindingFlags.NonPublic | BindingFlags.Static,
                Type.DefaultBinder, new Type[] { typeof(object), typeof(Uri), typeof(int), typeof(int) }, null);

            object[] parameters = new object[] { obj, sourceUri, elementLineNumber, elementLinePosition };
            method.Invoke(null, parameters);
        }

        public void Dispose()
        {
            this.EnableSourceInfo(false);
        }
    }
}
