// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.DataServices
{
    public class ErrorObject
    {
        public ErrorObject(ErrorLevel level, string message)
        {
            this.Message = message;
            this.Level = level;
        }

        public string Message { get; set; }

        public ErrorLevel Level { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Level, Message);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ErrorObject))
                return false;

            ErrorObject compare = obj as ErrorObject;

            if (!this.Level.Equals(compare.Level) && !this.Message.Equals(compare.Message))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public enum ErrorLevel { Warning, Error }
}
