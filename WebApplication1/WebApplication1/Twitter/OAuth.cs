using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
namespace KMS.TwitterAPI.Twitter
{
    public class OAuth
    {
        public string Oauth_token {get; set;}
        public string Oauth_token_secret {get; set;}
        public string Oauth_consumer_key {get; set;}
        public string Oauth_consumer_secret {get; set;}
        public string Oauth_version {get; set;}
        public string Oauth_signature_method {get; set;}

        public OAuth()
        {
            Oauth_token = ConfigurationManager.AppSettings["oauth_token"];
            Oauth_token_secret = ConfigurationManager.AppSettings["oauth_token_secret"];
            Oauth_consumer_key = ConfigurationManager.AppSettings["oauth_consumer_key"];
            Oauth_consumer_secret = ConfigurationManager.AppSettings["oauth_consumer_secret"];
            Oauth_version = ConfigurationManager.AppSettings["oauth_version"];
            Oauth_signature_method = ConfigurationManager.AppSettings["oauth_signature_method"];
        }
    }
}