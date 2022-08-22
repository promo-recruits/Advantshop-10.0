using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AdvantShop.FileSystemTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace AdvantShop.FileSystemTests.Systems.AdvantShop.Core
{
    [TestFixture]
    public class TestFilesEncoding
    {
        [Test]
        public void Files_ShouldBeInUtf8([Values(".cs", ".csproj", ".config")] string fileExtension)
        {
            // Arrange
            var coreDirectory = new FileInfo(Path.Combine(DirectoryHelper.GetRootDirectory?.FullName + @"\AdvantShop.Core\")).Directory!;
            var (success, numberOfCheckedFiles, errors) = EncodingHelper.CheckFilesEncodingInDirectory(coreDirectory, Encoding.UTF8, fileExtension, new List<string>());
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
