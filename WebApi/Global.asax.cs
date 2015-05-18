using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;


namespace WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            String jsonSerializerSettingsIgnoreNulls = ConfigurationManager.AppSettings["json.SerializerSettings.ignoreNulls"];
            String jsonSerializerSettingsCamelCase = ConfigurationManager.AppSettings["json.SerializerSettings.camelCase"];

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = (jsonSerializerSettingsIgnoreNulls == "true") ? NullValueHandling.Ignore : NullValueHandling.Include,
                ContractResolver =  (jsonSerializerSettingsCamelCase == "true") ? new CamelCasePropertyNamesContractResolver() : new DefaultContractResolver()
            };
        }
    }
}
