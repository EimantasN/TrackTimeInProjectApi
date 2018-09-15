using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace TrackTimeSpendInProjectAndroidAapp.Models
{
    public class Error
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("lastAccured")]
        public DateTime LastAccrued { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("innerMessage")]
        public string InnerMessage { get; set; }

        [JsonProperty("methodName")]
        public string MethodName { get; set; }

        [JsonProperty("parameters")]
        public string Parameters { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        public Error() { }

        public Error(Exception e, string MethodProperties = "")
        {
            Created = DateTime.Now;
            LastAccrued = DateTime.Now;
            Message = e.Message;
            if (!string.IsNullOrEmpty(e.StackTrace))
            {
                InnerMessage = e.StackTrace;
            }
            else
            {
                InnerMessage = string.Empty;
            }
            MethodName = GetMethodName(e);
            Parameters = GetMethodParameters(e, MethodProperties);
            this.Count = 0;
        }

        public string GetMethodParameters(Exception e, string MethodProperties)
        {
            return MethodProperties;
        }

        public string GetMethodName(Exception e)
        {
            try
            {
                var Name = new StackTrace(e).GetFrame(0).GetMethod().Name;
                return Name;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void Increase() { Count++; }
    }
}
