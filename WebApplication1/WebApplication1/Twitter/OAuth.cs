using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
namespace KMS.TwitterAPI.Twitter
{
    public class OAuth
    {
        readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string OauthToken { get; set; }
        public string OauthTokenSecret { get; set; }
        public string OauthConsumerKey { get; set; }
        public string OauthConsumerSecret { get; set; }
        public string OauthVersion { get; set; }
        public string OauthSignatureMethod { get; set; }

        public OAuth()
        {
            log.Debug("Load web configuration");
            OauthToken = ConfigurationManager.AppSettings["OauthToken"];
            OauthTokenSecret = ConfigurationManager.AppSettings["OauthTokenSecret"];
            OauthConsumerKey = ConfigurationManager.AppSettings["OauthConsumerKey"];
            OauthConsumerSecret = ConfigurationManager.AppSettings["OauthConsumerSecret"];
            OauthVersion = ConfigurationManager.AppSettings["OauthVersion"];
            OauthSignatureMethod = ConfigurationManager.AppSettings["OauthSignatureMethod"];
            log.Debug("Done loading web configuration");
        }
    }
}