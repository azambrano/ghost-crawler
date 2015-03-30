using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostCrawler
{
    public class FormElement : WebElement
    {
        public FormElement(Action<string> setter)
            : base(setter)
        {
            
        }

        public IEnumerable<WebElement> GetDataPointElements()
        {
            var elements = GetElementsByQuery("input,select");
            return elements;
        }
    }
}
