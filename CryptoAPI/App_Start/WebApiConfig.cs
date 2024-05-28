using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CryptoAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
            name: "DefaultRoot",
            routeTemplate: "",
            defaults: new { controller = "Crypto_v1", action = "GetCryptoPrice" }
        );
        }
    }
}
