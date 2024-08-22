using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Nixill.Utils;

namespace Nixill.Collections.Grid.CSV;

/// <summary>
/// This class contains static methods to convert between DataTables and
/// comma separated value format files and text.
/// </summary>
public static class DataTableCSVParser
{
  public static DataTable FileToDataTable(string path, IDictionary<string, DataColumn> columnDefs = null,
    IDictionary<Type, Func<string, object>> deserializers = null, IEnumerable<string> primaryKey = null)
    => EnumerableToDataTable(FileUtils.FileCharEnumerator(path), columnDefs, deserializers, primaryKey);

  public static DataTable StreamToDataTable(StreamReader reader, IDictionary<string, DataColumn> columnDefs = null,
    IDictionary<Type, Func<string, object>> deserializers = null, IEnumerable<string> primaryKey = null)
    => EnumerableToDataTable(FileUtils.StreamCharEnumerator(reader), columnDefs, deserializers, primaryKey);

  public static DataTable EnumerableToDataTable(IEnumerable<char> input,
    IDictionary<string, DataColumn> columnDefs = null, IDictionary<Type, Func<string, object>> deserializers = null,
    IEnumerable<string> primaryKey = null)
  {
    DataTable table = new();

    columnDefs ??= new Dictionary<string, DataColumn>();
    deserializers ??= new Dictionary<Type, Func<string, object>>();

    var types = new List<Type>();

    IEnumerable<IList<string>> rows = CSVParser.EnumerableToRows(input);
    bool isHeaderRow = true;

    foreach (var row in rows)
    {
      if (isHeaderRow)
      {
        foreach (string item in row)
        {
          if (columnDefs.TryGetValue(item, out DataColumn col))
          {
            table.Columns.Add(col);
            types.Add(col.DataType);
          }
          else
          {
            table.Columns.Add(new DataColumn
            {
              ColumnName = item
            });
            types.Add(typeof(string));
          }
        }
      }
    }
  }
}