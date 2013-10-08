using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PoVWebsite.Models
{
    public partial class PushContact
    {
        public int id { get; set; }

        public int user_id { get; set; }

        public string phone_uri { get; set; }

        public string phone { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }




    }
}