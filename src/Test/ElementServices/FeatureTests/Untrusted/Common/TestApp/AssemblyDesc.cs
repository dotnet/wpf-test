// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class AssemblyDesc
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        public virtual void UpdateInfo(object o)
        {
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }
                       
            Assembly assembly = null;

            if (o is Assembly)
            {
                assembly = (Assembly)o;
            }

            if (o is Assembly)
            {
                assembly = (Assembly)o;
            }

            if (assembly == null)
            {
                return;
            }

            AssemblyName = assembly.GetName().FullName;
        }

        /// <summary>
        /// 
        /// </summary>
        public string AssemblyName
        {
            get { return _assemblyName; }
            set { _assemblyName = value; }
        }

        /// <summary>
        /// If AssemblyName is null or empty, the return value is null.  If not the Assembly 
        /// that corresponds to AssemblyName       
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public Assembly GetAssembly()
        {
            if (String.IsNullOrEmpty(AssemblyName))
            {
                return null;
            }

            if (String.IsNullOrEmpty(HintPath))
            {
                return Helper.LoadAssembly(AssemblyName);
            }
            else
            {
                return Helper.LoadAssembly(AssemblyName, HintPath);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual void ValidateState()
        {
            if (String.IsNullOrEmpty(AssemblyName))
            {
                throw new InvalidOperationException("AssemblyName is null or empty.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public virtual void Copy(object source)
        {
            AssemblyDesc assemblyDescSource = source as AssemblyDesc;

            if (assemblyDescSource != null)
            {
                this.AssemblyName = String.Copy(assemblyDescSource.AssemblyName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is AssemblyDesc))
            {
                return false;
            }

            Assembly objA = ((AssemblyDesc)obj).GetAssembly();

            if (objA.GetName() == this.GetAssembly().GetName())
            {
                return true;
            }

            return false;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue("")]
        public string HintPath
        {
            get
            {
                //if (_hintPath == String.Empty)
                //{
                    return _hintPath;
                //}
                //else
                //{
                //    return TestExtenderHelper.QualifyPath(_hintPath);
                //}
            }
            set { _hintPath = value; }
        }

        private string _assemblyName = "";
        private string _hintPath = String.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    public class TypeDesc : AssemblyDesc
    {
        /// <summary>
        /// 
        /// </summary>
        public TypeDesc() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public TypeDesc(Type type)
        {
            UpdateInfo(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is TypeDesc))
            {
                return false;
            }

            if (!base.Equals(obj))
            {
                return false;
            }

            TypeDesc t = (TypeDesc)obj;

            if (String.Compare(this._typeName, t._typeName, false) == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public override void Copy(object source)
        {
            base.Copy(source);

            TypeDesc typeDescSource = source as TypeDesc;

            if (typeDescSource != null)
            {
                this.TypeName = String.Copy(typeDescSource.TypeName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        public override void UpdateInfo(object o)
        {
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }

            Type type = null;

            if (o is Type)
            {
                type = (Type)o;
            }

            if (o is Type)
            {
                type = (Type)o;
            }

            if (type == null)
            {
                return;
            }

            TypeName = type.FullName;

            base.UpdateInfo(type.Assembly);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string TypeName
        {
            get { return _typeName; }
            set { _typeName = value; }
        }

        /// <summary>
        /// Get the Type correspoding to TypeName property.
        /// </summary>
        /// <returns>Null if TypeName is empty, null or if the Type cannot be found. If not the Type correspoding to TypeName.</returns>
        [CLSCompliant(false)] 
        public Type GetCurrentType()
        {
            if (String.IsNullOrEmpty(TypeName))
            {
                return null;
            }

            Assembly assembly = GetAssembly();
            
            if (assembly != null)
            {
                return assembly.GetType(TypeName, true);
            }


            return null;
        }

            
        /// <summary>
        /// 
        /// </summary>
        public override void ValidateState()
        {
            base.ValidateState();

            if (String.IsNullOrEmpty(TypeName))
            {
                throw new InvalidOperationException("TypeName is null or empty.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public object CreateInstance(object[] args)
        {
            Type currentType = this.GetCurrentType();

            // Static = abstract and sealed.
            if (((currentType.Attributes & TypeAttributes.Abstract) != 0) && ((currentType.Attributes & TypeAttributes.Sealed) != 0))
            {
                return null;
            }

            return Activator.CreateInstance(this.GetCurrentType(), args);
        }
        
        private string _typeName = "";
        
    }

    /// <summary>
    /// 
    /// </summary>
    public class MethodDesc : TypeDesc
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is MethodDesc))
            {
                return false;
            }

            if (!base.Equals(obj))
            {
                return false;
            }

            MethodDesc t = (MethodDesc)obj;

            if (String.Compare(this._methodName, t._methodName, false) == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public override void Copy(object source)
        {
            base.Copy(source);

            MethodDesc methodDescSource = source as MethodDesc;

            if (methodDescSource != null)
            {
                this.MethodName = String.Copy(methodDescSource.MethodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string MethodName
        {
            get { return _methodName; }
            set { _methodName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public MethodInfo GetMethodInfo()
        {
            Type type = GetCurrentType();

            MethodInfo methodInfo = type.GetMethod(_methodName, 
                BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            return methodInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callbackType"></param>        
        /// <param name="objectType"></param>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        public Delegate GetDelegate(Type callbackType, ref object objectType, bool throwOnError)
        {
            Delegate t = null;
            MethodInfo mi = GetMethodInfo();
            Type targetObjectType = GetCurrentType();
            objectType = null;

            try
            {               
                if (!mi.IsStatic)
                {
                    objectType = Activator.CreateInstance(targetObjectType);
                    t = Delegate.CreateDelegate(callbackType, objectType, _methodName, false, false);
                }
                else
                {
                    t = Delegate.CreateDelegate(callbackType, mi, false);
                }
            }
            catch (Exception) { }

            if (t == null && throwOnError)
            {
                throw new InvalidOperationException("The callback is not of type CallbackContentItem or was not found.");
            }

            return t;
        }


        /// <summary>
        /// 
        /// </summary>
        public override void ValidateState()
        {
            base.ValidateState();

            if (String.IsNullOrEmpty(MethodName))
            {
                throw new InvalidOperationException("MethodName is null or empty.");
            }
        }

        private string _methodName = "";
    }

}
