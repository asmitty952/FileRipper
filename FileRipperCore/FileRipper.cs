using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileRipperCore.Domain;
using FileRipperCore.Service;

namespace FileRipperCore
{
    public interface IFileRipper
    {
        public FileInstance Rip(FileStream fileStream, FileDefinition fileDefinition);

        public FileInstance<T> Rip<T>(FileStream fileStream, FileDefinition fileDefinition,
            Func<Dictionary<string, string>, T> objectBuilder);
    }

    public class FileRipper : IFileRipper
    {
        private readonly Func<FileDefinition, IFileService> _fileServiceFactory;

        public FileRipper() : this(IFileService.BuildFileService)
        {
        }

        internal FileRipper(Func<FileDefinition, IFileService> fileServiceFactory)
        {
            _fileServiceFactory = fileServiceFactory;
        }

        public FileInstance Rip(FileStream fileStream, FileDefinition fileDefinition)
        {
            var fileService = _fileServiceFactory(fileDefinition);
            return new FileInstance
            {
                FileName = fileStream.Name,
                FileRows = fileService.Process(fileStream)
            };
        }

        public FileInstance<T> Rip<T>(FileStream fileStream, FileDefinition fileDefinition,
            Func<Dictionary<string, string>, T> objectBuilder)
        {
            var fileService = _fileServiceFactory(fileDefinition);
            return new FileInstance<T>
            {
                FileName = fileStream.Name,
                FileRows = fileService.Process(fileStream).Select(fr => objectBuilder(fr.Fields)).ToList()
            };
        }
    }
}