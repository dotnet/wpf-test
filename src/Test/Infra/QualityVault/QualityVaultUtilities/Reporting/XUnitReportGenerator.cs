// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

﻿using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using Microsoft.Test;
using System;
using System.Text;

namespace Microsoft.Test.Reporting
{

    internal static class XUnitReportGenerator
    {

        static XUnitReportGenerator()
        {
            HelixResultsContainerUri = System.Environment.GetEnvironmentVariable("HELIX_RESULTS_CONTAINER_URI");
            HelixResultsContainerRsas = System.Environment.GetEnvironmentVariable("HELIX_RESULTS_CONTAINER_RSAS");
        }

        internal static void Generate(TestRecords records, DirectoryInfo ReportRoot)
        {
            // See https://xunit.net/docs/format-xml-v2.html for the documentation on how to format xunit logs
            var root = new XElement("assemblies");

            // We run our tests by Area in Helix. The SummaryReportGenerator does a nice job of aggregating the TestRecords
            // by area already, so we'l reuse that. This ensures we aren't making too many assummptions (like that all records are in the same Area)
            // that could break later.
            var resultsByArea = SummaryReportGenerator.ProduceAreaSummaries(records);
            foreach (SummaryReportGenerator.AreaSummaryEntry areaEntry in resultsByArea.Values)
            {
                string areaName = string.IsNullOrEmpty(areaEntry.Name) ? "UnknownArea" : areaEntry.Name;
                var assembly = new XElement("assembly");
                assembly.SetAttributeValue("name", areaName);
                assembly.SetAttributeValue("test-framework", "QualityVault");
                assembly.SetAttributeValue("run-date", DateTime.Now.ToString("yyyy-MM-dd"));
                assembly.SetAttributeValue("run-time", areaEntry.StartTime.ToString(@"hh\:mm\:ss"));
                assembly.SetAttributeValue("time", (areaEntry.EndTime - areaEntry.StartTime).TotalSeconds);
                assembly.SetAttributeValue("total", areaEntry.TotalVariations);
                assembly.SetAttributeValue("passed", areaEntry.TotalVariations - areaEntry.FailedVariations - areaEntry.IgnoredVariations);
                assembly.SetAttributeValue("failed", areaEntry.FailedVariations);
                assembly.SetAttributeValue("skipped", areaEntry.IgnoredVariations);
                assembly.SetAttributeValue("errors", 0);
                root.Add(assembly);

                foreach (TestRecord testRecord in areaEntry.AssociatedTests)
                {
                    var collection = new XElement("collection");
                    int testPassedCount = testRecord.Variations.Where(variation => variation.Result == Result.Pass).Count();
                    int testFailedCount = testRecord.Variations.Where(variation => variation.Result == Result.Fail).Count();
                    int testSkippedCount = testRecord.Variations.Where(variation => variation.Result == Result.Ignore).Count();

                    collection.SetAttributeValue("total", testRecord.Variations.Count);
                    collection.SetAttributeValue("passed", testPassedCount);
                    collection.SetAttributeValue("failed", testFailedCount);
                    collection.SetAttributeValue("skipped", testSkippedCount);
                    collection.SetAttributeValue("name", testRecord.TestInfo.Name);
                    collection.SetAttributeValue("time", ReportingUtilities.FormatTimeSpanAsSeconds(ReportingUtilities.GetTestDuration(testRecord)));
                    assembly.Add(collection);

                    foreach (VariationRecord variation in testRecord.Variations)
                    {
                        var test = new XElement("test");
                        string testName = testRecord.TestInfo.Name;

                        // Azure DevOps treats tests that share type and method as iterations of one
                        // data-driven test, so each test needs its own type/method pair.
                        test.SetAttributeValue("type", areaName);
                        test.SetAttributeValue("method", testName);
                        test.SetAttributeValue("name", GetResultName(areaName, testName, variation.VariationName));
                        test.SetAttributeValue("time", GetResultDuration(testRecord, variation).TotalSeconds);
                        test.SetAttributeValue("result", variation.Result == Result.Ignore ? "Skip" : variation.Result.ToString());

                        XElement traits = CreateTraits(testRecord.TestInfo);
                        if (traits.HasElements)
                        {
                            test.Add(traits);
                        }

                        if (variation.Result == Result.Ignore)
                        {
                            test.Add(new XElement("reason", new XCData("Ignored by QualityVault.")));
                        }
                        else if (variation.Result != Result.Pass)
                        {
                            var failure = new XElement("failure");
                            failure.SetAttributeValue("exception-type", "Exception");
                            failure.Add(new XElement("message", new XCData(GetFailureReason(testRecord, variation))));
                            test.Add(failure);

                            test.Add(new XElement("output", new XCData(GetOutput(testRecord, variation))));
                        }
                        collection.Add(test);
                    }

                }
            }


            string xunitOutputPath = Path.Combine(ReportRoot.FullName, "testResults.xml");
            File.WriteAllText(xunitOutputPath, root.ToString());
        }

        private const int MaxFailureReasonLength = 1000;
        private const int MaxOutputLength = 256 * 1024;

        private static readonly string[] OutcomeMarkers = { "Non-zero exit code", "Driver Error", "timed out", "timeout" };
        private static readonly string[] DetailMarkers = { "Exception", "Assert", "fail", "error" };

        private static string GetResultName(string areaName, string testName, string variationName)
        {
            string name = areaName + "." + testName;
            if (!string.IsNullOrEmpty(variationName) && !string.Equals(variationName, testName, StringComparison.Ordinal))
            {
                name += " - " + variationName;
            }
            return name;
        }

        private static TimeSpan GetResultDuration(TestRecord testRecord, VariationRecord variation)
        {
            if (testRecord.Variations.Count > 1)
            {
                TimeSpan variationDuration = ReportingUtilities.GetVariationDuration(variation);
                if (variationDuration > TimeSpan.Zero)
                {
                    return variationDuration;
                }
            }
            return testRecord.Duration;
        }

        private static XElement CreateTraits(TestInfo testInfo)
        {
            var traits = new XElement("traits");
            AddTrait(traits, "area", testInfo.Area);
            AddTrait(traits, "subarea", testInfo.SubArea);
            AddTrait(traits, "owner", testInfo.DriverParameters?["owner"]);
            AddTrait(traits, "priority", testInfo.Priority?.ToString(System.Globalization.CultureInfo.InvariantCulture));
            return traits;
        }

        private static void AddTrait(XElement traits, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var trait = new XElement("trait");
                trait.SetAttributeValue("name", name);
                trait.SetAttributeValue("value", value.Trim());
                traits.Add(trait);
            }
        }

        /// <summary>
        /// Returns a short reason for the failure, for example the DRT's last error line and
        /// its exit code, so that the Tests tab shows why a test failed without opening its log.
        /// </summary>
        private static string GetFailureReason(TestRecord testRecord, VariationRecord variation)
        {
            string[] lines = SplitLines(variation.Log).Concat(SplitLines(testRecord.Log)).ToArray();
            string outcome = FindLastLine(lines, OutcomeMarkers, null);
            string detail = FindLastLine(lines, DetailMarkers, outcome);

            string reason;
            if (detail != null && outcome != null)
            {
                reason = detail + " (" + outcome + ")";
            }
            else
            {
                reason = detail ?? outcome ?? string.Format("{0} failed. See the console log for details.", testRecord.TestInfo.Name);
            }
            return Truncate(reason, MaxFailureReasonLength);
        }

        private static string FindLastLine(string[] lines, string[] markers, string exclude)
        {
            return lines.LastOrDefault(line =>
                !string.Equals(line, exclude, StringComparison.Ordinal) &&
                !line.StartsWith("Logged Result:", StringComparison.Ordinal) &&
                !line.StartsWith("Logged File:", StringComparison.Ordinal) &&
                markers.Any(marker => line.IndexOf(marker, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        private static string GetOutput(TestRecord testRecord, VariationRecord variation)
        {
            var output = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(testRecord.Log))
            {
                output.AppendLine("QualityVault log:");
                output.AppendLine(testRecord.Log.TrimEnd());
                output.AppendLine();
            }
            output.AppendLine("Test output:");
            output.AppendLine(string.IsNullOrWhiteSpace(variation.Log) ? "(none)" : variation.Log.TrimEnd());

            string text = output.ToString();
            if (text.Length > MaxOutputLength)
            {
                text = "(truncated; showing the last " + MaxOutputLength + " characters)" + Environment.NewLine +
                    text.Substring(text.Length - MaxOutputLength);
            }
            return text;
        }

        private static string[] SplitLines(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new string[0];
            }
            return text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => line.Length > 0)
                .ToArray();
        }

        private static string Truncate(string text, int maxLength)
        {
            return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
        }

        private static string GetUploadedFileUrl(string filePath)
        {
            var filename = Path.GetFileName(filePath);
            return string.Format("{0}/{1}{2}", HelixResultsContainerUri, filename, HelixResultsContainerRsas);
        }

        private static string HelixResultsContainerUri;
        private static string HelixResultsContainerRsas;

    }
}
