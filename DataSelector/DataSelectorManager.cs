using System;
using System.Collections.Specialized;
using System.Data;
using System.Linq;

namespace GhostCrawler.DataSelector
{
    public class DataSelectorManager
    {
        private Object thisLock = new Object();
        private WebElement _htmlDocument;
        private readonly DataSet _dataSet = new DataSet();

        public DataSelectorManager(WebElement htmlDocument)
        {
            SetDocument(htmlDocument);
        }

		public NameValueCollection TryGetFromData(string formName = "")
        {
            var result = new NameValueCollection();
            var doc = _htmlDocument as DocElement;
            if (doc != null)
            {
                var forms = string.IsNullOrWhiteSpace(formName) ? doc.Forms().ToList() : doc.Forms().Where(x => x.Name == formName).ToList();
                foreach (var formElement in forms)
                {
                    foreach (var element in formElement.GetDataPointElements())
                        if (!string.IsNullOrWhiteSpace(element.Name))
                            result.Add(element.Name, element.GetStringValue());
                }
            }
            return result;
        }
		
        public void SetDocument(WebElement document)
        {
            _htmlDocument = document;
        }

        public void SetTable(string tableName, DataTableSetting setting)
        {
            var table = new DataTable(tableName);
            PrepareTable(table, setting);
            _dataSet.Tables.Add(table);
        }

        public void SetTable(string tableName, DataColumnDefinitionCollection rowDefinitions)
        {
            var table = new DataTable(tableName);
            PrepareTable(table, new DataTableSetting(rowDefinitions));
            _dataSet.Tables.Add(table);
        }

        private static void PrepareTable(DataTable table, DataTableSetting setting)
        {
            table.ExtendedProperties.Add("_Setting_", setting);
            table.ExtendedProperties["_HeaderIsComplete_"] = false;
            foreach (var row in setting.ColumnDefinitions)
                table.Columns.Add(new DataColumn
                {
                    ColumnName = row.Key.ToString(),
                    DataType = typeof(string)
                });
        }

        /// <summary>
        /// Extracts the data from the current page and fill the table(s).
        /// </summary>
        /// <param name="tableNames"></param>
        public void FillData(params string[] tableNames)
        {
            if (tableNames != null && tableNames.Any())
            {
                foreach (var tableName in tableNames)
                    FillTable(_dataSet.Tables[tableName]);
            }
            else
            {
                foreach (var table in _dataSet.Tables)
                    FillTable(table as DataTable);
            }
        }

        private void FillTable(DataTable dataTable)
        {
            if (dataTable != null)
            {
                var setting = dataTable.ExtendedProperties["_Setting_"] as DataTableSetting;
                if (setting != null)
                {
                    /*Header*/
                    ResolveHeader(dataTable);
                    
                    /*Values*/
                    ResolveData(dataTable);                   
                }
            }
        }

        private void ResolveHeader(DataTable dataTable)
        {
            var setting = dataTable.ExtendedProperties["_Setting_"] as DataTableSetting;
            if (setting != null)
            {
                lock (thisLock)
                {
                    if (!(bool)dataTable.ExtendedProperties["_HeaderIsComplete_"])
                    {
                        foreach (var rowDefinition in setting.ColumnDefinitions)
                        {
                            var columnData = dataTable.Columns[rowDefinition.Key.ToString()];
                            if (rowDefinition.IsHeaderSelector)
                            {
                                var el = _htmlDocument.GetElementByQuery(rowDefinition.Header);
                                if (el != null)
                                    columnData.Caption = el.GetStringValue();
                            }
                            else
                                columnData.Caption = rowDefinition.Header;
                        }
                        dataTable.ExtendedProperties["_HeaderIsComplete_"] = true;
                    }
                }
            }
        }

        private void ResolveData(DataTable dataTable)
        {
            var setting = dataTable.ExtendedProperties["_Setting_"] as DataTableSetting;
            if (setting != null)
            {
                if (!setting.MultipleRecords)
                {
                    lock (thisLock)
                    {
                        var rowData = dataTable.NewRow();
                        foreach (var rowDefinition in setting.ColumnDefinitions)
                        {
                            if (rowDefinition.IsCellSelector)
                            {
                                var elScope = FindScopeElement(rowDefinition.CellScope, _htmlDocument);
                                if (elScope != null)
                                {
                                    var el = _htmlDocument.GetElementByQuery(rowDefinition.Cell);
                                    if (el != null)
                                        rowData[rowDefinition.Key.ToString()] = el.GetStringValue();
                                }
                            }
                            else
                                rowData[rowDefinition.Key.ToString()] = rowDefinition.Cell;
                        }
                        dataTable.Rows.Add(rowData);
                    }
                }
                else
                {
                    var scope = FindScopeElement(setting.Scope, _htmlDocument);
                    if (scope != null)
                    {
                        var records = scope.GetElementsByQuery(setting.RepeatablePattern);
                        if (records != null && records.Any())
                            foreach (var webElement in records)
                            {
                                lock (thisLock)
                                {
                                    var rowData = dataTable.NewRow();
                                    foreach (var rowDefinition in setting.ColumnDefinitions)
                                    {
                                        if (rowDefinition.IsCellSelector)
                                        {
                                            var elScope = FindScopeElement(rowDefinition.CellScope, webElement);
                                            if (elScope != null)
                                            {
                                                var el = elScope.GetElementByQuery(rowDefinition.Cell);
                                                if (el != null)
                                                    rowData[rowDefinition.Key.ToString()] = el.GetStringValue();
                                            }
                                        }
                                        else
                                            rowData[rowDefinition.Key.ToString()] = rowDefinition.Cell;
                                    }
                                    dataTable.Rows.Add(rowData);
                                }
                            }
                    }
                }
            }
        }

        private WebElement FindScopeElement(string scope, WebElement selector)
        {
            var scopeElement = string.IsNullOrWhiteSpace(scope) ?
                                      selector :
                                      _htmlDocument.GetElementsByQuery(scope).FirstOrDefault();
            return scopeElement;
        }

        public DataSet GetDataSet()
        {
            return _dataSet;
        }
    }
}
