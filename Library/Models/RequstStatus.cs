using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models
{
    public class RequestStatus
    {
        public bool Status { get; set; }
        public string Msg { get; set; }

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
