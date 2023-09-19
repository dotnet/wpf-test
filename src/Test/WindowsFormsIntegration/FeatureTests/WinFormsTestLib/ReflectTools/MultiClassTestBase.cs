// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using ReflectTools;
using WFCTestLib.Log;

namespace ReflectTools {
	/// <remarks>
	/// Base class for multi-class tests, providing functionality for excluding classes and members,
	/// listing known bugs, and logging 

	public class MultiClassTestBase : ReflectBase {
		private string[] _excludedClasses = new string[0];
		private string[] _excludedMembers = new string[0];
		private KnownBug[] _knownBugs = new KnownBug[0];

		// Stat tracking: when LogKnownBugInfo is called it'll keep track of what bugs
		// were logged, and how many unknown bugs there were so these can be reported
		// at the end of the test.
		private List<KnownBug> _knownBugsLogged = new List<KnownBug>();
		private int _unknownBugCount = 0;

		public MultiClassTestBase(string[] args) : base(args) { }

		/// <summary>
		/// Classes excluded from testing.  Use IsExcludedClass() to determine whether to
		/// test a given class.
		/// </summary>
		/// <value>An array of fully-qualified class names.</value>
		public string[] ExcludedClasses {
			get { return _excludedClasses; }
			set { _excludedClasses = value; }
		}

		/// <summary>
		/// Members excluded from testing.  Use IsExcludedMember() to determine whether
		/// to test a given member.
		/// 
		/// If you want to exclude a base class property for all subclasses, refer
		/// to it by its base class name, e.g. Control.Location.  If you want to exclude it
		/// only for a single subclass, use that subclass's name, e.g. Button.Location.
		/// </summary>
		/// <value>An array of member names of the form "Class.Member".</value>
		public string[] ExcludedMembers {
			get { return _excludedMembers; }
			set { _excludedMembers = value; }
		}

		/// <summary>
		/// A list of known bugs associated with class members.  Use LogKnownBugInfo()
		/// to log this 



		public KnownBug[] KnownBugs {
			get { return _knownBugs; }
			set { _knownBugs = value; }
		}

		/// <summary>
		/// A list of known bugs that were logged by LogKnownBugInfo().
		/// </summary>
		/// <value>A List of logged known bugs.</value>
		public List<KnownBug> KnownBugsLogged {
			get { return _knownBugsLogged; }
		}

		/// <summary>
		/// The number of unknown bugs detected by LogKnownBugInfo().
		/// </summary>
		/// <value>The number of unknown bugs detected by LogKnownBugInfo().</value>
		public int UnknownBugCount {
			get { return _unknownBugCount; }
		}

		/// <summary>
		/// Determines if a given type is in the excluded class list.
		/// </summary>
		/// <param name="t">Type for which to search the ExcludedClasses list.</param>
		/// <returns>True if t is an excluded class, false otherwise.</returns>
		public bool IsExcludedClass(Type t) {
			foreach (string className in ExcludedClasses) {
				if (t.ToString() == className)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Determines if a given MemberInfo is in the excluded member list.  NOTE: this
		/// checks the entire inheritance chain of mi's ReflectedType, e.g. if mi is
		/// Button.Text, and "Control.Text" is in the ExcludedMember list, returns true.
		/// </summary>
		/// <param name="mi">MemberInfo for which to search ExcludedMembers list.</param>
		/// <returns>True if mi is an excluded member, false otherwise.</returns>
		public bool IsExcludedMember(MemberInfo mi) {
			foreach (string member in ExcludedMembers) {
				if (ReflectionUtils.MemberMatchesName(mi, member))
					return true;
			}

			return false;
		}

		/// <summary>
		/// If the given MemberInfo is found in one of the KnownBugs, the known 






		public void LogKnownBugInfo(Log log, MemberInfo mi, string alternateFailText) {
			bool foundBug = false;
			foreach (KnownBug bug in KnownBugs) {
				if (bug.CoversMember(mi)) {
					foundBug = true;
					log.LogKnownBug(bug.BugDb, bug.BugId);

					if (!_knownBugsLogged.Contains(bug))
						_knownBugsLogged.Add(bug);
				}
			}

			if (!foundBug) {
				_unknownBugCount++;

				if (!string.IsNullOrEmpty(alternateFailText))
					log.WriteLine(alternateFailText);
			}
		}

		/// <summary>
		/// Overload of LogKnownBugInfo where alternate text defaults to "NO KNOWN 



		public void LogKnownBugInfo(Log log, MemberInfo mi) {
			LogKnownBugInfo(log, mi, "NO KNOWN BUG");
		}

		public void LogBugSummary(Log log) {
			log.WriteTag("BugSummary", false);
			log.WriteTag("KnownBugs", false);
			foreach ( KnownBug bug in _knownBugsLogged ) {
				log.WriteLine(bug.ToString());
			}
			log.CloseTag();
			log.WriteTag("UnknownBugs", true, new LogAttribute("Count", _unknownBugCount.ToString()));
			log.CloseTag();
		}

        /// <summary>
        /// Returns a comma-separated list of bugs that this test hit.  Use this comment in the returned
        /// ScenarioResult so auto-analysis doesn't mistakenly analyze the test because the failure
        /// comment is the same every time, even though the bugs causing failures have changed.
        /// </summary>
        /// <returns></returns>
        public string GetBugComment() {
            StringBuilder buf = new StringBuilder();

            buf.Append("Known Bugs: ");

            for (int i = 0; i < _knownBugsLogged.Count; i++) {
                if (i < _knownBugsLogged.Count - 1)
                    buf.Append(_knownBugsLogged[i].BugId + ",");
                else
                    buf.Append(_knownBugsLogged[i].BugId);
            }

            return buf.ToString();
        }
    }

    static class ReflectionUtils
    {
        // Checks if the property matches the string name, checking the entire inheritance
		// chain of the property (e.g. property = "Button.Size" matches "Control.Size").
		//
		// propertyName is in the form Class.PropertyName
		public static bool MemberMatchesName(MemberInfo mi, string member) {
			int indexOfDot = member.IndexOf('.');
			string className = member.Substring(0, indexOfDot);
			string propertyName = member.Substring(indexOfDot + 1);

			return (mi.Name == propertyName) && (IsInInheritanceChain(mi.ReflectedType, className));
		}

		public static bool IsInInheritanceChain(Type type, string typeName) {
			Type curType = type;

			while (curType != null) {
				if (curType.Name == typeName)
					return true;

				curType = curType.BaseType;
			}

			return false;
		}
	}

	#region KnownBug class
	/// <remarks>
	/// Represents a known 

	public class KnownBug {
		List<string> _memberNames = new List<string>();
		BugDb _bugDb;
		int _bugId;

		/// <summary>
		/// Construct a KnownBug.
		/// </summary>
		/// <param name="bugDb">


		public KnownBug(BugDb bugDb, int bugId, string[] memberNames) {
			// HACK: string[] is supposed to implement IList<T> but it doesn't yet.
			// m_memberNames.AddRange(memberNames);
			foreach (string memberName in memberNames) {
				_memberNames.Add(memberName);
			}

			_bugDb = bugDb;
			_bugId = bugId;
		}

		public KnownBug(BugDb bugDb, int bugId, string memberName)	: this(bugDb, bugId, new string[] { memberName }) { }
		public KnownBug(int bugId, string[] memberNames)			: this(BugDb.VSWhidbey, bugId, memberNames) { }
		public KnownBug(int bugId, string memberName)				: this(BugDb.VSWhidbey, bugId, memberName) { }

		public List<string> MemberNames {
			get { return _memberNames; }
		}

		public BugDb BugDb {
			get { return _bugDb; }
			set { _bugDb = value; }
		}

		public int BugId {
			get { return _bugId; }
			set { _bugId = value; }
		}

		/// <summary>
		/// Does this 




		public bool CoversMember(MemberInfo mi) {
			foreach (string memberName in _memberNames) {
				if (ReflectionUtils.MemberMatchesName(mi, memberName))
					return true;
			}

			return false;
		}

		public override string ToString() {
			return _bugDb + " #" + _bugId;
		}

		// Same implementation as ToString() but we don't want to tie this method to
		// ToString().
		public override int GetHashCode() {
			return (_bugDb + " #" + _bugId).GetHashCode();
		}
	}
	#endregion
}
