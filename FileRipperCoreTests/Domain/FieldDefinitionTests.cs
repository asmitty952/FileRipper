using FileRipperCore.Domain;
using NUnit.Framework;

namespace FileRipperTests.Domain
{
    [TestFixture]
    public class FieldDefinitionTests
    {
        private string _fieldName;

        [SetUp]
        public void Setup()
        {
            _fieldName = "field-name";
        }

        [Test]
        public void BuildDelimitedField_ValidInputs_ReturnsCorrectFieldDefinition()
        {
            var positionInRow = 2;

            var fieldDefinition = FieldDefinition.BuildDelimitedField(_fieldName, positionInRow);

            Assert.AreEqual(_fieldName, fieldDefinition.FieldName);
            Assert.AreEqual(positionInRow, fieldDefinition.PositionInRow);
            Assert.AreEqual(FileTypes.Delimited, fieldDefinition.FileType);
            Assert.IsNull(fieldDefinition.FieldLength);
            Assert.IsNull(fieldDefinition.StartPosition);
            Assert.IsNull(fieldDefinition.XmlNodeName);
        }

        [Test]
        public void BuildXmlField_ValidInputs_ReturnsCorrectFieldDefinition()
        {
            var xmlNodeName = "xml-field-name";

            var fieldDefinition = FieldDefinition.BuildXmlField(_fieldName, xmlNodeName);
            
            Assert.AreEqual(_fieldName, fieldDefinition.FieldName);
            Assert.AreEqual(xmlNodeName, fieldDefinition.XmlNodeName);
            Assert.AreEqual(FileTypes.Xml, fieldDefinition.FileType);
            Assert.IsNull(fieldDefinition.FieldLength);
            Assert.IsNull(fieldDefinition.StartPosition);
            Assert.IsNull(fieldDefinition.PositionInRow);
        }

        [Test]
        public void BuildXmlField_NamesAreTheSame_ReturnsCorrectFieldDefinition()
        {
            var fieldDefinition = FieldDefinition.BuildXmlField(_fieldName);
            
            Assert.AreEqual(fieldDefinition.FieldName, fieldDefinition.XmlNodeName);
        }

        [Test]
        public void BuildFixedWidthField_ValidInputs_ReturnsCorrectFieldDefinition()
        {
            var startPosition = 3;
            var fieldLength = 10;

            var fieldDefinition = FieldDefinition.BuildFixedWidthField(_fieldName, startPosition, fieldLength);
            
            Assert.AreEqual(_fieldName, fieldDefinition.FieldName);
            Assert.AreEqual(startPosition, fieldDefinition.StartPosition);
            Assert.AreEqual(fieldLength, fieldDefinition.FieldLength);
            Assert.AreEqual(FileTypes.FixedWidth, fieldDefinition.FileType);
            Assert.IsNull(fieldDefinition.XmlNodeName);
            Assert.IsNull(fieldDefinition.PositionInRow);
        }
    }
}