using MEETJISTI2.App_Start;
using System.Web.Http;

namespace MEETJISTI2
{
    public class MEETJISTI2 : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}