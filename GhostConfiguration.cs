using GhostCrawler.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostCrawler
{
    public static class GhostConfiguration
    {
        private static IJsEngine _scriptEngine;
        public static IJsEngine ScriptEngine { get { return _scriptEngine; } }

        public static void Register(IJsEngine engine)
        {
            if (engine == null)
                throw new NullReferenceException();

            _scriptEngine = engine;           
        }
    }
}
