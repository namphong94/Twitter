using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class TwitterController : Controller
    {
        // GET: Twitter
        public string Index(string status)
        {
            TwitterClient test = new TwitterClient();
            test.POST(status);

            return "Hello, Twitter!!!";
        }
    }
}