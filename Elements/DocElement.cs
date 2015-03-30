using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostCrawler
{
    public class DocElement : WebElement
    {
        private readonly IEnumerable<FormElement> _forms; 

        public DocElement(Action<string> setter)
            : base(setter)
        {

        }

        internal DocElement(Action<string> setter, IEnumerable<FormElement> elements)
            : base(setter)
        {
            _forms = elements;
        }

        public string Domain { get; set; }

        public string Title { get; set; }

        public IEnumerable<FormElement> Forms()
        {
            return _forms;
        }
    }
}
