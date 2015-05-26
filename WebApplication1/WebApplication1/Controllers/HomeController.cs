using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KMS.TwitterAPI.Models;

namespace KMS.TwitterAPI.Controllers
{
    public class HomeController : Controller
    {
        readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult Index()
        {
            return View(LoadTimeline());
        }

        public ActionResult PostNewFeed(string status)
        {
            TwitterClient twitterClient = new TwitterClient();
            log.Debug("Post status");
            twitterClient.PostStatus(status);
            log.Debug("Done post status");
            return RedirectToAction("Index");
        }


        public ActionResult RefreshTimeline()
        {
            return RedirectToAction("Index");
        }


        public dynamic LoadTimeline()
        {
            TwitterClient tweet = new TwitterClient();
            dynamic jsondata = JsonConvert.DeserializeObject(tweet.GetStatus());
            var list = new List<TwitterModels>();
            foreach (var item in jsondata)
            {
                var tweetobj = new TwitterModels();
                tweetobj.Name = item.user.name;
                tweetobj.Text = item.text;
                tweetobj.Time = item.created_at;
                list.Add(tweetobj);
            }
            return list;
        }

    }
}