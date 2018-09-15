
using Newtonsoft.Json;
using System;

namespace TrackTimeSpendInProjectAndroidAapp.Models
{
    public partial class MemberModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("totalTime")]
        public TimeSpan TotalTime { get; set; }

        [JsonProperty("imgUrl")]
        public string ImgUrl { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("times")]
        public object Times { get; set; }

        [JsonProperty("member_Tasks")]
        public object MemberTasks { get; set; }
    }
}