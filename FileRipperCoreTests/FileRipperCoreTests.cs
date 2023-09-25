using System;
using System.Collections.Generic;
using System.IO;
using FileRipperCore;
using FileRipperCore.Domain;
using FileRipperCore.Service;
using NSubstitute;
using NUnit.Framework;

namespace FileRipperTests
{
    [TestFixture]
    public class FileRipperCoreTests
    {
        private IFileService _fileService;
        private FileRipper _fileRipper;

        [SetUp]
        public void SetUp()
        {
            _fileService = Substitute.For<IFileService>();
            _fileRipper = new FileRipper(BuildFileServiceStub(_fileService));
        }

        [Test]
        public void DefaultConstructor_ReturnsValidFileRipperObject()
        {
            Assert.DoesNotThrow(() => new FileRipper());
        }
        
        [Test]
        public void Rip_ValidInputs_ReturnsFileInstance()
        {
            using var fileStream = new FileStream("hello.txt", FileMode.Create, FileAccess.ReadWrite);
            var fileDefinition = FileDefinition.BuildDelimitedFile(",");
            var expectedRows = new List<FileRow>() {new FileRow()};

            _fileService.Process(fileStream).Returns(expectedRows);

            var fileInstance = _fileRipper.Rip(fileStream, fileDefinition);

            Assert.That(fileInstance.FileName.EndsWith("hello.txt"));
            Assert.AreEqual(expectedRows, fileInstance.FileRows);
            _fileService.Received().Process(fileStream);
        }
        
        [Test]
        public void Rip_ValidInputsAndObjectBuilder_ReturnsFileInstance()
        {
            using var fileStream = new FileStream("hello.txt", FileMode.Create, FileAccess.ReadWrite);
            var fileDefinition = FileDefinition.BuildDelimitedFile(",");
            var expectedRows = new List<Person>() {new Person()};

            _fileService.Process(fileStream).Returns(new List<FileRow>() {new FileRow()});

            var fileInstance = _fileRipper.Rip<Person>(fileStream, fileDefinition, GetPersonBuilder());

            Assert.That(fileInstance.FileName.EndsWith("hello.txt"));
            Assert.AreEqual(expectedRows, fileInstance.FileRows);
            _fileService.Received().Process(fileStream);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete("hello.txt");
        }

        private Func<FileDefinition, IFileService> BuildFileServiceStub(IFileService fileService) =>
            (fileDefinition) => fileService;

        private Func<Dictionary<string, string>, Person> GetPersonBuilder() => (fields) => new Person();
    }

    internal class Person
    {
        private string Name { get; }
        private string Age { get; }
        private string DateOfBirth { get; }

        internal Person()
        {
            Name = "Aaron";
            Age = "43";
            DateOfBirth = "09/04/1980";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Person)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Age, DateOfBirth);
        }
        
        protected bool Equals(Person other)
        {
            return Name == other.Name && Age == other.Age && DateOfBirth == other.DateOfBirth;
        }
    }
}
