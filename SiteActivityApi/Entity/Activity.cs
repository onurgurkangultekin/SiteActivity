using System;
using System.ComponentModel.DataAnnotations;

namespace SiteActivityApi.Entity
{
    public class Activity
    {
        public string Key { get; set; }
        
        [Required]
        public double Value { get; set; }
        
        public DateTime Time { get; set; }
    }
}
