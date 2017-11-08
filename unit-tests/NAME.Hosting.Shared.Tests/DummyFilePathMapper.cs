using NAME.Core;

namespace NAME.Hosting.Shared.Tests
{
    public class DummyFilePathMapper : IFilePathMapper
    {
        public string MapPath(string filePath)
        {
            return filePath;
        }
    }
}