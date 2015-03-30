using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GhostCrawler.Scripting
{
    public interface IJsEngine : IDisposable
    {
        void CreateEngine();
        string Run(string html, CookieContainer cookies);
    }
}
