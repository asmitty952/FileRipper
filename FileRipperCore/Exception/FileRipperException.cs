using System;

namespace FileRipperCore
{
    public class FileRipperException : Exception
    {
        public FileRipperException(string message) : base(message)
        {
        }
    }
}