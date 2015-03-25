using GhostCrawler.Parsers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostCrawler
{
    public class StandAloneNavigationHandle
    {
        private readonly XWebCrawler _browser;
        private readonly WebClientExtended _webClient;

        public StandAloneNavigationHandle(XWebCrawler browser, WebClientExtended webClient)
        {
            _browser = browser;
            _webClient = webClient;
        }

        #region asynchronous operations

        public Task<byte[]> DownloadFileAsync(Uri url)
        {
            return _webClient.DownloadDataTaskAsync(url);
        }

        public Task<WebElement> PostAsync(NameValueCollection parameters)
        {
            return PostToUrlAsync(_browser.Page, parameters);
        }

        public Task<WebElement> PostAsync(string parameters)
        {
            return PostToUrlAsync(_browser.Page, parameters);
        }

        public Task<WebElement> PostToUrlAsync(Uri url, string parameters)
        {
            return Task.Factory.StartNew(() =>
            {
                var html = _webClient.UploadString(url, "POST", parameters);                
                return ParseDocument(html, url);
            });
        }

        public Task<WebElement> PostToUrlAsync(Uri url, NameValueCollection parameters)
        {
            var result = Task.Factory.StartNew(() =>
            {
                var htmlArray = _webClient.UploadValues(url, "POST", parameters);
                var html = _browser.HtmlEncoding.GetString(htmlArray);
                return ParseDocument(html, url);
            });
            return result;
        }

        #endregion

        #region synchronous operations

        public byte[] DownloadFile(string url)
        {
            return _webClient.DownloadData(url);
        }

        public WebElement Post(NameValueCollection parameters)
        {
            return PostToUrl(_browser.Page, parameters);
        }

        public WebElement Post(string parameters)
        {
            return PostToUrl(_browser.Page, parameters);
        }

        public WebElement PostToUrl(Uri url, string parameters)
        {
            var html = _webClient.UploadString(url, "POST", parameters);
            return ParseDocument(html, url);
        }

        public WebElement PostToUrl(Uri url, NameValueCollection parameters)
        {
            var htmlArray = _webClient.UploadValues(url, "POST", parameters);
            var html = _browser.HtmlEncoding.GetString(htmlArray);
            return ParseDocument(html, url);
        }

        #endregion

        private WebElement ParseDocument(string html, Uri page)
        {
            var parser = _browser.GetParser();
            var htmlWithScripts = ScriptingParser.Execute(html, parser, page);
            parser.Load(htmlWithScripts);
            return parser.Document;
        }
    }
}
