using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostCrawler.Parsers
{
    internal static class ScriptingParser
    {
        public static string Execute(string html, IParserProvider parser, Uri page)
        {
            if (GhostConfiguration.ScriptEngine != null)
            {
                var config = new Configuration();
                var internalparse = new DocumentBuilder(config);
                var document = internalparse.FromHtml(html);
                var scripts = document.Scripts.Where(x => x.Attributes.Any(i => i.Name == "src"));
                foreach (var script in scripts)
                {
                    var url = script.Attributes.FirstOrDefault(x => x.Name == "src").Value;

                    if (!(url.StartsWith("http:") | url.StartsWith("https:")))
                    {
                        if (url.StartsWith("/"))
                            url = String.Concat(page.AbsoluteUri.Replace(page.AbsolutePath, ""), url);
                        else
                            url = String.Concat(page.AbsoluteUri, "/../", url);
                    }
                    script.SetAttribute("src", url);
                }

                var htmlParsed = document.ToHtml();
                return GhostConfiguration.ScriptEngine.Run(htmlParsed);
            }
            return html;

        }
    }
}
