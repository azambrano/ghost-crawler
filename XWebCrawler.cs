using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using GhostCrawler.DataSelector;
using GhostCrawler.Parsers;

namespace GhostCrawler
{
    public class XWebCrawler : IDisposable
    {
        private WebClientExtended _webClient;
        private NavigationHandle _navigation;

        public XWebCrawler()
        {
            _webClient = new WebClientExtended(new CookieContainer());
            Parser = GetParser();
            Initialize(_webClient);

            if (GhostConfiguration.ScriptEngine != null)
                GhostConfiguration.ScriptEngine.CreateEngine();
        }

        public bool Scripting { get; set; }

        private void Initialize(WebClientExtended wc)
        {
            Headers = new WebHeaderCollection();
            UserAgent = "User-Agent	Mozilla/5.0 (Windows NT 6.3; WOW64; rv:35.0) Gecko/20100101 Firefox/35.0";
            HtmlEncoding = Encoding.UTF8;

            _navigation = new NavigationHandle(this, wc);
            _navigation.OnFinishedNavigation += (s, html) =>
            {
                Page = _webClient.ResponseUri;
                var htmlWithScripts = ScriptingParser.Execute(html, Parser, Page, _webClient.CookieContainer);
                TextDocument = htmlWithScripts;
                Parser.Load(TextDocument);
                HtmlDocument = Parser.Document;
            };
        }

        public string UserAgent { get; set; }

        public WebHeaderCollection Headers { get; set; }

        public Encoding HtmlEncoding { get; set; }

        public NavigationHandle Navigate()
        {
            if (_webClient.Headers.AllKeys.All(x => x != HttpRequestHeader.UserAgent.ToString()))
                _webClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);

            if (Headers != null && Headers.Count > 0)
                foreach (var key in Headers.AllKeys)
                    if (_webClient.Headers.AllKeys.All(x => x != key))
                        _webClient.Headers.Add(key, Headers[key]);

            return _navigation;
        }

        public StandAloneNavigationHandle StandAloneNavigate()
        {
            var wc = new WebClientExtended(_webClient.CookieContainer);

            if (wc.Headers.AllKeys.All(x => x != HttpRequestHeader.UserAgent.ToString()))
                wc.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);

            if (Headers != null && Headers.Count > 0)
                foreach (var key in Headers.AllKeys)
                    if (wc.Headers.AllKeys.All(x => x != key))
                        wc.Headers.Add(key, Headers[key]);

            return new StandAloneNavigationHandle(this, wc);
        }

        public DataSelectorManager DataSelector(WebElement document)
        {
            return new DataSelectorManager(document);
        }

        public DataSelectorManager DataSelector()
        {
            return DataSelector(HtmlDocument);
        }

        public WebElement GetElementByName(string id)
        {
            return Parser.GetElementByName(id);
        }

        public WebElement GetElementById(string id)
        {
            return Parser.GetElementById(id);
        }

        public WebElement GetElementByQuery(string value)
        {
            return Parser.GetElementByQuery(value);
        }

        public IEnumerable<WebElement> GetElementsByQuery(string value)
        {
            return Parser.GetElementsByQuery(value);
        }

        internal IParserProvider Parser { get; private set; }

        internal IParserProvider GetParser()
        {
            var p = new AngleSharpProvider();
            return p;
        }

        public Uri Page { get; set; }

        public string TextDocument { get; set; }

        public WebElement HtmlDocument { get; set; }

        public static void Save<T>(T details, string fileName)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (TextWriter writer = new StreamWriter(fileName))
            {
                serializer.Serialize(writer, details);
            }
        }

        public void Dispose()
        {
            _webClient.Dispose();
            if (GhostConfiguration.ScriptEngine != null)
                GhostConfiguration.ScriptEngine.Dispose();
        }
    }
}
