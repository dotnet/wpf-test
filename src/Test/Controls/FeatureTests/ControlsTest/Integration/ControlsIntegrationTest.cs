using System;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// ControlsIntegrationTest
    /// </summary>
    public class ControlsIntegrationTest : StepsTest
    {
        public ControlsIntegrationTest(XmlElement variation)
        {
            this.variation = variation;
            if (variation.Attributes["Timeout"] != null)
            {
                timeout = Convert.ToDouble(variation.Attributes["Timeout"].Value) * 1000;
            }

            RunSteps += new TestStep(RunTest);
        }

        private XmlElement variation;
        // set 150 seconds timeout as default before we kill the WPF test app
        // because TestAttribute sets 180 seconds as default timeout
        private double timeout = 150000;

        private TestResult RunTest()
        {
            StringBuilder variationBuilder = new StringBuilder();
            variationBuilder.Append("Variation: ");
            foreach (XmlAttribute attribute in variation.Attributes)
            {
                variationBuilder.Append(attribute.Name);
                variationBuilder.Append("=");
                variationBuilder.Append(attribute.Value);
                variationBuilder.Append(" ");
            }

            LogComment(variationBuilder.ToString());

            string executableName = variation.Attributes["ExecutableName"].Value;
            XmlAttribute argsAttribute = variation.Attributes["Args"];
            string args = String.Empty;

            if (argsAttribute != null)
            {
                args = variation.Attributes["Args"].Value;
            }

            using (ControlsIntegrationValidator validator = new ControlsIntegrationValidator(executableName, args))
            {
                Validate(validator);

                if (!validator.TestResult)
                {
                    LogComment(validator.TestResultMessage);
                    return TestResult.Fail;
                }
            }

            return TestResult.Pass;
        }

        private void Validate(ControlsIntegrationValidator validator)
        {
            bool isTimeout = false;
            System.Timers.Timer timer = new System.Timers.Timer(timeout);
            timer.Elapsed += delegate(object sender, System.Timers.ElapsedEventArgs e)
            {
                LogComment("***TIMEOUT!");
                validator.Cleanup();
                isTimeout = true;
                timer.Stop();
            };
            timer.Start();

            // wait for the WPF test app to finish testing.
            while (true)
            {
                Thread.Sleep(10);
                if (validator.IsValidated || isTimeout)
                {
                    if (validator.IsValidated)
                    {
                        LogComment("***validator.IsValidated");
                    }

                    if (isTimeout)
                    {
                        LogComment("***isTimeout");
                    }

                    break;
                }
            }
        }
    }
}


