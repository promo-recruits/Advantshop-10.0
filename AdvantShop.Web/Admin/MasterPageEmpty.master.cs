//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web.UI;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Localization;
using AdvantShop.Core.UrlRewriter;

namespace Admin
{
    public partial class MasterPageEmpty : MasterPage
    {
      
        protected void Page_Load(object sender, EventArgs e)
        {
            form1.Action = UrlService.GetAdminBaseUrl();
        }

        public void Page_PreRender(object sender, EventArgs e)
        {
            var cultureName = Culture.Language != Culture.SupportLanguage.Other ? SettingsMain.Language : "ru-RU";

            JsCssTool.ReCreateIfNotExist();

            headStyle.Text = JsCssTool.MiniCss(new List<string>{
                                    "~/admin/css/validator.css"
                                   ,"~/admin/css/normalize.css"
                                   ,"~/admin/css/advcss/modal.css"
                                   ,"~/admin/css/jq/jquery.autocomplete.css"
                                   ,"~/admin/css/modalAdmin.css"

                                   ,"~/admin/js/plugins/progress/css/progress.css"
                                   ,"~/admin/js/plugins/tabs/css/tabs.css"
                                   ,"~/admin/js/plugins/bubble/css/bubble.css"
                                   ,"~/admin/css/jquery.tooltip.css"
                                   ,"~/admin/css/AdminStyle.css"
                                   ,"~/admin/css/advcss/notify.css"
                                   ,"~/admin/css/catalogDataTreeStyles.css"
                                   ,"~/admin/js/plugins/tooltip/css/tooltip.css"
                                   ,"~/admin/js/plugins/placeholder/css/placeholder.css"
                                   ,"~/admin/js/plugins/radiolist/css/radiolist.css"
                                   ,"~/admin/js/plugins/help/css/help.css"
                                   ,"~/admin/js/plugins/datepicker/css/datepicker.css"

                                   ,"~/admin/css/new_admin/buttons.css"
                                   ,"~/admin/css/new_admin/dropdown-menu.css"
                                   ,"~/admin/css/new_admin/icons.css"
                                   ,"~/admin/css/new_admin/admin.css"
                                   ,"~/admin/css/new_admin/pagenumber.css"
                                   ,"~/admin/css/new_admin/modules.css"},
                                   "admincss.css");

            headScript.Text = JsCssTool.MiniJs(new List<string>{
                                    "~/admin/js/jq/jquery-1.7.1.min.js"
                                    ,"~/admin/js/modernizr.custom.js"
                                    ,"~/admin/js/localization/" + cultureName + "/lang.js"
                                    ,"~/admin/js/ejs_fulljslint.js"
                                    ,"~/admin/js/ejs.js"
                                    },
                                    "adminlib.js");

            bottomScript.Text = JsCssTool.MiniJs(new List<string>{
                                       "~/admin/js/jq/jquery.validate.js"
                                      ,"~/admin/js/validateInit.js"
                                      ,"~/admin/js/string.format-1.0.js"
                                      ,"~/admin/js/jq/jquery.autocomplete.js"
                                      ,"~/admin/js/jq/jquery.metadata.js"
                                      ,"~/admin/js/advjs/advNotify.js"
                                      ,"~/admin/js/advjs/advModal.js"
                                      ,"~/admin/js/advjs/advTabs.js"
                                      ,"~/admin/js/advjs/advUtils.js"
                                      ,"~/admin/js/advantshop.js"
                                      ,"~/admin/js/services/Utilities.js"
                                      ,"~/admin/js/services/scriptsManager.js"
                                      ,"~/admin/js/services/jsuri-1.1.1.js"
                                      ,"~/admin/js/plugins/progress/progress.js"
                                      ,"~/admin/js/plugins/tabs/tabs.js"
                                      ,"~/admin/js/plugins/bubble/bubble.js"
                                      ,"~/admin/js/customValidate.js"
                                      ,"~/admin/js/smallThings.js"
                                      ,"~/admin/js/smallThings.js"

                                      ,"~/admin/js/jspage/modulesmanager.js"

                                      ,"~/admin/js/jquery.cookie.min.js"
                                      ,"~/admin/js/jquery.qtip.min.js"
                                      ,"~/admin/js/jquery.tooltip.min.js"
                                      ,"~/admin/js/slimbox2.js"
                                      ,"~/admin/js/jquery.history.js"
                                      ,"~/admin/js/jquerytimer.js"
                                      ,"~/admin/js/admin.js"
                                      ,"~/admin/js/grid.js"
                                      ,"~/admin/js/plugins/tooltip/tooltip.js"
                                      ,"~/admin/js/plugins/placeholder/placeholder.js"
                                      ,"~/admin/js/plugins/radiolist/radiolist.js"
                                      ,"~/admin/js/plugins/help/help.js"


                                      ,"~/admin/js/plugins/datepicker/bootstrap-datepicker.js"
                                      ,"~/admin/js/plugins/datepicker/locales/bootstrap-datepicker." + cultureName.Split('-')[0] + ".js"
                                      
                                      ,"~/admin/js/plugins/jqfileupload/vendor/jquery.ui.widget.js"
                                      ,"~/admin/js/plugins/jqfileupload/jquery.iframe-transport.js"
                                      ,"~/admin/js/plugins/jqfileupload/jquery.fileupload.js"

                                     },
                                      "adminall.js");

            lBase.Text = string.Format("<base href='{0}'/>", UrlService.GetUrl("admin/"));
        }

    }
}