using System;
using System.Collections.Generic;
using System.IO;
using FileRipperCore;
using FileRipperCore.Domain;
using FileRipperCore.Service;
using NUnit.Framework;
using static FileRipperCore.Domain.FieldDefinition;
using static FileRipperCore.Domain.FileDefinition;
using static FileRipperTests.TestHelpers.Assertions;
using static FileRipperCore.Service.IFileService;

namespace FileRipperTests.Service
{
    [TestFixture]
    public class FileServiceFactoryTests
    {
        [Test]
        public void BuildFileService_DelimitedFileDefinition_ReturnsDelimitedFileService()
        {
            var fileService = BuildFileService(BuildDelimitedFile("\t"));
            Assert.That(fileService is DelimitedFileService);
        }
        
        [Test]
        public void BuildFileService_FixedWidthFileDefinition_ReturnsFixedWidthFileService()
        {
            var fileService = BuildFileService(BuildFixedWidthFile());
            Assert.That(fileService is FixedWidthFileService);
        }
        
        [Test]
        public void BuildFileService_XmlFileDefinition_ReturnsXmlFileService()
        {
            var fileService = BuildFileService(BuildXmlFile("person"));
            Assert.That(fileService is XmlFileService);
        }
    }
    [TestFixture]
    public class DelimitedFileServiceTests
    {
        [TestCase("|")]
        [TestCase(",")]
        [TestCase("\t")]
        [TestCase(".")]
        public void Process_PipeDelimited_ReturnsFileRows(string delimiter)
        {
            BuildDelimitedFile(delimiter);
            var fieldDefinitions = new List<FieldDefinition>
            {
                BuildDelimitedField("name", 0),
                BuildDelimitedField("dob", 2),
                BuildDelimitedField("age", 1)
            };
            var fileDefinition = FileDefinition.BuildDelimitedFile(delimiter, true);
            fileDefinition.AddFields(fieldDefinitions);
            using var fileStream = new FileStream("people.txt", FileMode.Open, FileAccess.Read);

            var fileRows = new DelimitedFileService(fileDefinition).Process(fileStream);

            AssertFileRows(fileRows);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete("people.txt");
        }

        private void BuildDelimitedFile(string delimiter)
        {
            var records = new List<string>
            {
                $"name{delimiter}age{delimiter}dob",
                $"Aaron{delimiter}43{delimiter}09/04/1980",
                $"Heather{delimiter}42{delimiter}12/25/1980",
                $"Xander{delimiter}8{delimiter}11/22/2014",
                $"Ella{delimiter}5{delimiter}02/07/2018"
            };
            File.WriteAllLines("people.txt", records);
        }
    }

    [TestFixture]
    public class FixedWidthFileServiceTests
    {
        [Test]
        public void Process_FixedWidthFile_ReturnsFileRows()
        {
            BuildFixedWidthFile();
            var fieldDefinitions = new List<FieldDefinition>()
            {
                BuildFixedWidthField("age", 10, 3),
                BuildFixedWidthField("dob", 13, 10),
                BuildFixedWidthField("name", 0, 10)
            };
            var fileDefinition = FileDefinition.BuildFixedWidthFile();
            fileDefinition.AddFields(fieldDefinitions);
            using var fileStream = new FileStream("people.dat", FileMode.Open, FileAccess.Read);

            var fileRows = new FixedWidthFileService(fileDefinition).Process(fileStream);

            AssertFileRows(fileRows);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete("people.dat");
        }

        private void BuildFixedWidthFile()
        {
            var records = new List<string>()
            {
                "Aaron      4309/04/1980",
                "Heather    4212/25/1980",
                "Xander      811/22/2014",
                "Ella        502/07/2018",
            };
            File.WriteAllLines("people.dat", records);
        }
    }

    [TestFixture]
    public class XmlFileServiceTests
    {
        private FileDefinition _fileDefinition;

        [SetUp]
        public void SetUp()
        {
            var fieldDefinitions = new List<FieldDefinition>
            {
                BuildXmlField("name", "personName"),
                BuildXmlField("dob", "dateOfBirth"),
                BuildXmlField("age", "currentAge"),
            };
            _fileDefinition = FileDefinition.BuildXmlFile("person");
            _fileDefinition.AddFields(fieldDefinitions);
        }

        [Test]
        public void Process_XmlFile_ReturnsFileRows()
        {
            BuildXmlFile();
            using var fileStream = new FileStream("people.xml", FileMode.Open, FileAccess.Read);

            var fileRows = new XmlFileService(_fileDefinition).Process(fileStream);

            AssertFileRows(fileRows);
        }

        [Test]
        public void Process_FileMissingRecordNodes_ThrowsFileRipperException()
        {
            File.WriteAllText("people.xml", "<people></people>");
            using var fileStream = new FileStream("people.xml", FileMode.Open, FileAccess.Read);

            Assert.Throws<FileRipperException>(() => new XmlFileService(_fileDefinition).Process(fileStream));
        }

        private void BuildXmlFile()
        {
            var lines = new List<string>()
            {
                "<people>",
                "\t<person>",
                "\t\t<personName>Aaron</personName>",
                "\t\t<currentAge>43</currentAge>",
                "\t\t<dateOfBirth>09/04/1980</dateOfBirth>",
                "\t</person>",
                "\t<person>",
                "\t\t<personName>Heather</personName>",
                "\t\t<currentAge>42</currentAge>",
                "\t\t<dateOfBirth>12/25/1980</dateOfBirth>",
                "\t</person>",
                "\t<person>",
                "\t\t<personName>Xander</personName>",
                "\t\t<currentAge>8</currentAge>",
                "\t\t<dateOfBirth>11/22/2014</dateOfBirth>",
                "\t</person>",
                "\t<person>",
                "\t\t<personName>Ella</personName>",
                "\t\t<currentAge>5</currentAge>",
                "\t\t<dateOfBirth>02/07/2018</dateOfBirth>",
                "\t</person>",
                "</people>"
            };
            File.WriteAllLines("people.xml", lines);
        }
    }
}