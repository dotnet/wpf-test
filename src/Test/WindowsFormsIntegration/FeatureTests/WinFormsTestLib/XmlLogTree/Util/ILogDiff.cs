using System.IO;

namespace WFCTestLib.XmlLogTree.Util {
    interface ILogDiff {
        bool LogsMatch(Stream baseLog, Stream compareLog);
    }
}
