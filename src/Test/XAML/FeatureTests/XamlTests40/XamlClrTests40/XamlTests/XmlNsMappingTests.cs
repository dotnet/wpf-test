// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: System.Windows.Markup.XmlnsDefinition("http://cdf/test", "CDF.Test.TestCases.Xaml.XamlTests.XmlNsMappingTests")]
namespace CDF.Test.TestCases.Xaml.XamlTests.XmlNsMappingTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Infrastructure.Test;
    using System.Reflection;
    using CDF.Test.TestCases.Xaml.Types;
    using System.Windows.Markup;
    using CDF.Test.Common.TestObjects.XamlTestDriver;
    using CDF.Test.TestCases.Xaml.Driver;
    using CDF.Test.Common.TestObjects.Utilities;
    using Microsoft.Test.CDFInfrastructure;

    public class MyCustomClass
    {
        public string StringProperty { get; set; }
    }

    public static class XmlNsMappingTests
    {
        [TestCase(Category = TestCategory.BVT, Owner = "Microsoft")]
        public static void XmlnsMappingLoaded()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = DirectoryAssistance.GetTestBinsDirectory();

            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, setup);
            AppDomainHelper helper = (AppDomainHelper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                typeof(AppDomainHelper).FullName);
            helper.XmlnsMappingAssemblyLoaded();
            AppDomain.Unload(domain);
        }

        [TestCase(Category = TestCategory.BVT, Owner = "Microsoft")]
        public static void XmlnsMappingAssemblyNotLoaded()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = DirectoryAssistance.GetTestBinsDirectory();

            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, setup);
            AppDomainHelper helper = (AppDomainHelper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                typeof(AppDomainHelper).FullName);
            helper.XmlnsMappingAssemblyNotLoaded();
            AppDomain.Unload(domain);
        }

        [TestCase(Category = TestCategory.BVT, Owner = "Microsoft")]
        public static void NoAssemblyForNamespace()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = DirectoryAssistance.GetTestBinsDirectory();

            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, setup);
            AppDomainHelper helper = (AppDomainHelper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                typeof(AppDomainHelper).FullName);
            helper.NoAssemblyForNamespace();
            AppDomain.Unload(domain);
        }

        [TestCase(Category = TestCategory.BVT, Owner = "Microsoft")]
        public static void AssemblyCannotBeFound()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = DirectoryAssistance.GetTestBinsDirectory();

            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, setup);
            AppDomainHelper helper = (AppDomainHelper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
      typeof(AppDomainHelper).FullName);
            helper.AssemblyCannotBeFound();
            AppDomain.Unload(domain);
        }

        [TestCase(Category = TestCategory.BVT, Owner = "Microsoft")]
        public static void AttributeNamespaceDoesNotMatch()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = DirectoryAssistance.GetTestBinsDirectory();

            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, setup);
            AppDomainHelper helper = (AppDomainHelper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                typeof(AppDomainHelper).FullName);
            helper.AttributeNamespaceDoesNotMatch();
            AppDomain.Unload(domain);
        }

        [TestCase(Category = TestCategory.BVT, Owner = "Microsoft")]
        public static void AssemblyNameisPartial()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = DirectoryAssistance.GetTestBinsDirectory();

            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, setup);
            AppDomainHelper helper = (AppDomainHelper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                typeof(AppDomainHelper).FullName);
            helper.AssemblyNameisPartial();
            AppDomain.Unload(domain);
        }

        [TestCase(Category = TestCategory.BVT, Owner = "Microsoft")]
        public static void MappingForDefaultNamespace()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = DirectoryAssistance.GetTestBinsDirectory();

            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, setup);
            AppDomainHelper helper = (AppDomainHelper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                typeof(AppDomainHelper).FullName);
            helper.MappingForDefaultNamespace();
            AppDomain.Unload(domain);
        }

        [TestCase(Category = TestCategory.BVT, Owner = "Microsoft")]
        public static void EmptyNamespace()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = DirectoryAssistance.GetTestBinsDirectory();

            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, setup);
            AppDomainHelper helper = (AppDomainHelper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                typeof(AppDomainHelper).FullName);
            bool pass = false;
            try
            {
                helper.EmptyNamespace();
            }
            catch (ArgumentException ex)
            {
                pass = true;
                Console.WriteLine("Expected exception : " + ex.ToString());
            }
            AppDomain.Unload(domain);

            if (!pass)
                throw new TestCaseFailedException("Expected ArgumentException not caught");

        }

        [TestCase(Category = TestCategory.BVT, Owner = "Microsoft")]
        public static void NullNamespace()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = DirectoryAssistance.GetTestBinsDirectory();

            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, setup);
            AppDomainHelper helper = (AppDomainHelper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                typeof(AppDomainHelper).FullName);

            bool pass = false;
            try
            {
                helper.NullNamespace();
            }
            catch (ArgumentException ex)
            {
                pass = true;
                Console.WriteLine("Expected exception : " + ex.ToString());
            }
            AppDomain.Unload(domain);

            if (!pass)
                throw new TestCaseFailedException("Expected ArgumentException not caught");
        }

        [TestCase(Category = TestCategory.BVT, Owner = "Microsoft")]
        public static void EmptyAssemblyName()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = DirectoryAssistance.GetTestBinsDirectory();

            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, setup);
            AppDomainHelper helper = (AppDomainHelper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                typeof(AppDomainHelper).FullName);

            bool pass = false;
            try
            {
                helper.EmptyAssemblyName();
            }
            catch (ArgumentException ex)
            {
                pass = true;
                Console.WriteLine("Expected exception : " + ex.ToString());
            }
            AppDomain.Unload(domain);

            if (!pass)
                throw new TestCaseFailedException("Expected ArgumentException not caught");
        }

        [TestCase(Category = TestCategory.BVT, Owner = "Microsoft")]
        public static void NullAssemblyName()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = DirectoryAssistance.GetTestBinsDirectory();

            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, setup);
            AppDomainHelper helper = (AppDomainHelper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                typeof(AppDomainHelper).FullName);

            bool pass = false;
            try
            {
                helper.NullAssemblyName();
            }
            catch (ArgumentException ex)
            {
                pass = true;
                Console.WriteLine("Expected exception : " + ex.ToString());
            }
            AppDomain.Unload(domain);

            if (!pass)
                throw new TestCaseFailedException("Expected ArgumentException not caught");

        }

        [TestCase(Category = TestCategory.BVT, Owner = "Microsoft")]
        public static void AttributeClrNamespaceDoesNotMatch()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = DirectoryAssistance.GetTestBinsDirectory();

            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, setup);
            AppDomainHelper helper = (AppDomainHelper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                typeof(AppDomainHelper).FullName);
            helper.AttributeClrNamespaceDoesNotMatch();
            AppDomain.Unload(domain);

        }

        [TestCase(Category = TestCategory.BVT, Owner = "Microsoft")]
        public static void MultipleMappings()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = DirectoryAssistance.GetTestBinsDirectory();

            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, setup);
            AppDomainHelper helper = (AppDomainHelper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                typeof(AppDomainHelper).FullName);
            helper.MultipleMappings();
            AppDomain.Unload(domain);
        }

        [TestCase(Category = TestCategory.BVT, Owner = "Microsoft")]
        public static void AddMappingToDefault()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = DirectoryAssistance.GetTestBinsDirectory();

            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, setup);
            AppDomainHelper helper = (AppDomainHelper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                typeof(AppDomainHelper).FullName);
            helper.AddMappingToDefault();
            AppDomain.Unload(domain);
        }

        [TestCase(Category = TestCategory.BVT, Owner = "Microsoft")]
        public static void DefaultAndCustom()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = DirectoryAssistance.GetTestBinsDirectory();

            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, setup);
            AppDomainHelper helper = (AppDomainHelper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                typeof(AppDomainHelper).FullName);
            helper.DefaultAndCustom();
            AppDomain.Unload(domain);
        }


    }

    public class AppDomainHelper : MarshalByRefObject
    {
        public void XmlnsMappingAssemblyLoaded()
        {
            // this will load the type into this appdomain //
            MyCustomClass obj = new MyCustomClass() { StringProperty = "Hello world" };

            IList<XmlnsDefinitionMapping> mappings = new List<XmlnsDefinitionMapping>()
            {
                new XmlnsDefinitionMapping("http://cdf/test", "CDF.Test.TestCases.Xaml"),
            };

            string xaml = XamlServices.Save(obj);

            XamlObjectDeserializer deserializer = new XamlObjectDeserializer(new XamlObjectDeserializerSettings(), new XamlSchemaTypeResolver(mappings));

            MyCustomClass roundTripped = (MyCustomClass)deserializer.Parse(xaml);

            XamlObjectComparer.CompareObjects(obj, roundTripped);
        }

        public void XmlnsMappingAssemblyNotLoaded()
        {
            string xaml = @"<Sequence DisplayName='Sequence1' xmlns='http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel'/>";

            IList<XmlnsDefinitionMapping> mappings = new List<XmlnsDefinitionMapping>()
            {
                new XmlnsDefinitionMapping(@"http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel", "System.WorkflowModel.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"),
            };

            XamlObjectDeserializer deserializer = new XamlObjectDeserializer(new XamlObjectDeserializerSettings(), new XamlSchemaTypeResolver(mappings));

            var roundTripped = deserializer.Parse(xaml);

            if (roundTripped == null)
                throw new TestCaseFailedException("Did not get object after deserialization");
        }

        public void AddMappingToDefault()
        {
            string xaml = @"<Sequence DisplayName='Sequence1' xmlns='http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel'/>";

            XamlSchemaTypeResolver.AddXmlnsMappingsToDefaultResolver(new List<XmlnsDefinitionMapping>()
            {
                new XmlnsDefinitionMapping(@"http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel", "System.WorkflowModel.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"),
            });

            XamlObjectDeserializer deserializer = new XamlObjectDeserializer();

            var roundTripped = deserializer.Parse(xaml);

            if (roundTripped == null)
                throw new TestCaseFailedException("Did not get object after deserialization");
        }

        public void DefaultAndCustom()
        {
            string xaml = @"<Sequence DisplayName='Sequence1' xmlns='http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel'/>";

            XamlSchemaTypeResolver.AddXmlnsMappingsToDefaultResolver(new List<XmlnsDefinitionMapping>()
            {
                // broken mapping //
                new XmlnsDefinitionMapping(@"http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel", "System.WorkflowModel.Activities1, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"),
            });

            IList<XmlnsDefinitionMapping> mappings = new List<XmlnsDefinitionMapping>()
            {
                // note that this is incorrect namespace //
                new XmlnsDefinitionMapping(@"http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel", "System.WorkflowModel.Activities"),
            };

            XamlObjectDeserializer deserializer = new XamlObjectDeserializer(new XamlObjectDeserializerSettings(), new XamlSchemaTypeResolver(mappings));

            var roundTripped = deserializer.Parse(xaml);

            if (roundTripped == null)
                throw new TestCaseFailedException("Did not get object after deserialization");
        }

        // 
        public void NoAssemblyForNamespace()
        {
            string xaml = @"<Sequence DisplayName='Sequence1' xmlns='http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel1'/>";

            IList<XmlnsDefinitionMapping> mappings = new List<XmlnsDefinitionMapping>()
            {
                // note that this is incorrect namespace //
                new XmlnsDefinitionMapping(@"http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel", "System.WorkflowModel.Activities"),
            };

            XamlObjectDeserializer deserializer = new XamlObjectDeserializer(new XamlObjectDeserializerSettings(), new XamlSchemaTypeResolver(mappings));

            try
            {
                var roundTripped = deserializer.Parse(xaml);
            }
            catch (System.Runtime.Xaml.XamlParseException)
            {
                return;
            }

            throw new TestCaseFailedException("Did not get expected XamlParseException");
        }

        // 
        public void AssemblyCannotBeFound()
        {
            string xaml = @"<Sequence DisplayName='Sequence1' xmlns='http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel'/>";

            IList<XmlnsDefinitionMapping> mappings = new List<XmlnsDefinitionMapping>()
            {
                // note that this is incorrect assembly name //
                new XmlnsDefinitionMapping(@"http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel", "System.WorkflowModel.Activities1"),
            };

            XamlObjectDeserializer deserializer = new XamlObjectDeserializer(new XamlObjectDeserializerSettings(), new XamlSchemaTypeResolver(mappings));

            try
            {
                var roundTripped = deserializer.Parse(xaml);
            }
            catch (InvalidOperationException)
            {
                return;
            }

            throw new TestCaseFailedException("Did not get expected InvalidOperationException");
        }

        public void AttributeNamespaceDoesNotMatch()
        {
            string xaml = @"<Sequence DisplayName='Sequence1' xmlns='http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel'/>";

            IList<XmlnsDefinitionMapping> mappings = new List<XmlnsDefinitionMapping>()
            {
                // note that this is incorrect xmlns  name //
                new XmlnsDefinitionMapping(@"http://foo", "System.WorkflowModel.Activities"),
            };

            XamlObjectDeserializer deserializer = new XamlObjectDeserializer(new XamlObjectDeserializerSettings(), new XamlSchemaTypeResolver(mappings));

            try
            {
                var roundTripped = deserializer.Parse(xaml);
            }
            catch (System.Runtime.Xaml.XamlParseException)
            {
                return;
            }

            throw new TestCaseFailedException("Did not get expected XamlParseException");
        }

        public void AttributeClrNamespaceDoesNotMatch()
        {
            string xaml = @"<Sequence1 DisplayName='Sequence1' xmlns='http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel'/>";

            IList<XmlnsDefinitionMapping> mappings = new List<XmlnsDefinitionMapping>()
            {
                // note that this is incorrect xmlns  name //
                new XmlnsDefinitionMapping(@"http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel", "System.WorkflowModel.Activities"),
            };

            XamlObjectDeserializer deserializer = new XamlObjectDeserializer(new XamlObjectDeserializerSettings(), new XamlSchemaTypeResolver(mappings));

            try
            {
                var roundTripped = deserializer.Parse(xaml);
            }
            catch (System.Runtime.Xaml.XamlParseException)
            {
                return;
            }

            throw new TestCaseFailedException("Did not get expected XamlParseException");
        }

        // 
        public void MultipleMappings()
        {
            string xaml = @"<Sequence DisplayName='Sequence1' xmlns='http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel'/>";

            IList<XmlnsDefinitionMapping> mappings = new List<XmlnsDefinitionMapping>()
            {
                // note that this is incorrect xmlns  name //
                new XmlnsDefinitionMapping(@"http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel", "System.WorkflowModel"),
                new XmlnsDefinitionMapping(@"http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel", "System.WorkflowModel.Activities"),
            };

            XamlObjectDeserializer deserializer = new XamlObjectDeserializer(new XamlObjectDeserializerSettings(), new XamlSchemaTypeResolver(mappings));

            var roundTripped = deserializer.Parse(xaml);
            if (roundTripped == null)
                throw new TestCaseFailedException("Failed");
        }

        public void AssemblyNameisPartial()
        {
            string xaml = @"<sw:Sequence DisplayName='Sequence1' xmlns:sw='http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel'/>";

            IList<XmlnsDefinitionMapping> mappings = new List<XmlnsDefinitionMapping>()
            {
                new XmlnsDefinitionMapping(@"http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel", "System.WorkflowModel.Activities"),
            };

            XamlObjectDeserializer deserializer = new XamlObjectDeserializer(new XamlObjectDeserializerSettings(), new XamlSchemaTypeResolver(mappings));

            var roundTripped = deserializer.Parse(xaml);

            if (roundTripped == null)
                throw new TestCaseFailedException("Did not get object after deserialization");
        }

        public void MappingForDefaultNamespace()
        {
            string xaml = @"<Sequence DisplayName='Sequence1' xmlns='http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel'/>";

            IList<XmlnsDefinitionMapping> mappings = new List<XmlnsDefinitionMapping>()
            {
                new XmlnsDefinitionMapping(@"http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel", "System.WorkflowModel.Activities"),
            };

            XamlObjectDeserializer deserializer = new XamlObjectDeserializer(new XamlObjectDeserializerSettings(), new XamlSchemaTypeResolver(mappings));

            var roundTripped = deserializer.Parse(xaml);

            if (roundTripped == null)
                throw new TestCaseFailedException("Did not get object after deserialization");
        }

        public void EmptyNamespace()
        {
            string xaml = @"<Sequence DisplayName='Sequence1' xmlns="" />";

            IList<XmlnsDefinitionMapping> mappings = new List<XmlnsDefinitionMapping>()
            {
                new XmlnsDefinitionMapping(@"", "System.WorkflowModel.Activities"),
            };

            XamlObjectDeserializer deserializer = new XamlObjectDeserializer(new XamlObjectDeserializerSettings(), new XamlSchemaTypeResolver(mappings));
            var roundTripped = deserializer.Parse(xaml);
        }

        //Bug4 (email)
        public void NullNamespace()
        {
            string xaml = @"<Sequence DisplayName='Sequence1' xmlns='http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel'/>";

            IList<XmlnsDefinitionMapping> mappings = new List<XmlnsDefinitionMapping>()
            {
                new XmlnsDefinitionMapping(null, "System.WorkflowModel.Activities"),
                new XmlnsDefinitionMapping("http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel", "System.WorkflowModel.Activities"),
            };

            XamlObjectDeserializer deserializer = new XamlObjectDeserializer(new XamlObjectDeserializerSettings(), new XamlSchemaTypeResolver(mappings));
            var roundTripped = deserializer.Parse(xaml);
        }

        // Bug4 (argument exception happens in Assembly.LoadWithPartialName)
        public void EmptyAssemblyName()
        {
            string xaml = @"<Sequence DisplayName='Sequence1' xmlns='http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel'/>";

            IList<XmlnsDefinitionMapping> mappings = new List<XmlnsDefinitionMapping>()
            {
                new XmlnsDefinitionMapping("http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel", ""),
            };

            XamlObjectDeserializer deserializer = new XamlObjectDeserializer(new XamlObjectDeserializerSettings(), new XamlSchemaTypeResolver(mappings));
            var roundTripped = deserializer.Parse(xaml);
        }

        // Bug4 (email)
        public void NullAssemblyName()
        {
            string xaml = @"<Sequence DisplayName='Sequence1' xmlns='http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel'/>";

            IList<XmlnsDefinitionMapping> mappings = new List<XmlnsDefinitionMapping>()
            {
                new XmlnsDefinitionMapping("http://schemas.microsoft.com/netfx/2009/xaml/workflowmodel", null),
            };

            XamlObjectDeserializer deserializer = new XamlObjectDeserializer(new XamlObjectDeserializerSettings(), new XamlSchemaTypeResolver(mappings));
            var roundTripped = deserializer.Parse(xaml);
        }

    }

}
