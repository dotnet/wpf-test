// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// Data object that implementing INotifyDataErroInfoData.
    /// Class uses ErrorObject when reporting validattion errors.
    /// </summary>
    public class DataObjectErrors : INotifyDataErrorInfoObject<ErrorObject>
    {
        #region Constructors

        public DataObjectErrors(bool async) : base(async) { }

        #endregion Constructors

        /// <summary>
        /// Validation for StringProperty
        /// 1) If string is longer than 10 characters
        /// 2) If string contains '#'
        /// </summary>
        public override void ValidateStringProperty(string key, string value)
        {
            if (value.Length > 10)
            {
                UpdateErrors(AddError, key, new ErrorObject(ErrorLevel.Error, ErrorStrings.STRING_TOO_LONG));
            }
            else
            {
                UpdateErrors(RemoveError, key, new ErrorObject(ErrorLevel.Error, ErrorStrings.STRING_TOO_LONG));
            }

            if (value.Contains("#"))
            {
                UpdateErrors(AddError, key, new ErrorObject(ErrorLevel.Warning, ErrorStrings.STRING_INVALID_CHARACTER));
            }
            else
            {
                UpdateErrors(RemoveError, key, new ErrorObject(ErrorLevel.Warning, ErrorStrings.STRING_INVALID_CHARACTER));
            }
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
            if (value > 999)
            {
                UpdateErrors(AddError, key, new ErrorObject(ErrorLevel.Error, ErrorStrings.INT_TOO_LARGE));
            }
            else
            {
                UpdateErrors(RemoveError, key, new ErrorObject(ErrorLevel.Error, ErrorStrings.INT_TOO_LARGE));
            }

            if (value % 2 != 0)
            {
                UpdateErrors(AddError, key, new ErrorObject(ErrorLevel.Warning, ErrorStrings.INT_CANNOT_BE_ODD));
            }
            else
            {
                UpdateErrors(RemoveError, key, new ErrorObject(ErrorLevel.Warning, ErrorStrings.INT_CANNOT_BE_ODD));
            }

            if (value < 0)
            {
                // Ignore key, notify error for StringProperty
                UpdateErrors(AddError, "StringProperty", new ErrorObject(ErrorLevel.Error, ErrorStrings.INT_CANNOT_BE_NEGATIVE));
            }
            else
            {
                // Ignore key, notify error for StringProperty
                UpdateErrors(RemoveError, "StringProperty", new ErrorObject(ErrorLevel.Error, ErrorStrings.INT_CANNOT_BE_NEGATIVE));
            }
        }
    }
}
