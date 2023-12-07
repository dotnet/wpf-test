// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ReflectTools {
	public class ReflectBaseException: ApplicationException {
		public ReflectBaseException() : base() { }
		public ReflectBaseException(string s) : base(s) { }
		public ReflectBaseException(string s, Exception e) : base(s, e) { }
	}

	public class InitTestFailedException: ReflectBaseException {
		public InitTestFailedException() : base() { }
		public InitTestFailedException(string s) : base(s) { }
		public InitTestFailedException(string s, Exception e) : base(s, e) { }
	}
}
