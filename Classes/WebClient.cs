using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace JuicyReddit
{
    class WebClient
    {
        //Gets the JSON text for parsing
        public async Task<string> GetJsonText(string url)
        {
            var request = WebRequest.Create(url);
            string text;
            request.ContentType = "application/json; charset=utf-8";

            var response = (HttpWebResponse)await request.GetResponseAsync();
            
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            return text;
        }

        public async Task<List<Comments.CommentsObject>> GetComments(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<Comments.CommentsObject> comments = await Newtonsoft.Json.JsonConvert.DeserializeObjectAsync<List<Comments.CommentsObject>>(json);
                return comments;
            }
            else
                throw new Exception("Could not retrieve comments");
        }


        public async Task<User.RootObject> Login(string url, string username, string password)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);

            string postData = "user=" + username;
            postData += "&passwd=" + password;
            byte[] data = Encoding.UTF8.GetBytes(postData);

            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";

            using (Stream stream = await httpRequest.GetRequestStreamAsync())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = (HttpWebResponse)await httpRequest.GetResponseAsync();

            CookieContainer cookieContainer = new CookieContainer();
            httpRequest.CookieContainer = cookieContainer;

            string jsonUrl = "http://www.reddit.com/api/login/{username}?user=" + username + "&passwd="+ password + "&api_type=json";

            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            //string jsonText = await GetJsonText(jsonUrl);

            User.RootObject user = Newtonsoft.Json.JsonConvert.DeserializeObject<User.RootObject>(responseString);

            return user;
        }
    }
}
