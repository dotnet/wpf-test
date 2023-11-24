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
