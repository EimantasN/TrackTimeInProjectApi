using System;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Time
    {
        [Key]
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeSpan Diff { get; set; }
    }
}
