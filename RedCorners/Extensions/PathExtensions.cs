using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RedCorners
{
    public static class PathExtensions
    {
        public static string CreateDirectoryAndReturn(this string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            return path;
        }
    }
}
