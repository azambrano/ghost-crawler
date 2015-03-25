using System.Collections.Generic;
using System.Linq;
using GhostCrawler.Scripting;
using AngleSharp;
using AngleSharp.Dom;
using System;
using System.IO;

namespace GhostCrawler.Parsers
{
    public class AngleSharpProvider : IParserProvider
    {
        private Object thisLock = new Object();
        private readonly DocumentBuilder _builder;
        private IDocument _document;

        public AngleSharpProvider()
        {
            var config = new Configuration();
            _builder = new DocumentBuilder(config);
        }

        public void Load(string html)
        {         
            _document = _builder.FromHtml(html);
            Document = Map(_document.DocumentElement);
        }

        public WebElement GetElementById(string value)
        {
            var s = "#" + value;
            var node = _document.DocumentElement.QuerySelector(s);
            return Map(node);
        }

        public WebElement GetElementByName(string name)
        {
            var s = "input[name='" + name + "']";
            var node = _document.DocumentElement.QuerySelector(s);
            return Map(node);
        }

        public WebElement GetElementByQuery(string query)
        {
            var node = _document.DocumentElement.QuerySelector(query);
            return Map(node);
        }

        public IEnumerable<WebElement> GetElementsByQuery(string query)
        {
            var nodes = _document.DocumentElement.QuerySelectorAll(query);
            return nodes.Select(Map);
        }

        private static WebElement Map(IElement node)
        {
            if (node == null)
                return null;

            var el = new WebElement(value => node.SetAttribute("value", value))
            {
                Attributes = node.Attributes.ToDictionary(x => x.Name, y => y.Value),
                TagName = node.TagName,
                Text = node.TextContent,
                InnerHtml = node.InnerHtml,
                OuterHtml = node.OuterHtml               
            };

            el.OnQuerySelector(query => Map(node.QuerySelector(query)));
            el.OnQuerySelectorAll(query => node.QuerySelectorAll(query).Select(Map));

            return el;
        }

        public WebElement Document { get; private set; }
    }
}
