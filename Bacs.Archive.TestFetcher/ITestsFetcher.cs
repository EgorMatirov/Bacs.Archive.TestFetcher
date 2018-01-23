using System.Collections.Generic;
using Bacs.Archive.Client.CSharp;

namespace Bacs.Archive.TestFetcher
{
    public interface ITestsFetcher
    {
        IEnumerable<Test> FetchTests(IEnumerable<byte> problemArchive, string problemId, params string[] testsId);
        IEnumerable<Test> FetchTests(IArchiveClient archiveClient, string problemId, params string[] testId);
        IEnumerable<Test> FetchTests(IArchiveClient archiveClient, string problemId);
    }
}