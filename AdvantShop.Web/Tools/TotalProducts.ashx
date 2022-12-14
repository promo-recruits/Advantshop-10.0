<%@ WebHandler Language="C#" Class="TotalProducts" %>

using System.Web;
using AdvantShop.Catalog;

public class TotalProducts : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        context.Response.Write(ProductService.GetProductsCount("[Enabled] = 1"));
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}