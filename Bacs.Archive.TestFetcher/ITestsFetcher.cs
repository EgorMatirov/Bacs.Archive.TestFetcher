using System.Collections.Generic;

namespace Bacs.Archive.TestFetcher
{
    public interface ITestsFetcher
    {
        IEnumerable<Test> FetchTests(IEnumerable<byte> problemArchive, string problemId, params string[] testsId);
    }
}