using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using OpenTokSDK;


namespace Sample.Controllers
{
    public class HomeController : Controller
    {
        OpenTok opentok = new OpenTok(1, "");

        // GET Home/Index
        public ActionResult Index()
        {
            ViewBag.Title = "Sample app";
            return View();
        }

        // GET Home/HostView
        public ActionResult HostView()
        {
            return OpenTokView();
        }

        // GET Home/ParticipantView
        public ActionResult ParticipantView()
        {
            return OpenTokView();
        }

        private ActionResult OpenTokView()
        {
            string sessionId = GetSessionId(HttpContext.ApplicationInstance.Application);
            ViewBag.apikey = opentok.ApiKey;
            ViewBag.sessionId = sessionId;
            ViewBag.token = opentok.GenerateToken(sessionId);
            return View();
        }
        private string GetSessionId(HttpApplicationState Application)
        {
            if (Application["sessionId"] == null)
            {
                Application.Lock();
                Application["sessionId"] = opentok.CreateSession();
                Application.UnLock();
            }
            return (string)Application["sessionId"];
        }
    }
}