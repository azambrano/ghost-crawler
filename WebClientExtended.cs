using System;
using System.Net;

namespace GhostCrawler
{
    public class WebClientExtended : WebClient
    {
        private CookieContainer _container = new CookieContainer();

        public WebClientExtended(CookieContainer container)
        {
            _container = container;
        }

        public CookieContainer CookieContainer
        {
            get { return _container; }
            set { _container = value; }
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var r = base.GetWebRequest(address);
            var request = r as HttpWebRequest;
            if (request != null)
                request.CookieContainer = _container;
            return r;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            var response = base.GetWebResponse(request, result);
            ReadCookies(response);
            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = base.GetWebResponse(request);
            ResponseUri = response.ResponseUri;
            ReadCookies(response);
            return response;
        }

        public Uri ResponseUri { get; set; }

        private void ReadCookies(WebResponse r)
        {
            var response = r as HttpWebResponse;
            if (response != null)
            {
                var cookies = response.Cookies;
                _container.Add(cookies);
            }
        }
    }
}
