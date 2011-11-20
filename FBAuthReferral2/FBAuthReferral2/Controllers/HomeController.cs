using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;

namespace FBAuthReferral2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Access()
        {
            try
            {
                //TODO: CSRF protection
                var code = this.HttpContext.Request.QueryString["code"];
                code = @"AQACu9NM3ARgebtKR2OBWVxVmwppP1sWwADT0v-llA6a0f-7gf4mwcyYvB0Hntk0IzNPFD1vVT0DGqGEAi7dJ6hUmY1eYGmpMN-rHo4j7oUZz41_TeF1o_3H1zag3MtOb1K25AEuk-dVv0iCBaSUx7Dp7I5IjtuMVulh3iw8pGD1PHxq4emwJpPDO2vId4cPvkw#_=_";
                var url = @"https://graph.facebook.com/oauth/access_token?client_id=198552693558851&redirect_uri=http://localhost:50713/Home/&client_secret=2bbb5ea24cc13b4b8e4dfaa19d05e328&code=" + code;
                var uri = new Uri(url);
                string responseStr;
                var response = Redirect(url);
                WebRequest req = WebRequest.Create(uri);
                WebResponse resp = req.GetResponse();
                using (var stream = resp.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    responseStr = sr.ReadToEnd();
                }
                var accessToken = responseStr.Contains("&") ? responseStr.Substring(responseStr.IndexOf("=") + 1, responseStr.IndexOf("&") - responseStr.IndexOf("=") - 1) :
                    responseStr.Substring(responseStr.IndexOf("=") + 1, responseStr.Length - responseStr.IndexOf("=") - 1);
                var graphUrl = @"https://graph.facebook.com/me?access_token=" + accessToken;
                var uri2 = new Uri(graphUrl);
                WebRequest req2 = WebRequest.Create(uri2);
                WebResponse resp2 = req2.GetResponse();
                using (var stream = resp2.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    responseStr = sr.ReadToEnd();
                }
                ViewBag.Message = responseStr;
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}
