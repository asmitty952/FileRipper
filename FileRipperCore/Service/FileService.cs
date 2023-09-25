using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using FileRipperCore.Domain;

namespace FileRipperCore.Service
{
    public interface IFileService
    {
        public List<FileRow> Process(FileStream fileStream);

        internal static IFileService BuildFileService(FileDefinition fileDefinition)
        {
            return fileDefinition.FileType switch
            {
                FileTypes.Delimited => new DelimitedFileService(fileDefinition),
                FileTypes.FixedWidth => new FixedWidthFileService(fileDefinition),
                FileTypes.Xml => new XmlFileService(fileDefinition),
                _ => throw new ArgumentException("Invalid file type provided")
            };
        }
    }

    internal abstract class FileService : IFileService
    {
        protected FileDefinition FileDefinition { get; }

        protected FileService(FileDefinition fileDefinition)
        {
            FileDefinition = fileDefinition;
        }

        public List<FileRow> Process(FileStream fileStream)
        {
            return ProcessFileRecords(fileStream);
        }

        internal abstract List<FileRow> ProcessFileRecords(FileStream fileStream);
    }

    internal abstract class FlatFileService : FileService
    {
        protected FlatFileService(FileDefinition fileDefinition) : base(fileDefinition)
        {
        }

        internal override List<FileRow> ProcessFileRecords(FileStream fileStream)
        {
            var hasHeader = FileDefinition.HasHeader.GetValueOrDefault(false);
            var records = File.ReadAllLines(fileStream.Name, Encoding.UTF8);

            if (hasHeader)
            {
                records = records[1..];
            }

            return records.Select(ProcessRecord).ToList();
        }

        protected abstract FileRow ProcessRecord(string record);
    }

    internal class DelimitedFileService : FlatFileService
    {
        internal DelimitedFileService(FileDefinition fileDefinition) : base(fileDefinition)
        {
        }

        protected override FileRow ProcessRecord(string record)
        {
            var fields = record.Split(FileDefinition.Delimiter);
            var fileRow = new FileRow();
            foreach (var fieldDef in FileDefinition.FieldDefinitions)
            {
                var fieldPosition = fieldDef.PositionInRow.GetValueOrDefault();
                fileRow.Fields.Add(fieldDef.FieldName, fields[fieldPosition]);
            }

            return fileRow;
        }
    }

    internal class FixedWidthFileService : FlatFileService
    {
        internal FixedWidthFileService(FileDefinition fileDefinition) : base(fileDefinition)
        {
        }

        protected override FileRow ProcessRecord(string record)
        {
            var fileRow = new FileRow();
            foreach (var fieldDefinition in FileDefinition.FieldDefinitions)
            {
                var fieldValue = record.Substring(
                    fieldDefinition.StartPosition.GetValueOrDefault(),
                    fieldDefinition.FieldLength.GetValueOrDefault()
                );
                fileRow.Fields.Add(fieldDefinition.FieldName, fieldValue.Trim());
            }

            return fileRow;
        }
    }

    internal class XmlFileService : FileService
    {
        internal XmlFileService(FileDefinition fileDefinition) : base(fileDefinition)
        {
        }

        internal override List<FileRow> ProcessFileRecords(FileStream fileStream)
        {
            var document = new XmlDocument();
            document.Load(fileStream);
            var fileRows = new List<FileRow>();

            var nodes = document.DocumentElement?.SelectNodes($"//*/{FileDefinition.RecordElementName}");
            if (nodes == null || nodes.Count == 0)
            {
                throw new FileRipperException("XML file does not contain any records");
            }

            foreach (XmlNode node in nodes)
            {
                var fileRow = new FileRow();
                foreach (var fieldDefinition in FileDefinition.FieldDefinitions)
                {
                    var childNode = node.SelectSingleNode(fieldDefinition.XmlNodeName);
                    fileRow.Fields.Add(fieldDefinition.FieldName, childNode?.InnerText);
                }

                fileRows.Add(fileRow);
            }

            return fileRows;
        }
    }
}