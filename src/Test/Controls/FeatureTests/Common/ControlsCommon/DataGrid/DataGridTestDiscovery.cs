using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGrid test discovery.
    /// </summary>
    public static class DataGridTestDiscovery
    {
        public static List<Type> GetTests<T>(DataGridFeature dataGridFeature, DataGridTestType dataGridTestType)
        {
            List<Type> tests = new List<Type>();
            Assembly assembly = Assembly.GetAssembly(typeof(DataGridBehaviorTest));
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                foreach (Attribute attribute in type.GetCustomAttributes(true))
                {
                    if (attribute is DataGridTestAttribute)
                    {
                        if (((DataGridTestAttribute)attribute).DataGridType == typeof(T) && ((DataGridTestAttribute)attribute).DataGridFeature == dataGridFeature)
                        {
                            if (!tests.Contains(type))
                            {
                                if (dataGridTestType == DataGridTestType.All)
                                {
                                    tests.Add(type);
                                }
                                else
                                {
                                    if (((DataGridTestAttribute)attribute).DataGridTestType == dataGridTestType)
                                    {
                                        tests.Add(type);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return tests;
        }
    }
}
