using Newtonsoft.Json;

namespace TrackTimeSpendInProjectAndroidAapp.Models
{
    public class RequestStatus
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        public RequestStatus() { }

        public RequestStatus(bool Status)
        {
            this.Status = Status;
            Msg = string.Empty;
        }

        public RequestStatus(string Msg, bool Status)
        {
            this.Msg = Msg;
            this.Status = Status;
        }
    }
}
