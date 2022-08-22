using System;
using System.Drawing;
using System.IO;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using CsvHelper;
using CsvHelper.Configuration;
using Color = AdvantShop.Catalog.Color;

namespace AdvantShop.Core.Services.ExportImport
{
    public class CsvImportColors
    {
        private readonly string _fullFileName;
        private readonly ImportColorSettings _settings;

        public CsvImportColors(string fullFileName, ImportColorSettings settings)
        {
            _fullFileName = fullFileName;
            _settings = settings;
        }

        public void Process()
        {
            var hasErrors = false;

            using (var csvReader = new CsvReader(new StreamReader(_fullFileName), new CsvConfiguration() { Delimiter = ";" }))
            {
                while (csvReader.Read())
                {
                    try
                    {
                        AddUpdateColor(csvReader);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Warn(ex);
                        hasErrors = true;
                    }
                }
            }

            if (hasErrors)
                throw new BlException("Ошибка при импорте");
        }

        private void AddUpdateColor(CsvReader csvReader)
        {
            var colorName = (csvReader.GetField<string>("Name") ?? "").Trim();

            var color = ColorService.GetColor(colorName) ?? new Color() { ColorName = colorName };

            var isNew = color.ColorId == 0;

            if (_settings.UpdateOnlyColorWithoutCodeOrIcon && !isNew &&
                (((string.IsNullOrWhiteSpace(color.ColorCode) || color.ColorCode == "#000000") && !string.IsNullOrEmpty(color.IconFileName.PhotoName)) ||
                 (!string.IsNullOrWhiteSpace(color.ColorCode) && color.ColorCode != "#000000" && string.IsNullOrEmpty(color.IconFileName.PhotoName)))
                )
            {
                return;
            }

            color.ColorCode = csvReader.GetField<string>("Code");

            if (csvReader.FieldHeaders != null && csvReader.FieldHeaders.Contains("SortOrder"))
            {
                color.SortOrder = csvReader.GetField<int>("SortOrder");
            }

            if (isNew)
            {
                if (_settings.CreateNewColor)
                {
                    ColorService.AddColor(color);
                }
            }
            else
            {
                ColorService.UpdateColor(color);
            }

            if (color.ColorId != 0)
            {
                var photo = csvReader.GetField<string>("Photo");
                if (!string.IsNullOrEmpty(photo))
                {
                    PhotoFromString(color.ColorId, photo);
                }
            }
        }


        private void PhotoFromString(int colorId, string photo)
        {
            if (photo.Contains("http://") || photo.Contains("https://"))
            {
                if (!_settings.DownloadIconByLink)
                {
                    AddColorPhotoByLink(colorId, photo);
                    return;
                }

                var uri = new Uri(photo);

                var photoname = uri.PathAndQuery.Split('?')[0].Trim('/').Replace("/", "-");
                photoname = Path.GetInvalidFileNameChars().Aggregate(photoname, (current, c) => current.Replace(c.ToString(), ""));

                if (photoname.Contains("."))
                {
                    var fileExtention = photoname.Substring(photoname.LastIndexOf('.'));

                    if (!FileHelpers.GetAllowedFileExtensions(EAdvantShopFileTypes.Photo).Contains(fileExtention))
                        return;
                }

                if (photoname.Length > 100)
                {
                    photoname = photoname.Length - 245 > 0
                        ? photoname.Substring(photoname.Length - 245)
                        : photoname;
                }

                FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));

                var filename = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoname);

                if (!FileHelpers.DownloadRemoteImageFile(photo, filename))
                    return;

                AddColorPhoto(colorId, filename);
            }
            else
            {
                var filename = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photo);

                if (!File.Exists(filename))
                    return;

                AddColorPhoto(colorId, filename);
            }
        }

        private void AddColorPhoto(int colorId, string fileName)
        {
            try
            {
                PhotoService.DeletePhotos(colorId, PhotoType.Color);

                var tempName = PhotoService.AddPhoto(new Photo(0, colorId, PhotoType.Color) { OriginName = fileName });

                if (!string.IsNullOrWhiteSpace(tempName))
                {
                    using (Image image = Image.FromFile(fileName))
                    {
                        var isRotated = FileHelpers.RotateImageIfNeed(image);

                        FileHelpers.SaveResizePhotoFile(FoldersHelper.GetImageColorPathAbsolut(ColorImageType.Catalog, tempName),
                            SettingsPictureSize.ColorIconWidthCatalog, SettingsPictureSize.ColorIconHeightCatalog, image, isRotated: isRotated);

                        FileHelpers.SaveResizePhotoFile(FoldersHelper.GetImageColorPathAbsolut(ColorImageType.Details, tempName),
                            SettingsPictureSize.ColorIconWidthDetails, SettingsPictureSize.ColorIconHeightDetails, image, isRotated: isRotated);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        private void AddColorPhotoByLink(int colorId, string photo)
        {
            PhotoService.DeletePhotos(colorId, PhotoType.Color);
            PhotoService.AddPhotoWithOrignName(new Photo(0, colorId, PhotoType.Color) { OriginName = photo, PhotoName = photo });
        }
    }


    public class ImportColorSettings
    {
        public bool CreateNewColor { get; set; }
        public bool UpdateOnlyColorWithoutCodeOrIcon { get; set; }
        public bool DownloadIconByLink { get; set; }
    }
}
