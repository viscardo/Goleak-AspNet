using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Goleak.Helpers;
using Goleak.Infra.Models;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace Goleak
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    //http://www.codeproject.com/Articles/468777/Code-First-with-Entity-Framework-5-using-MVC4-and
    //Scaffold Controller User -Repository -Force
    //Scaffold Controller Leak -Repository -Force
    //Scaffold Controller LeakOpinion -Repository -Force


    public class MvcApplication : System.Web.HttpApplication
    {

   

        protected void Application_Start()
        {
            //System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<Goleak.Models.GoleakContext>());

            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(
            //new QueryStringMapping("type", "json", new MediaTypeHeaderValue("application/json")));

            //GlobalConfiguration.Configuration.Formatters.XmlFormatter.MediaTypeMappings.Add(
            //    new QueryStringMapping("type", "xml", new MediaTypeHeaderValue("application/xml")));

            //var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            //json.SerializerSettings.PreserveReferencesHandling =
            //    Newtonsoft.Json.PreserveReferencesHandling.All;

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            /* Teste para verificar se ele altera o banco de acordo com alteração no modelo */
            //var context = new GoleakContext();
            //var initializeDomain = new CreateDatabaseIfNotExists<GoleakContext>();
            //var initializeMigrations = new MigrateDatabaseToLatestVersion<GoleakContext, System.Data.Entity.Migrations.DbMigrationsConfiguration<GoleakContext>>();

            //initializeDomain.InitializeDatabase(context);
            //initializeMigrations.InitializeDatabase(context);

        }


        /* NerdDinner i18n Custom caching */
        public override string GetVaryByCustomString(HttpContext context, string arg)
        {
            // It seems this executes multiple times and early, so we need to extract language again from cookie.
            if (arg == "culture") // culture name (e.g. "en-US") is what should vary caching
            {
                string cultureName = null;
                // Attempt to read the culture cookie from Request
                HttpCookie cultureCookie = Request.Cookies["_culture"];
                if (cultureCookie != null)
                    cultureName = cultureCookie.Value;
                else
                    cultureName = Request.UserLanguages[0]; // obtain it from HTTP header AcceptLanguages

                // Validate culture name
                cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

                return cultureName.ToLower();// use culture name as cache key, "es", "en-us", "es-cl", etc.
            }

            return base.GetVaryByCustomString(context, arg);
        }
    }
}