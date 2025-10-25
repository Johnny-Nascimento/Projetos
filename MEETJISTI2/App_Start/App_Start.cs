using System.Web.Http;
using WebActivatorEx;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(YourNamespace.App_Start.SwaggerConfig), "Register")]

namespace YourNamespace.App_Start
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "API Jitsi");
                    //c.IncludeXmlComments(GetXmlCommentsPath()); // Opcional: documenta comentários
                })
                .EnableSwaggerUi();
        }

        private static string GetXmlCommentsPath()
        {
            return System.String.Format(@"{0}\bin\YourApi.xml", System.AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
