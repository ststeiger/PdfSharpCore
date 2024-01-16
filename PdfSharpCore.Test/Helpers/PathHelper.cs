using System.Collections.Generic;
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

        public string GetAssetPath(params string[] names)
        {
            var segments = new List<string> { RootDir, "Assets" };
            segments.AddRange(names);
            return Path.Combine(segments.ToArray());
        }

        public string RootDir { get; }

        public static PathHelper GetInstance()
        {
            return _instance ??= new PathHelper();
        }

        private static PathHelper _instance;
    }
}
