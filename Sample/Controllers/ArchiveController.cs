using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using OpenTokSDK;
using System.IO;
using System.Configuration;

namespace Sample.Controllers
{
    public class ArchiveController : Controller
    {
        private const int archivesPerPage = 5;
        private OpenTok opentok = new OpenTok(Convert.ToInt32(ConfigurationManager.AppSettings["opentok_key"]),
                                    ConfigurationManager.AppSettings["opentok_secret"]);

        // POST Archive/Start
        public string Start()
        {
            HttpApplicationState Application = HttpContext.ApplicationInstance.Application;
            return opentok.StartArchive((string)Application["sessionId"], "DotNet Archiving Sample").Id.ToString(); 
        }

        // POST Archive/Stop
        public string Stop(string id)
        {
            return opentok.StopArchive(id).Id.ToString(); 
        }

        public ActionResult Delete(string id)
        {
            if (id != null)
            {
                opentok.DeleteArchive(id);                
            }
            return Redirect("/Archive/List/");            
        }

        // GET Archive/Stop
        public ActionResult List(string id)
        {
            int page = 0;

            try
            {
                page = Int32.Parse(id);                
            }
            catch(Exception)
            {
                page = 0;
            }

            ViewBag.Archives = opentok.ListArchives(page*archivesPerPage, archivesPerPage);
            if (page > 0)
            {
                ViewBag.ShowPrevious = string.Format("/Archive/List/{0}", page - 1);
            }
            if (ViewBag.Archives.TotalCount > page*archivesPerPage + archivesPerPage)
            {
                ViewBag.ShowNext = string.Format("/Archive/List/{0}", page + 1);
            }                        
            return View();
        }
    }
}