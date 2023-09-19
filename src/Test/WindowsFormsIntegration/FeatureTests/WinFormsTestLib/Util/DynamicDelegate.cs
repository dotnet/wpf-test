// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Security.Permissions;
using WFCTestLib.Log;
using ReflectTools;
using System.Collections.ObjectModel;

#endregion

namespace WFCTestLib.Util
{

    /// <summary>
    /// The zero-index in the parameters array will be a string which contains the name of the attached event
    /// the one-index will usually be the "sender" parameter
    /// the two-index will usually be System.EventArgs-derived
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public delegate object DelegateHandler(object[] parameters);

    public delegate void TypedDelegateHandler(string eventName, object sender, EventArgs e);

    public class DynamicDelegate
    {
        public class EventHandlerRef
        {
            internal EventHandlerRef(Delegate routerDelegate, object eventSourceObject, EventInfo eventToAttach)
            {
                this._routerDelegate = routerDelegate;
                this._eventSourceObject = eventSourceObject;
                this._eventToAttach = eventToAttach;
            }
            Delegate _routerDelegate;
            object _eventSourceObject;
            EventInfo _eventToAttach;
            public void DetachHandler()
            { _eventToAttach.RemoveEventHandler(_eventSourceObject, _routerDelegate); }
            internal void AttachHandlerActual()
            { _eventToAttach.AddEventHandler(_eventSourceObject, _routerDelegate); }
        }

        public static void DetachHandler(EventHandlerRef eventHandlerRef)
        { eventHandlerRef.DetachHandler(); }

        public static EventHandlerRef AddTypedHandler(object source, EventInfo eventToAttach, TypedDelegateHandler target)
        {
            Type eventHandlerType = eventToAttach.EventHandlerType;
            MethodInfo invokeMethod = eventHandlerType.GetMethod("Invoke");
            ParameterInfo[] invokeParameters = invokeMethod.GetParameters();
            if (null == invokeParameters || invokeParameters.Length != 2 || !typeof(object).IsAssignableFrom(invokeParameters[0].ParameterType) || typeof(System.EventArgs).IsAssignableFrom(invokeParameters[1].ParameterType))
            {
                throw new ArgumentException("TypedDelegateHandler can only accept delegates with the (object, EventArgs) signature");
            }
            return AddHandler(source, eventToAttach, delegate(object[] parameters)
            {
                target(
                   (string)parameters[0],
                   (object)parameters[1],
                   (EventArgs)parameters[2]);
                return null;
            });
        }
        public static EventHandlerRef AddHandler(object source, EventInfo eventToAttach, DelegateHandler target)
        {
            new NamedPermissionSet("FullTrust").Assert();
            AssemblyBuilder _AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicAssembly"), AssemblyBuilderAccess.Run);
            ModuleBuilder _ModuleBuilder = _AssemblyBuilder.DefineDynamicModule("DelegateModule");


            if (null == target) { throw new ArgumentNullException("target"); }
            if (null == eventToAttach) { throw new ArgumentNullException("eventToAttach", "name"); }

            Type eventHandlerType = eventToAttach.EventHandlerType;
            string typeName = "Dynamic_" + eventHandlerType.Name + Guid.NewGuid().ToString("N");
            TypeBuilder typeBuilder = _ModuleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.AnsiClass | TypeAttributes.AutoClass, typeof(object));
            MethodInfo invokeMethod = eventHandlerType.GetMethod("Invoke");

            ParameterInfo[] invokeParameters = invokeMethod.GetParameters();
            Type[] parameterTypes = new Type[invokeParameters.Length];
            for (int i = 0; i < parameterTypes.Length; ++i)
            { parameterTypes[i] = invokeParameters[i].ParameterType; }

            FieldBuilder delegateHandlerField = typeBuilder.DefineField("TargetDelegate", typeof(DelegateHandler), FieldAttributes.Private);
            FieldBuilder eventNameField = typeBuilder.DefineField("EventName", typeof(string), FieldAttributes.Private);
            #region Build Constructor
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(DelegateHandler), typeof(string) });
            constructorBuilder.DefineParameter(1, ParameterAttributes.None, "handler");
            constructorBuilder.DefineParameter(2, ParameterAttributes.None, "eventName");
            ILGenerator constructorGenerator = constructorBuilder.GetILGenerator();
            //Put the this parameter on the stack
            constructorGenerator.Emit(OpCodes.Ldarg_0);
            //call base constructor
            constructorGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(new Type[0]));
            //this parameter on stack
            constructorGenerator.Emit(OpCodes.Ldarg_0);
            //input parameter on the stack
            constructorGenerator.Emit(OpCodes.Ldarg_1);
            //stuff it into the field
            constructorGenerator.Emit(OpCodes.Stfld, delegateHandlerField);
            //this parameter on stack
            constructorGenerator.Emit(OpCodes.Ldarg_0);
            //put string parameter on the stack
            constructorGenerator.Emit(OpCodes.Ldarg_2);
            //stuff it into the field
            constructorGenerator.Emit(OpCodes.Stfld, eventNameField);
            //return
            constructorGenerator.Emit(OpCodes.Ret);
            #endregion Build Constructor
            #region Build handler method
            MethodBuilder methodBuilder = typeBuilder.DefineMethod("DynamicHandler", MethodAttributes.Public, CallingConventions.HasThis, invokeMethod.ReturnType, parameterTypes);

            for (int i = 0; i < parameterTypes.Length; ++i)
            { methodBuilder.DefineParameter(i + 1, invokeParameters[i].Attributes, invokeParameters[i].Name); }

            ILGenerator ilGenerator = methodBuilder.GetILGenerator();

            //define the parameter to the delegate
            ilGenerator.DeclareLocal(typeof(object[]));
            //push the number of parameters onto the callstack
            ilGenerator.Emit(OpCodes.Ldc_I4, invokeParameters.Length + 1);
            //Create the new array
            ilGenerator.Emit(OpCodes.Newarr, typeof(object));
            //Store the reference to the new object array into the local member
            ilGenerator.Emit(OpCodes.Stloc_0);

            //Load the location of the string array
            ilGenerator.Emit(OpCodes.Ldloc_0);
            //Load the zero index (for the invoker name
            ilGenerator.Emit(OpCodes.Ldc_I4_0);
            //Load the this pointer
            ilGenerator.Emit(OpCodes.Ldarg_0);
            //Load the field value
            ilGenerator.Emit(OpCodes.Ldfld, eventNameField);
            //stuff it into the array
            ilGenerator.Emit(OpCodes.Stelem_Ref);
            for (int i = 1; i <= invokeParameters.Length; ++i)
            {
                //Load the location of the string array
                ilGenerator.Emit(OpCodes.Ldloc_0);
                //Load the current parameter index number
                ilGenerator.Emit(OpCodes.Ldc_I4, i);
                //Load the argument reference
                ilGenerator.Emit(OpCodes.Ldarg, i);
                //Copy the reference into the appropriate array index
                ilGenerator.Emit(OpCodes.Stelem_Ref);
            }
            //this parameter
            ilGenerator.Emit(OpCodes.Ldarg_0);
            //load the delegate field 
            ilGenerator.Emit(OpCodes.Ldfld, delegateHandlerField);
            //load the object array
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.EmitCall(OpCodes.Callvirt, typeof(DelegateHandler).GetMethod("Invoke", ~BindingFlags.Static), null);
            if (invokeMethod.ReturnType == typeof(void))
                ilGenerator.Emit(OpCodes.Pop);
            ilGenerator.Emit(OpCodes.Ret);
            #endregion Build handler method
            Type routerType = typeBuilder.CreateType();

            ConstructorInfo constructorInfo = routerType.GetConstructor(new Type[] { typeof(DelegateHandler), typeof(string) });
            object o = constructorInfo.Invoke(new object[] { target, eventToAttach.Name });

            EventHandlerRef r = new EventHandlerRef(Delegate.CreateDelegate(eventHandlerType, o, methodBuilder.Name), source, eventToAttach);
            r.AttachHandlerActual();
            return r;
        }
    }
}
