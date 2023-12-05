// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;

namespace WFCTestLib.Log
{
    // An object for indicating the result of a scenario.  Scenarios have two
    // types: FailType and PassType.  Also supports use of "sub scenarios" where
    // an individual scenario is made up of smaller pass/fail components.  An
    // example of this would be a scenario that loops through all the fonts
    // on the system.  Even though there are 1008 chances to fail, the scenario
    // only counts for one pass/fail.
    //
    public class ScenarioResult
    {
        //
        public static Version OrcasVersion
        { get { return new Version(2, 1); } }
        public static Version WhidbeyVersion
        { get { return new Version(2, 0); } }
        public static Version EverettVersion
        { get { return new Version(1, 1); } }
        public static Version RTMVersion
        { get { return new Version(1, 0); } }
        public static Version CurrentVersion
        { get { return Environment.Version; } }
        
        private void LogLineNumber()
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
            bool found = false;
            for (int i = 0; i < st.FrameCount; ++i)
            {
                if (st.GetFrame(i).GetMethod().DeclaringType.Assembly != System.Reflection.Assembly.GetCallingAssembly())
                {
                    if (st.GetFrame(i).GetFileLineNumber() != 0)
                    { ReflectTools.ReflectBase.ScenarioParams.log.WriteLine(string.Format("File: {0} Line: {1} in method {2}", st.GetFrame(i).GetFileName(), st.GetFrame(i).GetFileLineNumber(), st.GetFrame(i).GetMethod().Name)); }
                    found = true;
                }
                else if (found) { return; }
            }
        }
        // <doc>
        // <desc>
        //  Counters used for looping scenarios.
        // </desc>
        // <seealso member="IncCounters"/>
        // </doc>
        private int _pass,_fail;

        // <doc>
        // <desc>
        //  The actual type of this ScenarioResult.
        // </desc>
        // <seealso member="FailType"/>
        // <seealso member="PassType"/>
        // </doc>
        private bool _type = false;

        // <doc>
        // <desc>
        //  The comments associated with this ScenarioResult.
        // </desc>
        // </doc>
        private String _comments;

        // <doc>
        // <desc>
        //  Constructs a new ScenarioResult
        // </desc>
        // </doc>
        public ScenarioResult() {
            _pass = _fail = 0;
        }

        // <doc>
        // <desc>
        //  Constructs a new ScenarioResult of the specified type.
        // </desc>
        // <param term="b">
        //  True indicates success; false indicates failure.
        // </param>
        // </doc>
        public ScenarioResult(bool b) : this() {
            this._type = b;
            //if (!b)
            //{ LogLineNumber(); }
        }

        // <doc>
        // <desc>
        //  Constructs a new ScenarioResult of the specified type.
        // </desc>
        // <param term="b">
        //  True indicates success; false indicates failure.
        // </param>
        // <param term="comments">
        //  If "b" is false, the Comments property is set to this value.
        // </param>
        // </doc>
        public ScenarioResult(bool b, string comments) : this(b) {
			if (!b)
			{
				if (Comments == null || Comments.Trim() == String.Empty || Comments.Trim() == "")
					Comments = comments;
				else
					Comments += "\r\n" + comments;
			}
		}

        // <doc>
        // <desc>
        //  Constructs a new ScenarioResult of the specified type, and if this
        //  is of type Fail, prints the comments to the given log object.
        // </desc>
        // <param term="b">
        //  True indicates success; false indicates failure.
        // </param>
        // <param term="comments">
        //  If "b" is false, the Comments property is set to this value and printed to the log.
		//  Should NOT contain any random values, or auto-analysis tools will fail.
        // </param>
        // <param term="log">
        //  If "b" is false, prints comments to the log.
        // </param>
        // </doc>
        public ScenarioResult(bool b, string comments, Log log) : this(b, comments) {
            if ( !b && log != null )
                log.WriteLine(comments);
        }

		/// <summary>
		///  Constructs a new ScenarioResult of the specified type, and if this
		///  is of type Fail, prints the comments to the given log object.
		/// </summary>
		/// <param name="b">True indicates success; false indicates failure.</param>
		/// <param name="comments">
		///  If "b" is false, the Comments property is set to this value and printed to the log.
		///  Should NOT contain any random values, or auto-analysis tools will fail.
		/// </param>
		/// <param name="details">
		///  Additional details, which may contain random values.  This information is printed
		///  to the log but does not appear in the comment, and thus does not affect auto-analysis.
		/// </param>
		/// <param name="log">If "b" is false, prints comments to the log.</param>
		public ScenarioResult(bool b, string comments, string details, Log log) : this(b, comments, log) {	// prints out the comments
			if (!b && log != null)
				log.WriteLine(details);
		}

		/// <summary>
		///  Constructs a new ScenarioResult of the specified type, and if this
		///  is of type Fail, prints the comments to the given log object.
		/// </summary>
		/// <param name="b">True indicates success; false indicates failure.</param>
		/// <param name="comments">
		///  If "b" is false, the Comments property is set to this value and printed to the log.
		///  Should NOT contain any random values, or auto-analysis tools will fail.
		/// </param>
		/// <param name="expected">The expected value.  Printed to the log if b is false.</param>
		/// <param name="actual">The actual value.  Printed to the log if b is false.</param>
		/// <param name="log">If "b" is false, prints comments and actual/expected values to the log.</param>
		public ScenarioResult(bool b, string comments, object expected, object actual, Log log) : this(b, comments, log) {	// prints out the comments
			if (!b && log != null)
			{
				//log.WriteLine("Expected: {0}; Actual: {1}", expected, actual);
				log.LogExpectedActual(expected, actual);
			}
		}

		/// <summary>
		///  Constructs a new ScenarioResult of the specified type, and if expected.Equals(actual)
		///  is false, it's considered a Fail, and the comments and actual/expected values are
		///  printed to the given log object.
		/// </summary>
		/// <param name="expected">The expected value.  Printed to the log if b is false.</param>
		/// <param name="actual">The actual value.  Printed to the log if b is false.</param>
		/// <param name="log">If fail, prints comments and actual/expected values to the log.</param>
		public ScenarioResult(object expected, object actual, Log log) :
			this(expected, actual, "FAIL: Expected didn't match actual value", log)	{ }

		/// <summary>
		///  Constructs a new ScenarioResult of the specified type, and if expected.Equals(actual)
		///  is false, it's considered a Fail, and the comments and actual/expected values are
		///  printed to the given log object.
		/// </summary>
		/// <param name="expected">The expected value.  Printed to the log if b is false.</param>
		/// <param name="actual">The actual value.  Printed to the log if b is false.</param>
		/// <param name="comments">
		///  If !expected.Equals(actual), the Comments property is set to this value and printed to the log.
		///  Should NOT contain any random values, or auto-analysis tools will fail.
		/// </param>
		/// <param name="log">If fail, prints comments and actual/expected values to the log.</param>
		public ScenarioResult(object expected, object actual, string comments, Log log) :
			this(TestValues(expected, actual), comments, log)	// prints out the comments
		{
			if (!TestValues(expected, actual) && log != null)
			{
				//log.WriteLine("Expected: {0}; Actual: {1}", expected, actual);
				log.LogExpectedActual(expected, actual);
			}
		}

        public ScenarioResult(bool b, Log log, BugDb db, int bugId, string bugComment) : this(b, MakeBugComment(db, bugId, bugComment)) {
            if ( !b && log != null )
                log.LogKnownBug(db, bugId, bugComment);
        }

        public ScenarioResult(bool b, Log log, BugDb db, int bugId) : this(b, log, db, bugId, null) { }

		/// <summary>
		/// Constructs a new Scenario Result. If the Hack has expired it is considered a Fail else it 
		/// it considered Pass. Appropriate comments are added to the log file. 
		/// </summary>
		/// <param name="hackComments">
		///  If failure, then Comments property is set to this value and printed to the
		///  given log.
		/// </param>
		/// <param name="failAfter">
		/// Date till which the Hack is valid. If current DateTime is greater than failAfter
		/// date then failure is reported
		/// </param>
		/// <param name="log">
		///  If this is not null, comments and details are printed to the log.
		/// </param>	
		public ScenarioResult(string hackComments, DateTime failAfter, Log log) : this(IsHackValid(failAfter), GetHackComments(hackComments, failAfter), log)
		{
			if (IsHackValid(failAfter)) 
				log.WriteLine(GetHackComments(hackComments, failAfter)); // IncCounters won't write out the comment if it passes
		}
        public ScenarioResult(string hackComments, Version failAfterVersion, Log log)
            : this(IsHackValid(failAfterVersion), GetHackComments(hackComments, failAfterVersion), log)
        {
            if (IsHackValid(failAfterVersion))
            { log.WriteLine(GetHackComments(hackComments, failAfterVersion)); }
        }

		/// <summary>
		/// Constructs a new Scenario Result. If the Hack has expired it is considered a Fail else it 
		/// it considered Pass. Appropriate comments are added to the log file. In case of Failure the known
		/// 





















		public ScenarioResult(Log log, BugDb db, int bugId, string bugComment, string hackComments, DateTime failAfter) : this(IsHackValid(failAfter), GetHackComments(db, bugId, bugComment, hackComments, failAfter), log)
		{
			if (log != null)
			{
				if(IsHackValid(failAfter))
					log.WriteLine(GetHackComments(db, bugId, bugComment, hackComments, failAfter)); // IncCounters won't write out the comment if it passes
				else 
					log.LogKnownBug(db, bugId, bugComment);
			}
		}

        public ScenarioResult(Log log, BugDb db, int bugId, string bugComment, string hackComments, Version failAfterVersion)
            : this(IsHackValid(failAfterVersion), GetHackComments(db, bugId, bugComment, hackComments, failAfterVersion), log)
        {
            if (log != null)
            {
                if (IsHackValid(failAfterVersion))
                    log.WriteLine(GetHackComments(db, bugId, bugComment, hackComments, failAfterVersion)); // IncCounters won't write out the comment if it passes
                else
                    log.LogKnownBug(db, bugId, bugComment);
            }
        }

		/// <summary>
		/// Determines if the Hack is Valid
		/// </summary>
		/// <param name="failAfter">
		/// Date till which the Hack is valid. If current DateTime is greater than failAfter
		/// date then failure is reported
		/// </param>
		/// <returns>Boolean specifying if the hack is valid</returns>
		private static bool IsHackValid(DateTime failAfter)
		{
			if (failAfter > DateTime.Now)
				return true;
			else
				return false;
		}
        private static bool IsHackValid(Version failAfterVersion)
        {
            if (0 <= CurrentVersion.CompareTo(failAfterVersion))
            { return true; }
            else
            {
                //Override the default version failure state for Orcas
                // As per request from BrandonB
                //Because current plan is for Orcas to be a service-pack level release
                //for the runtime, Whidbey postponed bugs should _not_ fail Orcas tests by default
                if (failAfterVersion == WhidbeyVersion && CurrentVersion == OrcasVersion)
                { return true; }
                else
                { return false; }
            }
        }

		/// <summary>
		/// Gets comments for the hack along with the information on the expiration
		/// of the hack.
		/// </summary>
		/// <param name="hackComments">Hack comments</param>
		/// <param name="failAfter">
		/// Date till which the Hack is valid. If current DateTime is greater than failAfter
		/// date then failure is reported
		/// </param>
		/// <returns>Hack comments</returns>
		private static string GetHackComments(string hackComments, DateTime failAfter)
		{
			if (failAfter > DateTime.Now)
				return "Hack: " + hackComments + ". Hack expires on: " + failAfter.ToShortDateString();
			else
				return "Hack: " + hackComments + ". Hack expired on: " + failAfter.ToShortDateString();
		}
        private static string GetHackComments(string hackComments, Version failAfterVersion)
        {
            if (IsHackValid(failAfterVersion))
            { return string.Format("Hack will after version: {0}", failAfterVersion);  }
            else
            { return string.Format("Hack has expired after version: {0}", failAfterVersion); }
        }

		/// <summary>
		/// Gets comments for the hack along with the information on the expiration
		/// of the hack and the 



















		private static string GetHackComments(BugDb db, int bugId, string bugComment, string hackComments, DateTime failAfter)
		{
			if (failAfter > DateTime.Now)
				return "Hack: " + hackComments + ". Hack expires on: " + failAfter.ToShortDateString() + ". " + MakeBugComment(db, bugId, bugComment);
			else
				return "Hack: " + hackComments + ". Expired on: " + failAfter.ToShortDateString() + ". " + MakeBugComment(db, bugId, bugComment);
		}


        private static string GetHackComments(BugDb db, int bugId, string bugComment, string hackComments, Version failAfterVersion)
        {
            if (IsHackValid(failAfterVersion))
            { return string.Format("Hack: {0}. Hack expires on version : {1}. {2}", 
                hackComments, failAfterVersion, MakeBugComment(db, bugId, bugComment)); }
            else
            { return string.Format("Hack: {0}. Hack expired on version : {1}. {2}", 
                hackComments, failAfterVersion, MakeBugComment(db, bugId, bugComment)); }
        }            

        // <doc>
        // <desc>
        //  A ScenarioResult indicating success.
        // </desc>
        // </doc>
        public static ScenarioResult Pass {
            get {
                return new ScenarioResult(true);
            }
        }

        // <doc>
        // <desc>
        //  A ScenarioResult indicating failure.
        // </desc>
        // </doc>
        public static ScenarioResult Fail {
            get {
                return new ScenarioResult(false);
            }
        }

		/// <summary>
		/// Simply tells you if this result is passing or not.
		/// </summary>
		/// <value>True if this scenario is passing, false if it's failing.</value>
		public bool IsPassing {
			get { return _type; }
		}

        // <doc>
        // <desc>
        //  The comments associated with this Scenario. Comments are usually
        //  ignored unless the _type of the ScenarioResult is FailType.
        //  This property will be null if no comments have been set.
        // </desc>
        // </doc>
        public virtual String Comments {
            get {
                return _comments;
            }
            set {
                this._comments = value;
            }
        }
     
        // <doc>
        // <desc>
        //  The total number of sub-scenarios that pass.
        // </desc>
        // <seealso member="FailCount"/>
        // <seealso member="TotalCount"/>
        // <seealso member="IncCounters"/>
        // </doc>
        public virtual int PassCount {
            get {
                return _pass;
            }
            set {
                _pass = value;
                _type = (FailCount == 0);
            }
        }

        // <doc>
        // <desc>
        //  The total number of sub-scenarios that fail.
        // </desc>
        // <seealso member="PassCount"/>
        // <seealso member="TotalCount"/>
        // <seealso member="IncCounters"/>
        // </doc>
        public virtual int FailCount {
            get {
                return _fail;
            }
            set {
                _fail = value;
                _type = (FailCount == 0);
            }
        }

        // <doc>
        // <desc>
        //  The total number of sub-scenarios.
        // </desc>
        // <seealso member="PassCount"/>
        // <seealso member="FailCount"/>
        // <seealso member="IncCounters"/>
        // </doc>
        public virtual int TotalCount {
            get {
                return _pass + _fail;
            }
        }
 
        // <doc>
        // <desc>
        //  Increments the pass/fail counters.
        // </desc>
        // <param term="b">
        //  If this parameter is true, the pass counter is incremented;
        //  otherwise the fail counter is incremented.
        // </param>
        // <seealso member="PassCount"/>
        // <seealso member="FailCount"/>
        // <seealso member="TotalCount"/>
        // </doc>
        public void IncCounters (bool b) {
            if (!b)
            { LogLineNumber(); }
            if ( b )
                ++this._pass;
            else
                ++this._fail;
            _type = (FailCount == 0);
        }

        // <doc>
        // <desc>
        //  Increments the pass/fail counters.
        // </desc>
        // <param term="b">
        //  If this parameter is true, the pass counter is incremented;
        //  otherwise the fail counter is incremented.
        // </param>
        // <param term="comments">
        //  If "b" is false, the Comments property is set to this value.
        // </param>
        // <seealso member="PassCount"/>
        // <seealso member="FailCount"/>
        // <seealso member="TotalCount"/>
        // </doc>
        [Obsolete("IncCounters(bool, string) is obsolete.  Please use IncCounters(bool, string, Log)")]
        public void IncCounters(bool b, string comments) {
            InternalIncCounters(b, comments);
        }

        // Need this to get rid of compiler warnings.
        private void InternalIncCounters(bool b, string comments) {
            IncCounters(b);

			if (!b)
			{
				if (Comments == null || Comments.Trim() == String.Empty || Comments.Trim() == "")
					Comments = comments;
				else
					Comments += "\r\n" + comments;
			}
		}

        // <doc>
        // <desc>
        //  Increments the pass/fail counters.  If this increments the fail
        //  counter, the comments will be printed to the given Log object.
        // </desc>
        // <param term="b">
        //  If this parameter is true, the pass counter is incremented;
        //  otherwise the fail counter is incremented.
        // </param>
        // <param term="comments">
        //  If "b" is false, the Comments property is set to this value.
        // </param>
        // <param term="log">
        //  If "b" is false, the Comments property is printed to this object.
        // </param>
        // <seealso member="PassCount"/>
        // <seealso member="FailCount"/>
        // <seealso member="TotalCount"/>
        // </doc>
        public void IncCounters(bool b, string comments, Log log) {
            InternalIncCounters(b, comments);
			if (!b && log != null)
				log.WriteLine(comments);
        }


		/// <summary>
		///  Increments the pass/fail counters.  If this increments the fail
		///  counter, the comments will be printed to the given Log object.
		/// </summary>
		/// <param name="b">
		///  If this parameter is true, the pass counter is incremented;
		///  otherwise the fail counter is incremented.
		/// </param>
		/// <param name="comments">
		///  If "b" is false, the Comments property is set to this value and printed to the
		///  given log.
		/// </param>
		/// <param name="details">If b is false, this gets printed to the log.</param>
		/// <param name="log">
		///  If this is not null, comments and details are printed to the log.
		/// </param>
		public void IncCounters(bool b, string comments, string details, Log log) {
			IncCounters(b, comments, log);	// prints out the comments to the log

			if (!b && log != null)
				log.WriteLine(details);
		}

		/// <summary>
		///  Increments the pass/fail counters.  If this increments the fail counter,
		///  the comments and actual/expected values will be printed to the given Log object.
		/// </summary>
		/// <param name="b">
		///  If this parameter is true, the pass counter is incremented;
		///  otherwise the fail counter is incremented.
		/// </param>
		/// <param name="comments">
		///  If "b" is false, the Comments property is set to this value and printed to the
		///  given log.
		/// </param>
		/// <param name="expected">The expected value.  Printed to the log if b is false.</param>
		/// <param name="actual">The actual value.  Printed to the log if b is false.</param>
		/// <param name="log">
		///  If this is not null, comments and actual/expected values are printed to the log.
		/// </param>
		public void IncCounters(bool b, string comments, object expected, object actual, Log log) {
			IncCounters(b, comments, log);	// prints out the comments to the log

			if (!b && log != null)
			{
				//log.WriteLine("Expected: {0}; Actual: {1}", expected, actual);
				log.LogExpectedActual(expected, actual);
			}
		}

		/// <summary>
		///  Increments the pass counter if expected.Equals(actual), else increments the fail
		///  counter.  If this increments the fail counter, the comments and actual/expected
		///  values will be printed to the given Log object.
		/// </summary>
		/// <param name="expected">The expected value.  Printed to the log if b is false.</param>
		/// <param name="actual">The actual value.  Printed to the log if b is false.</param>
		/// <param name="log">
		///  If this is not null, actual/expected values are printed to the log.
		/// </param>
		public void IncCounters(object expected, object actual, Log log) {
			IncCounters(expected, actual, "FAIL: Expected didn't match actual value", log);
		}

		/// <summary>
		///  Increments the pass counter if expected.Equals(actual), else increments the fail
		///  counter.  If this increments the fail counter, the comments and actual/expected
		///  values will be printed to the given Log object.
		/// </summary>
		/// <param name="expected">The expected value.  Printed to the log if b is false.</param>
		/// <param name="actual">The actual value.  Printed to the log if b is false.</param>
		/// <param name="comments">
		///  If !expected.Equals(actual), the Comments property is set to this value and printed to the log.
		///  Should NOT contain any random values, or auto-analysis tools will fail.
		/// </param>
		/// <param name="log">
		///  If this is not null, actual/expected values are printed to the log.
		/// </param>
		public void IncCounters(object expected, object actual, string comment, Log log) {
			bool passed = TestValues(expected, actual);
			IncCounters(passed, comment, log);	// prints out the comments

			if (!passed && log != null)
			{
				//log.WriteLine("Expected: {0}; Actual: {1}", expected, actual);
				log.LogExpectedActual(expected, actual);
			}
		}

        //
        // This version of IncCounters allows you to merge two ScenarioResults.  This
        // instance will increment its pass and fail counts according to the merged
        // instance, and will use the merged ScenarioResult's comment if there was a
        // failure.
        //
        public void IncCounters(ScenarioResult result) {
            IncCounters(result, null);
        }

        public void IncCounters(ScenarioResult result, Log log) {
            if ( result == null )
                throw new ArgumentNullException("result can't be null");

            // PassCount only gets incremented when IncCounters is used, so we need
            // account for the situation when result is basically a "new ScenarioResult(true)"
            if ( result.TotalCount == 0 && result._type )
                PassCount++;
            else
                PassCount += result.PassCount;

            if ( !result._type ) {                  // result was a fail
                Comments = result.Comments;

                // FailCount only gets incremented when IncCounters is used, so we need
                // account for the situation when result is basically a "new ScenarioResult(false)"
                if ( result.TotalCount == 0 )
                    FailCount++;
                else
                    FailCount += result.FailCount;

                if ( log != null )
                    log.WriteLine(Comments);
            }
        }

        public void IncCounters(bool b, Log log, BugDb db, int bugId, string bugComment) {
            InternalIncCounters(b, MakeBugComment(db, bugId, bugComment));

            if ( !b && log != null )
                log.LogKnownBug(db, bugId, bugComment);
        }

		public void IncCounters(bool b, Log log, BugDb db, int bugId)
		{
			IncCounters(b, log, db, bugId, null);
		}

		/// <summary>
		///  Reports failure if hack has expired. Increments the pass/fail counters.  
		/// If this increments the fail counter, the comments will be printed to 
		/// the given Log object.
		/// </summary>
		/// <param name="hackComments">
		///  If failure, then Comments property is set to this value and printed to the
		///  given log.
		/// </param>
		/// <param name="failAfter">
		/// Date till which the Hack is valid. If current DateTime is greater than failAfter
		/// date then failure is reported
		/// </param>
		/// <param name="log">
		///  If this is not null, comments and details are printed to the log.
		/// </param>		
		public void IncCounters(string hackComments, DateTime failAfter, Log log)
		{
			string comments = GetHackComments(hackComments, failAfter);

			if (IsHackValid(failAfter))
			{
				IncCounters(true, comments, log);
				if(log != null)
					log.WriteLine(comments); // IncCounters won't write out the comment if it passes
			}
			else
				IncCounters(false, comments, log);
		}
        public void IncCounters(string hackComments, Version failAfter, Log log)
        {
            string comments = GetHackComments(hackComments, failAfter);

            if (IsHackValid(failAfter))
            {
                IncCounters(true, comments, log);
                if (log != null)
                    log.WriteLine(comments); // IncCounters won't write out the comment if it passes
            }
            else
                IncCounters(false, comments, log);
        }

		/// <summary>
		///  Reports failure if hack has expired. Increments the pass/fail counters.  
		/// If this increments the fail counter, the comments will be printed to 
		/// the given Log object.
		/// </summary>
		/// <param name="log">
		///  If this is not null, comments and details are printed to the log.
		/// </param>
		/// <param name="db">
		/// 















		public void IncCounters(Log log, BugDb db, int bugId, string bugComment, string hackComments, DateTime failAfterVersion)
		{
			string comments = GetHackComments(db, bugId, bugComment, hackComments, failAfterVersion);

			if (IsHackValid(failAfterVersion))
			{
				IncCounters(true, comments, log);
				if (log != null)
				{
					log.WriteLine(comments); // IncCounters won't write out the comment if it passes
					//log.LogKnownBug(db, bugId, bugComment);
				}
			}
			else
			{
				IncCounters(false, comments, log);
				if (log != null)
					log.LogKnownBug(db, bugId, bugComment);
			}
		}
        public void IncCounters(Log log, BugDb db, int bugId, string bugComment, string hackComments, Version failAfterVersion)
        {
            string comments = GetHackComments(db, bugId, bugComment, hackComments, failAfterVersion);

            if (IsHackValid(failAfterVersion))
            {
                IncCounters(true, comments, log);
                if (log != null)
                {
                    log.WriteLine(comments); // IncCounters won't write out the comment if it passes
                    //log.LogKnownBug(db, bugId, bugComment);
                }
            }
            else
            {
                IncCounters(false, comments, log);
                if (log != null)
                    log.LogKnownBug(db, bugId, bugComment);
            }
        }
        private static string MakeBugComment(BugDb db, int bugId, string bugComment) {
            return db + " Bug #" + bugId + (bugComment == null ? "" : ": " + bugComment);
        }

		//
		// Returns true if expected.Equals(actual).  Checks for null values.
		//
		private static bool TestValues(object expected, object actual) {
			if ( expected == null || actual == null ) {
				if ( expected == actual )
					return true;
				else
					return false;
			}

			return expected.Equals(actual);
		}

        // <doc>
        // <desc>
        //  Wraps the overloaded equals operator.  Returns true if obj is of type
        //  ScenarioResult and it equals this ScenarioResult.
        // </desc>
        // <param term="obj">
        //  The ScenarioResult to compare with.
        // </param>
        // </doc>
        public override bool Equals(object obj) {
            if ( obj is ScenarioResult )
                return this == (ScenarioResult)obj;     // use overloaded ==

            return false;
        }

        // <doc>
        // <desc>
        //  Compares two ScenarioResult objects and returns true if their
        //  types are the same.
        // </desc>
        // <param term="a">
        //  The ScenarioResult on the left side of the "==" or "!=" operator.
        // </param>
        // <param term="b">
        //  The ScenarioResult on the right side of the "==" or "!=" operator.
        // </param>
        // </doc>
        public static bool operator ==(ScenarioResult a, ScenarioResult b) {
            if ( (object)a == null && (object)b == null )
                return true;

            // If we're here, one of the two is not null.
            if ( (object)a == null || (object)b == null )
                return false;
                
            return a._type == b._type;
        }
 
        // <doc>
        // <desc>
        //  Compares two ScenarioResult objects and returns true if their
        //  types are not the same.
        // </desc>
        // <param term="a">
        //  The ScenarioResult on the left side of the "==" or "!=" operator.
        // </param>
        // <param term="b">
        //  The ScenarioResult on the right side of the "==" or "!=" operator.
        // </param>
        // </doc>
        public static bool operator !=(ScenarioResult a, ScenarioResult b) {
            return !(a == b);
        }
 
        // <doc>
        // <desc>
        //  Returns a hash code.  ScenarioResult does not support hash codes, but
        //  does override ==, so must implement this method or generate compiler
        //  warnings.  There is no guarantee that hashing on ScenarioResults will
        //  produce valid output
        // </desc>
        // </doc>
        public override int GetHashCode() {
            return base.GetHashCode();
        }

        // <doc>
        // <desc>
        //  Returns the String value of this ScenarioResult.
        //  Examples:
        //      "type="Pass""
        //      "type="Fail""
        //      "type="Pass" total="15" fail="0""
        //      "type="Fail" total="15" fail="7""
        // </desc>
        // </doc>
        public override String ToString() {
            String result = Handy.NameValuePair("type", _type ? "Pass" : "Fail");

            if (TotalCount != 0) {
                String total = Handy.NameValuePair("total", TotalCount.ToString());
                String failed = Handy.NameValuePair("fail", FailCount.ToString());

                result += " " + total + " " + failed;
            }

            return result;
        }

        internal LogAttribute[] GetResultLogAttributes() {
            LogAttribute type = new LogAttribute("type", _type ? "Pass" : "Fail");
            if ( TotalCount == 0 )
                return new LogAttribute[] { type };
            else {
                return new LogAttribute[] {
                    type,
                    new LogAttribute("total", TotalCount.ToString()),
                    new LogAttribute("fail", FailCount.ToString())
                };
            }
        }
    }
}
