using System;
using System.Collections.Generic;

namespace GhostCrawler
{
    public class WebElement
    {
        private readonly Action<string> _setter;
        private Func<string, WebElement> _onSelector;
        private Func<string, IEnumerable<WebElement>> _onAllSelector;

        public WebElement(Action<string> setter)
        {
            _setter = setter;
            Key = Guid.NewGuid();
        }

        #region properties..

        internal object InternalDom { get; set; }

        public Guid Key { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public string InnerHtml { get; set; }

        public string OuterHtml { get; set; }

        public string Text { get; set; }

        public string TagName { get; set; }

        public string XPath { get; set; }

        #endregion

        public void OnQuerySelector(Func<string, WebElement> query)
        {
            _onSelector = query;
        }
        public void OnQuerySelectorAll(Func<string, IEnumerable<WebElement>> query)
        {
            _onAllSelector = query;
        }

        public WebElement GetElementByQuery(string query)
        {
            return _onSelector(query);
        }

        public IEnumerable<WebElement> GetElementsByQuery(string query)
        {
            return _onAllSelector(query);
        }

        public string GetStringValue()
        {
            if (TagName.ToLower() == "input")
                return Attributes["value"] ?? string.Empty;

            if (TagName.ToLower() == "select")
                return string.Empty;

            return Text.Trim();
        }

        public void SendKey(string value)
        {
            _setter(value);
        }
    }
}
