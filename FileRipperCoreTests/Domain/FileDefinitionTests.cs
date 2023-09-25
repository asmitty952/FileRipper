using System.Collections.Generic;
using System.Linq;
using FileRipperCore;
using FileRipperCore.Domain;
using NUnit.Framework;
using static System.String;

namespace FileRipperTests.Domain
{
    [TestFixture]
    public class FileDefinitionTests
    {
        [Test]
        public void AddField_DelimitedField_AddsFieldToCollection()
        {
            var fieldDefinition = FieldDefinition.BuildDelimitedField("field-name", 2);
            var fileDefinition = FileDefinition.BuildDelimitedFile("|");

            fileDefinition.AddField(fieldDefinition);

            Assert.AreEqual(1, fileDefinition.FieldDefinitions.Count());
        }

        [Test]
        public void AddField_NullField_ThrowsFileRipperException()
        {
            var fileDefinition = FileDefinition.BuildDelimitedFile("|");

            Assert.Throws<FileRipperException>(() => fileDefinition.AddField(null));
        }

        [Test]
        public void AddField_WrongFieldType_ThrowsFileRipperException()
        {
            var fileDefinition = FileDefinition.BuildDelimitedFile(",");
            var fieldDefinition = FieldDefinition.BuildXmlField("field-name", "field-name");

            Assert.That(
                () => fileDefinition.AddField(fieldDefinition),
                Throws.TypeOf<FileRipperException>().With.Message.EqualTo("FileType for file and field must match")
            );
        }

        [Test]
        public void AddFields_ListWithTwoFields_AddsFieldsToList()
        {
            var fieldDefinitions = new List<FieldDefinition>
            {
                FieldDefinition.BuildXmlField("field-name-1"),
                FieldDefinition.BuildXmlField("field-name-2")
            };
            var fileDefinition = FileDefinition.BuildXmlFile("record");

            fileDefinition.AddFields(fieldDefinitions);

            Assert.AreEqual(2, fileDefinition.FieldDefinitions.Count());
        }

        [Test]
        public void AddFields_ListContainsFieldWithWrongType_ThrowsFileRipperException()
        {
            var fieldDefinitions = new List<FieldDefinition>
            {
                FieldDefinition.BuildXmlField("xml-field"),
                FieldDefinition.BuildDelimitedField("delimited-field", 2)
            };
            var fileDefinition = FileDefinition.BuildXmlFile("record");

            Assert.Throws<FileRipperException>(() => fileDefinition.AddFields(fieldDefinitions));
        }

        [Test]
        public void AddFields_ListContainsNullField_ThrowsFileRipperException()
        {
            var fieldDefinitions = new List<FieldDefinition>
            {
                FieldDefinition.BuildXmlField("xml-field"),
                null
            };
            var fileDefinition = FileDefinition.BuildXmlFile("record");

            Assert.Throws<FileRipperException>(() => fileDefinition.AddFields(fieldDefinitions));
        }

        [Test]
        public void BuildDelimitedFile_HasHeaderIsTrue_ReturnsCorrectlyBuiltFileDefinition()
        {
            var delimiter = "|";

            var fileDefinition = FileDefinition.BuildDelimitedFile(delimiter, true);

            Assert.AreEqual(FileTypes.Delimited, fileDefinition.FileType);
            Assert.AreEqual(delimiter, fileDefinition.Delimiter);
            Assert.IsTrue(fileDefinition.HasHeader);
            Assert.IsNull(fileDefinition.RecordElementName);
        }

        [Test]
        public void BuildDelimitedFile_HasHeaderIsNotProvided_ReturnsFileDefinitionWithHasHeaderFalse()
        {
            var fileDefinition = FileDefinition.BuildDelimitedFile(",");

            Assert.IsFalse(fileDefinition.HasHeader);
        }

        [Test]
        public void BuildDelimitedFile_DelimiterIsEmpty_ThrowsFileRipperException()
        {
            Assert.Throws<FileRipperException>(() => FileDefinition.BuildDelimitedFile(""));
        }

        [Test]
        public void BuildFixedFile_HasHeaderIsTrue_ReturnsCorrectlyBuiltFileDefinition()
        {
            var fileDefinition = FileDefinition.BuildFixedWidthFile(true);

            Assert.AreEqual(FileTypes.FixedWidth, fileDefinition.FileType);
            Assert.IsTrue(fileDefinition.HasHeader);
            Assert.IsNull(fileDefinition.Delimiter);
            Assert.IsNull(fileDefinition.RecordElementName);
        }

        [Test]
        public void BuildFixedWidthFile_HasHeaderIsNotProvided_ReturnsFileDefinitionWithHasHeaderFalse()
        {
            var fileDefinition = FileDefinition.BuildFixedWidthFile();

            Assert.IsFalse(fileDefinition.HasHeader);
        }

        [Test]
        public void BuildXmlFile_ValidInputs_ReturnsCorrectlyBuiltFileDefinition()
        {
            var recordElementName = "record-element-name";

            var fileDefinition = FileDefinition.BuildXmlFile(recordElementName);

            Assert.AreEqual(FileTypes.Xml, fileDefinition.FileType);
            Assert.AreEqual(recordElementName, fileDefinition.RecordElementName);
            Assert.IsNull(fileDefinition.Delimiter);
            Assert.IsNull(fileDefinition.HasHeader);
        }

        [Test]
        public void BuildXmlFile_RecordNameIsBlank_ThrowsFileRipperException()
        {
            Assert.Throws<FileRipperException>(() => FileDefinition.BuildXmlFile(Empty));
        }
    }
}