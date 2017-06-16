using NAME.Core;

namespace NAME.Tests
{
    public class DummyFilePathMapper : IFilePathMapper
    {
        public string MapPath(string filePath)
        {
            return filePath;
        }
    }
}