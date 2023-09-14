// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using Avalon.Test.CoreUI.Common;

using System.Runtime.InteropServices;

using System.Runtime.Serialization;



namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// </summary>
    public class ValidationTracking
    {
        /// <summary>
        /// </summary>
        public ValidationTracking(int numberValidation)
        {
           _validation = new bool[numberValidation];
           _validationString = new string[numberValidation];
        }

        /// <summary>
        /// </summary>
        public void AddValidation(int number, string message)
        {
            if (number >= _validation.Length)
                throw new ArgumentException("The number is bigger than the expected Number of validations");

            _validationString[number] = message;

        }

        /// <summary>
        /// </summary>
        public void SetValue(int validationNumber, bool passed)
        {
            if (validationNumber >= _validation.Length)
                throw new ArgumentException("The number is bigger than the expected Number of validations");

            _validation[validationNumber] = passed;
        }


        /// <summary>
        /// </summary>
        public void ValidateAll()
        {
            ValidateUntil(_validation.Length-1, false);
        }


        /// <summary>
        /// </summary>
        public void ValidateUntil(int number,bool validateRestFalse)
        {
            if (number >= _validation.Length)
                throw new ArgumentException("");            
            
            for (int i = 0; i<= number; i++)
            {
                if (!_validation[i])
                    throw new Microsoft.Test.TestValidationException(_validationString[i]);
            }

            if (validateRestFalse)
            {
                for (int i = number+1; i < _validation.Length;i++)
                {
                    if (_validation[i])
                        throw new Microsoft.Test.TestValidationException("This should not be true. " +
                            _validationString[i]);                
                }
            }
            
        }

        private bool[] _validation;
        private string[] _validationString;
    }
    
}
