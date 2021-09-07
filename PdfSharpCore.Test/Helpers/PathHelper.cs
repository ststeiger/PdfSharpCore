using System.IO;
using System.Reflection;

namespace PdfSharpCore.Test.Helpers
{
    public class PathHelper
    {
        public PathHelper()
        {
            RootDir = Path.GetDirectoryName(GetType().GetTypeInfo().Assembly.Location);
        }

        public string GetAssetPath(string name)
        {
            return Path.Combine(RootDir, "Assets", name);
        }

        public string RootDir { get; }

        public static PathHelper GetInstance()
        {
            return _instance ??= new PathHelper();
        }

        private static PathHelper _instance;
    }
}