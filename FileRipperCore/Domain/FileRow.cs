using System.Collections.Generic;

namespace FileRipperCore.Domain
{
    public class FileRow
    {
        public Dictionary<string, string> Fields { get; } = new Dictionary<string, string>();
    }
}