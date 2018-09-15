using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models
{
    public class WorkTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
    }

    public enum Status
    {
        Done, Doing, New
    }
}
