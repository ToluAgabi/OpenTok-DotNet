using OpenTokSDK;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleSample.Controllers
{
    public class HomeController : Controller
    {
        private OpenTok opentok = new OpenTok(Convert.ToInt32(ConfigurationManager.AppSettings["opentok_key"]),
                                    ConfigurationManager.AppSettings["opentok_secret"]);

        public ActionResult Index()
        {
            HttpApplicationState Application = HttpContext.ApplicationInstance.Application;
            ViewBag.sessionId = Application["sessionId"];
             ViewBag.token = opentok.GenerateToken(ViewBag.sessionId);
            ViewBag.apiKey = opentok.ApiKey;

            return View();
        }
    }
}