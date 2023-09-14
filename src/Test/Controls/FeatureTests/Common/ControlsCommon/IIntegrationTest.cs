using Microsoft.Test.Logging;
using System;
using System.Xml;


namespace Avalon.Test.ComponentModel
{

    /// <summary>
    /// Interface for reusable integration tests.
    /// </summary>
    public interface IIntegrationTest
    {
		/// <summary>
		/// Perform the Integration Test upon the given framework element.
		/// </summary>
		/// At some point we will want to determine a best practice for parsing the XmlElement to make IIntegrationTests more modular.
		/// (For example, a nested IIntegrationTest is identified by a particular node name)
		/// <param name="testElement">Framework element to test.</param>
		/// <param name="variation">Optional description for specific testing behavior.</param>
		/// <returns>Result of the test.</returns>
        TestResult Perform(object testElement, XmlElement variation);
    }

}
