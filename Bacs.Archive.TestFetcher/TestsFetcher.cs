using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Bacs.Archive.Client.CSharp;

namespace Bacs.Archive.TestFetcher
{
    public class TestsFetcher : ITestsFetcher
    {
        public IEnumerable<Test> FetchTests(IArchiveClient archiveClient, string problemId, params string[] testId)
        {
            var problemPackage = archiveClient.Download(SevenZipArchive.ZipFormat, problemId);
            return FetchTests(problemPackage, problemId, testId);
        }
        
        public IEnumerable<Test> FetchTests(IEnumerable<byte> problemArchive, string problemId, params string[] testsId)
        {
            var stream = new MemoryStream(problemArchive.ToArray());
            var zipArchive = new ZipArchive(stream);
            return testsId.Select(x => new Test
            {
                Id = x,
                Input = ReadTest(zipArchive, problemId, x, "in"),
                Output = ReadTest(zipArchive, problemId, x, "out")
            });
        }

        private static string ReadTest(ZipArchive zipArchive, string problemId, string testId, string suffix)
        {
            var inputStream = zipArchive.Entries.Single(x => x.FullName == $"{problemId}/tests/{testId}.{suffix}").Open();
            var reader = new StreamReader(inputStream);
            return reader.ReadToEnd();
        }
    }
}