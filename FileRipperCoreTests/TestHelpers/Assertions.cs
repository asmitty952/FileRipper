using System.Collections;
using System.Collections.Generic;
using FileRipperCore.Domain;
using NUnit.Framework;

namespace FileRipperTests.TestHelpers
{
    public class Assertions
    {
        public static void AssertFileRows(IList<FileRow> fileRows)
        {
            var aaron = fileRows[0];
            Assert.AreEqual("Aaron", aaron.Fields["name"]);
            Assert.AreEqual("43", aaron.Fields["age"]);
            Assert.AreEqual("09/04/1980", aaron.Fields["dob"]);
            var heather = fileRows[1];
            Assert.AreEqual("Heather", heather.Fields["name"]);
            Assert.AreEqual("42", heather.Fields["age"]);
            Assert.AreEqual("12/25/1980", heather.Fields["dob"]);
            var xander = fileRows[2];
            Assert.AreEqual("Xander", xander.Fields["name"]);
            Assert.AreEqual("8", xander.Fields["age"]);
            Assert.AreEqual("11/22/2014", xander.Fields["dob"]);
            var ella = fileRows[3];
            Assert.AreEqual("Ella", ella.Fields["name"]);
            Assert.AreEqual("5", ella.Fields["age"]);
            Assert.AreEqual("02/07/2018", ella.Fields["dob"]);
        }
    }
}