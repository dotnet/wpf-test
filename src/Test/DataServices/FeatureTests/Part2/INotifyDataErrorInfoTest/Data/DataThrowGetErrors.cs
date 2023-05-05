// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// Data object that implementing INotifyDataErroInfoData.
    /// Throws an exception in INotifyDataErrorInfo.GetErrors.
    /// </summary>
    public class DataThrowGetErrors : INotifyDataErrorInfoObject<object>
    {
        #region Constructors

        public DataThrowGetErrors(bool async, bool throwCritical)
            : base(async)
        {
            GetErrorsThrows = true;
            ThrowCritical = throwCritical;
        }

        #endregion Constructors

        /// <summary>
        /// Validation for StringProperty
        /// 1) If string is longer than 10 characters
        /// 2) If string contains '#'
        /// </summary>
        public override void ValidateStringProperty(string key, string value)
        {
            UpdateErrors(AddError, key, null);
        }

        /// <summary>
        /// Validation for IntProperty
        /// 1) If int is > 999
        /// 2) If int is odd
        /// 3) If int is negative, show error for StringProperty
        /// 4) If not an int, default convert will throw an exception
        /// </summary>
        public override void ValidateIntProperty(string key, int value)
        {
            UpdateErrors(AddError, key, null);
        }
    }
}
