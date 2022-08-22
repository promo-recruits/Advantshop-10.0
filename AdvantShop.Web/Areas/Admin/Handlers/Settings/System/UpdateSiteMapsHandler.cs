﻿using System.IO;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.ExportImport;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Web.Admin.Handlers.Settings.System
{
    public class UpdateSiteMapsHandler
    {
        public CommandResult Execute()
        {
            var result = new CommandResult { Result = false };

            var htmlMapPath =  new ExportHtmlMap().Create();
            var xmlMapPath = new ExportXmlMap().Create();

            var xmlLastWriteTime = string.Empty;
            var htmlLastWriteTime = string.Empty;

            if (File.Exists(xmlMapPath))
            {
                result.Result = true;
                xmlLastWriteTime = Localization.Culture.ConvertDate(new FileInfo(xmlMapPath).LastWriteTime);
            }

            if (File.Exists(htmlMapPath))
            {
                result.Result = true;
                htmlLastWriteTime = Localization.Culture.ConvertDate(new FileInfo(htmlMapPath).LastWriteTime);
            }

            result.Obj = new { xmlLastWriteTime = xmlLastWriteTime, htmlLastWriteTime = htmlLastWriteTime, SiteMapFileHtmlLink = SettingsMain.SiteUrl + "/sitemap.html", SiteMapFileXmlLink = SettingsMain.SiteUrl + "/sitemap.xml" };

            return result;
        }
    }
}
