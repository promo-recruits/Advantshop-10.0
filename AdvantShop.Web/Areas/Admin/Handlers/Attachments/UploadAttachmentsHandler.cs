using System.Web;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Web.Admin.Models.Tasks;
using System.Text.RegularExpressions;

namespace AdvantShop.Web.Admin.Handlers.Attachments
{
    public class UploadAttachmentsHandler
    {
        private readonly int? _objId;

        public UploadAttachmentsHandler(int? objId)
        {
            _objId = objId;
        }

        public UploadAttachmentsResult[] Execute<T>() where T : Attachment, new()
        {
            var result = new List<UploadAttachmentsResult>();
            for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                result.Add(AddAttachment<T>(HttpContext.Current.Request.Files[i]));
            }

            return result.ToArray();
        }

        public UploadAttachmentsResult[] Validate<T>() where T : Attachment, new()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request.Files.Count == 0)
                return new[] { new UploadAttachmentsResult { Result = false, Error = LocalizationService.GetResource("Admin.Attachments.FileNotFound") } };

            var type = new T().Type;
            
            // файлы не добавляются ни физически, ни в базу, суммируем размеры файлов
            int filesLength = 0;

            var result = new List<UploadAttachmentsResult>();
            for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                var file = HttpContext.Current.Request.Files[i];
                result.Add(ValidateFile<T>(file, type, ref filesLength));
            }

            return result.ToArray();
        }

        private UploadAttachmentsResult ValidateFile<T>(HttpPostedFile file, AttachmentType type, ref int filesLength) where T : Attachment, new()
        {
            if (file != null && file.ContentLength > 0)
            {
                var fileName = GetValidFileName(file.FileName);

                if (!AttachmentService.CheckFileExtension(fileName, type))
                    return new UploadAttachmentsResult
                    {
                        Error = LocalizationService.GetResource("InvalidFileExtension"),
                        Attachment = new AttachmentModel { FileName = fileName }
                    };

                filesLength += file.ContentLength;
                if (FileHelpers.FileStorageLimitReached(filesLength))
                    return new UploadAttachmentsResult
                    {
                        Error = LocalizationService.GetResource("Admin.Attachments.FileStorageLimitReached"),
                        Attachment = new AttachmentModel { FileName = fileName }
                    };

                return new UploadAttachmentsResult { Result = true, Attachment = new AttachmentModel { FileName = fileName } };
            }

            return new UploadAttachmentsResult { Error = LocalizationService.GetResource("Admin.Attachments.FileNotFound") };
        }

        private UploadAttachmentsResult AddAttachment<T>(HttpPostedFile file) where T : Attachment, new()
        {
            if (!_objId.HasValue)
                return new UploadAttachmentsResult() {Error = "No object to attach file"};

            var fileName = GetValidFileName(file.FileName);

            var attachment = new T()
            {
                ObjId = _objId.Value,
                FileName = fileName,
                FileSize = file.ContentLength,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
            };
            if (attachment.Type == AttachmentType.None)
                throw new Exception("Could not create an instance of Attachment with AttachmentType None");

            int filesLength = 0;
            var validateResult = ValidateFile<T>(file, attachment.Type, ref filesLength);
            if (!validateResult.Result)
                return validateResult;

            var existAttachments = AttachmentService.GetAttachments<T>(_objId.Value);
            if (existAttachments.Any(x => x.FileName.Equals(attachment.FileName, StringComparison.OrdinalIgnoreCase)))
                return new UploadAttachmentsResult()
                {
                    Result = false,
                    Error = LocalizationService.GetResource("Admin.Attachments.FileAlreadyExists"),
                    Attachment = new AttachmentModel { FileName = attachment.FileName }
                };

            attachment.Id = AttachmentService.AddAttachment(attachment);

            if (attachment.Id != 0)
            {
                FileHelpers.SaveFile(attachment.PathAbsolut, file.InputStream);

                return new UploadAttachmentsResult()
                {
                    Result = true,
                    Attachment = new AttachmentModel
                    {
                        Id = attachment.Id,
                        ObjId = attachment.ObjId,
                        FileName = attachment.FileName,
                        FilePath = attachment.Path,
                        FilePathAdmin = attachment.PathAdmin,
                        FileSize = attachment.FileSizeFormatted
                    },
                };
            }

            return new UploadAttachmentsResult()
            {
                Result = false,
                Error = LocalizationService.GetResource("Admin.Attachments.FileNotFound")
            };
        }

        private string GetValidFileName(string fileName)
        {
            var arr = fileName.Split(new []{'.'}, StringSplitOptions.RemoveEmptyEntries);

            var ext = arr.Last();
            var name = string.IsNullOrEmpty(ext) ? fileName : fileName.Substring(0, fileName.Length - ext.Length);

            var pattern = "[^a-zA-Zа-яА-Я0-9_-]+";
            var rg = new Regex(pattern, RegexOptions.Singleline);
            var temp = rg.Replace(name, "-");
            var preparedName = Regex.Replace(temp, "-+", "-").Trim('-') + "." + ext;

            return preparedName;
        }

    }
}
