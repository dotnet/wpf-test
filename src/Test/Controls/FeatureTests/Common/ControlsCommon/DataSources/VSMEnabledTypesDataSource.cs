using System;
using System.Reflection;
using System.Collections;
using System.Linq;

namespace Microsoft.Test.Controls
{
#if TESTBUILD_CLR40
    //public class VSMEnabledTypes
    //{
    //    public IEnumerable GetUIElementTypes()
    //    {
    //        // One by one we go through the assemblies that contain SL equivalents and try to find a type
    //        Type[] typesForAssemblies = new Type[] 
    //        {
    //            typeof(System.Windows.DependencyObject),                                // WindowsBase.dll
    //            typeof(System.Windows.Media.Animation.TimelineCollection),              // PresentationCore.dll
    //            typeof(System.Windows.FrameworkElement),                                // PresentationFramework.dll
    //            typeof(System.Xml.ConformanceLevel),                                    // System.Xml.dll
    //            typeof(System.Net.AuthenticationManager),                               // System.dll
    //            typeof(System.Xml.Linq.Extensions),                                     // System.Xml.Linq.dll
    //            typeof(System.Object)                                                  // mscorlib.dll
    //        };

    //        return
    //            from representativeType in typesForAssemblies
    //            from typeInAssembly in Assembly.GetAssembly(representativeType).GetTypes()
    //            where typeof(Control).IsAssignableFrom(typeInAssembly)
    //            where representativeType.IsDefined(typeof(TemplateVisualStateAttribute), false)
    //            where !typeInAssembly.IsAbstract
    //            where HasDefaultConstructor(typeInAssembly)
    //            orderby typeInAssembly.Name
    //            select typeInAssembly;
    //    }

    //    private bool HasDefaultConstructor(Type type)
    //    {
    //        var constructors = type.GetConstructors();
    //        foreach (ConstructorInfo ci in constructors)
    //        {
    //            if (ci.GetParameters().Count() == 0)
    //                return true;
    //        }

    //        return false;
    //    }
    //}
#endif
}
