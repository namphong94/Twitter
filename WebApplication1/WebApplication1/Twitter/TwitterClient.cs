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
        readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
            log.Debug("Create Timestamp");
            var timeSpan = DateTime.UtcNow
                                           - new DateTime(1970, 1, 1, 0, 0, 0, 0,
                                                 DateTimeKind.Utc);

            var timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();
            log.Debug("Done creating Timestamp");
            return timestamp;
        }

        public string CreateGetOAuthSignature(string oauthConsumerKey, string oauthSignatureMethod, string oauthToken,
                                                string oauthVersion, string oauthConsumerSecret, string oauthTokenSecret)
        {
            log.Debug("Create Get OAuthSignature");
            var baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
                            "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&screen_name={6}";
            var screenName = "NPhongPham";
            var resourceURL = "https://api.twitter.com/1.1/statuses/home_timeline.json";
            var baseString = string.Format(baseFormat,
                                        oauthConsumerKey,
                                        oauthNonce,
                                        oauthSignatureMethod,
                                        CreateTimestamp(),
                                        oauthToken,
                                        oauthVersion,
                                        Uri.EscapeDataString(screenName)
                                        );

            baseString = string.Concat("GET&", Uri.EscapeDataString(resourceURL), "&", Uri.EscapeDataString(baseString));

            var compositeKey = string.Concat(Uri.EscapeDataString(oauthConsumerSecret),
                                    "&", Uri.EscapeDataString(oauthTokenSecret));

            string oauthSignature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {
                oauthSignature = Convert.ToBase64String(
                    hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }
            log.Debug("Done creating Get OAuthSignature");
            return oauthSignature;
        }

        public string CreatePostOAuthSignature(string status, string oauthConsumerKey, string oauthSignatureMethod, string oauthToken,
                                                    string oauthVersion, string oauthConsumerSecret, string oauthTokenSecret)
        {
            log.Debug("Create Post OAuthSignature");
            var baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
                "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&status={6}";
            var resourceURL = "https://api.twitter.com/1.1/statuses/update.json";
            var baseString = string.Format(baseFormat,
                                        oauthConsumerKey,
                                        oauthNonce,
                                        oauthSignatureMethod,
                                        CreateTimestamp(),
                                        oauthToken,
                                        oauthVersion,
                                        Uri.EscapeDataString(status)
                                        );

            baseString = string.Concat("POST&", Uri.EscapeDataString(resourceURL),
                         "&", Uri.EscapeDataString(baseString));

            var compositeKey = string.Concat(Uri.EscapeDataString(oauthConsumerSecret),
                        "&", Uri.EscapeDataString(oauthTokenSecret));

            string oauthSignature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {
                oauthSignature = Convert.ToBase64String(
                    hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }
            log.Debug("Done creating Post OAuthSignature");
            return oauthSignature;
        }

        public void MakePostStatusRequest(string status, string resourceURL, string authHeader)
        {
            log.Debug("Make Post Status Request");
            var postBody = "status=" + Uri.EscapeDataString(status);
            resourceURL += "?" + postBody;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resourceURL);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
            }
            catch(Exception e)
            {
                log.Error(e);
            }
            log.Debug("Done making Post OAuthSignature");
        }

        public dynamic MakeGetStatusRequest(string screenName, string resourceURL, string authHeader)
        {
            log.Debug("Make Get stattus request");
            var postBody = "screen_name=" + Uri.EscapeDataString(screenName);
            resourceURL += "?" + postBody;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resourceURL);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            HttpWebResponse response = null;
            try
            {
               response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            log.Debug("Done make Get stattus request");

            return response;
        }

        public void PostStatus(string status)
        {
            log.Debug("Post status");
            OAuth Oauth = new OAuth();

            var resourceURL = "https://api.twitter.com/1.1/statuses/update.json";

            var headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                               "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                               "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                               "oauth_version=\"{6}\"";

            var authHeader = string.Format(headerFormat,
                                    Uri.EscapeDataString(oauthNonce),
                                    Uri.EscapeDataString(Oauth.OauthSignatureMethod),
                                    Uri.EscapeDataString(CreateTimestamp()),
                                    Uri.EscapeDataString(Oauth.OauthConsumerKey),
                                    Uri.EscapeDataString(Oauth.OauthToken),
                                    Uri.EscapeDataString(CreatePostOAuthSignature(status, Oauth.OauthConsumerKey,
                                                                                   Oauth.OauthSignatureMethod, Oauth.OauthToken,
                                                                                   Oauth.OauthVersion, Oauth.OauthConsumerSecret,
                                                                                   Oauth.OauthTokenSecret)),
                                    Uri.EscapeDataString(Oauth.OauthVersion)
                            );

            ServicePointManager.Expect100Continue = false;

            MakePostStatusRequest(status, resourceURL, authHeader);
            log.Debug(" Done Posting status");
        }

        public string GetStatus()
        {
            log.Debug("Get status");
            OAuth Oauth = new OAuth();
            string screenName = "NPhongPham";
            var resourceURL = "https://api.twitter.com/1.1/statuses/home_timeline.json";

            // create the request header
            var headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                               "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                               "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                               "oauth_version=\"{6}\"";

            var authHeader = string.Format(headerFormat,
                                    Uri.EscapeDataString(oauthNonce),
                                    Uri.EscapeDataString(Oauth.OauthSignatureMethod),
                                    Uri.EscapeDataString(CreateTimestamp()),
                                    Uri.EscapeDataString(Oauth.OauthConsumerKey),
                                    Uri.EscapeDataString(Oauth.OauthToken),
                                    Uri.EscapeDataString(CreateGetOAuthSignature(Oauth.OauthConsumerKey, Oauth.OauthSignatureMethod,
                                                                                  Oauth.OauthToken, Oauth.OauthVersion,
                                                                                  Oauth.OauthConsumerSecret, Oauth.OauthTokenSecret)),
                                    Uri.EscapeDataString(Oauth.OauthVersion)
                            );


            ServicePointManager.Expect100Continue = false;

            string responseData = new StreamReader(MakeGetStatusRequest(screenName, resourceURL, authHeader).GetResponseStream()).ReadToEnd();
            log.Debug("Done Getting status");
            return responseData;
        }

        // new method
    }
}