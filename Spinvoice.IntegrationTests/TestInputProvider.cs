using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Spinvoice.IntegrationTests
{
    public static class TestInputProvider
    {
        private const string testResourcesPath = @"C:\Projects\my\Spinvoice.TestResources\";

        private static string GetPath(string testCasePath, string fileName = "")
        {
            return $@"{testCasePath}\{fileName}";
        }

        public static IEnumerable<TestInput> GetInput(string testCasePath, string category)
        {
            var fileNames = Directory.GetFiles(testCasePath).Select(Path.GetFileName).ToArray();
            foreach (var pdf in fileNames.Where(s => s.StartsWith(category) && s.EndsWith(".pdf")))
            {
                var json = Path.ChangeExtension(pdf, ".json");
                yield return new TestInput(
                    GetPath(testCasePath, pdf),
                    GetPath(testCasePath, json));
            }
        }

        public static object[] GetTestData(string testClassName)
        {
            var data = Directory.GetDirectories(Path.Combine(testResourcesPath, testClassName))
                .Select(s => new object[] { s })
                .Cast<object>()
                .ToArray();
            return data;
        }

        public static string GetTestPath(string testClassName, string testName)
        {
            return Path.Combine(testResourcesPath, testClassName, testName);
        }
    }
}