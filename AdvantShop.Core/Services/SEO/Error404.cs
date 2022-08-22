//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.SEO
{
    public class Error404
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string UrlReferer { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
