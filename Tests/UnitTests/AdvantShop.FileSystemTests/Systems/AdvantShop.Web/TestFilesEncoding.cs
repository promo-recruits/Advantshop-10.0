using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AdvantShop.FileSystemTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace AdvantShop.FileSystemTests.Systems.AdvantShop.Web
{
    [TestFixture]
    public class TestFilesEncoding
    {
        [Test]
        public void Files_ShouldBeInUtf8([Values(".cs", ".csproj", ".config")] string fileExtension)
        {
            // Arrange
            var webDirectory = new FileInfo(Path.Combine(DirectoryHelper.GetRootDirectory?.FullName + @"\AdvantShop.Web\")).Directory!;
            var (success, numberOfCheckedFiles, errors) = EncodingHelper.CheckFilesEncodingInDirectory(webDirectory, Encoding.UTF8, fileExtension, new List<string>());
            // Act
            if (success is false)
            {
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }
            }
            Console.WriteLine($"Number for checked files - {numberOfCheckedFiles}");
            // Assert
            success.Should().BeTrue();
        }
    }
}
