using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace TrackTimeSpendInProjectAndroidAapp.Models
{
    public class Time
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("start")]
        public DateTime Start { get; set; }

        [JsonProperty("end")]
        public DateTime End { get; set; }

        [JsonProperty("diff")]
        public long Diff { get; set; }
    }
}
