using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebApi.Models.YadVashem;
using WebApi.Models.StackOverflow;
using NReco.PhantomJS;
using System.Windows.Forms;
using Awesomium.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebApi.Models;
using WebApi.Models.Extractor;

namespace WebApi.Controllers
{
    [AllowAnonymous]
    public class ScrapingController : ApiController
    {
        private string jsData;

        [HttpGet]
        [Route("api/GetQuestions")]
        public dynamic GetQuestions()
        {
            HtmlNode divUserQuestions = GetStartingHtmlNode("http://stackoverflow.com/users/1219280/veverke?tab=questions", "user-tab-questions");

            if (divUserQuestions != null)
            {
                List<HtmlNode> questionsSummary = divUserQuestions.CssSelect(".question-summary .summary").ToList();
                List<SOQuestion> originalData = new List<SOQuestion>();

                foreach (HtmlNode questionSummary in questionsSummary)
                {
                    originalData.Add(new SOQuestion
                    {
                        Text = questionSummary.CssSelect("h3").FirstOrDefault().InnerText,
                        Link = questionsSummary.CssSelect(".question-hyperlink").FirstOrDefault().GetAttributeValue("href"),
                        Tags = ExtractTags(questionSummary.CssSelect(".tags").FirstOrDefault())
                    });
                }

                List<string> tags = new List<string>(originalData.SelectMany(q => q.Tags).Distinct());
                List<SOQuestionsPerTag> questionsPerTagResult = new List<SOQuestionsPerTag>();

                foreach (string tag in tags)
                {
                    SOQuestionsPerTag questionsPerTag = new SOQuestionsPerTag();

                    questionsPerTag.Tag = tag;
                    foreach (SOQuestion question in originalData)
                    {
                        if (question.Tags != null && question.Tags.Contains(tag))
                        {
                            questionsPerTag.Questions.Add(question.Text);
                            questionsPerTag.Links.Add("http://www.stackoverflow.com/" + question.Link);
                        }
                    }

                    questionsPerTagResult.Add(questionsPerTag);
                }

                return new { autocompleteSrc = tags, questionsPerTag = questionsPerTagResult };

            }
            else
                return new { error = "could not get starting html node." };


        }

        [HttpGet]
        [Route("api/GetYadVashemRecords")]
        public dynamic GetYadVashemRecords()
        {
            Extractor extractor =
                new Extractor
                {
                    Uri = "http://db.yadvashem.org/names/nameResults.html?place=Secureni&placeType=THESAURUS&language=en",
                    RecordCssSelector = "#name_results .victim_status_unkown",
                    FieldSelectors = new Dictionary<string, string> 
                                    { 
                                         { "Name", "td:nth-child(1)" },
                                         { "Birth   ", "td:nth-child(2)" },
                                         { "Residence", "td:nth-child(3)" },
                                         { "Source", "td:nth-child(4)" },
                                         { "FateAccordingToThisSource", "td:nth-child(5)" },
                                    },
                    //BrowserActions = new Dictionary<BrowserActions, string>
                    //                {
                    //                    { BrowserActions.Paging, "img[src='next_ltr.png']" } 
                    //                },
                    Paging = new Paging
                    {
                        TotalRecordsCssElement = ".results_count",
                        NextPageCssSelector = ".pages_nav a",// "img[src='next_ltr.png']",
                        ExtractTotalRecords = (selector) => { var x = selector.Text.Split().ToList(); return Convert.ToInt32(x[x.Count - 3]); },
                        RecordsPerPage = 50
                    }
                };

            return extractor.Extract();
        }

        [HttpGet]
        [Route("api/GetAncestryRecords")]
        public dynamic GetAncestryRecords(int page = -1, int recordsPerPage = int.MaxValue, int maxmimumRecords = int.MaxValue)
        {
            Extractor extractor =
                new Extractor
                {
                    Uri = "http://search.ancestry.com/cgi-bin/sse.dll?gl=allgs&gss=sfs28_ms_f-2_s&new=1&rank=1&msT=1&mswpn__ftp=Sokiryany%2C%20Chernivtsi%2C%20Ukraine&mswpn=1582259&mswpn_PInfo=8-%7C0%7C1652381%7C0%7C5233%7C0%7C31442%7C0%7C0%7C1582259%7C0%7C&MSAV=1&cpxt=1&cp=101&catbucket=rstp&uidh=000",
                    RecordCssSelector = "#gsresults .tblrow.record",
                    FieldSelectors = new Dictionary<string, string> 
                                                { 
                                                     { "SourceDB", ".srchFoundDB" },
                                                     { "Catalog   ", ".srchFoundCat" },
                                                     { "Residence", "td:nth-child(3)" },
                                                     { "Source", "td:nth-child(4)" },
                                                     { "FateAccordingToThisSource", "td:nth-child(5)" },
                                                },
                    //BrowserActions = new Dictionary<BrowserActions, string>
                    //                {
                    //                    { BrowserActions.Paging, "img[src='next_ltr.png']" } 
                    //                },
                    Paging = new Paging
                    {
                        TotalRecordsCssElement = ".results_count",
                        NextPageCssSelector = ".pages_nav a",// "img[src='next_ltr.png']",
                        ExtractTotalRecords = (selector) => { var x = selector.Text.Split().ToList(); return Convert.ToInt32(x[x.Count - 3]); },
                        RecordsPerPage = 50
                    }
                };

            return extractor.Extract();

            //string response = GetResponseAsString("http://search.ancestry.com/cgi-bin/sse.dll?gl=allgs&gss=sfs28_ms_f-2_s&new=1&rank=1&msT=1&mswpn__ftp=Secureni&MSAV=1&cpxt=1&cp=101&catbucket=rstp&uidh=000");

            ////IWebDriver driver = new FirefoxDriver();
            //IWebDriver driver = new ChromeDriver();

            ////Notice navigation is slightly different than the Java version
            ////This is because 'get' is a keyword in C#
            //driver.Navigate().GoToUrl("http://search.ancestry.com/cgi-bin/sse.dll?gss=angs-g&new=1&rank=1&msT=1&mswpn__ftp=Secureni%2c+Chernivtsi%2c+Ukraine&mswpn=1582259&mswpn_PInfo=8-%7c0%7c1652381%7c0%7c5233%7c0%7c31442%7c0%7c0%7c1582259%7c0%7c&MSAV=1&cpxt=1&cp=101&catbucket=rstp&uidh=000&gl=allgs&gst=&ghc=50");

            //IJavaScriptExecutor js = driver as IJavaScriptExecutor;

            //List<dynamic> results = new List<dynamic>();
            //IEnumerable<IWebElement> records = driver.FindElements(By.CssSelector(".tblrow.record"));
            //foreach(IWebElement record in records)
            //{
            //    IWebElement sourceDB = record.FindElement(By.CssSelector(".srchFoundDB"));
            //    results.Add(new { sourceDB = sourceDB.Text });
            //}

            //driver.Quit();

            //return Response<dynamic>.Success(results);

            // Find the text input element by its name
            //IWebElement query = driver.FindElement(By.Name("q"));

            //// Enter something to search for
            //query.SendKeys("Cheese");

            //// Now submit the form. WebDriver will find the form for us from the element
            //query.Submit();

            //// Google's search is rendered dynamically with JavaScript.
            //// Wait for the page to load, timeout after 10 seconds
            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            //wait.Until((d) => { return d.Title.ToLower().StartsWith("cheese"); });

            // Should see: "Cheese - Google Search"
            //System.Console.WriteLine("Page title is: " + driver.Title);
        }

        void webView_DocumentReady(object sender, DocumentReadyEventArgs e)
        {
            throw new NotImplementedException();
        }

        void phantomJS_OutputReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            jsData = e.Data;
            List<YadVashemRecord> records = GetYadVashemRecords();
        }

        private HtmlNode GetStartingHtmlNode(string uri, string cssSelector)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(GetResponseAsString(uri));
            return doc.GetElementbyId(cssSelector);
        }

        private string GetResponseAsString(string uri)
        {
            /* does not work */
            //HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Origin", "http://devrecipeshb.blogspot.co.il/*");
            HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            HttpClient httpClient = new HttpClient();
            return httpClient.GetStringAsync(uri).Result;
        }

        private List<string> ExtractTags(HtmlNode tagsDiv)
        {
            return tagsDiv.InnerText.Trim().Split().ToList();
        }

        //private dynamic GetYadVashemRecords()
        //{
        //    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        //    doc.LoadHtml(jsData);
        //    HtmlNode divGrid = doc.GetElementbyId("#name_results");
        //    //("[class^='victim_status']");
        //    List<YadVashemRecord> responseRecords = new List<YadVashemRecord>();
        //    IEnumerable<HtmlNode> records = divGrid.CssSelect(".victim_status_unkown");
        //    foreach (HtmlNode record in records)
        //    {
        //        YadVashemRecord responseRecord = new YadVashemRecord();
        //        IEnumerable<HtmlNode> tds = record.CssSelect("td");

        //        responseRecord.Name = tds.ElementAt(0).InnerText;
        //        responseRecord.Birth = tds.ElementAt(1).InnerText;
        //        responseRecord.Residence = tds.ElementAt(2).InnerText;
        //        responseRecord.Source = tds.ElementAt(3).InnerText;
        //        responseRecord.FateBasedOnThisSource = tds.ElementAt(4).InnerText;
        //    }

        //    return responseRecords;
        //}
    }
}
