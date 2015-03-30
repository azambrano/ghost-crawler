using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GhostCrawler.Parsers
{
    internal static class ScriptingParser
    {
        public static string Execute(string html, IParserProvider parser, Uri page, CookieContainer cookies)
        {
            if (GhostConfiguration.ScriptEngine != null)
            {
                var config = new Configuration();
                var internalparse = new DocumentBuilder(config);
                var document = internalparse.FromHtml(html);

                //var scriptInitialize = document.CreateElement("script");
                //scriptInitialize.TextContent = string.Format(@"document.location.href='{0}';", page.OriginalString);
                //document.Head.Append(scriptInitialize);                

                var scripts = document.Scripts.Where(x => x.Attributes.Any(i => i.Name == "src")).ToList();
                var styles = document.QuerySelectorAll("link[href]").Cast<IElement>().ToList();
                var iframes = document.QuerySelectorAll("iframe[src]").Cast<IElement>().ToList();
                foreach (var script in scripts)
                    ResolveUrls(page, script, "src");

                foreach (var script in styles)
                    ResolveUrls(page, script, "href");

                foreach (var script in iframes)
                    ResolveUrls(page, script, "src");

                var htmlParsed = document.ToHtml();
                return GhostConfiguration.ScriptEngine.Run(htmlParsed, cookies);
            }
            return html;

        }

        public static void ResolveUrls(Uri currentPage, IElement item, string attribute)
        {
            var url = item.Attributes.FirstOrDefault(x => x.Name == attribute).Value;
            if (!(url.StartsWith("http:") | url.StartsWith("https:")))
            {
                if (url.StartsWith("/"))
                {
                    var part = currentPage.PathAndQuery == "/" ? "" : currentPage.PathAndQuery;
                    if (!string.IsNullOrWhiteSpace(part))
                        url = String.Concat(currentPage.AbsoluteUri.Replace(part, "").TrimEnd('/'), url);
                    else
                        url = String.Concat(currentPage.AbsoluteUri.TrimEnd('/'), url);
                }
                else
                    url = String.Concat(currentPage.AbsoluteUri, "/../", url);
            }
            item.SetAttribute(attribute, url);
        }
    }
}

