using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PoVWebsite.Models
{
    public class Token
    {
        public int id { get; set; }

        public int user_id { get; set; }

        public string public_key { get; set; }

        public string auth_token { get; set; }

        //public Nullable<DateTime> expires { get; set; }
        public System.DateTime expires { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}