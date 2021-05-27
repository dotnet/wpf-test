// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Collections.Generic;

namespace Microsoft.Test.Integration
{
    /// <summary>
    /// 
    /// </summary>
    public enum StorageItemType
    {
        ///<summary>
        ///</summary>
        Input,
        
        ///<summary>
        ///</summary>        
        Output
    }

    /// <summary>
    /// 
    /// </summary>
    public class StorageItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="siInput"></param>
        /// <param name="siOutput"></param>
        /// <returns></returns>
        internal static bool Matches(StorageItem siInput, StorageItem siOutput)
        {
            if (siInput.StorageItemType != StorageItemType.Input)
            {
                throw new ArgumentException("The siInput is not of StorageItemType.Input.", "siInput");
            }

            if (siOutput.StorageItemType != StorageItemType.Output)
            {
                throw new ArgumentException("The siOutput is not of StorageItemType.Output.", "siOutput");
            }

            if (!siInput.Type.Equals(siOutput.Type) ||
                String.Compare(siInput.Name, siOutput.Name, false) != 0)
            {
                return false;
            }

            // If Input doesn't have name, this mean that input only cares about something in storage

            if (String.IsNullOrEmpty(siInput.Name) &&
                siInput.Type.Equals(siOutput.Type))
            {
                return true;
            }

            // If Input has a name, output has to match the name.
            if (!String.IsNullOrEmpty(siInput.Name) &&
                String.Compare(siInput.Name, siOutput.Name, false) == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        internal StorageItemType StorageItemType
        {
            get { return _storageItemType; }
            set { _storageItemType = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public TypeDesc Type
        {
            get { return _typeName; }
            set { _typeName = value; }
        } 


        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="si1"></param>
        /// <param name="si2"></param>
        /// <returns></returns>
        public static bool operator ==(StorageItem si1, StorageItem si2)
        {
            if (Object.ReferenceEquals(si1, null) || Object.ReferenceEquals(si2, null))
            {
                return false;
            }
            
            return si1.Equals(si2);
        }

        ///<summary>
        ///</summary>
        public static bool operator !=(StorageItem si1, StorageItem si2)
        {
            if (Object.ReferenceEquals(si1, null) || Object.ReferenceEquals(si2, null))
            {
                return false;
            }

            return !si1.Equals(si2);
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            StorageItem si = obj as StorageItem;
            
            if (si != null)
            {
                if (si.Type.Equals(this.Type) &&
                    String.Compare(this.Name, si.Name, false) == 0 &&
                    this.StorageItemType == si.StorageItemType)
                {
                    return true;
                }                        
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

        private StorageItemType _storageItemType = StorageItemType.Input;
        private TypeDesc _typeName;
        private string _name;        
    }
}
