using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            TwitterClient tweet = new TwitterClient();
            dynamic jsondata = JsonConvert.DeserializeObject(tweet.GetStatus());
            var list = new List<TwitterModels>();
            foreach(var item in jsondata)
            {
                var tweetobj = new TwitterModels();
                tweetobj.Name = item.user.name;
                tweetobj.Text = item.text;
                tweetobj.Time = item.created_at;
                list.Add(tweetobj);
            }
            
            return View(list);
        }

        public ActionResult Post (string status)
        {
            TwitterClient test = new TwitterClient();
            test.GetStatus();
            test.POST(status);
            
            return RedirectToAction("Index");
        }

    }
}