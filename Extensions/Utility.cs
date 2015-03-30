using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace GhostCrawler.Extensions
{
    public static class Utility
    {
        public static string GetValueOrDefault(this Dictionary<string, string> src, string key)
        {
            if (src.ContainsKey(key))
                return src[key];
            return null;
        }

        public static byte[] ToCSV(this DataTable dtDataTable)
        {
            var ms = new MemoryStream();

            var sw = new StreamWriter(ms);
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i].Caption ?? string.Empty);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
            ms.Dispose();
            return ms.ToArray();
        }
    }
}
