using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Bacs.Archive.Client.CSharp;
using Bacs.Problem.Single;

namespace Bacs.Archive.TestFetcher
{
    public class TestsFetcher : ITestsFetcher
    {
        public IEnumerable<Test> FetchTests(IArchiveClient archiveClient, string problemId, params string[] testId)
        {
            var problemPackage = archiveClient.Download(SevenZipArchive.ZipFormat, problemId);
            return FetchTests(problemPackage, problemId, testId);
        }

        public IEnumerable<Test> FetchTests(IArchiveClient archiveClient, string problemId)
        {
            var importResultProblem = archiveClient.ImportResult(problemId)[problemId].Problem;

            var extensionValue = importResultProblem.Profile.First().Extension.Value;
            var testGroups = ProfileExtension.Parser.ParseFrom(extensionValue).TestGroup;
            var tests = testGroups
                .SelectMany(x => x.Tests.Query.Select(y => y.Id))
                .ToArray();

            return FetchTests(archiveClient, problemId, tests);
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