using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuicyReddit
{
    class User
    {
        public class Data
        {
            public bool has_mail { get; set; }
            public string name { get; set; }
            public bool is_friend { get; set; }
            public double created { get; set; }
            public string modhash { get; set; }
            public double created_utc { get; set; }
            public int link_karma { get; set; }
            public int comment_karma { get; set; }
            public bool over_18 { get; set; }
            public bool is_gold { get; set; }
            public bool is_mod { get; set; }
            public bool has_verified_email { get; set; }
            public string id { get; set; }
            public bool has_mod_mail { get; set; }
        }

        public class RootObject
        {
            public string kind { get; set; }
            public Data data { get; set; }
        }
    }
}
