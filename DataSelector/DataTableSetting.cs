namespace GhostCrawler.DataSelector
{
    public class DataTableSetting
    {
        public DataTableSetting()
            : this(new DataColumnDefinitionCollection())
        {

        }

        public DataTableSetting(DataColumnDefinitionCollection columnDefinitions)
        {
            ColumnDefinitions = columnDefinitions;
            MultipleRecords = false;
        }

        public bool MultipleRecords { get; set; }
        public string Scope { get; set; }
        private string _repeatablePattern;

        public string RepeatablePattern
        {
            get { return _repeatablePattern; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _repeatablePattern = value;
                    MultipleRecords = true;
                }
            }
        }
        public DataColumnDefinitionCollection ColumnDefinitions { get; set; }
    }
}
