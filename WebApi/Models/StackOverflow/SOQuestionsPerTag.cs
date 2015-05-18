using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.StackOverflow
{
    public class SOQuestionsPerTag
    {
        public string Tag { get; set; }
        public List<string> Questions { get; set; }
        public List<string> Links { get; set; }

        public SOQuestionsPerTag()
        {
            Questions = new List<string>();
            Links = new List<string>();
        }
    }
}