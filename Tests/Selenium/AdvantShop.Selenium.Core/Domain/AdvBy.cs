namespace AdvantShop.Selenium.Core.Domain;

public class AdvBy : By
{
    public static By DataE2E(string dataE2EToFind)
    {
        return CssSelector("[data-e2e=\"" + dataE2EToFind + "\"]");
    }
}