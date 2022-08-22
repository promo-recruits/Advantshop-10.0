//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Core.Compress
{
    /// <summary>
    /// Summary description for CompressContent
    /// </summary>
    public class CompressContent : IHttpModule
    {
        //10MB, 1048576 = 1MB
        private const long LargeFileSize = 10485760;

        private Dictionary<string, string> supportMime = new Dictionary<string, string>
        {
            {"text/plain", ""},
            {"text/css", ""},
            {"application/javascript", ""},
            {"application/x-javascript", ""},
            {"text/html", ""},
            {"application/xml", ""},
            {"text/xml", ""},
            {"application/json", ""},
            {"text/json", ""},
            {"image/svg+xml", ""}
        };

        public void Init(HttpApplication context)
        {
            context.PostRequestHandlerExecute += PostRequestHandlerExecute;
        }

        private void PostRequestHandlerExecute(object sender, EventArgs e)
        {
            if (bool.Parse(SettingProvider.GetConfigSettingValue("EnableCompressContent")) == false)
                return;

            var app = (HttpApplication)sender;

            var request = app.Context.Request;
            var path = request.FilePath;

            if (request.RawUrl.ToLower().StartsWith("/api/") || UrlService.IsDebugUrl(path))
                return;

            var response = app.Context.Response;
            
            if (!supportMime.ContainsKey(response.ContentType))
                return;
            
            var fileP = app.Server.MapPath(path);
            var ext = Path.GetExtension(fileP);
            if (!string.IsNullOrWhiteSpace(ext))
            {
                try
                {
                    var file = new FileInfo(fileP);
                    if (file.Length > LargeFileSize)
                        return;
                }
                catch
                {
                    return;
                }
            }

            var acceptEncoding = request.Headers[HttpConstants.HttpAcceptEncoding];
            if (string.IsNullOrEmpty(acceptEncoding))
                return;

            var temp = response.Headers[HttpConstants.HttpContentEncoding];
            if (temp.IsNotEmpty())
                return;

            acceptEncoding = acceptEncoding.ToLowerInvariant();
            if (acceptEncoding.Contains(HttpConstants.HttpContentEncodingGzip))
            {
                response.Filter = new HttpCompressStream(response.Filter, CompressionMode.Compress, HttpCompressStream.CompressionType.GZip);
                response.AddHeader(HttpConstants.HttpContentEncoding, HttpConstants.HttpContentEncodingGzip);
            }
            else if (acceptEncoding.Contains(HttpConstants.HttpContentEncodingDeflate))
            {
                response.Filter = new HttpCompressStream(response.Filter, CompressionMode.Compress, HttpCompressStream.CompressionType.Deflate);
                response.AddHeader(HttpConstants.HttpContentEncoding, HttpConstants.HttpContentEncodingDeflate);
            }
        }

        public void Dispose()
        {
            // Nothing to dispose; 
        }
    }
}