using System.Collections.Generic;

namespace FileRipperCore.Domain
{
    public class FileInstance
    {
        public string FileName { get; internal set; }

        public List<FileRow> FileRows { get; internal set; }
    }

    public class FileInstance<T>
    {
        public string FileName { get; internal set; }
        public List<T> FileRows { get; internal set; }
    }
}