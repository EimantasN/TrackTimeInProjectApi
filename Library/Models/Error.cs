using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Library.Models
{
    public class Error
    {
        [Key]
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastAccrued { get; set; }
        public string Message { get; set; }
        public string InnerMessage { get; set; }
        public string MethodName { get; set; }
        public string Parameters { get; set; }
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
