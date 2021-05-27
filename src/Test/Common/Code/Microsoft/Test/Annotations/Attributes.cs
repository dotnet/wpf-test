// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//---------------------------------------------------------------------
//
//  Description: TestSuite Attribute Definitions
//  Creator: Derek Mehlhorn (derekme)
//  Date Created: 1/20/06
//---------------------------------------------------------------------

using System;
using System.Windows;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Annotations.Test.Framework
{
    /// <summary>
    /// Keyword of TestCase
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class Keywords : Attribute
    {
        /// <summary>
        /// Set priority
        /// </summary>
        /// <param name="keyword"></param>
        public Keywords(string keywords)
        {
            Value = keywords;
        }

        /// <summary>
        /// </summary>
        public string Value;
    }

    /// <summary>
    /// Priority of TestCase
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class Priority : Attribute
    {
        /// <summary>
        /// Set priority
        /// </summary>
        /// <param name="priority"></param>
        public Priority(int priority)
        {
            Value = priority;
        }

        /// <summary>
        /// </summary>
        public int Value;
    }

    /// <summary>
    /// Determines if case should be in single execution group.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=true)]
    public class ExecutionGroupCompatible : Attribute
    {
        /// <summary>
        /// If false, eclude from execution groups.  
        /// </summary>
        /// <param name="isExecutionGroupCompatible"></param>
        public ExecutionGroupCompatible(bool isExecutionGroupCompatible)
        {
            IsExecutionGroupCompatible = isExecutionGroupCompatible;
        }

        /// <summary>
        /// </summary>
        public bool IsExecutionGroupCompatible;
    }

    /// <summary>
    /// Used to associate a method with a given Test Id.
    /// </summary>
    /// <remarks>
    /// This attribute is option, if it is not included then by default the
    /// method name will be used as the Test Id.  If a method is not a TestCase
    /// then use TestCase_Helper attribute.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class TestId : Attribute
    {      
        /// <summary>
        /// Override default test id.
        /// </summary>
        /// <param name="id"></param>
        public TestId(string id)
        {
            Id = id;
        }
        
        /// <summary>
        /// Id of test.
        /// </summary>
        public string Id;
    }

    /// <summary>
    /// Marks a TestCase as disabled.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DisabledTestCase : Attribute
    {
        /// <summary/>
        public DisabledTestCase()
            : this(string.Empty)
        {
            
        }

        /// <summary>
        /// Disable with comment.
        /// </summary>
        /// <param name="comment"></param>
        public DisabledTestCase(string comment)
        {
            Comment = comment;
        }

        /// <summary>
        /// Comment.
        /// </summary>
        public string Comment;
    }

    /// <summary>
    /// Used to mark that a test case has an active 

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true)]
    public class BugId : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bugId"></param>
        public BugId(int bugId)
            : this (bugId, string.Empty)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bugId"></param>
        /// <param name="description"></param>
        public BugId(int bugId, string description) 
        {
            Id = bugId;
            Description = description;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string  ToString()
        {
 	         return "Bug#" + Id + ": " + Description;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Id;
        /// <summary>
        /// 
        /// </summary>
        public string Description;
    }

    /// <summary>
    /// Used to mark that a test case has an active work item associated with it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class WorkItem : BugId
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public WorkItem(int id)
            : base(id, string.Empty)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        public WorkItem(int id, string description)
            : base(id, description)
        {
            // nothing.
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "WorkItem#" + Id + ": " + Description;
        }
    }

    /// <summary>
    /// Base class for attributes which associate a method with a test
    /// of a specific Id.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class TestCase_SupportMethod : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public TestCase_SupportMethod() 
            : this(DEFAULT)
        {
            // nothing.
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="regex"></param>
        public TestCase_SupportMethod(string regex)
        {
            CaseRegex = new Regex(regex);
        }
        /// <summary>
        /// 
        /// </summary>
        public Regex CaseRegex;
        /// <summary>
        /// 
        /// </summary>
        public static string DEFAULT = ".*";
    }

    /// <summary>
    /// Maps a method to be performed as the Setup operation for a set 
    /// of Test Ids, mapping is performed using a RegularExpression against
    /// Test Ids.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TestCase_Setup : TestCase_SupportMethod
    {
        /// <summary>
        /// Match all tests.
        /// </summary>
        public TestCase_Setup() 
            : base()
        {
            // nothing.
        }
        /// <summary>
        /// Declare setup for matching testids using regular expression.
        /// </summary>
        /// <param name="regex"></param>
        public TestCase_Setup(string regex)
            : base(regex)
        {
           // nothing.
        }
    }

    /// <summary>
    /// Maps a method to be performed as the Cleanup operation for a set 
    /// of Test Ids, mapping is performed using a RegularExpression against
    /// Test Ids.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TestCase_Cleanup : TestCase_SupportMethod
    {
        /// <summary>
        /// Match all tests.
        /// </summary>
        public TestCase_Cleanup() 
            : base()
        {
            // nothing.
        }
        /// <summary>
        /// Declare setup for matching testids using regular expression.
        /// </summary>
        /// <param name="regex"></param>
        public TestCase_Cleanup(string regex)
            : base(regex)
        {
           // nothing.
        }
    }

    /// <summary>
    /// Indicates that method is not a TestCase.
    /// </summary>
    /// <remarks>
    /// All methods declared in the currently executing TestSuite subclass
    /// is assumed to be a TestCase.  If class contains non-test cases methods
    /// they must be marked with this attribute otherwise they will be run
    /// as part of TestSuite.RunAll().
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class TestCase_Helper : Attribute
    {
        /// <summary/>
        public TestCase_Helper()
        {
            // nothing.
        }
    }

    /// <summary>
    /// Defines a single dimension for a TestCase.  Can be set on an individual Method controlling the
    /// dimensions of a specific test case, or defined on a class to define the dimensions for a whole
    /// suite of TestCases.  
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple=true,Inherited=true)]
    public class TestDimension : Attribute
    {
        /// <summary>
        /// Define values for a single test dimension.  This dimension will be in addition to
        /// any inherited dimensions.
        /// </summary>
        /// <param name="values">Comma delimited list of values for this dimension.</param>
        public TestDimension(string values)
        {
            Values = values;
        }
        /// <summary>
        /// Define a SINGLE dimension as a list of individual strings.
        /// </summary>
        public TestDimension(string[] values)
        {
            Values = string.Empty;
            for (int i=0; i < values.Length; i++)
            {
                Values += values[i];
                if (i != (values.Length - 1))
                    Values += ",";
            }
        }
        /// <summary>
        /// Values
        /// </summary>
        public string Values;
    }

    /// <summary>
    /// Used on a single TestCase to indicate that it shouldn't inherit TestDimensions that
    /// were defined on its enclosing class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class OverrideClassTestDimensions : Attribute
    {
        // Nothing.
    }

    //public enum VariationType
    //{
    //    Default,
    //    Major,
    //    Minor
    //}
}
