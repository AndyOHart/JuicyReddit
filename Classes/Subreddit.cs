using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuicyReddit
{
    class Subreddit
    {
        public class Data
        {
            public string submit_text_html { get; set; }
            public object user_is_banned { get; set; }
            public string id { get; set; }
            public string submit_text { get; set; }
            public string display_name { get; set; }
            public string header_img { get; set; }
            public string description_html { get; set; }
            public string title { get; set; }
            public bool over18 { get; set; }
            public object user_is_moderator { get; set; }
            public string header_title { get; set; }
            public string description { get; set; }
            public string submit_link_label { get; set; }
            public int accounts_active { get; set; }
            public bool public_traffic { get; set; }
            public List<int> header_size { get; set; }
            public int subscribers { get; set; }
            public object submit_text_label { get; set; }
            public string name { get; set; }
            public double created { get; set; }
            public string url { get; set; }
            public double created_utc { get; set; }
            public object user_is_contributor { get; set; }
            public string public_description { get; set; }
            public int comment_score_hide_mins { get; set; }
            public string subreddit_type { get; set; }
            public string submission_type { get; set; }
            public object user_is_subscriber { get; set; }
        }

        public class Info
        {
            public string name { get; set; }
            public string description { get; set; }
        }

        public class RootObject
        {
            public string kind { get; set; }
            public Data data { get; set; }
        }
    }
}
