using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.ExportImport;
using AdvantShop.Web.Infrastructure.Handlers;
using System;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportFeeds
{
    public class AddExportFeed : AbstractCommandHandler<object>
    {
        private readonly string _name;
        private readonly string _description;
        private readonly EExportFeedType _type;

        public AddExportFeed(string name, string description, EExportFeedType type)
        {
            _name = name;
            _description = description;
            _type = type;
        }

        protected override object Handle()
        {
            var exportFeedId = ExportFeedService.AddExportFeed(new ExportFeed
            {
                Name = _name,
                Type = _type,
                Description = _description
            });

            ExportFeedService.InsertCategory(exportFeedId, 0, false);

            var type = ReflectionExt.GetTypeByAttributeValue<ExportFeedKeyAttribute>(typeof(BaseExportFeed), atr => atr.Value, _type.ToString());
            var currentExportFeed = (BaseExportFeed)Activator.CreateInstance(type);
            currentExportFeed.SetDefaultSettings(exportFeedId);

            if (_type != EExportFeedType.None)
                Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_ExportFeeds_ExportFeedCreated, _type.ToString());

            return new
            {
                id = exportFeedId,
                typeUrlPostfix = _type == EExportFeedType.Csv || _type == EExportFeedType.CsvV2
                    ? string.Empty
                    : _type.StrName()
            };
        }
    }
}