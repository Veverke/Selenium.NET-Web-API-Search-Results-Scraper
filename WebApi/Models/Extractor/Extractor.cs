using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Text;

namespace WebApi.Models.Extractor
{
    public class Extractor
    {
        public string Uri { get; set; }
        public string RecordCssSelector { get; set; }
        public Dictionary<string, string> FieldSelectors { get; set; }
        public Paging Paging { get; set; }
        public Dictionary<string, string> Inputs { get; set; }
        public Dictionary<BrowserActions, string> BrowserActions { get; set; }

        public Extractor()
        {

        }

        //public Extractor(string uri, string recordSelector, Dictionary<string, string> fieldSelectors, Dictionary<BrowserActions, string> browserActions)
        //{
        //    Uri = uri;
        //    RecordCssSelector = recordSelector;
        //    FieldSelectors = fieldSelectors;
        //    BrowserActions = browserActions;

        //    Inputs = new Dictionary<string, string>();

        //}

        //works for chrome only, for now
        public List<dynamic> Extract(bool SaveToFile = false)
        {
//            Stopwatch watch = new Stopwatch();
            //IWebDriver driver = new FirefoxDriver();
            IWebDriver webDriver = new ChromeDriver();

            webDriver.Navigate().GoToUrl(Uri);

            int totalRecords = Paging.ExtractTotalRecords(webDriver.FindElement(By.CssSelector(Paging.TotalRecordsCssElement)));
            List<dynamic> results = new List<dynamic>();

            for (int page = 1; page <= totalRecords / Paging.RecordsPerPage ; page++)
            {
                IEnumerable<IWebElement> records = webDriver.FindElements(By.CssSelector(RecordCssSelector));
                foreach (IWebElement record in records)
                {
                    ExpandoObject resultRecord = new ExpandoObject();
                    foreach (KeyValuePair<string, string> fieldSelector in FieldSelectors)
                    {
                        IDictionary<string, object> dicRecord = resultRecord as IDictionary<string, object>;
                        dicRecord.Add(fieldSelector.Key, record.FindElement(By.CssSelector(fieldSelector.Value)).Text);
                    }

                    results.Add(resultRecord);
                }

                List<IWebElement> pageElements = webDriver.FindElements(By.CssSelector(Paging.NextPageCssSelector)).ToList();
                pageElements[pageElements.Count - 2].Click();
                                
                //WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));
                //wait.Until((d) => { return d.Title.ToLower().StartsWith("cheese"); });

                //if (SaveToFile)
                //    File.AppendAllLines(string.Format("C:/Users/avraham.kahana/Desktop/Extractor_{0}_{1}", DateTime.Today, watch.Elapsed.TotalMinutes), results.Select<dynamic, string>(record => MakeCsvRecord(record)));

            }


            //foreach (KeyValuePair<BrowserActions, string> browserAction in BrowserActions.Where(action => action.Key == Models.Extractor.BrowserActions.Paging))
            //{

            //}



            webDriver.Quit();

            return results;
        }

        private string MakeCsvRecord(ExpandoObject obj)
        {
            //StringBuilder result = new StringBuilder();
            IDictionary<string, object> dic = obj as IDictionary<string, object>;
            //foreach(KeyValuePair<string, object> keyValuePair in dic)
            //{
            //    result.AppendFormat("{0}{1}", keyValuePair.Value.ToString(), System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator);
            //}
            ////result.Remove()
            return string.Join(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator, dic.Select(element => element.Value));
        }
    }

    public enum BrowserActions
    {
        Search = 0,
        SearchWithSubmit,
        //Paging,
        Click
    }
}