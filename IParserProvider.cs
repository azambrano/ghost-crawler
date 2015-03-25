using System;
using System.Collections.Generic;

namespace GhostCrawler
{
    public interface IParserProvider
    {    
        WebElement Document { get; }
        WebElement GetElementByName(string name);
        WebElement GetElementById(string value);
        WebElement GetElementByQuery(string query);
        IEnumerable<WebElement> GetElementsByQuery(string query);                
        void Load(string html);
    }
}
