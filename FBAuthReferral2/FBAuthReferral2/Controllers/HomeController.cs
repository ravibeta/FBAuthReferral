using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace FBAuthReferral2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Nordstrom Store Locator";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        private string GetWebResponse(string url)
        {
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
            return responseStr;
        }
        public ActionResult Access()
        {
            try
            {
                //TODO: CSRF protection
                var code = this.HttpContext.Request.QueryString["code"];
                code = @"AQACu9NM3ARgebtKR2OBWVxVmwppP1sWwADT0v-llA6a0f-7gf4mwcyYvB0Hntk0IzNPFD1vVT0DGqGEAi7dJ6hUmY1eYGmpMN-rHo4j7oUZz41_TeF1o_3H1zag3MtOb1K25AEuk-dVv0iCBaSUx7Dp7I5IjtuMVulh3iw8pGD1PHxq4emwJpPDO2vId4cPvkw#_=_";
                var url = @"https://graph.facebook.com/oauth/access_token?client_id=198552693558851&redirect_uri=http://localhost:50713/Home/&client_secret=2bbb5ea24cc13b4b8e4dfaa19d05e328&code=" + code;
                string responseStr = GetWebResponse(url);
                var accessToken = responseStr.Contains("&") ? responseStr.Substring(responseStr.IndexOf("=") + 1, responseStr.IndexOf("&") - responseStr.IndexOf("=") - 1) :
                    responseStr.Substring(responseStr.IndexOf("=") + 1, responseStr.Length - responseStr.IndexOf("=") - 1);
                var graphUrl = @"https://graph.facebook.com/me/checkins?access_token=" + accessToken;
                string checkins = GetWebResponse(graphUrl);
                var zip = checkins.Contains("zip") ? 
                    checkins.Substring(responseStr.IndexOf(@"zip"":""")+6, 5) : null;
                if (zip != null)
                {
                    var bingUrl = @"http://api.bing.net/xml.aspx?Appid=27F556AEB7C7501091E5B878C273F7FE12AFA91D&query=Nordstrom%20<zip>&sources=web";
                    bingUrl.Replace(@"<zip>", zip);
                    WebRequest req3 = WebRequest.Create(bingUrl);
                    WebResponse resp3 = req3.GetResponse();
                    XmlReader xr = XmlReader.Create(resp3.GetResponseStream());
                    XDocument xdoc = XDocument.Load(xr);
                    var nodes = xdoc.Descendants(XName.Get("Results", "http://schemas.microsoft.com/LiveSearch/2008/04/XML/web")).Nodes();
                    if (nodes.Count() > 0)
                    {
                        foreach (var node in nodes)
                        {
                            var desc = ((XElement)node).Element(XName.Get("Description", "http://schemas.microsoft.com/LiveSearch/2008/04/XML/web"));
                            if (desc != null && string.IsNullOrEmpty(desc.Value) == false)
                            {
                                if (desc.Value.Contains(@"("))
                                {
                                    responseStr = @"There is a Nordstrom store near you at : " +
                                        desc.Value;
                                    break;
                                }
                            }
                        }

                    }
                }
                else
                {
                    var graphUrl4 = @"https://graph.facebook.com/me?access_token=" + accessToken;
                    responseStr = GetWebResponse(graphUrl4);
                }
                ViewBag.Message = responseStr + "\r\nWhen you enable facebook checkin, this application can try to find a Nordstorm store near you";
            }
            catch (Exception e)
            {
                return RedirectToAction("About", "Home");
            }
            return View();
        }
    }
}
