﻿using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using AdvantShop.Diagnostics;
using AdvantShop.App.Landing.Domain;
using AdvantShop.App.Landing.Models.Pictures;
using AdvantShop.Core.Services.Landing;

namespace AdvantShop.App.Landing.Handlers.Pictures
{
    public class CopyPictureHandler
    {
        private int _lpId;
        private int _blockId;
        private string _filePath;


        public CopyPictureHandler(int lpId, int blockId, string filePath)
        {
            _lpId = lpId;
            _blockId = blockId;
            _filePath = filePath;
        }

        public UploadPictureResult Execute()
        {
            try
            {
                var lp = new LpService().Get(_lpId);

                var landingBlockFolder = HostingEnvironment.MapPath(string.Format(LpFiles.UploadPictureFolderLandingBlock, lp.LandingSiteId, _lpId, _blockId));

                if (!Directory.Exists(landingBlockFolder))
                    Directory.CreateDirectory(landingBlockFolder);

                var ext = "." + _filePath.Split('.').LastOrDefault();
                var fileName = Guid.NewGuid().ToString("N") + ext;

                File.Copy(_filePath, landingBlockFolder + fileName);
                
                return new UploadPictureResult()
                {
                    Result = true,
                    Picture = string.Format(LpFiles.UploadPictureFolderLandingBlockRelative + fileName, lp.LandingSiteId, _lpId, _blockId)
                };
            }
            catch (Exception ex)
            {
                Debug.Log.Error("LandingPage module, UploadPictureHandler", ex);
                return new UploadPictureResult() { Error = ex.Message };
            }
        }
    }
}
