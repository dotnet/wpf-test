// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;

namespace ReflectTools
{
    /**
     * This class encapsulates all the params that get passed around to
     * the various test methods methods.  This way, if we want to add more
     * params, we don't have to manually change multiple files.
     */
    public class TParams
    {
        /**
         * This is the target being tested.  It's filled in by the
         * primary test in a call to createObject(). It is only
         * used by AutoPME tests.
         */
        public Object target = null;

        /**
         * Random number class.  Initialized in the constructor below.
         */
        public RandomUtil ru = null;

        /**
         * This is the logging target, which can use javatest logging or not, depending on
         * whether the CmdOpt.LOGTOSYSTEMOUT flag is passed at the command line. The default
         * is to use javatest logging.
         */
        public Log log = null;

        /**
         * Constructs a TParams object. Initializes the RandomUtil object and stores the
         * Log object provided.
         *
         * @param log The Log object the test is logging to
         */
        public TParams(Log log)
        {
            this.log = log;
            this.ru = new RandomUtil();
        }
    }
}
