using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using OsisModel.Models;
using System.Globalization;
using FluentValidation.Mvc;
using System.Threading;

namespace OsisModel
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FluentValidationModelValidatorProvider.Configure();

            Database.SetInitializer<ApplicationDbContext>(null);

            AutoMapperConfig.RegisterMappings();
        }

        protected void Application_BeginRequest()
        {
            //CultureInfo cInf = new CultureInfo.("en-GB", false);

            CultureInfo cInf;
            cInf = CultureInfo.CreateSpecificCulture("en-GB");

            // NOTE: change the culture name en-ZA to whatever culture suits your needs

            cInf.DateTimeFormat.DateSeparator = "/";
            cInf.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            cInf.DateTimeFormat.LongDatePattern = "dd/MM/yyyy hh:mm:ss tt";

            CultureInfo.DefaultThreadCurrentCulture = cInf;
            CultureInfo.DefaultThreadCurrentUICulture = cInf;

            //System.Threading.Thread.CurrentThread.CurrentCulture = cInf;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = cInf;

            Thread.CurrentThread.CurrentCulture = cInf;
            Thread.CurrentThread.CurrentUICulture = cInf;
        }

        
    }
}
