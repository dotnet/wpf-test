// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ReflectTools {
    using System;
    using System.Collections;
    using System.Security;
    
    //
    // Strongly typed collection for storing Permissions.
    //
    [Serializable()]
    public class PermissionCollection : CollectionBase {
        
        public PermissionCollection() {
        }
        
        public PermissionCollection(PermissionCollection value) {
            this.AddRange(value);
        }
        
        public PermissionCollection(CodeAccessPermission[] value) {
            this.AddRange(value);
        }
        
        public CodeAccessPermission this[int index] {
            get {
                return ((CodeAccessPermission)(List[index]));
            }
            set {
                List[index] = value;
            }
        }
        
        public int Add(CodeAccessPermission value) {
            return List.Add(value);
        }
        
        public void AddRange(CodeAccessPermission[] value) {
            for (int i = 0; (i < value.Length); i = (i + 1)) {
                this.Add(value[i]);
            }
        }
        
        public void AddRange(PermissionCollection value) {
            for (int i = 0; (i < value.Count); i = (i + 1)) {
                this.Add(value[i]);
            }
        }
        
        public bool Contains(CodeAccessPermission value) {
            return List.Contains(value);
        }
        
        public void CopyTo(CodeAccessPermission[] array, int index) {
            List.CopyTo(array, index);
        }
        
        public int IndexOf(CodeAccessPermission value) {
            return List.IndexOf(value);
        }
        
        public void Insert(int index, CodeAccessPermission value) {
            List.Insert(index, value);
        }
        
        public void Remove(CodeAccessPermission value) {
            List.Remove(value);
        }
    }
}
