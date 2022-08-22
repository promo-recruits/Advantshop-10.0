using System;
using System.Collections.Generic;
using AdvantShop.App.Landing.Domain;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;

namespace AdvantShop.Core.Services.Landing.Templates
{
    public enum LpTemplatePageType
    {
        None = 0,
        Main = 1,
        MainSecond = 2,
        UpsellFirst = 3,
        UpsellSecond = 4,
        Downsell = 5,
        ThankYouPage = 6,
        MainThird = 7,
        MainFour = 8,
        MainFive = 9,
        MainSix = 10
    };

    public enum LpSiteCategory
    {
        [Localize("Интернет-магазин")]
        Store,

        [Localize("Товарные воронки")]
        ProductSinglePages,

        [Localize("Сбор лидов")]
        CollectingLeads,

        [Localize("Опросники")]
        Surveys,

        [Localize("Презентационные воронки")]
        Presentations,

        [Localize("Лендинги")]
        CompanySite,
    }

    /// <summary>
    /// Шаблон для сайта лендинга
    /// </summary>
    public class LpTemplate
    {
        public string Key { get; set; }

        public LpFunnelType TemplateType { get; set; }
        public LpFunnelCategory Category { get; set; }

        public LpSiteCategory SiteCategory { get; set; }

        public string Video { get; set; }

        public string Scheme { get; set; }

        public int SortOrder { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public string Intent { get; set; }
        public string Advice { get; set; }

        public string Picture { get; set; }

        public List<LpBlockListItem> BlockLists { get; set; }

        public Dictionary<string, string> SiteSettings { get; set; }

        public List<LpTemplateLandingPageItem> Pages { get; set; }

        public List<LpTemplateScreen> Screens { get; set; }

        public string DemoLink { get; set; }
    }

    public class LpTemplateLandingPageItem
    {
        public string Name { get; set; }
        public LpTemplatePageType PageType { get; set; }
        public Dictionary<string, string> LpPageSettings { get; set; }
        public List<LpTemplateBlockItem> Blocks { get; set; }
    }

    [Serializable]
    public class LpTemplateBlockItem
    {
        public string Name { get; set; }

        public LpForm Form { get; set; }

        public Dictionary<string, object> Settings { get; set; }

        public List<LpSubBlockConfig> SubBlocks { get; set; }
    }

    public class LpTemplateScreen
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
