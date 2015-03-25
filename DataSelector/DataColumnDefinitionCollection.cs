using System.Collections.Generic;

namespace GhostCrawler.DataSelector
{
    public class DataColumnDefinitionCollection : IEnumerable<DataColumnDefinition>
    {
        private readonly List<DataColumnDefinition> _rows = new List<DataColumnDefinition>();

        public DataColumnDefinitionCollection()
        {

        }

        public void Add(string header, string cell, bool enableCellSelector)
        {
            _rows.Add(new DataColumnDefinition(header, false, string.Empty, cell, enableCellSelector, string.Empty));
        }

        public void Add(string header, string cell, bool enableCellSelector, string cellScope)
        {
            _rows.Add(new DataColumnDefinition(header, false, string.Empty, cell, enableCellSelector, cellScope));
        }

        public IEnumerator<DataColumnDefinition> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
