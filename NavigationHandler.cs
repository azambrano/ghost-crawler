using GhostCrawler.Parsers;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GhostCrawler
{
    public class NavigationHandle
    {
        private readonly XWebCrawler _browser;
        private readonly WebClientExtended _webClient;

        public NavigationHandle(XWebCrawler browser, WebClientExtended webClient)
        {
            _browser = browser;
            _webClient = webClient;
        }

        public void GoToUrl(Uri url)
        {
            var html = _webClient.DownloadString(url);
            ProcessResult(html);
        }

        public byte[] DownloadFile(string url)
        {
            return _webClient.DownloadData(url);
        }

        public string ExternalPostToUrl(Uri url, NameValueCollection parameters)
        {
            var htmlArray = _webClient.UploadValues(url, "POST", parameters);
            return _browser.HtmlEncoding.GetString(htmlArray);
        }

        public void Post(NameValueCollection parameters)
        {
            PostToUrl(_browser.Page, parameters);
        }

        public void Post(string parameters)
        {
            PostToUrl(_browser.Page, parameters);
        }

        public void PostToUrl(Uri url, string parameters)
        {
            var html = _webClient.UploadString(url, "POST", parameters);
            ProcessResult(html);
        }

        public void PostToUrl(Uri url, NameValueCollection parameters)
        {
            var htmlArray = _webClient.UploadValues(url, "POST", parameters);
            var html = _browser.HtmlEncoding.GetString(htmlArray);
            ProcessResult(html);
        }

        public event EventHandler<string> OnFinishedNavigation;
        private void ProcessResult(string html)
        {
            var handler = OnFinishedNavigation;
            if (handler != null)
                handler(this, html);
        }      
    }
}
