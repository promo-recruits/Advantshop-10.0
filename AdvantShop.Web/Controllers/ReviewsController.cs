using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Models.Review;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;
using Newtonsoft.Json;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class ReviewsController : BaseClientController
    {
        #region Help methods

        private bool IsValidData(ReviewModel review, out string errors)
        {
            errors = string.Empty;
            //try { _entityType = (EntityType)review.EntityType; }
            //catch { return false; }

            //if (!ReviewService.IsExistsEntity(review.EntityId, (EntityType)review.EntityType))
            //    return false;

            //if (!Int32.TryParse(context.Request["parentId"], out _parentId))
            //    return false;

            //_text = !string.IsNullOrEmpty(review.Text)
            //                ? HttpUtility.HtmlEncode(context.Request["text"].Trim()).Replace("\n", "<br />") : string.Empty;

            //if (_text.Length < 3)
            //    return false;

            //_name = !string.IsNullOrEmpty(context.Request["name"])
            //                ? HttpUtility.HtmlEncode(context.Request["name"].Trim()) : string.Empty;

            //if (_name.Length < 3)
            //    return false;

            //_email = !string.IsNullOrEmpty(context.Request["email"])
            //                ? HttpUtility.HtmlEncode(context.Request["email"].Trim()) : string.Empty;

            //if (_email.Length < 3 || !ValidationHelper.IsValidEmail(_email))
            //    return false;

            if (review.Agreement != SettingsCheckout.IsShowUserAgreementText)
            {
                errors = LocalizationService.GetResource("Js.Subscribe.ErrorAgreement");
                return false;
            }

            return true;
        }

        #endregion

        public JsonResult Add(List<HttpPostedFileBase> file, string data)
        {
            var error = string.Empty;
            var review = JsonConvert.DeserializeObject<ReviewModel>(data);
            if (!IsValidData(review, out error))
                return Json(new { error = true, errors = error });

            var allowAdd = ModulesExecuter.CheckInfo(System.Web.HttpContext.Current, Core.Modules.Interfaces.ECheckType.ProductReviews, review.Email, review.Name, message: review.Text);
            if (!allowAdd)
            {
                return Json(new { error = true, errors = T("Common.SpamCheckFailed") });
            }

            var text = HttpUtility.HtmlEncode(review.Text.Trim()).Replace("\n", "<br />");
            var name = HttpUtility.HtmlEncode(review.Name.Trim());
            var email = HttpUtility.HtmlEncode(review.Email.Trim());

            var reviewItem = new Review
            {
                ParentId = review.ParentId,
                EntityId = review.EntityId,
                CustomerId = CustomerContext.CustomerId,
                Text = text,
                Type = (EntityType)review.EntityType,
                Name = name,
                Email = email,
                Ip = Request.UserHostAddress.IsLocalIP() ? "local" : Request.UserHostAddress,
                AddDate = DateTime.Now
            };

            ReviewService.AddReview(reviewItem);

            if (SettingsCatalog.AllowReviewsImageUploading && file != null && file.Count > 0)
            {
                foreach (var fPhoto in file.Where(x => FileHelpers.CheckFileExtension(x.FileName, EAdvantShopFileTypes.Image)))
                {
                    var photoName = PhotoService.AddPhoto(new Photo(0, reviewItem.ReviewId, PhotoType.Review)
                    {
                        OriginName = fPhoto.FileName
                    });

                    if (!string.IsNullOrWhiteSpace(photoName))
                    {
                        using (var image = Image.FromStream(fPhoto.InputStream))
                        {
                            var isRotated = FileHelpers.RotateImageIfNeed(image);

                            FileHelpers.SaveResizePhotoFile(FoldersHelper.GetImageReviewPathAbsolut(ReviewImageType.Small, photoName),
                                SettingsPictureSize.ReviewImageWidth, SettingsPictureSize.ReviewImageHeight, image, isRotated: isRotated);
                            FileHelpers.SaveResizePhotoFile(FoldersHelper.GetImageReviewPathAbsolut(ReviewImageType.Big, photoName),
                                image.Width, image.Height, image, isRotated: isRotated);
                        }
                    }
                }
            }

            try
            {
                var p = ProductService.GetProduct(review.EntityId);
                if (p != null)
                {
                    var mailTemplate = new ProductDiscussMailTemplate(p.ArtNo, p.Name,
                        Url.AbsoluteRouteUrl("Product", new { url = p.UrlPath }), name, DateTime.Now.ToString(), text,
                        Url.AbsoluteRouteUrl("Product", new { url = p.UrlPath }), email);
                    
                    MailService.SendMailNow(SettingsMail.EmailForProductDiscuss, mailTemplate);

                    AdminInformerService.Add(new AdminInformer(AdminInformerType.Review, reviewItem.ReviewId, reviewItem.CustomerId)
                    {
                        EntityId = review.EntityId,
                        Title = string.Format("Новый комментарий к товару \"{0}\"", p.Name),
                        //Body = review.Text, // XSS инъекция
                    });

                    if (reviewItem.ParentId != 0)
                    {
                        var previousReview = ReviewService.GetReview(reviewItem.ParentId);
                        if (previousReview != null && !string.IsNullOrWhiteSpace(previousReview.Email))
                        {
                            var mailAnswerTemplate = new ProductDiscussAnswerMailTemplate(p.ArtNo, p.Name,
                                Url.AbsoluteRouteUrl("Product", new { url = p.UrlPath, tab = "tabReviews" }), name, DateTime.Now.ToString(),
                                previousReview.Text,
                                text);
                            
                            MailService.SendMailNow(previousReview.Email, mailAnswerTemplate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return Json(new
            {
                error = false,
                review = new
                {
                    ParentId = reviewItem.ParentId,
                    ReviewId = reviewItem.ReviewId,
                    Name = reviewItem.Name,
                    Text = reviewItem.Text,
                    Photos = reviewItem.Photos.Where(x => x.PhotoName.IsNotEmpty()).Select(x =>
                        new
                        {
                            photoId = x.PhotoId,
                            small = x.ImageSrc(),
                            big = x.BigImageSrc()
                        }),
                    Likes = reviewItem.LikesCount,
                    Dislikes = reviewItem.DislikesCount,
                    RatioByLikes = reviewItem.RatioByLikes,
                }
            });
        }

        public JsonResult Delete(int reviewId)
        {
            var customer = CustomerContext.CurrentCustomer;
            if (customer.IsAdmin || (customer.IsModerator && customer.HasRoleAction(RoleAction.Catalog)))
            {
                if (reviewId == 0)
                    return Json(false);

                ReviewService.DeleteReview(reviewId);
                return Json(true);
            }

            return Json(false);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult LikeVote(int? reviewId, bool vote)
        {
            if (!reviewId.HasValue || ReviewService.GetReview(reviewId.Value) == null)
                return Json(new { error = true, errors = "Отзыв не найден." });
            if (SettingsCatalog.ReviewsVoiteOnlyRegisteredUsers && !CustomerContext.CurrentCustomer.RegistredUser)
                return Json(new { error = true, errors = "Голосовать могут только зарегистрированные пользователи." });

            ReviewService.AddVote(reviewId.Value, vote);

            var reviewItem = ReviewService.GetReview(reviewId.Value);

            return Json(new
            {
                error = false,
                likeData = new
                {
                    Likes = reviewItem.LikesCount,
                    Dislikes = reviewItem.DislikesCount,
                    RatioByLikes = reviewItem.RatioByLikes,
                }
            });
        }

    }
}