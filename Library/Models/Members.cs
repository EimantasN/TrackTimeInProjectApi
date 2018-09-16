using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Members
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long TotalTime { get; set; }
        public long CurrentDayTime { get; set; }

        public string ImgUrl { get; set; }
        public bool Active { get; set; }

        public List<Time> Times { get; set; }
        public List<WorkTask> Member_Tasks { get; set; }
    }
}
