//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Crm.BusinessProcesses;

namespace AdvantShop.CMS
{
    public enum EntityType
    {
        Product = 0
    }

    public class Review : IBizObject
    {
        public int ReviewId { get; set; }
        public int EntityId { get; set; }
        public int ParentId { get; set; }
        public EntityType Type { get; set; }
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; }
        public DateTime AddDate { get; set; }
        public string Ip { get; set; }
        public int ChildrenCount { get; set; }
        public bool HasChildren { get { return ChildrenCount > 0; } }
        public string PhotoName { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
        public int RatioByLikes { get; set; }

        public int? ManagerId { get; set; }

        private List<ReviewPhoto> _photos;
        public List<ReviewPhoto> Photos
        {
            get
            {
                if (_photos != null)
                    return _photos;

                //if (!string.IsNullOrEmpty(PhotoName))
                //{
                //    _photo = new ReviewPhoto() { ObjId = ReviewId, PhotoName = PhotoName, Main = true };
                //    return _photo;
                //}
                
                return _photos ?? (_photos = PhotoService.GetPhotos<ReviewPhoto>(ReviewId, PhotoType.Review));
            }
        }
    }
}