// IValidation.cs
// Defines IValidation interface
//
// Test cases call validator classes to validate actions. Validator classes implement IValidation 
// interface  

using System;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// IValidation interface
    /// </summary>
    public interface IValidation
    {
        bool Validate(params object[] validationParams);
    }
}
