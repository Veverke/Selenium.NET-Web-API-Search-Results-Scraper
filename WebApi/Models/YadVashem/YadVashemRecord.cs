using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.YadVashem
{
    public class YadVashemRecord
    {
        public String Name { get; set; }
        public String Birth { get; set; }
        public String Residence { get; set; }
        public String Source { get; set; }
        public String FateBasedOnThisSource { get; set; }
    }
}