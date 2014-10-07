using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

using Newtonsoft.Json;
using System.IO;
using Windows.UI.Popups;
using Windows.Foundation;

namespace JuicyReddit
{
    class Reddit
    {
        private WebClient wc = new WebClient();
        private string[] defaultSubreddits = {"r/front","r/all","r/adviceanimals","r/AskReddit","r/aww","r/bestof","r/books","r/earthporn","r/explainlikeimfive","r/funny","r/gaming","r/gifs","r/IAmA","r/movies",
                                                 "r/music","r/news","r/pics", "r/science","r/technology","r/television","r/todayilearned","r/videos","r/worldnews","r/wtf"};
        
        public Reddit()
        {
        }

        //Returns a single topic
        public async Task<Page.Topic> GetTopic(string url, int index)
        {
            string jsonText = await wc.GetJsonText(url);
            Page.RootObject deserializeObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Page.RootObject>(jsonText);
            

            Page.Topic topic = new Page.Topic();
            topic.title = deserializeObject.data.children[index].data.title;
            topic.link = deserializeObject.data.children[index].data.permalink;
            topic.author = deserializeObject.data.children[index].data.author;
            topic.text = deserializeObject.data.children[index].data.selftext;
            topic.timeposted = deserializeObject.data.children[index].data.created_utc;
            topic.timeposted = GetHoursDifference(topic.timeposted);
            topic.points = deserializeObject.data.children[index].data.ups;
            topic.subreddit = deserializeObject.data.children[index].data.subreddit;
            topic.commentCount = deserializeObject.data.children[index].data.num_comments;
            topic.linkExternal = deserializeObject.data.children[index].data.url;
            topic.commentId = deserializeObject.data.children[index].data.name;
            string sub = topic.commentId.Substring(3);
            topic.commentId = sub;
            topic.thumbnail = deserializeObject.data.children[index].data.thumbnail;
            return topic;
        }

        //Returns a list of topics
        public async Task<List<Page.Topic>> GetTopicList(string url)
        {
            List<Page.Topic> topicList = new List<Page.Topic>();
            string jsonText;
            IAsyncOperation<IUICommand> asyncCommand;

            try
            {
                jsonText = await wc.GetJsonText(url);
                Page.RootObject deserializeObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Page.RootObject>(jsonText);
                for (int i = 0; i < deserializeObject.data.children.Count; i++)
                {
                    topicList.Add(await GetTopic(url, i));
                }
            }
            catch (HttpRequestException e)
            {
                MessageDialog md = new MessageDialog(e.Message);
                asyncCommand =  md.ShowAsync();
            }

            return topicList;
        }

        //Returns comments
        public async Task<List<Comments.CommentsObject>> GetComments(string currentSubreddit, string topicID)
        {
            string commentUrl = "http://www.reddit.com/r/" + currentSubreddit  + "/comments/" + topicID + "/.json";
            List<Comments.CommentsObject> comments = await wc.GetComments(commentUrl);
            return comments;

        }
        
        public async Task<List<Subreddit.Data>> GetSubredditList(string url, string [] defaultSubreddits)
        {
            List<Subreddit.Data> subredditList = new List<Subreddit.Data>();

            for (int j = 0; j < defaultSubreddits.Length; j++)
            {
                string subredditUrl = "http://www.reddit.com/" + subredditList[j] + "/about.json";

                if (subredditUrl.Equals("http://www.reddit.com/r/front/about.json") || subredditUrl.Equals("http://www.reddit.com/r/all/about.json"))
                    subredditUrl = "http://reddit.com/.json";

                string jsonText = await wc.GetJsonText(url);
                Subreddit.RootObject deserializeObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Subreddit.RootObject>(jsonText);
                subredditList.Add(await GetSubreddit(url, j));
            }

            return subredditList;
        }

        //Returns a single Subreddit
        public async Task<Subreddit.Data> GetSubreddit(string url, int index)
        {
            string jsonText = await wc.GetJsonText(url);
            Subreddit.RootObject deserializeObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Subreddit.RootObject>(jsonText);
            Subreddit.Data topic = new Subreddit.Data();

            return topic;
        }

        //Gets the hours ago the topic was posted
        public static double GetHoursDifference(double unixTimestamp)
        {
            long numX = Convert.ToInt64(unixTimestamp);
            DateTime test = UnixTimeStampToDateTime(numX);
            DateTime currentTime = DateTime.Now;

            TimeSpan difference = currentTime - test;
            double hours = Math.Round(difference.TotalHours);

            return hours;
        }

        //Converts UnixTimestamp to a Datetime object
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        { 
            //Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        //Returns list of default subreddits
        public string [] GetDefaultSubreddits()
        {
            return defaultSubreddits;
        }
    }
}
