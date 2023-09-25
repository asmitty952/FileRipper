using System.Collections.Generic;

namespace FileRipperCore.Domain
{
    public class FileDefinition
    {
        public FileTypes? FileType { get; protected set; }
        public List<FieldDefinition> FieldDefinitions { get; } = new List<FieldDefinition>();
        public bool? HasHeader { get; protected set; }
        public string Delimiter { get; protected set; }

        public string RecordElementName { get; protected set; }
        // public string InputDirectory { get; protected set; }
        // public string CompletedDirectory { get; protected set; }
        // public string FileMask { get; protected set; }

        public void AddField(FieldDefinition fieldDefinition)
        {
            if (fieldDefinition == null)
            {
                throw new FileRipperException("A valid FieldDefinition object is required");
            }

            if (fieldDefinition.FileType != FileType)
            {
                throw new FileRipperException("FileType for file and field must match");
            }

            FieldDefinitions.Add(fieldDefinition);
        }

        private FileDefinition(FileTypes fileType)
        {
            FileType = fileType;
        }

        public void AddFields(IEnumerable<FieldDefinition> fieldDefinitions)
        {
            foreach (var fieldDefinition in fieldDefinitions)
            {
                AddField(fieldDefinition);
            }
        }

        public static FileDefinition BuildDelimitedFile(string delimiter, bool hasHeader = false)
        {
            if (string.IsNullOrEmpty(delimiter))
            {
                throw new FileRipperException("delimiter is required");
            }

            return new FileDefinition(FileTypes.Delimited)
            {
                Delimiter = delimiter,
                HasHeader = hasHeader
            };
        }

        public static FileDefinition BuildFixedWidthFile(bool hasHeader = false)
        {
            return new FileDefinition(FileTypes.FixedWidth)
            {
                HasHeader = hasHeader
            };
        }

        public static FileDefinition BuildXmlFile(string recordElementName)
        {
            if (string.IsNullOrWhiteSpace(recordElementName))
            {
                throw new FileRipperException("recordElementName is required");
            }
            
            return new FileDefinition(FileTypes.Xml)
            {
                RecordElementName = recordElementName
            };
        }
    }
}