using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrackTimeSpendInProjectAndroidAapp.Models
{
    public class WorkTask
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }
    }

    public enum Status
    {
        Done, Doing, New
    }
}
