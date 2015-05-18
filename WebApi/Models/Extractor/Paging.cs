using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Extractor
{
    public class Paging
    {
        public string TotalRecordsCssElement { get; set; }
        public string NextPageCssSelector { get; set; }
//        public int NextPageElementIndex { get; set; }
        public int RecordsPerPage { get; set; }
        public Func<IWebElement, int> ExtractTotalRecords { get; set; }
       

        //public int Pages { get { return Function(TotalRecordsCssElement) / RecordsPerPage; } }
    }
}