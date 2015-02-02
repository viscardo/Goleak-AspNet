using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Goleak.Infra.Models;
using Goleak.Infra.Repositorios;
using Facebook;
using Newtonsoft.Json;



namespace Goleak.Controllers
{
    public class TermsController : BaseController
    {


        public ActionResult Index()
        {
            return View();
        }


        
    }
}
