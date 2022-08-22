namespace AdvantShop.Selenium.Core.Domain;

public class BaseMultiSeleniumTest : BaseSeleniumTest
{
    [TearDown]
    public void Teardown()
    {
        try
        {
            //  Console.Error.WriteLine(verificationErrors);
            Driver.Quit();
        }
        catch (Exception)
        {
            // Ignore errors if unable to close the browser
        }
    }
}