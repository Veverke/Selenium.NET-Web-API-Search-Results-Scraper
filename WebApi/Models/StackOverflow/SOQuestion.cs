using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.StackOverflow
{
    public class SOQuestion
    {
        public string Text { get; set; }
        public string Link { get; set; }
        public List<string> Tags { get; set; }
    }
}