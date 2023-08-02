using Microsoft.Test.Logging;
using System;
using System.Xml;


namespace Avalon.Test.ComponentModel
{

    /// <summary>
    /// Interface for reusable unit tests.
    /// </summary>
    public interface IUnitTest
    {
		/// <summary>
		/// Perform the Unit Test upon the given framework element.
		/// </summary>
		/// <param name="testElement">Framework element to test.</param>
		/// <param name="variation">Optional description for specific testing behavior.</param>
		/// <returns>Result of the test.</returns>
		TestResult Perform(object testElement, XmlElement variation);
    }

}
