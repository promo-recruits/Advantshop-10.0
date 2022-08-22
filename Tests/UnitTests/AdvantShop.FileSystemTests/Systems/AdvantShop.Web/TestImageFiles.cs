﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvantShop.FileSystemTests.Helpers;
using ByteSizeLib;
using FluentAssertions;
using NUnit.Framework;

namespace AdvantShop.FileSystemTests.Systems.AdvantShop.Web
{
    [TestFixture]
    public class TestImageFiles
    {
        private static readonly List<string> ImageFileExtensions = new()
        {
            ".apng",
            ".avif",
            ".gif",
            ".jpg", ".jpeg", ".jfif", ".pjpeg", ".pjp",
            ".png",
            ".svg",
            ".webp"
        };

        private static readonly ByteSize MaxFileSize = ByteSize.FromKiloBytes(500);
        private const int MaxFileWidth = 2000;
        private const int MaxFileHeight = 2000;

        [Test]
        public void Images_ShouldBeLessThanMaxFileSize()
        {
            List<string> fileWhiteList = new()
            {
                "contactsCenter.png",
                "cover_text_left_modal_video.jpg",
                "bg.png",
                "pic.jpg",
                "preview.png",
                "preview2.png",
                "fa-brands-400.svg",
                "fa-solid-900.svg",
                "preview.jpg"
            };

            var webDirectory = new FileInfo(Path.Combine(DirectoryHelper.GetRootDirectory?.FullName + @"\AdvantShop.Web\")).Directory!;

            var (success, numberOfCheckedFiles, errors) = CheckFilesSize(webDirectory, fileWhiteList);
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

        [Test]
        public void Images_ShouldBeLessThanMaxFileWidthAndMaxFileHeight()
        {
            List<string> dirsWhiteList = new()
            {
                "vendors"
            };
            List<string> fileWhiteList = new()
            {
                "Bars.png",
                "Maps.png",
                "scheme2.png",
                "headerImage.jpg",
                "preview.jpg"
            };
            var webDirectory = new FileInfo(Path.Combine(DirectoryHelper.GetRootDirectory?.FullName + @"\AdvantShop.Web\")).Directory!;

            var (success, numberOfCheckedFiles, errors) = CheckFilesResolution(webDirectory, dirsWhiteList: dirsWhiteList, fileWhiteList: fileWhiteList);
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

        private static (bool success, long numberOfCheckedFiles, List<string> errors) CheckFilesSize(DirectoryInfo directoryInfo, IReadOnlyCollection<string> fileWhiteList)
        {
            var result = (success: true, numberOfCheckedFiles: (long)0, errors: new List<string>());
            foreach (var file in directoryInfo.GetFiles().Where(f => !fileWhiteList.Contains(f.Name, StringComparer.OrdinalIgnoreCase)
                                                                     && ImageFileExtensions.Contains(Path.GetExtension(f.FullName), StringComparer.OrdinalIgnoreCase)))
            {
                result.numberOfCheckedFiles++;
                if (!(file.Length > MaxFileSize.Bytes))
                    continue;

                result.success = false;
                result.errors.Add($"Expected that file `{file.Name}` along the path `{file.FullName}` should be less than {MaxFileSize.Bytes} Bytes, but this is not so, real file size is {file.Length}");
            }

            foreach (var subDir in directoryInfo.GetDirectories())
            {
                var (success, numberOfCheckedFiles, errors) = CheckFilesSize(subDir, fileWhiteList);
                result.numberOfCheckedFiles += numberOfCheckedFiles;

                if (success)
                    continue;

                result.success = false;
                result.errors.AddRange(errors);
            }

            return result;
        }

        private static (bool success, long numberOfCheckedFiles, List<string> errors) CheckFilesResolution(DirectoryInfo directoryInfo, IReadOnlyCollection<string> dirsWhiteList, IReadOnlyCollection<string> fileWhiteList)
        {
            var result = (success: true, numberOfCheckedFiles: (long)0, errors: new List<string>());
            foreach (var file in directoryInfo.GetFiles().Where(f => !fileWhiteList.Contains(f.Name, StringComparer.OrdinalIgnoreCase)
                                                                     && ImageFileExtensions.Contains(Path.GetExtension(f.FullName), StringComparer.OrdinalIgnoreCase)
                                                                     && Path.GetExtension(f.FullName) != ".svg"))
            {
                result.numberOfCheckedFiles++;

                using (var image = NetVips.Image.NewFromFile(file.FullName))
                {
                    if (!(image.Width > MaxFileWidth || image.Height > MaxFileHeight))
                        continue;

                    result.success = false;
                    result.errors.Add($"Expected that file `{file.Name}` along the path `{file.FullName}` should be less than {MaxFileHeight}x{MaxFileWidth} px, but this is not so, real file resolution is {image.Height}x{image.Width}");
                }
            }

            foreach (var subDir in directoryInfo.GetDirectories().Where(d => !dirsWhiteList.Contains(d.Name, StringComparer.OrdinalIgnoreCase)))
            {
                var (success, numberOfCheckedFiles, errors) = CheckFilesResolution(subDir, dirsWhiteList, fileWhiteList);
                result.numberOfCheckedFiles += numberOfCheckedFiles;

                if (success)
                    continue;

                result.success = false;
                result.errors.AddRange(errors);
            }

            return result;
        }
    }
}
