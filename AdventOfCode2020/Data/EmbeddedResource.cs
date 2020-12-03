using System.IO;
using System.Reflection;

namespace AdventOfCode2020.Data
{
    public static class EmbeddedResource
    {
        public static string ReadInput(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{nameof(AdventOfCode2020)}.{nameof(Data)}.{fileName}";

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
