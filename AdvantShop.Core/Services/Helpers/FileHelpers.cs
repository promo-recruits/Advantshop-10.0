//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Saas;
using AdvantShop.Statistic;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Encoder = System.Drawing.Imaging.Encoder;

// class for work with file, managed
namespace AdvantShop.Helpers
{
    public enum EAdvantShopFileTypes
    {
        Image,
        Favicon,
        Zip,
        Catalog,
        FileInRootFolder,
        TaskAttachment,
        LeadAttachment,
        BookingAttachment,
        TemplateDocx,
        Photo
    }

    public class FileHelpers
    {
        private static Dictionary<EAdvantShopFileTypes, List<string>> AllowedFileExtensions = new Dictionary<EAdvantShopFileTypes, List<string>>
        {
            {
                EAdvantShopFileTypes.Image,
                new List<string> { ".jpg", ".gif", ".png", ".bmp", ".jpeg" }
            },
            {
                EAdvantShopFileTypes.Favicon,
                new List<string> { ".ico", ".gif", ".png" }
            },
            {
                EAdvantShopFileTypes.Zip,
                new List<string> { ".zip" }
            },
            {
                EAdvantShopFileTypes.Catalog,
                new List<string> { ".csv", ".txt" }
            },
            {
                EAdvantShopFileTypes.FileInRootFolder,
                new List<string> { ".txt", ".html", ".htm", ".xml", ".csv", ".yml"}
            },
            {
                EAdvantShopFileTypes.TaskAttachment,
                new List<string> { ".txt", ".pdf", ".doc", ".docx", ".xml", ".xls", ".xlsx", ".csv", ".rtf", ".jpg", ".jpeg", ".gif", ".png", ".bmp", ".zip", ".rar", ".mp3", ".mp4", ".avi", ".psd", ".tiff" }
            },
            {
                EAdvantShopFileTypes.LeadAttachment,
                new List<string> { ".txt", ".pdf", ".doc", ".docx", ".xml", ".xls", ".xlsx", ".csv", ".rtf", ".jpg", ".jpeg", ".gif", ".png", ".bmp", ".zip", ".rar", ".mp3", ".mp4", ".avi", ".psd", ".tiff" }
            },
            {
                EAdvantShopFileTypes.BookingAttachment,
                new List<string> { ".txt", ".pdf", ".doc", ".docx", ".xml", ".xls", ".xlsx", ".csv", ".rtf", ".jpg", ".jpeg", ".gif", ".png", ".bmp", ".zip", ".rar", ".mp3", ".mp4", ".avi", ".psd", ".tiff" }
            },
            {
                EAdvantShopFileTypes.TemplateDocx,
                new List<string> { ".docx" }
            },
            {
                EAdvantShopFileTypes.Photo,
                new List<string> { ".jpg", ".gif", ".png", ".bmp", ".jpeg", ".tiff", ".exif" }
            }
        };


        /// <summary>
        /// Delete file if it is exist
        /// </summary>
        /// <param name="fullname"></param>
        public static void DeleteFile(string fullname)
        {
            if (File.Exists(fullname))
            {
                try
                {
                    try
                    {
                        File.Delete(fullname);
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(100);
                        File.Delete(fullname);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(fullname, ex);
                }
            }
        }

        public static bool RenameFile(string prevName, string newName)
        {
            if (File.Exists(prevName) && !File.Exists(newName))
            {
                var di = new FileInfo(newName).Directory;
                if (di != null && !di.Exists)
                    di.Create();

                var count = 3;

                for (var i = 0; i < count; i++)
                {
                    try
                    {
                        File.Move(prevName, newName);
                        return true;
                    }
                    catch (IOException ex)
                    {
                        if (i != count - 1)
                        {
                            Thread.Sleep(100);
                            continue;
                        }

                        Debug.Log.Error("prevName - " + prevName + ", newName - " + newName, ex);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error("prevName - " + prevName + ", newName - " + newName, ex);
                        return false;
                    }
                }
            }

            return true;
        }

        public static void ReplaceFile(string fromFile, string resultFile)
        {
            var fileToRevert = resultFile + Guid.NewGuid();
            if (!RenameFile(resultFile, fileToRevert))
                return;

            if (RenameFile(fromFile, resultFile))
            {
                DeleteFile(fileToRevert);
            }
            else
            {
                RenameFile(fileToRevert, resultFile);
            }
        }

        public static long GetFileSize(string fileName)
        {
            try
            {
                var fi = new FileInfo(fileName);
                return fi.Length;
            }
            catch
            {
            }

            return 0;
        }

        public static void BackupPhoto(string fullname)
        {
            if (!File.Exists(fullname))
                return;

            var fullnameTo = fullname.Replace("pictures", "pictures_deleted");

            var index = fullnameTo.LastIndexOf('/');
            if (index > 0)
                CreateDirectory(fullnameTo.Substring(0, index));
            else
            {
                index = fullnameTo.LastIndexOf('\\');
                if (index > 0)
                    CreateDirectory(fullnameTo.Substring(0, index));
            }

            if (File.Exists(fullnameTo))
                File.Delete(fullnameTo);

            File.Move(fullname, fullnameTo);
        }

        public static void BackupPhotosDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                return;

            foreach (var dir in Directory.GetDirectories(directory, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dir.Replace("pictures", "pictures_deleted"));

            foreach (var file in Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories))
            {
                var newFilePath = file.Replace("pictures", "pictures_deleted");
                if (File.Exists(newFilePath))
                    File.Delete(newFilePath);

                File.Move(file, newFilePath);
            }
        }


        public static void DeleteDirectory(string directoryName, bool deleteSelf = true)
        {
            try
            {
                if (!Directory.Exists(directoryName)) return;
                var files = Directory.GetFiles(directoryName);
                var dirs = Directory.GetDirectories(directoryName);

                foreach (var file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }

                foreach (var dir in dirs)
                {
                    DeleteDirectory(dir);
                }

                if (deleteSelf)
                    Directory.Delete(directoryName, false);
            }
            catch (IOException)  // thx MS!
            {
                System.Threading.Thread.Sleep(0);
                if (deleteSelf)
                    Directory.Delete(directoryName, true);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(directoryName, ex);
            }
        }

        public static void CreateDirectory(string strPath)
        {
            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);
        }

        public static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs = false, bool copyFiles = false)
        {
            var currentDir = new DirectoryInfo(sourceDirName);

            if (!currentDir.Exists)
            {
                return;
            }
            
            Directory.CreateDirectory(destDirName);

            if (copyFiles)
            {
                var files = currentDir.GetFiles();
                foreach (var file in files)
                {
                    var tempPath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(tempPath, false);
                }
            }

            if (copySubDirs)
            {
                var subDirs = currentDir.GetDirectories();
                foreach (var subDir in subDirs)
                {
                    string tempPath = Path.Combine(destDirName, subDir.Name);
                    CopyDirectory(subDir.FullName, tempPath, copySubDirs, copyFiles);
                }
            }
        }

        public static long GetDirectorySize(string dirPath)
        {
            long length = 0;

            var dir = new DirectoryInfo(dirPath);
            foreach (var file in dir.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                length += file.Length;
            }

            return length;
        }

        public static void CreateFile(string filename)
        {
            if (!File.Exists(filename))
            {
                File.Create(filename).Dispose();
            }
        }

        public static void DeleteFilesFromPath(string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) return;
            var files = Directory.GetFiles(directoryPath); // prevent loop
            foreach (var file in files)
            {
                DeleteFile(file);
            }
        }

        public static void DeleteFilesFromImageTemp()
        {
            DeleteFilesFromPath(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
        }

        public static void DeleteFilesFromImageTempInBackground()
        {
            Task.Factory.StartNew(DeleteFilesFromImageTemp, TaskCreationOptions.LongRunning);
        }

        public static void UpdateDirectories()
        {
            var pictDirs = new List<string>
            {
                FoldersHelper.GetPathAbsolut(FolderType.Product),
                FoldersHelper.GetPathAbsolut(FolderType.News),
                FoldersHelper.GetPathAbsolut(FolderType.Category),
                FoldersHelper.GetPathAbsolut(FolderType.BrandLogo),
                FoldersHelper.GetPathAbsolut(FolderType.ManagerPhoto),
                FoldersHelper.GetPathAbsolut(FolderType.Carousel),
                FoldersHelper.GetPathAbsolut(FolderType.Color),
                FoldersHelper.GetPathAbsolut(FolderType.ReviewImage),
                FoldersHelper.GetPathAbsolut(FolderType.MobileAppIcon)
            };

            pictDirs.AddRange(FoldersHelper.ProductPhotoPrefix.Select(kvp => FoldersHelper.GetImageProductPathAbsolut(kvp.Key, string.Empty)));
            pictDirs.AddRange(FoldersHelper.CategoryPhotoPrefix.Select(kvp => FoldersHelper.GetImageCategoryPathAbsolut(kvp.Key, string.Empty)));
            pictDirs.AddRange(FoldersHelper.ColorPhotoPrefix.Select(kvp => FoldersHelper.GetImageColorPathAbsolut(kvp.Key, string.Empty)));
            pictDirs.AddRange(FoldersHelper.ReviewPhotoPrefix.Select(kvp => FoldersHelper.GetImageReviewPathAbsolut(kvp.Key, string.Empty)));
            foreach (var directory in pictDirs.Where(dir => (!Directory.Exists(dir) && dir.Trim().Length != 0)))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public static void SaveFile(string fullname, Stream serverFileStream)
        {
            const int length = 1024;
            var buffer = new Byte[length];
            serverFileStream.Position = 0;
            // write the required bytes
            using (var fs = new FileStream(fullname, FileMode.Create, FileAccess.Write))
            {
                int bytesRead;
                do
                {
                    bytesRead = serverFileStream.Read(buffer, 0, length);
                    fs.Write(buffer, 0, bytesRead);
                }
                while (bytesRead == length);
            }
        }

        public static void SaveResizePhotoFile(string resultPath, int maxWidth, int maxHeight, Image image, long? quality = null, bool isRotated = false)
        {
            UpdateDirectories();

            if (File.Exists(resultPath))
                DeleteFile(resultPath);

            if (!isRotated)
            {
                RotateImageIfNeed(image);
            }

            double resultWidth = image.Width;  // 0;
            double resultHeight = image.Height; // 0;

            if ((maxHeight != 0) && (image.Height > maxHeight))
            {
                resultHeight = maxHeight;
                resultWidth = (image.Width * resultHeight) / image.Height;
            }

            if ((maxWidth != 0) && (resultWidth > maxWidth))
            {
                resultHeight = (resultHeight * maxWidth) / resultWidth; // (resultHeight * resultWidth) / resultHeight;
                resultWidth = maxWidth;
            }

            try
            {
                using (var result = new Bitmap((int)resultWidth, (int)resultHeight))
                {
                    result.MakeTransparent();
                    using (var graphics = Graphics.FromImage(result))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.DrawImage(image, 0, 0, (int)resultWidth, (int)resultHeight);

                        graphics.Flush();
                        var ext = Path.GetExtension(resultPath);
                        var encoder = GetEncoder(ext);

                        if (!quality.HasValue)
                        {
                            quality = SettingsMain.ImageQuality;
                        }


                        using (var myEncoderParameters = new EncoderParameters(3))
                        {
                            myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality.Value);
                            myEncoderParameters.Param[1] = new EncoderParameter(Encoder.ScanMethod, (int)EncoderValue.ScanMethodInterlaced);
                            myEncoderParameters.Param[2] = new EncoderParameter(Encoder.RenderMethod, (int)EncoderValue.RenderProgressive);

                            using (var stream = new FileStream(resultPath, FileMode.CreateNew))
                            {
                                result.Save(stream, encoder, myEncoderParameters);
                                stream.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error("Error on upload " + resultPath, ex);
            }
        }

        public static void SaveResizePhotoFile(ProductImageType type, Image image, string destName, long? quality = null, bool isRotated = false)
        {
            var size = PhotoService.GetImageMaxSize(type);
            var resultPath = FoldersHelper.GetImageProductPathAbsolut(type, destName);

            SaveResizePhotoFile(resultPath, size.Width, size.Height, image, quality, isRotated);

            FilesStorageService.IncrementAttachmentsSize(resultPath);
        }


        public static void ResizeAllProductPhotos()
        {
            foreach (var photoObj in PhotoService.GetAllPhotos(PhotoType.Product))
            {
                try
                {
                    if ((!CommonStatistic.IsRun || CommonStatistic.IsBreaking))
                        return;

                    var photo = photoObj.PhotoName;
                    string originalPath = null;

                    if (photo.StartsWith("http://") || photo.StartsWith("https://"))
                    {
                        var url = photo;
                        if (url.Contains("cs71.advantshop.net"))  // http://cs71.advantshop.net/15705.jpg
                        {
                            var name = url.Split('/').LastOrDefault();
                            url = url.Replace(name, "") + "pictures/product/big/" + name.Replace(".", "_big.");
                        }

                        var photoName = photoObj.PhotoId + "." + photo.Split(new[] { '.' }).LastOrDefault();
                        originalPath = FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Original, photoName);

                        if (DownloadRemoteImageFile(url, originalPath))
                        {
                            photoObj.OriginName = photo;
                            photoObj.PhotoName = photo = photoName;

                            PhotoService.UpdatePhotoName(photoObj);
                        }
                    }

                    if (string.IsNullOrEmpty(originalPath))
                    {
                        originalPath = FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Original, photo);
                        if (!File.Exists(originalPath))
                        {
                            var bigPath = FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Big, photo);
                            if (File.Exists(bigPath))
                            {
                                File.Copy(bigPath, originalPath);
                            }
                        }
                    }

                    if (File.Exists(originalPath))
                    {
                        if (originalPath == null)
                            throw new ArgumentNullException("fileName");

                        using (var lStream = new FileStream(originalPath, FileMode.Open, FileAccess.Read))
                        {
                            using (var image = Image.FromStream(lStream))
                            {
                                FileHelpers.SaveProductImageUseCompress(photo, image, true);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                    CommonStatistic.TotalErrorRow++;
                }
                CommonStatistic.RowPosition++;
            }
        }


        private static RotateFlipType OrientationToFlipType(short orientation)
        {
            switch (orientation)
            {
                case 1:
                    return RotateFlipType.RotateNoneFlipNone;
                case 2:
                    return RotateFlipType.RotateNoneFlipX;
                case 3:
                    return RotateFlipType.Rotate180FlipNone;
                case 4:
                    return RotateFlipType.Rotate180FlipX;
                case 5:
                    return RotateFlipType.Rotate90FlipX;
                case 6:
                    return RotateFlipType.Rotate90FlipNone;
                case 7:
                    return RotateFlipType.Rotate270FlipX;
                case 8:
                    return RotateFlipType.Rotate270FlipNone;
                default:
                    return RotateFlipType.RotateNoneFlipNone;
            }
        }


        public static bool RotateImageIfNeed(Image image)
        {
            var itemOrientation = image.PropertyItems.FirstOrDefault(x => x.Id == 0x0112);
            if (itemOrientation == null)
                return false;

            var value = BitConverter.ToInt16(itemOrientation.Value, 0);
            var flip = OrientationToFlipType(value);

            if (flip == RotateFlipType.RotateNoneFlipNone)
                return false;

            image.RotateFlip(flip);
            itemOrientation.Value = BitConverter.GetBytes(1); //resert orientation

            return true;
        }

        public static void SaveProductImageUseCompress(string destName, Image image, bool skipOriginal = false)
        {
            UpdateDirectories();

            var isRotated = RotateImageIfNeed(image);

            //не удалять, создаем еще один image из-за багов в формате файла, если сохранять напрямую, выдает исключение GDI+
            using (var img = new Bitmap(image))
            {
                if (!skipOriginal && !Trial.TrialService.IsTrialEnabled)
                {
                    /* Uncomment if you want save original picture
                    if (image.Width > 3000 || image.Height > 3000)
                    {
                        SaveResizePhotoFile(ProductImageType.Original, img, destName, 100);
                    }
                    else
                    {
                        try
                        {
                            var path = FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Original, destName);
                            image.Save(path);
                        }
                        catch
                        {
                            SaveResizePhotoFile(ProductImageType.Original, img, destName, 100);
                        }
                    }
                     */
                    SaveResizePhotoFile(ProductImageType.Original, img, destName, 100, isRotated);
                }

                //if (!SettingsCatalog.CompressBigImage)
                //{
                //    var path = FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Big, destName);
                //    var size = PhotoService.GetImageMaxSize(ProductImageType.Original);

                //    SaveResizePhotoFile(path, size.Width, size.Height, img, 100, isRotated);

                //    FilesStorageService.IncrementAttachmentsSize(path);
                //}

                ModulesExecuter.ProcessPhoto(img);

                //if (SettingsCatalog.CompressBigImage)
                //{
                    SaveResizePhotoFile(ProductImageType.Big, img, destName, isRotated: isRotated);
                //}
                SaveResizePhotoFile(ProductImageType.Middle, img, destName, isRotated: isRotated);
                SaveResizePhotoFile(ProductImageType.Small, img, destName, isRotated: isRotated);
                SaveResizePhotoFile(ProductImageType.XSmall, img, destName, isRotated: isRotated);
            }
        }

        private static ImageCodecInfo GetEncoder(string fileExt)
        {
            fileExt = fileExt.TrimStart(".".ToCharArray()).ToLower().Trim();

            return CacheManager.Get("GetEncoder" + fileExt, () =>
            {
                string mimeType;
                switch (fileExt)
                {
                    case "jpg":
                    case "jpeg":
                        mimeType = "image/jpeg";
                        break;
                    case "png":
                        mimeType = "image/png";
                        break;
                    case "gif":
                        //if need transparency
                        //res = "image/png";
                        mimeType = "image/gif";
                        break;
                    default:
                        mimeType = "image/jpeg";
                        break;
                }

                return ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.MimeType == mimeType);
            });
        }

        public static string TryGetExtensionFromImage(string filePath)
        {
            try
            {
                using (var img = Image.FromFile(filePath))
                {
                    var format = img.RawFormat;

                    if (format.Equals(ImageFormat.Jpeg))
                        return ".jpeg";

                    else if (format.Equals(ImageFormat.Png))
                        return ".png";

                    else if (format.Equals(ImageFormat.Bmp))
                        return ".bmp";

                    else if (format.Equals(ImageFormat.Gif))
                        return ".gif";

                    else if (format.Equals(ImageFormat.Exif))
                        return ".exif";

                    else if (format.Equals(ImageFormat.Tiff))
                        return ".tiff";
                }
            }
            catch
            {
            }
            return "";
        }

        /// <summary>
        /// zip file or folder
        /// </summary>
        /// <param name="inputFolderPath"></param>
        /// <param name="outputPathAndFile"></param>
        /// <param name="password"></param>
        /// <param name="inscludeself">create zip with root folder</param>
        /// <returns></returns>
        public static bool ZipFiles(string inputFolderPath, string outputPathAndFile, string password = null, bool inscludeself = false)
        {
            try
            {
                int folderOffset;
                var attr = File.GetAttributes(inputFolderPath);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    if (inscludeself)
                    {
                        folderOffset = (Directory.GetParent(inputFolderPath)).ToString().Length;
                    }
                    else
                    {
                        folderOffset = inputFolderPath.Length;
                        // find number of chars to remove     // from orginal file path
                        folderOffset += (inputFolderPath.EndsWith("\\") ? 0 : 1); //remove '\'
                    }
                }
                else
                {
                    folderOffset = (Directory.GetParent(inputFolderPath)).ToString().Length;
                }

                using (var zipStream = new ZipOutputStream(File.Create(outputPathAndFile))) // create zip stream
                {
                    if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
                    zipStream.SetLevel(4); // //0-9, 9 being the highest level of compression

                    if (attr.HasFlag(FileAttributes.Directory))
                        CompressFolder(inputFolderPath, zipStream, folderOffset, Path.GetFullPath(outputPathAndFile));
                    else
                        CompressFile(inputFolderPath, zipStream, folderOffset);

                    zipStream.IsStreamOwner = true;
                    zipStream.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return false;
            }
        }

        private static void CompressFile(string filename, ZipOutputStream zipStream, int folderOffset)
        {
            FileInfo fi = new FileInfo(filename);
            string entryName = filename.Substring(folderOffset);
            entryName = ZipEntry.CleanName(entryName);
            ZipEntry newEntry = new ZipEntry(entryName);
            newEntry.DateTime = fi.LastWriteTime;
            newEntry.IsUnicodeText = true;
            newEntry.Size = fi.Length;
            zipStream.PutNextEntry(newEntry);
            byte[] buffer = new byte[4096];
            using (FileStream streamReader = File.OpenRead(filename))
            {
                StreamUtils.Copy(streamReader, zipStream, buffer);
            }
            zipStream.CloseEntry();
        }

        private static void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset, string excludeFile = null)
        {
            string[] files = Directory.GetFiles(Path.GetFullPath(path));

            if (!string.IsNullOrEmpty(excludeFile))
                files = files.Where(x => !x.Equals(excludeFile, StringComparison.OrdinalIgnoreCase)).ToArray();

            foreach (string filename in files)
            {
                CompressFile(filename, zipStream, folderOffset);
            }
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }

        [Obsolete("if not use delete in future")]
        private static List<string> GenerateFileList(string dir, bool recurse)
        {
            var files = new List<string>();

            var attr = File.GetAttributes(dir);
            if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
            {
                files.Add(dir);
                return files;
            }

            bool empty = true;
            foreach (string file in Directory.GetFiles(dir)) // add each file in directory
            {
                files.Add(file);
                empty = false;
            }

            if (empty)
            {
                // if directory is completely empty, add it
                if (Directory.GetDirectories(dir).Length == 0)
                {
                    files.Add(dir + @"/");
                }
            }

            if (recurse)
                foreach (string dirs in Directory.GetDirectories(dir)) // recursive
                {
                    files.AddRange(GenerateFileList(dirs, true));
                }
            return files; // return file list
        }

        public static bool CanUnZipFile(string inputPathOfZipFile)
        {
            int result;
            if (File.Exists(inputPathOfZipFile))
            {
                using (var zipStream = new ZipInputStream(File.OpenRead(inputPathOfZipFile)))
                {
                    zipStream.GetNextEntry();
                    result = zipStream.Available;
                }
            }
            else
            {
                return false;
            }
            return result == 1;
        }

        public static string RemoveInvalidFileNameChars(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !Path.GetInvalidFileNameChars().Any(fileName.Contains))
                return fileName;

            return Path.GetInvalidFileNameChars().Where(item => item.ToString() != "\\" && item.ToString() != "/").Aggregate(fileName, (current, charInvalid) => current.Replace(charInvalid.ToString(), string.Empty));
        }

        public static string RemoveInvalidPathChars(string path)
        {
            List<char> invalidChars = Path.GetInvalidPathChars().ToList();
            //invalidChars.Add(Path.DirectorySeparatorChar);
            //invalidChars.Add(Path.AltDirectorySeparatorChar);
            //invalidChars.Add(Path.PathSeparator);
            //invalidChars.Add(Path.VolumeSeparatorChar);

            if (string.IsNullOrEmpty(path) || !invalidChars.Any(path.Contains))
                return path;

            return invalidChars.Aggregate(path, (current, charInvalid) => current.Replace(charInvalid.ToString(), string.Empty));
        }

        private static bool UnZipFileMs(string inputPathOfZipFile, string outputDirPath)
        {
            if (!File.Exists(inputPathOfZipFile) || string.IsNullOrWhiteSpace(outputDirPath))
                return false;

            try
            {
                var file = new FileInfo(inputPathOfZipFile);
                if (file.Length == 0)
                    return false;

                using (var archive = System.IO.Compression.ZipFile.OpenRead(inputPathOfZipFile))
                {
                    foreach (var entry in archive.Entries)
                    {
                        var isDirectory = entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\");

                        if (isDirectory)
                        {
                            var newDirectory = outputDirPath + @"\" + RemoveInvalidPathChars(entry.FullName);
                            CreateDirectory(newDirectory);
                        }
                        else
                        {
                            var newFile = outputDirPath + @"\" + RemoveInvalidFileNameChars(entry.FullName);
                            DeleteFile(newFile);

                            entry.ExtractToFile(newFile);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return false;
        }

        // Unzip with folders and files
        public static bool UnZipFile(string archiveFilenameIn, string outFolder = null)
        {
            bool result = true;
            string baseDirectory = string.IsNullOrEmpty(outFolder) ? Path.GetDirectoryName(archiveFilenameIn) : outFolder;
            try
            {
                using (var zipStream = new ZipInputStream(File.OpenRead(archiveFilenameIn)))
                {
                    ZipEntry theEntry;
                    while ((theEntry = zipStream.GetNextEntry()) != null)
                    {
                        if (!theEntry.IsFile)
                        {
                            continue;           // Ignore directories
                        }
                        var strNewFile = Path.Combine(baseDirectory, theEntry.Name);
                        string directoryName = Path.GetDirectoryName(strNewFile);
                        string fileName = Path.GetFileName(strNewFile);

                        if (directoryName.Length > 0)
                            Directory.CreateDirectory(directoryName);
                        if (fileName == String.Empty) continue;

                        byte[] buffer = new byte[4096];
                        DeleteFile(strNewFile);
                        using (FileStream streamWriter = File.Create(strNewFile))
                        {
                            StreamUtils.Copy(zipStream, streamWriter, buffer);
                        }
                    }
                    zipStream.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zipStream.Close();
                }
            }
            catch (Exception ex)
            {
                result = false;
                Debug.Log.Error(ex);
            }

            if (!result)
                return UnZipFileMs(archiveFilenameIn, baseDirectory);

            return result;
        }

        public static bool IsDirectoryHaveFiles(string path)
        {
            if (!Directory.Exists(path)) return false;
            return Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Any();
        }

        public static bool DownloadRemoteImageFile(string uri, string fileName)
        {
            string msg;
            return DownloadRemoteImageFile(uri, fileName, out msg);
        }

        public static bool DownloadRemoteImageFile(string uri, string fileName, out string msg)
        {
            msg = LocalizationService.GetResource("Core.Errors.FailedToAccessFile");
            try
            {
                uri = StringHelper.ToPuny(uri);
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.134 Safari/537.36";
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                var response = (HttpWebResponse)request.GetResponse();
                var responseUrl = response.ResponseUri.ToString().ToLower();

                // Check that the remote file was found. The ContentType 
                // check is performed since a request for a non-existent 
                // image file might be redirected to a 404-page, which would 
                // yield the StatusCode "OK", even though the image was not found. 
                if ((response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Moved || response.StatusCode == HttpStatusCode.Redirect) &&
                    !response.ContentType.EndsWith("webp", StringComparison.OrdinalIgnoreCase) && // не поддерживаем webp
                    !response.ContentType.StartsWith("text/html", StringComparison.OrdinalIgnoreCase) && // валидные ссылки с изображением, но с html-контентом, напр. cloud.mail.ru
                    (response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase) || 
                     AllowedFileExtensions[EAdvantShopFileTypes.Image].Any(ext => responseUrl.EndsWith(ext)))
                   )
                {
                    DeleteFile(fileName);

                    var notEmptyFile = false;

                    // if the remote file was found, download it 
                    using (Stream inputStream = response.GetResponseStream())
                    using (Stream outputStream = File.Create(fileName))
                    {
                        var buffer = new byte[4096];
                        int bytesRead;
                        do
                        {
                            bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                            outputStream.Write(buffer, 0, bytesRead);

                            notEmptyFile |= bytesRead != 0;

                        } while (bytesRead != 0);
                    }
                    if (!notEmptyFile)
                        return false;

                    msg = null;
                    return true;
                }
                msg = LocalizationService.GetResource("Core.Errors.UnsupportedImageFormat");
                return false;

            }
            catch (WebException wex)
            {
                var errorResponse = wex.Response as HttpWebResponse;
                //404 Not Found
                if (errorResponse != null && errorResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    msg = LocalizationService.GetResource("Core.Errors.FileNotFound");
                    return false;
                }

                //Other status 401, 503, 403...
                if (wex.Status != WebExceptionStatus.ProtocolError || wex.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    return false;
                }

                //Other errors WebException. 
                Debug.Log.Error(wex);
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log.Error("Url:" + uri + ",filename:" + fileName + ". " + ex.Message, ex);
                return false;
            }
        }

        public static bool CheckFileExtension(string fileName, EAdvantShopFileTypes fileType)
        {
            if (fileName.Contains("?"))
                fileName = fileName.Split('?')[0];
            if (!AllowedFileExtensions.ContainsKey(fileType))
                throw new NotImplementedException(string.Format("Allowed file extensions for file type '{0}' are not specified", fileType.ToString()));

            return AllowedFileExtensions[fileType].Contains(Path.GetExtension(fileName.ToLower()));
        }

        public static List<FileInfo> GetFilesInRootDirectory()
        {
            var filesWithPath = Directory.GetFiles(SettingsGeneral.AbsolutePath);
            return filesWithPath.Where(file => !string.IsNullOrEmpty(file) && file.Contains("\\"))
                .Select(file => new FileInfo(file)).Where(file => CheckFileExtension(file.Name, EAdvantShopFileTypes.FileInRootFolder) && IsNotSystemFile(file.Name)).ToList();
        }

        private static bool IsNotSystemFile(string fileName)
        {
            var systemFiles = new List<string> { "app_offline", "cmsmagazine", "robots.txt" };
            if (!string.IsNullOrEmpty(fileName))
            {
                return systemFiles.All(name => !fileName.Contains(name));
            }
            return true;
        }

        public static string FileSize(string filename)
        {
            var len = new FileInfo(filename).Length;
            return FileSize(len);
        }

        public static string FileSize(long fileLength)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (fileLength >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                fileLength = fileLength / 1024;
            }
            var result = String.Format("{0:0.##} {1}", fileLength, sizes[order]);
            return result;
        }

        public static string GetExtension(string path)
        {
            if (path == null)
                return null;

            int length = path.Length;
            for (int i = length; --i >= 0;)
            {
                char ch = path[i];
                if (ch == '.')
                {
                    if (i != length - 1)
                        return path.Substring(i, length - i);
                    else
                        return String.Empty;
                }
                if (ch == '\\' || ch == '/' || ch == ':')
                    break;
            }
            return String.Empty;
        }

        public static long GetFileStorageSize()
        {
            long filesSize = 0;
            filesSize += AttachmentService.GetAllAttachmentsSize();
            return filesSize;
        }

        public static bool FileStorageLimitReached(int? newFileLength = null)
        {
            // GB to bytes
            var limitBytes = (long)SaasDataService.CurrentSaasData.FileStorageVolume * 1024 * 1024 * 1024;
            if (SaasDataService.IsSaasEnabled && limitBytes != 0 &&
                GetFileStorageSize() + (newFileLength.HasValue ? newFileLength.Value : 0) > limitBytes)
            {
                return true;
            }
            return false;
        }

        public static List<string> GetAllowedFileExtensions(EAdvantShopFileTypes advantShopFileTypes)
        {
            return AllowedFileExtensions[advantShopFileTypes];
        }

        public static string GetFilesHelpText(EAdvantShopFileTypes advantShopFileTypes, string maxFileSize = null)
        {
            var extensions = AllowedFileExtensions[advantShopFileTypes].AggregateString(", ");
            if (extensions.IsNullOrEmpty())
                return string.Empty;
            string result;
            switch (advantShopFileTypes)
            {
                case EAdvantShopFileTypes.TaskAttachment:
                    result = LocalizationService.GetResourceFormat("Core.FileHelpers.FilesHelpText.TaskAttachment", extensions);
                    break;
                case EAdvantShopFileTypes.LeadAttachment:
                    result = LocalizationService.GetResourceFormat("Core.FileHelpers.FilesHelpText.LeadAttachment", extensions);
                    break;
                case EAdvantShopFileTypes.BookingAttachment:
                    result = LocalizationService.GetResourceFormat("Core.FileHelpers.FilesHelpText.BookingAttachment", extensions);
                    break;
                default:
                    result = LocalizationService.GetResourceFormat("Core.FileHelpers.FilesHelpText.Common", extensions);
                    break;
            }
            return maxFileSize.IsNullOrEmpty()
                ? result
                : string.Format("{0}<br/>{1}", result, LocalizationService.GetResourceFormat("Core.FileHelpers.FilesHelpText.MaxFileSize", maxFileSize));
        }

        public static void ResizeMobileAppIcon(string fileName, Image image, string filePath = null)
        {
            using (var img = new Bitmap(image))
            {
                foreach(MobileAppIcon iconSize in Enum.GetValues(typeof(MobileAppIcon)))
                {
                    SaveResizeMobileAppIconFile(iconSize, img, fileName, filePath);
                }
            }
        }

        public static void DeleteMobileAppIcons(string fileName, string filePath = null)
        {
            foreach (MobileAppIcon iconSize in Enum.GetValues(typeof(MobileAppIcon)))
            {
                var resultPath = FoldersHelper.GetMobileAppIconPathAbsolut(iconSize, fileName, filePath);
                DeleteFile(resultPath);
            }
            //delete original photo
            DeleteFile((filePath ?? FoldersHelper.GetPathAbsolut(FolderType.MobileAppIcon)) + fileName);
        }

        private static void SaveResizeMobileAppIconFile(MobileAppIcon type, Image image, string fileName, string filePath = null, long quality = 90)
        {
            var size = GetMobileAppIconMaxSize(type);
            var resultPath = FoldersHelper.GetMobileAppIconPathAbsolut(type, fileName, filePath);

            SaveResizePhotoFile(resultPath, size.Width, size.Height, image, quality);

            FilesStorageService.IncrementAttachmentsSize(resultPath);
        }

        public static System.Drawing.Size GetMobileAppIconMaxSize(MobileAppIcon type)
        {
            switch (type)
            {
                case MobileAppIcon.x48:
                    return new System.Drawing.Size(48, 48);
                case MobileAppIcon.x72:
                    return new System.Drawing.Size(72, 72);
                case MobileAppIcon.x96:
                    return new System.Drawing.Size(92, 92);
                case MobileAppIcon.x128:
                    return new System.Drawing.Size(128, 128);
                case MobileAppIcon.x144:
                    return new System.Drawing.Size(144, 144);
                case MobileAppIcon.x152:
                    return new System.Drawing.Size(152, 152);
                case MobileAppIcon.x192:
                    return new System.Drawing.Size(192, 192);
                case MobileAppIcon.x384:
                    return new System.Drawing.Size(384, 384);
                case MobileAppIcon.x512:
                    return new System.Drawing.Size(512, 512);

                default:
                    throw new ArgumentException(@"Parameter must be ProductImageType", "type");
            }
        }
    }
}
