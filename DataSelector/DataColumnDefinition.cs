using System;

namespace GhostCrawler.DataSelector
{
    public class DataColumnDefinition
    {
        public DataColumnDefinition()
        {
                
        }

        public DataColumnDefinition(string header, bool enableHeaderSelector, string headerScope, string cell, bool enableCellSelector, string cellScope)
        {
            Key = Guid.NewGuid();

            Header = header;
            HeaderScope = headerScope;
            IsHeaderSelector = enableHeaderSelector;
            
            Cell = cell;
            CellScope = cellScope;
            IsCellSelector = enableCellSelector;
        }

        public Guid Key { get; set; }

        public string Header { get; set; }
        public string HeaderScope { get; set; }
        public bool IsHeaderSelector { get; set; }

        public string Cell { get; set; }
        public string CellScope { get; set; }
        public bool IsCellSelector { get; set; }
    }
}
