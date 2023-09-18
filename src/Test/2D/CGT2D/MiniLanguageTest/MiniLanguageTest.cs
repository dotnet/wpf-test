// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Text;
using Microsoft.Test.Graphics.TestTypes;

namespace Microsoft.Test.Graphics
{
    /// <summary/>
    public abstract class MiniLanguageTest : CoreGraphicsTest
    {
        /// <summary/>
        public override void Init( Variation v )
        {
            base.Init( v );

            Log( "This test covers {0} Mini language functionality.\n\n", ClassType );

            numIterations = GetTestParameter( "numIterations", 1000 );
            _maxNumSpaces = GetTestParameter( "maxNumSpaces", 40 );
            maxBounds = GetTestParameter( "maxBounds", 5000 );

            _logAllTests = GetTestParameter( "logAllTests", false );

            int randomSeed = GetTestParameter( "randomSeed", 0 );
            randomGenerator = new RandomTools( randomSeed );
        }

        /// <summary/>
        public override void RunTheTest()
        {
            Log( "\n\nChecking test mechanism." );
            CheckTest();

            if (priority == 0)
            {
                Log( "\n\nTesting valid input combinations." );
                RunParseTest( true );
            }
            else
            {
                Log( "\n\nTesting invalid combinations." );
                RunParseTest( false );
            }
        }

        /// <summary/>
        protected abstract void ParseTestCore();

        /// <summary/>
        protected virtual void RunParseTest( bool valid )
        {
            randomGenerator.KeepClean = valid;
            for (int i = 0; i < numIterations; i++)
            {
                try
                {
                    randomGenerator.ResetCleanFlag();
                    ParseTestCore();
                }
                catch (MiniLanguageTestTerminationException)
                {
                }
            }
        }

        /// <summary/>
        protected abstract void CheckTest();

        /// <summary/>
        protected int GetTestParameter( String name, int defaultVal )
        {
            int temp = defaultVal;
            if (variation[name] == null)
            {
                Log( "Test Parameter \"{0}\" defaulted to value: {1}", name, defaultVal );
            }
            else
            {
                if (int.TryParse(variation[name], out temp))
                {
                    Log("Test Parameter \"{0}\" set to: {1}", name, temp);
                }
            }
            return temp;
        }

        /// <summary>
        /// Returns negation of defaultVal if ANY value is specified.
        /// </summary>
        protected bool GetTestParameter( String name, bool defaultVal )
        {
            bool temp;
            if (variation[name] != null)
            {
                temp = !defaultVal;
                Log( "Test Parameter \"{0}\" set to: {1}", name, temp );
            }
            else
            {
                temp = defaultVal;
                Log( "Test Parameter \"{0}\" defaulted to value: {1}", name, defaultVal );
            }
            return temp;
        }

        /// <summary/>
        protected void LogProblemComparison( Object in1, Object in2, bool expectEqual )
        {
            AddFailure( "Unexpected Result on object comparison:" );
            LogParseString( serialValue );
            Log( "Expected: \n{0}", in1 );
            Log( "Actual: \n{0}", in2 );

            if (!expectEqual) //If this case is hit, something went wrong in the test.
            {
                Log( "An invalid input was successfully parsed and compared as equal!." );
            }
        }

        /// <summary/>
        protected void LogParseString( String val )
        {
            Log( "Using parse string:\n{0}", val );
        }

        /// <summary/>
        protected void LogObjects( Object source, Object duplicate )
        {
            if (_logAllTests)
            {
                Log( "Parsed Object to be compared:" );
                Log( "Source:\n{0}", source );
                if (duplicate != null)
                {
                    Log( "Duplicate:\n{0}", duplicate );
                }
                LogParseString( serialValue );
            }
        }

        /// <summary/>
        protected Object ConvertObject( String val, bool clean )
        {
            Object temp = null;

            if (_converter == null)
            {
                _converter = TypeDescriptor.GetConverter( ClassType );
            }

            try
            {
                if (!clean && _logAllTests)
                {
                    Log( "Preparing to parse invalid string:" );
                    LogParseString( val );
                }

                temp = _converter.ConvertFromString( val );

                if (!clean)
                {
                    AddFailure( "Invalid String was successfully parsed without warning." );
                    LogParseString( val );
                    throw new MiniLanguageTestTerminationException();
                }

            }
            catch (FormatException)
            {
                //Intercept exceptions involving inappropriate tokens while object parsing occurs
                if (clean)
                {
                    AddFailure( "Unexpected FormatException thrown on parse." );
                    LogParseString( val );
                }
                throw new MiniLanguageTestTerminationException();
            }
            catch (InvalidOperationException)
            {
                //Intercept exceptions arising from non-whitespace tokens after successfully parsing object
                if (clean)
                {
                    AddFailure( "Unexpected InvalidOperationException thrown on parse." );
                    LogParseString( val );
                }
                throw new MiniLanguageTestTerminationException();
            }

            if (temp == null)
            {
                AddFailure( "Conversion object is null." );
                throw new MiniLanguageTestTerminationException();
            }

            if (temp != null && !temp.GetType().Equals( ClassType ))
            {
                AddFailure( "Conversion object is not of Target Type!" );
                throw new MiniLanguageTestTerminationException();
            }

            return temp;
        }

        /// <summary>
        /// Internally defined exception class for prematurely terminating test iteration due to
        /// inability to continue test further.
        /// </summary>
        public class MiniLanguageTestTerminationException : Exception
        {
        }

        /// <summary/>
        protected abstract Type ClassType { get; }
        /// <summary/>
        protected RandomTools randomGenerator;
        /// <summary/>
        protected double maxBounds;
        /// <summary/>
        protected int numIterations;
        /// <summary/>
        protected String serialValue;
       
        private TypeConverter _converter;
        private bool _logAllTests;
        private int _maxNumSpaces;
    }
}

