using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace paginaWeb_pav2
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
			GlobalConfiguration.Configure(WebApiConfig.Register);


            //sirve para formatear el xml a json

            GlobalConfiguration.Configuration.Formatters
                            .Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);


        }
    }
}