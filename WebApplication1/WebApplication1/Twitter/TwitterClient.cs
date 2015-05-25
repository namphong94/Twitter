using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using KMS.TwitterAPI.Twitter;

namespace KMS.TwitterAPI
{
    
    public class TwitterClient
    {
        private string oauthNonce = Convert.ToBase64String(
                                                     new ASCIIEncoding().GetBytes(
                                                     DateTime.Now.Ticks.ToString()));

        public string OauthNonce1
        {
            get { return oauthNonce; }
            set { oauthNonce = value; }
        }
        public string CreateTimestamp()
        {
            var timeSpan = DateTime.UtcNow
                                           - new DateTime(1970, 1, 1, 0, 0, 0, 0,
                                                 DateTimeKind.Utc);

            var timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();
            return timestamp;
        }

        public string CreateGetOAuth_signature(string oauth_consumer_key, string oauth_signature_method, string oauth_token,
                                                string oauth_version, string oauth_consumer_secret, string oauth_token_secret)
        {
            var baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
                            "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&screen_name={6}";
            var screen_name = "NPhongPham";
            var resource_url = "https://api.twitter.com/1.1/statuses/home_timeline.json";
            var baseString = string.Format(baseFormat,
                                        oauth_consumer_key,
                                        oauthNonce,
                                        oauth_signature_method,
                                        CreateTimestamp(),  
                                        oauth_token,
                                        oauth_version,
                                        Uri.EscapeDataString(screen_name)
                                        );

            baseString = string.Concat("GET&", Uri.EscapeDataString(resource_url), "&", Uri.EscapeDataString(baseString));

            var compositeKey = string.Concat(Uri.EscapeDataString(oauth_consumer_secret),
                                    "&", Uri.EscapeDataString(oauth_token_secret));

            string oauth_signature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {
                oauth_signature = Convert.ToBase64String(
                    hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }
            return oauth_signature;
        }
        
        public string CreatePostOAuth_signature(string status, string oauth_consumer_key, string oauth_signature_method, string oauth_token,
                                                    string oauth_version, string oauth_consumer_secret, string oauth_token_secret)
        {
            var baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
                "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&status={6}";
            var resource_url = "https://api.twitter.com/1.1/statuses/update.json";
            var baseString = string.Format(baseFormat,
                                        oauth_consumer_key,
                                        oauthNonce,
                                        oauth_signature_method,
                                        CreateTimestamp(), 
                                        oauth_token,
                                        oauth_version,
                                        Uri.EscapeDataString(status)
                                        );

            baseString = string.Concat("POST&", Uri.EscapeDataString(resource_url),
                         "&", Uri.EscapeDataString(baseString));

            var compositeKey = string.Concat(Uri.EscapeDataString(oauth_consumer_secret),
                        "&", Uri.EscapeDataString(oauth_token_secret));

            string oauth_signature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {
                oauth_signature = Convert.ToBase64String(
                    hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }
            return oauth_signature;
        }

        public void MakePostStatusRequest(string status, string resource_url, string authHeader)
        {
            var postBody = "status=" + Uri.EscapeDataString(status);
            resource_url += "?" + postBody;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resource_url);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            var response = (HttpWebResponse)request.GetResponse();
        }

        public dynamic MakeGetStatusRequest(string screen_name, string resource_url, string authHeader)
        {
            var postBody = "screen_name=" + Uri.EscapeDataString(screen_name);
            resource_url += "?" + postBody;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resource_url);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            var response = (HttpWebResponse)request.GetResponse();
            return response;
        }

        public void PostStatus(string status)
        {
            OAuth oauth = new OAuth();

            var resource_url = "https://api.twitter.com/1.1/statuses/update.json";

            var headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                               "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                               "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                               "oauth_version=\"{6}\"";

            var authHeader = string.Format(headerFormat,
                                    Uri.EscapeDataString(oauthNonce),
                                    Uri.EscapeDataString(oauth.Oauth_signature_method),
                                    Uri.EscapeDataString(CreateTimestamp()), 
                                    Uri.EscapeDataString(oauth.Oauth_consumer_key),
                                    Uri.EscapeDataString(oauth.Oauth_token),
                                    Uri.EscapeDataString(CreatePostOAuth_signature(status, oauth.Oauth_consumer_key, 
                                                                                   oauth.Oauth_signature_method, oauth.Oauth_token,
                                                                                   oauth.Oauth_version, oauth.Oauth_consumer_secret,
                                                                                   oauth.Oauth_token_secret)), 
                                    Uri.EscapeDataString(oauth.Oauth_version)
                            );

            ServicePointManager.Expect100Continue = false;

            MakePostStatusRequest(status, resource_url, authHeader);
        }

        public string GetStatus()
        {
            OAuth oauth = new OAuth();
            string screen_name = "NPhongPham";
            var resource_url = "https://api.twitter.com/1.1/statuses/home_timeline.json";

            // create the request header
            var headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                               "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                               "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                               "oauth_version=\"{6}\"";

            var authHeader = string.Format(headerFormat,
                                    Uri.EscapeDataString(oauthNonce),
                                    Uri.EscapeDataString(oauth.Oauth_signature_method),
                                    Uri.EscapeDataString(CreateTimestamp()),  
                                    Uri.EscapeDataString(oauth.Oauth_consumer_key),
                                    Uri.EscapeDataString(oauth.Oauth_token),
                                    Uri.EscapeDataString(CreateGetOAuth_signature(oauth.Oauth_consumer_key, oauth.Oauth_signature_method,
                                                                                  oauth.Oauth_token, oauth.Oauth_version, 
                                                                                  oauth.Oauth_consumer_secret, oauth.Oauth_token_secret)), 
                                    Uri.EscapeDataString(oauth.Oauth_version)
                            );


            ServicePointManager.Expect100Continue = false;

            string responseData = new StreamReader(MakeGetStatusRequest(screen_name, resource_url, authHeader).GetResponseStream()).ReadToEnd();

            return responseData;
        }

        // new method
    }
}