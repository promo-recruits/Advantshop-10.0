using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Models.Catalog.Products;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class AddUpdateOffer : AbstractCommandHandler<bool>
    {
        private readonly AdminOfferModel _model;
        private Offer _offer;
        private List<Offer> _productOffers;

        public AddUpdateOffer(AdminOfferModel model)
        {
            _model = model;
        }

        protected override void Validate()
        {
            if (_model == null || _model.ProductId == 0 || ProductService.GetProduct(_model.ProductId) == null)
                throw new BlException(T("Admin.Product.UpdateOffer.ErrorSave"));

            _offer = _model.OfferId != 0
                            ? OfferService.GetOffer(_model.OfferId)
                            : new Offer() { ProductId = _model.ProductId };

            if (_offer == null)
                throw new BlException(T("Admin.Product.UpdateOffer.ErrorSave"));

            var allProductOffers = OfferService.GetProductOffers(_model.ProductId);

            _productOffers = allProductOffers.Where(x => x.OfferId != _offer.OfferId).ToList();

            var offerByArto = OfferService.GetOffer(_model.ArtNo);
            if (offerByArto != null && offerByArto.OfferId != _model.OfferId)
            {
                throw new BlException(T("Admin.Product.UpdateOffer.DuplicateArtNo"));
            }

            if (_model.ColorId == null && _productOffers.Any(o => o.ColorID != null))
            {
                throw new BlException(T("Admin.Product.UpdateOffer.ColorIsNotNull"));
            }
            
            if (_model.SizeId == null && _productOffers.Any(o => o.SizeID != null))
            {
                throw new BlException(T("Admin.Product.UpdateOffer.SizeIsNotNull"));
            }

            if (_productOffers.Find(x => x.ColorID == _model.ColorId && x.SizeID == _model.SizeId) != null)
            {
                throw new BlException(T("Admin.Product.UpdateOffer.Duplicate"));
            }
        }

        protected override bool Handle()
        {
            try
            {
                if (_model.Main)
                {
                    foreach (var o in _productOffers.Where(x => x.Main))
                    {
                        o.Main = false;
                        OfferService.UpdateOffer(o);
                    }
                }

                _offer.ProductId = _model.ProductId;
                _offer.ArtNo = _model.ArtNo;
                _offer.Main = !_model.Main ? !_productOffers.Any(x => x.Main) : true;
                _offer.ColorID = _model.ColorId;
                _offer.SizeID = _model.SizeId;
                _offer.BasePrice = _model.BasePrice;
                _offer.SupplyPrice = _model.SupplyPrice;
                //проблема в том что некоторые добавляют 1 млрд и может не работать выгрузка в маркет  taskid 10451
                _offer.Amount = _model.Amount > 1000000 ? 1000000 : _model.Amount;

                _offer.Weight = _model.Weight;
                _offer.Width = _model.Width;
                _offer.Height = _model.Height;
                _offer.Length = _model.Length;

                _offer.BarCode = _model.BarCode;

                if (_offer.OfferId == 0)
                {
                    OfferService.AddOffer(_offer, true);
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_AddOffer);
                }
                else
                {
                    OfferService.UpdateOffer(_offer, true);
                }
                
                ProductService.PreCalcProductParams(_offer.ProductId);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message, ex);
                throw new BlException(T("Admin.Product.UpdateOffer.ErrorSave"));
            }

            return true;
        }
    }
}
