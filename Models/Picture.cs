using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PoVWebsite.Models
{
    public partial class Picture
    {
        public int id { get; set; }

        public byte[] bytes { get; set; }

        public byte[] half_size { get; set; }

        public int user_id { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }




    }
}