using System;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// This tells whats the target type for a TestCase or Action
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TargetType : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public TargetType(Type type)
        {
            targetType = type;
        }

        /// <summary>
        /// 
        /// </summary>
        public Type Target
        {
            get
            {
                return targetType;
            }
        }

        private Type targetType;
    }
}
