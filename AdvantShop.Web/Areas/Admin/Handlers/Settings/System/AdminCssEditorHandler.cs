namespace AdvantShop.Web.Admin.Handlers.Settings.System
{
    public class AdminCssEditorHandler
    {
        readonly string _path;

        public string GetFileContent()
        {
            return FilePath.FoldersHelper.ReadCss(FilePath.CssType.extra_admin);
        }

        public bool SaveFileContent(string cssContent)
        {
            return FilePath.FoldersHelper.SaveCSS(cssContent, FilePath.CssType.extra_admin);
        }
    }
}
